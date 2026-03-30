using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class TcdBhtSeri
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public string MaNlvl { get; set; } = null!;

    public decimal SoSeri { get; set; }

    public bool SeriSd { get; set; }

    public string TenKhach { get; set; } = null!;

    public string SoCmnd { get; set; } = null!;

    public string DiaChi { get; set; } = null!;

    public string BienKsoat { get; set; } = null!;

    public string NhanHieu { get; set; } = null!;

    public string MaLoaixe { get; set; } = null!;

    public string DungTich { get; set; } = null!;

    public string MauSon { get; set; } = null!;

    public string NamSx { get; set; } = null!;

    public string TrongTai { get; set; } = null!;

    public string SoCngoi { get; set; } = null!;

    public bool MucDsd { get; set; }

    public string MaLoaikh { get; set; } = null!;

    public string DienThoai { get; set; } = null!;

    public decimal? TongTien { get; set; }

    public DateTime? NgayCapSeri { get; set; }

    public DateTime? NgayDauSeri { get; set; }

    public DateTime? NgayCuoiSeri { get; set; }

    public string NhomKhach { get; set; } = null!;

    public string SoMay { get; set; } = null!;

    public string HuyenKhach { get; set; } = null!;

    public string TinhKhach { get; set; } = null!;

    public decimal DtichRuong { get; set; }

    public string DchiRuong { get; set; } = null!;

    public string TinhRuong { get; set; } = null!;

    public string HuyenRuong { get; set; } = null!;

    public string VuluaRuong { get; set; } = null!;

    public bool GiongLua { get; set; }

    public string SoThe { get; set; } = null!;

    public string SoKhung { get; set; } = null!;

    public string NgGdichTh { get; set; } = null!;

    public string DiaChiTh { get; set; } = null!;

    public decimal? NamSd { get; set; }

    public decimal SlNgbh { get; set; }

    public string DienGiai { get; set; } = null!;

    public bool ViPham { get; set; }

    public DateTime? NgaySinh { get; set; }

    public string DcEmail { get; set; } = null!;

    public string MaCtrinh { get; set; } = null!;

    public string NoiDiTc { get; set; } = null!;

    public string NoiDenTc { get; set; } = null!;

    public string MaDongxe { get; set; } = null!;

    public decimal SoTienbhVcx { get; set; }

    public decimal SoTienbhKhacVcx { get; set; }

    public decimal NguyenTepBtc { get; set; }

    public decimal PhiPviTcd { get; set; }

    public bool BlackList { get; set; }

    public bool TaiCdDongBh { get; set; }

    public bool PhanboDt { get; set; }

    public bool Moigioi { get; set; }

    public int Dongchinh { get; set; }

    public decimal SoTienTh { get; set; }

    public bool IsPhiBtc { get; set; }

    public string DsDkbs { get; set; } = null!;

    public bool IsPhiPviTcd { get; set; }

    public bool IsThamgiaDkbs { get; set; }

    public bool? HideBks { get; set; }

    public byte[]? SeriEncrypt { get; set; }

    public string TinhTrangXe { get; set; } = null!;

    public DateTime? NgayDky { get; set; }

    public string TinhTrangTgbh { get; set; } = null!;

    public decimal TylephiPvi { get; set; }

    public DateTime? NgayCuoiTt { get; set; }

    public int TuoiAnchi { get; set; }

    public string TcdNamSx { get; set; } = null!;

    public string TcdBienKsoat { get; set; } = null!;

    public bool BksDd { get; set; }
}
