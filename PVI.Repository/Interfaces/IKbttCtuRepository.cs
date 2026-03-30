using PVI.DAO.Entities.Models;
using PVI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PVI.Repository.Repositories.KbttCtuRepository;

namespace PVI.Repository.Interfaces
{
    public interface IKbttCtuRepository : IGenericRepository<KbttCtu>
    {
        Task<PagedList<KbttCtuDto>> GetListPVIMobile(string email, KbttCtuParameters parameters, string MaDonbh);
        Task<List<KbttCtuDto>> GetListPVIMobileExcel(string email, KbttCtuParameters parameters, string MaDonbh);
        Task<dynamic?> GetDetailKbttCtu(decimal prKey);
        Task<List<ImageKbttResponse>> GetListAnhKbtt(decimal prKey);
        Task<PagedList<ListAddNewResponse>> GetListHoSo(ListAddNewParameters parameters, string MaDonbh);
        //Task<dynamic> GetListHoSo(ListAddNewParameters parameters);
        //Task<List<ListAddNewResponse>> GetListHoSo(ListAddNewParameters parameters);
        Task<KbttCtu> UpdateKbtt(decimal prKey, KbttCtuRequest request);
        Task<string> TaoDonHsgd(decimal prKey, string maLhsbt, string donviBth, string MaDonbh);
        Task<string> CapNhatSoHsgd(DateTime startDate, DateTime endDate);
        Task<string> CreateKbtt(CreateKbttCtuRequest request, string email);
        Task<List<LoaiHinhBhDTO>> GetListLoaiHinhBh();
    }
}
