using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.DAO.Entities.Models;

    public class DmDkbh
    {
        public string MaDkbh { get; set; } = null!;
        public string TenDkbh { get; set; } = null!;
        public string TenDkbhTa { get; set; } = null!;
        public string MaQtac { get; set; } = null!;
        public string MaDonvi { get; set; } = null!;
        public bool TongHop { get; set; }
        public int Cat { get; set; }
        public DateTime? NgayCnhat { get; set; }
        public string MaUser { get; set; } = null!;
        public bool KhongSdung { get; set; }
        public DateTime? NgayHluc { get; set; }
        public decimal TyleTor { get; set; }
    }

