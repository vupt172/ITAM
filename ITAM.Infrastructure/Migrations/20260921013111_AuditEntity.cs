using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AuditEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ViTriTaiSan",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "ViTriTaiSan",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ViTriTaiSan",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "ViTriTaiSan",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "VatTu",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "VatTu",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "VatTu",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "VatTu",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "Users",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "Users",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TaiSanDinhDanh",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "TaiSanDinhDanh",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "TaiSanDinhDanh",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "TaiSanDinhDanh",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "PhongBan",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "PhongBan",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "PhongBan",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "PhongBan",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "NhaCungCap",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "NhaCungCap",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "NhaCungCap",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "NhaCungCap",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "LoNhapChiTiet",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "LoNhapChiTiet",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "LoNhapChiTiet",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "LoNhapChiTiet",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "LoNhap",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "LoNhap",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "LoNhap",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "LoNhap",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "LoaiTaiSan",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "LoaiTaiSan",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "LoaiTaiSan",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "LoaiTaiSan",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "HangHoa",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "HangHoa",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "HangHoa",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "HangHoa",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DMTaiSan",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "DMTaiSan",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "DMTaiSan",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "DMTaiSan",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DieuChuyenChiTiet",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "DieuChuyenChiTiet",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "DieuChuyenChiTiet",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "DieuChuyenChiTiet",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DieuChuyen",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "DieuChuyen",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "DieuChuyen",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "DieuChuyen",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ViTriTaiSan");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ViTriTaiSan");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ViTriTaiSan");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ViTriTaiSan");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "VatTu");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "VatTu");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "VatTu");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "VatTu");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TaiSanDinhDanh");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TaiSanDinhDanh");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "TaiSanDinhDanh");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "TaiSanDinhDanh");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "PhongBan");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "PhongBan");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "PhongBan");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "PhongBan");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "NhaCungCap");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "NhaCungCap");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "NhaCungCap");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "NhaCungCap");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "LoNhapChiTiet");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "LoNhapChiTiet");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "LoNhapChiTiet");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "LoNhapChiTiet");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "LoNhap");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "LoNhap");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "LoNhap");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "LoNhap");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "LoaiTaiSan");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "LoaiTaiSan");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "LoaiTaiSan");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "LoaiTaiSan");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "HangHoa");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "HangHoa");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "HangHoa");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "HangHoa");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DMTaiSan");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "DMTaiSan");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "DMTaiSan");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "DMTaiSan");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DieuChuyenChiTiet");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "DieuChuyenChiTiet");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "DieuChuyenChiTiet");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "DieuChuyenChiTiet");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DieuChuyen");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "DieuChuyen");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "DieuChuyen");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "DieuChuyen");
        }
    }
}
