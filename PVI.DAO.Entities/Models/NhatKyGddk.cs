using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class NhatKyGddk
{
    public int PrKey { get; set; }

    public int FrKey { get; set; }

    public Guid MaUser { get; set; }

    public DateTime ThoiGian { get; set; }

    public string ThaoTac { get; set; } = null!;
}
