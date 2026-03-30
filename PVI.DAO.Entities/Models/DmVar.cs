using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.DAO.Entities.Models;

public partial class DmVar
{
    public decimal PrKey { get; set; }

    public string MaDonvi { get; set; } = null!;
    [NotMapped]
    public string DonViThanhToan { get; set; } = null!;

    public string Bien { get; set; } = null!;

    public string GhiChu { get; set; } = null!;

    public string GiaTri { get; set; } = null!;

    public string GiaTriEng { get; set; } = null!;

    public bool TongHop { get; set; }

    public string MaUser { get; set; } = null!;

    public DateTime? NgayCnhat { get; set; }

    public bool Khoa { get; set; }
}
