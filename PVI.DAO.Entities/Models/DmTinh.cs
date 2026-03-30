using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class DmTinh
{
    public string MaTinh { get; set; } = null!;

    public string TenTinh { get; set; } = null!;

    public int TongHop { get; set; }
    public int SuDung { get; set; }
}
