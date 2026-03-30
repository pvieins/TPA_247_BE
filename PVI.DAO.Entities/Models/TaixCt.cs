using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models
{
    public partial class TaixCt
    {
        public decimal PrKey { get; set; }
        public decimal FrKey { get; set; }
        public string MaSp { get; set; } = null!;
        public decimal MtnGtbhNte { get; set; }
        public decimal MtnGtbhTai { get; set; }
        public decimal MtnGtbhUsd { get; set; }
        public decimal GtbhPd { get; set; }
        public decimal GtbhPdsl { get; set; }
        public decimal GtbhOee { get; set; }
        public decimal GtbhOeesl { get; set; }
        public decimal GtbhTpl { get; set; }
        public decimal GtbhTplsl { get; set; }
        public decimal GtbhOther { get; set; }
        public decimal GtbhOthersl { get; set; }
        public decimal TylePhi { get; set; }
        public decimal NguyenTeps { get; set; }
        public decimal SoTienps { get; set; }
        public decimal PhiMoigioi { get; set; }
        public decimal PviFeeNte { get; set; }
        public decimal PviFee { get; set; }
        public string MucKhautru { get; set; } = null!;
        public string MucKhtruhoi { get; set; } = null!;
        public decimal NguyenTeth { get; set; }
        public decimal SoTienth { get; set; }
        public decimal MtnReten { get; set; }
        public decimal PerLimitReten { get; set; }
        public decimal TyleReten { get; set; }
        public decimal MtnGtbhHull { get; set; }
        public decimal NguyenTepHull { get; set; }
        public decimal MtnGtbhIv { get; set; }
        public decimal NguyenTepIv { get; set; }
        public decimal MtnGtbhWr { get; set; }
        public decimal NguyenTepWr { get; set; }
        public decimal MtnGtbhOther { get; set; }
        public decimal NguyenTepOther { get; set; }
        public decimal TyleDongtruoc { get; set; }
        public decimal PhiDongtruoc { get; set; }
        public decimal TyleDongsau { get; set; }
        public decimal PhiDongsau { get; set; }
        public decimal TylePhiuoc { get; set; }
        public decimal PhiUoc { get; set; }
        public decimal TylePhitai { get; set; }
        public decimal PhiTai { get; set; }
        public decimal NguyenTepi { get; set; }
        public decimal TyleTai { get; set; }
        public decimal MtnGiulai { get; set; }
        public decimal MtnRetenNte { get; set; }
        public decimal MtnRetenUsd { get; set; }
        public decimal NguyenTepReten { get; set; }
        public decimal TyleTor { get; set; }
        public decimal MtnTorUsd { get; set; }
        public bool TinhTay { get; set; }
        public string TenRuiro { get; set; } = null!;
        public decimal PrKeyTainCt { get; set; }
        public string MaDk { get; set; } = null!;
        public decimal PhiCodinh { get; set; }
        public string MaCat { get; set; } = null!;
        public decimal PrKeyNvuBhtCt { get; set; }
        public decimal TyleTaihoTty { get; set; }
        public string MaDdiembhCt { get; set; } = null!;
        public string Layer { get; set; } = null!;
        public string MaTteGoc { get; set; } = null!;
        public decimal TyleNhan { get; set; }
        public decimal MtnNhanNte { get; set; }
        public decimal NguyenTepNhan { get; set; }
        public decimal HhongNhanNte { get; set; }
        public decimal TyleNhuongPhityle { get; set; }
        public DateTime? NgayTinhTaicd { get; set; }
    }
}
