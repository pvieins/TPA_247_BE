using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class DmUserTtoan
{
    public decimal PrKey { get; set; }

    public string MaUser { get; set; } = null!;

    public string TenUser { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string MaDonvi { get; set; } = null!;

    public string DcEmail { get; set; } = null!;
}
