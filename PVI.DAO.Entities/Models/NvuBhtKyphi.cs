using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class NvuBhtKyphi
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public int Stt { get; set; }

    public DateTime? NgayHl { get; set; }

    public decimal TylePhithu { get; set; }

    public decimal SoTien { get; set; }

    public bool TraPhi { get; set; }

    public bool KeToan { get; set; }

    public decimal PrKeyKt { get; set; }

    public decimal PrKeyDbh { get; set; }

    public string NamNvu { get; set; } = null!;
}
