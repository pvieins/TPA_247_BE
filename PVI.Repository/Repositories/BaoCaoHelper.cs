using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using PdfSharpCore.Pdf.Advanced;
using PVI.DAO.Entities.Models;
using PVI.Helper;
using PVI.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using Microsoft.Office.Interop.Word;
using System.ComponentModel;
using System.ComponentModel.Design;

// BaoCaoHelper:
// File này để làm gì? Do nghiệp vụ báo cáo rất lớn và yêu cầu rất nhiều function hỗ trợ liên quan đến repository, các function hỗ trợ sẽ được tổng hợp hết vào file helper cho báo cáo này.
//

namespace PVI.Repository.Repositories
{

    // 1 - THỐNG KÊ GDTT THEO ĐƠN VỊ:

    public class ThongKe_GDTT_DonVi_Filter
    {
        public string? MaDonVi { get; set; } = null!;
        public DateTime? TuNgay { get; set; } = null!;
        public DateTime? DenNgay { get; set; } = null!;
    }

    public class ThongKe_GDTT_DonVi_Item
    {
        public string? TenDonvi { get; set; } = null!;
        public decimal? SoLuongNhapPIAS { get; set; } = 0;
        public decimal? SoLuongGDTT { get; set; } = 0;
        public decimal? SoLuongKhongGDTT { get; set; } = 0;
        public float? TyLeGDTT { get; set; } = 0;
        public decimal? ChuaGiaoGD { get; set; } = 0;
        public decimal? DaGiaoGD { get; set; } = 0;
        public decimal? DangGD { get; set; } = 0;
        public decimal? ChoPD { get; set; } = 0;
        public decimal? BSTT { get; set; } = 0;
        public decimal? DaDuyet { get; set; } = 0;
        public decimal? TongCong { get; set; } = 0;
    }

    // Tất gom lại thành hệ Data và Count để dễ phân trang
    public class ThongKe_GDTT_DonVi_Response
    {
        public long Count { get; set; } = 0;
        public List<ThongKe_GDTT_DonVi_Item>? Data { get; set; }
    }





    // 2 - THỐNG KÊ GDTT THEO GIÁM ĐỊNH VIÊN

    public class ThongKe_GDTT_GDV_Filter
    {
        public string? MaDonVi { get; set; } = null!;
        public DateTime? TuNgay { get; set; } = null!;
        public DateTime? DenNgay { get; set; } = null!;
    }

    public class ThongKe_GDTT_GDV_Item
    {
        public string? TenDonvi { get; set; } = null!;
        public string? TenGDV { get; set; } = null!;
        public string? LoaiUser { get; set; } = null!;
        public decimal? SoLuongNhapPIAS { get; set; } = 0;
        public decimal? SoLuongGDTT { get; set; } = 0;
        public decimal? SoLuongKhongGDTT { get; set; } = 0;
        public float? TyLeGDTT { get; set; } = 0;
        public decimal? ChuaGiaoGD { get; set; } = 0;
        public decimal? DaGiaoGD { get; set; } = 0;
        public decimal? DangGD { get; set; } = 0;
        public decimal? ChoPD { get; set; } = 0;
        public decimal? BSTT { get; set; } = 0;
        public decimal? DaDuyet { get; set; } = 0;
        public decimal? TongCong { get; set; } = 0;
    }

    // Tất gom lại thành hệ Data và Count để dễ phân trang
    public class ThongKe_GDTT_GDV_Response
    {
        public long Count { get; set; } = 0;
        public List<ThongKe_GDTT_GDV_Item>? Data { get; set; }
    }















    // 3 - THỐNG KÊ CHI TIẾT TÌNH HÌNH THỰC HIỆN GDTT
    // Khi gọi API sẽ gọi biến dưới đây, là tổng hợp của hai loại filter: 1 filter chính và 1 filter bảng
    public class ThongKeGDTT_GeneralFilter
    {
        // Filter chính
        public ThongKeGDTT_General_Main_Filter? mainFilter { get; set; } = null!;
        // Filter trong bảng
        //public ThongKeGDTT_General_Side_Filter? sideFilter { get; set; } = null!;
    }

    // Mở class cho filter chính
    public class ThongKeGDTT_General_Main_Filter
    {
        public string? MaDonVi { get; set; } = null!; 
        public string? MaDonViTt { get; set; } = null!;
        public string? TuNgay { get; set; } = null!;
        public string? DenNgay { get; set; } = null!;
        public string? TuNgayDuyettpc { get; set; } = null!;
        public string? DenNgayDuyettpc { get; set; } = null!;
        public string? TuNgayPDTT { get; set; } = null!;
        public string? DenNgayPDTT { get; set; } = null!;
        public string? MaTtrangGd { get; set; } = null!;
        public string? LoaiHsgd { get; set; } = null!;
        public string? SoHsgd { get; set; } = null!;
        public string? SoAnChi { get; set; } = null!;
        public string? BienKSoat { get; set; } = null!;
        public bool? IsTPC { get; set; } = null!; // 0 for DPC, 1 for TPC
        public string? maCanBo { get; set; } = null!; 
    }

