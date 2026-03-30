using AutoMapper;
using PVI.DAO.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;


namespace PVI.Service.Request
{
    // 3 Request khác nhau phục vụ 3 mục đích khác nhau.
    public class KhuVucFilter
    {

        public string? MaKv { get; set; } = null!;

        public string? TenKv { get; set; } = null!;

        public string? TenTinhtp { get; set; } = null!;

        public string? TenQuanHuyen { get; set; } = null!;

        public string? TenDonvi { get; set; } = null!;

        public bool? SuDung { get; set; }
    }

    public class KhuVucCreate
    {

        public string? MaKv { get; set; } = null!;

        public string? TenKv { get; set; } = null!;

        public string? Tinhtp { get; set; } = null!;

        public string? QuanHuyen { get; set; } = null!;

        public bool? SuDung { get; set; }

    }

    public class KhuVucUpdate
    {
        public string? TenKv { get; set; } = null!;

        public string? Tinhtp { get; set; } = null!;

        public string? QuanHuyen { get; set; } = null!;

        public bool? SuDung { get; set; }

    }



}