namespace QuanLyNhaThuoc.API.Models;

public class NhaCC
{
    public string MaNhaCC { get; set; } = null!;
    public string TenNhaCC { get; set; } = null!;
    public string? SDTNhaCC { get; set; }

    public ICollection<PhieuNhap> PhieuNhaps { get; set; } = new List<PhieuNhap>();
}