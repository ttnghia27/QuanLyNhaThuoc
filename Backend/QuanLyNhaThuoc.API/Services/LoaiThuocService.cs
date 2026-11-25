using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.API.Data;
using QuanLyNhaThuoc.API.DTOs;
using QuanLyNhaThuoc.API.Helpers;
using QuanLyNhaThuoc.API.Models;

namespace QuanLyNhaThuoc.API.Services;

public sealed class LoaiThuocService
{
    private readonly ApplicationDbContext _context;
    private readonly CodeGenerator _codeGenerator;

    public LoaiThuocService(ApplicationDbContext context, CodeGenerator codeGenerator)
    {
        _context = context;
        _codeGenerator = codeGenerator;
    }

    public Task<List<LoaiThuocListItem>> GetAllAsync() =>
        _context.LoaiThuocs
            .Select(x => new LoaiThuocListItem(x.MaLoai, x.TenLoai, x.Thuocs.Count))
            .ToListAsync();

    public ValueTask<LoaiThuoc?> GetByIdAsync(string id) =>
        _context.LoaiThuocs.FindAsync(id);

    public async Task<LoaiThuoc> CreateAsync(LoaiThuocDto dto)
    {
        var entity = new LoaiThuoc
        {
            MaLoai = await _codeGenerator.GenerateMaLoaiAsync(),
            TenLoai = dto.TenLoai.Trim()
        };

        _context.LoaiThuocs.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<LoaiThuoc?> UpdateAsync(string id, LoaiThuocDto dto)
    {
        var entity = await _context.LoaiThuocs.FindAsync(id);
        if (entity == null)
            return null;

        entity.TenLoai = dto.TenLoai.Trim();
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var entity = await _context.LoaiThuocs.FindAsync(id);
        if (entity == null)
            return false;

        if (await _context.Thuocs.AnyAsync(x => x.MaLoai == id))
            throw new BusinessRuleException("Không thể xóa loại thuốc đang có thuốc trực thuộc");

        _context.LoaiThuocs.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
