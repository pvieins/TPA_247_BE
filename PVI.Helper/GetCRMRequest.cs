using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVI.Helper
{
    public class GetCRMRequest
    {
        public string PathCrm { get; set; }
        public string SoHsgd { get; set; }
        public DateTime NgayTthat { get; set; }
        public DateTime NgayTbao { get; set; }
        public string SoDonbh { get; set; }
        public string SoSeri { get; set; }
        public int MaTtrangGd { get; set; }
        public int PrKey { get; set; }
    }
}
