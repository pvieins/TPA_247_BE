using AutoMapper;
using PVI.DAO.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;


namespace PVI.Service.Request
{

    public class TongThanhXeFilter
    {
        public string? MaTongThanhxe { get; set; } = null!;
        public string? TenTongthanhxe { get; set; } = null!;

        public int? SuDung { get; set; } = null!;

        public string? TenUser { get; set; } = null!;

        public DateTime? NgayCapnhat { get; set; }
    }

    public class DmNHmucFilter
    {
        public string? MaNhmuc { get; set; } = null!;

        public string? TenNhmuc { get; set; } = null!;

        public string? TenTongthanhxe { get; set; } = null!;

        public int? SuDung { get; set; } = null!;

        public string? TenUser { get; set; } = null!;

        public DateTime? NgayCapnhat { get; set; }
    }

    public class DmHmucFilter
    {
        public string? MaHmuc { get; set; } = null!;
        public string? TenHmuc { get; set; } = null!;
        public string? TenNhmuc { get; set; } = null!;
        public string? TenTongThanhXe { get; set; } = null!;
        public int? SuDung { get; set; } = null!;
        public string? TenUser { get; set; } = null!;
        public DateTime? NgayCapnhat { get; set; }
        public decimal PrKeyHsgd { get; set; }
    }

    public class DmHmuc_PASC_Filter
    {
        public string? TenHmuc { get; set; } = null!;
        public string? TenNhmuc { get; set; } = null!;
        public string? TenTongThanhXe { get; set; } = null!;
    }



    public class DmNHmucRequest
    {
        public string? MaTongthanhxe { get; set; } = null!;
        public string? MaNhmuc { get; set; } = null!;
        public string? TenNhmuc { get; set; } = null!;
        public int? SuDung { get; set; } = null!;
    }

    public class DmHmucRequest
    {
        public string? MaTongthanhxe { get; set; } = null!;
        public string? MaNhmuc { get; set; } = null!;
        public string? MaHmuc { get; set;} = null!;
        public string? TenHmuc { get; set; } = null!;
        public int? SuDung { get; set; } = null!;
    }

    public class DmNhmucUpdate
    {

        public string? TenNhmuc { get; set; } = null!;

        public int? SuDung { get; set; } = null!;
    }

    public class DmHmucUpdate
    {
        public string? TenHmuc { get; set; } = null!;
        public int? SuDung { get; set; } = null!;
    }

}
