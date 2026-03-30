using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PVI.DAO.Entities.Models;
using PVI.Helper;
using PVI.Repository.Interfaces;
using PVI.Repository.Repositories;
using PVI.Service;
using PVI.Service.ActionProcess;
using PVI.Service.Request;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace PVI.API.Web247.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HsgdDxController : ControllerBase
    {

        private readonly HsgdDxService _hsgdDxService;
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;

        public HsgdDxController(HsgdDxService hsgdDxService, Serilog.ILogger logger, IConfiguration configuration)
        {
            _hsgdDxService = hsgdDxService;
            _logger = logger;
            _configuration = configuration;
        }
        [HttpGet("GetListPhaiTraBT")]
        [Authorize]
        public async Task<IActionResult> GetListPhaiTraBT([Required] decimal pr_key_hsgd)
        {
            try
            {

                var entity = _hsgdDxService.GetListPhaiTraBT(pr_key_hsgd);
                //if (entity == null)
                //{
                //    _logger.Error("Không có dữ liệu");
                //    return BadRequest();
                //}
                _logger.Information("GET request GetListPhaiTraBT received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListPhaiTraBT An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListChiTietUocBT")]
        [Authorize]
        public async Task<IActionResult> GetListChiTietUocBT([Required] decimal hsbt_ct_pr_key)
        {
            try
            {
                var entities = await _hsgdDxService.GetListChiTietUocBT(hsbt_ct_pr_key);
                _logger.Information("GetListChiTietUocBT sussces");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListChiTietUocBT An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListPhaiTraGD")]
        [Authorize]
        public IActionResult GetListPhaiTraGD([Required] decimal pr_key_hsgd)
        {
            try
            {
                var entities = _hsgdDxService.GetListPhaiTraGD(pr_key_hsgd);
                _logger.Information("GetListPhaiTraGD sussces");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListPhaiTraGD An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListChiTietUocGD")]
        [Authorize]
        public async Task<IActionResult> GetListChiTietUocGD([Required] decimal hsbt_gd_pr_key)
        {
            try
            {
                var entities = await _hsgdDxService.GetListChiTietUocGD(hsbt_gd_pr_key);
                _logger.Information("GetListChiTietUocGD sussces");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListChiTietUocGD An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListThuDoi")]
        [Authorize]
        public IActionResult GetListThuDoi([Required] decimal pr_key_hsgd)
        {
            try
            {
                var entities = _hsgdDxService.GetListThuDoi(pr_key_hsgd);
                _logger.Information("GetListThuDoi sussces");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListThuDoi An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListPASC")]
        [Authorize]
        public IActionResult GetListPASC([Required] decimal pr_key_hsgd_dx_ct,decimal pr_key_hsgd_ctu)
        {
            try
            {
                var entities = _hsgdDxService.GetListPASC(pr_key_hsgd_dx_ct, pr_key_hsgd_ctu);
                _logger.Information("GetListPASC sussces");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListPASC An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("ReloadSum")]
        [Authorize]
        public IActionResult ReloadSum([Required] decimal pr_key_hsgd_dx_ct)
        {
            try
            {
                var entities = _hsgdDxService.ReloadSum(pr_key_hsgd_dx_ct);
                _logger.Information("ReloadSum sussces");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"ReloadSum An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpPost("CreateHsbtCt")]
        [Authorize]
        public async Task<IActionResult> CreateHsbtCt([FromBody] HsbtCtRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                else
                {
                    string checktrung = _hsgdDxService.CheckTrungHsbt(entity.PrKeyHsgdCtu, entity.hsbtCt.MaSp);
                    if (!string.IsNullOrEmpty(checktrung))
                    {
                        return BadRequest(checktrung);
                    }
                    if (entity.hsbtCt.MtnGtbh < 0 || entity.hsbtCt.TygiaBt < 0 || entity.hsbtCt.NguyenTep < 0 || entity.hsbtCt.NguyenTevp < 0 || entity.hsbtCt.SoTienp < 0 || entity.hsbtCt.MucVatp < 0 || entity.hsbtCt.SoTienvp < 0 || entity.hsbtCt.SoTienkt < 0 || entity.hsbtCt.TyleReten < 0 || entity.hsbtCt.MtnRetenVnd < 0 || entity.hsbtCt.MtnRetenNte < 0 || entity.hsbtCt.FrKey < 0 || entity.hsgdDxCt.SoTienctkh < 0 || entity.hsgdDxCt.TyleggPhutungvcx < 0 || entity.hsgdDxCt.TyleggSuachuavcx < 0 || entity.hsgdDxCt.SoTienGtbt < 0 || entity.hsgdDxCt.HieuXe < 0 || entity.hsgdDxCt.LoaiXe < 0 || entity.hsgdDxCt.NamSx < 0 || entity.hsgdDxCt.HieuXeTndsBen3 < 0 || entity.hsgdDxCt.LoaiXeTndsBen3 < 0 || entity.hsgdDxCt.ChkKhongHoadon < 0 || entity.PrKeyHsgdCtu <= 0 || entity.hsgdDxCt.SotienTtpin < 0)
                    {
                        return BadRequest();
                    }
                    if (string.IsNullOrEmpty(entity.hsbtCt.MaTtebt))
                    {
                        return BadRequest("Hồ sơ này chưa xác định loại tiền! Hãy nhập loại tiền");
                    }
                    if (entity.hsbtCt.TygiaBt == 0)
                    {
                        return BadRequest("Hồ sơ này chưa xác định tỷ giá hạch toán! Hãy nhập tỷ giá");
                    }
                    if (entity.hsbtCt.MaTtebt.Equals("VND"))
                    {
                        if (entity.hsbtCt.NguyenTep != entity.hsbtCt.SoTienp)
                        {
                            return BadRequest("Số tiền BT VND không bằng Nguyên tệ BT. Vui lòng kiểm tra lại");
                        }
                    }
                    else
                    {
                        if (entity.hsbtCt.NguyenTep != 0 && entity.hsbtCt.NguyenTep != entity.hsbtCt.SoTienp)
                        {
                            return BadRequest("Số tiền BT VND phải khác nguyên tệ BT. Vui lòng kiểm tra lại");
                        }
                    }
                    if (entity.hsbtCt.NguyenTep > entity.hsbtCt.MtnGtbh)
                    {
                        return BadRequest("Số tiền BT lớn hơn mức trách nhiệm bảo hiểm. Vui lòng kiểm tra lại");
                    }
                    if (string.IsNullOrEmpty(entity.hsbtCt.MaSp))
                    {
                        return BadRequest("Phải trả bồi thường chưa có mã sản phẩm! Hãy nhập sản phẩm");
                    }
                    if (entity.hsbtCt.MaSp.Equals("050101"))
                    {
                        if (string.IsNullOrEmpty(entity.hsbtCt.MaDkhoan))
                        {
                            return BadRequest("Phải trả bồi thường chưa có mã điều khoản! Hãy nhập mã điều khoản");
                        }
                        else
                        {
                            if (entity.hsbtCt.MaDkhoan != "05010101" && entity.hsbtCt.MaDkhoan != "05010102")
                            {
                                return BadRequest("Phải trả bồi thường có mã điều khoản không đúng(05010101 hoặc 05010102)! Hãy nhập lại mã điều khoản");
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(entity.hsbtCt.MaTtrangBt))
                    {
                        return BadRequest("Quý vị phải nhập mã tình trạng HSBT.");
                    }
                    if (entity.hsbtCt.MaTtrangBt == "03")
                    {
                        if (entity.hsbtCt.NgayHtoanBt == null)
                        {
                            return BadRequest("Quý vị phải nhập ngày duyệt bồi thường.");
                        }
                    }
                    else if (entity.hsbtCt.MaTtrangBt == "04")
                    {
                        if (entity.hsbtCt.NgayHtoanBt == null)
                        {
                            return BadRequest("Quý vị phải nhập ngày từ chối bồi thường.");
                        }
                    }
                    else if (entity.hsbtCt.MaTtrangBt == "05")
                    {
                    }
                    else
                    {
                        if (entity.hsbtCt.NgayHtoanBt != null)
                        {
                            return BadRequest("Quý vị phải xóa ngày duyệt bồi thường.");
                        }
                    }
                }
                //var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = await _hsgdDxService.CreateHsbtCt(entity);
                _logger.Information("POST request CreateHsbtCt received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"CreateHsbtCt An error occured: {ex}");
                _logger.Error("CreateHsbtCt Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }
        [HttpPut("UpdateHsbtCt")]
        [Authorize]
        public async Task<IActionResult> UpdateHsbtCt([FromBody] HsbtCtRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                else
                {
                    string checktrung = _hsgdDxService.CheckTrungHsbt(entity.PrKeyHsgdCtu, entity.hsbtCt.MaSp);
                    if (!string.IsNullOrEmpty(checktrung))
                    {
                        return BadRequest(checktrung);
                    }
                    if (entity.hsbtCt.MtnGtbh < 0 || entity.hsbtCt.TygiaBt < 0 || entity.hsbtCt.NguyenTep < 0 || entity.hsbtCt.NguyenTevp < 0 || entity.hsbtCt.SoTienp < 0 || entity.hsbtCt.MucVatp < 0 || entity.hsbtCt.SoTienvp < 0 || entity.hsbtCt.SoTienkt < 0 || entity.hsbtCt.TyleReten < 0 || entity.hsbtCt.MtnRetenVnd < 0 || entity.hsbtCt.MtnRetenNte < 0 || entity.hsbtCt.FrKey < 0 || entity.hsgdDxCt.SoTienctkh < 0 || entity.hsgdDxCt.TyleggPhutungvcx < 0 || entity.hsgdDxCt.TyleggSuachuavcx < 0 || entity.hsgdDxCt.SoTienGtbt < 0 || entity.hsgdDxCt.HieuXe < 0 || entity.hsgdDxCt.LoaiXe < 0 || entity.hsgdDxCt.NamSx < 0 || entity.hsgdDxCt.HieuXeTndsBen3 < 0 || entity.hsgdDxCt.LoaiXeTndsBen3 < 0 || entity.hsgdDxCt.ChkKhongHoadon < 0 || entity.PrKeyHsgdCtu <= 0 || entity.hsgdDxCt.SotienTtpin < 0)
                    {
                        return BadRequest();
                    }
                    if (string.IsNullOrEmpty(entity.hsbtCt.MaTtebt))
                    {
                        return BadRequest("Hồ sơ này chưa xác định loại tiền! Hãy nhập loại tiền");
                    }
                    if (entity.hsbtCt.TygiaBt == 0)
                    {
                        return BadRequest("Hồ sơ này chưa xác định tỷ giá hạch toán! Hãy nhập tỷ giá");
                    }
                    if (entity.hsbtCt.MaTtebt.Equals("VND"))
                    {
                        if (entity.hsbtCt.NguyenTep != entity.hsbtCt.SoTienp)
                        {
                            return BadRequest("Số tiền BT VND không bằng Nguyên tệ BT. Vui lòng kiểm tra lại");
                        }
                    }
                    else
                    {
                        if (entity.hsbtCt.NguyenTep != 0 && entity.hsbtCt.NguyenTep != entity.hsbtCt.SoTienp)
                        {
                            return BadRequest("Số tiền BT VND phải khác nguyên tệ BT. Vui lòng kiểm tra lại");
                        }
                    }
                    if (entity.hsbtCt.NguyenTep > entity.hsbtCt.MtnGtbh)
                    {
                        return BadRequest("Số tiền BT lớn hơn mức trách nhiệm bảo hiểm. Vui lòng kiểm tra lại");
                    }
                    if (string.IsNullOrEmpty(entity.hsbtCt.MaSp))
                    {
                        return BadRequest("Phải trả bồi thường chưa có mã sản phẩm! Hãy nhập sản phẩm");
                    }
                    if (entity.hsbtCt.MaSp.Equals("050101"))
                    {
                        if (string.IsNullOrEmpty(entity.hsbtCt.MaDkhoan))
                        {
                            return BadRequest("Phải trả bồi thường chưa có mã điều khoản! Hãy nhập mã điều khoản");
                        }
                        else
                        {
                            if (entity.hsbtCt.MaDkhoan != "05010101" && entity.hsbtCt.MaDkhoan != "05010102")
                            {
                                return BadRequest("Phải trả bồi thường có mã điều khoản không đúng(05010101 hoặc 05010102)! Hãy nhập lại mã điều khoản");
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(entity.hsbtCt.MaTtrangBt))
                    {
                        return BadRequest("Quý vị phải nhập mã tình trạng HSBT.");
                    }
                    if (entity.hsbtCt.MaTtrangBt == "03")
                    {
                        if (entity.hsbtCt.NgayHtoanBt == null)
                        {
                            return BadRequest("Quý vị phải nhập ngày duyệt bồi thường.");
                        }
                    }
                    else if (entity.hsbtCt.MaTtrangBt == "04")
                    {
                        if (entity.hsbtCt.NgayHtoanBt == null)
                        {
                            return BadRequest("Quý vị phải nhập ngày từ chối bồi thường.");
                        }
                    }
                    else if (entity.hsbtCt.MaTtrangBt == "05")
                    {
                    }
                    else
                    {
                        if (entity.hsbtCt.NgayHtoanBt != null)
                        {
                            return BadRequest("Quý vị phải xóa ngày duyệt bồi thường.");
                        }
                    }
                    var allowExt = new HashSet<string> { "pdf", "xml", "png", "jpg", "doc", "xls", "docx", "xlsx" };

                    if (entity.fileAttachBts == null || !entity.fileAttachBts.Any())
                    {
                        return BadRequest("Không có file đính kèm");
                    }

                    var invalidFiles = entity.fileAttachBts
                        .Select(x => new
                        {
                            FileName = x.FileName,
                            Ext = Path.GetExtension(x.FileName)?.Replace(".", "").ToLower()
                        })
                        .Where(x => string.IsNullOrWhiteSpace(x.Ext) || !allowExt.Contains(x.Ext))
                        .ToList();

                    if (invalidFiles.Any())
                    {
                        var fileNames = string.Join(", ", invalidFiles.Select(x => x.FileName));

                        return BadRequest($"Các file không hợp lệ: {fileNames}. Chỉ chấp nhận: pdf, xml, png, jpg, doc, xls, docx, xlsx");
                    }


                    //haipv1 05/01/2026 kiểm tra phải import đúng đơn vị xuất hóa đơn
                    if (entity.fileAttachBts.Any())
                    {
                        // kiểm tra có file XML hay không
                        string masovat_hoadon = "";
                        decimal so_tienpxml = 0m;
                        decimal so_tienvpxml = 0;
                        try
                        {
                            FileAttachBtRequest? xmlFile = entity.fileAttachBts?
                            .FirstOrDefault(f => f.FileData != "" &&
                                !string.IsNullOrWhiteSpace(f.FileExtension) &&
                                f.FileExtension.Equals(".xml", StringComparison.OrdinalIgnoreCase)
                            );
                            if (xmlFile != null)
                            {
                                string base64Xml = xmlFile.FileData;
                                byte[] xmlBytes = Convert.FromBase64String(base64Xml);
                                XDocument doc;
                                using (var ms = new MemoryStream(xmlBytes)) // xmlBytes là byte[] sau khi decode Base64
                                using (var reader = XmlReader.Create(ms, new XmlReaderSettings
                                {
                                    IgnoreComments = true,
                                    IgnoreWhitespace = true,
                                    DtdProcessing = DtdProcessing.Ignore
                                }))
                                {
                                    doc = XDocument.Load(reader);
                                }

                                XNamespace ns = doc.Root.GetDefaultNamespace();                               
                                masovat_hoadon=doc.Element(ns + "HDon")?
                                .Element(ns + "DLHDon")?
                                .Element(ns + "NDHDon")?
                                .Element(ns + "NMua")?
                                .Element(ns + "MST")?
                                .Value ?? "";

                                ////Lấy số tiền bồi thường
                                decimal.TryParse(
                                    doc.Element(ns + "HDon")?
                                       .Element(ns + "DLHDon")?
                                       .Element(ns + "NDHDon")?
                                       .Element(ns + "TToan")?
                                       .Element(ns + "TgTCThue")?
                                       .Value,
                                    out so_tienpxml);                                

                                if (entity.hsbtCt.SoTienp != 0 && so_tienpxml != 0 && entity.hsbtCt.SoTienp > so_tienpxml)
                                {
                                    _logger.Error($"check xml pr_key_hsgd: {entity.PrKeyHsgdCtu} entity.hsbtCt.SoTienp:{entity.hsbtCt.SoTienp} so_tienpxml: {so_tienpxml}");
                                    return BadRequest("Số tiền bồi thường=" + entity.hsbtCt.SoTienp.ToString("N0") + " không được nhập lớn hơn số tiền trên file xml import=" + so_tienpxml.ToString("N0") + ".");
                                }
                                if (masovat_hoadon != "")
                                {
                                    List<string> checkmst = _hsgdDxService.Lay_mst_donvicapdon(entity.PrKeyHsgdCtu);
                                    if (checkmst != null)
                                    {
                                        bool isValid = checkmst
                                            .Any(x => x.Replace("-", "").Replace(".", "").Replace("_", "").Trim() == masovat_hoadon.Replace("-", "").Replace(".", "").Replace("_", "").Trim());

                                        if (!isValid)
                                        {
                                            return BadRequest("Mã số thuế trên hóa đơn không đúng với mã số thuế đơn vị cấp đơn hoặc đơn vị bồi thường hộ, hãy kiểm tra lại.");
                                        }
                                    }
                                }

                            }
                           
                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"UpdateHsbtCt An error occured check xml pr_key_hsgd: {entity.PrKeyHsgdCtu} ex: { ex}");
                            return BadRequest("Có lỗi xảy ra trong quá trình kiểm tra dữ liệu XML!");
                        }
                          
                    }    
                }
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = await _hsgdDxService.UpdateHsbtCt(entity);
                _logger.Information("POST request UpdateHsbtCt received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"UpdateHsbtCt An error occured: {ex}");
                _logger.Error("UpdateHsbtCt Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }
        [HttpDelete("DeleteHsbtCt")]
        [Authorize]
        public async Task<IActionResult> DeleteHsbtCt(decimal pr_key)
        {
            try
            {
                var result = await _hsgdDxService.DeleteHsbtCt(pr_key);
                _logger.Information("DeleteHsbtCt received:" + result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"DeleteHsbtCt An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost("CreateHsbtGd")]
        [Authorize]
        public async Task<IActionResult> CreateHsbtGd([FromBody] HsbtGdRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                else
                {
                    if (entity.TygiaGd < 0 || entity.NguyenTegd < 0 || entity.SoTiengd < 0 || entity.MucVat < 0 || entity.NguyenTev < 0 || entity.SoTienv < 0 || entity.TyleReten < 0 || entity.MtnRetenNte < 0 || entity.MtnRetenVnd < 0 || entity.PrKeyHsgdCtu <= 0 || entity.FrKey < 0)
                    {
                        return BadRequest();
                    }
                    if (string.IsNullOrEmpty(entity.MaSp))
                    {
                        return BadRequest("Chi tiết giám định chưa có mã sản phẩm! Hãy nhập sản phẩm");
                    }
                    if (string.IsNullOrEmpty(entity.MaDvgd))
                    {
                        return BadRequest("Chi tiết giám định chưa có mã công ty giám định! Hãy nhập lại");
                    }
                    if (string.IsNullOrEmpty(entity.MaLoaiChiphi))
                    {
                        return BadRequest("Quý vị phải nhập mã loại chi phí giám định.");
                    }
                    if (string.IsNullOrEmpty(entity.MaTtegd))
                    {
                        return BadRequest("Chi tiết giám định chưa có mã tiền tệ! Hãy nhập lại");
                    }
                    if (entity.TygiaGd <= 0)
                    {
                        return BadRequest("Chi tiết giám định chưa xác định đúng tỷ giá! Hãy nhập lại");
                    }
                    if (string.IsNullOrEmpty(entity.MaTtrangGd))
                    {
                        return BadRequest("Quý vị phải nhập mã trạng thái giám định.");
                    }
                    if (entity.MaTtrangGd == "03")
                    {
                        if (entity.NgayHtoanGd == null)
                        {
                            return BadRequest("Quý vị phải nhập ngày duyệt giám định.");
                        }
                    }
                    else if (entity.MaTtrangGd == "04")
                    {
                        if (entity.NgayHtoanGd == null)
                        {
                            return BadRequest("Quý vị phải nhập ngày từ chối giám định.");
                        }
                    }
                    else
                    {
                        if (entity.NgayHtoanGd != null)
                        {
                            return BadRequest("Quý vị phải xóa ngày duyệt giám định.");
                        }
                    }
                }
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = await _hsgdDxService.CreateHsbtGd(entity);
                _logger.Information("POST request CreateHsbtGd received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"CreateHsbtGd An error occured: {ex}");
                _logger.Error("CreateHsbtGd Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }
        [HttpPut("UpdateHsbtGd")]
        [Authorize]
        public async Task<IActionResult> UpdateHsbtGd([FromBody] HsbtGdRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                else
                {
                    if (entity.TygiaGd < 0 || entity.NguyenTegd < 0 || entity.SoTiengd < 0 || entity.MucVat < 0 || entity.NguyenTev < 0 || entity.SoTienv < 0 || entity.TyleReten < 0 || entity.MtnRetenNte < 0 || entity.MtnRetenVnd < 0 || entity.PrKeyHsgdCtu <= 0 || entity.FrKey < 0)
                    {
                        return BadRequest();
                    }
                    if (string.IsNullOrEmpty(entity.MaSp))
                    {
                        return BadRequest("Chi tiết giám định chưa có mã sản phẩm! Hãy nhập sản phẩm");
                    }
                    if (string.IsNullOrEmpty(entity.MaDvgd))
                    {
                        return BadRequest("Chi tiết giám định chưa có mã công ty giám định! Hãy nhập lại");
                    }
                    if (string.IsNullOrEmpty(entity.MaLoaiChiphi))
                    {
                        return BadRequest("Quý vị phải nhập mã loại chi phí giám định.");
                    }
                    if (string.IsNullOrEmpty(entity.MaTtegd))
                    {
                        return BadRequest("Chi tiết giám định chưa có mã tiền tệ! Hãy nhập lại");
                    }
                    if (entity.TygiaGd <= 0)
                    {
                        return BadRequest("Chi tiết giám định chưa xác định đúng tỷ giá! Hãy nhập lại");
                    }
                    if (string.IsNullOrEmpty(entity.MaTtrangGd))
                    {
                        return BadRequest("Quý vị phải nhập mã trạng thái giám định.");
                    }
                    if (entity.MaTtrangGd == "03")
                    {
                        if (entity.NgayHtoanGd == null)
                        {
                            return BadRequest("Quý vị phải nhập ngày duyệt giám định.");
                        }
                    }
                    else if (entity.MaTtrangGd == "04")
                    {
                        if (entity.NgayHtoanGd == null)
                        {
                            return BadRequest("Quý vị phải nhập ngày từ chối giám định.");
                        }
                    }
                    else
                    {
                        if (entity.NgayHtoanGd != null)
                        {
                            return BadRequest("Quý vị phải xóa ngày duyệt giám định.");
                        }
                    }
                }
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = await _hsgdDxService.UpdateHsbtGd(entity);
                _logger.Information("POST request UpdateHsbtGd received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"UpdateHsbtGd An error occured: {ex}");
                _logger.Error("UpdateHsbtGd Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }
        [HttpDelete("DeleteHsbtGd")]
        [Authorize]
        public async Task<IActionResult> DeleteHsbtGd(decimal pr_key)
        {
            try
            {
                var result = await _hsgdDxService.DeleteHsbtGd(pr_key);
                _logger.Information("DeleteHsbtGd received:" + result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"DeleteHsbtGd An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost("CreateHsbtThts")]
        [Authorize]
        public async Task<IActionResult> CreateHsbtThts([FromBody] HsbtThtsRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                else
                {
                    if (entity.TygiaTd < 0 || entity.NguyenTeTd < 0 || entity.SoTienTd < 0 || entity.TyleReten < 0 || entity.MtnRetenVnd < 0 || entity.MtnRetenNte < 0 || entity.PrKeyHsgdCtu <= 0 || entity.FrKey < 0)
                    {
                        return BadRequest();
                    }
                    if (string.IsNullOrEmpty(entity.MaSp))
                    {
                        return BadRequest("Chi tiết thanh lý TS & thu đòi NT3 chưa có mã sản phẩm! Hãy nhập sản phẩm");
                    }
                    if (string.IsNullOrEmpty(entity.LoaiHinhtd))
                    {
                        return BadRequest("Chi tiết giám định chưa có loại hình thu đòi! Hãy nhập lại");
                    }
                    if (string.IsNullOrEmpty(entity.MaTte))
                    {
                        return BadRequest("Chi tiết thanh lý TS & thu đòi NT3 chưa có mã tiền tệ! Hãy nhập lại");
                    }
                    if (entity.TygiaTd <= 0)
                    {
                        return BadRequest("Chi tiết thanh lý TS & thu đòi NT3 chưa xác định đúng tỷ giá! Hãy nhập lại");
                    }
                    if (string.IsNullOrEmpty(entity.MaTtrangTd))
                    {
                        return BadRequest("Quý vị phải nhập mã tình trạng thu đòi.");
                    }
                    if (entity.MaTtrangTd == "03")
                    {
                        if (entity.NgayHtoanTd == null)
                        {
                            return BadRequest("Quý vị phải nhập ngày duyệt thu đòi.");
                        }
                    }
                    else if (entity.MaTtrangTd == "04")
                    {
                        if (entity.NgayHtoanTd == null)
                        {
                            return BadRequest("Quý vị phải nhập ngày từ chối thu đòi.");
                        }
                    }
                    else
                    {
                        if (entity.NgayHtoanTd != null)
                        {
                            return BadRequest("Quý vị phải xóa ngày duyệt thu đòi.");
                        }
                    }
                    if (entity.NguyenTeTd != 0 && entity.SoTienTd == 0)
                    {
                        return BadRequest("Số tiền thu hồi tài sản, thu đòi người thứ ba không hợp lệ.");
                    }
                    if (entity.NguyenTeTd == 0 && entity.SoTienTd != 0)
                    {
                        return BadRequest("Nguyên tệ thu hồi tài sản, thu đòi người thứ ba không hợp lệ.");
                    }
                }
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = await _hsgdDxService.CreateHsbtThts(entity);
                _logger.Information("POST request CreateHsbtThts received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"CreateHsbtThts An error occured: {ex}");
                _logger.Error("CreateHsbtThts Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }
        [HttpPut("UpdateHsbtThts")]
        [Authorize]
        public async Task<IActionResult> UpdateHsbtThts([FromBody] HsbtThtsRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                else
                {
                    if (entity.TygiaTd < 0 || entity.NguyenTeTd < 0 || entity.SoTienTd < 0 || entity.TyleReten < 0 || entity.MtnRetenVnd < 0 || entity.MtnRetenNte < 0 || entity.PrKeyHsgdCtu <= 0 || entity.FrKey < 0)
                    {
                        return BadRequest();
                    }
                    if (string.IsNullOrEmpty(entity.MaSp))
                    {
                        return BadRequest("Chi tiết thanh lý TS & thu đòi NT3 chưa có mã sản phẩm! Hãy nhập sản phẩm");
                    }
                    if (string.IsNullOrEmpty(entity.LoaiHinhtd))
                    {
                        return BadRequest("Chi tiết giám định chưa có loại hình thu đòi! Hãy nhập lại");
                    }
                    if (string.IsNullOrEmpty(entity.MaTte))
                    {
                        return BadRequest("Chi tiết thanh lý TS & thu đòi NT3 chưa có mã tiền tệ! Hãy nhập lại");
                    }
                    if (entity.TygiaTd <= 0)
                    {
                        return BadRequest("Chi tiết thanh lý TS & thu đòi NT3 chưa xác định đúng tỷ giá! Hãy nhập lại");
                    }
                    if (string.IsNullOrEmpty(entity.MaTtrangTd))
                    {
                        return BadRequest("Quý vị phải nhập mã tình trạng thu đòi.");
                    }
                    if (entity.MaTtrangTd == "03")
                    {
                        if (entity.NgayHtoanTd == null)
                        {
                            return BadRequest("Quý vị phải nhập ngày duyệt thu đòi.");
                        }
                    }
                    else if (entity.MaTtrangTd == "04")
                    {
                        if (entity.NgayHtoanTd == null)
                        {
                            return BadRequest("Quý vị phải nhập ngày từ chối thu đòi.");
                        }
                    }
                    else
                    {
                        if (entity.NgayHtoanTd != null)
                        {
                            return BadRequest("Quý vị phải xóa ngày duyệt thu đòi.");
                        }
                    }
                    if (entity.NguyenTeTd != 0 && entity.SoTienTd == 0)
                    {
                        return BadRequest("Số tiền thu hồi tài sản, thu đòi người thứ ba không hợp lệ.");
                    }
                    if (entity.NguyenTeTd == 0 && entity.SoTienTd != 0)
                    {
                        return BadRequest("Nguyên tệ thu hồi tài sản, thu đòi người thứ ba không hợp lệ.");
                    }
                }
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = await _hsgdDxService.UpdateHsbtThts(entity);
                _logger.Information("POST request UpdateHsbtThts received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"UpdateHsbtThts An error occured: {ex}");
                _logger.Error("UpdateHsbtThts Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }
        [HttpDelete("DeleteHsbtThts")]
        [Authorize]
        public async Task<IActionResult> DeleteHsbtThts(decimal pr_key)
        {
            try
            {
                var result = await _hsgdDxService.DeleteHsbtThts(pr_key);
                _logger.Information("DeleteHsbtThts received:" + result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"DeleteHsbtThts An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost("CreatePASC")]
        [Authorize]
        public IActionResult CreatePASC([FromBody] HsbtDxRequest entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                else
                {
                    if (entity.hsgdDx.Where(x=>x.SoTientt < 0 || x.SoTienph < 0 || x.SoTienson < 0 || x.SoTiensc < 0 || x.SoTienDoitru < 0  || x.VatSc < 0 || x.GiamTruBt < 0).Count() > 0 || entity.hsgdDxCt.SoTienctkh < 0 || entity.hsgdDxCt.TyleggSuachuavcx < 0 || entity.hsgdDxCt.TyleggPhutungvcx < 0 || entity.hsgdDxCt.SoTienGtbt < 0 || entity.hsgdDxCt.HieuXe < 0 || entity.hsgdDxCt.LoaiXe < 0 || entity.hsgdDxCt.NamSx < 0 || entity.hsgdDxCt.HieuXeTndsBen3 < 0 || entity.hsgdDxCt.LoaiXeTndsBen3 < 0 || entity.hsgdDxCt.SotienTtpin < 0)
                    {
                        return BadRequest();
                    }
                }
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _hsgdDxService.CreatePASC(entity);
                _logger.Information("POST request CreatePASC received");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"CreatePASC An error occured: {ex}");
                _logger.Error("CreatePASC Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }
        [HttpPut("ImportPASC")]
        [Authorize]
        public async Task<IActionResult> ImportPASC([FromBody] List<ImportPASCRequest> entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = await _hsgdDxService.ImportPASC(entity);
                _logger.Information("POST request ImportPASC received");
                if (result == "Thành công")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"ImportPASC An error occured: {ex}");
                _logger.Error("ImportPASC Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }
        [HttpGet("PrintPASC")]
        [Authorize]
        public IActionResult PrintPASC(decimal pr_key_hsbt_ct, decimal pr_key_hsgd_ctu, int loai_dx)
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //var email = "quyenvm@pvi.com.vn";
                var result =  _hsgdDxService.PrintPASC(pr_key_hsbt_ct, pr_key_hsgd_ctu, email, loai_dx);

                _logger.Information("PrintPASC success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"PrintPASC An error occurred: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost("CreatFilePasc")]
        //[Authorize]
        public bool CreatFilePasc(decimal pr_key_hsgd_ctu, string email_login)
        {
            try
            {
                var result =  _hsgdDxService.CreatFilePasc(pr_key_hsgd_ctu, email_login);

                _logger.Information("CreatFilePasc success");
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error($"CreatFilePasc An error occured: {ex}");
                return false;
            }
        }
        [HttpGet("GuiPASC")]
        [Authorize]
        public IActionResult GuiPASC(decimal pr_key_hsbt_ct, decimal pr_key_hsgd_ctu, bool chk_send_pasc, bool pasc_send_sms, string email_nhan, string phone_nhan)
        {
            try
            {
                var email_login = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //var email_login = "huynhchinh@pvi.com.vn";
                var result =_hsgdDxService.GuiPASC(pr_key_hsbt_ct,pr_key_hsgd_ctu,chk_send_pasc, pasc_send_sms,email_nhan,phone_nhan, email_login);
                _logger.Information("GuiPASC pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + ", pr_key_hsbt_ct = "+ pr_key_hsbt_ct + " success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"GuiPASC pr_key_hsgd_ctu = "+ pr_key_hsgd_ctu + ", pr_key_hsbt_ct = " + pr_key_hsbt_ct + " An error occurred: " + ex);
                return BadRequest("Có lỗi xảy ra. Hãy thử lại sau.");
            }

        }
        [HttpPost("LichSuPasc")]
        [Authorize]
        public async Task<IActionResult> LichSuPasc(LichsuPaParameters parameters)
        {
            try
            {
                var result = await _hsgdDxService.LichSuPasc(parameters);
                var metadata = new
                {

                    result.TotalCount,
                    result.PageSize,
                    result.CurrentPage,
                    result.TotalPages,
                    result.HasNext,
                    result.HasPrevious
                };
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.Information("LichSuPasc success");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"LichSuPasc An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetListDonViGiamDinh")]
        [Authorize]
        public async Task<IActionResult> GetListDonViGiamDinh()
        {
            try
            {
                var email_login = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //var email_login = "huynhchinh@pvi.com.vn";
                var entities = await _hsgdDxService.GetListDonViGiamDinh(email_login);
                _logger.Information("GetListDonViGiamDinh success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListDonViGiamDinh An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("GetFileAttachBt")]
        [Authorize]
        public IActionResult GetFileAttachBt([Required] decimal pr_key_hsbt_ct, [Required] string ma_ctu)
        {
            try
            {
                var entities = _hsgdDxService.GetFileAttachBt(pr_key_hsbt_ct, ma_ctu);
                _logger.Information("GetFileAttachBt sussces");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetFileAttachBt An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpPost("DownloadFileAttachBt_MDF1")]
        [Authorize]
        public IActionResult DownloadFileAttachBt_MDF1(decimal pr_key)
        {
            try
            {
                var result = _hsgdDxService.DownloadFileAttachBt_MDF1(pr_key);
                _logger.Information("DownloadFileAttachBt_MDF1 Completed");
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.Error($"DownloadFileAttachBt_MDF1 An error occured: {ex}");
                return BadRequest("An error occured");
            }



        }
        [HttpGet("GetSTBTByHsgd")]
        [Authorize]
        public IActionResult GetSTBTByHsgd([Required] decimal pr_key_hsbt_ctu)
        {
            try
            {
                var entities = _hsgdDxService.GetSTBTByHsgd(pr_key_hsbt_ctu);
                _logger.Information("GetSTBTByHsgd sussces");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetSTBTByHsgd An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpGet("PrintThongBaoBT")]
        [Authorize]
        public IActionResult PrintThongBaoBT(decimal pr_key_hsgd_ctu, bool pdf_file)
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //var email = "lanlt@pvi.com.vn";
                var result = _hsgdDxService.PrintThongBaoBT(pr_key_hsgd_ctu, email, pdf_file);

                _logger.Information("PrintThongBaoBT success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"PrintThongBaoBT An error occurred: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpGet("GetListLoaiDongCo")]
        [Authorize]
        public async Task<IActionResult> GetListLoaiDongCo()
        {
            try
            {
                var entities = await _hsgdDxService.GetListLoaiDongCo();
                _logger.Information("GetListLoaiDongCo success");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }
        [HttpPost("GuiThongBaoBT")]
        [Authorize]
        public IActionResult GuiThongBaoBT([FromBody] GuiThongBaoBTRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Request body không được để trống" });
                }

                if (string.IsNullOrEmpty(request.EmailNhan))
                {
                    return BadRequest(new { success = false, message = "Email nhận không được để trống" });
                }

                var email_login = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _hsgdDxService.GuiThongBaoBT(request.PrKeyHsgdCtu, request.EmailNhan, email_login);

                // Kiểm tra kết quả
                if (result.Success)
                {
                    _logger.Information($"GuiThongBaoBT pr_key_hsgd_ctu = {request.PrKeyHsgdCtu} success");
                    return Ok(new { success = true, message = result.Message, data = result.Data });
                }
                else
                {
                    return BadRequest(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"GuiThongBaoBT pr_key_hsgd_ctu = {request?.PrKeyHsgdCtu}, An error occurred: {ex}");
                return BadRequest(new { success = false, message = "Có lỗi xảy ra, vui lòng liên hệ IT" });
            }
        }
        [HttpGet("GetPrintPASC")]
        [Authorize]
        public IActionResult GetPrintPASC(decimal pr_key_hsbt_ct, decimal pr_key_hsgd_ctu, int loai_dx)
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //var email = "quyenvm@pvi.com.vn";
                var result = _hsgdDxService.GetPrintPASC(pr_key_hsbt_ct, pr_key_hsgd_ctu, email, loai_dx);

                _logger.Information("GetPrintPASC success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetPrintPASC An error occurred: {ex}");
                return BadRequest("An error occured");
            }
        }
        [HttpPost("PheDuyetTbaoBT")]
        [Authorize]
        public async Task<IActionResult> PheDuyetTbaoBT(int pr_key)
        {
            try
            {
                string currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = await _hsgdDxService.PheDuyetTBBT(pr_key, currentUserEmail);
                if (!result.Success)
                {
                    return Ok(new { success = false, message = result.Message });
                }

                try
                {
                    bool kysoSuccess = await _hsgdDxService.KySoTBBT(pr_key, currentUserEmail);
                    if (kysoSuccess)
                    {
                        return Ok(new
                        {
                            success = true,
                            message = "Phê duyệt và ký số thông báo bồi thường thành công",
                            data = result.Data
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            success = true,
                            message = "Phê duyệt thành công. Tuy nhiên, ký số gặp lỗi, vui lòng kiểm tra lại",
                            data = result.Data,
                            warning = "Ký số không thành công"
                        });
                    }
                }
                catch (Exception kysoEx)
                {
                    _logger.Error($"Lỗi khi ký số TBBT: {kysoEx}");
                    return Ok(new
                    {
                        success = true,
                        message = "Phê duyệt thành công. Tuy nhiên, ký số gặp lỗi, vui lòng liên hệ IT",
                        data = result.Data,
                        warning = "Có lỗi xảy ra khi ký số"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured at PheDuyetTbaoBT: {ex}");
                return BadRequest(new
                {
                    success = false,
                    message = "Có lỗi xảy ra, vui lòng liên hệ IT. Mã Lỗi: Exception_At_DUYETBAOLANH"
                });
            }
        }
        [HttpGet("DownloadFile")]
        [Authorize]
        public IActionResult DownloadFile([Required] string filePath)
        {
            try
            {
                var result = _hsgdDxService.DownloadFile(filePath);

                if (result == null || result.Status == "-500" || result.Status == "-400")
                {
                    return BadRequest(result?.Message ?? "Download failed");
                }

                _logger.Information("DownloadFile completed successfully");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"DownloadFile An error occurred: {ex}");
                return BadRequest("An error occurred while downloading the file");
            }
        }

    }
}