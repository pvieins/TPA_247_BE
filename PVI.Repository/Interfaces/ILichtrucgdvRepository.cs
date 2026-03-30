using Microsoft.EntityFrameworkCore;
using PVI.DAO.Entities.Models;
using static PVI.Repository.Repositories.LichtrucgdvRepository;

namespace PVI.Repository.Interfaces;

/* Interface cho danh mục điểm trực
 * khanhlh - 22/08/2024
 */

public interface ILichTrucGDVRepository : IGenericRepository<LichTrucgdv>
{
    // Lấy danh sách khu vực
    List<DmKhuvuc> getListKhuVuc();

    // Lấy danh sách gara khu vực
    List<DmGaraKhuvuc> getListGaraKhuVuc(string ma_kv);
    
    // Lấy danh sách cán bộ trực.
    List<DmUser> getListCanBoTruc();

    // Tra cứu lịch trực theo filter.
    DanhSachLichTruc searchFilterStationSchedule(string ma_kv, DateTime? ngay_xemlich);
    Task<List<GhichuLichtruc>> getListScheduleNotes(string ma_kv);
    List<LichTrucgdv> SearchLichTrucTheoFrKey(int fr_key);
    
    // Thao tác chỉnh sửa update ghi chú.
    Task<string> updateIndividualNote(int pr_key, string ghiChu);
    Task<string> updateScheduleNote(string ma_kv, string ghiChu);

    Task<string> updateSchedulePerson(string ma_kv, string ma_gara, string thu, string sang_chieu, string[] ma_user_deleted, string[] ma_user_added);

}
