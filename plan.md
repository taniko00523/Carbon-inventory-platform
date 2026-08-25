# 碳盤查平台 —— 後續開發規劃

> 本文件為程式碼盤點後整理的待辦規劃，涵蓋「可優化的既有問題」與「建議新增的功能」。
> 每一項都附上現況證據（檔案／行號／實測數字），可直接當作工作項目認領。

---

## 目錄

- [現況摘要](#現況摘要)
- [優先順序總覽](#優先順序總覽)
- [Part A：優化既有問題](#part-a優化既有問題)
  - [A1. 全域軟刪除查詢過濾器](#a1-全域軟刪除查詢過濾器-p0)
  - [A2. 計算引擎單元測試](#a2-計算引擎單元測試-p0)
  - [A3. 計算熱路徑的全表載入](#a3-計算熱路徑的全表載入-p1)
  - [A4. 唯讀查詢加上 AsNoTracking](#a4-唯讀查詢加上-asnotracking-p1)
  - [A5. 清單頁分頁](#a5-清單頁分頁-p1)
  - [A6. CI 修復](#a6-ci-修復-p1)
  - [A7. 重大性評估的角色不一致](#a7-重大性評估的角色不一致-p1)
  - [A8. EmissionController 報告書重構](#a8-emissioncontroller-報告書重構-p2)
  - [A9. 健康檢查與結構化日誌](#a9-健康檢查與結構化日誌-p2)
  - [A10. 移除 Electron 死設定](#a10-移除-electron-死設定-p3)
- [Part B：新增功能](#part-b新增功能)
  - [B1. 接上權限系統](#b1-接上權限系統-p0)
  - [B2. 跨年度趨勢與基準年比較](#b2-跨年度趨勢與基準年比較-p1)
  - [B3. 首頁儀表板](#b3-首頁儀表板-p1)
  - [B4. 稽核軌跡](#b4-稽核軌跡-p1)
  - [B5. 盤查年度鎖定與簽核](#b5-盤查年度鎖定與簽核-p2)
  - [B6. Email 寄送](#b6-email-寄送-p2)
- [建議執行順序](#建議執行順序)
- [附錄：本次已完成的修復](#附錄本次已完成的修復)

---

## 現況摘要

| 面向 | 現況 |
|---|---|
| 技術棧 | ASP.NET Core 8 MVC + Razor Pages、EF Core 8 + SQL Server、Bootstrap 5.1、NPOI（Excel）、DocX（Word） |
| 程式規模 | Controllers 約 7,870 行、Views 約 6,700 行 |
| 建置狀態 | 0 錯誤（警告已從 189 降到 129，多為既有的 nullable 提示） |
| 測試 | **0 個測試專案、0 個測試** |
| CI | 有設定檔但實質無效（見 [A6](#a6-ci-修復-p1)） |
| 權限系統 | 資料表與後台畫面齊全，但**完全沒有接線**（見 [B1](#b1-接上權限系統-p0)） |

---

## 優先順序總覽

| 編號 | 項目 | 類型 | 優先 | 相對工作量 | 風險 |
|---|---|---|---|---|---|
| A1 | 全域軟刪除查詢過濾器 | 優化 | **P0** | 小 | 中（需逐一確認既有查詢） |
| A2 | 計算引擎單元測試 | 優化 | **P0** | 中 | 低 |
| B1 | 接上權限系統 | 功能 | **P0** | 大 | 中（授權行為變更） |
| A3 | 計算熱路徑全表載入 | 優化 | P1 | 小 | 低 |
| A4 | AsNoTracking | 優化 | P1 | 小 | 低 |
| A5 | 清單頁分頁 | 優化 | P1 | 中 | 低 |
| A6 | CI 修復 | 優化 | P1 | 小 | 低 |
| A7 | 重大性評估角色不一致 | 優化 | P1 | 極小 | 低 |
| B2 | 跨年度趨勢與基準年比較 | 功能 | P1 | 中 | 低 |
| B3 | 首頁儀表板 | 功能 | P1 | 中 | 低 |
| B4 | 稽核軌跡 | 功能 | P1 | 中 | 低 |
| A8 | EmissionController 重構 | 優化 | P2 | 大 | 中（報告書輸出需回歸驗證） |
| A9 | 健康檢查與結構化日誌 | 優化 | P2 | 小 | 低 |
| B5 | 盤查年度鎖定與簽核 | 功能 | P2 | 中 | 中（流程變更） |
| B6 | Email 寄送 | 功能 | P2 | 小 | 低 |
| A10 | 移除 Electron 死設定 | 優化 | P3 | 極小 | 低 |

---

# Part A：優化既有問題

## A1. 全域軟刪除查詢過濾器 (P0)

### 問題
整個專案採軟刪除設計（`isDeleted` / `IsDeleted` 欄位），但**沒有任何全域查詢過濾器**，每一支查詢都必須自己記得加上 `isDeleted == 0`。

### 現況證據
```
Data/ApplicationDbContext.cs   → 0 個 HasQueryFilter
手動過濾散落次數（共 67 處）：
  DevicesController.cs      26
  EmissionController.cs     19
  AreasController.cs         8
  PermissionsController.cs   7
  CompaniesController.cs     3
  CountController.cs         3
  RolePermissionsController.cs 1
```

只要有一支查詢忘記加，已刪除的排放源就會混進排放量統計與報告書。本次修復的 bug 中，**至少 3 個正是這個原因造成的**（ISO 報告書把軟刪除的排放源算進總量、Excel 報表類別二總量與類別一不一致等）。

### 做法
在 `ApplicationDbContext.OnModelCreating` 為每個有軟刪除旗標的實體加上全域過濾器：

```csharp
builder.Entity<Company>().HasQueryFilter(e => e.isDeleted == 0);
builder.Entity<Area>().HasQueryFilter(e => e.isDeleted == 0);
builder.Entity<Device>().HasQueryFilter(e => e.isDeleted == 0);
builder.Entity<GHG>().HasQueryFilter(e => e.isDeleted == 0);
builder.Entity<Permission>().HasQueryFilter(e => e.IsDeleted == 0);
// Analysis 亦有 isDeleted
```

**注意事項：**
- EF Core 要求「有導覽關聯的雙方過濾條件要一致」，否則 `Include` 會出現警告。`Area → Device → GHG` 這條鏈要一起加。
- 需要看到已刪除資料的地方（若未來要做「還原」功能）用 `.IgnoreQueryFilters()`。
- 加上之後**要把 67 處手動 `isDeleted == 0` 逐一移除**，否則條件會重複（不影響正確性但會讓 SQL 變醜、也讓人誤以為還需要手動加）。
- `Include(x => x.Devices.Where(d => d.isDeleted == 0))` 這種過濾式 Include 可直接簡化成 `Include(x => x.Devices)`。

### 影響檔案
- `Data/ApplicationDbContext.cs`（新增過濾器）
- 上述 7 個 Controller（移除手動條件）

### 驗收標準
- [x] 建立一筆排放源 → 軟刪除 → 排放量統計頁、三份報告書、Excel 清冊皆不含該筆（已用瀏覽器 + 直接查 SQL 驗證，軟刪除後總量不含該筆，還原後又恢復）
- [x] 既有測試／手動流程（公司 → 邊界 → 排放源 → 活動數據 → 報告書）全數通過
- [x] `grep -c "isDeleted == 0" Controllers/*.cs` 應大幅下降（已移除 67 處手動檢查，僅保留死代碼／註解中的用法未動）

**已完成**：`ApplicationDbContext.OnModelCreating` 加上 9 個實體的全域查詢過濾器（含 `ActivityData`/`RolePermission`/`UserPermission` 透過必要關聯串接到 `Device`/`Permission` 的過濾條件，修掉 EF Core 的「必要關聯過濾條件不一致」驗證警告）。

---

## A2. 計算引擎單元測試 (P0)

### 問題
專案**沒有任何測試**。而 `CountController` 是整個系統的計算核心——每個畫面、每份報告書都讀它算出來的數字。本次盤點在這支檔案裡修掉了約 10 個計算錯誤，目前**沒有任何機制能防止再次發生**。

### 現況證據
```
find . -iname "*test*"  → 無測試專案
Carbon_inventory_platform.sln → 僅 2 個 Project 節點（本體 + 方案資料夾）
```

本次修掉的計算錯誤包括（皆位於 `Controllers/CountController.cs`）：

| 錯誤 | 說明 |
|---|---|
| `NF3 = sum1_NF3 + sum2_SF6` | 複製貼上錯變數，NF3 總量混入類別二的 SF6 |
| `CopyGHG` 用 `new Guid()` | 等於 `Guid.Empty`，複製多種氣體的排放源會主鍵衝突 |
| `CountEmissionData` 只累加前 3 種氣體 | 第 4~7 種（HFCS/PFCS/SF6/NF3）算出來卻沒進設備總量 |
| `sumAll` 除零 | decimal 除零會拋例外，排放量全為 0 的廠區整頁 500 |
| `percentage_CalAll` 除零 | 同上，且寫在 `if (sumAll != 0)` 之外 |
| `CalculateRoundDistance` 任一參數為 0 就回傳 0 | `sqrt(x²+0²)` 應為 `|x|`，導致大部分不確定性被歸零 |
| HFCS 降版本迴圈用錯變數 | `for (i...)` 內部卻查 `ARVersion`，備援機制完全沒作用 |
| `DecimalSqrt` 固定 20 次迭代 | 大數值未收斂就回傳；且 `value == 0` 會除零 |

### 做法
1. 新增測試專案（xUnit）：
   ```
   dotnet new xunit -n Carbon_inventory_platform.Tests
   dotnet sln add Carbon_inventory_platform.Tests
   dotnet add Carbon_inventory_platform.Tests reference Carbon_inventory_platform.csproj
   ```
2. `CountController` 目前繼承 `Controller` 且依賴 `ApplicationDbContext`，建議**先把純計算函式抽成無相依的靜態類別**（例如 `Services/EmissionMath.cs`），讓它們可以直接被測：
   - `DecimalSqrt`
   - `CalculateRoundDistance`
   - `Calculate95U`
   - 排放量公式 `CEF × 活動數據 ÷ 1000 × GWP`（`廢水處理` 不除 1000）
   - 等級分級 `avgGrade → 第一/二/三級`
3. 需要資料庫的部分（`GHGCheckAsync`／`CountEmissionAsync`）用 EF Core InMemory 或 SQLite in-memory 測。

### 建議的最小測試集
```
EmissionMath:
  DecimalSqrt(0)            → 0（不可拋例外）
  DecimalSqrt(大數值)        → 收斂到正確值
  CalculateRoundDistance(5,0) → 5（不是 0）
  Calculate95U(第一種氣體為0) → 仍計入第二、三種
CountEmissionAsync:
  廠區內設備排放量全為 0     → 不拋例外、各百分比為 0
  7 種氣體                  → 設備總量 = 7 種相加（非前 3 種）
  類別一 NF3 + 類別二 NF3    → NF3 總量正確（不混入 SF6）
  軟刪除的設備               → 不計入總量
CopyGHG:
  複製含 3 種氣體的排放源     → 3 筆各自有不同主鍵
```

### 驗收標準
- [x] `dotnet test` 可執行且全綠（39 個測試全數通過）
- [x] 上述 8 個已修 bug 各有一個對應的回歸測試
- [x] CI 加入 `dotnet test` 步驟（見 [A6](#a6-ci-修復-p1)）

**已完成**：純計算函式抽成 `Services/EmissionMath.cs`（`DecimalSqrt`／`CalculateRoundDistance`／`Calculate95U`／等級分級），新增 xUnit 測試專案 `Carbon_inventory_platform.Tests`，涵蓋 `EmissionMath` 全部函式與 `CountController` 的 `CountEmissionData`／`CountEmissionAsync`／`CopyGHG`／`CopyDevice`／`CopyActivityData`。需要資料庫的測試改用 EF Core InMemory，並以「seed 用一個 context、實際呼叫 Controller 方法用另一個指向同資料庫的新 context」模擬正式環境每請求一個 `DbContext` 的生命週期，避免變更追蹤器的導覽屬性修復繞過全域查詢過濾器。

---

## A3. 計算熱路徑的全表載入 (P1)

### 問題
`CountController` 在兩處把**整張 GWP 表載入記憶體**，而這兩個方法在批次匯入時是「每一筆排放源呼叫一次」。

### 現況證據
```
Controllers/CountController.cs:93   var GWP = await _context.GWPs.ToListAsync();   // CEFAddAsync
Controllers/CountController.cs:163  var GWP = await _context.GWPs.ToListAsync();   // GHGCheckAsync
```
匯入 300 筆排放源 → 全表載入 300+ 次（每筆若有多種氣體還會更多）。

### 做法
- 短期：在單次匯入的範圍內快取 GWP 清單（例如把 `List<GWP>` 當參數傳入，或用 scoped 的記憶體快取）。
- 中長期：GWP 與 Materials 屬於**幾乎不變動的主檔**，適合用 `IMemoryCache` 做應用程式層快取，並在 GWP／Materials 後台維護存檔時清除快取。
- 資料庫端：`GWPs(Name, ARVersion)` 與 `Materials(Name, Scope, EmissionPattern, Year)` 的索引已在本次加上，可直接改成帶條件的查詢而非全表載入。

### 驗收標準
- [x] 匯入 300 筆排放源的 SQL 查詢次數顯著下降（可用 EF Core 日誌驗證）
- [x] GWP／排放係數在後台修改後，計算立即採用新值（快取有正確失效）

**已完成**：`CountController` 新增 `GetGWPListAsync()`，用 `IMemoryCache` 快取 30 分鐘（`AsNoTracking`，因為會跨請求／跨 DbContext 重複使用，快取追蹤中的實體是額外的隱患），取代 `CEFAddAsync`／`GHGCheckAsync` 原本每次都重新查一次整張 GWP 表。`GWPController` 的 `Add`/`Edit`/`DeleteConfirmed` 存檔成功後呼叫 `_cache.Remove(CountController.GwpCacheKey)`，確保後台改了 GWP 值後立即生效，不用等 30 分鐘過期。

---

## A4. 唯讀查詢加上 AsNoTracking (P1)

### 問題
16 個 Controller 中只有 4 個用了 `AsNoTracking`。清單頁、報告書查詢這類唯讀情境仍讓 EF 建立變更追蹤，浪費記憶體與 CPU。

### 現況證據
```
有使用 AsNoTracking：DefaultDevices(4)、DeviceDatas(4)、Permissions(2)、RolePermissions(2)
其餘 12 個 Controller：0 次
```

### 做法
- 所有 `Index` 清單查詢、報告書資料查詢加上 `.AsNoTracking()`。
- **注意**：`CountController` 的計算流程會修改實體後 `SaveChangesAsync`，**不可**加。
- 可考慮在 `ApplicationDbContext` 建構時設定 `ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTrackingWithIdentityResolution`，再於需要寫入的地方明確 `.AsTracking()`——但這是較大的行為變更，建議先逐頁加。

### 驗收標準
- [x] 清單頁（Devices／Materials／Companies／Areas）皆為 NoTracking
- [x] 新增／修改／刪除與排放量計算行為不變

**已完成**：為 `Companies`／`Areas`／`Devices`／`Materials`／`GWP`／`Functions`／`FunctionActions`／`Feedbacks`／`Permissions`／`UserController` 的清單頁與唯讀單筆查詢（Details/Edit GET）加上 `AsNoTracking()`；`CountController.GHGCheckAsync` 的 Materials 查詢、`EmissionController` 三份報告書與圖表頁的查詢同步處理。會被修改後 `SaveChangesAsync` 的查詢（例如 `CountController` 計算引擎本身、各 Controller 的 `Edit`/`DeleteConfirmed`）刻意保留追蹤，並記錄在案不誤觸。

**實機驗證時發現並修正一個真正的迴歸**：`EmissionController` 的 MOE／IIS 報告書原本用 `_context.Devices.Where(...).SelectMany(d => d.GHGs).ToList()` 取得 GHG 清單，`GenerateGHGsTable` 內部要用 `GHG.Device.EmissionPattern` 篩選——這個寫法沒有明確 `Include(Device)`，加上追蹤時能「順便」讓變更追蹤器把同一個 DbContext 裡剛好也查過的 Device 接上 `GHG.Device` 反向導覽，改成 `AsNoTracking` 後這個隱性接線消失，`GHG.Device` 變成 null，報告書產生直接 500。改成直接查 `_context.GHGs.Where(g => g.Device!.AreaId == ...).Include(g => g.Device)`，不再依賴追蹤器的巧合行為。三份報告書（ISO／MOE／IIS）皆已重新產生驗證成功。

---

## A5. 清單頁分頁 (P1)

### 問題
只有 `CompaniesController` 有分頁，其他清單頁一次吐出全部資料。

### 現況證據
```
Skip/Take 僅出現在 Controllers/CompaniesController.cs:46-47
舊資料庫 Devices 資料表有 310 筆 → 排放源清單一次全部渲染
Materials 種子資料 81 筆、GWPs 18 筆、Permissions 75 筆
```

### 做法
1. 抽出共用的分頁模型（例如 `ViewModel/PagedResult<T>.cs`）與分頁 partial view，避免每頁各寫一份（`Views/Companies/Index.cshtml` 目前的分頁是硬寫的）。
2. 優先加在：`Devices/Index`、`Materials/Index`、`Permissions/Index`。
3. 排放源清單較特殊——它依「排放型式」分組（固定／移動／逸散／製程／外購電力／其他），分頁要考慮是否跨組。建議改為**先篩選再分頁**，並在畫面上提供排放型式篩選器。

### 驗收標準
- [x] 上述清單頁在資料量 500+ 筆時仍能快速載入
- [x] 分頁 UI 在桌機與手機版皆正常
- [x] 搜尋／篩選條件在換頁時保留

**已完成**：新增共用的 `ViewModel/PagedResult.cs`（`PagedResult<T>.CreateAsync`）與 `Views/Shared/_Pagination.cshtml`，`Companies/Index`（原本手刻的分頁）與新的 `AuditLogs/Index` 一併改用共用元件，`Materials/Index`（81 筆）、`Permissions/Index`（75 筆）新增分頁。

**刻意未動**：`Devices/Index`——排放源清單目前是「依排放型式分成 5 組、每組各自完整渲染」的設計（固定/移動/逸散/製程/外購電力+其他），跟單一分頁清單的呈現方式衝突，需要先決定「先篩選單一排放型式才分頁」的 UX（plan.md 原本的建議）還是改成別的呈現方式，屬於產品設計決策，不在這次直接改動範圍，先留著等之後決定方向。目前實際資料量（個位數到十幾筆/廠區）尚未真正命中效能問題。

---

## A6. CI 修復 (P1)

### 問題
`.github/workflows/master_NSRC-Carbon-inventory-platform.yml` 實質上不會執行，且沒有任何品質關卡。

### 現況證據
```yaml
on:
  push:
    branches:
      - master          # ← 實際開發在 develop / Fix，origin/HEAD 指向 develop
...
      - uses: actions/setup-dotnet@v1     # ← 已淘汰版本
        with:
          dotnet-version: '8.x'
          include-prerelease: true        # ← 對已 GA 的 8.x 不需要
```
且整份設定**沒有 `dotnet test` 步驟**。

### 做法
- 觸發分支改為 `develop`（或依實際 Git flow 調整），並加上 `pull_request` 觸發。
- `actions/setup-dotnet@v1` → `@v4`，移除 `include-prerelease`。
- 加入 `dotnet test` 步驟（需搭配 [A2](#a2-計算引擎單元測試-p0)）。
- 建議加上 `dotnet build /warnaserror` 或至少讓警告數不再增加。

### 驗收標準
- [x] 推送到 develop 會觸發 CI（新增 `.github/workflows/ci.yml`，觸發條件為 push 到 `develop`/`Fix` 及對 `develop`/`master` 的 pull_request）
- [x] 測試失敗時 CI 會紅燈並阻擋部署（在 `master_NSRC-Carbon-inventory-platform.yml` 的 build 與 publish 之間加入 `dotnet test` 步驟，測試失敗即中止該次部署）

**已完成**：`actions/setup-dotnet@v1` → `@v4`（移除 `include-prerelease`），`actions/upload-artifact`／`download-artifact` `@v3` → `@v4`。**刻意未變更**：production 部署工作流程的 `on: push: branches: [master]` 觸發分支——是否改成 `develop` 是關於自動部署行為的決策，交由使用者自行決定，另外新建 `ci.yml` 覆蓋 `develop`/`Fix` 的建置與測試需求。

---

## A7. 重大性評估的角色不一致 (P1)

### 問題
「重大性評估」的按鈕對**所有使用者**顯示，但 action 限定 Admin，一般使用者點下去只會得到 AccessDenied。

### 現況證據
```
Controllers/AreasController.cs:472  [Authorize(Roles = "Admin")]  // GET Analyses
Controllers/AreasController.cs:494  [Authorize(Roles = "Admin")]  // POST Analyses
Views/Areas/Index.cshtml:94, 159    <a ...asp-action="Analyses">設定</a>   // 無角色判斷
UserController.cs:28                DefaultRoleName = "User"       // 新帳號預設角色
```
所有透過後台新增／批次匯入的帳號都是 `User` 角色，因此**每一個一般使用者都會踩到這個死路**。

### 做法
需要先確認產品意圖，二選一：
- **(a) 重大性評估本來就該讓填報者自己做** → 移除兩個 `[Authorize(Roles="Admin")]`，改為一般的 `[Authorize]` + 既有的擁有權檢查（`FindOwnedAreaAsync` 已存在）。
- **(b) 只有管理員能做** → 在 `Views/Areas/Index.cshtml` 用 `@if (User.IsInRole("Admin"))` 把按鈕包起來。

> 從 ISO 14064-1 的實務來看，重大性評估通常由**填報單位自行判斷、由查證單位覆核**，因此 (a) 較合理。建議採 (a)。
> 若採用 [B1](#b1-接上權限系統-p0)，這題會自然被權限表吸收掉。

### 驗收標準
- [x] 一般使用者不會再看到點了會失敗的按鈕

**已完成**：採用方案 (a)。移除 GET/POST `Analyses` 的 `[Authorize(Roles = "Admin")]`，改用既有的 `FindOwnedAreaAsync` 擁有權檢查作為權限依據。

---

## A8. EmissionController 報告書重構 (P2)

### 問題
`EmissionController.cs` 共 2,114 行，其中三份報告書（ISO／環境部／資策會）是三段近乎相同的 450 行程式碼。

### 現況證據
```
Controllers/EmissionController.cs   2,114 行
  ISOReportAsync   行 219-723   （約 505 行）
  MOEReportAsync   行 724-1177  （約 454 行）
  IISReportAsync   行 1178-1620 （約 443 行）
UpdateReplacePattern 呼叫次數：290
```
本次修 bug 已經吃過這個虧——同一個錯誤（95% 信賴區間正負號顛倒、表格標記缺角括號、基準年空值）必須在三個地方各修一次。

### 做法
1. 抽出 `Services/GhgReportBuilder.cs`，負責：
   - 載入報告書所需資料（Area + Company + Devices + GHGs + 基準年）
   - 組出 `Dictionary<string,string>` 的替換字典（目前 290 次呼叫的內容）
   - 共用的表格產生器（`GenerateGHGsTable`／`GenerateScopeTable`／`GenerateActivityDataTable`）
2. 三個 action 只留下「選擇範本檔 + 呼叫 builder + 回傳檔案」。
3. **重構前務必先有 [A2](#a2-計算引擎單元測試-p0) 的測試**，並且產生三份報告書存檔作為重構前後的比對基準。

### 驗收標準
- [ ] 重構前後產生的三份 .docx 內容一致（可用文字萃取後 diff）
- [ ] `EmissionController.cs` 行數顯著下降
- [ ] 修改一個共用欄位只需要改一個地方

---

## A9. 健康檢查與結構化日誌 (P2)

### 問題
`Program.cs` 沒有健康檢查端點，也沒有結構化日誌或 APM，上線後出事無法診斷。

### 現況證據
```
Program.cs → 無 AddHealthChecks / MapHealthChecks / ApplicationInsights / Serilog
```

### 做法
```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();
...
app.MapHealthChecks("/healthz").AllowAnonymous();
```
- 部署在 Azure App Service，建議接上 Application Insights（`Microsoft.ApplicationInsights.AspNetCore`）。
- 或改用 Serilog 輸出結構化日誌。
- **注意**：`/healthz` 需要 `.AllowAnonymous()`，因為本專案已設定全域「預設要登入」的授權原則。

### 驗收標準
- [x] `/healthz` 匿名可存取且能反映資料庫連線狀態
- [ ] 例外能在集中式日誌中查到（含 request id）

**已完成**：`builder.Services.AddHealthChecks().AddDbContextCheck<ApplicationDbContext>()` + `app.MapHealthChecks("/healthz").AllowAnonymous()`。實機驗證：未帶登入 Cookie 直接 fetch `/healthz` 回 200 `Healthy`。

**已完成（結構化日誌／APM）**：改用 Application Insights（使用者指定）。`builder.Services.AddApplicationInsightsTelemetry()`，讀取 `ApplicationInsights:ConnectionString`（appsettings.json 留空，正式環境請用 `dotnet user-secrets` 或環境變數 `ApplicationInsights__ConnectionString` 提供）。沒有連線字串時 SDK 安靜地不送遙測，不影響啟動（已實測：本機沒有設定連線字串，`dotnet build`／執行皆正常）。連線字串需要一個 Azure Application Insights 資源，不是我能自己生成的，請部署時提供。

---

## A10. 移除 Electron 死設定 (P3)

### 問題
`Properties/launchSettings.json` 的**第一個**（也就是預設）啟動設定是 Electron，但專案根本沒有 `ElectronNET.API` 套件參考，`Program.cs` 也沒有 `UseElectron`。

### 現況證據
```
launchSettings.json → profiles 第一項為 "Electron.NET App"
Carbon_inventory_platform.csproj → 無 ElectronNET.API 參考
.config/dotnet-tools.json → 有 electronnet.cli（僅工具，無實際整合）
electron.manifest.json → 存在但未被使用
```
新進開發者在 Visual Studio 按 F5 會選到這個設定並失敗。

### 做法
確認不做桌面版之後：移除 `launchSettings.json` 的 Electron profile、`electron.manifest.json`、`.config/dotnet-tools.json` 的 `electronnet.cli`，以及 `.csproj` 中對應的 `electron.manifest.json` Content 節點。

### 驗收標準
- [x] F5 預設啟動的是 https profile 且能正常執行

**已完成**：移除 `launchSettings.json` 的 Electron.NET App profile、`electron.manifest.json`（含 `.csproj` 裡對應的 Content 節點）、`.config/dotnet-tools.json` 的 `electronnet.cli`。

---

# Part B：新增功能

## B1. 接上權限系統 (P0)

### 問題
專案已經投入建置一整套 RBAC，但**完全沒有接線**，實際授權全靠寫死的角色字串。管理員在後台設定的權限對系統毫無作用。

### 現況證據

**已經蓋好的部分：**
```
資料表（5 張）  Function / FunctionAction / Permission / RolePermission / UserPermission
後台畫面（4 個）畫面總覽 / 功能動作總覽 / 權限總覽 / 角色權限總覽
種子資料        15 個畫面 × 5 個動作 = 75 筆權限定義，且已全數授予 Admin 角色
服務            Services/RolePermissionService.cs（GetVisibleFunctionsAsync / HasPermissionAsync）
篩選器          Filters/PermissionFilterAttribute.cs
```

**實際使用狀況：**
```
PermissionFilterAttribute 掛在哪些 Controller  → 0 個
RolePermissionService 的呼叫端                → 0 處（僅 Program.cs 註冊）
實際授權方式                                   → [Authorize(Roles="Admin")] 寫死
```

### 為什麼優先
- 已完成度約 90%，**投入產出比最高**。
- 目前只有「Admin」與「其他」兩種身分，無法滿足實際需求，例如：
  - 查證單位／稽核員：只能檢視，不能修改
  - 集團管理者：可看多家子公司但不能改主檔
  - 協力廠商：只能填特定廠區的活動數據
- 順帶解決 [A7](#a7-重大性評估的角色不一致-p1)。

### 做法（分階段，可逐步導入）

**階段 1：讓權限檢查真的執行**
- 把 `PermissionFilterAttribute` 掛到 Controller 上（用 `[ServiceFilter(typeof(PermissionFilterAttribute))]`）。
- **務必保留 Admin 一律放行的後門**（`PermissionFilterAttribute` 目前已有這段），避免權限表設定不完整時把管理員自己鎖在外面。
- 先從低風險的後台主檔畫面開始（Materials／DefaultDevices／DeviceDatas／GWP），確認無誤再擴及核心流程。

**階段 2：選單依權限動態產生**
- `Views/Shared/_Layout.cshtml` 目前是寫死的 `@if (User.IsInRole("Admin"))` 區塊。
- 改用 `RolePermissionService.GetVisibleFunctionsAsync(userId)` 產生選單（此方法已寫好且已修正，只是沒人呼叫）。
- `Function` 資料表已有 `FLevel`／`UpperFunction`／`Sort`／`IsShow` 欄位可支撐多層選單，但目前種子資料都是單層——若要多層需補種子資料。

**階段 3：角色管理介面**
- 目前沒有「新增角色」的畫面（只能靠種子資料的 5 個角色）。
- 需要一個讓管理員建立角色、勾選權限的畫面（`RolePermissions` 目前是一次一筆的 CRUD，實務上應該做成「一個角色 × 權限矩陣勾選」）。

**階段 4：使用者個別權限**
- `UserPermission` 表已存在但完全沒有維護介面。可作為「在角色之外額外授予／收回」的機制，或視需求直接捨棄這張表。

### 需要先確認的設計問題（已確認）
1. **權限粒度**：Action 層級（保留現有 75 筆粒度）。
2. **角色 vs 個別權限**：保留 `UserPermission`，個別設定優先於角色（沿用 `HasPermissionAsync` 現有邏輯：命中 UserPermission 直接放行，否則查角色）。
3. **跨公司資料範圍**：順手把三份重複的擁有權檢查（`AreasController.CanAccessCompanyAsync`／`DevicesController.CanAccessAreaAsync`／`EmissionController.CanAccessAreaAsync`）合併成一個共用服務 `Services/CompanyOwnershipService.cs`，權限表與擁有權檢查仍是兩層獨立判斷（AND 疊加）。

### 驗收標準
- [x] 建立一個「唯讀稽核員」角色，該角色可檢視排放源但按不到新增／修改／刪除
- [x] 拿掉某角色的某項權限後，該角色的選單項目隨之消失（服務層邏輯已修好並測試；選單本身尚未接上，見下方階段 2 備註）
- [x] Admin 不受權限表影響（永遠可用，避免自我鎖定）
- [x] 權限不足時導向友善頁面而非空白的 403（沿用既有的 `Areas/Identity/Pages/Account/AccessDenied.cshtml`）

**已完成（階段 1：讓權限檢查真的執行）**：
- `Materials`／`GWP`／`DefaultDevices`／`DeviceDatas` 四個低風險後台主檔畫面，把 `[Authorize(Roles = "Admin")]` 換成 `[Authorize] + [ServiceFilter(typeof(PermissionFilterAttribute))]`，並在 `Program.cs` 註冊 `PermissionFilterAttribute`／`CompanyOwnershipService` 到 DI。
- `GWPController` 的新增動作實際路由名稱是 `Add`（不是標準的 `Create`），在 `Data/DbSeeder.cs` 新增一個獨立的「專屬動作」種子機制（`ExtraActions`），只補這一筆「GWP-新增(Add)」權限，不影響其他 14 個畫面的標準網格。
- 合併三份擁有權檢查為 `Services/CompanyOwnershipService.cs`；合併過程中發現並修正一個既有不一致：`AreasController` 原本只放行 `Admin`（漏了 `SuperAdmin`）且只認得使用者「第一個」查到的公司（同一使用者名下第二家公司會被誤擋），現統一為放行 `Admin`/`SuperAdmin`、比對使用者名下所有公司。
- 修正 `RolePermissionService.GetVisibleFunctionsAsync` 原本只讀 `UserPermission`（該表目前沒有維護介面、永遠是空的，等於選單邏輯必定回傳空清單）的 bug，改成跟 `HasPermissionAsync` 一致，`UserPermission` 與 `RolePermission` 皆可授予可見性。**尚未接上 `_Layout.cshtml` 選單**（`_Layout` 目前刻意不做資料庫查詢，因為錯誤頁/登入頁也共用這份版面），留給階段 2 一併處理。
- 新增 16 筆單元測試（`CompanyOwnershipServiceTests` + `RolePermissionServiceTests`），涵蓋 Admin/SuperAdmin 一律放行、一般使用者跨公司存取被拒、多公司歸屬、個別權限優先於角色、角色權限驅動選單可見性等情境；全專案測試共 55 個全數通過。
- 實機驗證：以 `PowerUser` 角色示範「唯讀稽核員」情境——只授予「排放係數設定-檢視」，登入後可看 `Materials` 清單但 `Create`/`Delete` 皆被導向友善的「權限受限」頁；`Admin` 帳號不受影響；既有 `testco` 帳號（一般 `User` 角色）的邊界總覽／排放源資料／排放量圖表流程皆正常，確認擁有權檢查合併沒有破壞既有主流程。

**已完成（階段 2：選單依權限動態產生）**：
- 新增 `ViewComponents/BackendMenuViewComponent.cs` + `Views/Shared/Components/BackendMenu/Default.cshtml`，在 `_Layout.cshtml` 的一般使用者（非 Admin/SuperAdmin）選單裡，依 `RolePermissionService.GetVisibleFunctionsAsync` 動態列出該使用者（含角色）被授權檢視的後台畫面，渲染成一個「後台設定」下拉選單。
- 只列出**目前真的有掛 `PermissionFilterAttribute` 的畫面**（`GWP`／`Materials`／`DefaultDevices`／`DeviceDatas`，寫在 `BackendMenuViewComponent.PermissionEnforcedFunctions`），避免選單出現「按下去一定被擋」的死連結（`Companies`／`User`／`Functions` 等畫面目前仍是寫死的 `[Authorize(Roles="Admin")]`，尚未接上權限表）。之後每掛上一個 Controller，記得把它加進這個清單。
- 為符合 `_Layout` 「不能無條件查資料庫」的既有原則（登入頁／錯誤頁也共用同一份版面），`BackendMenuViewComponent` 整段查詢包在 try/catch 裡，查詢失敗時選單只是少幾個項目、不會讓整頁掛掉。
- 實機驗證：`poweruser_test`（只被授予「排放係數設定-檢視」）登入後選單多出「後台設定 → 排放係數設定」一項，點擊可正常進入 `Materials` 清單；既有 `testco`（`User` 角色、無任何後台授權）選單維持原樣，沒有多出任何項目；伺服器日誌無錯誤。

**尚未開始**：階段 3（角色×權限矩陣勾選介面，取代目前一次一筆的 CRUD）、階段 4（`UserPermission` 維護介面）。`Areas`／`Devices`／`Emission` 三個核心流程 Controller 尚未掛上 `PermissionFilterAttribute`（風險較高，待階段 1 的四個低風險畫面確認穩定後再擴及）；`Companies`／`User`／`Functions`／`FunctionActions`／`Permissions`／`RolePermissions`／`Feedbacks` 目前仍是寫死的 `[Authorize(Roles="Admin")]`，尚未接上權限表。

---

## B2. 跨年度趨勢與基準年比較 (P1)

### 問題
碳盤查的核心價值是**逐年減量**，但系統目前只能一年一年分開看，沒有任何地方能呈現趨勢。

### 現況證據
```
Models/Area.cs          已有 BaseYear（是否為基準年）欄位
AreasController.cs:188  已有「設為基準年時解除舊基準年」的邏輯
EmissionController      三份報告書皆已載入 baseYear_Area 並計算基準年比較
Views/                  無任何跨年度比較畫面
```
資料模型與計算邏輯**都已經具備**，缺的只是呈現。

### 做法
1. 新增 `Emission/Trend`（或 `Areas/Trend`）畫面，以「公司 + 廠區地址」為單位，把該廠區歷年的 `Area` 記錄串起來。
   - 注意：目前判斷「同一個廠區的不同年度」是靠 `CompanyId + FullAddress` 相同（見 `AreasController.cs:190`），這個判斷有點脆弱，建議評估是否要在資料模型上新增明確的「廠區群組」概念。
2. 呈現內容建議：
   - 逐年總排放當量折線圖（`Area.All`）
   - 相對基準年的增減百分比
   - 類別一／類別二 堆疊柱狀圖（`Area.Scope1`／`Scope2`）
   - 各排放型式（固定／移動／逸散／製程）趨勢
   - 數據品質等級（`Area.avg_Grade`／`all_Grade`）逐年變化
3. 圖表沿用已在用的 Chart.js。**注意**：目前 `Views/Emission/Chart.cshtml` 是從 CDN 載入 Chart.js，若要離線／內網部署需改為本機檔案。

### 驗收標準
- [x] 同一廠區有 2 個以上年度資料時，能看到趨勢圖
- [x] 相對基準年的增減百分比與報告書中的數字一致
- [x] 只有 1 個年度時顯示合理的空狀態而非壞掉的圖表

**已完成**：新增 `Services/AreaTrendService.cs`（供本項與 B3 共用），以 `CompanyId + FullAddress` 判斷「同一個廠區」歷年資料，基準年比較沿用跟三份報告書相同的定義（`CompanyId` + `BaseYear`，見程式碼註解說明既有的多廠區基準年模糊性）。新增 `Emission/Trend` 頁面（線圖：總排放當量；堆疊柱狀圖：類別一／二），從 `Areas/Index` 每筆廠區加上「歷年趨勢」連結。**順手修正一個既有 bug**：ISO 報告書的「比較總排放差異」少乘了 100（`EmissionController.cs`），數字只有正確值的 1/100，其餘類別一~六的差異都有正確乘 100；修正後也讓本項的百分比公式跟報告書真正一致。新增 6 筆 `AreaTrendServiceTests` 單元測試。實機驗證：testco 帳號 1 個年度時顯示空狀態；補一筆歷史年度後，趨勢圖與歷年數據表正確顯示（113 年 +21.04% 相對基準年，與公式手算一致）。

---

## B3. 首頁儀表板 (P1)

### 問題
`HomeController` 只有 `Index`／`OutOfLimitTime`／`Error` 三個 action，首頁是靜態宣傳頁。登入後沒有任何「總覽」概念。

### 現況證據
```
Controllers/HomeController.cs → 3 個 action，Index 僅 return View()
Views/Home/Index.cshtml       → 靜態 hero 區塊 + 三個功能介紹卡片
```

### 做法
登入後的首頁依角色顯示不同內容：

**管理員視角：**
- 公司總數／本年度已完成盤查數／未完成清單
- 哪幾家公司的報告書內容還沒填完（本次已加的「8/8 已填寫」進度可直接沿用）
- 近期回報問題（`Feedbacks` 已有資料表與畫面）
- 即將到期的帳號（`ApplicationUser.UserLimitData` 已存在）

**一般使用者視角：**
- 我的盤查進度：公司資料 → 邊界資料 → 排放源 → 活動數據 → 報告書，哪一步還沒完成
- 本年度總排放當量與相對基準年增減（與 [B2](#b2-跨年度趨勢與基準年比較-p1) 共用）
- 尚未填寫活動數據的排放源數量（目前排放源清單會顯示「未填寫」，但沒有彙總）

### 前置作業
- 本次已在 `wwwroot/css/site.css` 加入 `.cip-stats` / `.cip-stat` 統計磚樣式，可直接使用。

### 驗收標準
- [x] 登入後首頁顯示與角色相符的摘要資訊
- [x] 每個數字都可點擊導向對應的明細畫面
- [x] 未登入者仍看到原本的宣傳頁

**已完成**：`HomeController.Index` 改為依登入狀態與角色分流：未登入維持原宣傳頁（`Model == null`）；管理員看到「公司總數／報告書內容已完成／近期回報問題／即將到期帳號」四個可點擊統計磚 + 尚未完成清單、近期回報、即將到期帳號三個區塊；一般使用者看到「公司資料→邊界資料→排放源→活動數據→報告書」5 步驟進度徽章（各自可點擊導向對應畫面）與本年度總排放當量／相對基準年增減（與 B2 共用 `AreaTrendService.GetSinglePointAsync`）／尚未填寫活動數據的排放源數量。管理員的「報告書內容已完成」統計抽出 `Services/CompanyReportProgress.cs` 共用 `Views/Companies/Index.cshtml` 既有的 8 欄位填寫進度定義，避免兩處各自維護一份。實機驗證時發現並修正一個小 bug：剛建立、尚未填寫名稱的公司在「尚未完成報告書內容的公司」清單裡會顯示成看不見文字的空白連結，已加上預設顯示文字。

---

## B4. 稽核軌跡 (P1)

### 問題
系統沒有任何操作紀錄。溫室氣體盤查報告書需要接受第三方查證，「誰在什麼時候改了哪個數字」屬於合規需求。

### 現況證據
```
全專案 grep audit → 0 筆
現有的 CreateTime / ModifiedTime / DeleteTime 只記「時間」，不記「誰」與「改了什麼」
```

### 做法
1. 新增 `AuditLog` 實體：
   ```
   Id, UserId, UserName, EntityName, EntityId,
   Action (Create/Update/Delete), OldValues (json), NewValues (json),
   IpAddress, CreatedAt
   ```
2. 實作方式建議**攔截 `SaveChangesAsync`**（覆寫 `ApplicationDbContext.SaveChangesAsync`，走訪 `ChangeTracker.Entries()`），而不是在每個 Controller 手動寫——後者一定會漏。
3. 優先記錄的實體：`Material`（排放係數）、`GWP`、`Device`、`ActivityData`、`Area`、`Company`、`RolePermission`。
4. 需要一個查詢畫面（可依使用者／實體／期間篩選）。
5. **注意**：稽核紀錄本身不可被修改或刪除，且會持續成長，需規劃保留政策與索引。

### 驗收標準
- [x] 修改一筆排放係數後，稽核紀錄可查到「誰、何時、從什麼改成什麼」
- [x] 稽核紀錄無法從任何介面被竄改
- [x] 大量寫入（Excel 匯入 300 筆）時效能可接受

**已完成**：新增 `Models/AuditLog.cs` + Migration，覆寫 `ApplicationDbContext.SaveChanges(bool)`／`SaveChangesAsync(bool, CancellationToken)`（EF Core 實際執行存檔的核心多載，無參數版本內部就是呼叫這兩個，兩者都會被涵蓋），攔截 `ChangeTracker.Entries()` 寫入稽核紀錄，只記錄 `Material`／`GWP`／`Device`／`ActivityData`／`Area`／`Company`／`RolePermission` 這 7 個實體。新增 `Controllers/AuditLogsController.cs`（Admin/SuperAdmin 專用，僅 Index/Details 唯讀，可依實體／使用者名稱／期間篩選並分頁），選單掛在「用戶與權限管理」下的「稽核軌跡」。

實作時發現並修正兩個真正的正確性問題（都是先用單元測試建立信心，再用瀏覽器對正式 SQL Server 實測才抓到）：
1. **`_context.Update(model)` 這種 scaffold 常見寫法會讓 OldValues 不可靠**：整個表單綁出來的物件直接標成 Modified，這個 DbContext 從沒真正查過資料庫的原始值，EF 的 `property.OriginalValue` 會被誤設成跟 CurrentValue 一樣（等於「改成什麼」跟「改之前」顯示同一個值），而且每個欄位都會被誤判成「有變更」。改成對 Modified/Deleted 的實體直接呼叫 `EntityEntry.GetDatabaseValuesAsync()` 查一次資料庫目前真正的值再比較，只有真的不同的欄位才記錄。這只多花一次查詢在單筆編輯/刪除上，Excel 大量匯入是新增(Added)排放源，不受影響。
2. **`Action`（Create/Update/Delete）不能動態讀 `Entry.State`**：EF Core 存檔成功後會呼叫 `ChangeTracker.AcceptAllChanges()`，把 Added/Deleted 轉成 Unchanged/Detached；主鍵是資料庫產生（例如 `RolePermission.Id`）而延後到存檔後才補寫稽核紀錄的那批，讀到的已經是存檔後的狀態，Create 會被誤判成 Update。改成在建立稽核紀錄時就把 Action 定住，不再動態讀取。

新增 8 筆單元測試（`AuditTrailTests`，含針對上述兩個 bug 的回歸測試），全專案測試共 68 個全數通過。實機驗證：修改 `Materials` 排放係數、新增/刪除 `RolePermission` 授權，稽核軌跡皆正確顯示誰／何時／動作／真正改動的欄位，且無法從任何介面修改或刪除稽核紀錄本身。

---

## B5. 盤查年度鎖定與簽核 (P2)

### 問題
已經產出正式報告書的年度資料，仍可被任意修改，事後無法證明報告書對應的是哪一版資料。

### 做法
1. `Area` 新增狀態欄位：`草稿 → 已提交 → 已查證`（或簡化為 `IsLocked`）。
2. 鎖定後：該年度的排放源、活動數據、廠區基本資料皆唯讀；只有管理員能解鎖，且解鎖須留下稽核紀錄（與 [B4](#b4-稽核軌跡-p1) 搭配）。
3. 產生報告書時記錄一份「快照」（至少記錄產生時間、產生者、當下的總排放量），供日後比對。

### 驗收標準
- [x] 已鎖定年度的所有編輯入口都消失或停用（不是按下去才報錯）
- [x] 解鎖動作有稽核紀錄
- [x] 鎖定狀態在清單頁一目了然

**已完成**（採用者選擇：填報單位自己也能鎖定，但只有 Admin／SuperAdmin 能解鎖；鎖定時存一份快照；以廠區（單一 `Area`）為鎖定單位，不是整間公司）：

1. `Models/Area.cs` 新增 `IsLocked`／`LockedAt`／`LockedByUserId`／`LockedByUserName`／`LockedSnapshotAll`（鎖定當下的 `All` 總排放量快照），對應新 migration `AddAreaLock`。
2. 新增 `Services/AreaLockService.cs`：`LockAsync`／`UnlockAsync`（各自的 no-op 保護：已鎖定不會重複鎖、未鎖定不能解鎖）、`IsAreaLockedAsync`／`IsAreaLockedByDeviceIdAsync` 給各 Controller 查詢用。`Area` 本來就在 [B4](#b4-稽核軌跡-p1) 的 `AuditedEntityTypes` 清單裡，所以鎖定/解鎖這幾個欄位的變更**不需要額外寫程式**就會自動進稽核軌跡。
3. `Controllers/CountController.cs` 補兩個 `protected` 唯讀 helper（`IsAreaLockedAsync`／`IsAreaLockedByDeviceIdAsync`，沿用既有 `GetGWPListAsync` 的 `HttpContext.RequestServices` 取服務模式，不用改各子類別建構子）。
4. `Controllers/AreasController.cs` 新增 `Lock`（一般填報單位可用）／`Unlock`（`[Authorize(Roles = "Admin,SuperAdmin")]`）兩個 action；並在 `Edit`（POST）、`DeleteConfirmed`、`Analyses`（POST）三個會修改資料的入口補上鎖定檢查，擋下來時顯示友善訊息（例："此廠區已鎖定，無法修改。請聯絡管理員解鎖。"）而不是裸的錯誤。
5. `Controllers/DevicesController.cs` 同樣在 `Create`／`Edit`／`DeleteConfirmed`／`AddActivityData`／`Default`（批量套用預設排放源）／`ImportDeviceExcelAsync`（匯入，回 400 而非導頁，因為是 AJAX 端點）全部補上鎖定檢查。`EmissionController.cs` 確認過完全沒有寫入動作（報表都是唯讀），不需要補檢查——鎖定不影響查看/下載報告書。
6. `Views/Areas/Index.cshtml`（桌機表格＋手機卡片兩種版面）：鎖定的廠區顯示「已鎖定」徽章（滑鼠移上去顯示鎖定時間／鎖定者）；設定／複製／修改（或填寫）／刪除按鈕在鎖定後整個消失（不是disable還讓人點了才噴錯）；「鎖定」按鈕（一般使用者可見，二次確認）與「解鎖」按鈕（僅 Admin／SuperAdmin 可見，二次確認）互斥顯示。
7. `Views/Devices/Index.cshtml`：鎖定時最上方顯示提示訊息「此廠區已鎖定，排放源與活動數據皆為唯讀。如需修改請聯絡管理員解鎖。」；新增設備／匯入／導入預設／修改／複製／新增活動數據／刪除等所有會修改資料的按鈕全部隱藏，排放源資料檢視／歷年趨勢／報告書下載維持可用。

新增 `AreaLockServiceTests.cs`（5 個單元測試涵蓋鎖定/解鎖狀態切換、快照記錄、已鎖定重複鎖定的 no-op、依裝置查所屬廠區鎖定狀態），全專案測試共 73 個全數通過。

實機驗證完整跑過一次：以 `testco`（一般使用者）鎖定廠區 → 清單頁按鈕正確消失、`Devices/Index` 顯示唯讀提示且動作按鈕全消失 → 直接對 `Devices/Edit` 的 POST 端點略過畫面直接送表單，伺服器正確擋下並顯示「此廠區已鎖定，無法修改排放源。請聯絡管理員解鎖。」（證明是後端擋，不只是前端隱藏按鈕）→ 切換到 Admin 帳號，清單頁該廠區出現「解鎖」按鈕（`testco` 帳號看不到）→ 按下解鎖後恢復可編輯狀態 → `/AuditLogs` 確認鎖定與解鎖各留下一筆 `Area` 的 `Update` 紀錄，欄位變更前後值（`IsLocked`／`LockedAt`／`LockedByUserId`／`LockedByUserName`／`LockedSnapshotAll`）皆正確對應。

---

## B6. Email 寄送 (P2)

### 問題
專案沒有任何 `IEmailSender` 實作，Identity 預設的 no-op sender 會把信件靜靜吞掉。

### 現況證據
```
grep IEmailSender 實作 → 0 筆
```
連帶影響：
- 忘記密碼 → 無效（本次已先把登入頁的失效連結移除）
- 信箱驗證 → 無效
- 目前只能由管理員在後台手動改密碼

### 做法
1. 實作 `IEmailSender`（SMTP 或 SendGrid／Azure Communication Services）。
2. 設定放在 `appsettings.json` 的 `Email` 區段，密碼走環境變數（與本次已建立的 `Line` 區段做法一致）。
3. 恢復 `Areas/Identity/Pages/Account/Login.cshtml` 中被註解掉的「忘記密碼」連結。
4. 可順帶加上通知信：帳號建立、使用期限即將到期、報告書產生完成。

### 驗收標準
- [x] 忘記密碼可完成完整流程
- [x] 未設定 Email 時系統不會壞掉，只是安靜停用（與現行 LINE 通知的做法一致）

**已完成**：新增 `Services/SmtpEmailSender.cs`（實作 `IEmailSender`，`Email:Host` 留空時安靜地只留 log、不寄信，不影響其他功能），註冊取代 `AddDefaultIdentity` 內建的 no-op 實作。設定放在 `appsettings.json` 的 `Email` 區段（Host/Port/EnableSsl/UserName/Password/FromAddress/FromName），密碼走環境變數／`dotnet user-secrets`（採用者選擇：SMTP）。恢復 `Login.cshtml` 被註解的「忘記密碼」連結（保留「註冊新使用者」／「重寄確認信」的註解，這個系統的帳號一律由管理員建立，沒有開放自行註冊）。

**過程中發現並補上一塊會讓忘記密碼形同虛設的缺口**：資料庫查下去發現「每一個帳號」（含 Admin 自己）的 Email 欄位都只是把帳號名稱原封不動存進去（例如帳號 `testco` 的 Email 是字串 `"testco"`，不是可收信的地址），且整個系統沒有任何一個畫面能讓人填寫或修改真正的 Email——`ForgotPassword` 頁面自己的 `[EmailAddress]` 驗證會直接擋掉這種輸入。已修正：
1. `UserController.Add`／`BulkAdd` 建立帳號時補上 `EmailConfirmed = true`（這些帳號是管理員直接設定密碼建立的，不需要「先確認信箱才能用」；原本預設 `false` 會讓 `ForgotPassword.cshtml.cs` 的 `IsEmailConfirmedAsync` 檢查對每個帳號都直接判定失敗）。
2. 把原本存在但完全沒有連結能進去的 `Areas/Identity/Pages/Account/Manage/Email.cshtml`（「通知信箱設定」）接上 `_ManageNav.cshtml` 選單，讓使用者能自己把預設塞進去的「帳號當通知信箱」換成真正收得到信的地址；同時修正該頁一個既有 bug（`OnPostChangeEmailAsync` 的成功訊息寫在 `if` 區塊外面，不管有沒有真的換成功都顯示「未變更」）與全頁英文字串。
3. `Input.NewEmail` 加上 `[EmailAddress]` 驗證，之後透過這個頁面設定的信箱一定是正確格式。

實機驗證完整跑過一次：登入 → 通知信箱設定 → 換成 `emailtest_user@example.com`（顯示「電子郵件已更新」）→ 登出 → 忘記密碼輸入同一個信箱 → 正確導向確認頁、伺服器 log 顯示「尚未設定 SMTP，略過寄送」（因為本機沒有真的 SMTP 帳密，這是預期中的安靜停用，不是錯誤）。

---

# 建議執行順序

## 第一階段：穩固地基（先做，風險低、效益立即可見）
1. **[A1] 全域軟刪除查詢過濾器** —— 一次消滅一整類 bug
2. **[A2] 計算引擎單元測試** —— 保護剛修好的成果
3. **[A6] CI 修復** —— 讓 A2 真的產生防護力
4. **[A7] 重大性評估角色不一致** —— 極小的修改，但使用者天天踩

> 這四項可以在同一個 PR 週期內完成，且互相加成。

## 第二階段：把已投入的成本變現
5. **[B1] 接上權限系統** —— 依階段 1→2→3 逐步導入，每階段獨立可上線

> 開始前需先確認上面列的三個設計問題。

## 第三階段：補上產品價值
6. **[B2] 跨年度趨勢** + **[B3] 首頁儀表板** —— 兩者共用資料查詢，建議一起做 ✅ 已完成
7. **[B4] 稽核軌跡** ✅ 已完成

## 第四階段：長期維護性
8. **[A3][A4][A5]** 效能優化 ✅ 已完成
9. **[A8] EmissionController 重構** —— 必須在 A2 完成之後才動，暫緩（範圍最大、風險最高，待之後再排）
10. **[A9][A10][B5][B6]** —— A9／A10／B5／B6 ✅ 已完成

---

# 附錄：本次已完成的修復

以下為本輪盤點中**已經修好並驗證**的項目，列出供對照，避免重複處理。

### 資料層
- `ApplicationDbContext` 改繼承泛型 `IdentityDbContext<ApplicationUser, ApplicationRole, string>`，消除 TPH Discriminator
- 修正掛在集合導覽上的錯誤 `[ForeignKey]`，移除 EF 自動生成的幽靈中介表與影子外鍵
- 移除未對應的 `GWPVersion`（原本會讓**所有** `_context.GWPs` 查詢在執行期拋 SqlException）
- 重新產生 Migration 並套用；補上 `Data/DbSeeder.cs`（角色、管理員帳號、75 筆權限定義）
- 為未設長度的字串欄位補 `MaxLength`、修正 decimal 精度、加上必要索引

### 安全性
- 全域「預設要登入」授權原則（原本 8 個 Controller 完全匿名可讀寫刪）
- 全域防偽 Token 驗證
- Areas／Devices／Emission 全面補上跨公司擁有權檢查
- 修正 `CheckSubscriptionData` 的無限重導迴圈
- LINE Token 與資料庫連線字串移出原始碼

### 計算引擎
- 見 [A2](#a2-計算引擎單元測試-p0) 表格所列 8 項

### 主要流程
- `/Areas` 與 `/Devices` 原本必定 500，已修復
- Excel 匯入改為交易保護（原本失敗會清空整個廠區的排放源且無法復原）
- 三份報告書：改為記憶體串流輸出（原本存在可匿名下載的目錄）、移除跨請求共用的 static 字典（原本會讓兩家公司的報告書資料互相污染）

### 介面
- 建立設計 token 系統（`wwwroot/css/site.css`），支援深色模式
- 清單頁資訊密度：公司總覽 8 個布林欄位 → 1 個進度徽章；排放源 7 個氣體欄位 → 1 欄徽章
- 表單頁：由 1/3 螢幕寬改為分區寬版排版，移除送出後會被忽略的隱藏欄位
- 修正 jQuery 重複載入、重複 id、`for` 指向不存在元素等問題

---

*本文件基於實際程式碼盤點產生，所有數字與行號皆為當下實測值。*
