using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.API.Data;

namespace QuanLyNhaThuoc.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CanhBaoController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CanhBaoController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("sap-het-han")]
    public async Task<IActionResult> GetSapHetHan(int soNgay = 30)
    {
        var ngayCanhBao = DateTime.Today.AddDays(soNgay);

        var result = await _context.LoThuocs
            .Include(x => x.Thuoc)
            .Where(x => x.SoLuongCon > 0 && x.HanSuDung <= ngayCanhBao)
            .OrderBy(x => x.HanSuDung)
            .Select(x => new
            {
                x.MaLo,
                x.MaThuoc,
                TenThuoc = x.Thuoc.TenThuoc,
                x.HanSuDung,
                SoNgayConLai = (x.HanSuDung - DateTime.Today).Days,
                x.SoLuongCon
            })
            .ToListAsync();

        return Ok(result);
    }
}