using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyNhaThuoc.API.DTOs;
using QuanLyNhaThuoc.API.Services;

namespace QuanLyNhaThuoc.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthService _service;

    public AuthController(AuthService service)
    {
        _service = service;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.TenDangNhap) ||
            string.IsNullOrWhiteSpace(request.MatKhau))
        {
            return BadRequest(new { message = "Cần nhập tên đăng nhập và mật khẩu" });
        }

        var response = await _service.LoginAsync(request);
        if (response == null)
            return Unauthorized(new { message = "Sai tên đăng nhập hoặc mật khẩu" });

        return Ok(response);
    }
}
