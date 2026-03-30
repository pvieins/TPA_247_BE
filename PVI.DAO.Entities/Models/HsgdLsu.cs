using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class HsgdLsu
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public string? MaUserChuyen { get; set; } = null!;

    public string? MaUserNhan { get; set; } = null!;

    public DateTime NgayCnhat { get; set; }

    public string GhiChu { get; set; } = null!;
}
