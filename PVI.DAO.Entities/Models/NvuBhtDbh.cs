using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class NvuBhtDbh
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public string SoHdong { get; set; } = null!;

    public string MaKhach { get; set; } = null!;

    public string VaiTro { get; set; } = null!;

    public string MaPatt { get; set; } = null!;

    public decimal TyleTg { get; set; }

    public decimal TyleTaiho { get; set; }

    public decimal TyleCapdon { get; set; }

    public decimal TyleMoigioi { get; set; }

    public decimal NguyenTep { get; set; }

    public decimal SoTienp { get; set; }

    public int MucVat { get; set; }

    public decimal NguyenTev { get; set; }

    public decimal TienVat { get; set; }

    public decimal TyleHhong { get; set; }
}
