using AutoMapper;
using Microsoft.Extensions.Configuration;
using PVI.Repository.Interfaces;
using PVI.Repository.Repositories;
using PVI.DAO.Entities.Models;
using PVI.Helper;
using PVI.Service.Request;
using Microsoft.Office.Interop.Word;
using System.Net.WebSockets;
using Microsoft.EntityFrameworkCore;
using Azure.Core;
using System.Text.RegularExpressions;
using System.Diagnostics.Eventing.Reader;
using System.Data;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ICSharpCode.SharpZipLib.Core;
using System.Net.Mail;
using System.Net.Mime;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using static iTextSharp.text.pdf.events.IndexEvents;
using static iTextSharp.text.pdf.AcroFields;

namespace PVI.Service
{
    public class HsgdDxService
    {
        private readonly IHsgdDxRepository _hsgdDxRepository;
        private readonly IHsgdDxCtRepository _hsgdDxCtRepository;
        private readonly IHsbtCtRepository _hsbtCtRepository;
        private readonly IHsbtUocRepository _hsbtUocRepository;
        private readonly IHsbtGdRepository _hsbtGdRepository;
        private readonly IHsbtUocGdRepository _hsbtUocGdRepository;
        private readonly IHsbtThtsRepository _hsbtThtsRepository;
        private readonly IDmHmucSuaChuaRepository _dmHmucSuaChuaRepository;
        private readonly IDmUserRepository _dmUserRepository;
        private readonly IFileAttachBtRepository _fileAttachBtRepository;
        private readonly Serilog.ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public HsgdDxService(IHsgdDxRepository hsgdDxRepository, IHsgdDxCtRepository hsgdDxCtRepository, IHsbtCtRepository hsbtCtRepository, IHsbtUocRepository hsbtUocRepository, IHsbtGdRepository hsbtGdRepository, IHsbtUocGdRepository hsbtUocGdRepository, IHsbtThtsRepository hsbtThtsRepository, IDmHmucSuaChuaRepository dmHmucSuaChuaRepository, IDmUserRepository dmUserRepository, IFileAttachBtRepository fileAttachBtRepository, IMapper mapper, Serilog.ILogger logger, IConfiguration conf)
        {
            _hsgdDxRepository = hsgdDxRepository;
            _hsgdDxCtRepository = hsgdDxCtRepository;
            _hsbtCtRepository = hsbtCtRepository;
            _hsbtUocRepository = hsbtUocRepository;
            _hsbtGdRepository = hsbtGdRepository;
            _hsbtUocGdRepository = hsbtUocGdRepository;
            _hsbtThtsRepository = hsbtThtsRepository;
            _dmHmucSuaChuaRepository = dmHmucSuaChuaRepository;
            _dmUserRepository = dmUserRepository;
            _fileAttachBtRepository = fileAttachBtRepository;
            _mapper = mapper;
            _logger = logger;
            _configuration = conf;
        }
        public List<HsbtCtView> GetListPhaiTraBT(decimal pr_key_hsgd)
        {
            List<HsbtCtView> obj_result = new List<HsbtCtView>();
            try
            {
                obj_result = _hsgdDxRepository.GetListPhaiTraBT(pr_key_hsgd);

            }
            catch (Exception ex)
            {
            }
            return obj_result;
        }
        public Task<List<HsbtUocBT>> GetListChiTietUocBT(decimal hsbt_ct_pr_key)
        {
            var list_hsbt_uoc = _hsgdDxRepository.GetListChiTietUocBT(hsbt_ct_pr_key);
            return list_hsbt_uoc;
        }
        public List<HsbtGDView> GetListPhaiTraGD(decimal pr_key_hsgd)
        {
            List<HsbtGDView> obj_result = new List<HsbtGDView>();
            try
            {
                obj_result = _hsgdDxRepository.GetListPhaiTraGD(pr_key_hsgd);

            }
            catch (Exception ex)
            {
            }
            return obj_result;
        }
        public Task<List<HsbtUocGD>> GetListChiTietUocGD(decimal hsbt_ct_pr_key)
        {
            var list_ugd = _hsgdDxRepository.GetListChiTietUocGD(hsbt_ct_pr_key);
            return list_ugd;
        }
        public List<HsbtThtsView> GetListThuDoi(decimal pr_key_hsgd)
        {
            List<HsbtThtsView> obj_result = new List<HsbtThtsView>();
            try
            {
                obj_result = _hsgdDxRepository.GetListThuDoi(pr_key_hsgd);

            }
            catch (Exception ex)
            {
            }
            return obj_result;
        }
        public List<HsgdDxView> GetListPASC(decimal pr_key_hsgd_dx_ct, decimal pr_key_hsgd_ctu)
        {
            List<HsgdDxView> obj_result = new List<HsgdDxView>();
            try
            {
                obj_result = _hsgdDxRepository.GetListPASC(pr_key_hsgd_dx_ct, pr_key_hsgd_ctu);

            }
            catch (Exception ex)
            {
            }
            return obj_result;
        }
        public List<HsgdDxSum> ReloadSum(decimal pr_key_hsgd_dx_ct)
        {
            List<HsgdDxSum> obj_result = new List<HsgdDxSum>();
            try
            {
                obj_result = _hsgdDxRepository.ReloadSum(pr_key_hsgd_dx_ct);

            }
            catch (Exception ex)
            {
            }
            return obj_result;
        }
        public async Task<string> CreateHsbtCt(HsbtCtRequest entity)
        {
            try
            {
                var hsbtCt = _mapper.Map<HsbtCtDetailRequest, HsbtCt>(entity.hsbtCt);
                var hsgdDxCt = _mapper.Map<HsgdDxCtRequest, HsgdDxCt>(entity.hsgdDxCt);
                //xử lý dữ liệu
                _logger.Information("CreateHsbtCt start ctu_tyle");
                var ctu_tyle = _hsgdDxRepository.GetTTDonBH(entity.PrKeyHsgdCtu, hsbtCt.MaSp);
                _logger.Information("CreateHsbtCt end ctu_tyle");
                if (ctu_tyle != null)
                {
                    hsbtCt.TyleReten = ctu_tyle.TyleReten;
                    hsbtCt.MtnRetenVnd = Math.Round(Convert.ToDecimal(hsbtCt.SoTienp) * Convert.ToDecimal(ctu_tyle.TyleReten) / 100, 0);
                    hsbtCt.MtnRetenNte = Math.Round(Convert.ToDecimal(hsbtCt.NguyenTep) * Convert.ToDecimal(ctu_tyle.TyleReten) / 100, 0);
                    //hsbtCt.SoTienp = Math.Round(Convert.ToDecimal(hsbtCt.SoTienp) * Convert.ToDecimal(ctu_tyle.TyleDong) / 100, 0);
                    //hsbtCt.NguyenTep = Math.Round(Convert.ToDecimal(hsbtCt.NguyenTep) * Convert.ToDecimal(ctu_tyle.TyleDong) / 100, 0);
                    //hsbtCt.MtnGtbh = ctu_tyle.MtnGtbhTsan;
                    //hsbtCt.MaDkhoan = ctu_tyle.MaDkbh ?? "";
                    //hsbtCt.MaTteGoc = ctu_tyle.MaTtep ?? "";
                }
                var hsbtUoc = new HsbtUoc
                {
                    NgayPs = DateTime.Today,
                    NguyenTebt = hsbtCt.NguyenTep,
                    SoTienbt = hsbtCt.SoTienp,
                    NguyenTebtReten = hsbtCt.MtnRetenNte,
                    SoTienbtReten = hsbtCt.MtnRetenVnd,
                    TyleReten = hsbtCt.TyleReten,
                    NguyenTebtPvi = hsbtCt.NguyenTep,
                    SoTienbtPvi = hsbtCt.SoTienp,

                };
                // file_attach_bt
                List<FileAttachBt> fileAttach = new List<FileAttachBt>();
                for (int i = 0; i < entity.fileAttachBts.Count; i++)
                {
                    if (!string.IsNullOrEmpty(entity.fileAttachBts[i].FileData))
                    {
                        string folderUpload = _configuration["UploadSettings:FolderUpload_MDF1"] ?? "";
                        string url_upload = _configuration["DownloadSettings:UlpoadServer_MDF1"] ?? "";
                        var utilityHelper = new UtilityHelper(_logger);
                        var file_path = utilityHelper.UploadFile_ToAPI(entity.fileAttachBts[i].FileData, entity.fileAttachBts[i].FileExtension, folderUpload, url_upload, false);
                        if (!string.IsNullOrEmpty(file_path))
                        {
                            FileAttachBt file = new FileAttachBt();
                            file.Directory = file_path;
                            file.FileName = entity.fileAttachBts[i].FileName;
                            file.PrKey = 0;
                            file.FrKey = 0;
                            file.MaCtu = "BTPT";
                            fileAttach.Add(file);
                        }
                    }
                }
                _logger.Information("CreateHsbtCt: Run CreateHsbtCt");
                var result = await _hsgdDxRepository.CreateHsbtCt(hsbtCt, hsgdDxCt, hsbtUoc, entity.PrKeyHsgdCtu, fileAttach);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("CreateHsbtCt:", ex);
                _logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
            }
            return null!;
        }
        public async Task<string> UpdateHsbtCt(HsbtCtRequest entity)
        {
            var result = "";
            try
            {
                var hsbtCt_old = _hsbtCtRepository.GetEntityByConditionNoAsyncPias(x => x.PrKey == entity.hsbtCt.PrKey);

                _logger.Information("UpdateHsbtCt có hsbtCt_old =" + JsonConvert.SerializeObject(hsbtCt_old));
                if (hsbtCt_old == null)
                {
                    return "entity is not found";
                }
                var hsbtCt_new = _mapper.Map(entity.hsbtCt, hsbtCt_old);
                _logger.Information("UpdateHsbtCt có hsbtCt_new =" + JsonConvert.SerializeObject(hsbtCt_new));
                var hsgdDxCtt_old = _hsgdDxCtRepository.GetEntityByConditionNoAsync(x => x.PrKey == entity.hsgdDxCt.PrKey);
                if (hsgdDxCtt_old == null)
                {
                    return "entity is not found";
                }
                var hsgdDxCt_new = _mapper.Map(entity.hsgdDxCt, hsgdDxCtt_old);

                //xử lý dữ liệu
                var ctu_tyle = _hsgdDxRepository.GetTTDonBH(entity.PrKeyHsgdCtu, hsbtCt_new.MaSp);
                if (ctu_tyle != null)
                {
                    hsbtCt_new.TyleReten = ctu_tyle.TyleReten;
                    hsbtCt_new.MtnRetenVnd = Math.Round(Convert.ToDecimal(hsbtCt_new.SoTienp) * Convert.ToDecimal(ctu_tyle.TyleReten) / 100, 0);
                    hsbtCt_new.MtnRetenNte = Math.Round(Convert.ToDecimal(hsbtCt_new.NguyenTep) * Convert.ToDecimal(ctu_tyle.TyleReten) / 100, 0);
                    //hsbtCt_new.SoTienp = Math.Round(Convert.ToDecimal(hsbtCt_new.SoTienp) * Convert.ToDecimal(ctu_tyle.TyleDong) / 100, 0);
                    //hsbtCt_new.NguyenTep = Math.Round(Convert.ToDecimal(hsbtCt_new.NguyenTep) * Convert.ToDecimal(ctu_tyle.TyleDong) / 100, 0);
                    //hsbtCt_new.MtnGtbh = ctu_tyle.MtnGtbhTsan;
                    //hsbtCt_new.MaDkhoan = ctu_tyle.MaDkbh ?? "";
                    //hsbtCt_new.MaTteGoc = ctu_tyle.MaTtep ?? "";
                }

                var hsbt_uoc_request = new HsbtUocRequest
                {
                    HsbtCtPrkey = hsbtCt_new.PrKey,
                    HsbtCtuPrKey = hsbtCt_new.FrKey,
                    MtnRetenNte = hsbtCt_new.MtnRetenNte,
                    MtnRetenVnd = hsbtCt_new.MtnRetenVnd,
                    NguyenTep = hsbtCt_new.NguyenTep,
                    SoTienp = hsbtCt_new.SoTienp,
                    TyleReten = hsbtCt_new.TyleReten,
                };
                var hsbt_uoc_new = GetHsbtUocData(hsbt_uoc_request);
                _logger.Information("UpdateHsbtCt có hsbt_uoc =" + JsonConvert.SerializeObject(hsbt_uoc_new));
                //file_attach_bt
                var file_attach_bt_old = _fileAttachBtRepository.GetListEntityByConditionNoAsyncPias(x => x.FrKey == entity.hsbtCt.PrKey);
                var file_attach_bt_delete = file_attach_bt_old.Where(x => !entity.fileAttachBts.Select(x => x.PrKey).ToArray().Contains(x.PrKey)).ToList();
                List<FileAttachBt> fileAttach = new List<FileAttachBt>();
                var list_file_add = entity.fileAttachBts.Where(x => x.PrKey == 0).ToList();
                for (int i = 0; i < list_file_add.Count; i++)
                {
                    if (!string.IsNullOrEmpty(list_file_add[i].FileData))
                    {
                        string folderUpload = _configuration["UploadSettings:FolderUpload_MDF1"] ?? "";
                        string url_upload = _configuration["DownloadSettings:UlpoadServer_MDF1"] ?? "";
                        var utilityHelper = new UtilityHelper(_logger);
                        var file_path = utilityHelper.UploadFile_ToAPI(list_file_add[i].FileData, list_file_add[i].FileExtension, folderUpload, url_upload, false);
                        if (!string.IsNullOrEmpty(file_path))
                        {
                            FileAttachBt file = new FileAttachBt();
                            file.Directory = file_path;
                            file.FileName = list_file_add[i].FileName;
                            file.PrKey = 0;
                            file.FrKey = list_file_add[i].FrKey;
                            file.MaCtu = "BTPT";
                            fileAttach.Add(file);
                        }
                    }
                }
                result = await _hsgdDxRepository.UpdateHsbtCt(hsbtCt_new, hsgdDxCt_new, hsbt_uoc_new, fileAttach, file_attach_bt_delete);
            }
            catch (Exception ex)
            {
                _logger.Error("UpdateHsbtCt PrKeyHsgdCtu"+ entity.PrKeyHsgdCtu.ToString()+" ex:", ex);
                //_logger.Error("UpdateHsbtCt Error record: " + JsonConvert.SerializeObject(entity));
            }
            return result;
        }
        public HsbtUoc? GetHsbtUocData(HsbtUocRequest request)
        {
            var hsbt_uoc = _hsbtUocRepository.GetListEntityByConditionNoAsyncPias(a => a.FrKey == request.HsbtCtPrkey);
            hsbt_uoc = hsbt_uoc.OrderByDescending(a => a.NgayPs).ToList();
            HsbtUoc? hsbt_uoc_new = new HsbtUoc();
            if (hsbt_uoc.Count() > 0)
            {
                if (hsbt_uoc[0].NgayPs < DateTime.Today)
                {
                    decimal nguyenTeBt = 0;
                    decimal soTienBt = 0;
                    decimal soTienBtReten = 0;
                    decimal soTienBtPvi = 0;
                    decimal nguyenTeBtPvi = 0;
                    decimal nguyenTeBtReten = 0;
                    for (int j = 0; j < hsbt_uoc.Count; j++)
                    {
                        nguyenTeBt += hsbt_uoc[j].NguyenTebt;
                        soTienBt += hsbt_uoc[j].SoTienbt;

                        soTienBtReten += hsbt_uoc[j].SoTienbtReten;

                        nguyenTeBtReten += hsbt_uoc[j].NguyenTebtReten;

                    }
                    hsbt_uoc_new.FrKey = request.HsbtCtPrkey;
                    hsbt_uoc_new.NgayPs = DateTime.Today;
                    hsbt_uoc_new.NguyenTebt = request.NguyenTep - nguyenTeBt;
                    hsbt_uoc_new.SoTienbt = request.SoTienp - soTienBt;
                    hsbt_uoc_new.NguyenTebtReten = request.MtnRetenNte - nguyenTeBtReten;
                    hsbt_uoc_new.SoTienbtReten = request.MtnRetenVnd - soTienBtReten;
                    hsbt_uoc_new.NguyenTebtPvi = request.NguyenTep - nguyenTeBt;
                    hsbt_uoc_new.SoTienbtPvi = request.SoTienp - soTienBt;
                    hsbt_uoc_new.TyleReten = request.TyleReten;
                    var nguyenTebtAbs = Math.Abs((decimal)hsbt_uoc_new.NguyenTebt);
                    var soTienbtAbs = Math.Abs((decimal)hsbt_uoc_new.SoTienbt);
                    var nguyenTebtRetenAbs = Math.Abs((decimal)hsbt_uoc_new.NguyenTebtReten);
                    var soTienbtRetenAbs = Math.Abs((decimal)hsbt_uoc_new.SoTienbtReten);

                    var result3 = nguyenTebtAbs + soTienbtAbs + nguyenTebtRetenAbs + soTienbtRetenAbs;
                    if (result3 == 0)
                    {
                        hsbt_uoc_new = null;
                    }
                }
                else
                {
                    decimal nguyenTeBt = 0;
                    decimal soTienBt = 0;
                    decimal soTienBtReten = 0;
                    decimal soTienBtPvi = 0;
                    decimal nguyenTeBtPvi = 0;
                    decimal nguyenTeBtReten = 0;
                    for (int j = 1; j < hsbt_uoc.Count; j++)
                    {
                        nguyenTeBt += hsbt_uoc[j].NguyenTebt;
                        soTienBt += hsbt_uoc[j].SoTienbt;
                        soTienBtPvi += hsbt_uoc[j].SoTienbtPvi;
                        soTienBtReten += hsbt_uoc[j].SoTienbtReten;
                        nguyenTeBtPvi += hsbt_uoc[j].NguyenTebtPvi;
                        nguyenTeBtReten += hsbt_uoc[j].NguyenTebtReten;

                    }
                    hsbt_uoc_new = hsbt_uoc[0];
                    hsbt_uoc_new.NgayPs = DateTime.Today;
                    hsbt_uoc_new.NguyenTebt = request.NguyenTep - nguyenTeBt;
                    hsbt_uoc_new.SoTienbt = request.SoTienp - soTienBt;
                    hsbt_uoc_new.NguyenTebtReten = request.MtnRetenNte - nguyenTeBtReten;
                    hsbt_uoc_new.SoTienbtReten = request.MtnRetenVnd - soTienBtReten;
                    hsbt_uoc_new.TyleReten = request.TyleReten;
                    hsbt_uoc_new.NguyenTebtPvi = request.NguyenTep - nguyenTeBt;
                    hsbt_uoc_new.SoTienbtPvi = request.SoTienp - soTienBt;
                }
            }

            return hsbt_uoc_new;
        }
        public async Task<string> DeleteHsbtCt(decimal pr_key)
        {
            string result = "";
            try
            {
                result = await _hsgdDxRepository.DeleteHsbtCt(pr_key);

            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public async Task<string> CreateHsbtGd(HsbtGdRequest entity)
        {
            try
            {
                var hsbtGd = _mapper.Map<HsbtGdRequest, HsbtGd>(entity);
                var ctu_tyle = _hsgdDxRepository.GetTTDonBH(entity.PrKeyHsgdCtu, hsbtGd.MaSp);
                if (ctu_tyle != null)
                {
                    hsbtGd.TyleReten = ctu_tyle.TyleReten;
                    hsbtGd.MtnRetenVnd = Math.Round(Convert.ToDecimal(hsbtGd.SoTiengd) * Convert.ToDecimal(ctu_tyle.TyleReten) / 100, 0);
                    hsbtGd.MtnRetenNte = Math.Round(Convert.ToDecimal(hsbtGd.NguyenTegd) * Convert.ToDecimal(ctu_tyle.TyleReten) / 100, 0);
                    //hsbtGd.NguyenTegd = Math.Round(Convert.ToDecimal(hsbtGd.NguyenTegd) * Convert.ToDecimal(ctu_tyle.TyleDong) / 100, 0);
                    //hsbtGd.SoTiengd = Math.Round(Convert.ToDecimal(hsbtGd.SoTiengd) * Convert.ToDecimal(ctu_tyle.TyleDong) / 100, 0);
                }
                hsbtGd.SoTiengdPvi = hsbtGd.SoTiengd;
                hsbtGd.NguyenTegdPvi = hsbtGd.NguyenTegd;

                var hsbtUocGd = new HsbtUocGd
                {
                    FrKey = hsbtGd.PrKey,
                    NgayPs = DateTime.Today,
                    NguyenTegd = hsbtGd.NguyenTegd,
                    SoTiengd = hsbtGd.SoTiengd,
                    NguyenTegdReten = hsbtGd.MtnRetenNte,
                    SoTiengdReten = hsbtGd.MtnRetenVnd,
                    NguyenTegdPvi = hsbtGd.NguyenTegd,
                    SoTiengdPvi = hsbtGd.SoTiengd,
                    GhiChu = "",
                    TyleReten = hsbtGd.TyleReten,
                };
                // file_attach_bt
                List<FileAttachBt> fileAttach = new List<FileAttachBt>();
                for (int i = 0; i < entity.fileAttachBts.Count; i++)
                {
                    if (!string.IsNullOrEmpty(entity.fileAttachBts[i].FileData))
                    {
                        string folderUpload = _configuration["UploadSettings:FolderUpload_MDF1"] ?? "";
                        string url_upload = _configuration["DownloadSettings:UlpoadServer_MDF1"] ?? "";
                        var utilityHelper = new UtilityHelper(_logger);
                        var file_path = utilityHelper.UploadFile_ToAPI(entity.fileAttachBts[i].FileData, entity.fileAttachBts[i].FileExtension, folderUpload, url_upload, false);
                        if (!string.IsNullOrEmpty(file_path))
                        {
                            FileAttachBt file = new FileAttachBt();
                            file.Directory = file_path;
                            file.FileName = entity.fileAttachBts[i].FileName;
                            file.PrKey = 0;
                            file.FrKey = 0;
                            file.MaCtu = "GDPT";
                            fileAttach.Add(file);
                        }
                    }
                }
                var result = await _hsgdDxRepository.CreateHsbtGd(hsbtGd, hsbtUocGd, fileAttach);
                return result;
            }
            catch (Exception ex)
            {
                //_logger.Error("CreateHsgdTtrinh:", ex);
                //_logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
            }
            return null!;
        }
        public async Task<string> UpdateHsbtGd(HsbtGdRequest entity)
        {
            var result = "";
            try
            {
                var hsbtGd_old = _hsbtGdRepository.GetEntityByConditionNoAsyncPias(x => x.PrKey == entity.PrKey);
                if (hsbtGd_old == null)
                {
                    return "entity is not found";
                }
                var hsbtGd_new = _mapper.Map(entity, hsbtGd_old);
                //xử lý dữ liệu
                var ctu_tyle = _hsgdDxRepository.GetTTDonBH(entity.PrKeyHsgdCtu, hsbtGd_new.MaSp);
                if (ctu_tyle != null)
                {
                    hsbtGd_new.TyleReten = ctu_tyle.TyleReten;
                    hsbtGd_new.MtnRetenVnd = Math.Round(Convert.ToDecimal(hsbtGd_new.SoTiengd) * Convert.ToDecimal(ctu_tyle.TyleReten) / 100, 0);
                    hsbtGd_new.MtnRetenNte = Math.Round(Convert.ToDecimal(hsbtGd_new.NguyenTegd) * Convert.ToDecimal(ctu_tyle.TyleReten) / 100, 0);
                    //hsbtGd_new.NguyenTegd = Math.Round(Convert.ToDecimal(hsbtGd_new.NguyenTegd) * Convert.ToDecimal(ctu_tyle.TyleDong) / 100, 0);
                    //hsbtGd_new.SoTiengd = Math.Round(Convert.ToDecimal(hsbtGd_new.SoTiengd) * Convert.ToDecimal(ctu_tyle.TyleDong) / 100, 0);
                }
                var hsbt_uoc_gd_request = new HsbtUocGdRequest
                {
                    HsbtGdPrkey = hsbtGd_new.PrKey,
                    HsbtCtuPrKey = (decimal)hsbtGd_new.FrKey,
                    MtnRetenNte = (decimal)hsbtGd_new.MtnRetenNte,
                    MtnRetenVnd = (decimal)hsbtGd_new.MtnRetenVnd,
                    NguyenTegd = (decimal)hsbtGd_new.NguyenTegd,
                    SoTiengd = (decimal)hsbtGd_new.SoTiengd,
                    TyleReten = (decimal)hsbtGd_new.TyleReten,
                };
                var hsbt_uoc_gd = GetHsbtUocGdData(hsbt_uoc_gd_request);
                //file_attach_bt
                var file_attach_bt_old = _fileAttachBtRepository.GetListEntityByConditionNoAsyncPias(x => x.FrKey == entity.PrKey);
                var file_attach_bt_delete = file_attach_bt_old.Where(x => !entity.fileAttachBts.Select(x => x.PrKey).ToArray().Contains(x.PrKey)).ToList();
                List<FileAttachBt> fileAttach = new List<FileAttachBt>();
                var list_file_add = entity.fileAttachBts.Where(x => x.PrKey == 0).ToList();
                for (int i = 0; i < list_file_add.Count; i++)
                {
                    if (!string.IsNullOrEmpty(list_file_add[i].FileData))
                    {
                        string folderUpload = _configuration["UploadSettings:FolderUpload_MDF1"] ?? "";
                        string url_upload = _configuration["DownloadSettings:UlpoadServer_MDF1"] ?? "";
                        var utilityHelper = new UtilityHelper(_logger);
                        var file_path = utilityHelper.UploadFile_ToAPI(list_file_add[i].FileData, list_file_add[i].FileExtension, folderUpload, url_upload, false);
                        if (!string.IsNullOrEmpty(file_path))
                        {
                            FileAttachBt file = new FileAttachBt();
                            file.Directory = file_path;
                            file.FileName = list_file_add[i].FileName;
                            file.PrKey = 0;
                            file.FrKey = list_file_add[i].FrKey;
                            file.MaCtu = "GDPT";
                            fileAttach.Add(file);
                        }
                    }
                }
                result = await _hsgdDxRepository.UpdateHsbtGd(hsbtGd_new, hsbt_uoc_gd, fileAttach, file_attach_bt_delete);
            }
            catch (Exception ex)
            {
                //_logger.Error("CreateHsgdTtrinh:", ex);
                //_logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
            }
            return result;
        }
        public HsbtUocGd? GetHsbtUocGdData(HsbtUocGdRequest request)
        {
            var hsbt_uoc_gd = _hsbtUocGdRepository.GetListEntityByConditionNoAsyncPias(a => a.FrKey == request.HsbtGdPrkey);
            hsbt_uoc_gd = hsbt_uoc_gd.OrderByDescending(a => a.NgayPs).ToList();
            HsbtUocGd? hsbt_uoc_gd_new = new HsbtUocGd();
            if (hsbt_uoc_gd.Count() > 0)
            {
                if (hsbt_uoc_gd[0].NgayPs < DateTime.Today)
                {
                    decimal nguyenTeGd = 0;
                    decimal soTiengd = 0;
                    decimal soTiengdReten = 0;
                    decimal soTiengdPvi = 0;
                    decimal nguyenTegdPvi = 0;
                    decimal nguyenTegdReten = 0;
                    for (int j = 0; j < hsbt_uoc_gd.Count; j++)
                    {
                        nguyenTeGd += hsbt_uoc_gd[j].NguyenTegd;
                        soTiengd += hsbt_uoc_gd[j].SoTiengd;

                        soTiengdReten += hsbt_uoc_gd[j].SoTiengdReten;

                        nguyenTegdReten += hsbt_uoc_gd[j].SoTiengdReten;

                    }
                    hsbt_uoc_gd_new.PrKey = 0;
                    hsbt_uoc_gd_new.FrKey = request.HsbtGdPrkey;
                    hsbt_uoc_gd_new.NgayPs = DateTime.Today;
                    hsbt_uoc_gd_new.NguyenTegd = request.NguyenTegd - nguyenTeGd;
                    hsbt_uoc_gd_new.SoTiengd = request.SoTiengd - soTiengd;
                    hsbt_uoc_gd_new.NguyenTegdReten = request.MtnRetenNte - nguyenTegdReten;
                    hsbt_uoc_gd_new.SoTiengdReten = request.MtnRetenVnd - soTiengdReten;
                    hsbt_uoc_gd_new.NguyenTegdPvi = request.NguyenTegd - nguyenTeGd;
                    hsbt_uoc_gd_new.SoTiengdPvi = request.SoTiengd - soTiengd;
                    hsbt_uoc_gd_new.TyleReten = request.TyleReten;
                    hsbt_uoc_gd_new.GhiChu = "";
                    var nguyenTeGdAbs = Math.Abs((decimal)hsbt_uoc_gd_new.NguyenTegd);
                    var soTiengdAbs = Math.Abs((decimal)hsbt_uoc_gd_new.SoTiengd);
                    var nguyenTegdRetenAbs = Math.Abs((decimal)hsbt_uoc_gd_new.NguyenTegdReten);
                    var soTiengdRetenAbs = Math.Abs((decimal)hsbt_uoc_gd_new.SoTiengdReten);

                    var result3 = nguyenTeGdAbs + soTiengdAbs + nguyenTegdRetenAbs + soTiengdRetenAbs;
                    if (result3 == 0)
                    {
                        hsbt_uoc_gd_new = null;
                    }
                }
                else
                {
                    decimal nguyenTeGd = 0;
                    decimal soTienGd = 0;
                    decimal soTienGdReten = 0;
                    decimal soTienGdPvi = 0;
                    decimal nguyenTeGdPvi = 0;
                    decimal nguyenTeGdReten = 0;
                    for (int j = 1; j < hsbt_uoc_gd.Count; j++)
                    {
                        nguyenTeGd += hsbt_uoc_gd[j].NguyenTegd;
                        soTienGd += hsbt_uoc_gd[j].SoTiengd;
                        soTienGdPvi += hsbt_uoc_gd[j].SoTiengdPvi;
                        soTienGdReten += hsbt_uoc_gd[j].SoTiengdReten;
                        nguyenTeGdPvi += hsbt_uoc_gd[j].NguyenTegdPvi;
                        nguyenTeGdReten += hsbt_uoc_gd[j].NguyenTegdReten;

                    }
                    hsbt_uoc_gd_new = hsbt_uoc_gd[0];
                    hsbt_uoc_gd_new.NgayPs = DateTime.Today;
                    hsbt_uoc_gd_new.NguyenTegd = request.NguyenTegd - nguyenTeGd;
                    hsbt_uoc_gd_new.SoTiengd = request.SoTiengd - soTienGd;
                    hsbt_uoc_gd_new.NguyenTegdReten = request.MtnRetenNte - nguyenTeGdReten;
                    hsbt_uoc_gd_new.SoTiengdReten = request.MtnRetenVnd - soTienGdReten;
                    hsbt_uoc_gd_new.TyleReten = request.TyleReten;
                    hsbt_uoc_gd_new.NguyenTegdPvi = request.NguyenTegd - nguyenTeGd;
                    hsbt_uoc_gd_new.SoTiengdPvi = request.SoTiengd - soTienGd;
                }
            }

            return hsbt_uoc_gd_new;
        }
        public async Task<string> DeleteHsbtGd(decimal pr_key)
        {
            string result = "";
            try
            {
                result = await _hsgdDxRepository.DeleteHsbtGd(pr_key);

            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public async Task<string> CreateHsbtThts(HsbtThtsRequest entity)
        {
            try
            {
                var HsbtThts = _mapper.Map<HsbtThtsRequest, HsbtTht>(entity);

                var ctu_tyle = _hsgdDxRepository.GetTTDonBH(entity.PrKeyHsgdCtu, HsbtThts.MaSp);
                if (ctu_tyle != null)
                {
                    HsbtThts.TyleReten = ctu_tyle.TyleReten;
                    HsbtThts.MtnRetenVnd = Math.Round(Convert.ToDecimal(HsbtThts.SoTienTd) * Convert.ToDecimal(ctu_tyle.TyleReten) / 100, 0);
                    HsbtThts.MtnRetenNte = Math.Round(Convert.ToDecimal(HsbtThts.NguyenTeTd) * Convert.ToDecimal(ctu_tyle.TyleReten) / 100, 0);
                    HsbtThts.NguyenTeTd = Math.Round(Convert.ToDecimal(HsbtThts.NguyenTeTd) * Convert.ToDecimal(ctu_tyle.TyleDong) / 100, 0);
                    HsbtThts.SoTienTd = Math.Round(Convert.ToDecimal(HsbtThts.SoTienTd) * Convert.ToDecimal(ctu_tyle.TyleDong) / 100, 0);
                }
                HsbtThts.NguyenTePvi = HsbtThts.NguyenTeTd;
                HsbtThts.SoTienPvi = HsbtThts.SoTienTd;
                HsbtThts.NguyenTetdPvi = HsbtThts.NguyenTeTd;
                HsbtThts.SoTientdPvi = HsbtThts.SoTienTd;
                var result = await _hsgdDxRepository.CreateHsbtThts(HsbtThts);
                return result;
            }
            catch (Exception ex)
            {
                //_logger.Error("CreateHsgdTtrinh:", ex);
                //_logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
            }
            return null!;
        }
        public async Task<string> UpdateHsbtThts(HsbtThtsRequest entity)
        {
            var result = "";
            try
            {
                var hsbtThts_old = _hsbtThtsRepository.GetEntityByConditionNoAsyncPias(x => x.PrKey == entity.PrKey);
                if (hsbtThts_old == null)
                {
                    return "entity is not found";
                }
                var hsbtThts_new = _mapper.Map(entity, hsbtThts_old);
                var ctu_tyle = _hsgdDxRepository.GetTTDonBH(entity.PrKeyHsgdCtu, hsbtThts_new.MaSp);
                if (ctu_tyle != null)
                {
                    hsbtThts_new.TyleReten = ctu_tyle.TyleReten;
                    hsbtThts_new.MtnRetenVnd = Math.Round(Convert.ToDecimal(hsbtThts_new.SoTienTd) * Convert.ToDecimal(ctu_tyle.TyleReten) / 100, 0);
                    hsbtThts_new.MtnRetenNte = Math.Round(Convert.ToDecimal(hsbtThts_new.NguyenTeTd) * Convert.ToDecimal(ctu_tyle.TyleReten) / 100, 0);
                    hsbtThts_new.NguyenTeTd = Math.Round(Convert.ToDecimal(hsbtThts_new.NguyenTeTd) * Convert.ToDecimal(ctu_tyle.TyleDong) / 100, 0);
                    hsbtThts_new.SoTienTd = Math.Round(Convert.ToDecimal(hsbtThts_new.SoTienTd) * Convert.ToDecimal(ctu_tyle.TyleDong) / 100, 0);
                }
                hsbtThts_new.NguyenTePvi = hsbtThts_new.NguyenTeTd;
                hsbtThts_new.SoTienPvi = hsbtThts_new.SoTienTd;
                hsbtThts_new.NguyenTetdPvi = hsbtThts_new.NguyenTeTd;
                hsbtThts_new.SoTientdPvi = hsbtThts_new.SoTienTd;
                result = await _hsgdDxRepository.UpdateHsbtThts(hsbtThts_new);
            }
            catch (Exception ex)
            {
                //_logger.Error("CreateHsgdTtrinh:", ex);
                //_logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
            }
            return result;
        }
        public async Task<string> DeleteHsbtThts(decimal pr_key)
        {
            string result = "";
            try
            {
                var entity = await _hsbtThtsRepository.GetEntityByConditionPias(x => x.PrKey == pr_key);
                if (entity == null)
                {
                    return "Không tồn tại thông tin thu hồi tài sản";
                }

                _hsbtThtsRepository.DeletePias(entity);
                await _hsbtThtsRepository.SaveAsyncPias();
                result = "Xoá thành công";

            }
            catch (Exception ex)
            {
                result = "Xoá thất bại";
            }
            return result;
        }
        public string CreatePASC(HsbtDxRequest entity)
        {
            try
            {


                //xử lý dữ liệu bảng HsgdDxCt chỉ update
                var hsgdDxCt_old = _hsgdDxCtRepository.GetEntityByConditionNoAsync(x => x.PrKey == entity.hsgdDxCt.PrKey);
                if (hsgdDxCt_old == null)
                {
                    return "entity is not found";
                }
                var hsgdDxCt_new = _mapper.Map(entity.hsgdDxCt, hsgdDxCt_old);
                var hsbtCt = _hsbtCtRepository.GetEntityByConditionNoAsyncPias(x => x.PrKey == hsgdDxCt_new.PrKeyHsbtCt);
                List<HsgdDx>? hsgdDx = new List<HsgdDx>();
                List<HsgdDxTsk>? hsgdDxTsk = new List<HsgdDxTsk>();
                if (hsbtCt != null)
                {
                    var ma_sp = hsbtCt.MaSp;
                    if (new[] { "050101", "050104" }.Contains(ma_sp))
                    {
                        hsgdDx = _mapper.Map<List<HsgdDxRequest>, List<HsgdDx>>(entity.hsgdDx);
                        hsgdDx.ForEach(a =>
                        {
                            a.NgayCapnhat = DateTime.Now;
                            a.GetDate = DateTime.Now;
                            a.SoTienpdtt = a.SoTientt;
                            a.SoTienpdsc = a.SoTienph + a.SoTienson;
                            a.PrKey = 0;
                        });
                        hsgdDxTsk = null;
                    }
                    else
                    {
                        hsgdDx = null;
                        hsgdDxTsk = _mapper.Map<List<HsgdDxRequest>, List<HsgdDxTsk>>(entity.hsgdDx);
                        hsgdDxTsk.ForEach(a =>
                        {
                            a.NgayCapnhat = DateTime.Now;
                            a.GetDate = DateTime.Now;
                            a.SoTienpdtt = a.SoTientt;
                            a.SoTienpdsc = a.SoTiensc;
                            a.PrKey = 0;
                        });
                    }

                }
                else
                {
                    return "entity error PrKeyHsbtCt";
                }
                var result = _hsgdDxRepository.CreatePASC(hsgdDxCt_new, hsgdDx, hsgdDxTsk);
                return result;
            }
            catch (Exception ex)
            {
            }
            return null!;
        }
        public async Task<string> ImportPASC(List<ImportPASCRequest> entity)
        {
            var result = "";
            try
            {
                //cập nhật cột mã hạng mục, xử lý dữ liệu null
                for (int i = 0; i < entity.Count; i++)
                {
                    if (entity[i].TenHmuc != "" && (entity[i].TenHmuc ?? "").IndexOf(">") > 0)
                    {
                        entity[i].MaHmuc = (entity[i].TenHmuc ?? "").Substring(0, (entity[i].TenHmuc ?? "").IndexOf(">")).Trim();
                    }
                    else
                    {
                        result = "Dòng " + (i + 2) + " cột 'Hạng mục sửa chữa' trong file excel nhập không định dạng đúng, đúng: MÃ HẠNG MỤC >> TÊN HẠNG MỤC. Vui lòng kiểm tra lại";
                        return result;
                    }
                    if (string.IsNullOrEmpty(entity[i].SoTientt))
                    {
                        entity[i].SoTientt = "0";
                    }
                    if (string.IsNullOrEmpty(entity[i].SoTienph))
                    {
                        entity[i].SoTienph = "0";
                    }
                    if (string.IsNullOrEmpty(entity[i].SoTienson))
                    {
                        entity[i].SoTienson = "0";
                    }
                    if (string.IsNullOrEmpty(entity[i].VatSc))
                    {
                        entity[i].VatSc = "0";
                    }
                }
                //xóa các dòng không có mã hạng mục
                entity.RemoveAll(x => x.MaHmuc == "");
                //kiểm tra dữ liệu
                for (int j = 0; j < entity.Count; j++)
                {
                    var hmuc = _dmHmucSuaChuaRepository.GetEntityByCondition(x => x.MaHmuc == entity[j].MaHmuc).Result;
                    if (hmuc == null)
                    {
                        result = "Dòng " + (j + 2) + " file excel có mã hạng mục không đúng với danh mục mã hạng mục. Vui lòng kiểm tra lại";
                        return result;
                    }
                    try
                    {
                        if (Convert.ToDecimal(entity[j].SoTientt) > 400000000)
                        {
                            result = "Dòng " + (j + 2) + " file excel có giá trị 'Thay thế' lớn hơn 400,000,000 triệu. Vui lòng kiểm tra lại";
                            return result;
                        }
                        if (Convert.ToDecimal(entity[j].SoTientt) < 0)
                        {
                            result = "Dòng " + (j + 2) + " file excel có giá trị 'Thay thế' âm. Vui lòng kiểm tra lại";
                            return result;
                        }
                    }
                    catch (Exception)
                    {
                        result = "Dòng " + (j + 2) + " file excel có giá trị 'Thay thế' nhập sai định dạng số. Vui lòng kiểm tra lại";
                        return result;
                    }

                    try
                    {
                        if (Convert.ToDecimal(entity[j].SoTienph) > 400000000)
                        {
                            result = "Dòng " + (j + 2) + " file excel có giá trị 'Phục hồi' lớn hơn 400,000,000 triệu. Vui lòng kiểm tra lại";
                            return result;
                        }
                        if (Convert.ToDecimal(entity[j].SoTienph) < 0)
                        {
                            result = "Dòng " + (j + 2) + " file excel có giá trị 'Phục hồi' âm. Vui lòng kiểm tra lại";
                            return result;
                        }
                    }
                    catch (Exception)
                    {
                        result = "Dòng " + (j + 2) + " file excel có giá trị 'Phục hồi' nhập sai định dạng số. Vui lòng kiểm tra lại";
                        return result;
                    }

                    try
                    {
                        if (Convert.ToDecimal(entity[j].SoTienson) > 400000000)
                        {
                            result = "Dòng " + (j + 2) + " file excel có giá trị 'Sơn' lớn hơn 400,000,000 triệu. Vui lòng kiểm tra lại";
                            return result;
                        }
                        if (Convert.ToDecimal(entity[j].SoTienson) < 0)
                        {
                            result = "Dòng " + (j + 2) + " file excel có giá trị 'Sơn' âm. Vui lòng kiểm tra lại";
                            return result;
                        }
                    }
                    catch (Exception)
                    {
                        result = "Dòng " + (j + 2) + " file excel có giá trị 'Sơn' nhập sai định dạng số. Vui lòng kiểm tra lại";
                        return result;
                    }
                    if ((entity[j].GhiChudv ?? "").Length >= 300)
                    {
                        result = "Dòng " + (j + 2) + " file excel có 'Ghi chú' quá dài. Vui lòng kiểm tra lại";
                        return result;
                    }
                    try
                    {
                        if (Convert.ToInt32(entity[j].VatSc) < 0 || Convert.ToInt32(entity[j].VatSc) > 10)
                        {
                            result = "Dòng " + (j + 2) + " file excel không được nhập VAT nhỏ hơn 0 hoặc lớn hơn 10. Vui lòng kiểm tra lại";
                            return result;
                        }
                    }
                    catch (Exception)
                    {
                        result = "Dòng " + (j + 2) + " file excel có giá trị 'VAT' nhập sai định dạng số. Vui lòng kiểm tra lại";
                        return result;
                    }
                }

                var hsgdDx = _mapper.Map<List<ImportPASCRequest>, List<HsgdDx>>(entity);
                hsgdDx.ForEach(a =>
                {
                    a.NgayCapnhat = DateTime.Now;
                    a.GetDate = DateTime.Now;
                    a.SoTienpdtt = a.SoTientt;
                    a.SoTienpdsc = a.SoTienph + a.SoTienson;
                });
                result = await _hsgdDxRepository.ImportPASC(hsgdDx);
            }
            catch (Exception ex)
            {
                //_logger.Error("CreateHsgdTtrinh:", ex);
                //_logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
            }
            return result;
        }
        public DownloadFileResult PrintPASC(decimal pr_key_hsbt_ct, decimal pr_key_hsgd_ctu, string email, int loai_dx)
        {
            try
            {
                DownloadFileResult result1 = null;
                var path_pasc = _hsgdDxRepository.GetFilePathPasc(pr_key_hsbt_ct);
                if (string.IsNullOrEmpty(path_pasc))
                {
                    var result = _hsgdDxRepository.PrintPASC(pr_key_hsbt_ct, pr_key_hsgd_ctu, email, loai_dx);

                    var downloadSettings = _configuration.GetSection("DownloadSettings").Get<DownloadSettings>();
                    var word2PdfSettings = _configuration.GetSection("Word2PdfSettings").Get<Word2PdfSettings>();

                    var optionsDownloadSettings = Microsoft.Extensions.Options.Options.Create(downloadSettings);
                    var optionsWord2PdfSettings = Microsoft.Extensions.Options.Options.Create(word2PdfSettings);


                    var contentHelper = new ContentHelper(optionsDownloadSettings, optionsWord2PdfSettings, _logger);

                    if (result != null && result.ThirdQueryResults != null)
                    {
                        result1 = contentHelper.ConvertFileWordToPdf_PASC(result.ThirdQueryResults, result.ListPascDetail, loai_dx);
                    }
                    var ma_ttrang_gd = _hsgdDxRepository.GetMaTtrangGd(pr_key_hsgd_ctu);
                    if (result1 != null && ma_ttrang_gd == "6")
                    {
                        string folderUpload = _configuration.GetValue<string>("UploadSettings:FolderUpload") ?? "";
                        string url_upload = _configuration.GetValue<string>("DownloadSettings:UlpoadServer") ?? "";
                        var utilityHelper = new UtilityHelper(_logger);
                        var file_path = utilityHelper.UploadFile_ToAPI(result1.Data, ".pdf", folderUpload, url_upload, false);
                        var update_pathfile = _hsgdDxRepository.UpdatePathPasc(pr_key_hsbt_ct, file_path);
                        //lấy thông tin chữ ký điện tử/
                        var SignContent = _hsgdDxRepository.GetThongTinKyDienTu(pr_key_hsgd_ctu, email);
                        //
                        var chk_ky = _hsgdDxRepository.KyPASCXCG(pr_key_hsgd_ctu, file_path, email, SignContent);
                        string url_download = _configuration.GetValue<string>("DownloadSettings:DownloadServer") ?? "";
                        result1 = UtilityHelper.DownloadFile_ToAPI(file_path, url_download);
                        return result1;
                    }
                    else
                    {
                        return result1;
                    }
                    return result1;
                }
                else
                {
                    string url_download = _configuration.GetValue<string>("DownloadSettings:DownloadServer") ?? "";
                    result1 = UtilityHelper.DownloadFile_ToAPI(path_pasc, url_download);
                    return result1;
                }


            }
            catch (Exception ex)
            {
            }
            return null;
        }
        public string GuiPASC(decimal pr_key_hsbt_ct, decimal pr_key_hsgd_ctu, bool chk_send_pasc, bool pasc_send_sms, string email_nhan, string phone_nhan, string email_login)
        {
            try
            {
                var pasc_send_mail = _hsgdDxRepository.GetPascSendMail(pr_key_hsbt_ct);
                if (pasc_send_mail == 1)
                {
                    return "Hồ sơ đã được gửi PASC. Vui lòng kiểm tra lại!";
                }
                //upload file pasc
                //var result = PrintPASC(pr_key_hsbt_ct, pr_key_hsgd_ctu, email_login, 0);
                //string folderUpload = _configuration.GetValue<string>("UploadSettings:FolderUpload") ?? "";
                //string url_upload = _configuration.GetValue<string>("DownloadSettings:UlpoadServer") ?? "";
                //var utilityHelper = new UtilityHelper(_logger);
                //var file_path = utilityHelper.UploadFile_ToAPI(result.Data, ".pdf", folderUpload, url_upload, false);
                var res = "";
                var ma_ttrang_gd = _hsgdDxRepository.GetMaTtrangGd(pr_key_hsgd_ctu);
                if (ma_ttrang_gd != "6")
                {
                    return "Hồ sơ giám định chưa được duyệt. Vui lòng kiểm tra lại!";
                }
                var file_path = _hsgdDxRepository.GetFilePathPasc(pr_key_hsbt_ct);
                if (string.IsNullOrEmpty(file_path))
                {
                    //return "Hồ sơ giám định đã được duyệt nhưng chưa có file đề xuất pasc . Vui lòng kiểm tra lại!";

                    //upload file pasc
                    var hsgd_dx_ct = _hsgdDxRepository.GetHsgdDxCt(pr_key_hsbt_ct);
                    var loai_dx = new[] { "050101", "050104" }.Contains(hsgd_dx_ct.MaSp) ? 0 : 1;
                    var result = PrintPASC(pr_key_hsbt_ct, pr_key_hsgd_ctu, email_login, loai_dx);
                    string folderUpload = _configuration.GetValue<string>("UploadSettings:FolderUpload") ?? "";
                    string url_upload = _configuration.GetValue<string>("DownloadSettings:UlpoadServer") ?? "";
                    var utilityHelper = new UtilityHelper(_logger);
                    var file_path_new = utilityHelper.UploadFile_ToAPI(result.Data, ".pdf", folderUpload, url_upload, false);
                    res = _hsgdDxRepository.GuiPASC(pr_key_hsbt_ct, pr_key_hsgd_ctu, chk_send_pasc, pasc_send_sms, email_nhan, phone_nhan, email_login, file_path_new);
                }
                else
                {
                    res = _hsgdDxRepository.GuiPASC(pr_key_hsbt_ct, pr_key_hsgd_ctu, chk_send_pasc, pasc_send_sms, email_nhan, phone_nhan, email_login, file_path);
                }


                return res;
            }
            catch (Exception ex)
            {
            }
            return null;
        }
        public bool CreatFilePasc(decimal pr_key_hsgd_ctu, string email)
        {
            try
            {
                var hsgd_ctu = _hsgdDxRepository.GetHsgdCtu(pr_key_hsgd_ctu);
                if (hsgd_ctu != null && hsgd_ctu.MaTtrangGd == "6")
                {
                    var hsgd_dx_ct = _hsgdDxRepository.GetListHsgdDxCt(hsgd_ctu.PrKeyBt);
                    foreach (var item in hsgd_dx_ct)
                    {
                        var loai_dx = new[] { "050101", "050104" }.Contains(item.MaSp) ? 0 : 1;
                        var result = _hsgdDxRepository.PrintPASC(item.PrKeyHsbtCt, pr_key_hsgd_ctu, email, loai_dx);

                        var downloadSettings = _configuration.GetSection("DownloadSettings").Get<DownloadSettings>();
                        var word2PdfSettings = _configuration.GetSection("Word2PdfSettings").Get<Word2PdfSettings>();

                        var optionsDownloadSettings = Microsoft.Extensions.Options.Options.Create(downloadSettings);
                        var optionsWord2PdfSettings = Microsoft.Extensions.Options.Options.Create(word2PdfSettings);


                        DownloadFileResult result1 = null;
                        var contentHelper = new ContentHelper(optionsDownloadSettings, optionsWord2PdfSettings, _logger);

                        if (result != null && result.ThirdQueryResults != null)
                        {
                            result1 = contentHelper.ConvertFileWordToPdf_PASC(result.ThirdQueryResults, result.ListPascDetail, loai_dx);
                        }
                        if (result1 != null)
                        {
                            string folderUpload = _configuration.GetValue<string>("UploadSettings:FolderUpload") ?? "";
                            string url_upload = _configuration.GetValue<string>("DownloadSettings:UlpoadServer") ?? "";
                            var utilityHelper = new UtilityHelper(_logger);
                            var file_path = utilityHelper.UploadFile_ToAPI(result1.Data, ".pdf", folderUpload, url_upload, false);
                            var update_pathfile = _hsgdDxRepository.UpdatePathPasc(item.PrKeyHsbtCt, file_path);
                            var chk_ky = _hsgdDxRepository.KyPASCXCG(pr_key_hsgd_ctu, file_path, email, "");
                        }
                    }

                }
                _logger.Information("CreatFilePasc có pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + "success");
                return true;

            }
            catch (Exception ex)
            {
                _logger.Error("CreatFilePasc có pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + "Error: " + ex);
                return false;
            }
        }
        public Task<PagedList<LichsuPa>> LichSuPasc(LichsuPaParameters parameters)
        {
            var list = _hsgdDxRepository.LichSuPasc(parameters);
            return list;
        }
        public Task<List<KHVat>> GetListDonViGiamDinh(string email_login)
        {
            var list_dvgd = _hsgdDxRepository.GetListDonViGiamDinh(email_login);
            return list_dvgd;
        }

        public List<FileAttachBt> GetFileAttachBt(decimal pr_key_hsbt_ct, string ma_ctu)
        {
            List<FileAttachBt> obj_result = new List<FileAttachBt>();
            try
            {
                obj_result = _hsgdDxRepository.GetFileAttachBt(pr_key_hsbt_ct, ma_ctu);

            }
            catch (Exception ex)
            {
            }
            return obj_result;
        }
        public DownloadFileResult DownloadFileAttachBt_MDF1(decimal pr_key)
        {
            DownloadFileResult result = new DownloadFileResult();
            try
            {
                string pathFile = "";
                var tt = _fileAttachBtRepository.GetByIdNoAsyncPias(pr_key);
                pathFile = tt != null ? tt.Directory : "";
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
        public decimal GetSTBTByHsgd(decimal pr_key_hsbt_ctu)
        {
            var sotien_bt = _hsgdDxRepository.GetSTBTByHsgd(pr_key_hsbt_ctu);
            return sotien_bt;
        }
        public async Task<ServiceResult> PheDuyetTBBT(int prKey, string currentUserEmail)
        {
            return await _hsgdDxRepository.PheDuyetTBBT(prKey, currentUserEmail);
        }

        public async Task<bool> KySoTBBT(decimal pr_key_hsgd_ctu, string email)
        {
            try
            {
                var result =  PrintThongBaoBT(pr_key_hsgd_ctu, email, true);
                if (result != null)
                {
                    string folderUpload = _configuration.GetValue<string>("UploadSettings:FolderUpload") ?? "";
                    string url_upload = _configuration.GetValue<string>("DownloadSettings:UlpoadServer") ?? "";
                    var utilityHelper = new UtilityHelper(_logger);
                    var file_path =  utilityHelper.UploadFile_ToAPI(result.Data, ".pdf", folderUpload, url_upload, false);

                    await _hsgdDxRepository.UpdatePathTBBT(pr_key_hsgd_ctu, file_path);
                    var SignContent = await _hsgdDxRepository.GetThongTinKyDienTuTBBT(pr_key_hsgd_ctu, email);
                    var chk_ky = await _hsgdDxRepository.KyTBBT(pr_key_hsgd_ctu, file_path, email, SignContent);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("KySoTBBT có pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + "Error: " + ex);
                return false;
            }
        }
        public DownloadFileResult PrintThongBaoBT(decimal pr_key_hsgd_ctu, string email,bool pdf_file)
        {
            try
            {
                DownloadFileResult result1 = null;
                var result = _hsgdDxRepository.PrintThongBaoBT(pr_key_hsgd_ctu, email);

                var downloadSettings = _configuration.GetSection("DownloadSettings").Get<DownloadSettings>();
                var word2PdfSettings = _configuration.GetSection("Word2PdfSettings").Get<Word2PdfSettings>();

                var optionsDownloadSettings = Microsoft.Extensions.Options.Options.Create(downloadSettings);
                var optionsWord2PdfSettings = Microsoft.Extensions.Options.Options.Create(word2PdfSettings);


                var contentHelper = new ContentHelper(optionsDownloadSettings, optionsWord2PdfSettings, _logger);

                if (result != null && result.ThirdQueryResults != null)
                {
                    result1 = contentHelper.ConvertFileWordToPdf_ThongBaoBT(result.ThirdQueryResults, result.ListThuHuong, pdf_file);
                }
                return result1;
            }
            catch (Exception ex)
            {
            }
            return null;
        }
        public Task<List<DanhMuc>> GetListLoaiDongCo()
        {
            var list_sp = _hsgdDxRepository.GetListLoaiDongCo();
            return list_sp;
        }
        public ServiceResult GuiThongBaoBT(decimal pr_key_hsgd_ctu, string email_nhan, string email_login)
        {
            try
            {
                // Kiểm tra đã gửi thông báo chưa
                var send_thongbao_bt = _hsgdDxRepository.GetSendMailThongBaoBT(pr_key_hsgd_ctu);
                if (send_thongbao_bt == 1)
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Hồ sơ đã được gửi thông báo bồi thường. Vui lòng kiểm tra lại!"
                    };
                }

                // Kiểm tra trạng thái giám định
                var ma_ttrang_gd = _hsgdDxRepository.GetMaTtrangGd(pr_key_hsgd_ctu);
                if (ma_ttrang_gd != "6")
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Hồ sơ giám định chưa được duyệt. Vui lòng kiểm tra lại!"
                    };
                }

                // Kiểm tra trạng thái bồi thường
                //haipv bỏ check theo yêu cầu: 73471 Bổ sung tính nắng cho phép gửi mail khi Thông báo bồi thường được duyệt
                //var check_trangthai_bt = _hsgdDxRepository.CheckTrangThaiBT(pr_key_hsgd_ctu);
                //if (!check_trangthai_bt)
                //{
                //    return new ServiceResult
                //    {
                //        Success = false,
                //        Message = "Hồ sơ bồi thường chưa được duyệt. Vui lòng kiểm tra lại!"
                //    };
                //}

                // Kiểm tra file thông báo đã ký
                string file_path_new = _hsgdDxRepository.CheckKyTBBT(pr_key_hsgd_ctu);
                if (string.IsNullOrEmpty(file_path_new))
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Thông báo bồi thường chưa được duyệt, không gửi được email. Vui lòng kiểm tra lại!"
                    };
                }

                // Gửi email
                var res = _hsgdDxRepository.GuiThongBaoBT(pr_key_hsgd_ctu, email_nhan, email_login, file_path_new);

                // Trả về kết quả từ repository
                return res;
            }
            catch (Exception ex)
            {
                _logger.Error($"GuiThongBaoBT Exception: pr_key_hsgd_ctu = {pr_key_hsgd_ctu}, Error: {ex}");

                return new ServiceResult
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi gửi email, vui lòng liên hệ IT"
                };
            }
        }
        public TTPrintPasc GetPrintPASC(decimal pr_key_hsbt_ct, decimal pr_key_hsgd_ctu, string email, int loai_dx)
        {
            TTPrintPasc result = new TTPrintPasc();
            try
            {
                result = _hsgdDxRepository.GetPrintPASC(pr_key_hsbt_ct, pr_key_hsgd_ctu,email, loai_dx);

            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public string CheckTrungHsbt(decimal pr_key_hsgd_ctu,string ma_sp)
        {
            var checktrung = _hsgdDxRepository.CheckTrungHsbt(pr_key_hsgd_ctu, ma_sp);
            return checktrung;
        }
        public List<string> Lay_mst_donvicapdon(decimal pr_key_hsgd_ctu)
        {
            List<string> mst = _hsgdDxRepository.Lay_mst_donvicapdon(pr_key_hsgd_ctu);
            return mst;
        }
        public string LayThongTinMDF(string PathFile)
        {
            string url_download = "";
            string url_download_mdf1 = _configuration["DownloadSettings:DownloadServer_MDF1"] ?? "";
            string url_download_mdf3 = _configuration["DownloadSettings:DownloadServer"] ?? "";

            if (PathFile.IndexOf("P247_Upload_New", StringComparison.OrdinalIgnoreCase) >= 0)
                url_download = url_download_mdf3;
            else if (PathFile.IndexOf("pias_upload", StringComparison.OrdinalIgnoreCase) >= 0)
                url_download = url_download_mdf1;
            else if (PathFile.IndexOf("cssk_upload", StringComparison.OrdinalIgnoreCase) >= 0)
                url_download = url_download_mdf3;

            return url_download;
        }
        public DownloadFileResult DownloadFile(string filePath)
        {
            DownloadFileResult result = new DownloadFileResult();
            try
            {
                   
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        string url_download = LayThongTinMDF(filePath);

                        if (!string.IsNullOrEmpty(url_download))
                        {
                            result = UtilityHelper.DownloadFile_ToAPI(filePath, url_download);
                            _logger.Information($"DownloadFile: pathFile = {filePath}, url = {url_download}, status = {result.Status}");
                        }
                        else
                        {
                            result.Status = "-404";
                            result.Message = "Cannot determine download server for the given path";
                            _logger.Warning($"DownloadFile: Cannot determine URL  pathFile = {filePath}");
                        }
                    }
                    else
                    {
                        result.Status = "-404";
                        result.Message = "File path is empty";
                    }
                
            }
            catch (Exception ex)
            {
                result.Status = "-500";
                result.Message = "An error occurred during download";
                _logger.Error($"DownloadFile error {ex}");
            }
            return result;
        }
    }
}