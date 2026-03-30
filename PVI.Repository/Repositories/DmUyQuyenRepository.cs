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

namespace PVI.Repository.Repositories
{

    /* Implementation cho interface Thiết lập ủy quyền 
     * lhkhanh - 01/10/2024
     */

    // Kế thừa base.
    public class DmUyQuyenRepository : GenericRepository<DmUqHstpc>, IDmUyQuyenRepository
    {
        public class DanhSachUyQuyen
        {
            public int count { get; set; }
            public List<DmUqHstpc> danhSachUyQuyen { get; set; }
        }

        public DmUyQuyenRepository(GdttContext context, Pvs2024Context context_pias, Serilog.ILogger logger, IConfiguration conf) : base(context, context_pias, logger, conf)
        {

        }

        // Sử dụng để lấy các loại ủy quyền
        // khanhlh - 20/03/2025
        public Dictionary<string, string> getTypeUyQuyen()
        {
            Dictionary<string, string> typeUyQuyen = new Dictionary<string, string>();

            typeUyQuyen.Add("6", "UQ phê duyệt HS");
            typeUyQuyen.Add("10", "UQ chuyển CPD HS");
            typeUyQuyen.Add("12", "UQ chuyển HS ngoài phân cấp");

            return typeUyQuyen;
        }

        // Lấy danh sách các ủy quyền 
        // Tham số: Page & Limit - Dùng để phân trang.
        public DanhSachUyQuyen GetDanhSachUyQuyen(int pageNumber, int limit, DmUqHstpc filter, string currentUserEmail)
        {
           
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();

            // Trường này sử dụng để so sánh số tiền ủy quyền.
            string soTienUQ = filter.GhSotienUq != null ? filter.GhSotienUq.ToString() : "";

            if (currentUser != null)
            {
                List<DmUqHstpc> list_uq = (from uyquyen in _context.DmUqHstpcs
                                           join user in _context.DmUsers on uyquyen.MaUserUq equals user.Oid.ToString()
                                           join donvi in _context.DmDonvis on uyquyen.MaDonvi equals donvi.MaDonvi
                                           orderby uyquyen.PrKey descending
                                           where (
                                            // Kiểm tra phân quyền và mã đơn vị của user, hiển thị theo mã đơn vị.
                                            ((currentUser.MaDonvi != "00" && currentUser.MaDonvi != "31" && currentUser.MaDonvi != "32") ? uyquyen.MaDonvi.Equals(currentUser.MaDonvi) : true) &&
                                            (filter.TenDonvi != null ? donvi.TenDonvi.Contains(filter.TenDonvi) : true) &&
                                            (filter.GhSotienUq != null ? uyquyen.GhSotienUq.ToString().StartsWith(soTienUQ) : true) &&
                                            (filter.NgayHl != null ? uyquyen.NgayHl.Value.Date >= filter.NgayHl.Value.Date : true) &&
                                            (filter.NgayCapnhat != null ? uyquyen.NgayCapnhat.Value.Date >= filter.NgayCapnhat.Value.Date : true) &&
                                            (filter.TenUserUq != null ? user.TenUser.Contains(filter.TenUserUq) : true) &&
                                            (filter.LoaiUyquyen != null ? uyquyen.LoaiUyquyen == filter.LoaiUyquyen : true)

                                           )
                                           select new DmUqHstpc
                                           {
                                               PrKey = uyquyen.PrKey,
                                               MaDonvi = uyquyen.MaDonvi,
                                               TenDonvi = donvi.TenDonvi,
                                               GhSotienUq = uyquyen.GhSotienUq,
                                               NgayHl = uyquyen.NgayHl,
                                               NgayCapnhat = uyquyen.NgayCapnhat,
                                               MaUserUq = uyquyen.MaUserUq,
                                               TenUserUq = user.TenUser,
                                               LoaiUyquyen = (uyquyen.LoaiUyquyen == "10" ? "UQ chuyển CPD HS" : (uyquyen.LoaiUyquyen == "6" ? "UQ phê duyệt HS" : (uyquyen.LoaiUyquyen == "12" ? "UQ chuyển HS ngoài phân cấp" : ""))),

                                           }
                          ).ToList();

                DanhSachUyQuyen dsuq = new DanhSachUyQuyen
                {
                    count = list_uq.Count,
                    danhSachUyQuyen = list_uq.Skip(limit * (pageNumber - 1)).Take(limit).ToList(),
                };
                return dsuq;
            } else
            {
                return null;
            }
        }

