using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class GddkCtu
{
    public decimal PrKey { get; set; }

    public string MaDonvi { get; set; } = null!;

    public DateTime? NgayCtu { get; set; }

    public decimal SoSeri { get; set; }

    public string BienKsoat { get; set; } = null!;

    public string SoKhung { get; set; } = null!;

    public string SoDonbh { get; set; } = null!;

    public string MaUser { get; set; } = null!;

    public DateTime? NgayCapnhat { get; set; }

    public string GhiChu { get; set; } = null!;

    public string MaCtu { get; set; } = null!;

    public string SoDangky { get; set; } = null!;

    public decimal PrKeyNvu { get; set; }
}
