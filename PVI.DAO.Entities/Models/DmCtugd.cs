using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class DmCtugd
{
    public int PrKey { get; set; }

    public string MaCtugd { get; set; } = null!;

    public string MaDonvi { get; set; } = null!;

    public decimal Num { get; set; }
}