    // Mở class cho filter trong bảng
    //public class ThongKeGDTT_General_Side_Filter
    //{
    //    public string? TenDonvi { get; set; } = null!;
    //    public string? SoHsgd { get; set; } = null!;
    //    public DateTime? TenKhach { get; set; } = null!;
    //    public string? SoDonBH { get; set; } = null!;
    //    public string? SoAnChi { get; set; } = null!;
    //    public string? BienKSoat { get; set; } = null!;
    //    public string? HieuXe { get; set; } = null!;
    //    public DateTime? NgayTThat { get; set; } = null!;
    //    public string? DiaDiemTThat { get; set; } = null!;
    //    public string? MaNguyenNhan { get; set; } = null!;
    //    public string? NguyenNhanTThat { get; set; } = null!;
    //    public string? NgayThongBaoDenNgayNhap { get; set; } = null!;
    //    public string? NgayNhapDenNgayDuyet { get; set; } = null!;
    //    public decimal? SoLanGD { get; set; } = null!;
    //    public decimal? UocBDDgia { get; set; } = null!;
    //    public decimal? UocKTDgia { get; set; } = null!;
    //    public string? GDV { get; set; } = null!;
    //    public string? Tinhtrang { get; set; } = null!;
    //    public string? LoaiHsgd { get; set; } = null!;
    //    public string? HoSo { get; set; } = null!;
    //    public string? Gara { get; set; } = null!;
    //    public string? TenTatGara { get; set; } = null!;
    //    public string? MaGara { get; set; } = null!;
    //    public decimal? UocBanDau { get; set; } = null!;
    //    public decimal? TongTienTT { get; set; } = null!;
    //    public decimal? TongTienSC { get; set; } = null!;
    //    public decimal? TongTienSon { get; set; } = null!;
    //    public decimal? SoTienDxVcx { get; set; } = null!;
    //    public decimal? SoTienPdVcx { get; set; } = null!;
    //    public decimal? SoTienDxTnds { get; set; } = null!;
    //    public decimal? SoTienPdTnds { get; set; } = null!;
    //    public decimal? SoTienDxSc { get; set; } = null!;
    //    public decimal? SoTienPdSc { get; set; } = null!;
    //    public DateTime? NgayPD { get; set; } = null!;
    //    public string? CanBoTT { get; set; } = null!;
    //    public DateTime? NgayBSTTCuoi { get; set; } = null!;
    //    public string? SoHsbt { get; set; } = null!;
    //    public string? GhiChuDvVcx { get; set; } = null!;
    //    public string? GhiChuTTVcx { get; set; } = null!;
    //    public string? GhiChuDvTnds { get; set; } = null!;
    //    public string? GhiChuTTTnds{ get; set; } = null!;
    //    public string? GhiChuDvTaiSanKhac { get; set; } = null!;
    //    public decimal? VatVcx { get; set; } = null!;
    //    public decimal? CheTaiKHVcx { get; set; } = null!;
    //    public string? LyDoCTKHVcx { get; set; } = null!;
    //    public decimal? VatTnds { get; set; } = null!;
    //    public decimal? CheTaiKHTnds { get; set; } = null!;
    //    public string? LyDoCTKHTnds { get; set; } = null!;
    //    public string? CheTaiKHScKhac { get; set; } = null!;
    //    public string? LyDoKHScKhac { get; set; } = null!;
    //    public decimal? GGPtVcx { get; set; } = null!;
    //    public string? GGScVcx { get; set; } = null!;
    //    public decimal? GGPtTnds { get; set; } = null!;
    //    public decimal? GGScTnds { get; set; } = null!;
    //    public decimal? GGPhuTungVcx { get; set; } = null!;
    //    public decimal? GGSuaChuaVcx { get; set; } = null!;
    //    public decimal? GGPhuTungTnds { get; set; } = null!;
    //    public decimal? GGSuaChuaTnds { get; set; } = null!;
    //    public string? NguoiDuyet { get; set; } = null!;
    //    public string? GhiChu { get; set; } = null!;
    //    public string? LoaiXe { get; set; } = null!;
    //    public decimal? GiamTruVcx { get; set; } = null!;
    //    public decimal? GiamTruTnds { get; set; } = null!;
    //    public decimal? GiamTruTsk { get; set; } = null!;
    //    public decimal? KhauTruVcx { get; set; } = null!;
    //    public decimal? KhauTruTnds { get; set; } = null!;
    //    public decimal? KhauTruTsk { get; set; } = null!;
    //    public decimal? PVIBaoLanhVcx { get; set; } = null!;
    //    public decimal? PVIBaoLanhTnds { get; set; } = null!;
    //}

    // Khi trả về, sẽ trả về cấu trúc sau để nhét vào bảng
    public class ThongKeGDTT_General_Item
    {
        public string PrKey { get; set; }
        public string? TenDonvi { get; set; } = "";
        public string? SoHsgd { get; set; } = "";
        public string? NgayNhap { get; set; } = "";
        public string? TenKhach { get; set; } = "";
        public string? SoDonBH { get; set; } = "";
        public string? SoAnChi { get; set; } = "";
        public string? BienKSoat { get; set; } = "";
        public string? HieuXe { get; set; } = "";
        public string? NgayTThat { get; set; } = "";
        public string? DiaDiemTThat { get; set; } = "";
        public string? MaNguyenNhan { get; set; } = "";
        public string? NguyenNhanTThat { get; set; } = "";
        public string? NgayThongBaoDenNgayNhap { get; set; } = "";
        public string? NgayNhapDenNgayDuyet { get; set; } = "";
        public string? SoLanGD { get; set; } = "";
        public string? UocBDDgia { get; set; } = "";
        public string? UocKTDgia { get; set; } = "";
        public string? GDV { get; set; } = "";
        public string? Tinhtrang { get; set; } = "";
        public string? LoaiHsgd { get; set; } = "";
        public string? HoSo { get; set; } = "";
        public string? Gara { get; set; } = "";
        public string? TenTatGara { get; set; } = "";
        public string? MaGara { get; set; } = "";
        public string? UocBanDau { get; set; } = "";
        public string? TongTienTT { get; set; } = "";
        public string? TongTienSC { get; set; } = "";
        public string? TongTienSon { get; set; } = "";
        public string? SoTienDxVcx { get; set; } = "";
        public string? SoTienPdVcx { get; set; } = "";
        public string? SoTienDxTnds { get; set; } = "";
        public string? SoTienPdTnds { get; set; } = "";
        public string? SoTienDxScKhac { get; set; } = "";
        public string? SoTienPdScKhac { get; set; } = "";
        public string? NgayPD { get; set; } = "";
        public string? CanBoTT { get; set; } = "";
        public string? NgayBSTTCuoi { get; set; } = "";
        public string? SoHsbt { get; set; } = "";
        public string? GhiChuDvVcx { get; set; } = "";
        public string? GhiChuTTVcx { get; set; } = "";
        public string? GhiChuDvTnds { get; set; } = "";
        public string? GhiChuTTTnds { get; set; } = "";
        public string? GhiChuDvTaiSanKhac { get; set; } = "";
        public string? GhiChuTTTaiSanKhac { get; set; } = "";
        public string? VatVcx { get; set; } = "";
        public string? CheTaiKHVcx { get; set; } = "";
        public string? LyDoCTKHVcx { get; set; } = "";
        public string? VatTnds { get; set; } = "";
        public string? CheTaiKHTnds { get; set; } = "";
        public string? LyDoCTKHTnds { get; set; } = "";
        public string? CheTaiKHScKhac { get; set; } = "";
        public string? LyDoKHScKhac { get; set; } = "";
        public string? GGPtVcx { get; set; } = "";
        public string? GGScVcx { get; set; } = "";
        public string? GGPtTnds { get; set; } = "";
        public string? GGScTnds { get; set; } = "";
        public string? GGPhuTungVcx { get; set; } = "";
        public string? GGSuaChuaVcx { get; set; } = "";
        public string? GGPhuTungTnds { get; set; } = "";
        public string? GGSuaChuaTnds { get; set; } = "";
        public string? NguoiDuyet { get; set; } = "";
        public string? GhiChu { get; set; } = "";
        public string? LoaiXe { get; set; } = "";
        public string? GiamTruVcx { get; set; } = "";
        public string? GiamTruTnds { get; set; } = "";
        public string? GiamTruTsk { get; set; } = "";
        public string? KhauTruVcx { get; set; } = "";
        public string? KhauTruTnds { get; set; } = "";
        public string? KhauTruTsk { get; set; } = "";
        public string? PVIBaoLanhVcx { get; set; } = "";
        public string? PVIBaoLanhTnds { get; set; } = "";
        public decimal PBTH_UNUS { get; set; } = 0; // Pr_Key Bồi Thường, đổi tên tránh Expose 
        public decimal PBTH_H_UNUS { get; set; } = 0; // Pr_Key Bồi thường hộ, đổi tên tránh Expose
        //public string MaTTrangGd { get; set; }
    }

