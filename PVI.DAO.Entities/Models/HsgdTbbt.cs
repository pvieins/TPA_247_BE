using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PVI.DAO.Entities.Models
{
   public partial class HsgdTbbt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal PrKey { get; set; }
        public decimal PrKeyHsgd { get; set; }
        public int PdTbbt { get; set; }
        public string? DsEmail { get; set; }
        public decimal TndsXeCoGioi { get; set; } = 0;
        public decimal TndsHangHoa { get; set; } = 0;
        public decimal TndsTaiNanHk { get; set; } = 0;
        public decimal TndsTaiSanKhac { get; set; } = 0;
        public decimal TndsNguoi { get; set; } = 0;
        public int SoNgayTtoan { get; set; } = 0;
        public string? PathTbbt { get; set; }       
        public string? GhiChu { get; set; }
        public int SendTbbt { get; set; }
        public string? MaDonviTT { get; set; }

    }
}
