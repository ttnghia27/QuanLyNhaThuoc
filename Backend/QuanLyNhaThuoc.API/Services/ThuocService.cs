using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.API.Data;
using QuanLyNhaThuoc.API.DTOs;
using QuanLyNhaThuoc.API.Helpers;
using QuanLyNhaThuoc.API.Models;

namespace QuanLyNhaThuoc.API.Services;

public sealed class ThuocService
{
    private readonly ApplicationDbContext _context;
    private readonly CodeGenerator _codeGenerator;

    public ThuocService(ApplicationDbContext context, CodeGenerator codeGenerator)
    {
        _context = context;
        _codeGenerator = codeGenerator;
    }

    public Task<List<ThuocListItem>> GetAllAsync(string? search, string? maLoai)
    {
        var query = _context.Thuocs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.TenThuoc.Contains(search)
                                  || x.MaThuoc.Contains(search)
                                  || (x.CongDung != null && x.CongDung.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(maLoai))
            query = query.Where(x => x.MaLoai == maLoai);

        return query
            .Select(x => new ThuocListItem(
                x.MaThuoc,
                x.TenThuoc,
                x.CongDung,
                x.DonGiaBan,
                x.SoLuongTonKho,
                x.MaLoai,
                x.LoaiThuoc.TenLoai))
            .ToListAsync();
    }

    public Task<Thuoc?> GetByIdAsync(string id) =>
        _context.Thuocs
            .Include(x => x.LoaiThuoc)
            .Include(x => x.LoThuocs.Where(l => l.SoLuongCon > 0))
            .FirstOrDefaultAsync(x => x.MaThuoc == id);

    public async Task<Thuoc> CreateAsync(ThuocDto dto)
    {
        if (!await _context.LoaiThuocs.AnyAsync(x => x.MaLoai == dto.MaLoai))
            throw new BusinessRuleException("Mã loại thuốc không tồn tại");

        var entity = new Thuoc
        {
            MaThuoc = await _codeGenerator.GenerateMaThuocAsync(),
            TenThuoc = dto.TenThuoc.Trim(),
            CongDung = dto.CongDung?.Trim(),
            DonGiaBan = dto.DonGiaBan,
            SoLuongTonKho = 0,
            MaLoai = dto.MaLoai
        };

        _context.Thuocs.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Thuoc?> UpdateAsync(string id, ThuocDto dto)
    {
        var entity = await _context.Thuocs.FindAsync(id);
        if (entity == null)
            return null;

        if (!await _context.LoaiThuocs.AnyAsync(x => x.MaLoai == dto.MaLoai))
            throw new BusinessRuleException("Mã loại thuốc không tồn tại");

        entity.TenThuoc = dto.TenThuoc.Trim();
        entity.CongDung = dto.CongDung?.Trim();
        entity.DonGiaBan = dto.DonGiaBan;
        entity.MaLoai = dto.MaLoai;

        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var entity = await _context.Thuocs.FindAsync(id);
        if (entity == null)
            return false;

        if (await _context.LoThuocs.AnyAsync(x => x.MaThuoc == id))
            throw new BusinessRuleException("Không thể xóa thuốc đã có lịch sử nhập lô");

        _context.Thuocs.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
