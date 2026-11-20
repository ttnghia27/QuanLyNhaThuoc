namespace QuanLyNhaThuoc.API.Models;

public class Thuoc
{
    public string MaThuoc { get; set; } = null!;
    public string TenThuoc { get; set; } = null!;
    public string? CongDung { get; set; }
    public decimal DonGiaBan { get; set; }
    public int SoLuongTonKho { get; set; }
    public string MaLoai { get; set; } = null!;

    public LoaiThuoc LoaiThuoc { get; set; } = null!;
    public ICollection<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = new List<ChiTietPhieuNhap>();
    public ICollection<LoThuoc> LoThuocs { get; set; } = new List<LoThuoc>();
}