using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class NhatKyPias
{
    public decimal PrKey { get; set; }

    public string PrKeyCtu { get; set; } = null!;

    public string MaDonvi { get; set; } = null!;

    public decimal MaBomb { get; set; }

    public decimal LanCnhat { get; set; }

    public string PhanHe { get; set; } = null!;

    public string SoCtu { get; set; } = null!;

    public DateTime? NgayCtu { get; set; }

    public string SuKien { get; set; } = null!;

    public DateTime? NgayCnhat { get; set; }

    public string TenUser { get; set; } = null!;

    public string TenMay { get; set; } = null!;

    public string PhienBan { get; set; } = null!;

    public string MaCty { get; set; } = null!;

    public decimal Id { get; set; }
}
