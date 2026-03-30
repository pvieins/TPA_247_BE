using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class DmSp
{
    public string MaSp { get; set; } = null!;

    public string TenSp { get; set; } = null!;

    public int MucVat { get; set; }

    public int DkienTaituc { get; set; }

    public string MaNsp { get; set; } = null!;

    public string MaNsp1 { get; set; } = null!;

    public string MaNsp2 { get; set; } = null!;

    public bool TongHop { get; set; }

    public string MaUser { get; set; } = null!;

    public DateTime NgayCnhat { get; set; }

    public string TenSpTa { get; set; } = null!;

    public string TenTat { get; set; } = null!;

    public string MaSpOld { get; set; } = null!;

    public string NhomDieutri { get; set; } = null!;

    public string TkCovathd { get; set; } = null!;

    public bool KhongThue { get; set; }

    public string MaSpTagetik { get; set; } = null!;

    public string MaTagetik { get; set; } = null!;

    public string MaHdi { get; set; } = null!;
}
