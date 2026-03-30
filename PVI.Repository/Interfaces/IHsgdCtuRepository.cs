using PVI.DAO.Entities.Models;
using PVI.Helper;
using PVI.Repository.Repositories;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Transactions;
using static PVI.Repository.Repositories.HsgdCtuRepository;

namespace PVI.Repository.Interfaces;

public interface IHsgdCtuRepository : IGenericRepository<HsgdCtu>
{
    DmUserView GetCurrentUserInfo(string currentUserEmail);
    Task<HsgdCtu> GetBySoHsgd(string so_hsgd);
    Task<HsgdCtu> GetHsgdByPrKey(decimal pr_key);
    List<HsgdTtrinhCt> GetListHsgdDx(string so_hsgd);
    HsgdTtrinhAll GetListHsgdDxNew(string so_hsgd);
    Task<List<DanhMuc>> GetListNguyenNhanTonThat();
    Task<List<DanhMucTinh>> GetListDiaDiemTonThat();
    Task<List<DanhMuc>> GetListSanPham();
    Task<List<DanhMuc>> GetListTrangThai();
    Task<List<DmTtrangGd>> GetListStatusName();
    Task<List<DmLoaiHsgd>> GetListTypeName();
    List<DmTyGia> getListTyGia();

    // Hiện tại số hồ sơ giám định có ký tự "/", không thể truyển vào url nên tạm thời dùng pr_key.
    string updateDetailFile(int pr_key, int type, string currentUserEmail);

    string updateFile(HsgdCtu hoSoUpdate);
    List<TtrangGdCount> GetCountByStatus(string fromDate,string toDate, string email, string MaDonbh);
    Task<HsgdCtuDetail> GetData_Detail_Hsgd(decimal pr_key);
    BCGiamDinh ReadOCR(decimal pr_key_hsgd);
    Task<PagedList<HsgdCtuResponse>> GetList(HsgdCtuParameters parameters, string email,string MaDonbh);
    Task<PagedList<NhatKy>> getListDiary(int pr_key, int pageNumber, int pageSize);

    List<DmTongthanhxe> getListTongThanhXe();
    List<DmNhmuc> getListNhmuc();
    List<DmHmuc> getListHmuc(string? ma_tongthanhxe, string? ma_Nhmuc);
    List<DmHmucGiamdinh> getListHmucGiamDinh();

    Task<Dictionary<string, DmUserView>> getListRelatedUsers(int pr_key);

    string GanNguoiTiepNhan(int pr_key, string ghiChu, string oidCanBoPheDuyet, string currentUserEmail);

    // Gán giám định
    Task<string> assignAppraisal(int pr_key, string? oidGiamDV, string? oidCanBoTT, bool guiEmail, bool guiSMS, string currentUserEmail);

    // Chuyển chờ phê duyệt
    Task<string> requestApproval(int pr_key, string? ghiChu, string currentUserEmail, string? oidCanBoPheDuyet);

    Task<string> ChuyenGanHoSo(int pr_key, string ghiChu, string? oidCanBoPheDuyet, string currentUserEmail);

    // Yêu cầu bổ sung thông tin
    Task<string> requestAdditionalDetail(int pr_key, bool guiEmail, bool guiSMS, string ghiChu, string currentUserEmail);

    // Phê duyệt
    Task<string> approveAppraisal(int pr_key, string ghiChu, string currentUserEmail);
    Task<string> Baogia_giamdinh(decimal pr_key,DateTime ngay_bao_gia,decimal so_tien,string de_xuat, string currentUserEmail);
    Task<string> Duyetgia_giamdinh(decimal pr_key, DateTime ngay_bao_gia, decimal so_tien, string de_xuat, string currentUserEmail);
    // Task<string> UploadAppraisalImage(UploadFileContent listFile, int pr_key, Guid oid);
    List<DonViThanhToanResponse> getListDonViThanhToan();
    bool Kiemtra_uynhiemchi(string ma_donvi);
    DonViThanhToanResponse GetInfoDonViTT(string ma_don_vi);

    // Các API bảo lãnh
    Task<string> pheDuyetBaoLanh(int pr_key, decimal pr_key_hsbt_ct, int bl1, int bl2, int bl3, int bl4, int bl5, int bl6, int bl7, int bl8, int bl9, string bl_tailieubs, string bl_dsemail, string bl_dsphone, string? ma_donvi_tt, string currentUserEmail);
    
    Task<string> LuuThongbaoBT(int pr_key,HsgdTbbt HsgdTbbt_,List<HsgdTbbtTt> LHsgdTbbtTt, string currentUserEmail);
    Task<LuuThongBaoBTResponse> LayThongbaoBT(int pr_key_hsgd, string currentUserEmail);
    
    // Function phụ, sử dụng để lấy danh sách các parameter cần thay thế trong file template bảo lãnh.
    // Function này sẽ được gọi trong Service để tạo PDF bảo lãnh.
    CombinedBaoLanhResult BaoLanh_GetListOfReplacable(decimal prKey, decimal pr_key_hsbt_ct, string currentUserEmail, string? ma_donvi_tt);

