using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class HsgdDnttView
{
    public decimal PrKey { get; set; }
    public string SoCtu { get; set; } = null!;
    public string MaCtuTtoan { get; set; } = null!;
    public string MaDonVi { get; set; } = null!;
    public DateTime? NgayCtu { get; set; } = null!;
    public string? NgayCtuText { get; set; } = null!;
    public string MaCbcnv { get; set; } = null!;// mã người đề nghị
    public string TenCbcnv { get; set; } = null!;//tên người đề nghị
    public string MaCbcnvXly { get; set; } = null!;//mã người xử lý
    public string TenCbcnvXly { get; set; } = null!;//tên người xử lý
    public string MaPban { get; set; } = null!;
    public string TenPban { get; set; } = null!;
    public string NguoiHuong { get; set; } = null!;
    public string DienGiai { get; set; } = null!;
    public decimal TongTien { get; set; }
    public string MaTte { get; set; } = null!;
    public string TrangThai { get; set; } = null!;//trạng thái phê duyệt
    public string TenTrangThai { get; set; } = null!;//trạng thái phê duyệt
    public bool BsCtu { get; set; }// tình trạng đề nghị thanh toán
    public bool IsCtien { get; set; }// tình trạng chuyển tiền
    public string MaHttoan { get; set; } = null!;// hình thức thanh toán
    public string TenHttoan { get; set; } = null!;// hình thức thanh toán
    public string LoaiCphi { get; set; } = null!; //loại chi phí
    public string TenLoaiCphi { get; set; } = null!; //loại chi phí
}
public partial class LichSuPheDuyet
{
    public int OrderId { get; set; }
    public string TrangThai { get; set; } = null!;
    public DateTime? NgayCnhat { get; set; }
    public string UserNhan { get; set; } = null!;
    public string TenUser { get; set; } = null!;
    public string TenUserNhan { get; set; } = null!;
    public string GhiChu { get; set; } = null!;
}