using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.DAO.Entities.Models;

public partial class DmKhuvuc
{
    public int PrKey { get; set; }

    public string? MaKv { get; set; } = null!;

    public string? TenKv { get; set; } = null!;

    public string? Tinhtp { get; set; } = null!;
    [NotMapped]
    public string? TenTinhtp { get; set; } = null!;

    public string? QuanHuyen { get; set; } = null!;
    [NotMapped]
    public string? TenQuanHuyen { get; set; } = null!;

    public string? MotaDiadiem { get; set; } = null!;

    public string? MaDonvi { get; set; } = null!;
    [NotMapped]
    public string? TenDonvi { get; set; } = null!;

    public bool? SuDung { get; set; }

    public DateTime? NgayTao { get; set; }

    public string? MaUser { get; set; } = null!;
}