        // Lấy danh sách các user có thể gắn ủy quyền
        public List<DmUserView> getListUserUyQuyen (string maDonvi, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
            if (currentUser != null && (currentUser.MaDonvi == "00" || currentUser.MaDonvi == "31" || currentUser.MaDonvi == "32"))
            {
                //int[] allowedUserTypes = new int[5] { 1, 6, 9, 10, 11 }; // Các loại user có thể được phân quyền.
                if (currentUser.LoaiUser == 1 || currentUser.LoaiUser == 6 || currentUser.LoaiUser == 9 || currentUser.LoaiUser == 10 || currentUser.LoaiUser == 11 || currentUser.LoaiUser == 16)
                {
                    //List<DmUser> listUser1 = _context.DmUsers.Where(x => (x.LoaiUser == 1 || x.LoaiUser == 6 || x.LoaiUser == 8 || x.LoaiUser == 9 || x.LoaiUser == 10 || x.LoaiUser == 11)).ToList();

                    List<DmUserView> listUser = (from x in _context.DmUsers
                                                 where (x.LoaiUser == 1 || x.LoaiUser == 4 || x.LoaiUser == 6 || x.LoaiUser == 8 || x.LoaiUser == 9 || x.LoaiUser == 10 || x.LoaiUser == 11 || x.LoaiUser == 16)
                                                 orderby x.LoaiUser descending
                                                 select new DmUserView
                                                 {
                                                     Oid = x.Oid,
                                                     TenUser = x.TenUser,
                                                     MaUser = x.MaUser,
                                                     LoaiUser = x.LoaiUser
                                                 }).Distinct().ToList();
                    return listUser;
                }
                else
                {
                    //List<DmUser> listUser1 = _context.DmUsers.Where(x => x.MaDonvi.Equals(maDonvi) && (x.LoaiUser == 1 || x.LoaiUser == 6 || x.LoaiUser == 8 || x.LoaiUser == 9 || x.LoaiUser == 10 || x.LoaiUser == 11)).ToList();
                    //return listUser;
                    return new List<DmUserView>();
                }
                   
            } else
            {
                return new List<DmUserView> ();
            }
        }
        
        // Tạo ủy quyền mới 
        public async Task<string> createUyQuyen(DmUqHstpc uyQuyen, string currentUserEmail)
        {
            DmUqHstpc checkDuplicate = _context.DmUqHstpcs.Where(x => x.MaDonvi.Equals(uyQuyen.MaDonvi) && x.GhSotienUq == uyQuyen.GhSotienUq && x.NgayHl.Value.Date == uyQuyen.NgayHl.Value.Date && x.MaUserUq.Equals(uyQuyen.MaUserUq) && x.LoaiUyquyen.Equals(uyQuyen.LoaiUyquyen)).FirstOrDefault();
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();

            bool checkPhanQuyenUser = false; // Kiểm tra lại phân quyền tài khoản.

            if (currentUser != null)
            {

                int[] allowedUserTypes = new int[6] { 1, 6, 9, 10, 11, 16 }; // Các loại user có thể được phân quyền.
                string[] allowedCompanyCode = new string[3] { "00", "31", "32" };
                if (!Array.Exists(allowedCompanyCode, x => x == currentUser.MaDonvi) && !Array.Exists(allowedUserTypes, x => x == currentUser.LoaiUser))
                {
                    return "User không được phân quyền";
                }
            } else
            {
                return "User không tồn tại";
            }
            if (checkDuplicate == null)
            {
                try
                {
                    uyQuyen.PrKey = 0;
                    if (uyQuyen.LoaiUyquyen.Contains("CPD"))
                    {
                        uyQuyen.LoaiUyquyen = "10";
                    }
                    else if (uyQuyen.LoaiUyquyen.Contains("duyệt"))
                    {
                        uyQuyen.LoaiUyquyen = "6";
                    } else if (uyQuyen.LoaiUyquyen.Contains("ngoài phân cấp"))
                    {
                        uyQuyen.LoaiUyquyen = "12";
                    }

                    uyQuyen.NgayCapnhat = DateTime.Now; // Chỉnh ngày cập nhật.
                    _context.DmUqHstpcs.Add(uyQuyen);
                    await _context.SaveChangesAsync();

                    return uyQuyen.PrKey.ToString();
                }
                catch (Exception ex)
                {
                    _logger.Error("dbContextTransaction Exception when createUyQuyen: " + ex.ToString());
                    _logger.Error("Error record: " + JsonConvert.SerializeObject(uyQuyen));
                    await _context.DisposeAsync();
                    throw;
                }
            } else
            {
                return "Ủy quyền này đã tổn tại";
            }
        }

