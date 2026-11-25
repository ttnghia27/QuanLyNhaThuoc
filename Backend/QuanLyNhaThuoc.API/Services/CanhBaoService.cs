using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.API.Data;
using QuanLyNhaThuoc.API.DTOs;

namespace QuanLyNhaThuoc.API.Services;

public sealed class CanhBaoService
{
    private readonly ApplicationDbContext _context;

    public CanhBaoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<CanhBaoSapHetHanItem>> GetSapHetHanAsync(int soNgay)
    {
        return _context.Database
            .SqlQuery<CanhBaoSapHetHanItem>(
                $"EXEC dbo.sp_CanhBaoSapHetHan @SoNgay = {soNgay}")
            .ToListAsync();
    }
}
