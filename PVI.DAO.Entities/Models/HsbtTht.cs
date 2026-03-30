using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class HsbtTht
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public string MaSp { get; set; } = null!;

    public string LoaiHinhtd { get; set; } = null!;

    public string MaTte { get; set; } = null!;

    public decimal TygiaTd { get; set; }

    public decimal NguyenTeTd { get; set; }

    public decimal SoTienTd { get; set; }

    public decimal NguyenTePvi { get; set; }

    public decimal SoTienPvi { get; set; }

    public string GhiChu { get; set; } = null!;

    public decimal TyleReten { get; set; }

    public decimal MtnRetenNte { get; set; }

    public decimal MtnRetenVnd { get; set; }

    public string MaTtrangTd { get; set; } = null!;

    public DateTime? NgayHtoanTd { get; set; }

    public decimal PrKeyBttCt { get; set; }

    public decimal NguyenTetdu { get; set; }

    public decimal SoTientdu { get; set; }

    public decimal FrKeyBk { get; set; }

    public decimal NguyenTedtd { get; set; }

    public decimal SoTiendtd { get; set; }

    public string MaQuyenloiThts { get; set; } = null!;

    public DateTime? NgayHtoanDtd { get; set; }

    public decimal NguyenTetdPvi { get; set; }

    public decimal SoTientdPvi { get; set; }

    public decimal PrKeyBthCt { get; set; }

    public decimal PrKeyNvuBhtCt { get; set; }

    public string LoaiPhiPi { get; set; } = null!;

    public bool TinhTay { get; set; }

    public decimal PrKeyKbttHsbtThts { get; set; }
}
