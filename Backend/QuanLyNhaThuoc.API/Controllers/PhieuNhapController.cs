using Microsoft.AspNetCore.Mvc;
using QuanLyNhaThuoc.API.DTOs;
using QuanLyNhaThuoc.API.Services;

namespace QuanLyNhaThuoc.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PhieuNhapController : ControllerBase
{
    private readonly PhieuNhapService _service;

    public PhieuNhapController(PhieuNhapService service)
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
            return NotFound(new { message = "Không tìm thấy phiếu nhập" });

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePhieuNhapRequest request)
    {
        return Ok(await _service.CreateAsync(request));
    }
}
