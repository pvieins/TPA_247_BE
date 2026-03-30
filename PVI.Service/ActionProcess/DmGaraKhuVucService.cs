using AutoMapper;
using Microsoft.Extensions.Configuration;
using PVI.Repository.Interfaces;
using PVI.Repository.Repositories;
using PVI.DAO.Entities.Models;
using PVI.Service.Request;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using static PVI.Repository.Repositories.DmGaraKhuvucRepository;
namespace PVI.Service
{
    /* Service cho danh mục hạng mục sửa chữa.
     * khanhlh - 26/09/2024
     */
    public class DmGaraKhuVucService
    {
        // Truyền các tham số vào service
        private readonly IDmGaraKhuVucRepository _dmKV;
        private readonly Serilog.ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public DmGaraKhuVucService(IDmGaraKhuVucRepository kvRepository, IMapper mapper, Serilog.ILogger logger, IConfiguration conf)
        {
            _dmKV = kvRepository;
            _mapper = mapper;
            _logger = logger;
            _configuration = conf;
        }

        // Tất cả GaraKhuVuc
        public DanhSachGaraKhuvuc getDanhSachGaraKhuVuc(int pageNumber, int limit, GaraKhuVucFilter searchTarget, string currentUserEmail)
        {
            var GaraKhuVucFilter = _mapper.Map<GaraKhuVucFilter, DmGaraKhuvuc>(searchTarget);
            var list_GaraKhuVuc = _dmKV.GetDanhSachGaraKhuvuc(pageNumber, limit, GaraKhuVucFilter, currentUserEmail);
            return list_GaraKhuVuc;
        }

        public List<GaraKhuVuc> getListGara(int pageNumber, int limit)
        {
            var list_GaraKhuVuc = _dmKV.getListGarageKhuVuc(pageNumber, limit);
            return list_GaraKhuVuc;
        }

        public List<DmKhuvuc> getListKhuvuc()
        {
            var list_GaraKhuVuc = _dmKV.getListKhuVuc();
            return list_GaraKhuVuc;
        }

        // Tất cả danh sách tỉnh
        public string createGaraKhuVuc(GaraKhuVucRequest toBeCreated, string currentUserEmail)
        {
            var GaraKhuVucCreate = _mapper.Map<GaraKhuVucRequest, DmGaraKhuvuc>(toBeCreated);
            var list_GaraKhuVuc = _dmKV.createGaraKhuVuc(GaraKhuVucCreate, currentUserEmail).Result;
            return list_GaraKhuVuc;
        }
        public string updateGaraKhuVuc(int prKey, GaraKhuVucRequest toBeUpdated, string currentUserEmail)
        {
            var GaraKhuVucUpdate = _mapper.Map<GaraKhuVucRequest, DmGaraKhuvuc>(toBeUpdated);
            var result = _dmKV.updateGaraKhuVuc(prKey, GaraKhuVucUpdate, currentUserEmail).Result;
            return result;
        }

    }
}