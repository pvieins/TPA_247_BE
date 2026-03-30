using AutoMapper;
using PVI.Repository.Interfaces;
using PVI.Repository.Repositories;
using PVI.DAO.Entities.Models;
using PVI.Service.Request;
using Azure.Core;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using PVI.Helper;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using Microsoft.Office.Interop.Word;
using ICSharpCode.SharpZipLib.Core;
using System.Collections.Generic;
using System;

namespace PVI.Service.ActionProcess
{
    public class HsgdTtrinhService
    {
        private readonly IHsgdTtrinhRepository _hsgdTtrinhRepository;
        private readonly IHsgdTtrinhCtRepository _hsgdTtrinhCtRepository;
        private readonly IHsgdTotrinhXmlRepository _hsgdTotrinhXmlRepository;
        private readonly IHsgdCtuRepository _hsgdCtuRepository;
        private readonly Serilog.ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public HsgdTtrinhService(IHsgdTtrinhRepository hsgdTtrinhRepository, IHsgdTtrinhCtRepository hsgdTtrinhCtRepository, IHsgdTotrinhXmlRepository hsgdTotrinhXmlRepository, IHsgdCtuRepository hsgdCtuRepository, IMapper mapper, Serilog.ILogger logger, IConfiguration conf)
        {
            _hsgdTtrinhRepository = hsgdTtrinhRepository;
            _hsgdTtrinhCtRepository = hsgdTtrinhCtRepository;
            _hsgdTotrinhXmlRepository = hsgdTotrinhXmlRepository;
            _hsgdCtuRepository = hsgdCtuRepository;
            _mapper = mapper;
            _logger = logger;
            _configuration = conf;
        }
        public async Task<string> CreateHsgdTtrinh(TtrinhRequest entity, string email)
        {
            try
            {
                var hsgdTtrinh = _mapper.Map<HsgdTtrinhRequest, HsgdTtrinh>(entity.hsgdTtrinh);
                var hsgdTtrinhCt = _mapper.Map<List<HsgdTtrinhCtRequest>, List<HsgdTtrinhCt>>(entity.hsgdTtrinhCt);
                var result = await _hsgdTtrinhRepository.CreateHsgdTtrinh(hsgdTtrinh, hsgdTtrinhCt, entity.hsgdTtrinhTt, email);

                return result;
            }
            catch (Exception ex)
            {
                //_logger.Error("CreateHsgdTtrinh:", ex);
                //_logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
            }
            return null!;
        }
        public string UpdateHsgdTtrinh(TtrinhRequest entity, string email)
        {
            var result = "";
            try
            {
                var hsgdTtrinh_old = _hsgdTtrinhRepository.GetByIdNoAsync(entity.hsgdTtrinh.PrKey);
                if (hsgdTtrinh_old != null)
                {
                    entity.hsgdTtrinh.MaDonvi = hsgdTtrinh_old.MaDonvi;
                    entity.hsgdTtrinh.TenDttt = hsgdTtrinh_old.TenDttt;
                    entity.hsgdTtrinh.NgayCtu = hsgdTtrinh_old.NgayCtu;
                    entity.hsgdTtrinh.MaTtrang = hsgdTtrinh_old.MaTtrang;
                    entity.hsgdTtrinh.SoHsbt = hsgdTtrinh_old.SoHsbt;
                    entity.hsgdTtrinh.NgayTthat = hsgdTtrinh_old.NgayTthat;
                    entity.hsgdTtrinh.NgGdich = hsgdTtrinh_old.NgGdich;
                    entity.hsgdTtrinh.SoTien = Convert.ToInt64(entity.hsgdTtrinhCt.Sum(item => item.SotienBt));
                    var hsgdTtrinh_new = _mapper.Map(entity.hsgdTtrinh, hsgdTtrinh_old);
                    var hsgdTtrinhCt_old = _hsgdTtrinhCtRepository.GetListEntityByConditionNoAsync(x => x.FrKey == entity.hsgdTtrinh.PrKey);
                    var hsgdTtrinhCt_delete = hsgdTtrinhCt_old.Where(x => !entity.hsgdTtrinhCt.Select(x => x.PrKey).ToArray().Contains(x.PrKey)).ToList();
                    var hsgdTtrinhCt_new = _mapper.Map(entity.hsgdTtrinhCt, hsgdTtrinhCt_old);
                    result = _hsgdTtrinhRepository.UpdateHsgdTtrinh(hsgdTtrinh_new, hsgdTtrinhCt_new, entity.hsgdTtrinhTt, hsgdTtrinhCt_delete, email);
                }
                else
                {
                    result = "Entity not found";
                }
            }
            catch (Exception ex)
            {
                //_logger.Error("CreateHsgdTtrinh:", ex);
                //_logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
            }
            return result;
        }
        public DownloadFileResult PrintToTrinh(decimal pr_key, string email)
        {
            try
            {
                var result = _hsgdTtrinhRepository.GetPrintToTrinh(pr_key, email);

                var downloadSettings = _configuration.GetSection("DownloadSettings").Get<DownloadSettings>();
                var word2PdfSettings = _configuration.GetSection("Word2PdfSettings").Get<Word2PdfSettings>();

                var optionsDownloadSettings = Microsoft.Extensions.Options.Options.Create(downloadSettings);
                var optionsWord2PdfSettings = Microsoft.Extensions.Options.Options.Create(word2PdfSettings);

                var contentHelper = new ContentHelper(optionsDownloadSettings, optionsWord2PdfSettings, _logger);

                DownloadFileResult result1 = null;

                if (result != null && result.ThirdQueryResults != null)
                {
                    result1 = contentHelper.ConvertFileWordToPdf(result.ThirdQueryResults, result.ListGiamDinh, result.ListThuHuong, result.ChkChuanopphi);
                }

                //_logger.Information("PrintToTrinh success");
                return result1;
            }
            catch (Exception ex)
            {
            }
            return null;
        }
        public string TrinhKy(decimal pr_key, string email)
        {
            string result = "Thất bại";

            try
            {
                var tt = _hsgdTtrinhRepository.GetByIdNoAsync(pr_key);
                if (tt != null && (tt.MaTtrang == "" || tt.MaTtrang == "01") && tt.PathTtrinh == "")
                {

                    var result_print_tt = PrintToTrinh(pr_key, email);
                    //_logger.Information("PrintToTrinh " + JsonConvert.SerializeObject(result_print_tt));
                    string folderUpload = _configuration.GetValue<string>("UploadSettings:FolderUpload") ?? "";
                    string url_upload = _configuration.GetValue<string>("DownloadSettings:UlpoadServer") ?? "";
                    var utilityHelper = new UtilityHelper(_logger);
                    var file_path = utilityHelper.UploadFile_ToAPI(result_print_tt.Data, ".pdf", folderUpload, url_upload, false);
                    if (!string.IsNullOrEmpty(file_path))
                    {
                        tt.PathTtrinh = file_path;
                        tt.MaTtrang = "01";
                        _hsgdTtrinhRepository.Update(tt);
                        _hsgdTtrinhRepository.Save();
                        result = "Thành công";
                    }
                }
                else
                {
                    result = "Tờ trình không được chuyển duyệt";
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public DownloadFileResult DownloadTtrinh(decimal pr_key)
        {
            DownloadFileResult result = new DownloadFileResult();
            try
            {
                string pathFile = "";
                var tt = _hsgdTtrinhRepository.GetByIdNoAsync(pr_key);
                pathFile = tt != null ? tt.PathTtrinh : "";
                if (pathFile.ToLower().Contains("\\\\pvi.com.vn\\p247_upload_new\\"))
                {
                    string url_download = _configuration.GetValue<string>("DownloadSettings:DownloadServer") ?? "";
                    result = UtilityHelper.DownloadFile_ToAPI(pathFile, url_download);
                }
                else
                {
                    result.Status = "-500";
                    result.Message = "Invalid FilePath";
                }

            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public string TrinhKy_MDF1(decimal pr_key, string email)
        {
            string result = "Thất bại";
            try
            {
                _logger.Information("1 TrinhKy_MDF1 pr_key =" + pr_key.ToString());
                var tt = _hsgdTtrinhRepository.GetHsgdTtrinhByIdAsync(pr_key);
                _logger.Information("1 TrinhKy_MDF1 pr_key = " + tt.PrKey + " MaTtrang = " + tt.MaTtrang + " PathTtrinh = " + tt.PathTtrinh);
                if (tt != null && (tt.MaTtrang == "" || tt.MaTtrang == "01") && tt.PathTtrinh == "")
                {
                    _logger.Information("2 TrinhKy_MDF1 pr_key = " + tt.PrKey + " MaTtrang = " + tt.MaTtrang + " PathTtrinh = " + tt.PathTtrinh);
                    if ((tt.ChkDaydu == null && tt.ChkChuanopphi == false) || tt.ChkDunghan == null)
                    {
                        result = "Tờ trình chưa nhập tình trạng phí và thời gian nộp phí. Vui lòng kiểm tra lại!";
                        return result;
                    }
                    _logger.Information("3 TrinhKy_MDF1 pr_key = " + tt.PrKey + " MaTtrang = " + tt.MaTtrang + " PathTtrinh = " + tt.PathTtrinh);
                    var result_print_tt = PrintToTrinh(pr_key, email);
                    //_logger.Information("PrintToTrinh " + JsonConvert.SerializeObject(result_print_tt));
                    string folderUpload = _configuration.GetValue<string>("UploadSettings:FolderUpload_MDF1") ?? "";
                    string url_upload = _configuration.GetValue<string>("DownloadSettings:UlpoadServer_MDF1") ?? "";
                    var utilityHelper = new UtilityHelper(_logger);
                    var file_path = utilityHelper.UploadFile_ToAPI(result_print_tt.Data, ".pdf", folderUpload, url_upload, false);
                    _logger.Information("4 TrinhKy_MDF1 pr_key = " + tt.PrKey + " MaTtrang = " + tt.MaTtrang + " PathTtrinh = " + tt.PathTtrinh);
                    if (!string.IsNullOrEmpty(file_path))
                    {
                        _logger.Information("5 TrinhKy_MDF1 pr_key = " + tt.PrKey + " MaTtrang = " + tt.MaTtrang + " PathTtrinh = " + tt.PathTtrinh);
                        tt.PathTtrinh = file_path;
                        tt.MaTtrang = "01";
                        _hsgdTtrinhRepository.Update(tt);
                        _hsgdTtrinhRepository.Save();
                        _logger.Information("TrinhKy_MDF1 pr_key = " + pr_key + " PathTtrinh = " + file_path + " thành công");
                        result = "Thành công";
                    }
                }
                else
                {   _logger.Information("TrinhKy_MDF1 tt: " + JsonConvert.SerializeObject(tt));
                    result = "Tờ trình không được chuyển duyệt";
                }
            }
            catch (Exception ex)
            {
                _logger.Information("ex TrinhKy_MDF1 pr_key = " + pr_key);
                _logger.Error("TrinhKy_MDF1 thất bại " + ex);
            }
            return result;
        }
        public DownloadFileResult DownloadTtrinh_MDF1(decimal pr_key)
        {
            DownloadFileResult result = new DownloadFileResult();
            try
            {
                string pathFile = "";
                var tt = _hsgdTtrinhRepository.GetByIdNoAsync(pr_key);
                pathFile = tt != null ? tt.PathTtrinh : "";
                if (pathFile.ToLower().Contains("\\\\pvi.com.vn\\data\\pias_upload\\"))
                {
                    string url_download = _configuration.GetValue<string>("DownloadSettings:DownloadServer_MDF1") ?? "";
                    result = UtilityHelper.DownloadFile_ToAPI(pathFile, url_download);
                }
                else
                {
                    result.Status = "-500";
                    result.Message = "Invalid FilePath";
                }


            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public ListHsgdTtrinh GetListTtrinh(decimal pr_key_hsgd)
        {
            ListHsgdTtrinh obj_result = new ListHsgdTtrinh();
            try
            {
                obj_result = _hsgdTtrinhRepository.GetListTtrinh(pr_key_hsgd);

            }
            catch (Exception ex)
            {
            }
            return obj_result;
        }
        public HsgdTtrinhDetail GetTtrinhById(decimal pr_key)
        {
            HsgdTtrinhDetail obj_result = new HsgdTtrinhDetail();
            try
            {
                obj_result = _hsgdTtrinhRepository.GetTtrinhById(pr_key);

            }
            catch (Exception ex)
            {
            }
            return obj_result;
        }
        public SeriPhiBH GetSoPhiBH(string so_donbh, decimal so_seri)
        {
            SeriPhiBH seriPhiBH = new SeriPhiBH();
            try
            {
                seriPhiBH = _hsgdTtrinhRepository.GetSoPhiBH(so_donbh, so_seri);

            }
            catch (Exception ex)
            {
            }
            return seriPhiBH;

        }
        public CheckDKBS007 CheckDKBS007(decimal pr_key_hsgd)
        {
            try
            {
                var check_dkbs007 = _hsgdTtrinhRepository.CheckDKBS007(pr_key_hsgd);
                return check_dkbs007;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<string> DeleteHsgdTtrinh(Guid oid)
        {
            string result = "";
            try
            {
                var entity = _hsgdTtrinhRepository.GetTtrinhByOid(oid);
                if (entity == null)
                {
                    result = "Không tồn tại tờ trình";
                }
                else
                {
                    var hsgd_tt_ct = await _hsgdTtrinhCtRepository.GetListEntityByCondition(x => x.FrKey == entity.PrKey);
                    var hsgd_tt_xml = await _hsgdTotrinhXmlRepository.GetListEntityByCondition(x => hsgd_tt_ct.Select(x => x.PrKey).ToArray().Contains(x.FrKey));
                    _hsgdTtrinhCtRepository.DeleteAll(hsgd_tt_ct);
                    _hsgdTotrinhXmlRepository.DeleteAll(hsgd_tt_xml);
                    _hsgdTtrinhRepository.Delete(entity);
                    await _hsgdTtrinhRepository.SaveAsync();
                    result = "Xoá thành công";
                }
            }
            catch (Exception ex)
            {
                result = "Xoá thất bại";
            }
            return result;
        }
        public async Task<string> DeleteHsgdTtrinhCt(decimal pr_key)
        {
            string result = "";
            try
            {
                var entity = await _hsgdTtrinhCtRepository.GetById(pr_key);
                if (entity == null)
                {
                    return "Không tồn tại chi tiết tờ trình";
                }

                _hsgdTtrinhCtRepository.Delete(entity);
                await _hsgdTtrinhCtRepository.SaveAsync();
                result = "Xoá thành công";
            }
            catch (Exception ex)
            {
                result = "Xoá thất bại";
            }
            return result;
        }
        public string UpdateTrangThaiHsbtCt(decimal pr_key_hsgd_ttrinh)
        {
            var result = "";
            try
            {
                result = _hsgdTtrinhRepository.UpdateTrangThaiHsbtCt(pr_key_hsgd_ttrinh);
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public string ChuyenDuyet(decimal pr_key_hsgd_ttrinh, string email_login, string oid_nhan, bool send_email)
        {
            string result = "";
            try
            {
                result = _hsgdTtrinhRepository.ChuyenDuyet(pr_key_hsgd_ttrinh, email_login, oid_nhan, send_email);
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public string KyHoSo(decimal pr_key_hsgd_ttrinh, string email_login)
        {
            string result = "";
            try
            {
                result = _hsgdTtrinhRepository.KyHoSo(pr_key_hsgd_ttrinh, email_login);
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public CheckHD CheckKyHoSo(decimal pr_key_hsgd_ttrinh)
        {
            CheckHD result = new CheckHD();
            try
            {
                result = _hsgdTtrinhRepository.CheckKyHoSo(pr_key_hsgd_ttrinh);
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public string ChuyenHoSo(decimal pr_key_hsgd_ttrinh, string email_login, string oid_nhan, bool send_email)
        {
            string result = "";
            try
            {
                result = _hsgdTtrinhRepository.ChuyenHoSo(pr_key_hsgd_ttrinh, email_login, oid_nhan, send_email);
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public string ChuyenKyHoSo(decimal pr_key_hsgd_ttrinh, string email_login, string oid_nhan, bool send_email)
        {
            string result = "";
            try
            {
                result = _hsgdTtrinhRepository.ChuyenKyHoSo(pr_key_hsgd_ttrinh, email_login, oid_nhan, send_email);
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public string TraLaiHoSo(decimal pr_key_hsgd_ttrinh, string email_login, string oid_nhan, string lido_tc, bool send_email)
        {
            string result = "";
            try
            {
                result = _hsgdTtrinhRepository.TraLaiHoSo(pr_key_hsgd_ttrinh, email_login, oid_nhan, lido_tc, send_email);
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public string HuyToTrinh(decimal pr_key_hsgd_ttrinh, string email_login)
        {
            string result = "";
            try
            {
                result = _hsgdTtrinhRepository.HuyToTrinh(pr_key_hsgd_ttrinh, email_login);
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public List<HsgdTtrinhNky> GetLichSuPheDuyet(decimal pr_key_hsgd_ttrinh)
        {
            List<HsgdTtrinhNky> obj_result = new List<HsgdTtrinhNky>();
            try
            {
                obj_result = _hsgdTtrinhRepository.GetLichSuPheDuyet(pr_key_hsgd_ttrinh);

            }
            catch (Exception ex)
            {
            }
            return obj_result;
        }
        public TtrinhCount CountTTrinhByTT(string email_login, int nam_dulieu)
        {
            TtrinhCount obj_result = new TtrinhCount();
            try
            {
                obj_result = _hsgdTtrinhRepository.CountTTrinhByTT(email_login, nam_dulieu);

            }
            catch (Exception ex)
            {
            }
            return obj_result;
        }

        public TtrinhLDCount CountTTrinhLDByTT(string email_login, int nam_dulieu)
        {
            TtrinhLDCount obj_result = new TtrinhLDCount();
            try
            {
                obj_result = _hsgdTtrinhRepository.CountTTrinhLDByTT(email_login, nam_dulieu);

            }
            catch (Exception ex)
            {
            }
            return obj_result;
        }
        public PagedList<HoSoTrinhKy> GetDataHsTrinhKy(string email_login, ToTrinhParameters totrinhParameters)
        {
            var list = _hsgdTtrinhRepository.GetDataHsTrinhKy(email_login, totrinhParameters);
            return list;
        }
        public PagedList<HoSoTrinhKy> GetDataHsTrinhKyKoHoaDon(string email_login, ToTrinhParameters totrinhParameters)
        {
            var list = _hsgdTtrinhRepository.GetDataHsTrinhKyKoHoaDon(email_login, totrinhParameters);
            return list;
        }
        public PagedList<HoSoTrinhKy> GetDataHsDaThanhToan(string email_login, ToTrinhParameters totrinhParameters)
        {
            var list = _hsgdTtrinhRepository.GetDataHsDaThanhToan(email_login, totrinhParameters);
            return list;
        }
        public PagedList<HoSoTrinhKy> GetDataHsTrinhKyLanhDao(string email_login, ToTrinhParameters totrinhParameters)
        {
            var list = _hsgdTtrinhRepository.GetDataHsTrinhKyLanhDao(email_login, totrinhParameters);
            return list;
        }
        public DmUser? GetUserLogin(string email_login)
        {
            DmUser? obj_result = new DmUser();
            try
            {
                obj_result = _hsgdTtrinhRepository.GetUserLogin(email_login);

            }
            catch (Exception ex)
            {
            }
            return obj_result;
        }
        public List<DmUser> GetListUserChuyenKy(string email_login)
        {
            List<DmUser> obj_result = new List<DmUser>();
            try
            {
                obj_result = _hsgdTtrinhRepository.GetListUserChuyenKy(email_login);

            }
            catch (Exception ex)
            {
            }
            return obj_result;
        }
        public DownloadFileResult CreateBiaHS(BiaHS biahs)
        {
            try
            {
                var result = _hsgdTtrinhRepository.CreateBiaHS(biahs);

                var downloadSettings = _configuration.GetSection("DownloadSettings").Get<DownloadSettings>();
                var word2PdfSettings = _configuration.GetSection("Word2PdfSettings").Get<Word2PdfSettings>();

                var optionsDownloadSettings = Microsoft.Extensions.Options.Options.Create(downloadSettings);
                var optionsWord2PdfSettings = Microsoft.Extensions.Options.Options.Create(word2PdfSettings);


                var contentHelper = new ContentHelper(optionsDownloadSettings, optionsWord2PdfSettings, _logger);

                DownloadFileResult result1 = null;

                if (result != null && result.ThirdQueryResults != null)
                {
                    result1 = contentHelper.ConvertFileWordToPdf_BiaHs(result.ThirdQueryResults);
                }

                return result1;
            }
            catch (Exception ex)
            {
            }
            return null;
        }
        public DownloadFileResult PrintToTrinhTPC(decimal pr_key_hsgd_ctu, string email, int loai_tt)
        {
            try
            {
                DownloadFileResult result1 = null;
                var hsgd_ctu = _hsgdTtrinhRepository.GetHsgdCtuByKey(pr_key_hsgd_ctu);
                if (string.IsNullOrEmpty(hsgd_ctu.PathTotrinhTpc))
                {
                    var result = _hsgdTtrinhRepository.GetPrintToTrinhTPC(pr_key_hsgd_ctu, email, loai_tt);

                    var downloadSettings = _configuration.GetSection("DownloadSettings").Get<DownloadSettings>();
                    var word2PdfSettings = _configuration.GetSection("Word2PdfSettings").Get<Word2PdfSettings>();

                    var optionsDownloadSettings = Microsoft.Extensions.Options.Options.Create(downloadSettings);
                    var optionsWord2PdfSettings = Microsoft.Extensions.Options.Options.Create(word2PdfSettings);

                    var contentHelper = new ContentHelper(optionsDownloadSettings, optionsWord2PdfSettings, _logger);

                    if (result != null && result.ThirdQueryResults != null)
                    {
                        result1 = contentHelper.ConvertFileWordToPdf_ToTrinh_TPC(result.ThirdQueryResults, loai_tt);
                    }
                }
                else
                {
                    string url_download = _configuration.GetValue<string>("DownloadSettings:DownloadServer_MDF1") ?? "";
                    result1 = UtilityHelper.DownloadFile_ToAPI(hsgd_ctu.PathTotrinhTpc, url_download);
                }

                //_logger.Information("PrintToTrinh success");
                return result1;
            }
            catch (Exception ex)
            {
            }
            return null;
        }
        public string UploadToTrinhTPC(UploadToTrinhTPC entity, string email_login)
        {
            var result = "";
            try
            {
                result = _hsgdTtrinhRepository.UploadToTrinhTPC(entity, email_login);
            }
            catch (Exception ex)
            {
                _logger.Error("UploadToTrinhTPC:", ex);
                _logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
            }
            return result;
        }
        public string PheDuyetHsTpc(decimal pr_key_hsgd_ctu, string email_login)
        {
            string result = "";
            try
            {
                var hsgd_ctu = _hsgdTtrinhRepository.GetHsgdCtuByKey(pr_key_hsgd_ctu);
                if (string.IsNullOrEmpty(hsgd_ctu.PathTotrinhTpc))
                {

                    //var result_print_tt = PrintToTrinhTPC(pr_key_hsgd_ctu, email_login, loai_tt);
                    //string folderUpload = _configuration["UploadSettings:FolderUpload_MDF1"] ?? "";
                    //string url_upload = _configuration["DownloadSettings:UlpoadServer_MDF1"] ?? "";
                    //var utilityHelper = new UtilityHelper(_logger);
                    //var file_path = utilityHelper.UploadFile_ToAPI(result_print_tt.Data, ".pdf", folderUpload, url_upload, false);
                    //if (!string.IsNullOrEmpty(file_path))
                    //{
                    //    hsgd_ctu.PathTotrinhTpc = file_path;
                    //    _hsgdCtuRepository.Update(hsgd_ctu);
                    //    _hsgdCtuRepository.Save();
                    //}
                    result = "HS trên phân cấp của TVP chưa được tạo tờ trình TPC. Không thực hiện chức năng này.";
                    return result;
                }
                result = _hsgdTtrinhRepository.PheDuyetHsTpc(pr_key_hsgd_ctu, email_login);


            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public bool CheckHsgdTPC(decimal pr_key_hsgd_ctu, string ma_ttrang, string email_login)
        {
            var result = false;
            try
            {
                var check_tpc = _hsgdTtrinhRepository.CheckHsgdTPC(pr_key_hsgd_ctu, ma_ttrang, email_login);
                if (check_tpc)
                {
                    result = true;
                }

            }
            catch (Exception ex)
            {
                _logger.Error("CheckHsgdTPC:", ex);
            }
            return result;
        }
        public string KyHoSoTPC(decimal pr_key_hsgd_ctu, string email_login)
        {
            string result = "";
            try
            {
                result = _hsgdTtrinhRepository.KyHoSoTPC(pr_key_hsgd_ctu, email_login);
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public DownloadFileResult DownloadToTrinhTPC(decimal pr_key_hsgd_ctu, string email, int loai_tt)
        {
            try
            {
                DownloadFileResult result1 = null;
                //var hsgd_ctu = _hsgdTtrinhRepository.GetHsgdCtuByKey(pr_key_hsgd_ctu);
                //if (string.IsNullOrEmpty(hsgd_ctu.PathTotrinhTpc))
                //{
                var result = _hsgdTtrinhRepository.GetPrintToTrinhTPC(pr_key_hsgd_ctu, email, loai_tt);

                var downloadSettings = _configuration.GetSection("DownloadSettings").Get<DownloadSettings>();
                var word2PdfSettings = _configuration.GetSection("Word2PdfSettings").Get<Word2PdfSettings>();

                var optionsDownloadSettings = Microsoft.Extensions.Options.Options.Create(downloadSettings);
                var optionsWord2PdfSettings = Microsoft.Extensions.Options.Options.Create(word2PdfSettings);

                var contentHelper = new ContentHelper(optionsDownloadSettings, optionsWord2PdfSettings, _logger);

                if (result != null && result.ThirdQueryResults != null)
                {
                    result1 = contentHelper.ConvertFileWord_ToTrinh_TPC(result.ThirdQueryResults, loai_tt);
                }
                //}
                //else
                //{
                //    string url_download = _configuration.GetValue<string>("DownloadSettings:DownloadServer_MDF1") ?? "";
                //    result1 = UtilityHelper.DownloadFile_ToAPI(hsgd_ctu.PathTotrinhTpc, url_download);
                //}

                //_logger.Information("PrintToTrinh success");
                return result1;
            }
            catch (Exception ex)
            {
            }
            return null;
        }
        public string TaoToTrinhTpc(decimal pr_key_hsgd_ctu, string email_login, int loai_tt)
        {
            string result = "";
            try
            {
                var hsgd_ctu = _hsgdTtrinhRepository.GetHsgdCtuByKey(pr_key_hsgd_ctu);
                if (string.IsNullOrEmpty(hsgd_ctu.PathTotrinhTpc))
                {

                    var result_print_tt = PrintToTrinhTPC(pr_key_hsgd_ctu, email_login, loai_tt);
                    string folderUpload = _configuration["UploadSettings:FolderUpload_MDF1"] ?? "";
                    string url_upload = _configuration["DownloadSettings:UlpoadServer_MDF1"] ?? "";
                    var utilityHelper = new UtilityHelper(_logger);
                    var file_path = utilityHelper.UploadFile_ToAPI(result_print_tt.Data, ".pdf", folderUpload, url_upload, false);
                    if (!string.IsNullOrEmpty(file_path))
                    {
                        hsgd_ctu.PathTotrinhTpc = file_path;
                        hsgd_ctu.LoaiTotrinhTpc = loai_tt;
                        _hsgdCtuRepository.Update(hsgd_ctu);
                        _hsgdCtuRepository.Save();
                    }
                    result = "Tạo tờ trình TPC thành công.";
                    return result;
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
    }
}