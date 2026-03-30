using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.DAO.Entities.Models;

public partial class DmUser
{
    public Guid Oid { get; set; }

    public string? Dienthoai { get; set; }

    public string? TenUser { get; set; }

    public string? MaUser { get; set; }

    public string? MaDonvi { get; set; }
    [NotMapped]
    public string? TenDonvi { get; set; }

    public string? Mail { get; set; }

    public int? LoaiUser { get; set; }

    public bool? IsActive { get; set; }

    //public string Password { get; set; } = null!;

    public string LoaiCbo { get; set; } = null!;

    public bool? PhanQuyen { get; set; }

    public DateTime? NgayCnhat { get; set; }

    public string? MaDonviPquyen { get; set; } = null!;

    public string? MaUserCapnhat { get; set; } = null!;

    public string? MaUserPias { get; set; } = null!;

    public bool? IsActiveGddk { get; set; }

    public bool? PquyenUplHinhAnh { get; set; }

    public bool? IsGdvHotro { get; set; }

    public bool? IsActiveGqkn { get; set; }

    public bool? IsactiveChkc { get; set; }

    public bool? IsActiveKytt { get; set; }

  
}
