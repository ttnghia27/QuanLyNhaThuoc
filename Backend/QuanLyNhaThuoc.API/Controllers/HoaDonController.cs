using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.API.Data;
using QuanLyNhaThuoc.API.DTOs;
using QuanLyNhaThuoc.API.Services;

namespace QuanLyNhaThuoc.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HoaDonController : ControllerBase
{
    private readonly HoaDonService _service;
    private readonly ApplicationDbContext _context;

    public HoaDonController(HoaDonService service, ApplicationDbContext context)
    {
        _service = service;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _context.HoaDons
            .Include(x => x.KhachHang)
            .OrderByDescending(x => x.NgayLapHD)
            .Select(x => new
            {
                x.MaHD,
                x.NgayLapHD,
                x.TongTien,
                x.MaKH,
                TenKH = x.KhachHang.TenKH
            })
            .ToListAsync();

        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var hd = await _context.HoaDons
            .Include(x => x.KhachHang)
            .Include(x => x.ChiTietHoaDons)
                .ThenInclude(ct => ct.LoThuoc)
                    .ThenInclude(l => l.Thuoc)
            .FirstOrDefaultAsync(x => x.MaHD == id);

        if (hd == null) return NotFound(new { message = "Không tìm thấy hóa đơn" });

        return Ok(new
        {
            hd.MaHD,
            hd.NgayLapHD,
            hd.TongTien,
            hd.MaKH,
            TenKH = hd.KhachHang.TenKH,
            ChiTiet = hd.ChiTietHoaDons.Select(ct => new
            {
                ct.MaLo,
                MaThuoc = ct.LoThuoc.MaThuoc,
                TenThuoc = ct.LoThuoc.Thuoc.TenThuoc,
                HanSuDung = ct.LoThuoc.HanSuDung,
                ct.SoLuongBan,
                ct.GiaBan,
                ThanhTien = ct.SoLuongBan * ct.GiaBan
            })
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHoaDonRequest request)
    {
        try
        {
            var result = await _service.CreateAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}