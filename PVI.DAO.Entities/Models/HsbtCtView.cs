using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class HsbtCtView
{
    public decimal PrKey { get; set; }
    public decimal FrKey { get; set; }
    public string MaSp { get; set; } = null!;
    public string MaDkhoan { get; set; } = null!;
    public decimal MtnGtbh { get; set; }
    public string MaTteGoc { get; set; } = null!;
    public string MaTtrangBt { get; set; } = null!;
    public string MaTtebt { get; set; } = null!;

    public decimal TygiaBt { get; set; }

    public decimal NguyenTep { get; set; }

    public decimal SoTienp { get; set; }

    public int MucVatp { get; set; }

    public decimal NguyenTevp { get; set; }

    public decimal SoTienvp { get; set; }

    public decimal SoTienkt { get; set; }
   
    public decimal TyleReten { get; set; }

    public decimal MtnRetenNte { get; set; }

    public decimal MtnRetenVnd { get; set; }

    public string? NgayHtoanBt { get; set; }
 
    public Nullable<decimal> SoTienkhTra { get; set; }// Số tiền KH đồng trả 
    public Nullable<decimal> SoTientcbt { get; set; }//Tổng từ chối 
    public decimal PrKeyHsgdDxCt { get; set; }
    //Thông tin chi tiết VAT
    public string MauSovat { get; set; } = null!;

    public string SerieVat { get; set; } = null!;

    public string SoHdvat { get; set; } = null!;

    public string? NgayHdvat { get; set; }

    public string MaKhvat { get; set; } = null!;

    public string TenKhvat { get; set; } = null!;

    public string MasoVat { get; set; } = null!;

    public string TenHhoavat { get; set; } = null!;
    //
    public int HieuXe { get; set; }

    public int LoaiXe { get; set; }

    public string XuatXu { get; set; } = null!;

    public int NamSx { get; set; }
    public decimal SoTienctkh { get; set; }

    public decimal TyleggPhutungvcx { get; set; }

    public decimal TyleggSuachuavcx { get; set; }

    public int VatTnds { get; set; }

    public int Vat { get; set; }

    public string LydoCtkh { get; set; } = null!;

    public string GhiChudx { get; set; } = null!;

    public decimal SoTienGtbt { get; set; }
    public string MaGara { get; set; } = null!;

    public string MaGara01 { get; set; } = null!;

    public string MaGara02 { get; set; } = null!;
    public decimal PrKeyHsgdDxCtu { get; set; }

    public int HieuXeTndsBen3 { get; set; }

    public int LoaiXeTndsBen3 { get; set; }

    public string DonviSuachuaTsk { get; set; } = null!;
    public string DoituongttTnds { get; set; } = null!;
    public int ChkKhongHoadon { get; set; }
    public bool KyTT { get; set; }
    public string MaLoaiDongco { get; set; } = null!;

    public decimal SotienTtpin { get; set; }
}
public partial class HsbtUocBT
{
    public decimal PrKey { get; set; }
    public decimal? FrKey { get; set; }
    public string? NgayPs { get; set; }//ngày ps
    //public decimal? NguyenTebt { get; set; }
    //public decimal? SoTienbt { get; set; }
    //public decimal? NguyenTebtPvi { get; set; }
    //public decimal? SoTienbtPvi { get; set; }
    public decimal? TyleReten { get; set; }//Tỷ lệ (%)
    //public decimal? NguyenTebtReten { get; set; }
    //public decimal? SoTienbtReten { get; set; }
    public decimal? NguyenTebtLk { get; set; }//Nguyên tệ Ước BT lũy kế
    public decimal? SoTienbtLk { get; set; }//Số tiền Ước BT lũy kế
    //public decimal? NguyenTebtPviLk { get; set; }
    //public decimal? SoTienbtPviLk { get; set; }
    public decimal? NguyenTebtRetenLk { get; set; }//Nguyên tệ Ước BT lũy kế đồng BH
    public decimal? SoTienbtRetenLk { get; set; }//Số tiền  Ước BT lũy kế đồng BH
    public string? GhiChu { get; set; } = null!;//ghi chú
}
public partial class HsbtGDView
{
    public decimal PrKey { get; set; }
    public decimal? FrKey { get; set; }
    public string? MaSp { get; set; } = null!;//mã sp
    public string? MaDvgd { get; set; } = null!;//mã đơn vị GD
    public string? MaLoaiChiphi { get; set; } = null!;//loại chi phí
    public string? MaTtegd { get; set; } = null!;//mã t.tê
    public decimal? TygiaGd { get; set; }//tỷ giá
    //Phải trả giám định
    public string? MaTtrangGd { get; set; } = null!;//t.trạng gd
    public decimal? NguyenTegd { get; set; }//Nguyên tệ
    public decimal? SoTiengd { get; set; }//số tiền vnd
    public decimal? MucVat { get; set; }// mức vat
    public decimal? NguyenTev { get; set; }//nguyên tệ vat
    public decimal SoTienv { get; set; }//số tiền vat
    //Phần trách nhiệm của CTY  
    public decimal? TyleReten { get; set; }//tỷ lệ (%)
    public decimal? MtnRetenNte { get; set; }//nguyên tệ phần trách nhiệm của cty
    public decimal? MtnRetenVnd { get; set; }//số tiền phần trách nhiệm của cty 
    public string? GhiChuGd { get; set; } = null!;//nội dung thanh toán
    public string? NgayHtoanGd { get; set; }//ngày duyệt GD 
    //Thông tin chi tiết VAT
    public string MauSovat { get; set; } = null!;

