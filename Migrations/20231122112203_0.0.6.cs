using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class _006 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CH4CEF", "CO2CEF", "CO2ULL", "N2OCEF", "Scope" },
                values: new object[] { 0.000106763255f, 2.5587628f, 0.015299f, 2.1352378E-05f, "範疇1" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CH4CEF", "CO2ULL", "N2OCEF", "Scope" },
                values: new object[] { 0.00010550725f, 0.020243f, 2.1101378E-05f, "範疇1" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "CH4CEF", "CO2CEF", "CO2ULL", "N2OCEF", "Scope" },
                values: new object[] { 9.797125E-05f, 2.2631328f, 0.025974f, 1.9594378E-05f, "範疇1" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "CH4CEF", "CO2CEF", "CO2ULL", "N2OCEF", "Scope" },
                values: new object[] { 2.7779251E-05f, 1.7528813f, 0.023772f, 2.777378E-06f, "範疇1" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "CH4CEF", "CO2CEF", "CO2ULL", "N2OCEF", "Scope" },
                values: new object[] { 0.00012057925f, 2.9461675f, 0.0191f, 2.4116378E-05f, "範疇1" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "CH4CEF", "CO2ULL", "N2OCEF", "Scope" },
                values: new object[] { 0.00011304325f, 0.015007f, 2.2608378E-05f, "範疇1" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "CH4CEF", "CO2CEF", "CO2ULL", "N2OCEF", "Scope" },
                values: new object[] { 4.643125E-05f, 2.8601873f, 0.082792f, 4.643378E-06f, "範疇1" });

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "Id", "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "EmissionPattern", "N2OCEF", "N2OULL", "N2OUUL", "Name", "Scope", "Unit", "Year" },
                values: new object[,]
                {
                    { 1, 2.4660252E-05f, 0.7f, 2f, 2.3328598f, 0.077167f, 0.067653f, "固定", 3.699038E-05f, 0.666667f, 2.333333f, "自產煤", "範疇1", "Kg", 0 },
                    { 2, 2.8470253E-05f, 0.7f, 2f, 2.6932847f, 0.077167f, 0.067653f, "固定", 4.270538E-05f, 0.666667f, 2.333333f, "原料煤", "範疇1", "Kg", 0 },
                    { 3, 2.5455252E-05f, 0.7f, 2f, 2.4081135f, 0.077167f, 0.067653f, "固定", 3.8183378E-05f, 0.666667f, 2.333333f, "燃料煤", "範疇1", "Kg", 0 },
                    { 4, 2.9726252E-05f, 0.7f, 2f, 2.9220934f, 0.03764f, 0.027467f, "固定", 4.458938E-05f, 0.666667f, 2.333333f, "無煙煤", "範疇1", "Kg", 0 },
                    { 5, 2.8470253E-05f, 0.7f, 2f, 2.6932847f, 0.077167f, 0.067653f, "固定", 4.270538E-05f, 0.666667f, 2.333333f, "焦煤", "範疇1", "Kg", 0 },
                    { 6, 2.5455252E-05f, 0.7f, 2f, 2.4081135f, 0.053911f, 0.053911f, "固定", 3.8183378E-05f, 0.666667f, 2.333333f, "煙煤", "範疇1", "Kg", 0 },
                    { 7, 2.0515252E-05f, 0.7f, 2f, 1.9715222f, 0.034339f, 0.040583f, "固定", 3.077338E-05f, 0.666667f, 2.333333f, "亞煙煤(發電)", "範疇1", "Kg", 0 },
                    { 8, 2.3446251E-05f, 0.7f, 2f, 2.2531683f, 0.034339f, 0.040583f, "固定", 3.5169378E-05f, 0.666667f, 2.333333f, "亞煙煤(其他)", "範疇1", "Kg", 0 },
                    { 9, 1.1907252E-05f, 0.7f, 2f, 1.2026331f, 0.1f, 0.138614f, "固定", 1.7860379E-05f, 0.666667f, 2.333333f, "褐煤", "範疇1", "Kg", 0 },
                    { 10, 8.905252E-06f, 0.7f, 2f, 0.95286965f, 0.157009f, 0.168224f, "固定", 1.3358378E-05f, 0.666667f, 2.333333f, "油頁岩", "範疇1", "Kg", 0 },
                    { 11, 9.767252E-06f, 0.7f, 2f, 1.0353873f, 0.056604f, 0.018868f, "固定", 1.4651378E-05f, 0.666667f, 2.333333f, "泥煤", "範疇1", "Kg", 0 },
                    { 12, 1.5909252E-05f, 0.7f, 2f, 1.5512094f, 0.104615f, 0.117949f, "固定", 2.3864379E-05f, 0.666667f, 2.333333f, "煤球", "範疇1", "Kg", 0 },
                    { 13, 2.9307252E-05f, 0.7f, 2f, 3.1359131f, 0.105607f, 0.11215f, "固定", 4.396138E-05f, 0.666667f, 2.333333f, "焦炭", "範疇1", "Kg", 0 },
                    { 14, 0.000102995255f, 0.666667f, 2.333333f, 3.3473465f, 0.149744f, 0.179487f, "固定", 2.0599378E-05f, 0.666667f, 2.333333f, "石油焦", "範疇1", "Kg", 0 },
                    { 15, 9.420325E-05f, 0.666667f, 2.333333f, 2.19807f, 0.035714f, 0.042857f, "固定", 1.8840377E-05f, 0.666667f, 2.333333f, "航空汽油", "範疇1", "L", 0 },
                    { 16, 0.00010048325f, 0.666667f, 2.333333f, 2.3948495f, 0.025175f, 0.040559f, "固定", 2.0096379E-05f, 0.666667f, 2.333333f, "航空燃油", "範疇1", "L", 0 },
                    { 17, 0.00011304325f, 0.666667f, 2.333333f, 2.762032f, 0.030014f, 0.030014f, "固定", 2.2608378E-05f, 0.666667f, 2.333333f, "原油", "範疇1", "L", 0 },
                    { 18, 8.255925E-05f, 0.666667f, 2.333333f, 2.1190274f, 0.1f, 0.109091f, "固定", 1.6511378E-05f, 0.666667f, 2.333333f, "奧里油", "範疇1", "Kg", 0 },
                    { 19, 0.00013268825f, 0.666667f, 2.333333f, 2.8395245f, 0.0919f, 0.096573f, "固定", 2.6537378E-05f, 0.666667f, 2.333333f, "天然氣凝結油", "範疇1", "M3", 0 },
                    { 21, 0.00010799425f, 0.666667f, 2.333333f, 2.7945626f, 0.075034f, 0.080491f, "固定", 2.1598378E-05f, 0.666667f, 2.333333f, "頁岩油", "範疇1", "Kg", 0 },
                    { 24, 0.00012057925f, 0.666667f, 2.333333f, 3.1109598f, 0.024548f, 0.018088f, "固定", 2.4116378E-05f, 0.666667f, 2.333333f, "蒸餘油 (燃料油)", "範疇1", "L", 0 },
                    { 26, 9.797125E-05f, 0.666667f, 2.333333f, 2.393761f, 0.05457f, 0.040928f, "固定", 1.9594378E-05f, 0.666667f, 2.333333f, "石油腦", "範疇1", "L", 0 },
                    { 27, 0.00012560425f, 0.666667f, 2.333333f, 3.3787477f, 0.095415f, 0.114002f, "固定", 2.5120378E-05f, 0.666667f, 2.333333f, "柏油", "範疇1", "L", 0 },
                    { 31, 3.3494252E-05f, 0.7f, 2f, 1.8790358f, 0.032086f, 0.039216f, "固定", 3.349378E-06f, 0.7f, 2f, "天然氣", "範疇1", "M3", 0 },
                    { 32, 3.7681253E-05f, 0.7f, 2f, 2.170437f, 0.163194f, 0.197917f, "固定", 3.768378E-06f, 0.7f, 2f, "煉油氣", "範疇1", "M3", 0 },
                    { 33, 1.7584252E-05f, 0.7f, 2f, 0.78075445f, 0.15991f, 0.218468f, "固定", 1.758378E-06f, 0.7f, 2f, "焦爐氣", "範疇1", "M3", 0 },
                    { 34, 3.253252E-06f, 0.7f, 2f, 0.8458173f, 0.157692f, 0.184615f, "固定", 3.25378E-07f, 0.7f, 2f, "高爐氣", "範疇1", "M3", 0 },
                    { 35, 0.00025492726f, 0.666667f, 2.333333f, 0.77922726f, 0.200654f, 0.31952f, "固定", 3.3990378E-05f, 0.625f, 2.75f, "一般廢棄物", "範疇1", "Kg", 0 },
                    { 36, 9.420325E-05f, 0.666667f, 2.333333f, 2.19807f, 0.035714f, 0.042857f, "移動", 1.8840377E-05f, 0.666667f, 2.333333f, "航空汽油", "範疇1", "L", 0 },
                    { 37, 0.00010048325f, 0.666667f, 2.333333f, 2.3948495f, 0.025175f, 0.040559f, "移動", 2.0096379E-05f, 0.666667f, 2.333333f, "航空燃油", "範疇1", "L", 0 },
                    { 38, 0.00081642624f, 0.666667f, 2.44f, 2.2631328f, 0.025974f, 0.053391f, "移動", 0.00026125638f, 0.666667f, 2.333333f, "車用汽油", "範疇1", "L", 0 },
                    { 39, 0.00013715925f, 0.589744f, 1.435897f, 2.606032f, 0.020243f, 0.009447f, "移動", 0.00013715938f, 0.666667f, 2.076923f, "柴油", "範疇1", "L", 0 },
                    { 40, 0.000106763255f, 0.666667f, 2.333333f, 2.5587628f, 0.015299f, 0.025035f, "移動", 2.1352378E-05f, 0.666667f, 2.333333f, "煤油", "範疇1", "L", 0 },
                    { 41, 0.00012057925f, 0.666667f, 2.333333f, 2.9461675f, 0.0191f, 0.025921f, "移動", 2.4116378E-05f, 0.666667f, 2.333333f, "潤滑油", "範疇1", "L", 0 },
                    { 42, 0.0017223232f, 0f, 0f, 1.7528813f, 0.023772f, 0.03962f, "移動", 5.555378E-06f, 0f, 0f, "液化石油氣", "範疇1", "L", 0 },
                    { 43, 0.0034666702f, 0.456522f, 15.73913f, 2.1139152f, 0.032086f, 0.039216f, "移動", 0.00011304338f, 0.666667f, 24.666668f, "液化天然氣", "範疇1", "M3", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CH4CEF", "CO2CEF", "CO2ULL", "N2OCEF", "Scope" },
                values: new object[] { 0.000107f, 2.558763f, -0.015299f, 2.1E-05f, "類別1" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CH4CEF", "CO2ULL", "N2OCEF", "Scope" },
                values: new object[] { 0.000106f, -0.020243f, 2.1E-05f, "類別1" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "CH4CEF", "CO2CEF", "CO2ULL", "N2OCEF", "Scope" },
                values: new object[] { 9.8E-05f, 2.263133f, -0.025974f, 2E-05f, "類別1" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "CH4CEF", "CO2CEF", "CO2ULL", "N2OCEF", "Scope" },
                values: new object[] { 2.8E-05f, 1.752881f, -0.023772f, 3E-06f, "類別1" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "CH4CEF", "CO2CEF", "CO2ULL", "N2OCEF", "Scope" },
                values: new object[] { 0.000121f, 2.946167f, -0.0191f, 2.4E-05f, "類別1" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "CH4CEF", "CO2ULL", "N2OCEF", "Scope" },
                values: new object[] { 0.000113f, -0.015007f, 2.3E-05f, "類別1" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "CH4CEF", "CO2CEF", "CO2ULL", "N2OCEF", "Scope" },
                values: new object[] { 4.6E-05f, 2.860187f, -0.082792f, 5E-06f, "類別1" });

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "Id", "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "EmissionPattern", "N2OCEF", "N2OULL", "N2OUUL", "Name", "Scope", "Unit", "Year" },
                values: new object[,]
                {
                    { 50, 0.000816f, 0.666667f, 2.44f, 2.263133f, -0.025974f, 0.053391f, "移動", 0.000261f, 0.666667f, 2.333333f, "車用汽油", "類別1", "L", 0 },
                    { 51, 0.000137f, 0.589744f, 1.435897f, 2.606032f, -0.020243f, 0.009447f, "移動", 0.000137f, 0.666667f, 2.076923f, "柴油", "類別1", "L", 0 },
                    { 52, 0.000107f, 0.666667f, 2.333333f, 2.558763f, -0.015299f, 0.025035f, "移動", 2.1E-05f, 0.666667f, 2.333333f, "煤油", "類別1", "L", 0 },
                    { 53, 0.000121f, 0.666667f, 2.333333f, 2.946167f, -0.0191f, 0.025921f, "移動", 2.4E-05f, 0.666667f, 2.333333f, "潤滑油", "類別1", "L", 0 },
                    { 54, 0.001722f, 0f, 0f, 1.752881f, -0.023772f, 0.03962f, "移動", 6E-06f, 0f, 0f, "液化石油氣", "類別1", "L", 0 },
                    { 55, 0.003467f, 0.456522f, 15.73913f, 2.113915f, -0.032086f, 0.039216f, "移動", 0.000113f, 0.666667f, 24.666668f, "液化天然氣", "類別1", "M3", 0 }
                });
        }
    }
}
