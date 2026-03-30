using AutoMapper;
using PVI.DAO.Entities.Models;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace PVI.Service.Request;

public  class HsbtThtsRequest
{
    public decimal PrKey { get; set; }
    public decimal FrKey { get; set; }
    public string MaSp { get; set; } = null!;//mã sp
    public string LoaiHinhtd { get; set; } = null!;//loại hình
    public string MaTte { get; set; } = null!;//mã tte
    public decimal TygiaTd { get; set; }//tỷ giá 
    //số tiền thu về
    public string MaTtrangTd { get; set; } = null!;//t.trang tđ
    public decimal NguyenTeTd { get; set; }// nguyên tệ thu đòi
    public decimal SoTienTd { get; set; }// số tiền thu đòi
    //Thu về thuộc TN Công ty
    public decimal TyleReten { get; set; }//tỷ lệ
    public decimal MtnRetenNte { get; set; }// nguyên tệ thuộc tn cty
    public decimal MtnRetenVnd { get; set; }//số tiền thuộc tn cty
    public string GhiChu { get; set; } = null!;//nội dung thanh toán
    public DateTime? NgayHtoanTd { get; set; }//KT nhập TĐ
    public decimal PrKeyHsgdCtu { get; set; }
}

