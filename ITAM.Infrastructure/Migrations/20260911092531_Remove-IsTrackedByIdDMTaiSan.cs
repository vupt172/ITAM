using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsTrackedByIdDMTaiSan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTrackedById",
                table: "DMTaiSan");

            migrationBuilder.AlterColumn<long>(
                name: "LoaiTaiSanId",
                table: "LoNhapChiTiet",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "LoaiTaiSanId",
                table: "LoNhapChiTiet",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<bool>(
                name: "IsTrackedById",
                table: "DMTaiSan",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
