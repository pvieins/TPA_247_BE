using PVI.DAO.Entities.Models;

namespace PVI.Repository.Interfaces;

/* Interface cho danh mục điểm trực
 * khanhlh - 22/08/2024
 */

public interface IDiemtrucRepository : IGenericRepository<DmDiemtruc>
{
    // Lấy đầy đủ thông tin điểm trực, có lọc theo filter
    Task<List<DmDiemtruc>> getStationList(int pageNumber, int limit);

    // Tra cứu điểm trực theo filter.
    public Task<List<DmDiemtruc>> searchFilterStationList(int pageNumber, int limit, DmDiemtruc searchTarget);

    // Lấy danh sách tên các điểm trực.
    Task<List<DmDiemtruc>> getStationNameList(int pageNumber, int limit);

    // Lấy danh sách các user GDTT
    Task<List<DmUser>> getStationUserList(int pageNumber, int limit);

    // Tạo điểm trực mới.
    Task<string> createStation(DmDiemtruc diemtruc, string currentUserEmail);

    // Update điểm trực
    string updateStation(DmDiemtruc diemtruc, string currentUserEmail);

}