    // Tất gom lại thành hệ Data và Count để dễ phân trang
    public class ThongKeGDTT_General_Response
    {
        public long Count { get; set; } = 0;
        public string Data { get; set; } = "";
    }
   


    // Các bảng chức năng cho nghiệp vụ tra cứu giá phụ tùng

    // Khi gọi API sẽ gọi biến dưới đây, là tổng hợp của hai loại filter: 1 filter chính và 1 filter bảng
    public class SearchGiaPhuTungFilter
    {
        public SearchGiaPhuTung_Main_Filter? mainFilter { get; set; } = null!;
        //public SearchGiaPhuTung_Side_Filter? sideFilter { get; set; } = null!;
    }

    public class SearchGiaPhuTung_Main_Filter
    {
        public string? MaDonVi { get; set; } = null!;
        public DateTime? TuNgay { get; set; } = null!;
        public DateTime? DenNgay { get; set; } = null!;
        public string? MaHmuc { get; set; } = null!;
        public decimal? HieuXe { get; set; } = null!;
        public decimal? LoaiXe { get; set; } = null!;
        public string? XuatXu { get; set; } = null!;
        public string? Tinh { get; set; } = null!;
        public string? QuanHuyen { get; set; } = null!;
    }

    // Class này là filter trong bảng đã trả về
    public class SearchGiaPhuTung_Side_Filter
    {
        public string? BienKSoat { get; set; } = null!;
        public string? SoHsgd { get; set; } = null!;
        public DateTime? NgayTThat { get; set; } = null!;
        public string? HieuXe { get; set; } = null!;
        public string? LoaiXe { get; set; } = null!;
        public decimal? NamSx { get; set; } = null!;
        public string? XuatXu { get; set; } = null!;
        public string? TenHmucThayThe { get; set; } = null!;
        public string? GhiChuDonVi { get; set; } = null!;
        public decimal? GiaPhuTung { get; set; } = null!;
        public decimal? GiaThayThe { get; set; } = null!;
        public string? TenGara { get; set; } = null!;
        public DateTime? NgayDuyet { get; set; } = null!;
        public string? Tinh { get; set; } = null!;
        public string? QuanHuyen { get; set; } = null!;
    }

    // Dữ liệu trả của nghiệp vụ tra cứu giá phụ tùng
    public class SearchGiaPhuTungItem
    {
        public decimal? PrKey { get; set; } = null!;
        public string? BienKSoat { get; set; } = null!;
        public string? SoHsgd { get; set; } = null!;
        public DateTime? NgayTThat { get; set; } = null!;
        public string? HieuXe { get; set; } = null!;
        public string? LoaiXe { get; set; } = null!;
        public decimal? NamSx { get; set; } = null!;
        public string? XuatXu { get; set; } = null!;
        public string? TenHmucThayThe { get; set; } = null!;
        public string? GhiChuDonVi { get; set; } = null!;
        public decimal? GiaPhuTung { get; set; } = null!;
        public decimal? GiaThayThe { get; set; } = null!;
        public string? TenGara { get; set; } = null!;
        public DateTime? NgayDuyet { get; set; } = null!;
        public string? Tinh { get; set; } = null!;
        public string? QuanHuyen { get; set; } = null!;
    }

    public class SearchGiaPhuTungResponse
    {
        public int Count { get; set; } = 0;
        public List<SearchGiaPhuTungItem>? Data { get; set; }
    }







    // Các bảng chức năng cho nghiệp vụ thống kê tình hình hồ sơ trên phân cấp.

    public class HSTPC_Filter
    {
        public string? MaDonVi { get; set; } = null!;
        public DateTime? TuNgay { get; set; } = null!;
        public DateTime? DenNgay { get; set; } = null!;
        public string? SoHsgd { get; set; } = null!;
        public string? SoAnChi { get; set; } = null!;
        public string? BienKSoat { get; set; } = null!;
    }

    // Class này là filter trong bảng đã trả về, nếu cần sẽ dùng đến, còn luồng hiện tại thì chưa cần.
    //public class HSTPC_Side_Filter
    //{
    //    public string? BienKSoat { get; set; } = null!;
    //    public string? SoHsgd { get; set; } = null!;
    //    public DateTime? NgayTThat { get; set; } = null!;
    //    public string? HieuXe { get; set; } = null!;
    //    public string? LoaiXe { get; set; } = null!;
    //    public decimal? NamSx { get; set; } = null!;
    //    public string? XuatXu { get; set; } = null!;
    //    public string? TenHmucThayThe { get; set; } = null!;
    //    public string? GhiChuDonVi { get; set; } = null!;
    //    public decimal? GiaPhuTung { get; set; } = null!;
    //    public decimal? GiaThayThe { get; set; } = null!;
    //    public string? TenGara { get; set; } = null!;
    //    public DateTime? NgayDuyet { get; set; } = null!;
    //    public string? Tinh { get; set; } = null!;
    //    public string? QuanHuyen { get; set; } = null!;
    //}

