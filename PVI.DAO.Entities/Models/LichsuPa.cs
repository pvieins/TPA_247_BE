using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class LichsuPa
{
    public int PrKey { get; set; }

    public string TenGara { get; set; } = null!;

    public string? GiaPhutung { get; set; }

    public string? GiaPheduyet { get; set; }

    public string? NgayTt { get; set; }

    public string? NgayPd { get; set; }

    public string TenTinh { get; set; } = null!;

    public string QuanHuyen { get; set; } = null!;

    public string GhiChudv { get; set; } = null!;

    public string TenHmuc { get; set; } = null!;

    public string MaDonvi { get; set; } = null!;

    public string? TenDonvi { get; set; }

    public int LoaiXe { get; set; }

    public string? TenloaiXe { get; set; }

    public string XuatXu { get; set; } = null!;

    public int NamSx { get; set; }

    public string HsgdTpc { get; set; } = null!;

    public string SoHsgd { get; set; } = null!;
}
