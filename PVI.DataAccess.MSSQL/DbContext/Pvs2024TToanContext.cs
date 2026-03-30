using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using static System.Net.WebRequestMethods;

namespace PVI.DAO.Entities.Models;

public partial class Pvs2024TToanContext : DbContext
{
    public Pvs2024TToanContext()
    {
    }

    public Pvs2024TToanContext(DbContextOptions<Pvs2024TToanContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DmCbcnv> DmCbcnvs { get; set; }

    public virtual DbSet<DmUserPias> DmUsers { get; set; }

    public virtual DbSet<TtoanCt> TtoanCts { get; set; }

    public virtual DbSet<TtoanCtu> TtoanCtus { get; set; }

    public virtual DbSet<TtoanNhatky> TtoanNhatkies { get; set; }

    public virtual DbSet<TtoanUnc> TtoanUncs { get; set; }
    public virtual DbSet<DmLuongTtoan> DmLuongTtoans { get; set; }
    public virtual DbSet<DmPban> DmPbans { get; set; }
    public virtual DbSet<Ktps> Ktps { get; set; }

    public virtual DbSet<NhatKyPias> NhatKies { get; set; }

    public string connect_pias_ttoan = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["PiasTToanContext"]!;
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(connect_pias_ttoan);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DmCbcnv>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("DM_CBCNV");

            entity.Property(e => e.KhongSdung).HasColumnName("khong_sdung");
            entity.Property(e => e.MaCbcnv)
                .HasMaxLength(11)
                .HasColumnName("Ma_cbcnv");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(8)
                .HasColumnName("ma_donvi");
            entity.Property(e => e.MaPban)
                .HasMaxLength(11)
                .HasColumnName("ma_pban");
            entity.Property(e => e.TenCbcnv)
                .HasMaxLength(150)
                .HasColumnName("Ten_cbcnv");
            entity.Property(e => e.ViewAll).HasColumnName("view_all");
        });
        modelBuilder.Entity<DmUserPias>(entity =>
        {
            entity.HasKey(e => e.MaUser).HasName("PK_Dm_user");

            entity.ToTable("dm_user", tb =>
            {
                tb.HasTrigger("Rep_Td_dm_user");
                tb.HasTrigger("Rep_Ti_dm_user");
                tb.HasTrigger("Rep_Tu_dm_user");
            });

            entity.HasIndex(e => e.TenUser, "Ten_user_index");

            entity.HasIndex(e => e.DcEmail, "dm_user_dc_email_IND");

            entity.HasIndex(e => e.MaCbo, "dm_user_ma_cbo_IND");

            entity.Property(e => e.MaUser)
                .HasMaxLength(10)
                .HasColumnName("ma_user");
            entity.Property(e => e.BanQuanly)
                .HasDefaultValueSql("((0))")
                .HasColumnName("ban_quanly");
            entity.Property(e => e.ChkChuyendonRi).HasColumnName("chk_chuyendonRI");
            entity.Property(e => e.ChkModonRi).HasColumnName("chk_modonRI");
            entity.Property(e => e.DcEmail)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("dc_email");
            entity.Property(e => e.DuyetTaifile).HasColumnName("duyet_taifile");
            entity.Property(e => e.FullName)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("full_name");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chu");
            entity.Property(e => e.KichhoatEmail).HasColumnName("kichhoat_email");
            entity.Property(e => e.KyHddtQlcd).HasColumnName("ky_hddt_qlcd");
            entity.Property(e => e.LiveNum).HasColumnName("live_num");
            entity.Property(e => e.LiveTime)
                .HasColumnType("datetime")
                .HasColumnName("live_time");
            entity.Property(e => e.LoaiKenh)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')")
                .HasColumnName("loai_kenh");
            entity.Property(e => e.LoginFailNumber).HasColumnName("login_fail_number");
            entity.Property(e => e.MaBenhvien)
                .HasMaxLength(20)
                .HasColumnName("ma_benhvien");
            entity.Property(e => e.MaCbo)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_cbo");
            entity.Property(e => e.MaChucvu)
                .HasMaxLength(2)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_chucvu");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi");
            entity.Property(e => e.MaNhang)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_nhang");
            entity.Property(e => e.MaNhom)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_nhom");
            entity.Property(e => e.MaPhong)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_phong");
            entity.Property(e => e.MaTthai)
                .HasMaxLength(2)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_tthai");
            entity.Property(e => e.MaTthaiTtoan)
                .HasMaxLength(8)
                .HasDefaultValueSql("('01')")
                .HasColumnName("ma_tthai_ttoan");
            entity.Property(e => e.NgayCnhat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cnhat");
            entity.Property(e => e.NgayCuoi)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cuoi");
            entity.Property(e => e.NgayKtao)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_ktao");
            entity.Property(e => e.NguoidungBlvp).HasColumnName("nguoidung_BLVP");
            entity.Property(e => e.OtpCode)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasDefaultValueSql("('')")
                .HasColumnName("OTP_code");
            entity.Property(e => e.OtpDisable).HasColumnName("OTP_disable");
            entity.Property(e => e.ParentId)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')");
            entity.Property(e => e.Password)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("password");
            entity.Property(e => e.PasswordSha256)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("password_sha256");
            entity.Property(e => e.PhanQuyen).HasColumnName("phan_quyen");
            entity.Property(e => e.QthtBlvp).HasColumnName("qtht_BLVP");
            entity.Property(e => e.TenUser)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_user");
            entity.Property(e => e.TrangThai)
                .IsRequired()
                .HasDefaultValueSql("((1))")
                .HasColumnName("trang_thai");
        });

        modelBuilder.Entity<TtoanCt>(entity =>
        {
            entity.HasKey(e => e.PrKey).HasName("PK_ttoan_ct_new");

            entity.ToTable("ttoan_ct");

            entity.HasIndex(e => e.FrKey, "Fr_key_Index");

            entity.Property(e => e.PrKey)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.DoanhSo)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("doanh_so");
            entity.Property(e => e.DoanhSoHdon)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("doanh_so_hdon");
            entity.Property(e => e.DuongDan)
                .HasMaxLength(2000)
                .HasDefaultValueSql("('')")
                .HasColumnName("duong_dan");
            entity.Property(e => e.FrKey)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("fr_key");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chu");
            entity.Property(e => e.HdongJson)
                .HasMaxLength(2000)
                .HasDefaultValueSql("('')")
                .HasColumnName("hdong_json");
            entity.Property(e => e.IsValid)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("isValid");
            entity.Property(e => e.IsXml).HasColumnName("isXML");
            entity.Property(e => e.KichCo)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("kich_co");
            entity.Property(e => e.MaHdong)
                .HasMaxLength(30)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_hdong");
            entity.Property(e => e.MaKhVat)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_kh_vat");
            entity.Property(e => e.MaSovat)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_sovat");
            entity.Property(e => e.MauSovat)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("mau_sovat");
            entity.Property(e => e.MsgValid)
                .HasDefaultValueSql("('')")
                .HasColumnName("msg_Valid");
            entity.Property(e => e.MsgValid2)
                .HasDefaultValueSql("('')")
                .HasColumnName("msg_Valid2");
            entity.Property(e => e.NgayHdvat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_hdvat");
            entity.Property(e => e.SerieVat)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("serie_vat");
            entity.Property(e => e.SoHdvat)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_hdvat");
            entity.Property(e => e.TenFile)
                .HasMaxLength(2000)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_file");
            entity.Property(e => e.TenHhoa)
                .HasMaxLength(4000)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_hhoa");
            entity.Property(e => e.TenKhVat)
                .HasMaxLength(2000)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_kh_vat");
            entity.Property(e => e.TienVat)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("tien_vat");
            entity.Property(e => e.TienVatHdon)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tien_vat_hdon");
            entity.Property(e => e.TsuatVat)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("tsuat_vat");
        });

        modelBuilder.Entity<TtoanCtu>(entity =>
        {
            entity.HasKey(e => e.PrKey).HasName("PK_ttoan_ctu_new");

            entity.ToTable("ttoan_ctu");

            entity.HasIndex(e => new { e.MaDonvi, e.MaPban }, "Ma_dvi_idx");

            entity.HasIndex(e => e.NgayCtu, "Ngay_ctu_idx");

            entity.HasIndex(e => new { e.MaDonvi, e.MaHttoan, e.PrKeyKtps, e.MaCtuTtoan, e.NgayCtu }, "ttoan_ctu_ma_donvi_ma_httoan_pr_key_ktps_ma_ctu_ttoan_ngay_ctu_IND");

            entity.Property(e => e.PrKey)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("Pr_key");
            entity.Property(e => e.BsCtu).HasColumnName("bs_ctu");
            entity.Property(e => e.CancuDenghi)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("cancu_denghi");
            entity.Property(e => e.CodeNhangTg)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("code_nhang_tg");
            entity.Property(e => e.CtuKtheo)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("Ctu_ktheo");
            entity.Property(e => e.DiaChiNh)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("Dia_chi_nh");
            entity.Property(e => e.DiachiNguoiTh)
                .HasMaxLength(1500)
                .HasDefaultValueSql("('')")
                .HasColumnName("diachi_nguoi_th");
            entity.Property(e => e.DiachiNhangTg)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("diachi_nhang_tg");
            entity.Property(e => e.DienGiai)
                .HasMaxLength(4000)
                .HasDefaultValueSql("('')")
                .HasColumnName("Dien_giai");
            entity.Property(e => e.DonVi)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("Don_vi");
            entity.Property(e => e.DsTtrinh)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ds_ttrinh");
            entity.Property(e => e.DuongDan)
                .HasMaxLength(2000)
                .HasDefaultValueSql("('')")
                .HasColumnName("duong_dan");
            entity.Property(e => e.HanTtoan)
                .HasColumnType("smalldatetime")
                .HasColumnName("han_ttoan");
            entity.Property(e => e.HthucCkhoan).HasColumnName("hthuc_ckhoan");
            entity.Property(e => e.IsCtien).HasColumnName("isCtien");
            entity.Property(e => e.IsCtienTheoDs).HasColumnName("isCtienTheoDS");
            entity.Property(e => e.LapUnc).HasColumnName("lap_unc");
            entity.Property(e => e.LoaiCphi)
                .HasMaxLength(9)
                .HasDefaultValueSql("('')")
                .HasColumnName("loai_cphi");
            entity.Property(e => e.LoaiTtoan)
                .HasMaxLength(30)
                .HasDefaultValueSql("('')")
                .HasColumnName("loai_ttoan");
            entity.Property(e => e.MaCbcnv)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("Ma_cbcnv");
            entity.Property(e => e.MaCbcnvXly)
                .HasMaxLength(30)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_cbcnv_xly");
            entity.Property(e => e.MaCtuTtoan)
                .HasMaxLength(4)
                .HasDefaultValueSql("('')")
                .HasColumnName("Ma_ctu_ttoan");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("Ma_donvi");
            entity.Property(e => e.MaHttoan)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("Ma_httoan");
            entity.Property(e => e.MaPban)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("Ma_pban");
            entity.Property(e => e.MaTte)
                .HasMaxLength(3)
                .HasDefaultValueSql("('')")
                .HasColumnName("Ma_tte");
            entity.Property(e => e.MaUser)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.MaUserKtoan)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user_ktoan");
            entity.Property(e => e.NamHt).HasColumnName("nam_ht");
            entity.Property(e => e.NgayCnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("Ngay_cnhat");
            entity.Property(e => e.NgayCtu)
                .HasColumnType("smalldatetime")
                .HasColumnName("Ngay_ctu");
            entity.Property(e => e.NguoiGdich)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("Nguoi_gdich");
            entity.Property(e => e.NguoiHuong)
                .HasMaxLength(150)
                .HasDefaultValueSql("('')")
                .HasColumnName("Nguoi_huong");
            entity.Property(e => e.NhangCode)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("nhang_code");
            entity.Property(e => e.PrKeyKtps)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("Pr_key_ktps");
            entity.Property(e => e.PrKeyLuong)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_luong");
            entity.Property(e => e.SoCtu)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_ctu");
            entity.Property(e => e.SoTknh)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_tknh");
            entity.Property(e => e.TenBangLuong)
                .HasMaxLength(30)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_bang_luong");
            entity.Property(e => e.TenFile)
                .HasMaxLength(2000)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_file");
            entity.Property(e => e.TenNhangTg)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_nhang_tg");
            entity.Property(e => e.TenTknh)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_tknh");
            entity.Property(e => e.TongTien)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("tong_tien");
            entity.Property(e => e.TongTienKvat)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("tong_tien_kvat");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(11)
                .HasDefaultValueSql("('01')")
                .HasColumnName("trang_thai");
            entity.Property(e => e.TtinLquan)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("Ttin_lquan");
            entity.Property(e => e.TygiaHt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Tygia_ht");
            entity.Property(e => e.TygiaTt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Tygia_tt");
            entity.Property(e => e.UserCtien)
                .HasMaxLength(30)
                .HasDefaultValueSql("('')")
                .HasColumnName("user_Ctien");
        });

        modelBuilder.Entity<TtoanNhatky>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ttoan_nhatky");

            entity.Property(e => e.FrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("fr_key");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(500)
                .HasColumnName("ghi_chu");
            entity.Property(e => e.KyTtoan)
                .HasColumnType("decimal(9, 0)")
                .HasColumnName("ky_ttoan");
            entity.Property(e => e.MaUserKtoan)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user_ktoan");
            entity.Property(e => e.NgayCnhat)
                .HasColumnType("datetime")
                .HasColumnName("ngay_cnhat");
            entity.Property(e => e.NgayCnhat1)
                .HasColumnType("datetime")
                .HasColumnName("ngay_cnhat_1");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PathCtuKy)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("path_ctu_ky");
            entity.Property(e => e.PathUncKy)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("path_unc_ky");
            entity.Property(e => e.PrKey)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(9)
                .HasDefaultValueSql("('')")
                .HasColumnName("trang_thai");
            entity.Property(e => e.UserChuyen)
                .HasMaxLength(50)
                .HasColumnName("user_chuyen");
            entity.Property(e => e.UserNhan)
                .HasMaxLength(50)
                .HasColumnName("user_nhan");
        });

        modelBuilder.Entity<TtoanUnc>(entity =>
        {
            entity.HasKey(e => e.TxId);

            entity.ToTable("ttoan_unc");

            entity.Property(e => e.TxId)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("txId");
            entity.Property(e => e.BenBnkCd)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("benBnkCd");
            entity.Property(e => e.FrAcctId)
                .HasMaxLength(34)
                .HasDefaultValueSql("('')")
                .HasColumnName("frAcctId");
            entity.Property(e => e.FrAcctNm)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("frAcctNm");
            entity.Property(e => e.FrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("fr_key");
            entity.Property(e => e.GhiChu)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chu");
            entity.Property(e => e.MessageId)
                .HasMaxLength(30)
                .HasDefaultValueSql("('')")
                .HasColumnName("messageId");
            entity.Property(e => e.NgayGuiUnc)
                .HasColumnType("datetime")
                .HasColumnName("ngay_gui_unc");
            entity.Property(e => e.SignChecker)
                .HasDefaultValueSql("('')")
                .HasColumnName("signChecker");
            entity.Property(e => e.SignMaker)
                .HasDefaultValueSql("('')")
                .HasColumnName("signMaker");
            entity.Property(e => e.ToAccId)
                .HasMaxLength(34)
                .HasDefaultValueSql("('')");
            entity.Property(e => e.ToAccNm)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')");
            entity.Property(e => e.TrangThai).HasColumnName("trang_thai");
            entity.Property(e => e.TrangThaiTkhoan).HasColumnName("trang_thai_tkhoan");
            entity.Property(e => e.TxAmt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("txAmt");
            entity.Property(e => e.TxDesc)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("txDesc");
            entity.Property(e => e.TxDt)
                .HasColumnType("datetime")
                .HasColumnName("txDt");
            entity.Property(e => e.TxIdTct)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("txIdTCT");
            entity.Property(e => e.TxnDate)
                .HasColumnType("datetime")
                .HasColumnName("txnDate");
            entity.Property(e => e.UsrChecker)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("usrChecker");
            entity.Property(e => e.UsrMaker)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("usrMaker");
        });
        modelBuilder.Entity<DmLuongTtoan>(entity =>
        {
            entity.HasKey(e => e.PrKey).HasName("PK__dm_luong__D53C590C9C58CD8D");

            entity.ToTable("dm_luong_ttoan");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.IsUse).HasColumnName("isUse");
            entity.Property(e => e.LoaiCphi)
                .HasMaxLength(22)
                .HasDefaultValueSql("('')")
                .HasColumnName("loai_cphi");
            entity.Property(e => e.LuongKy)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("luong_ky");
            entity.Property(e => e.LuongXly)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("luong_xly");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi");
            entity.Property(e => e.NgayHluc)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_hluc");
            entity.Property(e => e.TenLuongTtoan)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_luong_ttoan");
        });
        modelBuilder.Entity<DmPban>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("DM_PBAN");

            entity.Property(e => e.MaDonvi)
                .HasMaxLength(8)
                .HasColumnName("ma_donvi");
            entity.Property(e => e.MaDonviPban)
                .HasMaxLength(8)
                .HasColumnName("ma_donvi_pban");
            entity.Property(e => e.MaPban)
                .HasMaxLength(11)
                .HasColumnName("ma_pban");
            entity.Property(e => e.TenPban)
                .HasMaxLength(150)
                .HasColumnName("ten_pban");
            entity.Property(e => e.TenPbanEng)
                .HasMaxLength(100)
                .HasColumnName("ten_pban_eng");
            entity.Property(e => e.TenTat)
                .HasMaxLength(50)
                .HasColumnName("ten_tat");
            entity.Property(e => e.ViewAll).HasColumnName("view_all");
        });
        modelBuilder.Entity<Ktps>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("KTPS");

            entity.HasIndex(e => e.MaDonvi, "Ma_donvi_Index");

            entity.Property(e => e.PrKey)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("Pr_key");
            entity.Property(e => e.DienGiai)
                .HasMaxLength(150)
                .HasDefaultValueSql("('')")
                .HasColumnName("Dien_giai");
            entity.Property(e => e.DonVi)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("Don_vi");
            entity.Property(e => e.KhongDgcltg).HasColumnName("khong_dgcltg");
            entity.Property(e => e.LapUnc).HasColumnName("lap_unc");
            entity.Property(e => e.MaCtu)
                .HasMaxLength(4)
                .HasDefaultValueSql("('')")
                .HasColumnName("Ma_ctu");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("Ma_donvi");
            entity.Property(e => e.MaTte)
                .HasMaxLength(3)
                .HasDefaultValueSql("('')")
                .HasColumnName("Ma_tte");
            entity.Property(e => e.MaUser)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.MaUserUnc)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user_unc");
            entity.Property(e => e.NgayCnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("Ngay_cnhat");
            entity.Property(e => e.NgayCnhatUnc)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cnhat_unc");
            entity.Property(e => e.NgayCtu)
                .HasColumnType("smalldatetime")
                .HasColumnName("Ngay_ctu");
            entity.Property(e => e.NgayHt)
                .HasColumnType("smalldatetime")
                .HasColumnName("Ngay_ht");
            entity.Property(e => e.NguoiGdich)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("Nguoi_gdich");
            entity.Property(e => e.NhangCode)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("nhang_code");
            entity.Property(e => e.SoCtu)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("So_ctu");
            entity.Property(e => e.SoTknh)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_tknh");
            entity.Property(e => e.TenTknh)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_tknh");
            entity.Property(e => e.TygiaHt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Tygia_ht");
            entity.Property(e => e.TygiaTt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Tygia_tt");
            entity.Property(e => e.VersionEdit)
                .HasColumnType("datetime")
                .HasColumnName("version_edit");
        });

        modelBuilder.Entity<NhatKyPias>(entity =>
        {
            entity.ToTable("NHAT_KY", tb => tb.HasTrigger("Ti_nhat_ky"));

            entity.HasIndex(e => new { e.PrKeyCtu, e.MaDonvi }, "Pr_key_idx");

            entity.HasIndex(e => e.PrKeyCtu, "nhat_ky_pr_key_ctu_IND");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("ID");
            entity.Property(e => e.LanCnhat)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("lan_cnhat");
            entity.Property(e => e.MaBomb)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("ma_bomb");
            entity.Property(e => e.MaCty)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_cty");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi");
            entity.Property(e => e.NgayCnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cnhat");
            entity.Property(e => e.NgayCtu)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_ctu");
            entity.Property(e => e.PhanHe)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("phan_he");
            entity.Property(e => e.PhienBan)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("phien_ban");
            entity.Property(e => e.PrKey)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.PrKeyCtu)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("pr_key_ctu");
            entity.Property(e => e.SoCtu)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_ctu");
            entity.Property(e => e.SuKien)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("su_kien");
            entity.Property(e => e.TenMay)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_may");
            entity.Property(e => e.TenUser)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_user");
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
