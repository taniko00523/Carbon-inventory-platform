using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Carbon_inventory_platform.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.Globalization;
using Xceed.Document.NET;

[Authorize(Roles = "Admin")]
public class UserController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserEmailStore<ApplicationUser> _emailStore;
    private readonly ILogger<UserController> _logger;
    private readonly IEmailSender _emailSender;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _context;

    // 新增/匯入的帳號一律給這個角色。原本 Add 給 "User" 而 BulkAdd 給 "Guest"，
    // 但導覽選單只認 "User"/"Admin"，會導致批次匯入的帳號登入後看不到任何功能。
    private const string DefaultRoleName = "User";
    private const string AdminRoleName = "Admin";

    public UserController(UserManager<ApplicationUser> userManager,
        IUserStore<ApplicationUser> userStore,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        ILogger<UserController> logger,
        IEmailSender emailSender,
        ApplicationDbContext context)
    {
        _context = context;
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _signInManager = signInManager;
        _logger = logger;
        _emailSender = emailSender;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        return View(await BuildUserListAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkAdd(IFormFile excelFile)
    {
        // 原本錯誤只寫進 ModelState 後就 RedirectToAction，訊息在轉向時會全部遺失；改用 TempData 帶回 Index 顯示。
        var errors = new List<string>();

        if (excelFile == null || excelFile.Length == 0)
        {
            TempData["UserErrors"] = "請上傳有效的 Excel 檔案。";
            return RedirectToAction(nameof(Index));
        }

        var usersToAdd = new List<CompanyUserViewModel>();

        try
        {
            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                stream.Position = 0;

                // 使用 NPOI 讀取 Excel 文件
                IWorkbook workbook = new XSSFWorkbook(stream);
                ISheet sheet = workbook.GetSheetAt(0);

                // 假設第一行是標題行，從第二行開始讀取
                for (int row = 1; row <= sheet.LastRowNum; row++)
                {
                    IRow currentRow = sheet.GetRow(row);
                    if (currentRow == null) continue;

                    // 原本整份匯入共用一個 try，而且使用期限是
                    // DateTime.ParseExact(cell?.ToString(), "yyyyMMdd", ...)：
                    // 空白儲存格、日期格式儲存格（ToString 為 "2025/12/31 00:00:00"）或數字格式儲存格
                    // 都會丟例外並中斷整份匯入。改為逐列讀取＋驗證，壞的那一列記錄錯誤後繼續處理其餘資料。
                    var account = GetCellText(currentRow.GetCell(0));   // 假設第一列是帳號
                    var password = GetCellText(currentRow.GetCell(1));  // 假設第二列是密碼
                    var limitData = TryReadDate(currentRow.GetCell(2)); // 假設第三列是使用期限

                    // 整列空白視為表格結尾的空行，直接跳過
                    if (string.IsNullOrWhiteSpace(account) && string.IsNullOrWhiteSpace(password) && limitData == null)
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(account))
                    {
                        errors.Add($"第 {row + 1} 列：帳號為空白，已略過。");
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(password))
                    {
                        errors.Add($"第 {row + 1} 列（{account}）：密碼為空白，已略過。");
                        continue;
                    }

                    if (limitData == null)
                    {
                        errors.Add($"第 {row + 1} 列（{account}）：使用期限無法辨識，請填 yyyyMMdd 或日期格式，已略過。");
                        continue;
                    }

                    usersToAdd.Add(new CompanyUserViewModel
                    {
                        ApplicationUser = new ApplicationUser
                        {
                            UserName = account,
                        },
                        Password = password,
                        UserLimitData = limitData
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "讀取批次新增用戶的 Excel 檔案失敗。");
            TempData["UserErrors"] = $"讀取 Excel 檔案失敗：{ex.Message}";
            return RedirectToAction(nameof(Index));
        }

        // 批量創建用戶
        int created = 0;
        foreach (var model in usersToAdd)
        {
            var account = model.ApplicationUser!.UserName!;

            // 原本沒有先檢查重複帳號，重複匯入只會拿到 Identity 的英文錯誤，而且訊息還會在轉向時遺失。
            if (await _userManager.FindByNameAsync(account) != null)
            {
                errors.Add($"帳號 {account} 已存在，已略過。");
                continue;
            }

            var user = CreateUser();
            await _userStore.SetUserNameAsync(user, account, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, account, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, model.Password!);

            if (result.Succeeded)
            {
                // 原本批次匯入給的是 "Guest"，與 Add 的 "User" 不一致，匯入的帳號登入後導覽列是空的。
                var role = await _roleManager.FindByNameAsync(DefaultRoleName);

                if (role != null)
                {
                    await _userManager.AddToRoleAsync(user, role.Name!);
                }
                else
                {
                    // 原本角色不存在時靜靜地建立一個沒有角色的帳號，使用者登入後完全不能用。
                    _logger.LogError("找不到角色 {Role}，帳號 {Account} 建立後沒有任何角色。", DefaultRoleName, account);
                    errors.Add($"帳號 {account} 已建立，但找不到角色 {DefaultRoleName}，請聯絡管理員補設定角色。");
                }
                user.UserLimitData = model.UserLimitData;
                await _userManager.UpdateAsync(user);
                await CreateCompanyAsync(user);
                created++;
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    errors.Add($"帳號 {account} 建立失敗：{error.Description}");
                }
            }
        }

        TempData["UserMessage"] = $"批次匯入完成，成功新增 {created} 筆。";
        if (errors.Count > 0)
        {
            TempData["UserErrors"] = string.Join("\n", errors);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> SearchByRole(string role)
    {
        // 原本沒有檢查角色是否存在，GetUsersInRoleAsync 對不存在的角色會丟 InvalidOperationException 變成 500。
        if (string.IsNullOrWhiteSpace(role) || !await _roleManager.RoleExistsAsync(role))
        {
            TempData["UserErrors"] = "查詢的角色不存在。";
            return RedirectToAction(nameof(Index));
        }

        // 根據角色過濾用戶
        var usersInRole = await _userManager.GetUsersInRoleAsync(role);

        // 原本 SearchByRole 沒有像 Index 一樣把管理員帳號濾掉，管理員會出現在結果裡並可被刪除。
        var adminUserIds = await GetAdminUserIdsAsync();

        // 構建對應的 ViewModel
        var viewModel = usersInRole
            .Where(user => !adminUserIds.Contains(user.Id))
            .Select(user => new CompanyUserViewModel
            {
                ApplicationUser = user,
                Company = _context.Companies.FirstOrDefault(x => x.UserId == user.Id)
            }).ToList();

        return View("Index", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(CompanyUserViewModel model)
    {
        // 原本直接取用 model.ApplicationUser.UserName / model.Password，
        // 少填欄位時會 NullReferenceException，或讓 CreateAsync 因為密碼為 null 丟例外變成 500。
        if (model?.ApplicationUser == null || string.IsNullOrWhiteSpace(model.ApplicationUser.UserName))
        {
            ModelState.AddModelError(string.Empty, "請填寫使用者名稱。");
            return View("Index", await BuildUserListAsync());
        }

        if (string.IsNullOrWhiteSpace(model.Password))
        {
            ModelState.AddModelError(string.Empty, "請填寫密碼。");
            return View("Index", await BuildUserListAsync());
        }

        // 原本沒有先檢查重複帳號，錯誤訊息只有 Identity 的英文描述。
        if (await _userManager.FindByNameAsync(model.ApplicationUser.UserName) != null)
        {
            ModelState.AddModelError(string.Empty, $"帳號 {model.ApplicationUser.UserName} 已存在。");
            return View("Index", await BuildUserListAsync());
        }

        var user = CreateUser();
        await _userStore.SetUserNameAsync(user, model.ApplicationUser.UserName, CancellationToken.None);
        // 原本用 model.ApplicationUser.Email，但新增用戶的表單沒有 Email 欄位，一定是 null，
        // 會導致清單的「初始註冊帳號」永遠空白；與批次匯入一致，改用帳號當 Email。
        await _emailStore.SetEmailAsync(user,
            string.IsNullOrWhiteSpace(model.ApplicationUser.Email) ? model.ApplicationUser.UserName : model.ApplicationUser.Email,
            CancellationToken.None);
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("User created a new account with password.");
            var role = await _roleManager.FindByNameAsync(DefaultRoleName);

            if (role != null)
            {
                await _userManager.AddToRoleAsync(user, role.Name!);
            }
            else
            {
                // 原本角色不存在時靜靜地建立一個沒有角色的帳號，使用者登入後完全不能用。
                _logger.LogError("找不到角色 {Role}，帳號 {Account} 建立後沒有任何角色。", DefaultRoleName, model.ApplicationUser.UserName);
                TempData["UserErrors"] = $"帳號 {model.ApplicationUser.UserName} 已建立，但找不到角色 {DefaultRoleName}，請聯絡管理員補設定角色。";
            }
            user.UserLimitData = model.UserLimitData;
            await _userManager.UpdateAsync(user);
            await CreateCompanyAsync(user);
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        // Return the Index view with a model that matches the expected type
        return View("Index", await BuildUserListAsync());
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CompanyUserViewModel model)
    {
        // 原本沒有檢查 model.ApplicationUser 是否為 null，直接取 .Id 會 NullReferenceException 變成 500。
        if (model?.ApplicationUser == null || string.IsNullOrWhiteSpace(model.ApplicationUser.Id))
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(model.ApplicationUser.Id);
        if (user != null)
        {
            // 原本只在畫面上隱藏管理員的編輯按鈕，直接 POST 仍然可以改管理員帳號；補上伺服器端檢查。
            if (await _userManager.IsInRoleAsync(user, AdminRoleName))
            {
                return Forbid();
            }

            if (user.UserName != model.ApplicationUser.UserName)
            {
                // 原本丟掉 SetUserNameAsync 的回傳值，帳號重複時其實失敗了，畫面卻回報成功。
                var setUserNameResult = await _userManager.SetUserNameAsync(user, model.ApplicationUser.UserName);
                if (!setUserNameResult.Succeeded)
                {
                    foreach (var error in setUserNameResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("Index", await BuildUserListAsync());
                }
            }

            if (!string.IsNullOrEmpty(model.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    // 原本是 return View(model)，但 Views/User 下只有 Index.cshtml，
                    // 而且它的型別是 IEnumerable<CompanyUserViewModel>，會直接 500。
                    return View("Index", await BuildUserListAsync());
                }
            }

            user.UserLimitData = model.ApplicationUser.UserLimitData;
            // 原本丟掉 UpdateAsync 的回傳值，更新失敗時仍然轉回清單並當作成功。
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("Index", await BuildUserListAsync());
            }
            return RedirectToAction(nameof(Index));
        }
        // Return the Index view with a model that matches the expected type
        return View("Index", await BuildUserListAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        // 原本 Delete 是 GET 且沒有防偽驗證，管理員只要瀏覽到一個
        // <img src="/User/Delete/xxx"> 的頁面，帳號就會被永久刪除。
        if (id == null)
        {
            return NotFound();
        }
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // 原本沒有任何保護，管理員可以刪掉自己或其他管理員帳號，系統會變成沒有人能管理。
        if (id == _userManager.GetUserId(User) || await _userManager.IsInRoleAsync(user, AdminRoleName))
        {
            TempData["UserErrors"] = "無法刪除管理員帳號或自己的帳號。";
            return RedirectToAction(nameof(Index));
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            // 原本丟掉 DeleteAsync 的回傳值，刪除失敗時畫面完全看不出來。
            TempData["UserErrors"] = string.Join("\n", result.Errors.Select(x => x.Description));
        }
        else
        {
            TempData["UserMessage"] = $"已刪除帳號 {user.UserName}。";
        }

        return RedirectToAction(nameof(Index));
    }

    // 原本 Index / Add / Edit 各自重複同一段組清單的程式碼，而且都用
    // x.ApplicationUser.Email == "Admin" 來隱藏管理員。Email/UserName 是使用者可以自行修改的欄位，
    // 一旦改掉，管理員帳號就會出現在清單裡並且可以被編輯或刪除；改為實際查 Admin 角色的成員 Id。
    private async Task<List<CompanyUserViewModel>> BuildUserListAsync()
    {
        var adminUserIds = await GetAdminUserIdsAsync();

        List<ApplicationUser> users = _userManager.Users.ToList();
        List<CompanyUserViewModel> viewModel = new List<CompanyUserViewModel>();

        foreach (var user in users)
        {
            if (adminUserIds.Contains(user.Id))
            {
                continue;
            }

            CompanyUserViewModel model = new CompanyUserViewModel
            {
                ApplicationUser = user,
                Company = _context.Companies.FirstOrDefault(x => x.UserId == user.Id)
            };
            viewModel.Add(model);
        }
        return viewModel;
    }

    private async Task<List<string>> GetAdminUserIdsAsync()
    {
        var admins = await _userManager.GetUsersInRoleAsync(AdminRoleName);
        var adminUserIds = admins.Select(x => x.Id).ToList();
        // 讓畫面也能用角色（而不是可被修改的 UserName）判斷要不要顯示編輯/刪除按鈕。
        ViewData["AdminUserIds"] = adminUserIds;
        return adminUserIds;
    }

    // 原本 cell?.ToString() 對數字/公式/日期格式儲存格會拿到非預期的字串（例如 1.23E+09、
    // "2025/12/31 00:00:00"），改為依儲存格型別讀取。
    private static string? GetCellText(ICell? cell)
    {
        if (cell == null)
        {
            return null;
        }

        try
        {
            switch (cell.CellType)
            {
                case CellType.String:
                    return cell.StringCellValue?.Trim();
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        return DateUtil.GetJavaDate(cell.NumericCellValue).ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                    }
                    return cell.NumericCellValue.ToString("0.##########", CultureInfo.InvariantCulture);
                case CellType.Boolean:
                    return cell.BooleanCellValue ? "TRUE" : "FALSE";
                case CellType.Formula:
                    return cell.CachedFormulaResultType == CellType.Numeric
                        ? cell.NumericCellValue.ToString("0.##########", CultureInfo.InvariantCulture)
                        : cell.StringCellValue?.Trim();
                default:
                    return null;
            }
        }
        catch
        {
            // 壞掉的儲存格一律視為空白，由呼叫端當成該列的驗證錯誤處理。
            return null;
        }
    }

    // 原本使用期限直接 DateTime.ParseExact(cell?.ToString(), "yyyyMMdd", ...)，
    // 空白儲存格會丟 ArgumentNullException、日期格式儲存格與數字格式儲存格會丟 FormatException，
    // 而且整份匯入都會被中斷。改為容錯讀取，讀不出來就回 null 讓呼叫端記錄該列錯誤。
    private static DateTime? TryReadDate(ICell? cell)
    {
        if (cell == null)
        {
            return null;
        }

        try
        {
            bool numeric = cell.CellType == CellType.Numeric
                || (cell.CellType == CellType.Formula && cell.CachedFormulaResultType == CellType.Numeric);

            if (numeric)
            {
                if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                {
                    return DateUtil.GetJavaDate(cell.NumericCellValue).Date;
                }

                // 例如 20251231 被存成純數字
                var value = cell.NumericCellValue;
                if (value >= 19000101 && value <= 99991231
                    && DateTime.TryParseExact(((long)value).ToString(CultureInfo.InvariantCulture), "yyyyMMdd",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out var fromNumber))
                {
                    return fromNumber.Date;
                }
                return null;
            }
        }
        catch
        {
            return null;
        }

        var text = GetCellText(cell);
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        string[] formats =
        {
            "yyyyMMdd", "yyyy/MM/dd", "yyyy-MM-dd", "yyyy/M/d", "yyyy-M-d",
            "yyyy/MM/dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss"
        };

        if (DateTime.TryParseExact(text.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            return parsed.Date;
        }
        if (DateTime.TryParse(text.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
        {
            return parsed.Date;
        }
        return null;
    }

    private ApplicationUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor.");
        }
    }

    // 原本這是 public，Controller 上的 public 方法就是可路由的 action，
    // 任何人打 /User/CreateCompanyAsync 都會多建一筆公司資料；標成 [NonAction]。
    [NonAction]
    public async Task CreateCompanyAsync(ApplicationUser user)
    {
        var userId = await _userManager.GetUserIdAsync(user);

        var Company_id = Guid.NewGuid();
        var Area_id = Guid.NewGuid();
        var Analysis_id = Guid.NewGuid();
        await _context.Companies.AddAsync(new Company()
        {
            Id = Company_id,
            UserId = userId,
            CreateTime = DateTime.Now
        });
        await _context.SaveChangesAsync();
        await _context.Areas.AddAsync(new Area()
        {
            Id = Area_id,
            CompanyId = Company_id,
            Year = DateTime.Now.Year - 1912, //減去1911取得民國年 再減去1盤去年
            ARVersion = 6,
            BaseYear = true,
            CreateTime = DateTime.Now
        });
        await _context.Analyses.AddAsync(new Analysis()
        {
            Id = Analysis_id,
            AreaId = Area_id,
            CreateTime = DateTime.Now
        });
        await _context.SaveChangesAsync();
    }

    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<ApplicationUser>)_userStore;
    }
}
