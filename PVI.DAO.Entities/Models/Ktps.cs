using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class Ktps
{
    public decimal PrKey { get; set; }

    public string MaDonvi { get; set; } = null!;

    public string MaCtu { get; set; } = null!;

    public string SoCtu { get; set; } = null!;

    public DateTime? NgayCtu { get; set; }

    public DateTime? NgayHt { get; set; }

    public string NguoiGdich { get; set; } = null!;

    public string DonVi { get; set; } = null!;

    public string DienGiai { get; set; } = null!;

    public string MaTte { get; set; } = null!;

    public decimal TygiaHt { get; set; }

    public decimal TygiaTt { get; set; }

    public string SoTknh { get; set; } = null!;

    public string TenTknh { get; set; } = null!;

    public string MaUser { get; set; } = null!;

    public DateTime? NgayCnhat { get; set; }

    public string NhangCode { get; set; } = null!;

    public DateTime? VersionEdit { get; set; }

    public bool LapUnc { get; set; }

    public string MaUserUnc { get; set; } = null!;

    public DateTime? NgayCnhatUnc { get; set; }

    public bool KhongDgcltg { get; set; }
}
