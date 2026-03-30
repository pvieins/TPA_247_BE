using AutoMapper;
using PVI.DAO.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;


namespace PVI.Service.Request
{
    public class GaraKhuVucFilter
    {

        public string? MaKv { get; set; } = null!;

        public string? TenKv { get; set; } = null!;

        public int? Stt { get; set; }

        public string? MaGara { get; set; } = null!;

        public string? TenGara { get; set; } = null!;

        public string? TenDonvi { get; set; } = null!;

        public bool? SuDung { get; set; } = null!;

    }

    public class GaraKhuVucRequest
    {
        public int? Stt { get; set; }
        public string? MaKv { get; set; } = null!;

        public string? MaGara { get; set; } = null!;

        public bool? SuDung { get; set; } = null!;
    }

}