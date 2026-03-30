using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PVI.Service.Request;
using PVI.Repository.Repositories;
using PVI.Service;
using PVI.Helper;
using PVI.Service.ActionProcess;
using System.ComponentModel.DataAnnotations;
using PVI.DAO.Entities.Models;
using PVI.Repository.Interfaces;
using System.Data;
using System.Text;

namespace PVI.API.Web247.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HsgdDnttController : ControllerBase
    {

        private readonly HsgdDnttService _hsgdDnttService;
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;

        public HsgdDnttController(HsgdDnttService hsgdDnttService, Serilog.ILogger logger, IConfiguration configuration)
        {
            _hsgdDnttService = hsgdDnttService;
            _logger = logger;
            _configuration = configuration;
        }        
        
        [HttpPost("CreateDNTT")]
        [Authorize]
        public async Task<IActionResult> CreateDNTT([FromBody] DNTTRequest dNTTRequest, string pr_key_hsgd_ttrinh)
        {
            try
            {
                if (string.IsNullOrEmpty(pr_key_hsgd_ttrinh))
                {
                    return BadRequest();
                }
                var email_login = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //var email_login = "lanlt@pvi.com.vn";
                var result = _hsgdDnttService.CreateDNTT(dNTTRequest,pr_key_hsgd_ttrinh, email_login);
                _logger.Information("POST request CreateDNTT received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("CreateDNTT pr_key_hsgd_ttrinh = "+ pr_key_hsgd_ttrinh  + " An error occured: " + ex);
                return BadRequest("An error occured");
            }
        }
        [HttpGet("GetListNguoiDeNghi")]
        [Authorize]
        public async Task<IActionResult> GetListNguoiDeNghi(string ma_donvi_tt)
        {
            try
            {
                var entities = await _hsgdDnttService.GetListNguoiDeNghi(ma_donvi_tt);
                _logger.Information("GetListNguoiDeNghi success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListNguoiDeNghi An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListDonViTT")]
        [Authorize]
        public async Task<IActionResult> GetListDonViTT(string ma_donvi_tt)
        {
            try
            {
                var entities = await _hsgdDnttService.GetListDonViTT(ma_donvi_tt);
                _logger.Information("GetListDonViTT success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListDonViTT An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListNhomKT")]
        [Authorize]
        public async Task<IActionResult> GetListNhomKT(string ma_donvi)
        {
            try
            {
                var entities = await _hsgdDnttService.GetListNhomKT(ma_donvi);
                _logger.Information("GetListNhomKT success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListNhomKT An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListNguoiXuLy")]
        [Authorize]
        public async Task<IActionResult> GetListNguoiXuLy(string ma_donvi)
        {
            try
            {
                var entities = await _hsgdDnttService.GetListNguoiXuLy(ma_donvi);
                _logger.Information("GetListNguoiXuLy success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListNguoiXuLy An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpPost("GetListDntt")]
        [Authorize]
        public IActionResult GetListDntt(DnttParameters dnttParameters)
        {
            try
            {
                var email_login = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //var email_login = "lanlt@pvi.com.vn";
                var result = _hsgdDnttService.GetListDntt(email_login, dnttParameters);
                if (result != null)
                {
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
                }
               
                _logger.Information("GetListDntt success");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListDntt An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpDelete("DeleteDntt")]
        [Authorize]
        public async Task<IActionResult> DeleteDntt(string pr_key_dntt)
        {
            try
            {
                var result = await _hsgdDnttService.DeleteDntt(pr_key_dntt);
                _logger.Information("DELETE request DeleteDntt received:" + result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"DeleteDntt an error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpGet("GetLichSuPheDuyet")]
        [Authorize]
        public IActionResult GetLichSuPheDuyet(decimal pr_key_ttoan_ctu)
        {
            try
            {
                var entities =  _hsgdDnttService.GetLichSuPheDuyet(pr_key_ttoan_ctu);
                _logger.Information("GetLichSuPheDuyet success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetLichSuPheDuyet An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListCanBoTT")]
        [Authorize]
        public async Task<IActionResult> GetListCanBoTT(string ma_donvi_tt)
        {
            try
            {
                var entities = await _hsgdDnttService.GetListCanBoTT(ma_donvi_tt);
                _logger.Information("GetListCanBoTT success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListCanBoTT An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }
    }
}