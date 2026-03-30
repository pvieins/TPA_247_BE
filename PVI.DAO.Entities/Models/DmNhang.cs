using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVI.DAO.Entities.Models
{
    public partial class DmNhang
    {
        public string? MaNhang { get; set; } = null!;

        public string? TenNhang { get; set; } = null!;

        public string? SoTkNhang { get; set; } = null!;

        public string? MaDonviNhang { get; set; } = null!;

        public string? MaTteNhang { get; set; } = null!;

        public string? LoaiTaiKhoan { get; set; } = null!;

        public bool? TrangThai { get; set; }

        public string? TenTaiKhoan { get; set; } = null!;

    }
}
