using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Office.Interop.Word;
using PVI.DAO.Entities.Models;
using PVI.Helper;
using PVI.Repository.Interfaces;
using PVI.Repository.Repositories;
using PVI.Service.Request;
using Serilog.Core;
using ServiceReference1;
using System.Net.WebSockets;
using static iTextSharp.text.pdf.AcroFields;
using static iTextSharp.text.pdf.events.IndexEvents;
using static PVI.Repository.Repositories.HsgdCtuRepository;

namespace PVI.Service
{
    public class HsgdCtuService
    {
        private readonly IHsgdCtuRepository _hsgdCtuRepository;
        private readonly Serilog.ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration; 
        
        public HsgdCtuService(IHsgdCtuRepository hsgdCtuRepository, IMapper mapper, Serilog.ILogger logger, IConfiguration conf)
        {
            _hsgdCtuRepository = hsgdCtuRepository;
            _mapper = mapper;
            _logger = logger;
            _configuration = conf;
        }

        public DmUserView GetCurrentUserInfo(string currentUserEmail)
        {
            try
            {
                DmUserView currentUser = _hsgdCtuRepository.GetCurrentUserInfo(currentUserEmail);
                return currentUser;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<HsgdCtu> GetBySoHsgd(string so_hsgd)
        {
            HsgdCtu? objResult = new HsgdCtu();
            try
            {
                objResult = await _hsgdCtuRepository.GetBySoHsgd(so_hsgd);
            }
            catch (Exception ex)
            {
            }
            return objResult;
        }
        public async Task<HsgdCtu> GetHsgdByPrKey(decimal prKey)
        {
            HsgdCtu? objResult = new HsgdCtu();
            try
            {
                objResult = await _hsgdCtuRepository.GetHsgdByPrKey(prKey);
            }
            catch (Exception ex)
            {
            }
            return objResult;
        }
        public List<HsgdTtrinhCt> GetListHsgdDx(string so_hsgd)
        {
            List<HsgdTtrinhCt> obj_result = new List<HsgdTtrinhCt>();
            try
            {
                obj_result = _hsgdCtuRepository.GetListHsgdDx(so_hsgd);

            }
            catch (Exception ex)
            {
            }
            return obj_result;
        }
        public HsgdTtrinhAll GetListHsgdDxNew(string so_hsgd)
        {
            HsgdTtrinhAll obj_result = new HsgdTtrinhAll();
            try
            {
                obj_result = _hsgdCtuRepository.GetListHsgdDxNew(so_hsgd);

            }
            catch (Exception ex)
            {
            }
            return obj_result;
        }
        public Task<List<DanhMuc>> GetListNguyenNhanTonThat()
        {
            var list_nntt = _hsgdCtuRepository.GetListNguyenNhanTonThat();
            return list_nntt;
        }
        public Task<List<DanhMucTinh>> GetListDiaDiemTonThat()
        {
            var list_ddtt = _hsgdCtuRepository.GetListDiaDiemTonThat();
            return list_ddtt;
        }
        public Task<List<DanhMuc>> GetListSanPham()
        {
            var list_sp = _hsgdCtuRepository.GetListSanPham();
            return list_sp;
        }
        public Task<List<DanhMuc>> GetListTrangThai()
        {
            var list_tt = _hsgdCtuRepository.GetListTrangThai();
            return list_tt;
        }
        public List<TtrangGdCount> GetCountByStatus(string fromDate,string toDate, string email, string MaDonbh)
        {
            var list_tt = _hsgdCtuRepository.GetCountByStatus(fromDate,toDate, email,MaDonbh);
            return list_tt;
        }
        public async Task<HsgdCtuDetail> GetData_Detail_Hsgd(decimal prKey)
        {
            HsgdCtuDetail? objResult = new HsgdCtuDetail();
            try
            {
                objResult = await _hsgdCtuRepository.GetData_Detail_Hsgd(prKey);
            }
            catch (Exception ex)
            {
            }
            return objResult;
        }

        public async Task<HsgdCtuDetail> GetDataDetailBySoHsgd(string soHsgd)
        {
            HsgdCtuDetail? objResult = new HsgdCtuDetail();
            var prKey = await _hsgdCtuRepository.GetPrKeyBySoHsgd(soHsgd);
            if (prKey != 0)
            {


                try
                {
                    objResult = await _hsgdCtuRepository.GetData_Detail_Hsgd(prKey);
                }
                catch (Exception ex)
                {
                }
                return objResult;
            }
            else
            {
                return objResult;
            }

        }
        public BCGiamDinh ReadOCR(decimal pr_key_hsgd)
        {
            BCGiamDinh objResult = new BCGiamDinh();
            try
            {
                objResult = _hsgdCtuRepository.ReadOCR(pr_key_hsgd);
            }
            catch (Exception ex)
            {
            }
            return objResult;
        }
        public Task<List<DmTtrangGd>> GetListStatusName()
        {
            var list_tt = _hsgdCtuRepository.GetListStatusName();
            return list_tt;
        }

        public Task<List<DmLoaiHsgd>> GetListTypeName()
        {
            var list_loai_hsgd = _hsgdCtuRepository.GetListTypeName();
            return list_loai_hsgd;
        }

        public string updateDetailFile(int prKey, int type, string currentUserEmail)
        {
            string result = _hsgdCtuRepository.updateDetailFile(prKey, type, currentUserEmail);
            return result;
        }
        public Task<PagedList<HsgdCtuResponse>> GetList(HsgdCtuParameters parameters, string email,string MaDonbh)
        {
            var list_tt = _hsgdCtuRepository.GetList(parameters, email, MaDonbh);
            return list_tt;
        }

        // 3 method dưới đây trả về danh sách các hạng mục.
        // khanhlh - 09/05/2024

        public List<DmTongthanhxe> getListTongThanhXe()
        {
            List<DmTongthanhxe> list_hm = _hsgdCtuRepository.getListTongThanhXe();
            return list_hm;
        }

        public List<DmNhmuc> getListNhmuc()
        {
            List<DmNhmuc> list_hm = _hsgdCtuRepository.getListNhmuc();
            return list_hm;
        }

        public List<DmHmuc> getListHmuc(string? ma_tongthanhxe, string? ma_hmuc)
        {
            List<DmHmuc> list_hm = _hsgdCtuRepository.getListHmuc(ma_tongthanhxe, ma_hmuc);
            return list_hm;
        }

        public List<DmHmucGiamdinh> getListHmucGiamDinh()
        {
            List<DmHmucGiamdinh> list_hm = _hsgdCtuRepository.getListHmucGiamDinh();
            return list_hm;
        }

        public List<DmTtrangGd> getListTtrangGd()
        {
            List<DmTtrangGd> list_hm = _hsgdCtuRepository.getListTtrangGd();
            return list_hm;
        }
        public List<DmTte> getListTienTe()
        {
            List<DmTte> list_hm = _hsgdCtuRepository.getListTienTe();
            return list_hm;
        }

        public List<DmHieuxe> getListHieuxe()
        {
            List<DmHieuxe> list_hm = _hsgdCtuRepository.getListHieuxe();
            return list_hm;
        }

        public List<DmLoaixe> getListLoaixe(int prKey_hieu_xe)
        {
            List<DmLoaixe> list_hm = _hsgdCtuRepository.getListLoaixe(prKey_hieu_xe);
            return list_hm;
        }

        public List<DmLoaiHinhTd> getListLoaiChiPhi()
        {
            List<DmLoaiHinhTd> list_hm = _hsgdCtuRepository.getListLoaiChiPhi();
            return list_hm;
        }

        public async Task<DiaryResponse> GetListDiary(int prKey, int pageNumber, int pageSize)
        {
            PagedList<NhatKy> list_nhat_ky = await _hsgdCtuRepository.getListDiary(prKey, pageNumber, pageSize);

            Dictionary<string, DmUserView> userList = await _hsgdCtuRepository.getListRelatedUsers(prKey);

            DiaryResponse newDiaryResponse = new DiaryResponse();

            newDiaryResponse.users = userList;
            newDiaryResponse.data = list_nhat_ky;

            return newDiaryResponse;
        }
        public Task<List<ImageResponse>> GetListAppraisalImage(int prKey)
        {
            try
            {
                var list_tt = _hsgdCtuRepository.GetListAppraisalImage(prKey);
                return list_tt;
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return null;
            }

        }
        public string updateDetail(int prKey, HsgdCtuRequest entity)
        {
            var result = "";
            entity.HsgdCtuUdpdate.PrKey = prKey;
            try
            {
                HsgdCtu hsgd_old = _hsgdCtuRepository.GetEntityByCondition(x => x.PrKey == entity.HsgdCtuUdpdate.PrKey).Result;

                if (hsgd_old != null)
                {
                    var hsgd_new = _mapper.Map(entity.HsgdCtuUdpdate, hsgd_old);
                    result = _hsgdCtuRepository.updateFile(hsgd_new);
                }
                else
                {
                    result = $"Hồ sơ với PrKey {entity.HsgdCtuUdpdate.PrKey} không tồn tại.";
                }
            }
            catch (Exception ex)
            {
                //_logger.Error("CreateDiemtruc:", ex);
                //_logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
            }
            return result;
        }

        // Gán giám định:
        public async Task<string> assignAppraisal(int prKey, HsgdGanGiamDinh gdd, string currentUserEmail)
        {
            string result = await _hsgdCtuRepository.assignAppraisal(prKey, gdd.oidGiamDinhVien, gdd.oidCanBoTT, gdd.guiEmail, gdd.guiSMS, currentUserEmail);
            return result;
        }

        public string GanNguoiTiepNhan(int pr_key, string ghiChu, string oidCanBoPheDuyet, string currentUserEmail)
        {
            string result = _hsgdCtuRepository.GanNguoiTiepNhan(pr_key, ghiChu, oidCanBoPheDuyet, currentUserEmail);
            return result;
        }

        public string ChuyenGanHoSo(int pr_key, string ghiChu, string oidCanBoPheDuyet, string currentUserEmail)
        {
            string result = _hsgdCtuRepository.ChuyenGanHoSo(pr_key, ghiChu, oidCanBoPheDuyet, currentUserEmail).Result;
            return result;
        }

        public async Task<string> requestApproval(int prKey, HsgdCtuChoPheDuyet cpd, string currentUserEmail)
        {
            string result = await _hsgdCtuRepository.requestApproval(prKey, cpd.ghiChu, currentUserEmail, cpd.oidCanBoPheDuyet);
            return result;
        }

        // Bổ sung thông tin.
        public async Task<string> requestAdditionalDetail(int prKey, HsgdCtuBoSungThongTin bstt, string currentUserEmail)
        {
            string result = await _hsgdCtuRepository.requestAdditionalDetail(prKey, bstt.guiEmail, bstt.guiSMS, bstt.ghiChu, currentUserEmail);
            return result;
        }

        // Phê duyệt hồ sơ.
        public string approveAppraisal(int prKey, string ghiChu, string currentUserEmail)
        {
            string result = _hsgdCtuRepository.approveAppraisal(prKey, ghiChu, currentUserEmail).Result;
            return result;
        }
        public string Baogia_giamdinh(baogia_request hsgddg_Request,string currentUserEmail)
        {
            string result = _hsgdCtuRepository.Baogia_giamdinh(hsgddg_Request.pr_key, hsgddg_Request.ngay_bao_gia, hsgddg_Request.so_tien, hsgddg_Request.de_xuat, currentUserEmail).Result;
            return result;
        }
        public string Duyetgia_giamdinh(duyetgia_request hsgddg_Request, string currentUserEmail)
        {
            string result = _hsgdCtuRepository.Duyetgia_giamdinh(hsgddg_Request.pr_key, hsgddg_Request.ngay_duyet_gia, hsgddg_Request.so_tien, hsgddg_Request.de_xuat, currentUserEmail).Result;
            return result;
        }
        public List<DonViThanhToanResponse> GetListDonViTT()
        {
            List<DonViThanhToanResponse> donvi_tt = _hsgdCtuRepository.getListDonViThanhToan();
            return donvi_tt;
        }
        public bool Kiemtra_uynhiemchi(string ma_donvi)
        {
            bool uynhiemchi = _hsgdCtuRepository.Kiemtra_uynhiemchi(ma_donvi);
            return uynhiemchi;
        }
        public DonViThanhToanResponse GetInfoDonviTT(string ma_don_vi)
        {
            DonViThanhToanResponse donvi_tt = _hsgdCtuRepository.GetInfoDonViTT(ma_don_vi);
            return donvi_tt;
        }

        public async Task<string> pheDuyetBaoLanh(int prKey,decimal pr_key_hsbt_ct, PheDuyetBaoLanhRequest pdbl, string currentUserEmail)
        {
            string result = await _hsgdCtuRepository.pheDuyetBaoLanh(prKey, pr_key_hsbt_ct, pdbl.bl1.Value, pdbl.bl2.Value, pdbl.bl3.Value, pdbl.bl4.Value, pdbl.bl5.Value, pdbl.bl6.Value, pdbl.bl7.Value, pdbl.bl8.Value, pdbl.bl9.Value, pdbl.bl_tailieubs, pdbl.bl_dsemail, pdbl.bl_dsphone, pdbl.ma_donvi_tt, currentUserEmail);
            return result;
        }
        
        public async Task<string> LuuThongbaoBT(LuuThongBaoBTRequest tbbt,string currentUserEmail)
        {
            
            string result = await _hsgdCtuRepository.LuuThongbaoBT(tbbt.PrKeyHsgd, tbbt.HsgdTbbt, tbbt.HsgdTbbtTt,currentUserEmail);
            return result;
        }
        public async Task<LuuThongBaoBTResponse> LayThongbaoBT(int pr_key_hsgd, string currentUserEmail)
        {
            var result = await _hsgdCtuRepository.LayThongbaoBT(pr_key_hsgd, currentUserEmail);
            return result;
        }
        public async Task<string> UploadAppraisalImage(UploadFileContent listFile, int prKey, Guid oid, int stt, string maHmuc, string dienGiai, string maHmucSc)
        {
            string result = await _hsgdCtuRepository.UploadAppraisalImage(listFile, prKey, oid, stt, maHmuc, dienGiai, maHmucSc);
            return result;
        }



        public async Task<List<GDDKResponse>> GetAnhGDDK(int prKey)
        {
            var result = await _hsgdCtuRepository.GetAnhGDDK(prKey);
            return result;
        }
        public async Task<List<HsgdDg>> GetThongTinDuyetGia(int prKey)
        {
            var result = await _hsgdCtuRepository.GetThongTinDuyetGia(prKey);
            return result;
        }

        public async Task<string> UpdateAppraisalImage(List<UpdateImageRequest> request)
        {
            for (int i = 0; i < request.Count(); i++)
            {
                var result = await _hsgdCtuRepository.UpdateAppraisalImage(request[i].PrKey, request[i].DienGiai, request[i].MaHmuc, request[i].Stt, request[i].MaHmucSc);
                if (result != "Success")
                {
                    return "Error";
                }
            }
            return "Success";
        }

        public async Task<string> UpdateURLImage(UploadFileContent listFile, int pr_key_ct, int pr_key, Guid oid)
        {
            string result = await _hsgdCtuRepository.UpdateURLImage(listFile, pr_key_ct, pr_key,oid);
            return result;
        }
        public DownloadFileResult DownloadTtrinh11_MDF1(int prKey)
        {
            DownloadFileResult result = new DownloadFileResult();
            try
            {
                string pathFile = "";
                var tt = _hsgdCtuRepository.DownloadTtrinh11_MDF1(prKey);
                return tt;


            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public Task<List<HsgdDgCt>> GetListAnhDuyetGia(int prKey, bool loai_dg)
        {
            var list_tt = _hsgdCtuRepository.GetAnhDuyetGia(prKey, loai_dg);
            return list_tt;
        }
        public async Task<string> ChuyenAnhGDTT(string soHsgdChuyen, string soHsgdNhan)
        {
            var result = await _hsgdCtuRepository.ChuyenAnhGDTT(soHsgdChuyen, soHsgdNhan);
            return result;
        }
        public async Task<string> UploadAnhDuyetgia(UploadFileContent listFile, int prKey, bool loai_dg)
        {
            string result = await _hsgdCtuRepository.UploadAnhDuyetgia(listFile, prKey, loai_dg);
            return result;
        }
        public async Task<string> UpdateURLAnhDuyetgia(UploadFileContent listFile, int prKey, int prKeyDgCt)
        {
            string result = await _hsgdCtuRepository.UpdateURLAnhDuyetgia(listFile, prKey, prKeyDgCt);
            return result;
        }
        public async Task<string> UpdateAnhDuyetGia(UpdateDuyetGiaRequest request)
        {
            var result = await _hsgdCtuRepository.UpdateAnhDuyetGia(request.PrKey, request.LoaiDg, request.DeXuat, request.SoTien);
            return result;
        }

        public string UploadSampleFile(string localPath)
        {
            var result = _hsgdCtuRepository.uploadSampleFile(localPath);
            return result;
        }
        public List<DmLdonBt> GetDmLdonBt()
        {
            var result = _hsgdCtuRepository.GetDmLdonBt();
            return result;
        }
        public async Task<List<ProductInfoResponse>> GetProductInfo(string soDonBh, string soDonBhbs)
        {
            var result = await _hsgdCtuRepository.GetProductInfo(soDonBh, soDonBhbs);
            return result;
        }
        public async Task<string> GetSoDonBh(int prKey)
        {
            var result = await _hsgdCtuRepository.GetSoDonBh(prKey);
            return result;
        }
        public Task<List<DanhMuc>> GetListLoaiBang()
        {
            var list = _hsgdCtuRepository.GetListLoaiBang();
            return list;
        }

        public List<DmTyGia> GetListTyGia()
        {
            var list = _hsgdCtuRepository.getListTyGia();
            return list;
        }
        public Guid GetOidByEmail(string email)
        {
            var result = _hsgdCtuRepository.GetOidByEmail(email);
            return result;
        }

        // khanhlh - 18/11/2024: 
        // Lấy bảo lãnh PDF để có thể preview / download.
        public DownloadFileResult printPDFBaoLanh(decimal prKey,decimal pr_key_hsbt_ct, string currentUserEmail)
        {
            try
            {
                var result = _hsgdCtuRepository.BaoLanh_GetListOfReplacable(prKey, pr_key_hsbt_ct, currentUserEmail,"");
                _logger.Error(result.ToString());

                var downloadSettings = _configuration.GetSection("DownloadSettings").Get<DownloadSettings>();
                var word2PdfSettings = _configuration.GetSection("Word2PdfSettings").Get<Word2PdfSettings>();

                var optionsDownloadSettings = Microsoft.Extensions.Options.Options.Create(downloadSettings);
                var optionsWord2PdfSettings = Microsoft.Extensions.Options.Options.Create(word2PdfSettings);

                var contentHelper = new ContentHelper(optionsDownloadSettings, optionsWord2PdfSettings, _logger);

                DownloadFileResult result1 = null;

                if (result != null && result.ThirdQueryResults != null)
                {
                    result1 = contentHelper.ConvertFileWordToPdf_BaoLanh(result.ThirdQueryResults);
                }

                //_logger.Information("PrintToTrinh success");
                return result1;
            }
            catch (Exception ex)
            {
                _logger.Information(ex.Message);
                return null;
            }
        }

        // Gửi bảo lãnh
        public string GuiBaoLanh(decimal prKey,decimal pr_key_hsbt_ct, string currentUserEmail, string receiving_emails, string receiving_phones, string? ma_donvi_tt)
        {
            try
            {
                var hsgd_dx_ct = _hsgdCtuRepository.GetHsgdDxCt(pr_key_hsbt_ct);
                var path_bl = "";
                if (hsgd_dx_ct != null)
                {
                    if (hsgd_dx_ct.BlPdbl != 1)
                    {
                        return "Hồ sơ chưa phê duyệt không thể gửi";
                    }
                    if (string.IsNullOrEmpty(hsgd_dx_ct.PathBaolanh))
                    {
                        var result = printPDFBaoLanh(prKey, pr_key_hsbt_ct, currentUserEmail);
                        if (result != null)
                        {
                            string folderUpload = _configuration.GetValue<string>("UploadSettings:FolderUpload") ?? "";
                            string url_upload = _configuration.GetValue<string>("DownloadSettings:UlpoadServer") ?? "";
                            var utilityHelper = new UtilityHelper(_logger);
                            path_bl = utilityHelper.UploadFile_ToAPI(result.Data, ".pdf", folderUpload, url_upload, false);
                            var update_pathfile = _hsgdCtuRepository.UpdatePathBaoLanh(pr_key_hsbt_ct, path_bl);
                            //lấy thông tin ký điện tử
                            var SignContent = _hsgdCtuRepository.GetThongTinKyDienTu(prKey, currentUserEmail);
                            //
                            var chk_ky = _hsgdCtuRepository.KyBaoLanh(prKey, pr_key_hsbt_ct, path_bl, currentUserEmail, SignContent);
                        }
                    }
                    else
                    {
                        path_bl = hsgd_dx_ct.PathBaolanh;
                    }
                }
                else
                {
                    return "Hồ sơ không tồn tại";
                }
                    var path_on_server = _hsgdCtuRepository.GuiBaoLanh(prKey, pr_key_hsbt_ct, path_bl, currentUserEmail, receiving_emails, receiving_phones, ma_donvi_tt).Result;

                    return path_on_server ?? "Có lỗi xảy ra. Hãy thử lại sau.";
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return "Có lỗi xảy ra. Hãy thử lại sau.";
            }
        }
        public bool CreatFileBaoLanh(decimal pr_key_hsgd_ctu,decimal pr_key_hsbt_ct, string email)
        {
            try
            {
                var result = printPDFBaoLanh(pr_key_hsgd_ctu, pr_key_hsbt_ct, email);
                if (result != null)
                {
                    string folderUpload = _configuration.GetValue<string>("UploadSettings:FolderUpload") ?? "";
                    string url_upload = _configuration.GetValue<string>("DownloadSettings:UlpoadServer") ?? "";
                    var utilityHelper = new UtilityHelper(_logger);
                    var file_path = utilityHelper.UploadFile_ToAPI(result.Data, ".pdf", folderUpload, url_upload, false);
                    var update_pathfile = _hsgdCtuRepository.UpdatePathBaoLanh(pr_key_hsbt_ct, file_path);
                    var chk_ky =_hsgdCtuRepository.KyBaoLanh(pr_key_hsgd_ctu, pr_key_hsbt_ct, file_path, email,"");                    
                }
                _logger.Information("CreatFilePasc có pr_key_hsbt_ct =" + pr_key_hsgd_ctu + "success");
                return true;

            }
            catch (Exception ex)
            {
                _logger.Error("CreatFilePasc có pr_key_hsbt_ct =" + pr_key_hsgd_ctu + "Error: " + ex);
                return false;
            }
        }
        
        // khanhlh - 18/11/2024: 
        // Preview
        public DownloadFileResult previewBaoLanh(decimal prKey, decimal pr_key_hsbt_ct, string currentUserEmail, string? ma_donvi_tt)
        {
            try
            {
                DownloadFileResult result1 = null;
                var path_bl = _hsgdCtuRepository.GetFilePathBaoLanh(pr_key_hsbt_ct);
                if (string.IsNullOrEmpty(path_bl))
                {
                    var result = _hsgdCtuRepository.BaoLanh_GetListOfReplacable(prKey, pr_key_hsbt_ct, currentUserEmail, ma_donvi_tt);
                    _logger.Error(result.ToString());

                    var downloadSettings = _configuration.GetSection("DownloadSettings").Get<DownloadSettings>();
                    var word2PdfSettings = _configuration.GetSection("Word2PdfSettings").Get<Word2PdfSettings>();

                    var optionsDownloadSettings = Microsoft.Extensions.Options.Options.Create(downloadSettings);
                    var optionsWord2PdfSettings = Microsoft.Extensions.Options.Options.Create(word2PdfSettings);

                    var contentHelper = new ContentHelper(optionsDownloadSettings, optionsWord2PdfSettings, _logger);


                    if (result != null && result.ThirdQueryResults != null)
                    {
                        result1 = contentHelper.ConvertFileWordToPdf_BaoLanh(result.ThirdQueryResults);
                    }

                    //_logger.Information("PrintToTrinh success");
                    return result1;
                }
                else
                {
                    string url_download = _configuration.GetValue<string>("DownloadSettings:DownloadServer") ?? "";
                    result1 = UtilityHelper.DownloadFile_ToAPI(path_bl, url_download);
                    return result1;
                }
            }
            catch (Exception ex)
            {
                _logger.Information(ex.Message);
                return null;
            }
        }



        public async Task<TraSeri> TracuuPhi(string so_donbh_tracuu, string so_seri_tracuu, int nam_tracuu, int nam_tracuu_goc, string ma_donvi_tracuu)
        {
            TraSeri obj_result = new TraSeri();
            try
            {
                obj_result = await _hsgdCtuRepository.TracuuPhi(so_donbh_tracuu, so_seri_tracuu, nam_tracuu, nam_tracuu_goc, ma_donvi_tracuu);

            }
            catch (Exception ex)
            {
            }
            return obj_result;
        }

        public string GenerateImage(List<string> imagePaths, string tempDir)
        {
            try
            {
                // Upload file bảo lãnh
                var result = _hsgdCtuRepository.GenerateWordDocumentWithImages(imagePaths, tempDir);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return "Error";
            }
        }

        public ReloadSumChecker ReloadSumCheck(int prKey)
        {
            return _hsgdCtuRepository.ReloadSumCheck(prKey);
        }

        public string LoiGiamDinh(int pr_key, string currentUserEmail, LoiGiamDinhRequest request)
        {
            string result = _hsgdCtuRepository.LoiGiamDinh(pr_key, currentUserEmail, request.ThieuAnhGDDK, request.ThuPhiSai, request.SaiDKDK, request.SaiPhanCap, request.TrucLoiBH, request.SaiPhamKhac);
            return result;
        }
        public List<DmUserView> GetListGDV()
        {
            List<DmUserView> obj_result = new List<DmUserView>();
            try
            {
                obj_result = _hsgdCtuRepository.GetListGDV();

            }
            catch (Exception ex)
            {
            }
            return obj_result;
        }

        public async Task<string> GetKbttAnh(string so_hsgd)
        {
            var result = await _hsgdCtuRepository.GetAnhKbttCtu(so_hsgd);
            return result;
        }
        public async Task<string> GetAnhKbttCt(string so_hsgd)
        {
            var result = await _hsgdCtuRepository.GetAnhKbttCt(so_hsgd);
            return result;
        }
        public async Task<string> DeleteHsgdCt (List<decimal> request)
        {
            var result = await _hsgdCtuRepository.DeleteAnhHsgdCt(request);
            return result;
        }
        public async Task<List<DmDkbh>> GetListDkbh(string maSp)
        {
            var result = await _hsgdCtuRepository.GetListDkbh(maSp);
            return result;
        }
        public async Task<bool> KTPquyen_YCBS(decimal pr_key,string email)
        {
            var result = await _hsgdCtuRepository.KTPquyen_YCBS(pr_key,email);
            return result;
        }
        public string PheDuyet_NgoaiPhanCap_12(int pr_key, string currentUserEmail, decimal sum_sotien_hoso, string? ghiChu = "")
        {
            var result = _hsgdCtuRepository.PheDuyet_NgoaiPhanCap_12(pr_key, currentUserEmail, sum_sotien_hoso, ghiChu);
            return result;
        }

        public string TraHS_NgoaiPhanCap_12(int pr_key, string currentUserEmail, string? ghiChu = "")
        {
            var result = _hsgdCtuRepository.TraHS_NgoaiPhanCap_12(pr_key, currentUserEmail, ghiChu);
            return result;
        }

        public string ChuyenTrinh_NgoaiPhanCap(int pr_key, string currentUserEmail, string? ghiChu = "")
        {
            var result = _hsgdCtuRepository.ChuyenTrinh_NgoaiPhanCap(pr_key, currentUserEmail, ghiChu);
            return result;
        }
        public async Task<dynamic> ProcessCRMAsync(int pr_key)
        {
            var result = await _hsgdCtuRepository.ProcessCRMAsync(pr_key);
            return result;
        }
        public HsgdDxCt? GetTTBaoLanh(decimal pr_key_hsgd_dx_ct)
        {
            var result = _hsgdCtuRepository.GetTTBaoLanh(pr_key_hsgd_dx_ct);
            return result;
        }
        public string CheckTrungHsbtUpdate(decimal pr_key_hsgd_ctu)
        {
            var checktrung = _hsgdCtuRepository.CheckTrungHsbtUpdate(pr_key_hsgd_ctu);
            return checktrung;
        }
        public string CheckThoiHanSDBS(string so_donbh,DateTime? ngay_tthat, string so_seri)
        {
            var checktrung = _hsgdCtuRepository.CheckThoiHanSDBS(so_donbh, ngay_tthat, so_seri);
            return checktrung;
        }
        public GetListFileResponse GetListFile(decimal pr_key_hsgd_ctu)
        {
            GetListFileResponse obj_result = new GetListFileResponse();
            try
            {
                obj_result = _hsgdCtuRepository.GetListFile(pr_key_hsgd_ctu);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListFile error: {ex}");
            }
            return obj_result;
        }
        public async Task<string> UploadHoSoTT_MDF1(UploadHoSoTTRequest entity)
        {
            string result = "Thất bại";
            try
            {
                // file_attach_bt
                List<HsgdAttachFile> fileAttach = new List<HsgdAttachFile>();
                for (int i = 0; i < entity.hsgdattachfiles.Count; i++)
                {
                    string finalFilePath = null;

                    // Xử lý upload file từ base64
                    if (!string.IsNullOrEmpty(entity.hsgdattachfiles[i].base64))
                    {
                        string folderUpload = _configuration["UploadSettings:FolderUpload_MDF1"] ?? "";
                        string url_upload = _configuration["DownloadSettings:UlpoadServer_MDF1"] ?? "";
                        var utilityHelper = new UtilityHelper(_logger);

                        // Extract file extension từ FileName
                        string fileExtension = Path.GetExtension(entity.hsgdattachfiles[i].fileName ?? "") ?? ".pdf";

                        // Xác định isImage dựa trên file extension
                        string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".jfif" };
                        bool isImage = imageExtensions.Contains(fileExtension.ToLower());

                        finalFilePath = utilityHelper.UploadFile_ToAPI(entity.hsgdattachfiles[i].base64, fileExtension, folderUpload, url_upload, isImage);

                        if (string.IsNullOrEmpty(finalFilePath))
                        {
                            _logger.Warning($"Failed to upload file from base64: {entity.hsgdattachfiles[i].fileName}");
                            continue;
                        }

                        _logger.Information($"Uploaded file from base64: {entity.hsgdattachfiles[i].fileName} -> {finalFilePath}");
                    }
                    // Sử dụng filePath có sẵn
                    else if (!string.IsNullOrEmpty(entity.hsgdattachfiles[i].filePath))
                    {
                        finalFilePath = entity.hsgdattachfiles[i].filePath;
                        _logger.Information($"Using existing file path: {entity.hsgdattachfiles[i].fileName} -> {finalFilePath}");
                    }
                    else
                    {
                        _logger.Warning($"No base64 or filePath provided for file: {entity.hsgdattachfiles[i].fileName}");
                        continue;
                    }

                    // Nếu có đường dẫn file hợp lệ thì thêm vào danh sách
                    if (!string.IsNullOrEmpty(finalFilePath))
                    {
                        // Check if pr_key exists in HsgdAttachFile table
                        string prKeyToUse;
                        if (!string.IsNullOrEmpty(entity.hsgdattachfiles[i].pr_key))
                        {
                            // Check if pr_key exists in database
                            var existingFile = _hsgdCtuRepository.GetAttachFileByPrKey(entity.hsgdattachfiles[i].pr_key);
                            if (existingFile != null)
                            {
                                // Update existing file
                                prKeyToUse = entity.hsgdattachfiles[i].pr_key;
                            }
                            else
                            {
                                // Generate new GUID
                                prKeyToUse = Guid.NewGuid().ToString();
                            }
                        }
                        else
                        {
                            // Generate new GUID if pr_key is not provided
                            prKeyToUse = Guid.NewGuid().ToString();
                        }

                        HsgdAttachFile file = new HsgdAttachFile();
                        file.Directory = finalFilePath;
                        file.FileName = entity.hsgdattachfiles[i].fileName ?? "";
                        file.PrKey = prKeyToUse;
                        file.FrKey = entity.PrKeyHsgdCtu;
                        file.MaCtu = "HSTT";
                        file.ngay_cnhat = DateTime.Now;
                        file.GhiChu = entity.hsgdattachfiles[i].ghiChu ?? "Upload hồ sơ thanh toán";
                        file.NguonTao = "Web";
                        fileAttach.Add(file);
                    }
                }

                // Lưu files vào database thông qua repository
                if (fileAttach.Any())
                {
                    var uploadResult = await _hsgdCtuRepository.UploadHsgdAttachFiles(fileAttach);
                    if (uploadResult)
                    {
                        result = "Thành công";
                        _logger.Information($"UploadHoSoTT_MDF1: Successfully uploaded {fileAttach.Count} files for PrKeyHsgdCtu = {entity.PrKeyHsgdCtu}");
                    }
                    else
                    {
                        result = "Lỗi khi lưu file";
                        _logger.Error($"UploadHoSoTT_MDF1: Failed to save files to database for PrKeyHsgdCtu = {entity.PrKeyHsgdCtu}");
                    }
                }
                else
                {
                    result = "Không có file nào để upload";
                    _logger.Warning($"UploadHoSoTT_MDF1: No valid files to upload for PrKeyHsgdCtu = {entity.PrKeyHsgdCtu}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"UploadHoSoTT_MDF1 thất bại cho PrKeyHsgdCtu = {entity.PrKeyHsgdCtu}: {ex}");
                result = "Thất bại: " + ex.Message;
            }
            return result;
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
        public DownloadFileResult DownloadHoSoTT_MDF1(string pr_key)
        {
            DownloadFileResult result = new DownloadFileResult();
            try
            {
                var attachFile = _hsgdCtuRepository.GetAttachFileByPrKey(pr_key);
                if (attachFile != null)
                {
                    string pathFile = attachFile.Directory ?? "";
                    if (!string.IsNullOrEmpty(pathFile))
                    {
                        string url_download = LayThongTinMDF(pathFile);

                        if (!string.IsNullOrEmpty(url_download))
                        {
                            result = UtilityHelper.DownloadFile_ToAPI(pathFile, url_download);
                            _logger.Information($"DownloadHoSoTT_MDF1: pr_key = {pr_key}, file = {attachFile.FileName}, pathFile = {pathFile}, url = {url_download}, status = {result.Status}");
                        }
                        else
                        {
                            result.Status = "-404";
                            result.Message = "Cannot determine download server for the given path";
                            _logger.Warning($"DownloadHoSoTT_MDF1: Cannot determine URL for pr_key = {pr_key}, pathFile = {pathFile}");
                        }
                    }
                    else
                    {
                        result.Status = "-404";
                        result.Message = "File path is empty";
                    }
                }
                else
                {
                    result.Status = "-404";
                    result.Message = "Attach file not found";
                }
            }
            catch (Exception ex)
            {
                result.Status = "-500";
                result.Message = "An error occurred during download";
                _logger.Error($"DownloadHoSoTT_MDF1 error for pr_key = {pr_key}: {ex}");
            }
            return result;
        }
        public async Task<string> DeleteAttachFile(string pr_key)
        {
            try
            {
                var result = await _hsgdCtuRepository.DeleteAttachFile(pr_key);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error($"DeleteAttachFile service error for pr_key = {pr_key}: {ex}");
                return "Có lỗi xảy ra khi xóa file";
            }
        }
        public async Task<string> UpdateHoanThienHstt(decimal prKeyHsgdCtu, bool hoanThienHstt, string currentUserEmail)
        {
            try
            {
                var result = await _hsgdCtuRepository.UpdateHoanThienHstt(prKeyHsgdCtu, hoanThienHstt, currentUserEmail);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error($"UpdateHoanThienHstt service error: {ex}");
                return "Có lỗi xảy ra khi cập nhật trạng thái hoàn thiện HSTT";
            }
        }
    }
}