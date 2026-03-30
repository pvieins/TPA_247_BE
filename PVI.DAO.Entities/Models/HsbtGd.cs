using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class HsbtGd
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public string MaSp { get; set; } = null!;

    public string MaDvgd { get; set; } = null!;

    public string MaTtegd { get; set; } = null!;

    public decimal TygiaGd { get; set; }

    public decimal NguyenTegdu { get; set; }

    public decimal SoTiengdu { get; set; }

    public decimal MucVatgdu { get; set; }

    public decimal NguyenTevu { get; set; }

    public decimal SoTienvu { get; set; }

    public decimal NguyenTegd { get; set; }

    public decimal SoTiengd { get; set; }

    public decimal MucVat { get; set; }

    public decimal NguyenTev { get; set; }

    public decimal SoTienv { get; set; }

    public string MaTtrangGd { get; set; } = null!;

    public DateTime? NgayHtoanGd { get; set; }

    public decimal PrKeyBttCt { get; set; }

    public decimal TyleReten { get; set; }

    public decimal MtnRetenNte { get; set; }

    public decimal MtnRetenVnd { get; set; }

    public string GhiChuGd { get; set; } = null!;

    public bool AddnewEdit { get; set; }

    public string NamNvu { get; set; } = null!;

    public string MaLoaiChiphi { get; set; } = null!;

    public string MaIcd { get; set; } = null!;

    public decimal NguyenTegdPvi { get; set; }

    public decimal SoTiengdPvi { get; set; }

    public string MauSovat { get; set; } = null!;

    public string SerieVat { get; set; } = null!;

    public string SoHdvat { get; set; } = null!;

    public DateTime? NgayHdvat { get; set; }

    public string MaKhvat { get; set; } = null!;

    public string TenKhvat { get; set; } = null!;

    public string MasoVat { get; set; } = null!;

    public string TenHhoavat { get; set; } = null!;

    public decimal PrKeyBthCt { get; set; }

    public decimal PrKeyNvuBhtCt { get; set; }

    public string LoaiPhiPi { get; set; } = null!;

    public bool TinhTay { get; set; }

    public decimal PrKeyKbttHsbtGd { get; set; }
}
