using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDMTaiSanHangHoaRequireSerial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequireSerial",
                table: "DMTaiSan");

            migrationBuilder.AddColumn<bool>(
                name: "RequireSerial",
                table: "HangHoa",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequireSerial",
                table: "HangHoa");

            migrationBuilder.AddColumn<bool>(
                name: "RequireSerial",
                table: "DMTaiSan",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
