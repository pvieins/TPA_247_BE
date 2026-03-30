using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace PVI.DAO.Entities.Models;

public partial class Pvs2024Context : DbContext
{
    public Pvs2024Context()
    {
    }

    public Pvs2024Context(DbContextOptions<Pvs2024Context> options)
        : base(options)
    {
    }

    public virtual DbSet<DmSp> DmSps { get; set; }
    public virtual DbSet<NvuBhtCt> NvuBhtCts { get; set; }
    public virtual DbSet<DmUserPias> DmUserPiases { get; set; }
    public virtual DbSet<DmTte> DmTtes { get; set; }
    public virtual DbSet<DmTyGia> DmTyGias { get; set; }
    public virtual DbSet<DmLoaiHinhTd> DmLoaiHinhTds { get; set; }
    public virtual DbSet<NvuBhtCtu> NvuBhtCtus { get; set; }
    public virtual DbSet<HsbtCtu> HsbtCtus { get; set; }
    public virtual DbSet<DmVar> DmVars { get; set; }
    public virtual DbSet<DmKhach> DmKhaches { get; set; }
    public virtual DbSet<NvuBhtDbh> NvuBhtDbhs { get; set; }
    public virtual DbSet<HsbtTht> HsbtThts { get; set; }
    public virtual DbSet<NvuBhtSeri> NvuBhtSeris { get; set; }
    public virtual DbSet<NvuBhtSeriCt> NvuBhtSeriCts { get; set; }
    public virtual DbSet<NvuBhtSeriDk> NvuBhtSeriDks { get; set; }
    public virtual DbSet<HsbtCt> HsbtCts { get; set; }
    public virtual DbSet<HsbtGd> HsbtGds { get; set; }
    public virtual DbSet<HsbtUoc> HsbtUocs { get; set; }
    public virtual DbSet<NvuBhtKyphi> NvuBhtKyphis { get; set; }
    public virtual DbSet<HsbtUocGd> HsbtUocGds { get; set; }
    public virtual DbSet<ReDmReten> ReDmRetens { get; set; }
    public virtual DbSet<DmTinhPIAS> DmTinhPIASes { get; set; }
    public virtual DbSet<TaixCt> TaixCts { get; set; } = null!;
    public virtual DbSet<TaixCtu> TaixCtus { get; set; } = null!;
    public virtual DbSet<DmDkbh> DmDkbhs { get; set; } = null!;
    public virtual DbSet<DmDonbh> DmDonbhs { get; set; }
    public virtual DbSet<DmPban> DmPbans { get; set; } = null!;    
    public virtual DbSet<DmNhang> DmNhangs { get; set; } = null!;
    public string connect_pias = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["PiasContext"]!;
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(connect_pias);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {


        modelBuilder.Entity<DmSp>(entity =>
        {
            entity.HasKey(e => e.MaSp);

            entity.ToTable("dm_sp", tb =>
                {
                    tb.HasTrigger("Rep_Td_dm_sp");
                    tb.HasTrigger("Rep_Ti_dm_sp");
                    tb.HasTrigger("Rep_Tu_dm_sp");
                });

            entity.Property(e => e.MaSp)
                .HasMaxLength(8)
                .HasColumnName("ma_sp");
            entity.Property(e => e.DkienTaituc).HasColumnName("dkien_taituc");
            entity.Property(e => e.KhongThue).HasColumnName("khong_thue");
            entity.Property(e => e.MaHdi)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_hdi");
            entity.Property(e => e.MaNsp)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_nsp");
            entity.Property(e => e.MaNsp1)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_nsp1");
            entity.Property(e => e.MaNsp2)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_nsp2");
            entity.Property(e => e.MaSpOld)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_sp_old");
            entity.Property(e => e.MaSpTagetik)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_sp_tagetik");
            entity.Property(e => e.MaTagetik)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_tagetik");
            entity.Property(e => e.MaUser)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.MucVat).HasColumnName("muc_vat");
            entity.Property(e => e.NgayCnhat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cnhat");
            entity.Property(e => e.NhomDieutri)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("nhom_dieutri");
            entity.Property(e => e.TenSp)
                .HasMaxLength(400)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_sp");
            entity.Property(e => e.TenSpTa)
                .HasMaxLength(400)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_sp_ta");
            entity.Property(e => e.TenTat)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_tat");
            entity.Property(e => e.TkCovathd)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("tk_covathd");
            entity.Property(e => e.TongHop).HasColumnName("tong_hop");
        });


        modelBuilder.Entity<DmTte>(entity =>
        {
            entity.HasKey(e => e.MaTte);

            entity.ToTable("dm_tte");

            entity.Property(e => e.MaTte)
                .HasMaxLength(8)
                .HasColumnName("ma_tte");
            entity.Property(e => e.TenTte).HasColumnName("ten_tte");
            entity.Property(e => e.MaTteTageTik).HasColumnName("ma_tte_tagetik");
        });


        modelBuilder.Entity<DmTyGia>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("dm_tygia");

            entity.Property(e => e.PrKey).HasColumnName("pr_key");
            entity.Property(e => e.MaTTe).HasColumnName("ma_tte");
            entity.Property(e => e.NgayHluc).HasColumnType("smalldatetime").HasColumnName("ngay_hl");

            entity.Property(e => e.Tygia).HasColumnType("numeric(18,2)").HasColumnName("ty_gia");
            entity.Property(e => e.LoaiHT).HasColumnName("loai_ht");
            entity.Property(e => e.MaUser).HasColumnName("ma_user");
            entity.Property(e => e.NgayCapNhat).HasColumnType("smalldatetime").HasColumnName("ngay_cnhat");
        });


        modelBuilder.Entity<DmUserPias>(entity =>
        {
            entity.HasKey(e => e.MaUser);

            entity.ToTable("dm_user");

            entity.Property(e => e.MaUser).HasColumnName("ma_user");
            entity.Property(e => e.TenUser).HasColumnName("ten_user");
            entity.Property(e => e.FullName).HasColumnName("full_name");
            entity.Property(e => e.DcEmail).HasColumnName("dc_email");
            entity.Property(e => e.MaDonvi).HasColumnName("ma_donvi");
            entity.Property(e => e.MaCbo).HasColumnName("ma_cbo");
            entity.Property(e => e.TrangThai).HasColumnName("trang_thai");
        });

        modelBuilder.Entity<DmTinhPIAS>(entity =>
        {
            entity.HasKey(e => e.MaTinh);

            entity.ToTable("dm_tinh");

            entity.Property(e => e.MaTinh).HasColumnName("ma_tinh");
            entity.Property(e => e.TenTinh).HasColumnName("ten_tinh");
            entity.Property(e => e.DongBang).HasColumnType("bit").HasColumnName("dong_bang");
            entity.Property(e => e.WindStorm).HasColumnName("windstorm");
            entity.Property(e => e.Flood).HasColumnName("flood");
            entity.Property(e => e.NgayCapNhat).HasColumnType("smalldatetime").HasColumnName("ngay_cnhat");
            entity.Property(e => e.MaUser).HasColumnName("ma_user");
        });

        modelBuilder.Entity<NvuBhtCt>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("nvu_bht_ct");

            entity.HasIndex(e => e.FrKey, "nvu_bht_ct_fr_key_IND");

            entity.Property(e => e.PrKey)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.DgiaDam)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("dgia_dam");
            entity.Property(e => e.DkienQtac)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasComment("muc trach nhiem doi voi nghiep vu tau song, tau ven bien.")
                .HasColumnName("dkien_qtac");
            entity.Property(e => e.FrKey)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("fr_key");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chu");
            entity.Property(e => e.KluongDam)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("kluong_dam");
            entity.Property(e => e.MaCat)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_cat");
            entity.Property(e => e.MaCthuc)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_cthuc");
            entity.Property(e => e.MaDdiembh)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ddiembh");
            entity.Property(e => e.MaDk)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasComment("Ma dieu khoan cua sp bao hiem")
                .HasColumnName("ma_dk");
            entity.Property(e => e.MaPhi)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_phi");
            entity.Property(e => e.MaSp)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_sp");
            entity.Property(e => e.MtnGtbhTai)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("mtn_gtbh_tai");
            entity.Property(e => e.MucKhtru)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("muc_khtru");
            entity.Property(e => e.MucPhi)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("muc_phi");
            entity.Property(e => e.MucVat).HasColumnName("muc_vat");
            entity.Property(e => e.MucphiTvien)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("mucphi_tvien");
            entity.Property(e => e.NguyenTep)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("nguyen_tep");
            entity.Property(e => e.NguyenTev)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("nguyen_tev");
            entity.Property(e => e.PhiCodinh)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("phi_codinh");
            entity.Property(e => e.PhiDongsau)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("phi_dongsau");
            entity.Property(e => e.PhiDongtruoc)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("phi_dongtruoc");
            entity.Property(e => e.PhiTai)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("phi_tai");
            entity.Property(e => e.PhiUoc)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("phi_uoc");
            entity.Property(e => e.PrKeyOld)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("pr_key_old");
            entity.Property(e => e.SoTienbh)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("so_tienbh");
            entity.Property(e => e.SoTienbhDon)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("so_tienbh_don");
            entity.Property(e => e.SoTienbhLke)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("so_tienbh_lke");
            entity.Property(e => e.SoTienp)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("so_tienp");
            entity.Property(e => e.SoTvien)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("so_tvien");
            entity.Property(e => e.TienHhoa)
                .HasComment("So tien hang hoa can bao hiem")
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("tien_hhoa");
            entity.Property(e => e.TienVat)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("tien_vat");
            entity.Property(e => e.TongDtich)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("tong_dtich");
            entity.Property(e => e.TrongTai)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("trong_tai");
            entity.Property(e => e.TyleCovat).HasColumnName("tyle_covat");
            entity.Property(e => e.TyleDongsau)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("tyle_dongsau");
            entity.Property(e => e.TyleDongtruoc)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("tyle_dongtruoc");
            entity.Property(e => e.TyleHhhoi)
                .HasColumnType("numeric(18, 4)")
                .HasColumnName("tyle_hhhoi");
            entity.Property(e => e.TyleHhong)
                .HasColumnType("numeric(18, 4)")
                .HasColumnName("tyle_hhong");
            entity.Property(e => e.TyleLoadp)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tyle_loadp");
            entity.Property(e => e.TylePhi)
                .HasColumnType("numeric(18, 6)")
                .HasColumnName("tyle_phi");
            entity.Property(e => e.TylePhitai)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("tyle_phitai");
            entity.Property(e => e.TylePhiuoc)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("tyle_phiuoc");
            entity.Property(e => e.TyleTor)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tyle_tor");
            entity.Property(e => e.TyleTthang)
                .HasComment("ty le phi theo thang-su dung cho tau dong moi")
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tyle_tthang");
        });


        modelBuilder.Entity<DmLoaiHinhTd>(entity =>
        {
            entity.HasKey(e => e.MaLoaiHinhTd);

            entity.ToTable("dm_loai_hinhtd");

            entity.Property(e => e.MaLoaiHinhTd).HasColumnName("ma_loai_hinhtd");
            entity.Property(e => e.TenLoaiHinhTd).HasColumnName("ten_loai_hinhtd");
        });


        modelBuilder.Entity<NvuBhtCtu>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("nvu_bht_ctu");

            entity.HasIndex(e => e.SoDonbhBs, "Nvu_bht_ctu_so_donbh_bs_IND");

            entity.HasIndex(e => e.MaCtu, "nvu_bht_ctu_ma_ctu_IND");

            entity.HasIndex(e => e.MaDonvi, "nvu_bht_ctu_ma_donvi_IND");

            entity.HasIndex(e => e.MaKh, "nvu_bht_ctu_ma_kh_IND");

            entity.HasIndex(e => e.MaPkt, "nvu_bht_ctu_ma_pkt_IND");

            entity.HasIndex(e => new { e.SoDonbh, e.SoDonbhBs }, "nvu_bht_ctu_so_donbh_IND");

            entity.HasIndex(e => e.SoDonbhSdbs, "nvu_bht_ctu_so_donbh_sdbs_IND");

            entity.Property(e => e.PrKey)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.BiCode)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasComment("Nghiệp vụ tài sản")
                .HasColumnName("bi_code");
            entity.Property(e => e.CamKetSdbs)
                .HasDefaultValueSql("('')")
                .HasComment("Thong tin de in SDBS")
                .HasColumnType("ntext")
                .HasColumnName("cam_ket_sdbs");
            entity.Property(e => e.Category)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasComment("chỉ số rủi ro t.sản: nghiệp vụ tài sản")
                .HasColumnName("category");
            entity.Property(e => e.ChkTpc).HasColumnName("chk_tpc");
            entity.Property(e => e.ChkWf).HasColumnName("chk_wf");
            entity.Property(e => e.ChuyenTai).HasColumnName("chuyen_tai");
            entity.Property(e => e.CosoNguyhiemCnbb)
                .HasMaxLength(2500)
                .HasDefaultValueSql("('')")
                .HasColumnName("coso_nguyhiem_cnbb");
            entity.Property(e => e.CtybhCu)
                .HasMaxLength(2500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ctybh_cu");
            entity.Property(e => e.DamBao)
                .HasDefaultValueSql("('')")
                .HasComment("Luu thong tin Express warranties cua don bao hiem nong nghiep")
                .HasColumnType("ntext")
                .HasColumnName("dam_bao");
            entity.Property(e => e.DdiemDen)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasComment("Dia diem den - Phan hang hoa")
                .HasColumnName("ddiem_den");
            entity.Property(e => e.DdiemDi)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasComment("Dia diem di - Phan hang hoa")
                .HasColumnName("ddiem_di");
            entity.Property(e => e.DiaChi)
                .HasMaxLength(800)
                .HasDefaultValueSql("('')")
                .HasColumnName("dia_chi");
            entity.Property(e => e.DiaChiTh)
                .HasMaxLength(800)
                .HasDefaultValueSql("('')")
                .HasColumnName("dia_chi_th");
            entity.Property(e => e.DiemLoaitru)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("diem_loaitru");
            entity.Property(e => e.DienGiai)
                .HasDefaultValueSql("('')")
                .HasComment("Dieu khoan bh- hoac dk bao hiem bo sung")
                .HasColumnType("ntext")
                .HasColumnName("dien_giai");
            entity.Property(e => e.DiengiaiThoihan)
                .HasDefaultValueSql("('')")
                .HasComment("Dien giai thoi han bao hiem, cho phep go text thoi han bao hiem trong don bao hiem nong nghiep")
                .HasColumnType("ntext")
                .HasColumnName("diengiai_thoihan");
            entity.Property(e => e.DiengiaiTonthat)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("diengiai_tonthat");
            entity.Property(e => e.DieukienBb)
                .HasDefaultValueSql("('')")
                .HasComment("điều kiện bắt buộc (Hàng hải trọn gói năng lượng)")
                .HasColumnType("ntext")
                .HasColumnName("dieukien_bb");
            entity.Property(e => e.DkBosung)
                .HasDefaultValueSql("('')")
                .HasComment("dieu khoan bo sung-phan he nghiep vu tai san")
                .HasColumnType("ntext")
                .HasColumnName("dk_bosung");
            entity.Property(e => e.DkMuckt)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("dk_muckt");
            entity.Property(e => e.DkNhakhoa)
                .HasColumnType("ntext")
                .HasColumnName("dk_nhakhoa");
            entity.Property(e => e.DkhoanTtoan)
                .HasDefaultValueSql("('')")
                .HasComment("Thong tin dieu khoan thanh toan tren GCN")
                .HasColumnType("ntext")
                .HasColumnName("dkhoan_ttoan");
            entity.Property(e => e.GhanBh)
                .HasDefaultValueSql("('')")
                .HasComment("Dieu khoan bo sung - nghiep vu")
                .HasColumnType("ntext")
                .HasColumnName("ghan_bh");
            entity.Property(e => e.GiaTrida)
                .HasComment("Gia tri du an")
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("gia_trida");
            entity.Property(e => e.GiamdinhDk)
                .HasDefaultValueSql("('')")
                .HasComment("Giám định điều kiện (Hàng hải trọn gói năng lượng)")
                .HasColumnType("ntext")
                .HasColumnName("giamdinh_dk");
            entity.Property(e => e.GiamdinhTt)
                .HasDefaultValueSql("('')")
                .HasComment("Giám định tổn thất (Hàng hải trọn gói năng lượng)")
                .HasColumnType("ntext")
                .HasColumnName("giamdinh_tt");
            entity.Property(e => e.GiatriTau)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("giatri_tau");
            entity.Property(e => e.GioKvuc)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("gio_kvuc");
            entity.Property(e => e.GioihanLanhtho)
                .HasDefaultValueSql("('')")
                .HasComment("Thong tin gioi han lanh tho tren GCN")
                .HasColumnType("ntext")
                .HasColumnName("gioihan_lanhtho");
            entity.Property(e => e.GtriDdht)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasComment("số tiền dọn dẹp hiện trường-cháy tài sản")
                .HasColumnName("gtri_ddht");
            entity.Property(e => e.GtriHhoa)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("gtri_hhoa");
            entity.Property(e => e.GtriMaymoc)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasComment("Gia tri may moc")
                .HasColumnName("gtri_maymoc");
            entity.Property(e => e.GtriTaisan)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("gtri_taisan");
            entity.Property(e => e.GtriTrangtb)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasComment("Gia tri trang thiet bi")
                .HasColumnName("gtri_trangtb");
            entity.Property(e => e.GtriTtau)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasComment("Gia tri vo tau")
                .HasColumnName("gtri_ttau");
            entity.Property(e => e.KhoanGtru)
                .HasDefaultValueSql("('')")
                .HasComment("Các khoản giảm trừ (Hàng hải trọn gói năng lượng)")
                .HasColumnType("ntext")
                .HasColumnName("khoan_gtru");
            entity.Property(e => e.LoaiHinhbh)
                .HasDefaultValueSql("('')")
                .HasComment("loai hinh bao hiem don xdld ngoai khoi")
                .HasColumnType("ntext")
                .HasColumnName("loai_hinhbh");
            entity.Property(e => e.LuatApdung)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasComment("Luat ap dung- phan he nghiep vu tai san")
                .HasColumnName("luat_apdung");
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
            entity.Property(e => e.MaDd)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasComment("Ma Dac diem doi tuong BH-Phan he Hanghoa vaTai san")
                .HasColumnName("ma_dd");
            entity.Property(e => e.MaDdiembh)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasComment("Mã địa điểm bhiểm -dùng cho TSKT và XDLD")
                .HasColumnName("ma_ddiembh");
            entity.Property(e => e.MaDonbh)
                .HasMaxLength(4)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donbh");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi");
            entity.Property(e => e.MaGdtt)
                .HasMaxLength(1000)
                .HasDefaultValueSql("('')")
                .HasComment("Ma giam dinh ton that - hang hoa")
                .HasColumnName("ma_gdtt");
            entity.Property(e => e.MaGqbt)
                .HasMaxLength(1000)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_gqbt");
            entity.Property(e => e.MaHdong)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasComment("Ma hop dong di kem don - Cac phan he")
                .HasColumnName("ma_hdong");
            entity.Property(e => e.MaHoi)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_hoi");
            entity.Property(e => e.MaKenhbh)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_kenhbh");
            entity.Property(e => e.MaKh)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_kh");
            entity.Property(e => e.MaKhTh)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasComment("Ma khach hang thu huong BH")
                .HasColumnName("ma_kh_th");
            entity.Property(e => e.MaKieutp)
                .HasMaxLength(4)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_kieutp");
            entity.Property(e => e.MaKthac)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_kthac");
            entity.Property(e => e.MaLdon)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ldon");
            entity.Property(e => e.MaLdonhang)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ldonhang");
            entity.Property(e => e.MaLoaits)
                .HasMaxLength(18)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_loaits");
            entity.Property(e => e.MaMoigioi)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_moigioi");
            entity.Property(e => e.MaNhang)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_nhang");
            entity.Property(e => e.MaNhhhoa)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_nhhhoa");
            entity.Property(e => e.MaNhkenhbh)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_nhkenhbh");
            entity.Property(e => e.MaNhloaixe)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_nhloaixe");
            entity.Property(e => e.MaNkd)
                .HasMaxLength(2000)
                .HasDefaultValueSql("('')")
                .HasComment("Ma nganh kd - phan bhkthuat")
                .HasColumnName("ma_nkd");
            entity.Property(e => e.MaNoicapd)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_noicapd");
            entity.Property(e => e.MaPkt)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_pkt");
            entity.Property(e => e.MaPthvc)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasComment("Phuong thuc van chuyen hhoa")
                .HasColumnName("ma_pthvc");
            entity.Property(e => e.MaQtbh)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_qtbh");
            entity.Property(e => e.MaSdbs)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_sdbs");
            entity.Property(e => e.MaSdbsCt)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_sdbs_ct");
            entity.Property(e => e.MaTau)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_tau");
            entity.Property(e => e.MaTinh)
                .HasMaxLength(2000)
                .HasDefaultValueSql("('')")
                .HasComment("Ma ddiem bao hiem - phan bhkthuat")
                .HasColumnName("ma_tinh");
            entity.Property(e => e.MaTte)
                .HasMaxLength(3)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_tte");
            entity.Property(e => e.MaUser)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.MauDon)
                .HasDefaultValueSql("('')")
                .HasComment("mau don xay dung lap dat ngoai khoi")
                .HasColumnType("ntext")
                .HasColumnName("mau_don");
            entity.Property(e => e.MauLogo)
                .HasDefaultValueSql("((1))")
                .HasColumnName("mau_logo");
            entity.Property(e => e.MucglRate)
                .HasComment("Muc giu lai cua PVI - Ban BHNL")
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("mucgl_rate");
            entity.Property(e => e.NgGdich)
                .HasMaxLength(2500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ng_gdich");
            entity.Property(e => e.NgGdichTh)
                .HasMaxLength(2500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ng_gdich_th");
            entity.Property(e => e.NgayCapd)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_capd");
            entity.Property(e => e.NgayCaphk)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_caphk");
            entity.Property(e => e.NgayCnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cnhat");
            entity.Property(e => e.NgayCtu)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_ctu");
            entity.Property(e => e.NgayCuoi)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cuoi");
            entity.Property(e => e.NgayCuoida)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cuoida");
            entity.Property(e => e.NgayDau)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_dau");
            entity.Property(e => e.NgayDauda)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_dauda");
            entity.Property(e => e.NgayHdong)
                .HasComment("Ngay hop dong vchh")
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_hdong");
            entity.Property(e => e.NgayHluc)
                .HasComment("Ngay hieu luc bh - phan bhiem xd lap dat")
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_hluc");
            entity.Property(e => e.NgayHoito)
                .HasComment("Ngay hoi to trong bao hiem trach nhiem")
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_hoito");
            entity.Property(e => e.NgayKhanh)
                .HasComment("")
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_khanh");
            entity.Property(e => e.NgayKy)
                .HasColumnType("datetime")
                .HasColumnName("ngay_ky");
            entity.Property(e => e.NgayTtoan)
                .HasComment("Han thanh toan")
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_ttoan");
            entity.Property(e => e.NgayVchuyen)
                .HasComment("Ngay khoi hanh hhoa")
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_vchuyen");
            entity.Property(e => e.NgayVdon)
                .HasComment("Ngay van don vchh")
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_vdon");
            entity.Property(e => e.NgayYcauSdbs)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_ycau_sdbs");
            entity.Property(e => e.NoiCaphk)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("noi_caphk");
            entity.Property(e => e.NoiCtai)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasComment("Noi chuyen tai - phan he hang hoa")
                .HasColumnName("noi_ctai");
            entity.Property(e => e.NoiDen)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasComment("Noi van chuyen hhoa den")
                .HasColumnName("noi_den");
            entity.Property(e => e.NoiDi)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasComment("Noi van chuyen hhoa di")
                .HasColumnName("noi_di");
            entity.Property(e => e.NoiDungSdbs)
                .HasDefaultValueSql("('')")
                .HasComment("Thong tin de in SDBS")
                .HasColumnType("ntext")
                .HasColumnName("noi_dung_sdbs");
            entity.Property(e => e.NvuMaKtp)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("nvu_ma_ktp");
            entity.Property(e => e.PhamviHdong)
                .HasMaxLength(2500)
                .HasDefaultValueSql("('')")
                .HasComment("Pham vi hdong cua tau- pham vi bh cua nvu khac")
                .HasColumnName("phamvi_hdong");
            entity.Property(e => e.PhamviTaiphan)
                .HasDefaultValueSql("('')")
                .HasComment("Thong tin pham vi tai phan tren GCN")
                .HasColumnType("ntext")
                .HasColumnName("phamvi_taiphan");
            entity.Property(e => e.PheDuyetdv)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("phe_duyetdv");
            entity.Property(e => e.PhiBh)
                .HasDefaultValueSql("('')")
                .HasComment("Phí bảo hiểm- chi tiết các hạng mục XDLD")
                .HasColumnType("ntext")
                .HasColumnName("phi_bh");
            entity.Property(e => e.PhiBhSdbs)
                .HasDefaultValueSql("('')")
                .HasComment("Thong tin de in SDBS")
                .HasColumnType("ntext")
                .HasColumnName("phi_bh_sdbs");
            entity.Property(e => e.PhiCnbb)
                .HasColumnType("decimal(18, 3)")
                .HasColumnName("phi_cnbb");
            entity.Property(e => e.PhiTuvan)
                .HasComment("Phi tu van")
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("phi_tuvan");
            entity.Property(e => e.PrKeyBhtt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_bhtt");
            entity.Property(e => e.ProfitRate)
                .HasComment("Bao hiem nang luong ")
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("profit_rate");
            entity.Property(e => e.QuyenLoibh)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("quyen_loibh");
            entity.Property(e => e.RctrucXdung)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("rctruc_xdung");
            entity.Property(e => e.RenewRate)
                .HasComment("Bao hiem nang luong")
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("renew_rate");
            entity.Property(e => e.RloaiDon)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("rloai_don");
            entity.Property(e => e.RnamXdung).HasColumnName("rnam_xdung");
            entity.Property(e => e.RnhomRro)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("rnhom_rro");
            entity.Property(e => e.RtangHam).HasColumnName("rtang_ham");
            entity.Property(e => e.RtangNoi).HasColumnName("rtang_noi");
            entity.Property(e => e.SoBke)
                .HasMaxLength(30)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_bke");
            entity.Property(e => e.SoChuyen).HasColumnName("so_chuyen");
            entity.Property(e => e.SoChuyengia)
                .HasComment("Luu so chuyen gia trong bao hiem trach nhiem")
                .HasColumnName("so_chuyengia");
            entity.Property(e => e.SoCtu)
                .HasMaxLength(6)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_ctu");
            entity.Property(e => e.SoDonbh)
                .HasMaxLength(30)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_donbh");
            entity.Property(e => e.SoDonbhBs)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_donbh_bs");
            entity.Property(e => e.SoDonbhSdbs)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_donbh_sdbs");
            entity.Property(e => e.SoDonbhTt)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_donbh_tt");
            entity.Property(e => e.SoHdong)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasComment("So hop dong vc hang hoa")
                .HasColumnName("so_hdong");
            entity.Property(e => e.SoHokhau)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_hokhau");
            entity.Property(e => e.SoNgtg)
                .HasComment("dung cho nghiep vu con nguoi TNC")
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_ngtg");
            entity.Property(e => e.SoSeri)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("so_seri");
            entity.Property(e => e.SoTienbhct)
                .HasDefaultValueSql("('')")
                .HasComment("Số tiền bảo hiểm- chi tiết các hạng mục XDLD")
                .HasColumnType("ntext")
                .HasColumnName("so_tienbhct");
            entity.Property(e => e.SoTroly)
                .HasComment("Luu so tro ly giup viec trong bao hiem trach nhiem")
                .HasColumnName("so_troly");
            entity.Property(e => e.SoVdon)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasComment("So van don phan he hang hoa")
                .HasColumnName("so_vdon");
            entity.Property(e => e.SoYcauSdbs)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_ycau_sdbs");
            entity.Property(e => e.SurveyRate)
                .HasComment("Bao hiem nang luong")
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("survey_rate");
            entity.Property(e => e.TamTinh)
                .HasComment("Dung cho viec tam tinh hhoa dua sang KT")
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("tam_tinh");
            entity.Property(e => e.TbhTt).HasColumnName("tbh_tt");
            entity.Property(e => e.TenGroup)
                .HasMaxLength(1000)
                .HasColumnName("ten_group");
            entity.Property(e => e.TenHhoa)
                .HasDefaultValueSql("('')")
                .HasComment("Ten hang hoa - phan he hang hoa")
                .HasColumnType("ntext")
                .HasColumnName("ten_hhoa");
            entity.Property(e => e.TenPtvc)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasComment("Phuong tien van chuyen hhoa")
                .HasColumnName("ten_ptvc");
            entity.Property(e => e.ThangBh)
                .HasComment("So thang bao hanh ctrinh - Phan bh ky thuat")
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("thang_bh");
            entity.Property(e => e.ThoihanSuco)
                .HasDefaultValueSql("('')")
                .HasComment("Luu thong tin The event period cua don nghiep vu bao hiem nong nghiep + thong tin ve nguoi dbh (Nang luong)")
                .HasColumnType("ntext")
                .HasColumnName("thoihan_suco");
            entity.Property(e => e.ThongTin)
                .HasDefaultValueSql("('')")
                .HasComment("Energy Care")
                .HasColumnType("ntext")
                .HasColumnName("thong_tin");
            entity.Property(e => e.ThongtinBosung)
                .HasDefaultValueSql("('')")
                .HasComment("Thong tin bo sung tren GCN")
                .HasColumnType("ntext")
                .HasColumnName("thongtin_bosung");
            entity.Property(e => e.ThuePtra)
                .HasDefaultValueSql("('')")
                .HasComment("Thuế và các khoản phải trả (Hàng hải trọn gói năng lượng)")
                .HasColumnType("ntext")
                .HasColumnName("thue_ptra");
            entity.Property(e => e.TimeNgayCuoi)
                .HasMaxLength(5)
                .HasDefaultValueSql("('23:59')")
                .HasColumnName("time_ngay_cuoi");
            entity.Property(e => e.TimeNgayDau)
                .HasMaxLength(5)
                .HasDefaultValueSql("('00:00')")
                .HasColumnName("time_ngay_dau");
            entity.Property(e => e.TrachNhiem)
                .HasDefaultValueSql("('')")
                .HasComment("Trách nhiệm  (Hàng hải trọn gói năng lượng)")
                .HasColumnType("ntext")
                .HasColumnName("trach_nhiem");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasComment("Trạng thái của đơn")
                .HasColumnName("trang_thai");
            entity.Property(e => e.TygiaHt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tygia_ht");
            entity.Property(e => e.TygiaTt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tygia_tt");
            entity.Property(e => e.TyleClaim)
                .HasColumnType("numeric(18, 4)")
                .HasColumnName("tyle_claim");
            entity.Property(e => e.TyleCnbb)
                .HasColumnType("decimal(9, 6)")
                .HasColumnName("tyle_cnbb");
            entity.Property(e => e.TyleDong)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tyle_dong");
            entity.Property(e => e.TyleGiamphi)
                .HasColumnType("decimal(9, 6)")
                .HasColumnName("tyle_giamphi");
            entity.Property(e => e.TyleMoigioi)
                .HasColumnType("numeric(18, 4)")
                .HasColumnName("tyle_moigioi");
            entity.Property(e => e.TylePhibh)
                .HasDefaultValueSql("('')")
                .HasComment("Tỷ lệ phí  bảo hiểm- chi tiết các hạng mục XDLD")
                .HasColumnType("ntext")
                .HasColumnName("tyle_phibh");
            entity.Property(e => e.TyleThuxep)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("tyle_thuxep");
            entity.Property(e => e.TyleTonthat)
                .HasColumnType("decimal(9, 6)")
                .HasColumnName("tyle_tonthat");
            entity.Property(e => e.VersionEdit)
                .HasColumnType("datetime")
                .HasColumnName("version_edit");
            entity.Property(e => e.WetRisk).HasColumnName("wet_risk");
            entity.Property(e => e.WindFire)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasComment("chỉ số rủi ro gió lửa: nghiệp vụ tài sản")
                .HasColumnName("wind_fire");
        });
        modelBuilder.Entity<HsbtCtu>(entity =>
        {
            entity.HasKey(e => e.PrKey)
                .HasName("PK_Hsbt_ctu_new")
                .IsClustered(false);

            entity.ToTable("Hsbt_ctu");

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
                .HasMaxLength(2)
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
                .HasDefaultValueSql("('')")
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
                .HasMaxLength(10)
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
        modelBuilder.Entity<DmVar>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("dm_vars", tb =>
            {
                tb.HasTrigger("Rep_Td_dm_vars");
                tb.HasTrigger("Rep_Ti_dm_vars");
                tb.HasTrigger("Rep_Tu_dm_vars");
            });

            entity.HasIndex(e => new { e.MaDonvi, e.Bien }, "Idx_ma_donvi");

            entity.Property(e => e.PrKey)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.Bien)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("bien");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("ghi_chu");
            entity.Property(e => e.GiaTri)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("gia_tri");
            entity.Property(e => e.GiaTriEng)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("gia_tri_eng");
            entity.Property(e => e.Khoa).HasColumnName("khoa");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi");
            entity.Property(e => e.MaUser)
                .HasMaxLength(16)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.NgayCnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cnhat");
            entity.Property(e => e.TongHop).HasColumnName("tong_hop");
        });
        modelBuilder.Entity<DmKhach>(entity =>
        {
            entity.HasKey(e => e.MaKh);

            entity.ToTable("dm_khach", tb =>
            {
                tb.HasTrigger("Rep_Td_dm_khach");
                tb.HasTrigger("Rep_Ti_dm_khach");
                tb.HasTrigger("Rep_Tu_dm_khach");
            });

            entity.HasIndex(e => e.MaDonvi, "Ma_donvi_idx");

            entity.Property(e => e.MaKh)
                .HasMaxLength(11)
                .HasColumnName("ma_kh");
            entity.Property(e => e.CanBo).HasColumnName("can_bo");
            entity.Property(e => e.DaiLy).HasColumnName("dai_ly");
            entity.Property(e => e.DiaChi)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("dia_chi");
            entity.Property(e => e.DiaChiEng)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("dia_chi_eng");
            entity.Property(e => e.DoiTru).HasColumnName("doi_tru");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("email");
            entity.Property(e => e.Fax)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("fax");
            entity.Property(e => e.Gara).HasColumnName("gara");
            entity.Property(e => e.GaraTthai)
                .IsRequired()
                .HasDefaultValueSql("((1))")
                .HasColumnName("gara_tthai");
            entity.Property(e => e.GiamDinh).HasColumnName("giam_dinh");
            entity.Property(e => e.GiamDoc)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("giam_doc");
            entity.Property(e => e.KhongSdung).HasColumnName("khong_sdung");
            entity.Property(e => e.LienquanPvi).HasColumnName("lienquan_pvi");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi");
            entity.Property(e => e.MaDonviPban)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasComment("Ma don vi phuc vu cho viec theo doi phan chia doanh thu")
                .HasColumnName("ma_donvi_pban");
            entity.Property(e => e.MaKhMoi)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_kh_moi");
            entity.Property(e => e.MaNhkh)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_nhkh");
            entity.Property(e => e.MaNkhtcty)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_nkhtcty");
            entity.Property(e => e.MaPban)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_pban");
            entity.Property(e => e.MaTctdHn)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_tctd_hn");
            entity.Property(e => e.MaTinh)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_tinh");
            entity.Property(e => e.MaUser)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.MasoVat)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("maso_vat");
            entity.Property(e => e.MoiGioiTbh).HasColumnName("moi_gioi_tbh");
            entity.Property(e => e.NganHang)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ngan_hang");
            entity.Property(e => e.NgayCap)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cap");
            entity.Property(e => e.NgayCnhat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cnhat");
            entity.Property(e => e.NgayThanhlap).HasColumnName("ngay_thanhlap");
            entity.Property(e => e.NoiCap)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("noi_cap");
            entity.Property(e => e.PathDvi)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("path_dvi");
            entity.Property(e => e.PhongBan).HasColumnName("phong_ban");
            entity.Property(e => e.SoCmnd)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_cmnd");
            entity.Property(e => e.Tel)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("tel");
            entity.Property(e => e.TenKh)
                .HasMaxLength(150)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_kh");
            entity.Property(e => e.TenKhanh)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_khanh");
            entity.Property(e => e.TenTat)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_tat");
            entity.Property(e => e.Thue).HasColumnName("thue");
            entity.Property(e => e.TkUsd)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("tk_usd");
            entity.Property(e => e.TkVnd)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("tk_vnd");
            entity.Property(e => e.ToChuc).HasColumnName("to_chuc");
            entity.Property(e => e.ToaDoGara)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("toa_do_gara");
            entity.Property(e => e.ViewAll).HasColumnName("view_all");
            entity.Property(e => e.VpKv).HasColumnName("vp_kv");
        });

        modelBuilder.Entity<NvuBhtDbh>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("nvu_bht_dbh");

            entity.HasIndex(e => e.FrKey, "Nvu_bht_dbh_fr_key_IND");

            entity.Property(e => e.PrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.FrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("fr_key");
            entity.Property(e => e.MaKhach)
                .HasMaxLength(11)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_khach");
            entity.Property(e => e.MaPatt)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_patt");
            entity.Property(e => e.MucVat).HasColumnName("muc_vat");
            entity.Property(e => e.NguyenTep)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("nguyen_tep");
            entity.Property(e => e.NguyenTev)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("nguyen_tev");
            entity.Property(e => e.SoHdong)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_hdong");
            entity.Property(e => e.SoTienp)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("so_tienp");
            entity.Property(e => e.TienVat)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("tien_vat");
            entity.Property(e => e.TyleCapdon)
                .HasColumnType("numeric(18, 4)")
                .HasColumnName("tyle_capdon");
            entity.Property(e => e.TyleHhong)
                .HasColumnType("numeric(18, 4)")
                .HasColumnName("tyle_hhong");
            entity.Property(e => e.TyleMoigioi)
                .HasColumnType("numeric(18, 4)")
                .HasColumnName("tyle_moigioi");
            entity.Property(e => e.TyleTaiho)
                .HasColumnType("numeric(18, 4)")
                .HasColumnName("tyle_taiho");
            entity.Property(e => e.TyleTg)
                .HasColumnType("numeric(18, 4)")
                .HasColumnName("tyle_tg");
            entity.Property(e => e.VaiTro)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("vai_tro");
        });
        modelBuilder.Entity<HsbtTht>(entity =>
         {
             entity.HasKey(e => e.PrKey).HasName("PK_Hsbt_tsth");

             entity.ToTable("hsbt_thts");

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
        modelBuilder.Entity<NvuBhtSeri>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("nvu_bht_seri");

            entity.HasIndex(e => e.FrKey, "Index_nvu_bht_seri_fr_key");

            entity.HasIndex(e => e.NgayDauSeri, "Index_nvu_bht_seri_ngay_dau_seri");

            entity.HasIndex(e => new { e.NhanHieu, e.NamSx, e.MaDongxe }, "NonClusteredIndex-20221228-144226");

            entity.HasIndex(e => e.BienKsoat, "nvu_bht_seri_bien_ksoat_IND");

            entity.HasIndex(e => e.FrKey, "nvu_bht_seri_fr_key_IND");

            entity.HasIndex(e => new { e.SoSeri, e.MaNlvl }, "nvu_bht_seri_so_seri_ma_nlvl_index");

            entity.Property(e => e.PrKey)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.BienKsoat)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("bien_ksoat");
            entity.Property(e => e.DcEmail)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("dc_email");
            entity.Property(e => e.DchiRuong)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("dchi_ruong");
            entity.Property(e => e.DiaChi)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("dia_chi");
            entity.Property(e => e.DiaChiTh)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("dia_chi_th");
            entity.Property(e => e.DienGiai)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("dien_giai");
            entity.Property(e => e.DienThoai)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("dien_thoai");
            entity.Property(e => e.DtichRuong)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("dtich_ruong");
            entity.Property(e => e.DungTich)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("dung_tich");
            entity.Property(e => e.FrKey)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("fr_key");
            entity.Property(e => e.GiongLua).HasColumnName("giong_lua");
            entity.Property(e => e.HuyenKhach)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("huyen_khach");
            entity.Property(e => e.HuyenRuong)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("huyen_ruong");
            entity.Property(e => e.MaCtrinh)
                .HasMaxLength(25)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ctrinh");
            entity.Property(e => e.MaDongxe)
                .HasMaxLength(18)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_dongxe");
            entity.Property(e => e.MaId)
                .HasMaxLength(50)
                .HasColumnName("ma_id");
            entity.Property(e => e.MaLoaikh)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_loaikh");
            entity.Property(e => e.MaLoaixe)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_loaixe");
            entity.Property(e => e.MaNlvl)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_nlvl");
            entity.Property(e => e.MauSon)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("mau_son");
            entity.Property(e => e.MoiQh)
                .HasMaxLength(50)
                .HasColumnName("moi_qh");
            entity.Property(e => e.MucDsd).HasColumnName("muc_dsd");
            entity.Property(e => e.NamSd)
                .HasDefaultValueSql("((0))")
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nam_sd");
            entity.Property(e => e.NamSx)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("nam_sx");
            entity.Property(e => e.NgGdichTh)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ng_gdich_th");
            entity.Property(e => e.NgayCapSeri)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cap_seri");
            entity.Property(e => e.NgayCuoiSeri)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cuoi_seri");
            entity.Property(e => e.NgayDauSeri)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_dau_seri");
            entity.Property(e => e.NgaySinh)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_sinh");
            entity.Property(e => e.NhanHieu)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("nhan_hieu");
            entity.Property(e => e.NhomKhach)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("nhom_khach");
            entity.Property(e => e.NoiDenTc)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("noi_den_tc");
            entity.Property(e => e.NoiDiTc)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("noi_di_tc");
            entity.Property(e => e.SeriSd).HasColumnName("seri_sd");
            entity.Property(e => e.SlNgbh)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("sl_ngbh");
            entity.Property(e => e.SoCmnd)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_cmnd");
            entity.Property(e => e.SoCngoi)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_cngoi");
            entity.Property(e => e.SoKhung)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_khung");
            entity.Property(e => e.SoMay)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_may");
            entity.Property(e => e.SoSeri)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("so_seri");
            entity.Property(e => e.SoThe)
                .HasMaxLength(30)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_the");
            entity.Property(e => e.TenKhach)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_khach");
            entity.Property(e => e.TinhKhach)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("tinh_khach");
            entity.Property(e => e.TinhRuong)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("tinh_ruong");
            entity.Property(e => e.TongTien)
                .HasDefaultValueSql("((0))")
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("tong_tien");
            entity.Property(e => e.TrongTai)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("trong_tai");
            entity.Property(e => e.ViPham).HasColumnName("vi_pham");
            entity.Property(e => e.VuluaRuong)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("vulua_ruong");
        });

        modelBuilder.Entity<NvuBhtSeriCt>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("nvu_bht_seri_ct");

            entity.HasIndex(e => e.FrKey, "Index_nvu_bht_seri_ct_fr_key");

            entity.HasIndex(e => e.MaSp, "Index_nvu_bht_seri_ct_ma_sp");

            entity.HasIndex(e => e.FrKey, "nvu_bht_seri_ct_fr_key_IND");

            entity.HasIndex(e => e.MaSp, "nvu_bht_seri_ct_ma_sp_IND");

            entity.Property(e => e.PrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.DgiaDam)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("dgia_dam");
            entity.Property(e => e.FrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("fr_key");
            entity.Property(e => e.GiatriTte)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("giatri_tte");
            entity.Property(e => e.KluongDam)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("kluong_dam");
            entity.Property(e => e.MaCsdg)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_csdg");
            entity.Property(e => e.MaDkbh)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_dkbh");
            entity.Property(e => e.MaMuckt)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_muckt");
            entity.Property(e => e.MaSp)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_sp");
            entity.Property(e => e.MaTtep)
                .HasMaxLength(3)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ttep");
            entity.Property(e => e.MtnGtbhNte)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("mtn_gtbh_nte");
            entity.Property(e => e.MtnGtbhTsan)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("mtn_gtbh_tsan");
            entity.Property(e => e.MtnGtbhVnd)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("mtn_gtbh_vnd");
            entity.Property(e => e.MucMienthuong)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("muc_mienthuong");
            entity.Property(e => e.MucVat).HasColumnName("muc_vat");
            entity.Property(e => e.NguyenTep)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tep");
            entity.Property(e => e.NguyenTev)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tev");
            entity.Property(e => e.SoNguoi)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("so_nguoi");
            entity.Property(e => e.SoTan)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("so_tan");
            entity.Property(e => e.SoTienp)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("so_tienp");
            entity.Property(e => e.TienVat)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tien_vat");
            entity.Property(e => e.TygiaHt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tygia_ht");
        });
        modelBuilder.Entity<NvuBhtSeriDk>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("nvu_bht_seri_dk");

            entity.HasIndex(e => e.FrKey, "nvu_bht_seri_dk_fr_key_IND");

            entity.Property(e => e.PrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.FrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("fr_key");
            entity.Property(e => e.MaDkhoanBs)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_dkhoan_bs");
            entity.Property(e => e.TenDkhoanBs)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("ten_dkhoan_bs");
        });
        modelBuilder.Entity<HsbtCt>(entity =>
        {
            entity.HasKey(e => e.PrKey)
                .HasName("PK_hsbt_ct")
                .IsClustered(false);

            entity.ToTable("Hsbt_ct");

            entity.HasIndex(e => e.NgayHtoanBt, "IX_Hsbt_ct_ngay_htoan_bt");

            entity.HasIndex(e => e.FrKey, "hsbt_ct_fr_key_IND");

            entity.HasIndex(e => e.PrKeyBthCt, "hsbt_ct_pr_key_bth_ct_IND");

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

            entity.ToTable("Hsbt_gd");

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
        modelBuilder.Entity<HsbtUoc>(entity =>
        {
            entity.HasKey(e => e.PrKey)
                .HasName("PK_hsbt_uoc_new")
                .IsClustered(false);

            entity.ToTable("hsbt_uoc");

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
        modelBuilder.Entity<NvuBhtKyphi>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("nvu_bht_kyphi");

            entity.HasIndex(e => e.FrKey, "Nvu_bht_kyphi_fr_key_IND");

            entity.Property(e => e.PrKey)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.FrKey)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("fr_key");
            entity.Property(e => e.KeToan).HasColumnName("ke_toan");
            entity.Property(e => e.NamNvu)
                .HasMaxLength(4)
                .HasDefaultValueSql("('')")
                .HasColumnName("nam_nvu");
            entity.Property(e => e.NgayHl)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_hl");
            entity.Property(e => e.PrKeyDbh)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("pr_key_dbh");
            entity.Property(e => e.PrKeyKt)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("pr_key_kt");
            entity.Property(e => e.SoTien)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("so_tien");
            entity.Property(e => e.Stt).HasColumnName("stt");
            entity.Property(e => e.TraPhi).HasColumnName("tra_phi");
            entity.Property(e => e.TylePhithu)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("tyle_phithu");
        });
        modelBuilder.Entity<HsbtUocGd>(entity =>
        {
            entity.HasKey(e => e.PrKey).IsClustered(false);

            entity.ToTable("hsbt_uoc_gd");

            entity.HasIndex(e => e.NgayPs, "IX_hsbt_uoc_gd_ngay_ps");

            entity.HasIndex(e => e.FrKey, "fr_key_idx");

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
        modelBuilder.Entity<ReDmReten>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("RE_DM_RETEN");

            entity.Property(e => e.MaSp)
                .HasMaxLength(8)
                .HasColumnName("ma_sp");
            entity.Property(e => e.MtnRetenUsd)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("mtn_reten_usd");
            entity.Property(e => e.MtnRetenVnd)
                .HasColumnType("numeric(38, 4)")
                .HasColumnName("mtn_reten_vnd");
            entity.Property(e => e.PrKeyNvuBhtCt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_nvu_bht_ct");
            entity.Property(e => e.SoDonbh)
                .HasMaxLength(100)
                .HasColumnName("so_donbh");
            entity.Property(e => e.SoDonbhRi)
                .HasMaxLength(100)
                .HasColumnName("so_donbh_ri");
            entity.Property(e => e.SoDonbhbs)
                .HasMaxLength(100)
                .HasColumnName("so_donbhbs");
            entity.Property(e => e.TyleReten)
                .HasColumnType("numeric(38, 5)")
                .HasColumnName("tyle_reten");
        });
        OnModelCreatingPartial(modelBuilder);

        modelBuilder.Entity<TaixCt>(entity =>
        {
            entity.HasKey(e => e.PrKey)
                .HasName("PK_tai_ct");

            entity.ToTable("taix_ct");

            entity.HasIndex(e => e.FrKey, "taix_ct_fr_key_IND");

            entity.Property(e => e.PrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key");

            entity.Property(e => e.FrKey)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("fr_key");

            entity.Property(e => e.GtbhOee)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("gtbh_oee");

            entity.Property(e => e.GtbhOeesl)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("gtbh_oeesl");

            entity.Property(e => e.GtbhOther)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("gtbh_other");

            entity.Property(e => e.GtbhOthersl)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("gtbh_othersl");

            entity.Property(e => e.GtbhPd)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("gtbh_pd");

            entity.Property(e => e.GtbhPdsl)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("gtbh_pdsl");

            entity.Property(e => e.GtbhTpl)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("gtbh_tpl");

            entity.Property(e => e.GtbhTplsl)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("gtbh_tplsl");

            entity.Property(e => e.HhongNhanNte)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hhong_nhan_nte");

            entity.Property(e => e.Layer)
                .HasMaxLength(8)
                .HasColumnName("layer")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaCat)
                .HasMaxLength(8)
                .HasColumnName("ma_cat")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaDdiembhCt)
                .HasMaxLength(8)
                .HasColumnName("ma_ddiembh_ct")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaDk)
                .HasMaxLength(8)
                .HasColumnName("ma_dk")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaSp)
                .HasMaxLength(8)
                .HasColumnName("ma_sp")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaTteGoc)
                .HasMaxLength(3)
                .HasColumnName("ma_tte_goc")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MtnGiulai)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("mtn_giulai");

            entity.Property(e => e.MtnGtbhHull)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("mtn_gtbh_hull");

            entity.Property(e => e.MtnGtbhIv)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("mtn_gtbh_iv");

            entity.Property(e => e.MtnGtbhNte)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("mtn_gtbh_nte");

            entity.Property(e => e.MtnGtbhOther)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("mtn_gtbh_other");

            entity.Property(e => e.MtnGtbhTai)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("mtn_gtbh_tai");

            entity.Property(e => e.MtnGtbhUsd)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("mtn_gtbh_usd");

            entity.Property(e => e.MtnGtbhWr)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("mtn_gtbh_wr");

            entity.Property(e => e.MtnNhanNte)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("mtn_nhan_nte");

            entity.Property(e => e.MtnReten)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("mtn_reten");

            entity.Property(e => e.MtnRetenNte)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("mtn_reten_nte");

            entity.Property(e => e.MtnRetenUsd)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("mtn_reten_usd");

            entity.Property(e => e.MtnTorUsd)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("mtn_tor_usd");

            entity.Property(e => e.MucKhautru)
                .HasMaxLength(500)
                .HasColumnName("muc_khautru")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MucKhtruhoi)
                .HasMaxLength(500)
                .HasColumnName("muc_khtruhoi")
                .HasDefaultValueSql("((0))");

            entity.Property(e => e.NgayTinhTaicd)
                .HasColumnType("datetime")
                .HasColumnName("ngay_tinh_taicd");

            entity.Property(e => e.NguyenTepHull)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tep_hull");

            entity.Property(e => e.NguyenTepIv)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tep_iv");

            entity.Property(e => e.NguyenTepNhan)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tep_nhan");

            entity.Property(e => e.NguyenTepOther)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tep_other");

            entity.Property(e => e.NguyenTepReten)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tep_reten");

            entity.Property(e => e.NguyenTepWr)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tep_wr");

            entity.Property(e => e.NguyenTepi)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tepi");

            entity.Property(e => e.NguyenTeps)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_teps");

            entity.Property(e => e.NguyenTeth)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_teth");

            entity.Property(e => e.PerLimitReten)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("per_limit_reten");

            entity.Property(e => e.PhiCodinh)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("phi_codinh");

            entity.Property(e => e.PhiDongsau)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("phi_dongsau");

            entity.Property(e => e.PhiDongtruoc)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("phi_dongtruoc");

            entity.Property(e => e.PhiMoigioi)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("phi_moigioi");

            entity.Property(e => e.PhiTai)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("phi_tai");

            entity.Property(e => e.PhiUoc)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("phi_uoc");

            entity.Property(e => e.PrKeyNvuBhtCt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_nvu_bht_ct");

            entity.Property(e => e.PrKeyTainCt)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_tain_ct");

            entity.Property(e => e.PviFee)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pvi_fee");

            entity.Property(e => e.PviFeeNte)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("pvi_fee_nte");

            entity.Property(e => e.SoTienps)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienps");

            entity.Property(e => e.SoTienth)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_tienth");

            entity.Property(e => e.TenRuiro)
                .HasMaxLength(500)
                .HasColumnName("ten_ruiro")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.TinhTay).HasColumnName("tinh_tay");

            entity.Property(e => e.TyleDongsau)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("tyle_dongsau");

            entity.Property(e => e.TyleDongtruoc)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("tyle_dongtruoc");

            entity.Property(e => e.TyleNhan)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("tyle_nhan");

            entity.Property(e => e.TyleNhuongPhityle)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("tyle_nhuong_phityle");

            entity.Property(e => e.TylePhi)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("tyle_phi");

            entity.Property(e => e.TylePhitai)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("tyle_phitai");

            entity.Property(e => e.TylePhiuoc)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("tyle_phiuoc");

            entity.Property(e => e.TyleReten)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("tyle_reten");

            entity.Property(e => e.TyleTai)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tyle_tai");

            entity.Property(e => e.TyleTaihoTty)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tyle_taiho_tty");

            entity.Property(e => e.TyleTor)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("tyle_tor");
        });
       
        modelBuilder.Entity<TaixCtu>(entity =>
        {
            entity.HasKey(e => e.PrKey)
                .HasName("PK_tai_ctu");

            entity.ToTable("taix_ctu");

            entity.HasIndex(e => new { e.MaDonvi, e.MaCtu }, "taix_ctu_ma_donvi_ma_ctu_IND");

            entity.HasIndex(e => e.SoHdgcn, "taix_ctu_so_donbh_IND");

            entity.HasIndex(e => e.SoDonbhSdbs, "taix_ctu_so_donbh_sdbs_IND");

            entity.HasIndex(e => e.SoDonbhbs, "taix_ctu_so_donbhbs_IND");

            entity.HasIndex(e => new { e.SoHdgcn, e.SoDonbhbs }, "taix_ctu_so_hdgcn_IND");

            entity.Property(e => e.PrKey)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("pr_key");

            entity.Property(e => e.BaoHanh)
                .HasMaxLength(50)
                .HasColumnName("bao_hanh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.BiCode)
                .HasMaxLength(8)
                .HasColumnName("bi_code")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.Category)
                .HasMaxLength(8)
                .HasColumnName("category")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ChkDdiem)
                .HasColumnName("chk_ddiem")
                .HasComment("Nhung don co cung dia diem co gia tri bang 1");

            entity.Property(e => e.CsNhamay).HasColumnName("cs_nhamay");

            entity.Property(e => e.CsTurbine).HasColumnName("cs_turbine");

            entity.Property(e => e.DiaDiembh)
                .HasMaxLength(8)
                .HasColumnName("dia_diembh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.DiaDiemden).HasColumnName("dia_diemden");

            entity.Property(e => e.DiaDiemdi).HasColumnName("dia_diemdi");

            entity.Property(e => e.DienGiai)
                .HasColumnType("ntext")
                .HasColumnName("dien_giai")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.DkienDkhoan)
                .HasColumnType("ntext")
                .HasColumnName("dkien_dkhoan")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.DtuongBh)
                .HasMaxLength(500)
                .HasColumnName("dtuong_bh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.HanBaohanh)
                .HasColumnType("smalldatetime")
                .HasColumnName("han_baohanh");

            entity.Property(e => e.KeyStatus).HasColumnName("key_status");

            entity.Property(e => e.KhongTdbt).HasColumnName("khong_tdbt");

            entity.Property(e => e.KhongTtoanPhinhuong).HasColumnName("khong_ttoan_phinhuong");

            entity.Property(e => e.KhongTtoanPhinhuongGhichu)
                .HasMaxLength(100)
                .HasColumnName("khong_ttoan_phinhuong_ghichu")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.KhongTtoanThudoibt).HasColumnName("khong_ttoan_thudoibt");

            entity.Property(e => e.KhongTtoanThudoibtGhichu)
                .HasMaxLength(100)
                .HasColumnName("khong_ttoan_thudoibt_ghichu")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.LoaiHinhbh)
                .HasMaxLength(8)
                .HasColumnName("loai_hinhbh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaCtu)
                .HasMaxLength(4)
                .HasColumnName("ma_ctu")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaCtyDbh)
                .HasMaxLength(11)
                .HasColumnName("ma_cty_dbh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaCtydong)
                .HasMaxLength(11)
                .HasColumnName("ma_ctydong")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaCtytaicd)
                .HasMaxLength(11)
                .HasColumnName("ma_ctytaicd")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaDaily)
                .HasMaxLength(11)
                .HasColumnName("ma_daily")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaDd)
                .HasMaxLength(8)
                .HasColumnName("ma_dd")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaDonbh)
                .HasMaxLength(4)
                .HasColumnName("ma_donbh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaDonvi)
                .HasMaxLength(8)
                .HasColumnName("ma_donvi")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaHangtau)
                .HasMaxLength(150)
                .HasColumnName("ma_hangtau")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaHdong)
                .HasMaxLength(250)
                .HasColumnName("ma_hdong")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaHoi)
                .HasMaxLength(11)
                .HasColumnName("ma_hoi")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaKenhbh)
                .HasMaxLength(11)
                .HasColumnName("ma_kenhbh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaKh)
                .HasMaxLength(11)
                .HasColumnName("ma_kh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaKthac)
                .HasMaxLength(8)
                .HasColumnName("ma_kthac")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaLdon).HasColumnName("ma_ldon");

            entity.Property(e => e.MaLoaixe)
                .HasMaxLength(8)
                .HasColumnName("ma_loaixe")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaLtau)
                .HasMaxLength(150)
                .HasColumnName("ma_ltau")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaNhkenhbh)
                .HasMaxLength(11)
                .HasColumnName("ma_nhkenhbh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaNhkh)
                .HasMaxLength(8)
                .HasColumnName("ma_nhkh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaNhloaixe)
                .HasMaxLength(8)
                .HasColumnName("ma_nhloaixe")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaNhruiro)
                .HasMaxLength(8)
                .HasColumnName("ma_nhruiro")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaNkd)
                .HasMaxLength(500)
                .HasColumnName("ma_nkd")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaPkt)
                .HasMaxLength(11)
                .HasColumnName("ma_pkt")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaPtvc)
                .HasMaxLength(250)
                .HasColumnName("ma_ptvc")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaSdbs)
                .HasMaxLength(8)
                .HasColumnName("ma_sdbs")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaSdbsCt)
                .HasMaxLength(8)
                .HasColumnName("ma_sdbs_ct")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaTau)
                .HasMaxLength(8)
                .HasColumnName("ma_tau")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaTinh)
                .HasMaxLength(500)
                .HasColumnName("ma_tinh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaTte)
                .HasMaxLength(3)
                .HasColumnName("ma_tte")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaUser)
                .HasMaxLength(10)
                .HasColumnName("ma_user")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MdonBh)
                .HasMaxLength(8)
                .HasColumnName("mdon_bh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MucKhautru)
                .HasMaxLength(50)
                .HasColumnName("muc_khautru")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.NamDong).HasColumnName("nam_dong");

            entity.Property(e => e.NamNoidong)
                .HasMaxLength(150)
                .HasColumnName("nam_noidong")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.NamNvu)
                .HasColumnType("smalldatetime")
                .HasColumnName("nam_nvu");

            entity.Property(e => e.NgGdich)
                .HasMaxLength(2500)
                .HasColumnName("ng_gdich")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.NgayCapd)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_capd");

            entity.Property(e => e.NgayCtu)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_ctu");

            entity.Property(e => e.NgayCuoi)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cuoi");

            entity.Property(e => e.NgayDau)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_dau");

            entity.Property(e => e.NgayKhoihanh)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_khoihanh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.NgayThuphi)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_thuphi");

            entity.Property(e => e.NoiChuyenTai)
                .HasMaxLength(150)
                .HasColumnName("noi_chuyen_tai")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.NoiDen)
                .HasMaxLength(250)
                .HasColumnName("noi_den")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.NoiDi)
                .HasMaxLength(250)
                .HasColumnName("noi_di")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.NvuMaDoitau)
                .HasMaxLength(8)
                .HasColumnName("nvu_ma_doitau")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.PrKeyGoc)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_goc");

            entity.Property(e => e.PrKeyPi)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_pi");

            entity.Property(e => e.PrKeyRetro)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_retro")
                .HasComment("luu pr_key cua don retro");

            entity.Property(e => e.PrKeyXepchuyen)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pr_key_xepchuyen");

            entity.Property(e => e.PthucVchuyen)
                .HasMaxLength(250)
                .HasColumnName("pthuc_vchuyen")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.SoCtu)
                .HasMaxLength(6)
                .HasColumnName("so_ctu")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.SoDonbhSdbs)
                .HasMaxLength(100)
                .HasColumnName("so_donbh_sdbs")
                .HasComputedColumnSql("(case when [so_donbhbs]<>'' then [so_donbhbs] else [so_hdgcn] end)", true);

            entity.Property(e => e.SoDonbhbs)
                .HasMaxLength(100)
                .HasColumnName("so_donbhbs")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.SoDonbhtt)
                .HasMaxLength(100)
                .HasColumnName("so_donbhtt")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.SoHdgcn)
                .HasMaxLength(100)
                .HasColumnName("so_hdgcn")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.SoHdong)
                .HasMaxLength(500)
                .HasColumnName("so_hdong")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.SoNgay)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_ngay");

            entity.Property(e => e.SoNgtg)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_ngtg");

            entity.Property(e => e.SoSeri)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("so_seri");

            entity.Property(e => e.TamTinh)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("tam_tinh");

            entity.Property(e => e.TenKh)
                .HasMaxLength(500)
                .HasColumnName("ten_kh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.TrangThai)
                .HasMaxLength(8)
                .HasColumnName("trang_thai")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.TrongTai)
                .HasMaxLength(150)
                .HasColumnName("trong_tai")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.TrongTaigt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("trong_taigt");

            entity.Property(e => e.TygiaHt)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("tygia_ht");

            entity.Property(e => e.TygiaTt)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("tygia_tt");

            entity.Property(e => e.TyleDong)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("tyle_dong");

            entity.Property(e => e.TyleGiamphi)
                .HasColumnType("decimal(9, 6)")
                .HasColumnName("tyle_giamphi");

            entity.Property(e => e.TyleHhongcd)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tyle_hhongcd");

            entity.Property(e => e.TyleTaicd)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tyle_taicd");

            entity.Property(e => e.TyleTaiho)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tyle_taiho");

            entity.Property(e => e.TyleTonthat)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tyle_tonthat");

            entity.Property(e => e.VanDon)
                .HasMaxLength(150)
                .HasColumnName("van_don")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.VungHdong)
                .HasMaxLength(350)
                .HasColumnName("vung_hdong")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.WetRisk).HasColumnName("wet_risk");

            entity.Property(e => e.WindFire)
                .HasMaxLength(8)
                .HasColumnName("wind_fire")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.XepChuyen).HasColumnName("xep_chuyen");
        });
        modelBuilder.Entity<DmDonbh>(entity =>
        {
            entity.HasKey(e => e.MaDonbh);

            entity.ToTable("dm_donbh", tb =>
            {
                tb.HasTrigger("Rep_Td_dm_donbh");
                tb.HasTrigger("Rep_Ti_dm_donbh");
                tb.HasTrigger("Rep_Tu_dm_donbh");
            });

            entity.Property(e => e.MaDonbh)
                .HasMaxLength(8)
                .HasColumnName("ma_donbh");
            entity.Property(e => e.AnChi)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("an_chi");
            entity.Property(e => e.MaDkhoanBs)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_dkhoan_bs");
            entity.Property(e => e.MaHieu)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_hieu");
            entity.Property(e => e.MaSp)
                .HasMaxLength(300)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_sp");
            entity.Property(e => e.MaUser)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_user");
            entity.Property(e => e.NgayCnhat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cnhat");
            entity.Property(e => e.TenDonbh)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_donbh");
            entity.Property(e => e.TenDonbhTa)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_donbh_ta");
        });
        modelBuilder.Entity<DmPban>(entity =>
        {
            entity.HasNoKey();

            entity.ToView("DM_PBAN");

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

        modelBuilder.Entity<DmDkbh>(entity =>
        {
            entity.HasKey(e => new { e.MaDkbh, e.MaQtac });

            entity.ToTable("dm_dkbh");

            entity.Property(e => e.MaDkbh)
                .HasMaxLength(8)
                .HasColumnName("ma_dkbh");

            entity.Property(e => e.MaQtac)
                .HasMaxLength(8)
                .HasColumnName("ma_qtac");

            entity.Property(e => e.Cat).HasColumnName("cat");

            entity.Property(e => e.KhongSdung).HasColumnName("khong_sdung");

            entity.Property(e => e.MaDonvi)
                .HasMaxLength(8)
                .HasColumnName("ma_donvi")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.MaUser)
                .HasMaxLength(10)
                .HasColumnName("ma_user")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.NgayCnhat)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cnhat");

            entity.Property(e => e.NgayHluc)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_hluc");

            entity.Property(e => e.TenDkbh)
                .HasMaxLength(500)
                .HasColumnName("ten_dkbh")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.TenDkbhTa)
                .HasMaxLength(500)
                .HasColumnName("ten_dkbh_ta")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.TongHop).HasColumnName("tong_hop");

            entity.Property(e => e.TyleTor)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tyle_tor");
        });

        modelBuilder.Entity<DmNhang>(entity =>
        {
            entity.HasKey(e => e.MaNhang);

            entity.ToTable("dm_nhang");

            entity.Property(e => e.MaNhang)
                .HasMaxLength(11)
                .HasColumnName("ma_nhang");

            entity.Property(e => e.TenNhang)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_nhang");

            entity.Property(e => e.SoTkNhang)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_tknhang");

            entity.Property(e => e.MaDonviNhang)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi_nganhang");

            entity.Property(e => e.MaTteNhang)
                .HasMaxLength(3)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_tte_nganhang");

            entity.Property(e => e.LoaiTaiKhoan)
                .HasMaxLength(1)
                .HasDefaultValueSql("('')")
                .HasColumnName("loai_taikhoan");

            entity.Property(e => e.TenTaiKhoan)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_taikhoan");

            entity.Property(e => e.TrangThai).HasColumnName("trang_thai");
        });

    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
