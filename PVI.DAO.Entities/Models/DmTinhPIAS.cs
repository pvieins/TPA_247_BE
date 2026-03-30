using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class DmTinhPIAS
{
    public string MaTinh { get; set; } = null!;

    public string TenTinh { get; set; } = null!;

    public bool DongBang { get; set; }

    public string WindStorm { get; set; } = null!;

    public string Flood { get; set; } = null!;

    public DateTime? NgayCapNhat { get; set; } = null!;

    public string MaUser { get; set; } = null!;

}
