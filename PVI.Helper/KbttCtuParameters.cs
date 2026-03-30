using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVI.Helper
{
    public class KbttCtuParameters : QueryStringParameters
    {
        public string? SoSeriSearch {  get; set; }
        public string? BienKiemSoatSearch { get; set; }
        
        public string? NgayCuoiSearch { get; set; }
        public List<int>? TrangThaiSearch {  get; set; }
        public string? NguoiKTaoSearch { get; set; }
        public string? NguoiKbSearch { get; set; }
        public string? NgayKbSearch { get; set; }
        public string? TenDonViSearch { get; set; }
        public string? SoDonBhSearch { get; set; }
        public string? NgayDauSearch { get; set; }
        public int LoaiKbttSearch { get; set; }
        public string? NgayGDinhSearch { get; set; }
        //public string? HotlineSearch {  get; set; }
        public string? NguyenNhanSearch { get; set; }
       
    }
}
