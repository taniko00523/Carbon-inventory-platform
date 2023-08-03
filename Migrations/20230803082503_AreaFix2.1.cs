using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace 碳盤查平台.Migrations
{
    /// <inheritdoc />
    public partial class AreaFix21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Areas_Company_CompanyNo",
                table: "Areas");

            migrationBuilder.RenameColumn(
                name: "CompanyNo",
                table: "Areas",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Areas_CompanyNo",
                table: "Areas",
                newName: "IX_Areas_CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Areas_Company_CompanyId",
                table: "Areas",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Areas_Company_CompanyId",
                table: "Areas");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "Areas",
                newName: "CompanyNo");

            migrationBuilder.RenameIndex(
                name: "IX_Areas_CompanyId",
                table: "Areas",
                newName: "IX_Areas_CompanyNo");

            migrationBuilder.AddForeignKey(
                name: "FK_Areas_Company_CompanyNo",
                table: "Areas",
                column: "CompanyNo",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
