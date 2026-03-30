using AutoMapper;
using PVI.DAO.Entities.Models;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace PVI.Service.Request;

public  class HsbtGdRequest
{
    public decimal PrKey { get; set; }
    public decimal FrKey { get; set; }
    public string MaSp { get; set; } = null!;//mã sp
    public string MaDvgd { get; set; } = null!;//mã đơn vị GD
    public string MaLoaiChiphi { get; set; } = null!;//loại chi phí
    public string MaTtegd { get; set; } = null!;//mã t.tê
    public decimal TygiaGd { get; set; }//tỷ giá
    //Phải trả giám định
    public string MaTtrangGd { get; set; } = null!;//t.trạng gd
    public decimal NguyenTegd { get; set; }//Nguyên tệ
    public decimal SoTiengd { get; set; }//số tiền vnd
    public decimal MucVat { get; set; }// mức vat
    public decimal NguyenTev { get; set; }//nguyên tệ vat
    public decimal SoTienv { get; set; }//số tiền vat
    //Phần trách nhiệm của CTY  
    public decimal TyleReten { get; set; }//tỷ lệ (%)
    public decimal MtnRetenNte { get; set; }//nguyên tệ phần trách nhiệm của cty
    public decimal MtnRetenVnd { get; set; }//số tiền phần trách nhiệm của cty 
    public string GhiChuGd { get; set; } = null!;//nội dung thanh toán
    public DateTime? NgayHtoanGd { get; set; }//ngày duyệt GD 
    //Thông tin chi tiết VAT
    public string MauSovat { get; set; } = null!;

    public string SerieVat { get; set; } = null!;

    public string SoHdvat { get; set; } = null!;

    public DateTime? NgayHdvat { get; set; }

    public string MaKhvat { get; set; } = null!;

    public string TenKhvat { get; set; } = null!;

    public string MasoVat { get; set; } = null!;

    public string TenHhoavat { get; set; } = null!;
    public List<FileAttachBtRequest> fileAttachBts { get; set; }
    public decimal PrKeyHsgdCtu { get; set; }
}
public class HsbtUocGdRequest
{
    public decimal HsbtCtuPrKey { get; set; }
    public decimal HsbtGdPrkey { get; set; }
    public decimal NguyenTegd { get; set; }
    public decimal SoTiengd { get; set; }
    public decimal MtnRetenNte { get; set; }
    public decimal MtnRetenVnd { get; set; }
    public decimal TyleReten { get; set; }
}
