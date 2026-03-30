using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class DmGhHstpc
{
    public string MaDonvi { get; set; } = null!;

    public decimal GhSotientpc { get; set; }

    public DateTime? NgayHl { get; set; }

    public DateTime? NgayCapnhat { get; set; }

    public string MaUser { get; set; } = null!;
}
