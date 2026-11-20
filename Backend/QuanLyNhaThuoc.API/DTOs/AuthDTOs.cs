namespace QuanLyNhaThuoc.API.DTOs;

public class LoginRequest
{
    public string TenDangNhap { get; set; } = null!;
    public string MatKhau { get; set; } = null!;
}

public class LoginResponse
{
    public string Token { get; set; } = null!;
    public string HoTen { get; set; } = null!;
}