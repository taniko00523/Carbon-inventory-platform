using Carbon_inventory_platform.Models;

namespace Carbon_inventory_platform.Services
{
    // A8：三份報告書（ISO 14064-1／環境部／資策會）原本是三段各約 450 行、彼此 73%~99% 相同的程式碼，
    // 同一個 bug 必須在三個地方各修一次（專案歷史上已經發生過好幾次）。
    // 現在把「真正不一樣的地方」全部集中成這個描述物件，三個 static 實例並排看就是全部差異；
    // 共用的流程只有一份，放在 GhgReportBuilder。

    /// <summary>載入資料的方式。兩種查詢產生的「排放源排序」不同，而下游的表格/敘述都吃這個順序，所以不能統一。</summary>
    public enum ReportDataLoad
    {
        /// <summary>ISO：Include 整棵物件樹，排序用原本那個（實際上不生效的）Name 對照表，也就是維持 EF 回傳的自然順序。</summary>
        IncludeGraph,
        /// <summary>環境部／資策會：投影查詢，明確 OrderBy(Scope).ThenBy(EmissionPattern)。</summary>
        ProjectedOrdered,
    }

    /// <summary>
    /// 類別小計的來源。ISO 用「當下把 Device.Emissions 加總」，環境部／資策會用「Area 上已存的欄位」。
    /// 目前兩者數值相同，但刻意不統一：Area 只有 Scope1/Scope2 兩個欄位，
    /// ISO 需要的類別三~六沒有對應的資料庫欄位，只能即時加總；而且 Device.Emissions 會累加所有氣體，
    /// Area.Scope1 只累加七種已知氣體，哪天多一種氣體兩者就會分岔。
    /// </summary>
    public enum ScopeTotalsSource
    {
        DeviceSums,
        AreaColumns,
    }

    /// <summary>排放源清單要寫成哪種敘述。ISO 寫「固定/移動/逸散排放源說明」，環境部／資策會寫「類別一/二Device」編號清單。</summary>
    public enum ScopeNarrativeStyle
    {
        PatternDirections,
        NumberedDeviceList,
    }

    /// <summary>報告書裡的一張圖。Marker 是實際傳給 replaceImage 的字串——ISO 沒有角括號，其餘兩份有，這個差異要保留。</summary>
    public sealed class ImageSlot
    {
        public ImageSlot(string key, string marker, Func<Area, string?> path)
        {
            Key = key;
            Marker = marker;
            Path = path;
        }

        /// <summary>沒有圖時要寫進替換字典、把範本標記清空的鍵。</summary>
        public string Key { get; }
        /// <summary>有圖時傳給 replaceImage 的比對字串。</summary>
        public string Marker { get; }
        /// <summary>從 Area 取出圖片檔名。</summary>
        public Func<Area, string?> Path { get; }
    }

    public sealed class GhgReportSpec
    {
        private GhgReportSpec(
            string templateFileName,
            ReportDataLoad load,
            ScopeTotalsSource totals,
            ScopeNarrativeStyle narrative,
            bool emitEnglishCompanyName,
            bool emitForewordAndPurpose,
            bool emitInventoryYear,
            bool emitBaseYearComparison,
            IReadOnlyList<ImageSlot> images)
        {
            TemplateFileName = templateFileName;
            Load = load;
            Totals = totals;
            Narrative = narrative;
            EmitEnglishCompanyName = emitEnglishCompanyName;
            EmitForewordAndPurpose = emitForewordAndPurpose;
            EmitInventoryYear = emitInventoryYear;
            EmitBaseYearComparison = emitBaseYearComparison;
            Images = images;
        }

        public string TemplateFileName { get; }
        public ReportDataLoad Load { get; }
        public ScopeTotalsSource Totals { get; }
        public ScopeNarrativeStyle Narrative { get; }
        /// <summary>ISO 範本才有 &lt;公司英文名稱&gt; 標記。</summary>
        public bool EmitEnglishCompanyName { get; }
        /// <summary>環境部／資策會才有「前言」「預期用途」。</summary>
        public bool EmitForewordAndPurpose { get; }
        /// <summary>資策會範本沒有獨立的 &lt;盤查年&gt; 標記，原本就沒寫這個鍵，保持一致。</summary>
        public bool EmitInventoryYear { get; }
        /// <summary>只有 ISO 有基準年比較那一整段（30 個鍵）與類別三~六。</summary>
        public bool EmitBaseYearComparison { get; }
        /// <summary>順序即文件內的插圖順序，不能調換。</summary>
        public IReadOnlyList<ImageSlot> Images { get; }

