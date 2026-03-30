using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.DAO.Entities.Models;

// User của PIAS để dùng cho việc trực tiếp kéo thông tin từ PIAS vào 247.
public partial class DmUserPias
{
    public string MaUser { get; set; } = null!;

    public string TenUser { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string MaNhom { get; set; } = null!;

    public string MaDonvi { get; set; } = null!;

    public string MaPhong { get; set; } = null!;

    public bool PhanQuyen { get; set; }

    public string? MaBenhvien { get; set; }

    public bool? NguoidungBlvp { get; set; }

    public bool? QthtBlvp { get; set; }

    public string MaTthai { get; set; } = null!;

    public DateTime? NgayCnhat { get; set; }

    public string MaTthaiTtoan { get; set; } = null!;

    public DateTime? NgayKtao { get; set; }

    public DateTime? NgayCuoi { get; set; }

    public bool? TrangThai { get; set; }

    public string GhiChu { get; set; } = null!;

    public string MaNhang { get; set; } = null!;

    public string DcEmail { get; set; } = null!;

    public bool KichhoatEmail { get; set; }

    public string MaChucvu { get; set; } = null!;

    public string MaCbo { get; set; } = null!;

    public bool KyHddtQlcd { get; set; }

    public string ParentId { get; set; } = null!;

    public string OtpCode { get; set; } = null!;

    public bool OtpDisable { get; set; }

    public DateTime? LiveTime { get; set; }

    public int LiveNum { get; set; }

    public string LoaiKenh { get; set; } = null!;

    public bool? BanQuanly { get; set; }

    public int LoginFailNumber { get; set; }

    public bool ChkModonRi { get; set; }

    public bool ChkChuyendonRi { get; set; }

    public string PasswordSha256 { get; set; } = null!;

    public bool DuyetTaifile { get; set; }
}
