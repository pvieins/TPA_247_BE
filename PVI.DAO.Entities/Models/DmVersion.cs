using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class DmVersion
{
    public decimal PrKey { get; set; }

    public string Logic { get; set; } = null!;

    public string Version { get; set; } = null!;

    public string PathUrl { get; set; } = null!;

    public bool Active { get; set; }

    public bool Type { get; set; }

    public string PathUrl1 { get; set; } = null!;

    public string AppName { get; set; } = null!;
}
