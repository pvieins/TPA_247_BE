using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class DmTyGia
{
    public decimal PrKey { get; set; }

    public string MaTTe { get; set; } = null!;

    public DateTime? NgayHluc { get; set; }

    public decimal Tygia { get; set; }

    public string? LoaiHT { get; set; }

    public string? MaUser { get; set; } 
    public DateTime? NgayCapNhat { get; set; }
}
