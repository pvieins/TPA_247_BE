using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class HsgdDxCt
{
    public decimal PrKey { get; set; }

    public decimal PrKeyHsbtCt { get; set; }

    public int HieuXe { get; set; }

    public int LoaiXe { get; set; }

    public string XuatXu { get; set; } = null!;

    public int NamSx { get; set; }

    public string MaGara { get; set; } = null!;

    public string MaGara01 { get; set; } = null!;

    public string MaGara02 { get; set; } = null!;

    public decimal SoTienctkh { get; set; }

    public decimal TyleggPhutungvcx { get; set; }

    public decimal TyleggSuachuavcx { get; set; }

    public int VatTnds { get; set; }

    public int Vat { get; set; }

    public string LydoCtkh { get; set; } = null!;

    public string GhiChudx { get; set; } = null!;

    public string DoituongttTnds { get; set; } = null!;

    public decimal SoTienGtbt { get; set; }

    public decimal PrKeyHsbtCtu { get; set; }

    public string MaSp { get; set; } = null!;

    public int HieuXeTndsBen3 { get; set; }

    public int LoaiXeTndsBen3 { get; set; }

    public string DonviSuachuaTsk { get; set; } = null!;

    public string MaDkhoan { get; set; } = null!;

    public int ChkKhonghoadon { get; set; }

    public string PathPasc { get; set; } = null!;

    public int Bl1 { get; set; }

    public int Bl2 { get; set; }

    public int Bl3 { get; set; }

    public int Bl4 { get; set; }

    public int Bl5 { get; set; }

    public int Bl6 { get; set; }

    public int Bl7 { get; set; }

    public int Bl8 { get; set; }

    public int Bl9 { get; set; }

    public string BlTailieubs { get; set; } = null!;

    public string BlDsemail { get; set; } = null!;

    public string BlDsphone { get; set; } = null!;

    public int BlSendEmail { get; set; }

    public int BlPdbl { get; set; }

    public string MaDonviTt { get; set; } = null!;

    public string? GhiChu { get; set; }

    public string PathBaolanh { get; set; } = null!;

    public int PascSendEmail { get; set; }

    public string MaLoaiDongco { get; set; } = null!;

    public decimal SotienTtpin { get; set; }
}
