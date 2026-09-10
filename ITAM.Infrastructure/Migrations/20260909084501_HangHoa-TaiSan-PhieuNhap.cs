using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HangHoaTaiSanPhieuNhap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HangHoa",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HangSanXuat = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DMTaiSanId = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HangHoa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HangHoa_DMTaiSan_DMTaiSanId",
                        column: x => x.DMTaiSanId,
                        principalTable: "DMTaiSan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoNhap",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoLo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayNhap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NhaCungCapId = table.Column<long>(type: "bigint", nullable: false),
                    NguoiLapPhieuId = table.Column<long>(type: "bigint", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoNhap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoNhap_NhaCungCap_NhaCungCapId",
                        column: x => x.NhaCungCapId,
                        principalTable: "NhaCungCap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoNhap_Users_NguoiLapPhieuId",
                        column: x => x.NguoiLapPhieuId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VatTu",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HangHoaId = table.Column<long>(type: "bigint", nullable: false),
                    DonViTinh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SoLuongTon = table.Column<int>(type: "int", nullable: false),
                    SoLuongChoMuon = table.Column<int>(type: "int", nullable: false),
                    ViTriTaiSanId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VatTu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VatTu_HangHoa_HangHoaId",
                        column: x => x.HangHoaId,
                        principalTable: "HangHoa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VatTu_ViTriTaiSan_ViTriTaiSanId",
                        column: x => x.ViTriTaiSanId,
                        principalTable: "ViTriTaiSan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoNhapChiTiet",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoLo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LoNhapId = table.Column<long>(type: "bigint", nullable: false),
                    HangHoaId = table.Column<long>(type: "bigint", nullable: false),
                    TenVatPham = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SoLuongNhap = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoaiTaiSanId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoNhapChiTiet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoNhapChiTiet_HangHoa_HangHoaId",
                        column: x => x.HangHoaId,
                        principalTable: "HangHoa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoNhapChiTiet_LoNhap_LoNhapId",
                        column: x => x.LoNhapId,
                        principalTable: "LoNhap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoNhapChiTiet_LoaiTaiSan_LoaiTaiSanId",
                        column: x => x.LoaiTaiSanId,
                        principalTable: "LoaiTaiSan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaiSanDinhDanh",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HangHoaId = table.Column<long>(type: "bigint", nullable: false),
                    Serial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NamSuDung = table.Column<int>(type: "int", nullable: true),
                    GiaNhap = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoaiTaiSanId = table.Column<long>(type: "bigint", nullable: false),
                    TrangThaiTaiSan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LoNhapChiTietId = table.Column<long>(type: "bigint", nullable: false),
                    ViTriTaiSanId = table.Column<long>(type: "bigint", nullable: false),
                    DangChoMuon = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiSanDinhDanh", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaiSanDinhDanh_HangHoa_HangHoaId",
                        column: x => x.HangHoaId,
                        principalTable: "HangHoa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaiSanDinhDanh_LoNhapChiTiet_LoNhapChiTietId",
                        column: x => x.LoNhapChiTietId,
                        principalTable: "LoNhapChiTiet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaiSanDinhDanh_LoaiTaiSan_LoaiTaiSanId",
                        column: x => x.LoaiTaiSanId,
                        principalTable: "LoaiTaiSan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaiSanDinhDanh_ViTriTaiSan_ViTriTaiSanId",
                        column: x => x.ViTriTaiSanId,
                        principalTable: "ViTriTaiSan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HangHoa_Code",
                table: "HangHoa",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HangHoa_DMTaiSanId",
                table: "HangHoa",
                column: "DMTaiSanId");

            migrationBuilder.CreateIndex(
                name: "IX_LoNhap_NguoiLapPhieuId",
                table: "LoNhap",
                column: "NguoiLapPhieuId");

            migrationBuilder.CreateIndex(
                name: "IX_LoNhap_NhaCungCapId",
                table: "LoNhap",
                column: "NhaCungCapId");

            migrationBuilder.CreateIndex(
                name: "IX_LoNhap_SoLo",
                table: "LoNhap",
                column: "SoLo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoNhapChiTiet_HangHoaId",
                table: "LoNhapChiTiet",
                column: "HangHoaId");

            migrationBuilder.CreateIndex(
                name: "IX_LoNhapChiTiet_LoaiTaiSanId",
                table: "LoNhapChiTiet",
                column: "LoaiTaiSanId");

            migrationBuilder.CreateIndex(
                name: "IX_LoNhapChiTiet_LoNhapId",
                table: "LoNhapChiTiet",
                column: "LoNhapId");

            migrationBuilder.CreateIndex(
                name: "IX_LoNhapChiTiet_SoLo",
                table: "LoNhapChiTiet",
                column: "SoLo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaiSanDinhDanh_Code",
                table: "TaiSanDinhDanh",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaiSanDinhDanh_HangHoaId",
                table: "TaiSanDinhDanh",
                column: "HangHoaId");

            migrationBuilder.CreateIndex(
                name: "IX_TaiSanDinhDanh_LoaiTaiSanId",
                table: "TaiSanDinhDanh",
                column: "LoaiTaiSanId");

            migrationBuilder.CreateIndex(
                name: "IX_TaiSanDinhDanh_LoNhapChiTietId",
                table: "TaiSanDinhDanh",
                column: "LoNhapChiTietId");

            migrationBuilder.CreateIndex(
                name: "IX_TaiSanDinhDanh_Serial",
                table: "TaiSanDinhDanh",
                column: "Serial");

            migrationBuilder.CreateIndex(
                name: "IX_TaiSanDinhDanh_ViTriTaiSanId",
                table: "TaiSanDinhDanh",
                column: "ViTriTaiSanId");

            migrationBuilder.CreateIndex(
                name: "IX_VatTu_Code",
                table: "VatTu",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VatTu_HangHoaId",
                table: "VatTu",
                column: "HangHoaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VatTu_ViTriTaiSanId",
                table: "VatTu",
                column: "ViTriTaiSanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaiSanDinhDanh");

            migrationBuilder.DropTable(
                name: "VatTu");

            migrationBuilder.DropTable(
                name: "LoNhapChiTiet");

            migrationBuilder.DropTable(
                name: "HangHoa");

            migrationBuilder.DropTable(
                name: "LoNhap");
        }
    }
}
