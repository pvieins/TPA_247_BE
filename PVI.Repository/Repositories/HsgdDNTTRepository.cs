using Azure;
using Azure.Core;
using ICSharpCode.SharpZipLib.Core;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using PVI.DAO.Entities.Models;
using PVI.Helper;
using PVI.Repository.Interfaces;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.Text;
using System.util;
using System.Web;
using static Azure.Core.HttpHeader;
using static iTextSharp.text.pdf.events.IndexEvents;
using static PVI.Repository.Repositories.HsgdCtuRepository;
using static PVI.Repository.Repositories.HsgdDxRepository;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PVI.Repository.Repositories
{
    public class HsgdDnttRepository : GenericRepository<HsgdDntt>, IHsgdDnttRepository
    {
        public HsgdDnttRepository(GdttContext context, Pvs2024Context context_pias, Pvs2024UpdateContext context_pias_update, Pvs2024TToanContext context_pias_ttoan, Serilog.ILogger logger, IConfiguration conf) : base(context, context_pias, context_pias_update, context_pias_ttoan, logger, conf)
        {


        }
        
        public string CreateDNTT(DNTTRequest dNTTRequest, string pr_key_hsgd_ttrinh,string email_login)
        {
            string result = "";
            //haipv1 note
            //Số tiền tạo đề nghị thanh toán có hóa đơn là sum tổng số tiền từ hsbt_ct loại trừ !(x.MaSp == "050101" && x.MaDkhoan == "05010101")
            //Trường hợp mỗi 1 dòng trong hsbt_ct lại muốn lập 1 ĐNTT thì chỉ có cách tách tờ trình, nếu để chung thì phải sửa lại số tiền khi lập ĐNTT
            try
            {
                var user_login = _context.DmUsers.Where(x => x.Mail == email_login).FirstOrDefault();
                var list_key = pr_key_hsgd_ttrinh.Split(",").ToList();
                var list_tt = _context.HsgdTtrinhs.Where(x => list_key.Contains(x.PrKey.ToString())).OrderBy(o => o.PrKey).Select(s => new { s.PathTtrinh, s.SoHsbt }).ToList();
                var list_hsbt = _context_pias_update.HsbtCtus.Where(x => list_tt.Select(s => s.SoHsbt).Contains(x.SoHsbt)).Select(s => new
                {
                    PrKey = s.PrKey,
                    SoHsbt = s.SoHsbt,
                    MaDonviTt = !string.IsNullOrEmpty(s.MaDonviTt) ? s.MaDonviTt : s.MaDonvi
                }).ToList();
                //haipv1 dào lại đoạn này do chuyển thanh toán về đơn vị gốc MaDonviTt chỉ là nơi lưu trữ hồ sơ
                //for (int t = 0; t < list_hsbt.Count; t++)
                //{
                //    if (dNTTRequest.ma_donvi != list_hsbt[t].MaDonviTt)
                //    {
                //        return "Tờ trình của hồ sơ bồi thường số " + list_hsbt[t].SoHsbt + " có mã đơn vị thanh toán khác đơn vị thanh toán được chọn.Vui lòng kiểm tra lại!";
                //    }

                //}
                string url_download = _configuration["DownloadSettings:DownloadServer_MDF1"] ?? "";
                var list_hsgd_ttrinh_ct = _context.HsgdTtrinhCts.Where(x => list_key.Contains(x.FrKey.ToString())).Join(_context.HsgdTtrinhs, a => a.FrKey, b => b.PrKey, (a, b) => new { a.FrKey, a.MaSp, b.SoHsbt }).ToList();
                var list_hsbt_ct = _context_pias_update.HsbtCts.Where(x => list_hsbt.Select(s => s.PrKey).ToArray().Contains(x.FrKey)).Join(_context_pias_update.HsbtCtus, a => a.FrKey, b => b.PrKey, (a, b) => new { a.PrKey, a.FrKey, a.MaSp, b.SoHsbt }).ToList();
                var ttoan_ct = ToListWithNoLock((from a in _context_pias_update.HsbtCts.Where(x => !(x.MaSp == "050101" && x.MaDkhoan == "05010101"))
                                                 join c in _context_pias_update.FileAttachBts on a.PrKey equals c.FrKey into c1
                                     from c in c1.DefaultIfEmpty()
                                     where list_hsbt.Select(s => s.PrKey).ToArray().Contains(a.FrKey) && a.NgayHdvat != null
                                     select new TtoanCtRequest
                                     {
                                         pr_key = a.PrKey,
                                         fr_key = a.FrKey,
                                         tsuat_vat = a.MucVatp,
                                         doanh_so_hdon = a.SoTienp,
                                         doanh_so = a.SoTienp,
                                         tien_vat = a.SoTienvp,
                                         tien_vat_hdon = a.SoTienvp,
                                         ma_sovat = a.MasoVat,
                                         ma_kh_vat = a.MaKhvat,
                                         ten_kh_vat = a.TenKhvat,
                                         serie_vat = a.SerieVat,
                                         so_hdvat = a.SoHdvat,
                                         ngay_hdvat = a.NgayHdvat != null ? Convert.ToDateTime(a.NgayHdvat).ToString("dd/MM/yyyy") : DateTime.Today.ToString("dd/MM/yyyy"),
                                         ten_hhoa = a.TenHhoavat,
                                         mau_sovat = a.MauSovat,
                                         duong_dan = c != null ? c.Directory : "",
                                         ten_file = c != null ? c.FileName : ""
                                     }).AsQueryable());
                var list_ttoan_ct = ttoan_ct.GroupBy(n => new { n.pr_key, n.fr_key, n.tsuat_vat, n.doanh_so_hdon, n.doanh_so, n.tien_vat, n.tien_vat_hdon, n.ma_sovat, n.ma_kh_vat, n.ten_kh_vat, n.serie_vat, n.so_hdvat, n.ngay_hdvat, n.ten_hhoa, n.mau_sovat }).Select(s => new TtoanCtRequest
                {
                    tsuat_vat = s.Key.tsuat_vat,
                    doanh_so_hdon = s.Key.doanh_so_hdon,
                    doanh_so = s.Key.doanh_so,
                    tien_vat = s.Key.tien_vat,
                    tien_vat_hdon = s.Key.tien_vat_hdon,
                    ma_sovat = s.Key.ma_sovat,
                    ma_kh_vat = s.Key.ma_kh_vat,
                    ten_kh_vat = s.Key.ten_kh_vat,
                    serie_vat = s.Key.serie_vat,
                    so_hdvat = s.Key.so_hdvat,
                    ngay_hdvat = s.Key.ngay_hdvat,
                    ten_hhoa = s.Key.ten_hhoa,
                    mau_sovat = s.Key.mau_sovat,
                    //duong_dan = string.Join(";", s.Select(x=>x.duong_dan)),
                    //ten_file = string.Join(";", s.Select(x => x.ten_file)),
                    duong_dan = "",
                    ten_file = "",
                    file_attachs = _context_pias_update.FileAttachBts.Where(x => x.FrKey == s.Key.pr_key).Select(s => new FileAttach
                    {
                        file_data = UtilityHelper.DownloadFile_ToAPI(s.Directory, url_download).Data,
                        ten_file = s.FileName
                    }).ToList()
                }
                                  ).ToList();
                var list_hsbt_ct_tt = (from a in list_hsbt_ct
                                       join b in list_hsgd_ttrinh_ct on new { a.SoHsbt, a.MaSp } equals new { b.SoHsbt, b.MaSp }
                                       select a.PrKey
                                       ).ToList();
                var file_attach_bt = _context_pias_update.FileAttachBts.Where(x => list_hsbt_ct_tt.Contains(x.FrKey)).Select(s => new FileAttach
                {
                    file_data = UtilityHelper.DownloadFile_ToAPI(s.Directory, url_download).Data,
                    ten_file = s.FileName
                }).ToList();
                //lấy toàn bộ file trong hsgd_attachfile của từng hồ sơ ghép thành 1 file rồi đính kèm vào đề nghị thanh toán              
                bool ghep_file = MegreFileHSTT(pr_key_hsgd_ttrinh, url_download);
                if(!ghep_file)
                {
                    return "Không tạo được đề nghị thanh toán, Qua trình tạo file thanh toán lỗi!";
                }    
                _logger.Error("MegreFileHSTT 1");
                var list_filett=(from a in _context.HsgdTtrinhs
                                join b in _context.HsgdAttachFiles on a.PrKeyHsgd equals b.FrKey
                                where b.MaCtu == "FileMegre"
                                && list_key.Contains(a.PrKey.ToString())
                                select new
                                {
                                    SoHsbt = a.SoHsbt,
                                    PathFile = b.Directory
                                }).ToList();                   
                var list_tt_res = list_filett.Select((r, i) => new FileAttach
                {
                    file_data = UtilityHelper.DownloadFile_ToAPI(r.PathFile, url_download).Data,
                    ten_file = r.SoHsbt + "_" + (i + 1) + ".pdf"
                }).ToList();
                //dao return này lại khi public
                //return JsonConvert.SerializeObject(list_tt_res);
                //var tt_ct = _context.HsgdTtrinhCts.Where(x => list_key.Contains(x.FrKey.ToString()) && !(x.MaSp == "050101" && x.MaDKhoan == "05010101")).GroupBy(g => 1 == 1).Select(p => new
                //{
                //    SoTienBtVat = p.Sum(x => x.SoTienBtVat),
                //    SoTienBt = Math.Round(p.Sum(x => x.SotienBt), 0, MidpointRounding.AwayFromZero)

                //}).FirstOrDefault();
                var tt_ct = _context.HsgdTtrinhCts.Where(x => list_key.Contains(x.FrKey.ToString())).GroupBy(g => 1 == 1).Select(p => new
                {
                    SoTienBtVat = p.Sum(x => x.SoTienBtVat),
                    SoTienBt = Math.Round(p.Sum(x => x.SotienBt), 0, MidpointRounding.AwayFromZero) 

                }).FirstOrDefault();

                var cbcnv = _context_pias_ttoan.DmUsers.Where(x => x.MaUser == dNTTRequest.ma_cbcnv).FirstOrDefault();
                var cbcnv_xly = _context_pias_ttoan.DmUsers.Where(x => x.MaUser == dNTTRequest.ma_cbcnv_xly).FirstOrDefault();
                if (cbcnv == null)
                {
                    return "Không tồn tại người đề nghị. Vui lòng kiểm tra lại!";
                }
                if (cbcnv_xly == null)
                {
                    return "Không tồn tại người xử lý. Vui lòng kiểm tra lại!";
                }
                else
                {
                    if (string.IsNullOrEmpty(cbcnv_xly.MaPhong))
                    {
                        return "Người xử lý không thuộc mã phòng ban nào. Vui lòng kiểm tra lại!";
                    }
                }
                string url = _configuration["DownloadSettings:url_thanhtoanapi"] ?? "";
                _logger.Information($"CreateDNTT - API URL: {url}");
                string frAcctId = "";
                string frAcctNm = "";
                string ma_donvi = cbcnv_xly != null ? cbcnv_xly.MaDonvi ?? "" : "";
                // Lấy thông tin tài khoản ngân hàng nguồn
                var dm_nhang = _context_pias.DmNhangs
                    .Where(x => x.MaDonviNhang.Contains(ma_donvi)
                             && x.MaTteNhang.Contains("VND")
                             && x.LoaiTaiKhoan == "4")
                    .Select(s => new
                    {
                        frAcctId = s.SoTkNhang,
                        frAcctNm = s.TenTaiKhoan
                    })
                    .FirstOrDefault();

                if (dm_nhang != null)
                {
                    frAcctId = dm_nhang.frAcctId ?? "";
                    frAcctNm = dm_nhang.frAcctNm ?? "";
                }
                else
                {
                    _logger.Error("CreateDNTT: Không tìm thấy tài khoản ngân hàng cho đơn vị: " + ma_donvi);
                }
                DNTTContent dntt = new DNTTContent();
                //dntt.ma_cbo = dNTTRequest.ma_cbcnv;
                dntt.ngay_ctu = dNTTRequest.ngay_ctu;
                dntt.ma_httoan = "HT2";
                dntt.nguoi_huong = dNTTRequest.nguoi_huong;
                dntt.so_tknh = dNTTRequest.so_tknh??"";
                dntt.ten_tknh = dNTTRequest.ten_tknh??"";
                dntt.loai_cphi = dNTTRequest.loai_cpi;
                dntt.ma_user = cbcnv_xly != null ? cbcnv_xly.MaUser ?? "" : ""; 
                dntt.ma_ctu_ttoan = "CT6";
                dntt.ma_cbcnv = cbcnv != null ? cbcnv.MaUser??"" : "";
                dntt.dien_giai = dNTTRequest.dien_giai;
                dntt.ttin_lquan = dNTTRequest.ttin_lquan??"";
                dntt.username = cbcnv_xly != null ? cbcnv_xly.MaUser ?? "" : "";
                dntt.ma_cbcnv_xly = cbcnv_xly != null ? cbcnv_xly.MaUser ?? "" : "";
                dntt.ten_cbcnv_xly = cbcnv_xly != null ? cbcnv_xly.TenUser ?? "" : "";
                dntt.ma_tte = "VND";
                dntt.tygia_ht = 1.00;
                dntt.tygia_tt = 1.00;
                dntt.loai_ttoan = "01";
                dntt.tong_tien_kvat = tt_ct != null ? Decimal.ToDouble(tt_ct.SoTienBtVat) : 0.0;
                dntt.tong_tien = tt_ct != null ? Decimal.ToDouble(tt_ct.SoTienBt) : 0.0;
                //dntt.duong_dan = list_tt_res != null ? string.Join(";", list_tt_res.Select(s=>s.PathTtrinh)) : "";
                //dntt.ten_file = list_tt_res != null ? string.Join(";", list_tt_res.Select(s => s.TenTtrinh)) : "";
                dntt.duong_dan = "";
                dntt.ten_file = "";
                dntt.ds_ttrinh = new List<string>();
                dntt.don_vi = "";
                dntt.nguoi_gdich = "";
                dntt.dia_chi_nh = "";
                dntt.ctu_ktheo = "";
                dntt.ngay_cnhat = "";
                dntt.nhang_code = "";
                dntt.so_ctu = "";
                dntt.han_ttoan = "";
                dntt.ten_nhang_tg = "";
                dntt.code_nhang_tg = "";
                dntt.diachi_nhang_tg = "";
                dntt.diachi_nguoi_th = "";
                dntt.chi_tiet = list_ttoan_ct;
                dntt.pr_key = 0;
                dntt.nguoi_thu_huong_temp = "";
                dntt.file_attachs =  (list_tt_res ?? Enumerable.Empty<FileAttach>()).Union(file_attach_bt ?? Enumerable.Empty<FileAttach>()).ToList();
                dntt.pr_key_luong = "0";
                dntt.ten_bang_luong = "";
                dntt.ma_donvi = cbcnv_xly != null ? cbcnv_xly.MaDonvi??"" : "";
                dntt.ma_pban = cbcnv_xly != null ? cbcnv_xly.MaPhong : "";
                dntt.ma_tthai_ttoan = "01";
                dntt.CpId = "ad5b04d374093366dd7bd7b69ad84151";
                dntt.email = email_login;
                dntt.sign =ContentHelper.MD5("8085d140d1fc47be83cc5ac13c233d1c" + email_login + DateTime.Now.ToString("yyyyMMddHH"));
                dntt.isCtienTheoDS = false;
                dntt.benBnkCd = dNTTRequest.bnkCode ?? "";
                dntt.txAmt = tt_ct != null ? Decimal.ToDouble(tt_ct.SoTienBt) : 0.0;
                dntt.txDesc = dNTTRequest.dien_giai ?? "";
                dntt.frAcctId = frAcctId ?? "";
                dntt.frAcctNm = frAcctNm ?? "";
                dntt.trang_thai_unc = dm_nhang != null ? "0" : "-1";
                var options = new RestClientOptions(url)
                {
                    MaxTimeout = -1,
                };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var client = new RestClient(options);
                var request = new RestRequest("/Main/Save_Ttoan_CSSK", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                var body = JsonConvert.SerializeObject(dntt);
                //_logger.Error("CreateDNTT HsgdDntts body  : " + body);
                request.AddStringBody(body, DataFormat.Json);
                RestResponse response = client.Execute(request);
                CreateDNTTResult response_ev_kyso = JsonConvert.DeserializeObject<CreateDNTTResult>(response.Content);
           
                _logger.Error("CreateDNTT HsgdDntts response : " + JsonConvert.SerializeObject(response.Content));
                _logger.Error("CreateDNTT HsgdDntts response : " + JsonConvert.SerializeObject(response_ev_kyso));
                // insert table hsgd_dntt
                if (response_ev_kyso.code == "200")
                {
                    try
                    {
                        List<HsgdDntt> list_dn = new List<HsgdDntt>();
                        for (int i = 0; i < list_key.Count; i++)
                        {
                            HsgdDntt dn = new HsgdDntt();
                            dn.PrKeyTtoanCtu = decimal.Parse(response_ev_kyso.message.Split(';')[0]);
                            dn.PrKeyTtrinh = Convert.ToDecimal(list_key[i]);
                            dn.PrKeyTtrinhCt = 0;
                            dn.MaCbo = user_login.MaUser ?? "";
                            dn.MaCbcnvXly = cbcnv_xly.MaUser;
                            dn.SoCtu = response_ev_kyso.message.Split(';')[1];
                            list_dn.Add(dn);
                        }
                        _context.HsgdDntts.AddRange(list_dn);
                        _context.SaveChanges();
                        result = "Lưu thành công";
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("CreateDNTT insert HsgdDntts error pr_key_hsgd_ttrinh: "+ pr_key_hsgd_ttrinh +" ex: " + ex.ToString());
                        result = "Lưu thất bại error: " + ex.ToString();
                    }

                }
                else
                {
                    if (false)
                    {
                        _logger.Error("CreateDNTT insert HsgdDntts error : " + body.ToString());
                    }
                    _logger.Error("CreateDNTT insert HsgdDntts error pr_key_hsgd_ttrinh: "+ pr_key_hsgd_ttrinh + ", body: " + body.ToString());
                    result = "Lưu thất bại kyso.code:"+ response_ev_kyso.code;
                }
                
                
            }
            catch (Exception ex)
            {
                _logger.Information("CreateDNTT pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " error: " + ex);
                result = "Lưu thất bại error: " + ex;
            }
            return result;
        }
        public string CreateDNTTKoHoaDon(DNTTRequest dNTTRequest, string pr_key_hsgd_ttrinh, string email_login)
        {
            string result = "";
            //haipv1 note
            //Số tiền tạo đề nghị thanh toán có hóa đơn là sum tổng số tiền từ hsbt_ct loại trừ !(x.MaSp == "050101" && x.MaDkhoan == "05010101")
            //Trường hợp mỗi 1 dòng trong hsbt_ct lại muốn lập 1 ĐNTT thì chỉ có cách tách tờ trình, nếu để chung thì phải sửa lại số tiền khi lập ĐNTT
            try
            {
                var user_login = _context.DmUsers.Where(x => x.Mail == email_login).FirstOrDefault();
                var list_key = pr_key_hsgd_ttrinh.Split(",").ToList();
                var list_tt = _context.HsgdTtrinhs.Where(x => list_key.Contains(x.PrKey.ToString())).OrderBy(o => o.PrKey).Select(s => new { s.PathTtrinh, s.SoHsbt }).ToList();
                var list_hsbt = _context_pias_update.HsbtCtus.Where(x => list_tt.Select(s => s.SoHsbt).Contains(x.SoHsbt)).Select(s => new
                {
                    PrKey = s.PrKey,
                    SoHsbt = s.SoHsbt,
                    MaDonviTt = !string.IsNullOrEmpty(s.MaDonviTt) ? s.MaDonviTt : s.MaDonvi
                }).ToList();
                 
                string url_download = _configuration["DownloadSettings:DownloadServer_MDF1"] ?? "";
                var list_hsgd_ttrinh_ct = _context.HsgdTtrinhCts.Where(x => list_key.Contains(x.FrKey.ToString())).Join(_context.HsgdTtrinhs, a => a.FrKey, b => b.PrKey, (a, b) => new { a.FrKey, a.MaSp, b.SoHsbt }).ToList();
                var list_hsbt_ct = _context_pias_update.HsbtCts.Where(x => list_hsbt.Select(s => s.PrKey).ToArray().Contains(x.FrKey)).Join(_context_pias_update.HsbtCtus, a => a.FrKey, b => b.PrKey, (a, b) => new { a.PrKey, a.FrKey, a.MaSp, b.SoHsbt }).ToList();
                var ttoan_ct = ToListWithNoLock((from a in _context_pias_update.HsbtCts.Where(x => (x.MaSp == "050101" && x.MaDkhoan == "05010101"))
                                                 join c in _context_pias_update.FileAttachBts on a.PrKey equals c.FrKey into c1
                                                 from c in c1.DefaultIfEmpty()
                                                 where list_hsbt.Select(s => s.PrKey).ToArray().Contains(a.FrKey) && a.NgayHdvat != null
                                                 select new TtoanCtRequest
                                                 {
                                                     pr_key = a.PrKey,
                                                     fr_key = a.FrKey,
                                                     tsuat_vat = a.MucVatp,
                                                     doanh_so_hdon = a.SoTienp,
                                                     doanh_so = a.SoTienp,
                                                     tien_vat = a.SoTienvp,
                                                     tien_vat_hdon = a.SoTienvp,
                                                     ma_sovat = a.MasoVat,
                                                     ma_kh_vat = a.MaKhvat,
                                                     ten_kh_vat = a.TenKhvat,
                                                     serie_vat = a.SerieVat,
                                                     so_hdvat = a.SoHdvat,
                                                     ngay_hdvat = a.NgayHdvat != null ? Convert.ToDateTime(a.NgayHdvat).ToString("dd/MM/yyyy") : DateTime.Today.ToString("dd/MM/yyyy"),
                                                     ten_hhoa = a.TenHhoavat,
                                                     mau_sovat = a.MauSovat,
                                                     duong_dan = c != null ? c.Directory : "",
                                                     ten_file = c != null ? c.FileName : ""
                                                 }).AsQueryable());
                var list_ttoan_ct = ttoan_ct.GroupBy(n => new { n.pr_key, n.fr_key, n.tsuat_vat, n.doanh_so_hdon, n.doanh_so, n.tien_vat, n.tien_vat_hdon, n.ma_sovat, n.ma_kh_vat, n.ten_kh_vat, n.serie_vat, n.so_hdvat, n.ngay_hdvat, n.ten_hhoa, n.mau_sovat }).Select(s => new TtoanCtRequest
                {
                    tsuat_vat = s.Key.tsuat_vat,
                    doanh_so_hdon = s.Key.doanh_so_hdon,
                    doanh_so = s.Key.doanh_so,
                    tien_vat = s.Key.tien_vat,
                    tien_vat_hdon = s.Key.tien_vat_hdon,
                    ma_sovat = s.Key.ma_sovat,
                    ma_kh_vat = s.Key.ma_kh_vat,
                    ten_kh_vat = s.Key.ten_kh_vat,
                    serie_vat = s.Key.serie_vat,
                    so_hdvat = s.Key.so_hdvat,
                    ngay_hdvat = s.Key.ngay_hdvat,
                    ten_hhoa = s.Key.ten_hhoa,
                    mau_sovat = s.Key.mau_sovat,                    
                    duong_dan = "",
                    ten_file = "",
                    file_attachs = _context_pias_update.FileAttachBts.Where(x => x.FrKey == s.Key.pr_key).Select(s => new FileAttach
                    {
                        file_data = UtilityHelper.DownloadFile_ToAPI(s.Directory, url_download).Data,
                        ten_file = s.FileName
                    }).ToList()
                }
                                  ).ToList();
                var list_hsbt_ct_tt = (from a in list_hsbt_ct
                                       join b in list_hsgd_ttrinh_ct on new { a.SoHsbt, a.MaSp } equals new { b.SoHsbt, b.MaSp }
                                       select a.PrKey
                                       ).ToList();
                var file_attach_bt = _context_pias_update.FileAttachBts.Where(x => list_hsbt_ct_tt.Contains(x.FrKey)).Select(s => new FileAttach
                {
                    file_data = UtilityHelper.DownloadFile_ToAPI(s.Directory, url_download).Data,
                    ten_file = s.FileName
                }).ToList();
                //lấy toàn bộ file trong hsgd_attachfile của từng hồ sơ ghép thành 1 file rồi đính kèm vào đề nghị thanh toán              
                bool ghep_file = MegreFileHSTT(pr_key_hsgd_ttrinh, url_download);
                if (!ghep_file)
                {
                    return "Không tạo được đề nghị thanh toán, Qua trình tạo file thanh toán lỗi!";
                }
                _logger.Error("MegreFileHSTT 1");
                var list_filett = (from a in _context.HsgdTtrinhs
                                   join b in _context.HsgdAttachFiles on a.PrKeyHsgd equals b.FrKey
                                   where b.MaCtu == "FileMegre"
                                   && list_key.Contains(a.PrKey.ToString())
                                   select new
                                   {
                                       SoHsbt = a.SoHsbt,
                                       PathFile = b.Directory
                                   }).ToList();
                var list_tt_res = list_filett.Select((r, i) => new FileAttach
                {
                    file_data = UtilityHelper.DownloadFile_ToAPI(r.PathFile, url_download).Data,
                    ten_file = r.SoHsbt + "_" + (i + 1) + ".pdf"
                }).ToList();
                //dao return này lại khi public
                //return JsonConvert.SerializeObject(list_tt_res);
                              
                var tt_ct = _context.HsgdTtrinhCts.Where(x => list_key.Contains(x.FrKey.ToString()) && (x.MaSp == "050101" && x.MaDKhoan == "05010101")).GroupBy(g => 1 == 1).Select(p => new
                {
                    SoTienBtVat = p.Sum(x => x.SoTienBtVat),
                    SoTienBt = Math.Round(p.Sum(x => x.SotienBt), 0, MidpointRounding.AwayFromZero)

                }).FirstOrDefault();

                var cbcnv = _context_pias_ttoan.DmUsers.Where(x => x.MaUser == dNTTRequest.ma_cbcnv).FirstOrDefault();
                var cbcnv_xly = _context_pias_ttoan.DmUsers.Where(x => x.MaUser == dNTTRequest.ma_cbcnv_xly).FirstOrDefault();
                if (cbcnv == null)
                {
                    return "Không tồn tại người đề nghị. Vui lòng kiểm tra lại!";
                }
                if (cbcnv_xly == null)
                {
                    return "Không tồn tại người xử lý. Vui lòng kiểm tra lại!";
                }
                else
                {
                    if (string.IsNullOrEmpty(cbcnv_xly.MaPhong))
                    {
                        return "Người xử lý không thuộc mã phòng ban nào. Vui lòng kiểm tra lại!";
                    }
                }
                string url = _configuration["DownloadSettings:url_thanhtoanapi"] ?? "";
                _logger.Information($"CreateDNTT - API URL: {url}");
                string frAcctId = "";
                string frAcctNm = "";
                string ma_donvi = cbcnv_xly != null ? cbcnv_xly.MaDonvi ?? "" : "";
                // Lấy thông tin tài khoản ngân hàng nguồn
                var dm_nhang = _context_pias.DmNhangs
                    .Where(x => x.MaDonviNhang.Contains(ma_donvi)
                             && x.MaTteNhang.Contains("VND")
                             && x.LoaiTaiKhoan == "4")
                    .Select(s => new
                    {
                        frAcctId = s.SoTkNhang,
                        frAcctNm = s.TenTaiKhoan
                    })
                    .FirstOrDefault();

                if (dm_nhang != null)
                {
                    frAcctId = dm_nhang.frAcctId ?? "";
                    frAcctNm = dm_nhang.frAcctNm ?? "";
                }
                else
                {
                    _logger.Error("CreateDNTT: Không tìm thấy tài khoản ngân hàng cho đơn vị: " + ma_donvi);
                }
                DNTTContent dntt = new DNTTContent();
                //dntt.ma_cbo = dNTTRequest.ma_cbcnv;
                dntt.ngay_ctu = dNTTRequest.ngay_ctu;
                dntt.ma_httoan = "HT2";
                dntt.nguoi_huong = dNTTRequest.nguoi_huong;
                dntt.so_tknh = dNTTRequest.so_tknh ?? "";
                dntt.ten_tknh = dNTTRequest.ten_tknh ?? "";
                dntt.loai_cphi = dNTTRequest.loai_cpi;
                dntt.ma_user = cbcnv_xly != null ? cbcnv_xly.MaUser ?? "" : "";
                dntt.ma_ctu_ttoan = "CT2"; //Không có hóa đơn thì lại trở về CT2
                dntt.ma_cbcnv = cbcnv != null ? cbcnv.MaUser ?? "" : "";
                dntt.dien_giai = dNTTRequest.dien_giai;
                dntt.ttin_lquan = dNTTRequest.ttin_lquan ?? "";
                dntt.username = cbcnv_xly != null ? cbcnv_xly.MaUser ?? "" : "";
                dntt.ma_cbcnv_xly = cbcnv_xly != null ? cbcnv_xly.MaUser ?? "" : "";
                dntt.ten_cbcnv_xly = cbcnv_xly != null ? cbcnv_xly.TenUser ?? "" : "";
                dntt.ma_tte = "VND";
                dntt.tygia_ht = 1.00;
                dntt.tygia_tt = 1.00;
                dntt.loai_ttoan = "01";
                dntt.tong_tien_kvat = tt_ct != null ? Decimal.ToDouble(tt_ct.SoTienBtVat) : 0.0;
                dntt.tong_tien = tt_ct != null ? Decimal.ToDouble(tt_ct.SoTienBt) : 0.0;
                dntt.duong_dan = "";
                dntt.ten_file = "";
                dntt.ds_ttrinh = new List<string>();
                dntt.don_vi = "";
                dntt.nguoi_gdich = "";
                dntt.dia_chi_nh = "";
                dntt.ctu_ktheo = "";
                dntt.ngay_cnhat = "";
                dntt.nhang_code = "";
                dntt.so_ctu = "";
                dntt.han_ttoan = "";
                dntt.ten_nhang_tg = "";
                dntt.code_nhang_tg = "";
                dntt.diachi_nhang_tg = "";
                dntt.diachi_nguoi_th = "";
                dntt.chi_tiet = new List<TtoanCtRequest>(); // không có hóa đơn thì không có phần chi tiết này
                dntt.pr_key = 0;
                dntt.nguoi_thu_huong_temp = "";
                dntt.file_attachs = (list_tt_res ?? Enumerable.Empty<FileAttach>()).Union(file_attach_bt ?? Enumerable.Empty<FileAttach>()).ToList();
                dntt.pr_key_luong = "0";
                dntt.ten_bang_luong = "";
                dntt.ma_donvi = cbcnv_xly != null ? cbcnv_xly.MaDonvi ?? "" : "";
                dntt.ma_pban = cbcnv_xly != null ? cbcnv_xly.MaPhong : "";
                dntt.ma_tthai_ttoan = "01";
                dntt.CpId = "ad5b04d374093366dd7bd7b69ad84151";
                dntt.email = email_login;
                dntt.sign = ContentHelper.MD5("8085d140d1fc47be83cc5ac13c233d1c" + email_login + DateTime.Now.ToString("yyyyMMddHH"));
                dntt.isCtienTheoDS = false;
                dntt.benBnkCd = dNTTRequest.bnkCode ?? "";
                dntt.txAmt = tt_ct != null ? Decimal.ToDouble(tt_ct.SoTienBt) : 0.0;
                dntt.txDesc = dNTTRequest.dien_giai ?? "";
                dntt.frAcctId = frAcctId ?? "";
                dntt.frAcctNm = frAcctNm ?? "";
                dntt.trang_thai_unc = dm_nhang != null ? "0" : "-1";
                var options = new RestClientOptions(url)
                {
                    MaxTimeout = -1,
                };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var client = new RestClient(options);
                var request = new RestRequest("/Main/Save_Ttoan_CSSK", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                var body = JsonConvert.SerializeObject(dntt);
                //_logger.Error("CreateDNTT HsgdDntts body  : " + body);
                request.AddStringBody(body, DataFormat.Json);
                RestResponse response = client.Execute(request);
                CreateDNTTResult response_ev_kyso = JsonConvert.DeserializeObject<CreateDNTTResult>(response.Content);

                _logger.Error("CreateDNTT HsgdDntts response : " + JsonConvert.SerializeObject(response.Content));
                _logger.Error("CreateDNTT HsgdDntts response : " + JsonConvert.SerializeObject(response_ev_kyso));
                // insert table hsgd_dntt
                if (response_ev_kyso.code == "200")
                {
                    try
                    {
                        List<HsgdDntt> list_dn = new List<HsgdDntt>();
                        for (int i = 0; i < list_key.Count; i++)
                        {
                            HsgdDntt dn = new HsgdDntt();
                            dn.PrKeyTtoanCtu = decimal.Parse(response_ev_kyso.message.Split(';')[0]);
                            dn.PrKeyTtrinh = Convert.ToDecimal(list_key[i]);
                            dn.PrKeyTtrinhCt = _context.HsgdTtrinhCts.Where(x => list_key.Contains(x.FrKey.ToString()) && x.MaSp == "050101" && x.MaDKhoan == "05010101").Select(x => (decimal?)x.PrKey).FirstOrDefault() ?? 0;
                            dn.MaCbo = user_login.MaUser ?? "";
                            dn.MaCbcnvXly = cbcnv_xly.MaUser;
                            list_dn.Add(dn);
                        }
                        _context.HsgdDntts.AddRange(list_dn);
                        _context.SaveChanges();
                        result = "Lưu thành công";
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("CreateDNTT insert HsgdDntts error : " + ex.ToString());
                        result = "Lưu thất bại";
                    }

                }
                else
                {
                    if (false)
                    {
                        _logger.Error("CreateDNTT insert HsgdDntts error : " + body.ToString());
                    }
                    result = "Lưu thất bại";
                }


            }
            catch (Exception ex)
            {
                _logger.Information("CreateDNTT pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " error: " + ex);
                result = "Lưu thất bại";
            }
            return result;
        }
        public string LayThongTimMDF(string PathFile)
        {
            string url_download = "";
            string url_download_mdf1 = _configuration["DownloadSettings:DownloadServer_MDF1"] ?? "";
            string url_download_mdf3 = _configuration["DownloadSettings:DownloadServer"] ?? "";
            if (PathFile.IndexOf("P247_Upload_New", StringComparison.OrdinalIgnoreCase) >= 0)
               url_download= url_download_mdf3;                             
            else if (PathFile.IndexOf("pias_upload", StringComparison.OrdinalIgnoreCase) >= 0)
               url_download = url_download_mdf1;
            else if (PathFile.IndexOf("cssk_upload", StringComparison.OrdinalIgnoreCase) >= 0)
                url_download = url_download_mdf3; 
            return url_download;
        }
        public Task<List<NguoiDeNghi>> GetListNguoiDeNghi(string ma_donvi)
        {

            var list = _context_pias.DmUserPiases.Where(x=>x.MaDonvi == ma_donvi && x.MaCbo != "").Select(s => new NguoiDeNghi
            {
                MaUser = s.MaUser,
                FullName = s.FullName,
                MaCbo = s.MaCbo
            }).AsQueryable();

            return ToListWithNoLockAsync(list);
        }
        public Task<List<DanhMuc>> GetListDonViTT(string ma_donvi)
        {
            var list_ma_donvi = ma_donvi.Split(",").ToList();
            var list = _context.DmDonvis.Where(x => list_ma_donvi.Contains(x.MaDonvi)).Select(s => new DanhMuc
            {
                MaDM = s.MaDonvi,
                TenDM = s.TenDonvi
            }).AsQueryable();

            return ToListWithNoLockAsync(list);
        }
        public Task<List<DanhMuc>> GetListNhomKT(string ma_donvi)
        {
            var list = _context_pias_ttoan.DmLuongTtoans.Where(x => x.MaDonvi == ma_donvi).Select(s => new DanhMuc
            {
                MaDM = s.LoaiCphi,
                TenDM = s.TenLuongTtoan
            }).AsQueryable();

            return ToListWithNoLockAsync(list);
        }
        public Task<List<ThuHuong>> GetThongtinTKThuHuong(decimal pr_key_hsgd)
        {
            try
            {               
                var LThuHuong = (from a in _context.HsgdCtus
                                join b in _context.HsgdTtrinhs on a.PrKey equals b.PrKeyHsgd
                                join c in _context.HsgdTtrinhTt on b.PrKey equals c.FrKey
                                where a.PrKey == pr_key_hsgd
                                select new ThuHuong
                                {
                                    TenChuTk = c.TenChuTk,
                                    SoTaikhoanNh = c.SoTaikhoanNh,
                                    TenNh = c.TenNh,
                                    LydoTt = c.LydoTt,
                                    SotienTt = c.SotienTt,
                                    bnkCode=c.bnkCode
                                }
                                ).AsQueryable();
                return ToListWithNoLockAsync(LThuHuong);

            }            
            catch (Exception ex)
            {
                _logger.Information("GetThongtinTKThuHuong =" + pr_key_hsgd.ToString() + " error: " + ex);
                 
            }
            return null;
        }
        public Task<List<DanhMuc>> GetListNguoiXuLy(string ma_donvi)
        {
            var url = "http://localhost";
                //_configuration["Jwt:ValidIssuer"] ?? "";
            var options = new RestClientOptions(url)
            {
                MaxTimeout = -1,
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var client = new RestClient(options);
            var request = new RestRequest("/api_iam/ApplicationUsers/GetUser/8085d140d1fc47be83cc5ac13c233d1c/"+ma_donvi+"/INS", Method.Get);
            RestResponse response = client.Execute(request);
            List<EmailIAM> lst = JsonConvert.DeserializeObject<List<EmailIAM>>(response.Content);

            var list = _context_pias_ttoan.DmUsers.Where(x => lst.Select(s=>s.email).Contains(x.DcEmail) && x.MaPhong != "").Select(s => new DanhMuc
            {
                MaDM = s.MaUser,
                TenDM = s.FullName
            }).AsQueryable();

            return ToListWithNoLockAsync(list);
        }
        public PagedList<HsgdDnttView> GetListDntt(string email_login, DnttParameters dnttParameters)
        {
            try
            {
                 var user = _context.DmUsers.Where(x => x.Mail == email_login).FirstOrDefault();
                var query = _context.HsgdDntts.AsQueryable();

                
                if (user!= null && user.LoaiUser != 1)
                {
                    query = query.Where(x => x.MaCbo == user.MaUser);
                }

                var dntt_by_user = ToListWithNoLock(
                    query.GroupBy(g => g.PrKeyTtoanCtu)
                         .Select(s => s.Key)
                         .AsQueryable()
                );
                //var dntt_by_user = ToListWithNoLock(_context.HsgdDntts.Where(x =>x.MaCbo == user.MaUser).GroupBy(g => g.PrKeyTtoanCtu).Select(s => s.Key).AsQueryable());
                if (dntt_by_user != null && dntt_by_user.Count > 0)
                {
                    var data = (from a in _context_pias_ttoan.TtoanCtus
                                where (
                                  dntt_by_user.Contains(a.PrKey) && (a.MaCtuTtoan == "CT2"|| a.MaCtuTtoan == "CT6") && a.LoaiCphi !="" && a.NgayCtu.Value.Year <= DateTime.Now.Year
                                   )
                                select new HsgdDnttView
                                {
                                    PrKey = a.PrKey,
                                    SoCtu = a.SoCtu,
                                    MaCtuTtoan = a.MaCtuTtoan,
                                    NgayCtu = a.NgayCtu,
                                    MaCbcnv = a.MaCbcnv,
                                    MaCbcnvXly = a.MaCbcnvXly,
                                    MaPban = a.MaPban,
                                    NguoiHuong = a.NguoiHuong,
                                    DienGiai = a.DienGiai,
                                    TongTien =  a.TongTien,
                                    MaTte = a.MaTte,
                                    TrangThai = a.TrangThai,
                                    BsCtu = a.BsCtu,
                                    IsCtien = a.IsCtien,
                                    MaHttoan = a.MaHttoan,
                                    LoaiCphi = a.LoaiCphi,
                                    MaDonVi = a.MaDonvi
                                }
                                  ).AsQueryable();
                    if (!string.IsNullOrEmpty(dnttParameters.so_dntt))
                    {
                        data = data.Where(x => x.SoCtu.Contains(dnttParameters.so_dntt));
                    }
                    if (!string.IsNullOrEmpty(dnttParameters.ma_nguoidenghi))
                    {
                        data = data.Where(x => x.MaCbcnv.Contains(dnttParameters.ma_nguoidenghi));
                    }
                    if (!string.IsNullOrEmpty(dnttParameters.ma_nguoipheduyet))
                    {
                        data = data.Where(x => x.MaCbcnvXly.Contains(dnttParameters.ma_nguoipheduyet));
                    }
                    if (!string.IsNullOrEmpty(dnttParameters.ma_trangthai))
                    {
                        data = data.Where(x => x.TrangThai.Contains(dnttParameters.ma_trangthai));
                    }
                    var data_grp = data.OrderByDescending(o => o.PrKey);
                    var page_list = PagedList<HsgdDnttView>.ToPagedList(data_grp, dnttParameters.pageNumber, dnttParameters.pageSize);
                    var cbcnv = ToListWithNoLock(_context_pias_ttoan.DmUsers.Where(x => x.MaCbo != "" && page_list.Select(x => x.MaCbcnv).ToArray().Contains(x.MaCbo)).Select(s => new { s.MaCbo, s.FullName }).AsQueryable());
                    var cbcnvXly = ToListWithNoLock(_context_pias_ttoan.DmUsers.Where(x => page_list.Select(x => x.MaCbcnvXly).ToArray().Contains(x.MaUser)).Select(s => new { s.MaUser, s.FullName }).AsQueryable());
                    var luongttoan = ToListWithNoLock(_context_pias_ttoan.DmLuongTtoans.Where(x => page_list.Select(x => x.LoaiCphi).ToArray().Contains(x.LoaiCphi)).Select(s => new { s.LoaiCphi, s.TenLuongTtoan }).AsQueryable());
                    var pban = ToListWithNoLock(_context_pias_ttoan.DmPbans.Where(x => page_list.Select(x => x.MaPban).ToArray().Contains(x.MaPban)).Select(s => new { s.MaPban, s.TenPban }).AsQueryable());
                    var list_data = page_list.GetRange(0, page_list.Count);
                    var list_data_end = (from a in list_data
                                         join b in cbcnv on a.MaCbcnv equals b.MaCbo into b1
                                         from b in b1.DefaultIfEmpty()
                                         join c in cbcnvXly on a.MaCbcnvXly equals c.MaUser into c1
                                         from c in c1.DefaultIfEmpty()
                                         join d in luongttoan on a.LoaiCphi equals d.LoaiCphi into d1
                                         from d in d1.DefaultIfEmpty()
                                         join e in pban on a.MaPban equals e.MaPban into e1
                                         from e in e1.DefaultIfEmpty()
                                         select new HsgdDnttView
                                         {
                                             PrKey = a.PrKey,
                                             SoCtu = a.SoCtu,
                                             MaCtuTtoan = a.MaCtuTtoan,
                                             NgayCtuText = a.NgayCtu != null ? Convert.ToDateTime(a.NgayCtu).ToString("dd/MM/yyyy") : null,
                                             MaCbcnv = a.MaCbcnv,
                                             TenCbcnv = b != null ? b.FullName : "",
                                             MaCbcnvXly = a.MaCbcnvXly,
                                             TenCbcnvXly = c != null ? c.FullName : "",
                                             MaPban = a.MaPban,
                                             TenPban =  e.TenPban ?? "",
                                             NguoiHuong = a.NguoiHuong,
                                             DienGiai = a.DienGiai,
                                             TongTien = a.TongTien,
                                             MaTte = a.MaTte,
                                             TrangThai = a.TrangThai,
                                             TenTrangThai = Map_trang_thai_dntt(a.TrangThai),
                                             BsCtu = a.BsCtu,
                                             IsCtien = a.IsCtien,
                                             MaDonVi = a.MaDonVi,
                                             TenHttoan = a.MaHttoan == "HT1" ? "Tiền mặt" : (a.MaHttoan == "HT2" ? "Chuyển khoản" : (a.MaHttoan == "HT3" ? "Thanh toán công nợ" : "")),
                                             TenLoaiCphi = d != null ? d.TenLuongTtoan : ""
                                         }).GroupBy(g => g.PrKey).Select(grp => grp.First()).ToList();
                    page_list.RemoveRange(0, page_list.Count);
                    page_list.AddRange(list_data_end);
                    return page_list;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        public static string Map_trang_thai_dntt(string trang_thai)
        {
            string ftrang_thai = "";
            if (trang_thai == "01")
                ftrang_thai = "CB tạo";
            else if (trang_thai == "02")
                ftrang_thai = "Đã chuyển TP";
            else if (trang_thai == "10")
                ftrang_thai = "KT thanh toán xử lý";
            else if (trang_thai == "03")
                ftrang_thai = "Chuyển hạch toán trên PIAS";
            else if (trang_thai == "09")
                ftrang_thai = "KTTT chuyển TP/Trưởng nhóm KT";
            else if (trang_thai == "08")
                ftrang_thai = "KTT duyệt";
            else if (trang_thai == "18")
                ftrang_thai = "Chuyển lãnh đạo phê duyệt";
            else if (trang_thai == "19")
                ftrang_thai = "Lãnh đạo duyệt";
            else if (trang_thai == "14")
                ftrang_thai = "KT viên xử lý";
            else if (trang_thai == "15")
                ftrang_thai = "TP/Trưởng nhóm KT duyệt";
            else if (trang_thai == "16")
                ftrang_thai = "KTTT xử lý";
            else if (trang_thai == "17")
                ftrang_thai = "LĐ Kế toán phê duyệt";
            else if (trang_thai == "0101")
                ftrang_thai = "Trả lại";
            else
                ftrang_thai = "";
            return ftrang_thai;
        }
        public bool MegreFileHSTT(string pr_key_hsgd_ttrinh,string url_download)
        {
            string url_download_mdf3 = _configuration["DownloadSettings:DownloadServer"] ?? "";
            try
            {
                var list_key = pr_key_hsgd_ttrinh.Split(",").ToList();
                var listMaCtu = new List<string> { "TTBT", "PABL", "PASC", "TBBT", "PADG","HSTT" };
                //Xoá file có MaCtu == "FileMegre" ở HsgdAttachFiles để tạo lại
                // Lấy danh sách PrKey cần xóa (FrKey trong HsgdAttachFile)
                _logger.Error("MegreFileHSTT 00 ");
                var listFrKey = _context.HsgdTtrinhs
                    .Where(a => list_key.Contains(a.PrKey.ToString()))
                    .Select(a => a.PrKeyHsgd)
                    .ToList();

                var attachFilesToDelete = _context.HsgdAttachFiles
                .Where(b => b.MaCtu == "FileMegre" && listFrKey.Contains(b.FrKey))
                .ToList();
                // Nếu có dữ liệu thì xóa
                if (attachFilesToDelete.Any())
                {
                    _context.HsgdAttachFiles.RemoveRange(attachFilesToDelete);
                    _context.SaveChanges();
                    _logger.Error("xóa ");
                }
                _logger.Error("MegreFileHSTT 11");
                //Xóa xong thì ghép file
                //Lấy các file thanh toán bao gồm cả file ảnh và file pdf(cũ)
                var list_filett = (from a in _context.HsgdTtrinhs
                                   join b in _context.HsgdAttachFiles on a.PrKeyHsgd equals b.FrKey
                                   where listMaCtu.Contains(b.MaCtu)
                                   && list_key.Contains(a.PrKey.ToString())
                                   orderby b.MaCtu == "TTBT" ? 1 :
                                    b.MaCtu == "PABL" ? 2 :
                                    b.MaCtu == "PASC" ? 3 :
                                    b.MaCtu == "TBBT" ? 4 :
                                    b.MaCtu == "PADG" ? 5 :
                                    b.MaCtu == "HSTT" ? 6 : 7
                                   select new
                                   {
                                       SoHsbt = a.SoHsbt,
                                       PathFile = b.Directory,
                                       FrKey = b.FrKey,
                                       MaCtu = b.MaCtu
                                   }).ToList();
                //LayThongTimMDF(r.PathFile) trả lại url_download theo MDF1 hoăc MDF3 theo PathFile
                var list_tt_res = list_filett.Select((r, i) => new FileAttach
                {
                    file_data = UtilityHelper.DownloadFile_ToAPI(r.PathFile.Replace(@"/",@"\"), LayThongTimMDF(r.PathFile)).Data,
                    ten_file = r.SoHsbt,
                    kich_co = Convert.ToInt32(r.FrKey)

                }).ToList();
                _logger.Error("MegreFileHSTT 12");
                #region dào do đẩy trực tiếp bảo lãnh và phương án sửa chữa lúc ký
                //  // lấy Pasc pdf
                //  var file_pasc_bx = (from a in _context.HsgdCtus
                //                join b in _context.HsgdDxCts on a.PrKeyBt equals b.PrKeyHsbtCtu
                //                where listFrKey.Contains(a.PrKey) && b.PathPasc != ""
                //                orderby a.PrKey descending
                //                select new
                //                {
                //                    SoHsbt = _context.HsgdTtrinhs
                //                                     .Where(t => t.PrKeyHsgd == a.PrKey)
                //                                     .Select(t => t.SoHsbt)
                //                                     .FirstOrDefault(),  
                //                    PathPasc = b.PathPasc,
                //                    PrKey = a.PrKey
                //                })
                //.ToList();
                //  var list_pasc_bx = file_pasc_bx.Select((r, i) => new FileAttach
                //  {
                //      file_data = UtilityHelper.DownloadFile_ToAPI(r.PathPasc, url_download_mdf3).Data,
                //      ten_file = r.SoHsbt,
                //      kich_co = Convert.ToInt32(r.PrKey)

                //  }).ToList();
                //  _logger.Error("MegreFileHSTT 13");
                //  var all_files1 = (list_tt_res ?? Enumerable.Empty<FileAttach>()).Concat(list_pasc_bx ?? Enumerable.Empty<FileAttach>()).ToList();

                //  // lấy bảo lãnh pdf
                //  var file_baolanh = (from a in _context.HsgdCtus
                //                      join b in _context.HsgdDxCts on a.PrKeyBt equals b.PrKeyHsbtCtu
                //                      where listFrKey.Contains(a.PrKey) && b.PathBaolanh != ""
                //                      orderby a.PrKey descending
                //                      select new
                //                      {
                //                          SoHsbt = _context.HsgdTtrinhs
                //                                           .Where(t => t.PrKeyHsgd == a.PrKey)
                //                                           .Select(t => t.SoHsbt)
                //                                           .FirstOrDefault(),
                //                          Path_baolanh = b.PathBaolanh,
                //                          PrKey = a.PrKey
                //                      })
                //.ToList();
                //  var list_baolanh = file_baolanh.Select((r, i) => new FileAttach
                //  {
                //      file_data = UtilityHelper.DownloadFile_ToAPI(r.Path_baolanh, url_download_mdf3).Data,
                //      ten_file = r.SoHsbt,
                //      kich_co = Convert.ToInt32(r.PrKey)

                //  }).ToList();
                //  _logger.Error("MegreFileHSTT 14");
                //  var all_files2 = (all_files1 ?? Enumerable.Empty<FileAttach>()).Concat(list_baolanh ?? Enumerable.Empty<FileAttach>()).ToList();
                #endregion
                _logger.Error("MegreFileHSTT 22 ");
                var groupedFiles = list_tt_res
                .GroupBy(x => new { x.kich_co, x.ten_file }) // gom theo kich_co & sohsbt
                .ToList();
                _logger.Error("MegreFileHSTT 33 ");
                var utilityHelper = new UtilityHelper(_logger);
                foreach (var group in groupedFiles)
                {
                    _logger.Error("MegreFileHSTT 44 ");
                    // Lấy danh sách các file base64 trong nhóm
                    var pdfFilesBase64 = group.Select(g => g.file_data).ToList();                   
                   // Ghép các file ảnh và pdf thành 1 file base64 duy nhất
                   //var mergedFileBase64 = UtilityHelper.MergePdfBase64Files(pdfFilesBase64);
                    var mergedFileBase64 = utilityHelper.MergeBase64FilesToPdf(pdfFilesBase64);                   
                    string folderUpload = _configuration["UploadSettings:FolderUpload_MDF1"] ?? "";
                    string url_upload = _configuration["DownloadSettings:UlpoadServer_MDF1"] ?? "";               
                    var file_path = utilityHelper.UploadFile_ToAPI(mergedFileBase64, ".pdf", folderUpload, url_upload, false);
                    //_logger.Error("MegreFileHSTT Lấy danh sách PrKey cần xóa " + file_path);
                    if (!string.IsNullOrEmpty(file_path))
                    {
                        // Tạo bản ghi HsgdAttachFile mới
                        var attach = new HsgdAttachFile
                        {
                            PrKey = Guid.NewGuid().ToString().ToLower(),
                            FrKey = group.Key.kich_co,
                            MaCtu = "FileMegre",
                            FileName = group.Key.ten_file + "_merged.pdf",
                            Directory = file_path, // đường dẫn từ API upload trả về
                            ngay_cnhat = DateTime.Now,
                            GhiChu = "File PDF gộp tự động",
                            NguonTao = "WebPvi247"
                        };
                        _context.HsgdAttachFiles.Add(attach);
                    }

                }                
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("MegreFileHSTT lỗi " + ex.Message.ToString());
                return false;
            }
            
            return true;
        }
        public async Task<string> DeleteDntt(string pr_key_dntt)
        {
            string result = "";
            if (string.IsNullOrEmpty(pr_key_dntt))
            {
                return "Chưa chọn đơn vị thanh toán. Vui lòng kiểm tra lại!";
            }
            var list_dntt = pr_key_dntt.Split(",").ToList();
            var ttoan_check = _context_pias_ttoan.TtoanCtus.Where(x => list_dntt.Contains(x.PrKey.ToString()) && x.TrangThai != "01").ToList();
            if (ttoan_check != null && ttoan_check.Count > 0)
            {
                return "Chỉ được xóa ĐNTT ở trạng thái 01. Vui lòng kiểm tra lại!";
            }
            using var contextnew = new GdttContext();
            using var dbContextTransaction = contextnew.Database.BeginTransaction();

            await using var context_pias_ttoan_new = new Pvs2024TToanContext();
            await using var dbContextTransaction2 = await context_pias_ttoan_new.Database.BeginTransactionAsync();
            try
            {
                var ttoan_nky = context_pias_ttoan_new.TtoanNhatkies.Where(x => list_dntt.Contains(x.FrKey.ToString())).ToList();
                context_pias_ttoan_new.TtoanNhatkies.RemoveRange(ttoan_nky);
                var ttoan_ct = context_pias_ttoan_new.TtoanCts.Where(x => list_dntt.Contains(x.FrKey.ToString())).ToList();
                context_pias_ttoan_new.TtoanCts.RemoveRange(ttoan_ct);
                var ttoan_ctu = context_pias_ttoan_new.TtoanCtus.Where(x => list_dntt.Contains(x.PrKey.ToString())).ToList();
                context_pias_ttoan_new.TtoanCtus.RemoveRange(ttoan_ctu);

                var hsgd_dntt = contextnew.HsgdDntts.Where(x => list_dntt.Contains(x.PrKeyTtoanCtu.ToString())).ToList();
                contextnew.HsgdDntts.RemoveRange(hsgd_dntt);

                await context_pias_ttoan_new.SaveChangesAsync();
                await dbContextTransaction2.CommitAsync();

                await contextnew.SaveChangesAsync();
                await dbContextTransaction.CommitAsync();

                result = "Xoá thành công";
            }
            catch (Exception ex)
            {
                result = "Xoá thất bại";
                _logger.Error("DeleteDntt Exception : " + ex.ToString());
                _logger.Error("DeleteDntt Error record  pr_key_dntt =" + pr_key_dntt);
                await dbContextTransaction2.RollbackAsync();
                await dbContextTransaction2.DisposeAsync();
                await dbContextTransaction.RollbackAsync();
                await dbContextTransaction.DisposeAsync();
                throw;
            }
            return result;
        }
        public List<LichSuPheDuyet>? GetLichSuPheDuyet(decimal pr_key_ttoan_ctu)
        {
            try
            {
                var nk1 = ToListWithNoLock((from a in _context_pias_ttoan.TtoanNhatkies
                                            join b in _context_pias_ttoan.DmUsers on a.UserChuyen equals b.MaUser into b1
                                            from b in b1.DefaultIfEmpty()
                                            join c in _context_pias_ttoan.DmUsers on a.UserNhan equals c.MaUser into c1
                                            from c in c1.DefaultIfEmpty()
                                            join d in _context_pias_ttoan.TtoanCtus on a.FrKey equals d.PrKey
                                            where a.FrKey == pr_key_ttoan_ctu
                                            select new LichSuPheDuyet
                                            {
                                                TrangThai = a.TrangThai,
                                                NgayCnhat = a.NgayCnhat,
                                                TenUser = c.TenUser,
                                                UserNhan = a.UserNhan,
                                                TenUserNhan = c.FullName,
                                                GhiChu = a.GhiChu,
                                                OrderId = a.OrderId
                                            }).AsQueryable());
                var nk2 = ToListWithNoLock((from a in _context_pias_ttoan.TtoanCtus
                                            join b in _context_pias_ttoan.Ktps on a.PrKeyKtps equals b.PrKey
                                            join c in _context_pias_ttoan.NhatKies on b.MaCtu + "_" + b.PrKey.ToString() equals c.PrKeyCtu
                                            join d in _context_pias_ttoan.DmUsers on c.TenUser equals d.TenUser
                                            where a.PrKey == pr_key_ttoan_ctu
                                            group new { a, b, c, d } by new { c.TenUser, d.MaUser, d.FullName, b.MaCtu, b.SoCtu } into g
                                            select new LichSuPheDuyet
                                            {
                                                TrangThai = "03",
                                                NgayCnhat = g.Max(s => s.a.NgayCnhat),
                                                TenUser = g.Key.TenUser,
                                                UserNhan = g.Key.MaUser,
                                                TenUserNhan = g.Key.FullName,
                                                GhiChu = "Hạch toán kế toán tại chứng từ " + g.Key.MaCtu + "-" + g.Key.SoCtu,
                                                OrderId = 0
                                            }).AsQueryable());
                var nk = nk1.Union(nk2).OrderByDescending(o => o.NgayCnhat).ToList();
                return nk;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public Task<List<NguoiDeNghi>> GetListCanBoTT(string ma_donvi)
        {

            var list = _context.DmUserTtoans.Where(x => x.MaDonvi == ma_donvi).Select(s => new NguoiDeNghi
            {
                MaUser = s.MaUser,
                FullName = s.FullName,
                DcEmail = s.DcEmail
            }).AsQueryable();

            return ToListWithNoLockAsync(list);
        }
    }
}