    // Dữ liệu trả của nghiệp vụ tra cứu giá phụ tùng
    public class HSTPC_Item
    {
        public decimal? PrKey { get; set; } = null!;
        public string? MaDonvi { get; set; } = null!;
        public string? SoHsgd { get; set; } = null!;
        public string? TenKhach { get; set; } = null!;
        public decimal? SoAnChi { get; set; } = null!;
        public string? BienKSoat { get; set; } = null!;
        public DateTime? TuNgay { get; set; } = null!;
        public DateTime? DenNgay { get; set; } = null!;
        public DateTime? NgayTThat { get; set; } = null!;
        public decimal? UocBT { get; set; } = null!;
        public DateTime? NgayDeXuat { get; set; } = null!;
        public decimal? UocDX { get; set; } = null!;
        public DateTime? NgayDuyet { get; set; } = null!;
        public decimal? SoTienPD { get; set; } = null!;
        public string? DonviSuaChua { get; set; } = null!;
        public string? MaTtrangGd { get; set; } = null!;
        public int SoNgayXuLy { get; set; }
        public string? giamDinhVien { get; set; } = null!;
        public string? canBoTT { get; set; } = null!;
        public string? GhiChu { get; set; } = null!;
    }

    public class HSTPC_Response
    {
        public int Count { get; set; } = 0;
        public List<HSTPC_Item>? Data { get; set; }
    }










    // Các bảng chức năng cho báo cáo tra cứu giá trị thực tế
    // Khi gọi API sẽ gọi biến dưới đây, là tổng hợp của hai loại filter: 1 filter chính và 1 filter bảng

    public class SearchGtttFilter
    {
        public SearchGttt_Main_Filter? mainFilter { get; set; } = null!;
        //public SearchGttt_Side_Filter? sideFilter { get; set; } = null!;
    }

    // Filter để tra cứu giá trị thực tế.
    public class SearchGttt_Main_Filter
    {
        public string? MaDonVi { get; set; } = null!;
        public string? SoHSGD { get; set; } = null!;
        public string? BienKSoat { get; set; } = null!;
        public DateTime? TuNgay { get; set; } = null!;
        public DateTime? DenNgay { get; set; } = null!;
        public string? HieuXe { get; set; } = null!;
        public string? LoaiXe { get; set; } = null!;
        public string? XuatXu { get; set; } = null!;
        public string? Tinh { get; set; } = null!;
        public decimal? NamSx { get; set; } = null!;
    }

    public class SearchGttt_Side_Filter
    {
        public decimal? PrKeySerial { get; set; }
        public string? TenDonvi { get; set; } = null!;
        public string? BienKSoat { get; set; } = null!;
        public string? SoHsgd { get; set; } = null!;
        public string? HieuXe { get; set; } = null!;
        public string? LoaiXe { get; set; } = null!;
        public decimal? NamSx { get; set; } = null!;
        public string? XuatXu { get; set; } = null!;
        public DateTime? NgayDuyet { get; set; } = null!;
        public string? Tinh { get; set; } = null!;
        public decimal? SoTienThucTe { get; set; } = null!;
    }

    // Dữ liệu trả của nghiệp vụ GTTT
    public class SearchGtttItem
    {
        public decimal? PrKey { get; set; }
        public decimal? PrKeySerial { get; set; }
        public string? TenDonvi { get; set; } = null!;
        public string? BienKSoat { get; set; } = null!;
        public string? SoHsgd { get; set; } = null!;
        public string? HieuXe { get; set; } = null!;
        public string? LoaiXe { get; set; } = null!;
        public decimal? NamSx { get; set; } = null!;
        public string? XuatXu { get; set; } = null!;
        public DateTime? NgayDuyet { get; set; } = null!;
        public string Tinh { get; set; } = "";
        public decimal? SoTienThucTe { get; set; } = null!;
    }

    // Response tổng trả về
    public class SearchGtttResponse
    {
        public int Count { get; set; } = 0;
        public List<SearchGtttItem>? Data { get; set; }
    }



    // Các bảng chức năng cho báo cáo Thu hồi tài sản

    // Khi gọi API sẽ gọi biến dưới đây, là tổng hợp của hai loại filter: 1 filter chính và 1 filter bảng
    public class BCThuHoiTSFilter
    {
        public BCThuHoiTS_Main_Filter? mainFilter { get; set; } = null!;

        //public BCThuHoiTS_Side_FIlter? sideFilter { get; set; } = null!;
    }

    public class BCThuHoiTS_Main_Filter
    {
        public string? MaDonVi { get; set; } = null!;
        public string? SoDonBH { get; set; } = null!;
        public string? MaTtrangGd { get; set; } = null!;
        public string? TuNgay { get; set; } = null!;
        public string? DenNgay { get; set; } = null!;
        public string? SoAnChi { get; set; } = null!;
        public string? LoaiHsgd { get; set; } = null!;
        public string? SoHsgd { get; set; } = null!;
        public string? BienKSoat { get; set; } = null!;
    }

    //public class BCThuHoiTS_Side_FIlter
    //{
    //    public string? SoHsgd { get; set; } = null!;
    //    public string? BienKSoat { get; set; } = null!;
    //    public string? LoaiXe { get; set; } = null!;
    //    public string? GDV { get; set; } = null!;
    //    public string? VatTuTH { get; set; } = null!;
    //    public bool? KhongThuHoi { get; set; } = null!;
    //    public bool? ThuHoiChoTL { get; set; } = null!;
    //    public decimal? DoiTru { get; set; } = null!;
    //    public string? LuuKhoTSD { get; set; } = null!;
    //    public DateTime? NgayCapNhat { get; set; } = null!;
    //    public string? GhiChu { get; set; } = null!;
    //}

    public class BCThuHoiTSItem
    {
        public decimal? PrKey { get; set; } = null!;
        public string? SoHsgd { get; set; } = null!;
        public string? BienKSoat { get; set; } = null!;
        public string? LoaiXe { get; set; } = null!;
        public string? GDV { get; set; } = null!;
        public string? VatTuTH { get; set; } = null!;
        public bool? KhongThuHoi { get; set; } = null!;
        public bool? ThuHoiChoTL { get; set; } = null!;
        public decimal? DoiTru { get; set; } = null!;
        public string? LuuKhoTSD { get; set; } = null!;
        public DateTime? NgayCapNhat { get; set; } = null!;
        public string? GhiChu { get; set; } = null!;
    }
   
