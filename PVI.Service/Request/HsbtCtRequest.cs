using AutoMapper;
using PVI.DAO.Entities.Models;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace PVI.Service.Request;

public  class HsbtCtRequest
{
    public HsbtCtDetailRequest hsbtCt { get; set; }
    public HsgdDxCtRequest hsgdDxCt { get; set; }
    public decimal PrKeyHsgdCtu { get; set; }
    public List<FileAttachBtRequest> fileAttachBts { get; set; }
}
public  class HsbtCtDetailRequest
{
    public decimal PrKey { get; set; }
    public decimal FrKey { get; set; }
    public string MaSp { get; set; } = null!;
    public string MaDkhoan { get; set; } = null!;
    public decimal MtnGtbh { get; set; }
    public string MaTteGoc { get; set; } = null!;
    public string MaTtrangBt { get; set; } = null!;
    public string MaTtebt { get; set; } = null!;

    public decimal TygiaBt { get; set; }

    public decimal NguyenTep { get; set; }

    public decimal SoTienp { get; set; }

    public int MucVatp { get; set; }

    public decimal NguyenTevp { get; set; }

    public decimal SoTienvp { get; set; }

    public decimal SoTienkt { get; set; }

    public decimal TyleReten { get; set; }

    public decimal MtnRetenNte { get; set; }

    public decimal MtnRetenVnd { get; set; }

    public DateTime? NgayHtoanBt { get; set; }
    //Thông tin chi tiết VAT
    public string MauSovat { get; set; } = null!;

    public string SerieVat { get; set; } = null!;

    public string SoHdvat { get; set; } = null!;

    public DateTime? NgayHdvat { get; set; }

    public string MaKhvat { get; set; } = null!;

    public string TenKhvat { get; set; } = null!;

    public string MasoVat { get; set; } = null!;

    public string TenHhoavat { get; set; } = null!;
}
public  class HsgdDxCtRequest
{
    public decimal PrKey { get; set; }//TH update là PrKeyHsgdDxCt trong GetListPhaiTraBT

    public decimal PrKeyHsbtCt { get; set; }//TH update là PrKey trong GetListPhaiTraBT

    public int HieuXe { get; set; }

    public int LoaiXe { get; set; }

    public string XuatXu { get; set; } = null!;

    public int NamSx { get; set; }

    public decimal SoTienctkh { get; set; }

    public decimal TyleggPhutungvcx { get; set; }

    public decimal TyleggSuachuavcx { get; set; }

    public decimal SoTienGtbt { get; set; }
    public string LydoCtkh { get; set; } = null!;

    public string GhiChudx { get; set; } = null!;
    //public int VatTnds { get; set; }

    //public int Vat { get; set; }
    public string MaGara { get; set; } = null!;

    public string MaGara01 { get; set; } = null!;

    public string MaGara02 { get; set; } = null!;
    public decimal PrKeyHsbtCtu { get; set; }

    public string MaSp { get; set; } = null!;

    public string MaDkhoan { get; set; } = null!;

    public int? HieuXeTndsBen3 { get; set; }

    public int? LoaiXeTndsBen3 { get; set; }

    public string? DonviSuachuaTsk { get; set; }
    public string DoituongttTnds { get; set; } = null!;
    public int ChkKhongHoadon { get; set; }
    public string MaLoaiDongco { get; set; } = null!;

    public decimal SotienTtpin { get; set; }

}
public class HsbtUocRequest
{
    public decimal HsbtCtuPrKey { get; set; }
    public decimal HsbtCtPrkey { get; set; }
    public decimal NguyenTep { get; set; }

    public decimal SoTienp { get; set; }

    public decimal MtnRetenNte { get; set; }
    public decimal MtnRetenVnd { get; set; }
    public decimal TyleReten { get; set; }
}
public partial class FileAttachBtRequest
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public string FileName { get; set; } = null!;
    public string Directory { get; set; } = null!;
    public string FileData { get; set; } = null!;
    public string FileExtension { get; set; } = null!;
}