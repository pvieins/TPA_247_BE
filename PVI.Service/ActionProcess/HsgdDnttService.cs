using AutoMapper;
using PVI.Repository.Interfaces;
using PVI.Repository.Repositories;
using PVI.DAO.Entities.Models;
using PVI.Service.Request;
using Azure.Core;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using PVI.Helper;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace PVI.Service.ActionProcess
{
    public class HsgdDnttService
    {
        private readonly IHsgdDnttRepository _hsgdDnttRepository;
        private readonly Serilog.ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public HsgdDnttService(IHsgdDnttRepository hsgdDnttRepository, IHsgdTtrinhCtRepository hsgdTtrinhCtRepository, IHsgdTotrinhXmlRepository hsgdTotrinhXmlRepository, IMapper mapper, Serilog.ILogger logger, IConfiguration conf)
        {
            _hsgdDnttRepository = hsgdDnttRepository;
            _mapper = mapper;
            _logger = logger;
            _configuration = conf;
        }       
        public string CreateDNTT(DNTTRequest dNTTRequest, string pr_key_hsgd_ttrinh, string email_login)
        {
            string result = "";
            try
            {
                result = _hsgdDnttRepository.CreateDNTT(dNTTRequest,pr_key_hsgd_ttrinh, email_login);
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public Task<List<NguoiDeNghi>> GetListNguoiDeNghi(string ma_donvi)
        {
            var list = _hsgdDnttRepository.GetListNguoiDeNghi(ma_donvi);
            return list;
        }
        public Task<List<DanhMuc>> GetListDonViTT(string ma_donvi)
        {
            var list = _hsgdDnttRepository.GetListDonViTT(ma_donvi);
            return list;
        }
        public Task<List<DanhMuc>> GetListNhomKT(string ma_donvi)
        {
            var list = _hsgdDnttRepository.GetListNhomKT(ma_donvi);
            return list;
        }
        public Task<List<ThuHuong>> GetThongtinTKThuHuong(decimal pr_key_hsgd)
        {
            var LThuHuong = _hsgdDnttRepository.GetThongtinTKThuHuong(pr_key_hsgd);
            return LThuHuong;
        }
        public Task<List<DanhMuc>> GetListNguoiXuLy(string ma_donvi)
        {
            var list = _hsgdDnttRepository.GetListNguoiXuLy(ma_donvi);
            return list;
        }
        public PagedList<HsgdDnttView> GetListDntt(string email_login, DnttParameters dnttParameters)
        {
            var list = _hsgdDnttRepository.GetListDntt(email_login, dnttParameters);
            return list;
        }
        public async Task<string> DeleteDntt(string pr_key_dntt)
        {
            string result = "";
            try
            {
                result = await _hsgdDnttRepository.DeleteDntt(pr_key_dntt);
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public List<LichSuPheDuyet>? GetLichSuPheDuyet(decimal pr_key_ttoan_ctu)
        {
            var list = _hsgdDnttRepository.GetLichSuPheDuyet(pr_key_ttoan_ctu);
            return list;
        }
        public Task<List<NguoiDeNghi>> GetListCanBoTT(string ma_donvi)
        {
            var list = _hsgdDnttRepository.GetListCanBoTT(ma_donvi);
            return list;
        }
    }
}