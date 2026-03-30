using System;
using System.Collections.Generic;

 namespace PVI.DAO.Entities.Models
{
    public partial class KbttCtu
    {
        public int PrKey { get; set; }
        public string UserId { get; set; } = null!;
        public int LoaiKbtt { get; set; }
        public DateTime? NgayKbtt { get; set; }
        public int TinhTrang { get; set; }
        public string MaDonvi { get; set; } = null!;
        public string TenDonvi { get; set; } = null!;
        public string SoDonbh { get; set; } = null!;
        public string MaKthac { get; set; } = null!;
        public string MaCbkt { get; set; } = null!;
        public string MaNhloaixe { get; set; } = null!;
        public string MaPkt { get; set; } = null!;
        public string MaKh { get; set; } = null!;
        public string TenKhach { get; set; } = null!;
        public string DiaChiKh { get; set; } = null!;
        public string? NgGdichTh { get; set; }
        public int SoSeri { get; set; }
        public string BienKsoat { get; set; } = null!;
        public string NhanHieu { get; set; } = null!;
        public string MaLoaixe { get; set; } = null!;
        public string MaDongxe { get; set; } = null!;
        public int NamSx { get; set; }
        public string TrongTai { get; set; } = null!;
        public string SoCngoi { get; set; } = null!;
        public DateTime? NgayCapSeri { get; set; }
        public DateTime? NgayDauSeri { get; set; }
        public DateTime? NgayCuoiSeri { get; set; }
        public string DienThoaiSeri { get; set; } = null!;
        public DateTime? NgayTthat { get; set; }
        public string ThoigianTthat { get; set; } = null!;
        public string DiaDiemtt { get; set; } = null!;
        public string NguyenNhanTtat { get; set; } = null!;
        public string CoquanGquyet { get; set; } = null!;
        public string HauquaTsan { get; set; } = null!;
        public string HauquaNguoi { get; set; } = null!;
        public DateTime? NgayTthatkh { get; set; }
        public string ThoigianTthatkh { get; set; } = null!;
        public string DiaDiemttkh { get; set; } = null!;
        public string NguyenNhanTtatkh { get; set; } = null!;
        public string HauquaTsankh { get; set; } = null!;
        public string HauquaNguoikh { get; set; } = null!;
        public DateTime? NgayGdinh { get; set; }
        public DateTime? NgayHengd { get; set; }
        public string NguoiLienhe { get; set; } = null!;
        public string DienthoaiLienhe { get; set; } = null!;
        public string GaraGiamdinh { get; set; } = null!;
        public string TengaraGdinh { get; set; } = null!;
        public string GaraSuachua { get; set; } = null!;
        public string TengaraSuachua { get; set; } = null!;
        public decimal SoTienugd { get; set; }
        public decimal PrKeySeri { get; set; }
        public decimal PrKeyGdtt { get; set; }
        public string SoHsgd { get; set; } = null!;
        public string LoaihinhBh { get; set; } = null!;
        public string MaUser { get; set; } = null!;
        public string TenUser { get; set; } = null!;
        public string MaUserGdv { get; set; } = null!;
        public int TaoQuaApp { get; set; }
        public int TinhTrangCaiapp { get; set; }
        public int GuiEmailKh { get; set; }
        public string? DangKiem { get; set; }
        public DateTime? NgayDauDk { get; set; }
        public DateTime? NgayCuoiDk { get; set; }
        public string? Gplx { get; set; }
        public string? HangXe { get; set; }
        public string? TenLaiXe { get; set; }
        public bool? IsChuXe { get; set; }
        public string? TenNguoiKy { get; set; }
        public bool IsdonviDuyet { get; set; }
        public string MaDieukhoanTnds { get; set; } = null!;
        public string MaDonviChuyen { get; set; } = null!;
        public decimal PrKeyBt { get; set; }
        public int LoaiChupanh { get; set; }
        public string TinhThanh { get; set; } = null!;
        public string QuanHuyen { get; set; } = null!;
        public DateTime? NgaybatdauGplx { get; set; }
        public DateTime? NgaykethucGplx { get; set; }
        public DateTime? NgaySuachua { get; set; }
        public string YeucauQuydinh { get; set; } = null!;
        public string YeucauKhac { get; set; } = null!;
    }
}
