using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.DAO.Entities.Models;

public partial class DmHmuc
{
    public string? MaHmuc { get; set; } = null!;
    public string? TenHmuc { get; set; } = null!;
    public string? MaTongthanhxe { get; set; } = null!;
    [NotMapped]
    public string? TenTongThanhXe { get; set; } = null!;
    public string? MaNhmuc { get; set; } = null!;
    [NotMapped]
    public string? TenNhmuc { get; set; } = null!;

    public int? SuDung { get; set; } = null!;

    public DateTime? NgayCapnhat { get; set; }

    public string? MaUser { get; set; } = null!;
    [NotMapped]
    public string? TenUser { get; set; } = null!;
    [NotMapped]
    public decimal PrKeyHsgd { get; set; }
    [NotMapped]
    public int stt { get; set; }
}
