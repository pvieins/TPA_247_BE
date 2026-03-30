using AutoMapper;
using Microsoft.Extensions.Configuration;
using PVI.Repository.Interfaces;
using PVI.Repository.Repositories;
using PVI.DAO.Entities.Models;
using PVI.Service.Request;
using System.Linq.Expressions;
namespace PVI.Service
{

    /* Service cho danh mục quyền ký số .
     * khanhlh - 22/08/2024
     */

    public class PQuyenKyHsService
    {
        // Truyền các tham số vào service
        private readonly IPquyenKyHsRepository _pquyenkyhsRepository;
        private readonly Serilog.ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public PQuyenKyHsService(IPquyenKyHsRepository pquyenkyhsRepository, IMapper mapper, Serilog.ILogger logger, IConfiguration conf)
        {
            _pquyenkyhsRepository = pquyenkyhsRepository;
            _mapper = mapper;
            _logger = logger;
            _configuration = conf;
        }

        /* Các function dưới được gọi từ Controller.
         * Tương tác với Repository.
         */


        // Tất cả quyền ký số
        public Task<List<DmPquyenKyhs>> getDigitalSignList(int pageNumber, int limit, string currentUserEmail) {
            var list_ky_so = _pquyenkyhsRepository.getDigitalSignList(pageNumber, limit, currentUserEmail);
            return list_ky_so;
        }

        // Tất cả quyền ký số, có lấy theo filter
        public Task<List<DmPquyenKyhs>> searchDigitalSignByFilter(int pageNumber, int limit, DmQuyenKyFilter searchTarget, string currentUserEmail)
        {
            var kySo = _mapper.Map<DmQuyenKyFilter, DmPquyenKyhs>(searchTarget);
            kySo.NgayCnhat = searchTarget.ngayCnhat;
            var list_ky_so = _pquyenkyhsRepository.searchDigitalSignByFilter(pageNumber, limit, kySo, currentUserEmail);
            return list_ky_so;
        }

        // Tất cả User
        public Task<List<DmUser>> getDigitalSignUserList(int pageNumber, int limit, string? maUser, string? tenUser, string? dienthoai)
        {
            var list_ten_user = _pquyenkyhsRepository.getDigitalSignUserList(pageNumber, limit, maUser, tenUser, dienthoai);
            return list_ten_user;
        }

        // Tất cả các sản phẩm, có filter 
        public Task<List<DanhMuc>> getProductList(string? maSp, string? tenSp)
        {
            var list_san_pham = _pquyenkyhsRepository.getProductList(maSp, tenSp);
            return list_san_pham;
        }

        
        // Tạo mới User ký số.
        // Trả vè mã ký hồ sơ nếu thành công, hoặc báo lỗi nếu thất bại.
        public async Task<string> createDigitalSign(QuyenKySoRequest entity)
        {
            try
            {
                var kySo = _mapper.Map<DmPquyenKyHsRequest, DmPquyenKyhs>(entity.QuyenKySo);
                kySo.PrKey = Guid.NewGuid(); // Tạo Guid mới
                kySo.NgayCnhat = DateTime.Now;
               
                var result = await _pquyenkyhsRepository.createDigitalSign(kySo, "");
                return result;
            }
            catch (Exception ex)
            {
                //_logger.Error("CreateDiemtruc:", ex);
                //_logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
            }
            return null!;
        }

        // Update quyền ký hồ sơ.
        // Để update được điểm trực, yêu cầu Mã user
        // Trả vè mã user nếu thành công và báo lỗi nếu thất bại.
        
        public string updateDigitalSign(QuyenKySoRequest entity)
        {
            var result = "";
           
            try
            {
                // Đảm bảo user có tồn tại trong bảng ký số.
                //string quyen_ky_so_id = _pquyenkyhsRepository.getDigitalSignIdFromUserId(entity.QuyenKySo.MaUser.ToString().ToLower());

                DmPquyenKyhs userExisted = _pquyenkyhsRepository.GetEntityByCondition(x => x.MaUser == entity.QuyenKySo.MaUser).Result;
               
                // Chỉ tiến hành updat khi user có tổn tại.
                if (userExisted != null)
                {   

                    // Phiên bản trước, lấy theo user id.
                    //DmPquyenKyhs quyen_ky_old = _pquyenkyhsRepository.GetEntityByCondition(x => x.MaUser == quyen_ky_so_id).Result;
                    //entity.QuyenKySo.MaUser = quyen_ky_so_id.ToLower(); // Format lại request cho đúng với cột trong DB 

                    var quyen_ky_new = _mapper.Map(entity.QuyenKySo, userExisted);
                    result = _pquyenkyhsRepository.updateDigitalSign(quyen_ky_new, ""); // Tiến hành update.
                }
                else
                {
                    result = $"Quyền ký cho User {entity.QuyenKySo.MaUser} không tồn tại";
                }
            }
            catch (Exception ex)
            {
                //_logger.Error("CreateDiemtruc:", ex);
                //_logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
            }
            return result;
        }

        public async Task<string> deleteDigitalSign(string ma_user)
        {
            string result = "";
            try
            {
                DmPquyenKyhs entity = await _pquyenkyhsRepository.GetEntityByCondition(x => x.MaUser == ma_user);
                if (entity == null)
                {
                    result = $"Không tồn tại quyền ký với user {ma_user}";
                }
                else
                {
                    _pquyenkyhsRepository.Delete(entity);
                    await _pquyenkyhsRepository.SaveAsync();
                    result = ma_user;
                }
            }
            catch (Exception ex)
            {
                result = "Lỗi xóa quyền kí";
            }
            return result;
        }

    }
}