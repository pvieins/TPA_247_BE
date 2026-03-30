using AutoMapper;
using Microsoft.Extensions.Configuration;
using PVI.DAO.Entities.Models;
using PVI.Helper;
using PVI.Repository.Interfaces;
using PVI.Repository.Repositories;
using PVI.Service.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PVI.Repository.Repositories.KbttCtuRepository;

namespace PVI.Service.ActionProcess
{
    public class KbttCtuService
    {
        // Truyền các tham số vào service
        private readonly IKbttCtuRepository _kbttCtuRepository;
        private readonly Serilog.ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public KbttCtuService(IKbttCtuRepository kbttCtuRepository, IMapper mapper, Serilog.ILogger logger, IConfiguration conf)
        {
            _kbttCtuRepository = kbttCtuRepository;
            _mapper = mapper;
            _logger = logger;
            _configuration = conf;
        }
        public async Task<PagedList<KbttCtuDto>> GetListPVIMobile(KbttCtuParameters parameters, string email, string MaDonbh)
        {
            var result = await _kbttCtuRepository.GetListPVIMobile(email, parameters, MaDonbh);
            return result;
        }
        public async Task<List<KbttCtuDto>> GetListPVIMobileExcel(KbttCtuParameters parameters, string email, string MaDonbh)
        {
            var result = await _kbttCtuRepository.GetListPVIMobileExcel(email, parameters, MaDonbh);
            return result;
        }
        public async Task<dynamic> GetDetailKbttCtu(decimal prKey)
        {
            var result = await _kbttCtuRepository.GetDetailKbttCtu(prKey);
            return result;
        }

        public async Task<string> CreateKbttCtu(CreateKbttCtuRequest request, string Email)
        {
            var result = await _kbttCtuRepository.CreateKbtt(request, Email);
            return result;
        }
        public async Task<List<ImageKbttResponse?>> GetListAnhKbtt(decimal prKey)
        {
            var result = await _kbttCtuRepository.GetListAnhKbtt(prKey);
            return result;
        }
        //public async Task<PagedList<dynamic>> GetListAddHoSo(ListAddNewParameters parameters)
        //{
        //    var result = await _kbttCtuRepository.GetListHoSo(parameters);
        //    return result;
        //}
        public async Task<PagedList<ListAddNewResponse>> GetListAddHoSo(ListAddNewParameters parameters, string MaDonbh)
        {
            var result = await _kbttCtuRepository.GetListHoSo(parameters, MaDonbh);
            return result;
        }
        public async Task<KbttCtu> UpdateKbttCtu(decimal prKey, KbttCtuRequest request)
        {
            var result = await _kbttCtuRepository.UpdateKbtt(prKey, request);
            return result;
        }

        public async Task<string> TaoDonHsgd(decimal prKey, string maLhsbt, string donviBth, string MaDonbh)
        {
            var result = await _kbttCtuRepository.TaoDonHsgd(prKey, maLhsbt, donviBth, MaDonbh);
            return result;
        }
        public async Task<string> CapNhatSoHsgd(DateTime startDate, DateTime endDate)
        {
            var result = await _kbttCtuRepository.CapNhatSoHsgd(startDate, endDate);
            return result;
        }
        public async Task<List<LoaiHinhBhDTO>> GetListLoaiHinhBh()
        {
            var result = await _kbttCtuRepository.GetListLoaiHinhBh();
            return result;
        }
    }
}
