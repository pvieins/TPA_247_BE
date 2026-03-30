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


    public class DmGaraKhuvucController : ControllerBase
    {
        // Truyền các tham số
        private readonly DmGaraKhuVucService _uqService;
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;

        public DmGaraKhuvucController(DmGaraKhuVucService uqService, Serilog.ILogger logger, IConfiguration configuration)
        {
            _uqService = uqService;
            _logger = logger;
            _configuration = configuration;
        }

        // Lấy tất cả danh sách Gara khu vực theo filter.
        [HttpPost("GetListGaraKhuvuc")]
        [Authorize]
        public IActionResult getListGaraKhuvuc([FromBody] GaraKhuVucFilter filter, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var list_uq = _uqService.getDanhSachGaraKhuVuc(pageNumber, pageSize, filter, currentUserEmail);
                if (list_uq == null)
                {
                    _logger.Error("Lỗi danh sách GaraKhuvuc");
                    return BadRequest("An error occured");
                }
                _logger.Information("GET request GetListGaraKhuvuc received");
                return Ok(list_uq);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy tất cả danh sách tất cả các gara có thể thêm mới 
        [HttpGet("GetListGaraThemMoi")]
        [Authorize]
        public IActionResult getListGarageToeBeInserted(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var list_uq = _uqService.getListGara(pageNumber, pageSize);
                if (list_uq == null)
                {
                    _logger.Error("Lỗi danh sách GaraKhuvuc");
                    return BadRequest("An error occured");
                }
                _logger.Information("GET request GetListGaraThemMoi received");
                return Ok(list_uq);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy tất cả danh sách khu vực 
        [HttpGet("GetListKhuVuc")]
        [Authorize]
        public IActionResult getListKhuVuc()
        {
            try
            {
                var list_uq = _uqService.getListKhuvuc();
                if (list_uq == null)
                {
                    _logger.Error("Lỗi danh sách khu vực");
                    return BadRequest("An error occured");
                }
                _logger.Information("GET request GetListKhuVuc received");
                return Ok(list_uq);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Thêm GaraKhuvuc
        // Trả vè thông tin GaraKhuvuc mới nếu thành công, hoặc báo lỗi nếu thất bại.

        [HttpPost("CreateGaraKhuvuc")]
        public IActionResult createGaraKhuvuc([FromBody] GaraKhuVucRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _uqService.createGaraKhuVuc(entity, currentUserEmail);
                _logger.Information("POST request CreateGaraKhuvuc received");
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


        // Update GaraKhuvuc - YÊU CẦU PARAMS PHẢI CÓ PR KEY
        // Trả vè key của GaraKhuvuc mới nếu thành công, hoặc báo lỗi nếu thất bại.

        [HttpPut("UpdateGaraKhuvuc")]
        [Authorize]
        public IActionResult updateGaraKhuvuc(int prKey, [FromBody] GaraKhuVucRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _uqService.updateGaraKhuVuc(prKey, entity, currentUserEmail);
                _logger.Information("POST request UpdateGaraKhuvuc received");
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