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
    public class HsgdTtrinhController : ControllerBase
    {

        private readonly HsgdTtrinhService _hsgdTtrinhService;
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;

        public HsgdTtrinhController(HsgdTtrinhService hsgdTtrinhService, Serilog.ILogger logger, IConfiguration configuration)
        {
            _hsgdTtrinhService = hsgdTtrinhService;
            _logger = logger;
            _configuration = configuration;
        }
        [Authorize]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] TtrinhRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                else
                {
                    if (entity.hsgdTtrinh.SoTien < 0 || entity.hsgdTtrinh.GiatriThuhoi < 0 || entity.hsgdTtrinh.ChiKhac < 0 || entity.hsgdTtrinh.SoNgchet < 0 || entity.hsgdTtrinh.SoBthuong < 0 || entity.hsgdTtrinh.SoPhibh < 0 || entity.hsgdTtrinhCt.Where(x=>x.SotienBh < 0 || x.SotienBt < 0 || x.SotienTu < 0 || x.MucVat < 0 || x.SoTienBtVat < 0).Count() > 0)
                    {
                        return BadRequest();
                    }
                }
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = await _hsgdTtrinhService.CreateHsgdTtrinh(entity,email);
                _logger.Information("POST request CreateHsgdTtrinh received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                _logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }
        [HttpPut("Update")]
        [Authorize]
        public IActionResult Update([FromBody] TtrinhRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                else
                {
                    if (entity.hsgdTtrinh.SoTien < 0 || entity.hsgdTtrinh.GiatriThuhoi < 0 || entity.hsgdTtrinh.ChiKhac < 0 || entity.hsgdTtrinh.SoNgchet < 0 || entity.hsgdTtrinh.SoBthuong < 0 || entity.hsgdTtrinh.SoPhibh < 0 || entity.hsgdTtrinhCt.Where(x => x.SotienBh < 0 || x.SotienBt < 0 || x.SotienTu < 0 || x.MucVat < 0 || x.SoTienBtVat < 0).Count() > 0)
                    {
                        return BadRequest();
                    }
                }
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _hsgdTtrinhService.UpdateHsgdTtrinh(entity,email);
                _logger.Information("POST request CreateHsgdTtrinh received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                _logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }

        [HttpGet("PrintToTrinh")]
        [Authorize]
        public async Task<IActionResult> PrintToTrinh(decimal pr_key)
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _hsgdTtrinhService.PrintToTrinh(pr_key, email);

                _logger.Information("PrintToTrinh success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpGet("GetListTtrinh")]
        [Authorize]
        public IActionResult GetListTtrinh([Required] decimal pr_key_hsgd)
        {
            try
            {                
                var entity = _hsgdTtrinhService.GetListTtrinh(pr_key_hsgd);
                _logger.Information("GET request GetListTtrinh received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred " + ex);
            }

        }
        [HttpGet("GetTtrinhById")]
        [Authorize]
        public IActionResult GetTtrinhById([Required] decimal pr_key)
        {
            try
            {

                var entity = _hsgdTtrinhService.GetTtrinhById(pr_key);
                if (entity.hsgdTtrinh == null)
                {
                    _logger.Error("Không tồn tại tờ trình");
                    return BadRequest("An error occured");
                }
                _logger.Information("GET request GetTtrinhById received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }

        }
        [HttpGet("GetSoPhiBH")]
        [Authorize]
        public IActionResult GetSoPhiBH(string so_donbh, decimal so_seri)
        {
            try
            {

                var entity = _hsgdTtrinhService.GetSoPhiBH(so_donbh, so_seri);
                _logger.Information("GET request GetSoPhiBH received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }

        }
        [HttpGet("CheckDKBS007")]
        [Authorize]
        public IActionResult CheckDKBS007(decimal pr_key_hsgd)
        {
            try
            {

                var entity = _hsgdTtrinhService.CheckDKBS007(pr_key_hsgd);
                _logger.Information("GET request CheckDKBS007 received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }

        }
        [HttpDelete("DeleteHsgdTtrinh")]
        [Authorize]
        public async Task<IActionResult> DeleteHsgdTtrinh(Guid oid)
        {
            try
            {
                var result = await _hsgdTtrinhService.DeleteHsgdTtrinh(oid);
                _logger.Information("DELETE request DeleteHsgdTtrinh received:" + result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpDelete("DeleteHsgdTtrinhCt")]
        [Authorize]
        public async Task<IActionResult> DeleteHsgdTtrinhCt(decimal pr_key)
        {
            try
            {
                var result = await _hsgdTtrinhService.DeleteHsgdTtrinhCt(pr_key);
                _logger.Information("DELETE request DeleteHsgdTtrinhCt received:" + result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpGet("TrinhKy")]
        [Authorize]
        public async Task<IActionResult> TrinhKy(decimal pr_key)
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _hsgdTtrinhService.TrinhKy(pr_key, email);
                _logger.Information("TrinhKy received:" + result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost("DownloadFile")]
        [Authorize]
        public IActionResult DownloadFile(decimal pr_key)
        {
            try
            {
                var result = _hsgdTtrinhService.DownloadTtrinh(pr_key);
                _logger.Information("Download File Completed");
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }



        }
        [HttpGet("TrinhKy_MDF1")]
        [Authorize]
        public IActionResult TrinhKy_MDF1(decimal pr_key)
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _hsgdTtrinhService.TrinhKy_MDF1(pr_key, email);
                _logger.Information("TrinhKy_MDF1 received:" + result);
                if (result == "Thành công")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
                
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost("DownloadFile_MDF1")]
        [Authorize]
        public IActionResult DownloadFile_MDF1(decimal pr_key)
        {
            try
            {
                var result = _hsgdTtrinhService.DownloadTtrinh_MDF1(pr_key);
                _logger.Information("Download File MDF1 Completed");
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }



        }
        [HttpPut("UpdateTrangThaiHsbtCt")]
        [Authorize]
        public IActionResult UpdateTrangThaiHsbtCt(decimal pr_key_hsgd_ttrinh)
        {
            try
            {
                var result = _hsgdTtrinhService.UpdateTrangThaiHsbtCt(pr_key_hsgd_ttrinh);
                _logger.Information("UpdateTrangThaiHsbtCt received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("An error occured");
            }
        }
        [HttpPost("ChuyenDuyet")]
        [Authorize]
        public IActionResult ChuyenDuyet(decimal pr_key_hsgd_ttrinh, string oid_nhan, bool send_email)
        {
            try
            {
                var email_login = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _hsgdTtrinhService.ChuyenDuyet(pr_key_hsgd_ttrinh, email_login, oid_nhan, send_email);

                _logger.Information("ChuyenDuyet success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"ChuyenDuyet An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost("KyHoSo")]
        [Authorize]
        public IActionResult KyHoSo(decimal pr_key_hsgd_ttrinh)
        {
            try
            {
                var email_login = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //var email_login = "quyenvm@pvi.com.vn";
                var result = _hsgdTtrinhService.KyHoSo(pr_key_hsgd_ttrinh, email_login);

                _logger.Information("KyHoSo success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"KyHoSo An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost("CheckKyHoSo")]
        [Authorize]
        public IActionResult CheckKyHoSo(decimal pr_key_hsgd_ttrinh)
        {
            try
            {
                //var email_login = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _hsgdTtrinhService.CheckKyHoSo(pr_key_hsgd_ttrinh);

                _logger.Information("CheckKyHoSo success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"CheckKyHoSo An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost("ChuyenHoSo")]
        [Authorize]
        public ActionResult ChuyenHoSo(decimal pr_key_hsgd_ttrinh, string oid_nhan, bool send_email)
        {
            try
            {
                var email_login = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _hsgdTtrinhService.ChuyenHoSo(pr_key_hsgd_ttrinh, email_login, oid_nhan, send_email);

                _logger.Information("ChuyenHoSo success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"ChuyenHoSo An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost("ChuyenKyHoSo")]
        [Authorize]
        public IActionResult ChuyenKyHoSo(decimal pr_key_hsgd_ttrinh,string oid_nhan, bool send_email)
        {
            try
            {
                var email_login = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _hsgdTtrinhService.ChuyenKyHoSo(pr_key_hsgd_ttrinh, email_login, oid_nhan, send_email);

                _logger.Information("ChuyenKyHoSo success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"ChuyenKyHoSo An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost("TraLaiHoSo")]
        [Authorize]
        public IActionResult TraLaiHoSo(decimal pr_key_hsgd_ttrinh,string oid_nhan, string lido_tc, bool send_email)
        {
            try
            {
                var email_login = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _hsgdTtrinhService.TraLaiHoSo(pr_key_hsgd_ttrinh, email_login, oid_nhan, lido_tc, send_email);

                _logger.Information("TraLaiHoSo success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"TraLaiHoSo An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost("HuyToTrinh")]
        [Authorize]
        public IActionResult HuyToTrinh(decimal pr_key_hsgd_ttrinh)
        {
            try
            {
                var email_login = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _hsgdTtrinhService.HuyToTrinh(pr_key_hsgd_ttrinh, email_login);

                _logger.Information("HuyToTrinh success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"HuyToTrinh An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpGet("GetLichSuPheDuyet")]
        [Authorize]
        public IActionResult GetLichSuPheDuyet(decimal pr_key_hsgd_ttrinh)
        {
            try
            {
                var result = _hsgdTtrinhService.GetLichSuPheDuyet(pr_key_hsgd_ttrinh);
                _logger.Information("GetLichSuPheDuyet received:" + result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetLichSuPheDuyet An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpGet("CountTTrinhByTT")]
        [Authorize]
        public IActionResult CountTTrinhByTT(int nam_dulieu)
        {
            try
            {
                var email_login = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var entity = _hsgdTtrinhService.CountTTrinhByTT(email_login, nam_dulieu);
                _logger.Information("GET request CountTTrinhByTT received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"CountTTrinhByTT An error occured: {ex}");
                return BadRequest("An error occurred");
            }

        }

        [HttpGet("CountTTrinhLDByTT")]
        [Authorize]
        public IActionResult CountTTrinhLDByTT(int nam_dulieu)
        {
            try
            {
                var email_login = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var entity = _hsgdTtrinhService.CountTTrinhLDByTT(email_login, nam_dulieu);
                _logger.Information("GET request CountTTrinhLDByTT received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"CountTTrinhLDByTT An error occured: {ex}");
                return BadRequest("An error occurred");
            }

        }

        [HttpPost("GetDataHsTrinhKy")]
        [Authorize]
        public IActionResult GetDataHsTrinhKy(ToTrinhParameters totrinhParameters)
        {
            try
            {
                var email_login = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //var email_login = "chinhnv@pvi.com.vn";
                var result =  _hsgdTtrinhService.GetDataHsTrinhKy(email_login,totrinhParameters);
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
                _logger.Information("GetDataHsTrinhKy success");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetDataHsTrinhKy An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpPost("GetDataHsTrinhKyKoHoaDon")]
        [Authorize]
        public IActionResult GetDataHsTrinhKyKoHoaDon(ToTrinhParameters totrinhParameters)
        {
            try
            {
                var email_login = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //var email_login = "chinhnv@pvi.com.vn";
                var result = _hsgdTtrinhService.GetDataHsTrinhKyKoHoaDon(email_login, totrinhParameters);
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
                _logger.Information("GetDataHsTrinhKy success");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetDataHsTrinhKy An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpPost("GetDataHsDaThanhToan")]
        [Authorize]
        public IActionResult GetDataHsDaThanhToan(ToTrinhParameters totrinhParameters)
        {
            try
            {
                var email_login = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);              
                var result = _hsgdTtrinhService.GetDataHsDaThanhToan(email_login, totrinhParameters);
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
                _logger.Information("GetDataHsTrinhKy success");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetDataHsTrinhKy An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpPost("GetDataHsTrinhKyLD")]
        [Authorize]
        public IActionResult GetDataHsTrinhKyLD(ToTrinhParameters totrinhParameters)
        {
            try
            {
                var email_login = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //var email_login = "chinhnv@pvi.com.vn";
                var result = _hsgdTtrinhService.GetDataHsTrinhKyLanhDao(email_login, totrinhParameters);
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
                _logger.Information("GetDataHsTrinhKyLanhDao success");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetDataHsTrinhKyLanhDao An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpGet("GetUserLogin")]
        [Authorize]
        public IActionResult GetUserLogin()
        {
            try
            {
                var email_login = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //var email_login = "dieulq@pvi.com.vn";
                var entity = _hsgdTtrinhService.GetUserLogin(email_login);
                _logger.Information("GET request GetUserLogin received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetUserLogin An error occured: {ex}");
                return BadRequest("An error occurred");
            }

        }
        [HttpGet("GetListUserChuyenKy")]
        [Authorize]
        public IActionResult GetListUserChuyenKy()
        {
            try
            {
                var email_login = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //var email_login = "chinhnv@pvi.com.vn";
                var entity = _hsgdTtrinhService.GetListUserChuyenKy(email_login);
                _logger.Information("GET request GetListUserChuyenKy received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListUserChuyenKy An error occured: {ex}");
                return BadRequest("An error occurred");
            }

        }
        [HttpPost("CreateBiaHS")]
        [Authorize]
        public async Task<IActionResult> CreateBiaHS([FromBody] BiaHS biahs)
        {
            try
            {
                var result = _hsgdTtrinhService.CreateBiaHS(biahs);
                _logger.Information("CreateBiaHS pr_key_hsgd_ctu = " + biahs.PrKey + " success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"CreateBiaHS pr_key_hsgd_ctu = " + biahs.PrKey + " An error occurred: " + ex);
                return BadRequest("Có lỗi xảy ra. Hãy thử lại sau.");
            }

        }
        [HttpGet("PrintToTrinhTPC")]
        [Authorize]
        public async Task<IActionResult> PrintToTrinhTPC(decimal pr_key_hsgd_ctu,int loai_tt)
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //var chk_tpc = _hsgdTtrinhService.CheckHsgdTPC(pr_key_hsgd_ctu,"10");
                //if (chk_tpc)
                //{
                    var result = _hsgdTtrinhService.PrintToTrinhTPC(pr_key_hsgd_ctu, email, loai_tt);

                    _logger.Information("PrintToTrinhTPC success");
                    return Ok(result);
                //}
                //else
                //{
                //    return BadRequest("Không phải HS trên phân cấp của TVP. Không thực hiện chức năng này.");
                //}
            }
            catch (Exception ex)
            {
                _logger.Error($"PrintToTrinhTPC An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost("UploadToTrinhTPC")]
        //[Authorize]
        public IActionResult UploadToTrinhTPC([FromBody] UploadToTrinhTPC entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _hsgdTtrinhService.UploadToTrinhTPC(entity, email);
                if (result == "Upload file thành công.")
                {
                    _logger.Information("POST request UploadToTrinhTPC received");
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"UploadToTrinhTPC An error occured: {ex}");
                _logger.Error("UploadToTrinhTPC Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }
        [HttpPost("PheDuyetHsTpc")]
        [Authorize]
        public IActionResult PheDuyetHsTpc(decimal pr_key_hsgd_ctu,string email_login)
        {
            try
            {
                var result = _hsgdTtrinhService.PheDuyetHsTpc(pr_key_hsgd_ctu, email_login);

                _logger.Information("PheDuyetHsTpc success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"PheDuyetHsTpc An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost("KyHoSoTPC")]
        [Authorize]
        public IActionResult KyHoSoTPC(decimal pr_key_hsgd_ctu, string email_login)
        {
            try
            {
                var result = _hsgdTtrinhService.KyHoSoTPC(pr_key_hsgd_ctu, email_login);

                _logger.Information("KyHoSoTPC success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"KyHoSoTPC An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpGet("DownloadToTrinhTPC")]
        [Authorize]
        public async Task<IActionResult> DownloadToTrinhTPC(decimal pr_key_hsgd_ctu, int loai_tt)
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //var email = "chinhnv@pvi.com.vn";
                //var chk_tpc = _hsgdTtrinhService.CheckHsgdTPC(pr_key_hsgd_ctu,"10");
                //if (chk_tpc)
                //{
                var result = _hsgdTtrinhService.DownloadToTrinhTPC(pr_key_hsgd_ctu, email, loai_tt);

                _logger.Information("DownloadToTrinhTPC success");
                return Ok(result);
                //}
                //else
                //{
                //    return BadRequest("Không phải HS trên phân cấp của TVP. Không thực hiện chức năng này.");
                //}
            }
            catch (Exception ex)
            {
                _logger.Error($"DownloadToTrinhTPC An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost("CheckHsgdTPC")]
        //[Authorize]
        public bool CheckHsgdTPC(decimal pr_key_hsgd_ctu,string ma_ttrang,string email_login)
        {
            try
            {
                var result = _hsgdTtrinhService.CheckHsgdTPC(pr_key_hsgd_ctu, ma_ttrang, email_login);

                _logger.Information("CheckHsgdTPC success");
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error($"CheckHsgdTPC An error occured: {ex}");
                return false;
            }
        }
        [HttpPost("TaoToTrinhTpc")]
        [Authorize]
        public IActionResult TaoToTrinhTpc(decimal pr_key_hsgd_ctu, int loai_tt)
        {
            try
            {
                var email_login = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //var email_login = "lanlt@pvi.com.vn";
                var result = _hsgdTtrinhService.TaoToTrinhTpc(pr_key_hsgd_ctu, email_login, loai_tt);

                _logger.Information("TaoToTrinhTpc success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"TaoToTrinhTpc An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
    }
}