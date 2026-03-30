using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class HsgdTotrinhXml
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public string PathXml { get; set; } = null!;

    public string TenFile { get; set; } = null!;
}