    public string SerieVat { get; set; } = null!;

    public string SoHdvat { get; set; } = null!;

    public string? NgayHdvat { get; set; }

    public string MaKhvat { get; set; } = null!;

    public string TenKhvat { get; set; } = null!;

    public string MasoVat { get; set; } = null!;

    public string TenHhoavat { get; set; } = null!;
    //
}
public partial class HsbtUocGD
{
    public decimal PrKey { get; set; }
    public decimal FrKey { get; set; }
    public string? NgayPs { get; set; }//ngày ps
    //public decimal NguyenTegd { get; set; }
    //public decimal SoTiengd { get; set; }
    //public decimal NguyenTegdPvi { get; set; }
    //public decimal SoTiengdPvi { get; set; }
    public decimal TyleReten { get; set; }//Tỷ lệ (%)
    //public decimal NguyenTegdReten { get; set; }
    //public decimal SoTiengdReten { get; set; }
    public string? GhiChu { get; set; } = null!;//ghi chú
    public decimal NguyenTegdLk { get; set; }//Nguyên tệ Ước GĐ lũy kế
    public decimal SoTiengdLk { get; set; }//Số tiền Ước GĐ lũy kế
    //public decimal NguyenTegdPviLk { get; set; }
    //public decimal SoTiengdPviLk { get; set; }
    public decimal NguyenTegdRetenLk { get; set; }//Nguyên tệ Ước GĐ thuộc TNGL
    public decimal SoTiengdRetenLk { get; set; }//Số tiền  Ước GĐ thuộc TNGL
}
public partial class HsbtThtsView
{
    public decimal PrKey { get; set; }
    public decimal FrKey { get; set; }
    public string? MaSp { get; set; } = null!;//mã sp
    public string? LoaiHinhtd { get; set; } = null!;//loại hình
    public string? MaTte { get; set; } = null!;//mã tte
    public decimal? TygiaTd { get; set; }//tỷ giá 
    //số tiền thu về
    public string? MaTtrangTd { get; set; } = null!;//t.trang tđ
    public decimal? NguyenTeTd { get; set; }// nguyên tệ thu đòi
    public decimal? SoTienTd { get; set; }// số tiền thu đòi
    //Thu về thuộc TN Công ty
    public decimal? TyleReten { get; set; }//tỷ lệ
    public decimal? MtnRetenNte { get; set; }// nguyên tệ thuộc tn cty
    public decimal? MtnRetenVnd { get; set; }//số tiền thuộc tn cty
    public string? GhiChu { get; set; } = null!;//nội dung thanh toán
    public string? NgayHtoanTd { get; set; }//KT nhập TĐ
}
public partial class HsgdDxView
{
    public int PrKey { get; set; }
    public int FrKey { get; set; }
    public int Stt { get; set; }
    public string MaHmuc { get; set; } = null!;
    public string TenHmuc { get; set; } = null!;
    public decimal SoTientt { get; set; }
    public decimal SoTienph { get; set; }
    public decimal SoTienson { get; set; }
    public decimal SoTiensc { get; set; }
    public int VatSc { get; set; }
    public int GiamTruBt { get; set; }
    public bool ThuHoiTs { get; set; }
    public decimal? SoTienDoitru { get; set; }
    public string GhiChudv { get; set; } = null!;
    public decimal SoTienpdtt { get; set; }// k dùng
    public decimal SoTienpdsc { get; set; }  // k dùng
    public string GhiChutt { get; set; } = null!;// k dùng
    public int LoaiDx { get; set; }
}
public partial class HsgdDxSum
{
    public int Sldx { get; set; }// số đề xuất
    public decimal? SumSoTienDoitru { get; set; } = 0;//Số tiền đối trừ
    public decimal? SumSoTientt { get; set; } = 0; // Tổng - thay thế
    public decimal? SumSoTienph { get; set; } = 0; // Tổng - phục hồi
    public decimal? SumSoTienson { get; set; } = 0; // Tổng - sơn   
    public decimal? SumSoTienVat { get; set; } = 0;// Tổng - vat
    public decimal? SumSoTienGiamtru { get; set; } = 0;// Tổng - Giảm trừ bồi thường và Số tiền giảm trừ bồi thường
    public decimal? SumSTDX { get; set; } = 0;//Tổng (TT+PH+sơn) không VAT
    public decimal? SumSoTienTtsc { get; set; } = 0;//Tổng (TT+PH+sơn) có VAT
    public decimal? SumSoTienGgsc { get; set; } = 0;//Số tiền giảm giá(theo TTHT)
    public decimal? StBl { get; set; } = 0;//Số tiền PVI Bảo lãnh TT
    public decimal? SoTienctkh { get; set; } = 0;//Số tiền khấu trừ/KH KHTT

