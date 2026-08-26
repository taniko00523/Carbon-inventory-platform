namespace Carbon_inventory_platform.ViewModel
{
    /// <summary>
    /// B1 階段 3／4：角色（或使用者）× 權限的勾選矩陣，取代原本「一次一筆」的
    /// RolePermission／UserPermission CRUD。列＝畫面（Function），欄＝動作（FunctionAction）。
    /// </summary>
    public class PermissionMatrixViewModel
    {
        public List<PermissionMatrixColumn> Columns { get; set; } = new();
        public List<PermissionMatrixRow> Rows { get; set; } = new();

        /// <summary>
        /// 建立矩陣當下、可異動範圍內已授權的權限 Id（排序後以逗號串接，可能是空字串——
        /// 代表當下完全沒有任何授權）。存檔時會拿來跟「送出當下」資料庫的實際狀態比對——
        /// 如果不一致，代表這段時間有其他管理員也改過這個角色／使用者，直接套用會悄悄蓋掉
        /// 對方的異動，所以改為拒絕存檔並請使用者重新整理。
        /// </summary>
        public string Baseline { get; set; } = string.Empty;

        /// <summary>
        /// 給表單隱藏欄位用的編碼版本。ASP.NET Core 的表單繫結會把「值為空字串」的欄位
        /// 直接繫結成 null（等同「完全沒有這個欄位」），這裡加上固定前綴，讓 Baseline 剛好是
        /// 空字串（角色／使用者當下完全沒有任何授權）時，送出的欄位仍然是非空字串，
        /// 才不會被誤判成「呼叫端沒有提供 baseline，不用檢查」而讓併發保護整個失效。
        /// 對應的還原邏輯在 Controller 端（DecodeExpectedBaseline）。
        /// </summary>
        public string EncodedBaseline => BaselineFieldPrefix + Baseline;

        public const string BaselineFieldPrefix = "v1:";

        /// <summary>把表單送回來的 expectedBaseline 欄位還原成可以直接跟 Baseline 比對的值。</summary>
        public static string? DecodeExpectedBaseline(string? posted)
            => posted != null && posted.StartsWith(BaselineFieldPrefix, StringComparison.Ordinal)
                ? posted.Substring(BaselineFieldPrefix.Length)
                : null;
    }

    public class PermissionMatrixColumn
    {
        public int FunctionActionId { get; set; }
        public string CName { get; set; } = string.Empty;
    }

    public class PermissionMatrixRow
    {
        public int FunctionId { get; set; }
        public string CName { get; set; } = string.Empty;

        /// <summary>跟 Columns 等長、依序對齊；null 表示這個畫面沒有定義這個動作的權限（畫面上顯示為空白格）。</summary>
        public List<PermissionMatrixCell?> Cells { get; set; } = new();
    }

    public class PermissionMatrixCell
    {
        public int PermissionId { get; set; }
        public bool Checked { get; set; }

        /// <summary>
        /// 只有使用者個別權限矩陣會用到：true 表示這項授權來自角色。
        /// 目前的資料結構無法表達「個別收回角色已授予的權限」，畫面上鎖住不能取消勾選。
        /// </summary>
        public bool Locked { get; set; }
    }

    public enum PermissionMatrixSaveResult
    {
        Saved,
        /// <summary>送出時資料庫的實際狀態已經跟畫面渲染當下不一樣（有別的管理員也改過），拒絕覆蓋。</summary>
        Conflict,
    }
}
