namespace QuanLyNhaThuoc.API.Models;

public class ChiTietPhieuNhap
{
    public string MaPhieuNhap { get; set; } = null!;
    public string MaThuoc { get; set; } = null!;
    public int SoLuongNhap { get; set; }
    public decimal DonGiaNhap { get; set; }

    public PhieuNhap PhieuNhap { get; set; } = null!;
    public Thuoc Thuoc { get; set; } = null!;
}