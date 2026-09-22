using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ViTriTaiSanIsSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSystem",
                table: "ViTriTaiSan",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "DieuChuyen",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoPhieu = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiTaoId = table.Column<long>(type: "bigint", nullable: false),
                    PhongBanChuyenDiId = table.Column<long>(type: "bigint", nullable: false),
                    PhongBanChuyenDenId = table.Column<long>(type: "bigint", nullable: false),
                    NguoiGiao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NguoiNhan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NguoiDuyetId = table.Column<long>(type: "bigint", nullable: true),
                    NgayDuyet = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DieuChuyen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DieuChuyen_PhongBan_PhongBanChuyenDenId",
                        column: x => x.PhongBanChuyenDenId,
                        principalTable: "PhongBan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DieuChuyen_PhongBan_PhongBanChuyenDiId",
                        column: x => x.PhongBanChuyenDiId,
                        principalTable: "PhongBan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DieuChuyen_Users_NguoiDuyetId",
                        column: x => x.NguoiDuyetId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DieuChuyen_Users_NguoiTaoId",
                        column: x => x.NguoiTaoId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DieuChuyenChiTiet",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DieuChuyenId = table.Column<long>(type: "bigint", nullable: false),
                    TaiSanDinhDanhId = table.Column<long>(type: "bigint", nullable: false),
                    ViTriChuyenDiId = table.Column<long>(type: "bigint", nullable: false),
                    ViTriChuyenDenId = table.Column<long>(type: "bigint", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DieuChuyenChiTiet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DieuChuyenChiTiet_DieuChuyen_DieuChuyenId",
                        column: x => x.DieuChuyenId,
                        principalTable: "DieuChuyen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DieuChuyenChiTiet_TaiSanDinhDanh_TaiSanDinhDanhId",
                        column: x => x.TaiSanDinhDanhId,
                        principalTable: "TaiSanDinhDanh",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DieuChuyenChiTiet_ViTriTaiSan_ViTriChuyenDenId",
                        column: x => x.ViTriChuyenDenId,
                        principalTable: "ViTriTaiSan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DieuChuyenChiTiet_ViTriTaiSan_ViTriChuyenDiId",
                        column: x => x.ViTriChuyenDiId,
                        principalTable: "ViTriTaiSan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LichSuDieuChuyenTaiSan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaiSanDinhDanhId = table.Column<long>(type: "bigint", nullable: false),
                    ViTriChuyenDiId = table.Column<long>(type: "bigint", nullable: true),
                    ViTriChuyenDenId = table.Column<long>(type: "bigint", nullable: false),
                    NgayDuyet = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiDuyetId = table.Column<long>(type: "bigint", nullable: false),
                    DieuChuyenChiTietId = table.Column<long>(type: "bigint", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuDieuChuyenTaiSan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LichSuDieuChuyenTaiSan_DieuChuyenChiTiet_DieuChuyenChiTietId",
                        column: x => x.DieuChuyenChiTietId,
                        principalTable: "DieuChuyenChiTiet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichSuDieuChuyenTaiSan_TaiSanDinhDanh_TaiSanDinhDanhId",
                        column: x => x.TaiSanDinhDanhId,
                        principalTable: "TaiSanDinhDanh",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichSuDieuChuyenTaiSan_Users_NguoiDuyetId",
                        column: x => x.NguoiDuyetId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichSuDieuChuyenTaiSan_ViTriTaiSan_ViTriChuyenDenId",
                        column: x => x.ViTriChuyenDenId,
                        principalTable: "ViTriTaiSan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichSuDieuChuyenTaiSan_ViTriTaiSan_ViTriChuyenDiId",
                        column: x => x.ViTriChuyenDiId,
                        principalTable: "ViTriTaiSan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DieuChuyen_NguoiDuyetId",
                table: "DieuChuyen",
                column: "NguoiDuyetId");

            migrationBuilder.CreateIndex(
                name: "IX_DieuChuyen_NguoiTaoId",
                table: "DieuChuyen",
                column: "NguoiTaoId");

            migrationBuilder.CreateIndex(
                name: "IX_DieuChuyen_PhongBanChuyenDenId",
                table: "DieuChuyen",
                column: "PhongBanChuyenDenId");

            migrationBuilder.CreateIndex(
                name: "IX_DieuChuyen_PhongBanChuyenDiId",
                table: "DieuChuyen",
                column: "PhongBanChuyenDiId");

            migrationBuilder.CreateIndex(
                name: "IX_DieuChuyen_SoPhieu",
                table: "DieuChuyen",
                column: "SoPhieu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DieuChuyenChiTiet_DieuChuyenId",
                table: "DieuChuyenChiTiet",
                column: "DieuChuyenId");

            migrationBuilder.CreateIndex(
                name: "IX_DieuChuyenChiTiet_TaiSanDinhDanhId",
                table: "DieuChuyenChiTiet",
                column: "TaiSanDinhDanhId");

            migrationBuilder.CreateIndex(
                name: "IX_DieuChuyenChiTiet_ViTriChuyenDenId",
                table: "DieuChuyenChiTiet",
                column: "ViTriChuyenDenId");

            migrationBuilder.CreateIndex(
                name: "IX_DieuChuyenChiTiet_ViTriChuyenDiId",
                table: "DieuChuyenChiTiet",
                column: "ViTriChuyenDiId");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuDieuChuyenTaiSan_DieuChuyenChiTietId",
                table: "LichSuDieuChuyenTaiSan",
                column: "DieuChuyenChiTietId");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuDieuChuyenTaiSan_NguoiDuyetId",
                table: "LichSuDieuChuyenTaiSan",
                column: "NguoiDuyetId");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuDieuChuyenTaiSan_TaiSanDinhDanhId",
                table: "LichSuDieuChuyenTaiSan",
                column: "TaiSanDinhDanhId");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuDieuChuyenTaiSan_ViTriChuyenDenId",
                table: "LichSuDieuChuyenTaiSan",
                column: "ViTriChuyenDenId");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuDieuChuyenTaiSan_ViTriChuyenDiId",
                table: "LichSuDieuChuyenTaiSan",
                column: "ViTriChuyenDiId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LichSuDieuChuyenTaiSan");

            migrationBuilder.DropTable(
                name: "DieuChuyenChiTiet");

            migrationBuilder.DropTable(
                name: "DieuChuyen");

            migrationBuilder.DropColumn(
                name: "IsSystem",
                table: "ViTriTaiSan");
        }
    }
}
