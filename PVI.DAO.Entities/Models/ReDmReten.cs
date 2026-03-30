using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class ReDmReten
{
    public string SoDonbh { get; set; } = null!;

    public string SoDonbhbs { get; set; } = null!;

    public string? SoDonbhRi { get; set; }

    public string MaSp { get; set; } = null!;

    public decimal? TyleReten { get; set; }

    public decimal? MtnRetenUsd { get; set; }

    public decimal? MtnRetenVnd { get; set; }

    public decimal PrKeyNvuBhtCt { get; set; }
}
