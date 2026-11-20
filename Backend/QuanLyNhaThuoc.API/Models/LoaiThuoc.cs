namespace QuanLyNhaThuoc.API.Models;

public class LoaiThuoc
{
    public string MaLoai { get; set; } = null!;
    public string TenLoai { get; set; } = null!;

    public ICollection<Thuoc> Thuocs { get; set; } = new List<Thuoc>();
}