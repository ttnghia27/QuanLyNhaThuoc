using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.API.Data;
using QuanLyNhaThuoc.API.DTOs;
using QuanLyNhaThuoc.API.Helpers;
using QuanLyNhaThuoc.API.Models;

namespace QuanLyNhaThuoc.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KhachHangController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly CodeGenerator _codeGen;

    public KhachHangController(ApplicationDbContext context, CodeGenerator codeGen)
    {
        _context = context;
        _codeGen = codeGen;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
    {
        var query = _context.KhachHangs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.TenKH.Contains(search)
                                  || (x.SDTKH != null && x.SDTKH.Contains(search)));
        }

        return Ok(await query.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var kh = await _context.KhachHangs
            .Include(x => x.HoaDons)
            .FirstOrDefaultAsync(x => x.MaKH == id);

        if (kh == null) return NotFound(new { message = "Không tìm thấy khách hàng" });
        return Ok(kh);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] KhachHangDto dto)
    {
        var entity = new KhachHang
        {
            MaKH = await _codeGen.GenerateMaKHAsync(),
            TenKH = dto.TenKH.Trim(),
            SDTKH = dto.SDTKH?.Trim(),
            DiaChiKH = dto.DiaChiKH?.Trim()
        };

        _context.KhachHangs.Add(entity);
        await _context.SaveChangesAsync();
        return Ok(entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] KhachHangDto dto)
    {
        var entity = await _context.KhachHangs.FindAsync(id);
        if (entity == null) return NotFound(new { message = "Không tìm thấy khách hàng" });

        entity.TenKH = dto.TenKH.Trim();
        entity.SDTKH = dto.SDTKH?.Trim();
        entity.DiaChiKH = dto.DiaChiKH?.Trim();

        await _context.SaveChangesAsync();
        return Ok(entity);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var entity = await _context.KhachHangs
            .Include(x => x.HoaDons)
            .FirstOrDefaultAsync(x => x.MaKH == id);

        if (entity == null) return NotFound(new { message = "Không tìm thấy khách hàng" });
        if (entity.HoaDons.Any())
            return BadRequest(new { message = "Không thể xóa khách hàng đã có hóa đơn" });

        _context.KhachHangs.Remove(entity);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Xóa thành công" });
    }
}