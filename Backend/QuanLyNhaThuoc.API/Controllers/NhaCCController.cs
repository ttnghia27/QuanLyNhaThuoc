using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.API.Data;
using QuanLyNhaThuoc.API.DTOs;
using QuanLyNhaThuoc.API.Helpers;
using QuanLyNhaThuoc.API.Models;

namespace QuanLyNhaThuoc.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NhaCCController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly CodeGenerator _codeGen;

    public NhaCCController(ApplicationDbContext context, CodeGenerator codeGen)
    {
        _context = context;
        _codeGen = codeGen;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _context.NhaCCs.ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var item = await _context.NhaCCs.FindAsync(id);
        if (item == null) return NotFound(new { message = "Không tìm thấy nhà cung cấp" });
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] NhaCCDto dto)
    {
        var entity = new NhaCC
        {
            MaNhaCC = await _codeGen.GenerateMaNhaCCAsync(),
            TenNhaCC = dto.TenNhaCC.Trim(),
            SDTNhaCC = dto.SDTNhaCC?.Trim()
        };

        _context.NhaCCs.Add(entity);
        await _context.SaveChangesAsync();
        return Ok(entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] NhaCCDto dto)
    {
        var entity = await _context.NhaCCs.FindAsync(id);
        if (entity == null) return NotFound(new { message = "Không tìm thấy nhà cung cấp" });

        entity.TenNhaCC = dto.TenNhaCC.Trim();
        entity.SDTNhaCC = dto.SDTNhaCC?.Trim();

        await _context.SaveChangesAsync();
        return Ok(entity);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var entity = await _context.NhaCCs
            .Include(x => x.PhieuNhaps)
            .FirstOrDefaultAsync(x => x.MaNhaCC == id);

        if (entity == null) return NotFound(new { message = "Không tìm thấy nhà cung cấp" });
        if (entity.PhieuNhaps.Any())
            return BadRequest(new { message = "Không thể xóa nhà cung cấp đã có phiếu nhập" });

        _context.NhaCCs.Remove(entity);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Xóa thành công" });
    }
}