        public static readonly GhgReportSpec Iso = new GhgReportSpec(
            templateFileName: "ISOReport.docx",
            load: ReportDataLoad.IncludeGraph,
            totals: ScopeTotalsSource.DeviceSums,
            narrative: ScopeNarrativeStyle.PatternDirections,
            emitEnglishCompanyName: true,
            emitForewordAndPurpose: false,
            emitInventoryYear: true,
            emitBaseYearComparison: true,
            images: new[]
            {
                // 原本 ISO 這兩個標記沒有角括號（其他兩份有），replaceImage 只會清掉內層文字、
                // 在範本留下多餘的 "<>"。這裡照原樣保留，避免這次重構動到輸出內容。
                new ImageSlot("廠區圖", "廠區圖", a => a.ShopDrawingsPath),
                new ImageSlot("地理位置圖", "地理位置圖", a => a.MapImagePath),
            });

        public static readonly GhgReportSpec Moe = new GhgReportSpec(
            templateFileName: "MOEReport.docx",
            load: ReportDataLoad.ProjectedOrdered,
            totals: ScopeTotalsSource.AreaColumns,
            narrative: ScopeNarrativeStyle.NumberedDeviceList,
            emitEnglishCompanyName: false,
            emitForewordAndPurpose: true,
            emitInventoryYear: true,
            emitBaseYearComparison: false,
            images: new[]
            {
                new ImageSlot("廠區圖", "<廠區圖>", a => a.ShopDrawingsPath),
                new ImageSlot("地理位置圖", "<地理位置圖>", a => a.MapImagePath),
            });

        // 範本檔名是 IIIReport.docx（三個 I），不是 IISReport.docx。
        public static readonly GhgReportSpec Iis = new GhgReportSpec(
            templateFileName: "IIIReport.docx",
            load: ReportDataLoad.ProjectedOrdered,
            totals: ScopeTotalsSource.AreaColumns,
            narrative: ScopeNarrativeStyle.NumberedDeviceList,
            emitEnglishCompanyName: false,
            emitForewordAndPurpose: true,
            emitInventoryYear: false,
            emitBaseYearComparison: false,
            images: new[]
            {
                // 公司組織圖夾在廠區圖與地理位置圖之間，順序會影響文件內插圖位置。
                new ImageSlot("廠區圖", "<廠區圖>", a => a.ShopDrawingsPath),
                new ImageSlot("公司組織圖", "<公司組織圖>", a => a.OrganizationImagePath),
                new ImageSlot("地理位置圖", "<地理位置圖>", a => a.MapImagePath),
            });
    }

    public enum GhgReportStatus
    {
        Ok,
        NotFound,
        /// <summary>可以回到排放源列表並顯示訊息的失敗（例：沒設定基準年、圖片讀不到）。</summary>
        Error,
    }

    /// <summary>
    /// 產生結果。刻意不用 IActionResult，讓 GhgReportBuilder 不依賴 MVC、可以單獨測試；
    /// 由 EmissionController 把它轉成 File()/NotFound()/RedirectToAction()。
    /// </summary>
    public sealed class GhgReportResult
    {
        private GhgReportResult(GhgReportStatus status, byte[]? fileBytes, string? fileName, string? errorMessage)
        {
            Status = status;
            FileBytes = fileBytes;
            FileName = fileName;
            ErrorMessage = errorMessage;
        }

        public GhgReportStatus Status { get; }
        public byte[]? FileBytes { get; }
        public string? FileName { get; }
        public string? ErrorMessage { get; }

        public static GhgReportResult Ok(byte[] fileBytes, string fileName)
            => new GhgReportResult(GhgReportStatus.Ok, fileBytes, fileName, null);

        public static GhgReportResult NotFound()
            => new GhgReportResult(GhgReportStatus.NotFound, null, null, null);

        public static GhgReportResult Error(string message)
            => new GhgReportResult(GhgReportStatus.Error, null, null, message);
    }
}
