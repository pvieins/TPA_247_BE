using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.DAO.Entities.Models;

public partial class DmDiemtruc
{
    public int PrKey { get; set; }

    public string MaDiemtruc { get; set; } = null!;

    public string TenDiemtruc { get; set; } = null!;

    public bool? Active { get; set; }

    public string Description { get; set; } = null!;

    public string MaUser { get; set; } = null!;

    public DateTime? NgayCnhat { get; set; }

    [NotMapped]
    public int Count { get; set; } // Đếm tổng số lượng record trong DB.
}
