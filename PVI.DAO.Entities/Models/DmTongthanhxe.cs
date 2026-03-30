using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.DAO.Entities.Models;

public partial class DmTongthanhxe
{
    public string? MaTongthanhxe { get; set; } = null!;

    public string? TenTongthanhxe { get; set; } = null!;

    public DateTime? NgayCapnhat { get; set; }

    public string? MaUser { get; set; } = null!;

    [NotMapped]
    public string? TenUser { get; set; } = null!;
    [NotMapped]
    public List<DmNhmuc> DanhSachNhmuc { get; set; } = null!;
}
