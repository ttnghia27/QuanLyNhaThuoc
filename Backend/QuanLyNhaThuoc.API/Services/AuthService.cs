using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuanLyNhaThuoc.API.Data;
using QuanLyNhaThuoc.API.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;

namespace QuanLyNhaThuoc.API.Services;

public sealed class AuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    public record NguoiDungLogin(String TenDangNhap, String MatKhau);

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.NguoiDungs
            .FirstOrDefaultAsync(x => x.TenDangNhap == request.TenDangNhap);
        if(user == null)
        {
            return null;
        }
        var logining_user = new NguoiDungLogin(request.TenDangNhap, request.MatKhau);
        var hasher = new PasswordHasher<NguoiDungLogin>();
        var result = hasher.VerifyHashedPassword(logining_user, user.MatKhau, request.MatKhau);
        bool isValid = result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
        Console.WriteLine($"[AuthService] result = {result}, isValid = {isValid}");       
        if (!isValid)
        {
            return null;
        }

        return new LoginResponse
        {
            Token = GenerateJwtToken(user.TenDangNhap, user.HoTen),
            HoTen = user.HoTen
        };
    }

    private string GenerateJwtToken(string username, string hoTen)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim("HoTen", hoTen)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
