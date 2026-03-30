using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVI.DAO.Entities.Models
{
    [Keyless]
    public class ThongKeGDTT_Item
    {
        public int pr_key { get; set; }
        public decimal pr_key_bt { get; set; }
        public string? ten_donvi { get; set; }
        public string? ten_donvi_tt { get; set; }
        public string ten_khach { get; set; } = string.Empty;
        public string so_donbh { get; set; } = string.Empty;
        public string so_hsgd { get; set; } = string.Empty;
        public decimal so_seri { get; set; }
        public string bien_ksoat { get; set; } = string.Empty;
        public string? hieu_xe { get; set; }
        public string? loai_xe { get; set; }
        public DateTime? ngay_dau_seri { get; set; }
        public DateTime? ngay_cuoi_seri { get; set; }
        public DateTime? ngay_ctu { get; set; }
        public DateTime? ngay_tthat { get; set; }
        public string dia_diemtt { get; set; } = string.Empty;
        public string nguyen_nhan_ttat { get; set; } = string.Empty;
        public int? so_ngaybh { get; set; }
        public int? tb_tt { get; set; }
        public int? ct_nd { get; set; }       
        public DateTime? ngay_huy_hs { get; set; }
        public decimal so_lan_gd { get; set; }
        public string? ma_user { get; set; }
        public decimal so_tienu { get; set; }
        public decimal so_tienp { get; set; }
        public string gdv { get; set; } = string.Empty;
        public string? tinh_trang { get; set; }
        public string ma_lhsbt { get; set; } = string.Empty;
        public string thoi_gian_xly { get; set; } = string.Empty;
        public string hsgd_tpc { get; set; } = string.Empty;
        public string ten_gara { get; set; } = string.Empty;
        public string ten_tat_gara { get; set; } = string.Empty;
        public string ma_gara { get; set; } = string.Empty;
        public string ghi_chu { get; set; } = string.Empty;
        public decimal so_tienUbandau { get; set; }
        public decimal? SoTienGtbt { get; set; }
        public decimal? SoTienGtbtTNDS { get; set; }
        public decimal? SoTienGtbtKhac { get; set; }
        public string ng_lienhe { get; set; } = string.Empty;
        public string dien_thoai { get; set; } = string.Empty;
        public string dien_thoai_ndbh { get; set; } = string.Empty;
        public string ma_lhsbt_new { get; set; } = string.Empty;
        public decimal? sum_tienthaythe { get; set; }
        public decimal? sum_tiensuachua { get; set; }
        public decimal? sum_tienson { get; set; }
        public decimal? sum_sotiendoitru_vcx { get; set; }
        public decimal? sum_sotiendoitru_tnds { get; set; }
        public decimal? st_bl_vcx { get; set; }
        public decimal? st_bl_tnds { get; set; }
        public decimal? so_tienugddx_vcx { get; set; }
        public decimal? tien_pheduyet_vcx { get; set; }
        public decimal? so_tienugddx_tnds_nguoi { get; set; }
        public decimal? tien_pheduyet_tnds_nguoi { get; set; }
        public decimal? so_tienugddx_tnds { get; set; }
        public decimal? tien_pheduyet_tnds { get; set; }
        public decimal? so_tienugddx_khac { get; set; }
        public decimal? tien_pheduyet_khac { get; set; }
        public DateTime? ngay_duyettpc { get; set; }
        public string? cbott { get; set; }
        public string? ngay_bstt { get; set; }
        public string ghi_chudx { get; set; } = string.Empty;
        public string ghi_chudxtt { get; set; } = string.Empty;
        public string ghi_chudx_tnds { get; set; } = string.Empty;
        public string ghi_chudx_tndstt { get; set; } = string.Empty;
        public string ghi_chudx_tsk { get; set; } = string.Empty;
        public string ghi_chudx_tsktt { get; set; } = string.Empty;
        public int? vat { get; set; }
        public decimal? so_tienctkh { get; set; }
        public string? lydo_ctkh { get; set; }
        public int? vat_tnds { get; set; }
        public string? lydo_ctkh_tnds { get; set; }
        public decimal? so_tienctkh_tnds { get; set; }
        public decimal? tylegg_phutungvcx { get; set; }
        public decimal? tylegg_suachuavcx { get; set; }
        public decimal? tylegg_phutungtnds { get; set; }
        public decimal? tylegg_suachuatnds { get; set; }
        public decimal? ggphutungvcx { get; set; }
        public decimal? ggsuachuavcx { get; set; }
        public decimal? ggphutungthds { get; set; }
        public decimal? ggsuachuatnds { get; set; }
        public decimal? so_tienctkh_tsk { get; set; }
        public string? lydo_ctkh_tsk { get; set; }
        public string? ma_nguyen_nhan_ttat { get; set; }
        public string? ten_nguyen_nhan_ttat { get; set; }
        public string ma_dkhoan { get; set; } = string.Empty;        
        public string? ten_loai_dongco { get; set; }
        public decimal sotien_ttpin { get; set; }
        public string ten_cbotrinh { get; set; } = string.Empty;
        public string vai_tro { get; set; } = string.Empty;
        public decimal tyle_tg { get; set; }
        public string so_hsbt { get; set; } = string.Empty;
        public DateTime? ngay_pd_tt { get; set; }
        public string? ten_nguoi_duyet { get; set; }
        public string? nguon_tao { get; set; }
        public string? canbo_pdtt { get; set; }
        public DateTime? ngay_dutlieu { get; set; }

    }

}
