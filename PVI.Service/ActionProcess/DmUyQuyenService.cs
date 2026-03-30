using AutoMapper;
using Microsoft.Extensions.Configuration;
using PVI.Repository.Interfaces;
using PVI.Repository.Repositories;
using PVI.DAO.Entities.Models;
using PVI.Service.Request;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using static PVI.Repository.Repositories.DmUyQuyenRepository;
namespace PVI.Service
{

    /* Service cho danh mục hạng mục sửa chữa.
     * khanhlh - 26/09/2024
     */
    public class DmUyQuyenService
    {
        // Truyền các tham số vào service
        private readonly IDmUyQuyenRepository _dmUQ;
        private readonly Serilog.ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public DmUyQuyenService(IDmUyQuyenRepository uqRepository, IMapper mapper, Serilog.ILogger logger, IConfiguration conf)
        {
            _dmUQ = uqRepository;
            _mapper = mapper;
            _logger = logger;
            _configuration = conf;
        }


        // Tất cả ủy quyền
        public Dictionary<string,string> GetTypeUyQuyen()
        {
            var list_uyQuyen = _dmUQ.getTypeUyQuyen();
            return list_uyQuyen;
        }

        // Tất cả ủy quyền
        public DanhSachUyQuyen GetDanhSachUyQuyen(int pageNumber, int limit, UyQuyenFilter filter, string currentUserEmail)
        {
            var uyQuyenFilter = _mapper.Map<UyQuyenFilter, DmUqHstpc>(filter);
            var list_uyQuyen = _dmUQ.GetDanhSachUyQuyen(pageNumber, limit, uyQuyenFilter, currentUserEmail);
            return list_uyQuyen;
        }

        // Tất cả nhóm hạng mục
        public List<DmUserView> getListUserUyQuyen(string maDonvi, string currentUserEmail)
        {           
            var list_uyQuyen = _dmUQ.getListUserUyQuyen(maDonvi, currentUserEmail);
            return list_uyQuyen;
        }

        // Tạo ủy quyền
        public string createUyQuyen(UyQuyenRequest toBeCreated, string currentUserEmail)
        {
            var uyQuyenCreate = _mapper.Map<UyQuyenRequest, DmUqHstpc>(toBeCreated);
            var result = _dmUQ.createUyQuyen(uyQuyenCreate, currentUserEmail).Result;
            return result;
        }

        // Cập nhật sửa ủy quyền.
        public string updateUyQuyen(int prKey, UyQuyenRequest toBeUpdated, string currentUserEmail)
        {
            var uyQuyenUpdate = _mapper.Map<UyQuyenRequest, DmUqHstpc>(toBeUpdated);
            var result = _dmUQ.updateUyQuyen(prKey, uyQuyenUpdate, currentUserEmail).Result;
            return result;
        }


    }
}