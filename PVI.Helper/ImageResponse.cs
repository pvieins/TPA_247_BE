using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVI.Helper
{
    public class ImageResponse
    {
        public int PrKey { get; set; }           
        public int FrKey { get; set; }           
        public int Stt { get; set; }             
        public int NhomAnh { get; set; }      
        public string? PathFile { get; set; }   
        public DateTime? NgayChup { get; set; } 
        public string? ViDoChup { get; set; }     
        public string? KinhDoChup { get; set; }   
        public string? DienGiai { get; set; }    
        public string? PathUrl { get; set; }      
        public string? PathOrginalFile { get; set; } 
        public string? MaHmuc { get; set; }
        public string? TenHmuc { get; set; }
        public string? TenHmucSc {  get; set; }
        public string? MaTongThanhXe {  get; set; }
        public string? MaNHmuc { get; set; }
        public string? MaHmucSc { get; set; }
    }
}
