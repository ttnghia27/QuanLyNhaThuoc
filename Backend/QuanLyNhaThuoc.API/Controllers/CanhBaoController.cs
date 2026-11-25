using Microsoft.AspNetCore.Mvc;
using QuanLyNhaThuoc.API.Services;

namespace QuanLyNhaThuoc.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CanhBaoController : ControllerBase
{
    private readonly CanhBaoService _service;

    public CanhBaoController(CanhBaoService service)
    {
        _service = service;
    }

    [HttpGet("sap-het-han")]
    public async Task<IActionResult> GetSapHetHan([FromQuery] int soNgay = 30)
    {
        return Ok(await _service.GetSapHetHanAsync(soNgay));
    }
}
