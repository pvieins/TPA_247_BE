using PVI.DAO.Entities.Models;
using PVI.Helper;
using PVI.Repository.Repositories;
using System.Linq.Expressions;

namespace PVI.Repository.Interfaces;

// Interface cho danh mục hạng mục sửa chữa.
// khanhlh - 26/07/2024

public interface IDmHmucSuaChuaRepository : IGenericRepository<DmHmuc>
{
    ListHmuc getListHmuc(int pageNumber, int limit, DmHmuc searchTarget);

    ListHmuc getListHmuc_HSGD_Anh(int pageNumber, int limit, int pr_key, DmHmuc? searchTarget);

    ListNHmuc getListNHmuc(int pageNumber, int limit, DmNhmuc searchTarget, bool getFull);

    ListTongThanhXe getListTongThanhXe(bool getFull);

    Task<string> CreateNHmuc(DmNhmuc nhomHangMuc, string currentUserEmail);

    Task<string> CreateHmuc(DmHmuc hangMuc, string currentUserEmail);

    Task<string> UpdateNHmuc(DmNhmuc nhomHangMuc, string currentUserEmail);

    Task<string> updateHmuc(DmHmuc hangMuc, string currentUserEmail);
}




