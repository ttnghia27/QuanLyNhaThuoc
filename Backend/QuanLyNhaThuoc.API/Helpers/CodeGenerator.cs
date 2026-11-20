
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using QuanLyNhaThuoc.API.Data;

namespace QuanLyNhaThuoc.API.Helpers;

public class CodeGenerator
{
    private readonly ApplicationDbContext _context;

    public CodeGenerator(ApplicationDbContext context)
    {
        _context = context;
    }

    // Hàm sinh mã dùng chung
    private async Task<string> GenerateCodeAsync(
        IQueryable<string> query,
        IEnumerable<string> localCodes,
        string prefix)
    {
        // Lấy mã hiện có trong database
        var databaseCodes = await query.ToListAsync();

        // Kết hợp mã trong database và mã đang được thêm
        // vào DbContext nhưng chưa SaveChanges
        var allCodes = databaseCodes
            .Concat(localCodes)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        int maxNumber = 0;

        foreach (var code in allCodes)
        {
            if (!code.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                continue;

            string numberPart = code[prefix.Length..];

            if (int.TryParse(
                numberPart,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out int number))
            {
                if (number > maxNumber)
                    maxNumber = number;
            }
        }

        int nextNumber = maxNumber + 1;

        return $"{prefix}{nextNumber:D2}";
    }

    // Sinh mã loại thuốc
    public Task<string> GenerateMaLoaiAsync()
    {
        return GenerateCodeAsync(
            _context.LoaiThuocs.Select(x => x.MaLoai),
            _context.LoaiThuocs.Local.Select(x => x.MaLoai),
            "LT"
        );
    }

    // Sinh mã thuốc
    public Task<string> GenerateMaThuocAsync()
    {
        return GenerateCodeAsync(
            _context.Thuocs.Select(x => x.MaThuoc),
            _context.Thuocs.Local.Select(x => x.MaThuoc),
            "T"
        );
    }

    // Sinh mã nhà cung cấp
    public Task<string> GenerateMaNhaCCAsync()
    {
        return GenerateCodeAsync(
            _context.NhaCCs.Select(x => x.MaNhaCC),
            _context.NhaCCs.Local.Select(x => x.MaNhaCC),
            "NCC"
        );
    }

    // Sinh mã phiếu nhập
    public Task<string> GenerateMaPhieuNhapAsync()
    {
        return GenerateCodeAsync(
            _context.PhieuNhaps.Select(x => x.MaPhieuNhap),
            _context.PhieuNhaps.Local.Select(x => x.MaPhieuNhap),
            "PN"
        );
    }

    // Sinh mã lô thuốc
    public Task<string> GenerateMaLoAsync()
    {
        return GenerateCodeAsync(
            _context.LoThuocs.Select(x => x.MaLo),
            _context.LoThuocs.Local.Select(x => x.MaLo),
            "LO"
        );
    }

    // Sinh mã khách hàng
    public Task<string> GenerateMaKHAsync()
    {
        return GenerateCodeAsync(
            _context.KhachHangs.Select(x => x.MaKH),
            _context.KhachHangs.Local.Select(x => x.MaKH),
            "KH"
        );
    }

    // Sinh mã hóa đơn
    public Task<string> GenerateMaHDAsync()
    {
        return GenerateCodeAsync(
            _context.HoaDons.Select(x => x.MaHD),
            _context.HoaDons.Local.Select(x => x.MaHD),
            "HD"
        );
    }
}
