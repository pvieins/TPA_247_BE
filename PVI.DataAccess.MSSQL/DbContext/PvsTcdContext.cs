using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace PVI.DAO.Entities.Models;

public partial class PvsTcdContext : DbContext
{
    public PvsTcdContext()
    {
    }

    public PvsTcdContext(DbContextOptions<PvsTcdContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TcdBhtCtu> TcdBhtCtus { get; set; }

    public virtual DbSet<TcdBhtSeri> TcdBhtSeris { get; set; }

    public string connectPvsTcd = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["PVSTCDContext"]!;


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {

            optionsBuilder.UseSqlServer(connectPvsTcd);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TcdBhtCtu>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("tcd_bht_ctu");

            entity.HasIndex(e => new { e.SoDonbh, e.SoDonbhBs }, "tcd_bht_ctu_so_donbh_IND");

            entity.HasIndex(e => e.SoDonbhBs, "tcd_bht_ctu_so_donbh_bs_IND");

            entity.Property(e => e.PrKey)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.BchaoPhi)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("bchao_phi");
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
            entity.Property(e => e.ChiTietHd)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("chi_tiet_hd");
            entity.Property(e => e.ChkTpc).HasColumnName("chk_tpc");
            entity.Property(e => e.ChuyenTai).HasColumnName("chuyen_tai");
            entity.Property(e => e.CtybhCu)
                .HasMaxLength(3000)
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
            entity.Property(e => e.DiaChiVat)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("dia_chi_vat");
            entity.Property(e => e.DiachiNguoiMuabh)
                .HasMaxLength(1000)
                .HasDefaultValueSql("('')")
                .HasColumnName("diachi_nguoi_muabh");
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
            entity.Property(e => e.DieukienBb)
                .HasDefaultValueSql("('')")
                .HasComment("điều kiện bắt buộc (Hàng hải trọn gói năng lượng)")
                .HasColumnType("ntext")
                .HasColumnName("dieukien_bb");
            entity.Property(e => e.DinhMucTl)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("dinh_muc_tl");
            entity.Property(e => e.DkBosung)
                .HasDefaultValueSql("('')")
                .HasComment("dieu khoan bo sung-phan he nghiep vu tai san")
                .HasColumnType("ntext")
                .HasColumnName("dk_bosung");
            entity.Property(e => e.DkMuckt)
                .HasDefaultValueSql("('')")
                .HasColumnType("ntext")
                .HasColumnName("dk_muckt");
            entity.Property(e => e.DkhoanTtoan)
                .HasDefaultValueSql("('')")
                .HasComment("Thong tin dieu khoan thanh toan tren GCN")
                .HasColumnType("ntext")
                .HasColumnName("dkhoan_ttoan");
            entity.Property(e => e.DoituongBhTcd)
                .HasMaxLength(1500)
                .HasDefaultValueSql("('')")
                .HasColumnName("doituong_bh_tcd");
            entity.Property(e => e.FileGyc)
                .HasMaxLength(2000)
                .HasColumnName("file_GYC");
            entity.Property(e => e.FileHopdong)
                .HasMaxLength(2000)
                .HasDefaultValueSql("('')")
                .HasColumnName("file_hopdong");
            entity.Property(e => e.FileName)
                .HasMaxLength(1500)
                .HasDefaultValueSql("('')")
                .HasColumnName("file_name");
            entity.Property(e => e.FileNth)
                .HasMaxLength(2000)
                .HasDefaultValueSql("('')")
                .HasColumnName("file_NTH");
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
            entity.Property(e => e.GuiSms).HasColumnName("gui_sms");
            entity.Property(e => e.GuiViber).HasColumnName("gui_viber");
            entity.Property(e => e.GuiZalo).HasColumnName("gui_zalo");
            entity.Property(e => e.HdonDientu).HasColumnName("hdon_dientu");
            entity.Property(e => e.IdControl)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("id_control");
            entity.Property(e => e.KhoanGtru)
                .HasDefaultValueSql("('')")
                .HasComment("Các khoản giảm trừ (Hàng hải trọn gói năng lượng)")
                .HasColumnType("ntext")
                .HasColumnName("khoan_gtru");
            entity.Property(e => e.LdonKhachhang).HasColumnName("ldon_khachhang");
            entity.Property(e => e.LoaiHinhbh)
                .HasDefaultValueSql("('')")
                .HasComment("loai hinh bao hiem don xdld ngoai khoi")
                .HasColumnType("ntext")
                .HasColumnName("loai_hinhbh");
            entity.Property(e => e.LoaiRuiro)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')")
                .HasColumnName("loai_ruiro");
            entity.Property(e => e.LoaiTtoan).HasColumnName("loai_ttoan");
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
            entity.Property(e => e.MaDoitac)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_doitac");
            entity.Property(e => e.MaDonbh)
                .HasMaxLength(4)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donbh");
            entity.Property(e => e.MaDonvi)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvi");
            entity.Property(e => e.MaDonvint)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_donvint");
            entity.Property(e => e.MaGdichDoitac)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_gdich_doitac");
            entity.Property(e => e.MaGdtt)
                .HasMaxLength(1000)
                .HasDefaultValueSql("('')")
                .HasComment("Ma giam dinh ton that - hang hoa")
                .HasColumnName("ma_gdtt");
            entity.Property(e => e.MaGqbt)
                .HasMaxLength(1000)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_gqbt");
            entity.Property(e => e.MaGthieu)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_gthieu");
            entity.Property(e => e.MaHdong)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasComment("Ma hop dong di kem don - Cac phan he")
                .HasColumnName("ma_hdong");
            entity.Property(e => e.MaHoi)
                .HasMaxLength(8)
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
            entity.Property(e => e.MaLoaits)
                .HasMaxLength(18)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_loaits");
            entity.Property(e => e.MaMdsd)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_mdsd");
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
            entity.Property(e => e.MaSdbsPias)
                .HasMaxLength(10)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_sdbs_pias");
            entity.Property(e => e.MaSovat)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_sovat");
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
            entity.Property(e => e.MauHd)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("mau_hd");
            entity.Property(e => e.MienThue).HasColumnName("mien_thue");
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
            entity.Property(e => e.NguonTao)
                .HasMaxLength(50)
                .HasDefaultValueSql("('WEB')")
                .HasColumnName("nguon_tao");
            entity.Property(e => e.NhomSp).HasColumnName("nhom_sp");
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
            entity.Property(e => e.PhiBh)
                .HasDefaultValueSql("(N'0')")
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
            entity.Property(e => e.RequestId).HasColumnType("numeric(18, 0)");
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
            entity.Property(e => e.SoAc)
                .HasMaxLength(500)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_ac");
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
            entity.Property(e => e.SoCuocQtac).HasColumnName("so_cuoc_qtac");
            entity.Property(e => e.SoDonPias)
                .HasMaxLength(30)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_don_pias");
            entity.Property(e => e.SoDonbh)
                .HasMaxLength(30)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_donbh");
            entity.Property(e => e.SoDonbhBs)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_donbh_bs");
            entity.Property(e => e.SoDonbhNt)
                .HasMaxLength(30)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_donbh_nt");
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
            entity.Property(e => e.SoHdongVvon)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_hdong_vvon");
            entity.Property(e => e.SoHokhau)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_hokhau");
            entity.Property(e => e.SoNgayQtac).HasColumnName("so_ngay_qtac");
            entity.Property(e => e.SoNgtg)
                .HasComment("dung cho nghiep vu con nguoi TNC")
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("so_ngtg");
            entity.Property(e => e.SoSeri)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("so_seri");
            entity.Property(e => e.SoTienbhQtac)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("so_tienbh_qtac");
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
            entity.Property(e => e.SoVubt).HasColumnName("so_vubt");
            entity.Property(e => e.SoYcauSdbs)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("so_ycau_sdbs");
            entity.Property(e => e.SotienBhTnanKhac)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("sotien_bh_tnan_khac");
            entity.Property(e => e.SurveyRate)
                .HasComment("Bao hiem nang luong")
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("survey_rate");
            entity.Property(e => e.TamTinh)
                .HasComment("Dung cho viec tam tinh hhoa dua sang KT")
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("tam_tinh");
            entity.Property(e => e.TenHhoa)
                .HasDefaultValueSql("('')")
                .HasComment("Ten hang hoa - phan he hang hoa")
                .HasColumnType("ntext")
                .HasColumnName("ten_hhoa");
            entity.Property(e => e.TenKhVat)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasColumnName("ten_kh_vat");
            entity.Property(e => e.TenPtvc)
                .HasMaxLength(250)
                .HasDefaultValueSql("('')")
                .HasComment("Phuong tien van chuyen hhoa")
                .HasColumnName("ten_ptvc");
            entity.Property(e => e.TennguoiMuabh)
                .HasMaxLength(2500)
                .HasDefaultValueSql("('')")
                .HasColumnName("tennguoi_muabh");
            entity.Property(e => e.ThangBh)
                .HasComment("So thang bao hanh ctrinh - Phan bh ky thuat")
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("thang_bh");
            entity.Property(e => e.ThoihanBh).HasColumnName("thoihan_bh");
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
            entity.Property(e => e.TimeRetry)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
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
            entity.Property(e => e.TthaiBthuong).HasColumnName("tthai_bthuong");
            entity.Property(e => e.TthaiQuery).HasColumnName("tthai_query");
            entity.Property(e => e.TthaiTbao)
                .HasDefaultValueSql("((1))")
                .HasColumnName("tthai_tbao");
            entity.Property(e => e.TthaiTtoan)
                .HasDefaultValueSql("((1))")
                .HasColumnName("tthai_ttoan");
            entity.Property(e => e.TtinHdDientu)
                .HasMaxLength(1500)
                .HasDefaultValueSql("('')")
                .HasColumnName("ttin_hd_dientu");
            entity.Property(e => e.TygiaHt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tygia_ht");
            entity.Property(e => e.TygiaTt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tygia_tt");
            entity.Property(e => e.TyleBthuong)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("tyle_bthuong");
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
                .HasColumnType("numeric(18, 4)")
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
            entity.Property(e => e.WindFire)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasComment("chỉ số rủi ro gió lửa: nghiệp vụ tài sản")
                .HasColumnName("wind_fire");
            entity.Property(e => e.XulyNvu)
                .HasDefaultValueSql("((1))")
                .HasColumnName("xuly_nvu");
        });

        modelBuilder.Entity<TcdBhtSeri>(entity =>
        {
            entity.HasKey(e => e.PrKey);

            entity.ToTable("tcd_bht_seri");

            entity.HasIndex(e => e.FrKey, "tcd_bht_seri_fr_key_IND");

            entity.Property(e => e.PrKey)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("pr_key");
            entity.Property(e => e.BienKsoat)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("bien_ksoat");
            entity.Property(e => e.BksDd).HasColumnName("bks_dd");
            entity.Property(e => e.BlackList).HasColumnName("black_list");
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
            entity.Property(e => e.Dongchinh).HasColumnName("dongchinh");
            entity.Property(e => e.DsDkbs)
                .HasMaxLength(1000)
                .HasDefaultValueSql("((0))")
                .HasColumnName("ds_dkbs");
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
            entity.Property(e => e.HideBks)
                .IsRequired()
                .HasDefaultValueSql("((1))")
                .HasColumnName("hide_bks");
            entity.Property(e => e.HuyenKhach)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("huyen_khach");
            entity.Property(e => e.HuyenRuong)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("huyen_ruong");
            entity.Property(e => e.IsPhiBtc).HasColumnName("is_phi_btc");
            entity.Property(e => e.IsPhiPviTcd).HasColumnName("is_phi_pvi_tcd");
            entity.Property(e => e.IsThamgiaDkbs).HasColumnName("is_thamgia_dkbs");
            entity.Property(e => e.MaCtrinh)
                .HasMaxLength(25)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_ctrinh");
            entity.Property(e => e.MaDongxe)
                .HasMaxLength(18)
                .HasDefaultValueSql("('')")
                .HasColumnName("ma_dongxe");
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
            entity.Property(e => e.Moigioi).HasColumnName("moigioi");
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
            entity.Property(e => e.NgayCuoiTt)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_cuoi_tt");
            entity.Property(e => e.NgayDauSeri)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_dau_seri");
            entity.Property(e => e.NgayDky)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_dky");
            entity.Property(e => e.NgaySinh)
                .HasColumnType("smalldatetime")
                .HasColumnName("ngay_sinh");
            entity.Property(e => e.NguyenTepBtc)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nguyen_tep_btc");
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
            entity.Property(e => e.PhanboDt).HasColumnName("phanbo_dt");
            entity.Property(e => e.PhiPviTcd)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("phi_pvi_tcd");
            entity.Property(e => e.SeriEncrypt)
                .HasMaxLength(150)
                .HasColumnName("seri_encrypt");
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
            entity.Property(e => e.SoTienTh)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("so_tien_th");
            entity.Property(e => e.SoTienbhKhacVcx)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("so_tienbh_khac_vcx");
            entity.Property(e => e.SoTienbhVcx)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("so_tienbh_vcx");
            entity.Property(e => e.TaiCdDongBh).HasColumnName("tai_cd_dong_bh");
            entity.Property(e => e.TcdBienKsoat)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("tcd_bien_ksoat");
            entity.Property(e => e.TcdNamSx)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("tcd_nam_sx");
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
            entity.Property(e => e.TinhTrangTgbh)
                .HasMaxLength(100)
                .HasDefaultValueSql("('')")
                .HasColumnName("tinh_trang_tgbh");
            entity.Property(e => e.TinhTrangXe)
                .HasMaxLength(8)
                .HasDefaultValueSql("('')")
                .HasColumnName("tinh_trang_xe");
            entity.Property(e => e.TongTien)
                .HasDefaultValueSql("((0))")
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("tong_tien");
            entity.Property(e => e.TrongTai)
                .HasMaxLength(50)
                .HasDefaultValueSql("('')")
                .HasColumnName("trong_tai");
            entity.Property(e => e.TuoiAnchi).HasColumnName("tuoi_anchi");
            entity.Property(e => e.TylephiPvi)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tylephi_pvi");
            entity.Property(e => e.ViPham).HasColumnName("vi_pham");
            entity.Property(e => e.VuluaRuong)
                .HasMaxLength(200)
                .HasDefaultValueSql("('')")
                .HasColumnName("vulua_ruong");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
