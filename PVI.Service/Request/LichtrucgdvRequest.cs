using AutoMapper;
using PVI.DAO.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;


namespace PVI.Service.Request
{
    public class LichtrucgdvFilter
    {
        public string? MaKv { get; set; } = null!;
    }

    public class ThemXoaCanBoTruc
    {
        public string maKv { get; set; }
        public string maGara { get; set; }

        public string thu { get; set; }

        public string sangChieu { get; set; }

        public string[]? maUserXoa { get; set; } = new string[0];

        public string[]? maUserDangTruc { get; set; } = new string[0];
    }

}
