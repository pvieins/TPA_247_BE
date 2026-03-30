using AutoMapper;
using Microsoft.Extensions.Configuration;
using PVI.Repository.Interfaces;
using PVI.Repository.Repositories;
using PVI.DAO.Entities.Models;
using PVI.Service.Request;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
namespace PVI.Service
{

    /* Service cho danh mục hạng mục sửa chữa.
     * khanhlh - 26/09/2024
     */
    public class DmHmucSuaChuaService
    {
        // Truyền các tham số vào service
        private readonly IDmHmucSuaChuaRepository _dmHmucSuaChua;
        private readonly Serilog.ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;


        public DmHmucSuaChuaService(IDmHmucSuaChuaRepository hmucSuaChua, IMapper mapper, Serilog.ILogger logger, IConfiguration conf)
        {
            _dmHmucSuaChua = hmucSuaChua;
            _mapper = mapper;
            _logger = logger;
            _configuration = conf;
        }

        // Tất cả tổng thành xe
        public ListTongThanhXe getListTongThanhXe( bool getFull)
        {
            var list_tongthanhxe = _dmHmucSuaChua.getListTongThanhXe(getFull);
            return list_tongthanhxe;
        }

        // Tất cả nhóm hạng mục
        public ListNHmuc getListNHmuc(int pageNumber, int limit, DmNHmucFilter searchTarget, bool getFull)
        {
            var NhmucFilter = _mapper.Map<DmNHmucFilter, DmNhmuc>(searchTarget);
            var list_hmuc = _dmHmucSuaChua.getListNHmuc(pageNumber, limit, NhmucFilter, getFull);
            return list_hmuc;
        }

        // Tất cả hạng mục
        public ListHmuc getListHmuc(int pageNumber, int limit, DmHmucFilter searchTarget)
        {
            var HmucFilter = _mapper.Map<DmHmucFilter, DmHmuc>(searchTarget);
            var list_hmuc = _dmHmucSuaChua.getListHmuc(pageNumber, limit, HmucFilter);
            return list_hmuc;
        }

        public ListHmuc getListHmuc_HSGD_Anh(int pageNumber, int limit, int pr_key, DmHmuc_PASC_Filter? searchTarget)
        {
            var HmucFilter = _mapper.Map<DmHmuc_PASC_Filter, DmHmuc>(searchTarget);
            var list_hmuc = _dmHmucSuaChua.getListHmuc_HSGD_Anh(pageNumber, limit, pr_key, HmucFilter);
            return list_hmuc;
        }

        // Tạo hạng mục và tạo nhóm hạng mục.
        public string CreateNHmuc(DmNHmucRequest toBeCreated, string currentUserEmail)
        {
            var NhmucRequest = _mapper.Map<DmNHmucRequest, DmNhmuc>(toBeCreated);
            var result = _dmHmucSuaChua.CreateNHmuc(NhmucRequest, currentUserEmail).Result;
            return result;
        }

        public string CreateHmuc(DmHmucRequest toBeCreated, string currentUserEmail)
        {
            var HmucRequest = _mapper.Map<DmHmucRequest, DmHmuc>(toBeCreated);
            var result = _dmHmucSuaChua.CreateHmuc(HmucRequest, currentUserEmail).Result;
            return result;
        }
        

        //var NhmucRequest = _mapper.Map<DmNHmucRequest, DmNhmuc>(searchTarget);

        // Update lại nhóm hạng mục.
        public string updateNhmuc(string maNhmuc, DmNhmucUpdate toBeUpdated, string currentUserEmail)
        {
            var hmucRequest = _mapper.Map<DmNhmucUpdate, DmNhmuc>(toBeUpdated);
            hmucRequest.MaNhmuc = maNhmuc;
            var result = _dmHmucSuaChua.UpdateNHmuc(hmucRequest, currentUserEmail).Result;
            return result;
        }

        // Update lại nhóm hạng mục.
        public string updateHmuc(string maHmuc, DmHmucUpdate toBeUpdated, string currentUserEmail)
        {
            var hmucRequest = _mapper.Map<DmHmucUpdate, DmHmuc>(toBeUpdated);
            hmucRequest.MaHmuc = maHmuc;
            var result = _dmHmucSuaChua.updateHmuc(hmucRequest, currentUserEmail).Result;
            return result;
        }


    }
}