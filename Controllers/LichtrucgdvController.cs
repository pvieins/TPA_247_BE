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
    // khanhlh - 18/09/2024

    public class LichtrucgdvController : ControllerBase
    {
        // Truyền các tham số
        private readonly LichtrucgdvService _lichtrucService;
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;

        public LichtrucgdvController(LichtrucgdvService lichtrucService, Serilog.ILogger logger, IConfiguration configuration)
        {
            _lichtrucService = lichtrucService;
            _logger = logger;
            _configuration = configuration;
        }

        // Lấy danh sách tất cả các lich trực, bao gồm cả filter. Nếu không có filter thì mặc định lấy hết. 
        // Các trường bắt buộc chỉ có pageNumber và pageSize
        [HttpPost("SearchFilterLichTruc")]
        [Authorize]
        public IActionResult searchFilterStationList(string ma_kv, DateTime? ngay_xemlich)
        {
            try
            {
                var list_lich_truc = _lichtrucService.searchFilterStationSchedule(ma_kv, ngay_xemlich);
                if (list_lich_truc.listGara == null)
                {
                    return BadRequest("An error occured");
                }
                else
                {
                    _logger.Information("GET request GetListDiemTruc received");
                    return Ok(list_lich_truc);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy danh sách tất cả các khu vực  
        [HttpGet("GetListKhuVuc")]
        [Authorize]
        public IActionResult getListKhuVuc()
        {
            try
            {
                var list_kv = _lichtrucService.getListKhuVuc();
                if (list_kv == null)
                {
                    return BadRequest("An error occured");
                }
                else
                {
                    _logger.Information("GET request GetListKhuVuc received");
                    return Ok(list_kv);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy danh sách tất cả các gara khu vực  
        [HttpGet("GetListGaraKhuVuc")]
        [Authorize]
        public IActionResult getListGaraKhuVuc(string ma_kv)
        {
            try
            {
                var list_kv = _lichtrucService.getListGaraKhuVuc(ma_kv);
                if (list_kv == null)
                {
                    return BadRequest("An error occured");
                }
                else
                {
                    _logger.Information("GET request GetListGaraKhuVuc received");
                    return Ok(list_kv);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy danh sách tất cả cán bộ trực.
        // Quét các cán bộ có cùng mã đơn vị với user hiện tại.

        [HttpGet("GetListCanBoTruc")]
        [Authorize]
        public IActionResult getListCanBoTruc()
        {
            try
            {
                var list_kv = _lichtrucService.getListCanBoTruc();
                if (list_kv == null)
                {
                    return BadRequest("An error occured.");
                }
                else
                {
                    _logger.Information("GET request GetListCanBoTruc received");
                    return Ok(list_kv);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy danh sách tất cả ghi chú   
        // Ghi chú được quét theo mã khu vực

        [HttpGet("GetListGhiChu")]
        [Authorize]
        public IActionResult GetListGhiChu(string ma_kv)
        {
            try
            {
                List<GhichuLichtruc>? list_gc = _lichtrucService.getListGhiChuLichTruc(ma_kv);
                if (list_gc != null)
                {
                    return Ok(list_gc);
                } else
                {
                    return BadRequest("An error occured");
                }
            }
            catch (Exception ex)
            {
                //_logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy danh sách lịch trực theo năm
        [HttpGet("SearchLichTrucTheoFrKey")]
        [Authorize]
        public IActionResult SearchLichTrucTheoFrKey(int fr_key)
        {
            try
            {
                var list_gc = _lichtrucService.SearchLichTrucTheoFrKey(fr_key);
                if (list_gc != null)
                {
                    return Ok(list_gc);
                }
                else
                {
                    return BadRequest("An error occured");
                }
            }
            catch (Exception ex)
            {
                //_logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }


        // Update thanh ghi chú của lịch trực.
        [HttpPut("UpdateThanhGhiChu")]
        [Authorize]
        public IActionResult UpdateThanhGhiChu(string ma_kv, string? ghiChu = "")
        {
            try
            {
                var result = _lichtrucService.updateScheduleNote(ma_kv, ghiChu).Result;
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
                //_logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Update ghi chú đơn trong bảng lịch trực.
        [HttpPut("UpdateBangGhiChu")]
        [Authorize]
        public IActionResult updateBangGhiChu(int prKey, string ghiChu)
        {
            try
            {
                var result = _lichtrucService.updateIndividualNote(prKey, ghiChu).Result;
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
                //_logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Update ghi chú đơn trong bảng lịch trực.
        [HttpPut("UpdateCanBoTruc")]
        [Authorize]
        public IActionResult updateCanBoTruc(ThemXoaCanBoTruc themXoa)
        {
            try
            {
                var result = _lichtrucService.updateSchedulePerson(themXoa).Result;
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
                //_logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

    }
}