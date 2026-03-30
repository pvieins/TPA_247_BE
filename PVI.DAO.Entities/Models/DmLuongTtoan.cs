using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class DmLuongTtoan
{
    public int PrKey { get; set; }

    public string MaDonvi { get; set; } = null!;

    public string TenLuongTtoan { get; set; } = null!;

    public string LuongXly { get; set; } = null!;

    public string LoaiCphi { get; set; } = null!;

    public DateTime? NgayHluc { get; set; }

    public bool IsUse { get; set; }

    public string LuongKy { get; set; } = null!;
}
