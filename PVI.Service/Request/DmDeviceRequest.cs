using AutoMapper;
using PVI.DAO.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;


namespace PVI.Service.Request
{
    public class DeviceFilter
    {
        public string? ImeiDevice { get; set; } = null!;

        public string? AddressDevice { get; set; } = null!;

        public string? MaUser { get; set; } = null!;

        public string? TenDonvi { get; set; } = null!;

        public string? TypeDevice { get; set; } = null!;

        public bool? Active { get; set; } = null!;

        public string? Description { get; set; } = null!;

        public bool? Status { get; set; } = null!;
    }

    public class DeviceRequest
    {
        public string? ImeiDevice { get; set; } = null!;

        public string? AddressDevice { get; set; } = null!;

        public string? MaUser { get; set; } = null!;

        public string? MaDonvi { get; set; } = null!;

        public string? TypeDevice { get; set; } = null!;

        public bool? Active { get; set; } = null!;

        public string? Description { get; set; } = null!;

        public bool? Status { get; set; } = null!;
    }

}