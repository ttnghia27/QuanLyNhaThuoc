using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.API.Data;
using QuanLyNhaThuoc.API.DTOs;
using QuanLyNhaThuoc.API.Services;

namespace QuanLyNhaThuoc.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PhieuNhapController : ControllerBase
{
    private readonly PhieuNhapService _service;
    private readonly ApplicationDbContext _context;

    public PhieuNhapController(PhieuNhapService service, ApplicationDbContext context)
    {
        _service = service;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _context.PhieuNhaps
            .Include(x => x.NhaCC)
            .OrderByDescending(x => x.NgayNhap)
            .Select(x => new
            {
                x.MaPhieuNhap,
                x.NgayNhap,
                x.TongTien,
                x.MaNhaCC,
                TenNhaCC = x.NhaCC.TenNhaCC
            })
            .ToListAsync();

        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var pn = await _context.PhieuNhaps
            .Include(x => x.NhaCC)
            .Include(x => x.LoThuocs)
                .ThenInclude(l => l.Thuoc)
            .FirstOrDefaultAsync(x => x.MaPhieuNhap == id);

        if (pn == null) return NotFound(new { message = "Không tìm thấy phiếu nhập" });

        return Ok(new
        {
            pn.MaPhieuNhap,
            pn.NgayNhap,
            pn.TongTien,
            pn.MaNhaCC,
            TenNhaCC = pn.NhaCC.TenNhaCC,
            ChiTietLo = pn.LoThuocs.Select(l => new
            {
                l.MaLo,
                l.MaThuoc,
                TenThuoc = l.Thuoc.TenThuoc,
                l.HanSuDung,
                l.SoLuongNhap,
                l.SoLuongCon,
                l.DonGiaNhap,
                ThanhTien = l.SoLuongNhap * l.DonGiaNhap
            })
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePhieuNhapRequest request)
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