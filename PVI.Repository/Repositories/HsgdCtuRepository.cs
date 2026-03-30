using Azure.Core;
using ICSharpCode.SharpZipLib.Core;
using iTextSharp.text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PVI.DAO.Entities.Models;
using PVI.Helper;
using PVI.Repository.Interfaces;
using PVI.Repository.Repositories;
using RestSharp;
using Serilog.Core;
using ServiceReference1;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.WebSockets;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static Azure.Core.HttpHeader;
using static iTextSharp.text.pdf.events.IndexEvents;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;
using File = System.IO.File;
using Task = System.Threading.Tasks.Task;

namespace PVI.Repository.Repositories
{
    public class HsgdCtuRepository : GenericRepository<HsgdCtu>, IHsgdCtuRepository
    {
        HsgdCtuHelper PheDuyetHelper; // Chứa các method helper cho quá trình phê duyệt hồ sơ.
        HsgdDxRepository _dx_repo; // Repository của HSGD_DX, sử dụng để tính toán và validate phê duyệt hồ sơ.


        public HsgdCtuRepository(GdttContext context, Pvs2024Context context_pias, Pvs2024UpdateContext context_pias_update, MY_PVIContext my_pvi_context, Serilog.ILogger logger, IConfiguration conf) : base(context, context_pias, context_pias_update, my_pvi_context, logger, conf)
        {
            PheDuyetHelper = new HsgdCtuHelper(context, context_pias, context_pias_update, logger, conf);
            _dx_repo = new HsgdDxRepository(context, context_pias, context_pias_update, logger, conf);
        }

