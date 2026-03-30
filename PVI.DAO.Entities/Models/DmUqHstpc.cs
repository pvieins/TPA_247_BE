using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.DAO.Entities.Models;

public partial class DmUqHstpc
{
    public int PrKey { get; set; }

    public string? MaDonvi { get; set; } = null!;
    [NotMapped]
    public string? TenDonvi { get; set; } = null!;

    public decimal? GhSotienUq { get; set; } = null!;

    public DateTime? NgayHl { get; set; }

    public DateTime? NgayCapnhat { get; set; }

    public string? MaUserUq { get; set; } = null!;
    [NotMapped]
    public string? TenUserUq { get; set; } = null!;

    public string? LoaiUyquyen { get; set; } = null!;
}
