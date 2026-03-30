using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PVI.DAO.Entities.Models;
using PVI.Repository.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.Design;
using System.Security.Cryptography;
using Microsoft.Office.Interop.Word;
using System;
using System.Globalization;
using PVI.Helper;
using Diacritics;

namespace PVI.Repository.Repositories
{

    /* Implementation cho interface Phân quyền ký hồ sơ / ký số.
     * lhkhanh - 22/08/2024
     */
    
    // Kế thừa base.
    public class PquyenKyHsRepository : GenericRepository<DmPquyenKyhs>, IPquyenKyHsRepository
    {
        public PquyenKyHsRepository(GdttContext context, Pvs2024Context context_pias, Serilog.ILogger logger, IConfiguration conf) : base(context, context_pias, logger, conf)
        {

        }

        // Lay danh sach cac quyen ky
        public Task<List<DmPquyenKyhs>> getDigitalSignList(int pageNumber, int limit, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail == currentUserEmail).FirstOrDefault();
            string currentUser_PhanQuyen = checkPhanQuyen(currentUser);

            if (currentUser != null)
            {
                var list_ky_so = (from ky_so in _context.DmPquyenKyhs
                                  join user in _context.DmUsers on ky_so.MaUser equals user.Oid.ToString()
                                  join donvi in _context.DmDonvis on user.MaDonvi equals donvi.MaDonvi
                                  orderby ky_so.PrKey descending
                                  where (currentUser_PhanQuyen == "OTHER_AUTHORITY" ? ky_so.MaUserCapnhat.Equals(currentUser.MaUser) :
                                        (currentUser_PhanQuyen == "AUTHORITY_31_32" ? user.MaDonvi.Equals(currentUser.MaDonvi) :
                                        (currentUser_PhanQuyen == "HALF_AUTHORITY_31_32") ? user.MaUser == currentUser.MaUser : true))
                                  select new DmPquyenKyhs
                                  {
                                      PrKey = ky_so.PrKey,
                                      MaUser = ky_so.MaUser,
                                      TenUser = user.TenUser,
                                      Mail = user.Mail,
                                      MaSp = ky_so.MaSp,
                                      SoTien = ky_so.SoTien,
                                      TenDonVi = donvi.TenDonvi,
                                      IsActive = ky_so.IsActive,
                                      MaUserPias = user.MaUserPias,
                                      NgayCnhat = ky_so.NgayCnhat,
                                      MaUserCapnhat = ky_so.MaUserCapnhat,
                                      Count = _context.DmPquyenKyhs.Count()
                                  }
                      ).Skip(limit * (pageNumber - 1)).Take(limit).AsQueryable();
                return ToListWithNoLockAsync(list_ky_so);
            } else
            {
                return null;
            }
        }

