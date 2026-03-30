using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Office.Interop.Word;
using Newtonsoft.Json;
using PVI.DAO.Entities.Models;
using PVI.Helper;
using PVI.Repository.Repositories;
using PVI.Service;
using PVI.Service.ActionProcess;
using PVI.Service.Request;
using System.ComponentModel.DataAnnotations;
using static PVI.Repository.Repositories.KbttCtuRepository;

namespace PVI.API.Web247.Controllers
{
    [ApiController]
    [Route("[controller]")]



    public class KbttCtuController : ControllerBase
    {

        private readonly KbttCtuService _kbttCtuService;
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;

        public KbttCtuController(KbttCtuService kbttCtuService, Serilog.ILogger logger, IConfiguration configuration)
        {
            _kbttCtuService = kbttCtuService;
            _logger = logger;
            _configuration = configuration;
        }


        [HttpPost("GetListPVIMobile")]
        [Authorize]
        public async Task<IActionResult> GetListPVIMobile(KbttCtuParameters parameters)
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetInfo(HttpContext, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
                //ma_don_vi = "31";
                var listPVIMobile = await _kbttCtuService.GetListPVIMobile(parameters, email, "0501");
                if (listPVIMobile == null)
                {
                    return BadRequest("An error occured");
                }
                else
                {
                    var metadata = new
                    {
                        listPVIMobile.TotalCount,
                        listPVIMobile.PageSize,
                        listPVIMobile.CurrentPage,
                        listPVIMobile.TotalPages,
                        listPVIMobile.HasNext,
                        listPVIMobile.HasPrevious

                    };


                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                    _logger.Information("GET request GetListPVIMobile received");
                    return Ok(listPVIMobile);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost("GetListPVIMobileExcel")]
        [Authorize]
        public async Task<IActionResult> GetListPVIMobileExcel(KbttCtuParameters parameters)
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetInfo(HttpContext, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
                //ma_don_vi = "31";
                var listPVIMobile = await _kbttCtuService.GetListPVIMobileExcel(parameters, email, "0501");
                if (listPVIMobile == null)
                {
                    return BadRequest("An error occured");
                }
                else
                {

                    _logger.Information("GET request GetListPVIMobile received");
                    return Ok(listPVIMobile);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost("GetListPVIMobileXM")]
        [Authorize]
        public async Task<IActionResult> GetListPVIMobileXM(KbttCtuParameters parameters)
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetInfo(HttpContext, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
                //ma_don_vi = "31";
                var listPVIMobile = await _kbttCtuService.GetListPVIMobile(parameters, email, "0502");
                if (listPVIMobile == null)
                {
                    return BadRequest("An error occured");
                }
                else
                {
                    var metadata = new
                    {
                        listPVIMobile.TotalCount,
                        listPVIMobile.PageSize,
                        listPVIMobile.CurrentPage,
                        listPVIMobile.TotalPages,
                        listPVIMobile.HasNext,
                        listPVIMobile.HasPrevious

                    };


                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                    _logger.Information("GET request GetListPVIMobile received");
                    return Ok(listPVIMobile);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpGet("{prKey}")]
        [Authorize]
        public async Task<IActionResult> GetDetailsKbttCtu(decimal prKey)
        {
            try
            {

                var result = await _kbttCtuService.GetDetailKbttCtu(prKey);

                _logger.Information("GET request GetDetailsKbttCtu received");
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost()]
        [Authorize]
        public async Task<IActionResult> CreateKbttCtu(CreateKbttCtuRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest();
                }
                else
                {
                    if (request.PrKeySeri < 0 || request.TygiaHt < 0 || request.TygiaTt < 0 || request.TongTien < 0 || request.SoLanBt < 0 || request.SoSeri < 0 || request.TinhTrang < 0 || request.LoaiKbtt < 0)
                    {
                        return BadRequest();
                    }
                }
                var email = JwtTokenHelper.ExtractTokenInfoAndSetInfo(HttpContext, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
                var result = await _kbttCtuService.CreateKbttCtu(request, email);

                _logger.Information("POST request CreateKbttCtu received");
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpGet("GetListAnhKbtt")]
        [Authorize]
        public async Task<IActionResult> GetListAnhKbtt(decimal prKey)
        {
            try
            {

                var result = await _kbttCtuService.GetListAnhKbtt(prKey);

                _logger.Information("GET request GetListAnhKbtt received");
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }


        [HttpPost("GetListAddHs")]
        [Authorize]
        public async Task<IActionResult> GetListAddHs(ListAddNewParameters parameters)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(parameters.SoSeriSearch) &&
              string.IsNullOrWhiteSpace(parameters.BienKiemSoatSearch) &&
              string.IsNullOrWhiteSpace(parameters.SoKhungSearch))
                {
                    return BadRequest("Bạn phải nhập thông tin tìm kiếm!");
                }

                if ((string.IsNullOrWhiteSpace(parameters.SoSeriSearch) &&
                    (!string.IsNullOrWhiteSpace(parameters.BienKiemSoatSearch) || !string.IsNullOrWhiteSpace(parameters.SoKhungSearch))) &&
                    parameters.NgayCuoiSearch == null)
                {
                    return BadRequest("Bạn phải nhập ngày cuối thời hạn bảo hiểm!");
                }

                var listAddNew = await _kbttCtuService.GetListAddHoSo(parameters, "0501");
                if (listAddNew == null)
                {
                    return BadRequest("An error occured");
                }
                else
                {
                    var metadata = new
                    {
                        listAddNew.TotalCount,
                        listAddNew.PageSize,
                        listAddNew.CurrentPage,
                        listAddNew.TotalPages,
                        listAddNew.HasNext,
                        listAddNew.HasPrevious

                    };


                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                    _logger.Information("GET request GetListPVIMobile received");
                    return Ok(listAddNew);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost("GetListAddHsXM")]
        [Authorize]
        public async Task<IActionResult> GetListAddHsXM(ListAddNewParameters parameters)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(parameters.SoSeriSearch) &&
              string.IsNullOrWhiteSpace(parameters.BienKiemSoatSearch) &&
              string.IsNullOrWhiteSpace(parameters.SoKhungSearch))
                {
                    return BadRequest("Bạn phải nhập thông tin tìm kiếm!");
                }

                if ((string.IsNullOrWhiteSpace(parameters.SoSeriSearch) &&
                    (!string.IsNullOrWhiteSpace(parameters.BienKiemSoatSearch) || !string.IsNullOrWhiteSpace(parameters.SoKhungSearch))) &&
                    parameters.NgayCuoiSearch == null)
                {
                    return BadRequest("Bạn phải nhập ngày cuối thời hạn bảo hiểm!");
                }

                var listAddNew = await _kbttCtuService.GetListAddHoSo(parameters, "0502");
                if (listAddNew == null)
                {
                    return BadRequest("An error occured");
                }
                else
                {
                    var metadata = new
                    {
                        listAddNew.TotalCount,
                        listAddNew.PageSize,
                        listAddNew.CurrentPage,
                        listAddNew.TotalPages,
                        listAddNew.HasNext,
                        listAddNew.HasPrevious

                    };


                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                    _logger.Information("GET request GetListPVIMobile received");
                    return Ok(listAddNew);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPut("{prKey}")]
        [Authorize]
        public async Task<IActionResult> UpdateKbttCtu(KbttCtuRequest request, decimal prKey)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest();
                }
                else
                {
                    if (request.SoTienugd < 0 || request.LoaiKbtt < 0 || request.TinhTrang < 0)
                    {
                        return BadRequest();
                    }
                }
                var updateResult = await _kbttCtuService.UpdateKbttCtu(prKey, request);
                if (updateResult == null)
                {
                    return BadRequest("An error occured");
                }
                else
                {
                    _logger.Information("PUT request UpdateKbttCtu received");
                    return Ok(updateResult);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        [HttpPost("TaoDonHsgd")]
        [Authorize]
        public async Task<IActionResult> TaoDonHsgd(TaoDonHsgdRequest request)
        {
            try
            {

                var updateResult = await _kbttCtuService.TaoDonHsgd(request.PrKey, request.MaLhsbt, request.DonviBth, "0501");
                if (updateResult == null || updateResult != "Success")
                {
                    updateResult ??= "An Error occured";
                    return BadRequest(updateResult);
                }
                else
                {
                    _logger.Information("POST request TaoDonHsgd received");
                    return Ok(updateResult);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        [HttpPost("TaoDonHsgdXM")]
        [Authorize]
        public async Task<IActionResult> TaoDonHsgdXM(TaoDonHsgdRequest request)
        {
            try
            {

                var updateResult = await _kbttCtuService.TaoDonHsgd(request.PrKey, request.MaLhsbt, request.DonviBth, "0502");
                if (updateResult == null || updateResult != "Success")
                {
                    updateResult ??= "An Error occured";
                    return BadRequest(updateResult);
                }
                else
                {
                    _logger.Information("POST request TaoDonHsgd received");
                    return Ok(updateResult);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost("CapNhatSoDonHSGD")]
        public async Task<IActionResult> CapNhatDonHsgd(DateTime startDate, DateTime endDate)
        {
            try
            {

                var updateResult = await _kbttCtuService.CapNhatSoHsgd(startDate, endDate);
                if (updateResult == null)
                {
                    return BadRequest("An error occured");
                }
                else
                {
                    _logger.Information("PUT request UpdateKbttCtu received");
                    return Ok(updateResult);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpGet("GetListLoaiHinhBh")]
        public async Task<IActionResult> GetListLoaiHinhBh()
        {
            try
            {

                var updateResult = await _kbttCtuService.GetListLoaiHinhBh();
                if (updateResult == null)
                {
                    return BadRequest("An error occured");
                }
                else
                {
                    _logger.Information("PUT request UpdateKbttCtu received");
                    return Ok(updateResult);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
    }
}