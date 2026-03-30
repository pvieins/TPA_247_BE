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

    /* Implementation cho interface Danh mục điểm trực.
     * lhkhanh - 22/08/2024
     */

    // Kế thừa base.
    public class DiemTrucRepository : GenericRepository<DmDiemtruc>, IDiemtrucRepository
    {
        public DiemTrucRepository(GdttContext context, Pvs2024Context context_pias, Serilog.ILogger logger, IConfiguration conf) : base(context, context_pias, logger, conf)
        {

        }

        // Lay danh sach cac diem truc
        // Tham số: Page & Limit - Dùng để phân trang.
        public Task<List<DmDiemtruc>> getStationList(int pageNumber, int limit)
        {
           
            var list_diem_truc = (from diem_truc in _context.DmDiemtrucs
                                  join user in _context.DmUsers on diem_truc.MaUser equals user.MaUser
                                  orderby diem_truc.PrKey descending
                                   select new DmDiemtruc
                                  {
                                      PrKey = diem_truc.PrKey,
                                      MaDiemtruc = diem_truc.MaDiemtruc,
                                      TenDiemtruc = diem_truc.TenDiemtruc,
                                      Description = diem_truc.Description,
                                      Active = diem_truc.Active,
                                      MaUser = diem_truc.MaUser,
                                      NgayCnhat = diem_truc.NgayCnhat,
                                      Count = _context.DmDiemtrucs.Count()
                                  }
                      ).Skip(limit*(pageNumber-1)).Take(limit).AsQueryable();

            return ToListWithNoLockAsync(list_diem_truc);
        }

        //Tra cứu có Filter.
        // Đẩy body vào filter search.
        public Task<List<DmDiemtruc>> searchFilterStationList(int pageNumber, int limit, DmDiemtruc searchTarget)
        {
            var list_diem_truc = (from diem_truc in _context.DmDiemtrucs
                                  join user in _context.DmUsers on diem_truc.MaUser equals user.MaUser
                                  where (
                                    (searchTarget.MaDiemtruc != null ? diem_truc.MaDiemtruc.Contains(searchTarget.MaDiemtruc) : true) &&
                                    (searchTarget.TenDiemtruc != null ? diem_truc.TenDiemtruc.Contains(searchTarget.TenDiemtruc) : true) &&
                                    (searchTarget.Description != null ? diem_truc.Description.Contains(searchTarget.Description) : true) &&
                                    (searchTarget.MaUser != null ? diem_truc.MaUser.Contains(searchTarget.MaUser) : true) &&
                                    (searchTarget.Active!= null ? diem_truc.Active == searchTarget.Active : true) &&
                                    (searchTarget.NgayCnhat != null ? diem_truc.NgayCnhat >= searchTarget.NgayCnhat : true)
                                    )

                                  select new DmDiemtruc
                                  {
                                      PrKey = diem_truc.PrKey,
                                      MaDiemtruc = diem_truc.MaDiemtruc,
                                      TenDiemtruc = diem_truc.TenDiemtruc,
                                      Description = diem_truc.Description,
                                      Active = diem_truc.Active,
                                      MaUser = diem_truc.MaUser,
                                      NgayCnhat = diem_truc.NgayCnhat,
                                      Count = _context.DmDiemtrucs.Count()
                                  }
                      ).Skip(limit * (pageNumber - 1)).Take(limit).AsQueryable();

            return ToListWithNoLockAsync(list_diem_truc);
        }


        // Lấy danh sách các user, không lấy các user loại 0, 1, 2, 3, 5, 6, 7
        public Task<List<DmUser>> getStationUserList(int pageNumber, int limit)
        {
            int[] acceptedUserTypes = { 1, 6, 9, 10, 11 };
            var list_user_gdtt = (from user in _context.DmUsers
                                  where (acceptedUserTypes.Contains((int)user.LoaiUser))
                                  select new DmUser
                                  {
                                      Oid = user.Oid,
                                      MaUser = user.MaUser,
                                      TenUser = user.TenUser,
                                      LoaiUser = user.LoaiUser,
                                      Dienthoai = user.Dienthoai
                                  }

                      ).Skip(limit*(pageNumber-1)).Take(limit).Distinct().AsQueryable(); // Chọn Distinct, loại các user trùng nhau.
            return ToListWithNoLockAsync(list_user_gdtt);
        }

        // Lay danh sach ten cac diem truc
        public Task<List<DmDiemtruc>> getStationNameList(int pageNumber, int limit)
        {
            var list_user_gdtt = (from diem_truc in _context.DmDiemtrucs
                                  select new DmDiemtruc
                                  {
                                      MaDiemtruc = diem_truc.MaDiemtruc,
                                      TenDiemtruc = diem_truc.TenDiemtruc,
                                      Count = _context.DmDiemtrucs.Count()
                                  }

                      ).Skip(limit * (pageNumber-1)).Take(limit).Distinct().AsQueryable();
            return ToListWithNoLockAsync(list_user_gdtt);
        }

        // Tao diem truc moi
        public async Task<string> createStation(DmDiemtruc diemtruc, string currentUserEmail)
        {
            
            // Kiểm tra nếu điểm trực không tồn tại thì mới tiến hành insert
            var checkExist = FirstOrDefaultWithNoLock(_context.DmDiemtrucs.Where(x => x.TenDiemtruc == diemtruc.TenDiemtruc).AsQueryable());
            if (checkExist == null)
            {
                DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
                int[] acceptedUsers = new int[] { 1, 6, 9, 10, 11 };
                if (currentUser != null && Array.Exists(acceptedUsers, x => x == currentUser.LoaiUser))
                {
                    try
                    {
                        // Đánh mã điểm trực theo số thứ tự
                        int currCount = _context.DmDiemtrucs.Count(); // Lấy số lượng điểm trực
                        diemtruc.MaDiemtruc = (currCount + 1).ToString();

                        diemtruc.NgayCnhat = DateTime.Now; // Chỉnh ngày cập nhật.
                        await _context.DmDiemtrucs.AddAsync(diemtruc);
                        await _context.SaveChangesAsync();

                        return diemtruc.MaDiemtruc.ToString();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("dbContextTransaction Exception when CreateHsgdTtrinh: " + ex.ToString());
                        _logger.Error("Error record: " + JsonConvert.SerializeObject(diemtruc));
                        await _context.DisposeAsync();
                        throw;
                    }
                } else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


        // Update điểm trực
        public string updateStation(DmDiemtruc diemtruc, string currentUserEmail)
        {
            try
            {
                DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
                int[] acceptedUsers = new int[] { 1, 6, 9, 10, 11 };
                if (currentUser != null && Array.Exists(acceptedUsers, x => x == currentUser.LoaiUser))
                {
                    diemtruc.NgayCnhat = DateTime.Now; // Chỉnh ngày cập nhật
                    _context.DmDiemtrucs.Update(diemtruc);
                    _context.SaveChanges();
                    return diemtruc.MaDiemtruc.ToString();
                } else
                {
                    return "User không tồn tại";
                }
            }
            catch (Exception ex)
            {
                _logger.Error("dbContextTransaction Exception when updateStation: " + ex.ToString());
                _logger.Error("Error record: " + JsonConvert.SerializeObject(diemtruc));
                _context.Dispose();
                throw;
            }
        }
    }
}