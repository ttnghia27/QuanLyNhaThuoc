using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.API.Data;
using QuanLyNhaThuoc.API.DTOs;
using QuanLyNhaThuoc.API.Helpers;
using QuanLyNhaThuoc.API.Models;

namespace QuanLyNhaThuoc.API.Services;

public sealed class NhaCCService
{
    private readonly ApplicationDbContext _context;
    private readonly CodeGenerator _codeGenerator;

    public NhaCCService(ApplicationDbContext context, CodeGenerator codeGenerator)
    {
        _context = context;
        _codeGenerator = codeGenerator;
    }

    public Task<List<NhaCC>> GetAllAsync() => _context.NhaCCs.ToListAsync();

    public ValueTask<NhaCC?> GetByIdAsync(string id) => _context.NhaCCs.FindAsync(id);

    public async Task<NhaCC> CreateAsync(NhaCCDto dto)
    {
        var entity = new NhaCC
        {
            MaNhaCC = await _codeGenerator.GenerateMaNhaCCAsync(),
            TenNhaCC = dto.TenNhaCC.Trim(),
            SDTNhaCC = dto.SDTNhaCC?.Trim()
        };

        _context.NhaCCs.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<NhaCC?> UpdateAsync(string id, NhaCCDto dto)
    {
        var entity = await _context.NhaCCs.FindAsync(id);
        if (entity == null)
            return null;

        entity.TenNhaCC = dto.TenNhaCC.Trim();
        entity.SDTNhaCC = dto.SDTNhaCC?.Trim();
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var entity = await _context.NhaCCs.FindAsync(id);
        if (entity == null)
            return false;

        if (await _context.PhieuNhaps.AnyAsync(x => x.MaNhaCC == id))
            throw new BusinessRuleException("Không thể xóa nhà cung cấp đã có phiếu nhập");

        _context.NhaCCs.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
