using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.DAO.Entities.Models;

public partial class DmNhmuc
{
    public string? MaNhmuc { get; set; } = null!;

    public string? MaTongthanhxe { get; set; } = null!;

    [NotMapped]
    public string? TenTongThanhXe { get; set; } = null!;

    public string? TenNhmuc { get; set; } = null!;

    public int? SuDung { get; set; } = null!;

    public DateTime? NgayCapnhat { get; set; }

    public string? MaUser { get; set; } = null!;

    [NotMapped]
    public string? TenUser { get; set; } = null!;
    [NotMapped]
    public List<DmHmuc> DanhSachHmuc { get; set; } = null!;
}
