using AutoMapper;
using Microsoft.Extensions.Configuration;
using PVI.Repository.Interfaces;
using PVI.Repository.Repositories;
using PVI.DAO.Entities.Models;
using PVI.Service.Request;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using static PVI.Repository.Repositories.BaoCaoRepository;
using static PVI.Repository.Repositories.BaoCaoHelper;
using System.Configuration;
using Microsoft.Office.Interop.Word;
namespace PVI.Service
{

    /* Service cho các báo cáo.
     * khanhlh - 28/10/2024
     */
    public class BaoCaoService
    {
        // Truyền các tham số vào service
        private readonly IBaoCaoRepository _BaoCaoRepository;
        private readonly Serilog.ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public BaoCaoService(IBaoCaoRepository bcRepository, IMapper mapper, Serilog.ILogger logger, IConfiguration conf)
        {
            _BaoCaoRepository = bcRepository;
            _mapper = mapper;
            _logger = logger;
            _configuration = conf;
        }

        // Lấy danh sách đơn vị
        public List<DmDonvi> getListDonvi_BaoCao(string currentUserEmail)
        {
            List<DmDonvi> searchResult = _BaoCaoRepository.getListDonvi_BaoCao(currentUserEmail);
            return searchResult;
        }

        public ThongKe_GDTT_DonVi_Response ThongKe_GDTT_DonVi(ThongKe_GDTT_DonVi_Filter filter, string currentUserEmail)
        {
            ThongKe_GDTT_DonVi_Response searchResult = _BaoCaoRepository.ThongKe_GDTT_Donvi(filter, currentUserEmail);
            return searchResult;
        }

        public ThongKe_GDTT_GDV_Response ThongKe_GDTT_GDV(ThongKe_GDTT_GDV_Filter filter, string currentUserEmail)
        {
            ThongKe_GDTT_GDV_Response searchResult = _BaoCaoRepository.ThongKe_GDTT_GDV(filter, currentUserEmail);
            return searchResult;
        }


        public async Task<ThongKeGDTT_General_Response> ThongKeGDTT(ThongKeGDTT_General_Main_Filter filter, int pageNumber, int pageSize, string currentUserEmail)
        {
            ThongKeGDTT_General_Response searchResult = await _BaoCaoRepository.ThongKeGDTT(filter, pageNumber, pageSize, currentUserEmail);
            return searchResult;
        }

        // Tra cứu giá phụ tùng
        public SearchGiaPhuTungResponse SearchGiaPhuTung (SearchGiaPhuTung_Main_Filter filter, int pageNumber, int pageSize, string currentUserEmail)
        {
            SearchGiaPhuTungResponse searchResult = _BaoCaoRepository.SearchGiaPhuTung(filter, pageNumber, pageSize, currentUserEmail);
            return searchResult;
        }


        public HSTPC_Response BCHSTPC_TrenPhanCap(HSTPC_Filter filter, int pageNumber, int pageSize, string currentUserEmail)
        {
            HSTPC_Response searchResult = _BaoCaoRepository.BCHSTPC_TrenPhanCap(filter, pageNumber, pageSize, currentUserEmail);
            return searchResult;
        }

        //Tra cứu giá trị thực tế
        public SearchGtttResponse SearchGttt(SearchGttt_Main_Filter filter, int pageNumber, int pageSize, string currentUserEmail)
        {
            SearchGtttResponse searchResult = _BaoCaoRepository.SearchGttt(filter, pageNumber, pageSize, currentUserEmail);
            return searchResult;
        }

        // Báo cáo thu hồi tài sản
        public BCThuHoiTSItemResponse BCThuHoiTS(BCThuHoiTS_Main_Filter filter, int pageNumber, int pageSize, string currentUserEmail)
        {
            BCThuHoiTSItemResponse searchResult = _BaoCaoRepository.SearchBCThuHoiTS(filter, pageNumber, pageSize, currentUserEmail);
            return searchResult;
        }
        // Export Báo cáo thu hồi tài sản
        public BCThuHoiTSItemResponse ExportBCThuHoiTS(BCThuHoiTS_Main_Filter filter, string currentUserEmail)
        {
            BCThuHoiTSItemResponse searchResult = _BaoCaoRepository.SearchBCThuHoiTS(filter, -1,-1, currentUserEmail);
            return searchResult;
        }
    }
}