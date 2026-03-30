using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PVI.Helper;

namespace PVI.DAO.Entities.Models;

public partial class Pvs2024UpdateContext : DbContext
{
    public Pvs2024UpdateContext()
    {
    }

    public Pvs2024UpdateContext(DbContextOptions<Pvs2024UpdateContext> options)
        : base(options)
    {
    }
    public virtual DbSet<HsbtCtu> HsbtCtus { get; set; }
    public virtual DbSet<HsbtCt> HsbtCts { get; set; }
    public virtual DbSet<HsbtGd> HsbtGds { get; set; }

    public virtual DbSet<HsbtTht> HsbtThts { get; set; }

    public virtual DbSet<HsbtUoc> HsbtUocs { get; set; }

    public virtual DbSet<HsbtUocGd> HsbtUocGds { get; set; }
    public virtual DbSet<DmCtukt> DmCtukts { get; set; }
    public virtual DbSet<FileAttachBt> FileAttachBts { get; set; }
    public virtual DbSet<DmPhe> DmPhes { get; set; }

    public virtual DbSet<DmPheCt> DmPheCts { get; set; }
    public string connect_pias_update = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["PiasUpdateContext"]!;
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(connect_pias_update);
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HsbtCtu>(entity =>
        {
            entity.HasKey(e => e.PrKey).IsClustered(false);

            entity.ToTable("Hsbt_ctu", tb =>
            {
                tb.HasTrigger("Rep_Td_Hsbt_ctu");
                tb.HasTrigger("Rep_Ti_Hsbt_ctu");
                tb.HasTrigger("Rep_Tu_Hsbt_ctu");
            });

            entity.HasIndex(e => e.MaCtu, "hsbt_ctu_ma_ctu_INC_IND");

            entity.HasIndex(e => new { e.MaCtu, e.MaDonvi }, "hsbt_ctu_ma_ctu_ma_donvi_IND");

            entity.HasIndex(e => e.NgayCtu, "hsbt_ctu_ngay_ctu_IND");

            entity.HasIndex(e => e.PrKeyBth, "hsbt_ctu_pr_key_bth_IND");

            entity.HasIndex(e => e.SoDonbhSdbs, "hsbt_ctu_so_donbh_sdbs_IND");

            entity.HasIndex(e => new { e.SoHdgcn, e.SoSeri, e.NgayThuPhi }, "hsbt_ctu_so_hdgcn_so_seri_ngay_thu_phi_IND");

            entity.HasIndex(e => e.SoHsbt, "hsbt_ctu_so_hsbt_IND");

            entity.HasIndex(e => e.SoSeri, "hsbt_ctu_so_seri_IND");

            entity.HasIndex(e => e.SoThe, "hsbt_ctu_so_the_IND");

            entity.Property(e => e.PrKey)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.BiensoXeTnds)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("bienso_xe_tnds");
            entity.Property(e => e.BlTt).HasColumnName("bl_tt");
            entity.Property(e => e.CancuDexuat)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("cancu_dexuat");
            entity.Property(e => e.CancuDexuatTnds)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("cancu_dexuat_tnds");
            entity.Property(e => e.CheTai)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("che_tai");
            entity.Property(e => e.CheTaiTnds)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("che_tai_tnds");
            entity.Property(e => e.ChiKhac)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("chi_khac");
            entity.Property(e => e.ChkCpd).HasColumnName("chk_cpd");
            entity.Property(e => e.ChkHuybt).HasColumnName("chk_huybt");
            entity.Property(e => e.CtuKemtheo)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("ctu_kemtheo");
            entity.Property(e => e.CtuKtheo)
                .HasDefaultValueSql("('')")
                .HasColumnName("ctu_ktheo");
            entity.Property(e => e.DchiRuong)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("dchi_ruong");
            entity.Property(e => e.DexuatPan)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("dexuat_pan");
            entity.Property(e => e.DgiaDam)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("dgia_dam");
            entity.Property(e => e.DiaChi)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("dia_chi");
            entity.Property(e => e.DiaDiem)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("dia_diem");
            entity.Property(e => e.DienThoai)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("dien_thoai");
            entity.Property(e => e.DiengiaiBt)
                .HasColumnType("ntext")
                .HasColumnName("diengiai_bt");
            entity.Property(e => e.DtichRuong)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("dtich_ruong");
            entity.Property(e => e.DuyetHsbt).HasColumnName("duyet_hsbt");
            entity.Property(e => e.DuyetPcap).HasColumnName("duyet_pcap");
            entity.Property(e => e.Email)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("email");
            entity.Property(e => e.GhiChu)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chu");
            entity.Property(e => e.GhiChuThuoc)
                .HasMaxLength(2500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chu_thuoc");
            entity.Property(e => e.GiatriThuhoi)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("giatri_thuhoi");
            entity.Property(e => e.GiatriTteXe)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("giatri_tte_xe");
            entity.Property(e => e.GtrinhChikhac)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("gtrinh_chikhac");
            entity.Property(e => e.HauQua)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("hau_qua");
            entity.Property(e => e.HosoPcap)
                .HasMaxLength(8)
                .HasDefaultValueSql("('DPC')")
                .HasColumnName("hoso_pcap");
            entity.Property(e => e.HosoPhaply)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("hoso_phaply");
            entity.Property(e => e.HthucTtoan).HasColumnName("hthuc_ttoan");
            entity.Property(e => e.HuyenKhach)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("huyen_khach");
            entity.Property(e => e.HuyenRuong)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("huyen_ruong");
            entity.Property(e => e.KhauHao)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("khau_hao");
            entity.Property(e => e.KhauHaoTnds)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("khau_hao_tnds");
            entity.Property(e => e.KluongDam)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("kluong_dam");
            entity.Property(e => e.KquaDtri)
                .HasMaxLength(2500)
                .HasDefaultValueSql("('')")
                .HasColumnName("kqua_dtri");
            entity.Property(e => e.LoaiXe)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("loai_xe");
            entity.Property(e => e.LoaiXeTnds)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("loai_xe_tnds");
            entity.Property(e => e.LydoTcap)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("lydo_tcap");
            entity.Property(e => e.LydoTuchoibt)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("lydo_tuchoibt");
            entity.Property(e => e.MaBtXol)
                .HasMaxLength(15)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_bt_xol");
            entity.Property(e => e.MaCbcnv)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_cbcnv");
            entity.Property(e => e.MaCbgd)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_cbgd");
            entity.Property(e => e.MaCbkt)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_cbkt");
            entity.Property(e => e.MaCtu)
                .HasMaxLength(4)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ctu");
            entity.Property(e => e.MaDaily)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_daily");
            entity.Property(e => e.MaDdiemTthat)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ddiem_tthat");
            entity.Property(e => e.MaDieutri)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_dieutri");
            entity.Property(e => e.MaDonbh)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donbh");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi");
            entity.Property(e => e.MaDvbtHo)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_dvbt_ho");
            entity.Property(e => e.MaGaraTnds)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_gara_tnds");
            entity.Property(e => e.MaGaraTnds2)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_gara_tnds2");
            entity.Property(e => e.MaGaraTnds3)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_gara_tnds3");
            entity.Property(e => e.MaGaraVcx)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_gara_vcx");
            entity.Property(e => e.MaGaraVcx2)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_gara_vcx2");
            entity.Property(e => e.MaGaraVcx3)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_gara_vcx3");
            entity.Property(e => e.MaHoi)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_hoi");
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
            entity.Property(e => e.MaLoaixe)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_loaixe");
            entity.Property(e => e.MaLruiro)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_lruiro");
            entity.Property(e => e.MaLydoTuchoibt)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_lydo_tuchoibt");
            entity.Property(e => e.MaNgnhanTn)
                .HasMaxLength(5)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ngnhan_tn");
            entity.Property(e => e.MaNnhanTthat)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_nnhan_tthat");
            entity.Property(e => e.MaPkt)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_pkt");
            entity.Property(e => e.MaTte)
                .HasMaxLength(3)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_tte");
            entity.Property(e => e.MaUser)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.NamSinh)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("nam_sinh");
            entity.Property(e => e.NamsxTnds)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("namsx_tnds");
            entity.Property(e => e.NamsxVcx)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("namsx_vcx");
            entity.Property(e => e.NgayCapd)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_capd");
            entity.Property(e => e.NgayCtu)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_ctu");
            entity.Property(e => e.NgayCuoi)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cuoi");
            entity.Property(e => e.NgayCuoiLaixe)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cuoi_laixe");
            entity.Property(e => e.NgayCuoiLuuhanh)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cuoi_luuhanh");
            entity.Property(e => e.NgayCuoit)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cuoit");
            entity.Property(e => e.NgayDau)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_dau");
            entity.Property(e => e.NgayDauLaixe)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_dau_laixe");
            entity.Property(e => e.NgayDauLuuhanh)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_dau_luuhanh");
            entity.Property(e => e.NgayDaut)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_daut");
            entity.Property(e => e.NgayDkyxe)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_dkyxe");
            entity.Property(e => e.NgayDkyxeTnds)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_dkyxe_tnds");
            entity.Property(e => e.NgayDuyet)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_duyet");
            entity.Property(e => e.NgayDuyetgia)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_duyetgia");
            entity.Property(e => e.NgayGdinh)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_gdinh");
            entity.Property(e => e.NgayKham)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_kham");
            entity.Property(e => e.NgayPhhanhCv)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_phhanh_cv");
            entity.Property(e => e.NgayTbao)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_tbao");
            entity.Property(e => e.NgayThuPhi)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_thu_phi");
            entity.Property(e => e.NgayTthat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_tthat");
            entity.Property(e => e.NgayXxuong)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_xxuong");
            entity.Property(e => e.NgayYcaubt)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_ycaubt");
            entity.Property(e => e.NgaydPcap)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngayd_pcap");
            entity.Property(e => e.NgdcBh)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ngdc_bh");
            entity.Property(e => e.NgnhanTuchoibt)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("ngnhan_tuchoibt");
            entity.Property(e => e.NguyenNhan)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("nguyen_nhan");
            entity.Property(e => e.NguyenNhanTtat)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("nguyen_nhan_ttat");
            entity.Property(e => e.NoiDieutri)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("noi_dieutri");
            entity.Property(e => e.NoiKham)
                .HasMaxLength(150)
                .HasDefaultValueSql("('')")
                .HasColumnName("noi_kham");
            entity.Property(e => e.NoidungDenghi)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("noidung_denghi");
            entity.Property(e => e.PanTdoiNt3)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("pan_tdoi_nt3");
            entity.Property(e => e.PanThoiTs)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("pan_thoi_ts");
            entity.Property(e => e.PrKeyBth)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_bth");
            entity.Property(e => e.PrKeyGoc)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_goc");
            entity.Property(e => e.PrKeySeri)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_seri");
            entity.Property(e => e.SoBthuong)
                .HasColumnType("decimal(9, 0)")
                .HasColumnName("so_bthuong");
            entity.Property(e => e.SoCmnd)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_cmnd");
            entity.Property(e => e.SoDonbhSdbs)
                .HasMaxLength(50)
                .HasComputedColumnSql("(case when [so_donbhbs]<>'' then [so_donbhbs] else [so_hdgcn] end)", true)
                .HasColumnName("so_donbh_sdbs");
            entity.Property(e => e.SoDonbhTaibh)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_donbh_taibh");
            entity.Property(e => e.SoDonbhbs)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_donbhbs");
            entity.Property(e => e.SoGcnbh)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_gcnbh");
            entity.Property(e => e.SoGphepLaixe)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_gphep_laixe");
            entity.Property(e => e.SoGphepLuuhanh)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_gphep_luuhanh");
            entity.Property(e => e.SoHdbh)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_hdbh");
            entity.Property(e => e.SoHdgcn)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_hdgcn");
            entity.Property(e => e.SoHsbt)
                .HasMaxLength(30)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_hsbt");
            entity.Property(e => e.SoHsbtCommos)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_hsbt_commos");
            entity.Property(e => e.SoHshoi)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_hshoi");
            entity.Property(e => e.SoLanBt).HasColumnName("so_lan_bt");
            entity.Property(e => e.SoNgchet)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_ngchet");
            entity.Property(e => e.SoPhibh)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_phibh");
            entity.Property(e => e.SoSeri)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_seri");
            entity.Property(e => e.SoThe)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_the");
            entity.Property(e => e.SotheBhyt)
                .HasMaxLength(30)
                .HasDefaultValueSql("('')")
                .HasColumnName("sothe_bhyt");
            entity.Property(e => e.SotheVcxMoto)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("sothe_vcx_moto");
            entity.Property(e => e.SotienDenghiTtoan)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("sotien_denghi_ttoan");
            entity.Property(e => e.SotienTnds)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("sotien_tnds");
            entity.Property(e => e.SotienTnds2)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("sotien_tnds2");
            entity.Property(e => e.SotienTnds3)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("sotien_tnds3");
            entity.Property(e => e.SotienTuchoibt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("sotien_tuchoibt");
            entity.Property(e => e.SotienVcx)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("sotien_vcx");
            entity.Property(e => e.SotienVcx2)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("sotien_vcx2");
            entity.Property(e => e.SotienVcx3)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("sotien_vcx3");
            entity.Property(e => e.SotkNghang)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("sotk_nghang");
            entity.Property(e => e.TaisanThuhoi)
                .HasMaxLength(1800)
                .HasDefaultValueSql("('')")
                .HasColumnName("taisan_thuhoi");
            entity.Property(e => e.TcnamVien)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("tcnam_vien");
            entity.Property(e => e.TenBenhMt)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_benh_mt");
            entity.Property(e => e.TenChuxeTnds)
                .HasMaxLength(150)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_chuxe_tnds");
            entity.Property(e => e.TenDttt)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_dttt");
            entity.Property(e => e.TenKhle)
                .HasMaxLength(2500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_khle");
            entity.Property(e => e.TenLaixe)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_laixe");
            entity.Property(e => e.TenNghang)
                .HasMaxLength(150)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_nghang");
            entity.Property(e => e.TenngDenghi)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("tenng_denghi");
            entity.Property(e => e.TenngNhtien)
                .HasMaxLength(150)
                .HasDefaultValueSql("('')")
                .HasColumnName("tenng_nhtien");
            entity.Property(e => e.ThamGia007).HasColumnName("tham_gia007");
            entity.Property(e => e.ThinhNphi)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("thinh_nphi");
            entity.Property(e => e.ThonKhach)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("thon_khach");
            entity.Property(e => e.ThonRuong)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("thon_ruong");
            entity.Property(e => e.ThuaRuong)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("thua_ruong");
            entity.Property(e => e.TienBhyt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("tien_bhyt");
            entity.Property(e => e.TienChetai)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tien_chetai");
            entity.Property(e => e.TienKham)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("tien_kham");
            entity.Property(e => e.TienThuoc)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("tien_thuoc");
            entity.Property(e => e.TienThuthuat)
                .HasDefaultValueSql("((0))")
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("tien_thuthuat");
            entity.Property(e => e.TienXnat)
                .HasDefaultValueSql("((0))")
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("tien_xnat");
            entity.Property(e => e.TienXndt)
                .HasDefaultValueSql("((0))")
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("tien_xndt");
            entity.Property(e => e.TinhKhach)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("tinh_khach");
            entity.Property(e => e.TinhRuong)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("tinh_ruong");
            entity.Property(e => e.TinhToanbtCng)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("tinh_toanbt_cng");
            entity.Property(e => e.TinhToanbtHhoa)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("tinh_toanbt_hhoa");
            entity.Property(e => e.TinhToanbtLpx)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("tinh_toanbt_lpx");
            entity.Property(e => e.TinhToanbtVcx)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("tinh_toanbt_vcx");
            entity.Property(e => e.TinhtoanbtCng1)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("tinhtoanbt_cng");
            entity.Property(e => e.TtrangXe)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ttrang_xe");
            entity.Property(e => e.TygiaHt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tygia_ht");
            entity.Property(e => e.TygiaTt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tygia_tt");
            entity.Property(e => e.TyleDong)
                .HasDefaultValueSql("((0))")
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tyle_dong");
            entity.Property(e => e.TyleTtat)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tyle_ttat");
            entity.Property(e => e.UserDuyet)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')")
                .HasColumnName("user_duyet");
            entity.Property(e => e.UserdPcap)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("userd_pcap");
            entity.Property(e => e.VersionEdit)
                .HasColumnType("datetime")
                .HasColumnName("version_edit");
            entity.Property(e => e.VienPhi)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("vien_phi");
            entity.Property(e => e.VuluaRuong)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("vulua_ruong");
            entity.Property(e => e.XaKhach)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("xa_khach");
            entity.Property(e => e.XaRuong)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("xa_ruong");
            entity.Property(e => e.YkienGdinh)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("ykien_gdinh");
            entity.Property(e => e.MaDonviTt)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi_tt");
        });
        modelBuilder.Entity<HsbtCt>(entity =>
        {
            entity.HasKey(e => e.PrKey)
                .HasName("PK_hsbt_ct")
                .IsClustered(false);

            entity.ToTable("Hsbt_ct", tb =>
            {
                tb.HasTrigger("Rep_Td_Hsbt_ct");
                tb.HasTrigger("Rep_Ti_Hsbt_ct");
                tb.HasTrigger("Rep_Tu_Hsbt_ct");
            });

            entity.HasIndex(e => e.NgayHtoanBt, "IX_Hsbt_ct_ngay_htoan_bt");

            entity.HasIndex(e => e.FrKey, "hsbt_ct_fr_key_IND");

            entity.HasIndex(e => e.PrKeyBthCt, "hsbt_ct_pr_key_bth_ct_IND");

            entity.HasIndex(e => new { e.NgayHtoanBt, e.PrKeyCareNoitru }, "hsbt_ct_pr_key_care_noitru_IND");

            entity.HasIndex(e => new { e.NgayHtoanBt, e.PrKeyKbttHsbtCt }, "hsbt_ct_pr_key_kbtt_hsbt_ct_IND");

            entity.HasIndex(e => new { e.NgayHtoanBt, e.PrKeyNvuBhtCt }, "hsbt_ct_pr_key_nvu_bht_ct_IND");

            entity.Property(e => e.PrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.ChenhLechKtru)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("chenh_lech_ktru");
            entity.Property(e => e.FrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("fr_key");
            entity.Property(e => e.GhiChuBt)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chu_bt");
            entity.Property(e => e.LoaiPhiPi)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')")
                .HasColumnName("loai_phi_pi");
            entity.Property(e => e.MaDkhoan)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_dkhoan");
            entity.Property(e => e.MaIcd)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_icd");
            entity.Property(e => e.MaKhvat)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_khvat");
            entity.Property(e => e.MaQuyenloi)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_quyenloi");
            entity.Property(e => e.MaSp)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_sp");
            entity.Property(e => e.MaTteGoc)
                .HasMaxLength(3)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_tte_goc");
            entity.Property(e => e.MaTtebt)
                .HasMaxLength(3)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ttebt");
            entity.Property(e => e.MaTtrangBt)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ttrang_bt");
            entity.Property(e => e.MasoVat)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("maso_vat");
            entity.Property(e => e.MauSovat)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("mau_sovat");
            entity.Property(e => e.MtnGtbh)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("mtn_gtbh");
            entity.Property(e => e.MtnGtbhVnd)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("mtn_gtbh_vnd");
            entity.Property(e => e.MtnRetenNte)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("mtn_reten_nte");
            entity.Property(e => e.MtnRetenVnd)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("mtn_reten_vnd");
            entity.Property(e => e.MucVatp).HasColumnName("muc_vatp");
            entity.Property(e => e.MucVatu).HasColumnName("muc_vatu");
            entity.Property(e => e.MucktHoi)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("muckt_hoi");
            entity.Property(e => e.NamNvu)
                .HasMaxLength(4)
                .HasDefaultValueSql("('')")
                .HasColumnName("nam_nvu");
            entity.Property(e => e.NgayHdvat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_hdvat");
            entity.Property(e => e.NgayHtoanBt)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_htoan_bt");
            entity.Property(e => e.NgayTamung)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_tamung");
            entity.Property(e => e.NguyenTeYcbt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_te_ycbt");
            entity.Property(e => e.NguyenTebtGoc)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tebt_goc");
            entity.Property(e => e.NguyenTekt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tekt");
            entity.Property(e => e.NguyenTep)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tep");
            entity.Property(e => e.NguyenTepu)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tepu");
            entity.Property(e => e.NguyenTevp)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tevp");
            entity.Property(e => e.NguyenTevu)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tevu");
            entity.Property(e => e.PrKeyBthCt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_bth_ct");
            entity.Property(e => e.PrKeyBttCt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_btt_ct");
            entity.Property(e => e.PrKeyCare)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_care");
            entity.Property(e => e.PrKeyCareNoitru)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_care_noitru");
            entity.Property(e => e.PrKeyKbttHsbtCt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_kbtt_hsbt_ct");
            entity.Property(e => e.PrKeyNvuBhtCt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_nvu_bht_ct");
            entity.Property(e => e.SerieVat)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("serie_vat");
            entity.Property(e => e.SoHdvat)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_hdvat");
            entity.Property(e => e.SoTienTc)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tien_tc");
            entity.Property(e => e.SoTienYcbt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tien_ycbt");
            entity.Property(e => e.SoTienbtGoc)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienbt_goc");
            entity.Property(e => e.SoTienkt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienkt");
            entity.Property(e => e.SoTienp)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienp");
            entity.Property(e => e.SoTienpu)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienpu");
            entity.Property(e => e.SoTienvp)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienvp");
            entity.Property(e => e.SoTienvu)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienvu");
            entity.Property(e => e.TenHhoavat)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_hhoavat");
            entity.Property(e => e.TenKhvat)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_khvat");
            entity.Property(e => e.TinhTay).HasColumnName("tinh_tay");
            entity.Property(e => e.TygiaBt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tygia_bt");
            entity.Property(e => e.TyleReten)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("tyle_reten");
        });

        modelBuilder.Entity<HsbtGd>(entity =>
        {
            entity.HasKey(e => e.PrKey).IsClustered(false);

            entity.ToTable("Hsbt_gd", tb =>
            {
                tb.HasTrigger("Rep_Td_Hsbt_gd");
                tb.HasTrigger("Rep_Ti_Hsbt_gd");
                tb.HasTrigger("Rep_Tu_Hsbt_gd");
            });

            entity.HasIndex(e => e.NgayHtoanGd, "IX_Hsbt_gd_ngay_htoan_gd");

            entity.HasIndex(e => e.FrKey, "hsbt_gd_fr_key_IND");

            entity.Property(e => e.PrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.AddnewEdit).HasColumnName("addnew_edit");
            entity.Property(e => e.FrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("fr_key");
            entity.Property(e => e.GhiChuGd)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chu_gd");
            entity.Property(e => e.LoaiPhiPi)
                .HasMaxLength(2)
                .HasDefaultValueSql("('')")
                .HasColumnName("loai_phi_pi");
            entity.Property(e => e.MaDvgd)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_dvgd");
            entity.Property(e => e.MaIcd)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_icd");
            entity.Property(e => e.MaKhvat)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_khvat");
            entity.Property(e => e.MaLoaiChiphi)
                .HasMaxLength(8)
                .HasDefaultValueSql("(N'GD')")
                .HasColumnName("ma_loai_chiphi");
            entity.Property(e => e.MaSp)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_sp");
            entity.Property(e => e.MaTtegd)
                .HasMaxLength(3)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ttegd");
            entity.Property(e => e.MaTtrangGd)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ttrang_gd");
            entity.Property(e => e.MasoVat)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("maso_vat");
            entity.Property(e => e.MauSovat)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("mau_sovat");
            entity.Property(e => e.MtnRetenNte)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("mtn_reten_nte");
            entity.Property(e => e.MtnRetenVnd)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("mtn_reten_vnd");
            entity.Property(e => e.MucVat)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("muc_vat");
            entity.Property(e => e.MucVatgdu)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("muc_vatgdu");
            entity.Property(e => e.NamNvu)
                .HasMaxLength(4)
                .HasDefaultValueSql("('')")
                .HasColumnName("nam_nvu");
            entity.Property(e => e.NgayHdvat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_hdvat");
            entity.Property(e => e.NgayHtoanGd)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_htoan_gd");
            entity.Property(e => e.NguyenTegd)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tegd");
            entity.Property(e => e.NguyenTegdPvi)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tegd_pvi");
            entity.Property(e => e.NguyenTegdu)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tegdu");
            entity.Property(e => e.NguyenTev)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tev");
            entity.Property(e => e.NguyenTevu)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tevu");
            entity.Property(e => e.PrKeyBthCt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_bth_ct");
            entity.Property(e => e.PrKeyBttCt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_btt_ct");
            entity.Property(e => e.PrKeyKbttHsbtGd)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_kbtt_hsbt_gd");
            entity.Property(e => e.PrKeyNvuBhtCt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_nvu_bht_ct");
            entity.Property(e => e.SerieVat)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("serie_vat");
            entity.Property(e => e.SoHdvat)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_hdvat");
            entity.Property(e => e.SoTiengd)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tiengd");
            entity.Property(e => e.SoTiengdPvi)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tiengd_pvi");
            entity.Property(e => e.SoTiengdu)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tiengdu");
            entity.Property(e => e.SoTienv)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienv");
            entity.Property(e => e.SoTienvu)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienvu");
            entity.Property(e => e.TenHhoavat)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_hhoavat");
            entity.Property(e => e.TenKhvat)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_khvat");
            entity.Property(e => e.TinhTay).HasColumnName("tinh_tay");
            entity.Property(e => e.TygiaGd)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tygia_gd");
            entity.Property(e => e.TyleReten)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("tyle_reten");
        });

        modelBuilder.Entity<HsbtTht>(entity =>
        {
            entity.HasKey(e => e.PrKey).HasName("PK_Hsbt_tsth");

            entity.ToTable("hsbt_thts", tb =>
            {
                tb.HasTrigger("Rep_Td_hsbt_thts");
                tb.HasTrigger("Rep_Ti_hsbt_thts");
                tb.HasTrigger("Rep_Tu_hsbt_thts");
            });

            entity.HasIndex(e => e.FrKey, "hsbt_thts_fr_key_IND");

            entity.Property(e => e.PrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.FrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("fr_key");
            entity.Property(e => e.FrKeyBk)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("fr_key_bk");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chu");
            entity.Property(e => e.LoaiHinhtd)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("loai_hinhtd");
            entity.Property(e => e.LoaiPhiPi)
                .HasMaxLength(2)
                .HasDefaultValueSql("('')")
                .HasColumnName("loai_phi_pi");
            entity.Property(e => e.MaQuyenloiThts)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_quyenloi_thts");
            entity.Property(e => e.MaSp)
                .HasMaxLength(8)
                .HasColumnName("ma_sp");
            entity.Property(e => e.MaTte)
                .HasMaxLength(3)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_tte");
            entity.Property(e => e.MaTtrangTd)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ttrang_td");
            entity.Property(e => e.MtnRetenNte)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("mtn_reten_nte");
            entity.Property(e => e.MtnRetenVnd)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("mtn_reten_vnd");
            entity.Property(e => e.NgayHtoanDtd)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_htoan_dtd");
            entity.Property(e => e.NgayHtoanTd)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_htoan_td");
            entity.Property(e => e.NguyenTePvi)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_te_pvi");
            entity.Property(e => e.NguyenTeTd)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_te_td");
            entity.Property(e => e.NguyenTedtd)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tedtd");
            entity.Property(e => e.NguyenTetdPvi)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tetd_pvi");
            entity.Property(e => e.NguyenTetdu)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tetdu");
            entity.Property(e => e.PrKeyBthCt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_bth_ct");
            entity.Property(e => e.PrKeyBttCt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_btt_ct");
            entity.Property(e => e.PrKeyKbttHsbtThts)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_kbtt_hsbt_thts");
            entity.Property(e => e.PrKeyNvuBhtCt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_nvu_bht_ct");
            entity.Property(e => e.SoTienPvi)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tien_pvi");
            entity.Property(e => e.SoTienTd)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tien_td");
            entity.Property(e => e.SoTiendtd)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tiendtd");
            entity.Property(e => e.SoTientdPvi)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tientd_pvi");
            entity.Property(e => e.SoTientdu)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tientdu");
            entity.Property(e => e.TinhTay).HasColumnName("tinh_tay");
            entity.Property(e => e.TygiaTd)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tygia_td");
            entity.Property(e => e.TyleReten)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("tyle_reten");
        });

        modelBuilder.Entity<HsbtUoc>(entity =>
        {
            entity.HasKey(e => e.PrKey)
                .HasName("PK_hsbt_uoc_new")
                .IsClustered(false);

            entity.ToTable("hsbt_uoc", tb =>
            {
                tb.HasTrigger("Rep_Td_hsbt_uoc");
                tb.HasTrigger("Rep_Ti_hsbt_uoc");
                tb.HasTrigger("Rep_Tu_hsbt_uoc");
            });

            entity.HasIndex(e => e.FrKey, "hsbt_uoc_new_fr_key_IND");

            entity.Property(e => e.PrKey)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.FrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("fr_key");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chu");
            entity.Property(e => e.LoaiPhiUpi)
                .HasMaxLength(2)
                .HasDefaultValueSql("('')")
                .HasColumnName("loai_phi_upi");
            entity.Property(e => e.MaTtrangUoc)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ttrang_uoc");
            entity.Property(e => e.MaUser)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.MucVat)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("muc_vat");
            entity.Property(e => e.NgayPs)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_ps");
            entity.Property(e => e.NguyenTebt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tebt");
            entity.Property(e => e.NguyenTebtPvi)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tebt_pvi");
            entity.Property(e => e.NguyenTebtReten)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tebt_reten");
            entity.Property(e => e.NguyenTev)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tev");
            entity.Property(e => e.SoTienbt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienbt");
            entity.Property(e => e.SoTienbtPvi)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienbt_pvi");
            entity.Property(e => e.SoTienbtReten)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienbt_reten");
            entity.Property(e => e.SoTienv)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienv");
            entity.Property(e => e.TyleReten)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("tyle_reten");
        });

        modelBuilder.Entity<HsbtUocGd>(entity =>
        {
            entity.HasKey(e => e.PrKey).IsClustered(false);

            entity.ToTable("hsbt_uoc_gd", tb =>
            {
                tb.HasTrigger("Rep_Td_hsbt_uoc_gd");
                tb.HasTrigger("Rep_Ti_hsbt_uoc_gd");
                tb.HasTrigger("Rep_Tu_hsbt_uoc_gd");
            });

            entity.HasIndex(e => e.NgayPs, "IX_hsbt_uoc_gd_ngay_ps");

            entity.Property(e => e.PrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.FrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("fr_key");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(500)
                .HasColumnName("ghi_chu");
            entity.Property(e => e.LoaiPhiUpi)
                .HasMaxLength(2)
                .HasDefaultValueSql("('')")
                .HasColumnName("loai_phi_upi");
            entity.Property(e => e.MaTtrangUoc)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ttrang_uoc");
            entity.Property(e => e.MaUser)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.MucVat)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("muc_vat");
            entity.Property(e => e.NgayPs)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_ps");
            entity.Property(e => e.NguyenTegd)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tegd");
            entity.Property(e => e.NguyenTegdPvi)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tegd_pvi");
            entity.Property(e => e.NguyenTegdReten)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tegd_reten");
            entity.Property(e => e.NguyenTev)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tev");
            entity.Property(e => e.SoTiengd)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tiengd");
            entity.Property(e => e.SoTiengdPvi)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tiengd_pvi");
            entity.Property(e => e.SoTiengdReten)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tiengd_reten");
            entity.Property(e => e.SoTienv)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienv");
            entity.Property(e => e.TyleReten)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("tyle_reten");
        });
        modelBuilder.Entity<DmCtukt>(entity =>
        {
            entity.HasKey(e => new { e.MaCtukt, e.MaDonvi })
                .HasName("PK_Dm_ctukt")
                .IsClustered(false);

            entity.ToTable("dm_ctukt", tb => tb.HasTrigger("Insert_dm_ctukt"));

            entity.HasIndex(e => e.MaDviInt, "Ma_dvi_int").IsClustered();

            entity.Property(e => e.MaCtukt)
                .HasMaxLength(4)
                .HasColumnName("ma_ctukt");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi");
            entity.Property(e => e.MaDviInt).HasColumnName("ma_dvi_int");
            entity.Property(e => e.MaNhang)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_nhang");
            entity.Property(e => e.MaNhctukt)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_nhctukt");
            entity.Property(e => e.MaUser)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.NgayCnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cnhat");
            entity.Property(e => e.NgoaiTe).HasColumnName("ngoai_te");
            entity.Property(e => e.Num)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("num");
            entity.Property(e => e.NumDt)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("num_dt");
            entity.Property(e => e.Stt)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("stt");
            entity.Property(e => e.TenCtukt)
                .HasMaxLength(100)
                .HasColumnName("ten_ctukt");
            entity.Property(e => e.TenCtuktIn)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_ctukt_in");
            entity.Property(e => e.TkCo)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("tk_co");
            entity.Property(e => e.TkNo)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("tk_no");
            entity.Property(e => e.TkTrung)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("tk_trung");
        });
        modelBuilder.Entity<FileAttachBt>(entity =>
        {
            entity.HasKey(e => e.PrKey).HasName("PK_taix_file_attach_bt");

            entity.ToTable("file_attach_bt", tb =>
            {
                tb.HasTrigger("Rep_Td_file_attach_bt");
                tb.HasTrigger("Rep_Ti_file_attach_bt");
                tb.HasTrigger("Rep_Tu_file_attach_bt");
            });

            entity.HasIndex(e => e.FrKey, "Fr_key_idx");

            entity.Property(e => e.PrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.Directory)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("directory");
            entity.Property(e => e.FileName)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("file_name");
            entity.Property(e => e.FrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("fr_key");
            entity.Property(e => e.KyHieu)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("ky_hieu");
            entity.Property(e => e.MaCtu)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ctu");
            entity.Property(e => e.NgayCtu)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_ctu");
            entity.Property(e => e.TrichYeu)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("trich_yeu");
        });
        modelBuilder.Entity<DmPhe>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("dm_phe", tb =>
            {
                tb.HasTrigger("Rep_Td_dm_phe");
                tb.HasTrigger("Rep_Ti_dm_phe");
                tb.HasTrigger("Rep_Tu_dm_phe");
            });

            entity.Property(e => e.PrKey)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.MaPh)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ph");
            entity.Property(e => e.RequiredVersion)
                .HasColumnType("datetime")
                .HasColumnName("required_version");
            entity.Property(e => e.TenPh)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_ph");
        });

        modelBuilder.Entity<DmPheCt>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("dm_phe_ct", tb =>
            {
                tb.HasTrigger("Rep_Td_dm_phe_ct");
                tb.HasTrigger("Rep_Ti_dm_phe_ct");
                tb.HasTrigger("Rep_Tu_dm_phe_ct");
            });

            entity.HasIndex(e => e.FrKey, "Fr_key_idx");

            entity.Property(e => e.PrKey)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.DenNgay)
                .HasColumnType("smalldatetime")
                .HasColumnName("den_ngay");
            entity.Property(e => e.FrKey)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("fr_key");
            entity.Property(e => e.KhoaSo).HasColumnName("khoa_so");
            entity.Property(e => e.KhoaSoFull)
                .HasComment("Neu co gia tri bang 1 thi chi co admin moi co quyen mo khoa")
                .HasColumnName("khoa_so_full");
            entity.Property(e => e.KyKhoa)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("ky_khoa");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi");
            entity.Property(e => e.TuNgay)
                .HasColumnType("smalldatetime")
                .HasColumnName("tu_ngay");
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
