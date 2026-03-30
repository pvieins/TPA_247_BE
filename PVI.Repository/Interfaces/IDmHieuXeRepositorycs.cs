using PVI.DAO.Entities.Models;
using PVI.Helper;
using PVI.Repository.Repositories;
using System.Linq.Expressions;
using static PVI.Repository.Repositories.DmHieuXeRepository;

namespace PVI.Repository.Interfaces;

// Interface cho danh mục hiệu xe loại xe.
// khanhlh - 01/10/2024

public interface IDmHieuXeRepository : IGenericRepository<DmHieuxe>
{
    ListHieuXe getListHieuXe(int pageNumber, int limit, DmHieuxe searchTarget);
    ListLoaiXe getListLoaiXe(int pageNumber, int limit, DmLoaixe searchTarget);
    Task<string> createHieuXe(DmHieuxe hieuxe, string currentUserEmail);
    Task<string> createLoaiXe(DmLoaixe loaixe, string currentUserEmail);
    Task<string> updateHieuXe(int prKey, DmHieuxe hieuxe, string currentUserEmail);
    Task<string> updateLoaiXe(int prKey, DmLoaixe loaixe , string currentUserEmail);
}
