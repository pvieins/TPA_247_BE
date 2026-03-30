using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVI.Helper
{
    public class ToTrinhParameters : QueryStringParameters
    {
        public int nam_dulieu {  get; set; }
        public string ma_ttrang_tt { get; set; } = null!;
        public string? so_hsbt { get; set; } = null!;
        public string? ma_donvi { get; set; } = null!;
        public string? so_hsgd { get; set; } = null!;
        public string? ma_gdv { get; set; } = null!;
        public string? ma_nguoiduyet { get; set; } = null!;
        public string?  ten_ndbh { get; set; } = null!;
        public string? ngay_nhap { get; set; } = null!;
        public string? bien_ks { get; set; } = null!;
        public string? ngay_tt { get; set; } = null!;
        public decimal? so_tien { get; set; } = null!;
        public string? ma_trangthai { get; set; } = null!;
        public string? ma_ttrang_gd {  get; set; } = null!;
        public string? ngay_duyet { get; set; } = null!;
        public int Chuatao_dntt { get; set; } = 0;
        public string? ngay_psinh { get; set; } = null!;

        public string? nguoiThuHuong { get; set; } = null!;
    }
    public class DnttParameters : QueryStringParameters
    {
        public string? so_dntt { get; set; } = null!;
        public string? ma_nguoidenghi { get; set; } = null!;
        public string? ma_nguoipheduyet { get; set; } = null!;
        public string? ma_trangthai { get; set; } = null!;

    }
    public class DmGaraFilter : QueryStringParameters
    {
        public string? maGara { get; set; } = null!;
        public string? tenGara { get; set; } = null!;
        public string? tenTat { get; set; } = null!;
        public string? diaChi { get; set; } = null!;
        public string? diaChiXuong { get; set; } = null!;

        public string? tenTinh { get; set; } = null!;
        public string? quanHuyen { get; set; } = null!;

        public decimal? tyleggPhutung { get; set; } = null!;
        public decimal? tyleggSuachua { get; set; } = null!;
        public string? emailGara { get; set; } = null!;
        public string? dienthoaiGara { get; set; } = null!;
        public DateTime? ngayCnhat { get; set; } = null!;
        public string? MasoVat { get; set; } = null!;

        public bool? thoaThuanHopTac { get; set; }
    }
    public class CreateDNTTResult
    {
        public string code { get; set; }
        public string message { get; set; }

    }
    public class EmailIAM
    {
        public string userId { get; set; }
        public string email { get; set; }
        public string userName { get; set; }

    }
    public class FileAttach
    {
        public string ten_file { get; set; }
        public string duong_dan { get; set; }
        public int kich_co { get; set; }
        public string file_data { get; set; }
    }
    public class DNTTContent
    {
        public string ma_cbo { get; set; } = null!;
        public string ngay_ctu { get; set; } = null!;
        public string don_vi { get; set; } = null!;
        public string nguoi_gdich { get; set; } = null!;
        public string ma_httoan { get; set; } = null!;
        public string nguoi_huong { get; set; } = null!;
        public string so_tknh { get; set; } = null!;
        public string ten_tknh { get; set; } = null!;
        public string dia_chi_nh { get; set; } = null!;
        public string ttin_lquan { get; set; } = null!;
        public string ctu_ktheo { get; set; } = null!;
        public string ma_user { get; set; } = null!;
        public string ngay_cnhat { get; set; } = null!;
        public string nhang_code { get; set; } = null!;
        public string so_ctu { get; set; } = null!;
        public double tong_tien_kvat { get; set; }
        public string han_ttoan { get; set; } = null!;
        public string ten_nhang_tg { get; set; } = null!;
        public string code_nhang_tg { get; set; } = null!;
        public string diachi_nhang_tg { get; set; } = null!;
        public string diachi_nguoi_th { get; set; } = null!;
        public List<TtoanCtRequest> chi_tiet { get; set; } = null!;
        public int pr_key { get; set; }
        public string ma_ctu_ttoan { get; set; } = null!;
        public string ten_cbcnv_xly { get; set; } = null!;
        public string ma_cbcnv { get; set; } = null!;
        public string ma_tte { get; set; } = null!;
        public double tygia_tt { get; set; }
        public double tygia_ht { get; set; }
        public string loai_cphi { get; set; } = null!;
        public string nguoi_thu_huong_temp { get; set; } = null!;
        public List<string> ds_ttrinh { get; set; } = null!;
        public string dien_giai { get; set; } = null!;
        public List<FileAttach> file_attachs { get; set; } = null!;
        public double tong_tien { get; set; }
        public string pr_key_luong { get; set; } = null!;
        public string ten_bang_luong { get; set; } = null!;
        public string ma_donvi { get; set; } = null!;
        public string ma_pban { get; set; } = null!;
        public string loai_ttoan { get; set; } = null!;
        public string duong_dan { get; set; } = null!;
        public string ten_file { get; set; } = null!;
        public string username { get; set; } = null!;
        public string ma_cbcnv_xly { get; set; } = null!;
        public string ma_tthai_ttoan { get; set; } = null!;
        public string CpId { get; set; } = null!;
        public string email { get; set; } = null!;  //Email PVI
        public string sign { get; set; } = null!;  //MD5(gAppId + email + DateTime.Now.ToString("yyyyMMddHH")); gAppId = 8085d140d1fc47be83cc5ac13c233d1c
        public bool isCtienTheoDS { get; set; }
        public List<TtoanUncRequest> ttoan_unc { get; set; }
        public string benBnkCd { get; set; }     // Mã ngân hàng người nhận
        public double txAmt { get; set; }        // Số tiền
        public string txDesc { get; set; }       // Diễn giải
        public string frAcctId { get; set; }     // Số tài khoản nguồn
        public string frAcctNm { get; set; }     // Tên tài khoản nguồn
        public string trang_thai_unc { get; set; } // Trạng thái
    }
    public class TtoanUncRequest
    {
        public string ToAccNm { get; set; }      // Tên tài khoản người nhận
        public string benBnkCd { get; set; }     // Mã ngân hàng người nhận
        public string ToAccId { get; set; }      // Số tài khoản người nhận
        public double txAmt { get; set; }        // Số tiền
        public string txDesc { get; set; }       // Diễn giải
        public string frAcctId { get; set; }     // Số tài khoản nguồn
        public string frAcctNm { get; set; }     // Tên tài khoản nguồn
        public string trang_thai_unc { get; set; } // Trạng thái
    }
    public partial class TtoanCtRequest
    {
        public decimal pr_key { get; set; }

        public decimal fr_key { get; set; }

        public decimal doanh_so { get; set; }

        public decimal tsuat_vat { get; set; }

        public decimal tien_vat { get; set; }

        public string? ma_sovat { get; set; }

        public string ma_kh_vat { get; set; } = null!;

        public string? ten_kh_vat { get; set; }

        public string serie_vat { get; set; } = null!;

        public string so_hdvat { get; set; } = null!;

        public string? ngay_hdvat { get; set; }

        public string ten_hhoa { get; set; } = null!;

        public string mau_sovat { get; set; } = null!;

        public string ghi_chu { get; set; } = null!;

        public string? ten_file { get; set; }

        public string? duong_dan { get; set; }

        public string kich_co { get; set; } = null!;

        public decimal doanh_so_hdon { get; set; }

        public decimal tien_vat_hdon { get; set; }

        public string ma_hdong { get; set; } = null!;

        public string hdong_json { get; set; } = null!;

        public bool isXML { get; set; }

        public string isValid { get; set; } = null!;

        public string msg_Valid { get; set; } = null!;

        public string? msg_Valid2 { get; set; }
        public List<FileAttach> file_attachs { get; set; } = null!;
    }
    public class UploadToTrinhTPC
    {
        public decimal PrKeyHsgdCtu { get; set; }
        public int LoaiTotrinhTpc { get; set; }
        public FileToTrinhTPC fileTT { get; set; }
    }
    public partial class FileToTrinhTPC
    {
        public string FileName { get; set; } = null!;
        public string Directory { get; set; } = null!;
        public string FileData { get; set; } = null!;
        public string FileExtension { get; set; } = null!;
    }
}
