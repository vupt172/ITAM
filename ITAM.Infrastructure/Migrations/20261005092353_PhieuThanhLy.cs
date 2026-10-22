using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PhieuThanhLy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PhieuThanhLyChiTietId",
                table: "LichSuDieuChuyenTaiSan",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PhieuThanhLy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoPhieu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PhongBanId = table.Column<long>(type: "bigint", nullable: false),
                    LyDo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
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
                    table.PrimaryKey("PK_PhieuThanhLy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhieuThanhLy_PhongBan_PhongBanId",
                        column: x => x.PhongBanId,
                        principalTable: "PhongBan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuThanhLy_Users_NguoiDuyetId",
                        column: x => x.NguoiDuyetId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuThanhLy_Users_NguoiTaoId",
                        column: x => x.NguoiTaoId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhieuThanhLyChiTiet",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhieuThanhLyId = table.Column<long>(type: "bigint", nullable: false),
                    TaiSanDinhDanhId = table.Column<long>(type: "bigint", nullable: false),
                    ViTriTruocId = table.Column<long>(type: "bigint", nullable: false),
                    GiaThanhLy = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuThanhLyChiTiet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhieuThanhLyChiTiet_PhieuThanhLy_PhieuThanhLyId",
                        column: x => x.PhieuThanhLyId,
                        principalTable: "PhieuThanhLy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhieuThanhLyChiTiet_TaiSanDinhDanh_TaiSanDinhDanhId",
                        column: x => x.TaiSanDinhDanhId,
                        principalTable: "TaiSanDinhDanh",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuThanhLyChiTiet_ViTriTaiSan_ViTriTruocId",
                        column: x => x.ViTriTruocId,
                        principalTable: "ViTriTaiSan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LichSuDieuChuyenTaiSan_PhieuThanhLyChiTietId",
                table: "LichSuDieuChuyenTaiSan",
                column: "PhieuThanhLyChiTietId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThanhLy_NguoiDuyetId",
                table: "PhieuThanhLy",
                column: "NguoiDuyetId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThanhLy_NguoiTaoId",
                table: "PhieuThanhLy",
                column: "NguoiTaoId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThanhLy_PhongBanId",
                table: "PhieuThanhLy",
                column: "PhongBanId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThanhLy_SoPhieu",
                table: "PhieuThanhLy",
                column: "SoPhieu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThanhLyChiTiet_PhieuThanhLyId_TaiSanDinhDanhId",
                table: "PhieuThanhLyChiTiet",
                columns: new[] { "PhieuThanhLyId", "TaiSanDinhDanhId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThanhLyChiTiet_TaiSanDinhDanhId",
                table: "PhieuThanhLyChiTiet",
                column: "TaiSanDinhDanhId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThanhLyChiTiet_ViTriTruocId",
                table: "PhieuThanhLyChiTiet",
                column: "ViTriTruocId");

            migrationBuilder.AddForeignKey(
                name: "FK_LichSuDieuChuyenTaiSan_PhieuThanhLyChiTiet_PhieuThanhLyChiTietId",
                table: "LichSuDieuChuyenTaiSan",
                column: "PhieuThanhLyChiTietId",
                principalTable: "PhieuThanhLyChiTiet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichSuDieuChuyenTaiSan_PhieuThanhLyChiTiet_PhieuThanhLyChiTietId",
                table: "LichSuDieuChuyenTaiSan");

            migrationBuilder.DropTable(
                name: "PhieuThanhLyChiTiet");

            migrationBuilder.DropTable(
                name: "PhieuThanhLy");

            migrationBuilder.DropIndex(
                name: "IX_LichSuDieuChuyenTaiSan_PhieuThanhLyChiTietId",
                table: "LichSuDieuChuyenTaiSan");

            migrationBuilder.DropColumn(
                name: "PhieuThanhLyChiTietId",
                table: "LichSuDieuChuyenTaiSan");
        }
    }
}
