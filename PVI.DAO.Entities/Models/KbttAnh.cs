using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models
{
    public partial class KbttAnh
    {
        public int PrKey { get; set; }
        public int FrKey { get; set; }
        public DateTime? NgayChup { get; set; }
        public DateTime? NgayUpload { get; set; }
        public string Path { get; set; } = null!;
        public string PathThumnail { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string UrlThumnail { get; set; } = null!;
        public string KinhDo { get; set; } = null!;
        public string ViDo { get; set; } = null!;
        public int LoaiAnh { get; set; }
        public int LoaiBh { get; set; }
        public int? BhcnCtuId { get; set; }
        public string MaHmuc { get; set; } = null!;
        public string NguonTao { get; set; } = null!;
    }
}
