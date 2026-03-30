using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PVI.DAO.Entities.Models;
using PVI.Repository.Interfaces;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Office.Interop.Word;
using System;
using Microsoft.Extensions.Primitives;
using System.Net.Http;

namespace PVI.Repository.Repositories
{

    /* Implementation cho interface Danh mục điểm trực.
     * lhkhanh - 22/08/2024
     */

    // Kế thừa base.
    public class DmDeviceRepository : GenericRepository<DmDevice>, IDmDeviceRepository
    {
        public DmDeviceRepository(GdttContext context, Pvs2024Context context_pias, Serilog.ILogger logger, IConfiguration conf) : base(context, context_pias, logger, conf)
        {

        }

        public class DanhSachDevice
        {
            public int count { get; set; } = 0;
            public List<DmDevice> listDevice { get; set; }
        }

        //Tra cứu Device theo filter.
        // Đẩy body vào filter search.
        public DanhSachDevice getListDevice(int pageNumber, int limit, DmDevice searchTarget, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.FirstOrDefault(x => x.Mail.Equals(currentUserEmail));

            if (currentUser != null)
            {
                // Nếu user thuộc trụ sở PVI hoặc là 1 trong 3 loại user cho phép.
                // Kiếm tra loại user trong bảng dm_loai_user.

                if (currentUser.MaDonvi.Equals("00") || currentUser.LoaiUser == 1 || currentUser.LoaiUser == 2 || currentUser.LoaiUser == 3)
                {
                    List<DmDevice> list_device = (from device in _context.DmDevices
                                       join donvi in _context.DmDonvis on device.MaDonvi equals donvi.MaDonvi
                                       where (
                                         (searchTarget.ImeiDevice != null ? device.ImeiDevice.Contains(searchTarget.ImeiDevice) : true) &&
                                         (searchTarget.AddressDevice != null ? device.AddressDevice.Contains(searchTarget.AddressDevice) : true) &&
                                         (searchTarget.MaUser != null ? device.MaUser.Contains(searchTarget.MaUser) : true) &&
                                         (searchTarget.TenDonvi != null ? donvi.TenDonvi.Contains(searchTarget.TenDonvi) : true) &&
                                         (searchTarget.TypeDevice != null ? device.TypeDevice.Equals(searchTarget.TypeDevice) : true) &&
                                         (searchTarget.Active != null ? device.Active == searchTarget.Active : true) &&
                                         (searchTarget.Description != null ? device.Description.Contains(searchTarget.Description) : true) &&
                                         (searchTarget.Status != null ? device.Status == searchTarget.Status : true)
                                         )
                                       orderby device.PrKey descending
                                       select new DmDevice
                                       {
                                           PrKey = device.PrKey,
                                           ImeiDevice = device.ImeiDevice,
                                           AddressDevice = device.AddressDevice,
                                           MaUser = device.MaUser,
                                           TenDonvi = donvi.TenDonvi,
                                           MaDonvi = device.MaDonvi,
                                           TypeDevice = device.TypeDevice,
                                           Active = device.Active,
                                           Description = device.Description,
                                           Status = device.Status,
                                       }
                          ).ToList();

                    // Trả về danh sách thiết bị.
                    DanhSachDevice result = new DanhSachDevice
                    {
                        count = list_device.Count(),
                        listDevice = list_device.Skip(limit * (pageNumber - 1)).Take(limit).ToList()
                    };
                    return result;

                // Nếu trường hợp user thuộc đơn vị 31 hoặc đơn vị 32:
                // Quét bảng dm_user, lấy ra tất cả các user cùng đơn vị.
                // Với mỗi user đã quét được, nếu user đó có thiết bị nào bên bảng dm_device thì lấy hết.

                } else if (currentUser.MaDonvi.Equals("31") || currentUser.MaDonvi.Equals("32"))
                {

                    List<DmDevice> list_device = (from device in _context.DmDevices
                                                  join user in _context.DmUsers on device.MaUser equals user.MaUser
                                                  join donvi in _context.DmDonvis on device.MaDonvi equals donvi.MaDonvi
                                       where (
                                         (user.MaDonvi.Equals(currentUser.MaDonvi)) && 
                                         (searchTarget.ImeiDevice != null ? device.ImeiDevice.Contains(searchTarget.ImeiDevice) : true) &&
                                         (searchTarget.AddressDevice != null ? device.AddressDevice.Contains(searchTarget.AddressDevice) : true) &&
                                         (searchTarget.MaUser != null ? device.MaUser.Contains(searchTarget.MaUser) : true) &&
                                         (searchTarget.TenDonvi != null ? donvi.TenDonvi.Contains(searchTarget.TenDonvi) : true) &&
                                         (searchTarget.TypeDevice != null ? device.TypeDevice.Equals(searchTarget.TypeDevice) : true) &&
                                         (searchTarget.Active != null ? device.Active == searchTarget.Active : true) &&
                                         (searchTarget.Description != null ? device.Description.Contains(searchTarget.Description) : true) &&
                                         (searchTarget.Status != null ? device.Status == searchTarget.Status : true) 
                                         )

                                       select new DmDevice
                                       {
                                           PrKey = device.PrKey,
                                           ImeiDevice = device.ImeiDevice,
                                           AddressDevice = device.AddressDevice,
                                           MaUser = device.MaUser,
                                           TenDonvi = donvi.TenDonvi,
                                           MaDonvi = device.MaDonvi,
                                           TypeDevice = device.TypeDevice,
                                           Active = device.Active,
                                           Description = device.Description,
                                           Status = device.Status,
                                           
                                       }
                        ).ToList();

                    // Trả về danh sách thiết bị.
                    DanhSachDevice result = new DanhSachDevice
                    {
                        count = list_device.Count(),
                        listDevice = list_device.Skip(limit * (pageNumber - 1)).Take(limit).ToList()
                    };
                        return result;
                } else
                {
                    return new DanhSachDevice
                    {
                        count = 0,
                        listDevice = new List<DmDevice>()
                    };

                }
            } else
            {
                return new DanhSachDevice();
            }
        }

        
        // Thêm mới thiết bị
        public async Task<string> createDevice(DmDevice device, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.FirstOrDefault(x => x.Mail.Equals(currentUserEmail));

            if (currentUser != null)
            {
                if ((currentUser.MaDonvi.Equals("00") && (currentUser.LoaiUser == 1 || currentUser.LoaiUser == 6)) || currentUser.LoaiUser == 9 || currentUser.LoaiUser == 10 || currentUser.LoaiUser == 11) {
                    // Kiểm tra nếu thiết bị không tồn tại thì mới tiến hành insert
                    var checkExist = _context.DmDevices.Where(x => x.ImeiDevice == device.ImeiDevice).FirstOrDefault();
                    if (checkExist == null)
                    {
                        DmUser toBeAssigned = _context.DmUsers.Where(x => x.MaUser.Equals(device.MaUser)).FirstOrDefault();
                        if (toBeAssigned != null) {
                            try
                            {
                                device.PrKey = 0;
                                device.NgayCnhat = DateTime.Now;
                                _context.DmDevices.Add(device);
                                await _context.SaveChangesAsync();
                                return device.PrKey.ToString();
                            }
                            catch (Exception ex)
                            {
                                _logger.Error("dbContextTransaction Exception when CreateHsgdTtrinh: " + ex.ToString());
                                _logger.Error("Error record: " + JsonConvert.SerializeObject(device));
                                await _context.DisposeAsync();

                                throw;
                            }
                        } else
                        {
                            return $"User với mã {device.MaUser} không tồn tại";
                        }
                    }
                    else
                    {
                        return "Device với Imei này đã tổn tại";
                    }
                } else
                {
                    return "User không được phân quyền";
                }
            } else
            {
                return "OID User hiện tại không tồn tại, yêu cầu kiểm tra lại.";
            }
        }


