using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Office.Interop.Word;
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
using System.Net.Http;

namespace PVI.API.Web247.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HsgdCtuController : ControllerBase
    {

        private readonly HsgdCtuService _hsgdCtuService;
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly HsgdTtrinhService _hsgdTtrinhService;
        private readonly HsgdTtrinhController _hsgd_ttrinh_ctro;
        private readonly HsgdDxService _hsgdDxService;
        private readonly HsgdDxController _hsgd_dx_ctro;
        public HsgdCtuController(HsgdCtuService hsgdCtuService, HsgdTtrinhService hsgdTtrinhService, HsgdDxService hsgdDxService, Serilog.ILogger logger, IConfiguration configuration, HttpClient httpClient)
        {
            _hsgdCtuService = hsgdCtuService;
            _hsgdTtrinhService = hsgdTtrinhService;
            _hsgdDxService = hsgdDxService;
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
            _hsgd_ttrinh_ctro = new HsgdTtrinhController(_hsgdTtrinhService,logger,configuration);
            _hsgd_dx_ctro = new HsgdDxController(_hsgdDxService, logger, configuration);
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
        //[Authorize]
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
        public IActionResult GetCountByStatus(string fromDate, string toDate)
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //var email = "quyenvm@pvi.com.vn";
                var entity = _hsgdCtuService.GetCountByStatus(fromDate,toDate, email,"0501");
                _logger.Information("GET request GetCountByStatus received");
                if (entity != null)
                {
                    return Ok(entity);
                }
                else
                {
                    return BadRequest("Không có dữ liệu");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"GetCountByStatus An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetCountByStatusXM")]
        [Authorize]
        public IActionResult GetCountByStatusXM(string fromDate,string toDate)
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //var email = "quyenvm@pvi.com.vn";
                var entity = _hsgdCtuService.GetCountByStatus(fromDate,toDate, email, "0502");
                _logger.Information("GET request GetCountByStatus received");
                if (entity != null)
                {
                    return Ok(entity);
                }
                else
                {
                    return BadRequest("Không có dữ liệu");
                }
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
        [HttpGet("ReadOCR")]
        [Authorize]
        public IActionResult ReadOCR(decimal pr_key_hsgd)
        {
            try
            {

                var entity = _hsgdCtuService.ReadOCR(pr_key_hsgd);
                _logger.Information("GET request ReadOCR received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"ReadOCR An error occurred: {ex}");
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
                var result = await _hsgdCtuService.GetList(parameters, email,"0501");

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
        [HttpPost("GetListHsgdXM")]
        [Authorize]
        public async Task<IActionResult> GetListHsgdXM(HsgdCtuParameters parameters)
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //var email = "quyenvm@pvi.com.vn";
                var result = await _hsgdCtuService.GetList(parameters, email,"0502");

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
                _logger.Information("GetListHsgdXM success");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListHsgdXM An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpGet("GetDetailBySoHsgd")]
        [Authorize]
        public async Task<IActionResult> GetDetailBySoHsgd(string soHsgd)
        {

            try
            {

                var entity = await _hsgdCtuService.GetDataDetailBySoHsgd(soHsgd);
                if (entity == null)
                {
                    _logger.Error("Không tồn tại HSGD");
                    return BadRequest();
                }
                _logger.Information("GET request GetDetailBySoHsgd received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetDetailBySoHsgd An error occurred: {ex}");
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
        [HttpGet("GetListAppraisalImage_BVN")]
        public async Task<IActionResult> getListAppraisalImage_BVN(int pr_key)
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

        public IActionResult UpdateDetailFile(int pr_key, int type)
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                string result = _hsgdCtuService.updateDetailFile(pr_key, type, email);
                _logger.Information("Request UpdateDetailFile has been received.");
                try
                {
                    Int64.Parse(result);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured when huy ho so: {ex.Message}");
                return BadRequest(ex.Message);
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
                    if (entity.HsgdCtuUdpdate.SoLanBt < 0)
                    {
                        return BadRequest("Số lần bồi thường không được nhập số âm");
                    }
                    if (entity.HsgdCtuUdpdate.SoTienThucTe < 0)
                    {
                        return BadRequest("Số tiền thực tế không được nhập số âm");
                    }
                    if (entity.HsgdCtuUdpdate.DangKiem < 0 || entity.HsgdCtuUdpdate.NamSinh < 0)
                    {
                        return BadRequest();
                    }
                    if (entity.HsgdCtuUdpdate.NgayTthat == null)
                    {
                        return BadRequest("Chưa nhập ngày tổn thất! Hãy nhập lại.");
                    }
                    if (entity.so_seri == null)
                    {
                        return BadRequest("Thiếu số seri! Hãy kiếm tra lại.");
                    }
                    var hsgd_ctu = _hsgdCtuService.GetHsgdByPrKey(pr_key);
                    if (entity.HsgdCtuUdpdate.NgayTthat < hsgd_ctu.Result.NgayDauSeri || entity.HsgdCtuUdpdate.NgayTthat > hsgd_ctu.Result.NgayCuoiSeri)
                    {
                        return BadRequest("Ngày tổn thất phải trong thời hạn bảo hiểm! Hãy nhập lại.");
                    }
                    if (entity.HsgdCtuUdpdate.NgayTthat > DateTime.Now)
                    {
                        return BadRequest("Ngày tổn thất không được lớn hơn ngày hiện tại! Hãy nhập lại.");
                    }
                    if (entity.HsgdCtuUdpdate.NgayTbao == null)
                    {
                        return BadRequest("Chưa nhập ngày thông báo! Hãy nhập lại.");
                    }
                    if (entity.HsgdCtuUdpdate.NgayTbao < entity.HsgdCtuUdpdate.NgayTthat)
                    {
                        return BadRequest("Ngày thông báo không được nhỏ hơn ngày tổn thất! Hãy nhập lại");
                    }
                    if (entity.HsgdCtuUdpdate.NgayTbao > DateTime.Now)
                    {
                        return BadRequest("Ngày thông báo không được lớn hơn ngày hiện tại! Hãy nhập lại.");
                    }
                    if (!string.IsNullOrEmpty(entity.HsgdCtuUdpdate.NguyenNhanTtat) && entity.HsgdCtuUdpdate.NguyenNhanTtat.Length > 250)
                    {
                        return BadRequest("Nguyên nhân tổn thất không được nhập quá 250 ký tự!");
                    }
                    if (entity.HsgdCtuUdpdate.NgayTthat < entity.HsgdCtuUdpdate.NgayDauLaixe || entity.HsgdCtuUdpdate.NgayTthat > entity.HsgdCtuUdpdate.NgayCuoiLaixe)
                    {
                        return BadRequest("Ngày xảy ra tai nạn phải trong thời hạn cấp bằng lại xe! Hãy nhập lại");
                    }
                    if (entity.HsgdCtuUdpdate.NgayTthat < entity.HsgdCtuUdpdate.NgayDauLuuhanh || entity.HsgdCtuUdpdate.NgayTthat > entity.HsgdCtuUdpdate.NgayCuoiLuuhanh)
                    {
                        return BadRequest("Ngày xảy ra tai nạn phải trong thời hạn đăng kiểm! Hãy nhập lại");
                    }
                    if (hsgd_ctu.Result.PrKeyBt > 0)
                    {
                        var checktrung = _hsgdCtuService.CheckTrungHsbtUpdate(pr_key);
                        if (!string.IsNullOrEmpty(checktrung))
                        {
                            return BadRequest(checktrung);
                        }
                    }
                    var check_thoihan_sdbs = _hsgdCtuService.CheckThoiHanSDBS(hsgd_ctu.Result.SoDonbh, entity.HsgdCtuUdpdate.NgayTthat, entity.so_seri);
                    if (!string.IsNullOrEmpty(check_thoihan_sdbs))
                    {
                        return BadRequest(check_thoihan_sdbs);
                    }
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
        [HttpPut("LoiGiamDinh")]
        [Authorize]
        public IActionResult LoiGiamDinh(int pr_key, [FromBody] LoiGiamDinhRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                else
                {
                    if (entity.ThieuAnhGDDK < 0 || entity.ThuPhiSai < 0 || entity.SaiDKDK < 0 || entity.SaiPhanCap < 0 || entity.TrucLoiBH < 0 || entity.SaiPhamKhac < 0)
                    {
                        return BadRequest();
                    }
                    var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                    string result = _hsgdCtuService.LoiGiamDinh(pr_key, email, entity);
                    _logger.Information("PUT request UpdateHsgdCtu received");
                    try
                    {
                        long prKey = Int64.Parse(result);
                        return Ok(result);
                    }
                    catch (Exception ex)
                    {
                        return Ok(result);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured at Loi Giam Dinh: {ex}");
                _logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("Loi Tich Chon Giam Dinh");
            }
        }



        // Gán giám định viên.

        [HttpPut("GanGiamDinh")]
        [Authorize]

        public async Task<IActionResult> GanGiamDinh(int pr_key, [FromBody] HsgdGanGiamDinh gdd)
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
                return BadRequest("Có lỗi xảy ra, vui lòng liên hệ IT. Mã Lỗi: Exception_At_GANGIAMDINH");
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
                string result = _hsgdCtuService.GanNguoiTiepNhan(pr_key, ganDuyet.ghiChu, ganDuyet.oidCanBoPheDuyet, currentUSerEmail);
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
                return BadRequest("Có lỗi xảy ra, vui lòng liên hệ IT. Mã Lỗi: Exception_At_GANNGUOINHAN");
            }
        }

        // Gán cán bộ tiếp nhận / người duyệt hồ sơ

        [HttpPut("ChuyenGanHoSo")]
        [Authorize]

        public IActionResult ChuyenGanHoSo(int pr_key, [FromBody] HsgdGanNguoiDuyet ganDuyet)
        {
            try
            {
                string currentUSerEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                string result = _hsgdCtuService.ChuyenGanHoSo(pr_key, ganDuyet.ghiChu, ganDuyet.oidCanBoPheDuyet, currentUSerEmail);
                _logger.Information("Request ChuyenGanHoSo has been received.");
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
                _logger.Error($"Chuyen Gan Ho So error occured: {ex}");
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
                return BadRequest("Có sản phẩm chưa nhập đề xuất PASC. Nếu tất cả các sản phẩm đã nhập và vẫn lỗi, vui lòng liên hệ IT");
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
                return BadRequest("Có lỗi xảy ra, vui lòng liên hệ IT. Mã Lỗi: Exception_At_BSTT");
            }
        }


        [HttpPut("PheDuyetHoSo")]
        [Authorize]

        public IActionResult PheDuyetHoSo(int pr_key, [FromBody] string ghiChu)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //string currentUserEmail = "hoinv@pvi.com.vn";
                string result = _hsgdCtuService.approveAppraisal(pr_key, ghiChu, currentUserEmail);              
                try
                {
                    Decimal.Parse(result);
                    // check trạng thái 06 đã duyệt thì tạo đường dẫn file pasc
                    _hsgd_dx_ctro.CreatFilePasc(pr_key, currentUserEmail);
                    //
                    var check_tpc = _hsgd_ttrinh_ctro.CheckHsgdTPC(pr_key, "12", currentUserEmail);
                    if (check_tpc)
                    {
                        _hsgd_ttrinh_ctro.PheDuyetHsTpc(pr_key, currentUserEmail);
                    }
                    check_tpc = _hsgd_ttrinh_ctro.CheckHsgdTPC(pr_key, "6", currentUserEmail);
                    if (check_tpc)
                    {
                        _hsgd_ttrinh_ctro.KyHoSoTPC(pr_key, currentUserEmail);
                    }
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
                return BadRequest("Có sản phẩm chưa nhập đề xuất PASC. Nếu tất cả các sản phẩm đã nhập và vẫn lỗi, vui lòng liên hệ IT");
            }
        }
        [HttpPut("Baogia_giamdinh")]
        [Authorize]
        public IActionResult Baogia_giamdinh([FromBody] baogia_request hsgddg_Request)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);               
                string result = _hsgdCtuService.Baogia_giamdinh(hsgddg_Request, currentUserEmail);                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("Có sản phẩm chưa nhập đề xuất PASC. Nếu tất cả các sản phẩm đã nhập và vẫn lỗi, vui lòng liên hệ IT");
            }
        }
        [HttpPut("Duyetgia_giamdinh")]
        [Authorize]
        public IActionResult Duyetgia_giamdinh([FromBody] duyetgia_request hsgddg_Request)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                string result = _hsgdCtuService.Duyetgia_giamdinh(hsgddg_Request, currentUserEmail);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("Có sản phẩm chưa nhập đề xuất PASC. Nếu tất cả các sản phẩm đã nhập và vẫn lỗi, vui lòng liên hệ IT");
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
        [HttpGet("Kiemtra_uynhiemchi")]
        [Authorize]
        public IActionResult Kiemtra_uynhiemchi(string ma_donvi)
        {
            try
            {
                var entities = _hsgdCtuService.Kiemtra_uynhiemchi(ma_donvi);                
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
        public async Task<IActionResult> PheDuyetBaoLanh(int pr_key,decimal pr_key_hsbt_ct, [FromBody] PheDuyetBaoLanhRequest pdbl)
        {
            try
            {
                if (pdbl == null)
                {
                    return BadRequest();
                }
                else
                {
                    if (pdbl.bl1 < 0 || pdbl.bl2 < 0 || pdbl.bl3 < 0 || pdbl.bl4 < 0 || pdbl.bl5 < 0 || pdbl.bl6 < 0 || pdbl.bl7 < 0 ||  pdbl.bl8 < 0 || pdbl.bl9 < 0)
                    {
                        return BadRequest();
                    }
                }
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var entities = await _hsgdCtuService.pheDuyetBaoLanh(pr_key, pr_key_hsbt_ct, pdbl, currentUserEmail);
                try
                {
                    Decimal.Parse(entities);
                    // phê duyệt bảo lãnh thành công thì tạo đường dẫn file bảo lãnh
                   _hsgdCtuService.CreatFileBaoLanh(pr_key, pr_key_hsbt_ct, currentUserEmail);
                    //
                    _logger.Information("PheDuyetBaoLanh success");
                    return Ok(entities);
                }
                catch (Exception err)
                {
                    //return BadRequest("An error occured");
                    return BadRequest(entities);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured at Phe Duyet Bao Lanh: {ex}");
                return BadRequest("Có lỗi xảy ra, vui lòng liên hệ IT. Mã Lỗi: Exception_At_DUYETBAOLANH");
            }
        }


        [HttpGet("PrintBaoLanh")]
        [Authorize]
        public IActionResult PrintBaoLanh(int pr_key,decimal pr_key_hsbt_ct, string? ma_donvi_tt = null)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //string currentUserEmail = "khanhlh@pvi.com.vn";
                var entities = _hsgdCtuService.previewBaoLanh(pr_key, pr_key_hsbt_ct, currentUserEmail, ma_donvi_tt);
                if (entities != null)
                {
                    return Ok(entities);
                }
                else
                {
                    //
                    return BadRequest("Lỗi khi tạo bảo lãnh");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("Có lỗi xảy ra, vui lòng liên hệ IT. Mã Lỗi: Exception_At_PRINTBL");
            }
        }

        [HttpPost("GuiBaoLanh")]
        [Authorize]
        public IActionResult GuiBaoLanh(int pr_key,decimal pr_key_hsbt_ct, [FromBody] GuiBaolanhRequest request, string? ma_donvi_tt = null)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var entities = _hsgdCtuService.GuiBaoLanh(pr_key, pr_key_hsbt_ct, currentUserEmail, request.receiving_emails, request.receiving_phones, ma_donvi_tt);
                if (entities.Equals("Gửi bảo lãnh thành công"))
                {
                    return Ok(entities);
                }
                else
                {
                    return BadRequest(entities);
                }
                //try
                //{
                //    Decimal.Parse(entities);
                //    return Ok(entities);
                //}
                //catch (Exception err)
                //{
                //    _logger.Error($"An error occured: {err} {entities}");
                //    return BadRequest(entities);
                //}
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("Có lỗi xảy ra, vui lòng liên hệ IT. Mã Lỗi: Exception_At_GUIBL");
            }
        }
        
        [HttpPost("LuuThongbaoBT")]
        [Authorize]
        public async Task<IActionResult> LuuThongbaoBT([FromBody] LuuThongBaoBTRequest LuuThongBaoBT)
        {
            try
            {
                if (LuuThongBaoBT == null)
                {
                    return BadRequest();
                }
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var entities = await _hsgdCtuService.LuuThongbaoBT(LuuThongBaoBT, currentUserEmail); 
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured at Phe Duyet Bao Lanh: {ex}");
                return BadRequest("Có lỗi xảy ra, vui lòng liên hệ IT. Mã Lỗi: Exception_At_DUYETBAOLANH");
            }
        }
        [HttpGet("LayThongbaoBT")]
        [Authorize]
        public async Task<ActionResult<LuuThongBaoBTResponse>> LayThongbaoBT(int pr_key_hsgd)
        {
            try
            {   
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var entities = await _hsgdCtuService.LayThongbaoBT(pr_key_hsgd, currentUserEmail);
                if (entities == null)
                    return NotFound($"Không tìm thấy thông báo BT với PrKeyHsgd = {pr_key_hsgd}");

                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured at Phe Duyet Bao Lanh: {ex}");
                return BadRequest("Có lỗi xảy ra, vui lòng liên hệ IT. Mã Lỗi: Exception_At_DUYETBAOLANH");
            }
        }

        [HttpPost("UploadImage")]
        [Authorize]
        public async Task<IActionResult> UploadImage(UploadImageRequest request)
        {
            try
            {
                if (request.Stt < 0)
                {
                    return BadRequest();
                }
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var oid = _hsgdCtuService.GetOidByEmail(email);
                string result = await _hsgdCtuService.UploadAppraisalImage(request.File, request.PrKey, oid, request.Stt, request.MaHmuc, request.DienGiai, request.MaHmucSc);
                _logger.Information("Request UploadImage has been received.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"UploadImage An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }


        [HttpPost("UpdateImage")]
        [Authorize]

        public async Task<IActionResult> UpdateImage(UpdateURLImageRequest request)
        {
            try
            {
                if (request.Stt < 0)
                {
                    return BadRequest();
                }
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var oid = _hsgdCtuService.GetOidByEmail(email);
                string result = await _hsgdCtuService.UpdateURLImage(request.File,request.PrKeyCt, request.PrKey, oid);
                _logger.Information("Request UpdateImage has been received.");
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
                if (request == null)
                {
                    return BadRequest();
                }
                else
                {
                    if (request.Where(x => x.Stt < 0).Count() > 0)
                    {
                        return BadRequest();
                    }
                }
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
        public async Task<IActionResult> GetAnhDuyetGia(int pr_key, bool loai_dg)
        {
            try
            {
                var entities = await _hsgdCtuService.GetListAnhDuyetGia(pr_key, loai_dg);
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
        public async Task<IActionResult> ChuyenAnhGdtt(ChuyenAnhGdttRequest request)
        {
            try
            {
                var entities = await _hsgdCtuService.ChuyenAnhGDTT(request.SoHsgdChuyen, request.SoHsgdNhan);
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


        [HttpPost("UpdateURLAnhDuyetGia")]
        [Authorize]

        public async Task<IActionResult> UpdateURLAnhDuyetGia(UpdateURLAnhDuyetGiaRequest request)
        {
            try
            {

                string result = await _hsgdCtuService.UpdateURLAnhDuyetgia(request.File, request.PrKey, request.PrKeyDgCt);
                _logger.Information("Request UpdateURLAnhDuyetGia has been received.");
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
                if (request == null)
                {
                    return BadRequest();
                }
                else
                {
                    if (request.SoTien < 0)
                    {
                        return BadRequest("Không được nhập số tiền âm.");
                    }
                }
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

        public IActionResult UploadSampleFile([FromBody] string localPath)
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
                var entities = await _hsgdCtuService.GetProductInfo(soDonBh, soDonBhbs);
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

        [HttpPost("Preview-Image")]
        public async Task<IActionResult> GenerateWordFile([FromBody] List<string> imageUrls)
        {
            if (imageUrls == null || imageUrls.Count == 0)
            {
                return BadRequest("A list of image URLs is required.");
            }

            try
            {
                var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempDir);

                var imagePaths = new List<string>();
                for (int i = 0; i < imageUrls.Count; i++)
                {
                    imageUrls[i] = imageUrls[i].Replace("cdn247.pvi.com.vn", "192.168.250.77");
                    _logger.Information("Start download image" + imageUrls[i]);
                    var response = await _httpClient.GetAsync(imageUrls[i]);
                    _logger.Information("Download image success" + imageUrls[i]);
                    response.EnsureSuccessStatusCode();

                    var imageBytes = await response.Content.ReadAsByteArrayAsync();
                    var imagePath = Path.Combine(tempDir, $"{Guid.NewGuid()}.jpg");
                    await System.IO.File.WriteAllBytesAsync(imagePath, imageBytes);
                    _logger.Information("Write all Bytes " + imageUrls[i]);
                    imagePaths.Add(imagePath);
                }
                //foreach (var imageUrl in imageUrls)
                //{
                //    imageUrl = imageUrl.Replace("cdn247.pvi.com.vn", "192.168.250.77");
                //    var response = await _httpClient.GetAsync(imageUrl);
                //    response.EnsureSuccessStatusCode();

                //    var imageBytes = await response.Content.ReadAsByteArrayAsync();
                //    var imagePath = Path.Combine(tempDir, $"{Guid.NewGuid()}.jpg");
                //    await System.IO.File.WriteAllBytesAsync(imagePath, imageBytes);
                //    imagePaths.Add(imagePath);
                //}

                var wordFilePath = _hsgdCtuService.GenerateImage(imagePaths, tempDir);
                if (!String.IsNullOrEmpty(wordFilePath))
                {
                    return Ok(wordFilePath);
                }
                else
                {
                    return BadRequest("Error");
                }
                //var fileStream = new FileStream(wordFilePath, FileMode.Open, FileAccess.Read);
                //return File(fileStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "images.docx");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error downloading image: {ex.Message}");
            }
        }

        private string GenerateWordDocumentWithImages(List<string> imagePaths, string tempDir)
        {
            var wordApp = new Microsoft.Office.Interop.Word.Application();
            var wordDoc = wordApp.Documents.Add();

            foreach (var imagePath in imagePaths)
            {
                var paragraph = wordDoc.Content.Paragraphs.Add();
                paragraph.Range.InlineShapes.AddPicture(imagePath);
                paragraph.Range.InsertParagraphAfter();
            }

            var wordFilePath = Path.Combine(tempDir, "images.docx");
            wordDoc.SaveAs2(wordFilePath);
            wordDoc.Close();
            wordApp.Quit();

            return wordFilePath;
        }
        [HttpGet("TracuuPhi")]
        [Authorize]
        public async Task<IActionResult> TracuuPhi([Required] string so_donbh_tracuu, [Required] string so_seri_tracuu, [Required] int nam_tracuu, [Required] int nam_tracuu_goc, [Required] string ma_donvi_tracuu)
        {
            try
            {

                var entity = await _hsgdCtuService.TracuuPhi(so_donbh_tracuu, so_seri_tracuu, nam_tracuu, nam_tracuu_goc, ma_donvi_tracuu);

                _logger.Information("GET request TracuuPhi received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"TracuuPhi An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }


        [HttpGet("ReloadSumCheck")]
        //[Authorize]
        public IActionResult ReloadSumCheck([Required] int prKey)
        {
            try
            {
                var entity = _hsgdCtuService.ReloadSumCheck(prKey);
                return Ok(entity);
            }
            catch (Exception ex)
            {
                return BadRequest("Error at Reload Sum Check: " + ex.Message);
            }
        }

        [HttpGet("GetListGDV")]
        //[Authorize]
        public IActionResult GetListGDV()
        {
            try
            {
                var entity = _hsgdCtuService.GetListGDV();
                _logger.Information("GET request GetListGDV received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListGDV An error occured: {ex}");
                return BadRequest("An error occurred");
            }

        }


        [HttpPost("GetAnhKbtt")]

        public async Task<IActionResult> GetAnhKbtt(string so_hsgd)
        {
            var result = await  _hsgdCtuService.GetKbttAnh(so_hsgd);
            if(result == "Success")
            {
                return Ok(result);

            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost("GetAnhKbttCt")]

        public async Task<IActionResult> GetAnhKbttCt(string so_hsgd)
        {
            var result = await _hsgdCtuService.GetKbttAnh(so_hsgd);
            if (result == "Success")
            {
                return Ok(result);

            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost("GetAnhKbttCts")]

        public async Task<IActionResult> GetAnhKbttCts(string so_hsgd)
        {
            var result = await _hsgdCtuService.GetAnhKbttCt(so_hsgd);
            if (result == "Success")
            {
                return Ok(result);

            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost("DeleteAnhHsgdCt")]
        public async Task<IActionResult> DeleteAnh(List<decimal> listKey)
        {
            var result = await _hsgdCtuService.DeleteHsgdCt(listKey);
            if (result == "Success")
            {
                return Ok(result);

            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet("GetDkbh")]
        public async Task<IActionResult> GetDkbh(string maSp)
        {
            var result = await _hsgdCtuService.GetListDkbh(maSp);
            return Ok(result);
        }
        [HttpGet("KTPquyen_YCBS")]
        public async Task<IActionResult> KTPquyen_YCBS([Required] decimal pr_key)
        {
            var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
            var result = await _hsgdCtuService.KTPquyen_YCBS(pr_key,email);
            return Ok(result);
        }
        [HttpPost("PheDuyetNgoaiPhanCap")]
        [Authorize]

        public IActionResult PheDuyet_NgoaiPhanCap_12(int pr_key, decimal tong_sotien_hs, string? ghiChu = "")
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                string result = _hsgdCtuService.PheDuyet_NgoaiPhanCap_12(pr_key, email, tong_sotien_hs, ghiChu);
                _logger.Information("Request PheDuyet_NgoaiPhanCap_12 has been received.");
                try
                {
                    Int64.Parse(result);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    _logger.Error("PheDuyet_NgoaiPhanCap_12 ERROR: " + ex.Message);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("PheDuyet_NgoaiPhanCap_12 ERROR: " + ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("TraHSNgoaiPhanCap")]
        [Authorize]

        public IActionResult TraHS_NgoaiPhanCap_12(int pr_key, string? ghiChu = "")
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                string result = _hsgdCtuService.TraHS_NgoaiPhanCap_12(pr_key, email, ghiChu);
                _logger.Information("Request TraHS_NgoaiPhanCap_12 has been received.");
                try
                {
                    Int64.Parse(result);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    _logger.Error("TraHS_NgoaiPhanCap_12 ERROR: " + ex.Message);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("TraHS_NgoaiPhanCap_12 ERROR: " + ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ChuyenTrinhNgoaiPhanCap")]
        [Authorize]

        public IActionResult ChuyenTrinhNgoaiPhanCap(int pr_key, string? ghiChu = "")
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                string result = _hsgdCtuService.ChuyenTrinh_NgoaiPhanCap(pr_key, email, ghiChu);
                _logger.Information("Request ChuyenTrinh_NgoaiPhanCap has been received.");
                try
                {
                    Int64.Parse(result);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    _logger.Error("ChuyenTrinh_NgoaiPhanCap ERROR: " + ex.Message);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ChuyenTrinh_NgoaiPhanCap ERROR: " + ex.Message);
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("GetCRM")]
        public async Task<IActionResult> GetCRM(int prKey)
        {
            try
            {

                var updateResult = await _hsgdCtuService.ProcessCRMAsync(prKey);
                if (updateResult == null || updateResult.Status != "00")
                {
                    return BadRequest(updateResult.Message);
                }
                else
                {
                    _logger.Information("GET request GetCRM received");
                    return Ok(updateResult);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured in GetCRM: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpGet("GetTTBaoLanh")]
        [Authorize]
        public IActionResult GetTTBaoLanh(decimal pr_key_hsgd_dx_ct)
        {
            try
            {

                var entity = _hsgdCtuService.GetTTBaoLanh(pr_key_hsgd_dx_ct);
                if (entity == null)
                {
                    _logger.Error("Không lấy được thông tin bảo lãnh");
                    return BadRequest();
                }
                _logger.Information("GET request GetTTBaoLanh received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetTTBaoLanh An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpGet("GetListFile")]
        [Authorize]
        public IActionResult GetListFile([Required] decimal pr_key_hsgd_ctu)
        {
            try
            {
                var entities = _hsgdCtuService.GetListFile(pr_key_hsgd_ctu);
                _logger.Information("GetListFile success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpPost("UploadHoSoTT_MDF1")]
        [Authorize]
        public async Task<IActionResult> UploadHoSoTT_MDF1([FromBody] UploadHoSoTTRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest("Dữ liệu request không được để trống");
                }

                if (entity.PrKeyHsgdCtu <= 0)
                {
                    return BadRequest("Mã hồ sơ giám định không hợp lệ");
                }

                if (entity.hsgdattachfiles == null || !entity.hsgdattachfiles.Any())
                {
                    return BadRequest("Phải có ít nhất một file đính kèm");
                }

                // Validate file data
                foreach (var file in entity.hsgdattachfiles)
                {
                    // Phải có ít nhất một trong hai: base64 hoặc filePath
                    if (string.IsNullOrEmpty(file.base64) && string.IsNullOrEmpty(file.filePath))
                    {
                        return BadRequest("Phải cung cấp dữ liệu file (base64) hoặc đường dẫn file (filePath)");
                    }

                    if (string.IsNullOrEmpty(file.fileName))
                    {
                        return BadRequest("Tên file không được để trống");
                    }

                    // Validate file extension
                    string fileExtension = Path.GetExtension(file.fileName);
                    if (string.IsNullOrEmpty(fileExtension))
                    {
                        return BadRequest($"File '{file.fileName}' phải có phần mở rộng");
                    }

                    // Check allowed extensions
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".jfif", ".xml", ".pdf", ".docx" };
                    if (!allowedExtensions.Contains(fileExtension.ToLower()))
                    {
                        return BadRequest($"Định dạng file '{fileExtension}' không được phép cho file: {file.fileName}");
                    }
                }

                var result = await _hsgdCtuService.UploadHoSoTT_MDF1(entity);

                _logger.Information($"UploadHoSoTT_MDF1 received: {result}");

                if (result == "Thành công")
                {
                    return Ok(new
                    {
                        message = result,
                        uploadedFiles = entity.hsgdattachfiles.Count,
                        details = "Tải lên hồ sơ thành công"
                    });
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"UploadHoSoTT_MDF1 error: {ex}");
                return BadRequest("Có lỗi xảy ra trong quá trình tải lên file");
            }
        }
        [HttpGet("DownloadHoSoTT_MDF1")]
        [Authorize]
        public IActionResult DownloadHoSoTT_MDF1(string pr_key)
            {
                try
                {
                    var result = _hsgdCtuService.DownloadHoSoTT_MDF1(pr_key);
        
                    _logger.Information($"DownloadHoSoTT_MDF1 received: pr_key = {pr_key}, status = {result.Status}");
        
                    if (result?.Status == "00")
                    {
                        return Ok(result);
                    }
                    else
                    {
                        return BadRequest(result?.Message ?? "Download failed");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"DownloadHoSoTT_MDF1 error: {ex}");
                    return BadRequest("An error occurred");
                }
            }

        [HttpDelete("DeleteAttachFile/{pr_key}")]
        [Authorize]
        public async Task<IActionResult> DeleteAttachFile(string pr_key)
        {
            try
            {
                if (string.IsNullOrEmpty(pr_key))
                {
                    return BadRequest("Pr_key không được để trống");
                }

                var result = await _hsgdCtuService.DeleteAttachFile(pr_key);

                if (result == "Xóa file thành công")
                {
                    _logger.Information($"DELETE request DeleteAttachFile successful for pr_key: {pr_key}");
                    return Ok(new { success = true, message = result });
                }
                else
                {
                    _logger.Warning($"DELETE request DeleteAttachFile failed for pr_key: {pr_key}, reason: {result}");
                    return BadRequest(new { success = false, message = result });
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"DELETE request DeleteAttachFile error for pr_key: {pr_key}, error: {ex}");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra trên server" });
            }
        }

        [HttpPut("UpdateHoanThienHstt")]
        [Authorize]
        public async Task<IActionResult> UpdateHoanThienHstt([FromBody] UpdateHoanThienHsttRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Dữ liệu request không hợp lệ");
                }

                if (request.PrKeyHsgdCtu <= 0)
                {
                    return BadRequest("Mã hồ sơ giám định không hợp lệ");
                }

                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = await _hsgdCtuService.UpdateHoanThienHstt(request.PrKeyHsgdCtu, request.HoanThienHstt, email);

                if (result.Contains("thành công"))
                {
                    _logger.Information($"UpdateHoanThienHstt successful: PrKeyHsgdCtu = {request.PrKeyHsgdCtu}, HoanThienHstt = {request.HoanThienHstt}");
                    return Ok(new { success = true, message = result });
                }
                else
                {
                    _logger.Warning($"UpdateHoanThienHstt failed: PrKeyHsgdCtu = {request.PrKeyHsgdCtu}, reason = {result}");
                    return BadRequest(new { success = false, message = result });
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"UpdateHoanThienHstt controller error: {ex}");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra trên server" });
            }
        }
    }
}