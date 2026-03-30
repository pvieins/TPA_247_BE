using PVI.DAO.Entities.Models;
using PVI.Helper;
using PVI.Repository.Repositories;
using System.Linq.Expressions;
using static PVI.Repository.Repositories.BaoCaoRepository;

namespace PVI.Repository.Interfaces;

// Interface cho các báo cáo hệ thống.
// khanhlh - 22/10/2024

public interface IBaoCaoRepository : IGenericRepository<HsgdCtu>
{
    public List<DmDonvi> getListDonvi_BaoCao(string currentUserEmail);
    ThongKe_GDTT_DonVi_Response ThongKe_GDTT_Donvi(ThongKe_GDTT_DonVi_Filter filter, string currentUserEmail);
    ThongKe_GDTT_GDV_Response ThongKe_GDTT_GDV(ThongKe_GDTT_GDV_Filter filter, string currentUserEmail);
    SearchGiaPhuTungResponse SearchGiaPhuTung(SearchGiaPhuTung_Main_Filter filter, int pageNumber, int pageSize, string currentUserEmail);
    HSTPC_Response BCHSTPC_TrenPhanCap(HSTPC_Filter filter, int pageNumber, int pageSize, string currentUserEmail);
    SearchGtttResponse SearchGttt(SearchGttt_Main_Filter filter, int pageNumber, int pageSize, string currentUserEmail);
    BCThuHoiTSItemResponse SearchBCThuHoiTS(BCThuHoiTS_Main_Filter filter, int pageNumber, int pageSize, string currentUserEmail);
    Task<ThongKeGDTT_General_Response> ThongKeGDTT(ThongKeGDTT_General_Main_Filter filter, int pageNumber, int pageSize, string currentUserEmail);

}
