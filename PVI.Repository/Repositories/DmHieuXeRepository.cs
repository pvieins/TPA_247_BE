using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PVI.DAO.Entities.Models;
using PVI.Repository.Interfaces;
using System.Linq;
using System.Collections.Generic;

namespace PVI.Repository.Repositories
{

    /* Implementation cho interface hiệu xe loại xe
     * lhkhanh - 26/09/2024
     */

    // Class con để trả thông tin của hạng mục.
    public class ListHieuXe
    {
        public int count { get; set; }
        public List<DmHieuxe> listHieuXe { get; set; }
    }

    // Class con để trả thông tin của nhóm hạng mục.
    public class ListLoaiXe
    {
        public int count { get; set; }
        public List<DmLoaixe> listLoaiXe { get; set; }
    }

    // Class con để trả thông tin của tổng thành xe.
    // Kế thừa base.
    public class DmHieuXeRepository : GenericRepository<DmHieuxe>, IDmHieuXeRepository
    {
        public DmHieuXeRepository(GdttContext context, Pvs2024Context context_pias, Serilog.ILogger logger, IConfiguration conf) : base(context, context_pias, logger, conf)
        {

        }

        // Tham số: Page & Limit - Dùng để phân trang, có kèm theo các filter.
        public ListHieuXe getListHieuXe(int pageNumber, int limit, DmHieuxe searchTarget)
        {
            List<DmHieuxe> list_hieuxe = (from hieuxe in _context.DmHieuxes
                               where (
                             (searchTarget.HieuXe != null ? hieuxe.HieuXe.Contains(searchTarget.HieuXe) : true) &&
                             (hieuxe.HieuXe != "")
                             )
                             orderby hieuxe.HieuXe
                               select new DmHieuxe
                               {
                                   PrKey = hieuxe.PrKey,
                                   HieuXe = hieuxe.HieuXe
                               }
                        ).ToList();

            ListHieuXe toBeReturned = new ListHieuXe
            {
                count = (list_hieuxe != null ? list_hieuxe.Count() : 0),
                listHieuXe = list_hieuxe.Skip(limit * (pageNumber - 1)).Take(limit).ToList()
            };

            return toBeReturned;
        }

        // Tham số: Page & Limit - Dùng để phân trang, có kèm theo các filter.
        public ListLoaiXe getListLoaiXe(int pageNumber, int limit, DmLoaixe searchTarget)
        {
            var list_loaiXe = (from loaiXe in _context.DmLoaixes
                              join hieuXe in _context.DmHieuxes on loaiXe.FrKey equals hieuXe.PrKey
                              //join user in _context.DmUsers on loaiXe.MaUser equals user.Oid.ToString()
                              orderby loaiXe.Hieuxe
                              where (
                              (loaiXe.LoaiXe != null) &&
                              (searchTarget.LoaiXe != null ? loaiXe.LoaiXe.Contains(searchTarget.LoaiXe) : true) &&
                              (searchTarget.Hieuxe != null ? hieuXe.HieuXe.Contains(searchTarget.Hieuxe) : true) &&
                              //(searchTarget.TenUser != null ? user.TenUser.Contains(searchTarget.TenUser) : true) &&
                              (searchTarget.NgayCapnhat != null ? loaiXe.NgayCapnhat >= searchTarget.NgayCapnhat : true)
                              )

                              select new DmLoaixe
                              {
                                  PrKey = loaiXe.PrKey,
                                  FrKey = loaiXe.FrKey,
                                  Hieuxe = hieuXe.HieuXe,
                                  LoaiXe = loaiXe.LoaiXe,
                                  MaUser = loaiXe.MaUser,
                                  //TenUser = !String.IsNullOrEmpty(loaiXe.MaUser) ? _context.DmUsers.Where user.TenUser,
                                  NgayCapnhat = loaiXe.NgayCapnhat,
                              }
                        ).ToList();

            // Trả về danh sách loại xe.
            ListLoaiXe toBeReturned = new ListLoaiXe
            {
                count = (list_loaiXe != null ? list_loaiXe.Count() : 0),
                listLoaiXe = list_loaiXe.Skip(limit * (pageNumber - 1)).Take(limit).ToList()
            };

            return toBeReturned;
        }

        // Tạo hiệu xe
        public async Task<string> createHieuXe(DmHieuxe hieuxe, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.Where(x=>x.Mail.Equals(currentUserEmail)).FirstOrDefault();
            
            // Kiểm tra phân quyền
            if ((currentUser != null) && (currentUser.LoaiUser == 1 || currentUser.LoaiUser == 6 || currentUser.LoaiUser == 8 || currentUser.LoaiUser == 9 || currentUser.LoaiUser == 10 ||currentUser.LoaiUser == 11))
            {
                // Sau đó kiểm tra nếu hiệu xe không tồn tại thì mới được insert
                var checkExist = _context.DmHieuxes.Where(x => x.HieuXe == hieuxe.HieuXe).FirstOrDefault();
                if (checkExist == null)
                {
                    try
                    {
                        // Tiến hành lưu vào DB.
                        hieuxe.PrKey = 0;
                        _context.DmHieuxes.Add(hieuxe);
                        await _context.SaveChangesAsync();

                        return hieuxe.PrKey.ToString();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("dbContextTransaction Exception when CreateloaiXe: " + ex.ToString());
                        _logger.Error("Error record: " + JsonConvert.SerializeObject(hieuxe));
                        await _context.DisposeAsync();
                        throw;
                    }
                }
                else
                {
                    return $"Loại xe {hieuxe.HieuXe} đã tồn tại.";
                }
            } else
            {
                return "User không được phân quyền";
            }
        }

