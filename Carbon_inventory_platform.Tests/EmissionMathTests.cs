using Carbon_inventory_platform.Services;

namespace Carbon_inventory_platform.Tests;

/// <summary>
/// 對應 plan.md A2 清單中「已修好、需要回歸測試」的計算錯誤。
/// 每個測試方法的命名直接說明它防的是哪一個 bug，
/// 之後如果有人不小心改回舊行為，測試會立刻紅燈。
/// </summary>
public class EmissionMathTests
{
    // ---- DecimalSqrt --------------------------------------------------

    [Fact]
    public void DecimalSqrt_零值_回傳零_不拋除以零例外()
    {
        // 原本 guess = value / 2 在 value == 0 時也是 0，下一輪 value / guess 會除以零。
        var result = EmissionMath.DecimalSqrt(0m);

        Assert.Equal(0m, result);
    }

    [Fact]
    public void DecimalSqrt_負數_拋出例外()
    {
        Assert.Throws<ArgumentException>(() => EmissionMath.DecimalSqrt(-1m));
    }

    [Theory]
    [InlineData(4, 2)]
    [InlineData(9, 3)]
    [InlineData(2, 1.4142135624)]
    [InlineData(0.25, 0.5)]
    public void DecimalSqrt_一般數值_結果正確(double value, double expectedSqrt)
    {
        var result = EmissionMath.DecimalSqrt((decimal)value);

        Assert.True(Math.Abs(result - (decimal)expectedSqrt) < 0.0001m,
            $"sqrt({value}) 應為約 {expectedSqrt}，實際得到 {result}");
    }

    [Fact]
    public void DecimalSqrt_大數值_能在100次迭代內收斂()
    {
        // 原本固定跑 20 次牛頓法、起始值是 value/2；廠區不確定性平方和常達 1e12 以上，
        // 起始值離真解太遠，20 次跑不完就回傳一個明顯偏高、尚未收斂的數字。
        // 這裡驗證大數值仍能得到精確解（而不僅僅是「沒有拋例外」）。
        decimal bigValue = 1_000_000_000_000m; // 1e12，等同排放量平方和的典型量級
        decimal expected = 1_000_000m; // sqrt(1e12) = 1e6

        var result = EmissionMath.DecimalSqrt(bigValue);

        Assert.True(Math.Abs(result - expected) < 0.01m,
            $"大數值應收斂到約 {expected}，實際得到 {result}");
    }

    // ---- CalculateRoundDistance ---------------------------------------

    [Fact]
    public void CalculateRoundDistance_其中一個參數為零_回傳另一參數的絕對值()
    {
        // 原本 num1==0 || num2==0 就直接回傳 0，但 sqrt(x²+0²) 應該是 |x|。
        // Materials 種子資料裡 DataUUL/DataULL 常常只填一邊，另一邊是 0，
        // 這個 bug 會讓大部分排放源的不確定性直接被歸零。
        var result = EmissionMath.CalculateRoundDistance(5m, 0m);

        Assert.Equal(5.00000m, result);
    }

    [Fact]
    public void CalculateRoundDistance_兩個參數都為零_回傳零()
    {
        var result = EmissionMath.CalculateRoundDistance(0m, 0m);

        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateRoundDistance_兩個參數都不為零_回傳平方和的平方根()
    {
        var result = EmissionMath.CalculateRoundDistance(3m, 4m);

        Assert.Equal(5.00000m, result);
    }

    [Fact]
    public void CalculateRoundDistance_結果四捨五入到小數五位()
    {
        var result = EmissionMath.CalculateRoundDistance(1m, 1m);

        // sqrt(2) ≈ 1.41421356...，四捨五入到小數點後5位應為 1.41421
        Assert.Equal(1.41421m, result);
    }

    // ---- Calculate95U ---------------------------------------------------

    [Fact]
    public void Calculate95U_第一種氣體排放量為零_仍計入第二三種氣體的不確定性()
    {
        // 原本整段合成邏輯包在「第一種氣體(通常是CO2)排放量與不確定性都不為0」的條件裡，
        // CO2 為 0（例如純電力排放源）就直接回傳 0，CH4/N2O 的不確定性被整個丟掉。
        var resultWithFirstZero = EmissionMath.Calculate95U(
            ghg1: 0m, ghg2: 10m, ghg3: 0m,
            ghg1Uul: 0m, ghg2Uul: 0.5m, ghg3Uul: 0m);

        Assert.NotEqual(0m, resultWithFirstZero);
    }

    [Fact]
    public void Calculate95U_只有單一氣體_結果等於該氣體自己的不確定性占比()
    {
        // 單一氣體時，count = sqrt((GHG*UUL)^2) = GHG*UUL，count/allGHG = UUL。
        var result = EmissionMath.Calculate95U(
            ghg1: 100m, ghg2: 0m, ghg3: 0m,
            ghg1Uul: 0.2m, ghg2Uul: 0m, ghg3Uul: 0m);

        Assert.Equal(0.2m, result);
    }

    [Fact]
    public void Calculate95U_三種氣體排放量總和為零_回傳零_不除以零()
    {
        var result = EmissionMath.Calculate95U(0m, 0m, 0m, 0m, 0m, 0m);

        Assert.Equal(0m, result);
    }

    // ---- GradeLabel / GradeBucket ---------------------------------------

    [Theory]
    [InlineData(0f, "第一級")]
    [InlineData(9.99f, "第一級")]
    [InlineData(10f, "第二級")]
    [InlineData(18.99f, "第二級")]
    [InlineData(19f, "第三級")]
    [InlineData(100f, "第三級")]
    public void GradeLabel_邊界值_分級正確(float avgGrade, string expectedLabel)
    {
        Assert.Equal(expectedLabel, EmissionMath.GradeLabel(avgGrade));
    }

    [Fact]
    public void GradeBucket_設備等級為零_歸入第一級_而非第二級()
    {
        // 原本第一級的判斷多了 device.Grade > 0 這個條件，
        // Grade == 0（修正係數沒有任何一項被設定）的設備會被錯誤歸到第二級，
        // 跟 GradeLabel／畫面上顯示的「第一/二/三級」分級門檻不一致。
        Assert.Equal(1, EmissionMath.GradeBucket(0));
    }

    [Theory]
    [InlineData(9, 1)]
    [InlineData(10, 2)]
    [InlineData(18, 2)]
    [InlineData(19, 3)]
    [InlineData(27, 3)] // Device.Grade 的預設值
    public void GradeBucket_邊界值_與GradeLabel使用相同門檻(int grade, int expectedBucket)
    {
        Assert.Equal(expectedBucket, EmissionMath.GradeBucket(grade));
    }
}
