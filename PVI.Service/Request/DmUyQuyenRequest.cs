using AutoMapper;
using PVI.DAO.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;


namespace PVI.Service.Request
{
    public class UyQuyenFilter
    {
        public string? TenDonvi { get; set; } = null!;

        public decimal? GhSotienUq { get; set; } = null!;

        public DateTime? NgayHl { get; set; }

        public string? TenUserUq { get; set; } = null!;

        public string? LoaiUyquyen { get; set; } = null!;
    }

    public class UyQuyenRequest
    {
        public string? MaDonvi { get; set; } = null!;

        public decimal? GhSotienUq { get; set; } = null!;

        public DateTime? NgayHl { get; set; }

        public string? MaUserUq { get; set; } = null!;

        public string? LoaiUyquyen { get; set; } = null!;
    }


}