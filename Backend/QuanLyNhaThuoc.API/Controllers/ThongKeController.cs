using Microsoft.AspNetCore.Mvc;
using QuanLyNhaThuoc.API.Services;

namespace QuanLyNhaThuoc.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ThongKeController : ControllerBase
{
    private readonly ThongKeService _service;

    public ThongKeController(ThongKeService service)
    {
        _service = service;
    }

    [HttpGet("tong-quan")]
    public async Task<IActionResult> GetTongQuan([FromQuery] int months = 1)
    {
        try
        {
            var data = await _service.GetTongQuanAsync(months);
            return Ok(data);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("doanh-thu")]
    public async Task<IActionResult> GetDoanhThuTheoNgay(
        [FromQuery] DateTime? tuNgay,
        [FromQuery] DateTime? denNgay)
    {
        return Ok(await _service.GetDoanhThuTheoNgayAsync(tuNgay, denNgay));
    }
}
