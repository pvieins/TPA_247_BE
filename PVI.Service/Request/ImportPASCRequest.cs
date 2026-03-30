using AutoMapper;
using PVI.DAO.Entities.Models;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace PVI.Service.Request;

public  class ImportPASCRequest
{
    public int PrKey { get; set; }

    public int FrKey { get; set; }

    public string? MaHmuc { get; set; } = null!;

    public string? TenHmuc { get; set; } = null!;

    public string? SoTientt { get; set; }

    public string? SoTienph { get; set; }

    public string? SoTienson { get; set; }

    public string? GhiChudv { get; set; } = null!;

    public int? LoaiDx { get; set; }

    public string? VatSc { get; set; }

    public decimal PrKeyDx { get; set; }
}

