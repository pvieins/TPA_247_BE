using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVI.Helper
{
    public class HsgdCtuResponse
    {
        public int Nam { get; set; }
        public int Thang { get; set; }
        public int? SoNgaybh { get; set; }
        public int PrKey { get; set; }
        public decimal SoLanBt { get; set; }
        public string SoHsgd { get; set; }
        public DateTime? NgayCtu { get; set; }
        public string TenKhach { get; set; }
        public decimal SoSeri { get; set; }
        public string BienKsoat { get; set; }
        public DateTime? NgayTthat { get; set; }
        public string TenUser { get; set; }
        public string MaUser { get; set; }
        public string MaDonvi { get; set; }
        public string TenDonvi { get; set; }
        public string TenTtrangGd { get; set; }
        public string? TenLhsbt { get; set; }
        public decimal SoTienUocbt { get; set; }
        public decimal TienPheduyet { get; set; }
        public int HieuXe { get; set; }
        public int LoaiXe { get; set; }
        public int NamSx { get; set; }
        public string XuatXu { get; set; }
        public decimal SoTienugd { get; set; }
        public string NguoiXuly { get; set; }
        public string? NguoiGiao { get; set; }
        public decimal SoTienugddx { get; set; }
        public string MaDonviGd {  get; set; }
        public string SoDonBh { get; set; }
        public string MaTtrangGd { get; set; }
        public decimal PrKeyBt { get; set; }
        public int HsgdTpc { get; set; }
        public string MaDonviTt { get; set; }
        
        //public string NguoiDuyet { get; set; }
        //public string MaNguoiDuyet { get; set; }
        //public decimal PrKeyHsgdLsu { get; set; }
        //public string MaNguoiXuLy { get; set; }
    }
}
