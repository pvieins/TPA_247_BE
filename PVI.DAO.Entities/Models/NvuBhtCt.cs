using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class NvuBhtCt
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public string MaSp { get; set; } = null!;

    /// <summary>
    /// Ma dieu khoan cua sp bao hiem
    /// </summary>
    public string MaDk { get; set; } = null!;

    /// <summary>
    /// muc trach nhiem doi voi nghiep vu tau song, tau ven bien.
    /// </summary>
    public string DkienQtac { get; set; } = null!;

    public string GhiChu { get; set; } = null!;

    public string MaPhi { get; set; } = null!;

    /// <summary>
    /// So tien hang hoa can bao hiem
    /// </summary>
    public decimal TienHhoa { get; set; }

    public decimal SoTienbh { get; set; }

    public decimal TongDtich { get; set; }

    public decimal TylePhi { get; set; }

    public decimal NguyenTep { get; set; }

    public decimal SoTienp { get; set; }

    public int MucVat { get; set; }

    public decimal NguyenTev { get; set; }

    public decimal TienVat { get; set; }

    public string MucKhtru { get; set; } = null!;

    public decimal TyleHhong { get; set; }

    public decimal TyleHhhoi { get; set; }

    public decimal SoTvien { get; set; }

    public string MaCthuc { get; set; } = null!;

    public decimal MucPhi { get; set; }

    public decimal TrongTai { get; set; }

    public decimal MucphiTvien { get; set; }

    /// <summary>
    /// ty le phi theo thang-su dung cho tau dong moi
    /// </summary>
    public decimal TyleTthang { get; set; }

    public bool TyleCovat { get; set; }

    public decimal KluongDam { get; set; }

    public decimal DgiaDam { get; set; }

    public string MaCat { get; set; } = null!;

    public string MaDdiembh { get; set; } = null!;

    public decimal SoTienbhLke { get; set; }

    public decimal PrKeyOld { get; set; }

    public decimal PhiCodinh { get; set; }

    public decimal TyleDongtruoc { get; set; }

    public decimal PhiDongtruoc { get; set; }

    public decimal TyleDongsau { get; set; }

    public decimal PhiDongsau { get; set; }

    public decimal TylePhiuoc { get; set; }

    public decimal PhiUoc { get; set; }

    public decimal TylePhitai { get; set; }

    public decimal PhiTai { get; set; }

    public decimal TyleLoadp { get; set; }

    public decimal MtnGtbhTai { get; set; }

    public decimal SoTienbhDon { get; set; }

    public decimal TyleTor { get; set; }
}
