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
    // khanhlh - 26/08/2024

    public class DmGaraController : ControllerBase
    {
        // Truyền các tham số
        private readonly DmGaraService _garaService;
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;

        public DmGaraController(DmGaraService garaService, Serilog.ILogger logger, IConfiguration configuration)
        {
            _garaService = garaService;
            _logger = logger;
            _configuration = configuration;
        }

        // Lấy danh sách tất cả các gara
        [HttpGet("GetListGara")]
        [Authorize]
        public async Task<IActionResult> getGarageList(int pageNumber = 1, int limit = 10)
        {
            try
            {
                var list_gara = await _garaService.getGarageList(pageNumber, limit);
                if (list_gara == null)
                {
                    _logger.Error("Loi danh sach gara");
                    return NotFound();
                }
                _logger.Information("GET request GetListGara received");
                return Ok(list_gara);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy danh sách tất cả các gara, có kèm các trường để filter 
        [HttpPost("SearchFilterGara")]
        [Authorize]
        public async Task<IActionResult> searchFilterGara([FromBody] DmGaraFilter searchTarget ,int pageNumber = 1, int limit = 10)
        {
            try
            {
                var list_gara = await _garaService.searchFilterGarage(pageNumber, limit, searchTarget);
                if (list_gara == null)
                {
                    _logger.Error("Loi danh sach gara");
                    return NotFound();
                }
                _logger.Information("GET request GetListGara received");
                return Ok(list_gara);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Update gara
        // Trả vè mã gara nếu thành công, hoặc báo lỗi nếu thất bại.
        [HttpPut("UpdateGara")]
        [Authorize]
        public IActionResult updateGarage([FromBody] GaraRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _garaService.updateGarage(entity, currentUserEmail);
                _logger.Information("PUT request UpdateGarage received");
                if (result.Contains("Lỗi") || result.Contains("không"))
                {
                    return BadRequest(result);
                }
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