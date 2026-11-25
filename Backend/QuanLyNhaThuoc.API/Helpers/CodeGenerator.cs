using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.API.Data;

namespace QuanLyNhaThuoc.API.Helpers;

public sealed class CodeGenerator
{
    private readonly ApplicationDbContext _context;

    public CodeGenerator(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<string> GenerateMaLoaiAsync() =>
        GenerateAsync("SELECT dbo.fn_SinhMaLoai() AS [Value]");

    public Task<string> GenerateMaThuocAsync() =>
        GenerateAsync("SELECT dbo.fn_SinhMaThuoc() AS [Value]");

    public Task<string> GenerateMaNhaCCAsync() =>
        GenerateAsync("SELECT dbo.fn_SinhMaNhaCC() AS [Value]");

    public Task<string> GenerateMaPhieuNhapAsync() =>
        GenerateAsync("SELECT dbo.fn_SinhMaPhieuNhap() AS [Value]");

    public Task<string> GenerateMaLoAsync() =>
        GenerateAsync("SELECT dbo.fn_SinhMaLo() AS [Value]");

    public Task<string> GenerateMaKHAsync() =>
        GenerateAsync("SELECT dbo.fn_SinhMaKH() AS [Value]");

    public Task<string> GenerateMaHDAsync() =>
        GenerateAsync("SELECT dbo.fn_SinhMaHD() AS [Value]");

    private Task<string> GenerateAsync(string sql) =>
        _context.Database.SqlQueryRaw<string>(sql).SingleAsync();
}
