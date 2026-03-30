using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVI.Helper
{
    public class ListAddNewParameters : QueryStringParameters
    {
        public string? SoSeriSearch { get; set; }
        public string? BienKiemSoatSearch { get; set; }
        public string? SoKhungSearch { get; set; }
        public string? NgayCuoiSearch { get; set; }

    }
   
}
