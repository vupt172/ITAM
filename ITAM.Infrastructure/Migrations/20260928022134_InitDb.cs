using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DMTaiSan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMTaiSan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoaiTaiSan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MinValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiTaiSan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NhaCungCap",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhaCungCap", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhongBan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhongBan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HangHoa",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HangSanXuat = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DMTaiSanId = table.Column<long>(type: "bigint", nullable: false),
                    DonViTinh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RequireSerial = table.Column<bool>(type: "bit", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
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
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PhongBanId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_PhongBan_PhongBanId",
                        column: x => x.PhongBanId,
                        principalTable: "PhongBan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ViTriTaiSan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhongBanId = table.Column<long>(type: "bigint", nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViTriTaiSan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViTriTaiSan_PhongBan_PhongBanId",
                        column: x => x.PhongBanId,
                        principalTable: "PhongBan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleFeatures",
                columns: table => new
                {
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    FeatureId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleFeatures", x => new { x.RoleId, x.FeatureId });
                    table.ForeignKey(
                        name: "FK_RoleFeatures_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleFeatures_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
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
                name: "LoNhap",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoLo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayNhap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NhaCungCapId = table.Column<long>(type: "bigint", nullable: false),
                    NguoiGiao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaHoaDon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NguoiLapPhieuId = table.Column<long>(type: "bigint", nullable: false),
                    NguoiDuyetId = table.Column<long>(type: "bigint", nullable: true),
                    NgayDuyet = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
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
                        name: "FK_LoNhap_Users_NguoiDuyetId",
                        column: x => x.NguoiDuyetId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LoNhap_Users_NguoiLapPhieuId",
                        column: x => x.NguoiLapPhieuId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserPhongBans",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    PhongBanId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPhongBans", x => new { x.UserId, x.PhongBanId });
                    table.ForeignKey(
                        name: "FK_UserPhongBans_PhongBan_PhongBanId",
                        column: x => x.PhongBanId,
                        principalTable: "PhongBan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPhongBans_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VatTu",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HangHoaId = table.Column<long>(type: "bigint", nullable: false),
                    DonViTinh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SoLuongTon = table.Column<int>(type: "int", nullable: false),
                    ViTriTaiSanId = table.Column<long>(type: "bigint", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
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
                    LoaiTaiSanId = table.Column<long>(type: "bigint", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
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
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
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
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
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
                name: "IX_Features_Code",
                table: "Features",
                column: "Code",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_LoNhap_NguoiDuyetId",
                table: "LoNhap",
                column: "NguoiDuyetId");

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
                name: "IX_RoleFeatures_FeatureId",
                table: "RoleFeatures",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Code",
                table: "Roles",
                column: "Code",
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
                name: "IX_UserPhongBans_PhongBanId",
                table: "UserPhongBans",
                column: "PhongBanId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhongBanId",
                table: "Users",
                column: "PhongBanId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
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

            migrationBuilder.CreateIndex(
                name: "IX_ViTriTaiSan_PhongBanId",
                table: "ViTriTaiSan",
                column: "PhongBanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LichSuDieuChuyenTaiSan");

            migrationBuilder.DropTable(
                name: "RoleFeatures");

            migrationBuilder.DropTable(
                name: "UserPhongBans");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "VatTu");

            migrationBuilder.DropTable(
                name: "DieuChuyenChiTiet");

            migrationBuilder.DropTable(
                name: "Features");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "DieuChuyen");

            migrationBuilder.DropTable(
                name: "TaiSanDinhDanh");

            migrationBuilder.DropTable(
                name: "LoNhapChiTiet");

            migrationBuilder.DropTable(
                name: "ViTriTaiSan");

            migrationBuilder.DropTable(
                name: "HangHoa");

            migrationBuilder.DropTable(
                name: "LoNhap");

            migrationBuilder.DropTable(
                name: "LoaiTaiSan");

            migrationBuilder.DropTable(
                name: "DMTaiSan");

            migrationBuilder.DropTable(
                name: "NhaCungCap");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "PhongBan");
        }
    }
}
