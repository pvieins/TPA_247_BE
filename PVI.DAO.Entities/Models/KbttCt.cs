using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models
{
    public partial class KbttCt
    {
        public int PrKey { get; set; }
        public int FrKey { get; set; }
        public string MaNhmuc { get; set; } = null!;
        public string MaHmuc { get; set; } = null!;
        public string TenHmuc { get; set; } = null!;
        public int Stt { get; set; }
        public DateTime? NgayTao { get; set; }
    }
}
