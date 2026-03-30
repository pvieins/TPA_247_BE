using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.DAO.Entities.Models;

public partial class LichTrucgdv
{

    [Key]
    public int PrKey { get; set; }

    public int FrKey { get; set; }

    public string MaKv { get; set; } = null!;

    public string MaGara { get; set; } = null!;

    public string TenGara { get; set; } = null!;

    public string Thu { get; set; } = null!;

    public string SangChieu { get; set; } = null!;

    public string Thoigian { get; set; } = null!;

    public DateTime? NgayTao { get; set; }

    public DateTime? NgayBo { get; set; }

    public DateTime? NgayCapnhat { get; set; }

    public int SuDung { get; set; }

    public string MaUser { get; set; } = null!;

    public string TenUser { get; set; } = null!;

}


