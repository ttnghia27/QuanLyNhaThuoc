using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.API.Data;

namespace QuanLyNhaThuoc.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ThongKeController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ThongKeController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("tong-quan")]
    public async Task<IActionResult> GetTongQuan()
    {
        var homNay = DateTime.Today;
        var ngayCanhBao = homNay.AddDays(30);

        var doanhThuHomNay = await _context.HoaDons
            .Where(x => x.NgayLapHD >= homNay)
            .SumAsync(x => (decimal?)x.TongTien) ?? 0;

        var tongDoanhThu = await _context.HoaDons
            .SumAsync(x => (decimal?)x.TongTien) ?? 0;

        var tongTienNhap = await _context.PhieuNhaps
            .SumAsync(x => (decimal?)x.TongTien) ?? 0;

        var soHoaDon = await _context.HoaDons.CountAsync();
        var soLoaiThuoc = await _context.Thuocs.CountAsync();
        var thuocSapHetHang = await _context.Thuocs.CountAsync(x => x.SoLuongTonKho <= 10);
        var loSapHetHan = await _context.LoThuocs.CountAsync(x => x.SoLuongCon > 0 && x.HanSuDung <= ngayCanhBao);

        return Ok(new
        {
            doanhThuHomNay,
            tongDoanhThu,
            tongTienNhap,
            loiNhuanUocTinh = tongDoanhThu - tongTienNhap,
            soHoaDon,
            soLoaiThuoc,
            thuocSapHetHang,
            loSapHetHan
        });
    }

    [HttpGet("doanh-thu")]
    public async Task<IActionResult> GetDoanhThuTheoNgay([FromQuery] DateTime? tuNgay, [FromQuery] DateTime? denNgay)
    {
        var start = (tuNgay ?? DateTime.Today.AddDays(-7)).Date;
        var end = (denNgay ?? DateTime.Today).Date.AddDays(1).AddTicks(-1);

        var data = await _context.HoaDons
            .Where(x => x.NgayLapHD >= start && x.NgayLapHD <= end)
            .GroupBy(x => x.NgayLapHD.Date)
            .Select(g => new
            {
                Ngay = g.Key,
                SoHoaDon = g.Count(),
                DoanhThu = g.Sum(x => x.TongTien)
            })
            .OrderBy(x => x.Ngay)
            .ToListAsync();

        return Ok(data);
    }
}