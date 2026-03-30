using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class DmListPhone
{
    public decimal PrKey { get; set; }

    public string MaUser { get; set; } = null!;

    public string Phone { get; set; } = null!;
}