        public DmUserView GetCurrentUserInfo(string currentUserEmail)
        {
            try
            {
                DmUserView currentUser = (from user in _context.DmUsers
                                          where user.Mail.Equals(currentUserEmail)
                                          select new DmUserView
                                          {
                                              Oid = user.Oid,
                                              MaDonvi = user.MaDonvi,
                                              TenUser = user.TenUser,
                                              MaUser = user.MaUser,
                                              LoaiUser = user.LoaiUser,
                                              Dienthoai = user.Dienthoai,
                                              UQ_HoSo_TPC = PheDuyetHelper.check_UyQuyen_HoSoTPC(user),
                                              isGdvHoTro = user.IsGdvHotro,
                                              MaDonviPquyen = user.MaDonviPquyen
                                          }).FirstOrDefault();

                return currentUser;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<HsgdCtu> GetBySoHsgd(string so_hsgd)
        {

            HsgdCtu? objResult = new HsgdCtu();
            try
            {
                objResult = await GetEntityByCondition(x => x.SoHsgd == so_hsgd);
            }
            catch (Exception ex)
            {
            }
            return objResult;
        }
        public async Task<HsgdCtu> GetHsgdByPrKey(decimal pr_key)
        {
            HsgdCtu? objResult = new HsgdCtu();
            try
            {
                objResult = await GetEntityByCondition(x => x.PrKey == pr_key);
            }
            catch (Exception ex)
            {
            }
            return objResult;
        }
        public List<HsgdTtrinhCt> GetListHsgdDx(string so_hsgd)
        {
            List<HsgdTtrinhCt> obj_result = new List<HsgdTtrinhCt>();
            try
            {
                var obj = (from A in _context.HsgdCtus
                           join B in _context.HsgdDxes on A.PrKey equals B.FrKey
                           where A.SoHsgd == so_hsgd
                           select new
                           {
                               TyleggSuachua = B.LoaiDx == 0 ? (decimal)A.TyleggSuachuavcx : (decimal)A.TyleggSuachuatnds,
                               TyleggPhutung = B.LoaiDx == 0 ? (decimal)A.TyleggPhutungvcx : (decimal)A.TyleggPhutungtnds,
                               A.SoTienctkh,
                               A.SoTienGtbt,
                               MaSp = B.LoaiDx == 0 ? "050104" : "050101",
                               Tienpdtt = B.SoTienpdtt * (1 + (decimal)B.VatSc / 100),
                               Tienpdsc = B.SoTienpdsc * (1 + (decimal)B.VatSc / 100),
                               GiamTruBt = (decimal)B.GiamTruBt
                               //GiamGia = (B.SoTienpdtt * (1 + B.VatSc) / 100) * A.TyleggPhutungvcx / 100 + (B.SoTienpdsc * (1 + B.VatSc) / 100) * A.TyleggSuachuavcx / 100
                           }).AsQueryable();
                var obj_gg = (from A in obj
                              select new
                              {
                                  A.SoTienctkh,
                                  A.SoTienGtbt,
                                  A.MaSp,
                                  A.Tienpdtt,
                                  A.Tienpdsc,
                                  Tienpd = A.Tienpdtt + A.Tienpdsc,
                                  GiamGia = A.Tienpdtt * A.TyleggPhutung / 100 + A.Tienpdsc * A.TyleggSuachua / 100,
                                  SotienGtruBt = A.Tienpdtt * (1 - A.TyleggPhutung / 100) * A.GiamTruBt / 100 + A.Tienpdsc * (1 - A.TyleggSuachua / 100) * A.GiamTruBt / 100
                              }).AsQueryable();

                var obj_stbt = ToListWithNoLock((from A in obj_gg
                                                 select new
                                                 {
                                                     A.MaSp,
                                                     A.SoTienctkh,
                                                     A.SoTienGtbt,
                                                     SotienBt = A.Tienpd - A.GiamGia - A.SotienGtruBt
                                                 }).AsQueryable());
                obj_result = obj_stbt.GroupBy(n => new { n.MaSp, n.SoTienctkh }).Select(p => new HsgdTtrinhCt
                {
                    MaSp = p.Key.MaSp,
                    SotienBt = p.Sum(x => x.SotienBt) - p.Key.SoTienctkh
                }).ToList();
                var so_donbh = FirstOrDefaultWithNoLock((from a in _context.HsgdCtus
                                                         where a.SoHsgd == so_hsgd
                                                         select a.SoDonbh)
                                                    .AsQueryable());
                if (!string.IsNullOrEmpty(so_donbh))
                {
                    var mtn = (from A in _context_pias.NvuBhtCtus
                               join B in _context_pias.NvuBhtCts on A.PrKey equals B.FrKey
                               where A.SoDonbhSdbs == so_donbh
                               select new
                               {
                                   B.MaSp,
                                   SotienBh = B.SoTienbhLke != 0 ? B.SoTienbhLke : B.SoTienbh
                               }).AsQueryable();
                    obj_result = ToListWithNoLock((from A in obj_result
                                                   join B in mtn on A.MaSp equals B.MaSp into B1
                                                   from B in B1.DefaultIfEmpty()
                                                   select new HsgdTtrinhCt
                                                   {
                                                       MaSp = A.MaSp,
                                                       SotienBt = A.SotienBt,
                                                       SotienBh = B.SotienBh
                                                   }).AsQueryable());
                }

            }
            catch (Exception ex)
            {
            }
            return obj_result;
        }
        public HsgdTtrinhAll GetListHsgdDxNew(string so_hsgd)
        {
            HsgdTtrinhAll tt = new HsgdTtrinhAll();
            List<HsgdTtrinhCt> obj_result = new List<HsgdTtrinhCt>();
            try
            {
                var pr_key_bt = _context.HsgdCtus.Where(x => x.SoHsgd == so_hsgd).Select(s => s.PrKeyBt).FirstOrDefault();
                if (pr_key_bt != 0)
                {
                    var hsbt_ct = ToListWithNoLock(_context_pias.HsbtCts.Where(x => x.FrKey == pr_key_bt).Select(s => new
                    {
                        s.PrKey,
                        s.MaSp

                    }).AsQueryable());
                    //var hsgd_dx_ct_tmp = ToListWithNoLock(_context.HsgdDxCts.Where(p => hsbt_ct.Any(p2 => p2.PrKey == p.PrKeyHsbtCt)).AsQueryable());
                    var hsgd_dx_ct_tmp = (from a in _context.HsgdDxCts where hsbt_ct.Select(x => x.PrKey).ToArray().Contains(a.PrKeyHsbtCt) select a).AsQueryable();
                    var hsgd_dx_ct = ToListWithNoLock((from A in hsbt_ct
                                                       join B in hsgd_dx_ct_tmp on A.PrKey equals B.PrKeyHsbtCt
                                                       // where new[] { "050101", "050104" }.Contains(B.MaSp)
                                                       select new
                                                       {
                                                           PrKey = B.PrKey,
                                                           MaSp = A.MaSp,
                                                           TyleggPhutungvcx = B.TyleggPhutungvcx,
                                                           TyleggSuachuavcx = B.TyleggSuachuavcx,
                                                           SoTienctkh = B.SoTienctkh,
                                                           SoTienGtbt = B.SoTienGtbt
                                                       }).AsQueryable());
                    List<TT_HsgdDx> obj_dx = new List<TT_HsgdDx>();
                    if (hsgd_dx_ct.Where(x => new[] { "050101", "050104" }.Contains(x.MaSp)).Count() > 0)
                    {
                        var hsgd_dx = ToListWithNoLock((from a in _context.HsgdDxes where hsgd_dx_ct.Select(x => x.PrKey).ToArray().Contains(a.PrKeyDx) select a).AsQueryable());
                        obj_dx = (from A in hsgd_dx
                                  join B in hsgd_dx_ct on A.PrKeyDx equals B.PrKey
                                  where new[] { "050101", "050104" }.Contains(B.MaSp)
                                  select new TT_HsgdDx
                                  {
                                      TyleggSuachua = (decimal)B.TyleggSuachuavcx,
                                      TyleggPhutung = (decimal)B.TyleggPhutungvcx,
                                      SoTienctkh = B.SoTienctkh,
                                      SoTienGtbt = B.SoTienGtbt,
                                      MaSp = B.MaSp,
                                      Tienpdtt = A.SoTienpdtt * (1 + (decimal)A.VatSc / 100),
                                      Tienpdsc = A.SoTienpdsc * (1 + (decimal)A.VatSc / 100),
                                      GiamTruBt = (decimal)A.GiamTruBt
                                  }).ToList();
                    }
                    List<TT_HsgdDx> obj_dx_tsk = new List<TT_HsgdDx>();
                    if (hsgd_dx_ct.Where(x => !new[] { "050101", "050104" }.Contains(x.MaSp)).Count() > 0)
                    {
                        var hsgd_dx_tsk = ToListWithNoLock((from a in _context.HsgdDxTsks where hsgd_dx_ct.Select(x => x.PrKey).ToArray().Contains(a.PrKeyDx) select a).AsQueryable());
                        obj_dx_tsk = (from A in hsgd_dx_tsk
                                      join B in hsgd_dx_ct on A.PrKeyDx equals B.PrKey
                                      where !new[] { "050101", "050104" }.Contains(B.MaSp)
                                      select new TT_HsgdDx
                                      {
                                          TyleggSuachua = (decimal)B.TyleggSuachuavcx,
                                          TyleggPhutung = (decimal)B.TyleggPhutungvcx,
                                          SoTienctkh = B.SoTienctkh,
                                          SoTienGtbt = B.SoTienGtbt,
                                          MaSp = B.MaSp,
                                          Tienpdtt = A.SoTientt * (1 + (decimal)A.VatSc / 100),
                                          Tienpdsc = A.SoTiensc * (1 + (decimal)A.VatSc / 100),
                                          GiamTruBt = (decimal)A.GiamTruBt
                                      }).ToList();
                    }
                    var obj = obj_dx.Union(obj_dx_tsk).ToList();
                    var obj_gg = (from A in obj
                                  select new
                                  {
                                      A.SoTienctkh,
                                      A.SoTienGtbt,
                                      A.MaSp,
                                      A.Tienpdtt,
                                      A.Tienpdsc,
                                      Tienpd = A.Tienpdtt + A.Tienpdsc,
                                      GiamGia = A.Tienpdtt * A.TyleggPhutung / 100 + A.Tienpdsc * A.TyleggSuachua / 100,
                                      SotienGtruBt = A.Tienpdtt * (1 - A.TyleggPhutung / 100) * A.GiamTruBt / 100 + A.Tienpdsc * (1 - A.TyleggSuachua / 100) * A.GiamTruBt / 100
                                  }).ToList();

                    var obj_stbt = (from A in obj_gg
                                    select new
                                    {
                                        A.MaSp,
                                        A.SoTienctkh,
                                        A.SoTienGtbt,
                                        SotienBt = A.Tienpd - A.GiamGia - A.SotienGtruBt
                                    }).ToList();
                    obj_result = obj_stbt.GroupBy(n => new { n.MaSp, n.SoTienctkh, n.SoTienGtbt }).Select(p => new HsgdTtrinhCt
                    {
                        MaSp = p.Key.MaSp,
                        SotienBt = p.Sum(x => x.SotienBt) - p.Key.SoTienctkh - p.Key.SoTienGtbt
                    }).ToList();

                    var so_donbh = FirstOrDefaultWithNoLock((from a in _context.HsgdCtus
                                                             where a.SoHsgd == so_hsgd
                                                             select a.SoDonbh)
                                                    .AsQueryable());
                    if (!string.IsNullOrEmpty(so_donbh))
                    {
                        var mtn = (from A in _context_pias.NvuBhtCtus
                                   join B in _context_pias.NvuBhtCts on A.PrKey equals B.FrKey
                                   where A.SoDonbhSdbs == so_donbh
                                   select new
                                   {
                                       B.MaSp,
                                       SotienBh = B.SoTienbhLke != 0 ? B.SoTienbhLke : B.SoTienbh
                                   }).AsQueryable();
                        obj_result = ToListWithNoLock((from A in obj_result
                                                       join B in mtn on A.MaSp equals B.MaSp into B1
                                                       from B in B1.DefaultIfEmpty()
                                                       select new HsgdTtrinhCt
                                                       {
                                                           MaSp = A.MaSp,
                                                           SotienBt = Math.Round(A.SotienBt, 0, MidpointRounding.AwayFromZero),
                                                           SotienBh = B.SotienBh
                                                       }).AsQueryable());
                    }
                    tt.hsgdTtrinhCt = obj_result;
                    var tmp_thuhuong = (from A in obj_result
                                        join B in hsgd_dx_ct_tmp on A.MaSp equals B.MaSp
                                        select new
                                        {
                                            MaSp = A.MaSp,
                                            SotienBt = A.SotienBt,
                                            MaGara = B.MaGara
                                        }).ToList();
                    var tmp_thuhuong_grp = tmp_thuhuong.GroupBy(n => new { n.MaSp, n.SotienBt }).Select(p => new
                    {
                        MaSp = p.Key.MaSp,
                        SotienBt = p.Key.SotienBt,
                        MaGara = p.Max(x => x.MaGara)
                    }).ToList();
                    var dm_kh = ToListWithNoLock((from a in _context_pias.DmKhaches
                                                  where a.Gara == true
                                                  select new
                                                  {
                                                      a.MaKh,
                                                      a.TenKh,
                                                      a.NganHang,
                                                      a.TkVnd
                                                  }).AsQueryable());
                    tt.hsgdTtrinhTt = (from A in tmp_thuhuong_grp
                                       join B in dm_kh on A.MaGara equals B.MaKh
                                       join C in _context.DmGaRas on A.MaGara equals C.MaGara
                                       select new HsgdTtrinhTt
                                       {
                                           SotienTt = A.SotienBt,
                                           TenChuTk = C.ten_ctk,
                                           SoTaikhoanNh = B.TkVnd,
                                           TenNh = B.NganHang,
                                           bnkCode=C.bnkCode
                                       }).ToList();
                }


            }
            catch (Exception ex)
            {
            }
            return tt;
        }
        // Đổi thành huỷ hồ sơ
        // khanhlh - 07/09/2024
        public string updateDetailFile(int pr_key, int type, string currentUserEmail)
        {
            try
            {
                // Huỷ hồ sơ cho cho MyPVI
                if (type == 1)
                {
                    KbttCtu ho_so_update = _context_my_pvi.KbttCtus.Where(x => x.PrKey == pr_key).FirstOrDefault();
                    DmUser currentUser = _context.DmUsers.Where(x => x.Mail == currentUserEmail).FirstOrDefault();
                    if (ho_so_update != null && currentUser != null)
                    {
                        if (ho_so_update.TinhTrang != 4)
                        {
                            // Phân quyền user.
                            int[] allowedUserTypes = new int[] { 9, 10, 11 };
                            if (Array.Exists(allowedUserTypes, x => x == currentUser.LoaiUser))
                            {
                                ho_so_update.TinhTrang = 5;
                                _context_my_pvi.KbttCtus.Update(ho_so_update);
                                _context_my_pvi.SaveChanges();
                                return ho_so_update.PrKey.ToString();
                            }
                            else
                            {
                                return "Bạn không được phân quyền huỷ hồ sơ";
                            }
                        }
                        else
                        {
                            return "Không thể huỷ hồ sơ đã duyệt";
                        }
                    }
                    else
                    {
                        return "Hồ sơ lỗi, vui lòng liên hệ IT";
                    }
                }
                // Huỷ hồ sơ cho HSGD_CTU
                else
                {
                    HsgdCtu ho_so_update = FirstOrDefaultWithNoLock(_context.HsgdCtus.Where(x => x.PrKey == pr_key).AsQueryable());
                    DmUser currentUser = _context.DmUsers.Where(x => x.Mail == currentUserEmail).FirstOrDefault();
                    if (ho_so_update != null && currentUser != null)
                    {
                        // Phân quyền user.
                        int[] allowedUserTypes = new int[] { 1, 9, 10, 11 };
                        if (Array.Exists(allowedUserTypes, x => x == currentUser.LoaiUser))
                        {
                            if (ho_so_update.MaTtrangGd != "6")
                            {
                                var hsbt_ct_check = _context_pias_update.HsbtCts.Where(x => x.FrKey != 0 && x.FrKey == ho_so_update.PrKeyBt && x.MaTtrangBt == "03").ToList();
                                if (hsbt_ct_check.Count() == 0)
                                {
                                    using var context_gdtt_new = new GdttContext();
                                    using var dbContextTransaction = context_gdtt_new.Database.BeginTransaction();

                                    using var context_pias_new = new Pvs2024UpdateContext();
                                    using var dbContextTransaction2 = context_pias_new.Database.BeginTransaction();

                                    try
                                    {
                                        ho_so_update.MaTtrangGd = "7";
                                        context_gdtt_new.HsgdCtus.Update(ho_so_update);

                                        NhatKy new_nhatky = new NhatKy
                                        {
                                            FrKey = ho_so_update.PrKey,
                                            MaTtrangGd = "7",
                                            TenTtrangGd = "Hồ sơ đã huỷ",
                                            GhiChu = "Cán bộ huỷ hồ sơ",
                                            NgayCapnhat = DateTime.Now,
                                            MaUser = currentUser.Oid
                                        };

                                        context_gdtt_new.NhatKies.Add(new_nhatky);
                                        // đóng hs bên 247 thì đóng 04 pias
                                        var hsbt_ct = context_pias_new.HsbtCts.Where(x => x.FrKey != 0 && x.FrKey == ho_so_update.PrKeyBt).ToList();
                                        if (hsbt_ct.Count > 0)
                                        {
                                            hsbt_ct.ForEach(a => {
                                                a.MaTtrangBt = "04";
                                                a.NgayHtoanBt = DateTime.Today;
                                            });
                                            context_pias_new.HsbtCts.UpdateRange(hsbt_ct);
                                        }

                                        context_pias_new.SaveChanges();
                                        dbContextTransaction2.Commit();

                                        context_gdtt_new.SaveChanges();
                                        dbContextTransaction.Commit();
                                        return ho_so_update.PrKey.ToString();
                                    }
                                    catch (Exception ex)
                                    {

                                        _logger.Error("updateDetailFile Exception : " + ex.ToString());
                                        dbContextTransaction2.Rollback();
                                        dbContextTransaction2.Dispose();
                                        dbContextTransaction.Rollback();
                                        dbContextTransaction.Dispose();
                                        return "Không thành công";
                                    }
                                }
                                else
                                {
                                    return "Hsbt đã chuyển trạng thái 03. Không thể hủy hồ sơ.";
                                }

                            }
                            else
                            {
                                return "Không thể huỷ hồ sơ đã duyệt";
                            }
                        }
                        else
                        {
                            return "Bạn không được phân quyền huỷ hồ sơ";
                        }
                    }
                    else
                    {
                        return "Hồ sơ lỗi, vui lòng liên hệ IT";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("dbContextTransaction Exception when CreateHsgdTtrinh: " + ex.ToString());
                _context.Dispose();
                throw;
            }
        }

        public List<TtrangGdCount> GetCountByStatus(string fromDate, string toDate, string email, string MaDonbh)
        {

            DateTime? fromDate1 = null;
            DateTime? toDate1 = null;
            if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
            {
                try
                {
                    fromDate1 = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    toDate1 = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    // Ensure ToDate includes the entire day
                    toDate1 = toDate1.Value.AddDays(1).AddTicks(-1); // Set to end of the day
                }
                catch (FormatException)
                {
                    throw new ArgumentException("Invalid date format for FromDate or ToDate. Expected format: dd/MM/yyyy");
                }
            }
            var user = _context.DmUsers.Where(x => x.Mail == email && x.IsActive == true).FirstOrDefault();
            if (user != null)
            {
                var tt = (from a in _context.HsgdCtus
                          where (fromDate1 == null || a.NgayCtu >= fromDate1) &&
                    (toDate1 == null || a.NgayCtu <= toDate1) && a.MaDonbh == MaDonbh
                          select new
                          {
                              a.PrKey,
                              a.MaTtrangGd,
                              a.MaDonvigd,
                              a.MaDonvi,
                              a.PrKeyBt
                          }).AsQueryable();
                var listDV = new List<string> { "00", "31", "32" };
                if (user.MaDonvi.ToString().Equals("00"))
                {
                    tt = tt.Where(x => listDV.Contains(x.MaDonvigd));
                }
                else if (user.MaDonvi.ToString().Equals("31"))
                {
                    tt = tt.Where(x => x.MaDonvigd == "31");
                }
                else if (user.MaDonvi.ToString().Equals("32"))
                {
                    tt = tt.Where(x => x.MaDonvigd == "32");
                }
                else
                {
                    tt = tt.Where(x => x.MaDonvigd != "");
                }
                var ma_donvi_pquyen = user.MaDonviPquyen.ToString();
                if (!string.IsNullOrEmpty(ma_donvi_pquyen))
                {
                    List<string> list_ma_donvi_pquyen = ma_donvi_pquyen.Split(",").ToList();
                    tt = tt.Where(x => list_ma_donvi_pquyen.Contains(x.MaDonvi));
                }
                else
                {
                    tt = tt.Where(x => x.MaDonvi == user.MaDonvi);
                }
                var countHsChuaSangPias = tt.Where(x => x.PrKeyBt == 0 && x.MaTtrangGd != "7").Count();
                var tt_gr = ToListWithNoLock(tt.GroupBy(n => new { n.MaTtrangGd }).Select(p => new TtrangGdCount
                {
                    MaTtrangGd = p.Key.MaTtrangGd,
                    SoHsgd = p.Count()
                }).AsQueryable());

                var list_tt = ToListWithNoLock((from a in tt_gr
                                                join b in _context.DmTtrangGds on a.MaTtrangGd equals b.MaTtrangGd
                                                select new TtrangGdCount
                                                {
                                                    MaTtrangGd = a.MaTtrangGd,
                                                    TenTtrangGd = b.TenTtrangGd,
                                                    SoHsgd = a.SoHsgd
                                                }).OrderBy(x => x.MaTtrangGd).AsQueryable());
                var ttChuaSangPias = new TtrangGdCount
                {
                    MaTtrangGd = "00",
                    TenTtrangGd = "Hồ sơ chưa sang PIAS",
                    SoHsgd = countHsChuaSangPias
                };
                list_tt.Add(ttChuaSangPias);
                return list_tt;
            }
            else
            {
                return null!;
            }
        }

        public async Task<PagedList<HsgdCtuResponse>> GetList(HsgdCtuParameters parameters, string email, string MaDonbh)
        {
            try
            {
                _context.Database.SetCommandTimeout(300);
                //validMaDonvi.Contains(A.MaDonvi) &&
                //A.PrKeyBt == 0 &&
                //A.NgayCtu >= new DateTime(2016, 11, 11) &&
                //!excludedMaTtrangGd.Contains(A.MaTtrangGd) &&
                //A.HsgdTpc == 1 &&
                var userProfile = await _context.DmUsers.Where(x => x.Mail == email).Select(x => new
                {
                    x.MaDonviPquyen,
                    x.MaDonvi
                }).FirstOrDefaultAsync();
                if (userProfile == null) { return null; }
                // Parse date range from parameters
                DateTime? fromDate = null;
                DateTime? toDate = null;
                if (!string.IsNullOrEmpty(parameters.FromDate) && !string.IsNullOrEmpty(parameters.ToDate))
                {
                    try
                    {
                        fromDate = DateTime.ParseExact(parameters.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        toDate = DateTime.ParseExact(parameters.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        // Ensure ToDate includes the entire day
                        toDate = toDate.Value.AddDays(1).AddTicks(-1); // Set to end of the day
                    }
                    catch (FormatException)
                    {
                        throw new ArgumentException("Invalid date format for FromDate or ToDate. Expected format: dd/MM/yyyy");
                    }
                }
                // A.NgayCtu.Value.Year == parameters.Year &&
                //var validMaDonvi = new List<string> { "01", "12", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "29", "33", "35", "36", "39", "40", "43", "44" };
                //var excludedMaTtrangGd = new List<string> { "6", "7" };

                //(fromDate == null || A.NgayCtu >= fromDate) &&
                //(toDate == null || A.NgayCtu <= toDate) &&
                var result =
        from A in _context.HsgdCtus
        join B in _context.DmUsers on A.MaUser equals B.Oid into userJoin
        from B in userJoin.DefaultIfEmpty()
        join C in _context.DmDonvis on A.MaDonvi equals C.MaDonvi
        join E in _context.DmTtrangGds on A.MaTtrangGd equals E.MaTtrangGd
        where
      (fromDate == null || A.NgayCtu >= fromDate) &&
      (toDate == null || A.NgayCtu <= toDate) &&

      A.MaDonbh == MaDonbh
        orderby A.PrKey descending
        select new HsgdCtuResponse
        {
            Nam = A.NgayCtu.Value.Year,
            Thang = A.NgayCtu.Value.Month,
            SoNgaybh = EF.Functions.DateDiffDay(A.NgayDauSeri, A.NgayTthat),
            PrKey = A.PrKey,
            SoLanBt = A.SoLanBt,
            SoDonBh = A.SoDonbh,
            SoHsgd = A.SoHsgd,
            NgayCtu = A.NgayCtu,
            TenKhach = A.TenKhach,
            SoSeri = A.SoSeri,
            BienKsoat = A.BienKsoat,
            NgayTthat = A.NgayTthat,
            MaUser = A.MaUser.ToString() ?? "",
            TenUser = B.TenUser,
            MaDonvi = A.MaDonvi,
            PrKeyBt = A.PrKeyBt,
            TenDonvi = C.TenDonvi,
            MaTtrangGd = E.MaTtrangGd,
            TenTtrangGd = E.TenTtrangGd,
            MaDonviGd = A.MaDonvigd,
            MaDonviTt = A.MaDonviTt,
            TenLhsbt = A.MaLhsbt == "1" ? "Tự giám định" :
                        A.MaLhsbt == "2" ? "Nhờ giám định" :
                        A.MaLhsbt == "3" ? "Giám định hộ" : null,
            SoTienUocbt = (from D in _context.HsgdDgs
                           where D.FrKey == A.PrKey && D.LoaiDg == false
                           orderby D.SoTien descending
                           select (decimal?)D.SoTien).FirstOrDefault() ?? 0m,
            TienPheduyet = (from dx in _context.HsgdDxes
                            where dx.FrKey == A.PrKey
                            select dx.SoTienpdtt + dx.SoTienpdsc).Sum(),
            HieuXe = A.HieuXe,
            LoaiXe = A.LoaiXe,
            NamSx = A.NamSx,
            XuatXu = A.XuatXu,
            SoTienugd = A.SoTienugd,
            NguoiXuly = string.IsNullOrEmpty(A.NguoiXuly) ? "" :
                         (from user in _context.DmUsers
                          where user.Oid.ToString() == A.NguoiXuly
                          select user.TenUser).FirstOrDefault(),
            NguoiGiao = string.IsNullOrEmpty(A.NguoiGiao) ? "" :
                         (from user in _context.DmUsers
                          where user.Oid.ToString() == A.NguoiGiao
                          select user.TenUser).FirstOrDefault(),
            SoTienugddx = Int32.Parse(A.MaTtrangGd) < 4 ? A.SoTienugd :
                   (from dx in _context.HsgdDxes
                    where dx.FrKey == A.PrKey
                    select dx.SoTientt + dx.SoTienph + dx.SoTienson).Sum()
            };
                if (!string.IsNullOrEmpty(parameters.SoHsgd))
                {
                    result = result.Where(x => x.SoHsgd.Contains(parameters.SoHsgd.Trim()));
                }
                if (parameters.SoSeri != null)
                {
                    result = result.Where(x => x.SoSeri == parameters.SoSeri);
                }
                if (!string.IsNullOrEmpty(parameters.BienKsoat))
                {
                    result = result.Where(x => x.BienKsoat.Contains(parameters.BienKsoat.Trim()));
                }
                if (!string.IsNullOrEmpty(parameters.SoDonBh))
                {
                    result = result.Where(x => x.SoDonBh.Contains(parameters.SoDonBh.Trim()));
                }
                if (!string.IsNullOrEmpty(parameters.MaTtrangGd))
                {
                    result = result.Where(x => x.MaTtrangGd == parameters.MaTtrangGd);
                }
                if (parameters.ChuaSangPias)
                {
                    result = result.Where(x => x.PrKeyBt == 0 && x.MaTtrangGd != "7");
                }

                if (parameters.SoTienugd != null)
                {
                    result = result.Where(x => x.SoTienugd == parameters.SoTienugd);
                }
                if (!string.IsNullOrEmpty(parameters.TenDonVi))
                {
                    result = result.Where(x => x.TenDonvi.Contains(parameters.TenDonVi));
                }
                if (!string.IsNullOrEmpty(parameters.TenKhach))
                {
                    result = result.Where(x => x.TenKhach.Contains(parameters.TenKhach));
                }
                if (!string.IsNullOrEmpty(parameters.TenLhsbt))
                {
                    result = result.Where(x => x.TenLhsbt.Contains(parameters.TenLhsbt));
                }
                if (!string.IsNullOrEmpty(parameters.NgayTthat))
                {
                    DateTime ngayTthat = DateTime.ParseExact(parameters.NgayTthat, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    result = result.Where(x => x.NgayTthat.Value.Date == ngayTthat);
                }
                if (!string.IsNullOrEmpty(parameters.NgayCtu))
                {
                    DateTime ngayCtu = DateTime.ParseExact(parameters.NgayCtu, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    result = result.Where(x => x.NgayCtu.Value.Date == ngayCtu);
                }
                if (userProfile.MaDonvi == "00" ||
                userProfile.MaDonvi == "31" ||
                userProfile.MaDonvi == "32")
                {
                    string[] maDonviPquyenList = Array.Empty<string>();
                    if (string.IsNullOrEmpty(userProfile.MaDonviPquyen))
                    {
                        result = result.Where(a => a.MaDonvi == userProfile.MaDonvi || a.MaDonviTt == userProfile.MaDonvi);
                    }
                    else
                    {
                        maDonviPquyenList = userProfile.MaDonviPquyen.Split(',');
                        if (userProfile.MaDonvi == "00")
                        {
                            result = result.Where(a => a.MaDonviGd == "00" || a.MaDonviGd == "31" || a.MaDonviGd == "32" || maDonviPquyenList.Contains(a.MaDonvi) || maDonviPquyenList.Contains(a.MaDonviTt));
                        }
                        else if (userProfile.MaDonvi == "31")
                        {
                            result = result.Where(a => a.MaDonviGd == "31" || maDonviPquyenList.Contains(a.MaDonvi) || maDonviPquyenList.Contains(a.MaDonviTt));
                        }
                        else if (userProfile.MaDonvi == "32")
                        {
                            result = result.Where(a => a.MaDonviGd == "32" || maDonviPquyenList.Contains(a.MaDonvi) || maDonviPquyenList.Contains(a.MaDonviTt));
                        }
                    }


                }
                else
                {
                    if (string.IsNullOrEmpty(userProfile.MaDonviPquyen))
                    {
                        result = result.Where(a => a.MaDonvi == userProfile.MaDonvi || a.MaDonviTt == userProfile.MaDonvi);
                    }
                    else
                    {
                        // Apply the filtering based on Session["Ma_dvi_pquyen"]
                        var maDonviPquyenList = userProfile.MaDonviPquyen.Split(',');

                        // Filter the result based on the MaDonviPquyen list
                        result = result.Where(a => maDonviPquyenList.Contains(a.MaDonvi) || maDonviPquyenList.Contains(a.MaDonviTt));
                    }
                }
                if (!string.IsNullOrEmpty(parameters.MaGDV))
                {
                    result = result.Where(x => !string.IsNullOrEmpty(x.MaUser) && x.MaUser.ToLower().Contains(parameters.MaGDV.ToLower()));
                }

                // Apply additional in-memory filtering
                if (!string.IsNullOrEmpty(parameters.NguoiXuly))
                {
                    var resultList = result.ToList();

                    var resultList2 = resultList.Where(x => x.NguoiXuly.Contains(parameters.NguoiXuly)).ToList();
                    var filteredResult = resultList2.AsQueryable();
                    return PagedList<HsgdCtuResponse>.ToPagedList(filteredResult, parameters.pageNumber, parameters.pageSize);
                }

                return await PagedList<HsgdCtuResponse>.ToPagedListAsync(result, parameters.pageNumber, parameters.pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        // Lấy danh sách nhật ký.
        // khanhlh - 08/09/2024
        public async Task<PagedList<NhatKy>> getListDiary(int pr_key, int pageNumber, int pageSize)
        {
            var result = (from nk in _context.NhatKies
                          join user in _context.DmUsers on nk.MaUser equals user.Oid

                          where nk.FrKey == pr_key
                          orderby nk.PrKey descending
                          select new NhatKy
                          {
                              PrKey = nk.PrKey,
                              FrKey = nk.FrKey,
                              MaTtrangGd = nk.MaTtrangGd,
                              TenTtrangGd = nk.TenTtrangGd,
                              GhiChu = nk.GhiChu,
                              NgayCapnhat = nk.NgayCapnhat,
                              MaUser = nk.MaUser,
                              TenUser = user.TenUser
                          }).AsQueryable();

            return await PagedList<NhatKy>.ToPagedListAsync(result, pageNumber, pageSize);
        }


        // Lấy danh sách các cán bộ tạo hồ sơ và kiểm duyệt, sẽ được trả về cùng với nhật ký.
        // khanhlh - 08/00/2024
        public async Task<Dictionary<string, DmUserView>> getListRelatedUsers(int pr_key)
        {
            try
            {
                Dictionary<string, DmUserView> userList = new Dictionary<string, DmUserView>();

                // Cán bộ tạo hồ sơ 
                // Cán bộ tạo hồ sơ lấy trên nhật ký lấy ông gần nhất
                var canBoTaoHoSo = await (from ctu in _context.HsgdCtus
                                          join nky in _context.NhatKies on ctu.PrKey equals nky.FrKey
                                          join user in _context.DmUsers on nky.MaUser equals user.Oid
                                          where (ctu.PrKey == pr_key) && (nky.MaTtrangGd == "1" || nky.MaTtrangGd == "2" || nky.MaTtrangGd == "9")
                                          orderby nky.PrKey ascending
                                          select new DmUserView
                                          {
                                              Oid = nky.MaUser,
                                              TenUser = user.TenUser,
                                              MaUser = user.MaUser,
                                              LoaiUser = user.LoaiUser,
                                          }).FirstOrDefaultAsync();

                if (canBoTaoHoSo != null)
                {
                    userList.Add("canBoTaoHoSo", canBoTaoHoSo);
                }

                // Kiểm duyệt viên 
                // giám định viên luôn theo ma_user trong hsgd_ctu
                var kiemDuyetVien = await (from ctu in _context.HsgdCtus
                                           join user in _context.DmUsers on ctu.MaUser equals user.Oid
                                           where ctu.PrKey == pr_key
                                           select new DmUserView
                                           {
                                               Oid = ctu.MaUser,
                                               TenUser = user.TenUser,
                                               MaUser = user.MaUser,
                                               LoaiUser = user.LoaiUser,
                                           }).FirstOrDefaultAsync();

                //var kiemDuyetVien = await (
                //                        from ctu in _context.HsgdCtus
                //                        from user in _context.DmUsers
                //                        where ctu.PrKey == pr_key
                //                              && (
                //                                    (!string.IsNullOrEmpty(ctu.NguoiXuly) && ctu.NguoiXuly == user.Oid.ToString())
                //                                    || (string.IsNullOrEmpty(ctu.NguoiXuly) && ctu.MaUser == user.Oid)
                //                                 )
                //                        select new DmUserView
                //                        {
                //                            Oid = !string.IsNullOrEmpty(ctu.NguoiXuly) ? Guid.Parse(ctu.NguoiXuly): ctu.MaUser,
                //                            TenUser = user.TenUser,
                //                            MaUser = user.MaUser,
                //                            LoaiUser = user.LoaiUser
                //                        }
                //                    ).FirstOrDefaultAsync();

                if (kiemDuyetVien != null)
                {
                    userList.Add("kiemDuyetVien", kiemDuyetVien);
                }

                // Cán bộ trung tâm
                //var canBoTT = await (from ctu in _context.HsgdCtus
                //                     join user in _context.DmUsers on ctu.NguoiXuly equals user.Oid.ToString()
                //                     where ctu.PrKey == pr_key
                //                     orderby ctu.MaTtrangGd ascending
                //                     select new DmUserView
                //                     {
                //                         Oid = new Guid(ctu.NguoiXuly),
                //                         TenUser = user.TenUser,
                //                         MaUser = user.MaUser,
                //                         LoaiUser = user.LoaiUser,
                //                     }).FirstOrDefaultAsync();

                //if (canBoTT != null)
                //{
                //    userList.Add("canBoPD", canBoTT);
                //}

                //// Cán bộ phê duyệt
                //var canBoPheDuyet = await (from lsu in _context.HsgdLsus
                //                           join user in _context.DmUsers on lsu.MaUserNhan equals user.Oid.ToString()
                //                           where lsu.FrKey == pr_key
                //                           orderby lsu.PrKey descending
                //                           select new DmUserView
                //                           {
                //                               Oid = Guid.Parse(lsu.MaUserNhan),
                //                               TenUser = user.TenUser,
                //                               MaUser = user.MaUser,
                //                               LoaiUser = user.LoaiUser,
                //                           }).FirstOrDefaultAsync();

                //if (canBoPheDuyet != null)
                //{

                //    userList.Remove("canBoPD");
                //    userList.Add("canBoPD", canBoPheDuyet);
                //}
                // Cán bộ phê duyệt
                var canBoPheDuyet = await (from ctu in _context.HsgdCtus
                       join nky in _context.NhatKies on ctu.PrKey equals nky.FrKey
                       join user in _context.DmUsers on nky.MaUser equals user.Oid
                       where (ctu.PrKey == pr_key) && (nky.MaTtrangGd == "6")
                       orderby nky.PrKey descending
                       select new DmUserView
                       {
                           Oid = nky.MaUser,
                           TenUser = user.TenUser,
                           MaUser = user.MaUser,
                           LoaiUser = user.LoaiUser,
                       }).FirstOrDefaultAsync();
                if (canBoPheDuyet != null)
                {
                    userList.Add("canBoPD", canBoPheDuyet);
                }

                // Cán bộ ngoài phân cấp
                var canBoNgoaiPhanCap = await (from ctu in _context.HsgdCtus
                                               join nky in _context.NhatKies on ctu.PrKey equals nky.FrKey
                                               join user in _context.DmUsers on nky.MaUser equals user.Oid
                                               where (ctu.PrKey == pr_key) && (nky.MaTtrangGd == "12")
                                               orderby ctu.MaTtrangGd ascending
                                               select new DmUserView
                                               {
                                                   Oid = nky.MaUser,
                                                   TenUser = user.TenUser,
                                                   MaUser = user.MaUser,
                                                   LoaiUser = user.LoaiUser,
                                               }).FirstOrDefaultAsync();

                if (canBoNgoaiPhanCap != null)
                {
                    userList.Add("CanBoNgoaiPhanCap", canBoNgoaiPhanCap);
                }

                return userList;

            }
            catch (Exception err)
            {
                Console.WriteLine(err);
                return null;
            }
        }

        // Tất cả các đầu API Get để lấy thông tin cần thiết.
        // khanhlh - 19/09/2024
        // Bắt đầu từ đây:

        // Nguyên nhân tổn thất.
        public Task<List<DanhMuc>> GetListNguyenNhanTonThat()
        {
            var list_nntt = (from k in _context.DmNguyennhanTonthats
                             select new DanhMuc
                             {
                                 MaDM = k.MaNntt,
                                 TenDM = k.TenNntt
                             }
                       ).AsQueryable();
            return ToListWithNoLockAsync(list_nntt);
        }

        // Địa điểm tổn thất.
        public Task<List<DanhMucTinh>> GetListDiaDiemTonThat()
        {
            var list_ddtt = (from k in _context.DmTinhs
                             select new DanhMucTinh
                             {
                                 MaDM = k.MaTinh,
                                 TenDM = k.TenTinh,
                                 SuDung=k.SuDung
                             }
                       ).AsQueryable();
            return ToListWithNoLockAsync(list_ddtt);
        }
        public Task<List<DanhMuc>> GetListSanPham()
        {
            var list_sp = (from k in _context_pias.DmSps
                           where k.MaNsp1 == "0501"
                           select new DanhMuc
                           {
                               MaDM = k.MaSp,
                               TenDM = k.TenSp
                           }
                       ).AsQueryable();
            return ToListWithNoLockAsync(list_sp);
        }

        // List trạng thái.
        public Task<List<DanhMuc>> GetListTrangThai()
        {
            var list_tt = (from k in _context.DmTtrangTtrinhs
                           select new DanhMuc
                           {
                               MaDM = k.MaTtrangTt,
                               TenDM = k.TenTtrangTt
                           }
                       ).AsQueryable();
            return ToListWithNoLockAsync(list_tt);
        }

        // Lấy tên và danh sách các tình trạng hồ sơ.
        public Task<List<DmTtrangGd>> GetListStatusName()
        {
            var list_tt = (from k in _context.DmTtrangGds
                           select new DmTtrangGd
                           {
                               MaTtrangGd = k.MaTtrangGd,
                               TenTtrangGd = k.TenTtrangGd,
                           }
                       ).AsQueryable();
            return ToListWithNoLockAsync(list_tt);
        }

        // Lấy tên các loại hồ sơ.
        public Task<List<DmLoaiHsgd>> GetListTypeName()
        {
            var list_tt = (from k in _context.DmLoaiHsgds
                           select new DmLoaiHsgd
                           {
                               ma_loai_hsgd = k.ma_loai_hsgd,
                               ten_loai_hsgd = k.ten_loai_hsgd
                           }
                       ).AsQueryable();
            return ToListWithNoLockAsync(list_tt);
        }


        public List<DmTongthanhxe> getListTongThanhXe()
        {
            return _context.DmTongthanhxes.ToList();
        }
        public List<DmNhmuc> getListNhmuc()
        {
            return _context.DmNhmucs.ToList();
        }

        public List<DmHmuc> getListHmuc(string? ma_tongthanhxe, string? ma_nhmuc)
        {
            List<DmHmuc> listHmuc = (from hmuc in _context.DmHmucs
                                     join nhmuc in _context.DmNhmucs on hmuc.MaNhmuc equals nhmuc.MaNhmuc
                                     join ttx in _context.DmTongthanhxes on hmuc.MaTongthanhxe equals ttx.MaTongthanhxe
                                     where
                                        (ma_tongthanhxe != null ? hmuc.MaTongthanhxe.Contains(ma_tongthanhxe) : true) &&
                                        (ma_nhmuc != null ? hmuc.MaNhmuc.Contains(ma_nhmuc) : true)
                                     select new DmHmuc
                                     {
                                         MaHmuc = hmuc.MaHmuc,
                                         TenHmuc = hmuc.TenHmuc,
                                         MaNhmuc = hmuc.MaNhmuc,
                                         TenNhmuc = nhmuc.TenNhmuc,
                                         MaTongthanhxe = hmuc.MaTongthanhxe,
                                         TenTongThanhXe = ttx.TenTongthanhxe,
                                         SuDung = hmuc.SuDung,
                                         NgayCapnhat = hmuc.NgayCapnhat,
                                         MaUser = hmuc.MaUser
                                     }
                                     ).ToList();
            return listHmuc;
        }

        public List<DmHmucGiamdinh> getListHmucGiamDinh()
        {
            return _context.DmHmucGiamdinhs.ToList();
        }

        public List<DmTtrangGd> getListTtrangGd()
        {
            return _context.DmTtrangGds.ToList();
        }
        public List<DmTte> getListTienTe()
        {
            return _context_pias.DmTtes.ToList();
        }

        public List<DmTyGia> getListTyGia()
        {
            List<DmTte> list_tien_te = getListTienTe();

            List<DmTyGia> list_ty_gia = new List<DmTyGia>();

            list_tien_te.ForEach(x =>
            {
                DmTyGia tygia = _context_pias.DmTyGias.Where(tg => tg.MaTTe.Equals(x.MaTte)).OrderByDescending(ls => ls.PrKey).FirstOrDefault();
                list_ty_gia.Add(tygia);
            });

            return list_ty_gia;
        }

        public List<DmHieuxe> getListHieuxe()
        {
            return _context.DmHieuxes.Where(x => !x.HieuXe.Equals("")).ToList();
        }
        public List<DmLoaixe> getListLoaixe(int pr_key_hieu_xe)
        {
            return _context.DmLoaixes.Where(x => x.FrKey == pr_key_hieu_xe).ToList();
        }

        public List<DmLoaiHinhTd> getListLoaiChiPhi()
        {
            return _context_pias.DmLoaiHinhTds.ToList();
        }
        //
        // Kết thúc phần get API - khanhlh.
        //




        // Lấy Data.
        public async Task<HsgdCtuDetail> GetData_Detail_Hsgd(decimal pr_key)
        {
            HsgdCtuDetail? objResult = new HsgdCtuDetail();
            try
            {
                objResult = await (from k in _context.HsgdCtus
                                   join donvi in _context.DmDonvis on k.MaDonvi equals donvi.MaDonvi
                                   where (k.PrKey == pr_key)
                                   select new HsgdCtuDetail
                                   {
                                       PrKey = k.PrKey,
                                       MaDonvi = k.MaDonvi != null ? k.MaDonvi : "",
                                       SoHsgd = k.SoHsgd,
                                       SoDonbh = k.SoDonbh,
                                       SoLanBt = k.SoLanBt,
                                       TenKhach = k.TenKhach,
                                       NgayDauSeri = k.NgayDauSeri != null ? Convert.ToDateTime(k.NgayDauSeri).ToString("dd/MM/yyyy") : null,
                                       NgayCuoiSeri = k.NgayCuoiSeri != null ? Convert.ToDateTime(k.NgayCuoiSeri).ToString("dd/MM/yyyy") : null,
                                       SoSeri = k.SoSeri,
                                       NgGdichTh = k.NgGdichTh != null ? k.NgGdichTh : "",
                                       BienKsoat = k.BienKsoat != null ? k.BienKsoat : "",
                                       SoTienBaoHiem = k.SoTienBaoHiem,
                                       SoTienThucTe = k.SoTienThucTe,
                                       NgayCtu = k.NgayCtu != null ? Convert.ToDateTime(k.NgayCtu).ToString("dd/MM/yyyy HH:mm") : null,
                                       NgayTbao = k.NgayTbao != null ? Convert.ToDateTime(k.NgayTbao).ToString("dd/MM/yyyy HH:mm") : null,
                                       NgayTthat = k.NgayTthat != null ? Convert.ToDateTime(k.NgayTthat).ToString("dd/MM/yyyy HH:mm") : null,
                                       MaTtrangGd = k.MaTtrangGd != null ? k.MaTtrangGd : "",
                                       MaLhsbt = k.MaLhsbt,
                                       HsgdTpc = k.HsgdTpc,
                                       MaDdiemTthat = k.MaDdiemTthat,
                                       DiaDiemtt = k.DiaDiemtt,
                                       NguyenNhanTtat = k.NguyenNhanTtat,
                                       MaNguyenNhanTtat = k.MaNguyenNhanTtat,
                                       TenLaixe = k.TenLaixe,
                                       DienThoai = k.DienThoai,
                                       DienThoaiNdbh = k.DienThoaiNdbh,
                                       GhiChu = k.GhiChu,
                                       NgayGdinh = Convert.ToDateTime(k.NgayGdinh).ToString("dd/MM/yyyy HH:mm"),
                                       DiaDiemgd = k.DiaDiemgd,
                                       PrKeyBt = k.PrKeyBt,
                                       PrKeyGoc = k.PrKeyGoc,
                                       NgayThuphi = k.NgayThuphi,
                                       ChkDaydu = k.ChkDaydu,
                                       ChkDunghan = k.ChkDunghan,
                                       ChkTheohopdong = k.ChkTheohopdong,
                                       NamSinh = k.NamSinh,
                                       SoGphepLaixe = k.SoGphepLaixe,
                                       NgayDauLaixe = k.NgayDauLaixe != null ? Convert.ToDateTime(k.NgayDauLaixe).ToString("dd/MM/yyyy") : "",
                                       NgayCuoiLaixe = k.NgayCuoiLaixe != null ? Convert.ToDateTime(k.NgayCuoiLaixe).ToString("dd/MM/yyyy") : "",
                                       MaLoaibang = k.MaLoaibang,
                                       SoGphepLuuhanh = k.SoGphepLuuhanh,
                                       NgayDauLuuhanh = k.NgayDauLuuhanh != null ? Convert.ToDateTime(k.NgayDauLuuhanh).ToString("dd/MM/yyyy") : "",
                                       NgayCuoiLuuhanh = k.NgayCuoiLuuhanh != null ? Convert.ToDateTime(k.NgayCuoiLuuhanh).ToString("dd/MM/yyyy") : "",
                                       HosoPhaply = k.HosoPhaply,
                                       YkienGdinh = k.YkienGdinh,
                                       DexuatPan = k.DexuatPan,
                                       DangKiem = k.DangKiem,
                                       MaDonviTt = k.MaDonviTt,
                                       HauQua = k.HauQua,
                                       ThieuAnh = k.ThieuAnh,
                                       ChuaThuPhi = k.ChuaThuphi,
                                       SaiDKDK = k.SaiDkdk,
                                       SaiPhanCap = k.SaiPhancap,
                                       TrucLoiBH = k.TrucloiBh,
                                       SaiPhamKhac = k.SaiphamKhac,
                                       LoaiTotrinhTpc = k.LoaiTotrinhTpc,
                                       Tpc = false,
                                       PathTotrinhTpc = k.PathTotrinhTpc,
                                       SoTienugd = k.SoTienugd,
                                       SoHsbt=k.SoHsbt
                                   }).FirstOrDefaultAsync();
                if (objResult != null)
                {
                    decimal pr_key_goc = 0;
                    var namTraCuu = 0;
                    //objResult.ThinhNphi = await _context_pias.HsbtCtus.Where(x => x.PrKey == objResult.PrKeyBt).Select(x=>x.ThinhNphi).FirstOrDefaultAsync();
                    //objResult.SoHsbt = await _context_pias_update.HsbtCtus.Where(x => x.PrKey == objResult.PrKeyBt).Select(x => x.SoHsbt).FirstOrDefaultAsync();
                    var dataNvuBhtCtu = await _context_pias.NvuBhtCtus
    .Where(x => x.SoDonbhSdbs == objResult.SoDonbh)
    .Select(x => new { x.PrKey, x.NgayCtu })
    .FirstOrDefaultAsync();


                    if (dataNvuBhtCtu != null)
                    {
                        pr_key_goc = dataNvuBhtCtu.PrKey;
                        namTraCuu = dataNvuBhtCtu.NgayCtu.GetValueOrDefault().Year;


                    }
                    objResult.NamTraCuu = namTraCuu;
                    if (pr_key_goc != 0)
                    {
                        objResult.nvuBhtKyphis = await _context_pias.NvuBhtKyphis.Where(x => x.FrKey == pr_key_goc).Select(s => new NvuBhtKyphiView
                        {
                            PrKey = s.PrKey,
                            FrKey = s.FrKey,
                            Stt = s.Stt,
                            NgayHl = s.NgayHl != null ? Convert.ToDateTime(s.NgayHl).ToString("dd/MM/yyyy") : null,
                            TylePhithu = s.TylePhithu,
                            SoTien = s.SoTien
                        }).OrderBy(o => o.Stt).ToListAsync();
                    }
                    objResult.TenDonvi = await _context.DmDonvis.Where(x => x.MaDonvi == objResult.MaDonvi).Select(s => s.TenDonvi).FirstOrDefaultAsync();
                    //check lại số tiền thực tế
                    if (objResult.SoTienThucTe == 0)
                    {
                        var check_seri = Get_SoPhiBH(objResult.SoDonbh, objResult.SoSeri);
                        if (check_seri != null)
                        {
                            objResult.SoTienThucTe = check_seri.GiaTri_Tte;
                        }
                    }
                    //check lại số tiền bảo hiểm
                    if (objResult.SoTienBaoHiem == 0)
                    {
                        var check_seri = Get_SoPhiBH(objResult.SoDonbh, objResult.SoSeri);
                        if (check_seri != null)
                        {
                            objResult.SoTienBaoHiem = check_seri.MtnGtbhVnd;
                        }
                    }
                    var hsgd_dx_ct = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCtu == objResult.PrKeyBt).ToList();
                    decimal sum_trachnhiempvi = 0;

                    if (hsgd_dx_ct.Count > 0)
                    {
                        for (int i = 0; i < hsgd_dx_ct.Count; i++)
                        {
                            var sum_dx = _dx_repo.ReloadSum(hsgd_dx_ct[i].PrKey);
                            if (sum_dx != null && sum_dx.Count > 0)
                            {
                                sum_trachnhiempvi += sum_dx[0].StBl ?? 0;
                            }
                        }
                    }
                    if (sum_trachnhiempvi >= 250000000)
                    {
                        objResult.Tpc = true;
                    }
                    ////nếu chưa có tên lái xe thì lấy từ bằng
                    //if (string.IsNullOrEmpty(objResult.TenLaixe))
                    //{
                    //    try
                    //    {
                    //        var PathOrginalFile_BANG = _context.HsgdCts.Where(x => x.FrKey == pr_key && x.MaHmuc == "BANG").Select(s => s.PathOrginalFile.Replace("\\", "/")).Take(2).ToList();
                    //        _logger.Information("GetData_Detail_Hsgd pr_key = " + pr_key + " gọi Driving_license có PathOrginalFile_BANG_List = " + JsonConvert.SerializeObject(PathOrginalFile_BANG));
                    //        if (PathOrginalFile_BANG.Count > 0)
                    //        {
                    //            foreach (var item_BANG in PathOrginalFile_BANG)
                    //            {
                    //                _logger.Information("GetData_Detail_Hsgd pr_key = " + pr_key + " gọi Driving_license có PathOrginalFile = " + item_BANG);
                    //                var bc_gd = Driving_license(item_BANG, pr_key);
                    //                _logger.Information("GetData_Detail_Hsgd pr_key = " + pr_key + " gọi Driving_license có PathOrginalFile = " + item_BANG + " kq bc_gd = " + JsonConvert.SerializeObject(bc_gd));
                    //                if (bc_gd != null)
                    //                {
                    //                    if (bc_gd.NamSinh > 0)
                    //                    {
                    //                        objResult.NamSinh = bc_gd.NamSinh;
                    //                    }
                    //                    if (!string.IsNullOrEmpty(bc_gd.SoGphepLaixe))
                    //                    {
                    //                        objResult.SoGphepLaixe = bc_gd.SoGphepLaixe;
                    //                    }
                    //                    if (bc_gd.NgayDauLaixe != null)
                    //                    {
                    //                        objResult.NgayDauLaixe = Convert.ToDateTime(bc_gd.NgayDauLaixe).ToString("dd/MM/yyyy");
                    //                    }
                    //                    if (bc_gd.NgayCuoiLaixe != null)
                    //                    {
                    //                        objResult.NgayCuoiLaixe = Convert.ToDateTime(bc_gd.NgayCuoiLaixe).ToString("dd/MM/yyyy");
                    //                    }
                    //                    if (!string.IsNullOrEmpty(bc_gd.MaLoaibang))
                    //                    {
                    //                        objResult.MaLoaibang = bc_gd.MaLoaibang;
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        _logger.Information("Driving_license pr_key_hsgd_ctu =" + pr_key + " error: " + ex);
                    //    }

                    //}
                    ////nếu chưa có số giấy phép lưu hành thì lấy từ đăng kiểm
                    //if (string.IsNullOrEmpty(objResult.SoGphepLuuhanh))
                    //{
                    //    try
                    //    {
                    //        var PathOrginalFile_DKIE = _context.HsgdCts.Where(x => x.FrKey == pr_key && x.MaHmuc == "DKIE").Select(s => s.PathOrginalFile.Replace("\\", "/")).FirstOrDefault();
                    //        if (PathOrginalFile_DKIE != null)
                    //        {
                    //            _logger.Information("GetData_Detail_Hsgd pr_key = " + pr_key + " gọi Vehicle_Inspection có PathOrginalFile = " + PathOrginalFile_DKIE);
                    //            var bc_gd = Vehicle_Inspection(PathOrginalFile_DKIE, pr_key);
                    //            _logger.Information("GetData_Detail_Hsgd pr_key = " + pr_key + " gọi Vehicle_Inspection có PathOrginalFile = " + PathOrginalFile_DKIE + " kq bc_gd = " + JsonConvert.SerializeObject(bc_gd));
                    //            if (bc_gd != null)
                    //            {
                    //                if (!string.IsNullOrEmpty(bc_gd.SoGphepLuuhanh))
                    //                {
                    //                    objResult.SoGphepLuuhanh = bc_gd.SoGphepLuuhanh;
                    //                }
                    //                if (bc_gd.NgayDauLuuhanh != null)
                    //                {
                    //                    objResult.NgayDauLuuhanh = Convert.ToDateTime(bc_gd.NgayDauLuuhanh).ToString("dd/MM/yyyy");
                    //                }
                    //                if (bc_gd.NgayCuoiLuuhanh != null)
                    //                {
                    //                    objResult.NgayCuoiLuuhanh = Convert.ToDateTime(bc_gd.NgayCuoiLuuhanh).ToString("dd/MM/yyyy");
                    //                }
                    //            }
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        _logger.Information("Vehicle_Inspection pr_key_hsgd_ctu =" + pr_key + " error: " + ex);
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
            }
            return objResult;
        }

        public async Task<decimal> GetPrKeyBySoHsgd(string soHsgd)
        {
            if (string.IsNullOrEmpty(soHsgd))
            {
                return 0;
            }
            else
            {
                var prKey = await _context.HsgdCtus.Where(x => x.SoHsgd == soHsgd).Select(x => x.PrKey).FirstOrDefaultAsync();
                return prKey;
            }
        }
        public BCGiamDinh ReadOCR(decimal pr_key_hsgd)
        {
            BCGiamDinh bcgd = new BCGiamDinh();
            try
            {
                var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == pr_key_hsgd).FirstOrDefault();

                if (hsgd_ctu != null)
                {
                    //nếu chưa có tên lái xe thì lấy từ bằng
                    if (string.IsNullOrEmpty(hsgd_ctu.TenLaixe))
                    {
                        try
                        {
                            var PathOrginalFile_BANG = _context.HsgdCts.Where(x => x.FrKey == pr_key_hsgd && x.MaHmuc == "BANG").Select(s => s.PathOrginalFile.Replace("\\", "/")).Take(2).ToList();
                            _logger.Information("GetData_Detail_Hsgd pr_key = " + pr_key_hsgd + " gọi Driving_license có PathOrginalFile_BANG_List = " + JsonConvert.SerializeObject(PathOrginalFile_BANG));
                            if (PathOrginalFile_BANG.Count > 0)
                            {
                                foreach (var item_BANG in PathOrginalFile_BANG)
                                {
                                    _logger.Information("GetData_Detail_Hsgd pr_key = " + pr_key_hsgd + " gọi Driving_license có PathOrginalFile = " + item_BANG);
                                    var bang = Driving_license(item_BANG, pr_key_hsgd);
                                    _logger.Information("GetData_Detail_Hsgd pr_key = " + pr_key_hsgd + " gọi Driving_license có PathOrginalFile = " + item_BANG + " kq bc_gd = " + JsonConvert.SerializeObject(bang));
                                    if (bang != null)
                                    {
                                        if (!string.IsNullOrEmpty(bang.TenLaiXe))
                                        {
                                            bcgd.TenLaiXe = bang.TenLaiXe;
                                        }
                                        if (bang.NamSinh > 0)
                                        {
                                            bcgd.NamSinh = bang.NamSinh;
                                        }
                                        if (!string.IsNullOrEmpty(bang.SoGphepLaixe))
                                        {
                                            bcgd.SoGphepLaixe = bang.SoGphepLaixe;
                                        }
                                        if (bang.NgayDauLaixe != null)
                                        {
                                            bcgd.NgayDauLaixe = Convert.ToDateTime(bang.NgayDauLaixe).ToString("dd/MM/yyyy");
                                        }
                                        if (bang.NgayCuoiLaixe != null)
                                        {
                                            bcgd.NgayCuoiLaixe = Convert.ToDateTime(bang.NgayCuoiLaixe).ToString("dd/MM/yyyy");
                                        }
                                        if (!string.IsNullOrEmpty(bang.MaLoaibang))
                                        {
                                            bcgd.MaLoaibang = bang.MaLoaibang;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Information("Driving_license pr_key_hsgd_ctu =" + pr_key_hsgd + " error: " + ex);
                        }

                    }
                    //nếu chưa có số giấy phép lưu hành thì lấy từ đăng kiểm
                    if (string.IsNullOrEmpty(hsgd_ctu.SoGphepLuuhanh))
                    {
                        try
                        {
                            var PathOrginalFile_DKIE = _context.HsgdCts.Where(x => x.FrKey == pr_key_hsgd && x.MaHmuc == "DKIE").Select(s => s.PathOrginalFile.Replace("\\", "/")).FirstOrDefault();
                            if (PathOrginalFile_DKIE != null)
                            {
                                _logger.Information("GetData_Detail_Hsgd pr_key = " + pr_key_hsgd + " gọi Vehicle_Inspection có PathOrginalFile = " + PathOrginalFile_DKIE);
                                var dangkiem = Vehicle_Inspection(PathOrginalFile_DKIE, pr_key_hsgd);
                                _logger.Information("GetData_Detail_Hsgd pr_key = " + pr_key_hsgd + " gọi Vehicle_Inspection có PathOrginalFile = " + PathOrginalFile_DKIE + " kq bc_gd = " + JsonConvert.SerializeObject(dangkiem));
                                if (dangkiem != null)
                                {
                                    if (!string.IsNullOrEmpty(dangkiem.SoGphepLuuhanh))
                                    {
                                        bcgd.SoGphepLuuhanh = dangkiem.SoGphepLuuhanh;
                                    }
                                    if (dangkiem.NgayDauLuuhanh != null)
                                    {
                                        bcgd.NgayDauLuuhanh = Convert.ToDateTime(dangkiem.NgayDauLuuhanh).ToString("dd/MM/yyyy");
                                    }
                                    if (dangkiem.NgayCuoiLuuhanh != null)
                                    {
                                        bcgd.NgayCuoiLuuhanh = Convert.ToDateTime(dangkiem.NgayCuoiLuuhanh).ToString("dd/MM/yyyy");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Information("Vehicle_Inspection pr_key_hsgd_ctu =" + pr_key_hsgd + " error: " + ex);
                        }
                    }
                    _logger.Information("GetData_Detail_Hsgd pr_key = " + pr_key_hsgd + " return bc_gd = " + JsonConvert.SerializeObject(bcgd));
                }
            }
            catch (Exception ex)
            {
            }
            return bcgd;
        }
        public string updateFile(HsgdCtu hoSoUpdate)
        {
            var result = "";
            using var context_gdtt_new = new GdttContext();
            using var dbContextTransaction = context_gdtt_new.Database.BeginTransaction();

            using var context_pias_new = new Pvs2024UpdateContext();
            using var dbContextTransaction2 = context_pias_new.Database.BeginTransaction();
            try
            {
                context_gdtt_new.HsgdCtus.Update(hoSoUpdate);
                // update hsbt_ctu
                if (hoSoUpdate.PrKeyBt > 0)
                {
                    var hsbt_ctu = context_pias_new.HsbtCtus.Where(x => x.PrKey == hoSoUpdate.PrKeyBt).FirstOrDefault();
                    if (hsbt_ctu != null)
                    {
                        //hsbt_ctu.NgayCtu = DateTime.Today;
                        hsbt_ctu.NgayTthat = hoSoUpdate.NgayTthat != null ? hoSoUpdate.NgayTthat.Value.Date : null;
                        hsbt_ctu.NgayTbao = hoSoUpdate.NgayTbao != null ? hoSoUpdate.NgayTbao.Value.Date : null;
                        hsbt_ctu.DiaChi = hoSoUpdate.DiaChi;
                        hsbt_ctu.NguyenNhanTtat = hoSoUpdate.NguyenNhanTtat;
                        hsbt_ctu.GhiChu = hoSoUpdate.GhiChu;
                        hsbt_ctu.NguyenNhan = hoSoUpdate.NguyenNhanTtat;
                        hsbt_ctu.DiaDiem = hoSoUpdate.DiaDiemtt;
                        hsbt_ctu.MaDdiemTthat = hoSoUpdate.MaDdiemTthat;
                        hsbt_ctu.TenKhle = hoSoUpdate.TenKhach;
                        hsbt_ctu.NamSinh = hoSoUpdate.NamSinh;
                        hsbt_ctu.MaLhsbt = hoSoUpdate.MaLhsbt == "1" ? "TBT" : (hoSoUpdate.MaLhsbt == "2" ? "NBT" : "BTH");
                        hsbt_ctu.HosoPhaply = hoSoUpdate.HosoPhaply;
                        hsbt_ctu.YkienGdinh = hoSoUpdate.YkienGdinh;
                        hsbt_ctu.DexuatPan = hoSoUpdate.DexuatPan;
                        hsbt_ctu.TenLaixe = hoSoUpdate.TenLaixe;
                        hsbt_ctu.SoGphepLaixe = hoSoUpdate.SoGphepLaixe;
                        hsbt_ctu.NgayDauLaixe = hoSoUpdate.NgayDauLaixe;
                        hsbt_ctu.NgayCuoiLaixe = hoSoUpdate.NgayCuoiLaixe;
                        hsbt_ctu.SoGphepLuuhanh = hoSoUpdate.SoGphepLuuhanh;
                        hsbt_ctu.NgayDauLuuhanh = hoSoUpdate.NgayDauLuuhanh;
                        hsbt_ctu.NgayCuoiLuuhanh = hoSoUpdate.NgayCuoiLuuhanh;
                        hsbt_ctu.NgdcBh = hoSoUpdate.TenKhach;
                        hsbt_ctu.DienThoai = hoSoUpdate.DienThoai;
                        hsbt_ctu.NgayGdinh = hoSoUpdate.NgayGdinh;
                        hsbt_ctu.MaLoaibang = hoSoUpdate.MaLoaibang;
                        hsbt_ctu.MaLoaixe = hoSoUpdate.MaNhloaixe;
                        hsbt_ctu.MaDonviTt = hoSoUpdate.MaDonviTt;
                        context_pias_new.HsbtCtus.Update(hsbt_ctu);
                    }
                }
                context_pias_new.SaveChanges();
                dbContextTransaction2.Commit();

                context_gdtt_new.SaveChanges();
                dbContextTransaction.Commit();

                result = hoSoUpdate.PrKey.ToString();
            }
            catch (Exception ex)
            {
                result = "0";
                _logger.Error("dbContextTransaction Exception when updateFile: " + ex.ToString());
                _logger.Error("Error record: " + JsonConvert.SerializeObject(hoSoUpdate));
                dbContextTransaction2.Rollback();
                dbContextTransaction2.Dispose();
                dbContextTransaction.Rollback();
                dbContextTransaction.Dispose();
            }
            return result;
            //try
            //{
            //    _context.HsgdCtus.Update(hoSoUpdate);
            //    _context.SaveChanges();
            //    return hoSoUpdate.PrKey.ToString();
            //}
            //catch (Exception ex)
            //{
            //    _logger.Error("dbContextTransaction Exception when updateFile: " + ex.ToString());
            //    _logger.Error("Error record: " + JsonConvert.SerializeObject(hoSoUpdate));
            //    _context.Dispose();
            //    throw;
            //}
        }
        public async Task<List<ImageResponse>> GetListAppraisalImage(int pr_key)
        {
            try
            {
                var listImage = await (from hsgd_ct in _context.HsgdCts
                                       join gds in _context.DmHmucGiamdinhs
                                       on hsgd_ct.MaHmuc equals gds.MaHmuc into gdsGroup
                                       from gds in gdsGroup.DefaultIfEmpty()
                                       join gdscs in _context.DmHmucs on hsgd_ct.MaHmucSc equals gdscs.MaHmuc into gdscsGroup
                                       from gdscs in gdscsGroup.DefaultIfEmpty()
                                       where hsgd_ct.FrKey == pr_key
                                       select new ImageResponse
                                       {
                                           PrKey = hsgd_ct.PrKey,
                                           FrKey = hsgd_ct.FrKey,
                                           Stt = hsgd_ct.Stt,
                                           NhomAnh = hsgd_ct.NhomAnh,
                                           PathFile = hsgd_ct.PathFile.Replace("\\", "/"),
                                           NgayChup = hsgd_ct.NgayChup,
                                           ViDoChup = hsgd_ct.ViDoChup,
                                           KinhDoChup = hsgd_ct.KinhDoChup,
                                           DienGiai = hsgd_ct.DienGiai,
                                           PathUrl = hsgd_ct.PathUrl.Replace("https://pvi247.pvi.com.vn", "https://cdn247.pvi.com.vn")
                                                                    .Replace("//pvi.com.vn/p247_upload_new", "https://cdn247.pvi.com.vn/upload_01"),
                                           PathOrginalFile = hsgd_ct.PathOrginalFile.Replace("\\", "/"),
                                           MaHmuc = hsgd_ct.MaHmuc,
                                           TenHmuc = gds != null ? gds.TenHmuc : null,
                                           TenHmucSc = gdscs != null ? gdscs.TenHmuc : null,
                                           MaNHmuc = gdscs != null ? gdscs.MaNhmuc : null,
                                           MaTongThanhXe = gdscs != null ? gdscs.MaTongthanhxe : null,
                                           MaHmucSc = gdscs != null ? gdscs.MaHmuc : null
                                       }).ToListAsync();

                if (listImage.Count > 0)
                {
                    var uniqueDirectories = new HashSet<string>();
                    foreach (var image in listImage)
                    {
                        var pathFile = Path.Combine(Path.GetDirectoryName(image.PathFile), "1.jpg");
                        string dir_source = Path.GetDirectoryName(pathFile) + "\\";
                        string dir_target = PheDuyetHelper.ModifyTargetDirectory(dir_source);

                        // Always update PathUrl with CSSK case
                        int dIndex = pathFile.IndexOf("CSSK_upload", StringComparison.OrdinalIgnoreCase);
                        if (dIndex > -1)
                        {
                            string baseUrl = "https://cdn247.pvi.com.vn/upload_01/";
                            string urlPath = baseUrl + dir_target.Replace(@"\\192.168.250.77\P247_Upload_New\", "").Replace("\\", "/") + Path.GetFileName(image.PathFile);
                            image.PathUrl = urlPath;
                        }
                        //Chạy lại những bộ cũ không map đúng đường dẫn để load ảnh: đổi từ \\pvi.com.vn\DATA\P247_upload-->\\pvi.com.vn\P247_upload_new
                        //update b
                        //set path_url = replace(path_url, 'http://pvi247.pvi.com.vn/upload', 'https://cdn247.pvi.com.vn/upload_01'),
                        //path_orginal_file = replace(path_orginal_file, '\\pvi.com.vn\DATA\P247_upload', '\\pvi.com.vn\P247_upload_new'), path_file = replace(path_file, '\\pvi.com.vn\DATA\P247_upload', '\\pvi.com.vn\P247_upload_new')
                        //from hsgd_ctu a inner join hsgd_ct b on a.pr_key=b.fr_key
                        //where so_hsgd in (
                        //'16/08/000763') 

                        if (!uniqueDirectories.Add(dir_source))
                            continue;

                        pathFile = UtilityHelper.CopyFile(dir_source, dir_target);
                    }
                    return listImage;
                }
                else
                {
                    return new List<ImageResponse>();
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occured: {ex}");
                return null;
            }
        }
     
        //private static string ModifyTargetDirectory(string dir_source)
        //{
        //    string dir_target = dir_source.Replace("pvi.com.vn", "192.168.250.77");

        //    if (dir_source.IndexOf("CSSK_upload", StringComparison.OrdinalIgnoreCase) > -1)
        //    {
        //        dir_target = dir_target.Replace("DATA\\", "P247_Upload_New\\")
        //                               .Replace("data\\", "P247_Upload_New\\")
        //                               .Replace("CSSK_upload\\", "TCD\\CLAIM_XCG\\")
        //                               .Replace("cssk_upload\\", "TCD\\CLAIM_XCG\\")
        //                               .Replace("\\pvi\\data\\GCNDT_Upload", "192.168.250.77\\P247_Upload_New");
        //    }
        //    else
        //    {
        //        dir_target = dir_target.Replace("\\DATA", "")
        //                               .Replace("P247_upload\\", "P247_Upload_New\\");
        //    }

        //    return dir_target;
        //}

        // Giao giám định - Gán giám định
        // khanhlh - 08/09/2024
        public async Task<string> assignAppraisal(int pr_key, string? oidGiamDV, string? oidCanBoXuLy, bool guiEmail, bool guiSMS, string currentUserEmail)
        {
            try
            {
                HsgdCtu hoSoGiamDinh = await _context.HsgdCtus.FirstOrDefaultAsync(x => x.PrKey == pr_key); // Tìm hồ sơ muốn gán

                if (hoSoGiamDinh != null)
                {
                    DmUser currentUser = await _context.DmUsers.FirstOrDefaultAsync(x => x.Mail.Equals(currentUserEmail));

                    if (currentUser == null)
                    {
                        return "User không có quyền thực hiện giao giám định.";
                    }
                    else
                    {
                        // Nếu là hồ sơ đang kích hoạt.
                        // Hiện đang cho reset hồ sơ đã chuyển chờ. Check lại luồng sau.
                        if (true)//hoSoGiamDinh.MaTtrangGd.Equals("1") || hoSoGiamDinh.MaTtrangGd.Equals("2") || hoSoGiamDinh.MaTtrangGd.Equals("3"))
                        {
                            hoSoGiamDinh.MaUser = new Guid(oidGiamDV);
                            if (oidCanBoXuLy != null)
                            {
                                hoSoGiamDinh.NguoiXuly = oidCanBoXuLy;
                            }
                            hoSoGiamDinh.MaTtrangGd = "9";
                            _context.HsgdCtus.Update(hoSoGiamDinh);

                            string ghiChu = " Giám định viên được giao: ";

                            string oidGiamDVFormat = oidGiamDV.ToLower();
                            DmUser giamDinhVien = await _context.DmUsers.FirstOrDefaultAsync(x => x.Oid.ToString().ToLower().Equals(oidGiamDVFormat));

                            if (giamDinhVien != null)
                            {
                                ghiChu += giamDinhVien.TenUser;

                                if (guiEmail)
                                {
                                    ghiChu += ". Đã gửi Email.";
                                    //PheDuyetHelper.SendEmail(0, giamDinhVien.Mail.Trim(), "Yêu cầu giám định tổn thất", hoSoGiamDinh.SoDonbh, hoSoGiamDinh.SoHsgd, "", currentUser.TenUser, hoSoGiamDinh.SoSeri.ToString(), hoSoGiamDinh.BienKsoat.ToString(), hoSoGiamDinh.TenKhach, hoSoGiamDinh.NgayDauSeri.ToString(), hoSoGiamDinh.NgayCuoiSeri.ToString(), hoSoGiamDinh.NgayTbao.ToString(), hoSoGiamDinh.NgayTthat.ToString(), hoSoGiamDinh.DiaDiemtt, hoSoGiamDinh.NguyenNhanTtat, hoSoGiamDinh.MaGaraVcx, ghiChu, hoSoGiamDinh.NgLienhe, hoSoGiamDinh.DienThoai);
                                }

                                if (guiSMS)
                                {
                                    ghiChu += ". Đã gửi SMS.";
                                    //PheDuyetHelper.SendSMS(giamDinhVien.Dienthoai.Trim(), hoSoGiamDinh.BienKsoat, hoSoGiamDinh.SoSeri.ToString(), hoSoGiamDinh.NgayTthat.Value.Date.ToString(), hoSoGiamDinh.DienThoai, "BL", hoSoGiamDinh.PrKey);
                                }
                            }
                            else
                            {
                                return "Đang có lỗi khi map giám định viên, vui lòng liên hệ IT";
                            }

                            NhatKy nk = new NhatKy()
                            {
                                //PrKey = _context.NhatKies.Count() + 1,
                                FrKey = hoSoGiamDinh.PrKey,
                                MaTtrangGd = "9",
                                TenTtrangGd = "Hồ sơ TPC đang xử lý",
                                GhiChu = ghiChu,
                                NgayCapnhat = DateTime.Now,
                                MaUser = currentUser.Oid
                            };
                            await _context.NhatKies.AddAsync(nk);

                            _context.SaveChanges();
                            return nk.PrKey.ToString();
                        }

                        // PHẦN DƯỚI ĐÂY LÀ CHO HỒ SƠ TỒN - TẠM THỜI CHƯA TRIỂN KHAI ĐẾN.
                        /*
                        // Trong trường hợp không phải hồ sơ trên phân cấp.
                        else
                        {
                            hoSoGiamDinh.MaUser = new Guid(oidGiamDV);
                            if (oidCanBoXuLy != null)
                            {
                                hoSoGiamDinh.NguoiXuly = oidCanBoXuLy;
                            }
                            hoSoGiamDinh.MaTtrangGd = "2";
                            _context.HsgdCtus.Update(hoSoGiamDinh);

                            string oidGiamDVFormat = oidGiamDV.ToLower();
                            DmUser giamDinhVien = await _context.DmUsers.FirstOrDefaultAsync(x => x.Oid.ToString().ToLower().Equals(oidGiamDVFormat));

                            NhatKy nk = new NhatKy()
                            {
                                //PrKey = _context.NhatKies.Count() + 1,
                                FrKey = hoSoGiamDinh.PrKey,
                                MaTtrangGd = "2",
                                TenTtrangGd = "Đã giao giám định",
                                GhiChu = "Giám định viên được giao: " + giamDinhVien.TenUser,
                                NgayCapnhat = DateTime.Now,
                                MaUser = new Guid(oidCurrentUser),
                            };
                            await _context.NhatKies.AddAsync(nk);

                            _context.SaveChanges();
                            return nk.PrKey.ToString();
                        }
                        */

                        else
                        {
                            return "Chỉ có các hồ sơ chưa giao giám định, đã giao giám định hoặc đang giám định mới có thể giao giám định.";
                        }
                    }
                }
                else
                {
                    return $"Hồ sơ {hoSoGiamDinh.SoHsgd} không tồn tại";
                }
            }
            catch (Exception ex)
            {
                _logger.Error("dbContextTransaction Exception when assignApproval: " + ex.ToString());
                _logger.Error("Error record: " + JsonConvert.SerializeObject(pr_key));
                _context.Dispose();
                throw;
            }
        }

        // Chuyển chờ phê duyệt - có bao gồm cả gán giám định cho các hồ sơ trên phân cấp (TPC).
        // Các bước: Kiểm tra hồ sơ tồn tại -> Kiểm tra user tồn tại -> Kiểm tra cán bộ tồn tại -> Validate hồ sơ đã tạo phương án sửa chữa -> Validate báo cáo giám định
        // khanhlh - 08/09/2024

        //ducla comment: Nếu user là giám định viên, chuyển cho đội trưởng thì không cần phải check uỷ quyền
        public async Task<string> requestApproval(int pr_key, string ghiChu, string currentUserEmail, string? oidCanBoPheDuyet = null)
        {
            try
            {
                // Lấy thông tin người dùng hiện tại
                DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();

                // Lấy thông tin hồ sơ.
                HsgdCtu hoSoGiamDinh = await _context.HsgdCtus.FirstOrDefaultAsync(x => x.PrKey == pr_key);

                if (hoSoGiamDinh != null && currentUser != null)
                {
                    //1.Kiểm tra xem hồ sơ giám định đã tạo pias chưa
                    List<HsbtCt> chiTietBoiThuong = _context_pias.HsbtCts.Where(x => x.FrKey == hoSoGiamDinh.PrKeyBt).ToList();
                    if (chiTietBoiThuong != null)
                    {
                        bool phainhappasc = chiTietBoiThuong.Any(x => x.MaSp == "050104" || (x.MaSp == "050101" && x.MaDkhoan == "05010102")) ? true : false;
                        if (phainhappasc)
                        {
                            //Nếu phải nhập PASC thì kiểm tra đã nhập PASC hay chưa?
                            bool createdPASC = PheDuyetHelper.checkCreatedPASC(hoSoGiamDinh);
                            if (createdPASC)
                            {
                                //Nếu phải đã nhập PASC rồi thì kiểm tra tiếp báo cáo giám định
                                string ketquaValidateBaoCao = PheDuyetHelper.validateBaoCaoGiamDinh(hoSoGiamDinh);
                                if (!ketquaValidateBaoCao.Equals("0"))
                                {
                                    return "Báo cáo giám định lỗi: " + ketquaValidateBaoCao;
                                }
                            }
                            else
                            {
                                return "Vui lòng đảm bảo tất cả các sản phầm đều đã được nhập đề xuất PASC, hoặc xoá các PASC trống.";
                            }
                        }
                        //Nếu kiểm tra phải nhập phương án sửa chữa và báo cáo giám định xong rồi thì chuyển chờ
                        if (currentUser != null)
                        {
                            // Dùng để lập nhật ký cho gọn. Sẽ dùng tới ở dưới.
                            string diaryStatusCode = "";
                            string diaryStatus = "";
                            string diaryNote = "";

                            bool dieu_kien_ttrang_3_5 = (hoSoGiamDinh.MaTtrangGd == "3" || hoSoGiamDinh.MaTtrangGd == "5") && (currentUser.LoaiUser == 1 || currentUser.LoaiUser == 2 || currentUser.LoaiUser == 3 || currentUser.LoaiUser == 4 || currentUser.LoaiUser == 7 || currentUser.LoaiUser == 6 || currentUser.LoaiUser == 9 || currentUser.LoaiUser == 10 || currentUser.LoaiUser == 11 || PheDuyetHelper.check_UyQuyen_HoSoTPC(currentUser) == "FULL_QUYEN" || PheDuyetHelper.check_UyQuyen_HoSoTPC(currentUser) == "CHUYENCHO_PD");
                            bool dieu_kien_ttrang_9 = ((hoSoGiamDinh.MaTtrangGd == "9" || hoSoGiamDinh.MaTtrangGd == "10") && (currentUser.LoaiUser == 1 || currentUser.LoaiUser == 4 || currentUser.LoaiUser == 6 || currentUser.LoaiUser == 9 || currentUser.LoaiUser == 10 || currentUser.LoaiUser == 11 || PheDuyetHelper.check_UyQuyen_HoSoTPC(currentUser) == "FULL_QUYEN" || PheDuyetHelper.check_UyQuyen_HoSoTPC(currentUser) == "CHUYENCHO_PD"));

                            // Nếu hồ sơ đang ở trạng thái 9:
                            if (hoSoGiamDinh.MaTtrangGd == "9" || hoSoGiamDinh.MaTtrangGd == "10" || (hoSoGiamDinh.MaTtrangGd == "3") || (hoSoGiamDinh.MaTtrangGd == "5"))
                            {
                                if (!dieu_kien_ttrang_3_5 && !dieu_kien_ttrang_9)
                                {
                                    return "Trạng thái hồ sơ không phù hợp để chuyển chờ PD, hoặc tài khoản này không có quyền phê duyệt !";
                                }
                                else
                                {
                                    string currentOID = currentUser.Oid.ToString().ToLower();
                                    DmUqHstpc uyQuyenUser = _context.DmUqHstpcs.Where(x => x.MaUserUq.ToLower().Equals(currentOID) && x.LoaiUyquyen == "10").OrderByDescending(x => x.NgayHl).FirstOrDefault();

                                    if (uyQuyenUser != null)
                                    {
                                        decimal sum_bao_lanh = 0; // Kiểm tra user phải có số tiền giới hạn uỷ quyền lớn hơn số tiền của hồ sơ.

                                        // Tính tổng số tiền của hồ sơ
                                        // Tính dựa theo logic và hàm Reload Sum
                                        List<HsbtCt> list_ct = (from hsbtCt in _context_pias.HsbtCts
                                                                where hsbtCt.FrKey == hoSoGiamDinh.PrKeyBt
                                                                select new HsbtCt
                                                                {
                                                                    PrKey = hsbtCt.PrKey
                                                                }).ToList();

                                        if (list_ct.Count > 0)
                                        {

                                            for (int i = 0; i < list_ct.Count; i++)
                                            {
                                                HsgdDxCt to_be_added = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCt == list_ct[i].PrKey).FirstOrDefault();
                                                if (to_be_added != null)
                                                {
                                                    var sum_dx = _dx_repo.ReloadSum(to_be_added.PrKey);
                                                    if (sum_dx != null && sum_dx.Count() > 0)
                                                    {
                                                        sum_bao_lanh += sum_dx[0].StBl ?? 0;
                                                    }

                                                }
                                            }
                                        }
                                        else
                                        {
                                            return "Không thể chuyển chờ phê duyệt khi có sản phẩm chưa nhập đề xuất PASC";
                                        }

                                        if (uyQuyenUser.GhSotienUq < sum_bao_lanh)
                                        {
                                            return "Tổng số tiền " + (Int128)sum_bao_lanh + " vượt hạn mức uỷ quyền của user";
                                        }
                                    }
                                    else
                                    {
                                        return "User không được uỷ quyền chuyển chờ phê duyệt";
                                    }

                                    ///////
                                    // Khi gán xong xuôi người duyệt thì bắt đầu tiến hành chuyển chờ.
                                    ///////
                                    if (dieu_kien_ttrang_3_5 || dieu_kien_ttrang_9)
                                    {
                                        hoSoGiamDinh.MaTtrangGd = "10";
                                        _context.HsgdCtus.Update(hoSoGiamDinh);

                                        diaryStatusCode = "10";
                                        diaryStatus = "Hồ sơ TPC chờ duyệt";
                                        diaryNote = "Chuyển HSGĐ TPC từ đang xử lý sang chờ phê duyệt. Ghi chú: " + ghiChu;
                                    }

                                    // Thêm nhật ký chuyển chờ phê duyệt.
                                    NhatKy nk2 = new NhatKy()
                                    {
                                        FrKey = hoSoGiamDinh.PrKey,
                                        MaTtrangGd = diaryStatusCode,
                                        TenTtrangGd = diaryStatus,
                                        GhiChu = diaryNote,
                                        NgayCapnhat = DateTime.Now,
                                        MaUser = currentUser.Oid,
                                    };
                                    await _context.NhatKies.AddAsync(nk2);

                                    // Sau đó update hồ sơ qua Pias:
                                    if (hoSoGiamDinh.PrKeyBt != 0)
                                    {
                                        HsbtCtu hoSoBoiThuong = await _context_pias.HsbtCtus.FirstOrDefaultAsync(x => x.PrKey == hoSoGiamDinh.PrKeyBt);
                                        if (hoSoBoiThuong != null)
                                        {
                                            await PheDuyetHelper.updateHSBTPias(hoSoGiamDinh, hoSoBoiThuong);
                                        }
                                    }

                                    await _context.SaveChangesAsync();

                                    return hoSoGiamDinh.PrKey.ToString();
                                }
                            }
                            else
                            {
                                return "Hồ sơ đang không trong trạng thái có thể chuyển chờ phê duyệt.";
                            }
                        }
                        else
                        {
                            return "User không tồn tại trong hệ thống";
                        }
                    }
                    else
                    {
                        return "Vui lòng tạo hồ sơ bồi thường Pias trước khi thực hiện thao tác";
                    }                        
                }
                else
                {
                    return $"Hồ sơ {pr_key} không tồn tại.";
                }
            }
            catch (Exception ex)
            {
                _logger.Error("dbContextTransaction Exception when assignApproval: " + ex.ToString());
                _logger.Error("Error record: " + JsonConvert.SerializeObject(pr_key));
                _context.Dispose();
                return "Có sản phẩm chưa nhập đề xuất PASC. Nếu đã nhập hết mà vẫn có lỗi, vui lòng liên hệ IT";
                throw;
            }
        }


        // Chuyển gán hồ sơ:
        // Sử dụng khi hồ sơ trên phân cấp của đội và muốn chuyển gán cho đội trưởng

        // Chuyển chờ phê duyệt - có bao gồm cả gán giám định cho các hồ sơ trên phân cấp (TPC).
        // Các bước: Kiểm tra hồ sơ tồn tại -> Kiểm tra user tồn tại -> Kiểm tra cán bộ tồn tại -> Validate hồ sơ đã tạo phương án sửa chữa -> Validate báo cáo giám định
        // khanhlh - 08/09/2024
        public async Task<string> ChuyenGanHoSo(int pr_key, string ghiChu, string oidCanBoPheDuyet, string currentUserEmail)
        {
            try
            {
                // Lấy thông tin người dùng hiện tại
                DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();

                // Lấy thông tin hồ sơ.
                HsgdCtu hoSoGiamDinh = await _context.HsgdCtus.FirstOrDefaultAsync(x => x.PrKey == pr_key);

                if (hoSoGiamDinh != null && currentUser != null)
                {
                    //1.Kiểm tra xem hồ sơ giám định đã tạo pias chưa
                    List<HsbtCt> chiTietBoiThuong = _context_pias.HsbtCts.Where(x => x.FrKey == hoSoGiamDinh.PrKeyBt).ToList();
                    if (chiTietBoiThuong != null)
                    {
                        bool phainhappasc = chiTietBoiThuong.Any(x => x.MaSp == "050104" || (x.MaSp == "050101" && x.MaDkhoan == "05010102")) ? true: false;
                         if (phainhappasc)
                         {
                            //Nếu phải nhập PASC thì kiểm tra đã nhập PASC hay chưa?
                            bool createdPASC = PheDuyetHelper.checkCreatedPASC(hoSoGiamDinh);
                            if (createdPASC)
                            {
                                //Nếu phải đã nhập PASC rồi thì kiểm tra tiếp báo cáo giám định
                                string ketquaValidateBaoCao = PheDuyetHelper.validateBaoCaoGiamDinh(hoSoGiamDinh);
                                if (!ketquaValidateBaoCao.Equals("0"))
                                {
                                    return ketquaValidateBaoCao;
                                }                               
                            }
                            else
                            {
                                return "Vui lòng đảm bảo tất cả các sản phầm đều đã được nhập đề xuất PASC, hoặc xoá các PASC trống.";
                            }    
                         }                       
                        if (currentUser != null)
                        {
                            // Dùng để lập nhật ký cho gọn. Sẽ dùng tới ở dưới.
                            string diaryStatusCode = "";
                            string diaryStatus = "";
                            string diaryNote = "";
                            // Nếu hồ sơ đang ở trạng thái 9, 10:
                            if (hoSoGiamDinh.MaTtrangGd == "9" || hoSoGiamDinh.MaTtrangGd == "10" || (hoSoGiamDinh.MaTtrangGd == "3") || (hoSoGiamDinh.MaTtrangGd == "5"))
                            {
                                string currentOID = currentUser.Oid.ToString().ToLower();
                                if (currentUser.LoaiUser == 0 || currentUser.LoaiUser == 5 || currentUser.LoaiUser == 12)
                                {
                                    return "User không được uỷ quyền chuyển gán duyệt";
                                }

                                hoSoGiamDinh.MaTtrangGd = "9";
                                _context.HsgdCtus.Update(hoSoGiamDinh);

                                DmUser nguoiDuyet = null;
                                if (oidCanBoPheDuyet != null)
                                {
                                    if (!oidCanBoPheDuyet.Equals(""))
                                    {
                                        nguoiDuyet = _context.DmUsers.FirstOrDefault(x => x.Oid == Guid.Parse(oidCanBoPheDuyet));
                                    }
                                }

                                if (nguoiDuyet != null)
                                {
                                    if (currentUser.LoaiUser == 4)
                                    {
                                        hoSoGiamDinh.MaTtrangGd = "9";
                                        _context.HsgdCtus.Update(hoSoGiamDinh);
                                        diaryStatusCode = "9";
                                        diaryStatus = "Hồ sơ TPC đang xử lý";
                                        diaryNote = "Cán bộ GĐV Hỗ trợ ĐV " + currentUser.TenUser + " chuyển gán cho " + nguoiDuyet.TenUser + ". Ghi chú: " + ghiChu;
                                    }
                                    // Các trường hợp còn lại.
                                    else
                                    {
                                        hoSoGiamDinh.MaTtrangGd = "9";
                                        _context.HsgdCtus.Update(hoSoGiamDinh);
                                        diaryStatusCode = "9";
                                        diaryStatus = "Hồ sơ TPC đang xử lý";
                                        diaryNote = diaryNote = "Cán bộ " + currentUser.TenUser + " chuyển gán cho " + nguoiDuyet.TenUser + ". Ghi chú: " + ghiChu + "; GR_VCX: " + hoSoGiamDinh.MaGaraVcx + "; GR_TNDS: " + hoSoGiamDinh.MaGaraTnds;
                                    }
                                    // Thêm lịch sử chuyển chờ phê duyệt.
                                    HsgdLsu newLsu = new HsgdLsu()
                                    {
                                        FrKey = hoSoGiamDinh.PrKey,
                                        MaUserChuyen = currentUser.Oid.ToString(),
                                        MaUserNhan = oidCanBoPheDuyet,
                                        GhiChu = ghiChu,
                                        NgayCnhat = DateTime.Now
                                    };
                                    _context.HsgdLsus.Add(newLsu);
                                    // Thêm nhật ký giao giám định.
                                    NhatKy nk = new NhatKy()
                                    {
                                        FrKey = hoSoGiamDinh.PrKey,
                                        MaTtrangGd = diaryStatusCode,
                                        TenTtrangGd = diaryStatus,
                                        GhiChu = diaryNote,
                                        NgayCapnhat = DateTime.Now,
                                        MaUser = currentUser.Oid,
                                    };
                                    _context.NhatKies.Add(nk);
                                    _context.SaveChanges();
                                    return nk.PrKey.ToString();
                                }
                                else
                                {
                                    return "Chưa có thông tin cán bộ tiếp nhận";
                                }
                            }
                            else
                            {
                                return "Trạng thái hồ sơ không cho phép thực hiện hành động này";
                            }
                        }
                        else
                        {
                            return "Người dùng null";
                        }
                    }
                    else
                    {
                        return "Vui lòng tạo hồ sơ bồi thường Pias trước khi thực hiện thao tác";
                    }    
                                      
                }
                else
                {
                    return "Mã hồ sơ lỗi, hoặc không dò được người dùng, vui lòng tải lại trang hoặc liên hệ IT";
                }
            }
            catch (Exception err)
            {
                _logger.Error("ChuyenGanHoSo: " + pr_key.ToString() + err.ToString());
                return "Lỗi gán cán bộ tiếp nhận";
            }
        }


        public string GanNguoiTiepNhan(int pr_key, string ghiChu, string oidCanBoPheDuyet, string currentUserEmail)
        {
            try
            {
                HsgdCtu hoSoGiamDinh = _context.HsgdCtus.Where(x => x.PrKey == pr_key).FirstOrDefault();
                DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();

                if (hoSoGiamDinh != null && currentUser != null)
                {
                    //DmUqHstpc uyQuyenUser = _context.DmUqHstpcs.Where(x => x.MaUserUq.ToLower().Equals(currentUser.Oid.ToString().ToLower()) && x.LoaiUyquyen == "10" && x.NgayHl < DateTime.Today.AddDays(1)).OrderByDescending(x => x.NgayHl).FirstOrDefault();

                    //if (uyQuyenUser != null)
                    //{
                    //    decimal sum_bao_lanh = 0; // Kiểm tra user phải có số tiền giới hạn uỷ quyền lớn hơn số tiền của hồ sơ.

                    //    List<HsbtCt> list_ct = (from hsbtCt in _context_pias.HsbtCts
                    //                            where hsbtCt.FrKey == hoSoGiamDinh.PrKeyBt
                    //                            select new HsbtCt
                    //                            {
                    //                                PrKey = hsbtCt.PrKey
                    //                            }).ToList();

                    //    if (list_ct.Count > 0)
                    //    {

                    //        for (int i = 0; i < list_ct.Count; i++)
                    //        {
                    //            HsgdDxCt to_be_added = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCt == list_ct[i].PrKey).FirstOrDefault();
                    //            if (to_be_added != null)
                    //            {
                    //                sum_bao_lanh += _dx_repo.ReloadSum(to_be_added.PrKey)[0].StBl ?? 0;
                    //            }
                    //        }

                    //        if (uyQuyenUser.GhSotienUq <= sum_bao_lanh)
                    //        {
                    //            return "Tổng số tiền " + (Int128)sum_bao_lanh + " vượt hạn mức uỷ quyền của user";
                    //        }
                    //    }
                    //    else
                    //    {
                    //        return "Lỗi gán người duyệt";
                    //    }
                    //}
                    //else
                    //{
                    //    return "User không được uỷ quyền chuyển chờ phê duyệt";
                    //}

                    bool dieu_kien_ttrang_3_5 = (hoSoGiamDinh.MaTtrangGd == "3" || hoSoGiamDinh.MaTtrangGd == "5") && (currentUser.LoaiUser == 1 || currentUser.LoaiUser == 2 || currentUser.LoaiUser == 3 || currentUser.LoaiUser == 4 || currentUser.LoaiUser == 6 || currentUser.LoaiUser == 7 || currentUser.LoaiUser == 8 || currentUser.LoaiUser == 9 || currentUser.LoaiUser == 10 || currentUser.LoaiUser == 11 || PheDuyetHelper.check_UyQuyen_HoSoTPC(currentUser) == "FULL_QUYEN" || PheDuyetHelper.check_UyQuyen_HoSoTPC(currentUser) == "CHUYENCHO_PD");
                    bool dieu_kien_ttrang_9 = ((hoSoGiamDinh.MaTtrangGd == "9" || hoSoGiamDinh.MaTtrangGd == "10") && (currentUser.LoaiUser == 1 || currentUser.LoaiUser == 6 || currentUser.LoaiUser == 8 || currentUser.LoaiUser == 9 || currentUser.LoaiUser == 10 || currentUser.LoaiUser == 11 || PheDuyetHelper.check_UyQuyen_HoSoTPC(currentUser) == "FULL_QUYEN" || PheDuyetHelper.check_UyQuyen_HoSoTPC(currentUser) == "CHUYENCHO_PD"));
                    // Nếu có gán người duyệt thì tiến hành kiểm tra và lấy thông tin người duyệt

                    string diaryStatusCode = "";
                    string diaryStatus = "";
                    string diaryNote = "";

                    DmUser nguoiDuyet = null;
                    if (oidCanBoPheDuyet != null)
                    {
                        if (!oidCanBoPheDuyet.Equals(""))
                        {
                            nguoiDuyet = _context.DmUsers.FirstOrDefault(x => x.Oid == Guid.Parse(oidCanBoPheDuyet));
                        }
                    }

                    if (nguoiDuyet != null)
                    {
                        bool checkQuyenChuyenCho = (dieu_kien_ttrang_3_5 || dieu_kien_ttrang_9) || (currentUser.LoaiUser == 4 && currentUser.IsGdvHotro.Value && !currentUser.MaDonvi.Equals("00"));

                        // Nếu user hiện tại là user có phân quyền yêu cầu phê duyệt.
                        if (checkQuyenChuyenCho)
                        {
                            // Nếu user là GDV Hỗ Trợ
                            if (currentUser.LoaiUser == 4)
                            {
                                hoSoGiamDinh.MaTtrangGd = "9";
                                _context.HsgdCtus.Update(hoSoGiamDinh);
                                diaryStatusCode = "9";
                                diaryStatus = "Hồ sơ TPC đang xử lý";
                                diaryNote = "Cán bộ GĐV Hỗ trợ ĐV " + currentUser.TenUser + " chuyển cho " + nguoiDuyet.TenUser + ". Ghi chú: " + ghiChu;
                            }
                            // Các trường hợp còn lại.
                            else
                            {
                                hoSoGiamDinh.MaTtrangGd = "9";
                                _context.HsgdCtus.Update(hoSoGiamDinh);
                                diaryStatusCode = "9";
                                diaryStatus = "Hồ sơ TPC đang xử lý";
                                diaryNote = diaryNote = "Cán bộ " + currentUser.TenUser + " chuyển cho " + nguoiDuyet.TenUser + ". Ghi chú: " + ghiChu + "; GR_VCX: " + hoSoGiamDinh.MaGaraVcx + "; GR_TNDS: " + hoSoGiamDinh.MaGaraTnds;
                            }

                            // Thêm lịch sử chuyển chờ phê duyệt.
                            HsgdLsu newLsu = new HsgdLsu()
                            {
                                FrKey = hoSoGiamDinh.PrKey,
                                MaUserChuyen = currentUser.Oid.ToString(),
                                MaUserNhan = oidCanBoPheDuyet,
                                GhiChu = ghiChu,
                                NgayCnhat = DateTime.Now
                            };
                            _context.HsgdLsus.Add(newLsu);

                            // Thêm nhật ký giao giám định.
                            NhatKy nk = new NhatKy()
                            {
                                FrKey = hoSoGiamDinh.PrKey,
                                MaTtrangGd = diaryStatusCode,
                                TenTtrangGd = diaryStatus,
                                GhiChu = diaryNote,
                                NgayCapnhat = DateTime.Now,
                                MaUser = currentUser.Oid,
                            };
                            _context.NhatKies.Add(nk);

                            _context.SaveChanges();

                            return nk.PrKey.ToString();

                        }
                        else
                        {
                            return "User hiện không được phân quyền thực hiện hành động này.";
                        }
                    }
                    else
                    {
                        return "Người duyệt rỗng thì không nên gọi API này";
                    }
                }
                else
                {
                    return "Hồ sơ sai hoặc người dùng không tồn tại";
                }
            }
            catch (Exception err)
            {
                _logger.Error(pr_key.ToString() + "GanNguoiTiepNhan lỗi: " + err.ToString());
                return "Lỗi gán cán bộ tiếp nhận";
            }
        }

        // Yêu cầu bổ sung thông tin 
        // khanhllh - 04/09/2024

        // Tạm thời khóa phần yêu cầu cho các hồ sơ tồn.

        public async Task<string> requestAdditionalDetail(int pr_key, bool guiEmail, bool guiSMS, string ghiChu, string currentUserEmail)
        {
            try
            {
                DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
                if (currentUser.MaDonvi == "31")
                {
                    if (!(currentUser.LoaiUser == 1 || currentUser.LoaiUser == 9 || currentUser.LoaiUser == 10 || currentUser.LoaiUser == 11))
                    {
                        return "Chỉ loại User phó phòng trở lên mới thực hiện được chức năng này!";
                    }
                }                                              
                                
                // Tìm hồ sơ
                HsgdCtu hoSoGiamDinh = await _context.HsgdCtus.FirstOrDefaultAsync(x => x.PrKey == pr_key);
                if (hoSoGiamDinh != null && currentUser != null)
                {
                    // Lấy người dùng hiện tại từ DB.

                    DmUser giamDinhVien = await _context.DmUsers.FirstOrDefaultAsync(x => x.Oid == hoSoGiamDinh.MaUser);

                    if (currentUser != null || giamDinhVien != null)
                    {
                        string diaryMessage = "Yêu cầu bổ sung thông tin: "; // Đổi input nhật ký tùy theo trường hợp
                        string diaryStatus = "Bổ sung thông tin";

                        // Đổi trạng thái tùy theo tình trạng hồ sơ.
                        switch (hoSoGiamDinh.MaTtrangGd)
                        {
                            case "4":

                                // Hồ sơ chờ duyệt.
                                if (currentUser.LoaiUser == 1 || currentUser.LoaiUser == 2 || currentUser.LoaiUser == 3 || currentUser.LoaiUser == 6 || currentUser.LoaiUser == 8 || currentUser.LoaiUser == 9 || currentUser.LoaiUser == 10 || currentUser.LoaiUser == 11 || PheDuyetHelper.check_UyQuyen_HoSoTPC(currentUser).Equals("CHUYENCHO_PD") || PheDuyetHelper.check_UyQuyen_HoSoTPC(currentUser).Equals("FULL_QUYEN"))
                                {
                                    hoSoGiamDinh.MaTtrangGd = "5";
                                }
                                else
                                {
                                    return "User không được phân quyền thực hiện chức năng này";
                                }
                                break;

                            // Đã duyệt nhưng cần bổ sung thông tin.
                            case "6":

                                if (currentUser.LoaiUser == 1 || currentUser.LoaiUser == 2 || currentUser.LoaiUser == 3 || currentUser.LoaiUser == 6 || currentUser.LoaiUser == 8 || currentUser.LoaiUser == 9 || currentUser.LoaiUser == 10 || currentUser.LoaiUser == 11 || PheDuyetHelper.check_UyQuyen_HoSoTPC(currentUser).Equals("CHUYENCHO_PD") || PheDuyetHelper.check_UyQuyen_HoSoTPC(currentUser).Equals("FULL_QUYEN"))
                                {
                                    hoSoGiamDinh.MaTtrangGd = "9";
                                    diaryMessage = "Chuyển hồ sơ về trang thái: HSGĐ TPC đang xử lý. Ghi chú: "; // Gửi Email về Cán bộ trung tâm.
                                    diaryStatus = "Hồ sơ TPC đang xử lý";
                                    var hsgd_dx_ct = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCtu == hoSoGiamDinh.PrKeyBt).ExecuteUpdate(s => s.SetProperty(u => u.PathPasc, "").SetProperty(u => u.PathBaolanh, ""));
                                    var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == pr_key && x.SendThongbaoBt == 1).ExecuteUpdate(s => s.SetProperty(u => u.SendThongbaoBt, 0));
                                }
                                else
                                {
                                    return "User không được phân quyền thực hiện chức năng này";
                                }
                                break;

                            case "9":
                                if (currentUser.LoaiUser == 1 || currentUser.LoaiUser == 2 || currentUser.LoaiUser == 3 || currentUser.LoaiUser == 6 || currentUser.LoaiUser == 8 || currentUser.LoaiUser == 9 || currentUser.LoaiUser == 10 || currentUser.LoaiUser == 11 || PheDuyetHelper.check_UyQuyen_HoSoTPC(currentUser).Equals("CHUYENCHO_PD") || PheDuyetHelper.check_UyQuyen_HoSoTPC(currentUser).Equals("FULL_QUYEN"))
                                {
                                    hoSoGiamDinh.MaTtrangGd = "9";
                                    diaryMessage = "Chuyển hồ sơ về trang thái: HSGĐ TPC đang xử lý. Ghi chú: "; // Gửi Email về Cán bộ trung tâm.
                                    diaryStatus = "Hồ sơ TPC đang xử lý";
                                }
                                else
                                {
                                    return "User không được phân quyền thực hiện chức năng này";
                                }
                                break;
                            case "10":

                                if (currentUser.LoaiUser == 1 || currentUser.LoaiUser == 2 || currentUser.LoaiUser == 3 || currentUser.LoaiUser == 6 || currentUser.LoaiUser == 8 || currentUser.LoaiUser == 9 || currentUser.LoaiUser == 10 || currentUser.LoaiUser == 11 || PheDuyetHelper.check_UyQuyen_HoSoTPC(currentUser).Equals("CHUYENCHO_PD") || PheDuyetHelper.check_UyQuyen_HoSoTPC(currentUser).Equals("FULL_QUYEN"))
                                {
                                    hoSoGiamDinh.MaTtrangGd = "9";
                                    diaryMessage = "Chuyển hồ sơ về trang thái: HSGĐ TPC đang xử lý. Ghi chú: "; // Gửi Email về Cán bộ trung tâm.
                                    diaryStatus = "Hồ sơ TPC đang xử lý";
                                }
                                else
                                {
                                    return "User không được phân quyền thực hiện chức năng này";
                                }
                                break;

                            default:
                                return "Hồ sơ không trong trạng thái có thể yêu cầu bổ sung thông tin.";
                        }

                        _context.HsgdCtus.Update(hoSoGiamDinh);

                        // Tạo nhật ký 
                        NhatKy diary = new NhatKy();
                        diary.FrKey = hoSoGiamDinh.PrKey;
                        diary.MaTtrangGd = hoSoGiamDinh.MaTtrangGd;
                        diary.TenTtrangGd = diaryStatus;
                        diary.GhiChu = diaryMessage + ghiChu;
                        if (guiEmail)

                        {
                            diary.GhiChu += ". Đã gửi Email thông báo";
                            //PheDuyetHelper.SendEmail(1, giamDinhVien.Mail.Trim(), "Yêu cầu bổ sung thông tin", hoSoGiamDinh.SoDonbh, hoSoGiamDinh.SoHsgd, "", currentUser.TenUser, hoSoGiamDinh.SoSeri.ToString(), hoSoGiamDinh.BienKsoat.ToString(), hoSoGiamDinh.TenKhach, hoSoGiamDinh.NgayDauSeri.ToString(), hoSoGiamDinh.NgayCuoiSeri.ToString(), hoSoGiamDinh.NgayTbao.ToString(), hoSoGiamDinh.NgayTthat.ToString(), hoSoGiamDinh.DiaDiemtt, hoSoGiamDinh.NguyenNhanTtat, hoSoGiamDinh.MaGaraVcx, ghiChu, hoSoGiamDinh.NgLienhe, hoSoGiamDinh.DienThoai);
                        }

                        if (guiSMS)
                        {
                            diary.GhiChu += ". Đã gửi SMS thông báo";
                        }

                        diary.NgayCapnhat = DateTime.Now;
                        diary.MaUser = currentUser.Oid;

                        await _context.NhatKies.AddAsync(diary);
                        await _context.SaveChangesAsync();

                        return diary.PrKey.ToString();

                    }
                    else
                    {
                        return "User hoặc giám định viên không tồn tại";
                    }
                }
                else
                {
                    return "Hồ sơ không tồn tại.";
                }
            }
            catch (Exception ex)
            {
                _logger.Error("dbContextTransaction Exception when requestAdditionalDetail: " + ex.ToString());
                _logger.Error("Error record: " + JsonConvert.SerializeObject(pr_key));
                _context.Dispose();
                throw;
            }
        }

        // Phê duyệt hồ sơ.
        // LƯU Ý: HIỆN MỚI CÓ PHÊ DUYỆT CHO CÁC HỒ SƠ MỚI (KHÔNG PHẢI HỒ SƠ TỒN).
        // khanhllh - 04/09/2024
        public async Task<string> approveAppraisal(int pr_key, string ghiChu, string currentUserEmail)
        {
            try
            {
                DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
                HsgdCtu hoSoGiamDinh = await _context.HsgdCtus.FirstOrDefaultAsync(x => x.PrKey == pr_key); // Tìm hồ sơ
                if (hoSoGiamDinh != null && currentUser != null)
                {
                    if (currentUser != null)
                    {

                        // TRƯỜNG HỢP HỒ SƠ THƯỜNG 4,10 Chờ phê duyệt và 12 Hồ sơ NPC
                        // Haipv1 16/12/2025 kiểm tra phải duyệt giá mới được phê duyệt hồ sơ
                        // TNDS có madkhoan: 05010101 không phải phê duyệt ảnh duyệt giá
                        List<HsbtCt> chiTietBoiThuong = _context_pias.HsbtCts.Where(x => x.FrKey == hoSoGiamDinh.PrKeyBt).ToList();
                        bool phainhappasc = chiTietBoiThuong.Any(x => x.MaSp == "050104" || (x.MaSp == "050101" && x.MaDkhoan == "05010102")) ? true : false;
                        if(phainhappasc)
                        {
                            var hsgd_dg_cuoi = (from x in _context.HsgdDgs where x.NgayDuyetGia != null && x.LoaiDg == true && x.Hienthi == false && x.FrKey == pr_key orderby x.PrKey descending select x).FirstOrDefault();
                            if (hsgd_dg_cuoi == null)
                            {
                                return "Hồ sơ này chưa được duyệt giá, hãy duyệt giá trước rồi phê duyệt hồ sơ!";
                            }
                        }    
                        
                        if (hoSoGiamDinh.MaTtrangGd.Equals("4") || hoSoGiamDinh.MaTtrangGd.Equals("10") || hoSoGiamDinh.MaTtrangGd.Equals("12"))
                        {

                            // Nếu hồ sơ chưa ở Pias.
                            if (hoSoGiamDinh.PrKeyBt == 0)
                            {
                                return "Hồ sơ GĐTT chưa được lấy sang PIAS";
                            }

                            string validationResult = PheDuyetHelper.validateBaoCaoGiamDinh(hoSoGiamDinh);
                            if (validationResult.Equals("0"))
                            {
                                // Sum số tiền bảo lãnh
                                decimal sum_bao_lanh = 0; // Kiểm tra user phải có số tiền giới hạn uỷ quyền lớn hơn số tiền của hồ sơ.
                                List<HsbtCt> list_ct = (from hsbtCt in _context_pias.HsbtCts
                                                        where hsbtCt.FrKey == hoSoGiamDinh.PrKeyBt
                                                        select new HsbtCt
                                                        {
                                                            PrKey = hsbtCt.PrKey
                                                        }).ToList();

                                if (list_ct.Count > 0)
                                {
                                    List<HsgdDxCt> list_dx_ct = new List<HsgdDxCt>();
                                    for (int i = 0; i < list_ct.Count; i++)
                                    {
                                        HsgdDxCt to_be_added = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCt == list_ct[i].PrKey).FirstOrDefault();
                                        if (to_be_added != null)
                                        {
                                            var sum_dx = _dx_repo.ReloadSum(to_be_added.PrKey);
                                            if (sum_dx != null && sum_dx.Count() > 0)
                                            {
                                                sum_bao_lanh += sum_dx[0].StBl ?? 0;
                                            }
                                        }
                                    }
                                }


                                // Check các phân quyền theo số tiền được ủy quyền
                                string currentOID = currentUser.Oid.ToString().ToLower();
                                DmUqHstpc uyQuyenUser = _context.DmUqHstpcs.Where(x => x.LoaiUyquyen == "6" && x.MaUserUq.ToLower().Equals(currentOID) && x.GhSotienUq >= sum_bao_lanh).OrderByDescending(x => x.NgayHl).FirstOrDefault();
                                if (uyQuyenUser != null)
                                {
                                    // Check hồ sơ NPC
                                    if (uyQuyenUser.LoaiUyquyen.Equals("12") && hoSoGiamDinh.PathTotrinhTpc.Equals(""))
                                    {
                                        return "Hồ sơ GĐTT chưa tạo tờ trình NPC, hãy xem lại!";
                                    }


                                    // Lấy tên trạng thái
                                    DmTtrangGd dmTtrangGd = _context.DmTtrangGds.Where(x => x.MaTtrangGd.Equals(uyQuyenUser.LoaiUyquyen)).FirstOrDefault();

                                    hoSoGiamDinh.MaTtrangGd = uyQuyenUser.LoaiUyquyen;
                                    _context.HsgdCtus.Update(hoSoGiamDinh);

                                    // Tạo lịch sử phê duyệt.
                                    HsgdLsu newLsu = new HsgdLsu()
                                    {
                                        FrKey = hoSoGiamDinh.PrKey,
                                        MaUserChuyen = currentUser.Oid.ToString(),
                                        MaUserNhan = currentUser.Oid.ToString(),
                                        GhiChu = "Cán bộ duyệt hồ sơ",
                                        NgayCnhat = DateTime.Now
                                    };
                                    await _context.HsgdLsus.AddAsync(newLsu);

                                    // Tạo nhật ký 
                                    NhatKy diary = new NhatKy();
                                    diary.FrKey = hoSoGiamDinh.PrKey;
                                    diary.MaTtrangGd = uyQuyenUser.LoaiUyquyen;
                                    diary.TenTtrangGd = dmTtrangGd.TenTtrangGd; // Do chỉ định 1 trạng thái nên gán thẳng sẽ nhanh hơn.
                                    diary.GhiChu = "Hồ sơ giám định được phê duyệt. " + "GR_VCX: " + hoSoGiamDinh.MaGaraVcx + "; GR_TNDS: " + hoSoGiamDinh.MaGaraTnds + ". Ghi chú: " + ghiChu;
                                    diary.NgayCapnhat = DateTime.Now;
                                    diary.MaUser = currentUser.Oid;

                                    await _context.NhatKies.AddAsync(diary);

                                    // Sau đó update hồ sơ qua Pias:
                                    if (hoSoGiamDinh.PrKeyBt != 0)
                                    {
                                        HsbtCtu hoSoBoiThuong = await _context_pias.HsbtCtus.FirstOrDefaultAsync(x => x.PrKey == hoSoGiamDinh.PrKeyBt);
                                        if (hoSoBoiThuong != null)
                                        {
                                            await PheDuyetHelper.updateHSBTPias(hoSoGiamDinh, hoSoBoiThuong);
                                        }
                                    }

                                    await _context.SaveChangesAsync();
                                    return diary.PrKey.ToString();

                                }
                                else
                                {
                                    return "Bạn chưa có quyền phê duyệt, vui lòng kiểm tra trong danh mục uỷ quyền";
                                }
                            }
                            else
                            {
                                return "Lỗi báo cáo giám định: " + validationResult;
                            }

                        }
                        else
                        {
                            return "Hồ sơ đang không trong trạng thái có thể được phê duyệt";
                        }
                    }
                    else
                    {
                        return "User không tồn tại";
                    }
                }
                else
                {
                    return "Hồ sơ không tồn tại";
                }
            }
            catch (Exception ex)
            {
                _logger.Error("dbContextTransaction Exception when approveAppraisal: " + ex.ToString());
                _logger.Error("Error record: " + JsonConvert.SerializeObject(pr_key));
                _context.Dispose();
                return "Có sản phẩm chưa nhập đề xuất PASC. Nếu đã nhập hết mà vẫn có lỗi, vui lòng liên hệ IT";
                throw;
            }
        }
        public async Task<string> Baogia_giamdinh(decimal pr_key, DateTime ngay_bao_gia, decimal so_tien, string de_xuat, string currentUserEmail)
        {
            try
            {
                // Lấy user hiện tại
                var currentUser = await _context.DmUsers
                    .FirstOrDefaultAsync(x => x.Mail == currentUserEmail);

                if (currentUser == null)
                {
                    return "Không tìm thấy người dùng.";
                }

                // Lấy bản ghi giám định báo giá
                var hsgdDg = await _context.HsgdDgs
                    .FirstOrDefaultAsync(x => x.FrKey == pr_key && x.LoaiDg == false);
                var hsgdDgkt = await _context.HsgdDgs
                   .FirstOrDefaultAsync(x => x.FrKey == pr_key && x.LoaiDg == true);

                if (hsgdDg == null || hsgdDgkt==null)
                {
                    return "Không tìm thấy hồ sơ giám định cần cập nhật.";
                }

                // Cập nhật dữ liệu báo giá
                hsgdDg.NgayBaoGia = ngay_bao_gia;
                hsgdDg.SoTien = so_tien;
                hsgdDg.DeXuat = de_xuat;
                hsgdDg.MaUser = currentUser.Oid;
                hsgdDg.Hienthi = false;
                hsgdDg.NgayCapNhat = DateTime.Now;

                _context.HsgdDgs.Update(hsgdDg);
                //Cập nhật lại hiển thị cho nút phê duyệt báo giá
                hsgdDgkt.Hienthi = true;
                _context.HsgdDgs.Update(hsgdDgkt);
                // Ghi nhật ký
                var nhatKy = new NhatKy
                {
                    FrKey = (int)pr_key,
                    MaTtrangGd = "NBG",
                    TenTtrangGd = "3 - Đã gửi báo giá",
                    GhiChu = "Giám định viên gửi báo giá",
                    NgayCapnhat = DateTime.Now,
                    MaUser = currentUser.Oid
                };

                await _context.NhatKies.AddAsync(nhatKy);               
                await _context.SaveChangesAsync();

                return "Gửi báo giá thành công.";
            }
            catch (Exception ex)
            {
                return "Lỗi Baogia_giamdinh: " + ex.Message;
            }
        }
        public async Task<string> Duyetgia_giamdinh(decimal pr_key, DateTime ngay_bao_gia, decimal so_tien, string de_xuat, string currentUserEmail)
        {
            try
            {
                // Lấy user hiện tại
                var currentUser = await _context.DmUsers
                    .FirstOrDefaultAsync(x => x.Mail == currentUserEmail);
                //var soTienuq = _context.DmUqHstpcs.Where(x => x.MaUserUq == currentUser.Oid.ToString() && x.LoaiUyquyen == "6").OrderByDescending(x => x.NgayHl).Select(x => x.GhSotienUq).FirstOrDefault();
                //if (soTienuq != null)
                //{
                //    if(soTienuq<so_tien)
                //        return "Số tiền phê duyệt báo giá lớn hơn số tiền được ủy quyền phê duyệt báo giá vui lòng kiểm tra lại!";
                //}
                if (currentUser == null)
                {
                    return "Không tìm thấy người dùng.";
                }

                // Lấy bản ghi giám định báo giá
                var hsgdDg = await _context.HsgdDgs
                    .FirstOrDefaultAsync(x => x.FrKey == pr_key && x.LoaiDg == true);
                var hsgdDgbd = await _context.HsgdDgs
                   .FirstOrDefaultAsync(x => x.FrKey == pr_key && x.LoaiDg == false);

                if (hsgdDg == null || hsgdDgbd == null)
                {
                    return "Không tìm thấy hồ sơ giám định cần cập nhật.";
                }

                // Cập nhật dữ liệu báo giá
                hsgdDg.NgayDuyetGia = ngay_bao_gia;
                hsgdDg.SoTien = so_tien;
                hsgdDg.DeXuat = de_xuat;
                hsgdDg.MaUserDuyet = currentUser.Oid;
                hsgdDg.Hienthi = false;
                hsgdDg.NgayCapNhat = DateTime.Now;

                _context.HsgdDgs.Update(hsgdDg);
                //Cập nhật lại hiển thị cho nút phê duyệt báo giá
                hsgdDgbd.Hienthi = true;
                _context.HsgdDgs.Update(hsgdDgbd);
                // Ghi nhật ký
                var nhatKy = new NhatKy
                {
                    FrKey = (int)pr_key,
                    MaTtrangGd = "NDG",
                    TenTtrangGd = "4 - Đã duyệt báo giá",
                    GhiChu = "Cán Bộ duyệt bảo giá",
                    NgayCapnhat = DateTime.Now,
                    MaUser = currentUser.Oid
                };

                await _context.NhatKies.AddAsync(nhatKy);
                await _context.SaveChangesAsync();

                return "Duyệt báo giá thành công.";
            }
            catch (Exception ex)
            {
                return "Lỗi: " + ex.Message;
            }
        }
        // Theo y/c của VPPB: Hồ sơ trên 200 triệu phải chuyển cho PTGD ngoài phân cấp.
        // API này để trưởng văn phòng chuyển trình ngoài phân cấp
        // Điều kiện chuyển trình bao gốm:
        // - HSGD đang có mã trạng thái 10
        // - Hồ sơ có số tiền lớn hơn 200 triệu
        // - Tờ trình phê duyệt đã được tạo (có hsgd_ttrinh)
        // - Đã có đề xuất PASC
        public string ChuyenTrinh_NgoaiPhanCap(int pr_key, string currentUserEmail, string? ghiChu = "")
        {
            try
            {
                DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
                HsgdCtu hoSoGiamDinh = _context.HsgdCtus.FirstOrDefault(x => x.PrKey == pr_key); // Tìm hồ sơ
                if (hoSoGiamDinh != null && currentUser != null)
                {
                    // Chỉ cho phép trưởng văn phòng chuyển trình
                    if (currentUser.LoaiUser == 1 || currentUser.LoaiUser == 10)
                    {

                        // Sau đó tiến hành kiểm tra các điều kiện cẩn thiết để chuyển ký hồ sơ:
                        List<HsbtCt> chiTietBoiThuong = _context_pias.HsbtCts.Where(x => x.FrKey == hoSoGiamDinh.PrKeyBt).ToList();
                        if (chiTietBoiThuong != null)
                        {
                            bool phainhappasc = chiTietBoiThuong.Any(x => x.MaSp == "050104" || (x.MaSp == "050101" && x.MaDkhoan == "05010102")) ? true : false;
                            if (phainhappasc)
                            {
                                //Nếu phải nhập PASC thì kiểm tra đã nhập PASC hay chưa?
                                bool createdPASC = PheDuyetHelper.checkCreatedPASC(hoSoGiamDinh);
                                if (createdPASC)
                                {
                                    //Nếu phải đã nhập PASC rồi thì kiểm tra tiếp báo cáo giám định
                                    string ketquaValidateBaoCao = PheDuyetHelper.validateBaoCaoGiamDinh(hoSoGiamDinh);
                                    if (!ketquaValidateBaoCao.Equals("0"))
                                    {
                                        return ketquaValidateBaoCao;
                                    }
                                }
                                else
                                {
                                    return "Vui lòng đảm bảo tất cả các sản phầm đều đã được nhập đề xuất PASC, hoặc xoá các PASC trống.";
                                }
                            }
                        }
                        else
                        {
                            return "Vui lòng tạo hồ sơ bồi thường Pias trước khi thực hiện thao tác";
                        }    
                        bool da_duyet_pagd = hoSoGiamDinh.MaTtrangGd == "10";
                        //bool da_tao_pasc = PheDuyetHelper.checkCreatedPASC(hoSoGiamDinh);
                        bool da_co_to_trinh = PheDuyetHelper.checkCreatedTTrinh(hoSoGiamDinh);

                        if (da_duyet_pagd && da_co_to_trinh)
                        {
                            // Kiểm tra phân quyền & ủy quyền
                            //if (uyQuyenUser != null && uyQuyenUser.GhSotienUq >= sum_sotien_hoso)
                            //{
                            hoSoGiamDinh.MaTtrangGd = "12";

                            // Tạo nhật ký 
                            NhatKy diary = new NhatKy();
                            diary.FrKey = hoSoGiamDinh.PrKey;
                            diary.MaTtrangGd = "12";
                            diary.TenTtrangGd = "HSGĐ ngoài phân cấp"; // Do chỉ định 1 trạng thái nên gán thẳng sẽ nhanh hơn.
                            diary.GhiChu = "Cán bộ " + currentUser.TenUser + "chuyển trình hồ sơ ngoài phân cấp. Ghi chú: " + ghiChu;
                            diary.NgayCapnhat = DateTime.Now;
                            diary.MaUser = currentUser.Oid;

                            _context.HsgdCtus.Update(hoSoGiamDinh);
                            _context.NhatKies.Add(diary);
                            _context.SaveChanges();
                            return hoSoGiamDinh.PrKey.ToString();
                        }
                        else
                        {
                            return "Quý vị hãy đảm bảo hồ sơ này đang được chờ phê duyệt, đã nhập đủ PASC cho các sản phẩm và đã có tờ trình bồi thường";
                        }

                    }
                    else
                    {
                        return "Quý vị chưa có quyền phê duyệt. Vui lòng liên hệ IT.";
                    }

                }
                else
                {
                    return "Có lỗi xảy ra, vui lòng liên hệ IT. Mã lỗi: UNIDENTIFIED_AT_NGOAIPHANCAP_12";
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error when PheDuyet_NgoaiPhanCap_12: " + " at key: " + JsonConvert.SerializeObject(pr_key) + ". ERROR: " + ex.ToString());
                _context.Dispose();
                return "Có lỗi xảy ra, vui lòng liên hệ IT. Mã lỗi: EXCEPTION_AT_NGOAIPHANCAP_12";
                throw;
            }
        }

        // Theo y/c của VPPB: Hồ sơ trên 200 triệu phải chuyển cho PTGD ngoài phân cấp.
        // API này để PTGD duyệt hồ sơ ngoài phân cấp
        // khanhlh - 19/03/2025
        public string PheDuyet_NgoaiPhanCap_12(int pr_key, string currentUserEmail, decimal sum_sotien_hoso, string? ghiChu = "")
        {
            try
            {
                DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
                HsgdCtu hoSoGiamDinh = _context.HsgdCtus.FirstOrDefault(x => x.PrKey == pr_key); // Tìm hồ sơ
                if (hoSoGiamDinh != null && currentUser != null)
                {
                    // Chỉ cho phép Lãnh Đạo TCT thực hiện phê duyệt
                    if (currentUser.LoaiUser == 1 || currentUser.LoaiUser == 16)
                    {
                        string currentOID = currentUser.Oid.ToString().ToLower();
                        DmUqHstpc uyQuyenUser = _context.DmUqHstpcs.Where(x => x.MaUserUq.ToLower().Equals(currentOID) && x.LoaiUyquyen == "6").OrderByDescending(x => x.NgayHl).FirstOrDefault();

                        // Kiểm tra phân quyền & ủy quyền
                        if (uyQuyenUser != null && uyQuyenUser.GhSotienUq >= sum_sotien_hoso)
                        {
                            hoSoGiamDinh.MaTtrangGd = "6";

                            // Tạo nhật ký 
                            NhatKy diary = new NhatKy();
                            diary.FrKey = hoSoGiamDinh.PrKey;
                            diary.MaTtrangGd = "6";
                            diary.TenTtrangGd = "Đã duyệt"; // Do chỉ định 1 trạng thái nên gán thẳng sẽ nhanh hơn.
                            diary.GhiChu = "Hồ sơ giám định ngoài phân cấp được phê duyệt. " + "GR_VCX: " + hoSoGiamDinh.MaGaraVcx + "; GR_TNDS: " + hoSoGiamDinh.MaGaraTnds + ". Ghi chú: " + ghiChu;
                            diary.NgayCapnhat = DateTime.Now;
                            diary.MaUser = currentUser.Oid;

                            _context.HsgdCtus.Update(hoSoGiamDinh);
                            _context.NhatKies.Add(diary);
                            _context.SaveChanges();
                            return hoSoGiamDinh.PrKey.ToString();
                        }
                        else
                        {
                            return "Quý vị chưa có quyền phê duyệt. Vui lòng liên hệ IT.";
                        }
                    }
                    else
                    {
                        return "Quý vị không được phân quyền thực hiện chức năng này";
                    }

                }
                else
                {
                    return "Có lỗi xảy ra, vui lòng liên hệ IT. Mã lỗi: UNIDENTIFIED_AT_NGOAIPHANCAP_12";
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error when PheDuyet_NgoaiPhanCap_12: " + " at key: " + JsonConvert.SerializeObject(pr_key) + ". ERROR: " + ex.ToString());
                _context.Dispose();
                return "Có lỗi xảy ra, vui lòng liên hệ IT. Mã lỗi: EXCEPTION_AT_NGOAIPHANCAP_12";
                throw;
            }
        }

        // Theo y/c của VPPB: Hồ sơ trên 200 triệu phải chuyển cho PTGD ngoài phân cấp.
        // API này để PTGD trả hồ sơ về cho lãnh đạo
        // khanhlh - 19/03/2025
        public string TraHS_NgoaiPhanCap_12(int pr_key, string currentUserEmail, string? ghiChu = "")
        {
            try
            {
                DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
                HsgdCtu hoSoGiamDinh = _context.HsgdCtus.FirstOrDefault(x => x.PrKey == pr_key); // Tìm hồ sơ
                if (hoSoGiamDinh != null && currentUser != null)
                {
                    if (hoSoGiamDinh.MaTtrangGd == "12")
                    {
                        if (currentUser.LoaiUser == 1 || currentUser.LoaiUser == 16)
                        {
                            hoSoGiamDinh.MaTtrangGd = "10";

                            // Tạo nhật ký 
                            NhatKy diary = new NhatKy();
                            diary.FrKey = hoSoGiamDinh.PrKey;
                            diary.MaTtrangGd = "10";
                            diary.TenTtrangGd = "HS chờ phê duyệt";
                            diary.GhiChu = "Hồ sơ ngoai phân cấp bị trả về. Ghi chú: " + ghiChu;
                            diary.NgayCapnhat = DateTime.Now;
                            diary.MaUser = currentUser.Oid;

                            _context.HsgdCtus.Update(hoSoGiamDinh);
                            _context.NhatKies.Add(diary);
                            _context.SaveChanges();
                            return hoSoGiamDinh.PrKey.ToString();

                        }
                        else
                        {
                            return "Quý vị không được phân quyền thực hiện chức năng này";
                        }
                    }
                    else
                    {
                        return "Không thể trả HS không trong trạng thái ngoài phân cấp";
                    }
                }
                else
                {
                    return "Có lỗi xảy ra, vui lòng liên hệ IT. Mã lỗi: UNIDENTIFIED_AT_TRAHS_NGOAIPHANCAP_12";
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error when TraHS_NgoaiPhanCap_12: " + " at key: " + JsonConvert.SerializeObject(pr_key) + ". ERROR: " + ex.ToString());
                _context.Dispose();
                return "Có lỗi xảy ra, vui lòng liên hệ IT. Mã lỗi: EXCEPTION_AT_TRA_HS_sNGOAIPHANCAP_12";
                throw;
            }
        }

        // Lấy danh sách và thông tin của các đơn vị thanh toán từ Pias.
        public List<DonViThanhToanResponse> getListDonViThanhToan()
        {
            try
            {
                var dmDonvi = _context.DmDonvis.Where(x => (x.MaDonvi != "00" && x.MaDonvi != "27")).ToList();

                var dvtt = (from thongtindv in _context_pias.DmVars
                            where thongtindv.MaDonvi != "00" && thongtindv.MaDonvi != "27" && (thongtindv.Bien == "DON_VI" || thongtindv.Bien == "DIA_CHI" || thongtindv.Bien == "MASO_VAT")
                            orderby thongtindv.MaDonvi, thongtindv.Bien ascending
                            select new DmVar
                            {
                                PrKey = thongtindv.PrKey,
                                MaDonvi = thongtindv.MaDonvi,
                                Bien = thongtindv.Bien,
                                GhiChu = thongtindv.GhiChu,
                                GiaTri = thongtindv.GiaTri,
                                GiaTriEng = thongtindv.GiaTriEng,
                                TongHop = thongtindv.TongHop,
                                MaUser = thongtindv.MaUser,
                                NgayCnhat = thongtindv.NgayCnhat,
                                Khoa = thongtindv.Khoa
                            }).ToList();

                List<DonViThanhToanResponse> list_don_vi_tt = new List<DonViThanhToanResponse>();

                for (int i = 0; i < dvtt.Count; i += 3)
                {
                    try
                    {
                        DonViThanhToanResponse donvitt = new DonViThanhToanResponse();
                        donvitt.maDonVi = dvtt.ElementAt(i).MaDonvi;
                        donvitt.diaChi = dvtt.ElementAt(i).GiaTri;
                        donvitt.donViThanhToan = dmDonvi.FirstOrDefault(x => x.MaDonvi == dvtt.ElementAt(i).MaDonvi).TenDonvi;
                        donvitt.tenDonVi = dvtt.ElementAt(i + 1).GiaTri;
                        donvitt.maSoThue = dvtt.ElementAt(i + 2).GiaTri;
                        list_don_vi_tt.Add(donvitt);
                    }
                    catch (Exception err)
                    {
                        continue;
                    }


                }

                return list_don_vi_tt;

            }
            catch (Exception err)
            {
                Console.WriteLine($"Error encouter at getDonViThanhToan: " + err);
                _context.Dispose();
                throw;
            }
        }
        public bool Kiemtra_uynhiemchi(string ma_donvi)
        {
            try
            {
                using (var _context_128 = new Pvs2024TToanContext())
                {
                    var dm_donvi = _context_128.DmLuongTtoans
                        .Where(x => x.MaDonvi == ma_donvi && x.LuongKy != "")
                        .ToList();

                    return dm_donvi.Count > 0;
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("Error at Kiemtra_uynhiemchi: " + err);
                throw;
            }
        }
        public DonViThanhToanResponse GetInfoDonViTT(string ma_don_vi)
        {
            try
            {

                var dvtt = (from thongtindv in _context_pias.DmVars
                            where thongtindv.MaDonvi.Equals(ma_don_vi) && (thongtindv.Bien == "DON_VI" || thongtindv.Bien == "DIA_CHI" || thongtindv.Bien == "MASO_VAT")
                            orderby thongtindv.MaDonvi, thongtindv.Bien ascending
                            select new DmVar
                            {
                                PrKey = thongtindv.PrKey,
                                MaDonvi = thongtindv.MaDonvi,
                                Bien = thongtindv.Bien,
                                GhiChu = thongtindv.GhiChu,
                                GiaTri = thongtindv.GiaTri,
                                GiaTriEng = thongtindv.GiaTriEng,
                                TongHop = thongtindv.TongHop,
                                MaUser = thongtindv.MaUser,
                                NgayCnhat = thongtindv.NgayCnhat,
                                Khoa = thongtindv.Khoa
                            }).ToList();

                List<DonViThanhToanResponse> list_don_vi_tt = new List<DonViThanhToanResponse>();

                for (int i = 0; i < dvtt.Count; i += 3)
                {
                    DonViThanhToanResponse donvitt = new DonViThanhToanResponse();
                    donvitt.maDonVi = dvtt.ElementAt(i).MaDonvi;
                    donvitt.diaChi = dvtt.ElementAt(i).GiaTri;
                    donvitt.donViThanhToan = _context.DmDonvis.Where(x => x.MaDonvi == dvtt.ElementAt(i).MaDonvi).FirstOrDefault().TenDonvi;
                    donvitt.tenDonVi = dvtt.ElementAt(i + 1).GiaTri;
                    donvitt.maSoThue = dvtt.ElementAt(i + 2).GiaTri;
                    list_don_vi_tt.Add(donvitt);
                }

                return list_don_vi_tt[0];
            }
            catch (Exception err)
            {
                Console.WriteLine($"Error encouter at getDonViThanhToan: " + err);
                _context.Dispose();
                throw;
            }
        }


        // Phê duyệt bảo lãnh
        // khanhlh - 19/09/2024
        public async Task<string> pheDuyetBaoLanh(int pr_key, decimal pr_key_hsbt_ct, int bl1, int bl2, int bl3, int bl4, int bl5, int bl6, int bl7, int bl8, int bl9, string bl_tailieubs, string bl_dsemail, string bl_dsphone, string? ma_donvi_tt, string currentUserEmail)
        {
            try
            {
                DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
                HsgdCtu hoSoGiamDinh = _context.HsgdCtus.Where(x => x.PrKey == pr_key).FirstOrDefault();
                HsgdDxCt hsgd_dx_ct = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCt == pr_key_hsbt_ct).FirstOrDefault();
                if (currentUser != null && hoSoGiamDinh != null && hsgd_dx_ct != null)
                {

                    int[] acceptedUserTypes = new int[] { 1, 6, 9, 10, 11 };
                    List<PquyenCnang> list_phanquyen = PheDuyetHelper.Check_PquyenCnang(currentUser);

                    // Kiểm tra phân quyền của User
                    if (Array.Exists(acceptedUserTypes, x => x == currentUser.LoaiUser) || PheDuyetHelper.check_UyQuyen_HoSoTPC(currentUser).Equals("PHEDUYET_HS") || PheDuyetHelper.check_UyQuyen_HoSoTPC(currentUser).Equals("FULL_QUYEN") || (list_phanquyen.Count > 0 && list_phanquyen.Exists(x => x.LoaiQuyen.Equals("BAOLANHDT"))))
                    {
                        // Chỉ các hồ sơ đã duyệt và chưa được phê duyệt thì mới duyệt được bảo lãnh
                        if (hoSoGiamDinh.MaTtrangGd == "6" )
                        {
                            //haipv1 19/12/2025 cho phép VPPN phê duyệt lại bảo lãnh kể cả khi đã được phê duyệt 1 lần rồi 
                            if((hsgd_dx_ct.BlPdbl == 1 && currentUser.MaDonvi=="31"))
                            {
                                return "Hồ sơ này đã được duyệt bảo lãnh, không phê duyệt bảo lãnh lại được nữa!";
                            }
                            if (ma_donvi_tt != null && !ma_donvi_tt.Equals(""))
                            {
                                hsgd_dx_ct.BlPdbl = 1;

                                hsgd_dx_ct.Bl1 = bl1;
                                hsgd_dx_ct.Bl2 = bl2;
                                hsgd_dx_ct.Bl3 = bl3;
                                hsgd_dx_ct.Bl4 = bl4;
                                hsgd_dx_ct.Bl5 = bl5;
                                hsgd_dx_ct.Bl6 = bl6;
                                hsgd_dx_ct.Bl7 = bl7;
                                hsgd_dx_ct.Bl8 = bl8;
                                hsgd_dx_ct.Bl9 = bl9;

                                //List<HsbtCtView> listPhaiTraBT = _dx_repo.GetListPhaiTraBT(hoSoGiamDinh.PrKey);
                                string mailList = "";
                                string phoneList = "";
                                //if (listPhaiTraBT.Count > 0)
                                //{
                                DmGaRa garaGiamDinh = _context.DmGaRas.Where(x => x.MaGara == hsgd_dx_ct.MaGara).FirstOrDefault();
                                mailList = garaGiamDinh != null ? garaGiamDinh.EmailGara : "";
                                phoneList = garaGiamDinh != null ? garaGiamDinh.DienThoaiGara : "";
                                //}

                                hsgd_dx_ct.BlTailieubs = bl_tailieubs;
                                hsgd_dx_ct.BlDsemail += (mailList + ";" + bl_dsemail);
                                hsgd_dx_ct.BlDsphone += (phoneList + ";" + bl_dsphone);
                                hsgd_dx_ct.MaDonviTt = hoSoGiamDinh.MaDonvi;

                                _context.HsgdDxCts.Update(hsgd_dx_ct);

                                // Tạo nhật ký 
                                NhatKy diary = new NhatKy
                                {
                                    FrKey = hoSoGiamDinh.PrKey,
                                    MaTtrangGd = "DBL",
                                    TenTtrangGd = "Duyệt bảo lãnh", // Do chỉ định 1 trạng thái nên gán thẳng sẽ nhanh hơn.
                                    GhiChu = "Duyệt bảo lãnh. Email gửi bảo lãnh ghi nhận: " + (mailList + ";" + bl_dsemail) + ". SDT gửi bảo lãnh ghi nhận: " + (phoneList + ";" + bl_dsphone),
                                    NgayCapnhat = DateTime.Now,
                                    MaUser = currentUser.Oid
                                };

                                await _context.NhatKies.AddAsync(diary);
                                await _context.SaveChangesAsync();
                                return hoSoGiamDinh.PrKey.ToString();

                            }
                            else
                            {
                                return "Hồ sơ chưa có có đơn vị thanh toán";
                            }
                        }
                        else
                        {
                            return "Hồ sơ này chưa được phê duyệt nên không duyệt được bảo lãnh!";
                        }
                    }
                    else
                    {
                        return "Người dùng không có quyền duyệt bảo lãnh";
                    }
                }
                else
                {
                    return "Hồ sơ không tồn tại";

                }

            }
            catch (Exception ex)
            {
                _logger.Error("dbContextTransaction Exception when pheDuyetBaoLanh: " + ex.ToString());
                _logger.Error("Error record: " + JsonConvert.SerializeObject(pr_key));
                _context.Dispose();
                throw;
            }
        }
        
        public async Task<string> LuuThongbaoBT(int pr_keyhsgd, HsgdTbbt HsgdTbbt_, List<HsgdTbbtTt> LHsgdTbbtTt, string currentUserEmail)
        {
            try
            {
                string resutl = "Lưu Thành Công!";
                DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
                HsgdCtu hoSoGiamDinh = _context.HsgdCtus.Where(x => x.PrKey == pr_keyhsgd).FirstOrDefault();                
                if (currentUser != null && hoSoGiamDinh != null)
                {
                    if (HsgdTbbt_.PrKey == 0) 
                    {
                        var hsgd_tbbt = new HsgdTbbt
                        {
                            PrKeyHsgd = pr_keyhsgd,
                            PdTbbt = 0,
                            DsEmail = HsgdTbbt_.DsEmail,
                            TndsXeCoGioi = HsgdTbbt_.TndsXeCoGioi,
                            TndsHangHoa = HsgdTbbt_.TndsHangHoa,
                            TndsTaiNanHk = HsgdTbbt_.TndsTaiNanHk,
                            TndsTaiSanKhac = HsgdTbbt_.TndsTaiSanKhac,
                            TndsNguoi = HsgdTbbt_.TndsNguoi,
                            SoNgayTtoan = HsgdTbbt_.SoNgayTtoan,
                            PathTbbt = string.Empty,
                            GhiChu = string.Empty,
                            MaDonviTT= hoSoGiamDinh.SoDonbh.Substring(3,2)
                        };

                        _context.HsgdTbbts.Add(hsgd_tbbt);

                        if (LHsgdTbbtTt != null && LHsgdTbbtTt.Any())
                        {
                            foreach (var item in LHsgdTbbtTt)
                            {
                                item.FrKey = hsgd_tbbt.PrKey; // sẽ được cập nhật sau khi SaveChanges()
                                _context.HsgdTbbtTts.Add(item);
                            }
                        }

                        _context.SaveChanges();
                    }
                    else
                    {
                        var hsgd_tbbt = _context.HsgdTbbts
                            .FirstOrDefault(x => x.PrKey == HsgdTbbt_.PrKey);

                        if (hsgd_tbbt != null)
                        {
                            // Cập nhật các trường cần sửa
                            hsgd_tbbt.DsEmail = HsgdTbbt_.DsEmail;
                            hsgd_tbbt.TndsXeCoGioi = HsgdTbbt_.TndsXeCoGioi;
                            hsgd_tbbt.TndsHangHoa = HsgdTbbt_.TndsHangHoa;
                            hsgd_tbbt.TndsTaiNanHk = HsgdTbbt_.TndsTaiNanHk;
                            hsgd_tbbt.TndsTaiSanKhac = HsgdTbbt_.TndsTaiSanKhac;
                            hsgd_tbbt.TndsNguoi = HsgdTbbt_.TndsNguoi;
                            hsgd_tbbt.SoNgayTtoan = HsgdTbbt_.SoNgayTtoan;
                            hsgd_tbbt.GhiChu = HsgdTbbt_.GhiChu ?? string.Empty;
                            hsgd_tbbt.MaDonviTT = HsgdTbbt_.MaDonviTT ?? string.Empty;

                            _context.HsgdTbbts.Update(hsgd_tbbt);

                            // Nếu có danh sách tài khoản (cần làm lại toàn bộ)
                            if (LHsgdTbbtTt != null && LHsgdTbbtTt.Any())
                            {
                                // Xóa danh sách cũ trước (nếu cần cập nhật toàn bộ)
                                var oldItems = _context.HsgdTbbtTts
                                    .Where(x => x.FrKey == hsgd_tbbt.PrKey)
                                    .ToList();

                                if (oldItems.Any())
                                    _context.HsgdTbbtTts.RemoveRange(oldItems);

                                // Thêm lại danh sách mới
                                foreach (var item in LHsgdTbbtTt)
                                {
                                    item.FrKey = hsgd_tbbt.PrKey;
                                }

                                _context.HsgdTbbtTts.AddRange(LHsgdTbbtTt);
                            }

                            _context.SaveChanges();
                        }
                    }

                }
                else
                {
                    return "Hồ sơ không tồn tại";

                }
                return resutl;
            }
            catch (Exception ex)
            {
                _logger.Error("dbContextTransaction Exception when PheDuyetTBBT: " + ex.ToString());
                _context.Dispose();
                throw;
            }
        }
        public async Task<LuuThongBaoBTResponse> LayThongbaoBT(int pr_key_hsgd, string currentUserEmail)
        {
            try
            {
                 
                DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
                HsgdCtu hoSoGiamDinh = _context.HsgdCtus.Where(x => x.PrKey == pr_key_hsgd).FirstOrDefault();
                if (currentUser != null && hoSoGiamDinh != null)
                {
                    var hsgdTbbt = await _context.HsgdTbbts
                     .FirstOrDefaultAsync(x => x.PrKeyHsgd == pr_key_hsgd);                    

                    List<HsgdTbbtTt> listTt = new List<HsgdTbbtTt>();
                    if (hsgdTbbt == null)
                    {
                        // Nếu chưa có HsgdTbbt -> tạo mới =======
                        hsgdTbbt = new HsgdTbbt
                        {
                            PrKeyHsgd = pr_key_hsgd,
                            PdTbbt = 0,
                            DsEmail = string.Empty,
                            TndsXeCoGioi = 0,
                            TndsHangHoa = 0,
                            TndsTaiNanHk = 0,
                            TndsTaiSanKhac = 0,
                            TndsNguoi = 0,
                            SoNgayTtoan = 0,
                            PathTbbt = string.Empty,
                            GhiChu = string.Empty,
                            MaDonviTT= hoSoGiamDinh.SoDonbh.Substring(3, 2)
                        };

                        _context.HsgdTbbts.Add(hsgdTbbt);
                        await _context.SaveChangesAsync(); 
                        //hỗ trợ lấy luôn thông tin tài khoản bên tờ trình                      
                        var listNguon = (from a in _context.HsgdCtus
                                         join b in _context.HsgdTtrinhs on a.PrKey equals b.PrKeyHsgd
                                         join c in _context.HsgdTtrinhTt on b.PrKey equals c.FrKey
                                         where a.PrKey == pr_key_hsgd
                                         select c).ToList();
                        if (listNguon.Any())
                        {
                            listTt = listNguon.Select(c => new HsgdTbbtTt
                            {
                                FrKey = hsgdTbbt.PrKey,
                                TenChuTk = c.TenChuTk,
                                SoTaikhoanNh = c.SoTaikhoanNh,
                                TenNh = c.TenNh,
                                SotienTt = c.SotienTt,
                                LydoTt = c.LydoTt
                            }).ToList();
                            _context.HsgdTbbtTts.AddRange(listTt);
                            await _context.SaveChangesAsync();
                        }    
                        //Nếu danh sách tài khoản bên tờ trình cũng trống thì để trống để giám định viên tự nhập                     
                            
                    }
                    //Lấy tổng tiền TNDS trên PIAS
                    var hsbt_ct = (from a in _context_pias_update.HsbtCts
                                   where a.FrKey == hoSoGiamDinh.PrKeyBt
                                   select new
                                   {
                                       FrKey = a.FrKey,
                                       sotien_vcx = a.MaSp == "050104" && a.MaTtrangBt != "04" ? (a.SoTienp + a.SoTienvp) : 0,                                      
                                       sotien_tnds = new[] { "050101", "050102", "050105", "050201", "050203", "050204" }.Contains(a.MaSp) && a.MaTtrangBt != "04" ? (a.SoTienp + a.SoTienvp) : 0,
                                       sotien_tnds26 = new[] { "050103", "050202" }.Contains(a.MaSp) && a.MaTtrangBt != "04" ? (a.SoTienp + a.SoTienvp) : 0,
                                       sotien_vcxm_ng = new[] { "050205" }.Contains(a.MaSp) && a.MaDkhoan == "05010101" && a.MaTtrangBt != "04" ? (a.SoTienp + a.SoTienvp) : 0,
                                       sotien_vcxm_ts = new[] { "050205" }.Contains(a.MaSp) && a.MaDkhoan == "05010102" && a.MaTtrangBt != "04" ? (a.SoTienp + a.SoTienvp) : 0,
                                       sotien_vcxm = a.MaSp == "050205" && a.MaTtrangBt != "04" ? (a.SoTienp + a.SoTienvp) : 0
                                   }).AsQueryable();
                    var hsbt_ct_gr = hsbt_ct.GroupBy(g => g.FrKey)
                    .Select(s => new
                    {
                        sotien_vcx = s.Sum(x => x.sotien_vcx),
                        sotien_tnds = s.Sum(x => x.sotien_tnds),                      
                        sotien_tnds26 = s.Sum(x => x.sotien_tnds26),
                        sotien_vcxm_ng = s.Sum(x => x.sotien_vcxm_ng),
                        sotien_vcxm_ts = s.Sum(x => x.sotien_vcxm_ts),
                        sotien_vcxm = s.Sum(x => x.sotien_vcxm)
                    }).FirstOrDefault();
                    listTt=_context.HsgdTbbtTts.Where(x => x.FrKey == hsgdTbbt.PrKey).ToList();
                    // Trả về dữ liệu tổng hợp =======
                    return new LuuThongBaoBTResponse
                    {
                        PrKeyHsgd = pr_key_hsgd,
                        HsgdTbbt = hsgdTbbt,
                        HsgdTbbtTt = listTt ?? new List<HsgdTbbtTt>(),
                        SumTienTNDSChuXe= hsbt_ct_gr?.sotien_tnds ?? 0
                    };

                }
                else
                {
                    return null;

                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error("dbContextTransaction Exception when PheDuyetTBBT: " + ex.ToString());
                _context.Dispose();
                throw;
            }
        }
        // Gửi bảo lãnh.
        // khanhlh - 19/09/2024
        public async Task<string> GuiBaoLanh(decimal pr_key, decimal pr_key_hsbt_ct, string path_bl, string currentUserEmail, string receiving_emails, string receiving_phones, string? ma_donvi_tt)
        {
            try
            {
                DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
                if (currentUser != null)
                {
                    var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == pr_key).FirstOrDefault();
                    if (hsgd_ctu == null)
                    {
                        return "Hồ sơ giám định không tồn tại. Vui lòng kiểm tra lại!";
                    }
                    HsgdDxCt hsgd_dx_ct = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCt == pr_key_hsbt_ct).FirstOrDefault();
                    if (hsgd_dx_ct != null)
                    {
                        //if (hsgd_dx_ct.BlPdbl != 1)
                        //{
                        //    return "Hồ sơ chưa phê duyệt không thể gửi";
                        //}
                        //if (string.IsNullOrEmpty(hsgd_dx_ct.PathBaolanh))
                        //{
                        //    return "Hồ sơ đã được duyệt bảo lãnh nhưng chưa có file bảo lãnh . Vui lòng kiểm tra lại!";
                        //}
                        bool emailSent = false;
                        bool smsSent = false;
                        string url_download = _configuration["DownloadSettings:DownloadServer"] ?? "";
                        string strFileNameLocalPdf = UtilityHelper.getPathAndCopyTempServer(path_bl, url_download);
                        string ghichu_gui = "";
                        FormBaoLanh fbl = PheDuyetHelper.BaoLanh_GetInfo(pr_key, pr_key_hsbt_ct, currentUser, ma_donvi_tt);
                        if (true)//hoSoGiamDinh.BlSendEmail == 1)
                        {

                            // Chỉ để test - khi nào lên UAT sẽ đóng lại
                            if (currentUser.MaDonvi.Equals("31"))
                            {
                                // Khi nào lên UAT thì mở ra.
                                PheDuyetHelper.SendEmail_BaoLanh("vppb.xcg2.baolanh@gmail.com", ("PVI: Thư bảo lãnh: " + DateTime.Now.Year.ToString() + "-" + hsgd_ctu.MaDonvi + "-" + hsgd_ctu.SoHsgd + " cho xe ô tô BKS: " + hsgd_ctu.BienKsoat + " của " + hsgd_ctu.TenKhach), strFileNameLocalPdf, fbl);
                            }

                            if (!string.IsNullOrEmpty(hsgd_dx_ct.BlDsemail))
                            {
                                string[] separatedEmails = hsgd_dx_ct.BlDsemail.Split(";"); // Chia email gửi
                                Array.ForEach(separatedEmails, email =>
                                {
                                    if (true)//PheDuyetHelper.validateEmail(email.Trim()))
                                    {
                                        // Khi nào lên UAt thì mở ra
                                        PheDuyetHelper.SendEmail_BaoLanh(email.Trim(), ("PVI: Thư bảo lãnh: " + DateTime.Now.Year.ToString() + "-" + hsgd_ctu.MaDonvi + "-" + hsgd_ctu.SoHsgd + " cho xe ô tô BKS: " + hsgd_ctu.BienKsoat + " của " + hsgd_ctu.TenKhach), strFileNameLocalPdf, fbl);
                                        emailSent = true;
                                        ghichu_gui += email + "; ";
                                    }
                                });
                            }



                            if (!string.IsNullOrEmpty(receiving_emails))
                            {
                                string[] separatedEmails1 = receiving_emails.Split(";"); // Chia email gửi
                                Array.ForEach(separatedEmails1, email =>
                                {
                                    if (true)//PheDuyetHelper.validateEmail(email))
                                    {
                                        // Khi nào lên UAt thì mở ra
                                        PheDuyetHelper.SendEmail_BaoLanh(email.Trim(), ("PVI: Thư bảo lãnh: " + DateTime.Now.Year.ToString() + "-" + hsgd_ctu.MaDonvi + "-" + hsgd_ctu.SoHsgd + " cho xe ô tô BKS: " + hsgd_ctu.BienKsoat + " của " + hsgd_ctu.TenKhach), strFileNameLocalPdf, fbl);
                                        emailSent = true;
                                        ghichu_gui += email + "; ";
                                    }
                                });
                            }


                            if (!string.IsNullOrEmpty(hsgd_dx_ct.BlDsphone))
                            {
                                string[] separatedPhones = hsgd_dx_ct.BlDsphone.Replace(" ", "").Split(";"); // Chia số điện thoại gửi
                                Array.ForEach(separatedPhones, phone =>
                                {
                                    // Khi nào lên UAt thì mở ra
                                    PheDuyetHelper.SendSMS(phone.Trim(), hsgd_ctu.BienKsoat, hsgd_ctu.SoSeri.ToString(), hsgd_ctu.NgayTthat.Value.Date.ToString(), hsgd_ctu.DienThoai, "BL", hsgd_ctu.PrKey);
                                    smsSent = true;
                                });

                                //ghichu_gui += "Email: " + hoSoGiamDinh.BlDsemail.Replace(" ", "");

                            }

                            if (!string.IsNullOrEmpty(receiving_phones))
                            {
                                ghichu_gui += "SDT: ";
                                string[] separatedPhones1 = receiving_phones.Replace(" ", "").Split(";"); // Chia số điện thoại gửi
                                Array.ForEach(separatedPhones1, phone =>
                                {
                                    // Khi nào lên UAt thì mở ra
                                    PheDuyetHelper.SendSMS(phone.Trim(), hsgd_ctu.BienKsoat, hsgd_ctu.SoSeri.ToString(), hsgd_ctu.NgayTthat.Value.Date.ToString(), hsgd_ctu.DienThoai, "BL", hsgd_ctu.PrKey);
                                    smsSent = true;
                                    ghichu_gui += phone.Trim() + "; ";
                                });

                                //ghichu_gui += "Email: " + hoSoGiamDinh.BlDsemail.Replace(" ", "");

                            }

                        }



                        // kiểm tra và xóa file ở local 
                        if (System.IO.File.Exists(strFileNameLocalPdf))
                        {
                            System.IO.File.Delete(strFileNameLocalPdf);
                        }

                        // Call update trạng thái
                        if (emailSent || smsSent)
                        {
                            var nhat_ky = new NhatKy();
                            nhat_ky.FrKey = hsgd_ctu.PrKey;
                            nhat_ky.MaTtrangGd = "BLDT";
                            nhat_ky.TenTtrangGd = PheDuyetHelper.Map_tinh_trang("BLDT");
                            nhat_ky.GhiChu = "Gửi bảo lãnh điện tử. Email: " + ghichu_gui;
                            nhat_ky.NgayCapnhat = DateTime.Now;
                            nhat_ky.MaUser = currentUser.Oid;

                            await _context.NhatKies.AddAsync(nhat_ky);
                            await _context.SaveChangesAsync();
                            return "Gửi bảo lãnh thành công";

                            //_logger.Information("Gui Bao Lanh pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + ", pr_key_hsbt_ct = " + pr_key_hsbt_ct + " success");
                        }
                        else
                        {
                            // Nhớ mở ra khi UAT

                            //var nhat_ky = new NhatKy();
                            //nhat_ky.FrKey = hoSoGiamDinh.PrKey;
                            //nhat_ky.MaTtrangGd = "BLDT";
                            //nhat_ky.TenTtrangGd = PheDuyetHelper.Map_tinh_trang("BLDT");
                            //nhat_ky.GhiChu = "Gửi bảo lãnh điện tử: Ký KHÔNG THÀNH CÔNG. " ;
                            //nhat_ky.NgayCapnhat = DateTime.Now;
                            //nhat_ky.MaUser = currentUser.Oid;

                            //await _context.NhatKies.AddAsync(nhat_ky);
                            //await _context.SaveChangesAsync();

                            _logger.Information("Gui Bao Lanh THAT BAI, from " + hsgd_dx_ct.BlDsemail + " " + hsgd_dx_ct.BlDsphone);
                            return "Gửi bảo lãnh không thành công";
                        }


                    }
                    else
                    {
                        return "Hồ sơ không tồn tại";
                    }
                }
                else
                {
                    return "User không tồn tại";
                }
            }
            catch (Exception ex)
            {
                //_logger.Error("dbContextTransaction Exception when pheDuyetBaoLanh: " + ex.ToString());
                //_logger.Error("dbContextTransaction Exception when pheDuyetBaoLanh: " + ex.ToString());
                //_logger.Error("Error record: " + JsonConvert.SerializeObject(pr_key));
                _context.Dispose();
                _logger.Error(ex.Message);
                return "Khâu gửi bảo lãnh ở Repo lỗi";
            }
        }


        //// Phê duyệt bảo lãnh
        //// khanhlh - 19/09/2024
        //// Khi bảo lãnh của sản phẩm này được phê duyệt, 1 record mới sẽ được lưu vào bảng HSGD_BL. 
        //public async Task<string> pheDuyetBaoLanh_Individual (int pr_key, int pr_key_hsgd_dx_ct, int bl1, int bl2, int bl3, int bl4, int bl5, int bl6, int bl7, int bl8, int bl9, string bl_tailieubs, string bl_dsemail, string bl_dsphone, string? ma_donvi_tt, string currentUserEmail)
        //{
        //    try
        //    {
        //        DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
        //        HsgdCtu hoSoGiamDinh = _context.HsgdCtus.Where(x => x.PrKey == pr_key).FirstOrDefault();

        //        if (currentUser != null && hoSoGiamDinh != null)
        //        {
        //            int[] acceptedUserTypes = new int[] { 1, 6, 9, 10, 11 };
        //            List<PquyenCnang> list_phanquyen = PheDuyetHelper.Check_PquyenCnang(currentUser);

        //            // Kiểm tra phân quyền của User
        //            if (Array.Exists(acceptedUserTypes, x => x == currentUser.LoaiUser) || PheDuyetHelper.check_UyQuyen_HoSoTPC(currentUser).Equals("PHEDUYET_HS") || PheDuyetHelper.check_UyQuyen_HoSoTPC(currentUser).Equals("FULL_QUYEN") || (list_phanquyen.Count > 0 && list_phanquyen.Exists(x => x.LoaiQuyen.Equals("BAOLANHDT"))))
        //            {
        //                // Kiểm tra xem hiện tại dã có bảo lãnh chưa
        //                HsgdBl check_bao_lanh = _context.HsgdBls.Where(x => x.PrKeyHsgdDxCt == pr_key_hsgd_dx_ct).FirstOrDefault();

        //                // Chỉ các hồ sơ đã duyệt và chưa có bảo lãnh thì mới duyệt được bảo lãnh
        //                if (hoSoGiamDinh.MaTtrangGd == "6" && check_bao_lanh == null)
        //                {
        //                    if (!String.IsNullOrEmpty(hoSoGiamDinh.MaDonviTt) &&  !String.IsNullOrEmpty(ma_donvi_tt))
        //                    {
        //                        HsgdDxCt dxCt = _context.HsgdDxCts.Where(x => x.PrKey == pr_key_hsgd_dx_ct).FirstOrDefault();
        //                        List<HsbtCtView> ListPhaiTraBt = _dx_repo.GetListPhaiTraBT(hoSoGiamDinh.PrKey);

        //                        HsbtCtView phaiTraBT = dxCt != null ? ListPhaiTraBt.Where(x => x.PrKey == dxCt.PrKeyHsbtCt).FirstOrDefault() : null;

        //                        if (phaiTraBT != null)
        //                        {
        //                            HsgdBl BaoLanhSanPham = new HsgdBl();

        //                            BaoLanhSanPham.FrKey = hoSoGiamDinh.PrKey;
        //                            BaoLanhSanPham.
        //                            hoSoGiamDinh.BlPdbl = 1;

        //                            hoSoGiamDinh.Bl1 = bl1;
        //                            hoSoGiamDinh.Bl2 = bl2;
        //                            hoSoGiamDinh.Bl3 = bl3;
        //                            hoSoGiamDinh.Bl4 = bl4;
        //                            hoSoGiamDinh.Bl5 = bl5;
        //                            hoSoGiamDinh.Bl6 = bl6;
        //                            hoSoGiamDinh.Bl7 = bl7;
        //                            hoSoGiamDinh.Bl8 = bl8;
        //                            hoSoGiamDinh.Bl9 = bl9;

        //                            List<HsbtCtView> listPhaiTraBT = _dx_repo.GetListPhaiTraBT(hoSoGiamDinh.PrKey);
        //                            string mailList = "";
        //                            string phoneList = "";
        //                            if (listPhaiTraBT.Count > 0)
        //                            {
        //                                DmGaRa garaGiamDinh = _context.DmGaRas.Where(x => x.MaGara == listPhaiTraBT[0].MaGara).FirstOrDefault();
        //                                mailList = garaGiamDinh != null ? garaGiamDinh.EmailGara : "";
        //                                phoneList = garaGiamDinh != null ? garaGiamDinh.DienThoaiGara : "";
        //                            }

        //                            hoSoGiamDinh.BlTailieubs = bl_tailieubs;
        //                            hoSoGiamDinh.BlDsemail += (mailList + ";" + bl_dsemail);
        //                            hoSoGiamDinh.BlDsphone += (phoneList + ";" + bl_dsphone);
        //                            hoSoGiamDinh.MaDonviTt = ma_donvi_tt;

        //                            _context.HsgdCtus.Update(hoSoGiamDinh);

        //                            // Tạo nhật ký 
        //                            NhatKy diary = new NhatKy
        //                            {
        //                                FrKey = hoSoGiamDinh.PrKey,
        //                                MaTtrangGd = "DBL",
        //                                TenTtrangGd = "Duyệt bảo lãnh", // Do chỉ định 1 trạng thái nên gán thẳng sẽ nhanh hơn.
        //                                GhiChu = "Duyệt bảo lãnh. Email gửi bảo lãnh ghi nhận: " + (mailList + ";" + bl_dsemail) + ". SDT gửi bảo lãnh ghi nhận: " + (phoneList + ";" + bl_dsphone),
        //                                NgayCapnhat = DateTime.Now,
        //                                MaUser = currentUser.Oid
        //                            };

        //                            await _context.NhatKies.AddAsync(diary);
        //                            await _context.SaveChangesAsync();
        //                            return hoSoGiamDinh.PrKey.ToString();
        //                        } else
        //                        {
        //                            return "Vui lòng đảm bảo sản phẩm này đã được nhập đề xuất chi tiết";
        //                        }

        //                    }
        //                    else
        //                    {
        //                        return "Hồ sơ chưa có có đơn vị thanh toán, vui lòng kiểm tra lại";
        //                    }
        //                }
        //                else
        //                {
        //                    return "Bảo lãnh này đã được phê duyệt rồi; hoặc hồ sơ giám định này chưa được duyệt.";
        //                }
        //            }
        //            else
        //            {
        //                return "Người dùng không có quyền duyệt bảo lãnh";
        //            }
        //        }
        //        else
        //        {
        //            return "Hồ sơ không tồn tại";

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error("dbContextTransaction Exception when pheDuyetBaoLanh: " + ex.ToString());
        //        _logger.Error("Error record: " + JsonConvert.SerializeObject(pr_key));
        //        _context.Dispose();
        //        throw;
        //    }
        //}




        // Giống hệt hàm ở bên helper, nối vào để gọi đến service.
        public CombinedBaoLanhResult BaoLanh_GetListOfReplacable(decimal prKey, decimal pr_key_hsbt_ct, string currentUseEmail, string? ma_donvi_tt)
        {
            try
            {
                DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUseEmail)).FirstOrDefault();

                if (currentUser != null)
                {
                    return PheDuyetHelper.BaoLanh_GetListOfReplacable(prKey, pr_key_hsbt_ct, currentUser, ma_donvi_tt);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        // Giống hệt hàm ở bên helper, nối vào để gọi đến service.
        public CombinedBaoLanhResult BaoLanh_GetListOfReplacable_Preview(decimal prKey, decimal pr_key_hsbt_ct, string currentUseEmail, string? ma_donvi_tt)
        {
            try
            {
                DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUseEmail)).FirstOrDefault();

                if (currentUser != null)
                {
                    return PheDuyetHelper.BaoLanh_GetListOfReplacable_Preview(prKey, pr_key_hsbt_ct, currentUser, ma_donvi_tt);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public async Task<string> UpdateURLImage(UploadFileContent listFile, int pr_key_ct, int pr_key, Guid oid)
        {
            var utilityHelper = new UtilityHelper(_logger);
            string path = "";
            try
            {

                var hsgdCtu = await _context.HsgdCtus.Where(x => x.PrKey == pr_key).FirstOrDefaultAsync();

                if (hsgdCtu != null)
                {
                    var hsgd_ct = await _context.HsgdCts.Where(x => x.PrKey == pr_key_ct).FirstOrDefaultAsync();
                    if (hsgd_ct != null)
                    {
                        string ma_donvi = hsgdCtu.MaDonvi;
                        string nam = DateTime.Now.Year.ToString();
                        string thang = DateTime.Now.Month.ToString();
                        string so_hsgd = hsgdCtu.SoHsgd;

                        // Construct path
                        path = string.Format("{0}{1}\\{2}\\{3}\\{4}", _configuration["DownloadSettings:PathSaveFile"], nam, thang, ma_donvi, so_hsgd);
                        path += "\\";
                        string fileName = "";
                        string uploadFilePath = "";
                        //ASPxUploadControl uploadControl = sender as ASPxUploadControl;
                        string url_upload = _configuration["DownloadSettings:UlpoadServer"] ?? "";
                        if (listFile != null && listFile.FileData != null)
                        {
                            //var hsgdCt = _context.HsgdCts.Where(x => x.PrKey == pr_key).FirstOrDefault();

                            fileName = listFile.FileName;
                            //fname = file.FileName;
                            if (!string.IsNullOrEmpty(fileName))
                            {

                                string extension = Path.GetExtension(fileName);
                                uploadFilePath = utilityHelper.UploadFile_ToAPI(listFile.FileData, extension, path, url_upload, true);
                                if (string.IsNullOrEmpty(uploadFilePath))
                                {
                                    return "Lỗi khi upload file";
                                }

                            }
                            else
                            {
                                return "Lỗi upload file";
                            }

                            if (hsgdCtu.MaTtrangGd == "2")
                            {
                                hsgdCtu.MaTtrangGd = "9";
                                var nhatKy = new NhatKy
                                {
                                    FrKey = hsgdCtu.PrKey,
                                    MaTtrangGd = "9",
                                    TenTtrangGd = "Hồ sơ TPC đang xử lý",
                                    GhiChu = "upload ảnh từ máy tính HS chuyển đang giám định",
                                    NgayCapnhat = DateTime.Now,
                                    MaUser = oid
                                };
                                _context.NhatKies.Add(nhatKy);
                                await _context.SaveChangesAsync();
                            }
                            string cdn247 = _configuration["DownloadSettings:CDN247"] ?? "";




                            //_context.HsgdCts.Add(hsgdCt);
                            await _context.HsgdCts.Where(x => x.PrKey == pr_key_ct).ExecuteUpdateAsync(s => s.SetProperty(a => a.PathFile, a => uploadFilePath)
                                                                                                          .SetProperty(a => a.PathUrl, a => (uploadFilePath).Replace("\\\\pvi.com.vn\\p247_upload_new", cdn247 + "/upload_01").Replace("\\", "/"))
                                                                                                          .SetProperty(a => a.PathOrginalFile, a => uploadFilePath));


                            await _context.SaveChangesAsync();

                            return "Success";
                        }
                        else
                        {
                            return "Error";
                        }

                    }
                    else
                    {
                        return "Error";
                    }
                }
                else
                {
                    return "Fail";

                }
            }
            catch (Exception ex)
            {

                return "Error";
            }
        }

        public async Task<string> UploadAppraisalImage(UploadFileContent listFile, int pr_key, Guid oid, int stt, string maHmuc, string dienGiai, string maHmucSc)
        {
            var utilityHelper = new UtilityHelper(_logger);
            string path = "";
            try
            {

                var hsgdCtu = await _context.HsgdCtus.Where(x => x.PrKey == pr_key).FirstOrDefaultAsync();

                if (hsgdCtu != null)
                {
                    string ma_donvi = hsgdCtu.MaDonvi;
                    string nam = DateTime.Now.Year.ToString();
                    string thang = DateTime.Now.Month.ToString();
                    string so_hsgd = hsgdCtu.SoHsgd;

                    // Construct path
                    path = string.Format("{0}{1}\\{2}\\{3}\\{4}", _configuration["DownloadSettings:PathSaveFile"], nam, thang, ma_donvi, so_hsgd);
                    path += "\\";
                    string fileName = "";
                    string uploadFilePath = "";
                    //ASPxUploadControl uploadControl = sender as ASPxUploadControl;
                    string url_upload = _configuration["DownloadSettings:UlpoadServer"] ?? "";
                    if (listFile != null && listFile.FileData != null)
                    {
                        //string ffileName_tmp = "";

                        fileName = listFile.FileName;
                        //fname = file.FileName;
                        if (!string.IsNullOrEmpty(fileName))
                        {

                            string extension = Path.GetExtension(fileName);
                            uploadFilePath = utilityHelper.UploadFile_ToAPI(listFile.FileData, extension, path, url_upload, true);
                            if (string.IsNullOrEmpty(uploadFilePath))
                            {
                                return "Lỗi khi upload file";
                            }

                        }
                        else
                        {
                            return "Lỗi upload file";
                        }

                        if (hsgdCtu.MaTtrangGd == "2")
                        {
                            hsgdCtu.MaTtrangGd = "9";
                            var nhatKy = new NhatKy
                            {
                                FrKey = hsgdCtu.PrKey,
                                MaTtrangGd = "9",
                                TenTtrangGd = "Hồ sơ TPC đang xử lý",
                                GhiChu = "upload ảnh từ máy tính HS chuyển đang giám định",
                                NgayCapnhat = DateTime.Now,
                                MaUser = oid
                            };
                            _context.NhatKies.Add(nhatKy);
                            await _context.SaveChangesAsync();
                        }
                        string cdn247 = _configuration["DownloadSettings:CDN247"] ?? "";
                        var hsgdCt = new HsgdCt
                        {
                            FrKey = hsgdCtu.PrKey,
                            Stt = stt,
                            MaHmuc = maHmuc,
                            DienGiai = dienGiai,
                            PathFile = uploadFilePath,
                            NgayChup = DateTime.Now,
                            PathUrl = (uploadFilePath).Replace("\\\\pvi.com.vn\\p247_upload_new", cdn247 + "/upload_01").Replace("\\", "/"),
                            PathOrginalFile = uploadFilePath,
                            MaHmucSc = maHmucSc

                        };


                        _context.HsgdCts.Add(hsgdCt);
                        await _context.SaveChangesAsync();
                        if (hsgdCtu.MaLhsbt == "3")
                        {

                            var hsgdCtBtHo = new HsgdCt
                            {
                                FrKey = Convert.ToInt32(hsgdCtu.PrKeyBtHo),
                                Stt = stt,
                                MaHmuc = maHmuc,
                                DienGiai = dienGiai,
                                PathFile = uploadFilePath,
                                NgayChup = DateTime.Now,
                                PathUrl = (uploadFilePath).Replace(_configuration["DownloadSettings:PathSaveFile"], cdn247 + "/upload_01/").Replace("\\", "/"),
                                PathOrginalFile = uploadFilePath,
                                MaHmucSc = maHmucSc
                            };

                            // Insert the new record into hsgd_ct
                            _context.HsgdCts.Add(hsgdCtBtHo);
                            await _context.SaveChangesAsync();
                        }
                        return "Success";
                    }
                    else
                    {
                        return "Error";
                    }
                }
                else
                {
                    return "Fail";

                }
            }
            catch (Exception ex)
            {

                return "Error";
            }

        }

        public DownloadFileResult DownloadTtrinh11_MDF1(int pr_key)
        {
            DownloadFileResult result = new DownloadFileResult();
            try
            {
                var filePath = _context.HsgdCts.Where(x => x.PrKey == pr_key).FirstOrDefault();
                if (filePath.PathFile != null)
                {
                    string url_download = _configuration["DownloadSettings:DownloadServer"] ?? "";
                    result = UtilityHelper.DownloadFile_ToAPI(filePath.PathFile, url_download);
                }
                else
                {
                    result.Status = "-500";
                    result.Message = "Invalid FilePath";
                }


            }
            catch (Exception ex)
            {
            }
            return result;
        }

        public async Task<List<GDDKResponse>> GetAnhGDDK(int pr_key)
        {

            var hsgdCtuData = await _context.HsgdCtus
                .Where(h => h.PrKey == pr_key)
                .Select(h => new
                {
                    PrKey = h.PrKey,
                    SoSeri = ReplaceAllCharacters(h.SoSeri.ToString(), "^a-z0-9"),
                    BienKsoat = ReplaceAllCharacters(h.BienKsoat, "^a-z0-9"),
                    SoDonbh = h.SoDonbh
                })
                .FirstOrDefaultAsync();

            if (hsgdCtuData == null)
            {
                return new List<GDDKResponse>();
            }

            var soSeri = hsgdCtuData.SoSeri;
            var bienKsoat = hsgdCtuData.BienKsoat;
            var soDonbh = hsgdCtuData.SoDonbh;


            var gddkCtuData = await (from gddk in _context.GddkCtus
                                     join u in _context.DmUsers on gddk.MaUser equals u.Oid.ToString() into userGroup
                                     from ug in userGroup.DefaultIfEmpty()
                                     join d in _context.DmDonvis on gddk.MaDonvi equals d.MaDonvi into donviGroup
                                     from dg in donviGroup.DefaultIfEmpty()
                                     where gddk.SoSeri == Convert.ToDecimal(soSeri)
                                       && gddk.BienKsoat == bienKsoat &&
                                       gddk.SoDonbh == soDonbh

                                     orderby gddk.PrKey descending
                                     select new
                                     {
                                         gddk.PrKey,
                                         gddk.SoSeri,
                                         gddk.BienKsoat,
                                         gddk.SoDonbh,
                                         gddk.SoKhung,
                                         NgayCtu = ConvertDateTime.ConvertSmallDateTimeToString(gddk.NgayCtu),
                                         TenNgtao = ug.TenUser,
                                         TenDonvi = dg.TenDonvi
                                     }).FirstOrDefaultAsync();


            if (gddkCtuData == null)
            {

                gddkCtuData = await (from gddk in _context.GddkCtus
                                     join u in _context.DmUsers on gddk.MaUser equals u.Oid.ToString() into userGroup
                                     from ug in userGroup.DefaultIfEmpty()
                                     join d in _context.DmDonvis on gddk.MaDonvi equals d.MaDonvi into donviGroup
                                     from dg in donviGroup.DefaultIfEmpty()
                                     where gddk.SoSeri == Convert.ToDecimal(soSeri)
                                       && gddk.BienKsoat == bienKsoat
                                     orderby gddk.PrKey descending
                                     select new
                                     {
                                         gddk.PrKey,
                                         gddk.SoSeri,
                                         gddk.BienKsoat,
                                         gddk.SoDonbh,
                                         gddk.SoKhung,
                                         NgayCtu = ConvertDateTime.ConvertSmallDateTimeToString(gddk.NgayCtu),
                                         TenNgtao = ug.TenUser,
                                         TenDonvi = dg.TenDonvi
                                     }).FirstOrDefaultAsync();
            }
            if (gddkCtuData == null && !string.IsNullOrEmpty(hsgdCtuData.SoSeri))
            {
                gddkCtuData = await (from gddk in _context.GddkCtus
                                     join u in _context.DmUsers on gddk.MaUser equals u.Oid.ToString() into userGroup
                                     from ug in userGroup.DefaultIfEmpty()
                                     join d in _context.DmDonvis on gddk.MaDonvi equals d.MaDonvi into donviGroup
                                     from dg in donviGroup.DefaultIfEmpty()
                                     where gddk.SoSeri == Convert.ToDecimal(soSeri)

                                     orderby gddk.PrKey descending
                                     select new
                                     {
                                         gddk.PrKey,
                                         gddk.SoSeri,
                                         gddk.BienKsoat,
                                         gddk.SoDonbh,
                                         gddk.SoKhung,
                                         NgayCtu = ConvertDateTime.ConvertSmallDateTimeToString(gddk.NgayCtu),
                                         TenNgtao = ug.TenUser,
                                         TenDonvi = dg.TenDonvi
                                     }).FirstOrDefaultAsync();
            }





            if (gddkCtuData == null)
            {

                return new List<GDDKResponse>();
            }

            string cdn247Normal = _configuration["DownloadSettings:CDN247NORMAL"];
            string cdn247 = _configuration["DownloadSettings:CDN247"];

            var images = (from gddk in _context.GddkCts
                          where gddk.FrKey == gddkCtuData.PrKey
                          select new AnhGDDKData
                          {
                              ViTri = gddk.ViDoChup + ";" + gddk.KinhDoChup,
                              Thumbnail = gddk.PathFile.Replace("\\\\pvi.com.vn\\DATA\\P247_upload", cdn247 + "/upload").Replace("\\", "/"),
                              PathUrl = gddk.PathUrl.Replace("pvi247.pvi.com.vn", cdn247Normal).Replace(@"\", "/"),
                              PrKey = gddk.PrKey,
                              PathFile = gddk.PathFile.Replace("\\", "/"),
                          }).ToList();


            if (images.Count > 0)
            {
                var uniqueDirectories = new HashSet<string>();
                foreach (var image in images)
                {

                    var pathFile = Path.Combine(Path.GetDirectoryName(image.PathFile.Replace(cdn247Normal, "pvi247.pvi.com.vn")), "1.jpg");


                    int dIndex = pathFile.IndexOf("CSSK_upload");
                    string dir_source = Path.GetDirectoryName(pathFile) + "\\";
                    if (uniqueDirectories.Contains(dir_source))
                    {
                        continue;
                    }
                    uniqueDirectories.Add(dir_source);

                    string dir_target = dir_source.Replace("pvi.com.vn", "192.168.250.77")
                                                   .Replace("\\DATA", "");


                    if (dIndex > -1)
                    {
                        dir_target = dir_target.Replace("DATA\\", "P247_Upload_New\\")
                                               .Replace("CSSK_upload\\", "TCD\\CLAIM_XCG\\")
                                               .Replace("\\pvi\\data\\GCNDT_Upload", "192.168.250.77\\P247_Upload_New");
                    }
                    else
                    {
                        //dir_source = dir_source.Replace("\\DATA", "");
                        //.Replace("P247_upload\\", "P247_Upload_New\\");
                    }


                    pathFile = UtilityHelper.CopyFile(dir_source, dir_target);

                }
            }
            var response = new GDDKResponse
            {
                PrKey = gddkCtuData.PrKey,
                SoSeri = gddkCtuData.SoSeri,
                BienKsoat = gddkCtuData.BienKsoat,
                SoDonBh = gddkCtuData.SoDonbh,
                SoKhung = gddkCtuData.SoKhung,
                NgayCtu = gddkCtuData.NgayCtu,
                TenNgTao = gddkCtuData.TenNgtao,
                TenDonVi = gddkCtuData.TenDonvi,
                AnhGDDKData = images
            };

            return new List<GDDKResponse> { response };
        }



        public static string ReplaceAllCharacters(string input, string matchExpression)
        {

            Regex regex = new Regex(matchExpression, RegexOptions.Compiled);


            string result = regex.Replace(input, string.Empty);


            return result.ToUpper();
        }


        public async Task<string> UpdateAppraisalImage(int pr_key, string dienGiai, string maHmuc, int stt, string maHmucSc)
        {
            var hsgd_ct = await _context.HsgdCts.Where(x => x.PrKey == pr_key).Select(x => new HsgdCt
            {
                Stt = x.Stt,
                DienGiai = x.DienGiai,
                MaHmuc = x.MaHmuc,
            }).FirstOrDefaultAsync();

            if (hsgd_ct != null)
            {

                dienGiai = dienGiai ?? hsgd_ct.DienGiai;
                maHmuc = maHmuc ?? hsgd_ct.MaHmuc;
                await _context.HsgdCts.Where(x => x.PrKey == pr_key).ExecuteUpdateAsync(s => s.SetProperty(a => a.DienGiai, a => dienGiai).SetProperty(a => a.MaHmuc, a => maHmuc).SetProperty(a => a.Stt, a => stt).SetProperty(a => a.MaHmucSc, a => maHmucSc));

                await _context.SaveChangesAsync();
                return "Success";
            }
            else
            {
                return "Error";
            }
        }
        public async Task<string> ChuyenAnhGDTT(string soHsgdChuyen, string soHsgdNhan)
        {
            try
            {
                var hsgdChuyen = await _context.HsgdCtus.Where(x => x.SoHsgd == soHsgdChuyen).Select(x => x.PrKey).FirstOrDefaultAsync();
                if (hsgdChuyen == 0)
                {
                    return "Không tồn tại số hồ sơ chuyển";
                }
                var hsgdNhan = await _context.HsgdCtus.Where(x => x.SoHsgd == soHsgdNhan).Select(x => x.PrKey).FirstOrDefaultAsync();
                if (hsgdNhan == 0)
                {
                    return "Không tìm thấy số hồ sơ nhận";
                }
                var imagesToTransfer = from ct in _context.HsgdCts
                                       where ct.FrKey == hsgdChuyen
                                       select new HsgdCt
                                       {
                                           FrKey = hsgdNhan,
                                           Stt = ct.Stt,
                                           NhomAnh = ct.NhomAnh,
                                           PathFile = ct.PathFile,
                                           NgayChup = ct.NgayChup,
                                           ViDoChup = ct.ViDoChup,
                                           KinhDoChup = ct.KinhDoChup,
                                           DienGiai = ct.DienGiai,
                                           PathUrl = ct.PathUrl,
                                           PathOrginalFile = ct.PathOrginalFile,
                                           Android = ct.Android
                                       };


                await _context.HsgdCts.AddRangeAsync(imagesToTransfer);
                await _context.SaveChangesAsync();
                return "Success";
            }
            catch (Exception ex)
            {
                _logger.Error("Error: " + ex.ToString());
                return "Error";
            }

        }

        public async Task<List<HsgdDgCt>> GetAnhDuyetGia(int pr_key, bool loai_dg)
        {
            var prKeyHsgdDg = await _context.HsgdDgs.Where(x => x.FrKey == pr_key && x.LoaiDg == loai_dg).Select(x => x.PrKey).FirstOrDefaultAsync();
            string cdn247Normal = _configuration["DownloadSettings:CDN247NORMAL"];
            var listImage = await _context.HsgdDgCts.Where(x => x.FrKey == prKeyHsgdDg)
                                   .Select(x => new HsgdDgCt
                                   {

                                       PrKey = x.PrKey,
                                       FrKey = x.FrKey,

                                       PathFile = x.PathFile.Replace("\\", "/"),

                                       PathUrl = x.PathUrl.Replace("pvi247.pvi.com.vn", cdn247Normal).Replace(@"\", "/"),
                                       PathOrginalFile = x.PathOrginalFile.Replace("\\", "/"),

                                   }).ToListAsync();


            if (listImage.Count > 0)
            {
                var uniqueDirectories = new HashSet<string>();
                foreach (var image in listImage)
                {

                    var pathFile = Path.Combine(Path.GetDirectoryName(image.PathFile), "1.jpg");


                    int dIndex = pathFile.IndexOf("CSSK_upload");
                    string dir_source = Path.GetDirectoryName(pathFile) + "\\";
                    if (uniqueDirectories.Contains(dir_source))
                    {
                        continue;
                    }
                    uniqueDirectories.Add(dir_source);

                    string dir_target = dir_source.Replace("pvi.com.vn", "192.168.250.77");

                    if (dIndex > -1)
                    {
                        dir_target = dir_target.Replace("DATA\\", "P247_Upload_New\\")
                                               .Replace("CSSK_upload\\", "TCD\\CLAIM_XCG\\")
                                               .Replace("\\pvi\\data\\GCNDT_Upload", "192.168.250.77\\P247_Upload_New");
                    }
                    else
                    {
                        dir_source = dir_source.Replace("\\DATA", "")
                                               .Replace("P247_upload\\", "P247_Upload_New\\");
                    }


                    pathFile = UtilityHelper.CopyFile(dir_source, dir_target);

                }
                return listImage;
            }
            else
            {
                return null;
            }
        }


        public async Task<List<HsgdDg>> GetThongTinDuyetGia(int pr_key)
        {
            var listDuyetGia = await _context.HsgdDgs.Where(x => x.FrKey == pr_key).ToListAsync();
            //ban dau : false  cuoi: true
            var checkDuyetGiaBanDau = await _context.HsgdDgs.Where(x => x.FrKey == pr_key && x.LoaiDg == false).AnyAsync();
            if (!checkDuyetGiaBanDau)
            {
                var newHsgdDg = new HsgdDg
                {
                    FrKey = pr_key,
                    SoTien = 0,
                    LoaiDg = false,
                    Hienthi=true

                };
                _context.HsgdDgs.Add(newHsgdDg);
            }
            var checkDuyetGiaCuoi = await _context.HsgdDgs.Where(x => x.FrKey == pr_key && x.LoaiDg == true).AnyAsync();
            if (!checkDuyetGiaCuoi)
            {
                var newHsgdDg = new HsgdDg
                {
                    FrKey = pr_key,
                    SoTien = 0,
                    LoaiDg = true,
                    Hienthi=false

                };
                _context.HsgdDgs.Add(newHsgdDg);
            }
            await _context.SaveChangesAsync();
            if (!checkDuyetGiaBanDau || !checkDuyetGiaCuoi)
            {
                listDuyetGia = await _context.HsgdDgs.Where(x => x.FrKey == pr_key).ToListAsync();
            }

            return listDuyetGia;
        }
        public async Task<string> UpdateAnhDuyetGia(int pr_key, bool loai_dg, string de_xuat, decimal so_tien)
        {
            try
            {
                var hsgdDg = await _context.HsgdDgs.Where(x => x.FrKey == pr_key && x.LoaiDg == loai_dg).FirstOrDefaultAsync();
                hsgdDg.DeXuat = de_xuat;
                hsgdDg.SoTien = so_tien;
                _context.HsgdDgs.Update(hsgdDg);
                await _context.SaveChangesAsync();
                return "Success";
            }
            catch (Exception ex)
            {
                _logger.Error("Error record: " + ex.Message.ToString());
                return "Error";
            }

        }
        public async Task<dynamic> ProcessCRMAsync(int pr_key)
        {
            var hsgdCtu = await _context.HsgdCtus.Where(x => x.PrKey == pr_key).FirstOrDefaultAsync();
            var pathCRM = hsgdCtu != null ? hsgdCtu.PathCrm : "";
            var soDonBh = hsgdCtu != null ? hsgdCtu.SoDonbh : "";
            var soSeri = hsgdCtu != null ? hsgdCtu.SoSeri.ToString() : "";
            var maTtrangGd = hsgdCtu.MaTtrangGd;
            var ngayTthat = (DateTime)hsgdCtu.NgayTthat;
            var ngayTbao = (DateTime)hsgdCtu.NgayTbao;
            if (!string.IsNullOrEmpty(pathCRM))
            {
                string url_download = _configuration["DownloadSettings:DownloadServer"] ?? "";
                var result1 = UtilityHelper.DownloadFile_ToAPI(pathCRM, url_download);
                return result1;
                //return $"/DownloadFile?localPath={WebUtility.UrlEncode(path_crm)}&fileName={WebUtility.UrlEncode(so_hsgd + ".crm.docx")}&fileType=docx";
            }

            var client = new RestClient(_configuration["AgentsCRM"]);
            var request = new RestRequest();
            request.Method = Method.Get;
            request.AddHeader("Authorization", "Bearer " + _configuration["TokenCRM"]);
            DownloadFileResult resultDownload = new DownloadFileResult();
            var response = await client.ExecuteAsync(request);
            _logger.Information("start deserialize object result CRM");
            if (string.IsNullOrEmpty(response.ToString()))
            {
                _logger.Information("No Response CRM ");
            }
            var dict_assig = JsonConvert.DeserializeObject<DmCrmAssig>(response.Content);

            var created_since = ngayTthat.AddDays(-1);
            var created_to = ngayTbao.AddDays(7);

            var body = new
            {
                @params = new
                {
                    created_since = created_since.ToString("yyyy-MM-ddTHH:mm:ssK"),
                    created_to = created_to.ToString("yyyy-MM-ddTHH:mm:ssK"),
                    custom_fields = new[]
                    {
                        new { field_id = 142, field_value = soDonBh },
                        new { field_id = 162, field_value = soSeri }
                    }
                }
            };

            client = new RestClient(_configuration["SearchCRM"]);
            request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("Authorization", "Bearer " + _configuration["TokenCRM"]);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(body);
            _logger.Information("start execute param CRM");
            var response_crm = await client.ExecuteAsync(request);
            var dict_crm = JsonConvert.DeserializeObject<DmCrm>(response_crm.Content);
            Microsoft.Office.Interop.Word.Application app = null;
            if (dict_crm.docs.Count > 0)
            {
                var pathw = _configuration["BaseDirectory"];
                var fileCRM = Path.Combine(pathw, _configuration["FileCRM"].TrimStart('\\'));
                var tempFile = Path.Combine(pathw, _configuration["TempFile"].TrimStart('\\'));
                var fieldIdCRM = _configuration["FieldIdCRM"];

                if (!System.IO.File.Exists(fileCRM))
                {
                    throw new FileNotFoundException("File CRM.docx does not exist on the system!");
                }

                var nameFileCopy = Guid.NewGuid().ToString().Replace("-", "");
                var strFolderLocal = Path.Combine(tempFile, "Words", nameFileCopy + ".docx");

                if (!Directory.Exists(Path.Combine(tempFile, "Words")))
                {
                    Directory.CreateDirectory(Path.Combine(tempFile, "Words"));
                }

                System.IO.File.Copy(fileCRM, strFolderLocal, true);
                File.SetAttributes(strFolderLocal, File.GetAttributes(strFolderLocal) & ~FileAttributes.ReadOnly); // Remove read-only attribute
                //Microsoft.Office.Interop.Word.Application app = null;
                Microsoft.Office.Interop.Word.Document doc = null;
                //try
                //{
                //    Type acType = Type.GetTypeFromProgID("Word.Application");
                //    app = (Microsoft.Office.Interop.Word.Application)Activator.CreateInstance(acType, true);
                //    if (app == null)
                //    {
                //        app = new Microsoft.Office.Interop.Word.Application();
                //    }
                //}
                //catch (COMException ex)
                //{
                //    app = new Microsoft.Office.Interop.Word.Application();
                //}
                //var doc = app.Documents.Open(strFolderLocal);
                //app.Visible = false;
                //doc.Activate();

                try
                {
                    app = new Microsoft.Office.Interop.Word.Application();
                    doc = app.Documents.Open(
                        FileName: strFolderLocal,
                        ReadOnly: false,
                        AddToRecentFiles: false,
                        Visible: false
                    );
                    app.Visible = false;
                    doc.Activate();
                    ContentHelper.FindAndReplace(app, "[created_at]", dict_crm.docs[0].created_at.ToString("dd/MM/yyyy HH:mm:ss"));
                    ContentHelper.FindAndReplace(app, "[ticket_no]", dict_crm.docs[0].ticket_no.ToString());

                    var lstFieldIdCRM = fieldIdCRM.Split(",").ToList();
                    foreach (var dr in lstFieldIdCRM)
                    {
                        foreach (var customField in dict_crm.docs[0].custom_fields)
                        {
                            if (dr.Equals($"[{customField.field_id}]"))
                            {
                                ContentHelper.FindAndReplaceCRM(app, dr, customField.field_value.Replace("<br>", Environment.NewLine));
                            }
                        }
                    }

                    foreach (var customField in dict_crm.docs[0].custom_fields)
                    {
                        if (customField.field_id == 134)
                        {
                            ContentHelper.FindAndReplaceCRM(app, "[134]", string.Format("{0:N0}", Convert.ToDouble(customField.field_value)));
                        }
                    }

                    foreach (var agent in dict_assig.Agents)
                    {
                        if (dict_crm.docs[0].assignee_id == agent.Id)
                        {
                            ContentHelper.FindAndReplaceCRM(app, "[username]", agent.Username);
                        }
                    }
                    if (doc.ReadOnly)
                    {
                        _logger.Error("Document is read-only before saving!");
                    }
                    else
                    {
                        doc.Save();
                        doc.SaveAs2(strFolderLocal);
                    }
                    //doc.Save();  // Force saving before SaveAs2()
                    //doc.SaveAs2(strFolderLocal);
                    //doc.Close();
                    //Marshal.ReleaseComObject(doc);
                    //app.Quit();
                    //Marshal.ReleaseComObject(app);
                    //GC.Collect();

                }
                catch (Exception ex)
                {
                    _logger.Error("Error word: " + ex.Message.ToString());
                }
                finally
                {
                    if (doc != null)
                    {
                        doc.Close(WdSaveOptions.wdDoNotSaveChanges);
                        Marshal.ReleaseComObject(doc);
                    }
                    if (app != null)
                    {
                        app.Quit();
                        Marshal.ReleaseComObject(app);
                    }
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                string? strFileNamePdf = "";
                //CreateFolder_FileSign(DateTime.Now.Year, DateTime.Now.Month, nameFileCopy, ".docx")
                try
                {
                    var base64WordFile = Convert.ToBase64String(System.IO.File.ReadAllBytes(strFolderLocal));
                    string folderUpload = _configuration["UploadSettings:FolderUpload"] ?? "";
                    string url_upload = _configuration["DownloadSettings:UlpoadServer"] ?? "";
                    var utilityHelper = new UtilityHelper(_logger);
                    strFileNamePdf = utilityHelper.UploadFile_ToAPI(base64WordFile, ".docx", folderUpload, url_upload, false);
                    File.Delete(strFolderLocal);
                }
                catch (Exception ex)
                {
                    _logger.Error("Error when upload file " + ex.Message.ToString());
                    resultDownload.Status = "-500";
                    resultDownload.Message = "Có lỗi xảy ra ";
                    return resultDownload;
                }


                if (maTtrangGd == "6")
                {
                    //strFileNamePdf = strFileNamePdf.Replace(@"\", @"\\");
                    var updateResult = _context.HsgdCtus.Where(x => x.PrKey == pr_key).FirstOrDefault();
                    if (updateResult != null)
                    {
                        updateResult.PathCrm = strFileNamePdf;
                        await _context.SaveChangesAsync();
                    }
                    //var sql = $"UPDATE hsgd_ctu SET path_crm = N'{strFileNamePdf}' WHERE pr_key = {pr_key}";
                    //await _webService.ExecuteSQL(sql);
                }



                //return $"/DownloadFile?localPath={WebUtility.UrlEncode(strFileNamePdf)}&fileName={WebUtility.UrlEncode(so_hsgd + ".crm.docx")}&fileType=docx";
                string url_download = _configuration["DownloadSettings:DownloadServer"] ?? "";
                resultDownload = UtilityHelper.DownloadFile_ToAPI(strFileNamePdf, url_download);
                return resultDownload;
            }
            else
            {
                resultDownload.Status = "-500";
                resultDownload.Message = "Không tồn tại thông tin CRM";
                return resultDownload;
                //throw new Exception("No CRM information found in the system.");
            }
        }
        public class DmCrm
        {
            public decimal NumFound { get; set; }
            public decimal start { get; set; }
            public List<DmCrmDocs> docs { get; set; }
        }
        public class DmCrmDocs
        {
            public decimal ticket_no { get; set; }
            public string ticket_subject { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public string ticket_status { get; set; }
            public string ticket_source { get; set; }
            public string ticket_priority { get; set; }
            public decimal ticket_id { get; set; }
            public decimal requester_id { get; set; }
            public decimal assignee_id { get; set; }
            public List<DmCrmCustomField> custom_fields { get; set; }
        }
        public class DmCrmCustomField
        {
            public decimal field_id { get; set; }
            public string field_value { get; set; }
            public string label { get; set; }
        }
        public class DmCrmAssig
        {
            public string Code { get; set; }
            public List<DmCrmAgents> Agents { get; set; }
        }
        public class DmCrmAgents
        {
            public decimal Id { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string PhoneNo { get; set; }
            public string AgentId { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        public async Task<string> UpdateURLAnhDuyetgia(UploadFileContent listFile, int prKey, int prKeyDgCt)
        {
            //loai_dg = true : Kết thúc duyệt giá
            //        = false : Bắt đầu duyệt giá  
            var utilityHelper = new UtilityHelper(_logger);
            string path = "";
            try
            {

                var hsgdCtu = await _context.HsgdCtus.Where(x => x.PrKey == prKey).FirstOrDefaultAsync();
                var hsgdDgCt = await _context.HsgdDgCts.Where(x => x.PrKey == prKeyDgCt).FirstOrDefaultAsync();
                if (hsgdDgCt != null)
                {
                    if (hsgdCtu != null)
                    {
                        string ma_donvi = hsgdCtu.MaDonvi;
                        string nam = DateTime.Now.Year.ToString();
                        string thang = DateTime.Now.Month.ToString();
                        string so_hsgd = hsgdCtu.SoHsgd;


                        path = string.Format("{0}{1}\\{2}\\{3}\\{4}", _configuration["DownloadSettings:PathSaveFile"], nam, thang, ma_donvi, so_hsgd);

                        path += "\\";
                        string fileName = "";
                        string uploadFilePath = "";

                        string url_upload = _configuration["DownloadSettings:UlpoadServer"] ?? "";
                        if (listFile != null && listFile.FileData != null)
                        {
                            string ffileName_tmp = "";

                            fileName = listFile.FileName;
                            //fname = file.FileName;
                            if (!string.IsNullOrEmpty(fileName))
                            {

                                string extension = Path.GetExtension(fileName);
                                uploadFilePath = utilityHelper.UploadFile_ToAPI(listFile.FileData, extension, path, url_upload, true);
                                if (string.IsNullOrEmpty(uploadFilePath))
                                {
                                    return "Lỗi khi upload file";
                                }

                            }
                            else
                            {
                                return "Lỗi upload file";
                            }

                            if (hsgdCtu.MaTtrangGd == "2")
                            {
                                hsgdCtu.MaTtrangGd = "3";

                            }
                            string cdn247 = _configuration["DownloadSettings:CDN247"] ?? "";



                            _context.HsgdDgCts.Where(x => x.PrKey == prKeyDgCt)
                                               .ExecuteUpdate(s => s
                                               .SetProperty(u => u.PathFile, uploadFilePath)
                                               .SetProperty(u => u.PathUrl, (uploadFilePath).Replace("\\\\pvi.com.vn\\p247_upload_new", cdn247 + "/upload_01").Replace("\\", "/"))
                                               .SetProperty(u => u.PathOrginalFile, uploadFilePath)
                                               );
                            await _context.SaveChangesAsync();

                            return "Success";
                        }
                        else
                        {
                            return "Error";
                        }
                    }
                    else
                    {
                        return "Fail";
                    }
                }
                else
                {
                    return "Error";
                }

            }
            catch (Exception ex)
            {

                return "Error";
            }
        }

        public async Task<string> UploadAnhDuyetgia(UploadFileContent listFile, int pr_key, bool loai_dg)
        {
            //loai_dg = true : Kết thúc duyệt giá
            //        = false : Bắt đầu duyệt giá  
            var utilityHelper = new UtilityHelper(_logger);
            string path = "";
            try
            {

                var hsgdCtu = await _context.HsgdCtus.Where(x => x.PrKey == pr_key).FirstOrDefaultAsync();
                var prKeyHsgdDg = await _context.HsgdDgs.Where(x => x.FrKey == pr_key && x.LoaiDg == loai_dg).Select(x => x.PrKey).FirstOrDefaultAsync();
                if (hsgdCtu != null)
                {
                    string ma_donvi = hsgdCtu.MaDonvi;
                    string nam = DateTime.Now.Year.ToString();
                    string thang = DateTime.Now.Month.ToString();
                    string so_hsgd = hsgdCtu.SoHsgd;


                    path = string.Format("{0}{1}\\{2}\\{3}\\{4}", _configuration["DownloadSettings:PathSaveFile"], nam, thang, ma_donvi, so_hsgd);

                    path += "\\";
                    string fileName = "";
                    string uploadFilePath = "";

                    string url_upload = _configuration["DownloadSettings:UlpoadServer"] ?? "";
                    if (listFile != null && listFile.FileData != null)
                    {
                        string ffileName_tmp = "";

                        fileName = listFile.FileName;
                        //fname = file.FileName;
                        if (!string.IsNullOrEmpty(fileName))
                        {

                            string extension = Path.GetExtension(fileName);
                            uploadFilePath = utilityHelper.UploadFile_ToAPI(listFile.FileData, extension, path, url_upload, true);
                            if (string.IsNullOrEmpty(uploadFilePath))
                            {
                                return "Lỗi khi upload file";
                            }

                        }
                        else
                        {
                            return "Lỗi upload file";
                        }

                        if (hsgdCtu.MaTtrangGd == "2")
                        {
                            hsgdCtu.MaTtrangGd = "3";

                        }
                        string cdn247 = _configuration["DownloadSettings:CDN247"] ?? "";
                        var hsgdDgCt = new HsgdDgCt
                        {
                            FrKey = prKeyHsgdDg,

                            PathFile = uploadFilePath,

                            PathUrl = (uploadFilePath).Replace("\\\\pvi.com.vn\\p247_upload_new", cdn247 + "/upload_01").Replace("\\", "/"),
                            PathOrginalFile = uploadFilePath,


                        };

                        _context.HsgdDgCts.Add(hsgdDgCt);
                        await _context.SaveChangesAsync();

                        return "Success";
                    }
                    else
                    {
                        return "Error";
                    }
                }
                else
                {
                    return "Fail";
                }
            }
            catch (Exception ex)
            {

                return "Error";
            }
        }

        // Lưu ý: API này để upload document mẫu lên server. 
        // Khi sử dụng, lưu ý thay localPath thành đường dẫn file tương ứng trên máy.
        public string uploadSampleFile(string localPath)
        {
            string sourcePath = localPath;

            //string sourcePath = @"C:/LHK/PVI/Temp247/rptBaoLanh.docx"; // Source Path
            string destinationPath = @"C:\LHK\PVI\Temp247\rptBaoLanhCopied.docx";

            //string pdfPath = @"C:\LHK\PVI\Temp247\sampleBaoLanhConverted.pdf";

            //string url_download = _configuration["DownloadSettings:DownloadServer"] ?? "";
            string folderUpload = _configuration["UploadSettings:FolderUpload"] ?? "";
            string url_upload = _configuration["UploadSettings:FolderUpload"] ?? "";

            byte[] sampleDoc = System.IO.File.ReadAllBytes(sourcePath);
            string base64String = Convert.ToBase64String(sampleDoc);
            string extension = Path.GetExtension(sourcePath);

            UtilityHelper helper = new UtilityHelper(_logger);
            string result = helper.UploadFile_ToAPI(base64String, extension, folderUpload, url_upload, true);

            return result;
        }


        public List<DmLdonBt> GetDmLdonBt()
        {
            var result = _context.DmLdonBts.ToList();
            return result;
        }

        public async Task<string> GetSoDonBh(int prKey)
        {
            var result = await _context.HsgdCtus.Where(x => x.PrKey == prKey).Select(x => x.SoDonbh).FirstOrDefaultAsync();
            return result;
        }
        public async Task<List<ProductInfoResponse>> GetProductInfo(string soDonBh, string soDonBhBs)
        {
            // Fetch data in a single query to avoid multiple database calls in the loop
            var result = await (from A in _context_pias.NvuBhtCtus
                                join B in _context_pias.NvuBhtCts
                                on A.PrKey equals B.FrKey
                                where A.SoDonbh == soDonBh
                                select new ProductInfoResponse
                                {
                                    MaSp = B.MaSp,
                                    TenSp = _context_pias.DmSps
                                        .Where(sp => sp.MaSp == B.MaSp)
                                        .Select(sp => sp.TenSp)
                                        .FirstOrDefault(),
                                    MtnGtbhNte = B.SoTienbhLke != 0 ? B.SoTienbhLke : B.SoTienbh,
                                    MaTte = A.MaTte,
                                    PrKeyNvuBhtCt = B.PrKey
                                }).ToListAsync();

            // Gather `tyleGiuLai` values in one go
            var prKeys = result.Select(r => r.PrKeyNvuBhtCt).Distinct().ToList();
            var tyleGiuLaiDict = await GetTyleBtGiuLaiBatch(soDonBh, soDonBhBs, prKeys);

            // Assign calculated `tyleGiuLai` to each item in the result list
            foreach (var item in result)
            {
                item.TyLeGiuLai = tyleGiuLaiDict.ContainsKey(item.PrKeyNvuBhtCt)
                                  ? tyleGiuLaiDict[item.PrKeyNvuBhtCt]
                                  : 100;
            }

            return result;
        }

        private async Task<Dictionary<decimal, decimal>> GetTyleBtGiuLaiBatch(string soDonBh, string soDonBhBs, List<decimal> prKeys)
        {
            var tyleGiuLaiDict = new Dictionary<decimal, decimal>();


            var taixData = await (from taixCtu in _context_pias.TaixCtus
                                  join taixCt in _context_pias.TaixCts on taixCtu.PrKey equals taixCt.FrKey
                                  where taixCtu.SoHdgcn == soDonBh
                                        && taixCtu.SoDonbhbs == soDonBhBs
                                        && prKeys.Contains(taixCt.PrKeyNvuBhtCt)
                                  select new
                                  {
                                      PrKey = taixCt.PrKeyNvuBhtCt,
                                      TyleGiuLai = taixCt.TyleReten + taixCt.TyleNhuongPhityle
                                  }).ToListAsync();


            foreach (var item in taixData)
            {
                tyleGiuLaiDict[item.PrKey] = Math.Round(item.TyleGiuLai, 5);
            }


            var nvuData = await (from nvuBhtCtu in _context_pias.NvuBhtCtus
                                 where nvuBhtCtu.SoDonbh == soDonBh
                                       && nvuBhtCtu.SoDonbhBs == soDonBhBs
                                       && prKeys.Contains(nvuBhtCtu.PrKey)
                                 select new
                                 {
                                     PrKey = nvuBhtCtu.PrKey,
                                     TyleDong = nvuBhtCtu.TyleDong,
                                     SumTyleTg = _context_pias.NvuBhtDbhs
                                        .Where(nvuBhtDbh => nvuBhtDbh.FrKey == nvuBhtCtu.PrKey)
                                        .Sum(nvuBhtDbh => (decimal?)nvuBhtDbh.TyleTg) ?? 0
                                 }).ToListAsync();


            foreach (var item in nvuData)
            {
                var tyleDong = item.TyleDong != 0 ? 100 - item.TyleDong : 0;
                var sumTyleTg = item.SumTyleTg * (item.TyleDong != 0 ? item.TyleDong : 100) / 100;
                var tyleGiuLai = 100 - (tyleDong + sumTyleTg);
                tyleGiuLaiDict[item.PrKey] = Math.Round(tyleGiuLai, 5);
            }

            return tyleGiuLaiDict;
        }
        public Guid GetOidByEmail(string email)
        {
            var oid = _context.DmUsers.Where(x => x.Mail == email).Select(x => x.Oid).FirstOrDefault();
            return oid;
        }
        //public BCGiamDinh Driving_license(string url,decimal pr_key_hsgd_ctu)
        //{
        //    // Giay phep lai xe
        //    var bcgd = new BCGiamDinh();
        //    DataCardPost objData = new DataCardPost();
        //    objData.Id = "1";
        //    objData.Type = "";
        //    objData.RequestId = Guid.NewGuid().ToString().Replace("-", "").Replace("_", "");
        //    objData.Sign = ContentHelper.MD5("123456456789" + "" + objData.RequestId);
        //    ImageData obj = new ImageData();
        public BANG Driving_license(string url, decimal pr_key_hsgd_ctu)
        {
            // Giay phep lai xe
            var bcgd = new BANG();
            DataCardPost objData = new DataCardPost();
            objData.Id = "1";
            objData.Type = "";
            objData.RequestId = Guid.NewGuid().ToString().Replace("-", "").Replace("_", "");
            objData.Sign = ContentHelper.MD5("123456456789" + "" + objData.RequestId);
            ImageData obj = new ImageData();

            // obj.img1 = ConvertURLImageToBase64(url)
            // obj.img2 = ConvertURLImageToBase64(url)
            obj.img = ConvertURLImageToBase64(url);
            objData.DataContent = obj;
            string data = JsonConvert.SerializeObject(objData);
            var url_driving_license = _configuration["DownloadSettings:url_driving_license"];
            string result = ContentHelper.PostData(data, url_driving_license);
            if (!string.IsNullOrEmpty(result))
            {
                try
                {
                    ResultDrivingLicense objResult = JsonConvert.DeserializeObject<ResultDrivingLicense>(result);

                    if (objResult != null)
                    {
                        if (objResult.data != null)
                        {
                            if (objResult.data.info != null)
                            {
                                _logger.Information("Driving_license pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " url = " + url + " objResult.data.info = " + JsonConvert.SerializeObject(objResult.data.info));
                                if (!string.IsNullOrEmpty(objResult.data.info.name))
                                    bcgd.TenLaiXe = objResult.data.info.name;

                                if (!string.IsNullOrEmpty(objResult.data.info.dob))
                                {
                                    List<string> dob = objResult.data.info.dob.Split("/").ToList();
                                    bcgd.NamSinh = Convert.ToInt32(dob[2]);
                                }

                                if (!string.IsNullOrEmpty(objResult.data.info.id))
                                    bcgd.SoGphepLaixe = objResult.data.info.id;

                                if (!string.IsNullOrEmpty(objResult.data.info.issue_date))
                                {
                                    List<string> issue_date = objResult.data.info.issue_date.Split("-").ToList();

                                    if (issue_date.Count == 3)
                                    {
                                        bcgd.NgayDauLaixe = new DateTime(Convert.ToInt32(issue_date[2]), Convert.ToInt32(issue_date[1]), Convert.ToInt32(issue_date[0]));
                                        if (bcgd.NgayDauLaixe.Value.Year < 1990)
                                        {
                                            bcgd.NgayDauLaixe = null;
                                        }
                                    }
                                }

                                if (!string.IsNullOrEmpty(objResult.data.info.due_date))
                                {
                                    List<string> due_date = objResult.data.info.due_date.Split("/").ToList();
                                    if (due_date.Count == 3)
                                    {
                                        bcgd.NgayCuoiLaixe = new DateTime(Convert.ToInt32(due_date[2]), Convert.ToInt32(due_date[1]), Convert.ToInt32(due_date[0]));
                                        if (bcgd.NgayCuoiLaixe.Value.Year < 1990)
                                        {
                                            bcgd.NgayCuoiLaixe = null;
                                        }
                                    }
                                }

                                if (!string.IsNullOrEmpty(objResult.data.info.class_hang))
                                {
                                    if (objResult.data.info.class_hang.ToUpper().Length > 2)
                                    {
                                        bcgd.MaLoaibang = objResult.data.info.class_hang.ToUpper().Substring(0, 2);
                                    }
                                    else
                                    {
                                        bcgd.MaLoaibang = objResult.data.info.class_hang.ToUpper();
                                    }
                                }

                                try
                                {
                                    var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == pr_key_hsgd_ctu).FirstOrDefault();
                                    List<string> mlb = bcgd.MaLoaibang.Split("-").ToList();
                                    var ma_loaibang = "";
                                    if (mlb.Count > 0)
                                    {
                                        ma_loaibang = mlb[0];
                                    }

                                    if (hsgd_ctu.MaLhsbt == "3")
                                    {
                                        _context.HsgdCtus.Where(x => x.PrKey == hsgd_ctu.PrKey || x.PrKey == Convert.ToInt64(hsgd_ctu.PrKeyBtHo))
                                            .ExecuteUpdate(s => s
                                            .SetProperty(u => u.TenLaixe, bcgd.TenLaiXe ?? "")
                                            .SetProperty(u => u.NamSinh, bcgd.NamSinh)
                                            .SetProperty(u => u.SoGphepLaixe, bcgd.SoGphepLaixe ?? "")
                                            .SetProperty(u => u.NgayDauLaixe, bcgd.NgayDauLaixe ?? null)
                                            .SetProperty(u => u.NgayCuoiLaixe, bcgd.NgayCuoiLaixe ?? null)
                                            .SetProperty(u => u.MaLoaibang, ma_loaibang ?? "")
                                            );
                                    }
                                    else
                                    {
                                        _context.HsgdCtus.Where(x => x.PrKey == hsgd_ctu.PrKey)
                                            .ExecuteUpdate(s => s
                                            .SetProperty(u => u.TenLaixe, bcgd.TenLaiXe ?? "")
                                            .SetProperty(u => u.NamSinh, bcgd.NamSinh)
                                            .SetProperty(u => u.SoGphepLaixe, bcgd.SoGphepLaixe ?? "")
                                            .SetProperty(u => u.NgayDauLaixe, bcgd.NgayDauLaixe ?? null)
                                            .SetProperty(u => u.NgayCuoiLaixe, bcgd.NgayCuoiLaixe ?? null)
                                            .SetProperty(u => u.MaLoaibang, ma_loaibang ?? "")
                                            );
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.Error("Driving_license pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " error : " + ex.ToString());
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Driving_license pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " error : " + ex.ToString());
                }
            }
            return bcgd;
        }
        public DANGKIEM Vehicle_Inspection(string url, decimal pr_key_hsgd_ctu)
        {
            // Dang kiem
            var bcgd = new DANGKIEM();
            DataCardPost? objData = new DataCardPost();
            objData.Id = "1";
            objData.Type = "";
            objData.RequestId = Guid.NewGuid().ToString().Replace("-", "").Replace("_", "");
            objData.Sign = ContentHelper.MD5("123456456789" + "" + objData.RequestId);

            ImageData obj = new ImageData();

            // obj.img1 = ConvertURLImageToBase64(url)
            // obj.img2 = ConvertURLImageToBase64(url)
            obj.img = ConvertURLImageToBase64(url);

            objData.DataContent = obj;
            string data = JsonConvert.SerializeObject(objData);
            var url_vehicle_Inspection = _configuration["DownloadSettings:url_vehicle_inspection"];

            string result = ContentHelper.PostData(data, url_vehicle_Inspection);
            if (!string.IsNullOrEmpty(result))
            {
                try
                {
                    ResultVehicleInspection objResult = JsonConvert.DeserializeObject<ResultVehicleInspection>(result);

                    if (objResult != null)
                    {
                        if (objResult.data != null)
                        {
                            if (objResult.data.info != null)
                            {
                                _logger.Information("Vehicle_Inspection pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " url = " + url + " objResult.data.info = " + JsonConvert.SerializeObject(objResult.data.info));
                                if (!string.IsNullOrEmpty(objResult.data.info.seri))
                                    bcgd.SoGphepLuuhanh = objResult.data.info.seri;

                                if (!string.IsNullOrEmpty(objResult.data.info.regis_date))
                                {
                                    List<string> regis_date = objResult.data.info.regis_date.Split("/").ToList();
                                    if (regis_date.Count == 3)
                                    {
                                        bcgd.NgayDauLuuhanh = new DateTime(Convert.ToInt32(regis_date[2]), Convert.ToInt32(regis_date[1]), Convert.ToInt32(regis_date[0]));
                                        if (bcgd.NgayDauLuuhanh.Value.Year < 1990)
                                            bcgd.NgayDauLuuhanh = null;
                                    }
                                }

                                if (!string.IsNullOrEmpty(objResult.data.info.valid_until))
                                {
                                    List<string> valid_until = objResult.data.info.valid_until.Split("/").ToList();
                                    if (valid_until.Count == 3)
                                    {
                                        bcgd.NgayCuoiLuuhanh = new DateTime(Convert.ToInt32(valid_until[2]), Convert.ToInt32(valid_until[1]), Convert.ToInt32(valid_until[0]));
                                        if (bcgd.NgayCuoiLuuhanh.Value.Year < 1990)
                                            bcgd.NgayCuoiLuuhanh = null;
                                    }
                                }


                                try
                                {
                                    var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == pr_key_hsgd_ctu).FirstOrDefault();
                                    // List<string> mlb = bcgd.MaLoaibang.Split("-").ToList();
                                    if (hsgd_ctu.MaLhsbt == "3")
                                    {
                                        _context.HsgdCtus.Where(x => x.PrKey == hsgd_ctu.PrKey || x.PrKey == Convert.ToInt64(hsgd_ctu.PrKeyBtHo))
                                            .ExecuteUpdate(s => s
                                            .SetProperty(u => u.SoGphepLuuhanh, bcgd.SoGphepLuuhanh ?? "")
                                            .SetProperty(u => u.NgayDauLuuhanh, bcgd.NgayDauLuuhanh ?? null)
                                            .SetProperty(u => u.NgayCuoiLuuhanh, bcgd.NgayCuoiLuuhanh ?? null)
                                            );
                                    }
                                    else
                                    {
                                        _context.HsgdCtus.Where(x => x.PrKey == hsgd_ctu.PrKey)
                                            .ExecuteUpdate(s => s
                                            .SetProperty(u => u.SoGphepLuuhanh, bcgd.SoGphepLuuhanh ?? "")
                                            .SetProperty(u => u.NgayDauLuuhanh, bcgd.NgayDauLuuhanh ?? null)
                                            .SetProperty(u => u.NgayCuoiLuuhanh, bcgd.NgayCuoiLuuhanh ?? null)
                                            );
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.Error("Vehicle_Inspection pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " error : " + ex.ToString());
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Vehicle_Inspection pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " error : " + ex.ToString());
                }
            }
            return bcgd;
        }

        public string ConvertURLImageToBase64(string path)
        {
            string url_download = _configuration["DownloadSettings:DownloadServer"] ?? "";
            //var utilityHelper = new UtilityHelper(_logger);
            byte[] bytes = UtilityHelper.getFileBase64Server(path, url_download);
            return Convert.ToBase64String(bytes);
        }

        public string GenerateWordDocumentWithImages(List<string> imagePaths, string tempDir)
        {
            string base64String = "";
            var wordApp = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document wordDoc = null;
            _logger.Information("Start doc");

            try
            {
                // Create a new Word document
                wordDoc = wordApp.Documents.Add();
                _logger.Information("Create Doc");

                foreach (var imagePath in imagePaths)
                {
                    var paragraph = wordDoc.Content.Paragraphs.Add();
                    paragraph.Range.InlineShapes.AddPicture(imagePath);
                    paragraph.Range.InsertParagraphAfter();
                    _logger.Information("Add Image  " + imagePath);
                }

                // Save the document
                string wordFilePath = Path.Combine(tempDir, "images.docx");
                _logger.Information("Combine path");
                object filePathObj = wordFilePath;
                object missing = Type.Missing;

                wordDoc.SaveAs2(ref filePathObj, ref missing, ref missing, ref missing);
                _logger.Information("Save Document");

                // **Ensure the document is closed before accessing the file**
                wordDoc.Close(false);
                wordApp.Quit();
                wordDoc = null;
                wordApp = null;

                // **Wait for the file to be fully released**
                GC.Collect();
                GC.WaitForPendingFinalizers();

                // Read the file
                byte[] fileBytes = File.ReadAllBytes(wordFilePath);
                base64String = Convert.ToBase64String(fileBytes);
                _logger.Information("Converted document to Base64");
            }
            catch (Exception ex)
            {
                _logger.Error("error  " + ex.Message);
                throw new Exception("Error saving Word document: " + ex.Message);
            }
            finally
            {
                // Clean up temporary files **AFTER** reading the file
                try
                {
                    if (Directory.Exists(tempDir))
                    {
                        _logger.Information("Delete temp start");
                        Directory.Delete(tempDir, true);
                        _logger.Information("Delete temp success");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error deleting temp directory: {ex.Message}");
                }
            }
            return base64String;
        }

        public class BCGiamDinh
        {
            public string? TenLaiXe { get; set; } = null!;
            public int NamSinh { get; set; }
            public string? SoGphepLaixe { get; set; } = null!;

            public string? NgayDauLaixe { get; set; }

            public string? NgayCuoiLaixe { get; set; }

            public string? MaLoaibang { get; set; } = null!;

            public string? SoGphepLuuhanh { get; set; } = null!;

            public string? NgayDauLuuhanh { get; set; }

            public string? NgayCuoiLuuhanh { get; set; }
            //public string HosoPhaply { get; set; } = null!;

            //public string YkienGdinh { get; set; } = null!;

            //public string DexuatPan { get; set; } = null!;
        }
        public class BANG
        {
            public string TenLaiXe { get; set; } = null!;
            public int NamSinh { get; set; }
            public string SoGphepLaixe { get; set; } = null!;

            public DateTime? NgayDauLaixe { get; set; }

            public DateTime? NgayCuoiLaixe { get; set; }

            public string MaLoaibang { get; set; } = null!;
        }
        public class DANGKIEM
        {
            public string SoGphepLuuhanh { get; set; } = null!;

            public DateTime? NgayDauLuuhanh { get; set; }

            public DateTime? NgayCuoiLuuhanh { get; set; }
        }
        public Task<List<DanhMuc>> GetListLoaiBang()
        {

            var list = _context.DmLoaiBangs.Select(s => new DanhMuc
            {
                MaDM = s.MaLoaiBang,
                TenDM = s.TenLoaiBang
            }).AsQueryable();

            return ToListWithNoLockAsync(list);
        }
        public async Task<TraSeri> TracuuPhi(string so_donbh_tracuu, string so_seri_tracuu, int nam_tracuu, int nam_tracuu_goc, string ma_donvi_tracuu)
        {
            TraSeri obj_result = new TraSeri();
            var maDviTracuu = _context_pias.NvuBhtCtus.Where(x => x.SoDonbhSdbs == so_donbh_tracuu).Select(x => x.MaDonvi).FirstOrDefault();

            try
            {
                DownloadFileResult result = new DownloadFileResult();
                // soap pias
                var ws = new ServiceReference1.PiasSoapSoapClient(ServiceReference1.PiasSoapSoapClient.EndpointConfiguration.PiasSoapSoap, _configuration["client:endpoint:address"].Replace(DateTime.Now.Year.ToString(), nam_tracuu.ToString()));
                var tra_seri = await ws.Tra_SeriAsync(so_donbh_tracuu, so_seri_tracuu, nam_tracuu, nam_tracuu_goc.ToString(), maDviTracuu);
                obj_result.Thong_bao = tra_seri.Thong_bao;
                _logger.Information("TracuuPhi so_donbh:" + so_donbh_tracuu + " Số seri:" + so_seri_tracuu + " Kết quả gọi soap: "+ (string.IsNullOrEmpty(tra_seri.Tra_Seri)? "Tra_Seri: null": tra_seri.Tra_Seri) + " Tra_Seri_ttoan: "+ (string.IsNullOrEmpty(tra_seri.Tra_Seri_ttoan) ? "Tra_Seri_ttoan: null" : tra_seri.Tra_Seri_ttoan) + " Tra_seri_bthuong: " + (string.IsNullOrEmpty(tra_seri.Tra_seri_bthuong) ? "Tra_seri_bthuong: null" : tra_seri.Tra_seri_bthuong));
                string url_download = _configuration["DownloadSettings:DownloadServer"] ?? "";
                if (!string.IsNullOrEmpty(tra_seri.Tra_seri_bthuong))
                {
                    result = UtilityHelper.DownloadFile_ToAPI(tra_seri.Tra_seri_bthuong, url_download);
                    obj_result.Tra_seri_bthuong = result.Data;
                    obj_result.Thong_bao = result.Message;
                }
                if (!string.IsNullOrEmpty(tra_seri.Tra_seri_hhong))
                {
                    result = UtilityHelper.DownloadFile_ToAPI(tra_seri.Tra_seri_hhong, url_download);
                    obj_result.Tra_seri_hhong = result.Data;
                    obj_result.Thong_bao = result.Message;
                }
                if (!string.IsNullOrEmpty(tra_seri.Tra_Seri_ttoan))
                {
                    result = UtilityHelper.DownloadFile_ToAPI(tra_seri.Tra_Seri_ttoan, url_download);
                    obj_result.Tra_Seri_ttoan = result.Data;
                    obj_result.Thong_bao = result.Message;
                }
                if (!string.IsNullOrEmpty(tra_seri.Tra_Seri))
                {
                    result = UtilityHelper.DownloadFile_ToAPI(tra_seri.Tra_Seri, url_download);
                    obj_result.Tra_Seri = result.Data;
                    obj_result.Thong_bao = result.Message;
                }

            }
            catch (Exception ex)
            {
                _logger.Error($"TracuuPhi error : {ex}");
            }
            return obj_result;
        }


        public ReloadSumChecker ReloadSumCheck(int prKey)
        {
            return PheDuyetHelper.checkReloadSum(prKey);
        }

        public string LoiGiamDinh(int pr_key, string currentUserEmail, int thieuAnhGDDk, int chuaThuPhi, int saidkdk, int saiphancap, int trucloibh, int saiphamkhac)
        {
            try
            {
                // Lấy thông tin người dùng hiện tại
                DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();

                // Lấy thông tin hồ sơ.
                HsgdCtu hoSoGiamDinh = _context.HsgdCtus.Where(x => x.PrKey == pr_key).FirstOrDefault();

                if (currentUser != null && hoSoGiamDinh != null)
                {
                    hoSoGiamDinh.ThieuAnh = thieuAnhGDDk;
                    hoSoGiamDinh.ChuaThuphi = chuaThuPhi;
                    hoSoGiamDinh.SaiDkdk = saidkdk;
                    hoSoGiamDinh.SaiPhancap = saiphancap;
                    hoSoGiamDinh.TrucloiBh = trucloibh;
                    hoSoGiamDinh.SaiphamKhac = saiphamkhac;

                    _context.HsgdCtus.Update(hoSoGiamDinh);
                    _context.SaveChanges();
                    return hoSoGiamDinh.PrKey.ToString();
                }
                else
                {
                    return "Người dùng hoặc hồ sơ lỗi";
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error at LoiGiamDinh " + ex.Message);
                return "Lỗi xảy ra khi tích lỗi giám định";
            }
        }
        public List<DmUserView> GetListGDV()
        {
            List<DmUserView> result = new List<DmUserView>();
            try
            {
                var data = _context.DmUsers.Select(s => new DmUserView
                {
                    Oid = s.Oid,
                    TenUser = s.TenUser,
                    MaUser = s.MaUser,
                    LoaiUser = s.LoaiUser
                }).AsQueryable();
                result = ToListWithNoLock(data);
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        public async Task<List<DmDkbh>> GetListDkbh(string ma_sp)
        {
            List<DmDkbh> listDkbh = new List<DmDkbh>();
            try
            {
                switch (ma_sp)
                {
                    case "050101":
                    case "050201":
                        var result = await _context_pias.DmDkbhs.Where(x => x.MaDkbh.Contains("050101") && x.KhongSdung == false).ToListAsync();
                        listDkbh.AddRange(result);
                        break;

                    case "050205":
                        var result1 = await _context_pias.DmDkbhs.Where(x => x.MaDkbh.Contains("050205") && x.KhongSdung == false).ToListAsync();
                        listDkbh.AddRange(result1);
                        break;
                    default:
                        break;
                }
                return listDkbh;
                //var data = 
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<bool> KTPquyen_YCBS(decimal pr_key, string email)
        {
            var KTPquyen_YCBS = false;
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(email)).FirstOrDefault();
            try
            {
                var pr_key_bt = _context.HsgdCtus.Where(x => x.PrKey == pr_key).Select(x => x.PrKeyBt).FirstOrDefault();
                if (pr_key_bt != null)
                {
                    var sum_sotienp = await (from a in _context_pias.HsbtCtus
                                             join b in _context_pias.HsbtCts on a.PrKey equals b.FrKey
                                             where a.PrKey == pr_key_bt
                                             select b.SoTienp + b.SoTienvp).SumAsync();

                    var so_tien_uyquyen = (from a in _context.DmUsers
                                           join b in _context.DmUqHstpcs on a.Oid.ToString() equals b.MaUserUq
                                           where a.Mail == currentUser.Mail && b.LoaiUyquyen == "6"
                                           select b.GhSotienUq).FirstOrDefault();
                    if (so_tien_uyquyen >= sum_sotienp)
                        return true;
                    else
                        return false;
                }

            }
            catch (Exception ex)
            {
                return KTPquyen_YCBS;
            }
            return KTPquyen_YCBS;
        }
        public async Task<string> GetAnhKbttCtu(string so_hsgd)
        {
            try
            {
                var hsgd_ctu = await _context.HsgdCtus.Where(x => x.SoHsgd == so_hsgd).FirstOrDefaultAsync();
                var addedImageList = new List<HsgdCt>();
                if (hsgd_ctu != null)
                {
                    var myPviProfile = await _context_my_pvi.KbttCtus.Where(x => x.SoHsgd == so_hsgd).FirstOrDefaultAsync();
                    var myPviImage = await _context_my_pvi.KbttAnhs.Where(x => x.FrKey == myPviProfile.PrKey && x.MaHmuc != "").ToListAsync();
                    foreach (var image in myPviImage)
                    {
                        var newHsgdImage = new HsgdCt
                        {
                            FrKey = hsgd_ctu.PrKey,
                            Stt = 0,
                            NhomAnh = 0,
                            PathFile = image.Path,
                            NgayChup = image.NgayChup,
                            ViDoChup = image.ViDo,
                            KinhDoChup = image.KinhDo,
                            DienGiai = "",
                            PathUrl = image.Url,
                            PathOrginalFile = image.Url,


                        };
                        addedImageList.Add(newHsgdImage);

                    }
                    await _context.HsgdCts.AddRangeAsync(addedImageList);
                    await _context.SaveChangesAsync();
                }

                return "Success";
            }
            catch (Exception ex)
            {
                _logger.Error("error " + ex.Message.ToString());
                return "Error";
            }

        }

        public async Task<string> GetAnhKbttCt(string so_hsgd)
        {
            //PathUrl = (uploadFilePath).Replace("\\\\pvi.com.vn\\p247_upload_new", cdn247 + "/upload_01").Replace("\\", "/"),
            try
            {
                var hsgd_ctu = await _context.HsgdCtus.Where(x => x.SoHsgd == so_hsgd).FirstOrDefaultAsync();
                var addedImageList = new List<HsgdCt>();
                if (hsgd_ctu != null)
                {
                    var myPviProfile = await _context_my_pvi.KbttCtus.Where(x => x.SoHsgd == so_hsgd).FirstOrDefaultAsync();
                    var kbttCtProfile = await _context_my_pvi.KbttCts.Where(x => x.FrKey == myPviProfile.PrKey).ToListAsync();
                    //var myPviImage = await _context_my_pvi.KbttAnhs.Where(x => x.FrKey == myPviProfile.PrKey ).ToListAsync();
                    string cdn247 = _configuration["DownloadSettings:CDN247"] ?? "";
                    foreach (var image in kbttCtProfile)
                    {
                        var myPviImage = await _context_my_pvi.KbttAnhs.Where(x => x.FrKey == image.PrKey).FirstOrDefaultAsync();
                        var newHsgdImage = new HsgdCt
                        {
                            FrKey = hsgd_ctu.PrKey,
                            Stt = 0,
                            NhomAnh = 0,

                            PathFile = myPviImage.Path,
                            NgayChup = myPviImage.NgayChup,
                            ViDoChup = myPviImage.ViDo,
                            KinhDoChup = myPviImage.KinhDo,
                            DienGiai = image.TenHmuc,
                            PathUrl = Regex.Replace(myPviImage.Url, @"^https?://pvi247\.pvi\.com\.vn", "https://cdn247.pvi.com.vn"),

                            PathOrginalFile = myPviImage.Url,


                        };
                        addedImageList.Add(newHsgdImage);

                    }
                    await _context.HsgdCts.AddRangeAsync(addedImageList);
                    await _context.SaveChangesAsync();
                }

                return "Success";
            }
            catch (Exception ex)
            {
                _logger.Error("error " + ex.Message.ToString());
                return "Error";
            }

        }
        public async Task<string> DeleteAnhHsgdCt(List<decimal> listKey)
        {
            try
            {
                var listImage = new List<HsgdCt>();
                foreach (var key in listKey)
                {
                    var hsgd_ct = await _context.HsgdCts.Where(x => x.PrKey == key).FirstOrDefaultAsync();
                    if (hsgd_ct != null)
                    {
                        listImage.Add(hsgd_ct);
                    }
                }
                _context.HsgdCts.RemoveRange(listImage);
                await _context.SaveChangesAsync();
                return "Success";
            }
            catch (Exception ex)
            {
                _logger.Error("error " + ex.Message.ToString());
                return "Error";
            }
        }
        public string UpdatePathBaoLanh(decimal pr_key_hsbt_ct, string file_path)
        {
            var result = "";
            try
            {

                var hsgd_dx_ct = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCt == pr_key_hsbt_ct).FirstOrDefault();
                if (hsgd_dx_ct != null)
                {
                    hsgd_dx_ct.PathBaolanh = file_path;
                    _context.HsgdDxCts.Update(hsgd_dx_ct);
                    _context.SaveChanges();
                }
                result = "Thành công";
                _logger.Error("UpdatePathBaoLanh pr_key_hsbt_ct =" + pr_key_hsbt_ct + " succcess");
            }
            catch (Exception ex)
            {
                result = "Thất bại";
                _logger.Error("UpdatePathBaoLanh pr_key_hsbt_ct =" + pr_key_hsbt_ct + " error " + ex);
            }
            return result;
        }
        public bool KyBaoLanh(decimal pr_key_hsgd_ctu, decimal pr_key_hsbt_ct, string file_path, string email, string SignContent)
        {
            bool result = false;
            try
            {
                var user_login = _context.DmUsers.Where(x => x.Mail == email).FirstOrDefault();
                // soap pias
                //var ws = new ServiceReference1.PiasSoapSoapClient(ServiceReference1.PiasSoapSoapClient.EndpointConfiguration.PiasSoapSoap, _configuration["client:endpoint:address"]);
                PiasSoapSoap ws = new PiasSoapSoapClient(ServiceReference1.PiasSoapSoapClient.EndpointConfiguration.PiasSoapSoap, _configuration["client:endpoint:address"]);


                var client = new ServiceReference1.PiasSoapSoapClient(ServiceReference1.PiasSoapSoapClient.EndpointConfiguration.PiasSoapSoap, _configuration["client:endpoint:address"]);

                client.Endpoint.Binding.SendTimeout = new TimeSpan(0, 20, 0);
                client.InnerChannel.OperationTimeout = new TimeSpan(20, 20, 20);

                string strSQL = "select top 1 * from hddt_hsm where ma_donvi = '" + user_login.MaDonvi + "' and ngay_hluc < getdate() order by ngay_hluc desc ";
                var esign = client.SelectSQL_HDDT(DateTime.Now.Year.ToString(), strSQL, "hddt_hsm");
                var ds_esign = ConvetXMLToDataset(esign);
                if (ds_esign.Tables[0].Rows.Count > 0)
                {
                    var partitionAlias = ds_esign.Tables[0].Rows[0].Field<string>("partition_alias");
                    var privateKeyAlias = ds_esign.Tables[0].Rows[0].Field<string>("private_key_alias");
                    var password = ds_esign.Tables[0].Rows[0].Field<string>("password");
                    var partitionSerial = ds_esign.Tables[0].Rows[0].Field<string>("partition_serial");
                    result = client.KyPASCXCG(file_path, privateKeyAlias, "mediafile3", SignContent);
                    _logger.Information($"KyBaoLanh call KyPASCXCG pr_key_hsbt_ct = " + pr_key_hsbt_ct + " result =  " + result);
                    //ký xong bảo lãnh thành công thì đẩy sang làm hồ sơ thanh toán
                    //haipv1 13/11/2025 bỏ đẩy sang làm thanh toán theo yêu cầu: 73470 Bỏ  tính năng tự động thêm Thông Báo Bồi Thường và Thư Bảo Lãnh  vào hồ sơ thanh toán
                    //try
                    //{
                    //    if (result)
                    //    {
                    //        var attachFilesToDelete = _context.HsgdAttachFiles
                    //        .Where(b => b.MaCtu == "PABL" && b.FrKey == pr_key_hsgd_ctu)
                    //        .ToList();
                    //        // Nếu có dữ liệu thì xóa
                    //        if (attachFilesToDelete.Any())
                    //        {
                    //            _context.HsgdAttachFiles.RemoveRange(attachFilesToDelete);
                    //            _context.SaveChanges();                                
                    //        }
                    //        List<HsgdAttachFile> attachFiles = new List<HsgdAttachFile>();
                    //        var atf = new HsgdAttachFile
                    //        {
                    //            PrKey = Guid.NewGuid().ToString().ToLower(),
                    //            FrKey = pr_key_hsgd_ctu,
                    //            MaCtu = "PABL",
                    //            FileName = "PABL.pdf",
                    //            Directory = file_path,
                    //            ngay_cnhat = DateTime.Now,
                    //            GhiChu = "Cập nhật từ ký bảo lãnh",
                    //            NguonTao = "WebPvi247"
                    //        };
                    //        attachFiles.Add(atf);
                    //        // Add vào context
                    //        _context.HsgdAttachFiles.AddRange(attachFiles);                          
                    //        _context.SaveChanges();
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    _logger.Error($"Ký xong thêm bảo lãnh vào bảng hồ sơ lỗi pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " error occurred: " + ex);
                        
                    //}                    
                }
                else
                {
                    _logger.Error($"KyBaoLanh call KyPASCXCG pr_key_hsbt_ct = " + pr_key_hsbt_ct + " chưa phân quyền ký trong bảng hddt_hsm ");
                }

            }
            catch (Exception ex)
            {
                _logger.Error($"KyBaoLanh call KyPASCXCG pr_key_hsbt_ctu = " + pr_key_hsgd_ctu + " error occurred: " + ex);
                result = false;
            }
            return result;
        }
        public string GetFilePathBaoLanh(decimal pr_key_hsbt_ct)
        {
            var file_path = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCt == pr_key_hsbt_ct).Select(s => s.PathBaolanh).FirstOrDefault() ?? "";
            return file_path;
        }
        public HsgdDxCt? GetTTBaoLanh(decimal pr_key_hsgd_dx_ct)
        {
            var result = _context.HsgdDxCts.Where(x => x.PrKey == pr_key_hsgd_dx_ct).FirstOrDefault();
            return result;
        }
        public HsgdDxCt GetHsgdDxCt(decimal pr_key_hsbt_ct)
        {
            var hsgd_dx_ct = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCt == pr_key_hsbt_ct).FirstOrDefault();
            return hsgd_dx_ct;
        }
        public string GetThongTinKyDienTu(decimal pr_key_hsgd_ctu, string currentUserEmail)
        {
            string SignContent = "";
            var user_login = _context.DmUsers.Where(x => x.Mail == currentUserEmail).FirstOrDefault();
            if (user_login != null)
            {
                var dm_var = (from vars in _context_pias.DmVars
                              where vars.MaDonvi == user_login.MaDonvi && vars.Bien == "DON_VI"
                              select vars).FirstOrDefault();
                if (dm_var != null)
                {
                    SignContent = "Ký bởi: " + dm_var.GiaTri;
                }
                var nhat_ky_duyet = _context.NhatKies.Where(x => x.FrKey == pr_key_hsgd_ctu && x.MaTtrangGd == "DBL").OrderByDescending(o => o.PrKey).FirstOrDefault();
                if (nhat_ky_duyet != null)
                {
                    SignContent += "\n" + "Ngày ký: " + Convert.ToDateTime(nhat_ky_duyet.NgayCapnhat).ToString("dd/MM/yyyy HH:mm:ss");
                }
            }
            return SignContent;
        }
        public string CheckTrungHsbtUpdate(decimal pr_key_hsgd_ctu)
        {
            string result = "";
            try
            {
                var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == pr_key_hsgd_ctu).FirstOrDefault();
                if (hsgd_ctu != null)
                {
                    var list_sp = _context_pias_update.HsbtCts.Where(x => x.FrKey == hsgd_ctu.PrKeyBt).Select(s => s.MaSp).ToArray();
                    if (list_sp.Contains("0501"))
                    {
                        var hstrung = (from a in _context_pias_update.HsbtCtus
                                       join b in _context_pias_update.HsbtCts on a.PrKey equals b.FrKey
                                       where a.PrKey != hsgd_ctu.PrKeyBt && a.MaDonvi == hsgd_ctu.MaDonvi && list_sp.Contains(b.MaSp) && a.MaLhsbt == hsgd_ctu.MaLhsbt && a.SoHdgcn == hsgd_ctu.SoDonbh && a.SoSeri == hsgd_ctu.SoSeri && a.NgayTthat.Value.Date == hsgd_ctu.NgayTthat.Value.Date
                                       select a).ToList();
                        if (hstrung.Count() > 0)
                        {
                            result = "Đã tồn tại hồ sơ bồi thường có cùng số đơn, mã sản phẩm, số ấn chỉ và ngày tai nạn. Hãy xem lại!";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public string CheckThoiHanSDBS(string so_donbh, DateTime? ngay_tthat, string so_seri)
        {
            string result = "";
            try
            {
                var nvu_bht_ctu = _context_pias.NvuBhtCtus
                .Where(x => x.SoDonbh == so_donbh)
                .Select(s => new
                {
                    s.PrKey,
                    s.SoDonbh,
                })
            .FirstOrDefault();
                if (nvu_bht_ctu != null)
                {
                    var nvu_bht_seri = _context_pias.NvuBhtSeris
                        .Where(x => x.FrKey == nvu_bht_ctu.PrKey && x.SoSeri.ToString() == so_seri)
                        .Select(s => new
                        {
                            s.NgayDauSeri,
                            s.NgayCuoiSeri,
                            s.SoSeri
                        })
                        .FirstOrDefault();

                    if (nvu_bht_seri != null)
                    {
                        if (ngay_tthat < nvu_bht_seri.NgayDauSeri || ngay_tthat > nvu_bht_seri.NgayCuoiSeri)
                        {
                            result = $"Ngày tổn thất phải nằm trong thời hạn bảo hiểm của từ {nvu_bht_seri.NgayDauSeri?.ToString("dd/MM/yyyy")} đến {nvu_bht_seri.NgayCuoiSeri?.ToString("dd/MM/yyyy")}";
                        }
                    }
                    else
                    {
                        result = $"Không tìm thấy thông tin seri trong đơn bảo hiểm {so_donbh}";
                    }
                }
                else
                {
                    result = $"Không tìm thấy thông tin đơn bảo hiểm {so_donbh}";
                }
                
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        public GetListFileResponse GetListFile(decimal pr_key_hsgd_ctu)
        {
            try
            {
               var fileList = _context.HsgdAttachFiles
                    .Where(x => x.FrKey == pr_key_hsgd_ctu)
                    .OrderByDescending(x => x.ngay_cnhat)
                    .Select(x => new HsgdAttachFile
                    {
                        PrKey = x.PrKey,
                        FrKey = x.FrKey,
                        FileName = x.FileName,
                        Directory = x.Directory,
                        MaCtu = x.MaCtu,
                        ngay_cnhat = x.ngay_cnhat,
                        GhiChu = x.GhiChu,
                        NguonTao = x.NguonTao,
                        PathUrl = x.Directory != null ? x.Directory.Replace("//pvi.com.vn/p247_upload_new", "https://cdn247.pvi.com.vn/upload_01") : ""
                    })
                    .ToList();
                var hoanThienHstt = _context.HsgdCtus
            .Where(x => x.PrKey == pr_key_hsgd_ctu)
            .Select(x => x.HoanThienHstt)
            .FirstOrDefault();
                return new GetListFileResponse
        {
            Files = fileList,
            HoanThienHstt = hoanThienHstt
        };
            }
            catch (Exception ex)
            {
                _logger.Error($"GetListFile repository error: {ex}");
                return new GetListFileResponse
                {
                    Files = new List<HsgdAttachFile>(),
                    HoanThienHstt = null
                };
            }
        }
        public HsgdAttachFile GetAttachFileByPrKey(string pr_key)
        {
            try
            {
                var attachFile = _context.HsgdAttachFiles
                    .FirstOrDefault(x => x.PrKey == pr_key.ToString());
                return attachFile;
            }
            catch (Exception ex)
            {
                _logger.Error($"GetAttachFileByPrKey repository error: {ex}");
                return null;
            }
        }
        public async Task<bool> UploadHsgdAttachFiles(List<HsgdAttachFile> fileAttach)
        {
            try
            {
                if (fileAttach == null || !fileAttach.Any())
                {
                    _logger.Warning("UploadHsgdAttachFiles: No files to upload");
                    return false;
                }

                // Thêm danh sách files vào DbContext
                await _context.HsgdAttachFiles.AddRangeAsync(fileAttach);

                // Lưu thay đổi vào database
                await _context.SaveChangesAsync();

                _logger.Information($"UploadHsgdAttachFiles: Successfully uploaded {fileAttach.Count} files");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"UploadHsgdAttachFiles error: {ex}");
                return false;
            }
        }
        public async Task<string> DeleteAttachFile(string pr_key)
        {
            try
            {
                // Tìm file cần xóa
                var attachFile = await _context.HsgdAttachFiles
                    .FirstOrDefaultAsync(x => x.PrKey == pr_key);

                if (attachFile == null)
                {
                    return "File không tồn tại";
                }

                var hoSoGiamDinh = await _context.HsgdCtus
                    .FirstOrDefaultAsync(x => x.PrKey == attachFile.FrKey);

                if (hoSoGiamDinh == null)
                {
                    return "Hồ sơ giám định không tồn tại";
                }

                // Kiểm tra trạng thái hồ sơ có cho phép xóa file không
                if (hoSoGiamDinh.MaTtrangGd == "7")
                {
                    return "Không thể xóa file của hồ sơ đã hủy";
                }

                // Xóa file khỏi database
                _context.HsgdAttachFiles.Remove(attachFile);
                await _context.SaveChangesAsync();

                _logger.Information($"DeleteAttachFile: Successfully deleted file with pr_key = {pr_key}");

                return "Xóa file thành công";
            }
            catch (Exception ex)
            {
                _logger.Error($"DeleteAttachFile error for pr_key = {pr_key}: {ex}");
                return "Có lỗi xảy ra khi xóa file";
            }
        }
        public async Task<string> UpdateHoanThienHstt(decimal prKeyHsgdCtu, bool hoanThienHstt, string currentUserEmail)
        {
            try
            {
                DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
                HsgdCtu hoSoGiamDinh = await _context.HsgdCtus.FirstOrDefaultAsync(x => x.PrKey == prKeyHsgdCtu);

                if (hoSoGiamDinh == null)
                {
                    return "Hồ sơ giám định không tồn tại";
                }

                if (currentUser == null)
                {
                    return "Người dùng không tồn tại";
                }

                // Update the HoanThienHstt status
                hoSoGiamDinh.HoanThienHstt = hoanThienHstt;
                _context.HsgdCtus.Update(hoSoGiamDinh);

                // Create a diary entry for this action
                NhatKy nhatKy = new NhatKy
                {
                    FrKey = hoSoGiamDinh.PrKey,
                    MaTtrangGd = hoSoGiamDinh.MaTtrangGd,
                    TenTtrangGd = "Hoàn thiện HSTT",
                    GhiChu = $"Cập nhật trạng thái hoàn thiện hồ sơ thủ tục: {(hoanThienHstt ? "Hoàn thiện" : "Chưa hoàn thiện")}",
                    NgayCapnhat = DateTime.Now,
                    MaUser = currentUser.Oid
                };

                await _context.NhatKies.AddAsync(nhatKy);
                await _context.SaveChangesAsync();
                //Chạy thead gửi email
                if(hoanThienHstt)
                {
                    Task.Run(() => SendEmail_QLNV_GDV_HOANTHIENHSTT(hoSoGiamDinh.PrKey));
                }    
                

                return "Cập nhật trạng thái hoàn thiện HSTT thành công";
            }
            catch (Exception ex)
            {
                _logger.Error($"UpdateHoanThienHstt error: {ex}");
                return "Có lỗi xảy ra khi cập nhật trạng thái hoàn thiện HSTT";
            }
        }
        public void SendEmail_QLNV_GDV_HOANTHIENHSTT(decimal pr_key_hsgd)
        {
            try
            {
                //Gửi email cho kiểm soát viên ở đơn vị khi giám định viên kích hoàn thiện hồ sơ bồi thường
                // Ở VPPB thì hoanthienhstt =true khi tờ trình được ký duyệt, VPPN thì kích bằng tay

                using (var _context_gdtt_new = new GdttContext())
                {


                    var result =
                            (
                                from a in _context_gdtt_new.HsgdCtus
                                join b in _context_gdtt_new.HsgdTtrinhs
                                    on a.PrKey equals b.PrKeyHsgd
                                join c in _context_gdtt_new.HsgdTtrinhNkies
                                    on b.PrKey equals c.FrKey
                                join d in _context_gdtt_new.DmUsers
                                    on c.UserChuyen equals d.Oid.ToString()
                                where a.PrKey == pr_key_hsgd
                                   && c.Act == "CREATETOTRINH"
                                orderby c.NgayCnhat descending
                                select new
                                {
                                    a.MaDonvi,
                                    a.SoHsbt,
                                    a.SoHsgd,
                                    d.Mail
                                }
                            ).FirstOrDefault();

                    if (result != null)
                    {
                        var canboxulyhstt = _context_gdtt_new.DmUsers.Where(x => x.Mail == result.Mail).FirstOrDefault();
                        var dscanboxuly = _context_gdtt_new.DmUserTtoans.Where(x => x.MaDonvi == result.MaDonvi).ToList();
                        if (dscanboxuly != null)
                        {

                            MailAddress from = new MailAddress("baohiempvi@pvi.com.vn", "PVI.247", System.Text.Encoding.UTF8);
                            System.Net.Mail.MailMessage Mail = new System.Net.Mail.MailMessage();
                            Mail.From = from;
                            foreach (var u in dscanboxuly)
                            {
                                if (!string.IsNullOrWhiteSpace(u.DcEmail))
                                {
                                    Mail.To.Add(new MailAddress(u.DcEmail));
                                }
                            }

                            Mail.Subject = "PVI247 - Thông báo Giám định viên hoàn thiện hồ sơ bồi thường có thể làm đề nghị thanh toán số hồ sơ bồi thường:" + result.SoHsbt + ", Số HS giám định: " + result.SoHsgd + ".";
                            Mail.SubjectEncoding = System.Text.Encoding.UTF8;
                            string htmlBody = "PVI247 Thông báo!<br/>";
                            htmlBody = htmlBody + "Hồ sơ bồi thường:" + result.SoHsbt + ", Số HS giám định: " + result.SoHsgd + " đã được giám định viên hoàn thiện hồ sơ, Vui lòng tạo đề nghị thanh toán cho hồ sở trên.<br/>";
                            if (canboxulyhstt != null)
                            {
                                htmlBody = htmlBody + "Đơn vị: " + canboxulyhstt.TenDonvi + "<br/>";
                                htmlBody = htmlBody + "Tên giám định viên hoàn thiện hồ sơ: " + canboxulyhstt.TenUser + "<br/>";
                                htmlBody = htmlBody + "Email: " + canboxulyhstt.Mail + "<br/>";
                                //htmlBody = "Điện thoại: " + canboxulyhstt.Dienthoai + "<br/>";
                                //Mail.CC.Add(canboxulyhstt.Mail);
                            }

                            if (htmlBody != "")
                                Mail.Body = htmlBody;
                            Mail.BodyEncoding = System.Text.Encoding.UTF8;
                            Mail.IsBodyHtml = true;
                            SmtpClient SmtpServer = new SmtpClient();
                            SmtpServer.Port = 25;
                            SmtpServer.Host = "mailapp.pvi.com.vn";
                            SmtpServer.Timeout = 10000;
                            SmtpServer.Credentials = new NetworkCredential("baohiempvi", "bhpvi!@#");
                            SmtpServer.Send(Mail);
                            Mail.Dispose();
                        }


                    }
                }


            }
            catch (Exception ex)
            {
                _logger.Error(DateTime.Now.ToString() + "VPPN: Lỗi SendEmail_QLNV_GDV_HOANTHIENHSTT  Error: " + ex.Message.ToString());

            }
        }
    }
}