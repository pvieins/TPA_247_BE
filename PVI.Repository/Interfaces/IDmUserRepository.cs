using PVI.DAO.Entities.Models;
using PVI.Helper;
using PVI.Repository.Repositories;
using System.Linq.Expressions;
using System.Transactions;

namespace PVI.Repository.Interfaces;

public interface IDmUserRepository : IGenericRepository<DmUser>
{
    string getDonViById(string ma_donvi);
    List<DmDonvi> getDMDonvi(string currentUserEmail);
    List<DmLoaiUser> getDMLoaiUser();
    Task<PagedList<DmUser>> getListUserGDTT(int pageNumber, int pageSize, string currentUserEmail);
    Task<PagedList<DmUser>> getListUserGDDK(int pageNumber, int pageSize, string currentUserEmail);
    Task<DanhSachUser> searchFilterUserGDTT(int pageNumber, int limit, DmUser searchTarget, string currentUserEmail);
    Task<DanhSachUser> searchFilterUserGDDK(int pageNumber, int limit, DmUser searchTarget, string currentUserEmail);
    DmUser getUserPiasFromEmail(DmUser currentUser, string userEmail);
    List<DmUser> getListUserPiasFromDonvi(DmUser currentUser, int pageNumber, int pageSize);

    // Tạo user mới
    Task<string> createUser(DmUser user, string currentUserEmail);

    // Update lại user.
    Task<string> UpdateUser(DmUser user, string currentUserEmail);

    Task<string> GenerateJWTToken(string ma_user);

    List<DmUserView> GetListCanBoDuyet(string currentUserEmail);

    List<DmUserView> GetListDoiTruong(string currentUserEmail);
    List<DmUserView> GetListCanBoGDTT(string currentUserEmail);

}