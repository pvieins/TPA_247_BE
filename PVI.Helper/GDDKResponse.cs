using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVI.Helper
{
    public class GDDKResponse
    {
        public decimal PrKey { get; set; }
        public decimal SoSeri {  get; set; }
        public string? BienKsoat { get; set; }
        public string? SoDonBh {  get; set; }
        public string? SoKhung { get; set; }
        public string? NgayCtu { get; set; }
        public string? TenNgTao { get; set; }
        public string TenDonVi { get; set; }
        public List<AnhGDDKData> AnhGDDKData { get; set; }
    }


    public class AnhGDDKData
    {
        public string? ViTri { get; set; }
        public string? Thumbnail {  get; set; }
        public string? PathUrl { get; set; }
        public decimal PrKey { get; set; }
        public string? PathFile { get; set; }
    }
}
