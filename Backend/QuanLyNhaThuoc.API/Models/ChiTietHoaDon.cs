namespace QuanLyNhaThuoc.API.Models;

public class ChiTietHoaDon
{
    public string MaHD { get; set; } = null!;
    public string MaLo { get; set; } = null!;
    public int SoLuongBan { get; set; }
    public decimal GiaBan { get; set; }

    public HoaDon HoaDon { get; set; } = null!;
    public LoThuoc LoThuoc { get; set; } = null!;
}