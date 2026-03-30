using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class TcdBhtCtu
{
    public decimal PrKey { get; set; }

    public string MaDonvi { get; set; } = null!;

    public string MaCtu { get; set; } = null!;

    public string SoCtu { get; set; } = null!;

    public DateTime? NgayCtu { get; set; }

    public string MaPkt { get; set; } = null!;

    public string MaSdbs { get; set; } = null!;

    public string MaDonbh { get; set; } = null!;

    public string SoDonbh { get; set; } = null!;

    public decimal SoSeri { get; set; }

    public string SoDonbhTt { get; set; } = null!;

    public string SoDonbhBs { get; set; } = null!;

    public DateTime? NgayCapd { get; set; }

    public string MaNoicapd { get; set; } = null!;

    public string MaQtbh { get; set; } = null!;

    public string TimeNgayDau { get; set; } = null!;

    public DateTime? NgayDau { get; set; }

    public string TimeNgayCuoi { get; set; } = null!;

    public DateTime? NgayCuoi { get; set; }

    public string GioKvuc { get; set; } = null!;

    public string NvuMaKtp { get; set; } = null!;

    public string MaKh { get; set; } = null!;

    public string NgGdich { get; set; } = null!;

    public string? DiaChi { get; set; }

    /// <summary>
    /// Ma khach hang thu huong BH
    /// </summary>
    public string MaKhTh { get; set; } = null!;

    public string NgGdichTh { get; set; } = null!;

    public string DiaChiTh { get; set; } = null!;

    public string MaTau { get; set; } = null!;

    /// <summary>
    /// Pham vi hdong cua tau- pham vi bh cua nvu khac
    /// </summary>
    public string PhamviHdong { get; set; } = null!;

    public int SoChuyen { get; set; }

    public DateTime? NgayKhanh { get; set; }

    public string MaKthac { get; set; } = null!;

    public string MaDaily { get; set; } = null!;

    public string MaMoigioi { get; set; } = null!;

    public decimal TyleMoigioi { get; set; }

    public string MaLdon { get; set; } = null!;

    /// <summary>
    /// Han thanh toan
    /// </summary>
    public DateTime? NgayTtoan { get; set; }

    public string MaCbkt { get; set; } = null!;

    /// <summary>
    /// Ma giam dinh ton that - hang hoa
    /// </summary>
    public string? MaGdtt { get; set; }

    public string? MaGqbt { get; set; }

    public decimal TyleClaim { get; set; }

    public string MaTte { get; set; } = null!;

    public decimal TygiaHt { get; set; }

    public decimal TygiaTt { get; set; }

    /// <summary>
    /// Dieu khoan bh- hoac dk bao hiem bo sung
    /// </summary>
    public string DienGiai { get; set; } = null!;

    public string DkMuckt { get; set; } = null!;

    /// <summary>
    /// Dieu khoan bo sung - nghiep vu
    /// </summary>
    public string GhanBh { get; set; } = null!;

    public string MaKieutp { get; set; } = null!;

    public string MaHoi { get; set; } = null!;

    public decimal TyleThuxep { get; set; }

    /// <summary>
    /// Phuong tien van chuyen hhoa
    /// </summary>
    public string TenPtvc { get; set; } = null!;

    /// <summary>
    /// Phuong thuc van chuyen hhoa
    /// </summary>
    public string MaPthvc { get; set; } = null!;

    /// <summary>
    /// Ngay khoi hanh hhoa
    /// </summary>
    public DateTime? NgayVchuyen { get; set; }

    /// <summary>
    /// Noi van chuyen hhoa di
    /// </summary>
    public string NoiDi { get; set; } = null!;

    /// <summary>
    /// Noi van chuyen hhoa den
    /// </summary>
    public string NoiDen { get; set; } = null!;

    /// <summary>
    /// Dia diem di - Phan hang hoa
    /// </summary>
    public string DdiemDi { get; set; } = null!;

    /// <summary>
    /// Dia diem den - Phan hang hoa
    /// </summary>
    public string DdiemDen { get; set; } = null!;

    public decimal GtriTaisan { get; set; }

    /// <summary>
    /// Gia tri vo tau
    /// </summary>
    public string GtriTtau { get; set; } = null!;

    /// <summary>
    /// Gia tri may moc
    /// </summary>
    public string GtriMaymoc { get; set; } = null!;

    /// <summary>
    /// Gia tri trang thiet bi
    /// </summary>
    public string GtriTrangtb { get; set; } = null!;

    public string MaUser { get; set; } = null!;

    /// <summary>
    /// So hop dong vc hang hoa
    /// </summary>
    public string SoHdong { get; set; } = null!;

    /// <summary>
    /// Ngay hop dong vchh
    /// </summary>
    public DateTime? NgayHdong { get; set; }

    /// <summary>
    /// So van don phan he hang hoa
    /// </summary>
    public string SoVdon { get; set; } = null!;

    /// <summary>
    /// Ngay van don vchh
    /// </summary>
    public DateTime? NgayVdon { get; set; }

    /// <summary>
    /// Dung cho viec tam tinh hhoa dua sang KT
    /// </summary>
    public decimal TamTinh { get; set; }

    /// <summary>
    /// Noi chuyen tai - phan he hang hoa
    /// </summary>
    public string NoiCtai { get; set; } = null!;

    /// <summary>
    /// Ten hang hoa - phan he hang hoa
    /// </summary>
    public string TenHhoa { get; set; } = null!;

    public string GtriHhoa { get; set; } = null!;

    public decimal GiatriTau { get; set; }

    public bool ChuyenTai { get; set; }

    /// <summary>
    /// Ma ddiem bao hiem - phan bhkthuat
    /// </summary>
    public string MaTinh { get; set; } = null!;

    /// <summary>
    /// Ma nganh kd - phan bhkthuat
    /// </summary>
    public string MaNkd { get; set; } = null!;

    /// <summary>
    /// Ma hop dong di kem don - Cac phan he
    /// </summary>
    public string MaHdong { get; set; } = null!;

    /// <summary>
    /// Ngay hieu luc bh - phan bhiem xd lap dat
    /// </summary>
    public DateTime? NgayHluc { get; set; }

    /// <summary>
    /// So thang bao hanh ctrinh - Phan bh ky thuat
    /// </summary>
    public decimal ThangBh { get; set; }

    /// <summary>
    /// chỉ số rủi ro t.sản: nghiệp vụ tài sản
    /// </summary>
    public string Category { get; set; } = null!;

    /// <summary>
    /// chỉ số rủi ro gió lửa: nghiệp vụ tài sản
    /// </summary>
    public string WindFire { get; set; } = null!;

    /// <summary>
    /// Nghiệp vụ tài sản
    /// </summary>
    public string BiCode { get; set; } = null!;

    /// <summary>
    /// dung cho nghiep vu con nguoi TNC
    /// </summary>
    public decimal SoNgtg { get; set; }

    /// <summary>
    /// Luat ap dung- phan he nghiep vu tai san
    /// </summary>
    public string LuatApdung { get; set; } = null!;

    /// <summary>
    /// dieu khoan bo sung-phan he nghiep vu tai san
    /// </summary>
    public string DkBosung { get; set; } = null!;

    /// <summary>
    /// Luu so chuyen gia trong bao hiem trach nhiem
    /// </summary>
    public int SoChuyengia { get; set; }

    /// <summary>
    /// Luu so tro ly giup viec trong bao hiem trach nhiem
    /// </summary>
    public int SoTroly { get; set; }

    public DateTime? NgayDauda { get; set; }

    public DateTime? NgayCuoida { get; set; }

    /// <summary>
    /// Gia tri du an
    /// </summary>
    public decimal GiaTrida { get; set; }

    /// <summary>
    /// Phi tu van
    /// </summary>
    public decimal PhiTuvan { get; set; }

    /// <summary>
    /// Ngay hoi to trong bao hiem trach nhiem
    /// </summary>
    public DateTime? NgayHoito { get; set; }

    public string MaNhloaixe { get; set; } = null!;

    /// <summary>
    /// Ma Dac diem doi tuong BH-Phan he Hanghoa vaTai san
    /// </summary>
    public string MaDd { get; set; } = null!;

    /// <summary>
    /// Mã địa điểm bhiểm -dùng cho TSKT và XDLD
    /// </summary>
    public string MaDdiembh { get; set; } = null!;

    public string SoHokhau { get; set; } = null!;

    public DateTime? NgayCaphk { get; set; }

    public string NoiCaphk { get; set; } = null!;

    /// <summary>
    /// số tiền dọn dẹp hiện trường-cháy tài sản
    /// </summary>
    public string GtriDdht { get; set; } = null!;

    /// <summary>
    /// Energy Care
    /// </summary>
    public string ThongTin { get; set; } = null!;

    /// <summary>
    /// Bao hiem nang luong
    /// </summary>
    public decimal RenewRate { get; set; }

    /// <summary>
    /// Bao hiem nang luong
    /// </summary>
    public decimal SurveyRate { get; set; }

    /// <summary>
    /// Bao hiem nang luong 
    /// </summary>
    public decimal ProfitRate { get; set; }

    /// <summary>
    /// Muc giu lai cua PVI - Ban BHNL
    /// </summary>
    public decimal MucglRate { get; set; }

    public string SoYcauSdbs { get; set; } = null!;

    public DateTime? NgayYcauSdbs { get; set; }

    /// <summary>
    /// Thong tin de in SDBS
    /// </summary>
    public string NoiDungSdbs { get; set; } = null!;

    /// <summary>
    /// Thong tin de in SDBS
    /// </summary>
    public string PhiBhSdbs { get; set; } = null!;

    /// <summary>
    /// Thong tin de in SDBS
    /// </summary>
    public string CamKetSdbs { get; set; } = null!;

    public string QuyenLoibh { get; set; } = null!;

    /// <summary>
    /// Thong tin pham vi tai phan tren GCN
    /// </summary>
    public string PhamviTaiphan { get; set; } = null!;

    /// <summary>
    /// Thong tin dieu khoan thanh toan tren GCN
    /// </summary>
    public string DkhoanTtoan { get; set; } = null!;

    /// <summary>
    /// Thong tin bo sung tren GCN
    /// </summary>
    public string ThongtinBosung { get; set; } = null!;

    /// <summary>
    /// Thong tin gioi han lanh tho tren GCN
    /// </summary>
    public string GioihanLanhtho { get; set; } = null!;

    /// <summary>
    /// Số tiền bảo hiểm- chi tiết các hạng mục XDLD
    /// </summary>
    public string SoTienbhct { get; set; } = null!;

    /// <summary>
    /// Tỷ lệ phí  bảo hiểm- chi tiết các hạng mục XDLD
    /// </summary>
    public string TylePhibh { get; set; } = null!;

    /// <summary>
    /// Phí bảo hiểm- chi tiết các hạng mục XDLD
    /// </summary>
    public string PhiBh { get; set; } = null!;

    /// <summary>
    /// Luu thong tin Express warranties cua don bao hiem nong nghiep
    /// </summary>
    public string DamBao { get; set; } = null!;

    /// <summary>
    /// Luu thong tin The event period cua don nghiep vu bao hiem nong nghiep + thong tin ve nguoi dbh (Nang luong)
    /// </summary>
    public string ThoihanSuco { get; set; } = null!;

    /// <summary>
    /// Dien giai thoi han bao hiem, cho phep go text thoi han bao hiem trong don bao hiem nong nghiep
    /// </summary>
    public string DiengiaiThoihan { get; set; } = null!;

    public decimal TyleDong { get; set; }

    /// <summary>
    /// Trạng thái của đơn
    /// </summary>
    public string TrangThai { get; set; } = null!;

    /// <summary>
    /// Giám định điều kiện (Hàng hải trọn gói năng lượng)
    /// </summary>
    public string GiamdinhDk { get; set; } = null!;

    /// <summary>
    /// Giám định tổn thất (Hàng hải trọn gói năng lượng)
    /// </summary>
    public string GiamdinhTt { get; set; } = null!;

    /// <summary>
    /// điều kiện bắt buộc (Hàng hải trọn gói năng lượng)
    /// </summary>
    public string DieukienBb { get; set; } = null!;

    /// <summary>
    /// Trách nhiệm  (Hàng hải trọn gói năng lượng)
    /// </summary>
    public string TrachNhiem { get; set; } = null!;

    /// <summary>
    /// Các khoản giảm trừ (Hàng hải trọn gói năng lượng)
    /// </summary>
    public string KhoanGtru { get; set; } = null!;

    /// <summary>
    /// Thuế và các khoản phải trả (Hàng hải trọn gói năng lượng)
    /// </summary>
    public string ThuePtra { get; set; } = null!;

    /// <summary>
    /// mau don xay dung lap dat ngoai khoi
    /// </summary>
    public string MauDon { get; set; } = null!;

    /// <summary>
    /// loai hinh bao hiem don xdld ngoai khoi
    /// </summary>
    public string LoaiHinhbh { get; set; } = null!;

    public string? DiemLoaitru { get; set; }

    public string MaNhhhoa { get; set; } = null!;

    public string RloaiDon { get; set; } = null!;

    public string RctrucXdung { get; set; } = null!;

    public int RnamXdung { get; set; }

    public string RnhomRro { get; set; } = null!;

    public int RtangNoi { get; set; }

    public int RtangHam { get; set; }

    public decimal PhiCnbb { get; set; }

    public string MaLoaits { get; set; } = null!;

    public string MaKenhbh { get; set; } = null!;

    public string MaNhkenhbh { get; set; } = null!;

    public decimal PrKeyBhtt { get; set; }

    public DateTime? NgayCnhat { get; set; }

    public bool ChkTpc { get; set; }

    public DateTime? NgayKy { get; set; }

    public string MaNhang { get; set; } = null!;

    public decimal TyleCnbb { get; set; }

    public string SoBke { get; set; } = null!;

    public string SoDonbhSdbs { get; set; } = null!;

    public string LoaiRuiro { get; set; } = null!;

    public decimal SoTienbhQtac { get; set; }

    public int SoCuocQtac { get; set; }

    public int SoNgayQtac { get; set; }

    public string? DoituongBhTcd { get; set; }

    public decimal RequestId { get; set; }

    public string FileName { get; set; } = null!;

    public decimal TyleBthuong { get; set; }

    public decimal TyleGiamphi { get; set; }

    public string SoAc { get; set; } = null!;

    public string FileHopdong { get; set; } = null!;

    public int TthaiTtoan { get; set; }

    public string FileGyc { get; set; } = null!;

    public string FileNth { get; set; } = null!;

    public int XulyNvu { get; set; }

    public string BchaoPhi { get; set; } = null!;

    public string ChiTietHd { get; set; } = null!;

    public string MauHd { get; set; } = null!;

    public string MaSdbsPias { get; set; } = null!;

    public string SoDonPias { get; set; } = null!;

    public string DinhMucTl { get; set; } = null!;

    public int SoVubt { get; set; }

    public int ThoihanBh { get; set; }

    public string MaSdbsCt { get; set; } = null!;

    public string MaMdsd { get; set; } = null!;

    public decimal SotienBhTnanKhac { get; set; }

    public string MaSovat { get; set; } = null!;

    public bool LoaiTtoan { get; set; }

    public bool HdonDientu { get; set; }

    public string SoDonbhNt { get; set; } = null!;

    public string TtinHdDientu { get; set; } = null!;

    public string DiaChiVat { get; set; } = null!;

    public string TenKhVat { get; set; } = null!;

    public bool NhomSp { get; set; }

    public string MaDonvint { get; set; } = null!;

    public int GuiSms { get; set; }

    public int GuiZalo { get; set; }

    public int GuiViber { get; set; }

    public string SoHdongVvon { get; set; } = null!;

    public string NguonTao { get; set; } = null!;

    public bool LdonKhachhang { get; set; }

    public string MaGthieu { get; set; } = null!;

    public string TennguoiMuabh { get; set; } = null!;

    public string DiachiNguoiMuabh { get; set; } = null!;

    public string MaDoitac { get; set; } = null!;

    public int TthaiTbao { get; set; }

    public string MaGdichDoitac { get; set; } = null!;

    public int TthaiQuery { get; set; }

    public string IdControl { get; set; } = null!;

    public bool MienThue { get; set; }

    public int TthaiBthuong { get; set; }

    public string CtybhCu { get; set; } = null!;

    public DateTime? TimeRetry { get; set; }
}
