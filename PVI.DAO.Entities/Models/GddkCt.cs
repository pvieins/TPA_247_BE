using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class GddkCt
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public DateTime? NgayChup { get; set; }

    public string ViDoChup { get; set; } = null!;

    public string KinhDoChup { get; set; } = null!;

    public string PathFile { get; set; } = null!;

    public string PathOrginalFile { get; set; } = null!;

    public string PathUrl { get; set; } = null!;
}
