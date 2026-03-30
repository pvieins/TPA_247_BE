using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class HsgdTtrinhNky
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public string UserChuyen { get; set; } = null!;

    public string UserNhan { get; set; } = null!;

    public string GhiChu { get; set; } = null!;

    public DateTime? NgayCnhat { get; set; }
    public string Act { get; set; } = null!;
}
