using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class DmLhsbt
{
    public string MaLhsbt { get; set; } = null!;

    public string TenLhsbt { get; set; } = null!;

    public DateTime? NgayCnhat { get; set; }

    public string MaUser { get; set; } = null!;
}
