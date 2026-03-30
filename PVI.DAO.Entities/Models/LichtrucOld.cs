using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class LichtrucOld
{
    public int PrKey { get; set; }

    public string SoHsbt { get; set; } = null!;

    public string SoHdgcn { get; set; } = null!;

    public string SoThe { get; set; } = null!;

    public string SoTheOld { get; set; } = null!;

    public string TenNdbh { get; set; } = null!;

    public string TenNguoithuhuong { get; set; } = null!;

    public string SoTaikhoan { get; set; } = null!;

    public string SoTkcheck { get; set; } = null!;

    public string TenNthCheck { get; set; } = null!;

    public decimal SoTienp { get; set; }

    public string NganHang { get; set; } = null!;

    public string TentatNganhang { get; set; } = null!;

    public string TtrangPhi { get; set; } = null!;

    public string GhiChuphi { get; set; } = null!;

    public bool TaikhoanDung { get; set; }

    public bool TrangthaiCheck { get; set; }

    public string GhiChu { get; set; } = null!;

    public DateTime? NgayLay { get; set; }

    public string Cptt { get; set; } = null!;

    public string TenCanbo { get; set; } = null!;
}
