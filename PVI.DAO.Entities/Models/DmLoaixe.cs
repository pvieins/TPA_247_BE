using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.DAO.Entities.Models;

public partial class DmLoaixe
{
    public int? PrKey { get; set; }

    public int? FrKey { get; set; }
    [NotMapped]
    public string? Hieuxe { get; set; } = null!;

    public string? LoaiXe { get; set; } = null!;

    public DateTime? NgayCapnhat { get; set; } = null!;

    public string? MaUser { get; set; } = null!;
    [NotMapped]
    public string?  TenUser { get; set; } = null!;
}