        // Cập nhật thiết bị
        public async Task<string> updateDevice(int prKey, DmDevice device, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.FirstOrDefault(x => x.Mail.Equals(currentUserEmail));

            if (currentUser != null)
            {
                // Kiểm tra nếu thiết bị không có tại thì mới tiến hành update
                var checkExist = _context.DmDevices.Where(x => x.PrKey == prKey).FirstOrDefault();
                if (checkExist != null)
                {
                    DmUser toBeAssigned = _context.DmUsers.Where(x => x.MaUser.Equals(device.MaUser)).FirstOrDefault();
                    if (toBeAssigned != null)
                    {
                        try
                        {
                            //checkExist.ImeiDevice = device.ImeiDevice;
                            checkExist.AddressDevice = device.AddressDevice;
                            checkExist.MaUser = device.MaUser;
                            checkExist.MaDonvi = device.MaDonvi;
                            checkExist.Active = device.Active;
                            checkExist.Description = device.Description;
                            checkExist.Status = device.Status;
                            checkExist.TypeDevice = device.TypeDevice;
                            checkExist.NgayCnhat = DateTime.Now;
                            _context.DmDevices.Update(checkExist);
                            await _context.SaveChangesAsync();
                            return checkExist.PrKey.ToString();
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("dbContextTransaction Exception when CreateHsgdTtrinh: " + ex.ToString());
                            _logger.Error("Error record: " + JsonConvert.SerializeObject(checkExist));
                            await _context.DisposeAsync();

                            throw;
                        }
                    }
                    else
                    {
                        return $"User với mã {device.MaUser} không tồn tại";
                    }
                }
                else
                {
                    return "Device với PrKey này không tổn tại";
                }
            }
            else
            {
                return "OID User hiện tại không tồn tại, yêu cầu kiểm tra lại.";
            }
        }
        

    }
}