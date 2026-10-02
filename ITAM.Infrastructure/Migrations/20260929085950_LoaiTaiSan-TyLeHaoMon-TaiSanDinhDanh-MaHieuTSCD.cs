using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LoaiTaiSanTyLeHaoMonTaiSanDinhDanhMaHieuTSCD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SoHieuTSCD",
                table: "TaiSanDinhDanh",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TyLeHaoMon",
                table: "LoaiTaiSan",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "UX_TaiSanDinhDanh_SoHieuTSCD",
                table: "TaiSanDinhDanh",
                column: "SoHieuTSCD",
                unique: true,
                filter: "[SoHieuTSCD] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_TaiSanDinhDanh_SoHieuTSCD",
                table: "TaiSanDinhDanh");

            migrationBuilder.DropColumn(
                name: "SoHieuTSCD",
                table: "TaiSanDinhDanh");

            migrationBuilder.DropColumn(
                name: "TyLeHaoMon",
                table: "LoaiTaiSan");
        }
    }
}
