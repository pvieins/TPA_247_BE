using AutoMapper;
using PVI.DAO.Entities.Models;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;


namespace PVI.Service.Request
{
    public class QuyenKySoRequest
    {
        public DmPquyenKyHsRequest QuyenKySo { get; set; }
    }

    public class DmPquyenKyHsRequest
    {
        //public Guid PrKey { get; set; }

        public string MaUser { get; set; } = null!;

        public string MaSp { get; set; } = null!;

        public decimal SoTien { get; set; }

        public bool IsActive { get; set; }

        //public DateTime? NgayCnhat { get; set; }

        public string MaUserCapnhat { get; set; } = null!;

        /*
        // Chỉnh sửa model, add thêm 4 trường dưới đây để trả thông tin phù hợp  hợp với bảng ký số 
        // khanhlh - 23/08/2024
        public string TenUser { get; set; } = null!;

        public string Mail { get; set; } = null!;

        public string MaUserPias { get; set; } = null!;

        public string Donvi { get; set; } = null!;
        */
    }

    public class DmQuyenKyFilter
    {
        public string? maUser { get; set; } = null!;
        public string? tenUser { get; set; } = null!; 
        public string? mail { get; set; } = null!;
        public string? maSp { get; set; } = null!;
        public decimal? SoTien { get; set; } = null!;
        public string? TenDonvi { get; set; } = null!;
        public bool? isActive { get; set; } = null!;
        public string? maUserPias { get; set; } = null!;
        public DateTime? ngayCnhat { get; set; } = null!;

    }

}
