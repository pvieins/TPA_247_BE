using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class HsgdTtrinh
{
    public decimal PrKey { get; set; }

    public string MaDonvi { get; set; } = null!;

    public string SoHsbt { get; set; } = null!;

    public string TenDttt { get; set; } = null!;

    public string NgGdich { get; set; } = null!;

    public DateTime? NgayCtu { get; set; }

    public DateTime? NgayTthat { get; set; }

    public decimal SoTien { get; set; }

    public string MaTtrang { get; set; } = null!;

    public string PathTtrinh { get; set; } = null!;

    public string PrKeyCt { get; set; } = null!;

    public string NguyenNhan { get; set; } = null!;

    public string HauQua { get; set; } = null!;

    public string TaisanThuhoi { get; set; } = null!;

    public string PanThoiTs { get; set; } = null!;

    public string GtrinhChikhac { get; set; } = null!;

    public decimal GiatriThuhoi { get; set; }

    public decimal ChiKhac { get; set; }

    public decimal PrKeyHsgd { get; set; }

    public decimal SoNgchet { get; set; }

    public decimal SoBthuong { get; set; }

    public bool ThamGia007 { get; set; }

    public string NgayThuphi { get; set; } = null!;

    public bool? ChkDaydu { get; set; }

    public bool? ChkDunghan { get; set; }
    public bool ChkTheohopdong { get; set; }

    public decimal SoPhibh { get; set; }

    public bool ChkChuanopphi { get; set; }

    public Guid Oid { get; set; }
    public DateTime? NgayDuTlieu { get; set; }
    public DateTime? NgayTtoan { get; set; }
   
}
