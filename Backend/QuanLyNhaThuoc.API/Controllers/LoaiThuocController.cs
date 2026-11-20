using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.API.Data;
using QuanLyNhaThuoc.API.DTOs;
using QuanLyNhaThuoc.API.Helpers;
using QuanLyNhaThuoc.API.Models;

namespace QuanLyNhaThuoc.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoaiThuocController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly CodeGenerator _codeGen;

    public LoaiThuocController(ApplicationDbContext context, CodeGenerator codeGen)
    {
        _context = context;
        _codeGen = codeGen;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _context.LoaiThuocs
            .Select(x => new
            {
                x.MaLoai,
                x.TenLoai,
                SoLuongThuoc = x.Thuocs.Count
            })
            .ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var item = await _context.LoaiThuocs.FindAsync(id);
        if (item == null) return NotFound(new { message = "Không tìm thấy loại thuốc" });
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] LoaiThuocDto dto)
    {
        var entity = new LoaiThuoc
        {
            MaLoai = await _codeGen.GenerateMaLoaiAsync(),
            TenLoai = dto.TenLoai.Trim()
        };

        _context.LoaiThuocs.Add(entity);
        await _context.SaveChangesAsync();
        return Ok(entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] LoaiThuocDto dto)
    {
        var entity = await _context.LoaiThuocs.FindAsync(id);
        if (entity == null) return NotFound(new { message = "Không tìm thấy loại thuốc" });

        entity.TenLoai = dto.TenLoai.Trim();
        await _context.SaveChangesAsync();
        return Ok(entity);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var entity = await _context.LoaiThuocs.Include(x => x.Thuocs).FirstOrDefaultAsync(x => x.MaLoai == id);
        if (entity == null) return NotFound(new { message = "Không tìm thấy loại thuốc" });

        if (entity.Thuocs.Any())
            return BadRequest(new { message = "Không thể xóa loại thuốc đang có thuốc trực thuộc" });

        _context.LoaiThuocs.Remove(entity);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Xóa thành công" });
    }
}