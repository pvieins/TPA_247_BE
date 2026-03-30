using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class DmHmucGiamdinh
{
    public string MaHmuc { get; set; } = null!;

    public string TenHmuc { get; set; } = null!;

    public string MaUser { get; set; } = null!;

    public DateTime? NgayCapnhat { get; set; }
}
