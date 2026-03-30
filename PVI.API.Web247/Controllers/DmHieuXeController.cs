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
    // khanhlh - 02/10/2024


    public class DmHieuXeController : ControllerBase
    {
        // Truyền các tham số
        private readonly DmHieuXeService _hxlxService;
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;

        public DmHieuXeController(DmHieuXeService hxlxService, Serilog.ILogger logger, IConfiguration configuration)
        {
            _hxlxService = hxlxService;
            _logger = logger;
            _configuration = configuration;
        }

        // Lấy tất cả danh sách hiệu xe theo filter.
        [HttpPost("GetListHieuXe")]
        [Authorize]
        public IActionResult getListHieuXe([FromBody] HieuXeRequest filter, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var list_uq = _hxlxService.getListHieuXe(pageNumber, pageSize, filter);
                if (list_uq == null)
                {
                    _logger.Error("Lỗi danh sách hiệu xe");
                    return BadRequest("An error occured");
                }
                _logger.Information("GET request GetListHieuXe received");
                return Ok(list_uq);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy tất cả loại xe
        [HttpPost("GetListLoaiXe")]
        [Authorize]
        public IActionResult getListLoaiXe([FromBody] LoaiXeFilter filter, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var list_uq = _hxlxService.getListLoaiXe(pageNumber, pageSize, filter);
                if (list_uq == null)
                {
                    _logger.Error("GetListLoaiXe Error.");
                    return BadRequest("An error occured");
                }
                _logger.Information("GET request getListLoaiXe received");
                return Ok(list_uq);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Thêm hiệu xe
        // Trả vè thông tin hiệu xe mới nếu thành công, hoặc báo lỗi nếu thất bại.

        [HttpPost("CreateHieuXe")]
        public IActionResult createHieuXe([FromBody] HieuXeRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _hxlxService.createHieuXe(entity, currentUserEmail);
                _logger.Information("POST request CreateHieuXe received");
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

        // Thêm loại xe
        // Trả vè thông tin hiệu xe mới nếu thành công, hoặc báo lỗi nếu thất bại.

        [HttpPost("CreateLoaiXe")]
        public IActionResult createLoaiXe([FromBody] LoaiXeRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _hxlxService.createLoaiXe(entity, currentUserEmail);
                _logger.Information("POST request CreateLoaiXe received");
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


        // Update hiệu xe - YÊU CẦU PARAMS PHẢI CÓ PR KEY
        // Trả vè key của hiệu xe mới nếu thành công, hoặc báo lỗi nếu thất bại.

        [HttpPut("UpdateHieuXe")]
        [Authorize]
        public IActionResult updateHieuXe(int prKey, [FromBody] HieuXeRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _hxlxService.updateHieuXe(prKey, entity, currentUserEmail);
                _logger.Information("POST request updateHieuXe received");
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

        // Update loại xe - YÊU CẦU PARAMS PHẢI CÓ PR KEY
        // Trả vè key của hiệu xe mới nếu thành công, hoặc báo lỗi nếu thất bại.

        [HttpPut("UpdateLoaiXe")]
        [Authorize]
        public IActionResult updateLoaiXe(int prKey, [FromBody] LoaiXeRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _hxlxService.updateLoaiXe(prKey, entity, currentUserEmail);
                _logger.Information("POST request updateLoaiXe received");
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