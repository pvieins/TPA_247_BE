using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class PquyenCnang
{
    public int PrKey { get; set; }

    public string MaUser { get; set; } = null!;

    public string TenUser { get; set; } = null!;

    public string MaDonvi { get; set; } = null!;

    public string LoaiQuyen { get; set; } = null!;

    public int TrangThai { get; set; }

    public string MaDonviPquyen { get; set; } = null!;

    public string MaUserCap { get; set; } = null!;

    public DateTime? NgayCap { get; set; }

    public string GhiChu { get; set; } = null!;
}
