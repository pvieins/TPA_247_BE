using AutoMapper;
using PVI.DAO.Entities.Models;
using PVI.Helper;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;


namespace PVI.Service.Request
{
    public class GaraRequest
    {
        public DmGaraRequest Gara { get; set; }
        public string TkVnd { get; set; } = null!;
        public string NganHang { get; set; } = null!;
        
    }

    public class DmGaraRequest
    {
        public string MaGara { get; set; } = null!;

        public string DiaChiXuong { get; set; } = null!;

        public string TenTat { get; set; } = null!;

        public decimal? TyleggPhutung { get; set; }

        public decimal? TyleggSuachua { get; set; }

        public string MaUsercNhat { get; set; } = null!;

        public string EmailGara { get; set; } = null!;

        public string DienThoaiGara { get; set; } = null!;
        public int SongayThanhtoan { get; set; } = 0!;
        public string bnkCode { get; set; } = null!;
        public string ten_ctk { get; set; } = null!;

        public bool? thoa_thuan_hop_tac { get; set; }
    }



}