    public class BCThuHoiTSResponse
    {
        public int Count { get; set; } = 0;
        public List<BCThuHoiTSItem>? Data { get; set; }
    }
    public class BCThuHoiTSItemResponse
    {
        public int Count { get; set; } = 0;
        public Int64 TotalRecord { get; set; } = 0;
        public List<ThuHoiTSItems> Data { get; set; }
    }

    // Các function helper cùng với class chính sẽ ở dưới này.
    public class BaoCaoHelper : GenericRepository<HsgdCtu>
    {
        public BaoCaoHelper(GdttContext context, Pvs2024Context context_pias, Serilog.ILogger logger, IConfiguration conf) : base(context, context_pias, logger, conf)
        {

        }

        // Các function helper:
        // Validate lại phân quyền của user trước khi tiến hành chạy báo cáo
        // Kiểm tra phân quyền của User: 
        // - User phải thuộc đơn vị trung tâm và là loại user quản lý
        // - HOẶC User đã được cấp quyền.
        public bool validatePhanQuyenChayBC(DmUser currentUser)
        {
            List<PquyenCnang> phanquyen = _context.PquyenCnangs.Where(x => x.LoaiQuyen.Equals("BAOCAO03")).ToList();

            bool checkDonvi = (currentUser.MaDonvi == "00" || currentUser.MaDonvi == "31" || currentUser.MaDonvi == "32"); // Kiểm tra đơn vị
            bool checkLoaiUser = (currentUser.LoaiUser == 1 || currentUser.LoaiUser == 2 || currentUser.LoaiUser == 3 || currentUser.LoaiUser == 6 || currentUser.LoaiUser == 9 || currentUser.LoaiUser == 10); // Kiểm tra user phải thuộc loại quản lý.
            bool checkPhanQuyen = (phanquyen != null && phanquyen.Find(x => x.MaUser == currentUser.MaUser) != null); // Kiểm tra user được phân quyền

            return (checkPhanQuyen || (checkDonvi && checkLoaiUser));
        }


        // Sử dụng để map các tình trạng hồ sơ giám định trong trường hợp bảng dm_lhsbt không thể truy cập được.
        public string Map_Tinh_Trang_HSGD(string ma_lhsbt)
        {
            string result = "";

            switch (ma_lhsbt)
            {
                case "01": case "1":
                    result = "Tự giám định";
                    break;

                case "02": case "2":
                    result = "Nhờ giám định";
                    break;

                case "03": case "3":
                    result = "Giám định hộ";
                    break;

                case "04": case "4":
                    result = "Không xác định";
                    break;
            }
            return result;
        }

        // Hàm tính số ngày cách nhau giữa 2 DateTime
        // Sử dụng để handle các trường hợp Date Time nullable hoặc khi Hiệu số Date Time không chuẩn
        // Lưu ý: Dt2 > Dt1
        public int DAY_DIFF(DateTime dt1, DateTime dt2)
        {
            int year_diff = dt2.Year - dt1.Year;
            int month_diff = dt2.Month - dt1.Month;
            int day_diff = dt2.Day - dt1.Day;

            return 365 * year_diff + 30 * month_diff + day_diff;
        }


        // Hàm sử dụng để tính toán số lượng hồ sơ GDTT các tình trạng cho từng đơn vị, để dùng cho báo cáo số 1

        public List<decimal> TinhToan_SoLuong_GDTT_Donvi(string ma_donvi, DateTime? tu_ngay, DateTime? den_ngay)
        {
            List<decimal> result = new List<decimal>();

            // Tính toán tổng dữ liệu được nhập trên GDTT

            List<HsgdCtu> list_ctu = _context.HsgdCtus.Where(x => (x.MaDonvi.Equals(ma_donvi)) && (tu_ngay != null ? x.NgayCtu >= (tu_ngay) : true) && (den_ngay != null ? x.NgayCtu <= (den_ngay.Value.AddDays(1)) : true) && x.MaLhsbt != "2" && x.MaTtrangGd != "7").ToList();

            List<HsbtCtu> list_hsbt = (from hsbtCtu in _context_pias.HsbtCtus
                                       join hsbtCt in _context_pias.HsbtCts on hsbtCtu.PrKey equals hsbtCt.FrKey
                                       where (
                                            hsbtCtu.MaDonvi.Equals(ma_donvi) &&
                                            (hsbtCtu.MaLhsbt.Equals("TBT") || hsbtCtu.MaLhsbt.Equals("BTH")) &&
                                            hsbtCtu.MaCtu.Equals("BT01") &&
                                            hsbtCt.MaSp.Equals("050104") &&
                                            hsbtCt.MaTtrangBt != "04" &&
                                            (tu_ngay != null ? hsbtCtu.NgayCtu >= tu_ngay : true) &&
                                            (den_ngay != null ? hsbtCtu.NgayCtu <= den_ngay : true)
                                       )
                                       select new HsbtCtu
                                       {
                                           PrKey = hsbtCtu.PrKey,
                                           MaDonvi = hsbtCtu.MaDonvi,
                                           SoSeri = hsbtCtu.SoSeri,
                                           SoHsbt = hsbtCtu.SoHsbt,
                                           SoHdgcn = hsbtCtu.SoHdgcn,
                                       }
                                       ).Distinct().ToList();

            decimal luongNhapPias = list_hsbt.Count();
            decimal co_GDTT = (from hsbt in list_hsbt
                               join hsgd in list_ctu on hsbt.SoHdgcn equals hsgd.SoDonbh
                               where hsbt.SoSeri == hsgd.SoSeri && hsbt.MaDonvi == hsgd.MaDonvi
                               select new HsbtCtu()).Distinct().Count();
            if (co_GDTT > luongNhapPias)
            {
                co_GDTT = luongNhapPias; // Soft Cap
            }

            decimal khong_GDTT = luongNhapPias - co_GDTT;

            decimal chuaGiaoGD = 0;
            decimal daGiaoGD = 0;
            decimal dangGd = 0;
            decimal choPD = 0;
            decimal bstt = 0;
            decimal daDuyet = 0;

            list_ctu.ForEach(ctu =>
            {
                NhatKy nhatky_ctu = _context.NhatKies.Where(x => (
                    x.FrKey == ctu.PrKey &&
                    x.MaTtrangGd != "7" && x.MaTtrangGd != "BLDT" && x.MaTtrangGd != "DBL" && x.MaTtrangGd != "INDT" && x.MaTtrangGd != "HSBT" &&
                    (tu_ngay != null ? x.NgayCapnhat >= tu_ngay : true) && (den_ngay != null ? x.NgayCapnhat <= den_ngay.Value.Date.AddDays(1) : true)
                )).OrderByDescending(x => x.NgayCapnhat).FirstOrDefault();

                if (nhatky_ctu != null)
                {
                    switch (nhatky_ctu.MaTtrangGd)
                    {
                        case "1":
                            chuaGiaoGD += 1;
                            break;
                        case "2":
                            daGiaoGD += 1;
                            break;
                        case "3":
                            dangGd += ctu.PrKey;
                            break;
                        case "4":
                            choPD += 1;
                            break;
                        case "5":
                            bstt += 1;
                            break;
                        case "6":
                            daDuyet += 1;
                            break;
                    }
                }
            });

            result.Add(luongNhapPias); // [0]
            result.Add(co_GDTT); // [1]
            result.Add(khong_GDTT); // [2]
            result.Add(chuaGiaoGD); // [3]
            result.Add(daGiaoGD); // [4]
            result.Add(dangGd); // [5]
            result.Add(choPD); // [6]
            result.Add(bstt); // [7]
            result.Add(daDuyet); //[8]
            result.Add(chuaGiaoGD + daGiaoGD + dangGd + choPD + bstt + daDuyet); // [9]

            return result;
        }



