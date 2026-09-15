using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaiSanDinhDanhVatTU : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoLuongChoMuon",
                table: "VatTu");

            migrationBuilder.DropColumn(
                name: "DangChoMuon",
                table: "TaiSanDinhDanh");

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "VatTu",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "TaiSanDinhDanh",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "VatTu");

            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "TaiSanDinhDanh");

            migrationBuilder.AddColumn<int>(
                name: "SoLuongChoMuon",
                table: "VatTu",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "DangChoMuon",
                table: "TaiSanDinhDanh",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
