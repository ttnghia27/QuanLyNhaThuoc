namespace QuanLyNhaThuoc.API.Models;

public class KhachHang
{
    public string MaKH { get; set; } = null!;
    public string TenKH { get; set; } = null!;
    public string? SDTKH { get; set; }
    public string? DiaChiKH { get; set; }

    public ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}