using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class LsuDangnhap
{
    public int PrKey { get; set; }

    public string Username { get; set; } = null!;

    public DateTime ThoiGian { get; set; }

    public string ThaoTac { get; set; } = null!;

    public bool Mobile { get; set; }

    public int FrKey { get; set; }

    public string MaDonvi { get; set; } = null!;
}
