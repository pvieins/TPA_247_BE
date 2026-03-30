using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.DAO.Entities.Models;

public partial class DmGaRa
{
    [Key]
    public string MaGara { get; set; } = null!;

    public string TenGara { get; set; } = null!;

    public string TenTat { get; set; } = null!;

    public string MaDonvi { get; set; } = null!;

    public string DiaChi { get; set; } = null!;

    public string DiaChiXuong { get; set; } = null!;

    public bool GaraTthai { get; set; }

    public string TenTinh { get; set; } = null!;

    public string QuanHuyen { get; set; } = null!;

    public decimal? TyleggPhutung { get; set; }

    public decimal? TyleggSuachua { get; set; }

    public DateTime? NgayCnhat { get; set; }

    public string MaUsercNhat { get; set; } = null!;

    public string EmailGara { get; set; } = null!;

    public string DienThoaiGara { get; set; } = null!;
    public int? SongayThanhtoan { get; set; }
    [NotMapped]
    public int Count { get; set; } // Đếm tổng số record
    [NotMapped]
    public string? MasoVat { get; set; } = null!;// lấy mã số VAT
    [NotMapped]
    public string? TkVnd { get; set; } = null!;// lấy tài khoản VND
    [NotMapped]
    public string? NganHang { get; set; } = null!;// lấy tên ngân hàng
    public string bnkCode { get; set; } = string.Empty;
    public string ten_ctk { get; set; } = string.Empty;
    public bool? thoa_thuan_hop_tac { get; set; }
}
