using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.API.Data;
using QuanLyNhaThuoc.API.DTOs;
using QuanLyNhaThuoc.API.Helpers;
using QuanLyNhaThuoc.API.Models;

namespace QuanLyNhaThuoc.API.Services;

public class PhieuNhapService
{
    private readonly ApplicationDbContext _context;
    private readonly CodeGenerator _codeGen;

    public PhieuNhapService(ApplicationDbContext context, CodeGenerator codeGen)
    {
        _context = context;
        _codeGen = codeGen;
    }

    public async Task<PhieuNhap> CreateAsync(CreatePhieuNhapRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var maPN = await _codeGen.GenerateMaPhieuNhapAsync();

            var phieuNhap = new PhieuNhap
            {
                MaPhieuNhap = maPN,
                NgayNhap = request.NgayNhap ?? DateTime.Today,
                MaNhaCC = request.MaNhaCC,
                TongTien = 0
            };

            decimal tongTien = 0;

            foreach (var ct in request.ChiTiet)
            {
                // 1. Thêm chi tiết phiếu nhập
                var chiTiet = new ChiTietPhieuNhap
                {
                    MaPhieuNhap = maPN,
                    MaThuoc = ct.MaThuoc,
                    SoLuongNhap = ct.SoLuongNhap,
                    DonGiaNhap = ct.DonGiaNhap
                };
                _context.ChiTietPhieuNhaps.Add(chiTiet);

                // 2. Tự động tạo Lô thuốc
                var maLo = await _codeGen.GenerateMaLoAsync();
                var lo = new LoThuoc
                {
                    MaLo = maLo,
                    MaThuoc = ct.MaThuoc,
                    MaPhieuNhap = maPN,
                    NgayNhapKho = phieuNhap.NgayNhap,
                    HanSuDung = ct.HanSuDung,
                    SoLuongNhap = ct.SoLuongNhap,
                    SoLuongCon = ct.SoLuongNhap,
                    DonGiaNhap = ct.DonGiaNhap
                };
                _context.LoThuocs.Add(lo);

                // 3. Cập nhật tồn kho thuốc
                var thuoc = await _context.Thuocs.FindAsync(ct.MaThuoc);
                if (thuoc != null)
                {
                    thuoc.SoLuongTonKho += ct.SoLuongNhap;
                }

                tongTien += ct.SoLuongNhap * ct.DonGiaNhap;
            }

            phieuNhap.TongTien = tongTien;
            _context.PhieuNhaps.Add(phieuNhap);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return phieuNhap;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}