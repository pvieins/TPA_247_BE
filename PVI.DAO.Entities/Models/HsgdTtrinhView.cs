using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PVI.DAO.Entities.Models;
public class HsgdTtrinhDetail
{
    public HsgdTtrinhView? hsgdTtrinh { get; set; }
    public List<HsgdTtrinhCtView>? hsgdTtrinhCt { get; set; }
    public List<HsgdThuHuongView>? hsgdThuHuong { get; set; }
}

public class HsgdThuHuongView
{
    public int PrKey { get; set; }
    public decimal? FrKey { get; set; }
    public string? TenChuTk { get; set; }
    public string? SoTaikhoanNh { get; set; }
    public string? TenNh { get; set; }
    public decimal? SotienTt { get; set; }
    public string? LydoTt { get; set; }
    public string? bnkCode { get; set; }
}

public class ListHsgdTtrinh
{
    public string SoHsbt { get; set; } = null!;
    public string SoHsgd { get; set; } = null!;
    public List<HsgdTtrinhView>? listHsgdTtrinhView { get; set; }
}
public class HsgdTtrinhView
{
    public decimal PrKey { get; set; }

    public string MaDonvi { get; set; } = null!;

    public string SoHsbt { get; set; } = null!;

    public string TenDttt { get; set; } = null!;

    public string NgGdich { get; set; } = null!;

    public string? NgayCtu { get; set; }

    public string? NgayTthat { get; set; }

    public decimal SoTien { get; set; }

    public string MaTtrang { get; set; } = null!;
    public string TenTtrangTt { get; set; } = null!;

    public string PathTtrinh { get; set; } = null!;

    public string PrKeyCt { get; set; } = null!;

    public string NguyenNhan { get; set; } = null!;

    public string HauQua { get; set; } = null!;

    public string TaisanThuhoi { get; set; } = null!;

    public string PanThoiTs { get; set; } = null!;

    public string GtrinhChikhac { get; set; } = null!;

    public decimal GiatriThuhoi { get; set; }

    public decimal ChiKhac { get; set; }

    public decimal PrKeyHsgd { get; set; }
    public decimal SoNgchet { get; set; }

    public decimal SoBthuong { get; set; }

    public bool ThamGia007 { get; set; }

    public decimal SoPhibh { get; set; }
    public string NgayThuphi { get; set; } = null!;

    public bool? ChkDaydu { get; set; }

    public bool? ChkDunghan { get; set; }
    public bool ChkChuanopphi { get; set; }
    public bool? ChkTheohopdong { get; set; }
    public string? TenKhach { get; set; } = null!;
    public string? DienThoaiNdbh { get; set; } = null!;
    public decimal SoTienThucTe { get; set; }
    public string? TenDonVi { get; set; } = null!;

    public string? SoHsgd { get; set; } = null!;
    public string? TenGDV { get; set; } = null!;
    public string? TenNguoiDuyet { get; set; } = null!;
    public decimal PrKeyTtoanCtu { get; set; }
    public string MaDonviTt { get; set; } = null!;
    public Guid Oid { get; set; }
    public DateTime? NgayDuTlieu { get; set; }
    public DateTime? NgayTtoan { get; set; }
    public decimal Songay_TtGara { get; set; }
}
public class HsgdTtrinhCtView
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public string MaSp { get; set; } = null!;

    public decimal SotienBh { get; set; }

    public decimal SotienBt { get; set; }

    public decimal SotienTu { get; set; }

    public string TinhToanbt { get; set; } = null!;
    public int MucVat { get; set; }

    public decimal SoTienBtVat { get; set; }

    public decimal PrKeyXml { get; set; }
    public string PathXml { get; set; } = null!;
    public string TenFile { get; set; } = null!;
    public string maDKhoan { get; set; } = "";
}
public class DongBaoHiem
{

    public string MaKH { get; set; } = null!;

    public string TenCtyBh { get; set; } = null!;

    public decimal TyleTg { get; set; }

    public decimal TyleTaiho { get; set; }

    public string VaiTro { get; set; } = null!;
}
public class SeriPhiBH
{
    public decimal MtnGtbhVnd { get; set; }
    public decimal? TongTien { get; set; }
    public decimal GiaTri_Tte { get; set; }
}
public class CheckDKBS007
{
    public bool DKBS007 { get; set; }
    public int? Songay_TtGara { get; set; }

}
public class LichSuBT
{
    public string LoaiHs { get; set; } = null!;
    public string SoHsbt { get; set; } = null!;
    public decimal SoTienp { get; set; }
}
public partial class TT_HsgdDx
{
    public string MaSp { get; set; } = null!;
    public decimal SoTienctkh { get; set; }
    public decimal SoTienGtbt { get; set; }
    public decimal Tienpdtt { get; set; }
    public decimal Tienpdsc { get; set; }
    public decimal GiamTruBt { get; set; }
    public decimal TyleggPhutung { get; set; }

