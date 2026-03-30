using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVI.DAO.Entities.Models
{
    public partial class HsgdTtrinhTt
    {
        public int PrKey { get; set; }
        public decimal? FrKey { get; set; }
        public string? TenChuTk { get; set; }
        public string? SoTaikhoanNh { get; set; }
        public string? TenNh { get; set; }
        public decimal? SotienTt { get; set; }
        public string? LydoTt { get; set; }        
        public string bnkCode { get; set; } = string.Empty;
    }
}
