using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class HsbtCt
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public string MaSp { get; set; } = null!;

    public string MaDkhoan { get; set; } = null!;

    public decimal NguyenTepu { get; set; }

    public decimal SoTienpu { get; set; }

    public int MucVatu { get; set; }

    public decimal NguyenTevu { get; set; }

    public decimal SoTienvu { get; set; }

    public decimal NguyenTekt { get; set; }

    public decimal SoTienkt { get; set; }

    public decimal NguyenTep { get; set; }

    public decimal SoTienp { get; set; }

    public int MucVatp { get; set; }

    public decimal NguyenTevp { get; set; }

    public decimal SoTienvp { get; set; }

    public decimal MtnGtbh { get; set; }

    public decimal MucktHoi { get; set; }

    public string MaTtebt { get; set; } = null!;

    public decimal TygiaBt { get; set; }

    public string MaTtrangBt { get; set; } = null!;

    public DateTime? NgayHtoanBt { get; set; }

    public decimal PrKeyBttCt { get; set; }

    public decimal TyleReten { get; set; }

    public decimal MtnRetenNte { get; set; }

    public decimal MtnRetenVnd { get; set; }

    public string GhiChuBt { get; set; } = null!;

    public decimal MtnGtbhVnd { get; set; }

    public string MaTteGoc { get; set; } = null!;

    public string MaQuyenloi { get; set; } = null!;

    public decimal? PrKeyCare { get; set; }

    public string NamNvu { get; set; } = null!;

    public string MaIcd { get; set; } = null!;

    public decimal NguyenTebtGoc { get; set; }

    public decimal SoTienbtGoc { get; set; }

    public decimal PrKeyBthCt { get; set; }

    public DateTime? NgayTamung { get; set; }

    public string MauSovat { get; set; } = null!;

    public string SerieVat { get; set; } = null!;

    public string SoHdvat { get; set; } = null!;

    public DateTime? NgayHdvat { get; set; }

    public string MaKhvat { get; set; } = null!;

    public string TenKhvat { get; set; } = null!;

    public string MasoVat { get; set; } = null!;

    public string TenHhoavat { get; set; } = null!;

    public decimal PrKeyNvuBhtCt { get; set; }

    public decimal ChenhLechKtru { get; set; }

    public string? LoaiPhiPi { get; set; }

    public bool TinhTay { get; set; }

    public decimal NguyenTeYcbt { get; set; }

    public decimal SoTienYcbt { get; set; }

    public decimal SoTienTc { get; set; }

    public decimal? PrKeyCareNoitru { get; set; }

    public decimal PrKeyKbttHsbtCt { get; set; }
}
