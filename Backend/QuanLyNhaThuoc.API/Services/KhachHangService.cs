using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.API.Data;
using QuanLyNhaThuoc.API.DTOs;
using QuanLyNhaThuoc.API.Helpers;
using QuanLyNhaThuoc.API.Models;

namespace QuanLyNhaThuoc.API.Services;

public sealed class KhachHangService
{
    private readonly ApplicationDbContext _context;
    private readonly CodeGenerator _codeGenerator;

    public KhachHangService(ApplicationDbContext context, CodeGenerator codeGenerator)
    {
        _context = context;
        _codeGenerator = codeGenerator;
    }

    public Task<List<KhachHang>> GetAllAsync(string? search)
    {
        var query = _context.KhachHangs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.TenKH.Contains(search)
                                  || (x.SDTKH != null && x.SDTKH.Contains(search)));
        }

        return query.ToListAsync();
    }

    public Task<KhachHang?> GetByIdAsync(string id) =>
        _context.KhachHangs
            .Include(x => x.HoaDons)
            .FirstOrDefaultAsync(x => x.MaKH == id);

    public async Task<KhachHang> CreateAsync(KhachHangDto dto)
    {
        var entity = new KhachHang
        {
            MaKH = await _codeGenerator.GenerateMaKHAsync(),
            TenKH = dto.TenKH.Trim(),
            SDTKH = dto.SDTKH?.Trim(),
            DiaChiKH = dto.DiaChiKH?.Trim()
        };

        _context.KhachHangs.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<KhachHang?> UpdateAsync(string id, KhachHangDto dto)
    {
        var entity = await _context.KhachHangs.FindAsync(id);
        if (entity == null)
            return null;

        entity.TenKH = dto.TenKH.Trim();
        entity.SDTKH = dto.SDTKH?.Trim();
        entity.DiaChiKH = dto.DiaChiKH?.Trim();
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var entity = await _context.KhachHangs.FindAsync(id);
        if (entity == null)
            return false;

        if (await _context.HoaDons.AnyAsync(x => x.MaKH == id))
            throw new BusinessRuleException("Không thể xóa khách hàng đã có hóa đơn");

        _context.KhachHangs.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
