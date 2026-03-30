using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class GhichuLichtruc
{
    public int PrKey { get; set; }

    public int FrKey { get; set; }

    public string MaUser { get; set; } = null!;

    public string TenUser { get; set; } = null!;

    public string DienThoai { get; set; } = null!;

    public string GhiChu { get; set; } = null!;

    public bool? SuDung { get; set; }
}
