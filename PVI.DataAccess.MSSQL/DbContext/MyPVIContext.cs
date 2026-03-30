using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace PVI.DAO.Entities.Models;

public partial class MY_PVIContext : DbContext
{
    public MY_PVIContext()
    {
    }

    public MY_PVIContext(DbContextOptions<MY_PVIContext> options)
        : base(options)
    {
    }

    public virtual DbSet<KbttCtu> KbttCtus { get; set; } = null!;
    public virtual DbSet<DmUserMYPVI> DmUsers { get; set; } = null!;
    public virtual DbSet<KbttAnh> KbttAnhs { get; set; } = null!;
    public virtual DbSet<KbttCt> KbttCts { get; set; } = null!;
    public virtual DbSet<DmXe> DmXes { get; set; } = null!;
    public string connectMyPVI = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["MyPVIContext"]!;


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {

            optionsBuilder.UseSqlServer(connectMyPVI);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KbttCtu>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("kbtt_ctu");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");

            entity.Property(e => e.BienKsoat)
                .HasMaxLength(30)
                .HasColumnName("bien_ksoat")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CoquanGquyet)
                .HasMaxLength(500)
                .HasColumnName("coquan_gquyet")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.DangKiem)
                .HasMaxLength(50)
                .HasColumnName("dang_kiem")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.DiaChiKh)
                .HasMaxLength(500)
                .HasColumnName("dia_chi_kh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.DiaDiemtt)
                .HasMaxLength(500)
                .HasColumnName("dia_diemtt")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.DiaDiemttkh)
                .HasMaxLength(500)
                .HasColumnName("dia_diemttkh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.DienThoaiSeri)
                .HasMaxLength(50)
                .HasColumnName("dien_thoai_seri")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.DienthoaiLienhe)
                .HasMaxLength(30)
                .HasColumnName("dienthoai_lienhe")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.GaraGiamdinh)
                .HasMaxLength(50)
                .HasColumnName("gara_giamdinh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.GaraSuachua)
                .HasMaxLength(100)
                .HasColumnName("gara_suachua")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.Gplx)
                .HasMaxLength(50)
                .HasColumnName("gplx")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.GuiEmailKh).HasColumnName("gui_email_kh");

            entity.Property(e => e.HangXe)
                .HasMaxLength(50)
                .HasColumnName("hang_xe")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.HauquaNguoi)
                .HasMaxLength(500)
                .HasColumnName("hauqua_nguoi")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.HauquaNguoikh)
                .HasMaxLength(500)
                .HasColumnName("hauqua_nguoikh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.HauquaTsan)
                .HasMaxLength(500)
                .HasColumnName("hauqua_tsan")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.HauquaTsankh)
                .HasMaxLength(500)
                .HasColumnName("hauqua_tsankh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.IsChuXe)
                .HasColumnName("is_chu_xe")
                .HasDefaultValueSql("((0))");

            entity.Property(e => e.IsdonviDuyet).HasColumnName("isdonvi_duyet");

            entity.Property(e => e.LoaiChupanh).HasColumnName("loai_chupanh");

            entity.Property(e => e.LoaiKbtt)
                .HasColumnName("loai_kbtt")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.LoaihinhBh)
                .HasMaxLength(50)
                .HasColumnName("loaihinh_bh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaCbkt)
                .HasMaxLength(20)
                .HasColumnName("ma_cbkt")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaDieukhoanTnds)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("ma_dieukhoan_tnds")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaDongxe)
                .HasMaxLength(10)
                .HasColumnName("ma_dongxe")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaDonvi)
                .HasMaxLength(10)
                .HasColumnName("ma_donvi")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaDonviChuyen)
                .HasMaxLength(20)
                .HasColumnName("ma_donvi_chuyen")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaKh)
                .HasMaxLength(20)
                .HasColumnName("ma_kh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaKthac)
                .HasMaxLength(20)
                .HasColumnName("ma_kthac")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaLoaixe)
                .HasMaxLength(50)
                .HasColumnName("ma_loaixe")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaNhloaixe)
                .HasMaxLength(20)
                .HasColumnName("ma_nhloaixe")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaPkt)
                .HasMaxLength(20)
                .HasColumnName("ma_pkt")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaUser)
                .HasMaxLength(50)
                .HasColumnName("ma_user")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaUserGdv)
                .HasMaxLength(50)
                .HasColumnName("ma_user_gdv")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.NamSx)
                .HasColumnName("nam_sx")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.NgGdichTh)
                .HasMaxLength(500)
                .HasColumnName("ng_gdich_th");

            entity.Property(e => e.NgayCapSeri)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cap_seri");

            entity.Property(e => e.NgayCuoiDk)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cuoi_dk")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.NgayCuoiSeri)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cuoi_seri");

            entity.Property(e => e.NgayDauDk)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_dau_dk")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.NgayDauSeri)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_dau_seri");

            entity.Property(e => e.NgayGdinh)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_gdinh");

            entity.Property(e => e.NgayHengd)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_hengd");

            entity.Property(e => e.NgayKbtt)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_kbtt");

            entity.Property(e => e.NgaySuachua)
                .HasColumnType("datetime")
                .HasColumnName("ngay_suachua");

            entity.Property(e => e.NgayTthat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_tthat");

            entity.Property(e => e.NgayTthatkh)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_tthatkh");

            entity.Property(e => e.NgaybatdauGplx)
                .HasColumnType("datetime")
                .HasColumnName("ngaybatdau_gplx");

            entity.Property(e => e.NgaykethucGplx)
                .HasColumnType("datetime")
                .HasColumnName("ngaykethuc_gplx");

            entity.Property(e => e.NguoiLienhe)
                .HasMaxLength(300)
                .HasColumnName("nguoi_lienhe")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.NguyenNhanTtat)
                .HasMaxLength(500)
                .HasColumnName("nguyen_nhan_ttat")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.NguyenNhanTtatkh)
                .HasMaxLength(500)
                .HasColumnName("nguyen_nhan_ttatkh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.NhanHieu)
                .HasMaxLength(10)
                .HasColumnName("nhan_hieu")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.PrKeyBt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_bt");

            entity.Property(e => e.PrKeyGdtt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_gdtt");

            entity.Property(e => e.PrKeySeri)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_seri");

            entity.Property(e => e.QuanHuyen)
                .HasMaxLength(500)
                .HasColumnName("quan_huyen")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.SoCngoi)
                .HasMaxLength(50)
                .HasColumnName("so_cngoi")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.SoDonbh)
                .HasMaxLength(100)
                .HasColumnName("so_donbh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.SoHsgd)
                .HasMaxLength(20)
                .HasColumnName("so_hsgd")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.SoSeri).HasColumnName("so_seri");

            entity.Property(e => e.SoTienugd)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienugd");

            entity.Property(e => e.TaoQuaApp).HasColumnName("tao_qua_app");

            entity.Property(e => e.TenDonvi)
                .HasMaxLength(100)
                .HasColumnName("ten_donvi")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.TenKhach)
                .HasMaxLength(500)
                .HasColumnName("ten_khach")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.TenLaiXe)
                .HasMaxLength(300)
                .HasColumnName("ten_lai_xe")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.TenNguoiKy)
                .HasMaxLength(300)
                .HasColumnName("ten_nguoi_ky")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.TenUser)
                .HasMaxLength(50)
                .HasColumnName("ten_user")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.TengaraGdinh)
                .HasMaxLength(500)
                .HasColumnName("tengara_gdinh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.TengaraSuachua)
                .HasMaxLength(500)
                .HasColumnName("tengara_suachua")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ThoigianTthat)
                .HasMaxLength(50)
                .HasColumnName("thoigian_tthat")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ThoigianTthatkh)
                .HasMaxLength(50)
                .HasColumnName("thoigian_tthatkh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.TinhThanh)
                .HasMaxLength(500)
                .HasColumnName("tinh_thanh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.TinhTrang)
                .HasColumnName("tinh_trang")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.TinhTrangCaiapp).HasColumnName("tinh_trang_caiapp");

            entity.Property(e => e.TrongTai)
                .HasMaxLength(10)
                .HasColumnName("trong_tai")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.UserId)
                .HasMaxLength(10)
                .HasColumnName("user_id")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.YeucauKhac)
                .HasMaxLength(1500)
                .HasColumnName("yeucau_khac")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.YeucauQuydinh)
                .HasMaxLength(1500)
                .HasColumnName("yeucau_quydinh")
                .HasDefaultValueSql("('')");
        });
        modelBuilder.Entity<DmUserMYPVI>(entity =>
        {
            entity.ToTable("dm_user");

            entity.HasIndex(e => e.DienThoai, "dm_user_dien_thoai");

            entity.HasIndex(e => e.MaUser, "dm_user_ma_user_IND");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.DiaChi)
                .HasMaxLength(200)
                .HasColumnName("dia_chi");

            entity.Property(e => e.DienThoai)
                .HasMaxLength(12)
                .HasColumnName("dien_thoai")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");

            entity.Property(e => e.Imei)
                .HasMaxLength(100)
                .HasColumnName("imei")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.LoaiUser).HasColumnName("loai_user");

            entity.Property(e => e.MaGt)
                .HasMaxLength(100)
                .HasColumnName("ma_gt")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaUser)
                .HasMaxLength(50)
                .HasColumnName("ma_user")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.NgayCapnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_capnhat");

            entity.Property(e => e.NgaySinh)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_sinh");

            entity.Property(e => e.NgayTao)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_tao");

            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.QuanHuyen)
                .HasMaxLength(10)
                .HasColumnName("quan_huyen")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.TenUser)
                .HasMaxLength(200)
                .HasColumnName("ten_user")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.TimeLogin)
                .HasColumnType("smalldatetime")
                .HasColumnName("time_login");

            entity.Property(e => e.TimeLogout)
                .HasColumnType("smalldatetime")
                .HasColumnName("time_logout");

            entity.Property(e => e.TinhTp)
                .HasMaxLength(10)
                .HasColumnName("tinh_tp")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.TokenKey)
                .HasMaxLength(100)
                .HasColumnName("token_key");

            entity.Property(e => e.TrangThai)
                .HasColumnName("trang_thai")
                .HasDefaultValueSql("((1))");
        });
        modelBuilder.Entity<KbttAnh>(entity =>
        {
            entity.HasKey(e => e.PrKey)
                .HasName("PK_kbtt_ct");

            entity.ToTable("kbtt_anh");

            entity.HasIndex(e => e.FrKey, "idx_fr_key");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");

            entity.Property(e => e.BhcnCtuId).HasColumnName("bhcn_ctu_id");

            entity.Property(e => e.FrKey).HasColumnName("fr_key");

            entity.Property(e => e.KinhDo)
                .HasMaxLength(50)
                .HasColumnName("kinh_do")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.LoaiAnh).HasColumnName("loai_anh");

            entity.Property(e => e.LoaiBh).HasColumnName("loai_bh");

            entity.Property(e => e.MaHmuc)
                .HasMaxLength(50)
                .HasColumnName("ma_hmuc")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.NgayChup)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_chup");

            entity.Property(e => e.NgayUpload)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_upload");

            entity.Property(e => e.NguonTao)
                .HasMaxLength(50)
                .HasColumnName("nguon_tao")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.Path)
                .HasMaxLength(200)
                .HasColumnName("path")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.PathThumnail)
                .HasMaxLength(200)
                .HasColumnName("path_thumnail")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.Url)
                .HasMaxLength(200)
                .HasColumnName("url")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.UrlThumnail)
                .HasMaxLength(200)
                .HasColumnName("url_thumnail")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ViDo)
                .HasMaxLength(50)
                .HasColumnName("vi_do")
                .HasDefaultValueSql("('')");
        });

        modelBuilder.Entity<KbttCt>(entity =>
        {
            entity.HasKey(e => e.PrKey)
                .HasName("PK_kbtt_ct1");

            entity.ToTable("kbtt_ct");

            entity.HasIndex(e => e.FrKey, "kbtt_ct_fr_key_IND");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");

            entity.Property(e => e.FrKey).HasColumnName("fr_key");

            entity.Property(e => e.MaHmuc)
                .HasMaxLength(10)
                .HasColumnName("ma_hmuc")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaNhmuc)
                .HasMaxLength(10)
                .HasColumnName("ma_nhmuc")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.NgayTao)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_tao");

            entity.Property(e => e.Stt).HasColumnName("stt");

            entity.Property(e => e.TenHmuc)
                .HasMaxLength(50)
                .HasColumnName("ten_hmuc")
                .HasDefaultValueSql("('')");
        });
        modelBuilder.Entity<DmXe>(entity =>
        {
            entity.HasNoKey();

            entity.ToTable("dm_xe");

            entity.Property(e => e.MaHieuxe)
                .HasMaxLength(10)
                .HasColumnName("ma_hieuxe")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaLoaixe)
                .HasMaxLength(10)
                .HasColumnName("ma_loaixe")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.TenHieuxe)
                .HasMaxLength(50)
                .HasColumnName("ten_hieuxe")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.TenLoaixe)
                .HasMaxLength(100)
                .HasColumnName("ten_loaixe")
                .HasDefaultValueSql("('')");
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

