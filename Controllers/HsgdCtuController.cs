using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using PVI.DAO.Entities.Models;
using PVI.Helper;
using PVI.IAM.BE.Services;
using PVI.Repository.Repositories;
using PVI.Service;
using PVI.Service.ActionProcess;
using PVI.Service.Request;
using System.ComponentModel.DataAnnotations;

namespace PVI.API.Web247.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HsgdCtuController : ControllerBase
    {

        private readonly HsgdCtuService _hsgdCtuService;
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;

        public HsgdCtuController(HsgdCtuService hsgdCtuService, Serilog.ILogger logger, IConfiguration configuration)
        {
            _hsgdCtuService = hsgdCtuService;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet("GetCurrentUserInfo")]
        [Authorize]
        public IActionResult GetCurrentUserInfo()
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var entity = _hsgdCtuService.GetCurrentUserInfo(currentUserEmail);
                if (entity == null)
                {
                    _logger.Error("Không tồn tại user " + currentUserEmail);
                    return BadRequest();
                }
                _logger.Information("GET request GetBySoHsgd received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpGet("GetBySoHsgd")]
        [Authorize]
        public async Task<IActionResult> GetBySoHsgd([Required] string so_hsgd)
        {
            try
            {

                var entity = await _hsgdCtuService.GetBySoHsgd(so_hsgd);
                if (entity == null)
                {
                    _logger.Error("Không tồn tại Hsgd số " + so_hsgd);
                    return BadRequest();
                }
                _logger.Information("GET request GetBySoHsgd received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpGet("GetHsgdByPrKey")]
        [Authorize]
        public async Task<IActionResult> GetHsgdByPrKey([Required] decimal pr_key)
        {
            try
            {

                var entity = await _hsgdCtuService.GetHsgdByPrKey(pr_key);
                if (entity == null)
                {
                    _logger.Error("Không tồn tại Hsgd key " + pr_key);
                    return BadRequest();
                }
                _logger.Information("GET request GetBySoHsgd received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListHsgdDx")]
        [Authorize]
        public IActionResult GetListHsgdDx([Required] string so_hsgd)
        {
            try
            {

                var entity = _hsgdCtuService.GetListHsgdDx(so_hsgd);
                //if (entity == null)
                //{
                //    _logger.Error("Không có dữ liệu");
                //    return BadRequest();
                //}
                _logger.Information("GET request GetListHsgdDx received");
                return Ok(entity);

                
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListHsgdDxNew")]
        [Authorize]
        public IActionResult GetListHsgdDxNew([Required] string so_hsgd)
        {
            try
            {

                var entity = _hsgdCtuService.GetListHsgdDxNew(so_hsgd);
                //if (entity == null)
                //{
                //    _logger.Error("Không có dữ liệu");
                //    return BadRequest();
                //}
                _logger.Information("GET request GetListHsgdDxNew received");
                return Ok(entity);


            }
            catch (Exception ex)
            {
                _logger.Error($"GetListHsgdDxNew An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListNguyenNhanTonThat")]
        [Authorize]
        public async Task<IActionResult> GetListNguyenNhanTonThat()
        {
            try
            {
                var entities = await _hsgdCtuService.GetListNguyenNhanTonThat();
                _logger.Information("GetListNguyenNhanTonThat success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListDiaDiemTonThat")]
        [Authorize]
        public async Task<IActionResult> GetListDiaDiemTonThat()
        {
            try
            {
                var entities = await _hsgdCtuService.GetListDiaDiemTonThat();
                _logger.Information("GetListDiaDiemTonThat success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListSanPham")]
        [Authorize]
        public async Task<IActionResult> GetListSanPham()
        {
            try
            {
                var entities = await _hsgdCtuService.GetListSanPham();
                _logger.Information("GetListSanPham success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListTrangThai")]
        [Authorize]
        public async Task<IActionResult> GetListTrangThai()
        {
            try
            {
                var entities = await _hsgdCtuService.GetListTrangThai();
                _logger.Information("GetListTrangThai success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetCountByStatus")]
        [Authorize]
        public IActionResult GetCountByStatus(int dulieu_nam)
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //var email = "quyenvm@pvi.com.vn";
                var entity = _hsgdCtuService.GetCountByStatus(dulieu_nam, email);
                _logger.Information("GET request GetCountByStatus received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetCountByStatus An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("{pr_key}")]
        [Authorize]
        public async Task<IActionResult> GetData_Detail_Hsgd(decimal pr_key)
        {
            try
            {

                var entity = await _hsgdCtuService.GetData_Detail_Hsgd(pr_key);
                if (entity == null)
                {
                    _logger.Error("Không tồn tại HSGD");
                    return BadRequest();
                }
                _logger.Information("GET request GetData_Detail_Hsgd received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetData_Detail_Hsgd An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }

        // Lấy danh sách các trạng thái hồ sơ giám định 
        [HttpGet("GetListStatusName")]
        [Authorize]
        public async Task<IActionResult> GetListStatusName()
        {
            try
            {
                var entities = await _hsgdCtuService.GetListStatusName();
                _logger.Information("GetListStatusName success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        // Lấy danh sách tất cả các loại hồ sơ.
        [HttpGet("GetListTypeName")]
        [Authorize]
        public async Task<IActionResult> GetListTypeName()
        {
            try
            {
                var entities = await _hsgdCtuService.GetListTypeName();
                _logger.Information("GetListTypeName success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        // Lấy danh sách tổng thành xe.
        // khanhlh - 09/05/2024

        [HttpGet("GetListTongThanhXe")]
        [Authorize]
        public IActionResult GetListTongThanhXe()
        {
            try
            {
                var entities = _hsgdCtuService.getListTongThanhXe();
                _logger.Information("GetListCategoryHmuc success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }


        // Lấy danh sách các hạng mục.
        // khanhlh - 09/05/2024

        [HttpGet("GetListNHmuc")]
        [Authorize]
        public IActionResult getListNHmuc()
        {
            try
            {
                var entities = _hsgdCtuService.getListNhmuc();
                _logger.Information("GetListCategoryHmuc success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpGet("GetListHmuc")]
        [Authorize]
        public IActionResult getListHmuc(string? ma_tongthanhxe = null, string? ma_nhmuc = null)
        {
            try
            {
                var entities = _hsgdCtuService.getListHmuc(ma_tongthanhxe, ma_nhmuc);
                _logger.Information("GetListHmuc success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpGet("GetListHmucGiamDinh")]
        [Authorize]
        public IActionResult getListHmucGiamDinh()
        {
            try
            {
                var entities = _hsgdCtuService.getListHmucGiamDinh();
                _logger.Information("GetListHmucGiamDinh success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpPost("GetList")]
        [Authorize]
        public async Task<IActionResult> GetList(HsgdCtuParameters parameters)
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //var email = "quyenvm@pvi.com.vn";
                var result = await _hsgdCtuService.GetList(parameters,email);
               
                var metadata = new
                {
                    
                    result.TotalCount,
                    result.PageSize,
                    result.CurrentPage,
                    result.TotalPages,
                    result.HasNext,
                    result.HasPrevious
                };


                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.Information("GetList success");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpGet("GetListDiary")]
        [Authorize]
        public async Task<IActionResult> getListDiary(int pr_key, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                DiaryResponse entities = await _hsgdCtuService.GetListDiary(pr_key, pageNumber, pageSize);
                _logger.Information("GetListDiary success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListAppraisalImage")]
        [Authorize]
        public async Task<IActionResult> getListAppraisalImage(int pr_key)
        {
            try
            {
                var entities = await _hsgdCtuService.GetListAppraisalImage(pr_key);
                if (entities != null)
                {
                    _logger.Information("GetListAppraisalImage success");
                    return Ok(entities);
                }
                else
                {
                    
                    return BadRequest("An error occured");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpPut("UpdateTrangThaiHsgdCtu")]
        [Authorize]

        public IActionResult UpdateDetailFile(int pr_key, [FromBody][Required] int ma_trang_thai)
        {
            try
            {
                string result = _hsgdCtuService.updateDetailFile(pr_key, ma_trang_thai);
                _logger.Information("Request UpdateDetailFile has been received.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpPut("UpdateHsgdCtu")]
        [Authorize]
        public IActionResult updateDetail(int pr_key, [FromBody] HsgdCtuRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                else
                {
                    var result = _hsgdCtuService.updateDetail(pr_key, entity);
                    _logger.Information("PUT request UpdateHsgdCtu received");
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                _logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }

        // Gán giám định viên.

        [HttpPut("GanGiamDinh")]
        [Authorize]

        public async Task<IActionResult> GanGiamDinh(int pr_key, [FromBody]HsgdGanGiamDinh gdd)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                string result = await _hsgdCtuService.assignAppraisal(pr_key, gdd, currentUserEmail);
                _logger.Information("Request GanGiamDinh has been received.");
                try
                {
                    var parse = Decimal.Parse(result);
                    return Ok(result);
                }
                catch
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        // Gán cán bộ tiếp nhận / người duyệt hồ sơ

        [HttpPut("GanNguoiTiepNhan")]
        [Authorize]

        public IActionResult GanNguoiTiepNhan(int pr_key, [FromBody] HsgdGanNguoiDuyet ganDuyet)
        {
            try
            {
                string currentUSerEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                string result =  _hsgdCtuService.GanNguoiTiepNhan(pr_key, ganDuyet.ghiChu, ganDuyet.oidCanBoPheDuyet, currentUSerEmail);
                _logger.Information("Request GanNguoiTiepNhan has been received.");
                try
                {
                    var parse = Decimal.Parse(result);
                    return Ok(result);
                }
                catch
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        // Yêu cầu bổ sung thông tin

        [HttpPut("ChuyenChoPheDuyet")]
        [Authorize]

        public async Task<IActionResult> ChuyenChoPheDuyet(int pr_key, [FromBody] HsgdCtuChoPheDuyet cpd)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                string result = await _hsgdCtuService.requestApproval(pr_key, cpd, currentUserEmail);
                try
                {
                    Decimal.Parse(result);
                    _logger.Information("Request ChuyenChoPheDuyet has been received.");
                    return Ok(result);
                } catch
                {
                    //return BadRequest("An error occured");
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        // Yêu cầu bổ sung thông tin

        [HttpPut("YeuCauBoSungTT")]
        [Authorize]

        public async Task<IActionResult> BoSungTT(int pr_key, [FromBody] HsgdCtuBoSungThongTin bstt)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                string result = await _hsgdCtuService.requestAdditionalDetail(pr_key, bstt, currentUserEmail);
             
                try
                {
                    Decimal.Parse(result);
                    _logger.Information("Request YeuCauBoSungTT has been received.");
                    return Ok(result);
                }
                catch
                {
                    //return BadRequest("An error occured");
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        
        [HttpPut("PheDuyetHoSo")]
        [Authorize]

        public IActionResult PheDuyetHoSo(int pr_key, [FromBody] string ghiChu)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                string result = _hsgdCtuService.approveAppraisal(pr_key, ghiChu, currentUserEmail);
             
                try
                {
                    Decimal.Parse(result);
                    _logger.Information("Request Phe Duyet Ho So has been received.");
                    return Ok(result);
                }
                catch
                {
                    //return BadRequest("An error occured");
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpGet("GetListDonViThanhToan")]
        [Authorize]
        public IActionResult GetListDonviTT()
        {
            try
            {
                var entities = _hsgdCtuService.GetListDonViTT();
                _logger.Information("GetListDonViThanhToan success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }


        [HttpGet("GetInfoDonviTT")]
        [Authorize]
        public IActionResult GetInfoDonViTT(string ma_don_vi)
        {
            try
            {
                var entities = _hsgdCtuService.GetInfoDonviTT(ma_don_vi);
                //_logger.Information("GetListDonViThanhToan success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                //_logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }


        [HttpPost("PheDuyetBaoLanh")]
        [Authorize]
        public async Task<IActionResult> PheDuyetBaoLanh(int pr_key, [FromBody] PheDuyetBaoLanhRequest pdbl)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var entities = await _hsgdCtuService.pheDuyetBaoLanh(pr_key, pdbl, currentUserEmail);
                try
                {
                    Decimal.Parse(entities);
                    _logger.Information("PheDuyetBaoLanh success");
                    return Ok(entities);
                } catch (Exception err)
                {
                    //return BadRequest("An error occured");
                    return BadRequest(entities);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpGet("PrintBaoLanh")]
        [Authorize]
        public IActionResult PrintPDFBaoLanh(int pr_key)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //string currentUserEmail = "khanhlh@pvi.com.vn";
                var entities = _hsgdCtuService.printPDFBaoLanh(pr_key, currentUserEmail);
                if (entities != null)
                {
                    return Ok(entities);
                }
                else
                {
                    return BadRequest("Không thể view bảo lãnh thiếu thông tin");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpGet("GuiBaoLanh")]
        [Authorize]
        public IActionResult GuiBaoLanh(int pr_key, bool sendEmail, bool sendSMS)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var entities = _hsgdCtuService.GuiBaoLanh(pr_key, sendEmail, sendSMS, currentUserEmail);
                try
                {
                    Decimal.Parse(entities);
                    return Ok(entities);
                } catch (Exception err)
                {
                    _logger.Error($"An error occured: {err} {entities}");
                    return BadRequest(entities);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpPost("UploadImage")]
        [Authorize]

        public async Task<IActionResult> UploadImage(UploadImageRequest request)
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var oid =  _hsgdCtuService.GetOidByEmail(email);
                string result = await _hsgdCtuService.UploadAppraisalImage(request.File,request.PrKey,oid,request.Stt,request.MaHmuc,request.DienGiai,request.MaHmucSc);
                _logger.Information("Request UploadImage has been received.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetAnhGDDK")]
        [Authorize]

        public async Task<IActionResult> GetAnhGDDK(int pr_key)
        {
            try
            {
                var result = await _hsgdCtuService.GetAnhGDDK(pr_key);
                _logger.Information("Request GetAnhGDDK has been received.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpPut("UpdateAppraisalImage")]
        [Authorize]

        public async Task<IActionResult> UpdateAppraisalImage(List<UpdateImageRequest> request)
        {
            try
            {
                var result = await _hsgdCtuService.UpdateAppraisalImage(request);
                _logger.Information("Request UpdateAppraisalImage has been received.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpGet("GetAnhDuyetGia")]
        [Authorize]
        public async Task<IActionResult> GetAnhDuyetGia(int pr_key,bool loai_dg)
        {
            try
            {
                var entities = await _hsgdCtuService.GetListAnhDuyetGia(pr_key,loai_dg);
                if (entities != null)
                {
                    _logger.Information("GetAnhDuyetGia success");
                    return Ok(entities);
                }
                else
                {
                    return Ok(new List<HsgdCt> { });
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpGet("GetInfoDuyetGia")]
        [Authorize]
        public async Task<IActionResult> GetInfoDuyetGia(int pr_key)
        {
            try
            {
                var entities = await _hsgdCtuService.GetThongTinDuyetGia(pr_key);
                if (entities != null)
                {
                    _logger.Information("GetInfoDuyetGia success");
                    return Ok(entities);
                }
                else
                {
                    return Ok(entities);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        //[HttpPost("DownloadImage")]
        //[Authorize]

        //public IActionResult DownloadImage(int pr_key)
        //{
        //    try
        //    {
        //        var result =  _hsgdCtuService.DownloadTtrinh11_MDF1(pr_key);
        //        _logger.Information("Request DownloadImage has been received.");
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error($"An error occured: {ex}");
        //        return BadRequest("An error occurred");
        //    }
        //}



        [HttpGet("GetListTtrangGd")]
        [Authorize]
        public IActionResult getListTtrangGd()
        {
            try
            {
                var entities = _hsgdCtuService.getListTtrangGd();
                _logger.Information("GetListTTrangGd success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }


        [HttpGet("GetListTienTe")]
        [Authorize]
        public IActionResult GetListTte()
        {
            try
            {
                var entities = _hsgdCtuService.getListTienTe();
                _logger.Information("GetListTienTe success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpGet("GetListTyGia")]
        [Authorize]
        public IActionResult GetListTyGia()
        {
            try
            {
                var entities = _hsgdCtuService.GetListTyGia();
                _logger.Information("GetListTyGia success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }


        [HttpGet("GetListHieuxe")]
        [Authorize]
        public IActionResult GetListHieuxe()
        {
            try
            {
                var entities = _hsgdCtuService.getListHieuxe();
                _logger.Information("GetListHieuxe success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpGet("GetListLoaiXe")]
        [Authorize]
        public IActionResult GetListLoaixe(int prKeyHieuXe)
        {
            try
            {
                var entities = _hsgdCtuService.getListLoaixe(prKeyHieuXe);
                _logger.Information("GetListLoaiXe success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpGet("GetListLoaiChiPhi")]
        [Authorize]
        public IActionResult GetListLoaiChiPhi()
        {
            try
            {
                var entities = _hsgdCtuService.getListLoaiChiPhi();
                _logger.Information("GetListLoaiChiPhi success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        
        [HttpPost("ChuyenAnhGDTT")]
        [Authorize]
        public async Task<IActionResult> ChuyenAnhGdtt(ChuyenAnhGdttRequest request )
        {
            try
            {
                var entities = await _hsgdCtuService.ChuyenAnhGDTT(request.SoHsgdChuyen,request.SoHsgdNhan);
                _logger.Information("ChuyenAnhGdtt success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpPost("UploadAnhDuyetGia")]
        [Authorize]

        public async Task<IActionResult> UploadAnhDuyetGia(UploadAnhDuyetGiaRequest request)
        {
            try
            {
                
                string result = await _hsgdCtuService.UploadAnhDuyetgia(request.File, request.PrKey, request.LoaiDg);
                _logger.Information("Request UploadAnhDuyetGia has been received.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }


        [HttpPost("UpdateAnhDuyetGia")]
        [Authorize]

        public async Task<IActionResult> UpdateAnhDuyetGia(UpdateDuyetGiaRequest request)
        {
            try
            {

                string result = await _hsgdCtuService.UpdateAnhDuyetGia(request);
                _logger.Information("Request UpdateAnhDuyetGia has been received.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }


        [HttpPost("UploadSampleFile")]
        //[Authorize]

        public IActionResult UploadSampleFile ([FromBody]string localPath)
        {
            try
            {
                
                string result = _hsgdCtuService.UploadSampleFile(localPath);
                //_logger.Information("Request UploadsampleFile has been received.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListDmLdonBt")]
        [Authorize]
        public IActionResult GetListDmLdonBt()
        {
            try
            {
                var entities = _hsgdCtuService.GetDmLdonBt();
                _logger.Information("GetListDmLdonBt success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpGet("GetProductInfo")]
        [Authorize]
        public async Task<IActionResult> GetProductInfo(int prKey)
        {
            try
            {
                
                var soDonBh = await _hsgdCtuService.GetSoDonBh(prKey);
                var soDonBhbs = "";
                var entities = await _hsgdCtuService.GetProductInfo(soDonBh,soDonBhbs);
                _logger.Information("GetListDmLdonBt success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListLoaiBang")]
        [Authorize]
        public async Task<IActionResult> GetListLoaiBang()
        {
            try
            {
                var entities = await _hsgdCtuService.GetListLoaiBang();
                _logger.Information("GetListLoaiBang success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListLoaiBang An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

    }
}