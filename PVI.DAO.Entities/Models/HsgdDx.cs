using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class HsgdDx
{
    public int PrKey { get; set; }

    public int FrKey { get; set; }

    public string MaHmuc { get; set; } = null!;

    public decimal SoTientt { get; set; }

    public decimal SoTienph { get; set; }

    public decimal SoTienson { get; set; }

    public decimal SoTienpdtt { get; set; }

    public decimal SoTienpdsc { get; set; }

    public string GhiChudv { get; set; } = null!;

    public string GhiChutt { get; set; } = null!;

    public int LoaiDx { get; set; }

    public DateTime? NgayCapnhat { get; set; }

    public DateTime? GetDate { get; set; }

    public int VatSc { get; set; }

    public int GiamTruBt { get; set; }

    public bool ThuHoiTs { get; set; }

    public decimal? SoTienDoitru { get; set; }

    public decimal? SoTienpdDoitru { get; set; }

    public decimal PrKeyDx { get; set; }
    public string Hmuc { get; set; } = null!;
}
