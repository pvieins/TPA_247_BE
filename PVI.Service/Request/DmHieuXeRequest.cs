using AutoMapper;
using PVI.DAO.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;


namespace PVI.Service.Request
{
    public class HieuXeRequest
    {
        public string? Hieuxe { get; set; } = null!;
    }

    public class LoaiXeFilter
    {
        public string? Hieuxe { get; set; } = null!;

        public string? LoaiXe { get; set; } = null!;

        public string? TenUser { get; set; } = null!;

        public DateTime? NgayCapnhat { get; set; } = null!;
    }

    public class LoaiXeRequest
    {
        public int? PrKeyHieuXe { get; set; }

        public string? LoaiXe { get; set; } = null!;
    }

}