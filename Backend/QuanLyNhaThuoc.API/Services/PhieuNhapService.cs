using System.Data;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.API.Data;
using QuanLyNhaThuoc.API.DTOs;
using QuanLyNhaThuoc.API.Models;

namespace QuanLyNhaThuoc.API.Services;

public sealed class PhieuNhapService
{
    private readonly ApplicationDbContext _context;
    private readonly StoredProcedureExecutor _procedureExecutor;

    public PhieuNhapService(ApplicationDbContext context, StoredProcedureExecutor procedureExecutor)
    {
        _context = context;
        _procedureExecutor = procedureExecutor;
    }

    public async Task<List<PhieuNhapListItem>> GetAllAsync()
    {
        return await _context.PhieuNhaps
            .OrderByDescending(x => x.NgayNhap)
            .Select(x => new PhieuNhapListItem(
                x.MaPhieuNhap,
                x.NgayNhap,
                x.TongTien,
                x.MaNhaCC,
                x.NhaCC.TenNhaCC))
            .ToListAsync();
    }

    public async Task<PhieuNhapDetail?> GetByIdAsync(string id)
    {
        return await _context.PhieuNhaps
            .Where(x => x.MaPhieuNhap == id)
            .Select(x => new PhieuNhapDetail(
                x.MaPhieuNhap,
                x.NgayNhap,
                x.TongTien,
                x.MaNhaCC,
                x.NhaCC.TenNhaCC,
                x.LoThuocs.Select(l => new PhieuNhapLoItem(
                    l.MaLo,
                    l.MaThuoc,
                    l.Thuoc.TenThuoc,
                    l.HanSuDung,
                    l.SoLuongNhap,
                    l.SoLuongCon,
                    l.DonGiaNhap,
                    l.SoLuongNhap * l.DonGiaNhap))))
            .FirstOrDefaultAsync();
    }

    public async Task<PhieuNhap> CreateAsync(CreatePhieuNhapRequest request)
    {
        if (request.ChiTiet == null || request.ChiTiet.Count == 0)
            throw new BusinessRuleException("Phiếu nhập phải có ít nhất 1 sản phẩm");

        var chiTietJson = System.Text.Json.JsonSerializer.Serialize(request.ChiTiet);
        var id = await _procedureExecutor.ExecuteAndGetIdAsync(
            "dbo.sp_TaoPhieuNhap",
            "MaPhieuNhap",
            new ProcedureParameter("@MaNhaCC", DbType.String, request.MaNhaCC, 10),
            new ProcedureParameter("@NgayNhap", DbType.Date, request.NgayNhap),
            new ProcedureParameter("@ChiTiet", DbType.String, chiTietJson, -1));

        return await _context.PhieuNhaps.FirstOrDefaultAsync(x => x.MaPhieuNhap == id)
            ?? throw new InvalidOperationException(
                $"Stored procedure 'dbo.sp_TaoPhieuNhap' created '{id}' but the record could not be loaded.");
    }
}