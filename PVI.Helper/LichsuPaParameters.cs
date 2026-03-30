using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVI.Helper
{
    public class LichsuPaParameters : QueryStringParameters
    {
        public decimal pr_key_hsgd_dx_ct {  get; set; }
        public string ten_hmuc { get; set; } = null!;
        public bool loai_xe { get; set; }
        public bool xuat_xu { get; set; }
        public bool nam_sx {  get; set; } 
    }
}
