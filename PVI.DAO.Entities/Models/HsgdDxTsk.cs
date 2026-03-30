using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class HsgdDxTsk
{
    public int PrKey { get; set; }

    public int FrKey { get; set; }

    public string Hmuc { get; set; } = null!;

    public decimal SoTientt { get; set; }

    public decimal SoTiensc { get; set; }

    public decimal SoTienpdtt { get; set; }

    public decimal SoTienpdsc { get; set; }

    public string GhiChudv { get; set; } = null!;

    public string GhiChutt { get; set; } = null!;

    public DateTime? NgayCapnhat { get; set; }

    public DateTime? GetDate { get; set; }

    public int VatSc { get; set; }

    public int GiamTruBt { get; set; }

    public bool ThuHoiTs { get; set; }

    public decimal PrKeyDx { get; set; }
}
