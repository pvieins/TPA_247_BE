using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;
public partial class HsbtUocGd
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public DateTime? NgayPs { get; set; }

    public decimal NguyenTegd { get; set; }

    public decimal SoTiengd { get; set; }

    public decimal MucVat { get; set; }

    public decimal NguyenTev { get; set; }

    public decimal SoTienv { get; set; }

    public decimal TyleReten { get; set; }

    public decimal NguyenTegdReten { get; set; }

    public decimal SoTiengdReten { get; set; }

    public string GhiChu { get; set; } = null!;

    public decimal NguyenTegdPvi { get; set; }

    public decimal SoTiengdPvi { get; set; }

    public string MaTtrangUoc { get; set; } = null!;

    public string MaUser { get; set; } = null!;

    public string LoaiPhiUpi { get; set; } = null!;
}
