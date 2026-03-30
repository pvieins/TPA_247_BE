using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class TtoanCt
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public decimal DoanhSo { get; set; }

    public decimal TsuatVat { get; set; }

    public decimal TienVat { get; set; }

    public string? MaSovat { get; set; }

    public string MaKhVat { get; set; } = null!;

    public string? TenKhVat { get; set; }

    public string SerieVat { get; set; } = null!;

    public string SoHdvat { get; set; } = null!;

    public DateTime? NgayHdvat { get; set; }

    public string TenHhoa { get; set; } = null!;

    public string MauSovat { get; set; } = null!;

    public string GhiChu { get; set; } = null!;

    public string? TenFile { get; set; }

    public string? DuongDan { get; set; }

    public string KichCo { get; set; } = null!;

    public decimal DoanhSoHdon { get; set; }

    public decimal TienVatHdon { get; set; }

    public string MaHdong { get; set; } = null!;

    public string HdongJson { get; set; } = null!;

    public bool IsXml { get; set; }

    public string IsValid { get; set; } = null!;

    public string MsgValid { get; set; } = null!;

    public string? MsgValid2 { get; set; }
}
