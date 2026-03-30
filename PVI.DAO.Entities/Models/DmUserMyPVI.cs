using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models
{
    public partial class DmUserMYPVI
    {
        public int Id { get; set; }
        public string MaUser { get; set; } = null!;
        public string TenUser { get; set; } = null!;
        public int TrangThai { get; set; }
        public int LoaiUser { get; set; }
        public string Password { get; set; } = null!;
        public string DienThoai { get; set; } = null!;
        public string? Email { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string TinhTp { get; set; } = null!;
        public string QuanHuyen { get; set; } = null!;
        public string? DiaChi { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayCapnhat { get; set; }
        public string Imei { get; set; } = null!;
        public string? TokenKey { get; set; }
        public DateTime? TimeLogin { get; set; }
        public DateTime? TimeLogout { get; set; }
        public string? MaGt { get; set; }
    }
}
