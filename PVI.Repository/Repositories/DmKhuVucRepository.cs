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

namespace PVI.Repository.Repositories
{

    /* Implementation cho interface Khu vực 
     * lhkhanh - 01/10/2024
     */

    // Kế thừa base.
    public class DmKhuVucRepository : GenericRepository<DmKhuvuc>, IDmKhuVucRepository
    {
        public class DanhSachKhuVuc
        {
            public int count { get; set; }
            public List<DmKhuvuc> danhSachKhuVuc { get; set; }
        }

        public DmKhuVucRepository(GdttContext context, Pvs2024Context context_pias, Serilog.ILogger logger, IConfiguration conf) : base(context, context_pias, logger, conf)
        {

        }

        // Lấy danh sách các khu vực
        // Tham số: Page & Limit - Dùng để phân trang.
        public DanhSachKhuVuc GetDanhSachKhuVuc(int pageNumber, int limit, DmKhuvuc filter, string currentUserEmail)
        {
           
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
            if (currentUser != null)
            {
                List<DmKhuvuc> list_kv = (from khuvuc in _context.DmKhuvucs
                                           join tinhtp in _context.DmTinhs on khuvuc.Tinhtp equals tinhtp.MaTinh
                                           join quanhuyen in _context.DmTinhs on khuvuc.QuanHuyen equals quanhuyen.MaTinh
                                           join donvi in _context.DmDonvis on khuvuc.MaDonvi equals donvi.MaDonvi
                                           where (
                                            // Kiểm tra phân quyền và mã đơn vị của user, hiển thị theo mã đơn vị.
                                            ((currentUser.MaDonvi != "00") ? khuvuc.MaDonvi.Equals(currentUser.MaDonvi) : true) &&
                                            (filter.MaKv != null ? khuvuc.MaKv.Contains(filter.MaKv) : true) &&
                                            (filter.TenKv != null ? khuvuc.TenKv.Contains(filter.TenKv) : true) &&
                                            (filter.TenTinhtp != null ? tinhtp.TenTinh.Contains(filter.TenTinhtp) : true) &&
                                            (filter.TenQuanHuyen != null ? quanhuyen.TenTinh.Contains(filter.TenQuanHuyen): true) &&
                                            (filter.TenDonvi != null ? donvi.TenDonvi.Contains(filter.TenDonvi) : true) &&
                                            (filter.SuDung != null ? khuvuc.SuDung == filter.SuDung : true) 
                                           
                                           )
                                           orderby khuvuc.PrKey descending
                                           select new DmKhuvuc
                                           {
                                               PrKey = khuvuc.PrKey,
                                               MaKv = khuvuc.MaKv,
                                               TenKv = khuvuc.TenKv,
                                               Tinhtp  = khuvuc.Tinhtp,
                                               TenTinhtp = tinhtp.TenTinh,
                                               QuanHuyen = khuvuc.QuanHuyen,
                                               TenQuanHuyen = quanhuyen.TenTinh,
                                               MaDonvi = khuvuc.MaDonvi,
                                               TenDonvi = donvi.TenDonvi,
                                               SuDung = khuvuc.SuDung,
                                               NgayTao = khuvuc.NgayTao,
                                               MaUser = khuvuc.MaUser
                                           }
                          ).ToList();

                DanhSachKhuVuc dskv = new DanhSachKhuVuc
                {
                    count = list_kv.Count,
                    danhSachKhuVuc = list_kv.Skip(limit * (pageNumber - 1)).Take(limit).ToList(),
                };
                return dskv;
            }
            else
            {
                return null;
            }
        }

        // Lấy danh sách tỉnh và các quận huyện
        public List<DmTinh> getListTinh()
        {
            return _context.DmTinhs.Where(x => x.TongHop == 1).ToList();
        }

        public List<DmTinh> getListQuanHuyen(string MaTinh)
        {
            return _context.DmTinhs.Where(x => x.TongHop == 0 && x.MaTinh.Contains(MaTinh)).ToList();
        }

        // Tạo khu vực mới 
        public async Task<string> createKhuVuc(DmKhuvuc khuvuc, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
            if (currentUser != null)
            {
                DmKhuvuc checkDuplicate = _context.DmKhuvucs.Where(x => x.MaKv.Equals(khuvuc.MaKv)).FirstOrDefault();
                if (checkDuplicate == null)
                {
                    try
                    {
                        khuvuc.PrKey = 0;
                        khuvuc.NgayTao = DateTime.Now; // Chỉnh ngày cập nhật.
                        khuvuc.MaUser = currentUser.MaUser;
                        khuvuc.MaDonvi = currentUser.MaDonvi;
                        _context.DmKhuvucs.Add(khuvuc);
                        await _context.SaveChangesAsync();

                        return khuvuc.PrKey.ToString();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("dbContextTransaction Exception when createKhuVuc: " + ex.ToString());
                        _logger.Error("Error record: " + JsonConvert.SerializeObject(khuvuc));
                        await _context.DisposeAsync();
                        throw;
                    }
                }
                else
                {
                    return "Khu vực này đã tổn tại";
                }
            } else
            {
                return "OID User hiện tại bị lỗi";
            }
        }

        // Update khu vực
        public async Task<string> updateKhuVuc(int prKey, DmKhuvuc khuvuc, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
            if (currentUser != null)
            {
                DmKhuvuc checkExist = _context.DmKhuvucs.Where(x => x.PrKey == prKey).FirstOrDefault();
                if (checkExist != null)
                {
                    try
                    {
                        checkExist.TenKv = khuvuc.TenKv;
                        checkExist.Tinhtp = khuvuc.Tinhtp;
                        checkExist.QuanHuyen = khuvuc.QuanHuyen;
                        checkExist.SuDung = khuvuc.SuDung;
                        checkExist.MaUser = currentUser.MaUser; // Chỉnh ngày cập nhật
                        
                        _context.DmKhuvucs.Update(checkExist);
                        await _context.SaveChangesAsync();
                        return checkExist.PrKey.ToString();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("dbContextTransaction Exception when updateKhuVuc: " + ex.ToString());
                        _logger.Error("Error record: " + JsonConvert.SerializeObject(checkExist));
                        _context.Dispose();
                        throw;
                    }

                }
                else
                {
                    return $"Khu vực với PrKey {prKey} không tồn tại.";
                }
            } else
            {
                return "User không tồn tại";
            }
        }



    }
}