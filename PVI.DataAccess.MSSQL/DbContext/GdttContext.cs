using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PVI.Helper;

namespace PVI.DAO.Entities.Models;

public partial class GdttContext : DbContext
{
    public GdttContext()
    {
    }

    public GdttContext(DbContextOptions<GdttContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CrmCase> CrmCases { get; set; }

    public virtual DbSet<DmCtugd> DmCtugds { get; set; }

    public virtual DbSet<DmDevice> DmDevices { get; set; }

    public virtual DbSet<DmDiemtruc> DmDiemtrucs { get; set; }

    public virtual DbSet<DmDonvi> DmDonvis { get; set; }

    public virtual DbSet<DmGaRa> DmGaRas { get; set; }

    public virtual DbSet<DmGaraKhuvuc> DmGaraKhuvucs { get; set; }

    public virtual DbSet<DmGhHstpc> DmGhHstpcs { get; set; }

    public virtual DbSet<DmHieuxe> DmHieuxes { get; set; }

    public virtual DbSet<DmHmuc> DmHmucs { get; set; }

    public virtual DbSet<DmHmucGiamdinh> DmHmucGiamdinhs { get; set; }

    public virtual DbSet<DmKhuvuc> DmKhuvucs { get; set; }

    public virtual DbSet<DmLhsbt> DmLhsbts { get; set; }

    public virtual DbSet<DmListPhone> DmListPhones { get; set; }

    public virtual DbSet<DmLoaiUser> DmLoaiUsers { get; set; }

    public virtual DbSet<DmLoaixe> DmLoaixes { get; set; }

    public virtual DbSet<DmLoaiHsgd> DmLoaiHsgds { get; set; }

    public virtual DbSet<DmNguyennhanTonthat> DmNguyennhanTonthats { get; set; }

    public virtual DbSet<DmNhmuc> DmNhmucs { get; set; }

    public virtual DbSet<DmPquyenKyhs> DmPquyenKyhs { get; set; }

    public virtual DbSet<DmStatusInstall> DmStatusInstalls { get; set; }

    public virtual DbSet<DmTinh> DmTinhs { get; set; }

    public virtual DbSet<DmTongthanhxe> DmTongthanhxes { get; set; }

    public virtual DbSet<DmTtrangGd> DmTtrangGds { get; set; }

    public virtual DbSet<DmTtrangTtrinh> DmTtrangTtrinhs { get; set; }

    public virtual DbSet<DmUqHstpc> DmUqHstpcs { get; set; }

    public virtual DbSet<DmUser> DmUsers { get; set; }

    public virtual DbSet<DmVersion> DmVersions { get; set; }

    public virtual DbSet<ErrorLog> ErrorLogs { get; set; }

    public virtual DbSet<GddkCt> GddkCts { get; set; }

    public virtual DbSet<GddkCtu> GddkCtus { get; set; }

    public virtual DbSet<GhichuLichtruc> GhichuLichtrucs { get; set; }

    public virtual DbSet<HsgdCt> HsgdCts { get; set; }

    public virtual DbSet<HsgdCtu> HsgdCtus { get; set; }

    public virtual DbSet<HsgdDg> HsgdDgs { get; set; }

    public virtual DbSet<HsgdDgCt> HsgdDgCts { get; set; }

    public virtual DbSet<HsgdDx> HsgdDxes { get; set; }

    public virtual DbSet<HsgdDxTsk> HsgdDxTsks { get; set; }

    public virtual DbSet<HsgdLsu> HsgdLsus { get; set; }

    public virtual DbSet<HsgdTtrinh> HsgdTtrinhs { get; set; }

    public virtual DbSet<HsgdTtrinhTt> HsgdTtrinhTt { get; set; }

    public virtual DbSet<HsgdTtrinhCt> HsgdTtrinhCts { get; set; }

    public virtual DbSet<HsgdTtrinhNky> HsgdTtrinhNkies { get; set; }

    public virtual DbSet<LichTrucgdv> LichTrucgdvs { get; set; }

    public virtual DbSet<LichsuPa> LichsuPas { get; set; }

    public virtual DbSet<LichtrucCtu> LichtrucCtus { get; set; }

    public virtual DbSet<LichtrucOld> LichtrucOlds { get; set; }

    public virtual DbSet<LsuDangnhap> LsuDangnhaps { get; set; }

    public virtual DbSet<NhatKy> NhatKies { get; set; }

    public virtual DbSet<NhatKyGddk> NhatKyGddks { get; set; }

    public virtual DbSet<PquyenCnang> PquyenCnangs { get; set; }
    public virtual DbSet<HsgdDxCt> HsgdDxCts { get; set; }
    public virtual DbSet<DmLdonBt> DmLdonBts { get; set; } = null!;
    public virtual DbSet<DmLoaiBang> DmLoaiBangs { get; set; }
    public virtual DbSet<HsgdDntt> HsgdDntts { get; set; }
    public virtual DbSet<DmUserTtoan> DmUserTtoans { get; set; }
    public virtual DbSet<HsgdTotrinhXml> HsgdTotrinhXmls { get; set; }
    public virtual DbSet<DmLoaiDongco> DmLoaiDongcos { get; set; }
    public virtual DbSet<HsgdAttachFile> HsgdAttachFiles { get; set; }
    public virtual DbSet<HsgdTbbt> HsgdTbbts { get; set; }
    public virtual DbSet<HsgdTbbtTt> HsgdTbbtTts { get; set; }
    public DbSet<ThongKeGDTT_Item> ThongKeGDTT_Items { get; set; }
    public string connect_gdtt = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["GdttContext"]!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(connect_gdtt);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CrmCase>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            object value = entity.ToTable("crm_case");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.BienKsoat)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("bien_ksoat");
            entity.Property(e => e.DiaDiemtt)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("dia_diemtt");
            entity.Property(e => e.DienThoai)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("dien_thoai");
            entity.Property(e => e.DienthoaiNgtao)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("dienthoai_ngtao");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chu");
            entity.Property(e => e.HauQuaTt)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("hau_qua_tt");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi");
            entity.Property(e => e.NgLienhe)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("ng_lienhe");
            entity.Property(e => e.NgayCuoiSeri)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cuoi_seri");
            entity.Property(e => e.NgayDauSeri)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_dau_seri");
            entity.Property(e => e.NgayTao)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_tao");
            entity.Property(e => e.NgayTbao)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_tbao");
            entity.Property(e => e.NgayTthat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_tthat");
            entity.Property(e => e.NguoiTao)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("nguoi_tao");
            entity.Property(e => e.NguyenNhanTtat)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("nguyen_nhan_ttat");
            entity.Property(e => e.PrKeyHsgd).HasColumnName("pr_key_hsgd");
            entity.Property(e => e.SoDonbh)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_donbh");
            entity.Property(e => e.SoSeri).HasColumnName("so_seri");
            entity.Property(e => e.TicketId).HasColumnName("ticketID");
        });

        modelBuilder.Entity<DmCtugd>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("dm_ctugd");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.MaCtugd)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ctugd");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi");
            entity.Property(e => e.Num)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("num");
        });

        modelBuilder.Entity<DmDevice>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("dm_device");

            entity.Property(e => e.PrKey)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("((1))")
                .HasColumnName("active");
            entity.Property(e => e.AddressDevice)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("address_device");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("description");
            entity.Property(e => e.ImeiDevice)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("imei_device");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi");
            entity.Property(e => e.MaUser)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.NgayCnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cnhat");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TypeDevice)
                .HasMaxLength(12)
                .HasColumnName("type_device");
        });

        modelBuilder.Entity<DmDiemtruc>(entity =>
        {
            entity.HasKey(e => new { e.PrKey, e.MaDiemtruc });

            entity.ToTable("dm_diemtruc");

            entity.Property(e => e.PrKey)
                .ValueGeneratedOnAdd()
                .HasColumnName("pr_key");
            entity.Property(e => e.MaDiemtruc)
                .HasMaxLength(30)
                .HasColumnName("Ma_diemtruc");
            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("((1))")
                .HasColumnName("active");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("description");
            entity.Property(e => e.MaUser)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.NgayCnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cnhat");
            entity.Property(e => e.TenDiemtruc)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("Ten_diemtruc");
        });

        modelBuilder.Entity<DmDonvi>(entity =>
        {
            entity.HasKey(e => e.MaDonvi);

            entity.ToTable("dm_donvi");

            entity.Property(e => e.MaDonvi)
                .HasMaxLength(8)
                .HasColumnName("ma_donvi");
            entity.Property(e => e.MaDvchuquan)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_dvchuquan");
            entity.Property(e => e.MaKh)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_kh");
            entity.Property(e => e.TenDonvi)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_donvi");
        });

        modelBuilder.Entity<DmGaRa>(entity =>
        {
            entity
                //.HasNoKey()
                .ToTable("dm_ga_ra");

            entity.Property(e => e.DiaChi)
                .HasMaxLength(200)
                .HasColumnName("dia_chi");
            entity.Property(e => e.DiaChiXuong)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("dia_chi_xuong");
            entity.Property(e => e.DienThoaiGara)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("dien_thoai_gara");
            entity.Property(e => e.EmailGara)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("email_gara");
            entity.Property(e => e.GaraTthai).HasColumnName("gara_tthai");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(8)
                .HasColumnName("ma_donvi");
            entity.Property(e => e.MaGara)
                .HasMaxLength(11)
                .HasColumnName("ma_gara");
            entity.Property(e => e.MaUsercNhat)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_userc_nhat");
            entity.Property(e => e.NgayCnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cnhat");
            entity.Property(e => e.QuanHuyen)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("quan_huyen");
            entity.Property(e => e.TenGara)
                .HasMaxLength(150)
                .HasColumnName("ten_gara");
            entity.Property(e => e.TenTat)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_tat");
            entity.Property(e => e.TenTinh)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_tinh");
            entity.Property(e => e.TyleggPhutung)
                .HasDefaultValueSql("((0.00))")
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tylegg_phutung");
            entity.Property(e => e.TyleggSuachua)
                .HasDefaultValueSql("((0.00))")
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tylegg_suachua");
            entity.Property(e => e.SongayThanhtoan)
                .HasDefaultValueSql("((0))")
                .HasColumnType("int")
                .HasColumnName("songay_thanhtoan");
            entity.Property(e => e.bnkCode)
                .IsRequired()
                .HasMaxLength(30)
                .HasDefaultValueSql("('')")
                .HasColumnName("bnkCode");
            entity.Property(e => e.ten_ctk)
               .IsRequired()
               .HasMaxLength(300)
               .HasDefaultValueSql("('')")
               .HasColumnName("ten_ctk");
            entity.Property(e => e.thoa_thuan_hop_tac)
               .HasDefaultValueSql("((0))")
               .HasColumnName("thoa_thuan_hop_tac");
        });

        modelBuilder.Entity<DmGaraKhuvuc>(entity =>
        {
            entity.HasKey(e => e.PrKey).HasName("PK_khuvuc_gara");

            entity.ToTable("dm_gara_khuvuc");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi");
            entity.Property(e => e.MaGara)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_gara");
            entity.Property(e => e.MaKv)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_kv");
            entity.Property(e => e.MaUser)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.NgayCapnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_capnhat");
            entity.Property(e => e.Stt).HasColumnName("stt");
            entity.Property(e => e.SuDung)
                .IsRequired()
                .HasDefaultValueSql("((1))")
                .HasColumnName("su_dung");
            entity.Property(e => e.TenGara)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_gara");
            entity.Property(e => e.TenKv)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_kv");
        });

        modelBuilder.Entity<DmGhHstpc>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("dm_gh_hstpc");

            entity.Property(e => e.GhSotientpc)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("gh_sotientpc");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi");
            entity.Property(e => e.MaUser)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.NgayCapnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_capnhat");
            entity.Property(e => e.NgayHl)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_hl");
        });

        modelBuilder.Entity<DmHieuxe>(entity =>
        {
            entity.HasKey(e => e.PrKey).HasName("PK_dm_hangxe");

            entity.ToTable("dm_hieuxe");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.HieuXe)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("hieu_xe");
        });

        modelBuilder.Entity<DmHmuc>(entity =>
        {
            entity.HasKey(e => e.MaHmuc).HasName("PK_hang_mucxe");

            entity.ToTable("dm_hmuc");

            entity.Property(e => e.MaHmuc)
                .HasMaxLength(30)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_hmuc");
            entity.Property(e => e.MaNhmuc)
                .HasMaxLength(30)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_nhmuc");
            entity.Property(e => e.MaTongthanhxe)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_tongthanhxe");
            entity.Property(e => e.MaUser)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.NgayCapnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_capnhat");
            entity.Property(e => e.SuDung)
                .HasDefaultValueSql("((1))")
                .HasColumnName("su_dung");
            entity.Property(e => e.TenHmuc)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_hmuc");
        });

        modelBuilder.Entity<DmHmucGiamdinh>(entity =>
        {
            entity.HasKey(e => e.MaHmuc);

            entity.ToTable("dm_hmuc_giamdinh");

            entity.Property(e => e.MaHmuc)
                .HasMaxLength(4)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_hmuc");
            entity.Property(e => e.MaUser)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.NgayCapnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_capnhat");
            entity.Property(e => e.TenHmuc)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_hmuc");
        });

        modelBuilder.Entity<DmKhuvuc>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("dm_khuvuc");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi");
            entity.Property(e => e.MaKv)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_kv");
            entity.Property(e => e.MaUser)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.MotaDiadiem)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("mota_diadiem");
            entity.Property(e => e.NgayTao)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_tao");
            entity.Property(e => e.QuanHuyen)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("quan_huyen");
            entity.Property(e => e.SuDung)
                .IsRequired()
                .HasDefaultValueSql("((1))")
                .HasColumnName("su_dung");
            entity.Property(e => e.TenKv)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_kv");
            entity.Property(e => e.Tinhtp)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("tinhtp");
        });

        modelBuilder.Entity<DmLhsbt>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("dm_lhsbt");

            entity.Property(e => e.MaLhsbt)
                .HasMaxLength(8)
                .HasColumnName("ma_lhsbt");
            entity.Property(e => e.MaUser)
                .HasMaxLength(10)
                .HasColumnName("ma_user");
            entity.Property(e => e.NgayCnhat)
                .HasColumnType("datetime")
                .HasColumnName("ngay_cnhat");
            entity.Property(e => e.TenLhsbt)
                .HasMaxLength(50)
                .HasColumnName("ten_lhsbt");
        });

        modelBuilder.Entity<DmListPhone>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("dm_list_phone");

            entity.Property(e => e.PrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.MaUser)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("phone");
        });

        modelBuilder.Entity<DmLoaiUser>(entity =>
        {
            entity.ToTable("dm_loai_user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LoaiUser).HasColumnName("loai_user");
            entity.Property(e => e.NgayCnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cnhat");
            entity.Property(e => e.TenLoaiUser)
                .HasMaxLength(100)
                .HasColumnName("ten_loai_user");
        });

        // Thêm model cho loại hồ sơ giám định.
        // khanhlh - 28/08/2024

        modelBuilder.Entity<DmLoaiHsgd>(entity =>
        {
            entity.HasKey(e => e.ma_loai_hsgd);
            entity.ToTable("dm_loai_hsgd");
            entity.Property(e => e.ma_loai_hsgd).HasColumnName("ma_loai_hsgd").HasMaxLength(11);
            entity.Property(e => e.ten_loai_hsgd).HasColumnName("ten_loai_hsgd");
        });

        modelBuilder.Entity<DmLoaixe>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("dm_loaixe");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.FrKey).HasColumnName("fr_key");
            entity.Property(e => e.LoaiXe)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("loai_xe");
            entity.Property(e => e.MaUser)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.NgayCapnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_capnhat");
        });

        modelBuilder.Entity<DmNguyennhanTonthat>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("dm_nguyennhan_tonthat");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.MaNntt)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_nntt");
            entity.Property(e => e.TenNntt)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_nntt");
        });

        modelBuilder.Entity<DmNhmuc>(entity =>
        {
            entity.HasKey(e => e.MaNhmuc).HasName("PK_nhom_hmucxe");

            entity.ToTable("dm_nhmuc");

            entity.Property(e => e.MaNhmuc)
                .HasMaxLength(30)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_nhmuc");
            entity.Property(e => e.MaTongthanhxe)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_tongthanhxe");
            entity.Property(e => e.MaUser)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.NgayCapnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_capnhat");
            entity.Property(e => e.SuDung)
                .HasDefaultValueSql("((1))")
                .HasColumnName("su_dung");
            entity.Property(e => e.TenNhmuc)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_nhmuc");
        });

        modelBuilder.Entity<DmPquyenKyhs>(entity =>
        {
            entity
                //.HasNoKey()
                .ToTable("dm_pquyen_kyhs");

            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.MaSp)
                .HasMaxLength(150)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_sp");
            entity.Property(e => e.MaUser)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.MaUserCapnhat)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user_capnhat");
            entity.Property(e => e.NgayCnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cnhat");
            entity.Property(e => e.PrKey)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("pr_key");
            entity.Property(e => e.SoTien)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tien");
        });

        modelBuilder.Entity<DmStatusInstall>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("dm_statusInstall");

            entity.Property(e => e.PrKey)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.ImeiDevice)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("imei_device");
            entity.Property(e => e.NgayCnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cnhat");
            entity.Property(e => e.Status)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("status");
        });

        modelBuilder.Entity<DmTinh>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("dm_tinh");

            entity.Property(e => e.MaTinh)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_tinh");
            entity.Property(e => e.TenTinh)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_tinh");
            entity.Property(e => e.TongHop).HasColumnName("tong_hop");
            entity.Property(e => e.SuDung).HasColumnName("su_dung");
        });

        modelBuilder.Entity<DmTongthanhxe>(entity =>
        {
            entity.HasKey(e => e.MaTongthanhxe);

            entity.ToTable("dm_tongthanhxe");

            entity.Property(e => e.MaTongthanhxe)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_tongthanhxe");
            entity.Property(e => e.MaUser)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.NgayCapnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_capnhat");
            entity.Property(e => e.TenTongthanhxe)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_tongthanhxe");
        });

        modelBuilder.Entity<DmTtrangGd>(entity =>
        {
            entity.HasKey(e => e.MaTtrangGd);

            entity.ToTable("dm_ttrang_gd");

            entity.Property(e => e.MaTtrangGd)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ttrang_gd");
            entity.Property(e => e.TenTtrangGd)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_ttrang_gd");
        });

        modelBuilder.Entity<DmTtrangTtrinh>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("dm_ttrang_ttrinh");

            entity.Property(e => e.MaTtrangTt)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ttrang_tt");
            entity.Property(e => e.TenTtrangTt)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_ttrang_tt");
        });

        modelBuilder.Entity<DmUqHstpc>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("dm_uq_hstpc");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.GhSotienUq)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("gh_sotien_uq");
            entity.Property(e => e.LoaiUyquyen)
                .HasMaxLength(50)
                .HasColumnName("loai_uyquyen");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(11)
                .HasColumnName("ma_donvi");
            entity.Property(e => e.MaUserUq)
                .HasMaxLength(50)
                .HasColumnName("ma_user_uq");
            entity.Property(e => e.NgayCapnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_capnhat");
            entity.Property(e => e.NgayHl)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_hl");
        });

        modelBuilder.Entity<DmUser>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("dm_user");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.Dienthoai)
                .HasMaxLength(100)
                .HasColumnName("dienthoai");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((0))");
            entity.Property(e => e.IsActiveGddk).HasColumnName("IsActive_gddk");
            entity.Property(e => e.IsActiveGqkn).HasColumnName("IsActive_gqkn");
            entity.Property(e => e.IsActiveKytt).HasColumnName("IsActive_kytt");
            entity.Property(e => e.IsGdvHotro).HasColumnName("isGdv_hotro");
            entity.Property(e => e.IsactiveChkc).HasColumnName("isactive_chkc");
            entity.Property(e => e.LoaiCbo)
                .HasMaxLength(2)
                .HasDefaultValueSql("('')")
                .HasColumnName("loai_cbo");
            entity.Property(e => e.LoaiUser).HasColumnName("loai_user");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(50)
                .HasColumnName("ma_donvi");
            entity.Property(e => e.MaDonviPquyen)
                .HasMaxLength(150)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi_pquyen");
            entity.Property(e => e.MaUser)
                .HasMaxLength(100)
                .HasColumnName("ma_user");
            entity.Property(e => e.MaUserCapnhat)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user_capnhat");
            entity.Property(e => e.MaUserPias)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user_pias");
            entity.Property(e => e.Mail).HasMaxLength(100);
            entity.Property(e => e.NgayCnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cnhat");
            entity.Property(e => e.PhanQuyen).HasColumnName("phan_quyen");
            entity.Property(e => e.PquyenUplHinhAnh).HasColumnName("pquyen_upl_hinh_anh");
            entity.Property(e => e.TenUser)
                .HasMaxLength(100)
                .HasColumnName("ten_user");
        });

        modelBuilder.Entity<DmVersion>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("dm_version");

            entity.Property(e => e.PrKey)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.AppName)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')")
                .IsFixedLength()
                .HasColumnName("app_name");
            entity.Property(e => e.Logic)
                .HasMaxLength(50)
                .HasDefaultValueSql("(N'1')")
                .HasColumnName("logic");
            entity.Property(e => e.PathUrl)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("path_url");
            entity.Property(e => e.PathUrl1)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("path_url1");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.Version)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("version");
        });

        modelBuilder.Entity<ErrorLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ErrorLog");

            entity.Property(e => e.ErrorDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ErrorLogId)
                .ValueGeneratedOnAdd()
                .HasColumnName("ErrorLogID");
            entity.Property(e => e.ErrorMsg).IsUnicode(false);
            entity.Property(e => e.ErrorProc).HasMaxLength(128);
        });

        modelBuilder.Entity<GddkCt>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("gddk_ct");

            entity.HasIndex(e => e.FrKey, "gddk_ct_FR_KEY");

            entity.Property(e => e.PrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.FrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("fr_key");
            entity.Property(e => e.KinhDoChup)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("kinh_do_chup");
            entity.Property(e => e.NgayChup)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_chup");
            entity.Property(e => e.PathFile)
                .HasMaxLength(150)
                .HasDefaultValueSql("('')")
                .HasColumnName("path_file");
            entity.Property(e => e.PathOrginalFile)
                .HasMaxLength(150)
                .HasDefaultValueSql("('')")
                .HasColumnName("path_orginal_file");
            entity.Property(e => e.PathUrl)
                .HasMaxLength(150)
                .HasDefaultValueSql("('')")
                .HasColumnName("path_url");
            entity.Property(e => e.ViDoChup)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("vi_do_chup");
        });

        modelBuilder.Entity<GddkCtu>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("gddk_ctu");

            entity.Property(e => e.PrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.BienKsoat)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("bien_ksoat");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chu");
            entity.Property(e => e.MaCtu)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ctu");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi");
            entity.Property(e => e.MaUser)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.NgayCapnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_capnhat");
            entity.Property(e => e.NgayCtu)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_ctu");
            entity.Property(e => e.PrKeyNvu)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_nvu");
            entity.Property(e => e.SoDangky)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_dangky");
            entity.Property(e => e.SoDonbh)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_donbh");
            entity.Property(e => e.SoKhung)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_khung");
            entity.Property(e => e.SoSeri)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_seri");
        });

        modelBuilder.Entity<GhichuLichtruc>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("ghichu_lichtruc");

            entity.Property(e => e.PrKey).HasColumnName("pr_key").ValueGeneratedOnAdd();
            entity.Property(e => e.DienThoai)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("dien_thoai");
            entity.Property(e => e.FrKey).HasColumnName("fr_key");
            entity.Property(e => e.GhiChu)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chu");
            entity.Property(e => e.MaUser)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.SuDung)
                .IsRequired()
                .HasColumnName("su_dung");
            entity.Property(e => e.TenUser)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_user");
        });

        modelBuilder.Entity<HsgdCt>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("hsgd_ct", tb => tb.HasTrigger("Insert_stt"));

            entity.HasIndex(e => e.NgayChup, "NonClusteredIndex_ngay_chup");

            entity.HasIndex(e => e.FrKey, "hsgd_ctFRKEYIND");

            entity.HasIndex(e => e.FrKey, "ifr_key_hsgd_ct");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.Android).HasColumnName("android");
            entity.Property(e => e.DienGiai)
                .HasDefaultValueSql("('')")
                .HasColumnName("dien_giai");
            entity.Property(e => e.FrKey).HasColumnName("fr_key");
            entity.Property(e => e.KinhDoChup)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("kinh_do_chup");
            entity.Property(e => e.MaHmuc)
                .HasMaxLength(4)
                .HasDefaultValueSql("('CHUN')")
                .HasColumnName("ma_hmuc");
            entity.Property(e => e.NgayChup)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_chup");
            entity.Property(e => e.NhomAnh)
                .HasDefaultValueSql("('')")
                .HasColumnName("nhom_anh");
            entity.Property(e => e.PathFile)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("path_file");
            entity.Property(e => e.PathOrginalFile)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("path_orginal_file");
            entity.Property(e => e.PathUrl)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("path_url");
            entity.Property(e => e.Stt).HasColumnName("stt");
            entity.Property(e => e.ViDoChup)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("vi_do_chup");
            entity.Property(e => e.MaHmucSc)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_hmuc_sc");
        });

        modelBuilder.Entity<HsgdCtu>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("hsgd_ctu", tb =>
            {
                tb.HasTrigger("Update_so_hsgd");
                tb.HasTrigger("update_nhatky_danggd");
            });

            entity.HasIndex(e => e.SoDonbh, "Index_hsgd_ctu_so_donbh");

            entity.HasIndex(e => e.SoSeri, "Index_hsgd_ctu_so_seri");

            entity.HasIndex(e => e.SoHsgd, "hsgd_ct_so_hsgdIND");

            entity.HasIndex(e => e.MaDonvi, "hsgd_ctu_ma_donvi_IND");

            entity.HasIndex(e => e.MaTtrangGd, "hsgd_ctu_ma_ttrang_gd_IND");

            entity.HasIndex(e => e.MaUser, "hsgd_ctu_ma_user_IND");

            entity.HasIndex(e => e.PrKeyBt, "hsgd_ctu_pr_key_bt_IND");

            entity.HasIndex(e => e.PrKeyBtHo, "pr_key_bt_ho_IDX");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.BaoLanh).HasColumnName("bao_lanh");
            entity.Property(e => e.BienKsoat)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("bien_ksoat");
            entity.Property(e => e.Bl1)
                .HasDefaultValueSql("((1))")
                .HasColumnName("bl_1");
            entity.Property(e => e.Bl2)
                .HasDefaultValueSql("((1))")
                .HasColumnName("bl_2");
            entity.Property(e => e.Bl3)
                .HasDefaultValueSql("((1))")
                .HasColumnName("bl_3");
            entity.Property(e => e.Bl4)
                .HasDefaultValueSql("((1))")
                .HasColumnName("bl_4");
            entity.Property(e => e.Bl5)
                .HasDefaultValueSql("((1))")
                .HasColumnName("bl_5");
            entity.Property(e => e.Bl6).HasColumnName("bl_6");
            entity.Property(e => e.Bl7).HasColumnName("bl_7");
            entity.Property(e => e.Bl8).HasColumnName("bl_8");
            entity.Property(e => e.Bl9)
                .HasDefaultValueSql("((1))")
                .HasColumnName("bl_9");
            entity.Property(e => e.BlDsemail)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("bl_dsemail");
            entity.Property(e => e.BlDsphone)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("bl_dsphone");
            entity.Property(e => e.BlPdbl).HasColumnName("bl_pdbl");
            entity.Property(e => e.BlSendEmail).HasColumnName("bl_send_email");
            entity.Property(e => e.BlTailieubs)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("bl_tailieubs");
            entity.Property(e => e.ChkDaydu).HasColumnName("chk_daydu");
            entity.Property(e => e.ChkDunghan).HasColumnName("chk_dunghan");
            entity.Property(e => e.ChkTheohopdong).HasColumnName("chk_theohopdong");
            entity.Property(e => e.ChuaThuphi).HasColumnName("chua_thuphi");
            entity.Property(e => e.DangKiem).HasColumnName("dang_kiem");
            entity.Property(e => e.DexuatPan)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("dexuat_pan");
            entity.Property(e => e.DiaChi)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("dia_chi");
            entity.Property(e => e.DiaDiemgd)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("dia_diemgd");
            entity.Property(e => e.DiaDiemtt)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("dia_diemtt");
            entity.Property(e => e.DienThoai)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("dien_thoai");
            entity.Property(e => e.DienThoaiNdbh)
                .HasMaxLength(50)
                .HasColumnName("dien_thoai_ndbh");
            entity.Property(e => e.DoituongttTnds)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("doituongtt_tnds");
            entity.Property(e => e.DoituongttTsk)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("doituongtt_tsk");
            entity.Property(e => e.DonviSuachuaTsk)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("donvi_suachua_tsk");
            entity.Property(e => e.DvnhapPasc).HasColumnName("dvnhap_pasc");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(2000)
                .HasColumnName("ghi_chu");
            entity.Property(e => e.GhiChudx)
                .HasMaxLength(2000)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chudx");
            entity.Property(e => e.GhiChudxTnds)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chudx_tnds");
            entity.Property(e => e.GhiChudxTndstt)
                .HasMaxLength(2000)
                .HasColumnName("ghi_chudx_tndstt");
            entity.Property(e => e.GhiChudxTsk)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chudx_tsk");
            entity.Property(e => e.GhiChudxTsktt)
                .HasMaxLength(2000)
                .HasColumnName("ghi_chudx_tsktt");
            entity.Property(e => e.GhiChudxtt)
                .HasMaxLength(2000)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chudxtt");
            entity.Property(e => e.HauQua)
                .HasDefaultValueSql("('')")
                .HasColumnName("hau_qua");
            entity.Property(e => e.HieuXe).HasColumnName("hieu_xe");
            entity.Property(e => e.HieuXeTnds).HasColumnName("hieu_xe_tnds");
            entity.Property(e => e.HieuXeTndsBen3)
                .HasDefaultValueSql("((0))")
                .HasColumnName("hieu_xe_tnds_ben3");
            entity.Property(e => e.HosoPhaply)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("hoso_phaply");
            entity.Property(e => e.HsgdTpc).HasColumnName("hsgd_tpc");
            entity.Property(e => e.InsertMobile)
                .HasMaxLength(2)
                .HasDefaultValueSql("((0))")
                .HasColumnName("insert_mobile");
            entity.Property(e => e.LoaiTotrinhTpc).HasColumnName("loai_totrinh_tpc");
            entity.Property(e => e.LoaiXe).HasColumnName("loai_xe");
            entity.Property(e => e.LoaiXeTnds).HasColumnName("loai_xe_tnds");
            entity.Property(e => e.LoaiXeTndsBen3)
                .HasDefaultValueSql("((0))")
                .HasColumnName("loai_xe_tnds_ben3");
            entity.Property(e => e.LydoCtkh)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("lydo_ctkh");
            entity.Property(e => e.LydoCtkhTnds)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("lydo_ctkh_tnds");
            entity.Property(e => e.LydoCtkhTsk)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("lydo_ctkh_tsk");
            entity.Property(e => e.MaCbkt)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_cbkt");
            entity.Property(e => e.MaCtu)
                .HasMaxLength(11)
                .HasColumnName("ma_ctu");
            entity.Property(e => e.MaDaily)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_daily");
            entity.Property(e => e.MaDdiemTthat)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ddiem_tthat");
            entity.Property(e => e.MaDonbh)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donbh");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi");
            entity.Property(e => e.MaDonviTt)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi_tt");
            entity.Property(e => e.MaDonvigd)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvigd");
            entity.Property(e => e.MaDvbtHo)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_dvbt_ho");
            entity.Property(e => e.MaDviBtHo)
                .HasMaxLength(11)
                .HasColumnName("ma_dvi_bt_ho");
            entity.Property(e => e.MaGaraTnds)
                .HasMaxLength(50)
                .HasColumnName("ma_gara_tnds");
            entity.Property(e => e.MaGaraTnds01)
                .HasMaxLength(300)
                .HasColumnName("ma_gara_tnds01");
            entity.Property(e => e.MaGaraTnds02)
                .HasMaxLength(300)
                .HasColumnName("ma_gara_tnds02");
            entity.Property(e => e.MaGaraVcx)
                .HasMaxLength(50)
                .HasColumnName("ma_gara_vcx");
            entity.Property(e => e.MaGaraVcx01)
                .HasMaxLength(300)
                .HasColumnName("ma_gara_vcx01");
            entity.Property(e => e.MaGaraVcx02)
                .HasMaxLength(300)
                .HasColumnName("ma_gara_vcx02");
            entity.Property(e => e.MaKh)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_kh");
            entity.Property(e => e.MaKthac)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_kthac");
            entity.Property(e => e.MaLhsbt)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_lhsbt");
            entity.Property(e => e.MaLoaibang)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_loaibang");
            entity.Property(e => e.MaNguyenNhanTtat)
                .HasMaxLength(10)
                .HasColumnName("ma_nguyen_nhan_ttat");
            entity.Property(e => e.MaNhloaixe)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_nhloaixe");
            entity.Property(e => e.MaPkt)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_pkt");
            entity.Property(e => e.MaSanpham)
                .HasMaxLength(150)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_sanpham");
            entity.Property(e => e.MaTte)
                .HasMaxLength(3)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_tte");
            entity.Property(e => e.MaTtrangGd)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ttrang_gd");
            entity.Property(e => e.MaUser).HasColumnName("ma_user");
            entity.Property(e => e.NamSinh).HasColumnName("nam_sinh");
            entity.Property(e => e.NamSx).HasColumnName("nam_sx");
            entity.Property(e => e.NamSxTnds).HasColumnName("nam_sx_tnds");
            entity.Property(e => e.NgGdichTh)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ng_gdich_th");
            entity.Property(e => e.NgLienhe)
                .HasMaxLength(200)
                .HasColumnName("ng_lienhe");
            entity.Property(e => e.NgayCtu)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_ctu");
            entity.Property(e => e.NgayCuoiLaixe)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cuoi_laixe");
            entity.Property(e => e.NgayCuoiLuuhanh)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cuoi_luuhanh");
            entity.Property(e => e.NgayCuoiSeri)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cuoi_seri");
            entity.Property(e => e.NgayDauLaixe)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_dau_laixe");
            entity.Property(e => e.NgayDauLuuhanh)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_dau_luuhanh");
            entity.Property(e => e.NgayDauSeri)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_dau_seri");
            entity.Property(e => e.NgayDuyet)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_duyet");
            entity.Property(e => e.NgayGdinh)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_gdinh");
            entity.Property(e => e.NgayTbao)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_tbao");
            entity.Property(e => e.NgayThuphi)
                .HasMaxLength(1800)
                .HasDefaultValueSql("('')")
                .HasColumnName("ngay_thuphi");
            entity.Property(e => e.NgayTthat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_tthat");
            entity.Property(e => e.NguoiGiao)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("nguoi_giao");
            entity.Property(e => e.NguoiXuly)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("nguoi_xuly");
            entity.Property(e => e.NguyenNhanTtat)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("nguyen_nhan_ttat");
            entity.Property(e => e.PascDsemail)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("pasc_dsemail");
            entity.Property(e => e.PascDsphone)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("pasc_dsphone");
            entity.Property(e => e.PascSendEmail).HasColumnName("pasc_send_email");
            entity.Property(e => e.PascVatTnds).HasColumnName("pasc_vat_tnds");
            entity.Property(e => e.PascVatVcx).HasColumnName("pasc_vat_vcx");
            entity.Property(e => e.PathCrm)
                .HasMaxLength(200)
                .HasColumnName("path_crm");
            entity.Property(e => e.PathTndsDt)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("path_tnds_dt");
            entity.Property(e => e.PathTndsKhacDt)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("path_tnds_khac_dt");
            entity.Property(e => e.PathTotrinhTpc)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("path_totrinh_tpc");
            entity.Property(e => e.PathVcxDt)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("path_vcx_dt");
            entity.Property(e => e.PrKeyBt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_bt");
            entity.Property(e => e.PrKeyBtHo)
                .HasMaxLength(11)
                .HasDefaultValueSql("((0))")
                .HasColumnName("pr_key_bt_ho");
            entity.Property(e => e.PrKeyGoc)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_goc");
            entity.Property(e => e.PrKeyKbtt).HasColumnName("pr_key_kbtt");
            entity.Property(e => e.PrKeySeri)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_seri");
            entity.Property(e => e.SaiDkdk).HasColumnName("sai_dkdk");
            entity.Property(e => e.SaiPhancap).HasColumnName("sai_phancap");
            entity.Property(e => e.SaiphamKhac).HasColumnName("saipham_khac");
            entity.Property(e => e.SendThongbaoBt).HasColumnName("send_thongbao_bt");
            entity.Property(e => e.SoDonbh)
                .HasMaxLength(23)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_donbh");
            entity.Property(e => e.SoGphepLaixe)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_gphep_laixe");
            entity.Property(e => e.SoGphepLuuhanh)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_gphep_luuhanh");
            entity.Property(e => e.SoHsgd)
                .HasMaxLength(12)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_hsgd");
            entity.Property(e => e.SoLanBt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_lan_bt");
            entity.Property(e => e.SoSeri)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_seri");
            entity.Property(e => e.SoTienGtbt).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.SoTienGtbtKhac).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.SoTienGtbtTnds)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("SoTienGtbtTNDS");
            entity.Property(e => e.SoTienBaoHiem)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tien_bao_hiem");
            entity.Property(e => e.SoTienThucTe)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tien_thuc_te");
            entity.Property(e => e.SoTienctkh)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienctkh");
            entity.Property(e => e.SoTienctkhTnds)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienctkh_tnds");
            entity.Property(e => e.SoTienctkhTsk)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienctkh_tsk");
            entity.Property(e => e.SoTienugd)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienugd");
            entity.Property(e => e.TenKhach)
                .HasMaxLength(150)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_khach");
            entity.Property(e => e.TenLaixe)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_laixe");
            entity.Property(e => e.ThieuAnh).HasColumnName("thieu_anh");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("trang_thai");
            entity.Property(e => e.TrucloiBh).HasColumnName("trucloi_bh");
            entity.Property(e => e.TygiaHt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("tygia_ht");
            entity.Property(e => e.TygiaTt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("tygia_tt");
            entity.Property(e => e.TyleggPhutungtnds)
                .HasDefaultValueSql("((0.00))")
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tylegg_phutungtnds");
            entity.Property(e => e.TyleggPhutungvcx)
                .HasDefaultValueSql("((0.00))")
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tylegg_phutungvcx");
            entity.Property(e => e.TyleggSuachuatnds)
                .HasDefaultValueSql("((0.00))")
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tylegg_suachuatnds");
            entity.Property(e => e.TyleggSuachuavcx)
                .HasDefaultValueSql("((0.00))")
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tylegg_suachuavcx");
            entity.Property(e => e.Vat)
                .HasDefaultValueSql("((1))")
                .HasColumnName("vat");
            entity.Property(e => e.VatTnds)
                .HasDefaultValueSql("((1))")
                .HasColumnName("vat_tnds");
            entity.Property(e => e.VatTsk)
                .HasDefaultValueSql("((1))")
                .HasColumnName("vat_tsk");
            entity.Property(e => e.XuatXu)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("xuat_xu");
            entity.Property(e => e.XuatXuTnds)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("xuat_xu_tnds");
            entity.Property(e => e.YkienGdinh)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("ykien_gdinh");
            entity.Property(e => e.VaiTro)
                .HasMaxLength(20)
                .IsUnicode(true)
                .HasDefaultValue(string.Empty)
                .HasColumnName("vai_tro");
            entity.Property(e => e.TyleTg)
                .HasColumnType("decimal(18,4)")
                .HasDefaultValueSql("((0.00))")
                .HasColumnName("tyle_tg");
            entity.Property(e => e.SoHsbt)
                .HasMaxLength(30)
                .IsUnicode(true)
                .HasDefaultValue(string.Empty)
                .HasColumnName("so_hsbt");
            entity.Property(e => e.HoanThienHstt)
            .HasColumnName("hoanthien_hstt")           
            .IsRequired()
            .HasDefaultValue(false);
        });

        modelBuilder.Entity<HsgdDg>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("hsgd_dg");

            entity.HasIndex(e => e.FrKey, "hsgd_dgFRKEYIND");

            entity.HasIndex(e => e.FrKey, "ifr_key_hsgd_dg");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.DeXuat)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("de_xuat");
            entity.Property(e => e.FrKey).HasColumnName("fr_key");
            entity.Property(e => e.LoaiDg).HasColumnName("loai_dg");
            entity.Property(e => e.MaUser).HasColumnName("ma_user");
            entity.Property(e => e.MaUserDuyet).HasColumnName("ma_user_duyet");
            entity.Property(e => e.NgayBaoGia)
               .HasColumnType("smalldatetime")
               .HasColumnName("ngay_bao_gia");
            entity.Property(e => e.NgayDuyetGia)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_duyet_gia");
            entity.Property(e => e.SoTien)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tien");
            entity.Property(e => e.NgayCapNhat)
               .HasColumnType("smalldatetime")
               .HasColumnName("ngay_cnhat");
            entity.Property(e => e.Hienthi).HasColumnName("hien_thi");

        });

        modelBuilder.Entity<HsgdDgCt>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("hsgd_dg_ct");

            entity.HasIndex(e => e.FrKey, "hsgd_dg_ctFRKEYIND");

            entity.HasIndex(e => e.FrKey, "ifr_key_hsgd_dg_ct");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.FrKey).HasColumnName("fr_key");
            entity.Property(e => e.PathFile)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("path_file");
            entity.Property(e => e.PathOrginalFile)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("path_orginal_file");
            entity.Property(e => e.PathUrl)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("path_url");

            entity.HasOne(d => d.FrKeyNavigation).WithMany(p => p.HsgdDgCts)
                .HasForeignKey(d => d.FrKey)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_hsgd_dg_ct_fr_key");
        });

        modelBuilder.Entity<HsgdDx>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("hsgd_dx");

            entity.HasIndex(e => e.FrKey, "hsgd_dxFRKEYIND");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.FrKey).HasColumnName("fr_key");
            entity.Property(e => e.GetDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("smalldatetime")
                .HasColumnName("get_date");
            entity.Property(e => e.GhiChudv)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chudv");
            entity.Property(e => e.GhiChutt)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chutt");
            entity.Property(e => e.GiamTruBt).HasColumnName("giam_tru_bt");
            entity.Property(e => e.Hmuc)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("hmuc");
            entity.Property(e => e.LoaiDx).HasColumnName("loai_dx");
            entity.Property(e => e.MaHmuc)
                .HasMaxLength(30)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_hmuc");
            entity.Property(e => e.NgayCapnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_capnhat");
            entity.Property(e => e.PrKeyDx)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_dx");
            entity.Property(e => e.SoTienDoitru)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tien_doitru");
            entity.Property(e => e.SoTienpdDoitru)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienpd_doitru");
            entity.Property(e => e.SoTienpdsc)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienpdsc");
            entity.Property(e => e.SoTienpdtt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienpdtt");
            entity.Property(e => e.SoTienph)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienph");
            entity.Property(e => e.SoTienson)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienson");
            entity.Property(e => e.SoTientt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tientt");
            entity.Property(e => e.ThuHoiTs).HasColumnName("thu_hoi_ts");
            entity.Property(e => e.VatSc)
                .HasDefaultValueSql("((10))")
                .HasColumnName("vat_sc");
        });

        modelBuilder.Entity<HsgdDxTsk>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("hsgd_dx_tsk", tb => tb.HasTrigger("Rep_Tu_hsgd_dx_tsk"));

            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.FrKey).HasColumnName("fr_key");
            entity.Property(e => e.GetDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("smalldatetime")
                .HasColumnName("get_date");
            entity.Property(e => e.GhiChudv)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chudv");
            entity.Property(e => e.GhiChutt)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chutt");
            entity.Property(e => e.GiamTruBt).HasColumnName("giam_tru_bt");
            entity.Property(e => e.Hmuc)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("hmuc");
            entity.Property(e => e.NgayCapnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_capnhat");
            entity.Property(e => e.PrKeyDx)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_dx");
            entity.Property(e => e.SoTienpdsc)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienpdsc");
            entity.Property(e => e.SoTienpdtt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienpdtt");
            entity.Property(e => e.SoTiensc)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tiensc");
            entity.Property(e => e.SoTientt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tientt");
            entity.Property(e => e.ThuHoiTs).HasColumnName("thu_hoi_ts");
            entity.Property(e => e.VatSc)
                .HasDefaultValueSql("((10))")
                .HasColumnName("vat_sc");
        });

        modelBuilder.Entity<HsgdLsu>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("hsgd_lsu");

            entity.Property(e => e.PrKey)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.FrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("fr_key");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(2000)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chu");
            entity.Property(e => e.MaUserChuyen)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user_chuyen");
            entity.Property(e => e.MaUserNhan)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user_nhan");
            entity.Property(e => e.NgayCnhat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cnhat");
        });

        modelBuilder.Entity<HsgdTtrinh>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("hsgd_ttrinh");

            entity.HasIndex(e => e.MaDonvi, "hsgd_ttrinh_ma_donvi_IND");

            entity.HasIndex(e => e.MaTtrang, "hsgd_ttrinh_ma_ttrang_IND");

            entity.HasIndex(e => e.PrKeyCt, "hsgd_ttrinh_pr_key_ct_IND");

            entity.HasIndex(e => e.PrKeyHsgd, "hsgd_ttrinh_pr_key_hsgd_IND");

            entity.Property(e => e.PrKey)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.ChiKhac)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("chi_khac");
            entity.Property(e => e.ChkChuanopphi).HasColumnName("chk_chuanopphi");
            entity.Property(e => e.ChkDaydu).HasColumnName("chk_daydu");
            entity.Property(e => e.ChkDunghan).HasColumnName("chk_dunghan");
            entity.Property(e => e.ChkTheohopdong).HasColumnName("chk_theohopdong")
                 .HasDefaultValueSql("((0))");
            entity.Property(e => e.GiatriThuhoi)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("giatri_thuhoi");
            entity.Property(e => e.GtrinhChikhac)
                .HasDefaultValueSql("('')")
                .HasColumnName("gtrinh_chikhac");
            entity.Property(e => e.HauQua)
                .HasDefaultValueSql("('')")
                .HasColumnName("hau_qua");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi");
            entity.Property(e => e.MaTtrang)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ttrang");
            entity.Property(e => e.NgGdich)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("ng_gdich");
            entity.Property(e => e.NgayCtu)
                .HasColumnType("date")
                .HasColumnName("ngay_ctu");
            entity.Property(e => e.NgayThuphi)
                .HasMaxLength(1800)
                .HasDefaultValueSql("('')")
                .HasColumnName("ngay_thuphi");
            entity.Property(e => e.NgayTthat)
                .HasColumnType("date")
                .HasColumnName("ngay_tthat");
            entity.Property(e => e.NguyenNhan)
                .HasDefaultValueSql("('')")
                .HasColumnName("nguyen_nhan");
            entity.Property(e => e.Oid)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("oid");
            entity.Property(e => e.PanThoiTs)
                .HasDefaultValueSql("('')")
                .HasColumnName("pan_thoi_ts");
            entity.Property(e => e.PathTtrinh)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("path_ttrinh");
            entity.Property(e => e.PrKeyCt)
                .HasMaxLength(150)
                .HasDefaultValueSql("('')")
                .HasColumnName("pr_key_ct");
            entity.Property(e => e.PrKeyHsgd)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_hsgd");
            entity.Property(e => e.SoBthuong)
                .HasColumnType("decimal(9, 0)")
                .HasColumnName("so_bthuong");
            entity.Property(e => e.SoHsbt)
                .HasMaxLength(30)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_hsbt");
            entity.Property(e => e.SoNgchet)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_ngchet");
            entity.Property(e => e.SoPhibh)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_phibh");
            entity.Property(e => e.SoTien)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("so_tien");
            entity.Property(e => e.TaisanThuhoi)
                .HasDefaultValueSql("('')")
                .HasColumnName("taisan_thuhoi");
            entity.Property(e => e.TenDttt)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_dttt");
            entity.Property(e => e.ThamGia007).HasColumnName("tham_gia007");
            entity.Property(e => e.NgayDuTlieu)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_dutlieu");
            entity.Property(e => e.NgayTtoan)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_ttoan");
        });

        modelBuilder.Entity<HsgdTtrinhCt>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("hsgd_ttrinh_ct");

            entity.Property(e => e.PrKey)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.FrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("fr_key");
            entity.Property(e => e.MaSp)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_sp");
            entity.Property(e => e.MaDKhoan)
               .HasMaxLength(8)
               .HasDefaultValueSql("('')")
               .HasColumnName("ma_dkhoan");
            entity.Property(e => e.MucVat).HasColumnName("muc_vat");
            entity.Property(e => e.SoTienBtVat)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tien_bt_vat");
            entity.Property(e => e.SotienBh)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("sotien_bh");
            entity.Property(e => e.SotienBt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("sotien_bt");
            entity.Property(e => e.SotienTu)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("sotien_tu");
            entity.Property(e => e.TinhToanbt)
                .HasDefaultValueSql("('')")
                .HasColumnName("tinh_toanbt");
        });

        modelBuilder.Entity<HsgdTtrinhNky>(entity =>
        {
            entity.HasKey(e => e.PrKey);
            entity.ToTable("hsgd_ttrinh_nky");

            entity.Property(e => e.Act)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("act");
            entity.Property(e => e.FrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("fr_key");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chu");
            entity.Property(e => e.NgayCnhat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("ngay_cnhat");
            entity.Property(e => e.PrKey)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.UserChuyen)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("user_chuyen");
            entity.Property(e => e.UserNhan)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("user_nhan");
        });

        modelBuilder.Entity<LichTrucgdv>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("lich_trucgdv");

            entity.Property(e => e.PrKey).HasColumnName("pr_key").ValueGeneratedOnAdd();
            entity.Property(e => e.FrKey).HasColumnName("fr_key");
            entity.Property(e => e.MaGara)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_gara");
            entity.Property(e => e.MaKv)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_kv");
            entity.Property(e => e.MaUser)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.NgayBo)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_bo");
            entity.Property(e => e.NgayCapnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_capnhat");
            entity.Property(e => e.NgayTao)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_tao");
            entity.Property(e => e.SangChieu)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')")
                .HasColumnName("sang_chieu");
            entity.Property(e => e.SuDung)
                .HasColumnName("su_dung");
            entity.Property(e => e.TenGara)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_gara");
            entity.Property(e => e.TenUser)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_user");
            entity.Property(e => e.Thoigian)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("thoigian");
            entity.Property(e => e.Thu)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("thu");
        });

        modelBuilder.Entity<LichsuPa>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("lichsu_pa");

            entity.Property(e => e.GhiChudv)
                .HasMaxLength(300)
                .HasColumnName("ghi_chudv");
            entity.Property(e => e.GiaPheduyet)
                .HasMaxLength(4000)
                .HasColumnName("gia_pheduyet");
            entity.Property(e => e.GiaPhutung)
                .HasMaxLength(4000)
                .HasColumnName("gia_phutung");
            entity.Property(e => e.HsgdTpc)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("hsgd_tpc");
            entity.Property(e => e.LoaiXe).HasColumnName("loai_xe");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(50)
                .HasColumnName("ma_donvi");
            entity.Property(e => e.NamSx).HasColumnName("nam_sx");
            entity.Property(e => e.NgayPd)
                .HasMaxLength(30)
                .HasColumnName("ngay_pd");
            entity.Property(e => e.NgayTt)
                .HasMaxLength(30)
                .HasColumnName("ngay_tt");
            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.QuanHuyen)
                .HasMaxLength(100)
                .HasColumnName("quan_huyen");
            entity.Property(e => e.SoHsgd)
                .HasMaxLength(12)
                .HasColumnName("so_hsgd");
            entity.Property(e => e.TenDonvi)
                .HasMaxLength(50)
                .HasColumnName("ten_donvi");
            entity.Property(e => e.TenGara)
                .HasMaxLength(150)
                .HasColumnName("ten_gara");
            entity.Property(e => e.TenHmuc)
                .HasMaxLength(50)
                .HasColumnName("ten_hmuc");
            entity.Property(e => e.TenTinh)
                .HasMaxLength(100)
                .HasColumnName("ten_tinh");
            entity.Property(e => e.TenloaiXe)
                .HasMaxLength(50)
                .HasColumnName("tenloai_xe");
            entity.Property(e => e.XuatXu)
                .HasMaxLength(50)
                .HasColumnName("xuat_xu");
        });

        modelBuilder.Entity<LichtrucCtu>(entity =>
        {
            entity.HasKey(e => e.PrKey);
            entity.ToTable("lichtruc_ctu");

            entity.Property(e => e.DenNgay)
                .HasColumnType("smalldatetime")
                .HasColumnName("den_ngay");
            entity.Property(e => e.GhiChu)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chu");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi");
            entity.Property(e => e.MaKv)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_kv");
            entity.Property(e => e.NgayTao)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_tao");
            entity.Property(e => e.PrKey)
                .ValueGeneratedOnAdd()
                .HasColumnName("pr_key");
            entity.Property(e => e.TuNgay)
                .HasColumnType("smalldatetime")
                .HasColumnName("tu_ngay");
        });
        modelBuilder.Entity<HsgdTtrinhTt>(entity =>
        {
            entity.HasKey(e => e.PrKey);
            entity.ToTable("hsgd_ttrinh_tt");
            entity.Property(e => e.PrKey)
                .HasColumnName("pr_key")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.FrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("fr_key");

            entity.Property(e => e.LydoTt)
                .HasMaxLength(2000)
                .HasColumnName("lydo_tt");

            entity.Property(e => e.SoTaikhoanNh)
                .HasMaxLength(50)
                .HasColumnName("so_taikhoan_nh");

            entity.Property(e => e.SotienTt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("sotien_tt");

            entity.Property(e => e.TenChuTk)
                .HasMaxLength(100)
                .HasColumnName("ten_chu_tk");

            entity.Property(e => e.TenNh)
                .HasMaxLength(100)
                .HasColumnName("ten_nh");
            entity.Property(e => e.bnkCode)
               .HasMaxLength(30)
               .HasColumnName("bnkCode")
               .HasDefaultValue("");
        });
        modelBuilder.Entity<HsgdTbbt>(entity =>
        {
            entity.HasKey(e => e.PrKey);
            entity.ToTable("hsgd_tbbt");                    

            entity.Property(e => e.PrKey)
                .ValueGeneratedOnAdd()
                .HasColumnName("pr_key")
                .HasColumnType("decimal(18, 0)");

            entity.Property(e => e.PrKeyHsgd)
                .HasColumnName("pr_key_hsgd")
                .HasColumnType("decimal(18, 0)");

            entity.Property(e => e.PdTbbt)
                .HasColumnName("pd_tbbt")
                .HasColumnType("int");

            entity.Property(e => e.DsEmail)
                .HasColumnName("ds_email")
                .HasMaxLength(200)
                .IsUnicode(true);

            entity.Property(e => e.TndsXeCoGioi)
                .HasColumnName("tnds_xecogioi")
                .HasColumnType("decimal(18, 0)");

            entity.Property(e => e.TndsHangHoa)
                .HasColumnName("tnds_hanghoa")
                .HasColumnType("decimal(18, 0)");

            entity.Property(e => e.TndsTaiNanHk)
                .HasColumnName("tnds_tainanhk")
                .HasColumnType("decimal(18, 0)");

            entity.Property(e => e.TndsTaiSanKhac)
                .HasColumnName("tnds_taisankhac")
                .HasColumnType("decimal(18, 0)");

            entity.Property(e => e.TndsNguoi)
                .HasColumnName("tnds_nguoi")
                .HasColumnType("decimal(18, 0)");

            entity.Property(e => e.SoNgayTtoan)
                .HasColumnName("so_ngayttoan")
                .HasColumnType("int");

            entity.Property(e => e.PathTbbt)
                .HasColumnName("path_tbbt")
                .HasMaxLength(200)
                .IsUnicode(true);

            entity.Property(e => e.GhiChu)
                .HasColumnName("ghi_chu")
                .HasMaxLength(500)
                .IsUnicode(true);
            entity.Property(e => e.SendTbbt)
               .HasColumnName("send_tbbt")
               .HasColumnType("int");
            entity.Property(e => e.MaDonviTT)
               .HasColumnName("ma_donvitt")
               .HasMaxLength(8)
               .IsUnicode(true);
        });
        modelBuilder.Entity<HsgdTbbtTt>(entity =>
        {
            entity.HasKey(e => e.PrKey);
            entity.ToTable("hsgd_tbbt_tt");
            entity.Property(e => e.PrKey)
                .HasColumnName("pr_key")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.FrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("fr_key");

            entity.Property(e => e.LydoTt)
                .HasMaxLength(2000)
                .HasColumnName("lydo_tt");

            entity.Property(e => e.SoTaikhoanNh)
                .HasMaxLength(50)
                .HasColumnName("so_taikhoan_nh");

            entity.Property(e => e.SotienTt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("sotien_tt");

            entity.Property(e => e.TenChuTk)
                .HasMaxLength(100)
                .HasColumnName("ten_chu_tk");

            entity.Property(e => e.TenNh)
                .HasMaxLength(100)
                .HasColumnName("ten_nh");
        });
        modelBuilder.Entity<LichtrucOld>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("lichtruc_old");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.Cptt)
                .HasMaxLength(3)
                .HasDefaultValueSql("('')")
                .HasColumnName("cptt");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chu");
            entity.Property(e => e.GhiChuphi)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chuphi");
            entity.Property(e => e.NganHang)
                .HasMaxLength(300)
                .HasColumnName("ngan_hang");
            entity.Property(e => e.NgayLay)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_lay");
            entity.Property(e => e.SoHdgcn)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_hdgcn");
            entity.Property(e => e.SoHsbt)
                .HasMaxLength(50)
                .HasColumnName("so_hsbt");
            entity.Property(e => e.SoTaikhoan)
                .HasMaxLength(50)
                .HasColumnName("so_taikhoan");
            entity.Property(e => e.SoThe)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_the");
            entity.Property(e => e.SoTheOld)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_the_old");
            entity.Property(e => e.SoTienp)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienp");
            entity.Property(e => e.SoTkcheck)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_tkcheck");
            entity.Property(e => e.TaikhoanDung).HasColumnName("taikhoan_dung");
            entity.Property(e => e.TenCanbo)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_canbo");
            entity.Property(e => e.TenNdbh)
                .HasMaxLength(200)
                .HasColumnName("ten_ndbh");
            entity.Property(e => e.TenNguoithuhuong)
                .HasMaxLength(200)
                .HasColumnName("ten_nguoithuhuong");
            entity.Property(e => e.TenNthCheck)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_nth_check");
            entity.Property(e => e.TentatNganhang)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("tentat_nganhang");
            entity.Property(e => e.TrangthaiCheck).HasColumnName("trangthai_check");
            entity.Property(e => e.TtrangPhi)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("ttrang_phi");
        });

        modelBuilder.Entity<LsuDangnhap>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("lsu_dangnhap");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.FrKey).HasColumnName("fr_key");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(3)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi");
            entity.Property(e => e.Mobile).HasColumnName("mobile");
            entity.Property(e => e.ThaoTac)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("thao_tac");
            entity.Property(e => e.ThoiGian)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("smalldatetime")
                .HasColumnName("thoi_gian");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("username");
        });

        modelBuilder.Entity<NhatKy>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("nhat_ky");

            entity.HasIndex(e => new { e.FrKey, e.MaTtrangGd, e.NgayCapnhat }, "NonClusteredIndex-20250220-095048");

            entity.HasIndex(e => e.FrKey, "ifr_key_nhat_ky");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.FrKey).HasColumnName("fr_key");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(2000)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chu");
            entity.Property(e => e.MaTtrangGd)
                .HasMaxLength(5)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ttrang_gd");
            entity.Property(e => e.MaUser).HasColumnName("ma_user");
            entity.Property(e => e.NgayCapnhat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_capnhat");
            entity.Property(e => e.TenTtrangGd)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_ttrang_gd");
        });

        modelBuilder.Entity<NhatKyGddk>(entity =>
        {
            entity.HasKey(e => e.PrKey).HasName("PK__nhat_ky___D53C590CDC224C59");

            entity.ToTable("nhat_ky_gddk");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.FrKey).HasColumnName("fr_key");
            entity.Property(e => e.MaUser)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.ThaoTac)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("thao_tac");
            entity.Property(e => e.ThoiGian)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("smalldatetime")
                .HasColumnName("thoi_gian");
        });

        modelBuilder.Entity<PquyenCnang>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("pquyen_cnang");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chu");
            entity.Property(e => e.LoaiQuyen)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("loai_quyen");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi");
            entity.Property(e => e.MaDonviPquyen)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi_pquyen");
            entity.Property(e => e.MaUser)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.MaUserCap)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user_cap");
            entity.Property(e => e.NgayCap)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cap");
            entity.Property(e => e.TenUser)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_user");
            entity.Property(e => e.TrangThai)
                .HasDefaultValueSql("((1))")
                .HasColumnName("trang_thai");
        });
        modelBuilder.Entity<DmLoaiDongco>(entity =>
        {
            entity.HasKey(e => e.PrKey).HasName("PK__dm_loai___D53C590CDE144323");

            entity.ToTable("dm_loai_dongco");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.MaLoaiDongco)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_loai_dongco");
            entity.Property(e => e.TenLoaiDongco)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_loai_dongco");
        });
        modelBuilder.Entity<HsgdAttachFile>(entity =>
        {
            entity.HasKey(e => e.PrKey).HasName("PK_hsgd_attachfile");

            entity.ToTable("hsgd_attachfile");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.FrKey).HasColumnName("fr_key");
            entity.Property(e => e.MaCtu)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ctu");
            entity.Property(e => e.FileName)
                 .HasMaxLength(250)
                 .HasDefaultValueSql("('')")
                 .HasColumnName("file_name");
            entity.Property(e => e.Directory)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("directory");
            entity.Property(e => e.ngay_cnhat)
                .HasColumnType("smalldatetime")
               .HasColumnName("ngay_cnhat");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chu");
            entity.Property(e => e.NguonTao)
               .HasMaxLength(50)
               .HasDefaultValueSql("('')")
               .HasColumnName("nguon_tao");

        });
        modelBuilder.Entity<HsgdDxCt>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("hsgd_dx_ct");

            entity.HasIndex(e => e.MaSp, "hsgd_dx_ct_ma_sp_IND");

            entity.HasIndex(e => e.PrKeyHsbtCt, "hsgd_dx_ct_pr_key_hsbt_ct_IND");

            entity.HasIndex(e => e.PrKeyHsbtCtu, "hsgd_dx_ct_pr_key_hsbt_ctu_IND");

            entity.Property(e => e.PrKey)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.Bl1)
                .HasDefaultValueSql("((1))")
                .HasColumnName("bl_1");
            entity.Property(e => e.Bl2)
                .HasDefaultValueSql("((1))")
                .HasColumnName("bl_2");
            entity.Property(e => e.Bl3)
                .HasDefaultValueSql("((1))")
                .HasColumnName("bl_3");
            entity.Property(e => e.Bl4)
                .HasDefaultValueSql("((1))")
                .HasColumnName("bl_4");
            entity.Property(e => e.Bl5)
                .HasDefaultValueSql("((1))")
                .HasColumnName("bl_5");
            entity.Property(e => e.Bl6).HasColumnName("bl_6");
            entity.Property(e => e.Bl7).HasColumnName("bl_7");
            entity.Property(e => e.Bl8).HasColumnName("bl_8");
            entity.Property(e => e.Bl9)
                .HasDefaultValueSql("((1))")
                .HasColumnName("bl_9");
            entity.Property(e => e.BlDsemail)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("bl_dsemail");
            entity.Property(e => e.BlDsphone)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("bl_dsphone");
            entity.Property(e => e.BlPdbl).HasColumnName("bl_pdbl");
            entity.Property(e => e.BlSendEmail).HasColumnName("bl_send_email");
            entity.Property(e => e.BlTailieubs)
                .HasDefaultValueSql("('')")
                .HasColumnName("bl_tailieubs");
            entity.Property(e => e.ChkKhonghoadon).HasColumnName("chk_khonghoadon");
            entity.Property(e => e.DoituongttTnds)
                .HasDefaultValueSql("('')")
                .HasColumnName("doituongtt_tnds");
            entity.Property(e => e.DonviSuachuaTsk)
                .HasDefaultValueSql("('')")
                .HasColumnName("donvi_suachua_tsk");
            entity.Property(e => e.GhiChu)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chu");
            entity.Property(e => e.GhiChudx)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chudx");
            entity.Property(e => e.HieuXe).HasColumnName("hieu_xe");
            entity.Property(e => e.HieuXeTndsBen3).HasColumnName("hieu_xe_tnds_ben3");
            entity.Property(e => e.LoaiXe).HasColumnName("loai_xe");
            entity.Property(e => e.LoaiXeTndsBen3).HasColumnName("loai_xe_tnds_ben3");
            entity.Property(e => e.LydoCtkh)
                .HasDefaultValueSql("('')")
                .HasColumnName("lydo_ctkh");
            entity.Property(e => e.MaDkhoan)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_dkhoan");
            entity.Property(e => e.MaDonviTt)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi_tt");
            entity.Property(e => e.MaGara)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_gara");
            entity.Property(e => e.MaGara01)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_gara_01");
            entity.Property(e => e.MaGara02)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_gara_02");
            entity.Property(e => e.MaLoaiDongco)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_loai_dongco");
            entity.Property(e => e.MaSp)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_sp");
            entity.Property(e => e.NamSx).HasColumnName("nam_sx");
            entity.Property(e => e.PascSendEmail).HasColumnName("pasc_send_email");
            entity.Property(e => e.PathBaolanh)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("path_baolanh");
            entity.Property(e => e.PathPasc)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("path_pasc");
            entity.Property(e => e.PrKeyHsbtCt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_hsbt_ct");
            entity.Property(e => e.PrKeyHsbtCtu)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_hsbt_ctu");
            entity.Property(e => e.SoTienGtbt).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.SoTienctkh)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienctkh");
            entity.Property(e => e.SotienTtpin)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("sotien_ttpin");
            entity.Property(e => e.TyleggPhutungvcx)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("tylegg_phutungvcx");
            entity.Property(e => e.TyleggSuachuavcx)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("tylegg_suachuavcx");
            entity.Property(e => e.Vat).HasColumnName("vat");
            entity.Property(e => e.VatTnds).HasColumnName("vat_tnds");
            entity.Property(e => e.XuatXu)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("xuat_xu");
        });
        modelBuilder.Entity<DmLdonBt>(entity =>
        {
            entity.HasNoKey();

            entity.ToView("DM_LDON_BT");

            entity.Property(e => e.MaLdonBt)
                .HasMaxLength(8)
                .HasColumnName("ma_ldon_bt");

            entity.Property(e => e.TenLdonBt)
                .HasMaxLength(50)
                .HasColumnName("ten_ldon_bt");
        });
        modelBuilder.Entity<DmLoaiBang>(entity =>
        {
            entity.HasKey(e => e.MaLoaiBang);

            entity.ToTable("dm_loai_bang");

            entity.Property(e => e.MaLoaiBang)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_loai_bang");
            entity.Property(e => e.TenLoaiBang)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_loai_bang");
        });
        modelBuilder.Entity<HsgdDntt>(entity =>
        {
            entity.HasKey(e => e.PrKey).HasName("PK__hsgd_dnt__D53C590CB29BDB44");

            entity.ToTable("hsgd_dntt");

            entity.Property(e => e.PrKey)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.MaCbo)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_cbo");
            entity.Property(e => e.PrKeyTtoanCtu)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_ttoan_ctu");
            entity.Property(e => e.PrKeyTtrinh)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_ttrinh");
            entity.Property(e => e.PrKeyTtrinhCt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_ttrinhct");
            entity.Property(e => e.MaCbcnvXly)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_cbcnv_xly");
            entity.Property(e => e.SoCtu)
               .HasMaxLength(11)
               .HasDefaultValueSql("('')")
               .HasColumnName("so_ctu");
        });

        modelBuilder.Entity<HsgdTotrinhXml>(entity =>
        {
            entity.HasKey(e => e.PrKey).HasName("PK__hsgd_tot__D53C590CFADC2A9D");

            entity.ToTable("hsgd_totrinh_xml");

            entity.Property(e => e.PrKey)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.FrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("fr_key");
            entity.Property(e => e.PathXml)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("path_xml");
            entity.Property(e => e.TenFile)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_file");
        });
        modelBuilder.Entity<DmUserTtoan>(entity =>
        {
            entity.HasKey(e => e.PrKey).HasName("PK__dm_user___D53C590C2E9CE882");

            entity.ToTable("dm_user_ttoan");

            entity.Property(e => e.PrKey)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.DcEmail)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("dc_email");
            entity.Property(e => e.FullName)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("full_name");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi");
            entity.Property(e => e.MaUser)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.TenUser)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_user");
        });
        modelBuilder.Entity<ThongKeGDTT_Item>().HasNoKey();
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
