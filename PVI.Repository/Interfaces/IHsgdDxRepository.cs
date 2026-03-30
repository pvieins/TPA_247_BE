using PVI.DAO.Entities.Models;
using PVI.Helper;
using System.Linq.Expressions;
using System.Transactions;
using static PVI.Repository.Repositories.HsgdDxRepository;
namespace PVI.Repository.Interfaces;

public interface IHsgdDxRepository : IGenericRepository<HsgdDx>
{
    List<HsbtCtView> GetListPhaiTraBT(decimal pr_key_hsgd);
    Task<List<HsbtUocBT>> GetListChiTietUocBT(decimal hsbt_ct_pr_key);
    List<HsbtGDView> GetListPhaiTraGD(decimal pr_key_hsgd);
    Task<List<HsbtUocGD>> GetListChiTietUocGD(decimal hsbt_gd_pr_key);
    List<HsbtThtsView> GetListThuDoi(decimal pr_key_hsgd);
    List<HsgdDxView> GetListPASC(decimal pr_key_hsgd_dx_ct, decimal pr_key_hsgd_ctu);
    List<HsgdDxSum> ReloadSum(decimal pr_key_hsgd_dx_ct);
    Task<string> CreateHsbtCt(HsbtCt hsbtCt, HsgdDxCt hsgdDxCt, HsbtUoc hsbtUoc, decimal prKeyHsgdCtu, List<FileAttachBt> fileAttach);
    Task<string> UpdateHsbtCt(HsbtCt hsbtCt, HsgdDxCt hsgdDxCt, HsbtUoc? hsbtUoc, List<FileAttachBt> fileAttach, List<FileAttachBt> file_attach_bt_delete);
    Task<string> DeleteHsbtCt(decimal pr_key);
    Task<string> CreateHsbtGd(HsbtGd hsbtGd, HsbtUocGd hsbtUocGd, List<FileAttachBt> fileAttach);
    Task<string> UpdateHsbtGd(HsbtGd hsbtGd, HsbtUocGd? hsbtUocGd, List<FileAttachBt> fileAttach, List<FileAttachBt> file_attach_bt_delete);
    Task<string> DeleteHsbtGd(decimal pr_key);
    Task<string> CreateHsbtThts(HsbtTht hsbtThts);
    Task<string> UpdateHsbtThts(HsbtTht hsbtThts);
    string CreatePASC(HsgdDxCt hsgdDxCt, List<HsgdDx> hsgdDx, List<HsgdDxTsk> hsgdDxTsk);
    DonBH? GetTTDonBH(decimal pr_hsbt_ctu, string ma_sp);
    Task<string> ImportPASC(List<HsgdDx> entity);
    CombinedPASCResult PrintPASC(decimal pr_key_hsbt_ct, decimal pr_key_hsgd_ctu, string email,int loai_dx);
    void SendEmail_PVI247(string sTo, decimal pr_key_hsbt_ct, decimal pr_key_hsgd_ctu, string strFileNamePdf, string loai_gui);
    string UpdateNhatKyPADT(decimal pr_key_hsbt_ct, NhatKy? nhat_ky);
    string UpdateNhatKyThongBaoBT(decimal pr_key_hsgd_ctu, NhatKy? nhat_ky);
    Task<PagedList<LichsuPa>> LichSuPasc(LichsuPaParameters parameters);
    string GuiPASC(decimal pr_key_hsbt_ct, decimal pr_key_hsgd_ctu, bool chk_send_pasc, bool pasc_send_sms, string email_nhan, string phone_nhan, string email_login, string file_path);
    Task<List<KHVat>> GetListDonViGiamDinh(string email_login);
    List<FileAttachBt> GetFileAttachBt(decimal pr_key_hsbt_ct, string ma_ctu);
    string GetMaTtrangGd(decimal PrKeyHsgd);
    decimal GetSTBTByHsgd(decimal pr_key_hsbt_ctu);
    bool KyPASCXCG(decimal pr_key_hsgd_ctu, string file_path, string email, string SignContent);
    int GetPascSendMail(decimal pr_key_hsbt_ct);
    string GetFilePathPasc(decimal pr_key_hsbt_ct);
    string UpdatePathPasc(decimal pr_key_hsgd_ct, string file_path);
    HsgdCtu? GetHsgdCtu(decimal PrKeyHsgd);
    List<HsgdDxCt> GetListHsgdDxCt(decimal pr_key_hsbt_ctu);
    HsgdDxCt GetHsgdDxCt(decimal pr_key_hsbt_ct);
    string GetThongTinKyDienTu(decimal pr_key_hsgd_ctu, string email);
    CombinedTtrinhResult3 PrintThongBaoBT(decimal pr_key_hsgd_ctu, string email);
    Task<List<DanhMuc>> GetListLoaiDongCo();
    int GetSendMailThongBaoBT(decimal pr_key_hsgd_ctu);
    bool CheckTrangThaiBT(decimal pr_key_hsgd_ctu);
    string CheckKyTBBT(decimal pr_key_hsgd_ctu);
    ServiceResult GuiThongBaoBT(decimal pr_key_hsgd_ctu, string email_nhan, string email_login, string file_path);
    TTPrintPasc GetPrintPASC(decimal pr_key_hsbt_ct, decimal pr_key_hsgd_ctu, string email, int loai_dx);
    string CheckTrungHsbt(decimal pr_key_hsgd_ctu,string ma_sp);
    List<string> Lay_mst_donvicapdon(decimal pr_key_hsgd_ctu);
    Task<ServiceResult> PheDuyetTBBT(int prKey, string currentUserEmail);
    Task<string> UpdatePathTBBT(decimal pr_key_hsgd_ctu, string path_file);
    
    Task<bool> KyTBBT(decimal pr_key_hsgd_ctu, string file_path, string email, string SignContent);
    Task<string> GetThongTinKyDienTuTBBT(decimal pr_key_hsgd_ctu, string email);

}
