using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PVI.DAO.Entities.Models;
using PVI.Helper;
using PVI.Repository.Interfaces;
using PVI.Repository.Repositories;
using PVI.Service;
using PVI.Service.ActionProcess;
using PVI.Service.Request;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace PVI.API.Web247.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HsgdDxController : ControllerBase
    {

        private readonly HsgdDxService _hsgdDxService;
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;

        public HsgdDxController(HsgdDxService hsgdDxService, Serilog.ILogger logger, IConfiguration configuration)
        {
            _hsgdDxService = hsgdDxService;
            _logger = logger;
            _configuration = configuration;
        }
        [HttpGet("GetListPhaiTraBT")]
        [Authorize]
        public async Task<IActionResult> GetListPhaiTraBT([Required] decimal pr_key_hsgd)
        {
            try
            {

                var entity = _hsgdDxService.GetListPhaiTraBT(pr_key_hsgd);
                //if (entity == null)
                //{
                //    _logger.Error("Không có dữ liệu");
                //    return BadRequest();
                //}
                _logger.Information("GET request GetListPhaiTraBT received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListPhaiTraBT An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListChiTietUocBT")]
        [Authorize]
        public async Task<IActionResult> GetListChiTietUocBT([Required] decimal hsbt_ct_pr_key)
        {
            try
            {
                var entities = await _hsgdDxService.GetListChiTietUocBT(hsbt_ct_pr_key);
                _logger.Information("GetListChiTietUocBT sussces");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListChiTietUocBT An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListPhaiTraGD")]
        [Authorize]
        public IActionResult GetListPhaiTraGD([Required] decimal pr_key_hsgd)
        {
            try
            {
                var entities = _hsgdDxService.GetListPhaiTraGD(pr_key_hsgd);
                _logger.Information("GetListPhaiTraGD sussces");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListPhaiTraGD An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListChiTietUocGD")]
        [Authorize]
        public async Task<IActionResult> GetListChiTietUocGD([Required] decimal hsbt_gd_pr_key)
        {
            try
            {
                var entities = await _hsgdDxService.GetListChiTietUocGD(hsbt_gd_pr_key);
                _logger.Information("GetListChiTietUocGD sussces");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListChiTietUocGD An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListThuDoi")]
        [Authorize]
        public IActionResult GetListThuDoi([Required] decimal pr_key_hsgd)
        {
            try
            {
                var entities = _hsgdDxService.GetListThuDoi(pr_key_hsgd);
                _logger.Information("GetListThuDoi sussces");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListThuDoi An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListPASC")]
        [Authorize]
        public IActionResult GetListPASC([Required] decimal pr_key_hsgd_dx_ct,decimal pr_key_hsgd_ctu)
        {
            try
            {
                var entities = _hsgdDxService.GetListPASC(pr_key_hsgd_dx_ct, pr_key_hsgd_ctu);
                _logger.Information("GetListPASC sussces");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListPASC An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("ReloadSum")]
        [Authorize]
        public IActionResult ReloadSum([Required] decimal pr_key_hsgd_dx_ct)
        {
            try
            {
                var entities = _hsgdDxService.ReloadSum(pr_key_hsgd_dx_ct);
                _logger.Information("ReloadSum sussces");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"ReloadSum An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpPost("CreateHsbtCt")]
        [Authorize]
        public async Task<IActionResult> CreateHsbtCt([FromBody] HsbtCtRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                //var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = await _hsgdDxService.CreateHsbtCt(entity);
                _logger.Information("POST request CreateHsbtCt received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"CreateHsbtCt An error occured: {ex}");
                _logger.Error("CreateHsbtCt Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }
        [HttpPut("UpdateHsbtCt")]
        [Authorize]
        public async Task<IActionResult> UpdateHsbtCt([FromBody] HsbtCtRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = await _hsgdDxService.UpdateHsbtCt(entity);
                _logger.Information("POST request UpdateHsbtCt received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"UpdateHsbtCt An error occured: {ex}");
                _logger.Error("UpdateHsbtCt Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }
        [HttpDelete("DeleteHsbtCt")]
        [Authorize]
        public async Task<IActionResult> DeleteHsbtCt(decimal pr_key)
        {
            try
            {
                var result = await _hsgdDxService.DeleteHsbtCt(pr_key);
                _logger.Information("DeleteHsbtCt received:" + result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"DeleteHsbtCt An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost("CreateHsbtGd")]
        [Authorize]
        public async Task<IActionResult> CreateHsbtGd([FromBody] HsbtGdRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = await _hsgdDxService.CreateHsbtGd(entity);
                _logger.Information("POST request CreateHsbtGd received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"CreateHsbtGd An error occured: {ex}");
                _logger.Error("CreateHsbtGd Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }
        [HttpPut("UpdateHsbtGd")]
        [Authorize]
        public async Task<IActionResult> UpdateHsbtGd([FromBody] HsbtGdRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = await _hsgdDxService.UpdateHsbtGd(entity);
                _logger.Information("POST request UpdateHsbtGd received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"UpdateHsbtGd An error occured: {ex}");
                _logger.Error("UpdateHsbtGd Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }
        [HttpDelete("DeleteHsbtGd")]
        [Authorize]
        public async Task<IActionResult> DeleteHsbtGd(decimal pr_key)
        {
            try
            {
                var result = await _hsgdDxService.DeleteHsbtGd(pr_key);
                _logger.Information("DeleteHsbtGd received:" + result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"DeleteHsbtGd An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost("CreateHsbtThts")]
        [Authorize]
        public async Task<IActionResult> CreateHsbtThts([FromBody] HsbtThtsRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = await _hsgdDxService.CreateHsbtThts(entity);
                _logger.Information("POST request CreateHsbtThts received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"CreateHsbtThts An error occured: {ex}");
                _logger.Error("CreateHsbtThts Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }
        [HttpPut("UpdateHsbtThts")]
        [Authorize]
        public async Task<IActionResult> UpdateHsbtThts([FromBody] HsbtThtsRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = await _hsgdDxService.UpdateHsbtThts(entity);
                _logger.Information("POST request UpdateHsbtThts received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"UpdateHsbtThts An error occured: {ex}");
                _logger.Error("UpdateHsbtThts Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }
        [HttpDelete("DeleteHsbtThts")]
        [Authorize]
        public async Task<IActionResult> DeleteHsbtThts(decimal pr_key)
        {
            try
            {
                var result = await _hsgdDxService.DeleteHsbtThts(pr_key);
                _logger.Information("DeleteHsbtThts received:" + result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"DeleteHsbtThts An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost("CreatePASC")]
        [Authorize]
        public IActionResult CreatePASC([FromBody] HsbtDxRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _hsgdDxService.CreatePASC(entity);
                _logger.Information("POST request CreatePASC received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"CreatePASC An error occured: {ex}");
                _logger.Error("CreatePASC Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }
        [HttpPut("ImportPASC")]
        [Authorize]
        public async Task<IActionResult> ImportPASC([FromBody] List<ImportPASCRequest> entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = await _hsgdDxService.ImportPASC(entity);
                _logger.Information("POST request ImportPASC received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"ImportPASC An error occured: {ex}");
                _logger.Error("ImportPASC Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }
        [HttpGet("PrintPASC")]
        [Authorize]
        public async Task<IActionResult> PrintPASC(decimal pr_key_hsbt_ct, decimal pr_key_hsgd_ctu, int loai_dx)
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _hsgdDxService.PrintPASC(pr_key_hsbt_ct, pr_key_hsgd_ctu, email, loai_dx);

                _logger.Information("PrintPASC success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"PrintPASC An error occurred: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpGet("GuiPASC")]
        [Authorize]
        public async Task<IActionResult> GuiPASC(decimal pr_key_hsbt_ct, decimal pr_key_hsgd_ctu, bool chk_send_pasc, bool pasc_send_sms, string email_nhan, string phone_nhan)
        {
            try
            {
                var email_login = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //var email_login = "huynhchinh@pvi.com.vn";
                var result = _hsgdDxService.GuiPASC(pr_key_hsbt_ct,pr_key_hsgd_ctu,chk_send_pasc, pasc_send_sms,email_nhan,phone_nhan, email_login);
                _logger.Information("GuiPASC pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + ", pr_key_hsbt_ct = "+ pr_key_hsbt_ct + " success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"GuiPASC pr_key_hsgd_ctu = "+ pr_key_hsgd_ctu + ", pr_key_hsbt_ct = " + pr_key_hsbt_ct + " An error occurred: " + ex);
                return BadRequest("Có lỗi xảy ra. Hãy thử lại sau.");
            }

        }
        [HttpPost("LichSuPasc")]
        [Authorize]
        public async Task<IActionResult> LichSuPasc(LichsuPaParameters parameters)
        {
            try
            {
                var result = await _hsgdDxService.LichSuPasc(parameters);
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
                _logger.Information("LichSuPasc success");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"LichSuPasc An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListDonViGiamDinh")]
        [Authorize]
        public async Task<IActionResult> GetListDonViGiamDinh()
        {
            try
            {
                var email_login = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //var email_login = "huynhchinh@pvi.com.vn";
                var entities = await _hsgdDxService.GetListDonViGiamDinh(email_login);
                _logger.Information("GetListDonViGiamDinh success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListDonViGiamDinh An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetFileAttachBt")]
        [Authorize]
        public IActionResult GetFileAttachBt([Required] decimal pr_key_hsbt_ct, [Required] string ma_ctu)
        {
            try
            {
                var entities = _hsgdDxService.GetFileAttachBt(pr_key_hsbt_ct, ma_ctu);
                _logger.Information("GetFileAttachBt sussces");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetFileAttachBt An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpPost("DownloadFileAttachBt_MDF1")]
        [Authorize]
        public IActionResult DownloadFileAttachBt_MDF1(decimal pr_key)
        {
            try
            {
                var result = _hsgdDxService.DownloadFileAttachBt_MDF1(pr_key);
                _logger.Information("DownloadFileAttachBt_MDF1 Completed");
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.Error($"DownloadFileAttachBt_MDF1 An error occured: {ex}");
                return BadRequest("An error occured");
            }



        }
        [HttpGet("GetSTBTByHsgd")]
        [Authorize]
        public IActionResult GetSTBTByHsgd([Required] decimal pr_key_hsbt_ctu)
        {
            try
            {
                var entities = _hsgdDxService.GetSTBTByHsgd(pr_key_hsbt_ctu);
                _logger.Information("GetSTBTByHsgd sussces");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetSTBTByHsgd An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
    }
}