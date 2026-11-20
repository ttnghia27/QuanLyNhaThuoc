namespace QuanLyNhaThuoc.API.Models;

public class LoThuoc
{
    public string MaLo { get; set; } = null!;
    public string MaThuoc { get; set; } = null!;
    public string MaPhieuNhap { get; set; } = null!;
    public DateTime NgayNhapKho { get; set; }
    public DateTime HanSuDung { get; set; }
    public int SoLuongNhap { get; set; }
    public int SoLuongCon { get; set; }
    public decimal DonGiaNhap { get; set; }

    public Thuoc Thuoc { get; set; } = null!;
    public PhieuNhap PhieuNhap { get; set; } = null!;
    public ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
}