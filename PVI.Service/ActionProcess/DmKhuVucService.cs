using AutoMapper;
using Microsoft.Extensions.Configuration;
using PVI.Repository.Interfaces;
using PVI.Repository.Repositories;
using PVI.DAO.Entities.Models;
using PVI.Service.Request;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using static PVI.Repository.Repositories.DmKhuVucRepository;
namespace PVI.Service
{

    /* Service cho danh mục hạng mục sửa chữa.
     * khanhlh - 26/09/2024
     */
    public class DmKhuVucService
    {
        // Truyền các tham số vào service
        private readonly IDmKhuVucRepository _dmKV;
        private readonly Serilog.ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public DmKhuVucService(IDmKhuVucRepository kvRepository, IMapper mapper, Serilog.ILogger logger, IConfiguration conf)
        {
            _dmKV = kvRepository;
            _mapper = mapper;
            _logger = logger;
            _configuration = conf;
        }

        // Tất cả khu vực
        public DanhSachKhuVuc GetDanhSachKhuVuc(int pageNumber, int limit, KhuVucFilter filter, string currentUserEmail)
        {
            var KhuVucFilter = _mapper.Map<KhuVucFilter, DmKhuvuc>(filter);
            var list_KhuVuc = _dmKV.GetDanhSachKhuVuc(pageNumber, limit, KhuVucFilter, currentUserEmail);
            return list_KhuVuc;
        }

        // Tất cả danh sách tỉnh
        public List<DmTinh> getListTinh()
        {
            var list_KhuVuc = _dmKV.getListTinh();
            return list_KhuVuc;
        }

        // Lấy danh sách quận huyện
        public List<DmTinh> getListQuanHuyen(string MaTinh)
        {
            var result = _dmKV.getListQuanHuyen(MaTinh);
            return result;
        }

        // Tạo khu vực mới
        public string createKhuVuc(KhuVucCreate toBeCreated, string currentUserEmail)
        {
            var KhuVucCreate = _mapper.Map<KhuVucCreate, DmKhuvuc>(toBeCreated);
            var result = _dmKV.createKhuVuc(KhuVucCreate, currentUserEmail).Result;
            return result;
        }

        // Cập nhật khu vực
        public string updateKhuVuc(int prKey, KhuVucUpdate toBeUpdated, string currentUserEmail)
        {
            var KhuVucUpdate = _mapper.Map<KhuVucUpdate, DmKhuvuc>(toBeUpdated);
            var result = _dmKV.updateKhuVuc(prKey, KhuVucUpdate, currentUserEmail).Result;
            return result;
        }


    }
}