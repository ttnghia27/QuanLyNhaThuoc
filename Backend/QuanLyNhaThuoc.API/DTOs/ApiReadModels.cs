namespace QuanLyNhaThuoc.API.DTOs;

public sealed record LoaiThuocListItem(string MaLoai, string TenLoai, int SoLuongThuoc);

public sealed record ThuocListItem(
    string MaThuoc,
    string TenThuoc,
    string? CongDung,
    decimal DonGiaBan,
    int SoLuongTonKho,
    string MaLoai,
    string TenLoai);

public sealed record PhieuNhapListItem(
    string MaPhieuNhap,
    DateTime NgayNhap,
    decimal TongTien,
    string MaNhaCC,
    string TenNhaCC);

public sealed record PhieuNhapLoItem(
    string MaLo,
    string MaThuoc,
    string TenThuoc,
    DateTime HanSuDung,
    int SoLuongNhap,
    int SoLuongCon,
    decimal DonGiaNhap,
    decimal ThanhTien);

public sealed record PhieuNhapDetail(
    string MaPhieuNhap,
    DateTime NgayNhap,
    decimal TongTien,
    string MaNhaCC,
    string TenNhaCC,
    IEnumerable<PhieuNhapLoItem> ChiTietLo);

public sealed record HoaDonListItem(
    string MaHD,
    DateTime NgayLapHD,
    decimal TongTien,
    string MaKH,
    string TenKH);

public sealed record HoaDonChiTietItem(
    string MaLo,
    string MaThuoc,
    string TenThuoc,
    DateTime HanSuDung,
    int SoLuongBan,
    decimal GiaBan,
    decimal ThanhTien);

public sealed record HoaDonDetail(
    string MaHD,
    DateTime NgayLapHD,
    decimal TongTien,
    string MaKH,
    string TenKH,
    IEnumerable<HoaDonChiTietItem> ChiTiet);

public sealed record LoThuocListItem(
    string MaLo,
    string MaThuoc,
    string TenThuoc,
    string MaPhieuNhap,
    DateTime NgayNhapKho,
    DateTime HanSuDung,
    int SoLuongNhap,
    int SoLuongCon,
    decimal DonGiaNhap,
    bool DaHetHan);

public sealed class CanhBaoSapHetHanItem
{
    public string MaLo { get; set; } = null!;
    public string MaThuoc { get; set; } = null!;
    public string TenThuoc { get; set; } = null!;
    public DateTime HanSuDung { get; set; }
    public int SoNgayConLai { get; set; }
    public int SoLuongCon { get; set; }
}

public sealed class DoanhThuNgayItem
{
    public DateTime Ngay { get; set; }
    public int SoHoaDon { get; set; }
    public decimal DoanhThu { get; set; }
}

public sealed record TongQuan(
    decimal DoanhThuHomNay,
    decimal TongDoanhThu,
    decimal TongTienNhap,
    decimal LoiNhuanUocTinh,
    int SoHoaDon,
    int SoLoaiThuoc,
    int ThuocSapHetHang,
    int LoSapHetHan);
