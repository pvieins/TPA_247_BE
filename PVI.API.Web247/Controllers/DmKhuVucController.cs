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


    public class DmKhuVucController : ControllerBase
    {
        // Truyền các tham số
        private readonly DmKhuVucService _uqService;
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;

        public DmKhuVucController(DmKhuVucService uqService, Serilog.ILogger logger, IConfiguration configuration)
        {
            _uqService = uqService;
            _logger = logger;
            _configuration = configuration;
        }

        // Lấy tất cả danh sách khu vực theo filter.
        [HttpPost("GetListKhuVuc")]
        [Authorize]
        public IActionResult getListKhuVuc([FromBody] KhuVucFilter filter, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var list_uq = _uqService.GetDanhSachKhuVuc(pageNumber, pageSize, filter, currentUserEmail);
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

        // Lấy tất cả tỉnh
        [HttpGet("GetListTinh")]
        [Authorize]
        public IActionResult getListTinh()
        {
            try
            {
                var list_uq = _uqService.getListTinh();
                if (list_uq == null)
                {
                    _logger.Error("GetListTinh Error.");
                    return BadRequest("An error occured");
                }
                _logger.Information("GET request GetListTinh received");
                return Ok(list_uq);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy tất cả danh sách quận huyện
        [HttpGet("GetListQuanHuyen")]
        [Authorize]
        public IActionResult getListTinh(string MaTinh)
        {
            try
            {
                var list_uq = _uqService.getListQuanHuyen(MaTinh);
                if (list_uq == null)
                {
                    _logger.Error("GetListQuanHuyen Error.");
                    return BadRequest("An error occured");
                }
                _logger.Information("GET request GetListQuanHuyen received");
                return Ok(list_uq);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Thêm khu vực
        // Trả vè thông tin khu vực mới nếu thành công, hoặc báo lỗi nếu thất bại.

        [HttpPost("CreateKhuVuc")]
        public IActionResult createKhuVuc([FromBody] KhuVucCreate entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _uqService.createKhuVuc(entity, currentUserEmail);
                _logger.Information("POST request CreateKhuVuc received");
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


        // Update khu vực - YÊU CẦU PARAMS PHẢI CÓ PR KEY
        // Trả vè key của khu vực mới nếu thành công, hoặc báo lỗi nếu thất bại.

        [HttpPut("UpdateKhuVuc")]
        [Authorize]
        public IActionResult updateKhuVuc(int prKey, [FromBody] KhuVucUpdate entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _uqService.updateKhuVuc(prKey, entity, currentUserEmail);
                _logger.Information("POST request CreateKhuVuc received");
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