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
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System.Runtime.CompilerServices;

namespace PVI.Repository.Repositories
{

    /* Implementation cho interface Gara Khu vực 
     * lhkhanh - 01/10/2024
     */

    // Kế thừa base.
    public class DmGaraKhuvucRepository : GenericRepository<DmGaraKhuvuc>, IDmGaraKhuVucRepository
    {
        public class DanhSachGaraKhuvuc
        {
            public int count { get; set; }
            public List<DmGaraKhuvuc> danhSachGaraKhuvuc { get; set; }
        }

        public class GaraKhuVuc
        {
            public string? TenGara { get; set; }
            public string? MaGara { get; set; }
        }

        public DmGaraKhuvucRepository(GdttContext context, Pvs2024Context context_pias, Serilog.ILogger logger, IConfiguration conf) : base(context, context_pias, logger, conf)
        {

        }
        
        // Lấy danh sách các gara khu vực
        // Tham số: Page & Limit - Dùng để phân trang.
        public DanhSachGaraKhuvuc GetDanhSachGaraKhuvuc(int pageNumber, int limit, DmGaraKhuvuc filter, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
            if (currentUser != null)
            {
                List<DmGaraKhuvuc> list_kv = (from garaKV in _context.DmGaraKhuvucs
                                          join donvi in _context.DmDonvis on garaKV.MaDonvi equals donvi.MaDonvi
                                          join khuvuc in _context.DmKhuvucs on garaKV.MaKv equals khuvuc.MaKv
                                          where (
                                          
                                           // Kiểm tra phân quyền và mã đơn vị của user, hiển thị theo mã đơn vị. Nếu đơn vị là 00 thì quét hết, còn không thì quét các gara có cùng mã đơn vị với user
                                           ((currentUser.MaDonvi != "00") ? garaKV.MaDonvi.Equals(currentUser.MaDonvi) : true) &&
                                           (filter.MaKv != null ? garaKV.MaKv.Contains(filter.MaKv) : true) &&
                                           (filter.TenKv != null ? garaKV.TenKv.Contains(filter.TenKv) : true) &&
                                           (filter.Stt != null ? garaKV.Stt == filter.Stt : true) &&
                                           (filter.MaGara != null ? garaKV.MaGara.Contains(filter.MaGara) : true) &&
                                           (filter.TenGara != null ? garaKV.TenGara.Contains(filter.TenGara) : true) &&
                                           (filter.SuDung != null ? garaKV.SuDung == filter.SuDung : true) &&
                                           (filter.TenDonvi != null ? donvi.TenDonvi.Contains(filter.TenDonvi) : true)
                                          )
                                          select new DmGaraKhuvuc
                                          {
                                              PrKey = garaKV.PrKey,
                                              Stt = garaKV.Stt,
                                              MaKv = garaKV.MaKv,
                                              TenKv = khuvuc.TenKv,
                                              MaGara = garaKV.MaGara,
                                              TenGara = _context.DmGaRas.Where(x=>x.MaGara.Equals(garaKV.MaGara)).FirstOrDefault().TenGara,
                                              MaDonvi = garaKV.MaDonvi,
                                              TenDonvi = donvi.TenDonvi,
                                              SuDung = garaKV.SuDung,
                                              NgayCapnhat = garaKV.NgayCapnhat,
                                              MaUser = garaKV.MaUser
                                          }
                          ).ToList();

                DanhSachGaraKhuvuc dskv = new DanhSachGaraKhuvuc
                {
                    count = list_kv.Count,
                    danhSachGaraKhuvuc = list_kv.Skip(limit * (pageNumber - 1)).Take(limit).ToList(),
                };
                return dskv;
            }
            else
            {
                return null;
            }
        }

        // Lấy danh sách để về sau thêm sửa.
        public List<GaraKhuVuc> getListGarageKhuVuc(int pageNumber, int pageSize)
        {
            // Lấy danh sách tất cả gara
            List<DmGaRa> listGara = (from gara in _context.DmGaRas
                                     select new DmGaRa
                                     {
                                         TenGara = gara.TenGara,
                                         TenTat = gara.TenTat,
                                         MaGara = gara.MaGara,

                                     }).ToList();

            // Lấy danh sách tất cả điểm trực 
            List<DmDiemtruc> listDt = (from diemtruc in _context.DmDiemtrucs
                                       where diemtruc.Active == true
                                       select new DmDiemtruc
                                       {
                                           MaDiemtruc = diemtruc.MaDiemtruc,
                                           TenDiemtruc = diemtruc.TenDiemtruc
                                       }).ToList();

            // Danh sách gara để trả về, sẽ bao gồm cả gara và điểm trực.
            // Cần check lại nghiệp vụ vì hơi rối.
            List<GaraKhuVuc> toBeReturned = new List<GaraKhuVuc>();

            listGara.ForEach(x =>
            {
                GaraKhuVuc newGaraKV = new GaraKhuVuc
                {
                    TenGara = x.TenTat,
                    MaGara = x.MaGara,
                };
                toBeReturned.Add(newGaraKV);
            });

            listDt.ForEach(x =>
            {
                GaraKhuVuc newGaraKV = new GaraKhuVuc
                {
                    TenGara = x.TenDiemtruc,
                    MaGara = x.MaDiemtruc,
                };
                toBeReturned.Add(newGaraKV);
            });

            return toBeReturned.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
        }

