using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class DmCtukt
{
    public string MaCtukt { get; set; } = null!;

    public string TenCtukt { get; set; } = null!;

    public string TenCtuktIn { get; set; } = null!;

    public decimal Num { get; set; }

    public string MaNhctukt { get; set; } = null!;

    public string TkNo { get; set; } = null!;

    public string TkCo { get; set; } = null!;

    public string TkTrung { get; set; } = null!;

    public bool NgoaiTe { get; set; }

    public string MaUser { get; set; } = null!;

    public DateTime? NgayCnhat { get; set; }

    public decimal Stt { get; set; }

    public string MaDonvi { get; set; } = null!;

    public int MaDviInt { get; set; }

    public decimal NumDt { get; set; }

    public string MaNhang { get; set; } = null!;
}
