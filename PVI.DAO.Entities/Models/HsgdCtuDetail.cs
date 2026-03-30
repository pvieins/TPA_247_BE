using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.DAO.Entities.Models;
public class HsgdCtuDetail
{
    public int PrKey { get; set; }

    public string? MaDonvi { get; set; } = null!;// đơn vị
    public string? TenDonvi { get; set; } = null!;//tên đơn vị

    public string? SoHsgd { get; set; } = null!;//số hồ sơ giám định

    public string? SoDonbh { get; set; } = null!;//số đơn BH
    public string? SoHsbt { get; set; } = null!;//số đơn BH

    public string? TenKhach { get; set; } = null!;//tên NĐBH

    public string? NgayDauSeri { get; set; }// hiệu lực ngày đầu

    public string? NgayCuoiSeri { get; set; }// hiệu lực ngày cuối

    public decimal SoSeri { get; set; }// số ấn chỉ

    public string? ThinhNphi { get; set; } = null!;// tình trạng thu phí

    public string? NgGdichTh { get; set; } = null!;// Người thụ hưởng
   
    public string? BienKsoat { get; set; } = null!;// biển kiểm soát

    public decimal SoLanBt { get; set; } // SỐ lần bồi thường 
    public decimal SoTienBaoHiem { get; set; }//số tiền bảo hiểm
    public decimal SoTienThucTe { get; set; }//số tiền thực tế

    public string? NgayCtu { get; set; }//ngày nhập

    public string? NgayTbao { get; set; }//ngày thông báo

    public string? NgayTthat { get; set; }//ngày tổn thất

    public string? MaTtrangGd { get; set; } = null!;// mã tình trạng giám định

    public string? MaLhsbt { get; set; } = null!; // mã loại hồ sơ giám định

    public string? MaDdiemTthat { get; set; } = null!;//mã địa điểm tổn thất

    public string? DiaDiemtt { get; set; } = null!;//địa điểm tổn thất

    public string? NguyenNhanTtat { get; set; } = null!;//nguyên nhân tổn thất

    public string? MaNguyenNhanTtat { get; set; }//mã nguyên nhân tổn thất

    public string? TenLaixe { get; set; } = null!;// tên lái xe

    public string? DienThoai { get; set; } = null!;//điện thoại lái xe

    public string? DienThoaiNdbh { get; set; }//điện thoại chủ

    public string GhiChu { get; set; } = null!;//ghi chú

    public string? NgayGdinh { get; set; }//ngày giám định

    public string? DiaDiemgd { get; set; } = null!;//địa điểm giám định
    //ngày thu phí
    public string? NgayThuphi { get; set; }//ngày thu phí
    //hậu quả

    public string HauQua { get; set; } = null!;

    public decimal PrKeyBt { get; set; }
    public decimal PrKeyGoc { get; set; }
    public List<NvuBhtKyphiView>? nvuBhtKyphis { get; set; }

    public bool ChkDaydu { get; set; }

    public bool ChkDunghan { get; set; }
    public bool ChkTheohopdong { get; set; }

    public int HsgdTpc { get; set; }
    //báo cáo giám định
    public int NamSinh { get; set; }
    public string SoGphepLaixe { get; set; } = null!;

    public string NgayDauLaixe { get; set; } = null!;

    public string NgayCuoiLaixe { get; set; } = null!;

    public string MaLoaibang { get; set; } = null!;

    public string SoGphepLuuhanh { get; set; } = null!;

    public string NgayDauLuuhanh { get; set; } = null!;

    public string NgayCuoiLuuhanh { get; set; } = null!;
    public string HosoPhaply { get; set; } = null!;

    public string YkienGdinh { get; set; } = null!;

    public string DexuatPan { get; set; } = null!;
    public int DangKiem { get; set; }
    public string MaDonviTt { get; set; } = null!;
    public int NamTraCuu { get; set; }
    public int ThieuAnh { get; set; }
    public int ChuaThuPhi { get; set; }
    public int SaiDKDK { get; set; }
    public int SaiPhanCap { get; set; }
    public int TrucLoiBH { get; set; }
    public int SaiPhamKhac { get; set; }
    // END báo cáo giám định
    public int LoaiTotrinhTpc { get; set; }
    public string PathTotrinhTpc { get; set; } = null!;
    public bool Tpc { get; set; }
    // số tiền ước giám định
    public decimal SoTienugd { get; set; }
}
public partial class NvuBhtKyphiView
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public int Stt { get; set; }

    public string? NgayHl { get; set; }

    public decimal TylePhithu { get; set; }

    public decimal SoTien { get; set; }
}
public class DmUserView
{
    public Guid? Oid { get; set; } = null!; // Thêm trường Oid để đưa vào nhật ký 
    public string? MaDonvi { get; set; }
    public string? TenUser { get; set; }

    public string? MaUser { get; set; }
    public int? LoaiUser { get; set; }

    public string? Dienthoai { get; set; }
    public string? UQ_HoSo_TPC { get; set; }
    public bool? isGdvHoTro { get; set; }

    public string? MaDonviPquyen { get; set; } = "";

}
public class TraSeri
{
    public string Tra_Seri { get; set; }
    public string Tra_Seri_ttoan { get; set; }
    public string Tra_seri_hhong { get; set; }
    public string Tra_seri_bthuong { get; set; }
    public string Thong_bao { get; set; }

}
 