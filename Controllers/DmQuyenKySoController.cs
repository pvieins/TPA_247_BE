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
    // - Limit: Số record trên 1 trang.
    //
    // khanhlh - 22/08/2024


    public class DmQuyenKySoController : ControllerBase
    {
        // Truyền các tham số
        private readonly PQuyenKyHsService _pQuyenKyHsService;
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;

        public DmQuyenKySoController(PQuyenKyHsService pQuyenKyHsService, Serilog.ILogger logger, IConfiguration configuration)
        {
            _pQuyenKyHsService = pQuyenKyHsService;
            _logger = logger;
            _configuration = configuration;
        }

        // Lấy tất cả danh sách quyền ký số.
        [HttpGet("GetListQuyenKySo")]
        [Authorize]
        public async Task<IActionResult> getDigitalSignList(int pageNumber = 1, int limit = 10)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var list_ky_so = await _pQuyenKyHsService.getDigitalSignList(pageNumber, limit, currentUserEmail);
                if (list_ky_so == null)
                {
                    _logger.Error("Loi danh sach quyen ky so");
                    return NotFound();
                }
                _logger.Information("GET request GetListQuyenKySo received");
                return Ok(list_ky_so);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy tất cả danh sách quyền ký số.
        [HttpPost("SearchFilterQuyenKySo")]
        [Authorize]
        // Có thể thêm filter vào để xuất thông tin theo trường.
        public async Task<IActionResult> searchDigitalSignByFilter([FromBody] DmQuyenKyFilter searchTarget, int pageNumber = 1, int limit = 10)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var list_ky_so = await _pQuyenKyHsService.searchDigitalSignByFilter(pageNumber, limit, searchTarget, currentUserEmail);
                if (list_ky_so == null)
                {
                    _logger.Error("Loi danh sach quyen ky so");
                    return NotFound();
                }
                _logger.Information("GET request GetListQuyenKySo received");
                return Ok(list_ky_so);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy danh sách tên các user. Có thể thêm filter để lấy thông tin theo trường 
        // Trả vè tên các user ký số nếu thành công, hoặc báo lỗi nếu thất bại.
        // Lưu ý: Hiện tại chỉ lấy danh sách các user đang được ACTIVE. 

        [HttpGet("GetListUserKySo")]
        [Authorize]
        public async Task<IActionResult> getDigitalSignUserList(int pageNumber = 1, int limit = 10, string? maUser = null, string? tenUser = null, string? dienthoai = null)
        {
            try
            {
                var list_ten_user = await _pQuyenKyHsService.getDigitalSignUserList(pageNumber, limit, maUser, tenUser, dienthoai);
                if (list_ten_user == null)
                {
                    _logger.Error("Loi danh sach user ky so.");
                    return NotFound();
                }
                _logger.Information("GET request GetListTenDiemTruc received");
                return Ok(list_ten_user);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy danh sách các mã sản phẩm, có thể thêm filter để lọc 
        // Trả vè tên sản phẩm nếu thành công, hoặc báo lỗi nếu thất bại.

        [HttpGet("GetListSanPham")]
        [Authorize]
        public async Task<IActionResult> getProductList(string? maSp = null, string? tenSp = null)
        {
            try
            {
                var list_sp = await _pQuyenKyHsService.getProductList(maSp, tenSp);
                if (list_sp == null)
                {
                    _logger.Error("Loi danh sach san pham ");
                    return NotFound();
                }
                _logger.Information("GET request GetListSanPham received");
                return Ok(list_sp);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        
        // Thêm user ký số.
        // Trả vè thông tin ký số mới nếu thành công, hoặc báo lỗi nếu thất bại.

        [HttpPost("CreateQuyenKySo")]
        public async Task<IActionResult> createDigitalSign([FromBody] QuyenKySoRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                var result = await _pQuyenKyHsService.createDigitalSign(entity);
                _logger.Information("POST request CreateDigitalSign received");
                if (result != null)
                {
                    return Ok(result);
                }
                else
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

        
        // Update quyền ký số - YÊU CẦU PARAMS PHẢI CÓ Mã User
        // Trả vè mã user của quyền ký số nếu thành công, hoặc báo lỗi nếu thất bại.

        [HttpPut("UpdateQuyenKySo")]
        [Authorize]
        public IActionResult updateDigitalSign([FromBody] QuyenKySoRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                var result = _pQuyenKyHsService.updateDigitalSign(entity);
                _logger.Information("PUT request UpdateQuyenKySo received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                _logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }

        // Xóa quyền ký số.
        [HttpDelete("DeleteDigitalSign")]
        [Authorize]
        public async Task<IActionResult> DeleteHsgdTtrinh(string ma_user)
        {
            try
            {
                var result = await _pQuyenKyHsService.deleteDigitalSign(ma_user);
                _logger.Information("DELETE request DeleteDigitalSign received:" + result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
    }
}