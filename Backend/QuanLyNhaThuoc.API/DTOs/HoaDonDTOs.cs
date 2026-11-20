namespace QuanLyNhaThuoc.API.DTOs;

public class ChiTietHoaDonDto
{
    public string MaThuoc { get; set; } = null!;
    public int SoLuongBan { get; set; }
}

public class CreateHoaDonRequest
{
    public string MaKH { get; set; } = null!;
    public List<ChiTietHoaDonDto> ChiTiet { get; set; } = new();
}