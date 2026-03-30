using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class DmKhach
{
    public string MaKh { get; set; } = null!;

    public string MaNkhtcty { get; set; } = null!;

    public string MaKhMoi { get; set; } = null!;

    public string MaDonvi { get; set; } = null!;

    public string TenKh { get; set; } = null!;

    public string MaNhkh { get; set; } = null!;

    public string MaPban { get; set; } = null!;

    /// <summary>
    /// Ma don vi phuc vu cho viec theo doi phan chia doanh thu
    /// </summary>
    public string MaDonviPban { get; set; } = null!;

    public string TenKhanh { get; set; } = null!;

    public string TenTat { get; set; } = null!;

    public string MasoVat { get; set; } = null!;

    public string DiaChi { get; set; } = null!;

    public string DiaChiEng { get; set; } = null!;

    public string Tel { get; set; } = null!;

    public string Fax { get; set; } = null!;

    public string SoCmnd { get; set; } = null!;

    public DateTime? NgayCap { get; set; }

    public string NoiCap { get; set; } = null!;

    public string NganHang { get; set; } = null!;

    public string TkVnd { get; set; } = null!;

    public string TkUsd { get; set; } = null!;

    public string GiamDoc { get; set; } = null!;

    public bool DaiLy { get; set; }

    public bool PhongBan { get; set; }

    public bool Thue { get; set; }

    public bool CanBo { get; set; }

    public bool DoiTru { get; set; }

    public string PathDvi { get; set; } = null!;

    public bool ViewAll { get; set; }

    public bool VpKv { get; set; }

    public bool Gara { get; set; }

    public string MaTinh { get; set; } = null!;

    public bool? GaraTthai { get; set; }

    public string MaUser { get; set; } = null!;

    public DateTime NgayCnhat { get; set; }

    public string Email { get; set; } = null!;

    public bool ToChuc { get; set; }

    public bool KhongSdung { get; set; }

    public bool MoiGioiTbh { get; set; }

    public string MaTctdHn { get; set; } = null!;

    public string ToaDoGara { get; set; } = null!;

    public bool LienquanPvi { get; set; }

    public bool GiamDinh { get; set; }

    public DateTime? NgayThanhlap { get; set; }
}
