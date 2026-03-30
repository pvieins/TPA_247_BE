using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PVI.Service.Request;
using PVI.Repository.Repositories;
using PVI.Service;
using PVI.Helper;
using PVI.Service.ActionProcess;
using System.ComponentModel.DataAnnotations;
using PVI.DAO.Entities.Models;
using PVI.Repository.Interfaces;
using PVI.IAM.BE.Services;

namespace PVI.API.Web247.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DmUserController : ControllerBase
    {

        private readonly DmUserService _dmUserService;
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;

        public DmUserController(DmUserService dmUserService, Serilog.ILogger logger, IConfiguration configuration)
        {
            _dmUserService = dmUserService;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet("GetListGiamDV")]
        [Authorize]
        public IActionResult GetListGiamDV()
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);

                var entity = _dmUserService.GetListGiamDV(email);
                _logger.Information("GET request GetListGiamDV received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListGiamDV An error occurred: {ex}");
                return BadRequest("An error occurred");
            }

        }

        [HttpGet("GetListCanBoTT")]
        [Authorize]
        public IActionResult GetListCanBoTT()
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);

                var entity = _dmUserService.GetListCanBoTT(email);
                _logger.Information("GET request GetListCanBoTT received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListCanBoTT An error occurred: {ex}");
                return BadRequest("An error occurred");
            }

        }
        [HttpGet("GetListCanBoDuyet")]
        [Authorize]
        public IActionResult GetListCanBoDuyet()
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);

                var entity = _dmUserService.GetListCanBoDuyet(email);
                _logger.Information("GET request GetListCanBoDuyet received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListCanBoDuyet An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpGet("GetListDoiTruong")]
        [Authorize]
        public IActionResult GetListDoiTruong()
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);

                var entity = _dmUserService.GetListDoiTruong(email);
                _logger.Information("GET request GetListCanBoDuyet received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListCanBoDuyet An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }

        // Danh sách các loại user.
        [HttpGet("GetListLoaiUser")]
        [Authorize]
        public IActionResult GetListLoaiUser()
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);

                var entity = _dmUserService.getDMLoaiUser();
                _logger.Information("GET request GetListLoaiUser received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListLoaiUser An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }

        // Danh sách các đơn vị.
        [HttpGet("GetListDonVi")]
        [Authorize]
        public IActionResult GetListDonVI()
        {
            try
            {
                var currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);

                var entity = _dmUserService.getDMDonvi(currentUserEmail);
                _logger.Information("GET request GetListDonVi received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListDonvi An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }

        // Lấy danh sách tất cả các User GDTT

        [HttpGet("GetListUserGDTT")]
        [Authorize]
        public IActionResult GetListUserGDTT(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var entity = _dmUserService.getListUserGDTT(pageNumber, pageSize, currentUserEmail);
                _logger.Information("GET request GetListUserGDTT received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListUserGDTT An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }

        // Lấy danh sách tất cả các User GDDK
        [HttpGet("GetListUserGDDK")]
        [Authorize]
        public IActionResult GetListUserGDDK(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var entity = _dmUserService.getListUserGDDK(pageNumber, pageSize, currentUserEmail);
                _logger.Information("GET request GetListUserGDDK received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListUserGDDK An error occurred: {ex}");
                return BadRequest("An error occurred");
            }

        }

        // Lấy danh sách tất cả các user GDTT
        [HttpPost("SearchFilterUserGDTT")]
        [Authorize]
        // Có thể thêm filter vào để xuất thông tin theo trường.
        public IActionResult searchUserGDTT([FromBody] UserGDTT searchTarget, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //string currentUserEmail = "longnh4@pvi.com.vn";
                var list_user = _dmUserService.searchUserGDTT(pageNumber, pageSize, searchTarget, currentUserEmail);
                if (list_user == null)
                {
                    _logger.Error("Lỗi khi search user.");
                    return NotFound();
                }
                _logger.Information("GET request SearchFilterUserGDTT received");
                return Ok(list_user);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy danh sách tất cả các user GDDK
        [HttpPost("SearchFilterUserGDDK")]
        [Authorize]
        // Có thể thêm filter vào để xuất thông tin theo trường.
        public IActionResult searchUserGDDK([FromBody] UserGDDK searchTarget, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                //string currentUserEmail = "longnh6@pvi.com.vn";
                var list_user = _dmUserService.searchUserGDDK(pageNumber, pageSize, searchTarget, currentUserEmail);
                if (list_user == null)
                {
                    _logger.Error("Lỗi khi search user.");
                    return NotFound();
                }
                _logger.Information("GET request SearchFilterUserGDDK received");
                return Ok(list_user);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        // Lấy thông tin người dùng PIAS từ email
        [HttpGet("GetUserPiasFromEmail")]
        [Authorize]
        // Có thể thêm filter vào để xuất thông tin theo trường.
        public IActionResult GetUserPiasFromEmail(string userEmail)
        {
            try
            {
                DmUser userInfo = _dmUserService.getUserPiasFromEmail(userEmail);
                if (userInfo.MaUser == "" || userInfo.MaUser == null)
                {
                    //_logger.Error("Lỗi khi search user.");
                    return BadRequest("An error occured");
                }
                else
                {
                    //_logger.Information("GET request SearchFilterUserGDDK received");
                    return Ok(userInfo);
                }
            }
            catch (Exception ex)
            {
                //_logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }

        [HttpGet("GetListUserPiasFromDonvi")]
        [Authorize]
        // Có thể thêm filter vào để xuất thông tin theo trường.
        public IActionResult GetListUserPiasFromDonvi(int pageNumber, int pageSize)
        {
            try
            {
                List<DmUser> userInfo = _dmUserService.getListUserPiasFromDonvi(pageNumber, pageSize);
                if (userInfo.Count == 0)
                {
                    //_logger.Error("Lỗi khi search user.");
                    return BadRequest("An error occured");
                }
                else
                {
                    //_logger.Information("GET request SearchFilterUserGDDK received");
                    return Ok(userInfo);
                }
            }
            catch (Exception ex)
            {
                //_logger.Error($"An error occured: {ex}");
                return BadRequest("An error occured");
            }
        }


        // Tạo User GDTT
        // Trả vè mã User mới nếu thành công, hoặc báo lỗi nếu thất bại.

        [HttpPost("CreateUserGDTT")]
        [Authorize]
        public async Task<IActionResult> createUserGDTT([FromBody] UserGDTT entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                else
                {
                    if (entity.LoaiUser < 0)
                    {
                        return BadRequest();
                    }
                }
                var currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = await _dmUserService.createUserGDTT(entity, currentUserEmail);
                _logger.Information("POST request CreateUserGDTT received");
                if (result.Length < 11) // Nếu không báo lỗi
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
                _logger.Error($"Co loi xay ra khi tao User GDTT: {ex}");
                _logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }

        // Tạo User GDDK
        // Trả vè mã User mới nếu thành công, hoặc báo lỗi nếu thất bại.

        [HttpPost("CreateUserGDDK")]
        [Authorize]
        public async Task<IActionResult> createUserGDDK([FromBody] UserGDDK entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                else
                {
                    if (entity.LoaiUser < 0)
                    {
                        return BadRequest();
                    }
                }
                var currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = await _dmUserService.createUserGDDK(entity, currentUserEmail);
                _logger.Information("POST request CreateUserGDDK received");
                if (result.Length < 11) // Nếu không báo lỗi
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
                _logger.Error($"Co loi xay ra khi tao User GDDK: {ex}");
                _logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }

        // Update
        [HttpPut("UpdateUserGDTT")]
        [Authorize]
        public IActionResult UpdateUserGDTT([FromBody] UserGDTT entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                else
                {
                    if (entity.LoaiUser < 0)
                    {
                        return BadRequest();
                    }
                }
                var currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _dmUserService.updateUserGDTT(entity, currentUserEmail).Result;
                _logger.Information("PUT UpdateUserGDTT request up received");
                if (result.Length < 10) // Nếu không báo lỗi
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
                _logger.Error($"Co loi xay ra khi Update User GDTT: {ex}");
                _logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }


        // Update
        [HttpPut("UpdateUserGDDK")]
        [Authorize]
        public IActionResult UpdateUserGDDK([FromBody] UserGDTT entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }
                else
                {
                    if (entity.LoaiUser < 0)
                    {
                        return BadRequest();
                    }
                }
                var currentUserEmail = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);
                var result = _dmUserService.updateUserGDTT(entity, currentUserEmail).Result;
                _logger.Information("PUT UpdateUserGDTT request up received");
                if (result.Length < 10) // Nếu không báo lỗi
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
                _logger.Error($"Co loi xay ra khi Update User GDDK: {ex}");
                _logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
                return BadRequest("An error occured");
            }
        }

        [HttpPost("GenerateJWTToken")]
        //[Authorize]
        public async Task<IActionResult> Generate_JWT_Token(string ma_user)
        {
            try
            {
                //var entities = JwtTokenHelper.Generate_JWT_Token_From_Email(currentUserEmail);
                ////_logger.Information("Gener success");
                //return Ok(entities);

                string token = await _dmUserService.generateJWTToken(ma_user);

                if (!token.Contains("không tồn"))
                {
                    return Ok(token);
                }
                else
                {
                    return BadRequest("An error occured");
                }

            }
            catch (Exception ex)
            {
                _logger.Error($"GetListLoaiBang An error occured: {ex}");
                return BadRequest("An error occurred");
            }
        }

        [HttpGet("GetListCanBoGDTT")]
        [Authorize]
        public IActionResult GetListCanBoGDTT()
        {
            try
            {
                var email = JwtTokenHelper.ExtractTokenInfoAndSetEmail(HttpContext);

                var entity = _dmUserService.GetListCanBoGDTT(email);
                _logger.Information("GET request GetListCanBoGDTT received");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListCanBoGDTT An error occurred: {ex}");
                return BadRequest("An error occurred");
            }
        }

    }
}