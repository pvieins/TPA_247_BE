using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Office.Interop.Word;
using Newtonsoft.Json;
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
    //
    // TẤT CẢ ĐẦU GET YÊU CÀU PARAMS NHƯ SAU: 
    // - PageNumber: Số trang (bắt đầu từ 1).
    // - pageSize: Số record trên 1 trang.
    //
    // khanhlh - 22/08/2024


    public class DmDeviceController : ControllerBase
    {
        // Truyền các tham số
        private readonly DmDeviceService _uqService;
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;

        public DmDeviceController(DmDeviceService uqService, Serilog.ILogger logger, IConfiguration configuration)
        {
            _uqService = uqService;
            _logger = logger;
            _configuration = configuration;
        }

        // Lấy tất cả danh sách device theo filter.
        [HttpPost("GetListDevice")]
        [Authorize]
        public IActionResult getListDevice([FromBody] DeviceFilter filter, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var list_uq = _uqService.getListDevice(pageNumber, pageSize, filter, currentUserEmail);
                if (list_uq == null)
                {
                    _logger.Error("Lỗi danh sách device");
                    return BadRequest("An error occured");
                }
                _logger.Information("GET request GetListDevice received");
                return Ok(list_uq);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        // Thêm device
        // Trả vè thông tin device mới nếu thành công, hoặc báo lỗi nếu thất bại.

        [HttpPost("CreateDevice")]
        public IActionResult createDevice([FromBody] DeviceRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _uqService.createDevice(entity, currentUserEmail);
                _logger.Information("POST request CreateDevice received");
                try
                {
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


        // Update device - YÊU CẦU PARAMS PHẢI CÓ PR KEY
        // Trả vè key của device mới nếu thành công, hoặc báo lỗi nếu thất bại.

        [HttpPut("UpdateDevice")]
        [Authorize]
        public IActionResult updateDevice(int prKey, [FromBody] DeviceRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _uqService.updateDevice(prKey, entity, currentUserEmail);
                _logger.Information("POST request UpdateDevice received");
                try
                {
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