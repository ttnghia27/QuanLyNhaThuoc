namespace QuanLyNhaThuoc.API.DTOs;

// 1. Loại thuốc
public class LoaiThuocDto
{
    public string TenLoai { get; set; } = null!;
}

// 2. Thuốc
public class ThuocDto
{
    public string TenThuoc { get; set; } = null!;
    public string? CongDung { get; set; }
    public decimal DonGiaBan { get; set; }
    public decimal GiaNhapDuKien { get; set; }
    public string MaLoai { get; set; } = null!;
}

// 3. Nhà cung cấp
public class NhaCCDto
{
    public string TenNhaCC { get; set; } = null!;
    public string? SDTNhaCC { get; set; }
}

// 4. Khách hàng
public class KhachHangDto
{
    public string TenKH { get; set; } = null!;
    public string? SDTKH { get; set; }
    public string? DiaChiKH { get; set; }
}