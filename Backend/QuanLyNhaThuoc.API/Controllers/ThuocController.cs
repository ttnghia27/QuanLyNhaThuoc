using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.API.Data;
using QuanLyNhaThuoc.API.DTOs;
using QuanLyNhaThuoc.API.Helpers;
using QuanLyNhaThuoc.API.Models;

namespace QuanLyNhaThuoc.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ThuocController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly CodeGenerator _codeGen;

    public ThuocController(ApplicationDbContext context, CodeGenerator codeGen)
    {
        _context = context;
        _codeGen = codeGen;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? maLoai)
    {
        var query = _context.Thuocs.Include(x => x.LoaiThuoc).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.TenThuoc.Contains(search)
                                  || x.MaThuoc.Contains(search)
                                  || (x.CongDung != null && x.CongDung.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(maLoai))
        {
            query = query.Where(x => x.MaLoai == maLoai);
        }

        var result = await query
            .Select(x => new
            {
                x.MaThuoc,
                x.TenThuoc,
                x.CongDung,
                x.DonGiaBan,
                x.SoLuongTonKho,
                x.MaLoai,
                TenLoai = x.LoaiThuoc.TenLoai
            })
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var thuoc = await _context.Thuocs
            .Include(x => x.LoaiThuoc)
            .Include(x => x.LoThuocs.Where(l => l.SoLuongCon > 0))
            .FirstOrDefaultAsync(x => x.MaThuoc == id);

        if (thuoc == null) return NotFound(new { message = "Không tìm thấy thuốc" });
        return Ok(thuoc);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ThuocDto dto)
    {
        var loaiExists = await _context.LoaiThuocs.AnyAsync(x => x.MaLoai == dto.MaLoai);
        if (!loaiExists) return BadRequest(new { message = "Mã loại thuốc không tồn tại" });

        var entity = new Thuoc
        {
            MaThuoc = await _codeGen.GenerateMaThuocAsync(),
            TenThuoc = dto.TenThuoc.Trim(),
            CongDung = dto.CongDung?.Trim(),
            DonGiaBan = dto.DonGiaBan,
            SoLuongTonKho = 0, // Tồn kho tăng khi nhập phiếu nhập
            MaLoai = dto.MaLoai
        };

        _context.Thuocs.Add(entity);
        await _context.SaveChangesAsync();
        return Ok(entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] ThuocDto dto)
    {
        var entity = await _context.Thuocs.FindAsync(id);
        if (entity == null) return NotFound(new { message = "Không tìm thấy thuốc" });

        var loaiExists = await _context.LoaiThuocs.AnyAsync(x => x.MaLoai == dto.MaLoai);
        if (!loaiExists) return BadRequest(new { message = "Mã loại thuốc không tồn tại" });

        entity.TenThuoc = dto.TenThuoc.Trim();
        entity.CongDung = dto.CongDung?.Trim();
        entity.DonGiaBan = dto.DonGiaBan;
        entity.MaLoai = dto.MaLoai;

        await _context.SaveChangesAsync();
        return Ok(entity);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var entity = await _context.Thuocs
            .Include(x => x.LoThuocs)
            .FirstOrDefaultAsync(x => x.MaThuoc == id);

        if (entity == null) return NotFound(new { message = "Không tìm thấy thuốc" });
        if (entity.LoThuocs.Any())
            return BadRequest(new { message = "Không thể xóa thuốc đã có lịch sử nhập lô" });

        _context.Thuocs.Remove(entity);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Xóa thành công" });
    }
}