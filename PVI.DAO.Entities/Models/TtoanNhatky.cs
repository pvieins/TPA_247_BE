using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class TtoanNhatky
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public int OrderId { get; set; }

    public string UserChuyen { get; set; } = null!;

    public string UserNhan { get; set; } = null!;

    public string GhiChu { get; set; } = null!;

    public decimal KyTtoan { get; set; }

    public string TrangThai { get; set; } = null!;

    public DateTime? NgayCnhat1 { get; set; }

    public DateTime? NgayCnhat { get; set; }

    public string MaUserKtoan { get; set; } = null!;

    public string PathCtuKy { get; set; } = null!;

    public string PathUncKy { get; set; } = null!;
}
