using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.API.Data;
using QuanLyNhaThuoc.API.DTOs;

namespace QuanLyNhaThuoc.API.Services;

public sealed class ThongKeService
{
    private static readonly HashSet<int> AllowedMonths = [1, 3, 6, 12];
    private readonly ApplicationDbContext _context;

    public ThongKeService(ApplicationDbContext context)
    {
        _context = context;
    }
    // public async Task<TongQuan> GetTongQuanAsync()
    // {

    //     var homNay = DateTime.Today;
    //     var ngayCanhBao = homNay.AddDays(30);

    //     var doanhThuHomNay = await _context.HoaDons
    //         .Where(x => x.NgayLapHD >= homNay)
    //         .SumAsync(x => (decimal?)x.TongTien) ?? 0;

    //     var tongDoanhThu = await _context.HoaDons
    //         .SumAsync(x => (decimal?)x.TongTien) ?? 0;

    //     var tongTienNhap = await _context.PhieuNhaps
    //         .SumAsync(x => (decimal?)x.TongTien) ?? 0;

    //     var soHoaDon = await _context.HoaDons.CountAsync();
    //     var soLoaiThuoc = await _context.Thuocs.CountAsync();
    //     var thuocSapHetHang = await _context.Thuocs.CountAsync(x => x.SoLuongTonKho <= 10);
    //     var loSapHetHan = await _context.LoThuocs.CountAsync(
    //         x => x.SoLuongCon > 0 && x.HanSuDung <= ngayCanhBao);

    //     return new TongQuan(
    //         doanhThuHomNay,
    //         tongDoanhThu,
    //         tongTienNhap,
    //         tongDoanhThu - tongTienNhap,
    //         soHoaDon,
    //         soLoaiThuoc,
    //         thuocSapHetHang,
    //         loSapHetHan);
    // }
    public async Task<TongQuanDTO> GetTongQuanAsync(int months = 1)
    {
        if (!AllowedMonths.Contains(months))
        {
            throw new ArgumentException("Tham số months chỉ hỗ trợ các giá trị: 1, 3, 6, 12.", nameof(months));
        }

        var results = await _context.Database
            .SqlQuery<TongQuanDTO>($"EXEC dbo.sp_ThongKeTongQuan @Months = {months}")
            .ToListAsync();

        return results.Single();
    }

    public Task<List<DoanhThuNgayItem>> GetDoanhThuTheoNgayAsync(DateTime? tuNgay, DateTime? denNgay)
    {
        var start = (tuNgay ?? DateTime.Today.AddDays(-30)).Date;
        var end = (denNgay ?? DateTime.Today).Date;

        return _context.Database
            .SqlQuery<DoanhThuNgayItem>(
                $"EXEC dbo.sp_ThongKeDoanhThu @TuNgay = {start}, @DenNgay = {end}")
            .ToListAsync();
    }
}
