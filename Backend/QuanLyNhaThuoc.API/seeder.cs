using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.API.Models;

namespace QuanLyNhaThuoc.API.Data;
public record NguoiDungLogin(String TenDangNhap, String MatKhau);

public static class DbSeeder
{
    public static async Task SeedAdminUserAsync(ApplicationDbContext context)
    {
        const string adminUsername = "adminHuy";
        const string adminPassword = "adminpw";
        const string adminFullName = "Huy Le";

        bool exists = await context.NguoiDungs
            .AnyAsync(x => x.TenDangNhap == adminUsername);

        if (exists)
        {
            return;
        }

        var hasher = new PasswordHasher<NguoiDungLogin>();
        var userForHash = new NguoiDungLogin(adminUsername, adminPassword);
        string hashedPassword = hasher.HashPassword(userForHash, adminPassword);

        var adminUser = new NguoiDung{
            TenDangNhap = adminUsername,
            MatKhau = hashedPassword,
            HoTen = adminFullName
            };       
        await context.NguoiDungs.AddAsync(adminUser);
        await context.SaveChangesAsync();
    }
}