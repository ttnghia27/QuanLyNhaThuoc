namespace QuanLyNhaThuoc.API.Models;

public class HoaDon
{
    public string MaHD { get; set; } = null!;
    public DateTime NgayLapHD { get; set; }
    public decimal TongTien { get; set; }
    public string MaKH { get; set; } = null!;

    public KhachHang KhachHang { get; set; } = null!;
    public ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
}