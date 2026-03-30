using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Office.Interop.Word;
using Newtonsoft.Json;
using PVI.DAO.Entities.Models;
using PVI.Helper;
using PVI.IAM.BE.Services;
using PVI.Repository.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace PVI.Repository.Repositories
{
    public class DanhSachUser
    {
        public int Count { get; set; } = 0;
        public List<DmUser> Data { get; set; } = new List<DmUser>();
    }
    public class DmUserRepository : GenericRepository<DmUser>, IDmUserRepository
    {
        JwtGenerator jwtGenerator = null;
        public DmUserRepository(GdttContext context, Pvs2024Context context_pias, Serilog.ILogger logger, IConfiguration conf) : base(context, context_pias, logger, conf)
        {
            jwtGenerator = new JwtGenerator(_configuration, context);
        }
        public string getDonViById(string ma_donvi)
        {
            string objResult = "";
            try
            {
                objResult = _context.DmDonvis.Where(x => x.MaDonvi == ma_donvi).Select(s => s.MaDvchuquan).FirstOrDefault() ?? "";
            }
            catch (Exception ex)
            {
            }
            return objResult;
        }
        public List<DmLoaiUser> getDMLoaiUser()
        {
            List<DmLoaiUser> objResult = new List<DmLoaiUser>();
            try
            {
                objResult = _context.DmLoaiUsers.ToList();
            }
            catch (Exception ex)
            {
            }
            return objResult;
        }

        public List<DmDonvi> getDMDonvi(string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
            if (currentUser != null)
            {
                List<DmDonvi> objResult = new List<DmDonvi>();
                if (currentUser.MaDonvi.Equals("00") || currentUser.MaDonvi.Equals("31") || currentUser.MaDonvi.Equals("32"))
                {
                    objResult = _context.DmDonvis.ToList();
                }
                else
                {
                    objResult = _context.DmDonvis.Where(x => x.MaDonvi == currentUser.MaDonvi).ToList();
                }
                return objResult;
            }
            else
            {
                return new List<DmDonvi>();
            }
        }

        // khanhlh - 24/02/2025:
        // KiemTraPhanQuyen sử dụng để lấy danh sách các user mà cán bộ này có thể xem / update được.
        // Tham số: currentUser: Chỉ user hiện tại; và whichCase: Dùng trong trường hợp nào, "Xem" hay sửa
        public string checkUserCase(DmUser currentUser, string whichCase)
        {
            //
            if (whichCase.Equals("getUser"))
            {
                if (currentUser.MaDonvi.Contains("00")) // 
                {
                    if (currentUser.LoaiUser == 6)
                    {
                        return "ban_quan_ly"; // Nếu là ban quản lý
                    }
                    else
                    {
                        return "nv_tru_so"; // Các case còn lại
                    }
                }
                else if (currentUser.MaDonvi.Contains("31") || currentUser.MaDonvi.Contains("32"))
                {
                    if (currentUser.LoaiUser == 9 || currentUser.LoaiUser == 11)
                    {
                        return "truong_phong_tt";
                    }
                    else if (currentUser.LoaiUser == 10)
                    {
                        return "lanh_dao_tt";
                    }
                    else
                    {
                        return "can_bo_tt";
                    }
                }
                else if (currentUser.LoaiUser == 2 && currentUser.LoaiUser == 3)
                {
                    return "TRUONG_PHONG_DV"; // -1 là mã lỗi.
                }
                else
                {
                    return "NOT_AUTHORIZED";
                }


                //
            }
            else if (whichCase.Equals("createUser"))
            {
                if (currentUser.LoaiUser == 1) // Quản trị hệ thống.
                {
                    return "quan_tri_he_thong";
                }
                else if (currentUser.LoaiUser == 2 || currentUser.LoaiUser == 3)
                {
                    return "quan_tri_dv";
                }
                else if (currentUser.LoaiUser == 6)
                {
                    return "ban_quan_ly";
                }
                else if (currentUser.LoaiUser == 9 || currentUser.LoaiUser == 10 || currentUser.LoaiUser == 11)
                {
                    return "lanh_dao_tt";
                }
                else
                {
                    return "LOW_AUTHORITY";
                }

            }
            else
            {
                return "NOT_AUTHORIZED";
            }


        }

        // Lấy danh sách tất cả các user GDTT
        // Chwck theo thông tin của user hiện tại.

        public async Task<PagedList<DmUser>> getListUserGDTT(int pageNumber, int pageSize, string currentUserEmail)
        {
            // Hard-code, về sau sẽ đổi qua authentication xịn của người dùng  
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();

            if (currentUser != null)
            {

                string evaluatePhanQuyen = checkUserCase(currentUser, "getUser");

                var list_user = (from user in _context.DmUsers
                                 join donvi in _context.DmDonvis on user.MaDonvi equals donvi.MaDonvi
                                 join loaiUser in _context.DmLoaiUsers on user.LoaiUser equals loaiUser.LoaiUser

                                 where (
                                    (evaluatePhanQuyen != "NOT_AUTHORIZED" ? true : false) && // Nếu không được phân quyền thì không trả gì cả.
                                    (evaluatePhanQuyen == "ban_quan_ly" ? (user.LoaiUser != 0 && user.LoaiUser != 1) : true) &&
                                    (evaluatePhanQuyen == "nv_tru_so" ? (user.LoaiUser != 0) : true) &&
                                    (evaluatePhanQuyen == "lanh_dao_tt" ? (user.LoaiUser != 0 && user.LoaiUser != 1 && user.LoaiUser != 6 && user.MaDonvi.Equals(currentUser.MaDonvi)) : true) &&
                                    (evaluatePhanQuyen == "can_bo_tt" ? (user.LoaiUser != 0 && user.LoaiUser != 1 && user.MaUser.Equals(currentUser.MaUser)) : true)
                                 )
                                 select new DmUser
                                 {
                                     Oid = user.Oid,
                                     MaUser = user.MaUser,
                                     TenUser = user.TenUser,
                                     Dienthoai = user.Dienthoai,
                                     Mail = user.Mail,
                                     MaDonvi = user.MaDonvi,
                                     TenDonvi = donvi.TenDonvi,
                                     MaUserPias = user.MaUserPias,
                                     PhanQuyen = user.PhanQuyen,
                                     MaDonviPquyen = user.MaDonviPquyen,
                                     LoaiUser = user.LoaiUser,
                                     LoaiCbo = user.LoaiCbo,
                                     
                                     IsActive = user.IsActive,
                                     IsGdvHotro = user.IsGdvHotro,
                                     IsActiveGddk = user.IsActiveGddk,
                                     IsActiveGqkn = user.IsActiveGqkn,
                                     PquyenUplHinhAnh = user.PquyenUplHinhAnh,
                                     IsActiveKytt = user.IsActiveKytt,
                                     IsactiveChkc = user.IsactiveChkc,
                                     NgayCnhat = user.NgayCnhat,
                                 }
                            ).AsQueryable();

                return await PagedList<DmUser>.ToPagedListAsync(list_user, pageNumber, pageSize);
            }
            else
            {
                return new PagedList<DmUser>(new List<DmUser>(), 0, 1, 1);
            }
        }

        // Lấy danh sách các user GDDK

        public async Task<PagedList<DmUser>> getListUserGDDK(int pageNumber, int pageSize, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();

            if (currentUser != null)
            {
                string evaluatePhanQuyen = checkUserCase(currentUser, "getUser");
                var list_user = (from user in _context.DmUsers
                                 join donvi in _context.DmDonvis on user.MaDonvi equals donvi.MaDonvi
                                 join loaiUser in _context.DmLoaiUsers on user.LoaiUser equals loaiUser.LoaiUser
                                 where (
                                 // LƯU Ý: ĐÂY LÀ 1 ĐIỀU KIỆN, CHIA 2 NHÁNH !
                                    ((evaluatePhanQuyen == "ban_quan_ly" || evaluatePhanQuyen == "nv_tru_so") ? (user.LoaiUser != 1 && (user.LoaiCbo.Equals("CB") || user.LoaiCbo.Equals("DL"))) :
                                    (user.PhanQuyen.Value ? (user.LoaiUser != 1 && (user.LoaiCbo.Equals("CB") || user.LoaiCbo.Equals("DL")) && user.MaDonvi.Equals(currentUser.MaDonvi)) : (user.LoaiUser != 1  && (user.LoaiCbo.Equals("CB") || user.LoaiCbo.Equals("DL")) && user.MaUser.Equals(currentUser.MaUser))))
                                 )
                                 select new DmUser
                                 {
                                     Oid = user.Oid,
                                     MaUser = user.MaUser,
                                     TenUser = user.TenUser,
                                     Dienthoai = user.Dienthoai,
                                     Mail = user.Mail,
                                     MaDonvi = user.MaDonvi,
                                     TenDonvi = donvi.TenDonvi,
                                     MaUserPias = user.MaUserPias,
                                     PhanQuyen = user.PhanQuyen,
                                     MaDonviPquyen = user.MaDonviPquyen,
                                     LoaiUser = user.LoaiUser,
                                     LoaiCbo = user.LoaiCbo,
                                     
                                     IsActive = user.IsActive,
                                     IsGdvHotro = user.IsGdvHotro,
                                     IsActiveGddk = user.IsActiveGddk,
                                     IsActiveGqkn = user.IsActiveGqkn,
                                     PquyenUplHinhAnh = user.PquyenUplHinhAnh,
                                     IsActiveKytt = user.IsActiveKytt,
                                     IsactiveChkc = user.IsactiveChkc,
                                     NgayCnhat = user.NgayCnhat

                                 }
                     ).AsQueryable();

                return await PagedList<DmUser>.ToPagedListAsync(list_user, pageNumber, pageSize);
            }
            else
            {
                return new PagedList<DmUser>(new List<DmUser>(), 0, 1, 1);
            }
        }

        // Lay danh sach cac User
        // Tham số: Page & Limit - Dùng để phân trang, có kèm theo các filter.
        public async Task<DanhSachUser> searchFilterUserGDTT(int pageNumber, int limit, DmUser searchTarget, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();

            if (currentUser != null)
            {
                // Các user phân quyền cao:
                string allowedUserTypes = "1; 2; 3; 6; 9; 10; 11";

                //string evaluatePhanQuyen = checkUserCase(currentUser, "getUser");
                var list_user = (from user in _context.DmUsers
                                 join donvi in _context.DmDonvis on user.MaDonvi equals donvi.MaDonvi
                                 join loaiUser in _context.DmLoaiUsers on user.LoaiUser equals loaiUser.LoaiUser
                                 where (

                                 // Bước 1: Phân Quyền User
                                 // Nếu user không phải là 1 trong các cán bộ có phân quyền cao thì chỉ thấy được chính bản thân.
                                 (!allowedUserTypes.Contains(currentUser.LoaiUser.ToString()) ? user.MaUser.Equals(currentUser.MaUser) :
                                 (currentUser.LoaiUser == 1 ? true : (
                                 ((currentUser.LoaiUser == 2 || currentUser.LoaiUser == 3) ? (user.MaDonvi.Equals(currentUser.MaDonvi) && user.MaDonvi != "00" && user.MaDonvi != "31" && user.MaDonvi != "32") : true) && // Trưởng phòng đơn vị
                                 ((currentUser.LoaiUser == 6) ? (user.LoaiUser != 1) : true) && // Ban quản lý
                                 ((currentUser.LoaiUser == 10) ? ( user.LoaiUser != 1 && user.LoaiUser != 6) : true) && // Lãnh đạo trung tâm
                                 ((currentUser.LoaiUser == 9 || currentUser.LoaiUser == 11) ? (user.LoaiUser != 1 && user.LoaiUser != 6 && user.LoaiUser != 10 && user.LoaiUser < 12) : true)
                                 )
                                 )
                                 ) &&

                                 // Bước 2: Bộ lọc dữ liệu
                                 (searchTarget.MaUser != null ? user.MaUser.Contains(searchTarget.MaUser) : true) &&
                                 (searchTarget.TenUser != null ? user.TenUser.Contains(searchTarget.TenUser) : true) &&
                                 (searchTarget.Dienthoai != null ? user.Dienthoai.Contains(searchTarget.Dienthoai) : true) &&
                                 (searchTarget.Mail != null ? user.Mail.Contains(searchTarget.Mail) : true) &&
                                 (searchTarget.TenDonvi != null ? donvi.TenDonvi.Contains(searchTarget.TenDonvi) : true) &&
                                 (searchTarget.MaUserPias != null ? user.MaUserPias.Contains(searchTarget.MaUserPias) : true) &&
                                 (searchTarget.LoaiUser != null ? user.LoaiUser == searchTarget.LoaiUser : true) &&
                                 (searchTarget.LoaiCbo != null ? user.LoaiCbo == searchTarget.LoaiCbo : true) &&
                                 (searchTarget.IsActive != null ? user.IsActive == searchTarget.IsActive : true) &&
                                 (searchTarget.IsGdvHotro != null ? user.IsGdvHotro == searchTarget.IsGdvHotro : true) &&
                                 (searchTarget.IsActiveGddk != null ? user.IsActiveGddk == searchTarget.IsActiveGddk : true) &&
                                 (searchTarget.IsActiveGqkn != null ? user.IsActiveGqkn == searchTarget.IsActiveGqkn : true) &&
                                 (searchTarget.PquyenUplHinhAnh != null ? user.PquyenUplHinhAnh == searchTarget.PquyenUplHinhAnh : true) &&
                                 (searchTarget.PhanQuyen != null ? user.PhanQuyen == searchTarget.PhanQuyen : true) &&
                                 (searchTarget.IsActiveKytt != null ? user.IsActiveKytt == searchTarget.IsActiveKytt : true) &&
                                 (searchTarget.IsactiveChkc != null ? user.IsactiveChkc == searchTarget.IsactiveChkc : true)
                                 )
                                 select new DmUser
                                 {
                                     Oid = user.Oid,
                                     MaUser = user.MaUser,
                                     TenUser = user.TenUser,
                                     Dienthoai = user.Dienthoai,
                                     Mail = user.Mail,
                                     MaDonvi = user.MaDonvi,
                                     TenDonvi = donvi.TenDonvi,
                                     MaUserPias = user.MaUserPias,
                                     PhanQuyen = user.PhanQuyen,
                                     MaDonviPquyen = user.MaDonviPquyen,
                                     LoaiUser = user.LoaiUser,
                                     LoaiCbo = user.LoaiCbo,
                                     
                                     IsActive = user.IsActive,
                                     IsGdvHotro = user.IsGdvHotro,
                                     IsActiveGddk = user.IsActiveGddk,
                                     IsActiveGqkn = user.IsActiveGqkn,
                                     PquyenUplHinhAnh = user.PquyenUplHinhAnh,
                                     IsActiveKytt = user.IsActiveKytt,
                                     IsactiveChkc = user.IsactiveChkc,
                                     NgayCnhat = user.NgayCnhat,
                                 }
                      ).AsQueryable();
                return new DanhSachUser
                {
                    Count = list_user.Count(),
                    Data = await list_user.Skip(limit * (pageNumber - 1)).Take(limit).ToListAsync(),
                };
            }
            else
            {
                return new DanhSachUser();
            }
        }

        // Lay danh sach cac User
        // Tham số: Page & Limit - Dùng để phân trang, có kèm theo các filter.
        public async Task<DanhSachUser> searchFilterUserGDDK(int pageNumber, int limit, DmUser searchTarget, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();

            if (currentUser != null)
            {
                //string evaluatePhanQuyen = checkUserCase(currentUser, "getUser");
                var list_user = (from user in _context.DmUsers
                                 join donvi in _context.DmDonvis on user.MaDonvi equals donvi.MaDonvi
                                 join loaiUser in _context.DmLoaiUsers on user.LoaiUser equals loaiUser.LoaiUser

                                 // Bước 1: Phân quyền tài khoản:
                                 where ((currentUser.LoaiUser == 1 ? true : (
                                   ((currentUser.MaDonvi == "00" || currentUser.MaDonvi == "31" || currentUser.MaDonvi == "32") ? (user.LoaiUser != 1 && (user.LoaiCbo.Equals("CB") || user.LoaiCbo.Equals("DL"))) :
                                  (currentUser.PhanQuyen.Value ? (user.LoaiUser != 1 && (user.LoaiCbo.Equals("CB") || user.LoaiCbo.Equals("DL")) && user.MaDonvi.Equals(currentUser.MaDonvi)) : (user.LoaiUser != 1 && (user.LoaiCbo.Equals("CB") || user.LoaiCbo.Equals("DL")) && user.MaUser.Equals(currentUser.MaUser)))))) &&

                                 // Bước 2: Bộ Lọc dữ liệu:
                                 (searchTarget.MaUser != null ? user.MaUser.Contains(searchTarget.MaUser) : true) &&
                                 (searchTarget.TenUser != null ? user.TenUser.Contains(searchTarget.TenUser) : true) &&
                                 (searchTarget.Dienthoai != null ? user.Dienthoai.Contains(searchTarget.Dienthoai) : true) &&
                                 (searchTarget.Mail != null ? user.Mail.Contains(searchTarget.Mail) : true) &&
                                 (searchTarget.TenDonvi != null ? donvi.TenDonvi.Contains(searchTarget.TenDonvi) : true) &&
                                 (searchTarget.MaUserPias != null ? user.MaUserPias.Contains(searchTarget.MaUserPias) : true) &&
                                 (searchTarget.LoaiUser != null ? user.LoaiUser == searchTarget.LoaiUser : true) &&
                                 (searchTarget.LoaiCbo != null ? user.LoaiCbo == searchTarget.LoaiCbo : true) &&
                                 (searchTarget.IsActive != null ? user.IsActive == searchTarget.IsActive : true) &&
                                 (searchTarget.IsGdvHotro != null ? user.IsGdvHotro == searchTarget.IsGdvHotro : true) &&
                                 (searchTarget.IsActiveGddk != null ? user.IsActiveGddk == searchTarget.IsActiveGddk : true) &&
                                 (searchTarget.IsActiveGqkn != null ? user.IsActiveGqkn == searchTarget.IsActiveGqkn : true) &&
                                 (searchTarget.PquyenUplHinhAnh != null ? user.PquyenUplHinhAnh == searchTarget.PquyenUplHinhAnh : true) &&
                                 (searchTarget.PhanQuyen != null ? user.PhanQuyen == searchTarget.PhanQuyen : true) &&
                                 (searchTarget.IsActiveKytt != null ? user.IsActiveKytt == searchTarget.IsActiveKytt : true) &&
                                 (searchTarget.IsactiveChkc != null ? user.IsactiveChkc == searchTarget.IsactiveChkc : true))


                                 select new DmUser
                                 {
                                     Oid = user.Oid,
                                     MaUser = user.MaUser,
                                     TenUser = user.TenUser,
                                     Dienthoai = user.Dienthoai,
                                     Mail = user.Mail,
                                     MaDonvi = user.MaDonvi,
                                     TenDonvi = donvi.TenDonvi,
                                     MaUserPias = user.MaUserPias,
                                     PhanQuyen = user.PhanQuyen,
                                     MaDonviPquyen = user.MaDonviPquyen,
                                     LoaiUser = user.LoaiUser,
                                     LoaiCbo = user.LoaiCbo,
                                     
                                     IsActive = user.IsActive,
                                     IsGdvHotro = user.IsGdvHotro,
                                     IsActiveGddk = user.IsActiveGddk,
                                     IsActiveGqkn = user.IsActiveGqkn,
                                     PquyenUplHinhAnh = user.PquyenUplHinhAnh,
                                     IsActiveKytt = user.IsActiveKytt,
                                     IsactiveChkc = user.IsactiveChkc,
                                     NgayCnhat = user.NgayCnhat

                                 }
                      ).AsQueryable();
                return new DanhSachUser
                {
                    Count = list_user.Count(),
                    Data = await list_user.Skip(limit * (pageNumber - 1)).Take(limit).ToListAsync()
                };
            }
            else
            {
                return new DanhSachUser();
            }
        }

        // Dựa vào email, lấy thông tin của user từ PIAS.
        // Trong các biến, có thêm biến currentUser, tùy phân quyền để lấy.

        public DmUser getUserPiasFromEmail(DmUser currentUser, string userEmail)
        {
            try
            {
                // Chỉ lấy user dang đuọc kích hoạt
                DmUserPias userPias = _context_pias.DmUserPiases.Where(x => x.DcEmail.Equals(userEmail) && x.TrangThai == true).FirstOrDefault();
                if (userPias != null)
                {
                    DmUser toBeReturned = new DmUser
                    {
                        TenUser = userPias.TenUser,
                        MaUser = userPias.DcEmail.Split()[0],
                        Mail = userPias.DcEmail,
                        MaDonvi = userPias.MaDonvi,
                        TenDonvi = _context.DmDonvis.Where(x => x.MaDonvi.Equals(userPias.MaDonvi)).FirstOrDefault().TenDonvi,
                        MaUserPias = userPias.MaCbo,
                        IsActive = userPias.TrangThai,

                    };

                    return toBeReturned;
                }
                else
                {
                    return new DmUser();
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
                return new DmUser();
            }
        }

        // Sử dụng cho drop down lấy user. Hệ thống sẽ căn cứ vào mã user của người dùng để lấy tất cả các user từ đơn vị đó.

        public List<DmUser> getListUserPiasFromDonvi(DmUser currentUser, int pageNumber, int pageSize)
        {
            try
            {
                List<DmUserPias> userPiases = new List<DmUserPias>();
                int count = 0; // Đếm tổng số user có thể quét.

                if (currentUser.MaDonvi.Equals("00") || currentUser.MaDonvi.Equals("31") || currentUser.MaDonvi.Equals("32"))
                {
                    userPiases = _context_pias.DmUserPiases.Where(x => x.MaDonvi != "" && x.TrangThai == true).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                    count = _context_pias.DmUserPiases.Where(x => x.MaDonvi != "" && x.TrangThai == true).Count();
                }
                else
                {
                    userPiases = _context_pias.DmUserPiases.Where(x => x.MaDonvi.Equals(currentUser.MaDonvi) && x.TrangThai == true).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                    count = _context_pias.DmUserPiases.Where(x => x.MaDonvi.Equals(currentUser.MaDonvi) && x.TrangThai == true).Count();
                }

                List<DmUser> toBeReturned = new List<DmUser>();
                if (userPiases != null && userPiases.Count > 0)
                {
                    userPiases.ForEach(userPias =>
                    {
                        DmUser userData = new DmUser
                        {
                            TenUser = userPias.TenUser,
                            MaUser = userPias.DcEmail.Split()[0],
                            Mail = userPias.DcEmail,
                            MaDonvi = userPias.MaDonvi,
                            TenDonvi = _context.DmDonvis.Where(x => x.MaDonvi.Equals(userPias.MaDonvi)).FirstOrDefault().TenDonvi,
                            MaUserPias = userPias.MaCbo,
                            IsActive = userPias.TrangThai,
                        };

                        toBeReturned.Add(userData);
                    }
                    );

                    return toBeReturned;
                }
                else
                {
                    return new List<DmUser>();
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
                return new List<DmUser>();
            }
        }



        // Tạo user mới
        public async Task<string> createUser(DmUser user, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
            if (currentUser != null)
            {
                int[] allowedUsers = new int[] { 1, 2, 3, 6, 9, 10, 11 }; // Các loại user được phép thực hiện hành động này

                // Validate bằng tay do mỗi loại user lại có 1 phân quyền khác nhau.
                if (Array.Exists(allowedUsers, x => x == currentUser.LoaiUser))
                {
                    // Trưởng phòng đơn vị
                    if ((currentUser.LoaiUser == 2 || currentUser.LoaiUser == 3) && (user.LoaiUser == 1 || user.LoaiUser == 6 || user.LoaiUser == 8 || user.LoaiUser == 9 || user.LoaiUser == 10 || user.LoaiUser == 11 || user.LoaiUser == 12))
                    {
                        return "Bạn không thể tạo tài khoản trên phân cấp !";
                    }
                    // Lãnh đạo văn phòng
                    else if (currentUser.LoaiUser == 10 && (user.LoaiUser == 1 || user.LoaiUser == 6))
                    {
                        return "Lãnh đạo văn phòng không thể tạo tài khoản Quản trị hệ thống hoặc Ban Quản Lý !";
                    }
                    else if ((currentUser.LoaiUser == 9) && (user.LoaiUser == 10 || user.LoaiUser == 1 || user.LoaiUser == 6))
                    {
                        return "Bạn không thể tạo tài khoản trên phân cấp !";
                    }
                    else if ((currentUser.LoaiUser == 11) && (user.LoaiUser == 10 || user.LoaiUser == 1 || user.LoaiUser == 6 || currentUser.LoaiUser == 9))
                    {
                        return "Bạn không thể tạo tài khoản trên phân cấp !";
                    }
                    else if (currentUser.LoaiUser != 1 && user.LoaiUser == 1)
                    {
                        return "Bạn không thể tạo tài khoản trên phân cấp !";
                    }
                    else
                    {
                        // Kiểm tra nếu user không tồn tại thì mới tiến hành insert
                        var checkExist = await _context.DmUsers.FirstOrDefaultAsync(x => x.MaUser == user.MaUser);
                        if (checkExist == null)
                        {
                            // Kiểm tra loại cán bộ phải chính xác.
                            if (user.MaDonvi != null)
                            {
                                var donvi = await _context.DmDonvis.FirstOrDefaultAsync(x => x.MaDonvi == user.MaDonvi);
                                if (donvi == null)
                                {
                                    return $"Mã đơn vị {user.MaDonvi} không tồn tại.";
                                }
                                else
                                {
                                    try
                                    {

                                        DmUser toBeCreated = new DmUser
                                        {
                                            Oid = Guid.NewGuid(),
                                        };

                                        toBeCreated.Mail = !String.IsNullOrEmpty(user.Mail) ? user.Mail : "";
                                        toBeCreated.Dienthoai = user.Dienthoai ?? "";
                                        toBeCreated.MaUser = user.MaUser??"";
                                        toBeCreated.TenUser = user.TenUser??"";
                                        toBeCreated.LoaiUser = user.LoaiUser;
                                        toBeCreated.LoaiCbo = user.LoaiCbo??"";
                                        toBeCreated.MaDonvi = user.MaDonvi??"";
                                        toBeCreated.IsActive = true;
                                        toBeCreated.MaUserPias = user.MaUserPias??"";
                                        toBeCreated.MaDonviPquyen = user.MaDonviPquyen??"";
                                        toBeCreated.PhanQuyen = user.PhanQuyen ?? false;
                                        toBeCreated.IsGdvHotro = user.IsGdvHotro ?? false;
                                        toBeCreated.PquyenUplHinhAnh = user.PquyenUplHinhAnh ?? false;
                                        toBeCreated.IsActiveGddk = user.IsActiveGddk ?? false;
                                        toBeCreated.IsActiveGqkn = user.IsActiveGqkn ?? false;
                                        toBeCreated.IsActiveKytt = user.IsActiveKytt ?? false;
                                        toBeCreated.IsactiveChkc = user.IsactiveChkc ?? false;


                                        //Nếu user hiện tại không phải từ cơ quan trung tâm thì mặc định mã đơn vị giống với đơn vị của user.
                                        //if (!currentUser.MaDonvi.Equals("00") && !currentUser.MaDonvi.Equals("31") && !currentUser.MaDonvi.Equals("32"))
                                        //{
                                        //    user.MaDonvi = currentUser.MaDonvi;
                                        //}

                                        toBeCreated.NgayCnhat = DateTime.Now; // Chỉnh ngày cập nhật.
                                        toBeCreated.MaUserCapnhat = currentUser.MaUser;


                                        _context.DmUsers.Add(toBeCreated);
                                        _context.SaveChanges();
                                        return user.MaUser.ToString();
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.Error("dbContextTransaction Exception when CreateHsgdTtrinh: " + ex.ToString());
                                        _logger.Error("Error record: " + JsonConvert.SerializeObject(user));
                                        _context.Dispose();
                                        throw;
                                    }
                                }
                            }
                            else
                            {
                                return "Mã đơn vị không được để trống.";
                            }
                        }

                        else
                        {
                            return $"User {checkExist.MaUser} đã tồn tại";
                        }
                    }
                }
                else
                {
                    return "Bạn không được phân quyền thực hiện hành động này";
                }
            }
            else
            {
                return $"Mã user hiện tại không chính xác.";
            }
        }

        // Update User
        public async Task<string> UpdateUser(DmUser user, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();

            // Update thông tin
            DmUser toBeUpdated = _context.DmUsers.Where(x => x.MaUser == user.MaUser).FirstOrDefault();

            if (currentUser != null)
            {
                // Validate phân quyền của user hiện tại.

                int[] allowedUsers = new int[] { 1, 2, 3, 6, 9, 10, 11 }; // Các loại user được phép thực hiện hành động này

                // Validate bằng tay do mỗi loại user lại có 1 phân quyền khác nhau.
                if (Array.Exists(allowedUsers, x => x == currentUser.LoaiUser))
                {
                    // Trưởng phòng đơn vị
                    if ((currentUser.LoaiUser == 2 || currentUser.LoaiUser == 3) && (user.LoaiUser == 1 || user.LoaiUser == 6 || user.LoaiUser == 8 || user.LoaiUser == 9 || user.LoaiUser == 10 || user.LoaiUser == 11 || user.LoaiUser == 12))
                    {
                        return "Bạn không thể cập nhật tài khoản trên phân cấp !";
                    }
                    // Lãnh đạo văn phòng
                    else if (currentUser.LoaiUser == 10 && (user.LoaiUser == 1 || user.LoaiUser == 6))
                    {
                        return "Lãnh đạo văn phòng không thể cập nhật tài khoản Quản trị hệ thống hoặc Ban Quản Lý !";
                    }
                    else if ((currentUser.LoaiUser == 9) && (user.LoaiUser == 10 || user.LoaiUser == 1 || user.LoaiUser == 6))
                    {
                        return "Bạn không thể cập nhật tài khoản trên phân cấp !";
                    }
                    else if ((currentUser.LoaiUser == 11) && (user.LoaiUser == 10 || user.LoaiUser == 1 || user.LoaiUser == 6 || currentUser.LoaiUser == 9))
                    {
                        return "Bạn không thể cập nhật tài khoản trên phân cấp !";
                    }
                    else if (currentUser.LoaiUser != 1 && user.LoaiUser == 1)
                    {
                        return "Bạn không thể cập nhật tài khoản trên phân cấp !";
                    }
                    else
                    {
                        // Đảm bảo mã đơn vị phải được nhập
                        if (String.IsNullOrEmpty(user.MaDonvi) || _context.DmDonvis.FirstOrDefault(x => x.MaDonvi == user.MaDonvi) == null)
                        {
                            return $"Mã đơn vị {user.MaDonvi} không tồn tại.";
                        }
                        // Tiến hành cập nhật
                        else
                        {
                            try
                            {
                                toBeUpdated.Mail = !String.IsNullOrEmpty(user.Mail) ? user.Mail : "";
                                toBeUpdated.Dienthoai = user.Dienthoai??"";
                                toBeUpdated.TenUser = user.TenUser??"";
                                toBeUpdated.LoaiUser = user.LoaiUser;
                                toBeUpdated.LoaiCbo = user.LoaiCbo??"";
                                toBeUpdated.MaDonvi = user.MaDonvi??"";
                                toBeUpdated.IsActive = user.IsActive != null ? user.IsActive : false;
                                toBeUpdated.MaUserPias = user.MaUserPias??"";
                                toBeUpdated.MaDonviPquyen = user.MaDonviPquyen??"";
                                toBeUpdated.PhanQuyen = user.PhanQuyen ?? false;
                                toBeUpdated.IsGdvHotro = user.IsGdvHotro ?? false;
                                toBeUpdated.PquyenUplHinhAnh = user.PquyenUplHinhAnh ?? false;
                                toBeUpdated.IsActiveGddk = user.IsActiveGddk ?? false;
                                toBeUpdated.IsActiveGqkn = user.IsActiveGqkn ?? false;
                                toBeUpdated.IsActiveKytt = user.IsActiveKytt ?? false;
                                toBeUpdated.IsactiveChkc = user.IsactiveChkc ?? false;


                                //Nếu user hiện tại không phải từ cơ quan trung tâm thì mặc định mã đơn vị giống với đơn vị của user.
                                //if (!currentUser.MaDonvi.Equals("00") && !currentUser.MaDonvi.Equals("31") && !currentUser.MaDonvi.Equals("32"))
                                //{
                                //    user.MaDonvi = currentUser.MaDonvi;
                                //}

                                toBeUpdated.NgayCnhat = DateTime.Now; // Chỉnh ngày cập nhật.
                                toBeUpdated.MaUserCapnhat = currentUser.MaUser;

                                _context.DmUsers.Update(toBeUpdated);
                                _context.SaveChanges();
                                return toBeUpdated.MaUser.ToString();
                            }

                            catch (Exception ex)
                            {
                                _logger.Error("EXCEPTION_UPDATEUSER: " + ex.ToString());
                                _logger.Error("Error record: " + JsonConvert.SerializeObject(user));
                                _context.Dispose();
                                return "Lỗi xảy ra khi cập nhật user. Mã Lỗi: EXCEPTION_UPDATEUSER";
                                throw;
                            }
                        }
                    }
                }
                else
                {
                    return "Bạn không có quyền chỉnh sửa tài khoản";
                }

            }
            else
            {
                return "User hiện tại không tồn tại";
            }
        }

        // Dùng để tự tạo token JWT cho tester
        public async Task<string> GenerateJWTToken(string ma_user)
        {
            try
            {
                string token = await jwtGenerator.CreateToken(ma_user);
                return "Bearer " + token;

            }
            catch (Exception err)
            {
                Console.WriteLine(err);
                return "";
            }
        }

        public List<DmUserView> GetListCanBoDuyet(string currentUserEmail)
        {
            List<DmUserView> obj_result = new List<DmUserView>();
            try
            {
                var currentUser = _context.DmUsers.Where(x => x.Mail == currentUserEmail).FirstOrDefault();
                if (currentUser != null && currentUser.MaDonvi != null)
                {
                    if (currentUser.MaDonvi.Equals("00"))
                    {
                        obj_result = (from s in _context.DmUsers
                                      where (s.IsActive == true && (s.LoaiUser == 1 || s.LoaiUser == 6 || s.LoaiUser == 9 || s.LoaiUser == 10 || s.LoaiUser == 11))
                                      orderby s.LoaiUser
                                      select (new DmUserView
                                      {
                                          Oid = s.Oid,
                                          MaUser = s.MaUser,
                                          TenUser = s.TenUser,
                                          LoaiUser = s.LoaiUser,
                                          Dienthoai = s.Dienthoai
                                      })).Distinct().ToList();
                    }
                    else if (currentUser.MaDonvi.Equals("31") || currentUser.MaDonvi.Equals("32"))
                    {
                        obj_result = (from s in _context.DmUsers
                                      where (s.MaDonvi == currentUser.MaDonvi && s.IsActive == true && (s.LoaiUser == 6 || s.LoaiUser == 9 || s.LoaiUser == 10 || s.LoaiUser == 11))
                                      orderby s.LoaiUser
                                      select (new DmUserView
                                      {
                                          Oid = s.Oid,
                                          MaUser = s.MaUser,
                                          TenUser = s.TenUser,
                                          LoaiUser = s.LoaiUser,
                                          Dienthoai = s.Dienthoai
                                      })).Distinct().ToList();
                    }
                    else
                    {
                        obj_result = (from s in _context.DmUsers
                                      where (s.MaDonvi == currentUser.MaDonvi && s.IsActive == true && (s.LoaiUser == 6 || s.LoaiUser == 9 || s.LoaiUser == 10 || s.LoaiUser == 11))
                                      orderby s.LoaiUser
                                      select (new DmUserView
                                      {
                                          Oid = s.Oid,
                                          MaUser = s.MaUser,
                                          TenUser = s.TenUser,
                                          LoaiUser = s.LoaiUser,
                                          Dienthoai = s.Dienthoai
                                      })).Distinct().ToList();
                    }
                }
                // Lấy danh sách uỷ quyền phê duyệt
                List<DmUserView> listPheDuyet = (from s in _context.DmUsers
                                                 join uq in _context.DmUqHstpcs on s.Oid.ToString().ToLower() equals uq.MaUserUq.ToLower()
                                                 where ((s.MaDonvi == "31" || s.MaDonvi == "32" || s.MaDonvi == currentUser.MaDonvi) && s.IsActive == true && (s.LoaiUser == 4 || s.LoaiUser == 8))
                                                 orderby s.LoaiUser
                                                 select (new DmUserView
                                                 {
                                                     Oid = s.Oid,
                                                     MaUser = s.MaUser,
                                                     TenUser = s.TenUser,
                                                     LoaiUser = s.LoaiUser,
                                                     Dienthoai = s.Dienthoai
                                                 })).ToList();

                obj_result.AddRange(listPheDuyet);

                if (obj_result != null)
                {
                    var dm_loai_user = _context.DmLoaiUsers.ToList();
                    obj_result = (from a in obj_result
                                  join b in dm_loai_user on a.LoaiUser equals b.LoaiUser into b1
                                  from b in b1.DefaultIfEmpty()
                                  select new DmUserView
                                  {
                                      Oid = a.Oid,
                                      MaUser = a.MaUser,
                                      TenUser = a.TenUser + (b.TenLoaiUser != "" ? (": " + b.TenLoaiUser) : ""),
                                      LoaiUser = a.LoaiUser,
                                      Dienthoai = a.Dienthoai
                                  }).OrderByDescending(x => x.LoaiUser).Distinct().ToList();
                }

            }
            catch (Exception ex)
            {
            }
            return obj_result;
        }
        public List<DmUserView> GetListDoiTruong(string currentUserEmail)
        {
            List<DmUserView> obj_result = new List<DmUserView>();
            try
            {
                var currentUser = _context.DmUsers.Where(x => x.Mail == currentUserEmail).FirstOrDefault();
                if (currentUser != null && currentUser.MaDonvi != null)
                {

                    // Lấy danh sách uỷ quyền phê duyệt
                    if (currentUser.MaDonvi != "00" && currentUser.MaDonvi != "31" && currentUser.MaDonvi != "32")
                    {
                        obj_result = (from s in _context.DmUsers
                                      join uq in _context.DmUqHstpcs on s.Oid.ToString().ToLower() equals uq.MaUserUq.ToLower()
                                      where ((s.MaDonvi == "31" || s.MaDonvi == "32" || s.MaDonvi == currentUser.MaDonvi) && s.IsActive == true && (s.LoaiUser == 4 || s.LoaiUser == 8))
                                      orderby s.LoaiUser
                                      select (new DmUserView
                                      {
                                          Oid = s.Oid,
                                          MaUser = s.MaUser,
                                          TenUser = s.TenUser,
                                          LoaiUser = s.LoaiUser,
                                          Dienthoai = s.Dienthoai
                                      })).Distinct().ToList();
                    }
                    else
                    {
                        obj_result = (from s in _context.DmUsers
                                      join uq in _context.DmUqHstpcs on s.Oid.ToString().ToLower() equals uq.MaUserUq.ToLower()
                                      where (s.IsActive == true && (s.LoaiUser == 4 || s.LoaiUser == 6 || s.LoaiUser == 8 || s.LoaiUser == 9 || s.LoaiUser == 10 || s.LoaiUser == 11))
                                      orderby s.LoaiUser
                                      select (new DmUserView
                                      {
                                          Oid = s.Oid,
                                          MaUser = s.MaUser,
                                          TenUser = s.TenUser,
                                          LoaiUser = s.LoaiUser,
                                          Dienthoai = s.Dienthoai
                                      })).Distinct().ToList();
                    }

                    if (obj_result != null)
                    {
                        var dm_loai_user = _context.DmLoaiUsers.ToList();
                        obj_result = (from a in obj_result
                                      join b in dm_loai_user on a.LoaiUser equals b.LoaiUser into b1
                                      from b in b1.DefaultIfEmpty()
                                      select new DmUserView
                                      {
                                          Oid = a.Oid,
                                          MaUser = a.MaUser,
                                          TenUser = a.TenUser + (b.TenLoaiUser != "" ? (": " + b.TenLoaiUser) : ""),
                                          LoaiUser = a.LoaiUser,
                                          Dienthoai = a.Dienthoai
                                      }).OrderByDescending(x => x.LoaiUser).ToList();
                    }
                }
            }

            catch (Exception ex)
            {
            }
            return obj_result;
        }

        public List<DmUserView> GetListCanBoGDTT(string currentUserEmail)
        {
            List<DmUserView> obj_result = new List<DmUserView>();
            try
            {
                var currentUser = _context.DmUsers.Where(x => x.Mail == currentUserEmail).FirstOrDefault();
                if (currentUser != null)
                {
                    var listLoaiUser = new List<int> { 8, 9,11 };
                    obj_result = (from s in _context.DmUsers
                                  where (s.IsActive == true && listLoaiUser.Contains(s.LoaiUser ?? -1))
                                  orderby s.TenUser
                                  select (new DmUserView
                                  {
                                      Oid = s.Oid,
                                      MaDonvi = s.MaDonvi,
                                      TenUser = s.TenUser,
                                      MaUser = s.MaUser,
                                      LoaiUser = s.LoaiUser
                                  })).ToList();
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
            }
            return obj_result;
        }
    }
}