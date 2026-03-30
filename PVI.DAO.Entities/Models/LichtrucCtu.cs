using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class LichtrucCtu
{
    public int PrKey { get; set; }

    public string MaDonvi { get; set; } = null!;

    public string MaKv { get; set; } = null!;

    public DateTime? TuNgay { get; set; }

    public DateTime? DenNgay { get; set; }

    public string GhiChu { get; set; } = null!;

    public DateTime? NgayTao { get; set; }
}
