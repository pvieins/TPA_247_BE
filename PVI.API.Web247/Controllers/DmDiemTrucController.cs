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
    // khanhlh - 22/08/2024

    public class DmDiemTrucController : ControllerBase
    {
        // Truyền các tham số
        private readonly DiemTrucService _diemTrucService;
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;

        public DmDiemTrucController(DiemTrucService diemTrucService, Serilog.ILogger logger, IConfiguration configuration)
        {
            _diemTrucService = diemTrucService;
            _logger = logger;
            _configuration = configuration;
        }

        // Lấy danh sách tất cả các điểm trực 
        // Các trường bắt buộc chỉ có pageNumber và limit 
        [HttpGet("GetListDiemTruc")]
        [Authorize]
        public async Task<IActionResult> getStationList(int pageNumber = 1, int limit = 10)
        {
            try
            {
                var list_diem_truc = await _diemTrucService.getStationList(pageNumber, limit);
                if (list_diem_truc == null)
                {
                    _logger.Error("Loi danh sach diem truc");
                    return NotFound();
                }
                _logger.Information("GET request GetListDiemTruc received");
                return Ok(list_diem_truc);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy danh sách tất cả các điểm trực, bao gồm cả filter. Nếu không có filter thì mặc định lấy hết. 
        // Các trường bắt buộc chỉ có pageNumber và limit 
        [HttpPost("SearchFilterDiemTruc")]
        [Authorize]
        public async Task<IActionResult> searchFilterStationList([FromBody] DmDiemtrucFilter searchTarget, int pageNumber = 1, int limit = 10)
        {
            try
            {
                var list_diem_truc = await _diemTrucService.searchFilterStationList(pageNumber, limit, searchTarget);
                if (list_diem_truc == null)
                {
                    _logger.Error("Loi danh sach diem truc");
                    return NotFound();
                }
                _logger.Information("GET request GetListDiemTruc received");
                return Ok(list_diem_truc);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy danh sách tên các điểm trực
        // Trả vè tên cấc điểm trực nếu thành công, hoặc báo lỗi nếu thất bại.

        [HttpGet("GetListTenDiemTruc")]
        [Authorize]
        public async Task<IActionResult> getStationNameList(int pageNumber = 1, int limit = 10)
        {
            try
            {
                var list_ten_diem_truc = await _diemTrucService.getStationNameList(pageNumber, limit);
                if (list_ten_diem_truc == null)
                {
                    _logger.Error("Loi danh sach diem truc");
                    return NotFound();
                }
                _logger.Information("GET request GetListTenDiemTruc received");
                return Ok(list_ten_diem_truc);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy danh sách các user GDTT.
        // Trả vè tên các user nếu thành công, hoặc báo lỗi nếu thất bại.

        [HttpGet("GetListTenUser")]
        [Authorize]
        public async Task<IActionResult> getStationUserList(int pageNumber = 1, int limit = 10)
        {
            try
            {
                var list_user_gdtt = await _diemTrucService.getStationUserList(pageNumber, limit);
                if (list_user_gdtt == null)
                {
                    _logger.Error("Loi danh sach diem truc");
                    return NotFound();
                }
                _logger.Information("GET request GetListTenUserGDTT received");
                return Ok(list_user_gdtt);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Tạo điểm trực mới
        // Trả vè mã điểm trực mới nếu thành công, hoặc báo lỗi nếu thất bại.

        [HttpPost("CreateDiemTruc")]
        public async Task<IActionResult> createStation([FromBody] DiemtrucRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = await _diemTrucService.createStation(entity, currentUserEmail);
                _logger.Information("POST request CreateDiemTruc received");
                if (result != null)
                {
                    return Ok(result);
                } else
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

        // Update điểm trực - YÊU CẦU PARAMS PHẢI CÓ PRKEY VÀ MÃ ĐIỂM TRỰC !
        // Trả vè mã điểm trực nếu thành công, hoặc báo lỗi nếu thất bại.

        [HttpPut("UpdateDiemTruc")]
        [Authorize]
        public IActionResult updateStation([FromBody] DiemtrucRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _diemTrucService.updateStation(entity, currentUserEmail);
                _logger.Information("PUT request UpdateDiemTruc received");
                return Ok(result);
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