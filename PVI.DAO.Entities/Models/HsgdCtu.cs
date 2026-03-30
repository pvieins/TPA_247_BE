using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.DAO.Entities.Models;

public partial class HsgdCtu
{
    public int PrKey { get; set; }

    public string? MaCtu { get; set; }

    public DateTime? NgayCtu { get; set; }

    public string MaDonvi { get; set; } = null!;

    public string MaPkt { get; set; } = null!;

    public string MaDonbh { get; set; } = null!;

    public string SoHsgd { get; set; } = null!;

    public string SoDonbh { get; set; } = null!;

    public string BienKsoat { get; set; } = null!;

    public decimal SoSeri { get; set; }

    public DateTime? NgayDauSeri { get; set; }

    public DateTime? NgayCuoiSeri { get; set; }

    public DateTime? NgayTbao { get; set; }

    public DateTime? NgayTthat { get; set; }

    public string NguyenNhanTtat { get; set; } = null!;

    public string DiaDiemgd { get; set; } = null!;

    public string DiaDiemtt { get; set; } = null!;

    public string MaKh { get; set; } = null!;

    public string TenKhach { get; set; } = null!;

    public string MaTte { get; set; } = null!;

    public decimal TygiaHt { get; set; }

    public decimal TygiaTt { get; set; }

    public string GhiChu { get; set; } = null!;

    public DateTime NgayGdinh { get; set; }

    public string MaTtrangGd { get; set; } = null!;

    public string TrangThai { get; set; } = null!;

    public decimal PrKeySeri { get; set; }

    public string MaKthac { get; set; } = null!;

    public string MaDaily { get; set; } = null!;

    public string MaCbkt { get; set; } = null!;

    public string MaNhloaixe { get; set; } = null!;

    public string MaLhsbt { get; set; } = null!;

    public string DienThoai { get; set; } = null!;

    public string MaDvbtHo { get; set; } = null!;

    public decimal SoLanBt { get; set; }

    public string? MaDviBtHo { get; set; }

    public string PrKeyBtHo { get; set; } = null!;

    public decimal PrKeyGoc { get; set; }

    public string? MaGaraVcx { get; set; }

    public string? MaGaraVcx01 { get; set; }

    public string? MaGaraVcx02 { get; set; }

    public Guid? MaUser { get; set; }

    public string InsertMobile { get; set; } = null!;

    public string? NgLienhe { get; set; }

    public string DiaChi { get; set; } = null!;

    public int HieuXe { get; set; }

    public int LoaiXe { get; set; }

    public string XuatXu { get; set; } = null!;

    public int NamSx { get; set; }

    public decimal SoTienugd { get; set; }

    public decimal SoTienctkh { get; set; }

    public string LydoCtkh { get; set; } = null!;

    public DateTime? NgayDuyet { get; set; }

    public string GhiChudx { get; set; } = null!;

    public string GhiChudxtt { get; set; } = null!;

    public int HsgdTpc { get; set; }

    public string NguoiXuly { get; set; } = null!;

    public string NguoiGiao { get; set; } = null!;

    public int DvnhapPasc { get; set; }

    public int Vat { get; set; }

    public int HieuXeTnds { get; set; }

    public int LoaiXeTnds { get; set; }

    public string XuatXuTnds { get; set; } = null!;

    public int NamSxTnds { get; set; }

    public string? MaGaraTnds { get; set; }

    public string? MaGaraTnds01 { get; set; }

    public string? MaGaraTnds02 { get; set; }

    public int VatTnds { get; set; }

    public decimal SoTienctkhTnds { get; set; }

    public string DoituongttTnds { get; set; } = null!;

    public string LydoCtkhTnds { get; set; } = null!;

    public string GhiChudxTnds { get; set; } = null!;

    public string? GhiChudxTndstt { get; set; }

    public int VatTsk { get; set; }

    public decimal SoTienctkhTsk { get; set; }

    public string DoituongttTsk { get; set; } = null!;