        // Lấy danh sách khu vực.
        public List<DmKhuvuc> getListKhuVuc ()
        {
            return _context.DmKhuvucs.Where(x => x.SuDung == true).ToList();
        }

        // Tạo khu vực mới 
        public async Task<string> createGaraKhuVuc(DmGaraKhuvuc garageKhuvuc, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
            if (currentUser != null)
            {
                DmGaraKhuvuc checkDuplicate = _context.DmGaraKhuvucs.Where(x => x.Stt == garageKhuvuc.Stt && x.MaGara.Equals(garageKhuvuc.MaGara) && x.MaKv.Equals(garageKhuvuc.MaKv)).FirstOrDefault();
                if (checkDuplicate == null)
                {
                    try
                    { 
                        garageKhuvuc.PrKey = 0;
                        garageKhuvuc.TenGara = _context.DmGaRas.Where(x => x.MaGara.Equals(garageKhuvuc.MaGara)).FirstOrDefault().TenGara;
                        garageKhuvuc.TenKv = _context.DmKhuvucs.Where(x => x.MaKv.Equals(garageKhuvuc.MaKv)).FirstOrDefault().TenKv;
                        garageKhuvuc.NgayCapnhat = DateTime.Now; // Chỉnh ngày cập nhật.
                        garageKhuvuc.MaUser = currentUser.MaUser;
                        garageKhuvuc.MaDonvi = currentUser.MaDonvi;

                        _context.DmGaraKhuvucs.Add(garageKhuvuc);
                        await _context.SaveChangesAsync();

                        return garageKhuvuc.PrKey.ToString();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("dbContextTransaction Exception when createDmGaraKhuvuc: " + ex.ToString());
                        _logger.Error("Error record: " + JsonConvert.SerializeObject(garageKhuvuc));
                        await _context.DisposeAsync();
                        throw;
                    }
                }
                else
                {
                    return "Gara khu vực này đã tổn tại";
                }
            }
            else
            {
                return "OID User hiện tại bị lỗi";
            }
        }

        // Update gara khu vực
        public async Task<string> updateGaraKhuVuc(int prKey, DmGaraKhuvuc garageKhuvuc, string currentUserEmail)
        {
            // Kiểm tra phân quyền.
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
            if (currentUser != null)
            {
                // Kiểm tra xem có tồn tại hay chưa.
                DmGaraKhuvuc checkExist = _context.DmGaraKhuvucs.Where(x => x.PrKey == prKey).FirstOrDefault();
                if (checkExist != null)
                {
                    // Kiểm tra xem có bị trùng không.
                    DmGaraKhuvuc checkDuplicate = _context.DmGaraKhuvucs.Where(x => x.Stt == garageKhuvuc.Stt && x.MaGara.Equals(garageKhuvuc.MaGara) && x.MaKv.Equals(garageKhuvuc.MaKv)).FirstOrDefault();
                    if (checkDuplicate == null)
                    {
                        try
                        {
                            checkExist.Stt = garageKhuvuc.Stt;
                            var gara = _context.DmGaRas.Where(x => x.MaGara.Equals(garageKhuvuc.MaGara) &&x.GaraTthai == true).FirstOrDefault();
                            if (gara != null)
                            {
                                checkExist.MaGara = garageKhuvuc.MaGara;
                                checkExist.TenGara = gara.TenGara;
                            }
                            else
                            {
                                return "Gara chọn không tồn tại. Vui lòng kiểm tra lại.";
                            }
                            var khuvuc = _context.DmKhuvucs.Where(x => x.MaKv != null && x.MaKv.Equals(garageKhuvuc.MaKv) && x.SuDung == true).FirstOrDefault();
                            if (khuvuc != null)
                            {
                                checkExist.MaKv = garageKhuvuc.MaKv;
                                checkExist.TenKv = khuvuc.TenKv;
                            }
                            else
                            {
                                return "Khu vực chọn không tồn tại. Vui lòng kiểm tra lại.";
                            }
                            garageKhuvuc.NgayCapnhat = DateTime.Now; // Chỉnh ngày cập nhật.
                            checkExist.MaUser = currentUser.MaUser;
                            checkExist.MaDonvi = currentUser.MaDonvi;

                            _context.DmGaraKhuvucs.Update(checkExist);
                            await _context.SaveChangesAsync();

                            return prKey.ToString();
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("dbContextTransaction Exception when updateDmGaraKhuvuc: " + ex.ToString());
                            _logger.Error("Error record: " + JsonConvert.SerializeObject(checkExist));
                            await _context.DisposeAsync();
                            throw;
                        }
                    }
                    else
                    {
                        return "Thông tin trùng với 1 khu vực đã tồn tại. Vui lòng kiểm tra lại.";
                    }
                } else
                {
                    return $"Gara với PrKey {prKey} không tồn tại";
                }
            }
            else
            {
                return "OID User hiện tại bị lỗi";
            }
        }


    }
}