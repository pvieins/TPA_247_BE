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
    // khanhlh - 28/10/2024


    public class BaoCaoController : ControllerBase
    {
        // Truyền các tham số
        private readonly BaoCaoService _BaoCaoService;
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;

        public BaoCaoController(BaoCaoService bcService, Serilog.ILogger logger, IConfiguration configuration)
        {
            _BaoCaoService = bcService;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet("GetListDonviBaoCao")]
        [Authorize]
        public IActionResult GetListDonViBaoCao()
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                List<DmDonvi> list_gia_pt = _BaoCaoService.getListDonvi_BaoCao(currentUserEmail);
                //_logger.Information("GET request TraCuuGiaPhuTung received");
                return Ok(list_gia_pt);
            }
            catch (Exception ex)
            {
                //_logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // 1 - Thống kê tình hình GDTT theo đơn vị
        [HttpPost("BCDonViGDTT")]
        [Authorize]
        public IActionResult ThongKe_GDTT_DonVi([FromBody] ThongKe_GDTT_DonVi_Filter filter)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                ThongKe_GDTT_DonVi_Response list_gia_pt = _BaoCaoService.ThongKe_GDTT_DonVi(filter, currentUserEmail);
                //_logger.Information("GET request TraCuuGiaPhuTung received");
                return Ok(list_gia_pt);
            }
            catch (Exception ex)
            {
                //_logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // 2 - Thống kê tình hình GDTT theo GDV
        [HttpPost("BCGDVGDTT")]
        [Authorize]
        public IActionResult ThongKe_GDTT_GDV([FromBody] ThongKe_GDTT_GDV_Filter filter)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                ThongKe_GDTT_GDV_Response list_gia_pt = _BaoCaoService.ThongKe_GDTT_GDV(filter, currentUserEmail);
                //_logger.Information("GET request TraCuuGiaPhuTung received");
                return Ok(list_gia_pt);
            }
            catch (Exception ex)
            {
                //_logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Thống kê tình hình GTTT
        [HttpPost("ThongKeGDTT")]
        [Authorize]
        public IActionResult ThongKeGDTT([FromBody] ThongKeGDTT_General_Main_Filter filter, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                ThongKeGDTT_General_Response list_gia_pt = _BaoCaoService.ThongKeGDTT(filter, pageNumber, pageSize, currentUserEmail);
                //_logger.Information("GET request TraCuuGiaPhuTung received");
                return Ok(list_gia_pt);
            }
            catch (Exception ex)
            {
                //_logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy tất cả danh sách tra cứu phụ tùng theo filter.
        [HttpPost("TraCuuGiaPhuTung")]
        [Authorize]
        public IActionResult TraCuuGiaPhuTung([FromBody] SearchGiaPhuTung_Main_Filter filter, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                SearchGiaPhuTungResponse list_gia_pt = _BaoCaoService.SearchGiaPhuTung(filter, pageNumber, pageSize, currentUserEmail);
                //_logger.Information("GET request TraCuuGiaPhuTung received");
                return Ok(list_gia_pt);
            }
            catch (Exception ex)
            {
                //_logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy tất cả danh sách tra cứu phụ tùng theo filter.
        [HttpPost("BCHoSoTPC")]
        [Authorize]
        public IActionResult BCHSTPC([FromBody] HSTPC_Filter filter, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                HSTPC_Response list_gia_pt = _BaoCaoService.BCHSTPC_TrenPhanCap(filter, pageNumber, pageSize, currentUserEmail);
                //_logger.Information("GET request TraCuuGiaPhuTung received");
                return Ok(list_gia_pt);
            }
            catch (Exception ex)
            {
                //_logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }


        // Lấy tất cả danh sách device theo filter.
        [HttpPost("TraCuuGttt")]
        [Authorize]
        public IActionResult TraCuuGttt([FromBody] SearchGttt_Main_Filter filter, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //string currentUserEmail = "longnh4@pvi.com.vn";
                SearchGtttResponse list_gia_pt = _BaoCaoService.SearchGttt(filter, pageNumber, pageSize, currentUserEmail);
                //_logger.Information("GET request TraCuuGttt received");
                return Ok(list_gia_pt);
            }
            catch (Exception ex)
            {
                //_logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }


        // Lấy tất cả danh sách device theo filter.
        [HttpPost("BCThuHoiTS")]
        [Authorize]
        public IActionResult BaoCaoThuHoiTS([FromBody] BCThuHoiTS_Main_Filter filter, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //string currentUserEmail = "longnh4@pvi.com.vn";
                BCThuHoiTSResponse result = _BaoCaoService.BCThuHoiTS(filter, pageNumber, pageSize, currentUserEmail);
                //_logger.Information("GET request GetListDevice received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                //_logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }


    }
}