using PVI.DAO.Entities.Models;
using PVI.Helper;
using System.Linq.Expressions;
using static PVI.Repository.Repositories.DmGaraKhuvucRepository;

namespace PVI.Repository.Interfaces;

// Interface cho danh mục gara khu vực
// khanhlh - 01/10/2024

public interface IDmGaraKhuVucRepository : IGenericRepository<DmGaraKhuvuc>
{
    DanhSachGaraKhuvuc GetDanhSachGaraKhuvuc(int pageNumber, int limit, DmGaraKhuvuc filter, string currentUserEmail);

    List<GaraKhuVuc> getListGarageKhuVuc(int pageNumber, int pageSize);

    List<DmKhuvuc> getListKhuVuc();

    Task<string> createGaraKhuVuc(DmGaraKhuvuc garageKhuvuc, string currentUserEmail);

    Task<string> updateGaraKhuVuc(int prKey, DmGaraKhuvuc garageKhuvuc, string currentUserEmail);

}
