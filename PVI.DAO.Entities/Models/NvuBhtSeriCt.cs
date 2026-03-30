using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class NvuBhtSeriCt
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public string MaSp { get; set; } = null!;

    public string MaDkbh { get; set; } = null!;

    public string MaTtep { get; set; } = null!;

    public decimal TygiaHt { get; set; }

    public decimal MtnGtbhNte { get; set; }

    public decimal MtnGtbhVnd { get; set; }

    public decimal NguyenTep { get; set; }

    public decimal SoTienp { get; set; }

    public int MucVat { get; set; }

    public decimal NguyenTev { get; set; }

    public decimal TienVat { get; set; }

    public decimal MtnGtbhTsan { get; set; }

    public decimal SoTan { get; set; }

    public decimal SoNguoi { get; set; }

    public decimal MucMienthuong { get; set; }

    public decimal GiatriTte { get; set; }

    public string MaCsdg { get; set; } = null!;

    public decimal KluongDam { get; set; }

    public decimal DgiaDam { get; set; }

    public string MaMuckt { get; set; } = null!;
}
