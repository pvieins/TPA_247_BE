using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.DAO.Entities.Models;

public partial class DmGaraKhuvuc
{
    public int PrKey { get; set; }

    public int? Stt { get; set; }

    public string? MaGara { get; set; } = null!;

    public string? TenGara { get; set; } = null!;

    public string? MaKv { get; set; } = null!;

    public string? TenKv { get; set; } = null!;

    public string? MaDonvi { get; set; } = null!;
    [NotMapped]
    public string? TenDonvi { get; set; } = null!;

    public bool? SuDung { get; set; }

    public DateTime? NgayCapnhat { get; set; }

    public string? MaUser { get; set; } = null!;
}
