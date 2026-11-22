using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.API.Models;

namespace QuanLyNhaThuoc.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<LoaiThuoc> LoaiThuocs { get; set; }
    public DbSet<Thuoc> Thuocs { get; set; }
    public DbSet<NhaCC> NhaCCs { get; set; }
    public DbSet<PhieuNhap> PhieuNhaps { get; set; }
    public DbSet<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; }
    public DbSet<LoThuoc> LoThuocs { get; set; }
    public DbSet<KhachHang> KhachHangs { get; set; }
    public DbSet<HoaDon> HoaDons { get; set; }
    public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }
    public DbSet<NguoiDung> NguoiDungs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // LOAITHUOC
        modelBuilder.Entity<LoaiThuoc>(e =>
        {
            e.ToTable("LOAITHUOC");
            e.HasKey(x => x.MaLoai);
            e.Property(x => x.MaLoai).HasMaxLength(10);
            e.Property(x => x.TenLoai).HasMaxLength(100).IsRequired();
        });

        // THUOC
        modelBuilder.Entity<Thuoc>(e =>
        {
            e.ToTable("THUOC");
            e.HasKey(x => x.MaThuoc);
            e.Property(x => x.MaThuoc).HasMaxLength(10);
            e.Property(x => x.TenThuoc).HasMaxLength(200).IsRequired();
            e.Property(x => x.CongDung).HasMaxLength(500);
            e.Property(x => x.DonGiaBan).HasColumnType("decimal(18,0)");
            e.HasOne(x => x.LoaiThuoc)
                .WithMany(x => x.Thuocs)
                .HasForeignKey(x => x.MaLoai);
        });

        // NHACC
        modelBuilder.Entity<NhaCC>(e =>
        {
            e.ToTable("NHACC");
            e.HasKey(x => x.MaNhaCC);
            e.Property(x => x.MaNhaCC).HasMaxLength(10);
            e.Property(x => x.TenNhaCC).HasMaxLength(200).IsRequired();
            e.Property(x => x.SDTNhaCC).HasMaxLength(15);
        });

        // PHIEUNHAP
        modelBuilder.Entity<PhieuNhap>(e =>
        {
            e.ToTable("PHIEUNHAP");
            e.HasKey(x => x.MaPhieuNhap);
            e.Property(x => x.MaPhieuNhap).HasMaxLength(10);
            e.Property(x => x.TongTien).HasColumnType("decimal(18,0)");
            e.HasOne(x => x.NhaCC)
             .WithMany(x => x.PhieuNhaps)
             .HasForeignKey(x => x.MaNhaCC);
        });

        // CHITIETPHIEUNHAP
        modelBuilder.Entity<ChiTietPhieuNhap>(e =>
        {
            e.ToTable("CHITIETPHIEUNHAP");
            e.HasKey(x => new { x.MaPhieuNhap, x.MaThuoc });
            e.Property(x => x.DonGiaNhap).HasColumnType("decimal(18,0)");
            e.HasOne(x => x.PhieuNhap)
             .WithMany(x => x.ChiTietPhieuNhaps)
             .HasForeignKey(x => x.MaPhieuNhap);
            e.HasOne(x => x.Thuoc)
             .WithMany(x => x.ChiTietPhieuNhaps)
             .HasForeignKey(x => x.MaThuoc);
        });

        // LOTHUC
        modelBuilder.Entity<LoThuoc>(e =>
        {
            e.ToTable("LOTHUC");
            e.HasKey(x => x.MaLo);
            e.Property(x => x.MaLo).HasMaxLength(15);
            e.Property(x => x.DonGiaNhap).HasColumnType("decimal(18,0)");
            e.HasOne(x => x.Thuoc)
             .WithMany(x => x.LoThuocs)
             .HasForeignKey(x => x.MaThuoc);
            e.HasOne(x => x.PhieuNhap)
             .WithMany(x => x.LoThuocs)
             .HasForeignKey(x => x.MaPhieuNhap);
        });

        // KHACHHANG
        modelBuilder.Entity<KhachHang>(e =>
        {
            e.ToTable("KHACHHANG");
            e.HasKey(x => x.MaKH);
            e.Property(x => x.MaKH).HasMaxLength(10);
            e.Property(x => x.TenKH).HasMaxLength(100).IsRequired();
            e.Property(x => x.SDTKH).HasMaxLength(15);
            e.Property(x => x.DiaChiKH).HasMaxLength(300);
        });

        // HOADON
        modelBuilder.Entity<HoaDon>(e =>
        {
            e.ToTable("HOADON");
            e.HasKey(x => x.MaHD);
            e.Property(x => x.MaHD).HasMaxLength(10);
            e.Property(x => x.TongTien).HasColumnType("decimal(18,0)");
            e.HasOne(x => x.KhachHang)
             .WithMany(x => x.HoaDons)
             .HasForeignKey(x => x.MaKH);
        });

        // CHITIETHOADON
        modelBuilder.Entity<ChiTietHoaDon>(e =>
        {
            e.ToTable("CHITIETHOADON");
            e.HasKey(x => new { x.MaHD, x.MaLo });
            e.Property(x => x.GiaBan).HasColumnType("decimal(18,0)");
            e.HasOne(x => x.HoaDon)
             .WithMany(x => x.ChiTietHoaDons)
             .HasForeignKey(x => x.MaHD);
            e.HasOne(x => x.LoThuoc)
             .WithMany(x => x.ChiTietHoaDons)
             .HasForeignKey(x => x.MaLo);
        });

        // NGUOIDUNG
        modelBuilder.Entity<NguoiDung>(e =>
        {
            e.ToTable("NGUOIDUNG");
            e.HasKey(x => x.TenDangNhap);
            e.Property(x => x.TenDangNhap).HasMaxLength(50);
            e.Property(x => x.MatKhau).HasMaxLength(255);
            e.Property(x => x.HoTen).HasMaxLength(100);
        });
    }
}