    public decimal TyleggSuachua { get; set; }
}
public partial class HsgdDx_HM
{
    public string MaHmuc { get; set; } = null!;
    public string Hmuc { get; set; } = null!;
    public bool ThuHoiTs { get; set; }
}
public class HoSoTrinhKy
{
    public decimal PrKey { get; set; }
    public decimal PrKeyNky { get; set; }
    public string MaDonvi { get; set; } = null!;

    public string SoHsbt { get; set; } = null!;

    public string TenDttt { get; set; } = null!;

    public string NgGdich { get; set; } = null!;

    public DateTime? NgayCtu { get; set; }

    public DateTime? NgayTthat { get; set; }
    public string? NgayCtuText { get; set; }

    public string? NgayTthatText { get; set; }
    public decimal SoTien { get; set; }

    public string MaTtrang { get; set; } = null!;
    public string TenTtrangTt { get; set; } = null!;

    public decimal PrKeyHsgd { get; set; }

    public string? TenDonVi { get; set; } = null!;

    public string? SoHsgd { get; set; } = null!;
    public string? TenGDV { get; set; } = null!;
    public string? MaGDV { get; set; } = null!;
    public string? TenNguoiDuyet { get; set; } = null!;
    public decimal PrKeyTtoanCtu { get; set; }
    public string MaDonviTt { get; set; } = null!;
    public string? UserNhan { get; set; } = null!;
    public string? NgayDuyetText { get; set; }
    public DateTime? NgayDuyet { get; set; }
    public string MaDonviCapDon { get; set; } = null!;
    public DateTime? NgayTtoan { get; set; }
    public string? NguoiThuHuong { get; set; }
    public DateTime? NgayPsinh { get; set; }
    public string? NgayPsinhText { get; set; }
    public bool HoanThienHstt { get; set; }
    public string? SoTaikhoanNh { get; set; }
    public decimal PrKeyTTrinhCt { get; set; }
    public string Tinhtrang_thanhtoan { get; set; }
}
public class ThongTinToTrinhTPC
{
    public string TenKhach { get; set; } = null!;
    public string BienKsoat { get; set; } = null!;
    public decimal SoSeri { get; set; }
    public string NgayDauSeri { get; set; } = null!;
    public string NgayCuoiSeri { get; set; } = null!;
    public string TenDonvi { get; set; } = null!;
    public int LoaiXe { get; set; }
    public int HieuXe { get; set; }
    public int NamSx { get; set; }
    public decimal SoTienThucTe { get; set; }
    public string NgayTthat { get; set; } = null!;
    public string GioTthat { get; set; } = null!;
    public string DiaDiemtt { get; set; } = null!;
    public string NguyenNhanTtat { get; set; } = null!;
    public string MaDonvi { get; set; } = null!;
    public string MaDonvigd { get; set; } = null!;
    public string SoDonbh { get; set; } = null!;
    public decimal PrKeyBt { get; set; }
    public string DonviMe { get; set; } = null!;
    public string DonviU { get; set; } = null!;
    public string Donvi { get; set; } = null!;
    public string TP { get; set; } = null!;
    public string NgayThuPhi { get; set; } = null!;
    public string TenGara { get; set; } = null!;
    public string TenSP { get; set; } = null!;
    public string TenLoaiXe { get; set; } = null!;
    public decimal TongChiPhi { get; set; }
    public decimal SotienGiam { get; set; }
    public decimal TrachNhiemPVI { get; set; }
    public decimal TylephiPvi { get; set; }
    public string DsDkbs { get; set; } = null!;
    public string PathTotrinhTpc { get; set; } = null!;
    public string TenPTGD { get; set; } = null!;
    public string MailPTGD { get; set; } = null!;
}

public class GaRaView
{

    public string MaGara { get; set; } = null!;

    public string TenGara { get; set; } = null!;

    public string TenTat { get; set; } = null!;

    public decimal? TyleggPhutung { get; set; }

    public decimal? TyleggSuachua { get; set; }
    public string bnkCode { get; set; } = null!;
    public string ten_ctk { get; set; } = null!;

}
public class CheckHD
{
    public int ChkKhongHoadon { get; set; }
    public string ThongBao { get; set; }
}
