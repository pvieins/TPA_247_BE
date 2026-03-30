using AutoMapper;
using Microsoft.Extensions.Configuration;
using PVI.Repository.Interfaces;
using PVI.Repository.Repositories;
using PVI.DAO.Entities.Models;
using PVI.Service.Request;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using static PVI.Repository.Repositories.DmHieuXeRepository;
namespace PVI.Service
{

    /* Service cho danh mục hạng mục sửa chữa.
     * khanhlh - 26/09/2024
     */
    public class DmHieuXeService
    {
        // Truyền các tham số vào service
        private readonly IDmHieuXeRepository _dmHXLX;
        private readonly Serilog.ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        // Hiệu xe loại xe
        public DmHieuXeService(IDmHieuXeRepository hxlxRepository, IMapper mapper, Serilog.ILogger logger, IConfiguration conf)
        {
            _dmHXLX = hxlxRepository;
            _mapper = mapper;
            _logger = logger;
            _configuration = conf;
        }

        // Hiệu xe loại xe theo filter
        public ListHieuXe getListHieuXe (int pageNumber, int limit, HieuXeRequest filter)
        {
            var HieuXeFilter = _mapper.Map<HieuXeRequest, DmHieuxe>(filter);
            var list_HieuXe = _dmHXLX.getListHieuXe(pageNumber, limit, HieuXeFilter);
            return list_HieuXe;
        }

        // Hiệu xe loại xe theo filter
        public ListLoaiXe getListLoaiXe(int pageNumber, int limit, LoaiXeFilter filter)
        {
            var LoaixeFilter = _mapper.Map<LoaiXeFilter, DmLoaixe>(filter);
            var list_loaixe = _dmHXLX.getListLoaiXe(pageNumber, limit, LoaixeFilter);
            return list_loaixe;
        }

        // Tạo hiệu xe mới
        public string createHieuXe(HieuXeRequest toBeCreated, string currentUserEmail)
        {
            var HieuXeCreate = _mapper.Map<HieuXeRequest, DmHieuxe>(toBeCreated);
            var result = _dmHXLX.createHieuXe(HieuXeCreate, currentUserEmail).Result;
            return result;
        }

        // Tạo loại xe mới
        public string createLoaiXe(LoaiXeRequest toBeCreated, string currentUserEmail)
        {
            var LoaiXeCreate = _mapper.Map<LoaiXeRequest, DmLoaixe>(toBeCreated);
            LoaiXeCreate.FrKey = toBeCreated.PrKeyHieuXe;
            var result = _dmHXLX.createLoaiXe(LoaiXeCreate, currentUserEmail).Result;
            return result;
        }

        // Cập nhật hiệu xe
        public string updateHieuXe(int prKey, HieuXeRequest toBeUpdated, string currentUserEmail)
        {
            var HieuXeUpdate = _mapper.Map<HieuXeRequest, DmHieuxe>(toBeUpdated);
            var result = _dmHXLX.updateHieuXe(prKey, HieuXeUpdate, currentUserEmail).Result;
            return result;
        }

        // Cập nhật loại xe
        public string updateLoaiXe(int prKey, LoaiXeRequest toBeUpdated, string currentUserEmail)
        {
            var LoaiXeUpdate = _mapper.Map<LoaiXeRequest, DmLoaixe>(toBeUpdated);
            LoaiXeUpdate.FrKey = toBeUpdated.PrKeyHieuXe;
            var result = _dmHXLX.updateLoaiXe(prKey, LoaiXeUpdate, currentUserEmail).Result;
            return result;
        }

    }
}