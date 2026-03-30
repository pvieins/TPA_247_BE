using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class CrmCase
{
    public int PrKey { get; set; }

    public string MaDonvi { get; set; } = null!;

    public string SoDonbh { get; set; } = null!;

    public int SoSeri { get; set; }

    public string BienKsoat { get; set; } = null!;

    public DateTime? NgayDauSeri { get; set; }

    public DateTime? NgayCuoiSeri { get; set; }

    public DateTime? NgayTthat { get; set; }

    public DateTime? NgayTbao { get; set; }

    public string DiaDiemtt { get; set; } = null!;

    public string NguyenNhanTtat { get; set; } = null!;

    public string HauQuaTt { get; set; } = null!;

    public string NgLienhe { get; set; } = null!;

    public string DienThoai { get; set; } = null!;

    public string GhiChu { get; set; } = null!;

    public DateTime? NgayTao { get; set; }

    public string NguoiTao { get; set; } = null!;

    public string DienthoaiNgtao { get; set; } = null!;

    public int TicketId { get; set; }

    public int PrKeyHsgd { get; set; }
}
