using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DieuChuyenVatTu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VatTu_HangHoaId",
                table: "VatTu");

            migrationBuilder.CreateTable(
                name: "DieuChuyenVatTu",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoPhieu = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiTaoId = table.Column<long>(type: "bigint", nullable: false),
                    PhongBanChuyenDiId = table.Column<long>(type: "bigint", nullable: false),
                    PhongBanChuyenDenId = table.Column<long>(type: "bigint", nullable: false),
                    NguoiGiao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NguoiNhan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NguoiDuyetId = table.Column<long>(type: "bigint", nullable: true),
                    NgayDuyet = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DieuChuyenVatTu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DieuChuyenVatTu_PhongBan_PhongBanChuyenDenId",
                        column: x => x.PhongBanChuyenDenId,
                        principalTable: "PhongBan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DieuChuyenVatTu_PhongBan_PhongBanChuyenDiId",
                        column: x => x.PhongBanChuyenDiId,
                        principalTable: "PhongBan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DieuChuyenVatTu_Users_NguoiDuyetId",
                        column: x => x.NguoiDuyetId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DieuChuyenVatTu_Users_NguoiTaoId",
                        column: x => x.NguoiTaoId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichSuBienDongVatTu",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HangHoaId = table.Column<long>(type: "bigint", nullable: false),
                    ViTriTaiSanId = table.Column<long>(type: "bigint", nullable: false),
                    Loai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    TonSau = table.Column<int>(type: "int", nullable: false),
                    DieuChuyenVatTuChiTietId = table.Column<long>(type: "bigint", nullable: true),
                    NguoiDuyetId = table.Column<long>(type: "bigint", nullable: false),
                    NgayDuyet = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuBienDongVatTu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DieuChuyenVatTuChiTiet",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DieuChuyenVatTuId = table.Column<long>(type: "bigint", nullable: false),
                    HangHoaId = table.Column<long>(type: "bigint", nullable: false),
                    ViTriChuyenDiId = table.Column<long>(type: "bigint", nullable: false),
                    ViTriChuyenDenId = table.Column<long>(type: "bigint", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DieuChuyenVatTuChiTiet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DieuChuyenVatTuChiTiet_DieuChuyenVatTu_DieuChuyenVatTuId",
                        column: x => x.DieuChuyenVatTuId,
                        principalTable: "DieuChuyenVatTu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DieuChuyenVatTuChiTiet_HangHoa_HangHoaId",
                        column: x => x.HangHoaId,
                        principalTable: "HangHoa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DieuChuyenVatTuChiTiet_ViTriTaiSan_ViTriChuyenDenId",
                        column: x => x.ViTriChuyenDenId,
                        principalTable: "ViTriTaiSan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DieuChuyenVatTuChiTiet_ViTriTaiSan_ViTriChuyenDiId",
                        column: x => x.ViTriChuyenDiId,
                        principalTable: "ViTriTaiSan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VatTu_HangHoaId_ViTriTaiSanId",
                table: "VatTu",
                columns: new[] { "HangHoaId", "ViTriTaiSanId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DieuChuyenVatTu_NguoiDuyetId",
                table: "DieuChuyenVatTu",
                column: "NguoiDuyetId");

            migrationBuilder.CreateIndex(
                name: "IX_DieuChuyenVatTu_NguoiTaoId",
                table: "DieuChuyenVatTu",
                column: "NguoiTaoId");

            migrationBuilder.CreateIndex(
                name: "IX_DieuChuyenVatTu_PhongBanChuyenDenId",
                table: "DieuChuyenVatTu",
                column: "PhongBanChuyenDenId");

            migrationBuilder.CreateIndex(
                name: "IX_DieuChuyenVatTu_PhongBanChuyenDiId",
                table: "DieuChuyenVatTu",
                column: "PhongBanChuyenDiId");

            migrationBuilder.CreateIndex(
                name: "IX_DieuChuyenVatTu_SoPhieu",
                table: "DieuChuyenVatTu",
                column: "SoPhieu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DieuChuyenVatTuChiTiet_DieuChuyenVatTuId",
                table: "DieuChuyenVatTuChiTiet",
                column: "DieuChuyenVatTuId");

            migrationBuilder.CreateIndex(
                name: "IX_DieuChuyenVatTuChiTiet_HangHoaId",
                table: "DieuChuyenVatTuChiTiet",
                column: "HangHoaId");

            migrationBuilder.CreateIndex(
                name: "IX_DieuChuyenVatTuChiTiet_ViTriChuyenDenId",
                table: "DieuChuyenVatTuChiTiet",
                column: "ViTriChuyenDenId");

            migrationBuilder.CreateIndex(
                name: "IX_DieuChuyenVatTuChiTiet_ViTriChuyenDiId",
                table: "DieuChuyenVatTuChiTiet",
                column: "ViTriChuyenDiId");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuBienDongVatTu_HangHoaId_ViTriTaiSanId_NgayDuyet",
                table: "LichSuBienDongVatTu",
                columns: new[] { "HangHoaId", "ViTriTaiSanId", "NgayDuyet" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DieuChuyenVatTuChiTiet");

            migrationBuilder.DropTable(
                name: "LichSuBienDongVatTu");

            migrationBuilder.DropTable(
                name: "DieuChuyenVatTu");

            migrationBuilder.DropIndex(
                name: "IX_VatTu_HangHoaId_ViTriTaiSanId",
                table: "VatTu");

            migrationBuilder.CreateIndex(
                name: "IX_VatTu_HangHoaId",
                table: "VatTu",
                column: "HangHoaId",
                unique: true);
        }
    }
}
