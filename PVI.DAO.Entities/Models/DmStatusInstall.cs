using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class DmStatusInstall
{
    public decimal PrKey { get; set; }

    public string ImeiDevice { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? NgayCnhat { get; set; }
}