        public Task<List<DmPquyenKyhs>> searchDigitalSignByFilter(int pageNumber, int limit, DmPquyenKyhs searchTarget, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail == currentUserEmail).FirstOrDefault();
            if (currentUser != null)
            {
                // Trường này sử dụng để query số tiền, convert 1 lần cho nhanh.
                string soTienDuyet = searchTarget.SoTien != null ? searchTarget.SoTien.ToString() : "";
                string currentUser_PhanQuyen = checkPhanQuyen(currentUser);

                var list_ky_so = (from ky_so in _context.DmPquyenKyhs
                                  join user in _context.DmUsers on ky_so.MaUser equals user.Oid.ToString()
                                  join donvi in _context.DmDonvis on user.MaDonvi equals donvi.MaDonvi
                                  orderby ky_so.PrKey descending
                                  where (
                                        // Phân quyền dữ liệu:
                                        // Nếu thuộc đơn vị 00 thì xem được hết.
                                        (currentUser_PhanQuyen == "OTHER_AUTHORITY" ? ky_so.MaUserCapnhat.Equals(currentUser.MaUser) :
                                        (currentUser_PhanQuyen == "AUTHORITY_31_32" ? user.MaDonvi.Equals(currentUser.MaDonvi) : 
                                        (currentUser_PhanQuyen == "HALF_AUTHORITY_31_32") ? user.MaUser == currentUser.MaUser : true)) &&
                                        (searchTarget.MaUser != null ? ky_so.MaUser.Contains(searchTarget.MaUser.ToLower()) : true) &&
                                        (searchTarget.TenUser != null ? user.TenUser.Contains(searchTarget.TenUser) : true) &&
                                        (searchTarget.Mail != null ? user.Mail.Contains(searchTarget.Mail) : true) &&
                                        (searchTarget.MaSp != null ? ky_so.MaSp.Contains(searchTarget.MaSp) : true) &&
                                        (searchTarget.SoTien != null ? ky_so.SoTien.ToString().StartsWith(soTienDuyet) : true) &&
                                        (searchTarget.TenDonVi != null ? donvi.TenDonvi.Contains(searchTarget.TenDonVi) : true) &&
                                        (searchTarget.IsActive != null ? (ky_so.IsActive == searchTarget.IsActive) : true) &&
                                        (searchTarget.MaUserPias != null ? user.MaUserPias.Contains(searchTarget.MaUserPias) : true) &&
                                        (searchTarget.NgayCnhat != null ? ky_so.NgayCnhat >= searchTarget.NgayCnhat : true)
                                        )

                                  select new DmPquyenKyhs
                                  {
                                      PrKey = ky_so.PrKey,
                                      MaUser = ky_so.MaUser,
                                      TenUser = user.TenUser,
                                      Mail = user.Mail,
                                      MaSp = ky_so.MaSp,
                                      SoTien = ky_so.SoTien,
                                      TenDonVi = donvi.TenDonvi,
                                      IsActive = ky_so.IsActive,
                                      MaUserPias = user.MaUserPias,
                                      NgayCnhat = ky_so.NgayCnhat,
                                      MaUserCapnhat = ky_so.MaUserCapnhat,
                                      Count = _context.DmPquyenKyhs.Count()
                                  }
                          ).Skip(limit * (pageNumber - 1)).Take(limit).AsQueryable();
                return ToListWithNoLockAsync(list_ky_so);
            } else
            {
                return null;
            }
        }

        // Function này dùng để kiểm tra phân quyền của tài khoản.
        // Tuỳ vào phân quyền mà các chức năng và dữ liệu sẽ được hiển thị nhiều ít khác nhau.
        private string checkPhanQuyen(DmUser currentUser)
        {
            if (currentUser != null)
            {
                // Cho full quyền nếu thuộc đơn vị 00 hoặc là quản trị đơn vị
                if (currentUser.MaDonvi.Equals("00") || currentUser.LoaiUser == 1 || currentUser.LoaiUser == 6)
                {
                    return "FULL_AUTHORITY";
                // Nếu thuộc đơn vị 31, 32 thì check quyền
                } else if (currentUser.MaDonvi.Equals("31") || currentUser.MaDonvi.Equals("32"))
                {
                    if (currentUser.LoaiUser == 9 || currentUser.LoaiUser == 10 || currentUser.LoaiUser == 11)
                    {
                        return "AUTHORITY_31_32";
                    } else
                    {
                        return "HALF_AUTHORITY_31_32";
                    }
                // Các trường hợp còn lại
                } else
                {
                    return "OTHER_AUTHORITY";
                }
            }
            else
            {
                return "KHONG_TON_TAI";
            }
        }

        // Lấy danh sách người dùng 
        public Task<List<DmUser>> getDigitalSignUserList(int pageNumber, int limit, string? maUser, string? tenUser, string? dienthoai)
        {
            string decodedTenUser = (tenUser != null ? Uri.UnescapeDataString(tenUser) : "");

            var list_user_digitalSign = (from user in _context.DmUsers
                                         join donvi in _context.DmDonvis on user.MaDonvi equals donvi.MaDonvi
                                         
                                         where (
                                            user.IsActive == true && // Chỉ lấy các user đang được kích hoạt.  
                                            (tenUser != null ? user.TenUser.Contains(decodedTenUser) : true) &&
                                            (maUser != null ? user.MaUser.Contains(maUser) : true) &&
                                            (dienthoai != null ? user.Dienthoai.Contains(dienthoai) : true)
                                         )

                                         select new DmUser
                                         {
                                             Oid = user.Oid,
                                             MaUser = user.MaUser,
                                             TenUser = user.TenUser,
                                             Dienthoai = user.Dienthoai,
                                             Mail = user.Mail,
                                             MaDonvi = donvi.TenDonvi,
                                             MaUserPias = user.MaUserPias,
                                         }

                      ).Skip(limit * (pageNumber - 1)).Take(limit).Distinct().AsQueryable(); // Chọn Distinct, loại các user trùng nhau.
            return ToListWithNoLockAsync(list_user_digitalSign);
        }

        // Function phụ, giúp lấy mã ký số trong bảng ký số từ mã user id.
        public string getDigitalSignIdFromUserId(string userId)
        {
            var query = (from kyso in _context.DmPquyenKyhs
                         join user in _context.DmUsers on kyso.MaUser equals user.Oid.ToString()
                         where user.MaUser == userId
                         select new DmPquyenKyhs
                         {
                             MaUser = user.Oid.ToString() // Hiện trong bảng ký số, mã User đang được gắn với trường Oid của bảng dm_user.
                         }).ToList();
            if (query.Count > 0)
            {
                return query.ElementAt(0).MaUser;
            } else
            {
                return "";
            }
        }


        // Lấy danh sách tên sản phẩm 
        public Task<List<DanhMuc>> getProductList(string? maSp, string? tenSp)
        {
            string decodedTenSp = (tenSp != null ? Uri.UnescapeDataString(tenSp) : "");


            var list_sp = (from sp in _context_pias.DmSps
                           where (
                           (sp.MaNsp1 == "0501" || sp.MaNsp1 == "0502") && // Các loại bảo hiểm xe cơ giới 
                           (maSp != null ? sp.MaSp.Contains(maSp) : true) &&
                           (tenSp != null ? sp.TenSp.Contains(decodedTenSp) : true))

                           select new DanhMuc
                           {
                               MaDM = sp.MaSp,
                               TenDM = sp.TenSp
                           }
                       ).AsQueryable();
            return ToListWithNoLockAsync(list_sp);
        }

        // Thêm user ký số:
        public async Task<string> createDigitalSign(DmPquyenKyhs ky_so, string currentUserEmail)
        {
            try
            {
                //string digitalSignId = getDigitalSignIdFromUserId(ky_so.MaUser); // Lấy mã ký trong bảng phân quyền ký hồ sơ từ mã user. Kiểm tra nếu quyền ký số không tồn tại thì mới tiến hành insert
                DmUser currentUser = _context.DmUsers.Where(x => x.Mail == currentUserEmail).FirstOrDefault();
                if (currentUser != null)
                {
                    DmPquyenKyhs checkDigitalSignExist = FirstOrDefaultWithNoLock(_context.DmPquyenKyhs.Where(x => x.MaUser == ky_so.MaUser).AsQueryable());
                    if (checkDigitalSignExist == null) // Nếu quyền ký số chưa tồn tại: 
                    {
                        // Ngoài ra, User được ký số phải tồn tại trong hệ thống.
                        var check_User_Exist = FirstOrDefaultWithNoLock(_context.DmUsers.Where(x => x.Oid.ToString() == ky_so.MaUser).AsQueryable());
                        if (check_User_Exist != null)
                        {
                            try
                            {
                                ky_so.PrKey = Guid.NewGuid();
                                ky_so.MaUser = check_User_Exist.Oid.ToString();
                                ky_so.NgayCnhat = DateTime.Now;
                                ky_so.MaUserCapnhat = currentUser.MaUser;

                                await _context.DmPquyenKyhs.AddAsync(ky_so);
                                await _context.SaveChangesAsync();

                                return ky_so.MaUser;

                            }
                            catch (Exception ex)
                            {
                                _logger.Error("dbContextTransaction Exception when creaingDigitalSign: " + ex.ToString());
                                _logger.Error("Error record: " + JsonConvert.SerializeObject(ky_so));
                                await _context.DisposeAsync();
                                throw;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                } else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
            }
            return null!;
        }

        
        // Update quyền ký hồ sơ
        public string updateDigitalSign(DmPquyenKyhs ky_so, string currentUserEmail)
        {
            try
            {             
                DmUser currentUser = _context.DmUsers.Where(x => x.Mail == currentUserEmail).FirstOrDefault();
                if (currentUser != null)
                {
                    ky_so.NgayCnhat = DateTime.Now;
                    ky_so.MaUserCapnhat = currentUser.MaUser;
                    _context.DmPquyenKyhs.Update(ky_so);
                    _context.SaveChanges();
                    return ky_so.PrKey.ToString();
                } else
                {
                    return "ERROR - CURRENT USER NOT FOUND";
                }
            }
            catch (Exception ex)
            {
                _logger.Error("dbContextTransaction Exception when updateStation: " + ex.ToString());
                _logger.Error("Error record: " + JsonConvert.SerializeObject(ky_so));
                _context.Dispose();
                throw;
            }
        }
        
    }
}