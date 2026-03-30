using AutoMapper;
using PVI.DAO.Entities.Models;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace PVI.Service.Request;

public class TtrinhRequest
{
    public HsgdTtrinhRequest hsgdTtrinh { get; set; }
    public List<HsgdTtrinhCtRequest> hsgdTtrinhCt { get; set; }
    public List<HsgdTtrinhTt> hsgdTtrinhTt { get; set; }
}
public class HsgdTtrinhRequest
{
    public decimal PrKey { get; set; }

    public string? MaDonvi { get; set; } = null!;

    public string? SoHsbt { get; set; } = null!;

    public string? TenDttt { get; set; } = null!;

    public string? NgGdich { get; set; } = null!;

    public DateTime? NgayCtu { get; set; }

    public DateTime? NgayTthat { get; set; }

    public decimal? SoTien { get; set; }

    public string? MaTtrang { get; set; } = null!;

    public string? PathTtrinh { get; set; } = null!;

    public string? PrKeyCt { get; set; } = null!;

    public string? NguyenNhan { get; set; } = null!;

    public string? HauQua { get; set; } = null!;

    public string? TaisanThuhoi { get; set; } = null!;

    public string? PanThoiTs { get; set; } = null!;

    public string? GtrinhChikhac { get; set; } = null!;

    public decimal? GiatriThuhoi { get; set; }

    public decimal? ChiKhac { get; set; }

    public decimal PrKeyHsgd { get; set; }
    public decimal? SoNgchet { get; set; }

    public decimal? SoBthuong { get; set; }

    public bool? ThamGia007 { get; set; }

    public decimal? SoPhibh { get; set; }
    public string? NgayThuphi { get; set; } = null!;

    public bool? ChkDaydu { get; set; }

    public bool? ChkDunghan { get; set; }
    public bool? ChkChuanopphi { get; set; }
    public bool? ChkTheohopdong { get; set; }
    public DateTime? NgayDuTlieu { get; set; }
    public DateTime? NgayTtoan { get; set; }
}
public class HsgdTtrinhCtRequest
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public string? MaSp { get; set; } = null!;
    public string? MaDKhoan { get; set; } ="";
    
    public decimal? SotienBh { get; set; }

    public decimal? SotienBt { get; set; }

    public decimal? SotienTu { get; set; }

    public string? TinhToanbt { get; set; } = null!;
    public int MucVat { get; set; }

    public decimal SoTienBtVat { get; set; }

    public decimal PrKeyXml { get; set; }

    public string? TenFile { get; set; } = null!;
    public string? FileData { get; set; } = null!;

}
