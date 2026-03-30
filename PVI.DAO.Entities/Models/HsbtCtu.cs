using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class HsbtCtu
{
    public decimal PrKey { get; set; }

    public string MaDonvi { get; set; } = null!;

    public string MaCtu { get; set; } = null!;

    public DateTime? NgayCtu { get; set; }

    public string MaPkt { get; set; } = null!;

    public string SoHsbt { get; set; } = null!;

    public DateTime? NgayCapd { get; set; }

    public DateTime? NgayDau { get; set; }

    public DateTime? NgayCuoi { get; set; }

    public string MaDonbh { get; set; } = null!;

    public DateTime? NgayTthat { get; set; }

    public DateTime? NgayTbao { get; set; }

    public string MaKh { get; set; } = null!;

    public string TenDttt { get; set; } = null!;

    public string NguyenNhan { get; set; } = null!;

    public int SoLanBt { get; set; }

    public string MaTte { get; set; } = null!;

    public decimal TygiaHt { get; set; }

    public decimal TygiaTt { get; set; }

    public string GhiChu { get; set; } = null!;

    public string SoHdgcn { get; set; } = null!;

    public string SoDonbhbs { get; set; } = null!;

    public string NguyenNhanTtat { get; set; } = null!;

    public string LoaiXe { get; set; } = null!;

    public string DiaChi { get; set; } = null!;

    public decimal ChiKhac { get; set; }

    public string ThinhNphi { get; set; } = null!;

    public string DiaDiem { get; set; } = null!;

    public string HauQua { get; set; } = null!;

    public string TtrangXe { get; set; } = null!;

    public string TinhToanbtVcx { get; set; } = null!;

    public string TinhToanbtHhoa { get; set; } = null!;

    public string TinhToanbtLpx { get; set; } = null!;

    public string TinhToanbtCng { get; set; } = null!;

    public string PanThoiTs { get; set; } = null!;

    public string PanTdoiNt3 { get; set; } = null!;

    public string MaGaraVcx { get; set; } = null!;

    public decimal CheTai { get; set; }

    public decimal KhauHao { get; set; }

    public decimal SoPhibh { get; set; }

    public string TenKhle { get; set; } = null!;

    public string MaGaraVcx2 { get; set; } = null!;

    public string MaGaraVcx3 { get; set; } = null!;

    public string CancuDexuat { get; set; } = null!;

    public decimal NamsxVcx { get; set; }

    public string TenngDenghi { get; set; } = null!;

    public string NoidungDenghi { get; set; } = null!;

    public bool HthucTtoan { get; set; }

    public string TenngNhtien { get; set; } = null!;

    public string SotkNghang { get; set; } = null!;

    public string TenNghang { get; set; } = null!;

    public string CtuKemtheo { get; set; } = null!;

    public string MaLoaixe { get; set; } = null!;

    public string MaCbcnv { get; set; } = null!;

    public DateTime? NgayDkyxe { get; set; }

    public string SoThe { get; set; } = null!;

    public string MaDieutri { get; set; } = null!;

    public decimal NamSinh { get; set; }

    public string NoiDieutri { get; set; } = null!;

    public decimal TyleTtat { get; set; }

    public DateTime? NgayKham { get; set; }

    public decimal TcnamVien { get; set; }

    public decimal TienKham { get; set; }

    public decimal TienThuoc { get; set; }

    public decimal VienPhi { get; set; }

    public decimal SotienTuchoibt { get; set; }

    public string NgnhanTuchoibt { get; set; } = null!;

    public string MaUser { get; set; } = null!;

    public string GtrinhChikhac { get; set; } = null!;

    public decimal SotienVcx { get; set; }

    public decimal SotienVcx2 { get; set; }

    public decimal SotienVcx3 { get; set; }

    public string HosoPhaply { get; set; } = null!;

    public string YkienGdinh { get; set; } = null!;

    public string DexuatPan { get; set; } = null!;

    public decimal SoSeri { get; set; }

    public string LydoTuchoibt { get; set; } = null!;

    public string TinhtoanbtCng1 { get; set; } = null!;

    public string TenLaixe { get; set; } = null!;

    public string SoGphepLaixe { get; set; } = null!;

    public DateTime? NgayDauLaixe { get; set; }

    public DateTime? NgayCuoiLaixe { get; set; }

    public string SoGphepLuuhanh { get; set; } = null!;

    public DateTime? NgayDauLuuhanh { get; set; }

    public DateTime? NgayCuoiLuuhanh { get; set; }

    public string MaGaraTnds { get; set; } = null!;

    public decimal SotienTnds { get; set; }

    public string MaGaraTnds2 { get; set; } = null!;

    public decimal SotienTnds2 { get; set; }

    public string MaGaraTnds3 { get; set; } = null!;

    public decimal SotienTnds3 { get; set; }

    public string CancuDexuatTnds { get; set; } = null!;

    public DateTime? NgayDkyxeTnds { get; set; }

    public decimal CheTaiTnds { get; set; }

    public decimal KhauHaoTnds { get; set; }

    public string TenChuxeTnds { get; set; } = null!;

    public string LoaiXeTnds { get; set; } = null!;

    public decimal NamsxTnds { get; set; }

    public string BiensoXeTnds { get; set; } = null!;

    public string LydoTcap { get; set; } = null!;

    public string NoiKham { get; set; } = null!;

    public decimal GiatriTteXe { get; set; }

    public string MaDaily { get; set; } = null!;

    public string MaKthac { get; set; } = null!;

    public string MaCbkt { get; set; } = null!;

    public string MaLhsbt { get; set; } = null!;

    public string SoHshoi { get; set; } = null!;

    public string SoGcnbh { get; set; } = null!;

    public string MaDvbtHo { get; set; } = null!;

    public decimal SotienDenghiTtoan { get; set; }

    public string NgdcBh { get; set; } = null!;

    public string? MaCbgd { get; set; }

    public DateTime? NgayGdinh { get; set; }

    public decimal SoNgchet { get; set; }

    public string DienThoai { get; set; } = null!;

    public DateTime? NgayThuPhi { get; set; }

    public decimal? TienXnat { get; set; }

    public decimal? TienXndt { get; set; }

    public decimal? TienThuthuat { get; set; }

    public decimal PrKeyGoc { get; set; }

    public string MaHoi { get; set; } = null!;

    public decimal? TyleDong { get; set; }

    public string SoHdbh { get; set; } = null!;

    public string SoCmnd { get; set; } = null!;

    public string ThonKhach { get; set; } = null!;

    public string XaKhach { get; set; } = null!;

    public string HuyenKhach { get; set; } = null!;

    public string TinhKhach { get; set; } = null!;

    public string VuluaRuong { get; set; } = null!;

    public string DchiRuong { get; set; } = null!;

    public string ThonRuong { get; set; } = null!;

    public string XaRuong { get; set; } = null!;

    public string HuyenRuong { get; set; } = null!;

    public string TinhRuong { get; set; } = null!;

    public decimal DtichRuong { get; set; }

    public decimal KluongDam { get; set; }

    public decimal DgiaDam { get; set; }

    public string ThuaRuong { get; set; } = null!;

    public string MaLruiro { get; set; } = null!;

    public string TaisanThuhoi { get; set; } = null!;

    public decimal GiatriThuhoi { get; set; }

    public string SotheVcxMoto { get; set; } = null!;

    public DateTime? NgayDaut { get; set; }

    public DateTime? NgayCuoit { get; set; }

    public string? GhiChuThuoc { get; set; }

    public string TenBenhMt { get; set; } = null!;

    public string KquaDtri { get; set; } = null!;

    public string CtuKtheo { get; set; } = null!;

    public bool ThamGia007 { get; set; }

    public string MaDdiemTthat { get; set; } = null!;

    public string MaLydoTuchoibt { get; set; } = null!;

    public string SoHsbtCommos { get; set; } = null!;

    public decimal TienChetai { get; set; }

    public string MaNgnhanTn { get; set; } = null!;

    public string MaLoaibang { get; set; } = null!;

    public decimal SoBthuong { get; set; }

    public DateTime? NgayXxuong { get; set; }

    public decimal PrKeySeri { get; set; }

    public DateTime? NgayDuyetgia { get; set; }

    public DateTime? NgayYcaubt { get; set; }

    public DateTime? NgayPhhanhCv { get; set; }

    public DateTime? NgayDuyet { get; set; }

    public bool DuyetHsbt { get; set; }

    public string UserDuyet { get; set; } = null!;

    public int BlTt { get; set; }

    public string Email { get; set; } = null!;

    public string MaNnhanTthat { get; set; } = null!;

    public DateTime? VersionEdit { get; set; }

    public decimal PrKeyBth { get; set; }

    public bool ChkCpd { get; set; }

    public string HosoPcap { get; set; } = null!;

    public bool DuyetPcap { get; set; }

    public DateTime? NgaydPcap { get; set; }

    public string UserdPcap { get; set; } = null!;

    public string SoDonbhTaibh { get; set; } = null!;

    public string SoDonbhSdbs { get; set; } = null!;

    public bool ChkHuybt { get; set; }

    public string MaBtXol { get; set; } = null!;

    public decimal TienBhyt { get; set; }

    public string? DiengiaiBt { get; set; }

    public string SotheBhyt { get; set; } = null!;
    public string MaDonviTt { get; set; } = null!;
}
