using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using PdfSharpCore.Pdf.Advanced;
using PVI.DAO.Entities.Models;
using PVI.Helper;
using PVI.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Configuration;

using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using Microsoft.Office.Interop.Word;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Net.Mime;
using Newtonsoft.Json;
using System.Globalization;
using System.Runtime.InteropServices.JavaScript;
using iTextSharp.text.pdf.parser.clipper;
using iTextSharp.text;

// HSGDCTUHELPER: Do file HSGD CTU rất lớn và cần rất nhiều function hỗ trợ, các function hỗ trợ sẽ được tổng hợp hết vào file helper này.


namespace PVI.Repository.Repositories
{
    // Response khi lấy nhật ký về.
    public class DiaryResponse
    {
        public Dictionary<string, DmUserView> users { get; set; }
        public PagedList<NhatKy> data { get; set; }
    }

    // Response khi lấy đơn vị thanh toán:

    public class DonViThanhToanResponse
    {
        public string? donViThanhToan { get; set; } = null!;
        public string? maDonVi { get; set; } = null!;
        public string? tenDonVi { get; set; } = null!;
        public string? maSoThue { get; set; } = null!;
        public string? diaChi { get; set; } = null!;
    }
    public class LuuThongBaoBTResponse
    {
        public int PrKeyHsgd { get; set; }
        public decimal SumTienTNDSChuXe { get; set; }
        public HsgdTbbt HsgdTbbt { get; set; } = new();
        public List<HsgdTbbtTt> HsgdTbbtTt { get; set; } = new();
    }
    // Form này dùng trong mục bảo lãnh, để tính toán tiền bảo lãnh và đưa vào form.
    public class SoTienBaoLanh
    {
        public decimal? tongSoTienGomVAT { get; set; } = 0;
        public decimal? soTienGiamTru { get; set; } = 0;
        public decimal? soTienDoiTru { get; set; } = 0;
        public decimal? soTienGiamGia { get; set; } = 0;
        public decimal? soTienKhauTru { get; set; } = 0;
        public decimal? soTienGtBt { get; set; } = 0;
        public decimal? trachNhiemPVI { get; set; } = 0;
    }

    // Form này chuyên để tạo PDF bảo lãnh.
    public class FormBaoLanh
    {
        public DateTime ngayTao = DateTime.Now;
        public string maUser { get; set; } = "";
        public string tenUser { get; set; } = "";
        public string maDonvi { get; set; } = "";
        public string soHsgd { get; set; } = "";
        public string nguoiNhan { get; set; } = "";
        public string bienKiemSoat { get; set; } = "";
        public string tenKhach { get; set; } = "";
        public string donViBaoHiem { get; set; } = "";
        public string giamDinhVien { get; set; } = "";
        public string donViGiamDinhVien { get; set; } = "";
        public string tongSoTienGomVAT { get; set; }
        public string soTienGiamGia { get; set; }
        public string soTienGiamTru { get; set; }
        public string soTienKhauTru { get; set; }
        public string lyDoKhauTru { get; set; } = "";
        public string soTienDoiTru { get; set; }
        public string soTienTrachNhiemPVI { get; set; }
        public string soTienTrachNhiemPVIBangChu { get; set; } = "";
        public string donViThanhToan { get; set; } = "";
        public string maSoThue { get; set; } = "";
        public string diaChi { get; set; } = "";
        public decimal bl1 { get; set; } = 0;
        public decimal bl2 { get; set; } = 0;
        public decimal bl3 { get; set; } = 0;
        public decimal bl4 { get; set; } = 0;
        public decimal bl5 { get; set; } = 0;
        public decimal bl6 { get; set; } = 0;
        public decimal bl7 { get; set; } = 0;
        public decimal bl8 { get; set; } = 0;
        public decimal bl9 { get; set; } = 0;
        public string taiLieuCanBoSung { get; set; } = "";
        public string tenKy { get; set; } = "";
        public string maUserDuyetBl { get; set; } = "";
        public string tenUserDuyetBl { get; set; } = "";

        // Các phần dưới đây được sử dụng cho email bảo lãnh:
        public string TenGara { get; set; } = "";

        // Tất cả được cast về string, format từ đầu.
        public string NgayDau { get; set; } = "";
        public string NgayCuoi { get; set; } = "";
        public string NgayTThat { get; set; } = "";
        public string NgayTBao { get; set; } = "";
        public string NguyenNhan { get; set; } = "";
    }

    public class ReloadSumChecker
    {
        public List<HsbtCtView> HSBTView { get; set; } = new List<HsbtCtView>();
        public List<List<HsgdDxSum>> ReloadSum { get; set; } = new List<List<HsgdDxSum>>();
    }

    public class HsgdCtuHelper : GenericRepository<HsgdCtu>
    {
        public HsgdDxRepository _dx_Repo = null;

        public HsgdCtuHelper(GdttContext context, Pvs2024Context context_pias, Pvs2024UpdateContext context_pias_update, Serilog.ILogger logger, IConfiguration conf) : base(context, context_pias, context_pias_update, logger, conf)
        {
            _dx_Repo = new HsgdDxRepository(context, context_pias, context_pias_update, logger, conf);
        }


        // Các method helper:

        // Sử dụng để kiểm tra xem liệu chứng từ đã nhập phương án sửa chữa hay chưa.
        // Dựa theo base code cũ.
        public bool checkCreatedPASC(HsgdCtu hoSoGiamDinh)
        {
            try
            {
                bool finalResult = false;

                // Bước 1: Lấy danh sách các chi tiết bồi thường dựa theo PrKey bồi thường của hồ sơ.
                List<HsbtCt> chiTietBoiThuong = _context_pias.HsbtCts.Where(x => x.FrKey == hoSoGiamDinh.PrKeyBt).ToList();

                // Bước 2: Với mỗi chi tiết bồi thường đã lấy:
                if (chiTietBoiThuong != null && chiTietBoiThuong.Count > 0)
                {
                    chiTietBoiThuong.ForEach(item => {

                        // Kiểm tra nếu có tồn tại hsgdDx thì mới chấp nhận là đã tạo PASC.                        
                        List<HsgdDx> list_dx = (from hsgdDx in _context.HsgdDxes
                                                join hsgdDxCt in _context.HsgdDxCts on hsgdDx.PrKeyDx equals hsgdDxCt.PrKey
                                                where hsgdDxCt.PrKeyHsbtCt == item.PrKey
                                                select new HsgdDx
                                                {
                                                    PrKey = hsgdDx.PrKey
                                                }).ToList();
                        finalResult = list_dx.Count > 0;
                    });
                }
                return finalResult;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return false;
            }
        }

        // Các method helper:

        // Sử dụng để kiểm tra xem liệu hồ sơ đã nhập tờ trình chưa
        // Dựa theo base code cũ.
        public bool checkCreatedTTrinh(HsgdCtu hoSoGiamDinh)
        {
            try
            {

                // Bước 1: Lấy danh sách các chi tiết bồi thường dựa theo PrKey bồi thường của hồ sơ.
                List<HsgdTtrinh> ListTtrinh = _context.HsgdTtrinhs.Where(x => x.PrKeyHsgd == hoSoGiamDinh.PrKey).ToList();

                return ListTtrinh.Count > 0;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return false;
            }
        }


        public string ModifyTargetDirectory(string dir_source)
        {
            string dir_target = dir_source.Replace("pvi.com.vn", "192.168.250.77");

            if (dir_source.IndexOf("CSSK_upload", StringComparison.OrdinalIgnoreCase) > -1)
            {
                dir_target = dir_target.Replace("DATA\\", "P247_Upload_New\\")
                                       .Replace("data\\", "P247_Upload_New\\")
                                       .Replace("CSSK_upload\\", "TCD\\CLAIM_XCG\\")
                                       .Replace("cssk_upload\\", "TCD\\CLAIM_XCG\\")
                                       .Replace("\\pvi\\data\\GCNDT_Upload", "192.168.250.77\\P247_Upload_New");
            }
            else
            {
                dir_target = dir_target.Replace("\\DATA", "")
                                       .Replace("P247_upload\\", "P247_Upload_New\\");
            }

            return dir_target;
        }
        // haipv1 27/05/2016--> kiểm tra xem user có đc chuyển hsgd_tpc lên trung tâm theo số tiền này hay không
        // kiểm tra xem user trưởng phòng có đc ủy quyền ko và số tiền là bao nhiêu
        // không cần kiểm tra hồ sơ tpc vì bước chuyển chờ phê duyệt đã làm việc này rồi
        // Có thể lấy ngay tổng số tiền tt+ph+son của vcx,tnds,tsk vì đến bước phê duyệt này không nhập các thông tin này nữa
        public string check_UyQuyen_HoSoTPC(DmUser currentUser)
        {
            string ket_qua = "";
            try
            {
               if(currentUser!=null)
                {
                    ket_qua = _dx_Repo.check_UyQuyen_HoSoTPC(currentUser);
                }    

            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return "";
            }
            return ket_qua;
        }

        public List<PquyenCnang> Check_PquyenCnang(DmUser currentUser)
        {
            List<PquyenCnang> pqcn = null;

            if (currentUser == null) 
            {
                pqcn = _dx_Repo.Check_PquyenCnang(currentUser);
            }
            return pqcn;
        }

        // Sử dụng để đảm bảo tất cả các trường trong báo cáo giám định đều đúng.
        public string validateBaoCaoGiamDinh(HsgdCtu hoSoGiamDinh)
        {
            if (hoSoGiamDinh.TenLaixe == null || hoSoGiamDinh.TenLaixe.Equals(""))
            {
                return "Bạn chưa nhập tên lái xe trong phần báo cáo giám định";
            }
            else if (hoSoGiamDinh.NamSinh == 0)
            {
                return "Bạn chưa nhập năm sinh lái xe trong phần báo cáo giám định";
            }
            else if (hoSoGiamDinh.SoGphepLaixe == null || hoSoGiamDinh.SoGphepLaixe.Equals(""))
            {
                return "Bạn chưa nhập bằng lái xe trong phần báo cáo giám định";
            }
            else if (hoSoGiamDinh.MaLoaibang == null || hoSoGiamDinh.MaLoaibang.Equals(""))
            {
                return "Bạn chưa nhập mã loại bằng trong phần báo cáo giám định";
            }
            else if (hoSoGiamDinh.SoGphepLuuhanh == null || hoSoGiamDinh.SoGphepLuuhanh.Equals(""))
            {
                return "Bạn chưa nhập số đăng kiểm trong phần báo cáo giám định";
            }
            else if (hoSoGiamDinh.NgayDauLaixe == null)
            {
                return "Bạn chưa nhập ngày cấp bằng lái xe trong phần báo cáo giám định";
            }
            else if (hoSoGiamDinh.NgayCuoiLaixe == null)
            {
                return "Bạn chưa nhập ngày hết hạn bằng lái xe trong phần báo cáo giám định";
            }
            else if (hoSoGiamDinh.NgayDauLuuhanh == null)
            {
                return "Bạn chưa nhập ngày cấp đăng kiểm trong phần báo cáo giám định";
            }
            else if (hoSoGiamDinh.NgayDauLuuhanh == null)
            {
                return "Bạn chưa nhập ngày hết đăng kiểm trong phần báo cáo giám định";
            }
            else if (hoSoGiamDinh.HosoPhaply == null || hoSoGiamDinh.HosoPhaply.Equals(""))
            {
                return "Bạn chưa nhập Hồ sơ Pháp lý trong phần báo cáo giám định";
            }
            else if (hoSoGiamDinh.YkienGdinh == null || hoSoGiamDinh.YkienGdinh.Equals(""))
            {
                return "Bạn chưa nhập Ý kiến giám định viên trong phần báo cáo giám định";
            }
            else if (hoSoGiamDinh.DexuatPan == null || hoSoGiamDinh.DexuatPan.Equals(""))
            {
                return "Bạn chưa nhập Đề xuất P.án bồi thường trong phần báo cáo giám định";
            }
            else if (hoSoGiamDinh.NgayTthat > hoSoGiamDinh.NgayCuoiLaixe)
            {
                return "Bằng lái xe hết hạn trước ngày tổn thất, vui lòng kiểm tra lại";
            }
            else if (hoSoGiamDinh.NgayTthat > hoSoGiamDinh.NgayCuoiLuuhanh)
            {
                return "Đăng kiểm hết hạn trước ngày tổn thất, vui lòng kiểm tra lại";
            }
            else
            {
                return "0"; // Validate thành công sẽ trả về 0.
            }
        }

