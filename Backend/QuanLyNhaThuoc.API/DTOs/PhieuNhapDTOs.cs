namespace QuanLyNhaThuoc.API.DTOs;

public class ChiTietPhieuNhapDto
{
    public string MaThuoc { get; set; } = null!;
    public int SoLuongNhap { get; set; }
    public decimal DonGiaNhap { get; set; }
    public DateTime HanSuDung { get; set; }   // Quan trọng: để tạo lô
}

public class CreatePhieuNhapRequest
{
    public string MaNhaCC { get; set; } = null!;
    public DateTime? NgayNhap { get; set; }
    public List<ChiTietPhieuNhapDto> ChiTiet { get; set; } = new();
}