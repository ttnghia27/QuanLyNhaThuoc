namespace QuanLyNhaThuoc.API.Models;

public class PhieuNhap
{
    public string MaPhieuNhap { get; set; } = null!;
    public DateTime NgayNhap { get; set; }
    public decimal TongTien { get; set; }
    public string MaNhaCC { get; set; } = null!;

    public NhaCC NhaCC { get; set; } = null!;
    public ICollection<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = new List<ChiTietPhieuNhap>();
    public ICollection<LoThuoc> LoThuocs { get; set; } = new List<LoThuoc>();
}