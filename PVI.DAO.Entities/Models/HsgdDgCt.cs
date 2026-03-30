using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class HsgdDgCt
{
    public int PrKey { get; set; }

    public string PathFile { get; set; } = null!;

    public string PathUrl { get; set; } = null!;

    public int FrKey { get; set; }

    public string PathOrginalFile { get; set; } = null!;

    public virtual HsgdDg FrKeyNavigation { get; set; } = null!;
}
