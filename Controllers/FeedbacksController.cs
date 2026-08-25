using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Text;

namespace Carbon_inventory_platform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FeedbacksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<FeedbacksController> _logger;

        public FeedbacksController(
            ApplicationDbContext context,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<FeedbacksController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        
        // GET: Feedbacks
        public async Task<IActionResult> Index()
        {
            return View(await _context.Feedbacks.AsNoTracking().OrderByDescending(f => f.CreatedAt).ToListAsync());
        }

        // GET: Feedbacks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // GET: Feedbacks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Feedbacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // 原本新增時也綁定了自動編號主鍵 Id（會觸發 IDENTITY_INSERT 錯誤），
        // 而回報時間也交給表單決定，改為在伺服器端戳記。
        public async Task<IActionResult> Create([Bind("UserName,Email,Message")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                feedback.CreatedAt = DateTime.Now;
                _context.Add(feedback);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(feedback);
        }

        // GET: Feedbacks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
            if (feedback == null)
            {
                return NotFound();
            }
            return View(feedback);
        }

        // POST: Feedbacks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserName,Email,Message,CreatedAt")] Feedback feedback)
        {
            if (id != feedback.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feedback);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedbackExists(feedback.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(feedback);
        }

        // GET: Feedbacks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // POST: Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeedbackExists(int id)
        {
            return _context.Feedbacks.Any(e => e.Id == id);
        }

        

        // 頁尾的「回報問題」表單在每一頁都會出現（包含未登入的首頁），所以維持匿名，
        // 但改為驗證輸入並回報結果；防偽 Token 由 Program.cs 的全域
        // AutoValidateAntiforgeryTokenAttribute 統一驗證。
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SubmitFeedback([Bind("UserName,Email,Message")] Feedback feedback)
        {
            if (!ModelState.IsValid)
            {
                TempData["FeedbackError"] = string.Join(" ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return RedirectToAction("Index", "Home");
            }

            feedback.CreatedAt = DateTime.Now;
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            // 在此處調用 LINE BOT 發送消息給開發者。通知失敗不應該讓使用者的回報失敗。
            await SendLineNotification(feedback);

            TempData["FeedbackSuccess"] = "感謝您的回報，我們已收到訊息。";
            return RedirectToAction("Index", "Home");
        }

        private async Task SendLineNotification(Feedback feedback)
        {
            // Channel Access Token 與收件者 Id 改由設定檔/環境變數提供。
            // 原本是硬編碼在原始碼中並已進入 git 歷史，該 Token 必須重新發行。
            var lineBotToken = _configuration["Line:ChannelAccessToken"];
            var notifyUserId = _configuration["Line:NotifyUserId"];
            if (string.IsNullOrWhiteSpace(lineBotToken) || string.IsNullOrWhiteSpace(notifyUserId))
            {
                _logger.LogInformation("未設定 Line:ChannelAccessToken / Line:NotifyUserId，略過 LINE 通知。");
                return;
            }

            string companyName = string.Empty;
            if (_signInManager.IsSignedIn(User))
            {
                var userId = _userManager.GetUserId(User);
                var company = await _context.Companies.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId);
                if (company != null)
                {
                    companyName = company.Name;
                }
            }

            var message = new
            {
                to = notifyUserId,
                messages = new[]
                {
                    new
                    {
                        type = "text",
                        text = $"新回報問題\n\n使用者名稱: {feedback.UserName}\n公司: {companyName}\n電子郵件: {feedback.Email}\n訊息: {feedback.Message}"
                    }
                }
            };

            try
            {
                // 用 IHttpClientFactory 取得共用的 HttpClient，
                // 原本每次呼叫都 new HttpClient() 會耗盡通訊埠 (socket exhaustion)。
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromSeconds(10);
                using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.line.me/v2/bot/message/push")
                {
                    Content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json")
                };
                request.Headers.Add("Authorization", $"Bearer {lineBotToken}");

                var response = await httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("LINE 通知失敗，狀態碼 {StatusCode}", (int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "LINE 通知發送失敗");
            }
        }
    }
}
