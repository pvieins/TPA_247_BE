using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class DmCbcnv
{
    public string MaCbcnv { get; set; } = null!;

    public string TenCbcnv { get; set; } = null!;

    public string MaPban { get; set; } = null!;

    public bool ViewAll { get; set; }

    public string MaDonvi { get; set; } = null!;

    public bool KhongSdung { get; set; }
}
