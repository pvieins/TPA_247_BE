using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.DAO.Entities.Models;

public partial class DmDevice
{
    public decimal PrKey { get; set; }

    public string ImeiDevice { get; set; } = null!;

    public string AddressDevice { get; set; } = null!;

    public string MaUser { get; set; } = null!;

    public DateTime? NgayCnhat { get; set; }

    public string MaDonvi { get; set; } = null!;

    [NotMapped]
    public string TenDonvi { get; set; } = null!;

    public bool? Active { get; set; }

    public string Description { get; set; } = null!;

    public bool Status { get; set; }

    public string? TypeDevice { get; set; }

    [NotMapped]
    public int Count { get; set; }

}
