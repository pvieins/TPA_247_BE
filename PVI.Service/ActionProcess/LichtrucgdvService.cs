using AutoMapper;
using Microsoft.Extensions.Configuration;
using PVI.Repository.Interfaces;
using PVI.Repository.Repositories;
using PVI.DAO.Entities.Models;
using PVI.Service.Request;
using static PVI.Repository.Repositories.LichtrucgdvRepository;

namespace PVI.Service
{

    /* Service cho danh mục lịch trực.
     * khanhlh - 22/08/2024
     */
    
    public class LichtrucgdvService
    {
        // Truyền các tham số vào service
        private readonly ILichTrucGDVRepository _lichtrucRepository;
        private readonly Serilog.ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public LichtrucgdvService(ILichTrucGDVRepository lichtructRepository, IMapper mapper, Serilog.ILogger logger, IConfiguration conf)
        {
            _lichtrucRepository = lichtructRepository;
            _mapper = mapper;
            _logger = logger;
            _configuration = conf;
        }

        /* Các function dưới được gọi từ Controller.
         * Tương tác với Repository.
         */

        // Tất cả điểm trực

        public DanhSachLichTruc searchFilterStationSchedule(string ma_kv, DateTime? ngay_xemlich)
        {
            DanhSachLichTruc list_lich_truc =  _lichtrucRepository.searchFilterStationSchedule(ma_kv, ngay_xemlich);
            return list_lich_truc;
        }
       
        // Các đầu GET:
        public List<DmKhuvuc> getListKhuVuc()
        {
            var list_khu_vuc = _lichtrucRepository.getListKhuVuc();
            return list_khu_vuc;
        }

        public List<DmGaraKhuvuc> getListGaraKhuVuc(string ma_kv)
        {
            var list_gara = _lichtrucRepository.getListGaraKhuVuc(ma_kv);
            return list_gara;
        }

        public List<DmUser> getListCanBoTruc()
        {
            var list_user = _lichtrucRepository.getListCanBoTruc();
            return list_user;
        }

        public List<GhichuLichtruc> getListGhiChuLichTruc(string ma_kv)
        {
            List<GhichuLichtruc>? list_ghi_chu = _lichtrucRepository.getListScheduleNotes(ma_kv).Result;
            return list_ghi_chu;
        }

        public List<LichTrucgdv> SearchLichTrucTheoFrKey(int fr_key)
        {
            var list_khu_vuc = _lichtrucRepository.SearchLichTrucTheoFrKey(fr_key);
            return list_khu_vuc;
        }

        // Thao tác với DB:

        public async Task<string> updateScheduleNote(string ma_kv, string ghiChu)
        {
            var result = await _lichtrucRepository.updateScheduleNote(ma_kv, ghiChu);
            return result;
        }

        public async Task<string> updateIndividualNote(int pr_key, string ghiChu)
        {
            var result = await _lichtrucRepository.updateIndividualNote(pr_key, ghiChu);
            return result;
        }

        public async Task<string> updateSchedulePerson(ThemXoaCanBoTruc themXoa)
        {
            var result = await _lichtrucRepository.updateSchedulePerson(themXoa.maKv, themXoa.maGara, themXoa.thu, themXoa.sangChieu, themXoa.maUserXoa, themXoa.maUserDangTruc);
            return result;
        }

    }
}