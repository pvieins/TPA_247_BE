using PVI.DAO.Entities.Models;
using PVI.Helper;
using System.Linq.Expressions;
using static PVI.Repository.Repositories.DmUyQuyenRepository;

namespace PVI.Repository.Interfaces;

// Interface cho danh mục ủy quyền 
// khanhlh - 01/10/2024

public interface IDmUyQuyenRepository : IGenericRepository<DmUqHstpc>
{
    DanhSachUyQuyen GetDanhSachUyQuyen(int pageNumber, int limit, DmUqHstpc filter, string currentUserEmail);
    List<DmUserView> getListUserUyQuyen(string maDonvi, string currentUserEmail);
    Task<string> createUyQuyen(DmUqHstpc uyQuyen, string currentUserEmail);
    Task<string> updateUyQuyen(int prKey, DmUqHstpc uyQuyen, string currentUserEmail);

    Dictionary<string, string> getTypeUyQuyen();
}
