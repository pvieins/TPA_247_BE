using PVI.DAO.Entities.Models;
using PVI.Helper;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Transactions;
using static PVI.Repository.Repositories.HsgdTtrinhRepository;

namespace PVI.Repository.Interfaces;

public interface IHsgdTtrinhRepository : IGenericRepository<HsgdTtrinh>
{
    Task<string> CreateHsgdTtrinh(HsgdTtrinh hsgdTtrinh, List<HsgdTtrinhCt> hsgdTtrinhCt, List<HsgdTtrinhTt> hsgdTtrinhTt, string email);
    string UpdateHsgdTtrinh(HsgdTtrinh hsgdTtrinh, List<HsgdTtrinhCt> hsgdTtrinhCt, List<HsgdTtrinhTt> hsgdTtrinhTt, List<HsgdTtrinhCt> hsgdTtrinhCt_delete, string email);
    CombinedTtrinhResult4 GetPrintToTrinh(decimal prKey, string email);
    ListHsgdTtrinh GetListTtrinh(decimal pr_key_hsgd);
    HsgdTtrinhDetail GetTtrinhById(decimal pr_key);
    SeriPhiBH GetSoPhiBH(string so_donbh, decimal so_seri);
    CheckDKBS007 CheckDKBS007(decimal pr_key_hsgd);
    string UpdateTrangThaiHsbtCt(decimal pr_key_hsgd_ttrinh);
    string ChuyenDuyet(decimal pr_key_hsgd_ttrinh, string email_login, string oid_nhan, bool send_email);
    string KyHoSo(decimal pr_key_hsgd_ttrinh, string email_login);
    CheckHD CheckKyHoSo(decimal pr_key_hsgd_ttrinh);
    string ChuyenHoSo(decimal pr_key_hsgd_ttrinh, string email_login, string oid_nhan, bool send_email);
    string ChuyenKyHoSo(decimal pr_key_hsgd_ttrinh, string email_login, string oid_nhan, bool send_email);
    string TraLaiHoSo(decimal pr_key_hsgd_ttrinh, string email_login, string oid_nhan, string lido_tc, bool send_email);
    string HuyToTrinh(decimal pr_key_hsgd_ttrinh, string email_login);
    List<HsgdTtrinhNky> GetLichSuPheDuyet(decimal pr_key_hsgd_ttrinh);
    TtrinhCount CountTTrinhByTT(string email_login, int nam_dulieu);
    PagedList<HoSoTrinhKy> GetDataHsTrinhKy(string email_login, ToTrinhParameters totrinhParameters);
    PagedList<HoSoTrinhKy> GetDataHsTrinhKyKoHoaDon(string email_login, ToTrinhParameters totrinhParameters);
    PagedList<HoSoTrinhKy> GetDataHsDaThanhToan(string email_login, ToTrinhParameters totrinhParameters);    
    DmUser? GetUserLogin(string email_login);
    List<DmUser> GetListUserChuyenKy(string email_login);
    CombinedTtrinhResult3 CreateBiaHS(BiaHS biahs);
    HsgdTtrinh? GetTtrinhByOid(Guid oid);
    CombinedTtrinhResult4 GetPrintToTrinhTPC(decimal pr_key_hsgd_ctu, string email, int loai_tt);
    string UploadToTrinhTPC(UploadToTrinhTPC entity, string email_login);
    string PheDuyetHsTpc(decimal pr_key_hsgd_ctu, string email_login);
    bool CheckHsgdTPC(decimal pr_key_hsgd_ctu, string ma_ttrang, string email_login);
    PagedList<HoSoTrinhKy> GetDataHsTrinhKyLanhDao(string email_login, ToTrinhParameters totrinhParameters);
    HsgdCtu GetHsgdCtuByKey(decimal pr_key);
    TtrinhLDCount CountTTrinhLDByTT(string email_login, int nam_dulieu);
    List<HsgdTtrinhTt> GetHsgdTtrinhTt(decimal pr_key_tt);
    string KyHoSoTPC(decimal pr_key_hsgd_ctu, string email_login);
    HsgdTtrinh GetHsgdTtrinhByIdAsync(decimal pr_key);
}
