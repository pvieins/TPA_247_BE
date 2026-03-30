using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVI.Helper
{
    public class HsgdCtuParameters : QueryStringParameters
    {
        public string? SoHsgd {  get; set; }
        public string? SoDonBh { get; set; }
        public decimal? SoSeri { get; set; }
        public string? MaTtrangGd { get; set; }
        public string? BienKsoat {  get; set; } 
        public string? TenDonVi {  get; set; }
        public string? TenKhach { get; set; }
        public string? NgayTthat { get; set; }
        public decimal? SoTienugd { get; set; }
        public string? TenLhsbt { get; set; }
        public string? NguoiXuly { get; set; }
        public string? NgayCtu { get; set; }
        public bool ChuaSangPias { get; set; }
        public int Year { get; set; }
        public string? MaGDV { get; set; }
        public string? MaNguoiDuyet { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
    }
    public class BiaHS
    {
        public decimal PrKey { get; set; }
        public string? SoHsgd { get; set; } = null!;
        public string? SoSeri { get; set; }
        public string? NgayDauSeri { get; set; }

        public string? NgayCuoiSeri { get; set; }
        public string? TenDonvi { get; set; } = null!;
        public string? TenDonviTToan { get; set; } = null!;
        public string? BienKsoat { get; set; } = null!;
        public string? MaSP { get; set; } = null!;
        public string? SoTienThucTe { get; set; }
        public string? PviBl { get; set; }
        public string? TenGara { get; set; }
        public string? GiamDinhVien { get; set; }
        public string? TenKhach { get; set; } = null!;//Tên NĐBH
        public string? NguyenNhanTtat { get; set; } = null!;
        public string? NgayTthat { get; set; }
        public string? DiaDiemtt { get; set; } = null!;
        public string? SoHsbt { get; set; } = null!;
    }
    public class DNTTRequest
    {

        public string ngay_ctu { get; set; } = null!;

        public string ma_cbcnv { get; set; } = null!;
        public string ma_donvi { get; set; } = null!;
        public string ma_cbcnv_xly { get; set; } = null!;

        public string loai_cpi { get; set; } = null!;

        //public string ma_user { get; set; } = null!;

        //public string ten_cbcnv_xly { get; set; } = null!;

        public string dien_giai { get; set; } = null!;

        public string? ttin_lquan { get; set; } = null!;

        //public decimal tong_tien { get; set; }

        public string ma_httoan { get; set; } = null!;

        public string nguoi_huong { get; set; } = null!;

        public string? ten_tknh { get; set; } = null!;

        public string? so_tknh { get; set; }
        public string? bnkCode { get; set; }
    }
}
