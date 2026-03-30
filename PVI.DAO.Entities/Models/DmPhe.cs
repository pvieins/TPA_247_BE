using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;


public partial class DmPhe
{
    public decimal PrKey { get; set; }

    public string MaPh { get; set; } = null!;

    public string TenPh { get; set; } = null!;

    public DateTime? RequiredVersion { get; set; }
}