        // Hàm sử dụng để tính toán số lượng hồ sơ GDTT các tình trạng cho từng giám định viên, để dùng cho báo cáo số 2

        public List<ThongKe_GDTT_GDV_Item> TinhToan_SoLuong_GDTT_GDV(string ma_donvi, DateTime? tu_ngay, DateTime? den_ngay)
        {

            List<ThongKe_GDTT_GDV_Item> result = new List<ThongKe_GDTT_GDV_Item>();

            // Tính toán tổng dữ liệu được nhập trên GDTT
            //List<HsgdCtu> list_ctu = _context.HsgdCtus.Where(x => (x.MaDonvi.Equals(ma_donvi)) && (tu_ngay != null ? x.NgayCtu >= (tu_ngay) : true) && (den_ngay != null ? x.NgayCtu <= (den_ngay.Value) : true) && x.MaLhsbt != "2" && x.MaTtrangGd != "7").ToList();

            // Lấy danh sách tất cả HSGD
            List<HsgdCtu> list_ctu = _context.HsgdCtus.Where(x => (x.MaDonvi.Equals(ma_donvi)) && (tu_ngay != null ? x.NgayCtu >= (tu_ngay) : true) && (den_ngay != null ? x.NgayCtu <= (den_ngay.Value.AddDays(1)) : true) && x.MaLhsbt != "2" && x.MaTtrangGd != "7").OrderBy(x => x.MaUser).ToList();

            // Lọc lấy danh sách GDV trong đó.
            List<Guid> list_gdv = list_ctu.Where(x => x.MaUser != null).Select(x => x.MaUser.Value).Distinct().ToList();

            // Láy danh sách tất cả HSBT
            List<HsbtCtu> list_hsbt = (from hsbtCtu in _context_pias.HsbtCtus
                                       join hsbtCt in _context_pias.HsbtCts on hsbtCtu.PrKey equals hsbtCt.FrKey
                                       where (
                                            hsbtCtu.MaDonvi.Equals(ma_donvi) &&
                                            (hsbtCtu.MaLhsbt.Equals("TBT") || hsbtCtu.MaLhsbt.Equals("BTH")) &&
                                            hsbtCtu.MaCtu.Equals("BT01") &&
                                            hsbtCt.MaSp.Equals("050104") &&
                                            hsbtCt.MaTtrangBt != "04" &&
                                            (tu_ngay != null ? hsbtCtu.NgayCtu >= tu_ngay : true) &&
                                            (den_ngay != null ? hsbtCtu.NgayCtu <= den_ngay : true)
                                       )
                                       select new HsbtCtu
                                       {
                                           PrKey = hsbtCtu.PrKey,
                                           MaDonvi = hsbtCtu.MaDonvi,
                                           SoSeri = hsbtCtu.SoSeri,
                                           SoHsbt = hsbtCtu.SoHsbt,
                                           SoHdgcn = hsbtCtu.SoHdgcn,
                                       }
                                       ).Distinct().ToList();

            // Sau đó thống kê cho từng GDV
            list_gdv.ForEach(item =>
            {

                ThongKe_GDTT_GDV_Item record = new ThongKe_GDTT_GDV_Item();

                List<HsgdCtu> list_ctu_mini = list_ctu.Where(x => x.MaUser == item).ToList();
                record.TenDonvi = _context.DmDonvis.Where(x => x.MaDonvi == ma_donvi).FirstOrDefault().TenDonvi;
                DmUser GDV = _context.DmUsers.Where(x => x.Oid == item).FirstOrDefault();
                record.TenGDV = GDV.TenUser;
                record.LoaiUser = _context.DmLoaiUsers.Where(x => x.LoaiUser == GDV.LoaiUser).FirstOrDefault().TenLoaiUser;

                //decimal luongNhapPias = (from hsbt in list_hsbt
                //join hsgd in list_ctu on hsbt.PrKey equals hsgd.PrKeyBt
                //where hsgd.MaUser == item
                //select new HsbtCtu()).Distinct().Count();

                decimal luongNhapPias = list_ctu_mini.Count();

                decimal co_GDTT = (from hsbt in list_hsbt
                                   join hsgd in list_ctu on hsbt.SoHdgcn equals hsgd.SoDonbh
                                   where hsgd.MaUser == item
                                   where hsbt.SoSeri == hsgd.SoSeri && hsbt.MaDonvi == hsgd.MaDonvi
                                   select new HsbtCtu()).Distinct().Count();
                if (co_GDTT > luongNhapPias)
                {
                    co_GDTT = luongNhapPias; // Soft Cap
                }

                record.SoLuongNhapPIAS = luongNhapPias;
                record.SoLuongGDTT = co_GDTT;
                record.SoLuongKhongGDTT = luongNhapPias - co_GDTT;

                record.TyLeGDTT = (float) (luongNhapPias != 0 ? (co_GDTT / luongNhapPias) * 100 : 0);

             list_ctu_mini.ForEach(ctu =>
                {
                    NhatKy nhatky_ctu = _context.NhatKies.Where(x => (
                        x.FrKey == ctu.PrKey &&
                        x.MaTtrangGd != "7" && x.MaTtrangGd != "BLDT" && x.MaTtrangGd != "DBL" && x.MaTtrangGd != "INDT" && x.MaTtrangGd != "HSBT" &&
                        (tu_ngay != null ? x.NgayCapnhat >= tu_ngay : true) && (den_ngay != null ? x.NgayCapnhat <= den_ngay.Value.Date.AddDays(1) : true)
                    )).OrderByDescending(x => x.NgayCapnhat).FirstOrDefault();

                    if (nhatky_ctu != null)
                    {
                        switch (nhatky_ctu.MaTtrangGd)
                        {
                            case "1":
                                record.ChuaGiaoGD += 1;
                                break;
                            case "2":
                                record.DaGiaoGD += 1;
                                break;
                            case "3":
                                record.DangGD += ctu.PrKey;
                                break;
                            case "4":
                                record.ChoPD += 1;
                                break;
                            case "5":
                                record.BSTT += 1;
                                break;
                            case "6":
                                record.DaDuyet += 1;
                                break;
                        }
                    }
                });

                record.TongCong = record.ChuaGiaoGD + record.DaGiaoGD + record.DangGD + record.ChoPD + record.BSTT + record.DaDuyet;
             

                result.Add(record);
            });

            return result;
    }
    



