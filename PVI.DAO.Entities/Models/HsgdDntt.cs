using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class HsgdDntt
{
    public decimal PrKey { get; set; }

    public decimal PrKeyTtoanCtu { get; set; }

    public decimal PrKeyTtrinh { get; set; }
    public decimal PrKeyTtrinhCt { get; set; }

    public string MaCbo { get; set; }
    public string MaCbcnvXly { get; set; }
    public string SoCtu { get; set; }
}
