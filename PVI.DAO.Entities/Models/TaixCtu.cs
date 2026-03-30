using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models
{
    public partial class TaixCtu
    {
        public decimal PrKey { get; set; }
        public string MaDonvi { get; set; } = null!;
        public string MaCtu { get; set; } = null!;
        public string SoCtu { get; set; } = null!;
        public DateTime? NgayCtu { get; set; }
        public int MaLdon { get; set; }
        public string MaDonbh { get; set; } = null!;
        public string MaPkt { get; set; } = null!;
        public string SoHdgcn { get; set; } = null!;
        public string SoDonbhbs { get; set; } = null!;
        public string SoDonbhtt { get; set; } = null!;
        public decimal SoSeri { get; set; }
        public DateTime? NgayCapd { get; set; }
        public DateTime? NgayDau { get; set; }
        public DateTime? NgayCuoi { get; set; }
        public decimal SoNgay { get; set; }
        public DateTime? NgayThuphi { get; set; }
        public string MaKh { get; set; } = null!;
        public string TenKh { get; set; } = null!;
        public string NgGdich { get; set; } = null!;
        public string DtuongBh { get; set; } = null!;
        public string MaCtydong { get; set; } = null!;
        public decimal TyleDong { get; set; }
        public decimal TyleTaiho { get; set; }
        public string MaCtytaicd { get; set; } = null!;
        public decimal TyleTaicd { get; set; }
        public decimal TyleHhongcd { get; set; }
        public string DkienDkhoan { get; set; } = null!;
        public string MucKhautru { get; set; } = null!;
        public string DienGiai { get; set; } = null!;
        public string MaTte { get; set; } = null!;
        public decimal TygiaHt { get; set; }
        public decimal TygiaTt { get; set; }
        public int NamDong { get; set; }
        public string NamNoidong { get; set; } = null!;
        public string NvuMaDoitau { get; set; } = null!;
        public string MaLtau { get; set; } = null!;
        public string MaHangtau { get; set; } = null!;
        public string? MaHoi { get; set; }
        public decimal TrongTaigt { get; set; }
        public string TrongTai { get; set; } = null!;
        public string VungHdong { get; set; } = null!;
        public string MaPtvc { get; set; } = null!;
        public string PthucVchuyen { get; set; } = null!;
        public DateTime? NgayKhoihanh { get; set; }
        public string VanDon { get; set; } = null!;
        public string NoiDi { get; set; } = null!;
        public int DiaDiemdi { get; set; }
        public string NoiChuyenTai { get; set; } = null!;
        public string NoiDen { get; set; } = null!;
        public int DiaDiemden { get; set; }
        public DateTime? HanBaohanh { get; set; }
        public string BaoHanh { get; set; } = null!;
        public string DiaDiembh { get; set; } = null!;
        public string WindFire { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string BiCode { get; set; } = null!;
        public string MdonBh { get; set; } = null!;
        public string MaUser { get; set; } = null!;
        public string MaNhkh { get; set; } = null!;
        public string MaTinh { get; set; } = null!;
        /// <summary>
        /// Nhung don co cung dia diem co gia tri bang 1
        /// </summary>
        public bool ChkDdiem { get; set; }
        public decimal PrKeyGoc { get; set; }
        public decimal PrKeyPi { get; set; }
        /// <summary>
        /// luu pr_key cua don retro
        /// </summary>
        public decimal PrKeyRetro { get; set; }
        public int KeyStatus { get; set; }
        public bool WetRisk { get; set; }
        public string MaTau { get; set; } = null!;
        public decimal PrKeyXepchuyen { get; set; }
        public bool XepChuyen { get; set; }
        public string TrangThai { get; set; } = null!;
        public decimal TamTinh { get; set; }
        public decimal SoNgtg { get; set; }
        public string MaNkd { get; set; } = null!;
        public string SoHdong { get; set; } = null!;
        public string MaHdong { get; set; } = null!;
        public DateTime? NamNvu { get; set; }
        public string MaKthac { get; set; } = null!;
        public string MaDd { get; set; } = null!;
        public string MaNhruiro { get; set; } = null!;
        public string MaDaily { get; set; } = null!;
        public string MaNhkenhbh { get; set; } = null!;
        public string MaKenhbh { get; set; } = null!;
        public string LoaiHinhbh { get; set; } = null!;
        public string MaLoaixe { get; set; } = null!;
        public bool KhongTdbt { get; set; }
        public string MaCtyDbh { get; set; } = null!;
        public string SoDonbhSdbs { get; set; } = null!;
        public bool KhongTtoanPhinhuong { get; set; }
        public string KhongTtoanPhinhuongGhichu { get; set; } = null!;
        public bool KhongTtoanThudoibt { get; set; }
        public string KhongTtoanThudoibtGhichu { get; set; } = null!;
        public string MaSdbs { get; set; } = null!;
        public string MaSdbsCt { get; set; } = null!;
        public string MaNhloaixe { get; set; } = null!;
        public int CsNhamay { get; set; }
        public int CsTurbine { get; set; }
        public decimal TyleGiamphi { get; set; }
        public decimal TyleTonthat { get; set; }
    }
}
