using Microsoft.AspNetCore.Mvc;
using QuanLyNhaThuoc.API.DTOs;
using QuanLyNhaThuoc.API.Services;

namespace QuanLyNhaThuoc.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class NhaCCController : ControllerBase
{
    private readonly NhaCCService _service;

    public NhaCCController(NhaCCService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound(new { message = "Không tìm thấy nhà cung cấp" });

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] NhaCCDto dto)
    {
        return Ok(await _service.CreateAsync(dto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] NhaCCDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (result == null)
            return NotFound(new { message = "Không tìm thấy nhà cung cấp" });

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!await _service.DeleteAsync(id))
            return NotFound(new { message = "Không tìm thấy nhà cung cấp" });

        return Ok(new { message = "Xóa thành công" });
    }
}