        // Update hồ sơ bồi thường từ hồ sơ giám định.
        public async Task<string> updateHSBTPias(HsgdCtu hoSoGiamDinh, HsbtCtu hoSoBoiThuong)
        {
            try
            {
                DmUser giamDinhVien = await _context.DmUsers.Where(x => x.Oid == hoSoGiamDinh.MaUser).FirstOrDefaultAsync();

                hoSoBoiThuong.TenDttt = hoSoGiamDinh.BienKsoat.Replace(".", "").Replace("-", "").Replace(" ", "").Replace("/", "").Replace("`", "").Replace("\"", "").Replace(" | ", "").Replace("_", "").Replace(" < ", "").Replace(" > ", "").Replace("(", "").Replace("_", "");
                hoSoBoiThuong.DienThoai = hoSoGiamDinh.DienThoai;
                hoSoBoiThuong.NgayTthat = hoSoGiamDinh.NgayTthat;
                hoSoBoiThuong.NgayTbao = hoSoGiamDinh.NgayTbao;
                hoSoBoiThuong.NguyenNhan = hoSoGiamDinh.NguyenNhanTtat;
                hoSoBoiThuong.NguyenNhanTtat = hoSoGiamDinh.NguyenNhanTtat;
                hoSoBoiThuong.NgayGdinh = hoSoGiamDinh.NgayGdinh;
                hoSoBoiThuong.MaDdiemTthat = hoSoGiamDinh.MaDdiemTthat;
                hoSoBoiThuong.MaCbgd = (giamDinhVien != null ? giamDinhVien.MaUserPias : ""); // Mã PIAS của Giám Định Viên
                hoSoBoiThuong.MaLoaibang = hoSoGiamDinh.MaLoaibang;
                hoSoBoiThuong.TenLaixe = hoSoGiamDinh.TenLaixe;
                hoSoBoiThuong.NamSinh = hoSoGiamDinh.NamSinh;
                hoSoBoiThuong.SoGphepLaixe = hoSoGiamDinh.SoGphepLaixe;
                hoSoBoiThuong.NgayDauLaixe = hoSoGiamDinh.NgayDauLaixe;
                hoSoBoiThuong.NgayCuoiLaixe = hoSoGiamDinh.NgayCuoiLaixe;
                hoSoBoiThuong.SoGphepLuuhanh = hoSoGiamDinh.SoGphepLuuhanh;
                hoSoBoiThuong.NgayDauLuuhanh = hoSoGiamDinh.NgayDauLuuhanh;
                hoSoBoiThuong.NgayCuoiLuuhanh = hoSoGiamDinh.NgayCuoiLuuhanh;
                hoSoBoiThuong.HosoPhaply = hoSoGiamDinh.HosoPhaply;
                hoSoBoiThuong.YkienGdinh = hoSoGiamDinh.YkienGdinh;
                hoSoBoiThuong.DexuatPan = hoSoGiamDinh.DexuatPan;

                _context_pias_update.HsbtCtus.Update(hoSoBoiThuong);
                await _context_pias_update.SaveChangesAsync();
                return "1";

            }
            catch (Exception error)
            {
                _logger.Error("Lỗi lưu HSBT PIAS, phát sinh từ PR Key hồ sơ " + hoSoGiamDinh.PrKey + " và HSBT PR KEy" + hoSoBoiThuong.PrKey);
                Console.WriteLine(error);
                return "0";
            }
        }

        // Kiểm tra ảnh duyệt giá.
        public async Task<bool> checkAnhDuyetGia(HsgdCtu hoSoGiamDinh)
        {
            try
            {
                List<HsgdDgCt> listAnhDuyetGia = await (from anhDuyetGia in _context.HsgdDgCts
                                                        join hoSoDuyetGia in _context.HsgdDgs on anhDuyetGia.FrKey equals hoSoDuyetGia.PrKey
                                                        join hsgd in _context.HsgdCtus on hoSoDuyetGia.FrKey equals hsgd.PrKey
                                                        where (hsgd.PrKey == hoSoGiamDinh.PrKey)
                                                        select new HsgdDgCt()
                                                  ).ToListAsync();

                return listAnhDuyetGia.Count > 0;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return false;
            }
        }

        // Hai hàm dưới để kiểm tra Email và điện thoại.
        public bool validateEmail(string email)
        {
            var atpos = email.IndexOf("@");
            var dotpos = email.LastIndexOf(".");
            var kq = true;
            if (atpos < 1 || dotpos < atpos + 2 || dotpos + 2 >= email.Length)
            {
                kq = false;
            }
            return kq;
        }