        // Tạo loại xe
        public async Task<string> createLoaiXe(DmLoaixe loaixe, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();

            // Kiểm tra phân quyền
            if ((currentUser != null) && (currentUser.LoaiUser == 1 || currentUser.LoaiUser == 6 || currentUser.LoaiUser == 8 || currentUser.LoaiUser == 9 || currentUser.LoaiUser == 10 ||currentUser.LoaiUser == 11))
            {
                // Kiểm tra nhóm hạng mục không tồn tại thì mới tiến hành insert
                var checkExist = _context.DmLoaixes.Where(x => x.LoaiXe == loaixe.LoaiXe && x.FrKey == loaixe.FrKey).FirstOrDefault();
                if (checkExist == null)
                {
                    try
                    {
                        // Tiến hành lưu vào DB.
                        loaixe.NgayCapnhat = DateTime.Now;
                        loaixe.MaUser = currentUser.Oid.ToString();
                        _context.DmLoaixes.Add(loaixe);
                        await _context.SaveChangesAsync();
                        return loaixe.PrKey.ToString();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("dbContextTransaction Exception when CreateHmuc: " + ex.ToString());
                        _logger.Error("Error record: " + JsonConvert.SerializeObject(loaixe));
                        await _context.DisposeAsync();
                        throw;
                    }
                }
                else
                {
                    return $"Loại xe {loaixe.LoaiXe} của hiệu xe này đã tồn tại.";
                }
            } else
            {
                return "User không được phân quyền";
            }
        }

        // Tạo loại xe
        public async Task<string> updateHieuXe(int prKey, DmHieuxe hieuxe, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();

            // Kiểm tra phân quyền
            if ((currentUser != null) && (currentUser.LoaiUser == 1 || currentUser.LoaiUser == 6 || currentUser.LoaiUser == 8 || currentUser.LoaiUser == 9 || currentUser.LoaiUser == 10 ||currentUser.LoaiUser == 11))
            {
                // Kiểm tra nhóm hạng mục có tồn tại thì mới tiến hành update
                DmHieuxe checkExist = _context.DmHieuxes.Where(x => x.PrKey == prKey).FirstOrDefault();
                if (checkExist != null)
                {
                    // sau đó, kiếm tra là hiệu xe mới được update phải không trùng với hiệu xe đã có trước đó.
                    DmHieuxe checkDuplicate = _context.DmHieuxes.Where(x => x.HieuXe.Equals(hieuxe.HieuXe)).FirstOrDefault();
                    if (checkDuplicate == null)
                    {
                        try
                        {
                            checkExist.HieuXe = hieuxe.HieuXe;
                            _context.DmHieuxes.Update(checkExist);
                            await _context.SaveChangesAsync();

                            return checkExist.PrKey.ToString();
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("dbContextTransaction Exception when UpdateloaiXe: " + ex.ToString());
                            _logger.Error("Error record: " + JsonConvert.SerializeObject(checkExist));
                            await _context.DisposeAsync();
                            throw;
                        }
                    } else
                    {
                        return "Hiệu xe này đã tồn tại";
                    }
                }
                else
                {
                    return $"Hiệu xe {prKey} không tồn tại.";
                }
            } else
            {
                return "User không được phân quyền";
            }
        }

        // Update loại xe
        public async Task<string> updateLoaiXe(int prKey, DmLoaixe loaixe, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();

            // Kiểm tra phân quyền
            if ((currentUser != null) && (currentUser.LoaiUser == 1 || currentUser.LoaiUser == 6 || currentUser.LoaiUser == 8 || currentUser.LoaiUser == 9 || currentUser.LoaiUser == 10 ||currentUser.LoaiUser == 11))
            {
                // Kiểm tra loại xe có tồn tại thì mới tiến hành update
                var checkExist = _context.DmLoaixes.Where(x => x.PrKey == prKey).FirstOrDefault();

                // Sau đó, kiểm tra update phải không được trùng với các loại xe đã có sẵn.
                if (checkExist != null)
                {
                    DmLoaixe checkDuplicate = _context.DmLoaixes.Where(x => x.Hieuxe.Equals(loaixe.Hieuxe) && x.LoaiXe.Equals(loaixe.LoaiXe)).FirstOrDefault();
                    if (checkDuplicate == null)
                    {
                        try
                        {
                            checkExist.FrKey = loaixe.FrKey;
                            checkExist.LoaiXe = loaixe.LoaiXe;
                            checkExist.NgayCapnhat = DateTime.Now;
                            checkExist.MaUser = currentUser.Oid.ToString();

                            _context.DmLoaixes.Update(checkExist);
                            await _context.SaveChangesAsync();

                            return checkExist.PrKey.ToString();
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("dbContextTransaction Exception when UpdateHmuc: " + ex.ToString());
                            _logger.Error("Error record: " + JsonConvert.SerializeObject(checkExist));
                            await _context.DisposeAsync();
                            throw;
                        }
                    } else
                    {
                        return "Loại xe đã tồn tại";
                    }
                } 
                else
                {
                    return $"Loại xe {prKey} không tồn tại.";
                }
            } else
            {
                return "User không được phân quyền";
            }
        }



    }
}