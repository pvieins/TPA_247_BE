using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class FileAttachBt
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public string? MaCtu { get; set; }

    public string FileName { get; set; } = null!;

    public string Directory { get; set; } = null!;

    public string TrichYeu { get; set; } = null!;

    public DateTime? NgayCtu { get; set; }

    public string KyHieu { get; set; } = null!;
}
