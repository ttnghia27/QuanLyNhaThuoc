using System.Data;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.API.Data;
using QuanLyNhaThuoc.API.DTOs;
using QuanLyNhaThuoc.API.Models;

namespace QuanLyNhaThuoc.API.Services;

public sealed class HoaDonService
{
    private readonly ApplicationDbContext _context;
    private readonly StoredProcedureExecutor _procedureExecutor;

    public HoaDonService(ApplicationDbContext context, StoredProcedureExecutor procedureExecutor)
    {
        _context = context;
        _procedureExecutor = procedureExecutor;
    }

    public async Task<List<HoaDonListItem>> GetAllAsync()
    {
        return await _context.HoaDons
            .OrderByDescending(x => x.NgayLapHD)
            .Select(x => new HoaDonListItem(
                x.MaHD,
                x.NgayLapHD,
                x.TongTien,
                x.MaKH,
                x.KhachHang.TenKH))
            .ToListAsync();
    }

    public async Task<HoaDonDetail?> GetByIdAsync(string id)
    {
        return await _context.HoaDons
            .Where(x => x.MaHD == id)
            .Select(x => new HoaDonDetail(
                x.MaHD,
                x.NgayLapHD,
                x.TongTien,
                x.MaKH,
                x.KhachHang.TenKH,
                x.ChiTietHoaDons.Select(ct => new HoaDonChiTietItem(
                    ct.MaLo,
                    ct.LoThuoc.MaThuoc,
                    ct.LoThuoc.Thuoc.TenThuoc,
                    ct.LoThuoc.HanSuDung,
                    ct.SoLuongBan,
                    ct.GiaBan,
                    ct.SoLuongBan * ct.GiaBan))))
            .FirstOrDefaultAsync();
    }

    public async Task<HoaDon> CreateAsync(CreateHoaDonRequest request)
    {
        if (request.ChiTiet == null || request.ChiTiet.Count == 0)
            throw new BusinessRuleException("Hóa đơn phải có ít nhất 1 sản phẩm");

        if (request.ChiTiet.Any(x => x.SoLuongBan <= 0))
            throw new BusinessRuleException("Số lượng bán của mỗi thuốc phải lớn hơn 0");

        var chiTietJson = System.Text.Json.JsonSerializer.Serialize(request.ChiTiet);
        var id = await _procedureExecutor.ExecuteAndGetIdAsync(
            "dbo.sp_TaoHoaDon",
            "MaHD",
            new ProcedureParameter("@MaKH", DbType.String, request.MaKH, 10),
            new ProcedureParameter("@ChiTiet", DbType.String, chiTietJson, -1));

        return await _context.HoaDons.FirstOrDefaultAsync(x => x.MaHD == id)
            ?? throw new InvalidOperationException(
                $"Stored procedure 'dbo.sp_TaoHoaDon' created '{id}' but the record could not be loaded.");
    }
}