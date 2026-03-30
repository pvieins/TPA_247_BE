using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class TtoanCtu
{
    public decimal PrKey { get; set; }

    public string MaCtuTtoan { get; set; } = null!;

    public DateTime? NgayCtu { get; set; }

    public string MaDonvi { get; set; } = null!;

    public string MaPban { get; set; } = null!;

    public string DonVi { get; set; } = null!;

    public string MaCbcnv { get; set; } = null!;

    public string NguoiGdich { get; set; } = null!;

    public string DienGiai { get; set; } = null!;

    public string MaTte { get; set; } = null!;

    public decimal TygiaHt { get; set; }

    public decimal TygiaTt { get; set; }

    public string MaHttoan { get; set; } = null!;

    public string NguoiHuong { get; set; } = null!;

    public string SoTknh { get; set; } = null!;

    public string TenTknh { get; set; } = null!;

    public string DiaChiNh { get; set; } = null!;

    public string TtinLquan { get; set; } = null!;

    public string CtuKtheo { get; set; } = null!;

    public string MaUser { get; set; } = null!;

    public DateTime? NgayCnhat { get; set; }

    public string NhangCode { get; set; } = null!;

    public decimal PrKeyKtps { get; set; }

    public string TrangThai { get; set; } = null!;

    public decimal TongTien { get; set; }

    public string SoCtu { get; set; } = null!;

    public decimal TongTienKvat { get; set; }

    public DateTime? HanTtoan { get; set; }

    public string TenNhangTg { get; set; } = null!;

    public string CodeNhangTg { get; set; } = null!;

    public string DiachiNhangTg { get; set; } = null!;

    public string DiachiNguoiTh { get; set; } = null!;

    public string MaCbcnvXly { get; set; } = null!;

    public string LoaiCphi { get; set; } = null!;

    public bool BsCtu { get; set; }

    public string MaUserKtoan { get; set; } = null!;

    public bool LapUnc { get; set; }

    public string DsTtrinh { get; set; } = null!;

    public string TenFile { get; set; } = null!;

    public string DuongDan { get; set; } = null!;

    public string LoaiTtoan { get; set; } = null!;

    public decimal PrKeyLuong { get; set; }

    public string TenBangLuong { get; set; } = null!;

    public string CancuDenghi { get; set; } = null!;

    public bool IsCtien { get; set; }

    public string UserCtien { get; set; } = null!;

    public int NamHt { get; set; }

    public int HthucCkhoan { get; set; }

    public bool IsCtienTheoDs { get; set; }
}
