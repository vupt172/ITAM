using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePhieuNhapNguoiDuyet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "NguoiDuyetId",
                table: "LoNhap",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoNhap_NguoiDuyetId",
                table: "LoNhap",
                column: "NguoiDuyetId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoNhap_Users_NguoiDuyetId",
                table: "LoNhap",
                column: "NguoiDuyetId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoNhap_Users_NguoiDuyetId",
                table: "LoNhap");

            migrationBuilder.DropIndex(
                name: "IX_LoNhap_NguoiDuyetId",
                table: "LoNhap");

            migrationBuilder.DropColumn(
                name: "NguoiDuyetId",
                table: "LoNhap");
        }
    }
}
