using Microsoft.AspNetCore.Mvc;
using QuanLyNhaThuoc.API.Services;

namespace QuanLyNhaThuoc.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class LoThuocController : ControllerBase
{
    private readonly LoThuocService _service;

    public LoThuocController(LoThuocService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? maThuoc,
        [FromQuery] bool conHangOnly = false)
    {
        return Ok(await _service.GetAllAsync(maThuoc, conHangOnly));
    }

    [HttpPut("{maLo}/tieu-huy")]
    public async Task<IActionResult> TieuHuyLo(string maLo)
    {
        var soLuongHuy = await _service.TieuHuyAsync(maLo);
        if (soLuongHuy == null)
            return NotFound(new { message = "Không tìm thấy lô thuốc" });

        return Ok(new { message = $"Đã tiêu hủy {soLuongHuy} sản phẩm từ lô {maLo}" });
    }
}
