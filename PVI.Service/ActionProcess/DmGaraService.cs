using AutoMapper;
using Microsoft.Extensions.Configuration;
using PVI.Repository.Interfaces;
using PVI.Repository.Repositories;
using PVI.DAO.Entities.Models;
using PVI.Service.Request;
using PVI.Helper;

namespace PVI.Service
{

    /* Service cho danh mục gara.
     * khanhlh - 26/08/2024
     */

    public class DmGaraService
    {
        // Truyền các tham số vào service
        private readonly IDmGaraRepository _dmGaraRepository;
        private readonly Serilog.ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        
        public DmGaraService(IDmGaraRepository garaRepository, IMapper mapper, Serilog.ILogger logger, IConfiguration conf)
        {
            _dmGaraRepository = garaRepository;
            _mapper = mapper;
            _logger = logger;
            _configuration = conf;
        }

        /* Các function dưới được gọi từ Controller.
         * Tương tác với Repository.
         */

        
        // Tất cả gara
        public Task<List<DmGaRa>> getGarageList(int pageNumber, int limit)
        {
            var list_gara = _dmGaraRepository.getGarageList(pageNumber, limit);
            return list_gara;
        }

        // Tất cả gara, co filter
        public PagedList<DmGaRa> searchFilterGarage( DmGaraFilter searchTarget)
        {
            //var gara = _mapper.Map<DmGaraFilter, DmGaRa>(searchTarget);
            //gara.NgayCnhat = searchTarget.ngayCnhat;
            var list_gara = _dmGaraRepository.searchFilterGarage(searchTarget);
            return list_gara;
        }
        public Task<List<GaRaView>> getAllGara(DmGaraFilter searchTarget)
        {
            //var gara = _mapper.Map<DmGaraFilter, DmGaRa>(searchTarget);
            //gara.NgayCnhat = searchTarget.ngayCnhat;
            var list_gara = _dmGaraRepository.getAllGara(searchTarget);
            return list_gara;
        }
        // Update gara
        // Để update được điểm trực, yêu cầu PrKey và Mã điểm trực
        // Trả vè mã điểm trực nếu thành công, hoặc báo lỗi nếu thất bại.
        // 
        public string updateGarage(GaraRequest entity, string currentUserEmail)
        {
            var result = "";
            try
            {
                DmGaRa gara_old = _dmGaraRepository.GetEntityByCondition(x => x.MaGara == entity.Gara.MaGara).Result;
                
                if (gara_old != null)
                {
                    var gara_new = _mapper.Map(entity.Gara, gara_old);
                    gara_new.NgayCnhat = DateTime.Now;
                    result = _dmGaraRepository.updateGarage(gara_new, currentUserEmail, entity.TkVnd, entity.NganHang,entity.Gara.bnkCode, entity.Gara.ten_ctk);
                }
                else
                {
                    result = $"Gara với Mã {entity.Gara.MaGara} không tồn tại";
                }
            }
            catch (Exception ex)
            {
                //_logger.Error("CreateDiemtruc:", ex);
                //_logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
            }
            return result;
        }

        // Đồng bộ danh mục gara
        public string syncGarageFromPias()
        {
            return _dmGaraRepository.syncGarageFromPias();
        }
        
    }
}