using AutoMapper;
using Microsoft.Extensions.Configuration;
using PVI.Repository.Interfaces;
using PVI.Repository.Repositories;
using PVI.DAO.Entities.Models;
using PVI.Service.Request;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using static PVI.Repository.Repositories.DmDeviceRepository;
namespace PVI.Service
{

    /* Service cho danh mục hạng mục sửa chữa.
     * khanhlh - 26/09/2024
     */
    public class DmDeviceService
    {
        // Truyền các tham số vào service
        private readonly IDmDeviceRepository _dmKV;
        private readonly Serilog.ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public DmDeviceService(IDmDeviceRepository kvRepository, IMapper mapper, Serilog.ILogger logger, IConfiguration conf)
        {
            _dmKV = kvRepository;
            _mapper = mapper;
            _logger = logger;
            _configuration = conf;
        }

        // Tất cả device
        public DanhSachDevice getListDevice(int pageNumber, int limit, DeviceFilter searchTarget, string currentUserEmail)
        {
            var DeviceFilter = _mapper.Map<DeviceFilter, DmDevice>(searchTarget);
            var list_Device = _dmKV.getListDevice(pageNumber, limit, DeviceFilter, currentUserEmail);
            return list_Device;
        }

        // Tất cả danh sách tỉnh
        public string createDevice(DeviceRequest toBeCreated, string currentUserEmail)
        {
            var deviceCreate = _mapper.Map<DeviceRequest, DmDevice>(toBeCreated);
            var list_Device = _dmKV.createDevice(deviceCreate, currentUserEmail).Result;
            return list_Device;
        }
        public string updateDevice(int prKey, DeviceRequest toBeUpdated, string currentUserEmail)
        {
            var DeviceUpdate = _mapper.Map<DeviceRequest, DmDevice>(toBeUpdated);
            var result = _dmKV.updateDevice(prKey, DeviceUpdate, currentUserEmail).Result;
            return result;
        }

    }
}