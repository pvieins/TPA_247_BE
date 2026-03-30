using PVI.DAO.Entities.Models;
using System.Linq.Expressions;


namespace PVI.Repository.Interfaces;

/* Interface cho danh mục phân quyền ký 
 * khanhlh - 22/08/2024
 */

public interface IPquyenKyHsRepository : IGenericRepository<DmPquyenKyhs>
{
    // Lấy đầy đủ thông tin quyền ký số
    // Các params bắt buộc phải có bao gồm pageNumber (Số trang) và Limit (Số record muốn hiển thị trên mỗi trang).
    Task<List<DmPquyenKyhs>> getDigitalSignList(int pageNumber, int limit, string currentUserEmail);

    // Lấy đầy đủ thông tin quyền ký số, có thể lấy theo các filter.
    // Các params bắt buộc phải có bao gồm pageNumber (Số trang) và Limit (Số record muốn hiển thị trên mỗi trang).
    Task<List<DmPquyenKyhs>> searchDigitalSignByFilter(int pageNumber, int limit, DmPquyenKyhs searchTarget, string currentUserEmail);

    // Lấy danh sách tên các sản phẩm, có thể lấy theo filter.
    Task<List<DanhMuc>> getProductList(string? maSp, string? tenSp);

    // Lấy danh sách các user ký số, có thể lấy theo filter.
    Task<List<DmUser>> getDigitalSignUserList(int pageNumber, int limit, string? maUser, string? tenUser, string? dienthoai);

    //Tạo quyền ký số mới
    Task<string> createDigitalSign(DmPquyenKyhs ky_so, string curentUserEmail);

    // Update quyền ký số.
    string updateDigitalSign(DmPquyenKyhs ky_so, string currentUserEmail);

    // Lấy thông tin quyền ký số từ mã người dùng 
    string getDigitalSignIdFromUserId(string userId);

}