    //Số tiền giảm trừ bồi thường = SoTienGtbt nếu SumSoTienGiamtru = 0
    public decimal? SoTienGtbt { get; set; } = 0;// số tiền GTBT trong bảng hsgd_dx_ct
    public decimal? SumSoTiensc { get; set; } = 0;// Tổng - sửa chữa 
    public decimal? SumTskSoTienVat { get; set; } = 0;// Tổng - vat
    public decimal? SumTskStdx { get; set; } = 0;//Tổng Đ.X (TT+ SC) Không VAT
    public decimal? SumTskTtsc { get; set; } = 0;//Tổng Đ.X (TT + SC) có VAT
    public decimal? SumTskSoTienGiamtru { get; set; } = 0;// Tổng - giảm trừ BT và Số tiền giảm trừ bồi thường
}
public partial class HsgdDxSumTmp
{
    public decimal? SoTientt { get; set; }
    public decimal? SoTienph { get; set; }
    public decimal? SoTienson { get; set; }
    public decimal? SoTienVat { get; set; }
    public decimal? SoTienTtsc { get; set; }
    public decimal? SumSoTienGiamtru { get; set; }
    public decimal? SoTienGgsc { get; set; }
    public decimal? SoTienDoitru { get; set; }
    public decimal? SoTienctkh { get; set; }
    public decimal? SoTienGtbt { get; set; }
}
public partial class HsgdDxTsksSumTmp
{
    public decimal? SoTientt { get; set; }
    public decimal? SoTiensc { get; set; }
    public decimal? SoTienVat { get; set; }
    public decimal? SoTienThts { get; set; }
}
public partial class sum_hsgd_dx
{
    public decimal sumso_tien_tt_ph_son_gomVAT { get; set; }
    public decimal sumso_tien_giamtru_bt { get; set; }
    public decimal sumso_tien_so_tienggsc { get; set; }
    public decimal sumso_tien_doitru { get; set; }
    public decimal sum_trachnhienpvi { get; set; }
}
public partial class sum_hsgd_dx_tsk
{
    public decimal sumso_tien_tt_sc_gomVAT { get; set; }
    public decimal sumso_tien_giamtru_bt { get; set; }
    public decimal sum_trachnhienpvi { get; set; }
}
