using AutoMapper;
using Microsoft.Extensions.Configuration;
using PVI.Repository.Interfaces;
using PVI.Repository.Repositories;
using PVI.DAO.Entities.Models;
using PVI.Service.Request;

namespace PVI.Service
{

    /* Service cho danh mục điểm trực.
     * khanhlh - 22/08/2024
     */

    public class DiemTrucService
    {
        // Truyền các tham số vào service
        private readonly IDiemtrucRepository _diemtrucRepository;
        private readonly Serilog.ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public DiemTrucService(IDiemtrucRepository diemtrucRepository, IMapper mapper, Serilog.ILogger logger, IConfiguration conf)
        {
            _diemtrucRepository = diemtrucRepository;
            _mapper = mapper;
            _logger = logger;
            _configuration = conf;
        }

        /* Các function dưới được gọi từ Controller.
         * Tương tác với Repository.
         */

        // Tất cả điểm trực
        public Task<List<DmDiemtruc>> getStationList(int pageNumber, int limit)
        {
            var list_diem_truc = _diemtrucRepository.getStationList(pageNumber, limit);
            return list_diem_truc;
        }

        public Task<List<DmDiemtruc>> searchFilterStationList(int pageNumber, int limit, DmDiemtrucFilter searchTarget)
        {
            var diemTruc = _mapper.Map<DmDiemtrucFilter, DmDiemtruc>(searchTarget);
            diemTruc.NgayCnhat = (searchTarget.NgayCnhat != null ? DateTime.Parse(searchTarget.NgayCnhat) : null);
            var list_diem_truc = _diemtrucRepository.searchFilterStationList(pageNumber, limit, diemTruc);
            return list_diem_truc;
        }

        // Tên điểm trực
        public Task<List<DmDiemtruc>> getStationNameList(int pageNumber, int limit)
        {
            var list_ten_diem_truc = _diemtrucRepository.getStationNameList(pageNumber, limit);
            return list_ten_diem_truc;
        }

        // Tên các user GDTT
        public Task<List<DmUser>> getStationUserList(int pageNumber, int limit)
        {
            var list_user_gdtt_diem_truc = _diemtrucRepository.getStationUserList(pageNumber, limit);
            return list_user_gdtt_diem_truc;
        }

        // Tạo điểm trực
        // Trả vè mã điểm trực nếu thành công, hoặc báo lỗi nếu thất bại.
        public async Task<string> createStation(DiemtrucRequest entity, string currentUserEmail)
        {
            try
            {
                var diemTruc = _mapper.Map<DmDiemtrucRequest, DmDiemtruc>(entity.Diemtruc);
                diemTruc.NgayCnhat = DateTime.Now;
                var result = await _diemtrucRepository.createStation(diemTruc, currentUserEmail);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("CreateDiemtruc:", ex);
                //_logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
            }
            return null!;
        }

        // Update điẻm trực
        // Để update được điểm trực, yêu cầu PrKey và Mã điểm trực
        // Trả vè mã điểm trực nếu thành công, hoặc báo lỗi nếu thất bại.
        // 

        public string updateStation(DiemtrucRequest entity, string currentUserEmail)
        {
            var result = "";
            try
            {
                 var diemtruc_old = _diemtrucRepository.GetEntityByCondition(x => x.MaDiemtruc == entity.Diemtruc.MaDiemtruc).Result;
                
                if (diemtruc_old != null)
                {
                    //var Diemtruc_new = _mapper.Map(entity.Diemtruc, diemtruc_old);
                    diemtruc_old.NgayCnhat = DateTime.Now;
                    diemtruc_old.TenDiemtruc = entity.Diemtruc.TenDiemtruc;
                    diemtruc_old.Description = entity.Diemtruc.Description;
                    diemtruc_old.Active = entity.Diemtruc.Active;
                    result = _diemtrucRepository.updateStation(diemtruc_old, currentUserEmail);
                }
                else
                {
                    result = $"Điểm trực với Mã {entity.Diemtruc.MaDiemtruc} không tồn tại";
                }
            }
            catch (Exception ex)
            {
                //_logger.Error("CreateDiemtruc:", ex);
                //_logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
            }
            return result;
        }
    }
}