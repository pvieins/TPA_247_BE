using AutoMapper;
using Microsoft.Identity.Client;
using PVI.DAO.Entities.Models;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace PVI.Service.Request;

public class HsgdCtuRequest
{
    public HsgdCtuUpdateRequest HsgdCtuUdpdate { get; set; }
    public string? so_seri { get; set; } = null!;
}

public partial class HsgdCtuUpdateRequest
{

    public int PrKey { get; set; }
    ///public string? MaCtu { get; set; }

    public DateTime? NgayCtu { get; set; }
    //public string? SoHsgd { get; set; } = null!;

    public string? MaTtrangGd { get; set; } = null!;

    // Mã loại hồ sơ 
    public string MaLhsbt { get; set; } = null!;

    //public string? TrangThai { get; set; } = null!;

    public decimal SoLanBt { get; set; }

    public string? TenLaixe { get; set; } = null!;

    public string? DienThoai { get; set; } = null!;

    public bool? ChkDaydu { get; set; } = null;

    public bool? ChkDunghan { get; set; } = null;

    public string? DienThoaiNdbh { get; set; } = null!;

    public string NgayThuphi { get; set; } = null!;

    public string GhiChu { get; set; } = null!;

    public DateTime? NgayTbao { get; set; } = null!;

    public DateTime? NgayTthat { get; set; } = null!;

    public string? DiaDiemtt { get; set; } = null!;

    public string? MaNguyenNhanTtat { get; set; }

    public string? NguyenNhanTtat { get; set; } = null!;

    public string? HauQua { get; set; } = null!;

    public DateTime NgayGdinh { get; set; }

    public string DiaDiemgd { get; set; } = null!;

    public string MaDdiemTthat { get; set; } = null!;
    public decimal? SoTienBaoHiem { get; set; } = null!;
    public decimal? SoTienThucTe { get; set; } = null!;
    public string BienKsoat { get; set; } = null!;

    //báo cáo giám định
    public int NamSinh { get; set; }
    public string SoGphepLaixe { get; set; } = null!;

    public DateTime? NgayDauLaixe { get; set; }

    public DateTime? NgayCuoiLaixe { get; set; }

    public string MaLoaibang { get; set; } = null!;

    public string SoGphepLuuhanh { get; set; } = null!;

    public DateTime? NgayDauLuuhanh { get; set; }

    public DateTime? NgayCuoiLuuhanh { get; set; }
    public string HosoPhaply { get; set; } = null!;

    public string YkienGdinh { get; set; } = null!;

    public string DexuatPan { get; set; } = null!;
    public int DangKiem { get; set; }
    public string MaDonviTt { get; set; } = null!;
    // END báo cáo giám định

    //public string MaPkt { get; set; } = null!;

    //public string MaDonbh { get; set; } = null!;
}

public class HsgdGanGiamDinh
{
    public string oidGiamDinhVien { get; set; }
    public string? oidCanBoTT { get; set; } = null!;
    public bool guiEmail { get; set; }
    public bool guiSMS { get; set; }
}

public class HsgdGanNguoiDuyet
{
    public string? oidCanBoPheDuyet { get; set; } = null!;
    public string? ghiChu { get; set; } = "";
}


public class HsgdCtuChoPheDuyet
{
    public string? oidCanBoPheDuyet { get; set; } = null!;
    public string? ghiChu { get; set; } = null;
}

public class HsgdCtuBoSungThongTin
{
    public string? ghiChu { get; set; } = null!;
    public bool guiEmail { get; set; }
    public bool guiSMS { get; set; }
}
public class baogia_request
{
    public int pr_key { get; set; }
    public decimal so_tien { get; set; }
    public DateTime ngay_bao_gia { get; set; }    
    public string? de_xuat { get; set; }
}
public class duyetgia_request
{
    public int pr_key { get; set; }
    public decimal so_tien { get; set; }
    public DateTime ngay_duyet_gia { get; set; }
    public string? de_xuat { get; set; }
}

public class UpdateImageRequest
{
    public int PrKey { get; set; }
    public int Stt {  get; set; }
    public string? DienGiai {  get; set; }
    public string? MaHmuc {  get; set; }
    public string? MaHmucSc {  get; set; }
}

public class PheDuyetBaoLanhRequest
{
    public int? bl1 { get; set; } = 0;
    public int? bl2 { get; set; } = 0;
    public int? bl3 { get; set; } = 0;
    public int? bl4 { get; set; } = 0;
    public int? bl5 { get; set; } = 0;
    public int? bl6 { get; set; } = 0;
    public int? bl7 { get; set; } = 0;
    public int? bl8 { get; set; } = 0;
    public int? bl9 { get; set; } = 0;
    public string? bl_tailieubs { get; set; } = null;
    
    public string? bl_dsemail { get; set; } = null;
    
    public string? bl_dsphone { get; set; } = null; 
    
    public string? ma_donvi_tt { get; set; } = null;
}
public class PheDuyetTBaoBTRequest
{
     
    public string DsEmail { get; set; } = null!;
    public int SoNgayTtoan { get; set; }
    public decimal TndsXeCoGioi { get; set; }
    public decimal TndsHangHoa { get; set; }
    public decimal TndsTaiNanHk { get; set; }
    public decimal TndsTaiSanKhac { get; set; }
    public decimal TndsNguoi { get; set; }   

}

public class GuiBaolanhRequest
{
    public string receiving_emails { get; set; } = "";
    public string receiving_phones { get; set; } = "";
}

public class LoiGiamDinhRequest
{
    public int ThieuAnhGDDK { get; set; } = 0;
    public int ThuPhiSai { get; set; } = 0;
    public int SaiDKDK { get; set; } = 0;
    public int SaiPhanCap { get; set; } = 0;
    public int TrucLoiBH { get; set; } = 0;
    public int SaiPhamKhac { get; set; } = 0;
}
public class LuuThongBaoBTRequest
{
    public int PrKeyHsgd { get; set; }
    public HsgdTbbt HsgdTbbt { get; set; } = new();
    public List<HsgdTbbtTt> HsgdTbbtTt { get; set; } = new();
}
public class UploadHoSoTTRequest
{
    public decimal PrKeyHsgdCtu { get; set; }
    public List<UploadHoSoTTFileRequest> hsgdattachfiles { get; set; } = new List<UploadHoSoTTFileRequest>();
}
public class UploadHoSoTTFileRequest
{
    public string? pr_key { get; set; }
    public string fileName { get; set; } = null!;
    public string? base64 { get; set; } = null!;
    public string? filePath { get; set; } = null!;
    public string? ghiChu { get; set; }
}
public class GuiThongBaoBTRequest
{
    public decimal PrKeyHsgdCtu { get; set; }
    public string EmailNhan { get; set; } = null!;
}
public class UpdateHoanThienHsttRequest
{
    public decimal PrKeyHsgdCtu { get; set; }
    public bool HoanThienHstt { get; set; }
}