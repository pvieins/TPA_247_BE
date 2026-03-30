using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class HsbtUoc
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public DateTime? NgayPs { get; set; }

    public decimal NguyenTebt { get; set; }

    public decimal SoTienbt { get; set; }

    public decimal MucVat { get; set; }

    public decimal NguyenTev { get; set; }

    public decimal SoTienv { get; set; }

    public decimal TyleReten { get; set; }

    public decimal NguyenTebtReten { get; set; }

    public decimal SoTienbtReten { get; set; }

    public string GhiChu { get; set; } = null!;

    public decimal NguyenTebtPvi { get; set; }

    public decimal SoTienbtPvi { get; set; }

    public string MaTtrangUoc { get; set; } = null!;

    public string MaUser { get; set; } = null!;

    public string LoaiPhiUpi { get; set; } = null!;
}
