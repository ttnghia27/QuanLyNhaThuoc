using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.API.Data;

namespace QuanLyNhaThuoc.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoThuocController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LoThuocController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? maThuoc, [FromQuery] bool conHangOnly = false)
    {
        var query = _context.LoThuocs.Include(x => x.Thuoc).AsQueryable();

        if (!string.IsNullOrWhiteSpace(maThuoc))
            query = query.Where(x => x.MaThuoc == maThuoc);

        if (conHangOnly)
            query = query.Where(x => x.SoLuongCon > 0);

        var list = await query
            .OrderBy(x => x.HanSuDung)
            .Select(x => new
            {
                x.MaLo,
                x.MaThuoc,
                TenThuoc = x.Thuoc.TenThuoc,
                x.MaPhieuNhap,
                x.NgayNhapKho,
                x.HanSuDung,
                x.SoLuongNhap,
                x.SoLuongCon,
                x.DonGiaNhap,
                DaHetHan = x.HanSuDung < DateTime.Today
            })
            .ToListAsync();

        return Ok(list);
    }

    // Tiêu hủy lô thuốc quá hạn (trừ tồn kho tổng về đúng thực tế)
    [HttpPut("{maLo}/tieu-huy")]
    public async Task<IActionResult> TieuHuyLo(string maLo)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var lo = await _context.LoThuocs.Include(x => x.Thuoc).FirstOrDefaultAsync(x => x.MaLo == maLo);
            if (lo == null) return NotFound(new { message = "Không tìm thấy lô thuốc" });
            if (lo.SoLuongCon <= 0) return BadRequest(new { message = "Lô thuốc này đã hết số lượng tồn" });

            int soLuongHuy = lo.SoLuongCon;
            lo.SoLuongCon = 0;
            lo.Thuoc.SoLuongTonKho = Math.Max(0, lo.Thuoc.SoLuongTonKho - soLuongHuy);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { message = $"Đã tiêu hủy {soLuongHuy} sản phẩm từ lô {maLo}" });
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}