        // Hàm này để tổng hợp và tính toán thông tin của DX và DG, dùng cho báo cáo
        // Chỉ cần chọc DB 1 lần để lấy toàn bộ record (cải thiện performance so với việc chọc DB liên tục), sau đó sẽ thao tác trên các record này.
        // 
        public async Task<List<decimal>> TinhToan_DX_DG_BaoCao (int hsgd_pr_key)
        {
            HsgdCtu ctu = _context.HsgdCtus.Where(x=>x.PrKey == hsgd_pr_key).FirstOrDefault();
            // Láy danh sách các đè xuất
            List<HsgdDx> list_dxes = await _context.HsgdDxes.Where(x=>x.FrKey == ctu.PrKey).ToListAsync();

            // Lấy danh sách các duyệt giá
            List<HsgdDg> list_dgs = await _context.HsgdDgs.Where(x => x.FrKey == ctu.PrKey).ToListAsync();

            // Lấy danh sách TSK
            List<HsgdDxTsk> list_tsks = await _context.HsgdDxTsks.Where(x => x.FrKey == ctu.PrKey).ToListAsync();

            List<decimal> result = new List<decimal>();

            decimal uocBDGia = list_dgs.Where(x => !x.LoaiDg).FirstOrDefault() != null ? list_dgs.Where(x => !x.LoaiDg).FirstOrDefault().SoTien : 0;
            decimal uocKTDGia = list_dgs.Where(x => !x.LoaiDg).FirstOrDefault() != null ? list_dgs.Where(x => x.LoaiDg).FirstOrDefault().SoTien : 0;

            decimal tongTienTT = list_dxes.Select(x => x.SoTientt).Sum();
            decimal tongTienSC = list_dxes.Select(x => x.SoTienph).Sum();
            decimal tongTienSon = list_dxes.Select(x => x.SoTienson).Sum();
            decimal SoTienDxVcx = list_dxes.Where(x=>x.LoaiDx == 0).Select(x => x.SoTientt).Sum() + list_dxes.Where(x => x.LoaiDx == 0).Select(x => x.SoTienph).Sum() + list_dxes.Where(x => x.LoaiDx == 0).Select(x => x.SoTienson).Sum();
            decimal SoTienPdVcx = list_dxes.Where(x => x.LoaiDx == 0).Select(x => x.SoTienpdtt).Sum() + list_dxes.Where(x => x.LoaiDx == 0).Select(x => x.SoTienpdsc).Sum();
            decimal SoTienDxTnds = list_dxes.Where(x => x.LoaiDx == 1).Select(x => x.SoTientt).Sum() + list_dxes.Where(x => x.LoaiDx == 1).Select(x => x.SoTienph).Sum() + list_dxes.Where(x => x.LoaiDx == 1).Select(x => x.SoTienson).Sum();
            decimal SoTienPdTnds = list_dxes.Where(x => x.LoaiDx == 1).Select(x => x.SoTienpdtt).Sum() + list_dxes.Where(x => x.LoaiDx == 1).Select(x => x.SoTienpdsc).Sum();

            decimal SoTienDxScKhac = list_tsks.Select(x => x.SoTientt).Sum() + list_tsks.Select(x => x.SoTiensc).Sum();
            decimal SoTienPdScKhac = list_tsks.Select(x => x.SoTienpdtt).Sum() + list_tsks.Select(x => x.SoTienpdsc).Sum();

            decimal? GGPhuTungVcx = (ctu.HsgdTpc == 1 ? (list_dxes.Where(x => x.LoaiDx == 0).Select(x => x.SoTienpdtt).Sum()) * ctu.TyleggPhutungvcx / 100 : (list_dxes.Where(x => x.LoaiDx == 0).Select(x => x.SoTientt).Sum()) * ctu.TyleggPhutungvcx / 100); // Khác nhau ở đâu? 1 cái là PDTT còn 1 cái chỉ là TT thôi.
            decimal? GGSuaChuaVcx = (ctu.HsgdTpc == 1 ? (list_dxes.Where(x => x.LoaiDx == 0).Select(x => x.SoTienpdsc).Sum()) * ctu.TyleggSuachuavcx / 100 : (list_dxes.Where(x => x.LoaiDx == 0).Select(x => x.SoTienph).Sum() + list_dxes.Where(x => x.LoaiDx == 1).Select(x => x.SoTienson).Sum()) * ctu.TyleggSuachuavcx / 100);
            decimal? GGPhuTungTnds = (ctu.HsgdTpc == 1 ? (list_dxes.Where(x => x.LoaiDx == 1).Select(x => x.SoTienpdtt).Sum()) * ctu.TyleggPhutungtnds / 100 : (list_dxes.Where(x => x.LoaiDx == 1).Select(x => x.SoTientt).Sum()) * ctu.TyleggPhutungtnds / 100); // Khác nhau ở đâu? 1 cái là PDTT còn 1 cái chỉ là TT thôi.
            decimal? GGSuaChuaTnds = (ctu.HsgdTpc == 1 ? (list_dxes.Where(x => x.LoaiDx == 1).Select(x => x.SoTienpdsc).Sum()) * ctu.TyleggSuachuatnds / 100 : (list_dxes.Where(x => x.LoaiDx == 1).Select(x => x.SoTienph).Sum() + list_dxes.Where(x => x.LoaiDx == 1).Select(x => x.SoTienson).Sum()) * ctu.TyleggSuachuatnds / 100);

            result.Add(uocBDGia); // [0]
            result.Add(uocKTDGia); // [1]
            result.Add(tongTienTT); // [2]
            result.Add(tongTienSC); // [3] 
            result.Add(tongTienSon); // [4]
            result.Add(SoTienDxVcx); // [5]
            result.Add(SoTienPdVcx); // [6]
            result.Add(SoTienDxTnds); // [7]
            result.Add(SoTienPdTnds); // [8]
            result.Add(SoTienDxScKhac); // [9]
            result.Add(SoTienPdScKhac); // [10]
            result.Add(GGPhuTungVcx ?? 0); // [11]
            result.Add(GGSuaChuaVcx ?? 0); // [12]
            result.Add(GGPhuTungTnds ?? 0); // [13]
            result.Add(GGSuaChuaTnds ?? 0); // [14]


            // Tính tiền trách nhiệm PVI:

            decimal tongSoTienGomVAT = 0;
            decimal soTienGiamTru = 0;
            decimal soTienGiamGia = 0;
          

            // Số tiền bảo lãnh VCX:

            List<HsgdDx> vcx_dx = list_dxes.Where(x => x.LoaiDx == 0).ToList();

            decimal bao_lanh_vcx = 0; // "-1 để báo hiệu giá trị không tồn tại. Cái này phân biệt với "0", báo hiệu có giả trị nhưng không phát sinh."

            if (vcx_dx.Count > 0)
            {
                vcx_dx.ForEach(dx =>
                {
                    tongSoTienGomVAT += (dx.SoTientt + dx.SoTienph + dx.SoTienson) + (((dx.SoTientt + dx.SoTienph + dx.SoTienson) * dx.VatSc) / 100);
                    soTienGiamTru += ((((dx.SoTientt + (dx.SoTientt * dx.VatSc / 100)) - (((dx.SoTientt + (dx.SoTientt * dx.VatSc / 100)) * (ctu.TyleggPhutungvcx ?? 0)) / 100) + ((dx.SoTienph + dx.SoTienson) + ((dx.SoTienph + dx.SoTienson) * dx.VatSc / 100)) - ((((dx.SoTienph + dx.SoTienson) + ((dx.SoTienph + dx.SoTienson) * dx.VatSc / 100)) * (ctu.TyleggSuachuavcx ?? 0)) / 100)) * dx.GiamTruBt) / 100);
                    soTienGiamGia += ((((dx.SoTientt + (dx.SoTientt * dx.VatSc) / 100) * ((ctu.TyleggPhutungvcx ?? 0) / 100)) + (((dx.SoTienph + dx.SoTienson) + ((dx.SoTienph + dx.SoTienson) * dx.VatSc) / 100) * (ctu.TyleggSuachuavcx ?? 0) / 100)));
                });

                bao_lanh_vcx = Math.Round(tongSoTienGomVAT - ((soTienGiamTru != 0) ? soTienGiamTru : ctu.SoTienGtbt) - soTienGiamGia - ctu.SoTienctkh);
            }
                   
            result.Add(bao_lanh_vcx); // [15]


            // Số tiền bảo lãnh TNDS:

            List<HsgdDx> tnds_dx = list_dxes.Where(x => x.LoaiDx == 1).ToList();

            decimal bao_lanh_tnds = 0; // "-1 để báo hiệu giá trị không tồn tại. Cái này phân biệt với "0", báo hiệu có giả trị nhưng không phát sinh."

            if (tnds_dx.Count > 0)
            {
                tnds_dx.ForEach(dx =>
                {
                    tongSoTienGomVAT += (dx.SoTientt + dx.SoTienph + dx.SoTienson) + (((dx.SoTientt + dx.SoTienph + dx.SoTienson) * dx.VatSc) / 100);
                    soTienGiamTru += ((((dx.SoTientt + (dx.SoTientt * dx.VatSc / 100)) - (((dx.SoTientt + (dx.SoTientt * dx.VatSc / 100)) * (ctu.TyleggPhutungvcx ?? 0)) / 100) + ((dx.SoTienph + dx.SoTienson) + ((dx.SoTienph + dx.SoTienson) * dx.VatSc / 100)) - ((((dx.SoTienph + dx.SoTienson) + ((dx.SoTienph + dx.SoTienson) * dx.VatSc / 100)) * (ctu.TyleggSuachuavcx ?? 0)) / 100)) * dx.GiamTruBt) / 100);
                    soTienGiamGia += ((((dx.SoTientt + (dx.SoTientt * dx.VatSc) / 100) * ((ctu.TyleggPhutungvcx ?? 0) / 100)) + (((dx.SoTienph + dx.SoTienson) + ((dx.SoTienph + dx.SoTienson) * dx.VatSc) / 100) * (ctu.TyleggSuachuavcx ?? 0) / 100)));
                });

                bao_lanh_vcx = Math.Round(tongSoTienGomVAT - ((soTienGiamTru != 0) ? soTienGiamTru : ctu.SoTienGtbt) - soTienGiamGia - ctu.SoTienctkh);
            }

            result.Add(bao_lanh_tnds); // [16]

            return result;
        }

        // khanhlh - 21/02/2025
        // Sử dụng để cắt toàn bộ string sau 1 ký tự nhất định.
        public string sliceStringAfterACharacter(string originalString, string character)
        {
            string input = originalString;
            int index = input.IndexOf(character);
            if (index >= 0)
            {
                input = input.Substring(0, index);
            }
            return input;
        }


    }
}