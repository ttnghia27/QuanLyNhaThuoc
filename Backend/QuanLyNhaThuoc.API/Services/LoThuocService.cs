using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.API.Data;
using QuanLyNhaThuoc.API.DTOs;

namespace QuanLyNhaThuoc.API.Services;

public sealed class LoThuocService
{
    private readonly ApplicationDbContext _context;

    public LoThuocService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<LoThuocListItem>> GetAllAsync(string? maThuoc, bool conHangOnly)
    {
        var query = _context.LoThuocs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(maThuoc))
            query = query.Where(x => x.MaThuoc == maThuoc);

        if (conHangOnly)
            query = query.Where(x => x.SoLuongCon > 0);

        return query
            .OrderBy(x => x.HanSuDung)
            .Select(x => new LoThuocListItem(
                x.MaLo,
                x.MaThuoc,
                x.Thuoc.TenThuoc,
                x.MaPhieuNhap,
                x.NgayNhapKho,
                x.HanSuDung,
                x.SoLuongNhap,
                x.SoLuongCon,
                x.DonGiaNhap,
                x.HanSuDung < DateTime.Today))
            .ToListAsync();
    }

    public async Task<int?> TieuHuyAsync(string maLo)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        var lo = await _context.LoThuocs
            .Include(x => x.Thuoc)
            .FirstOrDefaultAsync(x => x.MaLo == maLo);

        if (lo == null)
            return null;

        if (lo.SoLuongCon <= 0)
            throw new BusinessRuleException("Lô thuốc này đã hết số lượng tồn");

        var soLuongHuy = lo.SoLuongCon;
        lo.SoLuongCon = 0;
        lo.Thuoc.SoLuongTonKho = Math.Max(0, lo.Thuoc.SoLuongTonKho - soLuongHuy);

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return soLuongHuy;
    }
}
