using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class HsgdCt
{
    public int PrKey { get; set; }

    public int FrKey { get; set; }

    public int Stt { get; set; }

    public int NhomAnh { get; set; }

    public string PathFile { get; set; } = null!;

    public DateTime? NgayChup { get; set; }

    public string ViDoChup { get; set; } = null!;

    public string KinhDoChup { get; set; } = null!;

    public string? DienGiai { get; set; }

    public string PathUrl { get; set; } = null!;

    public string PathOrginalFile { get; set; } = null!;

    public bool Android { get; set; }

    public string MaHmuc { get; set; } = null!;
    public string MaHmucSc {  get; set; } = null!;
}
