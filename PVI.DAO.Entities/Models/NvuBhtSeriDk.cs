using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class NvuBhtSeriDk
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public string MaDkhoanBs { get; set; } = null!;

    public string TenDkhoanBs { get; set; } = null!;
}
