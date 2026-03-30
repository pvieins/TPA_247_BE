using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVI.Service.Request
{
    public class UpdateDuyetGiaRequest
    {
        public int PrKey {  get; set; }
        public bool LoaiDg { get; set; }
        public string DeXuat { get; set; }
        public decimal SoTien { get; set; }
    }
}
