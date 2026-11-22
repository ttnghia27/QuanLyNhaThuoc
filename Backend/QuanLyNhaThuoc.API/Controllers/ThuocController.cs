using Microsoft.AspNetCore.Mvc;
using QuanLyNhaThuoc.API.DTOs;
using QuanLyNhaThuoc.API.Services;

namespace QuanLyNhaThuoc.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ThuocController : ControllerBase
{
    private readonly ThuocService _service;

    public ThuocController(ThuocService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? maLoai)
    {
        return Ok(await _service.GetAllAsync(search, maLoai));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound(new { message = "Không tìm thấy thuốc" });

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ThuocDto dto)
    {
        return Ok(await _service.CreateAsync(dto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] ThuocDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (result == null)
            return NotFound(new { message = "Không tìm thấy thuốc" });

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!await _service.DeleteAsync(id))
            return NotFound(new { message = "Không tìm thấy thuốc" });

        return Ok(new { message = "Xóa thành công" });
    }
}