    CombinedBaoLanhResult BaoLanh_GetListOfReplacable_Preview(decimal prKey,decimal pr_key_hsbt_ct, string currentUserEmail, string? ma_donvi_tt);
    Task<string> GuiBaoLanh(decimal pr_key, decimal pr_key_hsbt_ct,string path_bl, string currentUserEmail, string receiving_emails, string receiving_phones, string? ma_donvi_tt);
    Task<string> UploadAppraisalImage(UploadFileContent listFile, int pr_key, Guid oid, int stt, string maHmuc, string dienGiai, string maHmucSc);
    DownloadFileResult DownloadTtrinh11_MDF1(int pr_key);
    Task<List<ImageResponse>> GetListAppraisalImage(int pr_key);

    //Task<string> approveAppraisal(int pr_key, string oid, string ghiChu);
    Task<List<GDDKResponse>> GetAnhGDDK(int pr_key);
    Task<string> UpdateAppraisalImage(int pr_key, string dienGiai, string maHmuc, int stt, string maHmucSc);
    Task<List<HsgdDgCt>> GetAnhDuyetGia(int pr_key, bool loai_dg);
    List<DmTte> getListTienTe();
    List<DmTtrangGd> getListTtrangGd();
    List<DmHieuxe> getListHieuxe();
    List<DmLoaixe> getListLoaixe(int prKeyHieuXe);
    Task<string> ChuyenAnhGDTT(string soHsgdChuyen, string soHsgdNhan);
    Task<string> UploadAnhDuyetgia(UploadFileContent listFile, int pr_key, bool loai_dg);
    Task<string> UpdateAnhDuyetGia(int pr_key, bool loai_dg, string de_xuat, decimal so_tien);
    Task<List<HsgdDg>> GetThongTinDuyetGia(int pr_key);
    List<DmLoaiHinhTd> getListLoaiChiPhi();
    List<DmLdonBt> GetDmLdonBt();
    Task<List<ProductInfoResponse>> GetProductInfo(string soDonBh, string soDonBhBs);
    string uploadSampleFile(string localPath);
    Task<string> GetSoDonBh(int prKey);
    Guid GetOidByEmail(string email);
    string GenerateWordDocumentWithImages(List<string> imagePaths, string tempDir);
    Task<List<DanhMuc>> GetListLoaiBang();
   Task< TraSeri> TracuuPhi(string so_donbh_tracuu, string so_seri_tracuu, int nam_tracuu, int nam_tracuu_goc, string ma_donvi_tracuu);

    ReloadSumChecker ReloadSumCheck(int prKey);
    Task<decimal> GetPrKeyBySoHsgd(string soHsgd);

    public string LoiGiamDinh(int pr_key, string currentUserEmail, int thieuAnhGDDk, int chuaThuPhi, int saidkdk, int saiphancap, int trucloibh, int saiphamkhac);
    Task<string> UpdateURLImage(UploadFileContent listFile, int pr_key_ct, int pr_key, Guid oid);
    Task<string> UpdateURLAnhDuyetgia(UploadFileContent listFile, int prKey, int prKeyDgCt);
    List<DmUserView> GetListGDV();
    Task<string> GetAnhKbttCtu(string so_hsgd);
    Task<string> DeleteAnhHsgdCt(List<decimal> listKey);
    Task<List<DmDkbh>> GetListDkbh(string ma_sp);
    Task<bool> KTPquyen_YCBS(decimal pr_key,string email);

    string PheDuyet_NgoaiPhanCap_12(int pr_key, string currentUserEmail, decimal sum_sotien_hoso, string? ghiChu = "");
    string TraHS_NgoaiPhanCap_12(int pr_key, string currentUserEmail, string? ghiChu = "");

    string ChuyenTrinh_NgoaiPhanCap(int pr_key, string currentUserEmail, string? ghiChu = "");
    Task<dynamic> ProcessCRMAsync(int pr_key);
    Task<string> GetAnhKbttCt(string so_hsgd);
    string UpdatePathBaoLanh(decimal pr_key_hsbt_ct, string file_path);
    bool KyBaoLanh(decimal pr_key_hsgd_ctu, decimal pr_key_hsbt_ct, string file_path, string email, string SignContent);
    string GetFilePathBaoLanh(decimal pr_key_hsbt_ct);
    HsgdDxCt? GetTTBaoLanh(decimal pr_key_hsgd_dx_ct);
    HsgdDxCt GetHsgdDxCt(decimal pr_key_hsbt_ct);
    string GetThongTinKyDienTu(decimal pr_key_hsgd_ctu, string currentUserEmail);
    string CheckTrungHsbtUpdate(decimal pr_key_hsgd_ctu);
    string CheckThoiHanSDBS(string so_donbh, DateTime? ngay_tthat, string so_seri);
    GetListFileResponse GetListFile(decimal pr_key_hsgd_ctu);
    HsgdAttachFile GetAttachFileByPrKey(string pr_key);
    Task<bool> UploadHsgdAttachFiles(List<HsgdAttachFile> fileAttach);
    Task<string> DeleteAttachFile(string pr_key);
    Task<string> UpdateHoanThienHstt(decimal prKeyHsgdCtu, bool hoanThienHstt, string currentUserEmail);
}