using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVI.Helper
{
    public partial class ImageKbttResponse
    {
        public int PrKey { get; set; }
        public int FrKey { get; set; }
        public int Stt { get; set; }
        public string? ViDo { get; set; }
        public string? KinhDo { get; set; }
        public string? LoaiKbtt { get; set; }
        public string? PathUrl { get; set; }
       
        public string? MaHmuc { get; set; }
        public string? TenHmuc { get; set; }
        public string? PathFile { get; set; }
        public DateTime? NgayChup {  get; set; }
        
        
    }
}
