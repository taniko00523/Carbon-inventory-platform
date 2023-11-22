using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class _0010 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "N2OCEF",
                table: "Materials",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "CO2CEF",
                table: "Materials",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "CH4CEF",
                table: "Materials",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 2.4660252E-05, 2.3328598392000002, 3.6990377999999997E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 2.847024E-05, 2.6932847039999999, 4.2705359999999997E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 2.5455744000000001E-05, 2.4081133823999998, 3.8183616000000002E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 2.9726279999999999E-05, 2.922093324, 4.4589419999999998E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 2.847024E-05, 2.6932847039999999, 4.2705359999999997E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 2.5455744000000001E-05, 2.4081133823999998, 3.8183616000000002E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 2.0515320000000001E-05, 1.971522252, 3.0772979999999997E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 2.3446080000000001E-05, 2.2531682879999999, 3.5169120000000003E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 1.19072592E-05, 1.2026331792, 1.78608888E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 8.9053236000000002E-06, 0.95286962519999996, 1.33579854E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 9.7678044000000006E-06, 1.0353872663999999, 1.46517066E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 1.5909840000000001E-05, 1.5512094000000001, 2.386476E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 2.93076E-05, 3.1359132000000001, 4.3961400000000002E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00010299528000000001, 3.3473465999999998, 2.0599056E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 9.4203000000000006E-05, 2.19807, 1.8840600000000001E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00010048319999999999, 2.3948496000000001, 2.0096639999999999E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.0001130436, 2.7620319599999998, 2.2608720000000001E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 8.2559509200000002E-05, 2.1190274028, 1.6511901839999998E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00013268806559999999, 2.8395246038400002, 2.6537613120000002E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00010676339999999999, 2.5587628200000001, 2.1352679999999998E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00010799431920000001, 2.79456255864, 2.1598863839999998E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00010550736, 2.606031792, 2.1101472000000001E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 9.7971119999999996E-05, 2.2631328719999999, 1.9594223999999998E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00012057984, 3.110959872, 2.4115968E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 2.7779418E-05, 1.7528812758000001, 2.7779418000000001E-06 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 9.7971119999999996E-05, 2.393761032, 1.9594223999999998E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.000125604, 3.3787476000000001, 2.5120799999999998E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00012057984, 2.946167424, 2.4115968E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.0001130436, 2.7620319599999998, 2.2608720000000001E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 4.6431611999999997E-05, 2.8601872992000001, 4.6431611999999997E-06 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 3.3494400000000002E-05, 1.87903584, 3.3494399999999999E-06 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 3.7681200000000001E-05, 2.1704371199999999, 3.7681200000000001E-06 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 1.7584560000000002E-05, 0.78075446400000004, 1.758456E-06 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 3.2531436E-06, 0.84581733599999998, 3.2531435999999998E-07 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00025492713443999999, 0.77922727427159999, 3.3990284592000002E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 9.4203000000000006E-05, 2.19807, 1.8840600000000001E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00010048319999999999, 2.3948496000000001, 2.0096639999999999E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00081642599999999998, 2.2631328719999999, 0.00026125631999999999 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00013715956800000001, 2.606031792, 0.00013715956800000001 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00010676339999999999, 2.5587628200000001, 2.1352679999999998E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00012057984, 2.946167424, 2.4115968E-05 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.001722323916, 1.7528812758000001, 5.5558836000000003E-06 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.0034666704, 2.1139153199999998, 0.0001130436 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.0, 0.495, 0.0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.0025460619945079088, 0.0, 0.0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.0, 1.0, 0.0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "N2OCEF",
                table: "Materials",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "CO2CEF",
                table: "Materials",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "CH4CEF",
                table: "Materials",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 2.4660252E-05f, 2.3328598f, 3.699038E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 2.8470253E-05f, 2.6932847f, 4.270538E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 2.5455252E-05f, 2.4081135f, 3.8183378E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 2.9726252E-05f, 2.9220934f, 4.458938E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 2.8470253E-05f, 2.6932847f, 4.270538E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 2.5455252E-05f, 2.4081135f, 3.8183378E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 2.0515252E-05f, 1.9715222f, 3.077338E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 2.3446251E-05f, 2.2531683f, 3.5169378E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 1.1907252E-05f, 1.2026331f, 1.7860379E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 8.905252E-06f, 0.95286965f, 1.3358378E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 9.767252E-06f, 1.0353873f, 1.4651378E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 1.5909252E-05f, 1.5512094f, 2.3864379E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 2.9307252E-05f, 3.1359131f, 4.396138E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.000102995255f, 3.3473465f, 2.0599378E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 9.420325E-05f, 2.19807f, 1.8840377E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00010048325f, 2.3948495f, 2.0096379E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00011304325f, 2.762032f, 2.2608378E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 8.255925E-05f, 2.1190274f, 1.6511378E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00013268825f, 2.8395245f, 2.6537378E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.000106763255f, 2.5587628f, 2.1352378E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00010799425f, 2.7945626f, 2.1598378E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00010550725f, 2.606032f, 2.1101378E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 9.797125E-05f, 2.2631328f, 1.9594378E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00012057925f, 3.1109598f, 2.4116378E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 2.7779251E-05f, 1.7528813f, 2.777378E-06f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 9.797125E-05f, 2.393761f, 1.9594378E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00012560425f, 3.3787477f, 2.5120378E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00012057925f, 2.9461675f, 2.4116378E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00011304325f, 2.762032f, 2.2608378E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 4.643125E-05f, 2.8601873f, 4.643378E-06f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 3.3494252E-05f, 1.8790358f, 3.349378E-06f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 3.7681253E-05f, 2.170437f, 3.768378E-06f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 1.7584252E-05f, 0.78075445f, 1.758378E-06f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 3.253252E-06f, 0.8458173f, 3.25378E-07f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00025492726f, 0.77922726f, 3.3990378E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 9.420325E-05f, 2.19807f, 1.8840377E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00010048325f, 2.3948495f, 2.0096379E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00081642624f, 2.2631328f, 0.00026125638f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00013715925f, 2.606032f, 0.00013715938f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.000106763255f, 2.5587628f, 2.1352378E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.00012057925f, 2.9461675f, 2.4116378E-05f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.0017223232f, 1.7528813f, 5.555378E-06f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.0034666702f, 2.1139152f, 0.00011304338f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0f, 0.495f, 0f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0.002546062f, 0f, 0f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "CH4CEF", "CO2CEF", "N2OCEF" },
                values: new object[] { 0f, 1f, 0f });
        }
    }
}
