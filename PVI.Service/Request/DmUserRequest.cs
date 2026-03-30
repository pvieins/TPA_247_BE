using AutoMapper;
using PVI.DAO.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;


namespace PVI.Service.Request
{
    public class UserGDTT
    {
        //public Guid? Oid { get; set; } = null!;
        public string? MaUser { get; set; } = null!;

        public string? TenUser { get; set; } = null!;

        public string? Dienthoai { get; set; } = null!;

        public string? Mail { get; set; } = null!;

        public string? MaDonvi { get; set; } = null;
        public string? TenDonvi { get; set; } = null!;

        public int? LoaiUser { get; set; } = null!;

        public bool? IsActive { get; set; } = null!;

        public bool? IsActiveGddk { get; set; } = null!;

        public bool? IsGdvHotro { get; set; } = null!;

        public bool? IsActiveGqkn { get; set; } = null!;

        public bool? IsactiveChkc { get; set; } = null!;

        public bool? IsActiveKytt { get; set; } = null!;
        public bool? PquyenUplHinhAnh { get; set; } = null!;
        public bool? PhanQuyen { get; set; } = null!;

        public string? LoaiCbo { get; set; } = null!;

        public string? MaDonviPquyen { get; set; } = null!;

        public string? MaUserPias { get; set; } = null!;

    }

    public class UserGDDK
    {
        //public Guid? Oid { get; set; } = null!;
        public string? MaUser { get; set; } = null!;
        public string? TenUser { get; set; } = null!;

        public string? Dienthoai { get; set; } = null!;

        public string? MaDonvi { get; set; } = null;
        public string? TenDonvi { get; set; } = null!;

        public string? Mail { get; set; } = null!;

        public int? LoaiUser { get; set; } = null!;

        public string? LoaiCbo { get; set; } = null!;

        public bool? IsActiveGddk { get; set; } = null;

        public bool? IsActiveGqkn { get; set; } = null!;

        public bool? PquyenUplHinhAnh { get; set; } = null!;

        public bool? PhanQuyen { get; set; } = null!;

        public string? Password { get; set; } = null!;
    }

}
