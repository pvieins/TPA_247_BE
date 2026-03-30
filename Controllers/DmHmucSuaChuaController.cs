using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Office.Interop.Word;
using PVI.DAO.Entities.Models;
using PVI.Repository.Repositories;
using PVI.Service;
using PVI.Service.ActionProcess;
using PVI.Service.Request;
using System.ComponentModel.DataAnnotations;

namespace PVI.API.Web247.Controllers
{
    [ApiController]
    [Route("[controller]")]

    // Controller hiện tại chủ yếu đang là thêm sửa xóa
    // Đường gọi như sau: Controller -> Service -> Repository.
    // khanhlh - 26/08/2024

    public class DmHmucSuaChuaController : ControllerBase
    {
        // Truyền các tham số
        private readonly DmHmucSuaChuaService _hmucSuaChuaService;
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;

        public DmHmucSuaChuaController(DmHmucSuaChuaService hmucService, Serilog.ILogger logger, IConfiguration configuration)
        {
            _hmucSuaChuaService = hmucService;
            _logger = logger;
            _configuration = configuration;
        }

        // Lấy danh sách tất cả các tổng thành xe
        [HttpPost("GetListTongThanhXe")]
        [Authorize]
        public IActionResult getListTongThanhXe(bool getFull = false)
        {
            try
            {
                var list_ttx = _hmucSuaChuaService.getListTongThanhXe(getFull);
                if (list_ttx == null)
                {
                    _logger.Error("Lỗi tổng thành xe.");
                    return BadRequest("An error occured");
                }
                _logger.Information("GET request GetListTongThanhXe received");
                
                return Ok(list_ttx);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy danh sách tất cả các nhóm hạng mục.
        [HttpPost("GetListNhomHmuc")]
        [Authorize]
        public IActionResult getListNhmuc([FromBody] DmNHmucFilter nhomHmuc, int pageNumber = 1, int pageSize = 10, bool getFull = false)
        {
            try
            {
                var result = _hmucSuaChuaService.getListNHmuc(pageNumber, pageSize, nhomHmuc, getFull);
                if (result == null)
                {
                    _logger.Error("Lỗi nhóm hạng mục.");
                    return BadRequest("An error occured");
                }
                _logger.Information("GET request getListNhmuc received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy danh sách tất cả các hạng mục.
        [HttpPost("GetListHmuc")]
        [Authorize]
        public IActionResult getListHmuc([FromBody] DmHmucFilter hmuc, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = _hmucSuaChuaService.getListHmuc(pageNumber, pageSize, hmuc);
                if (result == null)
                {
                    _logger.Error("Lỗi hạng mục.");
                    return BadRequest("An error occured");
                }
                _logger.Information("GET request getListHmuc received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy danh sách tất cả các hạng mục.
        [HttpPost("GetListHmuc_PASC")]
        //[Authorize]
        public IActionResult getListHmuc_PASC([FromBody] DmHmuc_PASC_Filter filter,int prKeyHSGD, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = _hmucSuaChuaService.getListHmuc_HSGD_Anh(pageNumber, pageSize, prKeyHSGD, filter);
                if (result == null)
                {
                    _logger.Error("Lỗi hạng mục.");
                    return BadRequest("An error occured");
                }
                _logger.Information("GET request getListHmuc received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }


        // Thêm mới hạng mục và nhóm hạng mục.
        [HttpPost("CreateNhmuc")]
        [Authorize]
        public IActionResult createNhmuc([FromBody] DmNHmucRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _hmucSuaChuaService.CreateNHmuc(entity, currentUserEmail);
                _logger.Information("POST request createNHmuc received");
                try
                {
                    // Nếu trả về PrKey thì nghĩa là tạo thành công, trả lỗi là tạo thất bại.
                    Int64.Parse(result);
                    return Ok(result);
                } catch
                {
                    return BadRequest("An error occured");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                _logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }
        // Thêm mới hạng mục và nhóm hạng mục.
        [HttpPost("CreateHmuc")]
        [Authorize]
        public IActionResult createHmuc([FromBody] DmHmucRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _hmucSuaChuaService.CreateHmuc(entity, currentUserEmail);
                _logger.Information("POST request createHmuc received");
                try
                {
                    // Nếu trả về PrKey thì nghĩa là tạo thành công, trả lỗi là tạo thất bại.
                    Int64.Parse(result);
                    return Ok(result);
                }
                catch
                {
                    return BadRequest("An error occured");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                _logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }

        // Update hạng mục và nhóm hạng mục.
        [HttpPut("UpdateNhmuc")]
        [Authorize]
        public IActionResult updateNhmuc(string maNhmuc, [FromBody] DmNhmucUpdate entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _hmucSuaChuaService.updateNhmuc(maNhmuc, entity, currentUserEmail);
                _logger.Information("PUT request updateNhmuc received");
                try
                {
                    // Nếu trả về PrKey thì nghĩa là tạo thành công, trả lỗi là tạo thất bại.
                    Int64.Parse(result);
                    return Ok(result);
                }
                catch
                {
                    return BadRequest("An error occured");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                _logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }

        // Thêm mới hạng mục và nhóm hạng mục.
        [HttpPut("UpdateHmuc")]
        [Authorize]
        public IActionResult updateHmuc(string maHmuc, [FromBody] DmHmucUpdate entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _hmucSuaChuaService.updateHmuc(maHmuc, entity, currentUserEmail);
                _logger.Information("PUT request createHmuc received");
                try
                {
                    // Nếu trả về PrKey thì nghĩa là tạo thành công, trả lỗi là tạo thất bại.
                    Int64.Parse(result);
                    return Ok(result);
                }
                catch
                {
                    return BadRequest("An error occured");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                _logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }


    }
}