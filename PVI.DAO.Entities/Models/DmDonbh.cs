using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class DmDonbh
{
    public string MaDonbh { get; set; } = null!;

    public string TenDonbh { get; set; } = null!;

    public string AnChi { get; set; } = null!;

    public string MaSp { get; set; } = null!;

    public string MaUser { get; set; } = null!;

    public DateTime NgayCnhat { get; set; }

    public string TenDonbhTa { get; set; } = null!;

    public string MaHieu { get; set; } = null!;

    public string MaDkhoanBs { get; set; } = null!;
}
