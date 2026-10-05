using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PhieuThatLacPhieuThatLacChiTiet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PhieuThatLacChiTietId",
                table: "LichSuDieuChuyenTaiSan",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PhieuThatLac",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoPhieu = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PhongBanId = table.Column<long>(type: "bigint", nullable: false),
                    NgayPhatHien = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiPhatHien = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LyDo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    NguoiTaoId = table.Column<long>(type: "bigint", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NguoiDuyetId = table.Column<long>(type: "bigint", nullable: true),
                    NgayDuyet = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuThatLac", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhieuThatLac_PhongBan_PhongBanId",
                        column: x => x.PhongBanId,
                        principalTable: "PhongBan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuThatLac_Users_NguoiDuyetId",
                        column: x => x.NguoiDuyetId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuThatLac_Users_NguoiTaoId",
                        column: x => x.NguoiTaoId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhieuThatLacChiTiet",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhieuThatLacId = table.Column<long>(type: "bigint", nullable: false),
                    TaiSanDinhDanhId = table.Column<long>(type: "bigint", nullable: false),
                    ViTriTruocId = table.Column<long>(type: "bigint", nullable: false),
                    TrangThaiTruoc = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuThatLacChiTiet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhieuThatLacChiTiet_PhieuThatLac_PhieuThatLacId",
                        column: x => x.PhieuThatLacId,
                        principalTable: "PhieuThatLac",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhieuThatLacChiTiet_TaiSanDinhDanh_TaiSanDinhDanhId",
                        column: x => x.TaiSanDinhDanhId,
                        principalTable: "TaiSanDinhDanh",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuThatLacChiTiet_ViTriTaiSan_ViTriTruocId",
                        column: x => x.ViTriTruocId,
                        principalTable: "ViTriTaiSan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LichSuDieuChuyenTaiSan_PhieuThatLacChiTietId",
                table: "LichSuDieuChuyenTaiSan",
                column: "PhieuThatLacChiTietId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThatLac_NguoiDuyetId",
                table: "PhieuThatLac",
                column: "NguoiDuyetId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThatLac_NguoiTaoId",
                table: "PhieuThatLac",
                column: "NguoiTaoId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThatLac_PhongBanId",
                table: "PhieuThatLac",
                column: "PhongBanId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThatLac_SoPhieu",
                table: "PhieuThatLac",
                column: "SoPhieu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThatLacChiTiet_PhieuThatLacId_TaiSanDinhDanhId",
                table: "PhieuThatLacChiTiet",
                columns: new[] { "PhieuThatLacId", "TaiSanDinhDanhId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThatLacChiTiet_TaiSanDinhDanhId",
                table: "PhieuThatLacChiTiet",
                column: "TaiSanDinhDanhId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThatLacChiTiet_ViTriTruocId",
                table: "PhieuThatLacChiTiet",
                column: "ViTriTruocId");

            migrationBuilder.AddForeignKey(
                name: "FK_LichSuDieuChuyenTaiSan_PhieuThatLacChiTiet_PhieuThatLacChiTietId",
                table: "LichSuDieuChuyenTaiSan",
                column: "PhieuThatLacChiTietId",
                principalTable: "PhieuThatLacChiTiet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichSuDieuChuyenTaiSan_PhieuThatLacChiTiet_PhieuThatLacChiTietId",
                table: "LichSuDieuChuyenTaiSan");

            migrationBuilder.DropTable(
                name: "PhieuThatLacChiTiet");

            migrationBuilder.DropTable(
                name: "PhieuThatLac");

            migrationBuilder.DropIndex(
                name: "IX_LichSuDieuChuyenTaiSan_PhieuThatLacChiTietId",
                table: "LichSuDieuChuyenTaiSan");

            migrationBuilder.DropColumn(
                name: "PhieuThatLacChiTietId",
                table: "LichSuDieuChuyenTaiSan");
        }
    }
}
