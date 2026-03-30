using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.DAO.Entities.Models;

public class HsgdTtrinhAll
{
    public List<HsgdTtrinhCt> hsgdTtrinhCt { get; set; }
    public List<HsgdTtrinhTt> hsgdTtrinhTt { get; set; }
}
public partial class HsgdTtrinhCt
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public string MaSp { get; set; } = null!;
    public string MaDKhoan { get; set; } = "";

    public decimal SotienBh { get; set; }

    public decimal SotienBt { get; set; }

    public decimal SotienTu { get; set; }

    public string TinhToanbt { get; set; } = null!;
    public int MucVat { get; set; }

    public decimal SoTienBtVat { get; set; }
    [NotMapped]
    public decimal PrKeyXml { get; set; }
    [NotMapped]
    public string? TenFile { get; set; } = null!;
    [NotMapped]
    public string? FileData { get; set; } = null!;
    [NotMapped]
    public string? PathXml { get; set; } = null!;
}


