using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class DanhMuc
{
    public string? MaDM { get; set; } = null!;
    public string? TenDM { get; set; } = null!;
}
public partial class DanhMucTinh
{
    public string? MaDM { get; set; } = null!;
    public string? TenDM { get; set; } = null!;
    public int? SuDung { get; set; } = null!;
}
public partial class NguoiDeNghi
{
    public string? MaUser { get; set; } = null!;
    public string? FullName { get; set; } = null!;
    public string? MaCbo { get; set; } = null!;
    public string DcEmail { get; set; } = null!;
}
public partial class TtrangGdCount
{
    public string MaTtrangGd { get; set; } = null!;

    public string TenTtrangGd { get; set; } = null!;
    public int SoHsgd { get; set; }
}
public partial class TtrinhCount
{
    public int sl_gdvchoduyet { get; set; }
    public int sl_tpchoduyet { get; set; }
    public int sl_ldchoduyet { get; set; }
    public int sl_choduyetttoan { get; set; }
    public int sl_dahuy { get; set; }


}

public class TtrinhLDCount
{
    public int sl_daduyet { get; set; }
    public int sl_npc { get; set; }
    public int sl_choduyet { get; set; }


}
public partial class KHVat
{
    public string? MaDM { get; set; } = null!;
    public string? TenDM { get; set; } = null!;
    public string? MasoVat { get; set; } = null!;
}