        // Update điểm trực
        public async Task<string> updateUyQuyen(int prKey, DmUqHstpc uyQuyen, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
            if (currentUser != null)
            {
                int[] allowedUserTypes = new int[6] { 1, 6, 9, 10, 11, 16 }; // Các loại user có thể được phân quyền.
                string[] allowedCompanyCode = new string[3] { "00", "31", "32" };
                if (!Array.Exists(allowedCompanyCode, x=>x == currentUser.MaDonvi) && !Array.Exists(allowedUserTypes, x=> x == currentUser.LoaiUser))
                {
                    return "User không được phân quyền";
                }
            } else
            {
                return "User không tồn tại";
            }
            DmUqHstpc checkExist = _context.DmUqHstpcs.Where(x=>x.PrKey == prKey).FirstOrDefault();
            if (checkExist != null)
            {
                DmUqHstpc checkDuplicate = _context.DmUqHstpcs.Where(x => x.MaDonvi.Equals(uyQuyen.MaDonvi) && x.GhSotienUq == uyQuyen.GhSotienUq && x.NgayHl.Value.Date == uyQuyen.NgayHl.Value.Date && x.MaUserUq.Equals(uyQuyen.MaUserUq) && x.LoaiUyquyen.Equals(uyQuyen.LoaiUyquyen)).FirstOrDefault();
                if (checkDuplicate == null)
                {
                    try
                    {
                        if (uyQuyen.LoaiUyquyen.Contains("CPD"))
                        {
                            checkExist.LoaiUyquyen = "10";
                        } else if (uyQuyen.LoaiUyquyen.Contains("duyệt"))
                        {
                            checkExist.LoaiUyquyen = "6";
                        } 
                        else if (uyQuyen.LoaiUyquyen.Contains("ngoài phân cấp"))
                        {
                            checkExist.LoaiUyquyen = "12";
                        }
                        else
                        {
                            checkExist.LoaiUyquyen = uyQuyen.LoaiUyquyen;
                        }
                        checkExist.MaDonvi = uyQuyen.MaDonvi;
                        checkExist.GhSotienUq = uyQuyen.GhSotienUq;
                        checkExist.NgayHl = uyQuyen.NgayHl;
                        checkExist.MaUserUq = uyQuyen.MaUserUq;
                        
                        checkExist.NgayCapnhat = DateTime.Now; // Chỉnh ngày cập nhật
                        _context.DmUqHstpcs.Update(checkExist);
                        await _context.SaveChangesAsync();
                        return checkExist.PrKey.ToString();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("dbContextTransaction Exception when updateStation: " + ex.ToString());
                        _logger.Error("Error record: " + JsonConvert.SerializeObject(checkExist));
                        _context.Dispose();
                        throw;
                    }
                } else
                {
                    return "Đã tồn tại ủy quyền trùng lặp";
                }
            } else
            {
                return $"Ủy quyền với PrKey {prKey} không tồn tại.";
            }
        }


        
    }
}