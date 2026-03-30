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


    public class DmUyQuyenController : ControllerBase
    {
        // Truyền các tham số
        private readonly DmUyQuyenService _uqService;
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;

        public DmUyQuyenController(DmUyQuyenService uqService, Serilog.ILogger logger, IConfiguration configuration)
        {
            _uqService = uqService;
            _logger = logger;
            _configuration = configuration;
        }

        // Lấy tất cả danh sách ủy quyền theo filter.
        [HttpPost("GetListUyQuyen")]
        [Authorize]
        public IActionResult getListUyQuyen([FromBody] UyQuyenFilter filter,int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var list_uq =  _uqService.GetDanhSachUyQuyen(pageNumber, pageSize, filter, currentUserEmail);
                if (list_uq == null)
                {
                    _logger.Error("Lỗi danh sách ủy quyền");
                    return BadRequest("An error occured");
                }
                _logger.Information("GET request GetListUyQuyen received");
                return Ok(list_uq);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy tất cả danh sách user có thể ủy quyền theo mã đơn vị.
        [HttpGet("GetListUserUyQuyen")]
        [Authorize]
        public IActionResult getListUserUyQuyen(string maDonvi)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var list_uq = _uqService.getListUserUyQuyen(maDonvi, currentUserEmail);
                if (list_uq == null)
                {
                    _logger.Error("GetListUserUyQuyen Error.");
                    return BadRequest("An error occured");
                }
                _logger.Information("GET request GetListQuyenKySo received");
                return Ok(list_uq);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Thêm ủy quyền
        // Trả vè thông tin ủy quyền mới nếu thành công, hoặc báo lỗi nếu thất bại.

        [HttpPost("CreateUyQuyen")]
        public IActionResult createUyQuyen([FromBody] UyQuyenRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _uqService.createUyQuyen(entity, currentUserEmail);
                _logger.Information("POST request CreateUyQuyen received");
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


        // Update ủy quyền - YÊU CẦU PARAMS PHẢI CÓ PR KEY
        // Trả vè key của ủy quyền mới nếu thành công, hoặc báo lỗi nếu thất bại.

        [HttpPut("UpdateUyQuyen")]
        [Authorize]
        public IActionResult updateUyQuyen(int prKey, [FromBody] UyQuyenRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _uqService.updateUyQuyen(prKey, entity, currentUserEmail);
                _logger.Information("POST request CreateUyQuyen received");
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