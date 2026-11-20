using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.API.Data;
using QuanLyNhaThuoc.API.DTOs;
using QuanLyNhaThuoc.API.Helpers;
using QuanLyNhaThuoc.API.Models;

namespace QuanLyNhaThuoc.API.Services;

public class HoaDonService
{
    private readonly ApplicationDbContext _context;
    private readonly CodeGenerator _codeGen;

    public HoaDonService(ApplicationDbContext context, CodeGenerator codeGen)
    {
        _context = context;
        _codeGen = codeGen;
    }

    public async Task<HoaDon> CreateAsync(CreateHoaDonRequest request)
    {
        if (request.ChiTiet == null || !request.ChiTiet.Any())
            throw new Exception("Hóa đơn phải có ít nhất 1 sản phẩm");

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var khExists = await _context.KhachHangs.AnyAsync(x => x.MaKH == request.MaKH);
            if (!khExists)
                throw new Exception($"Không tìm thấy khách hàng {request.MaKH}");

            var maHD = await _codeGen.GenerateMaHDAsync();

            var hoaDon = new HoaDon
            {
                MaHD = maHD,
                NgayLapHD = DateTime.Now,
                MaKH = request.MaKH,
                TongTien = 0
            };

            decimal tongTien = 0;

            var nhomThuoc = request.ChiTiet
                .GroupBy(x => x.MaThuoc)
                .Select(g => new { MaThuoc = g.Key, SoLuongBan = g.Sum(i => i.SoLuongBan) });

            foreach (var item in nhomThuoc)
            {
                if (item.SoLuongBan <= 0)
                    throw new Exception($"Số lượng bán của thuốc {item.MaThuoc} phải lớn hơn 0");

                int soLuongCanBan = item.SoLuongBan;

                // Lấy các lô còn hàng và CHƯA HẾT HẠN, ưu tiên hạn sử dụng gần nhất (FEFO)
                var cacLo = await _context.LoThuocs
                    .Where(x => x.MaThuoc == item.MaThuoc
                             && x.SoLuongCon > 0
                             && x.HanSuDung >= DateTime.Today)
                    .OrderBy(x => x.HanSuDung)
                    .ThenBy(x => x.NgayNhapKho)
                    .ToListAsync();

                if (!cacLo.Any())
                    throw new Exception($"Thuốc {item.MaThuoc} đã hết hàng hoặc chỉ còn lô quá hạn");

                int tongCon = cacLo.Sum(x => x.SoLuongCon);
                if (tongCon < soLuongCanBan)
                    throw new Exception($"Thuốc {item.MaThuoc} không đủ số lượng khả dụng (còn {tongCon})");

                var thuoc = await _context.Thuocs.FindAsync(item.MaThuoc);
                decimal giaBan = thuoc?.DonGiaBan ?? 0;

                foreach (var lo in cacLo)
                {
                    if (soLuongCanBan <= 0) break;

                    int soLuongLayTuLo = Math.Min(soLuongCanBan, lo.SoLuongCon);

                    _context.ChiTietHoaDons.Add(new ChiTietHoaDon
                    {
                        MaHD = maHD,
                        MaLo = lo.MaLo,
                        SoLuongBan = soLuongLayTuLo,
                        GiaBan = giaBan
                    });

                    lo.SoLuongCon -= soLuongLayTuLo;
                    soLuongCanBan -= soLuongLayTuLo;

                    tongTien += soLuongLayTuLo * giaBan;
                }

                if (thuoc != null)
                {
                    thuoc.SoLuongTonKho -= item.SoLuongBan;
                }
            }

            hoaDon.TongTien = tongTien;
            _context.HoaDons.Add(hoaDon);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return hoaDon;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}