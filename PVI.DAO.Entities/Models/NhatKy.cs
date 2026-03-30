using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.DAO.Entities.Models;

public partial class NhatKy
{
    public int PrKey { get; set; }

    public int FrKey { get; set; }

    public string MaTtrangGd { get; set; } = null!;

    public string TenTtrangGd { get; set; } = null!;

    public string? GhiChu { get; set; }

    public DateTime NgayCapnhat { get; set; }

    public Guid? MaUser { get; set; }
    [NotMapped]
    public string? TenUser { get; set; }
    [NotMapped]
    public string? TenNguoiPheDuyet { get; set; }
    //public virtual HsgdCtu FrKeyNavigation { get; set; } = null!;
}