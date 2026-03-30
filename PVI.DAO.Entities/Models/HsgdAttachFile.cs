using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.DAO.Entities.Models
{
    public partial class HsgdAttachFile
    {
        public string? PrKey { get; set; }
        public decimal FrKey { get; set; }
        public string? MaCtu { get; set; }
        public string FileName { get; set; } = null!;
        public string Directory { get; set; } = null!;
        public DateTime? ngay_cnhat { get; set; }
        public string GhiChu { get; set; } = null!;
        public string NguonTao { get; set; } = null!;
        [NotMapped]
        public string? PathUrl { get; set; }

    }
    public class GetListFileResponse
    {
        public List<HsgdAttachFile> Files { get; set; } = new List<HsgdAttachFile>();
        public bool? HoanThienHstt { get; set; }
    }
}