        // Kiểm tra điện thoại nhập.
        public bool validatePhoneNumber(HsgdCtu hoSoGiamDinh)
        {
            string sdtLaiXe = hoSoGiamDinh.DienThoai;
            string sdtChuXe = hoSoGiamDinh.DienThoaiNdbh;
            string phoneno = @"^(09|08|07|05|02|03|01[1-9])([0-9]{8})\b";


            if ((sdtLaiXe == null || sdtLaiXe.Equals("") || sdtChuXe == null || sdtChuXe.Equals("")))
            {
                return false;
            }
            else
            {
                if (!Regex.IsMatch(sdtChuXe, phoneno) || !Regex.IsMatch(sdtLaiXe, phoneno))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        // Hàm gửi email chung cho gán duyệt giảm định.
        // Hàm gửi mail bảo lãnh KHÔNG PHẢI là hàm này.
        public void SendEmail(int type, string sTo, string sSubject, string so_donbh, string so_hsgd, string so_tiendx, string ng_gui, string seri, string bks, string ten_khach, string ngay_dau, string ngay_cuoi, string ngay_thongbao, string ngay_tonthat, string dia_diemtt, string nguyen_nhan, string gara, string ghi_chu, string ng_lienhe, string dien_thoai)
        {
            try
            {
                string htmlBody = "";
                MailAddress from = new MailAddress("OnlineInsurance@pvi.com.vn", "PVI.BHT", System.Text.Encoding.UTF8);
                MailAddress to = new MailAddress(sTo);

                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage(from, to);

                // If there are any CC recipients, add them here, e.g., mail.CC.Add("example@example.com");

                mail.Subject = sSubject;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.BodyEncoding = Encoding.UTF8;
                mail.IsBodyHtml = true;

                /* Các loại Send Email:
                 * 0: Gán giám định
                 * 1: Bổ sung thông tin.
                 */

                if (type == 0)
                {

                    htmlBody = HTMLBody_GiaoGD(so_donbh, so_hsgd, so_tiendx, ng_gui, seri, bks, ten_khach, ngay_dau, ngay_cuoi, ngay_thongbao, ngay_tonthat, dia_diemtt, nguyen_nhan, gara, ghi_chu, ng_lienhe, dien_thoai);
                    AlternateView avHTML = createAVT_HTML(htmlBody);
                    if (avHTML != null)
                    {
                        mail.AlternateViews.Add(avHTML);
                    }
                }
                else if (type == 1)
                {
                    htmlBody = HTMLBody_BS(so_hsgd, ng_gui, seri, bks, ten_khach, ghi_chu, ng_lienhe, dien_thoai);
                    AlternateView avHTML = createAVT_HTML(htmlBody);
                    if (avHTML != null)
                    {
                        mail.AlternateViews.Add(avHTML);
                    }
                }

                if (!string.IsNullOrEmpty(htmlBody))
                {
                    mail.Body = htmlBody;
                }

                SmtpClient smtpServer = new SmtpClient
                {
                    Port = 25, // Default là port 25, nếu test có thể dùng port 587.
                    Host = "mailapp.pvi.com.vn",
                    EnableSsl = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 10000
                };

                smtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        // LUỒNG GỬI BẢO LÃNH NHƯ SAU:

        // Gọi  HTMLBody_BaoLanh -> avHTML_BaoLanh -> SendEmail_BaoLanh

        // Sử dụng trong bước cuối cùng, tiến hành gửi email bảo lãnh
        public bool SendEmail_BaoLanh(string sTo, string sSubject, string strFileNamePdf, FormBaoLanh data)
        {
            try
            {
                string htmlBody = HTMLBody_BaoLanh(data);
                AlternateView avHtml = createAVT_HTML(htmlBody);

                MailAddress from = new MailAddress("baohiempvi@pvi.com.vn", "BAOHIEMPVI", System.Text.Encoding.UTF8);
                MailAddress to = new MailAddress(sTo);
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage(from, to);

                // If there are any CC recipients, add them here, e.g., mail.CC.Add("example@example.com");

                mail.Subject = sSubject;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.BodyEncoding = Encoding.UTF8;
                mail.IsBodyHtml = true;

                if (avHtml != null)
                {
                    mail.AlternateViews.Add(avHtml);
                }

                if (!string.IsNullOrEmpty(htmlBody))
                {
                    mail.Body = htmlBody;
                }

                string image = _configuration["Word2PdfSettings:BannerBHTT05"];

                LinkedResource pic = new LinkedResource(image, MediaTypeNames.Image.Jpeg);
                pic.ContentId = "Pic1";
                avHtml.LinkedResources.Add(pic);

                Attachment attachment = new System.Net.Mail.Attachment(strFileNamePdf);
                mail.Attachments.Add(attachment);

                SmtpClient smtpServer = new SmtpClient
                {
                    Port = 25, // Default là port 25, nếu test có thể dùng port 587.
                    Host = "mailapp.pvi.com.vn",
                    EnableSsl = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 15000
                };


                smtpServer.Send(mail);
                mail.Dispose();
                smtpServer.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool SendSMS(string phone, string bks, string seri, string so_hsgd, string phone_call, string send_type, int pr_key_hsgd_ctu)
        {
            bool result = true;

            string message = "";
            try
            {
                if (!Regex.IsMatch(phone, "^[0-9]+$") || phone.Length < 9 || phone.Length > 12)
                {
                    _logger.Information("sendsms so_hsgd = " + so_hsgd + " số điện thoại " + phone + " không đúng");
                    return false;
                }
                if (send_type == "0")
                    message = "BaohiemPVI thong bao ban duoc yeu cau GD xe " + bks + " theo ho so " + so_hsgd + " tren he thong Claim Online. De nghi lien he SDT " + phone_call + " de thuc hien GD";
                else if (send_type == "1")
                    message = "BaohiemPVI thong bao ban duoc yeu cau bo sung thong tin ho so " + so_hsgd + " xe " + bks + " tren he thong Claim Online. ";
                else if (send_type == "2")
                    message = "BaohiemPVI thong bao ho so " + so_hsgd + " xe " + bks + " da duoc phe duyet tren he thong Claim Online. ";
                else if (send_type == "3")
                    message = "BaohiemPVI thong bao ban duoc yeu cau giai quyet HS BKS " + bks + " ,so HS GDTT " + so_hsgd + " tu PVIMobile de hoan thien HS";
                else if (send_type == "BL")
                    message = "BaohiemPVI thong bao xe " + bks + " ton that ngay " + so_hsgd + " da duoc Bao hiem PVI bao lanh sua chua tai Gara. ";
                else if (send_type == "PASC")
                    message = "BaohiemPVI thong bao ho so " + so_hsgd + " xe " + bks + " da duoc len phuong an sua chua va gui ve don vi cap don. ";
                else if (send_type == "KBTT_INSTAL_APP")
                    message = "Ho so " + bks + " cua quy khach da duoc tao vui long vao ung dung PVI Mobile, chuc nang khai bao ton that chon ho so da duoc tao de giam dinh, dien thoai lien he:" + phone_call;
                else if (send_type == "KBTT_NOINSTAL_APP")
                    message = "Ho so " + bks + " cua quy khach da duoc tao quy khach vui long vao kho ung dung tai phan mem PVI Mobile de thuc hien giam dinh, dien thoai lien he:" + phone_call;
                //ws_sms.SendSms(message, phone, "BaohiemPVI", "5lGtsmmZ3Pf0d4Vqxc1eiw==", "T0bgxXd9QDWK_prS77leGw==", "Icm42ifKB_PQiumr35WtaA==", "southtelecom");
                if (phone.Substring(0, 1) == "0")
                {
                    phone = "84" + phone.Remove(0, 1);
                }
                else if (phone.Substring(0, 1) == "+")
                {
                    phone = phone.Remove(0, 1);
                }
                string dataCP = "";
                string resultCp = "";
                string key = "123456456789";
                string url = _configuration["DownloadSettings:url_sendmessage"];
                DataContentCP objCP = new DataContentCP();

                objCP.ma_donvi = so_hsgd;
                objCP.ma_doitac = "PVI_247";
                objCP.nguon_tao = "PVI_247";
                objCP.mang = "";
                objCP.id = "2";
                objCP.type = "SMS";
                objCP.To = phone;
                objCP.text = message;
                objCP.requestid = "PVI_247_" + so_hsgd + "_SMS";
                objCP.sign = ContentHelper.MD5(key + objCP.type + objCP.To + objCP.requestid);
                dataCP = JsonConvert.SerializeObject(objCP);
                resultCp = ContentHelper.PostData(dataCP, url);
                _logger.Information("sendsms to " + phone + " pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " kết quả " + resultCp);
            }
            catch (Exception ex)
            {
                result = false;
                _logger.Error($"sendsms to " + phone + " pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " error: " + ex);
            }
            return result;
        }

        private string HTMLBody_GiaoGD(string so_donbh, string so_hsgd, string so_tiendx, string ng_gui, string seri, string bks, string ten_khach, string ngay_dau, string ngay_cuoi, string ngay_thongbao, string ngay_tonthat, string dia_diemtt, string nguyen_nhan, string gara, string ghi_chu, string ng_lienhe, string dien_thoai)
        {
            string htmlBody = "";
            // Generate and return an AlternateView for email body

            htmlBody = "<html xmlns=\"http://www.w3.org/1999/xhtml\"> "
                 + "<head runat=\"server\"> "
                 + "    <title></title> "
                 + "    <style type=\"text/css\"> "
                 + "        #content p { "
                 + "            margin: 5px 0; "
                 + "            color: #085d60; "
                 + "        } "
                 + " "
                 + "        * { "
                 + "            margin: 0; "
                 + "            padding: 0; "
                 + "        } "
                 + " "
                 + "        a { "
                 + "            text-decoration: none; "
                 + "        } "
                 + "    </style> "
                 + "</head> "
                 + "<body> "
                 + "    <form id=\"form1\" runat=\"server\"> "
                 + "        <table align=\"center\" style=\"border-collapse: collapse\" bordercolor=\"#0185D0\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\"> "
                 + "            <tr> "
                 + "                <td style=\"width: 100%; height: 130px;\"> "
                 + "                    <img src=\"cid:Pic1\" style=\"width: 100%; height: 130px; border: none; margin: 0; display: block\" />                     "
                 + "                </td> "
                 + "            </tr> "
                 + "            <tr> "
                 + "                <td style=\"font-family: Arial; font-size: 13px; padding: 5px; vertical-align: top; text-align:justify\"> "
                 + "                    <p style=\"MARGIN-TOP: 30px\"> "
                 + "                        <strong>Bạn đã nhận được một yêu cầu giám định từ : " + ng_gui + ". Thông tin chi tiết giám định như dưới đây:</strong> "
                 + "                    </p> "
                 + "                    <p style=\"MARGIN-TOP: 20px\"> "
                 + "                       Số hồ sơ giám định: " + so_hsgd + ""
                 + "                    </p> "
                 + "                    <p style=\"MARGIN-TOP: 20px\"> "
                 + "                       Số đơn BH: " + so_donbh + ""
                 + "                    </p> "
                 + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
                 + "                        - <strong>Tên khách: " + ten_khach + "</strong> "
                 + "                    </p> "
                 + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
                 + "                        - <strong>Người liên hệ: " + ng_lienhe + "</strong> "
                 + "                    </p> "
                 + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
                 + "                        - <strong>Điện thoại: " + dien_thoai + "</strong> "
                 + "                    </p> "
                 + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
                 + "                        - <strong>Số ấn chỉ: " + seri + "</strong>"
                 + "                    </p> "
                 + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
                 + "                        - <strong>Biển kiểm soat(SK):  " + bks + "</strong>"
                 + "                    </p> "
                 + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
                 + "                        - <strong>Ngày bắt đầu bảo hiểm:  " + ngay_dau + "</strong>"
                 + "                    </p> "
                 + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
                 + "                        - <strong>Ngày kết thúc BH:  " + ngay_cuoi + "</strong>"
                 + "                    </p> "
                 + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
                 + "                        - <strong>Ngày thông báo:  " + ngay_thongbao + "</strong>"
                 + "                    </p> "
                 + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
                 + "                        - <strong>Ngày tổn thất:  " + ngay_tonthat + "</strong>"
                 + "                    </p> "
                 + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
                 + "                        - <strong>Địa điểm tổn thất:  " + dia_diemtt + "</strong>"
                 + "                    </p> "
                 + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
                 + "                        - <strong>Nguyên nhân tổn thất:  " + nguyen_nhan + "</strong>"
                 + "                    </p> "
                 + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
                 + "                        - <strong>Gara:  " + gara + "</strong>"
                 + "                    </p> "
                 + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
                 + "                        - <strong>Ghi chú:  " + ghi_chu + "</strong>"
                 + "                    </p> "
                 + "                    <p style=\"MARGIN-TOP: 20px\"> "
                 + "                        Để tra cứu thông tin vui lòng <b>đăng nhập</b> vào  website <a href=\"http://pvi247.pvi.com.vn\" style=\"FONT-WEIGHT: bold\" target=\"_blank\">http://pvi247.pvi.com.vn</a>"
                 + "                    </p> "
                 + "                    <p> Trân trọng cảm ơn! </p> "
                 + "                    <p> (*) Đây là email hệ thống gửi tự động, vui lòng không trả lời (reply) lại email này.</p> "
                 + "                </td> "
                 + "            </tr> "
                 + "        </table> "
                 + "    </form> "
                 + "</body> "
                 + "</html>";

            return htmlBody;
        }

        // Hàm sử dụng để tạo HTML Body cho email hồ sơ giám định
        private string HTMLBody_BS(string so_hsgd, string ng_gui, string seri, string bks, string ten_khach, string ghi_chu, string ng_lienhe, string dien_thoai)
        {
            string htmlBody = "";
            // Generate and return an AlternateView for email body


            htmlBody = "<html xmlns=\"http://www.w3.org/1999/xhtml\"> "
        + "<head runat=\"server\"> "
        + "    <title></title> "
        + "    <style type=\"text/css\"> "
        + "        #content p { "
        + "            margin: 5px 0; "
        + "            color: #085d60; "
        + "        } "
        + " "
        + "        * { "
        + "            margin: 0; "
        + "            padding: 0; "
        + "        } "
        + " "
        + "        a { "
        + "            text-decoration: none; "
        + "        } "
        + "    </style> "
        + "</head> "
        + "<body> "
        + "    <form id=\"form1\" runat=\"server\"> "
        + "        <table align=\"center\" style=\"border-collapse: collapse\" bordercolor=\"#0185D0\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\"> "
        + "            <tr> "
        + "                <td style=\"width: 100%; height: 130px;\"> "
        + "                    <img src=\"cid:Pic1\" style=\"width: 100%; height: 130px; border: none; margin: 0; display: block\" />                     "
        + "                </td> "
        + "            </tr> "
        + "            <tr> "
        + "                <td style=\"font-family: Arial; font-size: 13px; padding: 5px; vertical-align: top; text-align:justify\"> "
        + "                    <p style=\"MARGIN-TOP: 30px\"> "
        + "                        <strong>" + ng_gui + ": đã yêu cầu bạn bổ sung thông tin hồ sơ giám định số: " + so_hsgd + "</strong> "
        + "                    </p> "
        + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
        + "                        - <strong>Tên khách: " + ten_khach + "</strong> "
        + "                    </p> "
        + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
        + "                        - <strong>Người liên hệ: " + ng_lienhe + "</strong> "
        + "                    </p> "
        + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
        + "                        - <strong>Điện thoại: " + dien_thoai + "</strong> "
        + "                    </p> "
        + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
        + "                        - <strong>Số ấn chỉ: " + seri + "</strong>"
        + "                    </p> "
        + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
        + "                        - <strong>Biển kiểm soat(SK):  " + bks + "</strong>"
        + "                    </p> "
        + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
        + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
        + "                        - <strong>Ghi chú:  " + ghi_chu + "</strong>"
        + "                    </p> "
        + "                    <p style=\"MARGIN-TOP: 20px\"> "
        + "                        Để tra cứu thông tin vui lòng <b>đăng nhập</b> vào  website <a href=\"http://pvi247.pvi.com.vn\" style=\"FONT-WEIGHT: bold\" target=\"_blank\">http://pvi247.pvi.com.vn</a>"
        + "                    </p> "
        + "                    <p> Trân trọng cảm ơn! </p> "
        + "                    <p> (*) Đây là email hệ thống gửi tự động, vui lòng không trả lời (reply) lại email này.</p> "
        + "                </td> "
        + "            </tr> "
        + "        </table> "
        + "    </form> "
        + "</body> "
        + "</html>";

            return htmlBody;
        }



        // PASC 01 là phiên bản đầy đủ hơn của PASC 08, sử dụng cho cả gửi email và ký điện tử.
        // Theo mã phương án sửa chữa 01 của 247 gốc.
        // khanhlh - 20/09/2024
        public async Task<SoTienBaoLanh> DeXuat_PASC_01(HsgdCtu hoSoCtu)
        {
            try
            {
                // Đầu tiên, lấy toàn bộ danh sách đề xuất
                List<SoTienBaoLanh> hoSoDeXuat = await (from dx in _context.HsgdDxes
                                                        join ctu in _context.HsgdCtus on dx.FrKey equals ctu.PrKey
                                                        where ctu.PrKey == hoSoCtu.PrKey && dx.LoaiDx == 0
                                                        select new SoTienBaoLanh
                                                        {
                                                            // Phải bung hết công thức do variable hiện tại đang sử dụng Decimal, gộp lại thì tỷ lệ thuế sẽ bị quy về 0 -> Sai công thức.
                                                            tongSoTienGomVAT = (dx.SoTientt + dx.SoTienph + dx.SoTienson) + (((dx.SoTientt + dx.SoTienph + dx.SoTienson) * dx.VatSc) / 100),
                                                            soTienGiamTru = (((dx.SoTientt + (dx.SoTientt * dx.VatSc / 100)) - (((dx.SoTientt + (dx.SoTientt * dx.VatSc / 100)) * ctu.TyleggPhutungvcx) / 100) + ((dx.SoTienph + dx.SoTienson) + ((dx.SoTienph + dx.SoTienson) * dx.VatSc / 100)) - ((((dx.SoTienph + dx.SoTienson) + ((dx.SoTienph + dx.SoTienson) * dx.VatSc / 100)) * ctu.TyleggSuachuavcx) / 100)) * dx.GiamTruBt) / 100,
                                                            soTienGiamGia = (((dx.SoTientt + (dx.SoTientt * dx.VatSc) / 100) * (ctu.TyleggPhutungvcx / 100)) + (((dx.SoTienph + dx.SoTienson) + ((dx.SoTienph + dx.SoTienson) * dx.VatSc) / 100) * ctu.TyleggSuachuavcx / 100)),
                                                            soTienKhauTru = ctu.SoTienctkh,
                                                            soTienGtBt = ctu.SoTienGtbt
                                                        }).ToListAsync();
                // Tính tổng.
                HsgdDxSum sumSoTien = new HsgdDxSum();
                hoSoDeXuat.ForEach(x =>
                {
                    sumSoTien.SumSoTientt += x.tongSoTienGomVAT;
                    sumSoTien.SumSoTienGiamtru += x.soTienGiamTru;
                    sumSoTien.SumSoTienGgsc += x.soTienGiamGia;
                });

                // Sau đó mới tiến hành processing để trả về.
                SoTienBaoLanh tongSoTienBaoLanh = new SoTienBaoLanh();
                tongSoTienBaoLanh.tongSoTienGomVAT = Math.Round(sumSoTien.SumSoTientt.Value);
                tongSoTienBaoLanh.soTienGiamTru = Math.Round((sumSoTien.SumSoTienGiamtru != 0) ? sumSoTien.SumSoTienGiamtru.Value : hoSoCtu.SoTienGtbt);
                tongSoTienBaoLanh.soTienGiamGia = Math.Round(sumSoTien.SumSoTienGgsc.Value);
                tongSoTienBaoLanh.soTienKhauTru = Math.Round(hoSoCtu.SoTienctkh);
                tongSoTienBaoLanh.trachNhiemPVI = Math.Round(sumSoTien.SumSoTientt.Value - ((sumSoTien.SumSoTienGiamtru != 0) ? sumSoTien.SumSoTienGiamtru.Value : hoSoCtu.SoTienGtbt) - sumSoTien.SumSoTienGgsc.Value - hoSoCtu.SoTienctkh);

                return tongSoTienBaoLanh;
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
                return new SoTienBaoLanh();
            }
        }

        // Sử dụng để tỉnh tổng số tiền, tiền giảm trừ cũng như tiền trách nhiệm bồi thường.
        // Theo mã phương án sửa chữa 08 của 247 gốc.
        // khanhlh - 20/09/2024
        public async Task<SoTienBaoLanh> DeXuat_PASC_08(HsgdCtu hoSoCtu)
        {
            try
            {
                // Đầu tiên, lấy toàn bộ danh sách đề xuất
                List<SoTienBaoLanh> hoSoDeXuat = await (from dx in _context.HsgdDxes
                                                        join ctu in _context.HsgdCtus on dx.FrKey equals ctu.PrKey
                                                        where ctu.PrKey == hoSoCtu.PrKey && dx.LoaiDx == 0
                                                        select new SoTienBaoLanh
                                                        {
                                                            // Phải bung hết công thức do variable hiện tại đang sử dụng Decimal, gộp lại thì tỷ lệ thuế sẽ bị quy về 0 -> Sai công thức.
                                                            tongSoTienGomVAT = (dx.SoTientt + dx.SoTienph + dx.SoTienson) + (((dx.SoTientt + dx.SoTienph + dx.SoTienson) * dx.VatSc) / 100),
                                                            soTienGiamTru = (((dx.SoTientt + (dx.SoTientt * dx.VatSc / 100)) - (((dx.SoTientt + (dx.SoTientt * dx.VatSc / 100)) * ctu.TyleggPhutungvcx) / 100) + ((dx.SoTienph + dx.SoTienson) + ((dx.SoTienph + dx.SoTienson) * dx.VatSc / 100)) - ((((dx.SoTienph + dx.SoTienson) + ((dx.SoTienph + dx.SoTienson) * dx.VatSc / 100)) * ctu.TyleggSuachuavcx) / 100)) * dx.GiamTruBt) / 100,
                                                            soTienGiamGia = (((dx.SoTientt + (dx.SoTientt * dx.VatSc) / 100) * (ctu.TyleggPhutungvcx / 100)) + (((dx.SoTienph + dx.SoTienson) + ((dx.SoTienph + dx.SoTienson) * dx.VatSc) / 100) * ctu.TyleggSuachuavcx / 100)),
                                                            soTienKhauTru = ctu.SoTienctkh,
                                                            soTienGtBt = ctu.SoTienGtbt
                                                        }).ToListAsync();
                // Tính tổng.
                HsgdDxSum sumSoTien = new HsgdDxSum();
                hoSoDeXuat.ForEach(x =>
                {
                    sumSoTien.SumSoTientt += x.tongSoTienGomVAT;
                    sumSoTien.SumSoTienGiamtru += x.soTienGiamTru;
                    sumSoTien.SumSoTienGgsc += x.soTienGiamGia;
                });

                // Sau đó mới tiến hành processing để trả về.
                SoTienBaoLanh tongSoTienBaoLanh = new SoTienBaoLanh();
                tongSoTienBaoLanh.tongSoTienGomVAT = Math.Round(sumSoTien.SumSoTientt.Value);
                tongSoTienBaoLanh.soTienGiamTru = Math.Round((sumSoTien.SumSoTienGiamtru != 0) ? sumSoTien.SumSoTienGiamtru.Value : hoSoCtu.SoTienGtbt);
                tongSoTienBaoLanh.soTienGiamGia = Math.Round(sumSoTien.SumSoTienGgsc.Value);
                tongSoTienBaoLanh.soTienKhauTru = Math.Round(hoSoCtu.SoTienctkh);
                tongSoTienBaoLanh.trachNhiemPVI = Math.Round(sumSoTien.SumSoTientt.Value - ((sumSoTien.SumSoTienGiamtru != 0) ? sumSoTien.SumSoTienGiamtru.Value : hoSoCtu.SoTienGtbt) - sumSoTien.SumSoTienGgsc.Value - hoSoCtu.SoTienctkh);

                return tongSoTienBaoLanh;
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
                return new SoTienBaoLanh();
            }
        }

        // Sử dụng để tỉnh tổng số tiền, tiền giảm trừ cũng như tiền trách nhiệm bồi thường.
        // khanhlh - 20/09/2024
        public async Task<SoTienBaoLanh> DeXuat_PASC(HsgdCtu hoSoCtu)
        {
            try
            {
                // Đầu tiên, lấy toàn bộ danh sách đề xuất
                List<SoTienBaoLanh> hoSoDeXuat = await (from dx in _context.HsgdDxes
                                                        join ctu in _context.HsgdCtus on dx.FrKey equals ctu.PrKey
                                                        where ctu.PrKey == hoSoCtu.PrKey && dx.LoaiDx == 0

                                                        select new SoTienBaoLanh
                                                        {
                                                            // Phải bung hết công thức do variable hiện tại đang sử dụng Decimal, gộp lại thì tỷ lệ thuế sẽ bị quy về 0 -> Sai công thức.
                                                            tongSoTienGomVAT = (dx.SoTientt + dx.SoTienph + dx.SoTienson) + (((dx.SoTientt + dx.SoTienph + dx.SoTienson) * dx.VatSc) / 100),
                                                            soTienGiamTru = (((dx.SoTientt + (dx.SoTientt * dx.VatSc / 100)) - (((dx.SoTientt + (dx.SoTientt * dx.VatSc / 100)) * ctu.TyleggPhutungvcx) / 100) + ((dx.SoTienph + dx.SoTienson) + ((dx.SoTienph + dx.SoTienson) * dx.VatSc / 100)) - ((((dx.SoTienph + dx.SoTienson) + ((dx.SoTienph + dx.SoTienson) * dx.VatSc / 100)) * ctu.TyleggSuachuavcx) / 100)) * dx.GiamTruBt) / 100,
                                                            soTienGiamGia = (((dx.SoTientt + (dx.SoTientt * dx.VatSc) / 100) * (ctu.TyleggPhutungvcx / 100)) + (((dx.SoTienph + dx.SoTienson) + ((dx.SoTienph + dx.SoTienson) * dx.VatSc) / 100) * ctu.TyleggSuachuavcx / 100)),
                                                            soTienKhauTru = ctu.SoTienctkh,
                                                            soTienGtBt = ctu.SoTienGtbt
                                                        }).ToListAsync();
                // Sau đấy cộng tất cả 
                HsgdDxSum sumSoTien = new HsgdDxSum();
                hoSoDeXuat.ForEach(x =>
                {
                    sumSoTien.SumSoTientt += x.tongSoTienGomVAT;
                    sumSoTien.SumSoTienGiamtru += x.soTienGiamTru;
                    sumSoTien.SumSoTienGgsc += x.soTienGiamGia;
                });

                // Sau đó mới tiến hành processing để trả về.
                SoTienBaoLanh tongSoTienBaoLanh = new SoTienBaoLanh();
                tongSoTienBaoLanh.tongSoTienGomVAT = Math.Round(sumSoTien.SumSoTientt.Value);
                tongSoTienBaoLanh.soTienGiamTru = Math.Round((sumSoTien.SumSoTienGiamtru != 0) ? sumSoTien.SumSoTienGiamtru.Value : hoSoCtu.SoTienGtbt);
                tongSoTienBaoLanh.soTienGiamGia = Math.Round(sumSoTien.SumSoTienGgsc.Value);
                tongSoTienBaoLanh.soTienKhauTru = Math.Round(hoSoCtu.SoTienctkh);
                tongSoTienBaoLanh.trachNhiemPVI = Math.Round(sumSoTien.SumSoTientt.Value - ((sumSoTien.SumSoTienGiamtru != 0) ? sumSoTien.SumSoTienGiamtru.Value : hoSoCtu.SoTienGtbt) - sumSoTien.SumSoTienGgsc.Value - hoSoCtu.SoTienctkh);

                return tongSoTienBaoLanh;
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
                return new SoTienBaoLanh();
            }
        }

        // Lấy thông tin đơn vị từ mã đơn vị thanh toán.
        public DonViThanhToanResponse getInfoDonviFromId(string ma_donvi_tt)
        {
            try
            {
                var dvtt = (from thongtindv in _context_pias.DmVars

                            where thongtindv.MaDonvi == ma_donvi_tt && (thongtindv.Bien == "DON_VI" || thongtindv.Bien == "DIA_CHI" || thongtindv.Bien == "MASO_VAT")
                            orderby thongtindv.MaDonvi, thongtindv.Bien ascending
                            select new DmVar
                            {
                                PrKey = thongtindv.PrKey,
                                MaDonvi = thongtindv.MaDonvi,
                                Bien = thongtindv.Bien,
                                GhiChu = thongtindv.GhiChu,
                                GiaTri = thongtindv.GiaTri,
                                GiaTriEng = thongtindv.GiaTriEng,
                                TongHop = thongtindv.TongHop,
                                MaUser = thongtindv.MaUser,
                                NgayCnhat = thongtindv.NgayCnhat,
                                Khoa = thongtindv.Khoa
                            }).ToList();

                DonViThanhToanResponse donvitt = new DonViThanhToanResponse();
                donvitt.maDonVi = dvtt.ElementAt(0).MaDonvi;
                donvitt.diaChi = dvtt.ElementAt(0).GiaTri;
                donvitt.donViThanhToan = _context.DmDonvis.FirstOrDefault(x => x.MaDonvi == ma_donvi_tt).TenDonvi;
                donvitt.tenDonVi = dvtt.ElementAt(1).GiaTri;
                donvitt.maSoThue = dvtt.ElementAt(2).GiaTri;

                return donvitt;

            }
            catch (Exception err)
            {
                Console.WriteLine($"Error encouter at getDonViThanhToan: " + err);
                _context.Dispose();
                throw;
            }
        }

        // Map các tính trạng hồ sơ
        public string Map_tinh_trang(string tinh_trang)
        {
            string ftinh_trang = null;
            if (tinh_trang == "1")
                ftinh_trang = "Chưa giao giám định";
            else if (tinh_trang == "2")
                ftinh_trang = "Đã giao giám định";
            else if (tinh_trang == "3")
                ftinh_trang = "Đang giám định";
            else if (tinh_trang == "4")
                ftinh_trang = "Chờ phê duyệt";
            else if (tinh_trang == "5")
                ftinh_trang = "Bổ sung thông tin";
            else if (tinh_trang == "6")
                ftinh_trang = "Đã duyệt";
            else if (tinh_trang == "7")
                ftinh_trang = "Hồ sơ đã hủy";
            else if (tinh_trang == "8")
                ftinh_trang = "Hồ sơ TPC chưa xử lý";
            else if (tinh_trang == "9")
                ftinh_trang = "Hồ sơ TPC đang xử lý";
            else if (tinh_trang == "10")
                ftinh_trang = "Hồ sơ TPC chờ duyệt";
            else if (tinh_trang == "DBL")
                ftinh_trang = "Duyệt Bảo lãnh";
            else if (tinh_trang == "BLDT")
                ftinh_trang = "Gửi Bảo lãnh điện tử";
            else if (tinh_trang == "PADT")
                ftinh_trang = "Gửi PASC điện tử";
            else if (tinh_trang == "HSBT")
                ftinh_trang = "Tạo HSBT Pias";
            else if (tinh_trang == "PASC")
                ftinh_trang = "PASC ký điện tử";
            else
                ftinh_trang = "Không xác định";
            return ftinh_trang;
        }

        public FormBaoLanh BaoLanh_GetInfo(decimal pr_key, decimal pr_key_hsbt_ct, DmUser currentUser, string? ma_donvi_tt)
        {
            //bool isPreviewing = true;
            try
            {
                HsgdCtu hoSoGiamDinh = _context.HsgdCtus.Where(x => x.PrKey == pr_key).FirstOrDefault();
                HsgdDx dxGiamDinh = _context.HsgdDxes.Where(x => x.FrKey == pr_key).FirstOrDefault();
                if (hoSoGiamDinh != null) //&& dxGiamDinh != null && hoSoGiamDinh.MaGaraVcx != null && hoSoGiamDinh.MaGaraVcx != "") // Kiểm tra các điều kiện tạo form bảo lãnh
                {
                    FormBaoLanh preview = new FormBaoLanh();
                    preview.maUser = currentUser.MaUser;
                    preview.tenUser = currentUser.TenUser;
                    preview.maDonvi = currentUser.MaDonvi;

                    SoTienBaoLanh stbl = new SoTienBaoLanh();//DeXuat_PASC_08(hoSoGiamDinh).Result;
                    preview.soHsgd = hoSoGiamDinh.SoHsgd;


                    HsgdDxCt to_be_added = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCt == pr_key_hsbt_ct).FirstOrDefault();
                    if (to_be_added != null)
                    {
                        List<HsgdDxSum> reloadedSum = _dx_Repo.ReloadSum(to_be_added.PrKey);
                        for (int j = 0; j < reloadedSum.Count; j++)
                        {
                            stbl.trachNhiemPVI += reloadedSum[j].StBl != null ? Math.Round(reloadedSum[j].StBl.Value) : 0;
                            stbl.tongSoTienGomVAT += reloadedSum[j].SumSoTienTtsc != null ? Math.Round(reloadedSum[j].SumSoTienTtsc.Value) : 0;
                            stbl.soTienGiamGia += reloadedSum[j].SumSoTienGgsc != null ? Math.Round(reloadedSum[j].SumSoTienGgsc.Value) : 0;
                            stbl.soTienGiamTru += reloadedSum[j].SumSoTienGiamtru != 0 ? Math.Round(reloadedSum[j].SumSoTienGiamtru.Value) : reloadedSum[j].SoTienGtbt;
                            stbl.soTienKhauTru += reloadedSum[j].SoTienctkh != null ? Math.Round(reloadedSum[j].SoTienctkh.Value) : 0;
                            stbl.soTienDoiTru += reloadedSum[j].SumSoTienDoitru != 0 ? Math.Round(reloadedSum[j].SumSoTienDoitru.Value) : 0;
                        }

                        preview.lyDoKhauTru = to_be_added.LydoCtkh;

                        DmGaRa garaBaoLanh = _context.DmGaRas.FirstOrDefault(x => x.MaGara.Equals(to_be_added.MaGara));
                        if (garaBaoLanh != null)
                        {
                            preview.nguoiNhan = garaBaoLanh.TenGara;
                            preview.TenGara = garaBaoLanh.TenGara + " - " + garaBaoLanh.DiaChi;

                        }
                        else
                        {
                            preview.nguoiNhan = to_be_added.MaGara;
                            preview.TenGara = to_be_added.MaGara;
                        }
                        //List <HsbtCtView> listPhaiTraBT = _dx_Repo.GetListPhaiTraBT(hoSoGiamDinh.PrKey);
                        //if (listPhaiTraBT.Count > 0)
                        //{
                        //    for (int i = 0; i < listPhaiTraBT.Count; i++)
                        //    {
                        //        preview.lyDoKhauTru = listPhaiTraBT[0].LydoCtkh;

                        //        DmGaRa garaBaoLanh = _context.DmGaRas.FirstOrDefault(x => x.MaGara.Equals(listPhaiTraBT[0].MaGara));
                        //        if (garaBaoLanh != null)
                        //        {
                        //            preview.nguoiNhan = garaBaoLanh.TenGara;
                        //            preview.TenGara = garaBaoLanh.TenGara + " - " + garaBaoLanh.DiaChi;

                        //        }
                        //        else
                        //        {
                        //            preview.nguoiNhan = listPhaiTraBT[0].MaGara;
                        //            preview.TenGara = listPhaiTraBT[0].MaGara;
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    preview.nguoiNhan = "Không có mã gara nhận";
                        //    preview.TenGara = "Không có tên gara nhận";
                        //}



                        preview.NgayDau = hoSoGiamDinh.NgayDauSeri.Value.Day + "/" + hoSoGiamDinh.NgayDauSeri.Value.Month + "/" + hoSoGiamDinh.NgayDauSeri.Value.Year;
                        preview.NgayCuoi = hoSoGiamDinh.NgayCuoiSeri.Value.Day + "/" + hoSoGiamDinh.NgayCuoiSeri.Value.Month + "/" + hoSoGiamDinh.NgayCuoiSeri.Value.Year;

                        preview.bienKiemSoat = hoSoGiamDinh.BienKsoat;
                        preview.tenKhach = hoSoGiamDinh.TenKhach;

                        preview.NgayTThat = hoSoGiamDinh.NgayTthat.Value.Day + "/" + hoSoGiamDinh.NgayTthat.Value.Month + "/" + hoSoGiamDinh.NgayTthat.Value.Year;
                        preview.NgayTBao = hoSoGiamDinh.NgayTbao.Value.Day + "/" + hoSoGiamDinh.NgayTbao.Value.Month + "/" + hoSoGiamDinh.NgayTbao.Value.Year;
                        preview.NguyenNhan = hoSoGiamDinh.NguyenNhanTtat;

                        //if (hoSoGiamDinh.MaDonvi != null && hoSoGiamDinh.MaDonvi != "")
                        //{
                        //    DmDonvi dv = _context.DmDonvis.FirstOrDefault(x => x.MaDonvi.Equals(hoSoGiamDinh.MaDonvi));
                        //    preview.donViBaoHiem = dv.TenDonvi;
                        //}

                        if (hoSoGiamDinh.MaUser != null)
                        {
                            DmUser gdv = _context.DmUsers.FirstOrDefault(x => x.Oid == hoSoGiamDinh.MaUser);
                            preview.giamDinhVien = gdv.TenUser;
                            preview.donViGiamDinhVien = _context.DmDonvis.Where(x => x.MaDonvi.Equals(gdv.MaDonvi)).FirstOrDefault().TenDonvi;
                        }

                        preview.tongSoTienGomVAT = ContentHelper.formatMoney(((Int128)(stbl.tongSoTienGomVAT.Value)).ToString());
                        preview.soTienGiamGia = ContentHelper.formatMoney(((Int128)(stbl.soTienGiamGia.Value)).ToString());
                        preview.soTienGiamTru = ContentHelper.formatMoney(((Int128)(stbl.soTienGiamTru.Value)).ToString());
                        if (stbl.soTienGiamTru.Value == 0)
                        {
                            preview.soTienGiamTru = ContentHelper.formatMoney(((Int128)(stbl.soTienGtBt.Value)).ToString());
                        }
                        preview.soTienDoiTru = ContentHelper.formatMoney(((Int128)(stbl.soTienDoiTru.Value)).ToString());
                        preview.soTienKhauTru = ContentHelper.formatMoney(((Int128)(stbl.soTienKhauTru)).ToString());
                        preview.soTienTrachNhiemPVI = ContentHelper.formatMoney(((Int128)(stbl.trachNhiemPVI.Value)).ToString());

                        preview.soTienTrachNhiemPVIBangChu = ContentHelper.NumberToText((double)stbl.trachNhiemPVI.Value, true);

                        if (hoSoGiamDinh.SoDonbh.Substring(3, 2) != "")
                        {
                            if(hoSoGiamDinh.SoDonbh.Substring(3, 2)=="33")
                            {
                                DonViThanhToanResponse dvtt = getInfoDonviFromId("32");
                                preview.donViThanhToan = dvtt.tenDonVi;
                                preview.maSoThue = dvtt.maSoThue;
                                preview.diaChi = dvtt.diaChi;
                                preview.donViBaoHiem = dvtt.tenDonVi;
                            }
                            else
                            {
                                DonViThanhToanResponse dvtt = getInfoDonviFromId(hoSoGiamDinh.SoDonbh.Substring(3, 2));
                                preview.donViThanhToan = dvtt.tenDonVi;
                                preview.maSoThue = dvtt.maSoThue;
                                preview.diaChi = dvtt.diaChi;
                                preview.donViBaoHiem = dvtt.tenDonVi;
                            }    
                            
                        }
                        else
                        {
                            preview.donViThanhToan = "";
                            preview.maSoThue = "";
                            preview.diaChi = "";
                            preview.donViBaoHiem = "";
                        }

                        preview.taiLieuCanBoSung = to_be_added.BlTailieubs;
                        preview.bl1 = to_be_added.Bl1;
                        preview.bl2 = to_be_added.Bl2;
                        preview.bl3 = to_be_added.Bl3;
                        preview.bl4 = to_be_added.Bl4;
                        preview.bl5 = to_be_added.Bl5;
                        preview.bl6 = to_be_added.Bl6;
                        preview.bl7 = to_be_added.Bl7;
                        preview.bl8 = to_be_added.Bl8;
                        preview.bl9 = to_be_added.Bl9;

                        NhatKy ma_user_duyetbl = _context.NhatKies.Where(x => x.FrKey == hoSoGiamDinh.PrKey && x.MaTtrangGd.Equals("DBL")).FirstOrDefault();

                        if (ma_user_duyetbl != null)
                        {
                            DmUser userDuyetBL = _context.DmUsers.Where(x => x.Oid == ma_user_duyetbl.MaUser).FirstOrDefault();
                            preview.maUserDuyetBl = userDuyetBL.MaUser;
                            preview.tenUserDuyetBl = userDuyetBL.TenUser;
                            if (userDuyetBL.MaDonvi.Equals("31") || userDuyetBL.MaDonvi.Equals("32") || userDuyetBL.MaDonvi.Equals("00"))
                            {
                                if (userDuyetBL.LoaiUser != 10)
                                {
                                    preview.tenKy = "TUQ. GIÁM ĐỐC VĂN PHÒNG";
                                }
                                else
                                {
                                    preview.tenKy = "GIÁM ĐỐC VĂN PHÒNG";
                                }
                            }
                            else
                            {
                                preview.tenKy = "GIÁM ĐỐC";
                            }
                            return preview;
                        }
                        else
                        {
                            preview.maUserDuyetBl = "";
                            preview.tenUserDuyetBl = "";
                            preview.tenKy = "TUQ. GIÁM ĐỐC VĂN PHÒNG";
                            return preview;
                        }
                    }
                    else
                    {
                        return null;
                    }


                }

                else
                {
                    return null;
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
                return null;
            }
        }

        public FormBaoLanh BaoLanh_GetInfo_Preview(decimal pr_key, decimal pr_key_hsbt_ct, DmUser currentUser, string? ma_donvi_tt)
        {
            //bool isPreviewing = true;
            try
            {
                HsgdCtu hoSoGiamDinh = _context.HsgdCtus.Where(x => x.PrKey == pr_key).FirstOrDefault();
                HsgdDx dxGiamDinh = _context.HsgdDxes.Where(x => x.FrKey == pr_key).FirstOrDefault();
                if (hoSoGiamDinh != null) //&& dxGiamDinh != null && hoSoGiamDinh.MaGaraVcx != null && hoSoGiamDinh.MaGaraVcx != "") // Kiểm tra các điều kiện tạo form bảo lãnh
                {

                    if (hoSoGiamDinh.BlPdbl == 1)
                    {
                        FormBaoLanh preview = BaoLanh_GetInfo(pr_key, pr_key_hsbt_ct, currentUser, ma_donvi_tt);
                        return preview;
                    }
                    else
                    {
                        FormBaoLanh preview = new FormBaoLanh();
                        preview.maUser = currentUser.MaUser;
                        preview.tenUser = currentUser.TenUser;
                        preview.maDonvi = currentUser.MaDonvi;

                        SoTienBaoLanh stbl = new SoTienBaoLanh();//DeXuat_PASC_08(hoSoGiamDinh).Result;
                        preview.soHsgd = hoSoGiamDinh.SoHsgd;

                        //decimal sum_bao_lanh = 0; // Kiểm tra user phải có số tiền giới hạn uỷ quyền lớn hơn số tiền của hồ sơ.

                        List<HsbtCt> list_ct = (from hsbtCt in _context_pias.HsbtCts
                                                where hsbtCt.FrKey == hoSoGiamDinh.PrKeyBt
                                                select new HsbtCt
                                                {
                                                    PrKey = hsbtCt.PrKey
                                                }).ToList();

                        if (list_ct.Count > 0)
                        {
                            List<HsgdDxCt> list_dx_ct = new List<HsgdDxCt>();
                            for (int i = 0; i < list_ct.Count; i++)
                            {
                                // Hiện mới chỉ xử lý cho loại sản phẩm vật chất xe 050104.
                                //if (list_ct[i].MaSp == "050104")
                                //{
                                HsgdDxCt to_be_added = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCt == list_ct[0].PrKey).FirstOrDefault();
                                if (to_be_added != null)
                                {
                                    List<HsgdDxSum> reloadedSum = _dx_Repo.ReloadSum(to_be_added.PrKey);
                                    for (int j = 0; j < reloadedSum.Count; j++)
                                    {
                                        stbl.trachNhiemPVI += reloadedSum[j].StBl != null ? Math.Round(reloadedSum[j].StBl.Value) : 0;
                                        stbl.tongSoTienGomVAT += reloadedSum[j].SumSoTienTtsc != null ? Math.Round(reloadedSum[j].SumSoTienTtsc.Value) : 0;
                                        stbl.soTienGiamGia += reloadedSum[j].SumSoTienGgsc != null ? Math.Round(reloadedSum[j].SumSoTienGgsc.Value) : 0;
                                        stbl.soTienGiamTru = reloadedSum[j].SumSoTienGiamtru != null ? Math.Round(reloadedSum[j].SumSoTienGiamtru.Value) : 0;
                                        stbl.soTienDoiTru = reloadedSum[j].SumSoTienDoitru != null ? Math.Round(reloadedSum[j].SumSoTienDoitru.Value) : 0;
                                        stbl.soTienKhauTru = reloadedSum[j].SoTienctkh != null ? Math.Round(reloadedSum[j].SoTienctkh.Value) : 0;
                                        stbl.soTienGtBt = reloadedSum[j].SoTienGtbt != null ? Math.Round(reloadedSum[j].SoTienGtbt.Value) : 0;
                                    }
                                }
                                //}
                            }
                        }


                        //if (hoSoGiamDinh.MaGaraVcx != null && hoSoGiamDinh.MaGaraVcx != "")
                        //{
                        List<HsbtCtView> listPhaiTraBT = _dx_Repo.GetListPhaiTraBT(hoSoGiamDinh.PrKey);
                        if (listPhaiTraBT.Count > 0)
                        {
                            for (int i = 0; i < listPhaiTraBT.Count; i++)
                            {
                                // Hiện chỉ đang xử lý cho vật chất xe - sẽ test thêm.
                                //if (listPhaiTraBT[i].MaSp == "050104")
                                //{
                                preview.lyDoKhauTru = listPhaiTraBT[0].LydoCtkh;

                                DmGaRa garaBaoLanh = _context.DmGaRas.FirstOrDefault(x => x.MaGara.Equals(listPhaiTraBT[0].MaGara));
                                if (garaBaoLanh != null)
                                {
                                    preview.nguoiNhan = garaBaoLanh.TenGara;
                                    preview.TenGara = garaBaoLanh.TenGara + " - " + garaBaoLanh.DiaChi;

                                }
                                else
                                {
                                    preview.nguoiNhan = listPhaiTraBT[0].MaGara;
                                    preview.TenGara = listPhaiTraBT[0].MaGara;
                                }
                                //}
                            }
                        }
                        else
                        {
                            preview.nguoiNhan = "Không có mã gara nhận";
                            preview.TenGara = "Không có tên gara nhận";
                        }
                        //}

                        preview.NgayDau = hoSoGiamDinh.NgayDauSeri.Value.Day + "/" + hoSoGiamDinh.NgayDauSeri.Value.Month + "/" + hoSoGiamDinh.NgayDauSeri.Value.Year;
                        preview.NgayCuoi = hoSoGiamDinh.NgayCuoiSeri.Value.Day + "/" + hoSoGiamDinh.NgayCuoiSeri.Value.Month + "/" + hoSoGiamDinh.NgayCuoiSeri.Value.Year;

                        preview.bienKiemSoat = hoSoGiamDinh.BienKsoat;
                        preview.tenKhach = hoSoGiamDinh.TenKhach;

                        preview.NgayTThat = hoSoGiamDinh.NgayTthat.Value.Day + "/" + hoSoGiamDinh.NgayTthat.Value.Month + "/" + hoSoGiamDinh.NgayTthat.Value.Year;
                        preview.NgayTBao = hoSoGiamDinh.NgayTbao.Value.Day + "/" + hoSoGiamDinh.NgayTbao.Value.Month + "/" + hoSoGiamDinh.NgayTbao.Value.Year;
                        preview.NguyenNhan = hoSoGiamDinh.NguyenNhanTtat;

                        //if (hoSoGiamDinh.MaDonvi != null && hoSoGiamDinh.MaDonvi != "")
                        //{
                        //    DmDonvi dv = _context.DmDonvis.FirstOrDefault(x => x.MaDonvi.Equals(hoSoGiamDinh.MaDonvi));
                        //    preview.donViBaoHiem = dv.TenDonvi;
                        //}

                        if (hoSoGiamDinh.MaUser != null)
                        {
                            DmUser gdv = _context.DmUsers.FirstOrDefault(x => x.Oid == hoSoGiamDinh.MaUser);
                            preview.giamDinhVien = gdv.TenUser;
                            preview.donViGiamDinhVien = _context.DmDonvis.Where(x => x.MaDonvi.Equals(gdv.MaDonvi)).FirstOrDefault().TenDonvi;
                        }

                        preview.tongSoTienGomVAT = ContentHelper.formatMoney(((Int128)(stbl.tongSoTienGomVAT.Value)).ToString());
                        preview.soTienGiamGia = ContentHelper.formatMoney(((Int128)(stbl.soTienGiamGia.Value)).ToString());
                        preview.soTienGiamTru = ContentHelper.formatMoney(((Int128)(stbl.soTienGiamTru.Value)).ToString());
                        preview.soTienDoiTru = ContentHelper.formatMoney(((Int128)(stbl.soTienDoiTru.Value)).ToString());
                        preview.soTienKhauTru = ContentHelper.formatMoney(((Int128)(stbl.soTienKhauTru)).ToString());
                        if (stbl.soTienGiamTru.Value == 0)
                        {
                            preview.soTienGiamTru = ContentHelper.formatMoney(((Int128)(stbl.soTienGtBt.Value)).ToString());
                        }
                        preview.soTienTrachNhiemPVI = ContentHelper.formatMoney(((Int128)(stbl.trachNhiemPVI.Value)).ToString());

                        //preview.tongSoTienGomVAT = stbl.tongSoTienGomVAT.Value.ToString(); //ContentHelper.formatMoney(stbl.tongSoTienGomVAT.Value.ToString());
                        //preview.soTienGiamGia = stbl.soTienGiamGia.Value.ToString(); // ContentHelper.formatMoney(stbl.soTienGiamGia.Value.ToString());
                        //preview.soTienGiamTru = stbl.soTienGiamTru.Value.ToString(); // ContentHelper.formatMoney(stbl.soTienGiamTru.Value.ToString());
                        //preview.soTienKhauTru = (stbl.soTienKhauTru.ToString()); // ContentHelper.formatMoney(stbl.soTienKhauTru.ToString());
                        //preview.soTienTrachNhiemPVI = stbl.trachNhiemPVI.Value.ToString(); // ContentHelper.formatMoney(stbl.trachNhiemPVI.Value.ToString());

                        //preview.lyDoKhauTru = hoSoGiamDinh.LydoCtkh;

                        preview.soTienTrachNhiemPVIBangChu = ContentHelper.NumberToText((double)stbl.trachNhiemPVI.Value, true);

                        if (hoSoGiamDinh.SoDonbh.Substring(3, 2) != "")
                        {
                            if (hoSoGiamDinh.SoDonbh.Substring(3, 2) == "33")
                            {
                                DonViThanhToanResponse dvtt = getInfoDonviFromId("32");
                                preview.donViThanhToan = dvtt.tenDonVi;
                                preview.maSoThue = dvtt.maSoThue;
                                preview.diaChi = dvtt.diaChi;
                                preview.donViBaoHiem = dvtt.tenDonVi;
                            }
                            else
                            {
                                DonViThanhToanResponse dvtt = getInfoDonviFromId(hoSoGiamDinh.SoDonbh.Substring(3, 2));
                                preview.donViThanhToan = dvtt.tenDonVi;
                                preview.maSoThue = dvtt.maSoThue;
                                preview.diaChi = dvtt.diaChi;
                            }    
                            
                        }
                        else
                        {
                            preview.donViThanhToan = "";
                            preview.maSoThue = "";
                            preview.diaChi = "";
                            preview.donViBaoHiem = "";

                        }

                        NhatKy ma_user_duyetbl = _context.NhatKies.Where(x => x.FrKey == hoSoGiamDinh.PrKey && x.MaTtrangGd.Equals("DBL")).FirstOrDefault();

                        if (ma_user_duyetbl != null)
                        {
                            DmUser userDuyetBL = _context.DmUsers.Where(x => x.Oid == ma_user_duyetbl.MaUser).FirstOrDefault();
                            preview.maUserDuyetBl = userDuyetBL.MaUser;
                            preview.tenUserDuyetBl = userDuyetBL.TenUser;
                            if (userDuyetBL.MaDonvi.Equals("31") || userDuyetBL.MaDonvi.Equals("32") || userDuyetBL.MaDonvi.Equals("00"))
                            {
                                if (userDuyetBL.LoaiUser != 10)
                                {
                                    preview.tenKy = "TUQ. GIÁM ĐỐC VĂN PHÒNG";
                                }
                                else
                                {
                                    preview.tenKy = "GIÁM ĐỐC VĂN PHÒNG";
                                }
                            }
                            else
                            {
                                preview.tenKy = "GIÁM ĐỐC";
                            }
                            return preview;
                        }
                        else
                        {
                            preview.maUserDuyetBl = "";
                            preview.tenUserDuyetBl = "";
                            preview.tenKy = "TUQ. GIÁM ĐỐC VĂN PHÒNG";
                            return preview;
                        }
                    }
                }

                else
                {
                    return null;
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
                return null;
            }

        }



        // Hàm này dùng để lấy danh sách các thông tin cần thay trên file Word gốc.
        // Sau đó, tổng hợp tất cả các thông tin đó vào 1 list dưới dạng key - value.

        public CombinedBaoLanhResult BaoLanh_GetListOfReplacable(decimal prKey, decimal pr_key_hsbt_ct, DmUser currentUser, string? ma_donvi_tt)
        {
            try
            {
                var wordPdfRequest = new WordToPdfRequest();
                wordPdfRequest.ListData = new List<EntityContent>();
                UpdateProperties update = new UpdateProperties();

                FormBaoLanh formBaoLanh = BaoLanh_GetInfo(prKey, pr_key_hsbt_ct, currentUser, ma_donvi_tt);

                if (formBaoLanh != null)
                {

                    update.AddEntityContent(wordPdfRequest, "[so_hsgd]", formBaoLanh.soHsgd);

                    update.AddEntityContent(wordPdfRequest, "[current_day]", formBaoLanh.ngayTao.Day.ToString());
                    update.AddEntityContent(wordPdfRequest, "[current_month]", formBaoLanh.ngayTao.Month.ToString());
                    update.AddEntityContent(wordPdfRequest, "[current_year]", formBaoLanh.ngayTao.Year.ToString());

                    update.AddEntityContent(wordPdfRequest, "[nguoi_nhan]", formBaoLanh.nguoiNhan);
                    update.AddEntityContent(wordPdfRequest, "[bien_ks]", formBaoLanh.bienKiemSoat);
                    update.AddEntityContent(wordPdfRequest, "[ten_khach]", formBaoLanh.tenKhach);
                    update.AddEntityContent(wordPdfRequest, "[ten_donvi_bh]", formBaoLanh.donViBaoHiem);
                    update.AddEntityContent(wordPdfRequest, "[giam_dinh_vien]", formBaoLanh.giamDinhVien);
                    update.AddEntityContent(wordPdfRequest, "[tong_tien_gom_vat]", formBaoLanh.tongSoTienGomVAT.ToString());
                    update.AddEntityContent(wordPdfRequest, "[so_tien_giam_gia]", formBaoLanh.soTienGiamGia.ToString());
                    update.AddEntityContent(wordPdfRequest, "[so_tien_giam_tru]", formBaoLanh.soTienGiamTru.ToString());
                    update.AddEntityContent(wordPdfRequest, "[so_tien_khau_tru]", formBaoLanh.soTienKhauTru.ToString());
                    update.AddEntityContent(wordPdfRequest, "[so_tien_doi_tru]", formBaoLanh.soTienDoiTru.ToString());
                    List<string> list_lyDoKhauTru = ContentHelper.SplitString(ContentHelper.formatNewLine(formBaoLanh.lyDoKhauTru), 255);
                    for (int i = 0; i < list_lyDoKhauTru.Count(); i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[ly_do_khau_tru{i}]", list_lyDoKhauTru[i]);
                    }
                    int dem = list_lyDoKhauTru.Count();
                    if (dem == 0)
                    {
                        dem = 1;
                    }
                    for (int i = dem; i < 20; i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[ly_do_khau_tru{i}]", "");
                    }
                    //update.AddEntityContent(wordPdfRequest, "[ly_do_khau_tru]", formBaoLanh.lyDoKhauTru);
                    update.AddEntityContent(wordPdfRequest, "[so_tien_trach_nhiem_PVI]", formBaoLanh.soTienTrachNhiemPVI.ToString());
                    update.AddEntityContent(wordPdfRequest, "[so_tien_bang_chu]", formBaoLanh.soTienTrachNhiemPVIBangChu);
                    update.AddEntityContent(wordPdfRequest, "[don_vi_thanh_toan]", formBaoLanh.donViThanhToan);
                    update.AddEntityContent(wordPdfRequest, "[ma_so_thue]", formBaoLanh.maSoThue);
                    update.AddEntityContent(wordPdfRequest, "[dia_chi]", formBaoLanh.diaChi);
                    update.AddEntityContent(wordPdfRequest, "#1", (formBaoLanh.bl1 == 1 ? "☑" : "☐"));
                    update.AddEntityContent(wordPdfRequest, "#2", (formBaoLanh.bl2 == 1 ? "☑" : "☐"));
                    update.AddEntityContent(wordPdfRequest, "#3", (formBaoLanh.bl3 == 1 ? "☑" : "☐"));
                    update.AddEntityContent(wordPdfRequest, "#4", (formBaoLanh.bl4 == 1 ? "☑" : "☐"));
                    update.AddEntityContent(wordPdfRequest, "#5", (formBaoLanh.bl5 == 1 ? "☑" : "☐"));
                    update.AddEntityContent(wordPdfRequest, "#6", (formBaoLanh.bl6 == 1 ? "☑" : "☐"));
                    update.AddEntityContent(wordPdfRequest, "#7", (formBaoLanh.bl7 == 1 ? "☑" : "☐"));
                    update.AddEntityContent(wordPdfRequest, "#8", (formBaoLanh.bl8 == 1 ? "☑" : "☐"));
                    update.AddEntityContent(wordPdfRequest, "#9", (formBaoLanh.bl9 == 1 ? "☑" : "☐"));
                    update.AddEntityContent(wordPdfRequest, "[tai_lieu_bo_sung]", formBaoLanh.taiLieuCanBoSung);
                    update.AddEntityContent(wordPdfRequest, "[ten_ky]", formBaoLanh.tenKy);

                    switch (formBaoLanh.maDonvi)
                    {
                        case "31":
                            update.AddEntityContent(wordPdfRequest, "[ten_tru_so]", "BL/CSB-XCG");
                            update.AddEntityContent(wordPdfRequest, "[noi_luu]", "CSB, 1");
                            update.AddEntityContent(wordPdfRequest, "[ten_donvi_gdv]", "VPĐD CSKH BH PVI PHÍA BẮC");
                            update.AddEntityContent(wordPdfRequest, "[tieu_de_bao_lanh]", $"Số tiền bảo lãnh trên sẽ được thanh toán cho Quý đơn vị trong vòng 30 ngày làm việc kể từ ngày {formBaoLanh.donViBaoHiem} nhận được đầy đủ bản gốc các tài liệu sau");
                            update.AddEntityContent(wordPdfRequest, "[ten_tru_so]", "BL/CSB-XCG");
                            break;

                        case "32":
                            update.AddEntityContent(wordPdfRequest, "[ten_tru_so]", "BL/CSN-XCG");
                            update.AddEntityContent(wordPdfRequest, "[noi_luu]", "CSN, 1");
                            update.AddEntityContent(wordPdfRequest, "[ten_donvi_gdv]", "VPĐD CSKH BH PVI PHÍA NAM");
                            update.AddEntityContent(wordPdfRequest, "[tieu_de_bao_lanh]", $"Số tiền bảo lãnh trên sẽ được thanh toán cho Quý đơn vị trong vòng 30 ngày làm việc kể từ ngày {formBaoLanh.donViBaoHiem} nhận được đầy đủ bản gốc các tài liệu sau");
                            update.AddEntityContent(wordPdfRequest, "[ten_tru_so]", "BL/CSN-XCG");
                            break;

                        default:
                            update.AddEntityContent(wordPdfRequest, "[ten_tru_so]", "BL/GQKN");
                            update.AddEntityContent(wordPdfRequest, "[noi_luu]", "GQKN, 1");
                            update.AddEntityContent(wordPdfRequest, "[ten_donvi_gdv]", "Bảo hiểm PVI");
                            update.AddEntityContent(wordPdfRequest, "[tieu_de_bao_lanh]", $"Chúng tôi sẽ thanh toán số tiền bảo lãnh trên cho Quý đơn vị trong vòng 30 ngày làm việc kể từ ngày PVI nhận được đầy đủ bản gốc các tài liệu sau:");
                            update.AddEntityContent(wordPdfRequest, "[ten_tru_so]", "BL/GQKN");
                            break;
                    }


                    update.AddEntityContent(wordPdfRequest, "[ma_user_duyet_bl]", formBaoLanh.maUserDuyetBl);
                    update.AddEntityContent(wordPdfRequest, "[ten_user_duyet_bl]", formBaoLanh.tenUserDuyetBl);

                    var listData = wordPdfRequest.ListData;
                    _logger.Information("Print Bao Lanh " + JsonConvert.SerializeObject(listData));
                    var listNew = new CombinedBaoLanhResult
                    {
                        ThirdQueryResults = listData,
                    };

                    return listNew;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                return null;
            }
        }

        // Hàm tạo PDF chuyên dùng cho preview

        public CombinedBaoLanhResult BaoLanh_GetListOfReplacable_Preview(decimal prKey, decimal pr_key_hsbt_ct, DmUser currentUser, string? ma_donvi_tt)
        {
            try
            {
                var wordPdfRequest = new WordToPdfRequest();
                wordPdfRequest.ListData = new List<EntityContent>();
                UpdateProperties update = new UpdateProperties();

                FormBaoLanh formBaoLanh = BaoLanh_GetInfo_Preview(prKey, pr_key_hsbt_ct, currentUser, ma_donvi_tt);

                if (formBaoLanh != null)
                {

                    update.AddEntityContent(wordPdfRequest, "[so_hsgd]", formBaoLanh.soHsgd);

                    update.AddEntityContent(wordPdfRequest, "[current_day]", formBaoLanh.ngayTao.Day.ToString());
                    update.AddEntityContent(wordPdfRequest, "[current_month]", formBaoLanh.ngayTao.Month.ToString());
                    update.AddEntityContent(wordPdfRequest, "[current_year]", formBaoLanh.ngayTao.Year.ToString());

                    update.AddEntityContent(wordPdfRequest, "[nguoi_nhan]", formBaoLanh.nguoiNhan);
                    update.AddEntityContent(wordPdfRequest, "[bien_ks]", formBaoLanh.bienKiemSoat);
                    update.AddEntityContent(wordPdfRequest, "[ten_khach]", formBaoLanh.tenKhach);
                    update.AddEntityContent(wordPdfRequest, "[ten_donvi_bh]", formBaoLanh.donViBaoHiem);
                    update.AddEntityContent(wordPdfRequest, "[giam_dinh_vien]", formBaoLanh.giamDinhVien);
                    update.AddEntityContent(wordPdfRequest, "[tong_tien_gom_vat]", formBaoLanh.tongSoTienGomVAT.ToString());
                    update.AddEntityContent(wordPdfRequest, "[so_tien_giam_gia]", formBaoLanh.soTienGiamGia.ToString());
                    update.AddEntityContent(wordPdfRequest, "[so_tien_giam_tru]", formBaoLanh.soTienGiamTru.ToString());
                    update.AddEntityContent(wordPdfRequest, "[so_tien_doi_tru]", formBaoLanh.soTienDoiTru.ToString());
                    update.AddEntityContent(wordPdfRequest, "[so_tien_khau_tru]", formBaoLanh.soTienKhauTru.ToString());
                    List<string> list_lyDoKhauTru = ContentHelper.SplitString(ContentHelper.formatNewLine(formBaoLanh.lyDoKhauTru), 255);
                    for (int i = 0; i < list_lyDoKhauTru.Count(); i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[ly_do_khau_tru{i}]", list_lyDoKhauTru[i]);
                    }
                    int dem = list_lyDoKhauTru.Count();
                    if (dem == 0)
                    {
                        dem = 1;
                    }
                    for (int i = dem; i < 20; i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[ly_do_khau_tru{i}]", "");
                    }
                    //update.AddEntityContent(wordPdfRequest, "[ly_do_khau_tru]", formBaoLanh.lyDoKhauTru);
                    update.AddEntityContent(wordPdfRequest, "[so_tien_trach_nhiem_PVI]", formBaoLanh.soTienTrachNhiemPVI.ToString());
                    update.AddEntityContent(wordPdfRequest, "[so_tien_bang_chu]", formBaoLanh.soTienTrachNhiemPVIBangChu);
                    update.AddEntityContent(wordPdfRequest, "[don_vi_thanh_toan]", formBaoLanh.donViThanhToan);
                    update.AddEntityContent(wordPdfRequest, "[ma_so_thue]", formBaoLanh.maSoThue);
                    update.AddEntityContent(wordPdfRequest, "[dia_chi]", formBaoLanh.diaChi);
                    update.AddEntityContent(wordPdfRequest, "#1", "☐");
                    update.AddEntityContent(wordPdfRequest, "#2", "☐");
                    update.AddEntityContent(wordPdfRequest, "#3", "☐");
                    update.AddEntityContent(wordPdfRequest, "#4", "☐");
                    update.AddEntityContent(wordPdfRequest, "#5", "☐");
                    update.AddEntityContent(wordPdfRequest, "#6", "☐");
                    update.AddEntityContent(wordPdfRequest, "#7", "☐");
                    update.AddEntityContent(wordPdfRequest, "#8", "☐");
                    update.AddEntityContent(wordPdfRequest, "#9", "☐");
                    update.AddEntityContent(wordPdfRequest, "[tai_lieu_bo_sung]", "");
                    update.AddEntityContent(wordPdfRequest, "[ten_ky]", formBaoLanh.tenKy);

                    switch (formBaoLanh.maDonvi)
                    {
                        case "31":
                            update.AddEntityContent(wordPdfRequest, "[ten_tru_so]", "BL/CSB-XCG");
                            update.AddEntityContent(wordPdfRequest, "[noi_luu]", "CSB, 1");
                            update.AddEntityContent(wordPdfRequest, "[ten_donvi_gdv]", "VPĐD CSKH BH PVI PHÍA BẮC");
                            update.AddEntityContent(wordPdfRequest, "[tieu_de_bao_lanh]", $"Số tiền bảo lãnh trên sẽ được thanh toán cho Quý đơn vị trong vòng 30 ngày làm việc kể từ ngày {formBaoLanh.donViBaoHiem} nhận được đầy đủ bản gốc các tài liệu sau");
                            update.AddEntityContent(wordPdfRequest, "[ten_tru_so]", "BL/CSB-XCG");
                            break;

                        case "32":
                            update.AddEntityContent(wordPdfRequest, "[ten_tru_so]", "BL/CSN-XCG");
                            update.AddEntityContent(wordPdfRequest, "[noi_luu]", "CSN, 1");
                            update.AddEntityContent(wordPdfRequest, "[ten_donvi_gdv]", "VPĐD CSKH BH PVI PHÍA NAM");
                            update.AddEntityContent(wordPdfRequest, "[tieu_de_bao_lanh]", $"Số tiền bảo lãnh trên sẽ được thanh toán cho Quý đơn vị trong vòng 30 ngày làm việc kể từ ngày {formBaoLanh.donViBaoHiem} nhận được đầy đủ bản gốc các tài liệu sau");
                            update.AddEntityContent(wordPdfRequest, "[ten_tru_so]", "BL/CSN-XCG");
                            break;

                        default:
                            update.AddEntityContent(wordPdfRequest, "[ten_tru_so]", "BL/GQKN");
                            update.AddEntityContent(wordPdfRequest, "[noi_luu]", "GQKN, 1");
                            update.AddEntityContent(wordPdfRequest, "[ten_donvi_gdv]", "Bảo hiểm PVI");
                            update.AddEntityContent(wordPdfRequest, "[tieu_de_bao_lanh]", $"Chúng tôi sẽ thanh toán số tiền bảo lãnh trên cho Quý đơn vị trong vòng 30 ngày làm việc kể từ ngày PVI nhận được đầy đủ bản gốc các tài liệu sau:");
                            update.AddEntityContent(wordPdfRequest, "[ten_tru_so]", "BL/GQKN");
                            break;
                    }


                    update.AddEntityContent(wordPdfRequest, "[ma_user_duyet_bl]", formBaoLanh.maUserDuyetBl);
                    update.AddEntityContent(wordPdfRequest, "[ten_user_duyet_bl]", formBaoLanh.tenUserDuyetBl);

                    var listData = wordPdfRequest.ListData;
                    _logger.Information("Print Bao Lanh " + JsonConvert.SerializeObject(listData));
                    var listNew = new CombinedBaoLanhResult
                    {
                        ThirdQueryResults = listData,
                    };

                    return listNew;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                return null;
            }
        }
        // Hàm này dùng để lấy danh sách các thông tin cần thay trên file Word gốc.
        // Sau đó, tổng hợp tất cả các thông tin đó vào 1 list dưới dạng key - value.

        public CombinedBaoLanhResult BaoLanh_GetListPreview(decimal prKey, decimal pr_key_hsbt_ct, DmUser currentUser, string? ma_donvi_tt)
        {
            try
            {
                var wordPdfRequest = new WordToPdfRequest();
                wordPdfRequest.ListData = new List<EntityContent>();
                UpdateProperties update = new UpdateProperties();

                FormBaoLanh formBaoLanh = BaoLanh_GetInfo(prKey, pr_key_hsbt_ct, currentUser, ma_donvi_tt);

                if (formBaoLanh != null)
                {

                    update.AddEntityContent(wordPdfRequest, "[so_hsgd]", formBaoLanh.soHsgd);

                    update.AddEntityContent(wordPdfRequest, "[current_day]", formBaoLanh.ngayTao.Day.ToString());
                    update.AddEntityContent(wordPdfRequest, "[current_month]", formBaoLanh.ngayTao.Month.ToString());
                    update.AddEntityContent(wordPdfRequest, "[current_year]", formBaoLanh.ngayTao.Year.ToString());

                    update.AddEntityContent(wordPdfRequest, "[nguoi_nhan]", formBaoLanh.nguoiNhan);
                    update.AddEntityContent(wordPdfRequest, "[bien_ks]", formBaoLanh.bienKiemSoat);
                    update.AddEntityContent(wordPdfRequest, "[ten_khach]", formBaoLanh.tenKhach);
                    update.AddEntityContent(wordPdfRequest, "[ten_donvi_bh]", formBaoLanh.donViBaoHiem);
                    update.AddEntityContent(wordPdfRequest, "[giam_dinh_vien]", formBaoLanh.giamDinhVien);
                    update.AddEntityContent(wordPdfRequest, "[tong_tien_gom_vat]", formBaoLanh.tongSoTienGomVAT.ToString());
                    update.AddEntityContent(wordPdfRequest, "[so_tien_giam_gia]", formBaoLanh.soTienGiamGia.ToString());
                    update.AddEntityContent(wordPdfRequest, "[so_tien_giam_tru]", formBaoLanh.soTienGiamTru.ToString());
                    update.AddEntityContent(wordPdfRequest, "[so_tien_doi_tru]", formBaoLanh.soTienDoiTru.ToString());
                    update.AddEntityContent(wordPdfRequest, "[so_tien_khau_tru]", formBaoLanh.soTienKhauTru.ToString());
                    List<string> list_lyDoKhauTru = ContentHelper.SplitString(ContentHelper.formatNewLine(formBaoLanh.lyDoKhauTru), 255);
                    for (int i = 0; i < list_lyDoKhauTru.Count(); i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[ly_do_khau_tru{i}]", list_lyDoKhauTru[i]);
                    }
                    int dem = list_lyDoKhauTru.Count();
                    if (dem == 0)
                    {
                        dem = 1;
                    }
                    for (int i = dem; i < 20; i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[ly_do_khau_tru{i}]", "");
                    }
                    //update.AddEntityContent(wordPdfRequest, "[ly_do_khau_tru]", formBaoLanh.lyDoKhauTru);
                    update.AddEntityContent(wordPdfRequest, "[so_tien_trach_nhiem_PVI]", formBaoLanh.soTienTrachNhiemPVI.ToString());
                    update.AddEntityContent(wordPdfRequest, "[so_tien_bang_chu]", formBaoLanh.soTienTrachNhiemPVIBangChu);
                    update.AddEntityContent(wordPdfRequest, "[don_vi_thanh_toan]", formBaoLanh.donViThanhToan);
                    update.AddEntityContent(wordPdfRequest, "[ma_so_thue]", formBaoLanh.maSoThue);
                    update.AddEntityContent(wordPdfRequest, "[dia_chi]", formBaoLanh.diaChi);
                    update.AddEntityContent(wordPdfRequest, "#1", (formBaoLanh.bl1 == 1 ? "☑" : "☐"));
                    update.AddEntityContent(wordPdfRequest, "#2", (formBaoLanh.bl2 == 1 ? "☑" : "☐"));
                    update.AddEntityContent(wordPdfRequest, "#3", (formBaoLanh.bl3 == 1 ? "☑" : "☐"));
                    update.AddEntityContent(wordPdfRequest, "#4", (formBaoLanh.bl4 == 1 ? "☑" : "☐"));
                    update.AddEntityContent(wordPdfRequest, "#5", (formBaoLanh.bl5 == 1 ? "☑" : "☐"));
                    update.AddEntityContent(wordPdfRequest, "#6", (formBaoLanh.bl6 == 1 ? "☑" : "☐"));
                    update.AddEntityContent(wordPdfRequest, "#7", (formBaoLanh.bl7 == 1 ? "☑" : "☐"));
                    update.AddEntityContent(wordPdfRequest, "#8", (formBaoLanh.bl8 == 1 ? "☑" : "☐"));
                    update.AddEntityContent(wordPdfRequest, "#9", (formBaoLanh.bl9 == 1 ? "☑" : "☐"));
                    update.AddEntityContent(wordPdfRequest, "[tai_lieu_bo_sung]", formBaoLanh.taiLieuCanBoSung);
                    update.AddEntityContent(wordPdfRequest, "[ten_ky]", formBaoLanh.tenKy);

                    switch (formBaoLanh.maDonvi)
                    {
                        case "31":
                            update.AddEntityContent(wordPdfRequest, "[ten_tru_so]", "BL/CSB-XCG");
                            update.AddEntityContent(wordPdfRequest, "[noi_luu]", "CSB, 1");
                            update.AddEntityContent(wordPdfRequest, "[ten_donvi_gdv]", "VPĐD CSKH BH PVI PHÍA BẮC");
                            update.AddEntityContent(wordPdfRequest, "[tieu_de_bao_lanh]", $"Số tiền bảo lãnh trên sẽ được thanh toán cho Quý đơn vị trong vòng 30 ngày làm việc kể từ ngày {formBaoLanh.donViBaoHiem} nhận được đầy đủ bản gốc các tài liệu sau");
                            update.AddEntityContent(wordPdfRequest, "[ten_tru_so]", "BL/CSB-XCG");
                            break;

                        case "32":
                            update.AddEntityContent(wordPdfRequest, "[ten_tru_so]", "BL/CSN-XCG");
                            update.AddEntityContent(wordPdfRequest, "[noi_luu]", "CSN, 1");
                            update.AddEntityContent(wordPdfRequest, "[ten_donvi_gdv]", "VPĐD CSKH BH PVI PHÍA NAM");
                            update.AddEntityContent(wordPdfRequest, "[tieu_de_bao_lanh]", $"Số tiền bảo lãnh trên sẽ được thanh toán cho Quý đơn vị trong vòng 30 ngày làm việc kể từ ngày {formBaoLanh.donViBaoHiem} nhận được đầy đủ bản gốc các tài liệu sau");
                            update.AddEntityContent(wordPdfRequest, "[ten_tru_so]", "BL/CSN-XCG");
                            break;

                        default:
                            update.AddEntityContent(wordPdfRequest, "[ten_tru_so]", "BL/GQKN");
                            update.AddEntityContent(wordPdfRequest, "[noi_luu]", "GQKN, 1");
                            update.AddEntityContent(wordPdfRequest, "[ten_donvi_gdv]", "Bảo hiểm PVI");
                            update.AddEntityContent(wordPdfRequest, "[tieu_de_bao_lanh]", $"Chúng tôi sẽ thanh toán số tiền bảo lãnh trên cho Quý đơn vị trong vòng 30 ngày làm việc kể từ ngày PVI nhận được đầy đủ bản gốc các tài liệu sau:");
                            update.AddEntityContent(wordPdfRequest, "[ten_tru_so]", "BL/GQKN");
                            break;
                    }


                    update.AddEntityContent(wordPdfRequest, "[ma_user_duyet_bl]", formBaoLanh.maUserDuyetBl);
                    update.AddEntityContent(wordPdfRequest, "[ten_user_duyet_bl]", formBaoLanh.tenUserDuyetBl);

                    var listData = wordPdfRequest.ListData;
                    _logger.Information("Print Bao Lanh " + JsonConvert.SerializeObject(listData));
                    var listNew = new CombinedBaoLanhResult
                    {
                        ThirdQueryResults = listData,
                    };

                    return listNew;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                return null;
            }
        }


        // Hàm sử dụng để tạo HTML Body cho email gửi bảo lãnh
        private string HTMLBody_BaoLanh(FormBaoLanh data)
        {
            string htmlBody = "";
            // Generate and return an AlternateView for email body
            if (data != null)
            {
                htmlBody = "<html xmlns=\"http://www.w3.org/1999/xhtml\"> "
       + "<head runat=\"server\"> "
       + "    <title></title> "
       + "    <style type=\"text/css\"> "
       + "        #content p { "
       + "            margin: 5px 0; "
       + "            color: #085d60; "
       + "        } "
       + " "
       + "        * { "
       + "            margin: 0; "
       + "            padding: 0; "
       + "        } "
       + " "
       + "        a { "
       + "            text-decoration: none; "
       + "        } "
       + "    </style> "
       + "</head> "
       + "<body> "
       + "    <form id=\"form1\" runat=\"server\"> "
       + "        <table align=\"center\" style=\"border-collapse: collapse\" bordercolor=\"#0185D0\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\"> "
       + "            <tr> "
       + "                <td style=\"width: 100%; height: 130px;\"> "
       + "                    <img src=\"cid:Pic1\" style=\"width: 100%; height: 130px; border: none; margin: 0; display: block\" />                     "
       + "                </td> "
       + "            </tr> "
       + "            <tr> "
       + "                <td style=\"font-family: Arial; font-size: 13px; padding: 5px; vertical-align: top; text-align:justify\"> "
       + "                    <p style=\"MARGIN-TOP: 30px\"> "
       + "                        <strong>lblNg_gdich</strong> "
       + "                    </p> "
       + "                    <p style=\"MARGIN-TOP: 20px\"> "
       + "                        lblTitle"
       + "                    </p> ";
                htmlBody = htmlBody.Replace("lblTitle", "lbl_thong_bao");
                htmlBody = htmlBody
                + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px; font-weight:bold;color:red\"> "
                + "                        - Tên chủ xe: lbl_tenkh_duocbaolanh"
                + "                    </p> "
                + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
                + "                        - BKS/Số khung: lbl_bienksoat"
                + "                    </p> "
                + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
                + "                        - Ngày Tổn thất: lbl_ngaytt"
                + "                    </p> "
                + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
                + "                        - Ngày Thông báo: lbl_ngaytb"
                + "                    </p> "
                + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
                + "                        - Ng.nhân TT: lbl_nguyennhan"
                + "                    </p> ";
                htmlBody = htmlBody + "    lbl_loaiguiaa điện tử được đính kèm tại email này có giá trị pháp lý và là căn cứ xem xét thanh toán theo đúng các quy định tại Thỏa thuận Hợp tác đã ký kết giữa hai bên.";
                htmlBody = htmlBody + "    Trân trọng cảm ơn!"
                    + "                    <p style=\"MARGIN-TOP: 10px; font-weight:bold\">Tổng Công ty Bảo hiểm PVI</p> "
                    + "                    <p>Địa chỉ: PVI Tower, Số 1 Phạm Văn Bạch, Quận Cầu Giấy, Hà Nội.</p> "
                    + "                    <p>Website: <a href=\"http://www.pvi.com.vn\" style=\"color:blue\">http://www.pvi.com.vn</a></p> "
                    + "                    <p style=\"MARGIN-TOP: 10px; font-weight:bold\">Hỗ trợ mua bảo hiểm trực tuyến 24/7: 1900 54 54 58</p> "
                    + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\">- Hỗ trợ qua email: <a href=\"mailto:Support@pvi.com.vn\" style=\"color:blue\">Support@pvi.com.vn</a></p> "
                    + "                    <p style=\"font-style:italic\"> (*) Đây là email hệ thống gửi tự động, vui lòng không trả lời (reply) lại email này.</p> "
                    + "                </td> "
                    + "            </tr> "
                    + "        </table> "
                    + "    </form> "
                    + "</body> "
                    + "</html>";

                htmlBody = htmlBody.Replace("lblNg_gdich", " Kính gửi : " + data.TenGara);
                htmlBody = htmlBody.Replace("lbl_thong_bao", (" Liên quan đến xe ô tô BKS: " + data.bienKiemSoat + " tham gia BH tại " + data.donViBaoHiem + " đã được Quý Công ty thực hiện sửa chữa và do Giám định viên của Bảo hiểm PVI: " + data.giamDinhVien + " tiến hành giám định tổn thất, Bảo hiểm PVI đồng ý bảo lãnh thanh toán cho xe trên với các nội dung như file bảo lãnh điện tử số: " + DateTime.Today.Year.ToString() + "-" + data.maDonvi + "-" + data.soHsgd + " đính kèm"));
                htmlBody = htmlBody.Replace("lbl_tenkh_duocbaolanh", data.tenKhach);
                htmlBody = htmlBody.Replace("lblngay_dau", data.NgayDau);
                htmlBody = htmlBody.Replace("lblngay_cuoi", data.NgayCuoi);
                htmlBody = htmlBody.Replace("lbl_bienksoat", data.bienKiemSoat);
                htmlBody = htmlBody.Replace("lbl_ngaytt", data.NgayTThat);
                htmlBody = htmlBody.Replace("lbl_ngaytb", data.NgayTBao);
                htmlBody = htmlBody.Replace("lbl_nguyennhan", data.NguyenNhan);
                htmlBody = htmlBody.Replace("lbl_loaiguiaa", "Thư bảo lãnh");
            }

            return htmlBody;
        }

        // Sử dụng để tạo Alternative View Email bảo lãnh, từ body nhận được trong hàm trên.
        private static AlternateView createAVT_HTML(string htmlBody)
        {

            //string wordPdfRequestPath = System.AppDomain.CurrentDomain.BaseDirectory;
            //string image = wordPdfRequestPath + "imag.jpg";
            //LinkedResource pic = new LinkedResource(image, MediaTypeNames.Image.Jpeg);
            //pic.ContentId = "Pic1";
            //avHtml.LinkedResources.Add(pic);

            AlternateView avHtml = AlternateView.CreateAlternateViewFromString(htmlBody, null/* TODO Change to default(_) if this is not a reference type */, MediaTypeNames.Text.Html);
            return avHtml;
        }



        // Dùng cho Dev, kiểm tra reloadSum của từng hồ sơ:
        public ReloadSumChecker checkReloadSum(int prKey)
        {

            HsgdCtu hoSoGiamDinh = _context.HsgdCtus.Where(x => x.PrKey == prKey).FirstOrDefault();

            ReloadSumChecker result = new ReloadSumChecker();

            List<HsbtCt> list_ct = (from hsbtCt in _context_pias.HsbtCts
                                    where hsbtCt.FrKey == hoSoGiamDinh.PrKeyBt
                                    select new HsbtCt
                                    {
                                        PrKey = hsbtCt.PrKey
                                    }).ToList();

            List<HsbtCtView> listPhaiTraBT = _dx_Repo.GetListPhaiTraBT(hoSoGiamDinh.PrKey);
            result.HSBTView = listPhaiTraBT;

            if (listPhaiTraBT.Count > 0)
            {
                for (int i = 0; i < list_ct.Count; i++)
                {
                    // Hiện mới chỉ xử lý cho loại sản phẩm vật chất xe 050104.
                    //if (list_ct[i].MaSp == "050104")
                    //{
                    HsgdDxCt to_be_added = _context.HsgdDxCts.Where(x => x.PrKey == listPhaiTraBT[i].PrKeyHsgdDxCt).FirstOrDefault();
                    if (to_be_added != null)
                    {
                        List<HsgdDxSum> reloadedSum = _dx_Repo.ReloadSum(to_be_added.PrKey);
                        if (reloadedSum.Count > 0)
                        {
                            result.ReloadSum.Add(reloadedSum);
                        }
                    }
                    //}
                }
            }


            //if (hoSoGiamDinh.MaGaraVcx != null && hoSoGiamDinh.MaGaraVcx != "")
            //{

            return result;
        }

    }
}