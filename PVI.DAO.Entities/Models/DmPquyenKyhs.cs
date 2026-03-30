using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.DAO.Entities.Models;

public partial class DmPquyenKyhs
{
    public Guid PrKey { get; set; }

    [Key]
    // Do hiện tại bảng phân quyền ký đang không có khóa chính nên không thể map; tạm thời lấy mã user làm khóa chính.
    public required string MaUser { get; set; }

    public string MaSp { get; set; } = null!;

    public decimal? SoTien { get; set; } = null!;

    public bool? IsActive { get; set; } = null!;

    public DateTime? NgayCnhat { get; set; } = null!;

    public string MaUserCapnhat { get; set; } = null!;


    // Chỉnh sửa model, add thêm 4 trường dưới đây để trả thông tin phù hợp  hợp với bảng ký số. Không map vào DB.
    // khanhlh - 23/08/2024
    [NotMapped]
    public string TenUser { get; set; } = null!;
    [NotMapped]
    public string Mail { get; set; } = null!;
    [NotMapped]
    public string MaUserPias { get; set; } = null!;
    [NotMapped]
    public string TenDonVi { get; set; } = null!;
    [NotMapped]
    public int Count { get; set; }
}
