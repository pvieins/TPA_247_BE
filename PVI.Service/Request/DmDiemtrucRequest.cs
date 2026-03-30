using AutoMapper;
using PVI.DAO.Entities.Models;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;


namespace PVI.Service.Request
{
    public class DiemtrucRequest
    {
        public DmDiemtrucRequest Diemtruc { get; set; }
    }

    
    public class DmDiemtrucRequest
    {
        //public int PrKey { get; set; }

        public string MaDiemtruc { get; set; } = null!;

        public string TenDiemtruc { get; set; } = null!;

        public bool? Active { get; set; }

        public string? Description { get; set; } = null!;

        public string MaUser { get; set; } = null!;

        //public DateTime? NgayCnhat { get; set; }
    }
    
    public class DmDiemtrucFilter
    {
        //public int PrKey { get; set; }

        public string? MaDiemtruc { get; set; } = null!;

        public string? TenDiemtruc { get; set; } = null!;

        public bool? Active { get; set; } = null!;

        public string? Description { get; set; } = null!;

        public string? MaUser { get; set; } = null!;

        public string? NgayCnhat { get; set; }
    }

}
