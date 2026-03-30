using PVI.DAO.Entities.Models;
using PVI.Helper;
using System.Linq.Expressions;
using static PVI.Repository.Repositories.DmKhuVucRepository;
using static PVI.Repository.Repositories.DmUyQuyenRepository;

namespace PVI.Repository.Interfaces;

// Interface cho danh mục ủy quyền 
// khanhlh - 01/10/2024

public interface IDmKhuVucRepository : IGenericRepository<DmKhuvuc>
{
    DanhSachKhuVuc GetDanhSachKhuVuc(int pageNumber, int limit, DmKhuvuc filter, string currentUserEmail);
    List<DmTinh> getListTinh();

    List<DmTinh> getListQuanHuyen(string MaTinh);

    Task<string> createKhuVuc(DmKhuvuc khuvuc, string currentUserEmail);

    Task<string> updateKhuVuc(int prKey, DmKhuvuc khuvuc, string currentUserEmail);
}
