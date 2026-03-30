using PVI.DAO.Entities.Models;
using PVI.Helper;
using System.Linq.Expressions;
using System.Transactions;
using static PVI.Repository.Repositories.HsgdTtrinhRepository;

namespace PVI.Repository.Interfaces;

public interface IHsgdDnttRepository : IGenericRepository<HsgdDntt>
{    
    string CreateDNTT(DNTTRequest dNTTRequest, string pr_key_hsgd_ttrinh, string email_login);
    Task<List<NguoiDeNghi>> GetListNguoiDeNghi(string ma_donvi);
    Task<List<DanhMuc>> GetListDonViTT(string ma_donvi);
    Task<List<DanhMuc>> GetListNhomKT(string ma_donvi);
    Task<List<ThuHuong>> GetThongtinTKThuHuong(decimal pr_key_hsgd);
    Task<List<DanhMuc>> GetListNguoiXuLy(string ma_donvi);
    PagedList<HsgdDnttView> GetListDntt(string email_login, DnttParameters dnttParameters);
    Task<string> DeleteDntt(string pr_key_dntt);
    List<LichSuPheDuyet>? GetLichSuPheDuyet(decimal pr_key_ttoan_ctu);
    Task<List<NguoiDeNghi>> GetListCanBoTT(string ma_donvi);
}