    public string LydoCtkhTsk { get; set; } = null!;

    public string GhiChudxTsk { get; set; } = null!;

    public string? GhiChudxTsktt { get; set; }

    public decimal PrKeyBt { get; set; }

    public string MaDonvigd { get; set; } = null!;

    public decimal? TyleggPhutungvcx { get; set; }

    public decimal? TyleggSuachuavcx { get; set; }

    public decimal? TyleggPhutungtnds { get; set; }

    public decimal? TyleggSuachuatnds { get; set; }

    public int BaoLanh { get; set; }

    public string TenLaixe { get; set; } = null!;

    public int NamSinh { get; set; }

    public string SoGphepLaixe { get; set; } = null!;

    public DateTime? NgayDauLaixe { get; set; }

    public DateTime? NgayCuoiLaixe { get; set; }

    public string MaLoaibang { get; set; } = null!;

    public string SoGphepLuuhanh { get; set; } = null!;

    public DateTime? NgayDauLuuhanh { get; set; }

    public DateTime? NgayCuoiLuuhanh { get; set; }

    public string HosoPhaply { get; set; } = null!;

    public string YkienGdinh { get; set; } = null!;

    public string DexuatPan { get; set; } = null!;

    public string MaDdiemTthat { get; set; } = null!;

    public string NgGdichTh { get; set; } = null!;

    public int ThieuAnh { get; set; }

    public int ChuaThuphi { get; set; }

    public int SaiDkdk { get; set; }

    public int SaiPhancap { get; set; }

    public int TrucloiBh { get; set; }

    public int SaiphamKhac { get; set; }

    public string PascDsemail { get; set; } = null!;

    public string PascDsphone { get; set; } = null!;

    public int PascSendEmail { get; set; }

    public int Bl1 { get; set; }

    public int Bl2 { get; set; }

    public int Bl3 { get; set; }

    public int Bl4 { get; set; }

    public int Bl5 { get; set; }

    public int Bl6 { get; set; }

    public int Bl7 { get; set; }

    public int Bl8 { get; set; }

    public int Bl9 { get; set; }

    public string BlTailieubs { get; set; } = null!;

    public string BlDsemail { get; set; } = null!;

    public string BlDsphone { get; set; } = null!;

    public int BlSendEmail { get; set; }

    public int BlPdbl { get; set; }

    public int DangKiem { get; set; }

    public string MaDonviTt { get; set; } = null!;

    public int PrKeyKbtt { get; set; }

    public string? DienThoaiNdbh { get; set; }
    public decimal SoTienBaoHiem { get; set; }

    public decimal SoTienThucTe { get; set; }

    public string MaSanpham { get; set; } = null!;

    public int PascVatVcx { get; set; }

    public int PascVatTnds { get; set; }

    public decimal SoTienGtbt { get; set; }

    public decimal SoTienGtbtTnds { get; set; }

    public decimal SoTienGtbtKhac { get; set; }

    public string PathVcxDt { get; set; } = null!;

    public string PathTndsDt { get; set; } = null!;

    public string PathTndsKhacDt { get; set; } = null!;

    public string? DonviSuachuaTsk { get; set; }

    public int? HieuXeTndsBen3 { get; set; }

    public int? LoaiXeTndsBen3 { get; set; }

    public string? MaNguyenNhanTtat { get; set; }

    public string? PathCrm { get; set; }

    public bool ChkDaydu { get; set; }

    public bool ChkDunghan { get; set; }
    public bool ChkTheohopdong { get; set; }

    public string HauQua { get; set; } = null!;

    public string NgayThuphi { get; set; } = null!;

    public string PathTotrinhTpc { get; set; } = null!;

    public int LoaiTotrinhTpc { get; set; }

    public int SendThongbaoBt { get; set; }
    public string? VaiTro { get; set; } = string.Empty;
    public decimal? TyleTg { get; set; } = 0m;
    public string? SoHsbt { get; set; } = string.Empty;
    public bool HoanThienHstt { get; set; } = false;
}