namespace Carbon_inventory_platform.Services
{
    /// <summary>
    /// 排放量計算引擎中，不依賴資料庫的純數學函式。
    /// 從 Controllers/CountController.cs 抽出，讓這些邏輯可以直接寫單元測試
    /// （CountController 本身依賴 ApplicationDbContext，不適合直接測純計算）。
    /// CountController 上同名的方法改為呼叫這裡，行為完全不變，呼叫端不需要修改。
    /// </summary>
    public static class EmissionMath
    {
        /// <summary>用牛頓法逼近 Decimal 的平方根。</summary>
        public static decimal DecimalSqrt(decimal value)
        {
            if (value < 0)
            {
                throw new ArgumentException("不能計算負數的平方根");
            }
            // 原本固定只跑20次牛頓法且起始值為 value/2，大數值(廠區不確定性平方和常達1e12以上)
            // 還沒收斂就回傳，會導致95%信賴區間被高估；而且 value == 0 時 guess 也是0，
            // value / guess 會丟出 DivideByZeroException。
            if (value == 0)
            {
                return 0;
            }

            decimal guess = (decimal)Math.Sqrt((double)value); // 先用 double 開根號當起始值，收斂快很多
            if (guess <= 0)
            {
                guess = value;
            }

            for (int i = 0; i < 100; i++) // 逼近到收斂為止，並保留次數上限避免無窮迴圈
            {
                decimal next = 0.5m * (guess + value / guess);
                if (Math.Abs(next - guess) <= 0.0000000001m)
                {
                    return next;
                }
                guess = next;
            }

            return guess;
        }

        /// <summary>計算兩數平方和的平方根，並四捨五入到小數點後5位。</summary>
        public static decimal CalculateRoundDistance(decimal num1, decimal num2)
        {
            // 原本只要其中一個參數為0就直接回傳0，但 sqrt(x²+0²) 應該是 |x|，
            // 會導致大部分 Materials(DataUUL/DataULL 未填為0)的不確定性整個被歸零。
            if (num1 == 0 && num2 == 0)
            {
                return 0;
            }
            decimal distance = DecimalSqrt(num1 * num1 + num2 * num2);
            return Math.Round(distance, 5);
        }

        /// <summary>計算三種溫室氣體合成的 95% 信賴區間占比。</summary>
        public static decimal Calculate95U(decimal ghg1, decimal ghg2, decimal ghg3, decimal ghg1Uul, decimal ghg2Uul, decimal ghg3Uul)
        {
            // 原本整段合成邏輯都包在「第一個氣體的排放量與不確定性都不為0」的條件裡，
            // 只要CO2排放量或其不確定性為0(很常見)就直接回傳0，會導致後面CH4/N2O的不確定性被整個丟掉。
            // 改成直接對三個氣體做「排放量×不確定性」的平方和開根號，再除以總排放量(單一氣體的結果與原本相同)。
            decimal count1 = ghg1 * ghg1Uul;
            decimal count2 = ghg2 * ghg2Uul;
            decimal count3 = ghg3 * ghg3Uul;

            decimal count = DecimalSqrt((count1 * count1) + (count2 * count2) + (count3 * count3));
            decimal allGhg = ghg1 + ghg2 + ghg3;

            if (allGhg == 0)
            {
                return 0;
            }

            return count / allGhg;
        }

        /// <summary>
        /// 依平均等級分數判定清冊等級：avgGrade &lt; 10 為第一級，&lt; 19 為第二級，其餘第三級。
        /// 從 CountController.UpdateEmissionData 內的三元運算子抽出，方便單獨驗證邊界值。
        /// </summary>
        public static string GradeLabel(float avgGrade)
        {
            return avgGrade < 10 ? "第一級" : (avgGrade < 19 ? "第二級" : "第三級");
        }

        /// <summary>
        /// 依 device.Grade 分類到第一/二/三級的計數桶。原本第一級多了 device.Grade &gt; 0 的
        /// 條件，Grade 為 0（修正係數沒填）的設備會被錯誤歸到第二級，這裡與 GradeLabel 的
        /// 分級門檻保持一致（只看上限，不特別排除 0）。
        /// </summary>
        public static int GradeBucket(int deviceGrade)
        {
            if (deviceGrade < 10)
            {
                return 1;
            }
            if (deviceGrade < 19)
            {
                return 2;
            }
            return 3;
        }
    }
}
