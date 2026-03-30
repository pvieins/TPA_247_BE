namespace PVI.Helper
{
    public class EntityContent
    {
        public string KeyCode { get; set; }
        public string Value { get; set; }
    }
    public partial class pasc_detail
    {
        public int pr_key_dx { get; set; }
        public string? ma_hmuc { get; set; }
        public string? ten_hmuc { get; set; }
        public decimal so_tientt { get; set; }
        public decimal so_tienph { get; set; }
        public decimal so_tienson { get; set; }
        public int vat_sc { get; set; }
        public int giam_tru_bt { get; set; }
        public bool thu_hoi_ts { get; set; }
        public decimal vat_so_tientt { get; set; }
        public decimal vat_so_tienph { get; set; }
        public decimal vatso_tienson { get; set; }
        public decimal so_tientt_gomVAT { get; set; }
        public decimal so_tienph_gomVAT { get; set; }
        public decimal so_tienson_gomVAT { get; set; }
        public string? ghi_chudv { get; set; }
        public decimal so_tien_vat { get; set; }
        public decimal sum_tt_ph_son_gomVAT { get; set; }
        public decimal sum_giamtru_bt { get; set; }
        public decimal sum_so_tienggsc { get; set; }
        //  dùng cho phương án sửa chữa khác
        public decimal so_tiensc { get; set; }
        public decimal vat_so_tiensc { get; set; }
        public decimal so_tiensc_gomVAT { get; set; }
        public decimal sum_tt_sc_gomVAT { get; set; }
        public decimal so_tiendoitru { get; set; }

    }
    public partial class tt_giamdinh
    {
        public string? cty_gdinh { get; set; }
       
        public decimal sotien_gdinh { get; set; }

    }
    public partial class ThuHuong
    {
        public string? TenChuTk { get; set; }
        public string? SoTaikhoanNh { get; set; }
        public string? TenNh { get; set; }
        public string? LydoTt { get; set; }
        public decimal? SotienTt { get; set; }
        public string? bnkCode { get; set; }
    }
    public partial class TTPrintPasc
    {
        public string LBL_VP { get; set; } = null!;
        public string LBL_DEXUAT { get; set; } = null!;
        public string LBL_NG_KY { get; set; } = null!;
        public string LBL_PHONGGQ { get; set; } = null!;
        public string LBL_GDV { get; set; } = null!;
        public string SO_HSGD { get; set; } = null!;
        public string TEN_KHACH { get; set; } = null!;

        public string DIEN_THOAI { get; set; } = null!;

        public string BIEN_KSOAT { get; set; } = null!;

        public string NGAY_DAU { get; set; } = null!;

        public string NGAY_CUOI { get; set; } = null!;

        public string SO_SERI { get; set; } = null!;

        public string NGAY_TTHAT { get; set; } = null!;

        public string NGAY_TBAO { get; set; } = null!;

        public string NGUYEN_NHANTT { get; set; } = null!;

        public string HIEU_XE { get; set; } = null!;

        public string LOAI_XE { get; set; } = null!;

        public string XUAT_XU { get; set; } = null!;

        public string NAM_SX { get; set; } = null!;

        public string TEN_GARA { get; set; } = null!;

        public string TEN_GARA01 { get; set; } = null!;

        public string TEN_GARA02 { get; set; } = null!;

        public string LYDO_CTKH { get; set; } = null!;

        public string DOITUONGTT_TNDS { get; set; } = null!;

        public string LABEL_TRACHNHIEMPVI { get; set; } = null!;

        public string LABEL_TONGCHIPHI { get; set; } = null!;

        public decimal SUMSO_TIEN_TT_PH_SON_GOMVAT { get; set; }

        public decimal SUMSO_TIENGGSC { get; set; }

        public decimal SO_TIENCTKH { get; set; }

        public decimal SO_TIENGIAMTRUBT { get; set; }
        public decimal SO_TIENDOITRUBT { get; set; }

        public decimal SUM_TRACHNHIEMPVI { get; set; }

        public string SUM_TRACHNHIEMPVI_BC { get; set; } = null!;

        public int SV_CBT { get; set; }

        public decimal SOTIEN_UBT { get; set; }

        public int SV_BT { get; set; }

        public decimal SOTIEN_BT { get; set; }

        public string MAUSER_DUYET { get; set; } = null!;

        public string TENUSER_DUYET { get; set; } = null!;

        public string MAUSER_CCHOPD { get; set; } = null!;

        public string TENUSER_CCHOPD { get; set; } = null!;

        public string MAUSER_GDV { get; set; } = null!;

        public string TENUSER_GDV { get; set; } = null!;

        public List<pasc_detail> list_pasc_detail { get; set; }
    }
}
