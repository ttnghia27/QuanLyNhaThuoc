using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyNhaThuoc.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KHACHHANG",
                columns: table => new
                {
                    MaKH = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenKH = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SDTKH = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    DiaChiKH = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KHACHHANG", x => x.MaKH);
                });

            migrationBuilder.CreateTable(
                name: "LOAITHUOC",
                columns: table => new
                {
                    MaLoai = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenLoai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOAITHUOC", x => x.MaLoai);
                });

            migrationBuilder.CreateTable(
                name: "NGUOIDUNG",
                columns: table => new
                {
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NGUOIDUNG", x => x.TenDangNhap);
                });

            migrationBuilder.CreateTable(
                name: "NHACC",
                columns: table => new
                {
                    MaNhaCC = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenNhaCC = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SDTNhaCC = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NHACC", x => x.MaNhaCC);
                });

            migrationBuilder.CreateTable(
                name: "HOADON",
                columns: table => new
                {
                    MaHD = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NgayLapHD = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    MaKH = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HOADON", x => x.MaHD);
                    table.ForeignKey(
                        name: "FK_HOADON_KHACHHANG_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KHACHHANG",
                        principalColumn: "MaKH",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THUOC",
                columns: table => new
                {
                    MaThuoc = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenThuoc = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CongDung = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DonGiaBan = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    SoLuongTonKho = table.Column<int>(type: "int", nullable: false),
                    MaLoai = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THUOC", x => x.MaThuoc);
                    table.ForeignKey(
                        name: "FK_THUOC_LOAITHUOC_MaLoai",
                        column: x => x.MaLoai,
                        principalTable: "LOAITHUOC",
                        principalColumn: "MaLoai",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PHIEUNHAP",
                columns: table => new
                {
                    MaPhieuNhap = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NgayNhap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    MaNhaCC = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PHIEUNHAP", x => x.MaPhieuNhap);
                    table.ForeignKey(
                        name: "FK_PHIEUNHAP_NHACC_MaNhaCC",
                        column: x => x.MaNhaCC,
                        principalTable: "NHACC",
                        principalColumn: "MaNhaCC",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CHITIETPHIEUNHAP",
                columns: table => new
                {
                    MaPhieuNhap = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    MaThuoc = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    SoLuongNhap = table.Column<int>(type: "int", nullable: false),
                    DonGiaNhap = table.Column<decimal>(type: "decimal(18,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHITIETPHIEUNHAP", x => new { x.MaPhieuNhap, x.MaThuoc });
                    table.ForeignKey(
                        name: "FK_CHITIETPHIEUNHAP_PHIEUNHAP_MaPhieuNhap",
                        column: x => x.MaPhieuNhap,
                        principalTable: "PHIEUNHAP",
                        principalColumn: "MaPhieuNhap",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CHITIETPHIEUNHAP_THUOC_MaThuoc",
                        column: x => x.MaThuoc,
                        principalTable: "THUOC",
                        principalColumn: "MaThuoc",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LOTHUC",
                columns: table => new
                {
                    MaLo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    MaThuoc = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    MaPhieuNhap = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    NgayNhapKho = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HanSuDung = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoLuongNhap = table.Column<int>(type: "int", nullable: false),
                    SoLuongCon = table.Column<int>(type: "int", nullable: false),
                    DonGiaNhap = table.Column<decimal>(type: "decimal(18,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOTHUC", x => x.MaLo);
                    table.ForeignKey(
                        name: "FK_LOTHUC_PHIEUNHAP_MaPhieuNhap",
                        column: x => x.MaPhieuNhap,
                        principalTable: "PHIEUNHAP",
                        principalColumn: "MaPhieuNhap",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LOTHUC_THUOC_MaThuoc",
                        column: x => x.MaThuoc,
                        principalTable: "THUOC",
                        principalColumn: "MaThuoc",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CHITIETHOADON",
                columns: table => new
                {
                    MaHD = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    MaLo = table.Column<string>(type: "nvarchar(15)", nullable: false),
                    SoLuongBan = table.Column<int>(type: "int", nullable: false),
                    GiaBan = table.Column<decimal>(type: "decimal(18,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHITIETHOADON", x => new { x.MaHD, x.MaLo });
                    table.ForeignKey(
                        name: "FK_CHITIETHOADON_HOADON_MaHD",
                        column: x => x.MaHD,
                        principalTable: "HOADON",
                        principalColumn: "MaHD",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CHITIETHOADON_LOTHUC_MaLo",
                        column: x => x.MaLo,
                        principalTable: "LOTHUC",
                        principalColumn: "MaLo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CHITIETHOADON_MaLo",
                table: "CHITIETHOADON",
                column: "MaLo");

            migrationBuilder.CreateIndex(
                name: "IX_CHITIETPHIEUNHAP_MaThuoc",
                table: "CHITIETPHIEUNHAP",
                column: "MaThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_HOADON_MaKH",
                table: "HOADON",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_LOTHUC_MaPhieuNhap",
                table: "LOTHUC",
                column: "MaPhieuNhap");

            migrationBuilder.CreateIndex(
                name: "IX_LOTHUC_MaThuoc",
                table: "LOTHUC",
                column: "MaThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_PHIEUNHAP_MaNhaCC",
                table: "PHIEUNHAP",
                column: "MaNhaCC");

            migrationBuilder.CreateIndex(
                name: "IX_THUOC_MaLoai",
                table: "THUOC",
                column: "MaLoai");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CHITIETHOADON");

            migrationBuilder.DropTable(
                name: "CHITIETPHIEUNHAP");

            migrationBuilder.DropTable(
                name: "NGUOIDUNG");

            migrationBuilder.DropTable(
                name: "HOADON");

            migrationBuilder.DropTable(
                name: "LOTHUC");

            migrationBuilder.DropTable(
                name: "KHACHHANG");

            migrationBuilder.DropTable(
                name: "PHIEUNHAP");

            migrationBuilder.DropTable(
                name: "THUOC");

            migrationBuilder.DropTable(
                name: "NHACC");

            migrationBuilder.DropTable(
                name: "LOAITHUOC");
        }
    }
}
