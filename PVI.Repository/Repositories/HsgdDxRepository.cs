using Azure.Core;
using iTextSharp.xmp.impl.xpath;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using Newtonsoft.Json;
using PVI.DAO.Entities.Models;
using PVI.Helper;
using PVI.Repository.Interfaces;
using Serilog;
using ServiceReference1;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static Azure.Core.HttpHeader;
using static iTextSharp.text.pdf.events.IndexEvents;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Task = System.Threading.Tasks.Task;

namespace PVI.Repository.Repositories
{
    public class HsgdDxRepository : GenericRepository<HsgdDx>, IHsgdDxRepository
    {
       
        public HsgdDxRepository(GdttContext context, Pvs2024Context context_pias, Pvs2024UpdateContext context_pias_update, Serilog.ILogger logger, IConfiguration conf) : base(context, context_pias, context_pias_update, logger, conf)
        {
        }

        public List<HsbtCtView> GetListPhaiTraBT(decimal pr_key_hsgd)
        {
            List<HsbtCtView> obj_result = new List<HsbtCtView>();
            try
            {
                var pr_key_bt = _context.HsgdCtus.Where(x => x.PrKey == pr_key_hsgd).Select(s => s.PrKeyBt).FirstOrDefault();
                if (pr_key_bt != 0)
                {
                    var hsbt_ct = ToListWithNoLock((from ct in _context_pias_update.HsbtCts
                                                        //let ghp = _context.KbttHsbtGhphus.Where(p => p.FrKey == pr_key && p.MaQuyenloi == _context.DmDieutris.Where(x => x.GioiHanPhu == ct.MaQuyenloi).Select(x => x.MaDieutri).FirstOrDefault()).FirstOrDefault()
                                                    where ct.FrKey == pr_key_bt
                                                    select new HsbtCtView
                                                    {
                                                        PrKey = ct.PrKey,
                                                        FrKey = ct.FrKey,
                                                        MaSp = ct.MaSp,
                                                        MaDkhoan = ct.MaDkhoan,
                                                        MtnGtbh = ct.MtnGtbh,
                                                        MaTteGoc = ct.MaTteGoc,
                                                        MaTtrangBt = ct.MaTtrangBt,
                                                        MaTtebt = ct.MaTtebt,
                                                        TygiaBt = ct.TygiaBt,
                                                        NguyenTep = ct.NguyenTep,
                                                        SoTienp = ct.SoTienp,
                                                        MucVatp = ct.MucVatp,
                                                        NguyenTevp = ct.NguyenTevp,
                                                        SoTienvp = ct.SoTienvp,
                                                        SoTienkhTra = ct.SoTienYcbt == 0 ? (ct.SoTienp - ct.SoTienTc) * ct.ChenhLechKtru / 100 : (ct.SoTienYcbt - ct.SoTienTc) * ct.ChenhLechKtru / 100,
                                                        SoTientcbt = (ct.SoTienYcbt == 0 ? (ct.SoTienp - ct.SoTienTc) * ct.ChenhLechKtru / 100 : (ct.SoTienYcbt - ct.SoTienTc) * ct.ChenhLechKtru / 100) + ct.SoTienTc,
                                                        NgayHtoanBt = ct.NgayHtoanBt != null ? Convert.ToDateTime(ct.NgayHtoanBt).ToString("dd/MM/yyyy") : null,
                                                        TyleReten = ct.TyleReten,
                                                        MtnRetenNte = ct.MtnRetenNte,
                                                        MtnRetenVnd = ct.MtnRetenVnd,
                                                        SoTienkt = ct.SoTienkt,
                                                        MauSovat = ct.MauSovat,
                                                        SerieVat = ct.SerieVat,
                                                        SoHdvat = ct.SoHdvat,
                                                        NgayHdvat = ct.NgayHdvat != null ? Convert.ToDateTime(ct.NgayHdvat).ToString("dd/MM/yyyy") : null,
                                                        MaKhvat = ct.MaKhvat,
                                                        TenKhvat = ct.TenKhvat,
                                                        MasoVat = ct.MasoVat,
                                                        TenHhoavat = ct.TenHhoavat
                                                    }
                      ).AsQueryable());
                    var hsgd_dx_ct = ToListWithNoLock((from a in _context.HsgdDxCts where hsbt_ct.Select(x => x.PrKey).ToArray().Contains(a.PrKeyHsbtCt) select a).AsQueryable());
                    var hsgd_tt = ToListWithNoLock((from a in _context.HsgdTtrinhs
                                                    join b in _context.HsgdTtrinhNkies on a.PrKey equals b.FrKey
                                                    join c in _context.HsgdTtrinhCts on a.PrKey equals c.FrKey
                                                    where a.PrKeyHsgd == pr_key_hsgd && b.Act == "ChuyenDuyet"
                                                    select new
                                                    {
                                                        MaSp = c.MaSp
                                                        //KyTT = true
                                                    }).AsQueryable());
                    var hsgd_tt_gr = hsgd_tt.GroupBy(n => n.MaSp).Select(s => new
                    {
                        MaSp = s.Key,
                        KyTT = true
                    }).ToList();
                    obj_result = (from a in hsbt_ct
                                  join b in hsgd_dx_ct on a.PrKey equals b.PrKeyHsbtCt into b1
                                  from b in b1.DefaultIfEmpty()
                                  join c in hsgd_tt_gr on a.MaSp equals c.MaSp into c1
                                  from c in c1.DefaultIfEmpty()
                                  select new HsbtCtView
                                  {
                                      PrKey = a.PrKey,
                                      FrKey = a.FrKey,
                                      MaSp = a.MaSp,
                                      MaDkhoan = a.MaDkhoan,
                                      MtnGtbh = a.MtnGtbh,
                                      MaTteGoc = a.MaTteGoc,
                                      MaTtrangBt = a.MaTtrangBt,
                                      MaTtebt = a.MaTtebt,
                                      TygiaBt = a.TygiaBt,
                                      NguyenTep = a.NguyenTep,
                                      SoTienp = a.SoTienp,
                                      MucVatp = a.MucVatp,
                                      NguyenTevp = a.NguyenTevp,
                                      SoTienvp = a.SoTienvp,
                                      SoTienkhTra = a.SoTienkhTra,
                                      SoTientcbt = a.SoTientcbt,
                                      NgayHtoanBt = a.NgayHtoanBt,
                                      TyleReten = a.TyleReten,
                                      MtnRetenNte = a.MtnRetenNte,
                                      MtnRetenVnd = a.MtnRetenVnd,
                                      SoTienkt = a.SoTienkt,
                                      MauSovat = a.MauSovat,
                                      SerieVat = a.SerieVat,
                                      SoHdvat = a.SoHdvat,
                                      NgayHdvat = a.NgayHdvat,
                                      MaKhvat = a.MaKhvat,
                                      TenKhvat = a.TenKhvat,
                                      MasoVat = a.MasoVat,
                                      TenHhoavat = a.TenHhoavat,
                                      HieuXe = b != null ? b.HieuXe : 0,
                                      LoaiXe = b != null ? b.LoaiXe : 0,
                                      XuatXu = b != null ? b.XuatXu : "",
                                      NamSx = b != null ? b.NamSx : 0,
                                      MaGara = b != null ? b.MaGara : "",
                                      MaGara01 = b != null ? b.MaGara01 : "",
                                      MaGara02 = b != null ? b.MaGara02 : "",
                                      SoTienctkh = b != null ? b.SoTienctkh : 0,
                                      SoTienGtbt = b != null ? b.SoTienGtbt : 0,
                                      TyleggPhutungvcx = b != null ? b.TyleggPhutungvcx : 0,
                                      TyleggSuachuavcx = b != null ? b.TyleggSuachuavcx : 0,
                                      Vat = b != null ? b.Vat : 0,
                                      VatTnds = b != null ? b.VatTnds : 0,
                                      LydoCtkh = b != null ? b.LydoCtkh : "",
                                      GhiChudx = b != null ? b.GhiChudx : "",
                                      PrKeyHsgdDxCt = b != null ? b.PrKey : 0,
                                      PrKeyHsgdDxCtu = b != null ? b.PrKeyHsbtCtu : 0,
                                      DonviSuachuaTsk = b != null ? b.DonviSuachuaTsk : "",
                                      HieuXeTndsBen3 = b != null ? b.HieuXeTndsBen3 : 0,
                                      LoaiXeTndsBen3 = b != null ? b.LoaiXeTndsBen3 : 0,
                                      DoituongttTnds = b != null ? b.DoituongttTnds : "",
                                      ChkKhongHoadon = b != null ? b.ChkKhonghoadon : 0,
                                      SotienTtpin = b != null ? b.SotienTtpin : 0,
                                      MaLoaiDongco = b != null ? b.MaLoaiDongco : "",
                                      KyTT = c != null ? c.KyTT : false
                                  }).OrderBy(x => x.PrKey).ToList();

                }
            }
            catch (Exception ex)
            {
            }
            return obj_result;

        }
        public Task<List<HsbtUocBT>> GetListChiTietUocBT(decimal hsbt_ct_pr_key)
        {
            var ubt = (from u in _context_pias_update.HsbtUocs
                           //join ct in _context_pias.HsbtCts on u.FrKey equals ct.PrKey
                       where (u.FrKey == hsbt_ct_pr_key)
                       group u by new { u.FrKey, u.NgayPs } into sp
                       select new HsbtUocBT
                       {
                           PrKey = sp.Max(item => item.PrKey),
                           FrKey = sp.Key.FrKey,
                           NgayPs = sp.Key.NgayPs != null ? Convert.ToDateTime(sp.Key.NgayPs).ToString("dd/MM/yyyy") : null,
                           //NguyenTebt = sp.Sum(item => item.NguyenTebt),
                           //SoTienbt = sp.Sum(item => item.SoTienbt),
                           //NguyenTebtPvi = sp.Sum(item => item.NguyenTebtPvi),
                           //SoTienbtPvi = sp.Sum(item => item.SoTienbtPvi),
                           TyleReten = sp.Average(item => item.TyleReten),
                           //NguyenTebtReten = sp.Sum(item => item.NguyenTebtReten),
                           //SoTienbtReten = sp.Sum(item => item.SoTienbtReten),
                           GhiChu = sp.Max(item => item.GhiChu),
                           NguyenTebtLk = _context_pias_update.HsbtUocs.Where(x => x.FrKey == sp.Key.FrKey && x.NgayPs <= sp.Key.NgayPs).Sum(s => s.NguyenTebt),
                           SoTienbtLk = _context_pias_update.HsbtUocs.Where(x => x.FrKey == sp.Key.FrKey && x.NgayPs <= sp.Key.NgayPs).Sum(s => s.SoTienbt),
                           //NguyenTebtPviLk = _context_pias.HsbtUocs.Where(x => x.FrKey == sp.Key.FrKey && x.NgayPs <= sp.Key.NgayPs).Sum(s => s.NguyenTebtPvi),
                           // SoTienbtPviLk = _context_pias.HsbtUocs.Where(x => x.FrKey == sp.Key.FrKey && x.NgayPs <= sp.Key.NgayPs).Sum(s => s.SoTienbtPvi),
                           NguyenTebtRetenLk = _context_pias_update.HsbtUocs.Where(x => x.FrKey == sp.Key.FrKey && x.NgayPs <= sp.Key.NgayPs).Sum(s => s.NguyenTebtReten),
                           SoTienbtRetenLk = _context_pias_update.HsbtUocs.Where(x => x.FrKey == sp.Key.FrKey && x.NgayPs <= sp.Key.NgayPs).Sum(s => s.SoTienbtReten)

                       }
                   ).OrderBy(x => x.PrKey).AsQueryable();
            return ToListWithNoLockAsync(ubt);

        }
        public List<HsbtGDView> GetListPhaiTraGD(decimal pr_key_hsgd)
        {
            List<HsbtGDView> obj_result = new List<HsbtGDView>();
            try
            {
                var pr_key_bt = _context.HsgdCtus.Where(x => x.PrKey == pr_key_hsgd).Select(s => s.PrKeyBt).FirstOrDefault();
                if (pr_key_bt != 0)
                {
                    obj_result = ToListWithNoLock((from a in _context_pias_update.HsbtGds
                                                   where (a.MaLoaiChiphi != "CPTGD" && a.FrKey == pr_key_bt)
                                                   select new HsbtGDView
                                                   {
                                                       PrKey = a.PrKey,
                                                       FrKey = a.FrKey,
                                                       MaSp = a.MaSp,
                                                       MaDvgd = a.MaDvgd,
                                                       MaLoaiChiphi = a.MaLoaiChiphi,
                                                       MaTtegd = a.MaTtegd,
                                                       TygiaGd = a.TygiaGd,
                                                       MaTtrangGd = a.MaTtrangGd,
                                                       NguyenTegd = a.NguyenTegd,
                                                       SoTiengd = a.SoTiengd,
                                                       MucVat = a.MucVat,
                                                       NguyenTev = a.NguyenTev,
                                                       SoTienv = a.SoTienv,
                                                       TyleReten = a.TyleReten,
                                                       MtnRetenNte = a.MtnRetenNte,
                                                       MtnRetenVnd = a.MtnRetenVnd,
                                                       GhiChuGd = a.GhiChuGd,
                                                       NgayHtoanGd = a.NgayHtoanGd != null ? Convert.ToDateTime(a.NgayHtoanGd).ToString("dd/MM/yyyy") : null,
                                                       MauSovat = a.MauSovat,
                                                       SerieVat = a.SerieVat,
                                                       SoHdvat = a.SoHdvat,
                                                       NgayHdvat = a.NgayHdvat != null ? Convert.ToDateTime(a.NgayHdvat).ToString("dd/MM/yyyy") : null,
                                                       MaKhvat = a.MaKhvat,
                                                       TenKhvat = a.TenKhvat,
                                                       MasoVat = a.MasoVat,
                                                       TenHhoavat = a.TenHhoavat
                                                   }).OrderBy(x => x.PrKey).AsQueryable());
                }
            }
            catch (Exception ex)
            {
            }

            return obj_result;
        }
        public Task<List<HsbtUocGD>> GetListChiTietUocGD(decimal hsbt_gd_pr_key)
        {
            var ugd = (from u in _context_pias_update.HsbtUocGds
                           //join gd in _context_pias.HsbtGds on u.FrKey equals gd.PrKey
                       where (u.FrKey == hsbt_gd_pr_key)
                       group u by new { u.FrKey, u.NgayPs } into sp
                       select new HsbtUocGD
                       {
                           PrKey = sp.Max(item => item.PrKey),
                           FrKey = sp.Key.FrKey,
                           NgayPs = sp.Key.NgayPs != null ? Convert.ToDateTime(sp.Key.NgayPs).ToString("dd/MM/yyyy") : null,
                           //NguyenTegd = sp.Sum(item => item.NguyenTegd),
                           //SoTiengd = sp.Sum(item => item.SoTiengd),
                           //NguyenTegdPvi = sp.Sum(item => item.NguyenTegdPvi),
                           //SoTiengdPvi = sp.Sum(item => item.SoTiengdPvi),
                           TyleReten = sp.Average(item => item.TyleReten),
                           //NguyenTegdReten = sp.Sum(item => item.NguyenTegdReten),
                           //SoTiengdReten = sp.Sum(item => item.SoTiengdReten),
                           GhiChu = sp.Max(item => item.GhiChu),
                           NguyenTegdLk = _context_pias_update.HsbtUocGds.Where(x => x.FrKey == sp.Key.FrKey && x.NgayPs <= sp.Key.NgayPs).Sum(s => s.NguyenTegd),
                           SoTiengdLk = _context_pias_update.HsbtUocGds.Where(x => x.FrKey == sp.Key.FrKey && x.NgayPs <= sp.Key.NgayPs).Sum(s => s.SoTiengd),
                           //NguyenTegdPviLk = _context_pias.HsbtUocGds.Where(x => x.FrKey == sp.Key.FrKey && x.NgayPs <= sp.Key.NgayPs).Sum(s => s.NguyenTegdPvi),
                           //SoTiengdPviLk = _context_pias.HsbtUocGds.Where(x => x.FrKey == sp.Key.FrKey && x.NgayPs <= sp.Key.NgayPs).Sum(s => s.SoTiengdPvi),
                           NguyenTegdRetenLk = _context_pias_update.HsbtUocGds.Where(x => x.FrKey == sp.Key.FrKey && x.NgayPs <= sp.Key.NgayPs).Sum(s => s.NguyenTegdReten),
                           SoTiengdRetenLk = _context_pias_update.HsbtUocGds.Where(x => x.FrKey == sp.Key.FrKey && x.NgayPs <= sp.Key.NgayPs).Sum(s => s.SoTiengdReten)

                       }
                       ).OrderBy(x => x.PrKey).AsQueryable();
            return ToListWithNoLockAsync(ugd);
        }
        public List<HsbtThtsView> GetListThuDoi(decimal pr_key_hsgd)
        {
            List<HsbtThtsView> obj_result = new List<HsbtThtsView>();
            try
            {
                var pr_key_bt = _context.HsgdCtus.Where(x => x.PrKey == pr_key_hsgd).Select(s => s.PrKeyBt).FirstOrDefault();
                if (pr_key_bt != 0)
                {
                    obj_result = ToListWithNoLock((from a in _context_pias_update.HsbtThts
                                                   where a.FrKey == pr_key_bt
                                                   select new HsbtThtsView
                                                   {
                                                       PrKey = a.PrKey,
                                                       FrKey = a.FrKey,
                                                       MaSp = a.MaSp,
                                                       LoaiHinhtd = a.LoaiHinhtd,
                                                       MaTte = a.MaTte,
                                                       TygiaTd = a.TygiaTd,
                                                       NguyenTeTd = a.NguyenTeTd,
                                                       SoTienTd = a.SoTienTd,
                                                       GhiChu = a.GhiChu,
                                                       TyleReten = a.TyleReten,
                                                       MtnRetenNte = a.MtnRetenNte,
                                                       MtnRetenVnd = a.MtnRetenVnd,
                                                       MaTtrangTd = a.MaTtrangTd,
                                                       NgayHtoanTd = a.NgayHtoanTd != null ? Convert.ToDateTime(a.NgayHtoanTd).ToString("dd/MM/yyyy") : null,
                                                   }).OrderBy(x => x.PrKey).AsQueryable());
                }
            }
            catch (Exception ex)
            {
            }
            return obj_result;
        }
        public List<HsgdDxView> GetListPASC(decimal pr_key_hsgd_dx_ct, decimal pr_key_hsgd_ctu)
        {
            List<HsgdDxView> obj_result = new List<HsgdDxView>();
            try
            {
                var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == pr_key_hsgd_ctu).FirstOrDefault();
                //loại nhờ giám định hộ
                if (hsgd_ctu.MaLhsbt == "2")
                {
                    var pr_key_hsbt_ctu_ho = _context.HsgdCtus.Where(x => x.PrKeyBtHo == pr_key_hsgd_ctu.ToString()).Select(s => s.PrKeyBt).FirstOrDefault();
                    if (pr_key_hsbt_ctu_ho != 0)
                    {
                        var pr_key_hsbt_ct_ho = _context_pias.HsbtCts.Where(x => x.FrKey == pr_key_hsbt_ctu_ho && x.MaSp == "050104").Select(s => s.PrKey).FirstOrDefault();
                        if (pr_key_hsbt_ct_ho != 0)
                        {
                            pr_key_hsgd_dx_ct = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCt == pr_key_hsbt_ct_ho).Select(s => s.PrKey).FirstOrDefault();
                        }
                    }
                }
                var pr_key_hsbt_ct = _context.HsgdDxCts.Where(x => x.PrKey == pr_key_hsgd_dx_ct).Select(s => s.PrKeyHsbtCt).FirstOrDefault();
                var ma_sp = _context_pias.HsbtCts.Where(x => x.PrKey == pr_key_hsbt_ct).Select(s => s.MaSp).FirstOrDefault();
                if (!string.IsNullOrEmpty(ma_sp))
                {
                    if (new[] { "050101", "050104" }.Contains(ma_sp))
                    {

                        //var pasc = ToListWithNoLock(_context.HsgdDxes.Where(x => x.PrKeyDx == pr_key_hsgd_dx_ct).Join(_context.DmHmucs, a => a.MaHmuc, b => b.MaHmuc, (a, b) => new { a, b.TenHmuc }).OrderBy(x => x.a.PrKey).AsQueryable());
                        var pasc = ToListWithNoLock((from A in _context.HsgdDxes
                                                     join B in _context.DmHmucs on A.MaHmuc equals B.MaHmuc into B1
                                                     from B in B1.DefaultIfEmpty()
                                                     where A.PrKeyDx == pr_key_hsgd_dx_ct
                                                     select new HsgdDxView
                                                     {
                                                         PrKey = A.PrKey,
                                                         FrKey = A.FrKey,
                                                         MaHmuc = A.MaHmuc,
                                                         TenHmuc = B != null ? (B.TenHmuc ?? "") : A.Hmuc,
                                                         SoTientt = A.SoTientt,
                                                         SoTienph = A.SoTienph,
                                                         SoTienson = A.SoTienson,
                                                         VatSc = A.VatSc,
                                                         GiamTruBt = A.GiamTruBt,
                                                         ThuHoiTs = A.ThuHoiTs,
                                                         SoTienDoitru = A.SoTienDoitru,
                                                         GhiChudv = A.GhiChudv,
                                                     }).OrderBy(x => x.PrKey).AsQueryable());
                        obj_result = pasc.Select((r, i) => new HsgdDxView
                        {
                            PrKey = r.PrKey,
                            FrKey = r.FrKey,
                            MaHmuc = r.MaHmuc,
                            TenHmuc = r.TenHmuc,
                            SoTientt = r.SoTientt,
                            SoTienph = r.SoTienph,
                            SoTienson = r.SoTienson,
                            VatSc = r.VatSc,
                            GiamTruBt = r.GiamTruBt,
                            ThuHoiTs = r.ThuHoiTs,
                            SoTienDoitru = r.SoTienDoitru,
                            GhiChudv = r.GhiChudv,
                            Stt = i + 1
                        }).ToList();
                    }
                    else
                    {
                        var pasc = ToListWithNoLock(_context.HsgdDxTsks.Where(x => x.PrKeyDx == pr_key_hsgd_dx_ct).OrderBy(x => x.PrKey).AsQueryable());
                        obj_result = pasc.Select((r, i) => new HsgdDxView
                        {
                            PrKey = r.PrKey,
                            FrKey = r.FrKey,
                            TenHmuc = r.Hmuc,
                            SoTientt = r.SoTientt,
                            SoTiensc = r.SoTiensc,
                            SoTienpdsc = r.SoTienpdsc,
                            GhiChudv = r.GhiChudv,
                            GhiChutt = r.GhiChutt,
                            VatSc = r.VatSc,
                            GiamTruBt = r.GiamTruBt,
                            ThuHoiTs = r.ThuHoiTs,
                            Stt = i + 1
                        }).ToList();
                    }
                }
            }

            catch (Exception ex)
            {
            }
            return obj_result;

        }
        public List<HsgdDxSum> ReloadSum(decimal pr_key_hsgd_dx_ct)
        {
            List<HsgdDxSum> obj_result = new List<HsgdDxSum>();
            try
            {
                var pr_key_hsbt_ct = _context.HsgdDxCts.Where(x => x.PrKey == pr_key_hsgd_dx_ct).Select(s => s.PrKeyHsbtCt).FirstOrDefault();
                var ma_sp = _context_pias.HsbtCts.Where(x => x.PrKey == pr_key_hsbt_ct).Select(s => s.MaSp).FirstOrDefault();
                if (new[] { "050101", "050104" }.Contains(ma_sp))
                {
                    var dx = ToListWithNoLock((from A in _context.HsgdDxCts
                                               join B in _context.HsgdDxes on A.PrKey equals B.PrKeyDx
                                               where A.PrKey == pr_key_hsgd_dx_ct
                                               select new HsgdDxSumTmp
                                               {
                                                   SoTientt = B.SoTientt,
                                                   SoTienph = B.SoTienph,
                                                   SoTienson = B.SoTienson,
                                                   SoTienVat = (B.SoTientt + B.SoTienph + B.SoTienson) * ((decimal)B.VatSc / 100),
                                                   SoTienTtsc = ((B.SoTientt + B.SoTienph + B.SoTienson) + (B.SoTientt + B.SoTienph + B.SoTienson) * ((decimal)B.VatSc / 100)),
                                                   SumSoTienGiamtru = ((((B.SoTientt + B.SoTientt * ((decimal)B.VatSc / 100)) - ((B.SoTientt + B.SoTientt * ((decimal)B.VatSc / 100)) * (A.TyleggPhutungvcx / 100)))
                      + (((B.SoTienph + B.SoTienson) + (B.SoTienph + B.SoTienson) * ((decimal)B.VatSc / 100)) - ((B.SoTienph + B.SoTienson) + (B.SoTienph + B.SoTienson) * ((decimal)B.VatSc / 100)) * (A.TyleggSuachuavcx / 100))) * B.GiamTruBt / 100),
                                                   SoTienGgsc = ((B.SoTientt + B.SoTientt * ((decimal)B.VatSc / 100)) * (A.TyleggPhutungvcx / 100) + ((B.SoTienph + B.SoTienson) + (B.SoTienph + B.SoTienson) * ((decimal)B.VatSc / 100)) * (A.TyleggSuachuavcx / 100)),
                                                   SoTienDoitru = B.SoTienDoitru ?? 0,
                                                   SoTienctkh = A.SoTienctkh,
                                                   SoTienGtbt = A.SoTienGtbt
                                               }).AsQueryable());
                    if (dx != null && dx.Count() > 0)
                    {
                        obj_result = dx.GroupBy(n => new { n.SoTienGtbt, n.SoTienctkh })
                        .Select(s => new HsgdDxSum
                        {
                            Sldx = s.Count(),
                            SumSoTienDoitru = s.Sum(x => x.SoTienDoitru),
                            SumSoTientt = s.Sum(x => x.SoTientt),
                            SumSoTienph = s.Sum(x => x.SoTienph),
                            SumSoTienson = s.Sum(x => x.SoTienson),
                            SumSoTienVat = s.Sum(x => x.SoTienVat),
                            SumSoTienGiamtru = s.Sum(x => x.SumSoTienGiamtru),
                            //SumSoTienGiamtru = s.Sum(x => x.SumSoTienGiamtru) != 0 ? s.Sum(x => x.SumSoTienGiamtru) : s.Key.SoTienGtbt,
                            SumSTDX = s.Sum(x => x.SoTientt + x.SoTienph + x.SoTienson),
                            SumSoTienTtsc = s.Sum(x => x.SoTienTtsc),
                            SumSoTienGgsc = s.Sum(x => x.SoTienGgsc),
                            SoTienctkh = s.Key.SoTienctkh,
                            SoTienGtbt = s.Key.SoTienGtbt
                            //StBl = s.Sum(x => x.SoTientt + x.SoTienph + x.SoTienson + x.SoTienVat - x.SoTienGgsc ) - (s.Sum(x => x.SumSoTienGiamtru) == 0 ? s.Key.SoTienGtbt: s.Sum(x => x.SumSoTienGiamtru)) - s.Key.SoTienctkh
                        }).ToList();
                        obj_result = obj_result.Select(s => new HsgdDxSum
                        {
                            Sldx = s.Sldx,
                            SumSoTienDoitru = s.SumSoTienDoitru,
                            SumSoTientt = s.SumSoTientt,
                            SumSoTienph = s.SumSoTienph,
                            SumSoTienson = s.SumSoTienson,
                            SumSoTienVat = s.SumSoTienVat,
                            SumSoTienGiamtru = s.SumSoTienGiamtru,
                            SumSTDX = s.SumSTDX,
                            SumSoTienTtsc = s.SumSoTienTtsc,
                            SumSoTienGgsc = s.SumSoTienGgsc,
                            StBl = s.SumSoTientt + s.SumSoTienph + s.SumSoTienson + s.SumSoTienVat - s.SumSoTienGgsc - (s.SumSoTienGiamtru != 0 ? s.SumSoTienGiamtru : s.SoTienGtbt) - s.SoTienctkh - s.SumSoTienDoitru,
                            SoTienctkh = s.SoTienctkh,
                            SoTienGtbt = s.SoTienGtbt
                        }).ToList();
                    }

                }
                else
                {
                    var dx = ToListWithNoLock((from B in _context.HsgdDxTsks
                                               where B.PrKeyDx == pr_key_hsgd_dx_ct
                                               select new HsgdDxTsksSumTmp
                                               {
                                                   SoTientt = B.SoTientt,
                                                   SoTiensc = B.SoTiensc,
                                                   SoTienVat = (B.SoTientt + B.SoTiensc) * ((decimal)B.VatSc / 100),
                                                   SoTienThts = ((B.SoTientt + B.SoTiensc) + (B.SoTientt + B.SoTiensc) * ((decimal)B.VatSc / 100)) * B.GiamTruBt / 100
                                               }).AsQueryable());
                    if (dx != null && dx.Count() > 0)
                    {
                        obj_result = dx.GroupBy(n => 1 == 1)
                        .Select(s => new HsgdDxSum
                        {
                            Sldx = s.Count(),
                            SumSoTientt = s.Sum(x => x.SoTientt),
                            SumSoTiensc = s.Sum(x => x.SoTiensc),
                            SumTskSoTienVat = s.Sum(x => x.SoTienVat),
                            SumTskStdx = s.Sum(x => x.SoTientt + x.SoTiensc),
                            SumTskTtsc = s.Sum(x => x.SoTientt + x.SoTiensc + x.SoTienVat),
                            SumTskSoTienGiamtru = s.Sum(x => x.SoTienThts)
                        }).ToList();
                    }
                }

            }

            catch (Exception ex)
            {
            }
            return obj_result;

        }
        public async Task<string> CreateHsbtCt_save(HsbtCt hsbtCt, HsgdDxCt hsgdDxCt, HsbtUoc hsbtUoc, decimal prKeyHsgdCtu, List<FileAttachBt> fileAttach)
        {
            var result = "";
            HsbtCtu hsbtCtu = new HsbtCtu();
            // trường hợp chưa tạo hsbt
            var hsgd_ctu = await _context.HsgdCtus.Where(x => x.PrKey == prKeyHsgdCtu).FirstOrDefaultAsync();
            if (hsbtCt.FrKey == 0 && hsgd_ctu.PrKeyBt == 0)
            {
                var Key_Find1 = 0;
                decimal Key_Nbt_Find = 0;

                if (hsgd_ctu != null)
                {
                    Key_Find1 = hsgd_ctu.PrKey;
                    if (hsgd_ctu.MaLhsbt == "2")
                    {
                        var hsgd_ctu_ho = await _context.HsgdCtus.Where(x => x.PrKeyBtHo == prKeyHsgdCtu.ToString() && x.MaLhsbt == "3").FirstOrDefaultAsync();
                        if (hsgd_ctu_ho != null)
                        {
                            Key_Find1 = hsgd_ctu_ho.PrKey;
                        }
                    }
                    if (hsgd_ctu.MaLhsbt == "3")
                    {
                        Key_Nbt_Find = await _context.HsgdCtus.Where(x => x.PrKey.ToString() == hsgd_ctu.PrKeyBtHo).Select(s => s.PrKeyBt).FirstOrDefaultAsync();
                    }
                    var hsgd_bth = await _context.HsgdCtus.Where(x => x.PrKey == Key_Find1).FirstOrDefaultAsync();
                    if (hsgd_bth != null)
                    {
                        var strMa_donvi = hsgd_ctu.MaDonvi;
                        var Ma_dvitat = hsgd_ctu.MaDonvi;
                        if (hsgd_ctu.NgayCtu != null && hsgd_ctu.NgayCtu.Value.Date < DateTime.ParseExact("01/01/2020", "dd/MM/yyyy", CultureInfo.InvariantCulture))
                        {
                            if (hsgd_ctu.NgayCtu.Value.Date > DateTime.ParseExact("01/06/2019", "dd/MM/yyyy", CultureInfo.InvariantCulture))
                            {
                                if (hsgd_ctu.MaLhsbt == "1" && new[] { "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "26", "28", "36", "37", "40" }.Contains(strMa_donvi))
                                {
                                    if (hsgd_bth.NgayCtu != null && (DateTime.ParseExact("01/04/2019", "dd/MM/yyyy", CultureInfo.InvariantCulture) - hsgd_bth.NgayCtu).Value.TotalDays >= 0)
                                    {
                                        strMa_donvi = "31";
                                        Ma_dvitat = "31";
                                    }
                                }
                            }
                            else
                            {
                                if (new[] { "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "26", "28", "36", "37", "40" }.Contains(strMa_donvi))
                                {
                                    if (hsgd_bth.NgayCtu != null && (DateTime.ParseExact("01/04/2019", "dd/MM/yyyy", CultureInfo.InvariantCulture) - hsgd_bth.NgayCtu).Value.TotalDays >= 0)
                                    {
                                        strMa_donvi = "31";
                                        Ma_dvitat = "31";
                                    }
                                }
                            }
                        }
                        int So_lanBT = 0;
                        if (string.IsNullOrEmpty(hsgd_bth.SoSeri.ToString()))
                        {
                            So_lanBT = await _context_pias_update.HsbtCtus.Where(x => x.SoHdgcn == hsgd_bth.SoDonbh).CountAsync();
                        }
                        else
                        {
                            So_lanBT = await _context_pias_update.HsbtCtus.Where(x => x.SoSeri == hsgd_ctu.SoSeri && x.SoHdgcn == hsgd_bth.SoDonbh).CountAsync();
                        }
                        var so_hsbt = "";
                        var ma_hieu = await _context_pias.DmDonbhs.Where(x => x.MaDonbh == hsgd_bth.MaDonbh).Select(s => s.MaHieu).FirstOrDefaultAsync();
                        if (string.IsNullOrEmpty(ma_hieu))
                        {
                            return "Không lấy được mã hiệu để tạo số hsbt. Vui lòng thử lại!";
                        }
                        var ma_ctu = hsgd_bth.MaDonbh == "0501" ? "BT01" : "BT12";
                        var ctuKt = GetCtuKt(ma_ctu, int.Parse(Ma_dvitat));
                        int newID = 0;
                        string newID1 = "";
                        if (ctuKt != null)
                        {
                            newID = (int)ctuKt.Num;
                            newID1 = newID.ToString("D6");
                        }
                        else
                        {
                            return "Không lấy được số tự tăng của hsbt. Vui lòng thử lại!";
                        }
                        if (strMa_donvi == "31")
                        {
                            so_hsbt = $"{DateTime.Now.Year.ToString().Substring(2)}/{Ma_dvitat}/{hsgd_ctu.MaDonvi}/{ma_hieu}/CL{newID1}";
                        }
                        else
                        {
                            var tenTat = await _context_pias.DmKhaches.Where(k => k.PhongBan == true && k.MaKh == hsgd_ctu.MaPkt).Select(k => k.TenTat).FirstOrDefaultAsync();
                            if (string.IsNullOrEmpty(tenTat))
                            {
                                return "Không lấy được tên tắt của phòng khai thác để tạo số hsbt. Vui lòng thử lại!";
                            }
                            so_hsbt = $"{DateTime.Now.Year.ToString().Substring(2)}/{Ma_dvitat}/{tenTat}/{ma_hieu}/CL{newID1}";
                        }
                        //check trùng so_hsbt
                        _logger.Information("CreateHsbtCt: Check trung_hsbt");
                        var count_sohsbt = await _context_pias_update.HsbtCtus.Where(x => x.SoHsbt.Substring(0, 5) == so_hsbt.Trim().Substring(0, 5) && x.SoHsbt.Substring(9, 4) == so_hsbt.Trim().Substring(9, 4) && x.SoHsbt.Substring(x.SoHsbt.Length - 1, 6) == so_hsbt.Trim().Substring(so_hsbt.Length - 6, 6)).CountAsync();
                        if (count_sohsbt > 0)
                        {
                            return "Số hsbt bị trùng. Vui lòng thử lại!";
                        }
                        _logger.Information("CreateHsbtCt: end Check trung_hsbt");
                        hsbtCtu.PrKeyGoc = hsgd_bth.PrKeyGoc;
                        hsbtCtu.PrKeySeri = hsgd_bth.PrKeySeri;
                        hsbtCtu.MaDonvi = strMa_donvi;
                        hsbtCtu.MaCtu = ma_ctu;
                        hsbtCtu.NgayCtu = DateTime.Today;
                        hsbtCtu.SoLanBt = So_lanBT + 1;
                        hsbtCtu.MaPkt = hsgd_ctu.MaPkt;
                        hsbtCtu.MaDonbh = hsgd_bth.MaDonbh;
                        hsbtCtu.SoHsbt = so_hsbt;
                        hsbtCtu.SoHdgcn = hsgd_bth.SoDonbh;
                        hsbtCtu.SoSeri = hsgd_bth.SoSeri;
                        hsbtCtu.MaKthac = hsgd_bth.MaKthac;
                        hsbtCtu.MaDaily = hsgd_bth.MaDaily;
                        hsbtCtu.MaCbkt = hsgd_bth.MaCbkt;
                        hsbtCtu.NgayDau = hsgd_bth.NgayDauSeri;
                        hsbtCtu.NgayCuoi = hsgd_bth.NgayCuoiSeri;
                        hsbtCtu.TenDttt = hsgd_bth.BienKsoat;
                        hsbtCtu.NgayTthat = hsgd_bth.NgayTthat;
                        hsbtCtu.NgayTbao = hsgd_bth.NgayTbao;
                        hsbtCtu.MaKh = hsgd_bth.MaKh;
                        hsbtCtu.DiaChi = hsgd_bth.DiaChi;
                        hsbtCtu.NguyenNhanTtat = hsgd_bth.NguyenNhanTtat;
                        hsbtCtu.GhiChu = hsgd_bth.GhiChu;
                        hsbtCtu.NguyenNhan = hsgd_bth.NguyenNhanTtat;
                        hsbtCtu.DiaDiem = hsgd_bth.DiaDiemtt;
                        hsbtCtu.MaDdiemTthat = hsgd_bth.MaDdiemTthat;
                        hsbtCtu.TenKhle = hsgd_bth.TenKhach;
                        hsbtCtu.NamSinh = hsgd_bth.NamSinh;
                        hsbtCtu.MaLhsbt = hsgd_ctu.MaLhsbt == "1" ? "TBT" : (hsgd_ctu.MaLhsbt == "2" ? "NBT" : "BTH");
                        hsbtCtu.MaDvbtHo = hsgd_bth.MaDvbtHo;
                        //trường hợp update thì update thêm ma_loaixe,ma_gara_vcx,Ma_gara_tnds,namsx_vcx,loai_xe_tnds,loai_xe
                        hsbtCtu.MaCbcnv = hsgd_bth.MaCbkt;
                        hsbtCtu.HosoPhaply = hsgd_bth.HosoPhaply;
                        hsbtCtu.YkienGdinh = hsgd_bth.YkienGdinh;
                        hsbtCtu.DexuatPan = hsgd_bth.DexuatPan;
                        hsbtCtu.TenLaixe = hsgd_bth.TenLaixe;
                        hsbtCtu.SoGphepLaixe = hsgd_bth.SoGphepLaixe;
                        hsbtCtu.NgayDauLaixe = hsgd_bth.NgayDauLaixe;
                        hsbtCtu.NgayCuoiLaixe = hsgd_bth.NgayCuoiLaixe;
                        hsbtCtu.SoGphepLuuhanh = hsgd_bth.SoGphepLuuhanh;
                        hsbtCtu.NgayDauLuuhanh = hsgd_bth.NgayDauLuuhanh;
                        hsbtCtu.NgayCuoiLuuhanh = hsgd_bth.NgayCuoiLuuhanh;
                        hsbtCtu.NgdcBh = hsgd_bth.TenKhach;
                        hsbtCtu.DienThoai = hsgd_bth.DienThoai;
                        hsbtCtu.PrKeySeri = hsgd_bth.PrKeySeri;
                        hsbtCtu.NgayGdinh = hsgd_bth.NgayGdinh;
                        hsbtCtu.MaLoaibang = hsgd_bth.MaLoaibang;
                        hsbtCtu.MaLoaixe = hsgd_bth.MaNhloaixe;
                        hsbtCtu.MaDonviTt = hsgd_bth.MaDonviTt;
                        var tyle_dong = await (from A in _context_pias.NvuBhtCtus
                                               where A.SoDonbh == hsgd_bth.SoDonbh && A.MaSdbs == ""
                                               select 100 - ((A.TyleDong != 0 ? 100 - A.TyleDong : 0) + _context_pias.NvuBhtDbhs
                                                   .Where(dbh => dbh.FrKey == A.PrKey)
                                                   .Sum(dbh => dbh.TyleTg) * (A.TyleDong != 0 ? A.TyleDong : 100) / 100)).FirstOrDefaultAsync();
                        hsbtCtu.TyleDong = tyle_dong;

                    }
                }
            }

            _logger.Information("CreateHsbtCt prKeyHsgdCtu = " + prKeyHsgdCtu + " pr_key_bt = " + hsgd_ctu.PrKeyBt + " trước execute có hsbt_ct =" + JsonConvert.SerializeObject(hsbtCt));

            var context_gdtt_new = new GdttContext();
            var dbContextTransaction = await context_gdtt_new.Database.BeginTransactionAsync();

            var context_pias_new = new Pvs2024UpdateContext();
            context_pias_new.Database.SetCommandTimeout(9600);
            var dbContextTransaction2 = await context_pias_new.Database.BeginTransactionAsync();
            try
            {
                _logger.Information("CreateHsbtCt prKeyHsgdCtu = " + prKeyHsgdCtu + " pr_key_bt = " + hsgd_ctu.PrKeyBt + " có hsbt_ct =" + JsonConvert.SerializeObject(hsbtCt));
                if (hsbtCt.FrKey == 0 && hsgd_ctu.PrKeyBt == 0)
                {
                    //Add hsbt
                    await context_pias_new.HsbtCtus.AddAsync(hsbtCtu);
                    _logger.Information("CreateHsbtCt prKeyHsgdCtu = " + prKeyHsgdCtu + " pr_key_bt = " + hsgd_ctu.PrKeyBt + " có hsbt_ctu =" + JsonConvert.SerializeObject(hsbtCtu));
                    await context_pias_new.SaveChangesAsync();

                    hsbtCt.FrKey = hsbtCtu.PrKey;
                    //update lại pr_key_bt bảng hsgd_ctu
                    var hsgd_ctu_up = await context_gdtt_new.HsgdCtus.Where(x => x.PrKey == prKeyHsgdCtu).FirstOrDefaultAsync();
                    hsgd_ctu_up.PrKeyBt = hsbtCtu.PrKey;
                    context_gdtt_new.HsgdCtus.Update(hsgd_ctu_up);
                    await context_gdtt_new.SaveChangesAsync();

                    _logger.Information("CreateHsbtCt prKeyHsgdCtu = " + prKeyHsgdCtu + " pr_key_bt = " + hsgd_ctu.PrKeyBt + " thêm hsbt pr_key = " + hsbtCtu.PrKey + " so_hsbt = " + hsbtCtu.SoHsbt + " có hsbt_ct =" + JsonConvert.SerializeObject(hsbtCt));
                }
                hsbtCt.PrKey = await context_pias_new.HsbtCts.AsQueryable().MaxAsync(x => x.PrKey) + 1;
                _logger.Information("CreateHsbtCt prKeyHsgdCtu = " + prKeyHsgdCtu + " pr_key HsbtCt " + hsbtCt.PrKey);
                await context_pias_new.HsbtCts.AddAsync(hsbtCt);
                await context_pias_new.SaveChangesAsync();
                hsbtUoc.FrKey = hsbtCt.PrKey;
                _logger.Information("CreateHsbtCt prKeyHsgdCtu = " + prKeyHsgdCtu + " hsbtUoc " + JsonConvert.SerializeObject(hsbtUoc));
                await context_pias_new.HsbtUocs.AddAsync(hsbtUoc);
                await context_pias_new.SaveChangesAsync();

                hsgdDxCt.PrKeyHsbtCt = hsbtCt.PrKey;
                hsgdDxCt.PrKeyHsbtCtu = hsbtCt.FrKey;
                hsgdDxCt.MaSp = hsbtCt.MaSp;
                hsgdDxCt.MaDkhoan = hsbtCt.MaDkhoan;
                await context_gdtt_new.HsgdDxCts.AddAsync(hsgdDxCt);
                _logger.Information("CreateHsbtCt prKeyHsgdCtu = " + prKeyHsgdCtu + " hsbtDxCt " + JsonConvert.SerializeObject(hsgdDxCt));
                await context_gdtt_new.SaveChangesAsync();
                _logger.Information("CreateHsbtCt prKeyHsgdCtu = " + prKeyHsgdCtu + "Save thành công");
                //file_attach_bt
                if (fileAttach != null && fileAttach.Count > 0)
                {
                    for (int j = 0; j < fileAttach.Count; j++)
                    {
                        fileAttach[j].PrKey = await context_pias_new.FileAttachBts.AsQueryable().MaxAsync(x => x.PrKey) + 1;
                        fileAttach[j].FrKey = hsbtCt.PrKey;
                        await context_pias_new.FileAttachBts.AddAsync(fileAttach[j]);
                        await context_pias_new.SaveChangesAsync();
                    }
                }
                await dbContextTransaction.CommitAsync();

                await dbContextTransaction2.CommitAsync();


                result = hsbtCt.PrKey.ToString();
            }
            catch (Exception ex)
            {
                result = "0";
                _logger.Error("CreateHsbtCt prKeyHsgdCtu = " + prKeyHsgdCtu + " AddHsbtCt Exception : " + ex.ToString());
                _logger.Error("CreateHsbtCt prKeyHsgdCtu = " + prKeyHsgdCtu + "AddHsbtCt Error record  hsbtCt =" + JsonConvert.SerializeObject(hsbtCt));
                await dbContextTransaction2.RollbackAsync();
                await dbContextTransaction2.DisposeAsync();
                await dbContextTransaction.RollbackAsync();
                await dbContextTransaction.DisposeAsync();
                throw;
            }
            _logger.Information("CreateHsbtCt prKeyHsgdCtu = " + prKeyHsgdCtu + " pr_key_bt = " + hsgd_ctu.PrKeyBt + " sau execute có hsbt_ct =" + JsonConvert.SerializeObject(hsbtCt));
            _logger.Information("CreateHsbtCt có hsbt_uoc =" + JsonConvert.SerializeObject(hsbtUoc));
            return result;
        }
        public async Task<string> CreateHsbtCt(HsbtCt hsbtCt, HsgdDxCt hsgdDxCt, HsbtUoc hsbtUoc, decimal prKeyHsgdCtu, List<FileAttachBt> fileAttach)
        {
            var result = "";
            // trường hợp chưa tạo hsbt
            if (hsbtCt.FrKey == 0)
            {
                try
                {
                    var rowModifed = _context.Database.ExecuteSqlRaw("update hsgd_ctu set pr_key_bt = -1 where pr_key = " + prKeyHsgdCtu + " and pr_key_bt = 0 ");
                    _logger.Information($"update  pr_key_bt = -1 hsgd_ctu có pr_key = " + prKeyHsgdCtu);
                    if (rowModifed > 0)
                    {
                        HsbtCtu hsbtCtu = new HsbtCtu();
                        var Key_Find1 = 0;
                        decimal Key_Nbt_Find = 0;
                        string vai_tro = "";
                        var hsgd_ctu = await _context.HsgdCtus.Where(x => x.PrKey == prKeyHsgdCtu).FirstOrDefaultAsync();
                        _logger.Information($"CreateHsbtCt 1 pr_key = " + prKeyHsgdCtu);
                        if (hsgd_ctu != null)
                        {
                            _logger.Information($"CreateHsbtCt 2 pr_key = " + prKeyHsgdCtu);
                            Key_Find1 = hsgd_ctu.PrKey;
                            if (hsgd_ctu.MaLhsbt == "2")
                            {
                                var hsgd_ctu_ho = await _context.HsgdCtus.Where(x => x.PrKeyBtHo == prKeyHsgdCtu.ToString() && x.MaLhsbt == "3").FirstOrDefaultAsync();
                                if (hsgd_ctu_ho != null)
                                {
                                    Key_Find1 = hsgd_ctu_ho.PrKey;
                                }
                            }
                            if (hsgd_ctu.MaLhsbt == "3")
                            {
                                Key_Nbt_Find = await _context.HsgdCtus.Where(x => x.PrKey.ToString() == hsgd_ctu.PrKeyBtHo).Select(s => s.PrKeyBt).FirstOrDefaultAsync();
                            }
                            var hsgd_bth = await _context.HsgdCtus.Where(x => x.PrKey == Key_Find1).FirstOrDefaultAsync();
                            if (hsgd_bth != null)
                            {
                                var strMa_donvi = hsgd_ctu.MaDonvi;
                                var Ma_dvitat = hsgd_ctu.MaDonvi;
                                if (hsgd_ctu.NgayCtu != null && hsgd_ctu.NgayCtu.Value.Date < DateTime.ParseExact("01/01/2020", "dd/MM/yyyy", CultureInfo.InvariantCulture))
                                {
                                    if (hsgd_ctu.NgayCtu.Value.Date > DateTime.ParseExact("01/06/2019", "dd/MM/yyyy", CultureInfo.InvariantCulture))
                                    {
                                        if (hsgd_ctu.MaLhsbt == "1" && new[] { "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "26", "28", "36", "37", "40" }.Contains(strMa_donvi))
                                        {
                                            if (hsgd_bth.NgayCtu != null && (DateTime.ParseExact("01/04/2019", "dd/MM/yyyy", CultureInfo.InvariantCulture) - hsgd_bth.NgayCtu).Value.TotalDays >= 0)
                                            {
                                                strMa_donvi = "31";
                                                Ma_dvitat = "31";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (new[] { "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "26", "28", "36", "37", "40" }.Contains(strMa_donvi))
                                        {
                                            if (hsgd_bth.NgayCtu != null && (DateTime.ParseExact("01/04/2019", "dd/MM/yyyy", CultureInfo.InvariantCulture) - hsgd_bth.NgayCtu).Value.TotalDays >= 0)
                                            {
                                                strMa_donvi = "31";
                                                Ma_dvitat = "31";
                                            }
                                        }
                                    }
                                }
                                int So_lanBT = 0;
                                if (string.IsNullOrEmpty(hsgd_bth.SoSeri.ToString()))
                                {
                                    So_lanBT = await _context_pias_update.HsbtCtus.Where(x => x.SoHdgcn == hsgd_bth.SoDonbh).CountAsync();
                                }
                                else
                                {
                                    So_lanBT = await _context_pias_update.HsbtCtus.Where(x => x.SoSeri == hsgd_ctu.SoSeri && x.SoHdgcn == hsgd_bth.SoDonbh).CountAsync();
                                }
                                var so_hsbt = "";
                                var ma_hieu = await _context_pias.DmDonbhs.Where(x => x.MaDonbh == hsgd_bth.MaDonbh).Select(s => s.MaHieu).FirstOrDefaultAsync();
                                if (string.IsNullOrEmpty(ma_hieu))
                                {
                                    return "Không lấy được mã hiệu để tạo số hsbt. Vui lòng thử lại!";
                                }
                                var ma_ctu = hsgd_bth.MaDonbh == "0501" ? "BT01" : "BT12";
                                var ctuKt = GetCtuKt(ma_ctu, int.Parse(Ma_dvitat));
                                int newID = 0;
                                string newID1 = "";
                                if (ctuKt != null)
                                {
                                    newID = (int)ctuKt.Num;
                                    newID1 = newID.ToString("D6");
                                }
                                else
                                {
                                    return "Không lấy được số tự tăng của hsbt. Vui lòng thử lại!";
                                }
                                if (strMa_donvi == "31")
                                {
                                    so_hsbt = $"{DateTime.Now.Year.ToString().Substring(2)}/{Ma_dvitat}/{hsgd_ctu.MaDonvi}/{ma_hieu}/CL{newID1}";
                                }
                                else
                                {
                                    var tenTat = await _context_pias.DmKhaches.Where(k => k.PhongBan == true && k.MaKh == hsgd_ctu.MaPkt).Select(k => k.TenTat).FirstOrDefaultAsync();
                                    if (string.IsNullOrEmpty(tenTat))
                                    {
                                        return "Không lấy được tên tắt của phòng khai thác để tạo số hsbt. Vui lòng thử lại!";
                                    }
                                    so_hsbt = $"{DateTime.Now.Year.ToString().Substring(2)}/{Ma_dvitat}/{tenTat}/{ma_hieu}/CL{newID1}";
                                }
                                //check trùng so_hsbt
                                _logger.Information("CreateHsbtCt: Check trung_hsbt");
                                var count_sohsbt = await _context_pias_update.HsbtCtus.Where(x => x.SoHsbt.Substring(0, 5) == so_hsbt.Trim().Substring(0, 5) && x.SoHsbt.Substring(9, 4) == so_hsbt.Trim().Substring(9, 4) && x.SoHsbt.Substring(x.SoHsbt.Length - 1, 6) == so_hsbt.Trim().Substring(so_hsbt.Length - 6, 6)).CountAsync();
                                if (count_sohsbt > 0)
                                {
                                    return "Số hsbt bị trùng. Vui lòng thử lại!";
                                }
                                _logger.Information("CreateHsbtCt: end Check trung_hsbt");
                                hsbtCtu.PrKeyGoc = hsgd_bth.PrKeyGoc;
                                hsbtCtu.PrKeySeri = hsgd_bth.PrKeySeri;
                                hsbtCtu.MaDonvi = strMa_donvi;
                                hsbtCtu.MaCtu = ma_ctu;
                                hsbtCtu.NgayCtu = DateTime.Today;
                                hsbtCtu.SoLanBt = So_lanBT + 1;
                                hsbtCtu.MaPkt = hsgd_ctu.MaPkt;
                                hsbtCtu.MaDonbh = hsgd_bth.MaDonbh;
                                hsbtCtu.SoHsbt = so_hsbt;
                                hsbtCtu.SoHdgcn = hsgd_bth.SoDonbh;
                                hsbtCtu.SoSeri = hsgd_bth.SoSeri;
                                hsbtCtu.MaKthac = hsgd_bth.MaKthac;
                                hsbtCtu.MaDaily = hsgd_bth.MaDaily;
                                hsbtCtu.MaCbkt = hsgd_bth.MaCbkt;
                                hsbtCtu.NgayDau = hsgd_bth.NgayDauSeri;
                                hsbtCtu.NgayCuoi = hsgd_bth.NgayCuoiSeri;
                                hsbtCtu.TenDttt = hsgd_bth.BienKsoat;
                                hsbtCtu.NgayTthat = hsgd_bth.NgayTthat != null ? hsgd_bth.NgayTthat.Value.Date : null;
                                hsbtCtu.NgayTbao = hsgd_bth.NgayTbao != null ? hsgd_bth.NgayTbao.Value.Date : null;
                                hsbtCtu.MaKh = hsgd_bth.MaKh;
                                hsbtCtu.DiaChi = hsgd_bth.DiaChi;
                                hsbtCtu.NguyenNhanTtat = hsgd_bth.NguyenNhanTtat;
                                hsbtCtu.GhiChu = hsgd_bth.GhiChu;
                                hsbtCtu.NguyenNhan = hsgd_bth.NguyenNhanTtat;
                                hsbtCtu.DiaDiem = hsgd_bth.DiaDiemtt;
                                hsbtCtu.MaDdiemTthat = hsgd_bth.MaDdiemTthat;
                                hsbtCtu.TenKhle = hsgd_bth.TenKhach;
                                hsbtCtu.NamSinh = hsgd_bth.NamSinh;
                                hsbtCtu.MaLhsbt = hsgd_ctu.MaLhsbt == "1" ? "TBT" : (hsgd_ctu.MaLhsbt == "2" ? "NBT" : "BTH");
                                hsbtCtu.MaDvbtHo = hsgd_bth.MaDvbtHo;
                                //trường hợp update thì update thêm ma_loaixe,ma_gara_vcx,Ma_gara_tnds,namsx_vcx,loai_xe_tnds,loai_xe
                                hsbtCtu.MaCbcnv = hsgd_bth.MaCbkt;
                                hsbtCtu.HosoPhaply = hsgd_bth.HosoPhaply;
                                hsbtCtu.YkienGdinh = hsgd_bth.YkienGdinh;
                                hsbtCtu.DexuatPan = hsgd_bth.DexuatPan;
                                hsbtCtu.TenLaixe = hsgd_bth.TenLaixe;
                                hsbtCtu.SoGphepLaixe = hsgd_bth.SoGphepLaixe;
                                hsbtCtu.NgayDauLaixe = hsgd_bth.NgayDauLaixe;
                                hsbtCtu.NgayCuoiLaixe = hsgd_bth.NgayCuoiLaixe;
                                hsbtCtu.SoGphepLuuhanh = hsgd_bth.SoGphepLuuhanh;
                                hsbtCtu.NgayDauLuuhanh = hsgd_bth.NgayDauLuuhanh;
                                hsbtCtu.NgayCuoiLuuhanh = hsgd_bth.NgayCuoiLuuhanh;
                                hsbtCtu.NgdcBh = hsgd_bth.TenKhach;
                                hsbtCtu.DienThoai = hsgd_bth.DienThoai;
                                hsbtCtu.PrKeySeri = hsgd_bth.PrKeySeri;
                                hsbtCtu.NgayGdinh = hsgd_bth.NgayGdinh;
                                hsbtCtu.MaLoaibang = hsgd_bth.MaLoaibang;
                                hsbtCtu.MaLoaixe = hsgd_bth.MaNhloaixe;
                                hsbtCtu.MaDonviTt = hsgd_bth.MaDonviTt;
                                var tyle_dong = await (from A in _context_pias.NvuBhtCtus
                                                       where A.SoDonbh == hsgd_bth.SoDonbh && A.MaSdbs == ""
                                                       select 100 - ((A.TyleDong != 0 ? 100 - A.TyleDong : 0) + _context_pias.NvuBhtDbhs
                                                           .Where(dbh => dbh.FrKey == A.PrKey)
                                                           .Sum(dbh => dbh.TyleTg) * (A.TyleDong != 0 ? A.TyleDong : 100) / 100)).FirstOrDefaultAsync();
                                hsbtCtu.TyleDong = tyle_dong;
                                _logger.Information($"CreateHsbtCt 3 pr_key = " + prKeyHsgdCtu);
                                var vaiTro = await (from a in _context_pias.NvuBhtCtus
                                                    join b in _context_pias.NvuBhtDbhs on a.PrKey equals b.FrKey
                                                    where b.VaiTro == "Đồng chính"
                                                          && a.SoDonbh == hsgd_bth.SoDonbh
                                                          && a.MaSdbs == ""
                                                    select b.VaiTro).FirstOrDefaultAsync();
                                if (vaiTro == null)
                                    vai_tro = "Đồng chính";
                                else
                                    vai_tro = "Đồng phụ";
                                _logger.Information($"CreateHsbtCt 4 pr_key = " + prKeyHsgdCtu);
                            }
                        }
                        _logger.Information("CreateHsbtCt chưa có hsbt_ctu prKeyHsgdCtu11 = " + prKeyHsgdCtu + " pr_key_bt = " + hsgd_ctu.PrKeyBt + " trước execute có hsbt_ct =" + JsonConvert.SerializeObject(hsbtCt));

                        var context_gdtt_new = new GdttContext();
                        var dbContextTransaction = await context_gdtt_new.Database.BeginTransactionAsync();

                        var context_pias_new = new Pvs2024UpdateContext();
                        context_pias_new.Database.SetCommandTimeout(9600);
                        var dbContextTransaction2 = await context_pias_new.Database.BeginTransactionAsync();
                        try
                        {
                            _logger.Information("CreateHsbtCt chưa có hsbt_ctu prKeyHsgdCtu22 = " + prKeyHsgdCtu + " pr_key_bt = " + hsgd_ctu.PrKeyBt + " có hsbt_ct =" + JsonConvert.SerializeObject(hsbtCt));
                            //Add hsbt
                            await context_pias_new.HsbtCtus.AddAsync(hsbtCtu);
                            _logger.Information("CreateHsbtCt chưa có hsbt_ctu prKeyHsgdCtu33 = " + prKeyHsgdCtu + " pr_key_bt = " + hsgd_ctu.PrKeyBt + " có hsbt_ctu =" + JsonConvert.SerializeObject(hsbtCtu));
                            await context_pias_new.SaveChangesAsync();

                            hsbtCt.FrKey = hsbtCtu.PrKey;
                            //update lại pr_key_bt bảng hsgd_ctu
                            var hsgd_ctu_up = await context_gdtt_new.HsgdCtus.Where(x => x.PrKey == prKeyHsgdCtu).FirstOrDefaultAsync();
                            hsgd_ctu_up.PrKeyBt = hsbtCtu.PrKey;
                            hsgd_ctu_up.SoHsbt = hsbtCtu.SoHsbt;
                            hsgd_ctu_up.TyleTg = (decimal)(hsbtCtu.TyleDong ?? 100);
                            hsgd_ctu_up.VaiTro = vai_tro;
                            context_gdtt_new.HsgdCtus.Update(hsgd_ctu_up);
                            await context_gdtt_new.SaveChangesAsync();

                            _logger.Information("CreateHsbtCt chưa có hsbt_ctu prKeyHsgdCtu44 = " + prKeyHsgdCtu + " pr_key_bt = " + hsgd_ctu.PrKeyBt + " thêm hsbt pr_key = " + hsbtCtu.PrKey + " so_hsbt = " + hsbtCtu.SoHsbt + " có hsbt_ct =" + JsonConvert.SerializeObject(hsbtCt));
                            hsbtCt.PrKey = await context_pias_new.HsbtCts.AsQueryable().MaxAsync(x => x.PrKey) + 1;
                            //hsbtCt.PrKey = await context_pias_new.Database.SqlQuery<long>($"SELECT NEXT VALUE FOR sqe_hsbt_ct").FirstAsync();
                            
                            _logger.Information("CreateHsbtCt chưa có hsbt_ctu prKeyHsgdCtu55 = " + prKeyHsgdCtu + " pr_key HsbtCt " + hsbtCt.PrKey);
                            await context_pias_new.HsbtCts.AddAsync(hsbtCt);
                            await context_pias_new.SaveChangesAsync();
                            hsbtUoc.FrKey = hsbtCt.PrKey;
                            _logger.Information("CreateHsbtCt chưa có hsbt_ctu prKeyHsgdCtu66 = " + prKeyHsgdCtu + " hsbtUoc " + JsonConvert.SerializeObject(hsbtUoc));
                            await context_pias_new.HsbtUocs.AddAsync(hsbtUoc);
                            await context_pias_new.SaveChangesAsync();

                            hsgdDxCt.PrKeyHsbtCt = hsbtCt.PrKey;
                            hsgdDxCt.PrKeyHsbtCtu = hsbtCt.FrKey;
                            hsgdDxCt.MaSp = hsbtCt.MaSp;
                            hsgdDxCt.MaDkhoan = hsbtCt.MaDkhoan;
                            await context_gdtt_new.HsgdDxCts.AddAsync(hsgdDxCt);
                            _logger.Information("CreateHsbtCt chưa có hsbt_ctu prKeyHsgdCtu77 = " + prKeyHsgdCtu + " hsbtDxCt " + JsonConvert.SerializeObject(hsgdDxCt));
                            await context_gdtt_new.SaveChangesAsync();
                            _logger.Information("CreateHsbtCt chưa có hsbt_ctu prKeyHsgdCtu88 = " + prKeyHsgdCtu + "Save thành công");
                            //file_attach_bt
                            if (fileAttach != null && fileAttach.Count > 0)
                            {
                                for (int j = 0; j < fileAttach.Count; j++)
                                {
                                    fileAttach[j].PrKey = await context_pias_new.FileAttachBts.AsQueryable().MaxAsync(x => x.PrKey) + 1;
                                    fileAttach[j].FrKey = hsbtCt.PrKey;
                                    await context_pias_new.FileAttachBts.AddAsync(fileAttach[j]);
                                    await context_pias_new.SaveChangesAsync();
                                }
                            }
                            await dbContextTransaction.CommitAsync();

                            await dbContextTransaction2.CommitAsync();


                            result = hsbtCt.PrKey.ToString();
                        }
                        catch (Exception ex)
                        {
                            result = "0";
                            //_context.Database.ExecuteSqlRaw("update hsgd_ctu set pr_key_bt = 0 where pr_key = " + prKeyHsgdCtu + " and pr_key_bt = -1 ");
                            try
                            {
                                using (var ctx = new GdttContext())
                                {
                                    ctx.Database.ExecuteSqlRaw("update hsgd_ctu set pr_key_bt = 0 where pr_key = " + prKeyHsgdCtu + " and pr_key_bt = -1 ");
                                }
                            }
                            catch (Exception ex2)
                            {
                                _logger.Error("Rollback failed for pr_key = " + prKeyHsgdCtu + ": " + ex2.ToString());
                            }

                            _logger.Error("CreateHsbtCt chưa có hsbt_ctu prKeyHsgdCtu = " + prKeyHsgdCtu + " Exception : " + ex.ToString());
                            _logger.Error("CreateHsbtCt chưa có hsbt_ctu prKeyHsgdCtu = " + prKeyHsgdCtu + " Error record  hsbtCt =" + JsonConvert.SerializeObject(hsbtCt));
                            await dbContextTransaction2.RollbackAsync();
                            await dbContextTransaction2.DisposeAsync();
                            await dbContextTransaction.RollbackAsync();
                            await dbContextTransaction.DisposeAsync();
                            throw;
                        }
                        _logger.Information("CreateHsbtCt prKeyHsgdCtu = " + prKeyHsgdCtu + " pr_key_bt = " + hsgd_ctu.PrKeyBt + " sau execute có hsbt_ct =" + JsonConvert.SerializeObject(hsbtCt));
                        _logger.Information("CreateHsbtCt có hsbt_uoc =" + JsonConvert.SerializeObject(hsbtUoc));
                    }
                    else
                    {
                        return "Đang thực hiện ở một tiến trình khác";
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"CreateHsbtCt chưa có hsbt_ctu prKeyHsgdCtu = " + prKeyHsgdCtu + " Exception : " + ex.ToString());
                    _logger.Error("CreateHsbtCt chưa có hsbt_ctu prKeyHsgdCtu = " + prKeyHsgdCtu + " Error record  hsbtCt =" + JsonConvert.SerializeObject(hsbtCt));
                    //_context.Database.ExecuteSqlRaw("update hsgd_ctu set pr_key_bt = 0 where pr_key = " + prKeyHsgdCtu + " and pr_key_bt = -1 ");
                    try
                    {
                        using (var ctx = new GdttContext())
                        {
                            ctx.Database.ExecuteSqlRaw("update hsgd_ctu set pr_key_bt = 0 where pr_key = " + prKeyHsgdCtu + " and pr_key_bt = -1 ");
                        }
                    }
                    catch (Exception ex2)
                    {
                        _logger.Error("Rollback failed for pr_key = " + prKeyHsgdCtu + ": " + ex2.ToString());
                    }
                }

            }
            else
            {
                _logger.Information("CreateHsbtCt đã có hsbt_ctu prKeyHsgdCtu = " + prKeyHsgdCtu + " pr_key_bt = " + hsbtCt.FrKey + " trước execute có hsbt_ct =" + JsonConvert.SerializeObject(hsbtCt));

                var context_gdtt_new = new GdttContext();
                var dbContextTransaction = await context_gdtt_new.Database.BeginTransactionAsync();

                var context_pias_new = new Pvs2024UpdateContext();
                context_pias_new.Database.SetCommandTimeout(9600);
                var dbContextTransaction2 = await context_pias_new.Database.BeginTransactionAsync();
                try
                {
                    _logger.Information("CreateHsbtCt đã có hsbt_ctu prKeyHsgdCtu = " + prKeyHsgdCtu + " fr_key = " + hsbtCt.FrKey + " có hsbt_ct =" + JsonConvert.SerializeObject(hsbtCt));

                    hsbtCt.PrKey = await context_pias_new.HsbtCts.AsQueryable().MaxAsync(x => x.PrKey) + 1;
                    _logger.Information("CreateHsbtCt đã có hsbt_ctu prKeyHsgdCtu = " + prKeyHsgdCtu + " pr_key HsbtCt " + hsbtCt.PrKey);
                    await context_pias_new.HsbtCts.AddAsync(hsbtCt);
                    await context_pias_new.SaveChangesAsync();
                    hsbtUoc.FrKey = hsbtCt.PrKey;
                    _logger.Information("CreateHsbtCt đã có hsbt_ctu prKeyHsgdCtu = " + prKeyHsgdCtu + " hsbtUoc " + JsonConvert.SerializeObject(hsbtUoc));
                    await context_pias_new.HsbtUocs.AddAsync(hsbtUoc);
                    await context_pias_new.SaveChangesAsync();

                    hsgdDxCt.PrKeyHsbtCt = hsbtCt.PrKey;
                    hsgdDxCt.PrKeyHsbtCtu = hsbtCt.FrKey;
                    hsgdDxCt.MaSp = hsbtCt.MaSp;
                    hsgdDxCt.MaDkhoan = hsbtCt.MaDkhoan;
                    await context_gdtt_new.HsgdDxCts.AddAsync(hsgdDxCt);
                    _logger.Information("CreateHsbtCt đã có hsbt_ctu prKeyHsgdCtu = " + prKeyHsgdCtu + " hsbtDxCt " + JsonConvert.SerializeObject(hsgdDxCt));
                    await context_gdtt_new.SaveChangesAsync();
                    _logger.Information("CreateHsbtCt đã có hsbt_ctu prKeyHsgdCtu = " + prKeyHsgdCtu + "Save thành công");
                    //file_attach_bt
                    if (fileAttach != null && fileAttach.Count > 0)
                    {
                        for (int j = 0; j < fileAttach.Count; j++)
                        {
                            fileAttach[j].PrKey = await context_pias_new.FileAttachBts.AsQueryable().MaxAsync(x => x.PrKey) + 1;
                            fileAttach[j].FrKey = hsbtCt.PrKey;
                            await context_pias_new.FileAttachBts.AddAsync(fileAttach[j]);
                            await context_pias_new.SaveChangesAsync();
                        }
                    }
                    await dbContextTransaction.CommitAsync();

                    await dbContextTransaction2.CommitAsync();


                    result = hsbtCt.PrKey.ToString();
                }
                catch (Exception ex)
                {
                    result = "0";
                    _logger.Error("CreateHsbtCt đã có hsbt_ctu prKeyHsgdCtu = " + prKeyHsgdCtu + " Exception : " + ex.ToString());
                    _logger.Error("CreateHsbtCt đã có hsbt_ctu prKeyHsgdCtu = " + prKeyHsgdCtu + " Error record  hsbtCt =" + JsonConvert.SerializeObject(hsbtCt));
                    await dbContextTransaction2.RollbackAsync();
                    await dbContextTransaction2.DisposeAsync();
                    await dbContextTransaction.RollbackAsync();
                    await dbContextTransaction.DisposeAsync();
                    throw;
                }
                _logger.Information("CreateHsbtCt đã có hsbt_ctu prKeyHsgdCtu = " + prKeyHsgdCtu + " pr_key_bt = " + hsbtCt.FrKey + " sau execute có hsbt_ct =" + JsonConvert.SerializeObject(hsbtCt));
                _logger.Information("CreateHsbtCt đã có hsbt_ctu có hsbt_uoc =" + JsonConvert.SerializeObject(hsbtUoc));
            }



            return result;
        }
        public async Task<string> UpdateHsbtCt(HsbtCt hsbtCt, HsgdDxCt hsgdDxCt, HsbtUoc? hsbtUoc, List<FileAttachBt> fileAttach, List<FileAttachBt> file_attach_bt_delete)
        {
            var result = "";
            await using var context_gdtt_new = new GdttContext();
            await using var dbContextTransaction = await context_gdtt_new.Database.BeginTransactionAsync();

            await using var context_pias_new = new Pvs2024UpdateContext();
            await using var dbContextTransaction2 = await context_pias_new.Database.BeginTransactionAsync();
            try
            {

                context_pias_new.HsbtCts.Update(hsbtCt);
                if (hsbtUoc != null)
                {
                    if (hsbtUoc.PrKey == 0)
                    {
                        await context_pias_new.HsbtUocs.AddAsync(hsbtUoc);
                    }
                    else
                    {
                        context_pias_new.HsbtUocs.Update(hsbtUoc);
                    }
                }
                await context_pias_new.SaveChangesAsync();

                hsgdDxCt.MaSp = hsbtCt.MaSp;
                hsgdDxCt.MaDkhoan = hsbtCt.MaDkhoan;
                _logger.Information("HsgdDxCt: " + JsonConvert.SerializeObject(hsgdDxCt));
                context_gdtt_new.HsgdDxCts.Update(hsgdDxCt);
                await context_gdtt_new.SaveChangesAsync();
                //file_attach_bt
                var list_fileattach_bt = await context_pias_new.FileAttachBts.Where(x => file_attach_bt_delete.Select(x => x.PrKey).ToArray().Contains(x.PrKey)).ToListAsync();
                context_pias_new.FileAttachBts.RemoveRange(list_fileattach_bt);
                await context_pias_new.SaveChangesAsync();
                if (fileAttach != null && fileAttach.Count > 0)
                {
                    for (int j = 0; j < fileAttach.Count; j++)
                    {
                        fileAttach[j].PrKey = await context_pias_new.FileAttachBts.AsQueryable().MaxAsync(x => x.PrKey) + 1;
                        await context_pias_new.FileAttachBts.AddAsync(fileAttach[j]);
                        await context_pias_new.SaveChangesAsync();
                    }
                }
                //context_pias_new.FileAttachBts.AddRange(fileAttach);

                await context_pias_new.SaveChangesAsync();
                await dbContextTransaction2.CommitAsync();

                await context_gdtt_new.SaveChangesAsync();
                await dbContextTransaction.CommitAsync();
                result = hsbtCt.PrKey.ToString();
            }
            catch (Exception ex)
            {
                result = "Không thành công";
                _logger.Error("UpdateHsbtCt Exception : " + ex.ToString());
                _logger.Error("UpdateHsbtCt Error record  hsbtCt =" + JsonConvert.SerializeObject(hsbtCt));
                await dbContextTransaction2.RollbackAsync();
                await dbContextTransaction2.DisposeAsync();
                await dbContextTransaction.RollbackAsync();
                await dbContextTransaction.DisposeAsync();
                throw;
            }
            return result;
        }
        public async Task<string> DeleteHsbtCt(decimal pr_key)
        {
            string result = "";
            await using var context_gdtt_new = new GdttContext();
            await using var dbContextTransaction = await context_gdtt_new.Database.BeginTransactionAsync();

            await using var context_pias_new = new Pvs2024UpdateContext();
            await using var dbContextTransaction2 = await context_pias_new.Database.BeginTransactionAsync();
            try
            {
                var hsbt_ct = context_pias_new.HsbtCts.Where(x => x.PrKey == pr_key).FirstOrDefault();
                if (hsbt_ct == null)
                {
                    return "Không tồn tại thông tin phải trả bồi thường";
                }
                // check trước khi xóa
                var dmPheCtList = (from D in context_pias_new.DmPhes
                                   join E in context_pias_new.DmPheCts on D.PrKey equals E.FrKey
                                   where D.MaPh == "CL"
                                   select new DmPheCt
                                   {
                                       MaDonvi = E.MaDonvi,
                                       KhoaSoFull = E.KhoaSoFull,
                                       DenNgay = E.DenNgay
                                   }).ToList();

                var check1 = (from A in context_pias_new.HsbtCtus
                              join B in context_pias_new.HsbtCts on A.PrKey equals B.FrKey
                              join C in context_pias_new.HsbtUocs on B.PrKey equals C.FrKey
                              where B.PrKey == pr_key
                              select new
                              {
                                  A.PrKey,
                                  A.MaDonvi,
                                  C.NgayPs
                              })
              .AsEnumerable() // Force switch to client-side after fetching data
              .Where(x => dmPheCtList.Any(t =>
                          (t.MaDonvi == x.MaDonvi || t.KhoaSoFull == true)
                          && x.NgayPs <= t.DenNgay))
              .Select(x => new { x.PrKey })
              .ToList();

                if (check1 != null && check1.Count() > 0)
                {
                    return "Dữ liệu bị khóa sổ. Không được phép xóa.Quý vị hãy liên hệ với Ban PTKD để xử lý";
                }
                if (hsbt_ct.PrKeyBttCt < 0)
                {
                    return "Dữ liệu bị khóa sổ. Không được phép xóa.Quý vị hãy liên hệ với Ban PTKD để xử lý";
                }
                if (hsbt_ct.PrKeyBttCt > 0)
                {
                    return "Dòng này đã chuyển sang kế toán. Không được phép xóa";
                }
                //

                var hsbt_uoc = context_pias_new.HsbtUocs.Where(x => x.FrKey == pr_key).ToList();
                var hsgd_dx_ct = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCt == pr_key).FirstOrDefault();
                context_pias_new.HsbtCts.Remove(hsbt_ct);
                context_pias_new.HsbtUocs.RemoveRange(hsbt_uoc);
                if (hsgd_dx_ct != null)
                {
                    context_gdtt_new.HsgdDxCts.Remove(hsgd_dx_ct);
                }
                //file_attach_bt
                var file_attach_bt = context_pias_new.FileAttachBts.Where(x => x.FrKey == pr_key).ToList();
                if (file_attach_bt != null && file_attach_bt.Count > 0)
                {
                    context_pias_new.FileAttachBts.RemoveRange(file_attach_bt);
                }

                await context_pias_new.SaveChangesAsync();
                await dbContextTransaction2.CommitAsync();

                await context_gdtt_new.SaveChangesAsync();
                await dbContextTransaction.CommitAsync();

                result = "Xoá thành công";
            }
            catch (Exception ex)
            {
                result = "Xoá thất bại";
                _logger.Error("DeleteHsbtCt Exception : " + ex.ToString());
                _logger.Error("DeleteHsbtCt Error record  pr_key =" + pr_key);
                await dbContextTransaction2.RollbackAsync();
                await dbContextTransaction2.DisposeAsync();
                await dbContextTransaction.RollbackAsync();
                await dbContextTransaction.DisposeAsync();
                throw;
            }
            return result;
        }
        public async Task<string> CreateHsbtGd(HsbtGd hsbtGd, HsbtUocGd hsbtUocGd, List<FileAttachBt> fileAttach)
        {
            var result = "";
            await using var context_pias_new = new Pvs2024UpdateContext();
            await using var dbContextTransaction2 = await context_pias_new.Database.BeginTransactionAsync();
            try
            {
                hsbtGd.PrKey = context_pias_new.HsbtGds.AsQueryable().Max(x => x.PrKey) + 1;
                context_pias_new.HsbtGds.Add(hsbtGd);

                hsbtUocGd.PrKey = context_pias_new.HsbtUocGds.AsQueryable().Max(x => x.PrKey) + 1;
                hsbtUocGd.FrKey = hsbtGd.PrKey;
                context_pias_new.HsbtUocGds.Add(hsbtUocGd);
                //file_attach_bt
                if (fileAttach != null && fileAttach.Count > 0)
                {
                    for (int j = 0; j < fileAttach.Count; j++)
                    {
                        fileAttach[j].PrKey = context_pias_new.FileAttachBts.AsQueryable().Max(x => x.PrKey) + 1;
                        fileAttach[j].FrKey = hsbtGd.PrKey;
                        context_pias_new.FileAttachBts.Add(fileAttach[j]);
                        await context_pias_new.SaveChangesAsync();
                    }
                }
                await context_pias_new.SaveChangesAsync();
                await dbContextTransaction2.CommitAsync();
                result = hsbtGd.PrKey.ToString();
            }
            catch (Exception ex)
            {
                result = "0";
                _logger.Error("CreateHsbtGd Exception : " + ex.ToString());
                _logger.Error("CreateHsbtGd Error record  hsbtGd =" + JsonConvert.SerializeObject(hsbtGd));
                await dbContextTransaction2.RollbackAsync();
                await dbContextTransaction2.DisposeAsync();
                throw;
            }
            return result;
        }
        public async Task<string> UpdateHsbtGd(HsbtGd hsbtGd, HsbtUocGd? hsbtUocGd, List<FileAttachBt> fileAttach, List<FileAttachBt> file_attach_bt_delete)
        {
            var result = "";
            await using var context_pias_new = new Pvs2024UpdateContext();
            await using var dbContextTransaction2 = await context_pias_new.Database.BeginTransactionAsync();
            try
            {
                context_pias_new.HsbtGds.Update(hsbtGd);
                if (hsbtUocGd != null)
                {
                    if (hsbtUocGd.PrKey == 0)
                    {
                        hsbtUocGd.PrKey = context_pias_new.HsbtUocGds.AsQueryable().Max(x => x.PrKey) + 1;
                        context_pias_new.HsbtUocGds.Add(hsbtUocGd);
                    }
                    else
                    {
                        context_pias_new.HsbtUocGds.Update(hsbtUocGd);
                    }
                }
                //file_attach_bt
                context_pias_new.FileAttachBts.Where(x => file_attach_bt_delete.Select(x => x.PrKey).ToArray().Contains(x.PrKey)).ExecuteDelete();
                if (fileAttach != null && fileAttach.Count > 0)
                {
                    for (int j = 0; j < fileAttach.Count; j++)
                    {
                        fileAttach[j].PrKey = context_pias_new.FileAttachBts.AsQueryable().Max(x => x.PrKey) + 1;
                        context_pias_new.FileAttachBts.Add(fileAttach[j]);
                        await context_pias_new.SaveChangesAsync();
                    }
                }
                await context_pias_new.SaveChangesAsync();
                await dbContextTransaction2.CommitAsync();
                result = hsbtGd.PrKey.ToString();
            }
            catch (Exception ex)
            {
                result = "Không thành công";
                _logger.Error("UpdateHsbtGd Exception : " + ex.ToString());
                _logger.Error("UpdateHsbtGd Error record  hsbtGd =" + JsonConvert.SerializeObject(hsbtGd));
                await dbContextTransaction2.RollbackAsync();
                await dbContextTransaction2.DisposeAsync();
                throw;
            }
            return result;
        }
        public async Task<string> DeleteHsbtGd(decimal pr_key)
        {
            string result = "";

            await using var context_pias_new = new Pvs2024UpdateContext();
            await using var dbContextTransaction2 = await context_pias_new.Database.BeginTransactionAsync();
            try
            {
                var hsbt_gd = context_pias_new.HsbtGds.Where(x => x.PrKey == pr_key).FirstOrDefault();
                if (hsbt_gd == null)
                {
                    return "Không tồn tại thông tin giám định";
                }
                // check trước khi xóa

                if (hsbt_gd.PrKeyBttCt < 0)
                {
                    return "Dữ liệu bị khóa sổ. Không được phép xóa.Quý vị hãy liên hệ với Ban PTKD để xử lý";
                }
                if (hsbt_gd.PrKeyBttCt > 0)
                {
                    return "Dòng này đã chuyển sang kế toán. Không được phép xóa";
                }
                //
                var hsbt_uoc_gd = context_pias_new.HsbtUocGds.Where(x => x.FrKey == pr_key).ToList();
                context_pias_new.HsbtGds.Remove(hsbt_gd);
                context_pias_new.HsbtUocGds.RemoveRange(hsbt_uoc_gd);
                //file_attach_bt
                var file_attach_bt = context_pias_new.FileAttachBts.Where(x => x.FrKey == pr_key).ToList();
                if (file_attach_bt != null && file_attach_bt.Count > 0)
                {
                    context_pias_new.FileAttachBts.RemoveRange(file_attach_bt);
                }
                await context_pias_new.SaveChangesAsync();
                await dbContextTransaction2.CommitAsync();

                result = "Xoá thành công";
            }
            catch (Exception ex)
            {
                result = "Xoá thất bại";
                _logger.Error("DeleteHsbtGd Exception : " + ex.ToString());
                _logger.Error("DeleteHsbtGd Error record  pr_key =" + pr_key);
                await dbContextTransaction2.RollbackAsync();
                await dbContextTransaction2.DisposeAsync();
                throw;
            }
            return result;
        }
        public async Task<string> CreateHsbtThts(HsbtTht hsbtThts)
        {
            var result = "";
            await using var context_pias_new = new Pvs2024UpdateContext();
            try
            {
                hsbtThts.PrKey = context_pias_new.HsbtThts.AsQueryable().Max(x => x.PrKey) + 1;
                context_pias_new.HsbtThts.Add(hsbtThts);
                await context_pias_new.SaveChangesAsync();
                result = hsbtThts.PrKey.ToString();
            }
            catch (Exception ex)
            {
                result = "0";
                _logger.Error("CreateHsbtThts Exception : " + ex.ToString());
                _logger.Error("CreateHsbtThts Error record  hsbtThts =" + JsonConvert.SerializeObject(hsbtThts));
                throw;
            }
            return result;
        }
        public async Task<string> UpdateHsbtThts(HsbtTht hsbtThts)
        {
            var result = "";
            try
            {
                _context_pias_update.HsbtThts.Update(hsbtThts);
                await _context_pias_update.SaveChangesAsync();
                result = hsbtThts.PrKey.ToString();
            }
            catch (Exception ex)
            {
                result = "Không thành công";
                _logger.Error("UpdateHsbtThts Exception : " + ex.ToString());
                _logger.Error("UpdateHsbtThts Error record  hsbtThts =" + JsonConvert.SerializeObject(hsbtThts));
                throw;
            }
            return result;
        }
        public string CreatePASC(HsgdDxCt hsgdDxCt, List<HsgdDx> hsgdDx, List<HsgdDxTsk> hsgdDxTsk)
        {
            var result = "";

            using var context_gdtt_new = new GdttContext();
            using var dbContextTransaction = context_gdtt_new.Database.BeginTransaction();

            using var context_pias_new = new Pvs2024UpdateContext();
            using var dbContextTransaction2 = context_pias_new.Database.BeginTransaction();
            try
            {
                context_gdtt_new.HsgdDxCts.Update(hsgdDxCt);

                //cập nhật lại một số thông tin bảng hsbt_ctu mà lấy từ bảng hsgd_dx_Ct
                var hsbt_ctu = (from a in context_pias_new.HsbtCtus
                                join b in context_pias_new.HsbtCts on a.PrKey equals b.FrKey
                                where b.PrKey == hsgdDxCt.PrKeyHsbtCt
                                select a).FirstOrDefault();

                if (hsgdDx != null)
                {
                    var hsgd_dx = context_gdtt_new.HsgdDxes.Where(x => x.PrKeyDx == hsgdDxCt.PrKey).ToList();
                    context_gdtt_new.HsgdDxes.RemoveRange(hsgd_dx);
                    context_gdtt_new.HsgdDxes.AddRange(hsgdDx);
                    if (hsbt_ctu != null)
                    {
                        hsbt_ctu.MaGaraVcx = hsgdDxCt.MaGara;
                        //hsbt_ctu.MaGaraVcx2 = hsgdDxCt.MaGara01;
                        //hsbt_ctu.MaGaraVcx3 = hsgdDxCt.MaGara02;
                        hsbt_ctu.NamsxVcx = hsgdDxCt.NamSx;
                        //var loai_xe = context_gdtt_new.DmHieuxes.Where(x => x.PrKey == hsgdDxCt.HieuXe).Select(s => s.HieuXe).FirstOrDefault() ?? "" + " " + context_gdtt_new.DmLoaixes.Where(x => x.PrKey == hsgdDxCt.LoaiXe).Select(s => s.LoaiXe).FirstOrDefault() ?? "";
                        //var str_loai_xe = "";
                        //if (loai_xe.Length > 50)
                        //{
                        //    str_loai_xe = loai_xe.Substring(0, 50);
                        //}
                        //else
                        //{
                        //    str_loai_xe = loai_xe;
                        //}
                        //hsbt_ctu.LoaiXe = str_loai_xe;
                        context_pias_new.HsbtCtus.Update(hsbt_ctu);
                    }


                }
                if (hsgdDxTsk != null)
                {
                    var hsgd_dx_tsk = context_gdtt_new.HsgdDxTsks.Where(x => x.PrKeyDx == hsgdDxCt.PrKey).ToList();
                    context_gdtt_new.HsgdDxTsks.RemoveRange(hsgd_dx_tsk);
                    context_gdtt_new.HsgdDxTsks.AddRange(hsgdDxTsk);

                    if (hsbt_ctu != null)
                    {
                        hsbt_ctu.MaGaraTnds = hsgdDxCt.MaGara;
                        //hsbt_ctu.MaGaraTnds2 = hsgdDxCt.MaGara01;
                        //hsbt_ctu.MaGaraTnds3 = hsgdDxCt.MaGara02;
                        hsbt_ctu.NamsxTnds = hsgdDxCt.NamSx;
                        //hsbt_ctu.LoaiXeTnds = hsgdDxCt.LoaiXe.ToString();
                        context_pias_new.HsbtCtus.Update(hsbt_ctu);
                    }
                }
                context_pias_new.SaveChanges();
                dbContextTransaction2.Commit();

                context_gdtt_new.SaveChanges();
                dbContextTransaction.Commit();
                result = hsgdDxCt.PrKey.ToString();
            }
            catch (Exception ex)
            {
                result = "0";
                _logger.Error("CreatePASC Exception : " + ex.ToString());
                _logger.Error("CreatePASC Error record  hsbtCt =" + JsonConvert.SerializeObject(hsgdDxCt));
                dbContextTransaction2.Rollback();
                dbContextTransaction2.Dispose();
                dbContextTransaction.Rollback();
                dbContextTransaction.Dispose();
                return result;
            }
            // cập nhật ước bt
            if (hsgdDx != null && hsgdDx.Count > 0)
            {
                UpdateUocBT(hsgdDx[0].FrKey, hsgdDxCt.PrKey);
            }
            if (hsgdDxTsk != null && hsgdDxTsk.Count > 0)
            {
                UpdateUocBT(hsgdDxTsk[0].FrKey, hsgdDxCt.PrKey);
            }
            return result;
        }
        public DonBH? GetTTDonBH(decimal pr_hsgd_ctu, string ma_sp)
        {
            try
            {
                _context_pias.Database.SetCommandTimeout(300);
                var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == pr_hsgd_ctu).FirstOrDefault();
                if (hsgd_ctu != null)
                {
                    try
                    {
                        _logger.Information("GetTTDonBH 1 pr_key_hsbt=" + pr_hsgd_ctu.ToString());
                        var ctu_tyle = (from A in _context_pias.NvuBhtCtus
                                        join B in _context_pias.NvuBhtSeris on A.PrKey equals B.FrKey
                                        join C in _context_pias.NvuBhtSeriCts on B.PrKey equals C.FrKey
                                        where A.SoDonbh == hsgd_ctu.SoDonbh && A.SoDonbhBs == "" && B.SoSeri == hsgd_ctu.SoSeri && C.MaSp == ma_sp
                                        select new DonBH
                                        {
                                            PrKey = A.PrKey,
                                            MaDkbh = C.MaDkbh,
                                            MaTtep = C.MaTtep,
                                            TygiaHt = C.TygiaHt,
                                            MtnGtbhNte = C.MtnGtbhNte,
                                            MtnGtbhVnd = C.MtnGtbhVnd,
                                            NguyenTep = C.NguyenTep,
                                            SoTienp = C.SoTienp,
                                            MucVat = C.MucVat,
                                            NguyenTev = C.NguyenTev,
                                            TienVat = C.TienVat,
                                            GiatriTte = C.GiatriTte,
                                            MtnGtbhTsan = C.MtnGtbhTsan,
                                            TyleDong = 100 - ((A.TyleDong != 0 ? 100 - A.TyleDong : 0) + _context_pias.NvuBhtDbhs
                                            .Where(dbh => dbh.FrKey == A.PrKey)
                                            .Sum(dbh => (decimal?)dbh.TyleTg) ?? 0 * (A.TyleDong != 0 ? A.TyleDong : 100) / 100),
                                            TyleReten = (
                                from reten in _context_pias.ReDmRetens
                                where reten.SoDonbh == hsgd_ctu.SoDonbh && reten.SoDonbhbs == ""
                                select (int?)reten.TyleReten
                            ).FirstOrDefault() ?? 100
                                        }).FirstOrDefault();
                        return ctu_tyle ?? null;

                    }
                    catch (Exception ex)
                    {
                        _logger.Error("GetTTDonBH : cho pr_key_hsgd 1= " + pr_hsgd_ctu + " lỗi: " + ex.ToString());
                        return null;
                    }

                }
            }            
            catch (Exception ex)
            {
                _logger.Error("GetTTDonBH : cho pr_key_hsgd 2= " + pr_hsgd_ctu + " lỗi: " + ex.ToString());
                return null;
            }
            return null;
        }
        public class DonBH
        {
            public decimal PrKey { get; set; }
            public string? MaDkbh { get; set; }
            public string? MaTtep { get; set; }
            public decimal TygiaHt { get; set; }
            public decimal MtnGtbhNte { get; set; }
            public decimal MtnGtbhVnd { get; set; }
            public decimal NguyenTep { get; set; }
            public decimal SoTienp { get; set; }
            public decimal MucVat { get; set; }
            public decimal NguyenTev { get; set; }
            public decimal TienVat { get; set; }
            public decimal GiatriTte { get; set; }
            public decimal MtnGtbhTsan { get; set; }
            public decimal TyleDong { get; set; }
            public decimal TyleReten { get; set; }
        }
        public void UpdateUocBT_save(decimal pr_key_hsgd_ctu, decimal pr_key_hsgd_dx_ct)
        {
            using var contextnew = new Pvs2024UpdateContext();
            using var dbContextTransaction = contextnew.Database.BeginTransaction();
            try
            {
                var pr_key_hsbt_ct = _context.HsgdDxCts.Where(x => x.PrKey == pr_key_hsgd_dx_ct).Select(s => s.PrKeyHsbtCt).FirstOrDefault();
                var pr_key_hsbt_ctu = contextnew.HsbtCts.Where(x => x.PrKey == pr_key_hsbt_ct).Select(s => s.FrKey).FirstOrDefault();
                var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKeyBt == pr_key_hsbt_ctu).FirstOrDefault();
                if (hsgd_ctu != null)
                {
                    var obj_sum = ReloadSum(pr_key_hsgd_dx_ct);
                    decimal sotien_pvi_tt = 0;
                    var key_find = hsgd_ctu.PrKey;
                    if (obj_sum != null && obj_sum.Count > 0)
                    {
                        sotien_pvi_tt = obj_sum[0].StBl ?? 0;
                    }
                    if (hsgd_ctu.MaTtrangGd != "6" && hsgd_ctu.MaTtrangGd != "7" && sotien_pvi_tt > 0)
                    {
                        if (hsgd_ctu.MaLhsbt == "2")
                        {
                            var hsgd_ctu_ho = _context.HsgdCtus.Where(x => x.PrKeyBtHo == hsgd_ctu.PrKey.ToString() && x.MaLhsbt == "3").FirstOrDefault();
                            if (hsgd_ctu_ho != null)
                            {
                                key_find = hsgd_ctu_ho.PrKey;
                            }
                        }
                        var hsgd_ctu_find = _context.HsgdCtus.Where(x => x.PrKey == key_find).FirstOrDefault();
                        if (hsgd_ctu_find != null)
                        {
                            var pr_key_hsbt_ct_find = contextnew.HsbtCts.Where(x => x.FrKey == hsgd_ctu_find.PrKeyBt && x.MaSp == "050104").Select(s => s.PrKey).FirstOrDefault();
                            var pr_key_hsgd_dx_ct_find = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCt == pr_key_hsbt_ct_find).Select(s => s.PrKey).FirstOrDefault();
                            var obj_sum_find = ReloadSum(pr_key_hsgd_dx_ct_find);
                            if (obj_sum_find != null && obj_sum_find.Count > 0 && obj_sum_find[0].StBl != 0)
                            {
                                var hsbt_ct_key = (from A in contextnew.HsbtCts
                                                   join B in contextnew.HsbtUocs on A.PrKey equals B.FrKey
                                                   where A.FrKey == hsgd_ctu.PrKeyBt && A.MaSp == "050104" && A.MaTtrangBt == "01"
                                                   select new
                                                   {
                                                       PrKey = A.PrKey,
                                                       PrKeyUoc = B.PrKey
                                                   }).OrderBy(o => o.PrKey).FirstOrDefault();
                                if (hsbt_ct_key != null)
                                {
                                    var pr_key_ct = hsbt_ct_key.PrKey;
                                    var pr_key_uoc = hsbt_ct_key.PrKeyUoc;
                                    var muc_vatu = 8;
                                    var dtyle_reten = _context_pias.ReDmRetens.Where(x => x.MaSp == "050104" && x.SoDonbh == hsgd_ctu_find.SoDonbh && x.SoDonbhbs == "").Select(s => s.TyleReten).FirstOrDefault() ?? 0;
                                    var dtyle_reten1 = dtyle_reten;
                                    if (dtyle_reten == 0)
                                    {
                                        dtyle_reten1 = 100;
                                    }
                                    var pvibltt = Math.Round((sotien_pvi_tt * 100) / (100 + muc_vatu));
                                    var pvibltt_ho = pvibltt;
                                    var hsbt_ct = contextnew.HsbtCts.Where(x => x.PrKey == pr_key_ct).FirstOrDefault();
                                    _logger.Information("UpdateUocBT trước update hsbt_ct = " + JsonConvert.SerializeObject(hsbt_ct));
                                    //if (hsbt_ct != null)
                                    //{
                                    if (pvibltt > hsbt_ct.MtnGtbh)
                                    {
                                        pvibltt = hsbt_ct.MtnGtbh;
                                    }
                                    hsbt_ct.NguyenTepu = pvibltt;
                                    hsbt_ct.SoTienpu = pvibltt;
                                    hsbt_ct.MucVatu = muc_vatu;
                                    hsbt_ct.NguyenTevu = Convert.ToInt32(Math.Floor(pvibltt * muc_vatu / 100));
                                    hsbt_ct.SoTienvu = Convert.ToInt32(Math.Floor(pvibltt * muc_vatu / 100));
                                    hsbt_ct.NguyenTep = pvibltt;
                                    hsbt_ct.SoTienp = pvibltt;
                                    hsbt_ct.MucVatp = muc_vatu;
                                    hsbt_ct.NguyenTevp = Convert.ToInt32(Math.Floor(pvibltt * muc_vatu / 100));
                                    hsbt_ct.SoTienvp = Convert.ToInt32(Math.Floor(pvibltt * muc_vatu / 100));
                                    hsbt_ct.TyleReten = dtyle_reten1;
                                    hsbt_ct.MtnRetenNte = Math.Round(dtyle_reten1 * pvibltt / 100, 0);
                                    hsbt_ct.MtnRetenVnd = Math.Round(dtyle_reten1 * pvibltt / 100, 0);
                                    contextnew.HsbtCts.Update(hsbt_ct);

                                    _logger.Information("UpdateUocBT update hsbt_ct = " + JsonConvert.SerializeObject(hsbt_ct));
                                    //}
                                    var hsbt_uoc_delete = contextnew.HsbtUocs.Where(x => x.FrKey == pr_key_ct && x.NgayPs == DateTime.Today).ToList();
                                    contextnew.HsbtUocs.RemoveRange(hsbt_uoc_delete);

                                    var ds_Sum_uoc = contextnew.HsbtUocs
                                        .Where(x => x.FrKey == pr_key_ct && x.NgayPs < DateTime.Today)
                                        .GroupBy(g => g.FrKey)
                                        .Select(s => new
                                        {
                                            fr_key = s.Key,
                                            sum_nguyen_tebt = s.Sum(x => x.NguyenTebt),
                                            sum_so_tienbt = s.Sum(x => x.SoTienbt),
                                            sum_nguyen_tebt_reten = s.Sum(x => x.NguyenTebtReten),
                                            sum_so_tienbt_reten = s.Sum(x => x.SoTienbtReten)
                                        }).FirstOrDefault();
                                    HsbtUoc hsbtUoc = new HsbtUoc();
                                    if (ds_Sum_uoc != null)
                                    {
                                        hsbtUoc.FrKey = pr_key_ct;
                                        hsbtUoc.NgayPs = DateTime.Today;
                                        hsbtUoc.NguyenTebt = pvibltt - ds_Sum_uoc.sum_nguyen_tebt;
                                        hsbtUoc.SoTienbt = pvibltt - ds_Sum_uoc.sum_so_tienbt;
                                        hsbtUoc.TyleReten = dtyle_reten1;
                                        hsbtUoc.NguyenTebtReten = Math.Round(dtyle_reten1 * pvibltt / 100, 0) - ds_Sum_uoc.sum_nguyen_tebt_reten;
                                        hsbtUoc.SoTienbtReten = Math.Round(dtyle_reten1 * pvibltt / 100, 0) - ds_Sum_uoc.sum_so_tienbt_reten;
                                    }
                                    else
                                    {
                                        hsbtUoc.FrKey = pr_key_ct;
                                        hsbtUoc.NgayPs = DateTime.Today;
                                        hsbtUoc.NguyenTebt = pvibltt;
                                        hsbtUoc.SoTienbt = pvibltt;
                                        hsbtUoc.TyleReten = dtyle_reten1;
                                        hsbtUoc.NguyenTebtReten = Math.Round(dtyle_reten1 * pvibltt / 100, 0);
                                        hsbtUoc.SoTienbtReten = Math.Round(dtyle_reten1 * pvibltt / 100, 0);
                                    }
                                    contextnew.HsbtUocs.Add(hsbtUoc);
                                    _logger.Information("UpdateUocBT add hsbt_uoc = " + JsonConvert.SerializeObject(hsbtUoc));
                                    ///////// update hsbt hộ
                                    if (hsgd_ctu.MaLhsbt == "3" || hsgd_ctu.MaLhsbt == "2")
                                    {
                                        Decimal pr_key_bt = 0;
                                        if (hsgd_ctu.MaLhsbt == "2")
                                        {
                                            pr_key_bt = _context.HsgdCtus.Where(x => x.PrKey == key_find).Select(s => s.PrKeyBt).FirstOrDefault();
                                        }
                                        else
                                        {
                                            pr_key_bt = _context.HsgdCtus.Where(x => x.PrKey == Convert.ToInt32(hsgd_ctu.PrKeyBtHo)).Select(s => s.PrKeyBt).FirstOrDefault();
                                        }
                                        var hsbt_ct_key_ho = (from A in contextnew.HsbtCts
                                                              join B in contextnew.HsbtUocs on A.PrKey equals B.FrKey
                                                              where A.FrKey == pr_key_bt && A.MaSp == "050104" && A.MaTtrangBt == "01"
                                                              select new
                                                              {
                                                                  PrKey = A.PrKey,
                                                                  PrKeyUoc = B.PrKey
                                                              }).OrderBy(o => o.PrKey).FirstOrDefault();
                                        if (hsbt_ct_key_ho != null)
                                        {
                                            var pr_key_ct_ho = hsbt_ct_key_ho.PrKey;
                                            var pr_key_uoc_ho = hsbt_ct_key_ho.PrKeyUoc;
                                            var hsbt_ct_ho = contextnew.HsbtCts.Where(x => x.PrKey == pr_key_ct_ho).FirstOrDefault();
                                            _logger.Information("UpdateUocBT trước update hsbt_ct_ho = " + JsonConvert.SerializeObject(hsbt_ct_ho));
                                            //if (hsbt_ct_ho != null)
                                            //{
                                            if (pvibltt_ho > hsbt_ct_ho.MtnGtbh)
                                            {
                                                pvibltt_ho = hsbt_ct_ho.MtnGtbh;
                                            }
                                            hsbt_ct_ho.NguyenTepu = pvibltt_ho;
                                            hsbt_ct_ho.SoTienpu = pvibltt_ho;
                                            hsbt_ct_ho.MucVatu = muc_vatu;
                                            hsbt_ct_ho.NguyenTevu = Convert.ToInt32(Math.Floor(pvibltt_ho * muc_vatu / 100));
                                            hsbt_ct_ho.SoTienvu = Convert.ToInt32(Math.Floor(pvibltt_ho * muc_vatu / 100));
                                            hsbt_ct_ho.NguyenTep = pvibltt_ho;
                                            hsbt_ct_ho.SoTienp = pvibltt_ho;
                                            hsbt_ct_ho.MucVatp = muc_vatu;
                                            hsbt_ct_ho.NguyenTevp = Convert.ToInt32(Math.Floor(pvibltt_ho * muc_vatu / 100));
                                            hsbt_ct_ho.SoTienvp = Convert.ToInt32(Math.Floor(pvibltt_ho * muc_vatu / 100));
                                            hsbt_ct_ho.TyleReten = dtyle_reten1;
                                            hsbt_ct_ho.MtnRetenNte = Math.Round(dtyle_reten1 * pvibltt_ho / 100, 0);
                                            hsbt_ct_ho.MtnRetenVnd = Math.Round(dtyle_reten1 * pvibltt_ho / 100, 0);
                                            contextnew.HsbtCts.Update(hsbt_ct_ho);
                                            //}

                                            _logger.Information("UpdateUocBT update hsbt_ct_ho = " + JsonConvert.SerializeObject(hsbt_ct_ho));
                                            var hsbt_uoc_delete_ho = contextnew.HsbtUocs.Where(x => x.FrKey == pr_key_ct_ho && x.NgayPs == DateTime.Today).ToList();
                                            contextnew.HsbtUocs.RemoveRange(hsbt_uoc_delete_ho);

                                            var ds_Sum_uoc_ho = contextnew.HsbtUocs
                                                .Where(x => x.FrKey == pr_key_ct_ho && x.NgayPs < DateTime.Today)
                                                .GroupBy(g => g.FrKey)
                                                .Select(s => new
                                                {
                                                    fr_key = s.Key,
                                                    sum_nguyen_tebt = s.Sum(x => x.NguyenTebt),
                                                    sum_so_tienbt = s.Sum(x => x.SoTienbt),
                                                    sum_nguyen_tebt_reten = s.Sum(x => x.NguyenTebtReten),
                                                    sum_so_tienbt_reten = s.Sum(x => x.SoTienbtReten)
                                                }).FirstOrDefault();
                                            HsbtUoc hsbtUoc_ho = new HsbtUoc();
                                            if (ds_Sum_uoc_ho != null)
                                            {
                                                hsbtUoc_ho.FrKey = pr_key_ct_ho;
                                                hsbtUoc_ho.NgayPs = DateTime.Today;
                                                hsbtUoc_ho.NguyenTebt = pvibltt_ho - ds_Sum_uoc_ho.sum_nguyen_tebt;
                                                hsbtUoc_ho.SoTienbt = pvibltt_ho - ds_Sum_uoc_ho.sum_so_tienbt;
                                                hsbtUoc_ho.TyleReten = dtyle_reten1;
                                                hsbtUoc_ho.NguyenTebtReten = Math.Round(dtyle_reten1 * pvibltt_ho / 100, 0) - ds_Sum_uoc_ho.sum_nguyen_tebt_reten;
                                                hsbtUoc_ho.SoTienbtReten = Math.Round(dtyle_reten1 * pvibltt_ho / 100, 0) - ds_Sum_uoc_ho.sum_so_tienbt_reten;
                                            }
                                            else
                                            {
                                                hsbtUoc_ho.FrKey = pr_key_ct;
                                                hsbtUoc_ho.NgayPs = DateTime.Today;
                                                hsbtUoc_ho.NguyenTebt = pvibltt_ho;
                                                hsbtUoc_ho.SoTienbt = pvibltt_ho;
                                                hsbtUoc_ho.TyleReten = dtyle_reten1;
                                                hsbtUoc_ho.NguyenTebtReten = Math.Round(dtyle_reten1 * pvibltt_ho / 100, 0);
                                                hsbtUoc_ho.SoTienbtReten = Math.Round(dtyle_reten1 * pvibltt_ho / 100, 0);
                                            }
                                            contextnew.HsbtUocs.Add(hsbtUoc_ho);
                                            _logger.Information("UpdateUocBT add hsbt_uoc_ho = " + JsonConvert.SerializeObject(hsbtUoc_ho));
                                        }

                                    }
                                }
                            }
                        }

                        //}
                        // }
                    }
                }
                contextnew.SaveChanges();
                dbContextTransaction.Commit();
            }

            catch (Exception ex)
            {
                _logger.Error("dbContextTransaction Exception when UpdateUocBT: " + ex.ToString());
                _logger.Error("UpdateUocBT Error record pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + ", pr_key_hsgd_dx_ct = " + pr_key_hsgd_dx_ct);
                dbContextTransaction.Rollback();
                dbContextTransaction.Dispose();
            }

        }
        public void UpdateUocBT(decimal pr_key_hsgd_ctu, decimal pr_key_hsgd_dx_ct)
        {
            using var contextnew = new Pvs2024UpdateContext();
            using var dbContextTransaction = contextnew.Database.BeginTransaction();
            try
            {
                var pr_key_hsbt_ct = _context.HsgdDxCts.Where(x => x.PrKey == pr_key_hsgd_dx_ct).Select(s => s.PrKeyHsbtCt).FirstOrDefault();
                var pr_key_hsbt_ctu = contextnew.HsbtCts.Where(x => x.PrKey == pr_key_hsbt_ct).Select(s => s.FrKey).FirstOrDefault();
                var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKeyBt == pr_key_hsbt_ctu).FirstOrDefault();
                if (hsgd_ctu != null)
                {
                    var obj_sum = ReloadSum(pr_key_hsgd_dx_ct);
                    decimal sotien_pvi_tt = 0;
                    //var key_find = hsgd_ctu.PrKey;
                    if (obj_sum != null && obj_sum.Count > 0)
                    {
                        sotien_pvi_tt = obj_sum[0].StBl ?? 0;
                    }
                    if (hsgd_ctu.MaTtrangGd != "6" && hsgd_ctu.MaTtrangGd != "7" && sotien_pvi_tt > 0)
                    {
                        // var pr_key_ct = hsbt_ct_key.PrKey;
                        // var pr_key_uoc = hsbt_ct_key.PrKeyUoc;
                        var hsbt_ct = contextnew.HsbtCts.Where(x => x.PrKey == pr_key_hsbt_ct).FirstOrDefault();
                        if (hsbt_ct != null)
                        {
                            var muc_vatu = 8;
                            var dtyle_reten = _context_pias.ReDmRetens.Where(x => x.MaSp == hsbt_ct.MaSp && x.SoDonbh == hsgd_ctu.SoDonbh && x.SoDonbhbs == "").Select(s => s.TyleReten).FirstOrDefault() ?? 0;
                            var dtyle_reten1 = dtyle_reten;
                            if (dtyle_reten == 0)
                            {
                                dtyle_reten1 = 100;
                            }
                            var pvibltt = Math.Round((sotien_pvi_tt * 100) / (100 + muc_vatu), 0);
                            _logger.Information("UpdateUocBT trước update hsbt_ct = " + JsonConvert.SerializeObject(hsbt_ct));
                            if (pvibltt > hsbt_ct.MtnGtbh)
                            {
                                pvibltt = hsbt_ct.MtnGtbh;
                            }
                            hsbt_ct.NguyenTepu = pvibltt;
                            hsbt_ct.SoTienpu = pvibltt;
                            hsbt_ct.MucVatu = muc_vatu;
                            hsbt_ct.NguyenTevu = Math.Round((sotien_pvi_tt - pvibltt), 0);
                            hsbt_ct.SoTienvu = Math.Round((sotien_pvi_tt - pvibltt), 0);
                            hsbt_ct.NguyenTep = pvibltt;
                            hsbt_ct.SoTienp = pvibltt;
                            hsbt_ct.MucVatp = muc_vatu;
                            hsbt_ct.NguyenTevp = Math.Round((sotien_pvi_tt-pvibltt), 0);
                            hsbt_ct.SoTienvp = Math.Round((sotien_pvi_tt - pvibltt), 0);
                            hsbt_ct.TyleReten = dtyle_reten1;
                            hsbt_ct.MtnRetenNte = Math.Round(dtyle_reten1 * pvibltt / 100, 0);
                            hsbt_ct.MtnRetenVnd = Math.Round(dtyle_reten1 * pvibltt / 100, 0);
                            contextnew.HsbtCts.Update(hsbt_ct);

                            _logger.Information("UpdateUocBT update hsbt_ct = " + JsonConvert.SerializeObject(hsbt_ct));
                            var hsbt_uoc_delete = contextnew.HsbtUocs.Where(x => x.FrKey == pr_key_hsbt_ct && x.NgayPs == DateTime.Today).ToList();
                            contextnew.HsbtUocs.RemoveRange(hsbt_uoc_delete);

                            var ds_Sum_uoc = contextnew.HsbtUocs
                                .Where(x => x.FrKey == pr_key_hsbt_ct && x.NgayPs < DateTime.Today)
                                .GroupBy(g => g.FrKey)
                                .Select(s => new
                                {
                                    fr_key = s.Key,
                                    sum_nguyen_tebt = s.Sum(x => x.NguyenTebt),
                                    sum_so_tienbt = s.Sum(x => x.SoTienbt),
                                    sum_nguyen_tebt_reten = s.Sum(x => x.NguyenTebtReten),
                                    sum_so_tienbt_reten = s.Sum(x => x.SoTienbtReten)
                                }).FirstOrDefault();
                            HsbtUoc hsbtUoc = new HsbtUoc();
                            if (ds_Sum_uoc != null)
                            {
                                hsbtUoc.FrKey = pr_key_hsbt_ct;
                                hsbtUoc.NgayPs = DateTime.Today;
                                hsbtUoc.NguyenTebt = pvibltt - ds_Sum_uoc.sum_nguyen_tebt;
                                hsbtUoc.SoTienbt = pvibltt - ds_Sum_uoc.sum_so_tienbt;
                                hsbtUoc.TyleReten = dtyle_reten1;
                                hsbtUoc.NguyenTebtReten = Math.Round(dtyle_reten1 * pvibltt / 100, 0) - ds_Sum_uoc.sum_nguyen_tebt_reten;
                                hsbtUoc.SoTienbtReten = Math.Round(dtyle_reten1 * pvibltt / 100, 0) - ds_Sum_uoc.sum_so_tienbt_reten;
                            }
                            else
                            {
                                hsbtUoc.FrKey = pr_key_hsbt_ct;
                                hsbtUoc.NgayPs = DateTime.Today;
                                hsbtUoc.NguyenTebt = pvibltt;
                                hsbtUoc.SoTienbt = pvibltt;
                                hsbtUoc.TyleReten = dtyle_reten1;
                                hsbtUoc.NguyenTebtReten = Math.Round(dtyle_reten1 * pvibltt / 100, 0);
                                hsbtUoc.SoTienbtReten = Math.Round(dtyle_reten1 * pvibltt / 100, 0);
                            }
                            contextnew.HsbtUocs.Add(hsbtUoc);
                            _logger.Information("UpdateUocBT add hsbt_uoc = " + JsonConvert.SerializeObject(hsbtUoc));
                        }


                        //}
                        // }
                    }
                }
                contextnew.SaveChanges();
                dbContextTransaction.Commit();
            }

            catch (Exception ex)
            {
                _logger.Error("dbContextTransaction Exception when UpdateUocBT: " + ex.ToString());
                _logger.Error("UpdateUocBT Error record pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + ", pr_key_hsgd_dx_ct = " + pr_key_hsgd_dx_ct);
                dbContextTransaction.Rollback();
                dbContextTransaction.Dispose();
            }

        }
        public async Task<string> ImportPASC(List<HsgdDx> entity)
        {
            var result = "";
            try
            {
                _context.HsgdDxes.AddRange(entity);
                await _context.SaveChangesAsync();
                result = "Thành công";
            }
            catch (Exception ex)
            {
                result = "Không thành công";
                _logger.Error("ImportPASC Exception : " + ex.ToString());
                _logger.Error("ImportPASC Error record  entity =" + JsonConvert.SerializeObject(entity));
                throw;
            }
            return result;
        }
        public CombinedPASCResult PrintPASC_save(decimal pr_key_hsbt_ct, decimal pr_key_hsgd_ctu, string email, int loai_dx)
        {
            try
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
                var wordPdfRequest = new WordToPdfRequest();
                wordPdfRequest.ListData = new List<EntityContent>();
                var list_pasc_detail = new List<pasc_detail>();
                UpdateProperties update = new UpdateProperties();
                var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == pr_key_hsgd_ctu).Select(s => new
                {
                    PrKey = s.PrKey,
                    MaDonvi = s.MaDonvi,
                    MaDonvigd = s.MaDonvigd,
                    SoHsgd = s.SoHsgd,
                    TenKhach = s.TenKhach,
                    BienKsoat = s.BienKsoat,
                    SoSeri = s.SoSeri,
                    HsgdTpc = s.HsgdTpc,
                    MaTtrangGd = s.MaTtrangGd,
                    sNgayDau = s.NgayDauSeri != null ? Convert.ToDateTime(s.NgayDauSeri).ToString("dd/MM/yyyy") : "",
                    sNgayCuoi = s.NgayCuoiSeri != null ? Convert.ToDateTime(s.NgayCuoiSeri).ToString("dd/MM/yyyy") : "",
                    MaUser = s.MaUser,
                    sNgayTbao = s.NgayTbao != null ? Convert.ToDateTime(s.NgayTbao).ToString("dd/MM/yyyy") : "",
                    sNgayTthat = s.NgayTthat != null ? Convert.ToDateTime(s.NgayTthat).ToString("dd/MM/yyyy") : "",
                    NguyenNhanTtat = s.NguyenNhanTtat,
                    TitleNgayduyet = s.HsgdTpc == 1 ? "Hồ sơ đã được TCT phê duyệt:" : "Hồ sơ được phê duyệt ngày:",
                    sNgayDuyet = s.HsgdTpc == 1 ? "" : (s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ToString("dd/MM/yyyy") : ""),
                    DienThoai = s.DienThoai,
                    NgayDauSeri = s.NgayDauSeri,
                    NgayTthat = s.NgayTthat,
                    PrKeyBt = s.PrKeyBt,
                    SoDonbh = s.SoDonbh,
                    NguoiXuly = s.NguoiXuly
                }).FirstOrDefault();
                if (hsgd_ctu != null)
                {
                    var ten_donvi = _context.DmDonvis.Where(x => x.MaDonvi == hsgd_ctu.MaDonvi).Select(s => s.TenDonvi).FirstOrDefault();
                    var ma_donvi_user = _context.DmUsers.Where(x => x.Mail == email).Select(s => s.MaDonvi).FirstOrDefault();
                    //if (hsgd_ctu.HsgdTpc == 1 && new[] { "00", "31", "32" }.Contains(ma_donvi_user))
                    //{
                    update.AddEntityContent(wordPdfRequest, "[LBL_VP]", "VĂN PHÒNG ĐẠI DIỆN CSKH");
                    update.AddEntityContent(wordPdfRequest, "[LBL_DEXUAT]", "ĐỀ XUẤT PHƯƠNG ÁN SỬA CHỮA");
                    update.AddEntityContent(wordPdfRequest, "[LABEL_TRACHNHIEMPVI]", "Số tiền thuộc trách nhiệm bảo hiểm (Gồm VAT)");
                    update.AddEntityContent(wordPdfRequest, "[LBL_NG_KY]", "Lãnh đạo VPCSKH");
                    update.AddEntityContent(wordPdfRequest, "[LBL_PHONGGQ]", "Phòng GQKN XCG");
                    update.AddEntityContent(wordPdfRequest, "[LBL_GDV]", "CB GQKN XCG");
                    //}
                    //else
                    //{
                    //    if (!string.IsNullOrEmpty(ten_donvi))
                    //    {
                    //        update.AddEntityContent(wordPdfRequest, "[LBL_VP]", "BẢO HIỂM " + ten_donvi.ToUpper());
                    //    }
                    //    else
                    //    {
                    //        update.AddEntityContent(wordPdfRequest, "[LBL_VP]", "");
                    //    }
                    //    update.AddEntityContent(wordPdfRequest, "[LBL_DEXUAT]", "ĐỀ XUẤT PHƯƠNG ÁN SỬA CHỮA");
                    //    update.AddEntityContent(wordPdfRequest, "[LABEL_TRACHNHIEMPVI]", "Tổng chi phí còn lại (Gồm VAT)");
                    //    update.AddEntityContent(wordPdfRequest, "[LBL_NG_KY]", "Lãnh đạo đơn vị");
                    //    update.AddEntityContent(wordPdfRequest, "[LBL_PHONGGQ]", "Phòng GĐBT/GQKN");
                    //    update.AddEntityContent(wordPdfRequest, "[LBL_GDV]", "Giám định viên");
                    //}
                    update.AddEntityContent(wordPdfRequest, "[SO_HSGD]", hsgd_ctu.SoHsgd);
                    update.AddEntityContent(wordPdfRequest, "[TEN_KHACH]", hsgd_ctu.TenKhach);
                    update.AddEntityContent(wordPdfRequest, "[DIEN_THOAI]", hsgd_ctu.DienThoai);
                    update.AddEntityContent(wordPdfRequest, "[BIEN_KSOAT]", hsgd_ctu.BienKsoat);
                    update.AddEntityContent(wordPdfRequest, "[NGAY_DAU]", hsgd_ctu.sNgayDau);
                    update.AddEntityContent(wordPdfRequest, "[NGAY_CUOI]", hsgd_ctu.sNgayCuoi);
                    update.AddEntityContent(wordPdfRequest, "[SO_SERI]", hsgd_ctu.SoSeri.ToString());
                    update.AddEntityContent(wordPdfRequest, "[NGAY_TTHAT]", hsgd_ctu.sNgayTthat);
                    update.AddEntityContent(wordPdfRequest, "[NGAY_TBAO]", hsgd_ctu.sNgayTbao);
                    List<string> list_NguyenNhanTtat = ContentHelper.SplitString(hsgd_ctu.NguyenNhanTtat.ToString(), 255);
                    for (int i = 0; i < list_NguyenNhanTtat.Count(); i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[NGUYEN_NHANTT{i}]", list_NguyenNhanTtat[i]);
                    }
                    for (int i = list_NguyenNhanTtat.Count(); i < 2; i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[NGUYEN_NHANTT{i}]", "");
                    }

                    var hsgd_dx_ct = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCt == pr_key_hsbt_ct).Select(s => new
                    {
                        PrKey = s.PrKey,
                        HieuXe = s.HieuXe,
                        LoaiXe = s.LoaiXe,
                        XuatXu = s.XuatXu,
                        NamSx = s.NamSx,
                        MaGara = s.MaGara,
                        MaGara01 = s.MaGara01,
                        MaGara02 = s.MaGara02,
                        SoTienctkh = s.SoTienctkh,
                        SoTienGtbt = s.SoTienGtbt,
                        Vat = s.Vat,
                        TyleggPhutungvcx = s.TyleggPhutungvcx,
                        TyleggSuachuavcx = s.TyleggSuachuavcx,
                        LydoCtkh = s.LydoCtkh,
                        DoituongttTnds = s.DoituongttTnds
                    }).FirstOrDefault();
                    if (hsgd_dx_ct != null)
                    {

                        var hieu_xe = _context.DmHieuxes.Where(x => x.PrKey == hsgd_dx_ct.HieuXe).Select(s => s.HieuXe).FirstOrDefault();
                        update.AddEntityContent(wordPdfRequest, "[HIEU_XE]", hieu_xe != null ? hieu_xe : "");
                        var loai_xe = _context.DmLoaixes.Where(x => x.PrKey == hsgd_dx_ct.LoaiXe).Select(s => s.LoaiXe).FirstOrDefault();
                        update.AddEntityContent(wordPdfRequest, "[LOAI_XE]", loai_xe != null ? loai_xe : "");
                        update.AddEntityContent(wordPdfRequest, "[XUAT_XU]", hsgd_dx_ct.XuatXu ?? "");
                        update.AddEntityContent(wordPdfRequest, "[NAM_SX]", hsgd_dx_ct.NamSx.ToString());
                        var ten_gara = _context.DmGaRas.Where(x => x.MaGara == hsgd_dx_ct.MaGara).Select(s => s.TenGara + (s.DiaChi != "" ? " - " + s.DiaChi : "")).FirstOrDefault();
                        update.AddEntityContent(wordPdfRequest, "[TEN_GARA]", ten_gara != null ? ten_gara : "");
                        update.AddEntityContent(wordPdfRequest, "[TEN_GARA01]", hsgd_dx_ct.MaGara01 ?? "");
                        update.AddEntityContent(wordPdfRequest, "[TEN_GARA02]", hsgd_dx_ct.MaGara02 ?? "");
                        List<string> list_LydoCtkh = ContentHelper.SplitString(hsgd_dx_ct.LydoCtkh.ToString(), 255);
                        for (int i = 0; i < list_LydoCtkh.Count(); i++)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[LYDO_CTKH{i}]", list_LydoCtkh[i]);
                        }
                        for (int i = list_LydoCtkh.Count(); i < 11; i++)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[LYDO_CTKH{i}]", "");
                        }
                        List<string> list_DoituongttTnds = ContentHelper.SplitString(hsgd_dx_ct.DoituongttTnds.ToString(), 255);
                        for (int i = 0; i < list_DoituongttTnds.Count(); i++)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[DOITUONGTT_TNDS{i}]", list_DoituongttTnds[i]);
                        }
                        for (int i = list_DoituongttTnds.Count(); i < 11; i++)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[DOITUONGTT_TNDS{i}]", "");
                        }
                        if (loai_dx == 0 || loai_dx == 1)
                        {
                            list_pasc_detail = ToListWithNoLock((from a in _context.HsgdDxes
                                                                 join b in _context.DmHmucs on a.MaHmuc equals b.MaHmuc into b1
                                                                 from b in b1.DefaultIfEmpty()
                                                                 where a.PrKeyDx == hsgd_dx_ct.PrKey
                                                                 select new pasc_detail
                                                                 {
                                                                     pr_key_dx = a.PrKey,
                                                                     ma_hmuc = a.MaHmuc,
                                                                     ten_hmuc = b != null ? (b.TenHmuc ?? "") : a.Hmuc,
                                                                     so_tientt = a.SoTientt,
                                                                     so_tienph = a.SoTienph,
                                                                     so_tienson = a.SoTienson,
                                                                     vat_sc = a.VatSc,
                                                                     giam_tru_bt = a.GiamTruBt,
                                                                     thu_hoi_ts = a.ThuHoiTs,
                                                                     vat_so_tientt = a.SoTientt * ((decimal)a.VatSc / 100),
                                                                     vat_so_tienph = a.SoTienph * ((decimal)a.VatSc / 100),
                                                                     vatso_tienson = a.SoTienson * ((decimal)a.VatSc / 100),
                                                                     so_tientt_gomVAT = a.SoTientt + a.SoTientt * ((decimal)a.VatSc / 100),
                                                                     so_tienph_gomVAT = a.SoTienph + a.SoTienph * ((decimal)a.VatSc / 100),
                                                                     so_tienson_gomVAT = a.SoTienson + a.SoTienson * ((decimal)a.VatSc / 100),
                                                                     ghi_chudv = a.GhiChudv,
                                                                     so_tien_vat = (a.SoTientt + a.SoTienph + a.SoTienson) * ((decimal)a.VatSc / 100),
                                                                     sum_tt_ph_son_gomVAT = ((a.SoTientt + a.SoTienph + a.SoTienson) + (a.SoTientt + a.SoTienph + a.SoTienson) * ((decimal)a.VatSc / 100)),//sum_tt_ph_son_gomVAT
                                                                     sum_giamtru_bt = ((((a.SoTientt + a.SoTientt * ((decimal)a.VatSc / 100)) - ((a.SoTientt + a.SoTientt * ((decimal)a.VatSc / 100)) * (hsgd_dx_ct.TyleggPhutungvcx / 100)))
                       + (((a.SoTienph + a.SoTienson) + (a.SoTienph + a.SoTienson) * ((decimal)a.VatSc / 100)) - ((a.SoTienph + a.SoTienson) + (a.SoTienph + a.SoTienson) * ((decimal)a.VatSc / 100)) * (hsgd_dx_ct.TyleggSuachuavcx / 100))) * a.GiamTruBt / 100),//sum_giamtru_bt
                                                                     sum_so_tienggsc = ((a.SoTientt + a.SoTientt * ((decimal)a.VatSc / 100)) * (hsgd_dx_ct.TyleggPhutungvcx / 100) + ((a.SoTienph + a.SoTienson) + (a.SoTienph + a.SoTienson) * ((decimal)a.VatSc / 100)) * (hsgd_dx_ct.TyleggSuachuavcx / 100)),//sum_so_tienggsc
                                                                                                                                                                                                                                                                                                                 //
                                                                 }).OrderBy(o => o.pr_key_dx).AsQueryable());
                        }
                        else
                        {
                            list_pasc_detail = ToListWithNoLock((from a in _context.HsgdDxTsks
                                                                 where a.PrKeyDx == hsgd_dx_ct.PrKey
                                                                 select new pasc_detail
                                                                 {
                                                                     pr_key_dx = a.PrKey,
                                                                     ten_hmuc = a.Hmuc,
                                                                     so_tientt = a.SoTientt,
                                                                     so_tiensc = a.SoTiensc,
                                                                     vat_sc = a.VatSc,
                                                                     giam_tru_bt = a.GiamTruBt,
                                                                     thu_hoi_ts = a.ThuHoiTs,
                                                                     vat_so_tientt = a.SoTientt * ((decimal)a.VatSc / 100),
                                                                     vat_so_tiensc = a.SoTiensc * ((decimal)a.VatSc / 100),
                                                                     so_tientt_gomVAT = a.SoTientt + a.SoTientt * ((decimal)a.VatSc / 100),
                                                                     so_tiensc_gomVAT = a.SoTiensc + a.SoTiensc * ((decimal)a.VatSc / 100),
                                                                     ghi_chudv = a.GhiChudv,
                                                                     so_tien_vat = (a.SoTientt + a.SoTiensc) * ((decimal)a.VatSc / 100),
                                                                     sum_tt_sc_gomVAT = ((a.SoTientt + a.SoTiensc) + (a.SoTientt + a.SoTiensc) * ((decimal)a.VatSc / 100)),//sum_tt_sc_gomVAT
                                                                     sum_giamtru_bt = (((a.SoTientt + a.SoTientt * ((decimal)a.VatSc / 100)) + (a.SoTiensc + a.SoTiensc * ((decimal)a.VatSc / 100))) * a.GiamTruBt / 100),//sum_giamtru_bt

                                                                 }).OrderBy(o => o.pr_key_dx).AsQueryable());
                        }
                        if (list_pasc_detail.Count > 0 && list_pasc_detail.Where(x => x.vat_sc > 0).Count() > 0)
                        {
                            if (hsgd_ctu.HsgdTpc == 1 && new[] { "00", "31", "32" }.Contains(ma_donvi_user))
                            {
                                update.AddEntityContent(wordPdfRequest, "[LABEL_TRACHNHIEMPVI]", "Số tiền thuộc trách nhiệm bảo hiểm (Gồm VAT)");
                            }
                            else
                            {
                                update.AddEntityContent(wordPdfRequest, "[LABEL_TRACHNHIEMPVI]", "Tổng chi phí còn lại (Gồm VAT)");
                            }
                            update.AddEntityContent(wordPdfRequest, "[LABEL_TONGCHIPHI]", "Tổng chi phí thay thế + sửa chữa (Gồm VAT)");
                        }
                        else
                        {
                            if (hsgd_ctu.HsgdTpc == 1 && new[] { "00", "31", "32" }.Contains(ma_donvi_user))
                            {
                                update.AddEntityContent(wordPdfRequest, "[LABEL_TRACHNHIEMPVI]", "Số tiền thuộc trách nhiệm bảo hiểm");
                            }
                            else
                            {
                                update.AddEntityContent(wordPdfRequest, "[LABEL_TRACHNHIEMPVI]", "Tổng chi phí còn lại");
                            }
                            update.AddEntityContent(wordPdfRequest, "[LABEL_TONGCHIPHI]", "Tổng chi phí thay thế + sửa chữa");
                        }
                        if (list_pasc_detail.Count > 0)
                        {
                            if (loai_dx == 0 || loai_dx == 1)
                            {
                                var sum_hsgd_dx = list_pasc_detail.GroupBy(g => 1 == 1)
                                .Select(s => new sum_hsgd_dx
                                {
                                    sumso_tien_tt_ph_son_gomVAT = s.Sum(x => x.sum_tt_ph_son_gomVAT),
                                    sumso_tien_giamtru_bt = s.Sum(x => x.sum_giamtru_bt),
                                    sumso_tien_so_tienggsc = s.Sum(x => x.sum_so_tienggsc)
                                }).FirstOrDefault();
                                if (sum_hsgd_dx != null)
                                {
                                    if (sum_hsgd_dx.sumso_tien_giamtru_bt == 0)
                                    {
                                        sum_hsgd_dx.sumso_tien_giamtru_bt = hsgd_dx_ct.SoTienGtbt;
                                    }
                                    sum_hsgd_dx.sum_trachnhienpvi = sum_hsgd_dx.sumso_tien_tt_ph_son_gomVAT - sum_hsgd_dx.sumso_tien_so_tienggsc - sum_hsgd_dx.sumso_tien_giamtru_bt - hsgd_dx_ct.SoTienctkh;

                                    update.AddEntityContent(wordPdfRequest, "[SUMSO_TIEN_TT_PH_SON_GOMVAT]", sum_hsgd_dx.sumso_tien_tt_ph_son_gomVAT.ToString("#,###", cul.NumberFormat));
                                    update.AddEntityContent(wordPdfRequest, "[SUMSO_TIENGGSC]", sum_hsgd_dx.sumso_tien_so_tienggsc.ToString("#,###", cul.NumberFormat));
                                    update.AddEntityContent(wordPdfRequest, "[SO_TIENCTKH]", hsgd_dx_ct.SoTienctkh.ToString("#,###", cul.NumberFormat));
                                    update.AddEntityContent(wordPdfRequest, "[SO_TIENGIAMTRUBT]", sum_hsgd_dx.sumso_tien_giamtru_bt.ToString("#,###", cul.NumberFormat));
                                    update.AddEntityContent(wordPdfRequest, "[SUM_TRACHNHIEMPVI]", sum_hsgd_dx.sum_trachnhienpvi.ToString("#,###", cul.NumberFormat));
                                    update.AddEntityContent(wordPdfRequest, "[SUM_TRACHNHIEMPVI_BC]", ContentHelper.NumberToText((double)sum_hsgd_dx.sum_trachnhienpvi));
                                }
                            }
                            else
                            {
                                var sum_hsgd_dx_tsk = list_pasc_detail.GroupBy(g => 1 == 1)
                                .Select(s => new sum_hsgd_dx_tsk
                                {
                                    sumso_tien_tt_sc_gomVAT = s.Sum(x => x.sum_tt_sc_gomVAT),
                                    sumso_tien_giamtru_bt = s.Sum(x => x.sum_giamtru_bt)
                                }).FirstOrDefault();
                                if (sum_hsgd_dx_tsk != null)
                                {
                                    if (sum_hsgd_dx_tsk.sumso_tien_giamtru_bt == 0)
                                    {
                                        sum_hsgd_dx_tsk.sumso_tien_giamtru_bt = hsgd_dx_ct.SoTienGtbt;
                                    }
                                    sum_hsgd_dx_tsk.sum_trachnhienpvi = sum_hsgd_dx_tsk.sumso_tien_tt_sc_gomVAT - sum_hsgd_dx_tsk.sumso_tien_giamtru_bt - hsgd_dx_ct.SoTienctkh;

                                    update.AddEntityContent(wordPdfRequest, "[SUMSO_TIEN_TT_PH_SON_GOMVAT]", sum_hsgd_dx_tsk.sumso_tien_tt_sc_gomVAT.ToString("#,###", cul.NumberFormat));
                                    update.AddEntityContent(wordPdfRequest, "[SO_TIENCTKH]", hsgd_dx_ct.SoTienctkh.ToString("#,###", cul.NumberFormat));
                                    update.AddEntityContent(wordPdfRequest, "[SO_TIENGIAMTRUBT]", sum_hsgd_dx_tsk.sumso_tien_giamtru_bt.ToString("#,###", cul.NumberFormat));
                                    update.AddEntityContent(wordPdfRequest, "[SUM_TRACHNHIEMPVI]", sum_hsgd_dx_tsk.sum_trachnhienpvi.ToString("#,###", cul.NumberFormat));
                                    update.AddEntityContent(wordPdfRequest, "[SUM_TRACHNHIEMPVI_BC]", ContentHelper.NumberToText((double)sum_hsgd_dx_tsk.sum_trachnhienpvi));
                                }
                            }
                        }
                        else
                        {
                            update.AddEntityContent(wordPdfRequest, "[SUMSO_TIEN_TT_PH_SON_GOMVAT]", "0");
                            update.AddEntityContent(wordPdfRequest, "[SUMSO_TIENGGSC]", "0");
                            update.AddEntityContent(wordPdfRequest, "[SO_TIENCTKH]", hsgd_dx_ct.SoTienctkh.ToString("#,###", cul.NumberFormat));
                            update.AddEntityContent(wordPdfRequest, "[SO_TIENGIAMTRUBT]", "0");
                            update.AddEntityContent(wordPdfRequest, "[SUM_TRACHNHIEMPVI]", "0");
                            update.AddEntityContent(wordPdfRequest, "[SUM_TRACHNHIEMPVI_BC]", ContentHelper.NumberToText(0));
                        }
                    }
                    #region lấy dữ liệu lịch sử tổn thất
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    var tu_ngay = DateTime.ParseExact(Convert.ToDateTime(hsgd_ctu.NgayDauSeri).ToString("dd/MM/yyyy 00:00:00"), "dd/MM/yyyy HH:mm:ss", provider);
                    var den_ngay = DateTime.ParseExact(Convert.ToDateTime(hsgd_ctu.NgayTthat).ToString("dd/MM/yyyy 00:00:00"), "dd/MM/yyyy HH:mm:ss", provider);
                    var uoc1 = (from a in _context_pias_update.HsbtCtus
                                join b in _context_pias_update.HsbtCts on a.PrKey equals b.FrKey
                                join c in _context_pias_update.HsbtUocs on b.PrKey equals c.FrKey
                                where b.MaSp.StartsWith("05") && new[] { "01", "02", "05" }.Contains(b.MaTtrangBt) && new[] { "TBT", "NBT" }.Contains(a.MaLhsbt) && c.NgayPs <= den_ngay && a.PrKey != hsgd_ctu.PrKeyBt && a.SoHdgcn == hsgd_ctu.SoDonbh && a.SoSeri == hsgd_ctu.SoSeri
                                select new
                                {
                                    SoHsbt = a.SoHsbt,
                                    SoTienp = b.SoTienp
                                }).AsQueryable();
                    var uoc1_gr = ToListWithNoLock(uoc1.GroupBy(n => new { n.SoHsbt }).Select(p => new LichSuBT
                    {
                        LoaiHs = "UOC",
                        SoHsbt = p.Key.SoHsbt,
                        SoTienp = p.Sum(x => x.SoTienp)
                    }).AsQueryable());
                    var uoc2 = (from a in _context_pias_update.HsbtCtus
                                join b in _context_pias_update.HsbtCts on a.PrKey equals b.FrKey
                                join c in _context_pias_update.HsbtUocs on b.PrKey equals c.FrKey
                                where b.MaSp.StartsWith("05") && new[] { "03", "04" }.Contains(b.MaTtrangBt) && new[] { "TBT", "NBT" }.Contains(a.MaLhsbt) && c.NgayPs <= den_ngay && b.NgayHtoanBt > den_ngay && a.PrKey != hsgd_ctu.PrKeyBt && a.SoHdgcn == hsgd_ctu.SoDonbh && a.SoSeri == hsgd_ctu.SoSeri
                                select new
                                {
                                    SoHsbt = a.SoHsbt,
                                    SoTienp = c.SoTienbt
                                }).AsQueryable();
                    var uoc2_gr = ToListWithNoLock(uoc2.GroupBy(n => new { n.SoHsbt }).Select(p => new LichSuBT
                    {
                        LoaiHs = "UOC",
                        SoHsbt = p.Key.SoHsbt,
                        SoTienp = p.Sum(x => x.SoTienp)
                    }).AsQueryable());
                    var uoc3 = ToListWithNoLock((from a in _context_pias_update.HsbtCtus
                                                 join b in _context_pias_update.HsbtCts on a.PrKey equals b.FrKey
                                                 where b.MaSp.StartsWith("05") && b.MaTtrangBt == "03" && new[] { "TBT", "NBT" }.Contains(a.MaLhsbt) && b.NgayHtoanBt >= tu_ngay && b.NgayHtoanBt <= den_ngay && a.PrKey != hsgd_ctu.PrKeyBt && a.SoHdgcn == hsgd_ctu.SoDonbh && a.SoSeri == hsgd_ctu.SoSeri
                                                 select new LichSuBT
                                                 {
                                                     LoaiHs = "PT",
                                                     SoHsbt = a.SoHsbt,
                                                     SoTienp = b.SoTienp
                                                 }).AsQueryable());
                    var lsbt = uoc1_gr.Union(uoc2_gr).Union(uoc3).ToList();
                    var lsbt_gr = lsbt.GroupBy(g => new { g.LoaiHs, g.SoHsbt }).Select(p => new LichSuBT
                    {
                        LoaiHs = p.Key.LoaiHs,
                        SoHsbt = p.Key.SoHsbt,
                        SoTienp = p.Sum(x => x.SoTienp)
                    }).ToList();
                    var nguyen_te_ubt = lsbt_gr.Where(x => x.LoaiHs == "UOC").Select(t => t.SoTienp).Sum();
                    var so_lan_ubt = lsbt_gr.Where(x => x.LoaiHs == "UOC").Count();
                    var nguyen_te_bt = lsbt_gr.Where(x => x.LoaiHs == "PT").Select(t => t.SoTienp).Sum();
                    var so_lan_bt = lsbt_gr.Where(x => x.LoaiHs == "PT").Count();
                    update.AddEntityContent(wordPdfRequest, "[SV_CBT]", so_lan_ubt.ToString());
                    update.AddEntityContent(wordPdfRequest, "[SOTIEN_UBT]", nguyen_te_ubt.ToString("#,###", cul.NumberFormat));
                    update.AddEntityContent(wordPdfRequest, "[SV_BT]", so_lan_bt.ToString());
                    update.AddEntityContent(wordPdfRequest, "[SOTIEN_BT]", nguyen_te_bt.ToString("#,###", cul.NumberFormat));
                    #endregion
                    #region user duyệt, ký
                    var pr_key_nky_duyet = _context.NhatKies.Where(x => x.MaTtrangGd == "6" && x.FrKey == hsgd_ctu.PrKey).GroupBy(c => 1 == 1)
                    .Select(p => p.Max(g => g.PrKey)).FirstOrDefault();
                    var oid_user_duyet = _context.NhatKies.Where(x => x.PrKey == pr_key_nky_duyet).Select(s => s.MaUser).FirstOrDefault();
                    var user_duyet = _context.DmUsers.Where(x => x.Oid == oid_user_duyet).FirstOrDefault();
                    if (user_duyet != null)
                    {
                        update.AddEntityContent(wordPdfRequest, "[MAUSER_DUYET]", user_duyet.MaUser);
                        update.AddEntityContent(wordPdfRequest, "[TENUSER_DUYET]", user_duyet.TenUser);
                    }
                    else
                    {
                        update.AddEntityContent(wordPdfRequest, "[MAUSER_DUYET]", "");
                        update.AddEntityContent(wordPdfRequest, "[TENUSER_DUYET]", "");
                    }
                    var pr_key_nky_cchopd = _context.NhatKies.Where(x => x.MaTtrangGd == "10" && x.FrKey == hsgd_ctu.PrKey).GroupBy(c => 1 == 1)
                    .Select(p => p.Max(g => g.PrKey)).FirstOrDefault();
                    var oid_user_cchopd = _context.NhatKies.Where(x => x.PrKey == pr_key_nky_cchopd).Select(s => s.MaUser).FirstOrDefault();
                    var user_cchopd = _context.DmUsers.Where(x => x.Oid == oid_user_duyet).FirstOrDefault();
                    if (user_cchopd != null)
                    {
                        update.AddEntityContent(wordPdfRequest, "[MAUSER_CCHOPD]", user_cchopd.MaUser);
                        update.AddEntityContent(wordPdfRequest, "[TENUSER_CCHOPD]", user_cchopd.TenUser);
                    }
                    else
                    {
                        update.AddEntityContent(wordPdfRequest, "[MAUSER_CCHOPD]", "");
                        update.AddEntityContent(wordPdfRequest, "[TENUSER_CCHOPD]", "");
                    }
                    var user_gdv = _context.DmUsers.Where(x => x.Oid == (hsgd_ctu.NguoiXuly == "" ? hsgd_ctu.MaUser : Guid.Parse(hsgd_ctu.NguoiXuly))).FirstOrDefault();
                    if (user_gdv != null)
                    {
                        update.AddEntityContent(wordPdfRequest, "[MAUSER_GDV]", user_gdv.MaUser);
                        update.AddEntityContent(wordPdfRequest, "[TENUSER_GDV]", user_gdv.TenUser);
                    }
                    else
                    {
                        update.AddEntityContent(wordPdfRequest, "[MAUSER_GDV]", "");
                        update.AddEntityContent(wordPdfRequest, "[TENUSER_GDV]", "");
                    }
                    #endregion

                }




                var listData = wordPdfRequest.ListData;
                _logger.Information("PrintToTrinh " + JsonConvert.SerializeObject(listData));
                var listNew = new CombinedPASCResult
                {
                    ThirdQueryResults = listData,
                    ListPascDetail = list_pasc_detail
                };

                return listNew;

            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                return null;
            }
        }
        public string GuiPASC(decimal pr_key_hsbt_ct, decimal pr_key_hsgd_ctu, bool chk_send_pasc, bool pasc_send_sms, string email_nhan, string phone_nhan, string email_login, string file_path)
        {
            string result = "";
            try
            {
                var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == pr_key_hsgd_ctu).FirstOrDefault();
                if (hsgd_ctu != null)
                {
                    _logger.Information("bắt đầu GuiPASC pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + ", pr_key_hsbt_ct = " + pr_key_hsbt_ct);

                    var user_login = _context.DmUsers.Where(x => x.Mail == email_login).FirstOrDefault();
                    //// soap pias
                    //var ws = new ServiceReference1.PiasSoapSoapClient(ServiceReference1.PiasSoapSoapClient.EndpointConfiguration.PiasSoapSoap, _configuration["client:endpoint:address"]);

                    //string strSQL = "select top 1 * from hddt_hsm where ma_donvi = '" + user_login.MaDonvi + "' and ngay_hluc < getdate() order by ngay_hluc desc ";
                    //var esign = ws.SelectSQL_HDDT(DateTime.Now.Year.ToString(), strSQL, "hddt_hsm");
                    //var ds_esign = ConvetXMLToDataset(esign);
                    //if (ds_esign.Tables[0].Rows.Count > 0)
                    //{
                    //    var partitionAlias = ds_esign.Tables[0].Rows[0].Field<string>("partition_alias");
                    //    var privateKeyAlias = ds_esign.Tables[0].Rows[0].Field<string>("private_key_alias");
                    //    var password = ds_esign.Tables[0].Rows[0].Field<string>("password");
                    //    var partitionSerial = ds_esign.Tables[0].Rows[0].Field<string>("partition_serial");
                    //    if (ws.KyPASCXCG(file_path, privateKeyAlias, "mediafile3"))
                    //    {
                    string ghichu_gui = "";
                    if (chk_send_pasc)
                    {
                        var result_GuiPASC = GuiEmailPASC(file_path, pr_key_hsbt_ct, pr_key_hsgd_ctu, email_nhan, user_login.MaDonvi);
                        if (result_GuiPASC)
                        {
                            ghichu_gui = "Email: " + email_nhan.Replace(" ", "");
                        }
                    }

                    if (pasc_send_sms)
                    {
                        var result_pasc_send_sms = send_sms_pasc(hsgd_ctu.MaUser, phone_nhan, hsgd_ctu.BienKsoat, hsgd_ctu.SoSeri.ToString(), hsgd_ctu.SoHsgd, hsgd_ctu.DienThoai, hsgd_ctu.PrKey);
                        if (result_pasc_send_sms)
                        {
                            ghichu_gui = ghichu_gui + " SĐT: " + phone_nhan.Replace(" ", "");
                        }

                    }

                    //call update trạng thái
                    if (!string.IsNullOrEmpty(ghichu_gui))
                    {

                        var nhat_ky = new NhatKy();
                        nhat_ky.FrKey = hsgd_ctu.PrKey;
                        nhat_ky.MaTtrangGd = "PADT";
                        nhat_ky.TenTtrangGd = Map_tinh_trang("PADT");
                        nhat_ky.GhiChu = "Gửi PASC Điện tử " + ghichu_gui;
                        nhat_ky.NgayCapnhat = DateTime.Now;
                        nhat_ky.MaUser = user_login.Oid;
                        UpdateNhatKyPADT(pr_key_hsbt_ct, nhat_ky);
                        result = "Gửi PASC thành công";
                        _logger.Information("GuiPASC pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + ", pr_key_hsbt_ct = " + pr_key_hsbt_ct + " success");
                    }
                    else
                    {
                        result = "Gửi PASC thất bại";
                        _logger.Information("GuiPASC pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + ", pr_key_hsbt_ct = " + pr_key_hsbt_ct + " error");
                    }
                    //    }
                    //    else
                    //    {
                    //        result = "Ký số không thành công. Hãy thử lại sau.";
                    //    }
                    //}
                    //else
                    //{
                    //    result = "Tài khoản không có quyền hoặc hết hiệu lực ký số.";
                    //}
                }


            }
            catch (Exception ex)
            {
                _logger.Error($"GuiPASC pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + ", pr_key_hsbt_ct = " + pr_key_hsbt_ct + " An error occurred: " + ex);
                result = "Có lỗi xảy ra. Hãy thử lại sau.";
            }
            return result;
        }
        public bool GuiEmailPASC(string file_path, decimal pr_key_hsbt_ct, decimal pr_key_hsgd_ctu, string email_nhan, string ma_donvi)
        {
            bool result = true;
            try
            {
                string url_download = _configuration["DownloadSettings:DownloadServer"] ?? "";
                string strFileNameLocalPdf = UtilityHelper.getPathAndCopyTempServer(file_path, url_download);
                //test tạm bỏ
                if (ma_donvi == "31")
                {
                    SendEmail_PVI247("vppb.xcg2.baolanh@gmail.com", pr_key_hsbt_ct, pr_key_hsgd_ctu, strFileNameLocalPdf, ".PASC");
                }
                //gửi theo danh sách email
                var arremail = email_nhan.Replace(" ", "").Split(";");
                if (arremail.Length < 6)
                {
                    foreach (var email in arremail)
                    {
                        SendEmail_PVI247(email, pr_key_hsbt_ct, pr_key_hsgd_ctu, strFileNameLocalPdf, ".PASC");
                    }

                }
                // kiểm tra và xóa file ở local 
                if (System.IO.File.Exists(strFileNameLocalPdf))
                {
                    System.IO.File.Delete(strFileNameLocalPdf);
                }
                _logger.Information("GuiEmailPASC pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " success");

            }
            catch (Exception ex)
            {
                result = false;
                _logger.Error($"GuiEmailPASC pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " error: " + ex);
            }
            return result;
        }
        public bool send_sms_pasc(Guid? ma_user, string phone_nhan, string bks, string seri, string so_hsgd, string phone_call, decimal pr_key_hsgd_ctu)
        {
            var result = true;
            try
            {
                //test tạm bỏ
                if (ma_user != null)
                {
                    var gdv = _context.DmUsers.Where(x => x.MaUser == ma_user.ToString()).FirstOrDefault();
                    //_hsgdDxService.GetDmUserByMa(ma_user);
                    if (gdv != null)
                    {
                        sendsms(gdv.Dienthoai ?? "", bks, seri, so_hsgd, phone_call, "PASC", pr_key_hsgd_ctu);
                    }
                }
                var arrphone = phone_nhan.Replace(" ", "").Split(";");
                if (arrphone.Length < 3)
                {
                    foreach (var phone in arrphone)
                    {
                        sendsms(phone, bks, seri, so_hsgd, phone_call, "PASC", pr_key_hsgd_ctu);
                    }
                }
                _logger.Information("send_sms_pasc pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " success");
            }
            catch (Exception ex)
            {
                result = false;
                _logger.Information("send_sms_pasc pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " error " + ex);
            }

            return result;
        }
        public bool sendsms(string phone, string bks, string seri, string so_hsgd, string phone_call, string send_type, decimal pr_key_hsgd_ctu)
        {
            bool result = true;

            string message = "";
            try
            {
                if (!Regex.IsMatch(phone, "^[0-9]+$") || phone.Length < 9 || phone.Length > 12)
                {
                    _logger.Information("sendsms so_hsgd = " + so_hsgd + " số điện thoại " + phone + " không đúng");
                    return false;
                }
                if (send_type == "0")
                    message = "BaohiemPVI thong bao ban duoc yeu cau GD xe " + bks + " theo ho so " + so_hsgd + " tren he thong Claim Online. De nghi lien he SDT " + phone_call + " de thuc hien GD";
                else if (send_type == "1")
                    message = "BaohiemPVI thong bao ban duoc yeu cau bo sung thong tin ho so " + so_hsgd + " xe " + bks + " tren he thong Claim Online. ";
                else if (send_type == "2")
                    message = "BaohiemPVI thong bao ho so " + so_hsgd + " xe " + bks + " da duoc phe duyet tren he thong Claim Online. ";
                else if (send_type == "3")
                    message = "BaohiemPVI thong bao ban duoc yeu cau giai quyet HS BKS " + bks + " ,so HS GDTT " + so_hsgd + " tu PVIMobile de hoan thien HS";
                else if (send_type == "BL")
                    message = "BaohiemPVI thong bao xe " + bks + " ton that ngay " + so_hsgd + " da duoc Bao hiem PVI bao lanh sua chua tai Gara. ";
                else if (send_type == "PASC")
                    message = "BaohiemPVI thong bao ho so " + so_hsgd + " xe " + bks + " da duoc len phuong an sua chua va gui ve don vi cap don. ";
                else if (send_type == "KBTT_INSTAL_APP")
                    message = "Ho so " + bks + " cua quy khach da duoc tao vui long vao ung dung PVI Mobile, chuc nang khai bao ton that chon ho so da duoc tao de giam dinh, dien thoai lien he:" + phone_call;
                else if (send_type == "KBTT_NOINSTAL_APP")
                    message = "Ho so " + bks + " cua quy khach da duoc tao quy khach vui long vao kho ung dung tai phan mem PVI Mobile de thuc hien giam dinh, dien thoai lien he:" + phone_call;
                //ws_sms.SendSms(message, phone, "BaohiemPVI", "5lGtsmmZ3Pf0d4Vqxc1eiw==", "T0bgxXd9QDWK_prS77leGw==", "Icm42ifKB_PQiumr35WtaA==", "southtelecom");
                if (phone.Substring(0, 1) == "0")
                {
                    phone = "84" + phone.Remove(0, 1);
                }
                else if (phone.Substring(0, 1) == "+")
                {
                    phone = phone.Remove(0, 1);
                }
                string dataCP = "";
                string resultCp = "";
                string key = "123456456789";
                string url = _configuration["DownloadSettings:url_sendmessage"];
                DataContentCP objCP = new DataContentCP();

                objCP.ma_donvi = so_hsgd;
                objCP.ma_doitac = "PVI_247";
                objCP.nguon_tao = "PVI_247";
                objCP.mang = "";
                objCP.id = "2";
                objCP.type = "SMS";
                objCP.To = phone;
                objCP.text = message;
                objCP.requestid = "PVI_247_" + so_hsgd + "_SMS";
                objCP.sign = ContentHelper.MD5(key + objCP.type + objCP.To + objCP.requestid);
                dataCP = JsonConvert.SerializeObject(objCP);
                resultCp = ContentHelper.PostData(dataCP, url);
                _logger.Information("sendsms to " + phone + " pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " kết quả " + resultCp);
            }
            catch (Exception ex)
            {
                result = false;
                _logger.Error($"sendsms to " + phone + " pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " error: " + ex);
            }
            return result;
        }
        public void SendEmail_PVI247(string sTo, decimal pr_key_hsbt_ct, decimal pr_key_hsgd_ctu, string strFileNamePdf, string loai_gui)
        {
            AlternateView avHtml = null;
            string htmlBody = "";
            var withBlock = _context.HsgdCtus.Where(x => x.PrKey == pr_key_hsgd_ctu).FirstOrDefault();
            if (withBlock != null)
            {
                htmlBody = "<html xmlns=\"http://www.w3.org/1999/xhtml\"> "
            + "<head runat=\"server\"> "
            + "    <title></title> "
            + "    <style type=\"text/css\"> "
            + "        #content p { "
            + "            margin: 5px 0; "
            + "            color: #085d60; "
            + "        } "
            + " "
            + "        * { "
            + "            margin: 0; "
            + "            padding: 0; "
            + "        } "
            + " "
            + "        a { "
            + "            text-decoration: none; "
            + "        } "
            + "    </style> "
            + "</head> "
            + "<body> "
            + "    <form id=\"form1\" runat=\"server\"> "
            + "        <table align=\"center\" style=\"border-collapse: collapse\" bordercolor=\"#0185D0\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\"> "
            + "            <tr> "
            + "                <td style=\"width: 100%; height: 130px;\"> "
            + "                    <img src=\"cid:Pic1\" style=\"width: 100%; height: 130px; border: none; margin: 0; display: block\" />                     "
            + "                </td> "
            + "            </tr> "
            + "            <tr> "
            + "                <td style=\"font-family: Arial; font-size: 13px; padding: 5px; vertical-align: top; text-align:justify\"> "
            + "                    <p style=\"MARGIN-TOP: 30px\"> "
            + "                        <strong>lblNg_gdich</strong> "
            + "                    </p> "
            + "                    <p style=\"MARGIN-TOP: 20px\"> "
            + "                        lblTitle"
            + "                    </p> ";
                htmlBody = htmlBody.Replace("lblTitle", "lbl_thong_bao");
                htmlBody = htmlBody
                + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px; font-weight:bold;color:red\"> "
                + "                        - Tên chủ xe: lbl_tenkh_duocbaolanh"
                + "                    </p> "
                + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
                + "                        - BKS/Số khung: lbl_bienksoat"
                + "                    </p> "
                + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
                + "                        - Ngày Tổn thất: lbl_ngaytt"
                + "                    </p> "
                + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
                + "                        - Ngày Thông báo: lbl_ngaytb"
                + "                    </p> "
                + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
                + "                        - Ng.nhân TT: lbl_nguyennhan"
                + "                    </p> ";
                htmlBody = htmlBody + "    lbl_loaiguiaa điện tử được đính kèm tại email này có giá trị pháp lý và là căn cứ xem xét thanh toán theo đúng các quy định tại Thỏa thuận Hợp tác đã ký kết giữa hai bên.";
                htmlBody = htmlBody + "    Trân trọng cảm ơn!"
                    + "                    <p style=\"MARGIN-TOP: 10px; font-weight:bold\">Tổng Công ty Bảo hiểm PVI</p> "
                    + "                    <p>Địa chỉ: PVI Tower, Số 1 Phạm Văn Bạch, Quận Cầu Giấy, Hà Nội.</p> "
                    + "                    <p>Website: <a href=\"http://www.pvi.com.vn\" style=\"color:blue\">http://www.pvi.com.vn</a></p> "
                    + "                    <p style=\"MARGIN-TOP: 10px; font-weight:bold\">Hỗ trợ mua bảo hiểm trực tuyến 24/7: 1900 54 54 58</p> "
                    + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\">- Hỗ trợ qua email: <a href=\"mailto:Support@pvi.com.vn\" style=\"color:blue\">Support@pvi.com.vn</a></p> "
                    + "                    <p style=\"font-style:italic\"> (*) Đây là email hệ thống gửi tự động, vui lòng không trả lời (reply) lại email này.</p> "
                    + "                </td> "
                    + "            </tr> "
                    + "        </table> "
                    + "    </form> "
                    + "</body> "
                    + "</html>";
                var ten_donvi = _context.DmDonvis.Where(x => x.MaDonvi == withBlock.MaDonvi).Select(s => s.TenDonvi.ToUpper()).FirstOrDefault() ?? "";
                var ten_dvcapd = "";
                if (!string.IsNullOrEmpty(ten_donvi))
                {
                    ten_donvi = "BẢO HIỂM " + ten_donvi;
                    ten_dvcapd = "CÔNG TY BH " + ten_donvi;
                }
                var ten_user = _context.DmUsers.Where(x => x.Oid == withBlock.MaUser).Select(s => s.TenUser).FirstOrDefault() ?? "";
                if (loai_gui == "PASC")
                {
                    htmlBody = htmlBody.Replace("lblNg_gdich", " Kính gửi: " + ten_donvi);
                    htmlBody = htmlBody.Replace("lbl_thong_bao", " Liên quan đến xe ô tô BKS: " + withBlock.BienKsoat + " tham gia BH tại " + ten_dvcapd + ", do Giám định viên của Bảo hiểm PVI: " + ten_user + " tiến hành giám định tổn thất, " + (withBlock.MaDonvigd == "31" ? "VPĐD CSKH BH PVI PHÍA BẮC" : "VPĐD CSKH BH PVI PHÍA NAM") + " xin gửi phương án sửa chưa điện tử đính kèm trong email.");
                }
                else
                {
                    var ma_gara = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCt == pr_key_hsbt_ct).Select(s => s.MaGara).FirstOrDefault();
                    var ten_gara = "";
                    if (!string.IsNullOrEmpty(ma_gara))
                    {
                        ten_gara = _context.DmGaRas.Where(x => x.MaGara == ma_gara).Select(s => (s.TenGara + (s.DiaChi != "" ? " - " + s.DiaChi : ""))).FirstOrDefault();
                    }
                    htmlBody = htmlBody.Replace("lblNg_gdich", " Kính gửi : " + ten_gara);
                    htmlBody = htmlBody.Replace("lbl_thong_bao", " Liên quan đến xe ô tô BKS: " + withBlock.BienKsoat + " tham gia BH tại " + ten_dvcapd + " đã được Quý Công ty thực hiện sửa chữa và do Giám định viên của Bảo hiểm PVI: " + ten_user + " tiến hành giám định tổn thất, Bảo hiểm PVI đồng ý bảo lãnh thanh toán cho xe trên với các nội dung như file bảo lãnh điện tử số: " + DateTime.Now.Date.Year.ToString() + "-" + withBlock.MaDonvi + "-" + withBlock.SoHsgd + " đính kèm");
                }



                htmlBody = htmlBody.Replace("lbl_tenkh_duocbaolanh", withBlock.TenKhach);
                htmlBody = htmlBody.Replace("lblngay_dau", Convert.ToDateTime(withBlock.NgayDauSeri).ToString("dd/MM/yyyy"));
                htmlBody = htmlBody.Replace("lblngay_cuoi", Convert.ToDateTime(withBlock.NgayCuoiSeri).ToString("dd/MM/yyyy"));
                htmlBody = htmlBody.Replace("lbl_bienksoat", withBlock.BienKsoat);
                htmlBody = htmlBody.Replace("lbl_ngaytt", Convert.ToDateTime(withBlock.NgayTthat).ToString("dd/MM/yyyy"));
                htmlBody = htmlBody.Replace("lbl_ngaytb", Convert.ToDateTime(withBlock.NgayTbao).ToString("dd/MM/yyyy"));
                htmlBody = htmlBody.Replace("lbl_nguyennhan", withBlock.NguyenNhanTtat);
                if (loai_gui == "BL")
                    htmlBody = htmlBody.Replace("lbl_loaiguiaa", "Thư bảo lãnh");
                else
                    htmlBody = htmlBody.Replace("lbl_loaiguiaa", "Phương án sửa chưa");
                string image = _configuration["Word2PdfSettings:BannerBHTT05"];

                avHtml = AlternateView.CreateAlternateViewFromString(htmlBody, null/* TODO Change to default(_) if this is not a reference type */, MediaTypeNames.Text.Html);
                LinkedResource pic = new LinkedResource(image, MediaTypeNames.Image.Jpeg);
                pic.ContentId = "Pic1";
                avHtml.LinkedResources.Add(pic);
                SendEmail(sTo.Trim(), "PVI: " + (loai_gui == "BL" ? "PVI: Thư bảo lãnh: " + DateTime.Now.Date.Year.ToString() + "-" + withBlock.MaDonvi + "-" + withBlock.SoHsgd + " cho xe ô tô BKS: " + withBlock.BienKsoat + " của " + withBlock.TenKhach : "PVI: Thư gửi Phương án sửa chữa điện tử xe ô tô BKS: " + withBlock.BienKsoat), strFileNamePdf, htmlBody, avHtml, withBlock.PrKey);
            }


        }
        public void SendEmail(string sTo, string sSubject, string strFileNamePdf, string htmlBody, AlternateView avHtml, decimal pr_key_hsgd_ctu)
        {
            try
            {
                MailAddress from = new MailAddress("baohiempvi@pvi.com.vn", "BAOHIEMPVI", System.Text.Encoding.UTF8);
                MailAddress to = new MailAddress(sTo);

                System.Net.Mail.MailMessage Mail = new System.Net.Mail.MailMessage(from, to);
                Mail.Subject = sSubject;
                Mail.SubjectEncoding = System.Text.Encoding.UTF8;
                if (avHtml != null)
                {
                    Mail.AlternateViews.Add(avHtml);
                }
                if (htmlBody != "")
                {
                    Mail.Body = htmlBody;
                }
                Mail.BodyEncoding = System.Text.Encoding.UTF8;
                Mail.IsBodyHtml = true;
                System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(strFileNamePdf);
                Mail.Attachments.Add(attachment);
                SmtpClient SmtpServer = new SmtpClient();
                SmtpServer.Port = 25;
                SmtpServer.Host = "mailapp.pvi.com.vn";
                SmtpServer.EnableSsl = false;
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.Timeout = 15000;
                SmtpServer.Send(Mail);
                Mail.Dispose();
                SmtpServer.Dispose();
                _logger.Information("SendEmail to " + sTo + ", pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " success");
            }
            catch (Exception ex)
            {
                _logger.Error("Lỗi SendEmail to " + sTo + ", pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " error: " + ex.Message.ToString());
            }
        }
        public string UpdateNhatKyPADT(decimal pr_key_hsbt_ct, NhatKy? nhat_ky)
        {
            var result = "";
            try
            {
                _context.HsgdDxCts
                    .Where(x => x.PrKeyHsbtCt == pr_key_hsbt_ct)
                    .ExecuteUpdate(s => s.SetProperty(u => u.PascSendEmail, 1));
                if (nhat_ky != null)
                {
                    _context.NhatKies.Add(nhat_ky);
                    _context.SaveChanges();
                }
                result = "Thành công";
                _logger.Error("UpdateNhatKyPADT pr_key_hsbt_ct =" + pr_key_hsbt_ct + " succcess");
            }
            catch (Exception ex)
            {
                result = "Thất bại";
                _logger.Error("UpdateNhatKyPADT pr_key_hsbt_ct =" + pr_key_hsbt_ct + " error " + ex);
            }
            return result;
        }
        public string UpdateNhatKyThongBaoBT(decimal pr_key_hsgd_ctu, NhatKy? nhat_ky)
        {
            var result = "";
            try
            {
                _context.HsgdCtus
                    .Where(x => x.PrKey == pr_key_hsgd_ctu)
                    .ExecuteUpdate(s => s.SetProperty(u => u.SendThongbaoBt, 1));
                if (nhat_ky != null)
                {
                    _context.NhatKies.Add(nhat_ky);
                    _context.SaveChanges();
                }
                result = "Thành công";
                _logger.Error("UpdateNhatKyThongBaoBT pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " succcess");
            }
            catch (Exception ex)
            {
                result = "Thất bại";
                _logger.Error("UpdateNhatKyThongBaoBT pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " error " + ex);
            }
            return result;
        }
        public async Task<PagedList<LichsuPa>> LichSuPasc(LichsuPaParameters lichsuPaParameters)
        {

            var data = (from a in _context.LichsuPas
                        where a.TenHmuc.Equals(lichsuPaParameters.ten_hmuc)
                        select a
                             ).AsQueryable();
            if (lichsuPaParameters.loai_xe || lichsuPaParameters.xuat_xu || lichsuPaParameters.nam_sx)
            {
                var hsgd_dx_ct = _context.HsgdDxCts.Where(x => x.PrKey == lichsuPaParameters.pr_key_hsgd_dx_ct).FirstOrDefault();
                if (hsgd_dx_ct != null)
                {
                    if (lichsuPaParameters.loai_xe)
                    {
                        data = data.Where(x => x.LoaiXe == hsgd_dx_ct.LoaiXe);
                    }
                    if (lichsuPaParameters.xuat_xu)
                    {
                        data = data.Where(x => x.XuatXu == hsgd_dx_ct.XuatXu);
                    }
                    if (lichsuPaParameters.nam_sx)
                    {
                        data = data.Where(x => x.NamSx == hsgd_dx_ct.NamSx);
                    }
                }
            }
            return await PagedList<LichsuPa>.ToPagedListAsync(data, lichsuPaParameters.pageNumber, lichsuPaParameters.pageSize);
        }
        public string Map_tinh_trang(string tinh_trang)
        {
            string ftinh_trang = null;
            if (tinh_trang == "1")
                ftinh_trang = "Chưa giao giám định";
            else if (tinh_trang == "2")
                ftinh_trang = "Đã giao giám định";
            else if (tinh_trang == "3")
                ftinh_trang = "Đang giám định";
            else if (tinh_trang == "4")
                ftinh_trang = "Chờ phê duyệt";
            else if (tinh_trang == "5")
                ftinh_trang = "Bổ sung thông tin";
            else if (tinh_trang == "6")
                ftinh_trang = "Đã duyệt";
            else if (tinh_trang == "7")
                ftinh_trang = "Hồ sơ đã hủy";
            else if (tinh_trang == "8")
                ftinh_trang = "Hồ sơ TPC chưa xử lý";
            else if (tinh_trang == "9")
                ftinh_trang = "Hồ sơ TPC đang xử lý";
            else if (tinh_trang == "10")
                ftinh_trang = "Hồ sơ TPC chờ duyệt";
            else if (tinh_trang == "DBL")
                ftinh_trang = "Duyệt Bảo lãnh";
            else if (tinh_trang == "BLDT")
                ftinh_trang = "Gửi Bảo lãnh điện tử";
            else if (tinh_trang == "PADT")
                ftinh_trang = "Gửi PASC điện tử";
            else if (tinh_trang == "HSBT")
                ftinh_trang = "Tạo HSBT Pias";
            else if (tinh_trang == "PASC")
                ftinh_trang = "PASC ký điện tử";
            else if (tinh_trang == "TBBT")
                ftinh_trang = "Gửi thông báo BT";
            else
                ftinh_trang = "Không xác định";
            return ftinh_trang;
        }
        public DmCtukt? GetCtuKt(string maCtuKt, int maDviInt)
        {

            //TODO
            //Chuyen query sang kieu parameter
            //Chuyen sang transaction neu dung chung 1 DB connection, 2 DB thi thoi
            string query = $"DECLARE @Numctu NUMERIC(18,0); " +
                              $"UPDATE dm_ctukt SET DM_CTUKT.num = DM_CTUKT.num +1, @Numctu = DM_CTUKT.num + 1 WHERE ma_ctukt = @maCtuKt AND ma_dvi_int = @maDviInt; " +
                            $"SELECT @Numctu as Numctu";

            var conn = _context_pias_update.Database.GetDbConnection();
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }


            DmCtukt obj = new DmCtukt();
            var command = conn.CreateCommand();
            command.CommandText = query;
            var p = command.CreateParameter();
            p.ParameterName = "maCtuKt";
            p.Value = maCtuKt;

            command.Parameters.Add(p);
            var p1 = command.CreateParameter();
            p1.ParameterName = "maDviInt";
            p1.Value = maDviInt;
            command.Parameters.Add(p1);


            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var title = reader.GetDecimal(0);
                obj.Num = title;
            }

            conn.Close();
            return obj;

        }
        public Task<List<KHVat>> GetListDonViGiamDinh(string email_login)
        {
            var user_login = _context.DmUsers.Where(x => x.Mail == email_login).FirstOrDefault();

            var list_dvgd = (from k in _context_pias.DmKhaches
                             where k.MaDonvi == user_login.MaDonvi || k.ViewAll == true || (k.Gara == true && k.GaraTthai == true)
                             select new KHVat
                             {
                                 MaDM = k.MaKh,
                                 TenDM = k.TenKh,
                                 MasoVat = k.MasoVat
                             }
                       ).AsQueryable();
            return ToListWithNoLockAsync(list_dvgd);
        }
        public List<FileAttachBt> GetFileAttachBt(decimal pr_key_hsbt_ct, string ma_ctu)
        {
            var obj_result = ToListWithNoLock(_context_pias_update.FileAttachBts.Where(x => x.FrKey == pr_key_hsbt_ct && x.MaCtu == ma_ctu).AsQueryable());
            return obj_result;
        }
        public string GetMaTtrangGd(decimal PrKeyHsgd)
        {
            var MaTtrangGd = _context.HsgdCtus.Where(x => x.PrKey == PrKeyHsgd).Select(s => s.MaTtrangGd).FirstOrDefault() ?? "";
            return MaTtrangGd;
        }
        public decimal GetSTBTByHsgd(decimal pr_key_hsbt_ctu)
        {
            var pr_key_hsbt_ct = _context_pias_update.HsbtCts.Where(x => x.FrKey == pr_key_hsbt_ctu && new[] { "050101", "050104" }.Contains(x.MaSp) && x.PrKey != 0).Select(s => s.PrKey).ToArray();
            var pr_key_hsgd_dx_ct = _context.HsgdDxCts.Where(x => pr_key_hsbt_ct.Contains(x.PrKeyHsbtCt)).Select(s => s.PrKey).ToArray();
            decimal sotien_bt = 0;
            foreach (var item in pr_key_hsgd_dx_ct)
            {
                var sum = ReloadSum(item);
                if (sum != null && sum.Count > 0)
                {
                    sotien_bt += sum[0].StBl ?? 0;
                }
            }
            return sotien_bt;
        }
        public bool KyPASCXCG(decimal pr_key_hsgd_ctu, string file_path, string email, string SignContent)
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
                    try
                    {
                        if (result)
                        {
                            var attachFilesToDelete = _context.HsgdAttachFiles
                            .Where(b => b.MaCtu == "PASC" && b.FrKey == pr_key_hsgd_ctu)
                            .ToList();
                            // Nếu có dữ liệu thì xóa
                            if (attachFilesToDelete.Any())
                            {
                                _context.HsgdAttachFiles.RemoveRange(attachFilesToDelete);
                                _context.SaveChanges();
                            }
                            List<HsgdAttachFile> attachFiles = new List<HsgdAttachFile>();
                            var atf = new HsgdAttachFile
                            {
                                PrKey = Guid.NewGuid().ToString().ToLower(),
                                FrKey = pr_key_hsgd_ctu,
                                MaCtu = "PASC",
                                FileName = "PASC.pdf",
                                Directory = file_path,
                                ngay_cnhat = DateTime.Now,
                                GhiChu = "Cập nhật từ ký PASC",
                                NguonTao = "WebPvi247"
                            };
                            attachFiles.Add(atf);
                            // Add vào context
                            _context.HsgdAttachFiles.AddRange(attachFiles);
                            _context.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Ký xong thêm bảo lãnh vào bảng hồ sơ lỗi pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " error occurred: " + ex);

                    }
                    _logger.Information($"PrintPASC call KyPASCXCG pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " result =  " + result);
                }
                else
                {
                    _logger.Error($"PrintPASC call KyPASCXCG pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " chưa phân quyền ký trong bảng hddt_hsm ");
                }

            }
            catch (Exception ex)
            {
                _logger.Error($"PrintPASC call KyPASCXCG pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " error occurred: " + ex);
                result = false;
            }
            return result;
        }
        public int GetPascSendMail(decimal pr_key_hsbt_ct)
        {
            var pasc_send_mail = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCt == pr_key_hsbt_ct).Select(s => s.PascSendEmail).FirstOrDefault();
            return pasc_send_mail;
        }
        public string GetFilePathPasc(decimal pr_key_hsbt_ct)
        {
            var file_path = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCt == pr_key_hsbt_ct).Select(s => s.PathPasc).FirstOrDefault() ?? "";
            return file_path;
        }
        public string UpdatePathPasc(decimal pr_key_hsbt_ct, string file_path)
        {
            var result = "";
            try
            {
                var hsgd_dx_ct = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCt == pr_key_hsbt_ct).FirstOrDefault();
                if (hsgd_dx_ct != null)
                {
                    hsgd_dx_ct.PathPasc = file_path;
                    _context.HsgdDxCts.Update(hsgd_dx_ct);
                    _context.SaveChanges();
                }
                result = "Thành công";
                _logger.Error("UpdatePathPasc pr_key_hsbt_ct =" + pr_key_hsbt_ct + " succcess");
            }
            catch (Exception ex)
            {
                result = "Thất bại";
                _logger.Error("UpdatePathPasc pr_key_hsbt_ct =" + pr_key_hsbt_ct + " error " + ex);
            }
            return result;
        }
        public HsgdCtu? GetHsgdCtu(decimal PrKeyHsgd)
        {
            var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == PrKeyHsgd).FirstOrDefault();
            return hsgd_ctu;
        }
        public List<HsgdDxCt> GetListHsgdDxCt(decimal pr_key_hsbt_ctu)
        {
            var list_hsgd_dx_ct = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCtu == pr_key_hsbt_ctu).ToList();
            return list_hsgd_dx_ct;
        }
        public HsgdDxCt GetHsgdDxCt(decimal pr_key_hsbt_ct)
        {
            var hsgd_dx_ct = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCt == pr_key_hsbt_ct).FirstOrDefault();
            return hsgd_dx_ct;
        }
        public string GetThongTinKyDienTu(decimal pr_key_hsgd_ctu, string email)
        {
            //lấy thông tin chữ ký điện tử
            string SignContent = "";
            var user_login = _context.DmUsers.Where(x => x.Mail == email).FirstOrDefault();
            if (user_login != null)
            {
                var dm_var = (from vars in _context_pias.DmVars
                              where vars.MaDonvi == user_login.MaDonvi && vars.Bien == "DON_VI"
                              select vars).FirstOrDefault();
                if (dm_var != null)
                {
                    SignContent = "Ký bởi: " + dm_var.GiaTri;
                }
                var nhat_ky_duyet = _context.NhatKies.Where(x => x.FrKey == pr_key_hsgd_ctu && x.MaTtrangGd == "6").OrderByDescending(o => o.PrKey).FirstOrDefault();
                if (nhat_ky_duyet != null)
                {
                    SignContent += "\n" + "Ngày ký: " + Convert.ToDateTime(nhat_ky_duyet.NgayCapnhat).ToString("dd/MM/yyyy HH:mm:ss");
                }
            }
            return SignContent;
            //
        }
        public CombinedTtrinhResult3 PrintThongBaoBT(decimal pr_key_hsgd_ctu, string email)
        {
            try
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
                var wordPdfRequest = new WordToPdfRequest();
                wordPdfRequest.ListData = new List<EntityContent>();
                UpdateProperties update = new UpdateProperties();
                List<ThuHuong> hsgd_totrinh_tt = new List<ThuHuong>();
                var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == pr_key_hsgd_ctu).FirstOrDefault();
                if (hsgd_ctu != null)
                {
                    var hsbt_ctu = _context_pias_update.HsbtCtus.Where(x => x.PrKey == hsgd_ctu.PrKeyBt).FirstOrDefault();
                    var hsgd_tbbt = _context.HsgdTbbts.Where(x => x.PrKeyHsgd == pr_key_hsgd_ctu).FirstOrDefault();
                    if (hsbt_ctu != null)
                    {
                       
                        update.AddEntityContent(wordPdfRequest, "[TENNG_NHANTIEN]", hsbt_ctu.NgdcBh);
                        update.AddEntityContent(wordPdfRequest, "[NGAY_TBAO]", hsbt_ctu.NgayTbao != null ? Convert.ToDateTime(hsbt_ctu.NgayTbao).ToString("dd/MM/yyyy") : "");
                        update.AddEntityContent(wordPdfRequest, "[SO_HSBT]", hsbt_ctu.SoHsbt);
                        var so_hdgcn = "";
                        if (hsbt_ctu.SoSeri != 0)
                        {
                            so_hdgcn = hsbt_ctu.SoSeri.ToString();
                        }
                        else if (!string.IsNullOrEmpty(hsbt_ctu.SotheVcxMoto))
                        {
                            so_hdgcn = hsbt_ctu.SotheVcxMoto;
                        }
                        else
                        {
                            so_hdgcn = hsbt_ctu.SoHdgcn;
                        }

                        var hieu_xe = FirstOrDefaultWithNoLock(_context.HsgdDxCts
                                     .Where(a => a.PrKeyHsbtCtu == hsbt_ctu.PrKey)
                                     .Join(_context.DmHieuxes,
                                           a => a.HieuXe,
                                           b => b.PrKey,
                                           (a, b) => b.HieuXe).AsQueryable());

                        if (!string.IsNullOrEmpty(hieu_xe))
                            update.AddEntityContent(wordPdfRequest, "[LOAI_XE]", hieu_xe);
                        else
                            update.AddEntityContent(wordPdfRequest, "[LOAI_XE]", "");


                        update.AddEntityContent(wordPdfRequest, "[SO_HDGCN]", so_hdgcn);
                        update.AddEntityContent(wordPdfRequest, "[TEN_DTTT]", hsbt_ctu.TenDttt);
                        var ng_nhtien = "";
                        var dm_kh = _context_pias.DmKhaches.Where(x => x.MaKh == hsbt_ctu.MaKh).FirstOrDefault();
                        if (string.IsNullOrEmpty(hsbt_ctu.TenKhle))
                        {
                            ng_nhtien = dm_kh != null ? dm_kh.TenKh : "";
                        }
                        else
                        {
                            ng_nhtien = hsbt_ctu.TenKhle;
                        }
                        update.AddEntityContent(wordPdfRequest, "[NG_NHTIEN]", ng_nhtien);

                        string dia_chi_chuxe = "";
                        if (!string.IsNullOrEmpty(hsbt_ctu.DiaChi))
                        {
                            dia_chi_chuxe = hsbt_ctu.DiaChi;
                        }
                        else
                        {
                            dia_chi_chuxe = dm_kh.DiaChi;
                        }

                        List<string> list_diachi = ContentHelper.SplitString(dia_chi_chuxe, 255);
                        for (int i = 0; i < list_diachi.Count(); i++)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[DIA_CHI{i}]", list_diachi[i]);
                        }
                        for (int i = list_diachi.Count(); i < 10; i++)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[DIA_CHI{i}]", "");
                        }

                        update.AddEntityContent(wordPdfRequest, "[NGAY_TTHAT]", hsbt_ctu.NgayTthat != null ? Convert.ToDateTime(hsbt_ctu.NgayTthat).ToString("dd/MM/yyyy") : "");
                        List<string> list_diadiem = ContentHelper.SplitString(hsbt_ctu.DiaDiem.ToString(), 255);
                        for (int i = 0; i < list_diadiem.Count(); i++)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[DIA_DIEM{i}]", list_diadiem[i]);
                        }
                        for (int i = list_diadiem.Count(); i < 10; i++)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[DIA_DIEM{i}]", "");
                        }
                        List<string> list_nguyennhan = ContentHelper.SplitString(hsbt_ctu.NguyenNhanTtat.ToString(), 255);
                        for (int i = 0; i < list_nguyennhan.Count(); i++)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[NGUYEN_NHAN{i}]", list_nguyennhan[i]);
                        }
                        for (int i = list_nguyennhan.Count(); i < 20; i++)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[NGUYEN_NHAN{i}]", "");
                        }

                        var hsbt_ct = (from a in _context_pias_update.HsbtCts
                                       where a.FrKey == hsbt_ctu.PrKey
                                       select new
                                       {
                                           FrKey = a.FrKey,
                                           sotien_vcx = a.MaSp == "050104" && a.MaTtrangBt != "04" ? (a.SoTienp + a.SoTienvp) : 0,
                                           sotien_tnds21 = a.MaSp == "" ? (a.SoTienp + a.SoTienvp) : 0,
                                           sotien_tnds22 = a.MaSp == "050102" ? (a.SoTienp + a.SoTienvp) : 0,
                                           sotien_tnds23 = a.MaSp == "" ? (a.SoTienp + a.SoTienvp) : 0,
                                           sotien_tnds24 = a.MaSp == "" ? (a.SoTienp + a.SoTienvp) : 0,
                                           sotien_tnds25 = new[] { "050101", "050105", "050201", "050203", "050204" }.Contains(a.MaSp) ? (a.SoTienp + a.SoTienvp) : 0,
                                           sotien_tnds26 = new[] { "050103", "050202" }.Contains(a.MaSp) ? (a.SoTienp + a.SoTienvp) : 0,
                                           sotien_vcxm_ng = new[] { "050205" }.Contains(a.MaSp) && a.MaDkhoan == "05010101" ? (a.SoTienp + a.SoTienvp) : 0,
                                           sotien_vcxm_ts = new[] { "050205" }.Contains(a.MaSp) && a.MaDkhoan == "05010102" ? (a.SoTienp + a.SoTienvp) : 0,
                                           sotien_vcxm = a.MaSp == "050205" ? (a.SoTienp + a.SoTienvp) : 0
                                       }).AsQueryable();
                        var hsbt_ct_gr = hsbt_ct.GroupBy(g => g.FrKey)
                        .Select(s => new
                        {
                            sotien_vcx = s.Sum(x => x.sotien_vcx),
                            sotien_tnds21 = s.Sum(x => x.sotien_tnds21),
                            sotien_tnds22 = s.Sum(x => x.sotien_tnds22),
                            sotien_tnds23 = s.Sum(x => x.sotien_tnds23),
                            sotien_tnds24 = s.Sum(x => x.sotien_tnds24),
                            sotien_tnds25 = s.Sum(x => x.sotien_tnds25),
                            sotien_tnds26 = s.Sum(x => x.sotien_tnds26),
                            sotien_vcxm_ng = s.Sum(x => x.sotien_vcxm_ng),
                            sotien_vcxm_ts = s.Sum(x => x.sotien_vcxm_ts),
                            sotien_vcxm = s.Sum(x => x.sotien_vcxm)
                        }).FirstOrDefault();
                        if (hsbt_ct_gr != null)
                        {
                            update.AddEntityContent(wordPdfRequest, "[SOTIEN_VCX]", hsbt_ct_gr.sotien_vcx.ToString("#,###", cul.NumberFormat));

                            try
                            {
                               
                                //haipv1 do không xác định được chính các các mục từ 2.1-->2.5 nên để trống tự điện chỉ để lại số tiền TNDS tổng
                                if(hsgd_tbbt!=null)
                                {
                                    update.AddEntityContent(wordPdfRequest, "[SOTIEN_TNDS21]", hsgd_tbbt.TndsXeCoGioi.ToString("#,###", cul.NumberFormat));
                                    update.AddEntityContent(wordPdfRequest, "[SOTIEN_TNDS22]", hsgd_tbbt.TndsHangHoa.ToString("#,###", cul.NumberFormat));
                                    update.AddEntityContent(wordPdfRequest, "[SOTIEN_TNDS23]", hsgd_tbbt.TndsTaiNanHk.ToString("#,###", cul.NumberFormat));
                                    update.AddEntityContent(wordPdfRequest, "[SOTIEN_TNDS24]", hsgd_tbbt.TndsTaiSanKhac.ToString("#,###", cul.NumberFormat));
                                    update.AddEntityContent(wordPdfRequest, "[SOTIEN_TNDS25]", hsgd_tbbt.TndsNguoi.ToString("#,###", cul.NumberFormat));
                                }   
                                else
                                {
                                    update.AddEntityContent(wordPdfRequest, "[SOTIEN_TNDS21]", "");
                                    update.AddEntityContent(wordPdfRequest, "[SOTIEN_TNDS22]", "");
                                    update.AddEntityContent(wordPdfRequest, "[SOTIEN_TNDS23]", "");
                                    update.AddEntityContent(wordPdfRequest, "[SOTIEN_TNDS24]", "");
                                    update.AddEntityContent(wordPdfRequest, "[SOTIEN_TNDS25]", "");
                                }    
                                
                            }
                            catch (Exception ex)
                            {
                                _logger.Error(ex.ToString());                               
                            }
                            //update.AddEntityContent(wordPdfRequest, "[SOTIEN_TNDS21]", hsbt_ct_gr.sotien_tnds21.ToString("#,###", cul.NumberFormat));
                            //update.AddEntityContent(wordPdfRequest, "[SOTIEN_TNDS22]", hsbt_ct_gr.sotien_tnds22.ToString("#,###", cul.NumberFormat));
                            //update.AddEntityContent(wordPdfRequest, "[SOTIEN_TNDS23]", hsbt_ct_gr.sotien_tnds23.ToString("#,###", cul.NumberFormat));
                            //update.AddEntityContent(wordPdfRequest, "[SOTIEN_TNDS24]", hsbt_ct_gr.sotien_tnds24.ToString("#,###", cul.NumberFormat));
                            //update.AddEntityContent(wordPdfRequest, "[SOTIEN_TNDS25]", hsbt_ct_gr.sotien_tnds25.ToString("#,###", cul.NumberFormat));

                           
                            update.AddEntityContent(wordPdfRequest, "[SOTIEN_TNDS26]", hsbt_ct_gr.sotien_tnds26.ToString("#,###", cul.NumberFormat));
                            var tong_tien_tnds = hsbt_ct_gr.sotien_tnds21 + hsbt_ct_gr.sotien_tnds22 + hsbt_ct_gr.sotien_tnds23 + hsbt_ct_gr.sotien_tnds24 + hsbt_ct_gr.sotien_tnds25;
                            update.AddEntityContent(wordPdfRequest, "[TONG_TIEN_TNDS]", tong_tien_tnds.ToString("#,###", cul.NumberFormat));
                            var tong_cong = hsbt_ct_gr.sotien_vcx + tong_tien_tnds + hsbt_ct_gr.sotien_tnds26;
                            update.AddEntityContent(wordPdfRequest, "[TONG_CONG]", tong_cong.ToString("#,###", cul.NumberFormat));
                            update.AddEntityContent(wordPdfRequest, "[SOTIEN_TAMUNG]", "0");
                            update.AddEntityContent(wordPdfRequest, "[CHE_TAI]", "0");
                            update.AddEntityContent(wordPdfRequest, "[SOTIEN_THOI_SAUBT]", "0");
                            update.AddEntityContent(wordPdfRequest, "[CHI_KHAC]", hsbt_ctu.ChiKhac.ToString("#,###", cul.NumberFormat));
                            var sotien_bt = tong_cong + hsbt_ctu.ChiKhac;
                            update.AddEntityContent(wordPdfRequest, "[SOTIEN_BT]", sotien_bt.ToString("#,###", cul.NumberFormat));
                            update.AddEntityContent(wordPdfRequest, "[SOTIEN_BC]", ContentHelper.NumberToText((double)sotien_bt));
                        }
                    }
                    if (!string.IsNullOrEmpty(hsgd_ctu.MaDonvi))
                    {
                        DateTime ngayCanSo = new DateTime(2025, 10, 15);
                        string ma_donvighihoadon = "";
                        if (hsgd_tbbt != null)
                        {
                            ma_donvighihoadon = hsgd_tbbt.MaDonviTT;
                            //Lấy ngày duyệt thông báo bồi thường để cập nhật vào tờ thông báo
                            var ngayCapNhat = _context.NhatKies
                            .Where(x => x.FrKey == hsgd_tbbt.PrKey && x.MaTtrangGd == "DTBBT")
                            .OrderByDescending(x => x.PrKey)
                            .Select(x => x.NgayCapnhat)
                            .FirstOrDefault();
                            if(ngayCapNhat!= DateTime.MinValue)
                            {
                                update.AddEntityContent(wordPdfRequest, "[DD]", ngayCapNhat.Day.ToString());
                                update.AddEntityContent(wordPdfRequest, "[MM]", ngayCapNhat.Month.ToString());
                                update.AddEntityContent(wordPdfRequest, "[NAM]", ngayCapNhat.Year.ToString());                                
                            }
                            else
                            {
                                update.AddEntityContent(wordPdfRequest, "[DD]", DateTime.Now.Day.ToString());
                                update.AddEntityContent(wordPdfRequest, "[MM]", DateTime.Now.Month.ToString());
                                update.AddEntityContent(wordPdfRequest, "[NAM]", DateTime.Now.Year.ToString());
                            }    
                        }    
    
                        else
                        {
                            if (hsgd_ctu.NgayCtu > ngayCanSo)
                            {
                                ma_donvighihoadon = hsgd_ctu.MaDonvi;
                            }
                            else
                            {
                                ma_donvighihoadon = hsgd_ctu.MaDonviTt;
                            }
                        }
                        
                        var thong_tin = (from vars in _context_pias.DmVars
                                         where vars.MaDonvi == ma_donvighihoadon &&
                                               (vars.Bien == "DON_VI" || vars.Bien == "DIA_CHI" || vars.Bien == "MASO_VAT")
                                         select vars).ToList();
                        var donvi_tt = "";
                        var diachi_tt = "";
                        var ma_sothue_tt = "";
                        foreach (var record in thong_tin)
                        {
                            switch (record.Bien)
                            {
                                case "DON_VI":
                                    donvi_tt = record.GiaTri;
                                    break;
                                case "DIA_CHI":
                                    diachi_tt = record.GiaTri;
                                    break;
                                case "MASO_VAT":
                                    ma_sothue_tt = record.GiaTri;
                                    break;
                                default:
                                    break;
                            }
                        }
                        update.AddEntityContent(wordPdfRequest, "[DONVI_TT]", donvi_tt);
                        update.AddEntityContent(wordPdfRequest, "[DIACHI_TT]", diachi_tt);
                        update.AddEntityContent(wordPdfRequest, "[MASOTHUE_TT]", ma_sothue_tt);
                    }
                    var user_login = _context.DmUsers.Where(x => x.Mail == email).FirstOrDefault();
                    //var dv_cq = _context.DmDonvis.Where(x => x.MaDonvi == user_login.MaDonvi).FirstOrDefault();
                    var vpdd = (from vars in _context_pias.DmVars
                                where vars.MaDonvi == user_login.MaDonvi &&
                                    (vars.Bien == "DON_VI" || vars.Bien == "DIA_CHI")
                                select vars).ToList();
                    var vpdd_ten = "";
                    var vpdd_diachi = "";
                    foreach (var record in vpdd)
                    {
                        switch (record.Bien)
                        {
                            case "DON_VI":
                                vpdd_ten = record.GiaTri;
                                break;
                            case "DIA_CHI":
                                vpdd_diachi = record.GiaTri;
                                break;
                            default:
                                break;
                        }
                    }
                    update.AddEntityContent(wordPdfRequest, "[VPDD_TEN]", vpdd_ten);
                    update.AddEntityContent(wordPdfRequest, "[VPDD_DIACHI]", vpdd_diachi);
                    var user_gdv = _context.DmUsers.Where(x => x.Oid == (hsgd_ctu.NguoiXuly == "" ? hsgd_ctu.MaUser : Guid.Parse(hsgd_ctu.NguoiXuly))).FirstOrDefault();
                    if (user_gdv != null)
                    {
                        update.AddEntityContent(wordPdfRequest, "[GDV]", user_gdv.TenUser ?? "");
                        update.AddEntityContent(wordPdfRequest, "[GDV_SDT]", user_gdv.Dienthoai ?? "");
                    }
                    else
                    {
                        update.AddEntityContent(wordPdfRequest, "[GDV]", "");
                        update.AddEntityContent(wordPdfRequest, "[GDV_SDT]", "");
                    }

                    #region lấy thông tin thụ hưởng thanh toán
                    hsgd_totrinh_tt = ToListWithNoLock((from a in _context.HsgdCtus
                                                        join b in _context.HsgdTbbts on a.PrKey equals b.PrKeyHsgd
                                                        join c in _context.HsgdTbbtTts on b.PrKey equals c.FrKey
                                                        where a.PrKey == hsgd_ctu.PrKey
                                                        select new ThuHuong
                                                        {
                                                            TenChuTk = c.TenChuTk,
                                                            SoTaikhoanNh = c.SoTaikhoanNh,
                                                            TenNh = c.TenNh,
                                                            LydoTt = c.LydoTt,
                                                            SotienTt = c.SotienTt
                                                        }
                                                       ).AsQueryable());

                    #endregion


                }
                var listData = wordPdfRequest.ListData;
                _logger.Information("PrintToTrinh " + JsonConvert.SerializeObject(listData));
                var listNew = new CombinedTtrinhResult3
                {
                    ThirdQueryResults = listData,
                    ListThuHuong = hsgd_totrinh_tt

                };

                return listNew;

            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                return null;
            }
        }
        public Task<List<DanhMuc>> GetListLoaiDongCo()
        {
            var list_dc = (from k in _context.DmLoaiDongcos
                           where k.MaLoaiDongco != ""
                           select new DanhMuc
                           {
                               MaDM = k.MaLoaiDongco,
                               TenDM = k.TenLoaiDongco
                           }
                       ).AsQueryable();
            return ToListWithNoLockAsync(list_dc);
        }
        public int GetSendMailThongBaoBT(decimal pr_key_hsgd_ctu)
        {
            var send_thongbao_bt = _context.HsgdTbbts.Where(x => x.PrKeyHsgd == pr_key_hsgd_ctu).Select(s => s.SendTbbt).FirstOrDefault();
            return send_thongbao_bt;
        }
        public bool CheckTrangThaiBT(decimal pr_key_hsgd_ctu)
        {
            var result = false;
            try
            {
                var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == pr_key_hsgd_ctu).FirstOrDefault();
                if (hsgd_ctu != null)
                {
                    var count_hsbt_ct = _context_pias.HsbtCts.Where(x => x.FrKey == hsgd_ctu.PrKeyBt && x.FrKey != 0 && !new[] { "02", "03" }.Contains(x.MaTtrangBt)).Count();
                    if (count_hsbt_ct == 0)
                    {
                        result = true;
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("CheckTrangThaiBT pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " error " + ex);
            }
            return result;
        }
        public string CheckKyTBBT(decimal pr_key_hsgd_ctu)
        {
            var path_tbbt = "";
            try
            {
                var HsgdTbbt = _context.HsgdTbbts.Where(x => x.PrKeyHsgd == pr_key_hsgd_ctu ).FirstOrDefault();
                if (HsgdTbbt != null )
                {
                    if (HsgdTbbt.PdTbbt==1)
                    {
                        return HsgdTbbt.PathTbbt;
                    }    
                }

            }
            catch (Exception ex)
            {
                _logger.Error("CheckKyTBTT pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " error " + ex);
            }
            return path_tbbt;
        }
        public ServiceResult GuiThongBaoBT(decimal pr_key_hsgd_ctu, string email_nhan, string email_login, string file_path)
        {
            string result = "";
            try
            {
                var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == pr_key_hsgd_ctu).FirstOrDefault();
                if (hsgd_ctu != null)
                {
                    _logger.Information("bắt đầu GuiThongBaoBT pr_key_hsgd_ctu = " + pr_key_hsgd_ctu);

                    var user_login = _context.DmUsers.Where(x => x.Mail == email_login).FirstOrDefault();

                    string ghichu_gui = "";
                    if (!string.IsNullOrEmpty(email_nhan))
                    {
                        var result_GuiThongBaoBT = GuiEmailThongBaoBT(file_path, pr_key_hsgd_ctu, email_nhan, email_login, user_login.MaDonvi);
                        if (result_GuiThongBaoBT)
                        {
                            ghichu_gui = "Email: " + email_nhan.Replace(" ", "");
                        }
                    }

                    //call update trạng thái
                    if (!string.IsNullOrEmpty(ghichu_gui))
                    {
                        var nhat_ky = new NhatKy();
                        nhat_ky.FrKey = hsgd_ctu.PrKey;
                        nhat_ky.MaTtrangGd = "TBBT";
                        nhat_ky.TenTtrangGd = Map_tinh_trang("TBBT");
                        nhat_ky.GhiChu = "Gửi thông báo bồi thường " + ghichu_gui;
                        nhat_ky.NgayCapnhat = DateTime.Now;
                        nhat_ky.MaUser = user_login.Oid;
                        UpdateNhatKyThongBaoBT(pr_key_hsgd_ctu, nhat_ky);
                        result = "Gửi thông báo BT thành công";
                        _logger.Information("GuiThongBaoBT pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " success");

                        return new ServiceResult
                        {
                            Success = true,
                            Message = result,
                            Data = pr_key_hsgd_ctu.ToString()
                        };
                    }
                    else
                    {
                        result = "Gửi thông báo BT thất bại";
                        _logger.Information("GuiThongBaoBT pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " error");

                        return new ServiceResult
                        {
                            Success = false,
                            Message = result
                        };
                    }
                }
                else
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Không tìm thấy hồ sơ"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"GuiThongBaoBT pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " An error occurred: " + ex);
                result = "Có lỗi xảy ra. Hãy thử lại sau.";

                return new ServiceResult
                {
                    Success = false,
                    Message = result
                };
            }
        }
        public bool GuiEmailThongBaoBT(string file_path, decimal pr_key_hsgd_ctu, string email_nhan, string email_login, string ma_donvi)
        {
            bool result = true;
            try
            {
                string url_download = _configuration["DownloadSettings:DownloadServer"] ?? "";
                string strFileNameLocalPdf = UtilityHelper.getPathAndCopyTempServer(file_path, url_download, "Thông báo bồi thường.pdf");
                //test tạm bỏ
                //if (ma_donvi == "31")
                //{
                //    SendEmail_ThongBaoBT(email_login,"vppb.xcg2.baolanh@gmail.com", strFileNameLocalPdf, pr_key_hsgd_ctu);
                //}
                //gửi theo danh sách email
                var arremail = email_nhan.Replace(" ", "").Split(";");
                if (arremail.Length < 6)
                {
                    foreach (var email in arremail)
                    {
                        SendEmail_ThongBaoBT(email_login, email, strFileNameLocalPdf, pr_key_hsgd_ctu);
                    }

                }
                // kiểm tra và xóa file ở local 
                if (System.IO.File.Exists(strFileNameLocalPdf))
                {
                    System.IO.File.Delete(strFileNameLocalPdf);
                }
                _logger.Information("GuiEmailThongBaoBT pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " success");

            }
            catch (Exception ex)
            {
                result = false;
                _logger.Error($"GuiEmailThongBaoBT pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " error: " + ex);
            }
            return result;
        }
        public void SendEmail_ThongBaoBT(string email_login, string email_nhan, string strFileNameLocalPdf, decimal pr_key_hsgd_ctu)
        {
            try
            {
                var subject = "Thông báo bồi thường - Tổng Công ty Bảo hiểm PVI";
                // gen body email
                #region gen body email
                AlternateView avHtml = null;
                var email_thongbao_bt = _configuration["Word2PdfSettings:email_thongbao_bt"] ?? "";
                string htmlBody = File.ReadAllText(email_thongbao_bt);
                //string htmlBody = "";
                var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == pr_key_hsgd_ctu).FirstOrDefault();
                if (hsgd_ctu != null)
                {
                    subject += " - " + hsgd_ctu.TenKhach;
                    var hsbt_ctu = _context_pias.HsbtCtus.Where(x => x.PrKey == hsgd_ctu.PrKeyBt).FirstOrDefault();
                    if (hsbt_ctu != null)
                    {
                        htmlBody = htmlBody.Replace("[SO_HSBT]", hsbt_ctu.SoHsbt ?? "");
                        subject += " - Số HS (Ref. number of Claim): " + hsbt_ctu.SoHsbt;
                    }
                    else
                    {
                        htmlBody = htmlBody.Replace("[SO_HSBT]", "");
                    }
                    if (hsgd_ctu.NgayTthat != null)
                    {
                        subject += " - Ngày tai nạn (Date of loss): " + Convert.ToDateTime(hsgd_ctu.NgayTthat).ToString("dd/MM/yyyy hh:mm");
                    }
                    subject += " - Số seri (Seri No): " + hsgd_ctu.SoSeri;
                    htmlBody = htmlBody.Replace("[TEN_CHUXE]", hsgd_ctu.TenKhach);
                    htmlBody = htmlBody.Replace("[NGAY_TTHAT]", hsgd_ctu.NgayTthat != null ? Convert.ToDateTime(hsgd_ctu.NgayTthat).ToString("dd/MM/yyyy hh:mm") : "");


                    var user_login = _context.DmUsers.Where(x => x.Mail == email_login).FirstOrDefault();
                    var dv_cq = _context.DmDonvis.Where(x => x.MaDonvi == user_login.MaDonvi).FirstOrDefault();
                    var vpdd = (from vars in _context_pias.DmVars
                                where vars.MaDonvi == dv_cq.MaDvchuquan &&
                                    (vars.Bien == "DON_VI" || vars.Bien == "DIA_CHI")
                                select vars).ToList();
                    var vpdd_ten = "";
                    var vpdd_diachi = "";
                    foreach (var record in vpdd)
                    {
                        switch (record.Bien)
                        {
                            case "DON_VI":
                                vpdd_ten = record.GiaTri;
                                break;
                            case "DIA_CHI":
                                vpdd_diachi = record.GiaTri;
                                break;
                            default:
                                break;
                        }
                    }
                    htmlBody = htmlBody.Replace("[VPDD_TEN]", vpdd_ten);
                    htmlBody = htmlBody.Replace("[VPDD_DIACHI]", vpdd_diachi);
                    var user_gdv = _context.DmUsers.Where(x => x.Oid == (hsgd_ctu.NguoiXuly == "" ? hsgd_ctu.MaUser : Guid.Parse(hsgd_ctu.NguoiXuly))).FirstOrDefault();
                    if (user_gdv != null)
                    {
                        htmlBody = htmlBody.Replace("[GDV]", user_gdv.TenUser ?? "");
                        htmlBody = htmlBody.Replace("[GDV_SDT]", user_gdv.Dienthoai ?? "");
                    }
                    else
                    {
                        htmlBody = htmlBody.Replace("[GDV]", "");
                        htmlBody = htmlBody.Replace("[GDV_SDT]", "");
                    }

                    avHtml = AlternateView.CreateAlternateViewFromString(htmlBody, null/* TODO Change to default(_) if this is not a reference type */, MediaTypeNames.Text.Html);
                }
                #endregion

                MailAddress from = new MailAddress("baohiempvi@pvi.com.vn", "BAOHIEMPVI", System.Text.Encoding.UTF8);
                MailAddress to = new MailAddress(email_nhan);
                System.Net.Mail.MailMessage Mail = new System.Net.Mail.MailMessage(from, to);
                Mail.Subject = subject;
                Mail.SubjectEncoding = System.Text.Encoding.UTF8;

                if (avHtml != null)
                {
                    Mail.AlternateViews.Add(avHtml);
                }

                if (htmlBody != "")
                {
                    Mail.Body = htmlBody;
                }
                Mail.BodyEncoding = System.Text.Encoding.UTF8;
                Mail.IsBodyHtml = true;
                System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(strFileNameLocalPdf);
                Mail.Attachments.Add(attachment);
                SmtpClient SmtpServer = new SmtpClient();
                SmtpServer.Port = 25;
                SmtpServer.Host = "mailapp.pvi.com.vn";
                SmtpServer.EnableSsl = false;
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.Timeout = 15000;
                SmtpServer.Send(Mail);
                Mail.Dispose();
                SmtpServer.Dispose();
                _logger.Information("SendEmail_ThongBaoBT to " + email_nhan + ", pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " success");
            }
            catch (Exception ex)
            {
                _logger.Error("Lỗi SendEmail_ThongBaoBT to " + email_nhan + ", pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " error: " + ex.Message.ToString());
            }
        }
        public TTPrintPasc GetPrintPASC(decimal pr_key_hsbt_ct, decimal pr_key_hsgd_ctu, string email, int loai_dx)
        {
            TTPrintPasc pasc = new TTPrintPasc();
            var list_pasc_detail = new List<pasc_detail>();
            try
            {
                var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == pr_key_hsgd_ctu).Select(s => new
                {
                    PrKey = s.PrKey,
                    MaDonvi = s.MaDonvi,
                    MaDonvigd = s.MaDonvigd,
                    SoHsgd = s.SoHsgd,
                    TenKhach = s.TenKhach,
                    BienKsoat = s.BienKsoat,
                    SoSeri = s.SoSeri,
                    HsgdTpc = s.HsgdTpc,
                    MaTtrangGd = s.MaTtrangGd,
                    sNgayDau = s.NgayDauSeri != null ? Convert.ToDateTime(s.NgayDauSeri).ToString("dd/MM/yyyy") : "",
                    sNgayCuoi = s.NgayCuoiSeri != null ? Convert.ToDateTime(s.NgayCuoiSeri).ToString("dd/MM/yyyy") : "",
                    MaUser = s.MaUser,
                    sNgayTbao = s.NgayTbao != null ? Convert.ToDateTime(s.NgayTbao).ToString("dd/MM/yyyy") : "",
                    sNgayTthat = s.NgayTthat != null ? Convert.ToDateTime(s.NgayTthat).ToString("dd/MM/yyyy") : "",
                    NguyenNhanTtat = s.NguyenNhanTtat,
                    TitleNgayduyet = s.HsgdTpc == 1 ? "Hồ sơ đã được TCT phê duyệt:" : "Hồ sơ được phê duyệt ngày:",
                    sNgayDuyet = s.HsgdTpc == 1 ? "" : (s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ToString("dd/MM/yyyy") : ""),
                    DienThoai = s.DienThoai,
                    NgayDauSeri = s.NgayDauSeri,
                    NgayTthat = s.NgayTthat,
                    PrKeyBt = s.PrKeyBt,
                    SoDonbh = s.SoDonbh,
                    NguoiXuly = s.NguoiXuly
                }).FirstOrDefault();
                if (hsgd_ctu != null)
                {
                    var ten_donvi = _context.DmDonvis.Where(x => x.MaDonvi == hsgd_ctu.MaDonvi).Select(s => s.TenDonvi).FirstOrDefault();
                    var ma_donvi_user = _context.DmUsers.Where(x => x.Mail == email).Select(s => s.MaDonvi).FirstOrDefault();
                    pasc.LBL_VP = "VĂN PHÒNG ĐẠI DIỆN CSKH";
                    pasc.LBL_DEXUAT = "ĐỀ XUẤT PHƯƠNG ÁN SỬA CHỮA";
                    pasc.LABEL_TRACHNHIEMPVI = "Số tiền thuộc trách nhiệm bảo hiểm (Gồm VAT)";
                    pasc.LBL_NG_KY = "Lãnh đạo VPCSKH";
                    pasc.LBL_PHONGGQ = "Phòng GQKN XCG";
                    pasc.LBL_GDV = "CB GQKN XCG";
                    pasc.SO_HSGD = hsgd_ctu.SoHsgd;
                    pasc.TEN_KHACH = hsgd_ctu.TenKhach;
                    pasc.DIEN_THOAI = hsgd_ctu.DienThoai;
                    pasc.BIEN_KSOAT = hsgd_ctu.BienKsoat;
                    pasc.NGAY_DAU = hsgd_ctu.sNgayDau;
                    pasc.NGAY_CUOI = hsgd_ctu.sNgayCuoi;
                    pasc.SO_SERI = hsgd_ctu.SoSeri.ToString();
                    pasc.NGAY_TTHAT = hsgd_ctu.sNgayTthat;
                    pasc.NGAY_TBAO = hsgd_ctu.sNgayTbao;
                    pasc.NGUYEN_NHANTT = hsgd_ctu.NguyenNhanTtat.ToString();
                    var hsgd_dx_ct = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCt == pr_key_hsbt_ct).Select(s => new
                    {
                        PrKey = s.PrKey,
                        HieuXe = s.HieuXe,
                        LoaiXe = s.LoaiXe,
                        XuatXu = s.XuatXu,
                        NamSx = s.NamSx,
                        MaGara = s.MaGara,
                        MaGara01 = s.MaGara01,
                        MaGara02 = s.MaGara02,
                        SoTienctkh = s.SoTienctkh,
                        SoTienGtbt = s.SoTienGtbt,
                        Vat = s.Vat,
                        TyleggPhutungvcx = s.TyleggPhutungvcx,
                        TyleggSuachuavcx = s.TyleggSuachuavcx,
                        LydoCtkh = s.LydoCtkh,
                        DoituongttTnds = s.DoituongttTnds
                    }).FirstOrDefault();
                    if (hsgd_dx_ct != null)
                    {

                        var hieu_xe = _context.DmHieuxes.Where(x => x.PrKey == hsgd_dx_ct.HieuXe).Select(s => s.HieuXe).FirstOrDefault();
                        pasc.HIEU_XE = hieu_xe != null ? hieu_xe : "";
                        var loai_xe = _context.DmLoaixes.Where(x => x.PrKey == hsgd_dx_ct.LoaiXe).Select(s => s.LoaiXe).FirstOrDefault();
                        pasc.LOAI_XE = loai_xe != null ? loai_xe : "";
                        pasc.XUAT_XU = hsgd_dx_ct.XuatXu ?? "";
                        pasc.NAM_SX = hsgd_dx_ct.NamSx.ToString();
                        var ten_gara = _context.DmGaRas.Where(x => x.MaGara == hsgd_dx_ct.MaGara).Select(s => s.TenGara + (s.DiaChi != "" ? " - " + s.DiaChi : "")).FirstOrDefault();
                        pasc.TEN_GARA = ten_gara != null ? ten_gara : "";
                        pasc.TEN_GARA01 = hsgd_dx_ct.MaGara01 ?? "";
                        pasc.TEN_GARA02 = hsgd_dx_ct.MaGara02 ?? "";
                        pasc.LYDO_CTKH = hsgd_dx_ct.LydoCtkh.ToString();
                        pasc.DOITUONGTT_TNDS = hsgd_dx_ct.DoituongttTnds.ToString();
                        if (loai_dx == 0 || loai_dx == 1)
                        {
                            list_pasc_detail = ToListWithNoLock((from a in _context.HsgdDxes
                                                                 join b in _context.DmHmucs on a.MaHmuc equals b.MaHmuc into b1
                                                                 from b in b1.DefaultIfEmpty()
                                                                 where a.PrKeyDx == hsgd_dx_ct.PrKey
                                                                 select new pasc_detail
                                                                 {
                                                                     pr_key_dx = a.PrKey,
                                                                     ma_hmuc = a.MaHmuc,
                                                                     ten_hmuc = b != null ? (b.TenHmuc ?? "") : a.Hmuc,
                                                                     so_tientt = a.SoTientt,
                                                                     so_tienph = a.SoTienph,
                                                                     so_tienson = a.SoTienson,
                                                                     vat_sc = a.VatSc,
                                                                     giam_tru_bt = a.GiamTruBt,
                                                                     so_tiendoitru = (a.SoTienDoitru ?? 0),
                                                                     thu_hoi_ts = a.ThuHoiTs,
                                                                     vat_so_tientt = a.SoTientt * ((decimal)a.VatSc / 100),
                                                                     vat_so_tienph = a.SoTienph * ((decimal)a.VatSc / 100),
                                                                     vatso_tienson = a.SoTienson * ((decimal)a.VatSc / 100),
                                                                     so_tientt_gomVAT = a.SoTientt + a.SoTientt * ((decimal)a.VatSc / 100),
                                                                     so_tienph_gomVAT = a.SoTienph + a.SoTienph * ((decimal)a.VatSc / 100),
                                                                     so_tienson_gomVAT = a.SoTienson + a.SoTienson * ((decimal)a.VatSc / 100),
                                                                     ghi_chudv = a.GhiChudv,
                                                                     so_tien_vat = (a.SoTientt + a.SoTienph + a.SoTienson) * ((decimal)a.VatSc / 100),
                                                                     sum_tt_ph_son_gomVAT = ((a.SoTientt + a.SoTienph + a.SoTienson) + (a.SoTientt + a.SoTienph + a.SoTienson) * ((decimal)a.VatSc / 100)),//sum_tt_ph_son_gomVAT
                                                                     sum_giamtru_bt = ((((a.SoTientt + a.SoTientt * ((decimal)a.VatSc / 100)) - ((a.SoTientt + a.SoTientt * ((decimal)a.VatSc / 100)) * (hsgd_dx_ct.TyleggPhutungvcx / 100)))
                                                                      + (((a.SoTienph + a.SoTienson) + (a.SoTienph + a.SoTienson) * ((decimal)a.VatSc / 100)) - ((a.SoTienph + a.SoTienson) + (a.SoTienph + a.SoTienson) * ((decimal)a.VatSc / 100)) * (hsgd_dx_ct.TyleggSuachuavcx / 100))) * a.GiamTruBt / 100),//sum_giamtru_bt
                                                                     sum_so_tienggsc = ((a.SoTientt + a.SoTientt * ((decimal)a.VatSc / 100)) * (hsgd_dx_ct.TyleggPhutungvcx / 100) + ((a.SoTienph + a.SoTienson) + (a.SoTienph + a.SoTienson) * ((decimal)a.VatSc / 100)) * (hsgd_dx_ct.TyleggSuachuavcx / 100)),//sum_so_tienggsc
                                                                                                                                                                                                                                                                                                                 //
                                                                 }).OrderBy(o => o.pr_key_dx).AsQueryable());
                        }
                        else
                        {
                            list_pasc_detail = ToListWithNoLock((from a in _context.HsgdDxTsks
                                                                 where a.PrKeyDx == hsgd_dx_ct.PrKey
                                                                 select new pasc_detail
                                                                 {
                                                                     pr_key_dx = a.PrKey,
                                                                     ten_hmuc = a.Hmuc,
                                                                     so_tientt = a.SoTientt,
                                                                     so_tiensc = a.SoTiensc,
                                                                     vat_sc = a.VatSc,
                                                                     giam_tru_bt = a.GiamTruBt,
                                                                     so_tiendoitru = 0,
                                                                     thu_hoi_ts = a.ThuHoiTs,
                                                                     vat_so_tientt = a.SoTientt * ((decimal)a.VatSc / 100),
                                                                     vat_so_tiensc = a.SoTiensc * ((decimal)a.VatSc / 100),
                                                                     so_tientt_gomVAT = a.SoTientt + a.SoTientt * ((decimal)a.VatSc / 100),
                                                                     so_tiensc_gomVAT = a.SoTiensc + a.SoTiensc * ((decimal)a.VatSc / 100),
                                                                     ghi_chudv = a.GhiChudv,
                                                                     so_tien_vat = (a.SoTientt + a.SoTiensc) * ((decimal)a.VatSc / 100),
                                                                     sum_tt_sc_gomVAT = ((a.SoTientt + a.SoTiensc) + (a.SoTientt + a.SoTiensc) * ((decimal)a.VatSc / 100)),//sum_tt_sc_gomVAT
                                                                     sum_giamtru_bt = (((a.SoTientt + a.SoTientt * ((decimal)a.VatSc / 100)) + (a.SoTiensc + a.SoTiensc * ((decimal)a.VatSc / 100))) * a.GiamTruBt / 100),//sum_giamtru_bt

                                                                 }).OrderBy(o => o.pr_key_dx).AsQueryable());
                        }
                        if (list_pasc_detail.Count > 0 && list_pasc_detail.Where(x => x.vat_sc > 0).Count() > 0)
                        {
                            if (hsgd_ctu.HsgdTpc == 1 && new[] { "00", "31", "32" }.Contains(ma_donvi_user))
                            {
                                pasc.LABEL_TRACHNHIEMPVI = "Số tiền thuộc trách nhiệm bảo hiểm (Gồm VAT)";
                            }
                            else
                            {
                                pasc.LABEL_TRACHNHIEMPVI = "Tổng chi phí còn lại (Gồm VAT)";
                            }
                            pasc.LABEL_TONGCHIPHI = "Tổng chi phí thay thế + sửa chữa (Gồm VAT)";
                        }
                        else
                        {
                            if (hsgd_ctu.HsgdTpc == 1 && new[] { "00", "31", "32" }.Contains(ma_donvi_user))
                            {
                                pasc.LABEL_TRACHNHIEMPVI = "Số tiền thuộc trách nhiệm bảo hiểm";
                            }
                            else
                            {
                                pasc.LABEL_TRACHNHIEMPVI = "Tổng chi phí còn lại";
                            }
                            pasc.LABEL_TONGCHIPHI = "Tổng chi phí thay thế + sửa chữa";
                        }
                        if (list_pasc_detail.Count > 0)
                        {
                            if (loai_dx == 0 || loai_dx == 1)
                            {
                                var sum_hsgd_dx = list_pasc_detail.GroupBy(g => 1 == 1)
                                .Select(s => new sum_hsgd_dx
                                {
                                    sumso_tien_tt_ph_son_gomVAT = s.Sum(x => x.sum_tt_ph_son_gomVAT),
                                    sumso_tien_giamtru_bt = s.Sum(x => x.sum_giamtru_bt),
                                    sumso_tien_so_tienggsc = s.Sum(x => x.sum_so_tienggsc),
                                    sumso_tien_doitru = s.Sum(x => x.so_tiendoitru)
                                }).FirstOrDefault();
                                if (sum_hsgd_dx != null)
                                {
                                    if (sum_hsgd_dx.sumso_tien_giamtru_bt == 0)
                                    {
                                        sum_hsgd_dx.sumso_tien_giamtru_bt = hsgd_dx_ct.SoTienGtbt;
                                    }
                                    sum_hsgd_dx.sum_trachnhienpvi = Math.Round(sum_hsgd_dx.sumso_tien_tt_ph_son_gomVAT - sum_hsgd_dx.sumso_tien_so_tienggsc - sum_hsgd_dx.sumso_tien_giamtru_bt - hsgd_dx_ct.SoTienctkh - sum_hsgd_dx.sumso_tien_doitru, 0);

                                    pasc.SUMSO_TIEN_TT_PH_SON_GOMVAT = sum_hsgd_dx.sumso_tien_tt_ph_son_gomVAT;
                                    pasc.SUMSO_TIENGGSC = sum_hsgd_dx.sumso_tien_so_tienggsc;
                                    pasc.SO_TIENCTKH = hsgd_dx_ct.SoTienctkh;
                                    pasc.SO_TIENGIAMTRUBT = sum_hsgd_dx.sumso_tien_giamtru_bt;
                                    pasc.SO_TIENDOITRUBT = sum_hsgd_dx.sumso_tien_doitru;
                                    pasc.SUM_TRACHNHIEMPVI = sum_hsgd_dx.sum_trachnhienpvi;
                                    pasc.SUM_TRACHNHIEMPVI_BC = ContentHelper.NumberToText((double)sum_hsgd_dx.sum_trachnhienpvi);
                                }
                            }
                            else
                            {
                                var sum_hsgd_dx_tsk = list_pasc_detail.GroupBy(g => 1 == 1)
                                .Select(s => new sum_hsgd_dx_tsk
                                {
                                    sumso_tien_tt_sc_gomVAT = s.Sum(x => x.sum_tt_sc_gomVAT),
                                    sumso_tien_giamtru_bt = s.Sum(x => x.sum_giamtru_bt)
                                }).FirstOrDefault();
                                if (sum_hsgd_dx_tsk != null)
                                {
                                    if (sum_hsgd_dx_tsk.sumso_tien_giamtru_bt == 0)
                                    {
                                        sum_hsgd_dx_tsk.sumso_tien_giamtru_bt = hsgd_dx_ct.SoTienGtbt;
                                    }
                                    sum_hsgd_dx_tsk.sum_trachnhienpvi = Math.Round(sum_hsgd_dx_tsk.sumso_tien_tt_sc_gomVAT - sum_hsgd_dx_tsk.sumso_tien_giamtru_bt - hsgd_dx_ct.SoTienctkh, 0);

                                    pasc.SUMSO_TIEN_TT_PH_SON_GOMVAT = sum_hsgd_dx_tsk.sumso_tien_tt_sc_gomVAT;
                                    pasc.SO_TIENCTKH = hsgd_dx_ct.SoTienctkh;
                                    pasc.SO_TIENGIAMTRUBT = sum_hsgd_dx_tsk.sumso_tien_giamtru_bt;
                                    pasc.SO_TIENDOITRUBT = 0;
                                    pasc.SUM_TRACHNHIEMPVI = sum_hsgd_dx_tsk.sum_trachnhienpvi;
                                    pasc.SUM_TRACHNHIEMPVI_BC = ContentHelper.NumberToText((double)sum_hsgd_dx_tsk.sum_trachnhienpvi);
                                }
                            }
                        }
                        else
                        {
                            pasc.SUMSO_TIEN_TT_PH_SON_GOMVAT = 0;
                            pasc.SUMSO_TIENGGSC = 0;
                            pasc.SO_TIENCTKH = hsgd_dx_ct.SoTienctkh;
                            pasc.SO_TIENGIAMTRUBT = 0;
                            pasc.SO_TIENDOITRUBT = 0;
                            pasc.SUM_TRACHNHIEMPVI = 0;
                            pasc.SUM_TRACHNHIEMPVI_BC = ContentHelper.NumberToText(0);
                        }
                    }
                    #region lấy dữ liệu lịch sử tổn thất
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    var tu_ngay = DateTime.ParseExact(Convert.ToDateTime(hsgd_ctu.NgayDauSeri).ToString("dd/MM/yyyy 00:00:00"), "dd/MM/yyyy HH:mm:ss", provider);
                    var den_ngay = DateTime.ParseExact(Convert.ToDateTime(hsgd_ctu.NgayTthat).ToString("dd/MM/yyyy 00:00:00"), "dd/MM/yyyy HH:mm:ss", provider);
                    var uoc1 = (from a in _context_pias_update.HsbtCtus
                                join b in _context_pias_update.HsbtCts on a.PrKey equals b.FrKey
                                join c in _context_pias_update.HsbtUocs on b.PrKey equals c.FrKey
                                where b.MaSp.StartsWith("05") && new[] { "01", "02", "05" }.Contains(b.MaTtrangBt) && new[] { "TBT", "NBT" }.Contains(a.MaLhsbt) && c.NgayPs <= den_ngay && a.PrKey != hsgd_ctu.PrKeyBt && a.SoHdgcn == hsgd_ctu.SoDonbh && a.SoSeri == hsgd_ctu.SoSeri
                                select new
                                {
                                    SoHsbt = a.SoHsbt,
                                    SoTienp = b.SoTienp
                                }).AsQueryable();
                    var uoc1_gr = ToListWithNoLock(uoc1.GroupBy(n => new { n.SoHsbt }).Select(p => new LichSuBT
                    {
                        LoaiHs = "UOC",
                        SoHsbt = p.Key.SoHsbt,
                        SoTienp = p.Sum(x => x.SoTienp)
                    }).AsQueryable());
                    var uoc2 = (from a in _context_pias_update.HsbtCtus
                                join b in _context_pias_update.HsbtCts on a.PrKey equals b.FrKey
                                join c in _context_pias_update.HsbtUocs on b.PrKey equals c.FrKey
                                where b.MaSp.StartsWith("05") && new[] { "03", "04" }.Contains(b.MaTtrangBt) && new[] { "TBT", "NBT" }.Contains(a.MaLhsbt) && c.NgayPs <= den_ngay && b.NgayHtoanBt > den_ngay && a.PrKey != hsgd_ctu.PrKeyBt && a.SoHdgcn == hsgd_ctu.SoDonbh && a.SoSeri == hsgd_ctu.SoSeri
                                select new
                                {
                                    SoHsbt = a.SoHsbt,
                                    SoTienp = c.SoTienbt
                                }).AsQueryable();
                    var uoc2_gr = ToListWithNoLock(uoc2.GroupBy(n => new { n.SoHsbt }).Select(p => new LichSuBT
                    {
                        LoaiHs = "UOC",
                        SoHsbt = p.Key.SoHsbt,
                        SoTienp = p.Sum(x => x.SoTienp)
                    }).AsQueryable());
                    var uoc3 = ToListWithNoLock((from a in _context_pias_update.HsbtCtus
                                                 join b in _context_pias_update.HsbtCts on a.PrKey equals b.FrKey
                                                 where b.MaSp.StartsWith("05") && b.MaTtrangBt == "03" && new[] { "TBT", "NBT" }.Contains(a.MaLhsbt) && b.NgayHtoanBt >= tu_ngay && b.NgayHtoanBt <= den_ngay && a.PrKey != hsgd_ctu.PrKeyBt && a.SoHdgcn == hsgd_ctu.SoDonbh && a.SoSeri == hsgd_ctu.SoSeri
                                                 select new LichSuBT
                                                 {
                                                     LoaiHs = "PT",
                                                     SoHsbt = a.SoHsbt,
                                                     SoTienp = b.SoTienp
                                                 }).AsQueryable());
                    var lsbt = uoc1_gr.Union(uoc2_gr).Union(uoc3).ToList();
                    var lsbt_gr = lsbt.GroupBy(g => new { g.LoaiHs, g.SoHsbt }).Select(p => new LichSuBT
                    {
                        LoaiHs = p.Key.LoaiHs,
                        SoHsbt = p.Key.SoHsbt,
                        SoTienp = p.Sum(x => x.SoTienp)
                    }).ToList();
                    var nguyen_te_ubt = lsbt_gr.Where(x => x.LoaiHs == "UOC").Select(t => t.SoTienp).Sum();
                    var so_lan_ubt = lsbt_gr.Where(x => x.LoaiHs == "UOC").Count();
                    var nguyen_te_bt = lsbt_gr.Where(x => x.LoaiHs == "PT").Select(t => t.SoTienp).Sum();
                    var so_lan_bt = lsbt_gr.Where(x => x.LoaiHs == "PT").Count();
                    pasc.SV_CBT = so_lan_ubt;
                    pasc.SOTIEN_UBT = nguyen_te_ubt;
                    pasc.SV_BT = so_lan_bt;
                    pasc.SOTIEN_BT = nguyen_te_bt;
                    #endregion
                    #region user duyệt, ký
                    var pr_key_nky_duyet = _context.NhatKies.Where(x => x.MaTtrangGd == "6" && x.FrKey == hsgd_ctu.PrKey).GroupBy(c => 1 == 1)
                    .Select(p => p.Max(g => g.PrKey)).FirstOrDefault();
                    var oid_user_duyet = _context.NhatKies.Where(x => x.PrKey == pr_key_nky_duyet).Select(s => s.MaUser).FirstOrDefault();
                    var user_duyet = _context.DmUsers.Where(x => x.Oid == oid_user_duyet).FirstOrDefault();
                    if (user_duyet != null)
                    {
                        pasc.MAUSER_DUYET = user_duyet.MaUser ?? "";
                        pasc.TENUSER_DUYET = user_duyet.TenUser ?? "";
                    }
                    else
                    {
                        pasc.MAUSER_DUYET = "";
                        pasc.TENUSER_DUYET = "";
                    }
                    var pr_key_nky_cchopd = _context.NhatKies.Where(x => x.MaTtrangGd == "10" && x.FrKey == hsgd_ctu.PrKey).GroupBy(c => 1 == 1)
                    .Select(p => p.Max(g => g.PrKey)).FirstOrDefault();
                    var oid_user_cchopd = _context.NhatKies.Where(x => x.PrKey == pr_key_nky_cchopd).Select(s => s.MaUser).FirstOrDefault();
                    var user_cchopd = _context.DmUsers.Where(x => x.Oid == oid_user_duyet).FirstOrDefault();
                    if (user_cchopd != null)
                    {
                        pasc.MAUSER_CCHOPD = user_cchopd.MaUser ?? "";
                        pasc.TENUSER_CCHOPD = user_cchopd.TenUser ?? "";
                    }
                    else
                    {
                        pasc.MAUSER_CCHOPD = "";
                        pasc.TENUSER_CCHOPD = "";
                    }
                    var user_gdv = _context.DmUsers.Where(x => x.Oid == (hsgd_ctu.NguoiXuly == "" ? hsgd_ctu.MaUser : Guid.Parse(hsgd_ctu.NguoiXuly))).FirstOrDefault();
                    if (user_gdv != null)
                    {
                        pasc.MAUSER_GDV = user_gdv.MaUser ?? "";
                        pasc.TENUSER_GDV = user_gdv.TenUser ?? "";
                    }
                    else
                    {
                        pasc.MAUSER_GDV = "";
                        pasc.TENUSER_GDV = "";
                    }
                    #endregion

                }

                pasc.list_pasc_detail = list_pasc_detail;

            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
            return pasc;
        }
        public CombinedPASCResult PrintPASC(decimal pr_key_hsbt_ct, decimal pr_key_hsgd_ctu, string email, int loai_dx)
        {
            try
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
                var wordPdfRequest = new WordToPdfRequest();
                wordPdfRequest.ListData = new List<EntityContent>();
                UpdateProperties update = new UpdateProperties();

                var pasc = GetPrintPASC(pr_key_hsbt_ct, pr_key_hsgd_ctu, email, loai_dx);
                update.AddEntityContent(wordPdfRequest, "[LBL_VP]", pasc.LBL_VP);
                update.AddEntityContent(wordPdfRequest, "[LBL_DEXUAT]", pasc.LBL_DEXUAT);
                update.AddEntityContent(wordPdfRequest, "[LABEL_TRACHNHIEMPVI]", pasc.LABEL_TRACHNHIEMPVI);
                update.AddEntityContent(wordPdfRequest, "[LBL_NG_KY]", pasc.LBL_NG_KY);
                update.AddEntityContent(wordPdfRequest, "[LBL_PHONGGQ]", pasc.LBL_PHONGGQ);
                update.AddEntityContent(wordPdfRequest, "[LBL_GDV]", pasc.LBL_GDV);
                update.AddEntityContent(wordPdfRequest, "[SO_HSGD]", pasc.SO_HSGD);
                update.AddEntityContent(wordPdfRequest, "[TEN_KHACH]", pasc.TEN_KHACH);
                update.AddEntityContent(wordPdfRequest, "[DIEN_THOAI]", pasc.DIEN_THOAI);
                update.AddEntityContent(wordPdfRequest, "[BIEN_KSOAT]", pasc.BIEN_KSOAT);
                update.AddEntityContent(wordPdfRequest, "[NGAY_DAU]", pasc.NGAY_DAU);
                update.AddEntityContent(wordPdfRequest, "[NGAY_CUOI]", pasc.NGAY_CUOI);
                update.AddEntityContent(wordPdfRequest, "[SO_SERI]", pasc.SO_SERI);
                update.AddEntityContent(wordPdfRequest, "[NGAY_TTHAT]", pasc.NGAY_TTHAT);
                update.AddEntityContent(wordPdfRequest, "[NGAY_TBAO]", pasc.NGAY_TBAO);
                List<string> list_NguyenNhanTtat = ContentHelper.SplitString(pasc.NGUYEN_NHANTT, 255);
                for (int i = 0; i < list_NguyenNhanTtat.Count(); i++)
                {
                    update.AddEntityContent(wordPdfRequest, $"[NGUYEN_NHANTT{i}]", list_NguyenNhanTtat[i]);
                }
                for (int i = list_NguyenNhanTtat.Count(); i < 2; i++)
                {
                    update.AddEntityContent(wordPdfRequest, $"[NGUYEN_NHANTT{i}]", "");
                }
                update.AddEntityContent(wordPdfRequest, "[HIEU_XE]", pasc.HIEU_XE);
                update.AddEntityContent(wordPdfRequest, "[LOAI_XE]", pasc.LOAI_XE);
                update.AddEntityContent(wordPdfRequest, "[XUAT_XU]", pasc.XUAT_XU);
                update.AddEntityContent(wordPdfRequest, "[NAM_SX]", pasc.NAM_SX);
                update.AddEntityContent(wordPdfRequest, "[TEN_GARA]", pasc.TEN_GARA);
                update.AddEntityContent(wordPdfRequest, "[TEN_GARA01]", pasc.TEN_GARA01);
                update.AddEntityContent(wordPdfRequest, "[TEN_GARA02]", pasc.TEN_GARA02);
                List<string> list_LydoCtkh = ContentHelper.SplitString(pasc.LYDO_CTKH, 255);
                for (int i = 0; i < list_LydoCtkh.Count(); i++)
                {
                    update.AddEntityContent(wordPdfRequest, $"[LYDO_CTKH{i}]", list_LydoCtkh[i]);
                }
                for (int i = list_LydoCtkh.Count(); i < 11; i++)
                {
                    update.AddEntityContent(wordPdfRequest, $"[LYDO_CTKH{i}]", "");
                }
                List<string> list_DoituongttTnds = ContentHelper.SplitString(pasc.DOITUONGTT_TNDS, 255);
                for (int i = 0; i < list_DoituongttTnds.Count(); i++)
                {
                    update.AddEntityContent(wordPdfRequest, $"[DOITUONGTT_TNDS{i}]", list_DoituongttTnds[i]);
                }
                for (int i = list_DoituongttTnds.Count(); i < 11; i++)
                {
                    update.AddEntityContent(wordPdfRequest, $"[DOITUONGTT_TNDS{i}]", "");
                }
                update.AddEntityContent(wordPdfRequest, "[LABEL_TRACHNHIEMPVI]", pasc.LABEL_TRACHNHIEMPVI);
                update.AddEntityContent(wordPdfRequest, "[LABEL_TONGCHIPHI]", pasc.LABEL_TONGCHIPHI);

                update.AddEntityContent(wordPdfRequest, "[SUMSO_TIEN_TT_PH_SON_GOMVAT]", pasc.SUMSO_TIEN_TT_PH_SON_GOMVAT.ToString("#,###", cul.NumberFormat));
                update.AddEntityContent(wordPdfRequest, "[SUMSO_TIENGGSC]", pasc.SUMSO_TIENGGSC.ToString("#,###", cul.NumberFormat));
                update.AddEntityContent(wordPdfRequest, "[SO_TIENCTKH]", pasc.SO_TIENCTKH.ToString("#,###", cul.NumberFormat));
                update.AddEntityContent(wordPdfRequest, "[SO_TIENGIAMTRUBT]", pasc.SO_TIENGIAMTRUBT.ToString("#,###", cul.NumberFormat));
                update.AddEntityContent(wordPdfRequest, "[SO_TIENDOITRUBT]", pasc.SO_TIENDOITRUBT.ToString("#,###", cul.NumberFormat));
                update.AddEntityContent(wordPdfRequest, "[SUM_TRACHNHIEMPVI]", pasc.SUM_TRACHNHIEMPVI.ToString("#,###", cul.NumberFormat));
                update.AddEntityContent(wordPdfRequest, "[SUM_TRACHNHIEMPVI_BC]", pasc.SUM_TRACHNHIEMPVI_BC);

                update.AddEntityContent(wordPdfRequest, "[SV_CBT]", pasc.SV_BT.ToString());
                update.AddEntityContent(wordPdfRequest, "[SOTIEN_UBT]", pasc.SOTIEN_UBT.ToString("#,###", cul.NumberFormat));
                update.AddEntityContent(wordPdfRequest, "[SV_BT]", pasc.SV_BT.ToString());
                update.AddEntityContent(wordPdfRequest, "[SOTIEN_BT]", pasc.SOTIEN_BT.ToString("#,###", cul.NumberFormat));

                update.AddEntityContent(wordPdfRequest, "[MAUSER_DUYET]", pasc.MAUSER_DUYET);
                update.AddEntityContent(wordPdfRequest, "[TENUSER_DUYET]", pasc.TENUSER_DUYET);

                update.AddEntityContent(wordPdfRequest, "[MAUSER_CCHOPD]", pasc.MAUSER_CCHOPD);
                update.AddEntityContent(wordPdfRequest, "[TENUSER_CCHOPD]", pasc.TENUSER_CCHOPD);

                update.AddEntityContent(wordPdfRequest, "[MAUSER_GDV]", pasc.MAUSER_GDV);
                update.AddEntityContent(wordPdfRequest, "[TENUSER_GDV]", pasc.TENUSER_GDV);
                var listData = wordPdfRequest.ListData;
                var listNew = new CombinedPASCResult
                {
                    ThirdQueryResults = listData,
                    ListPascDetail = pasc.list_pasc_detail
                };

                return listNew;

            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                return null;
            }
        }
        public async Task<ServiceResult> PheDuyetTBBT(int pr_key, string currentUserEmail)
        {
            try
            {
                DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
                HsgdCtu hoSoGiamDinh = _context.HsgdCtus.Where(x => x.PrKey == pr_key).FirstOrDefault();
                HsgdTbbt hsgd_tbbt = _context.HsgdTbbts.Where(x => x.PrKeyHsgd == pr_key).FirstOrDefault();
                //Nếu chưa có thì thêm mới                
                if (currentUser != null && hoSoGiamDinh != null)
                {

                    int[] acceptedUserTypes = new int[] { 1, 6, 9, 10, 11 };
                    List<PquyenCnang> list_phanquyen = Check_PquyenCnang(currentUser);

                    // Kiểm tra phân quyền của User
                    if (Array.Exists(acceptedUserTypes, x => x == currentUser.LoaiUser)
                        || check_UyQuyen_HoSoTPC(currentUser).Equals("PHEDUYET_HS")
                        || check_UyQuyen_HoSoTPC(currentUser).Equals("FULL_QUYEN")
                        || (list_phanquyen.Count > 0 && list_phanquyen.Exists(x => x.LoaiQuyen.Equals("BAOLANHDT"))))
                    {
                        // chưa được phê duyệt thì mới duyệt được 
                        if (hsgd_tbbt.PdTbbt == 1 && currentUser.MaDonvi=="31")
                        {
                            return new ServiceResult
                            {
                                Success = false,
                                Message = "Thông báo bồi thường này đã được duyệt rồi, vui lòng kiểm tra lại nhật ký"
                            };
                        }                            
                            hsgd_tbbt.PdTbbt = 1;
                            _context.HsgdTbbts.Update(hsgd_tbbt);
                            //Tạo nhật ký 
                            NhatKy diary = new NhatKy
                            {
                                FrKey = hoSoGiamDinh.PrKey,
                                MaTtrangGd = "DTBBT",
                                TenTtrangGd = "Duyệt TB bồi thường",
                                GhiChu = "Duyệt Thông báo bồi thường",
                                NgayCapnhat = DateTime.Now,
                                MaUser = currentUser.Oid
                            };

                            await _context.NhatKies.AddAsync(diary);
                            await _context.SaveChangesAsync();
                            return new ServiceResult
                            {
                                Success = true,
                                Message = "Phê duyệt Thông báo bồi thường thành công",
                                Data = hoSoGiamDinh.PrKey.ToString()
                            };                      
                    }
                    else
                    {
                        return new ServiceResult
                        {
                            Success = false,
                            Message = "Người dùng không có quyền duyệt Thông báo bồi thường"
                        };
                    }
                }
                else
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Hồ sơ không tồn tại"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Error("dbContextTransaction Exception when PheDuyetTBBT: " + ex.ToString());
                _logger.Error("Error record: " + JsonConvert.SerializeObject(pr_key));
                _context.Dispose();

                return new ServiceResult
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi phê duyệt, vui lòng liên hệ IT"
                };
            }
        }
        public async Task<string> UpdatePathTBBT(decimal pr_key_hsgd_ctu, string path_file)
        {
            string result = "";
            try
            {
                HsgdTbbt hsgd_tbbt = _context.HsgdTbbts.Where(x => x.PrKeyHsgd == pr_key_hsgd_ctu).FirstOrDefault();
                hsgd_tbbt.PathTbbt = path_file;
                _context.HsgdTbbts.Update(hsgd_tbbt);
                await _context.SaveChangesAsync();
                return hsgd_tbbt.PrKey.ToString();              

            }
            catch (Exception ex)
            {
                _logger.Error("UpdatePathTBBT lỗi: " + ex.ToString());
            }
            return result;
        }
        public async Task<bool> KyTBBT(decimal pr_key_hsgd_ctu,string file_path, string email, string SignContent)
        {
            bool result = false;
            try
            {
                var user_login = await _context.DmUsers.Where(x => x.Mail == email).FirstOrDefaultAsync();
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
                    
                    //haipv1 13/11/2025 bỏ đoạn này theo yêu cầu: 73470 Bỏ  tính năng tự động thêm Thông Báo Bồi Thường và Thư Bảo Lãnh  vào hồ sơ thanh toán
                    //try
                    //{
                    //    if (result)
                    //    {
                    //        var attachFilesToDelete = await _context.HsgdAttachFiles
                    //    .Where(b => b.MaCtu == "TBBT" && b.FrKey == pr_key_hsgd_ctu)
                    //    .ToListAsync();
                    //        // Nếu có dữ liệu thì xóa
                    //        if (attachFilesToDelete.Any())
                    //        {
                    //            _context.HsgdAttachFiles.RemoveRange(attachFilesToDelete);
                    //            await _context.SaveChangesAsync();
                    //        }
                    //        List<HsgdAttachFile> attachFiles = new List<HsgdAttachFile>();
                    //        var atf = new HsgdAttachFile
                    //        {
                    //            PrKey = Guid.NewGuid().ToString().ToLower(),
                    //            FrKey = pr_key_hsgd_ctu,
                    //            MaCtu = "TBBT",
                    //            FileName = "TBBT.pdf",
                    //            Directory = file_path,
                    //            ngay_cnhat = DateTime.Now,
                    //            GhiChu = "Cập nhật từ ký Thông báo bồi thường",
                    //            NguonTao = "WebPvi247"
                    //        };
                    //        attachFiles.Add(atf);
                    //        // Add vào context
                    //        _context.HsgdAttachFiles.AddRange(attachFiles);
                    //        await _context.SaveChangesAsync();
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    _logger.Error($"Ký xong thêm Thông báo bồi thường vào bảng hồ sơ lỗi pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " error occurred: " + ex);

                    //}
                }
                else
                {
                    _logger.Error($"KyTBBT call pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " chưa phân quyền ký trong bảng hddt_hsm ");
                }

            }
            catch (Exception ex)
            {
                _logger.Error($"KyTBBT call pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " error occurred: " + ex);
                result = false;
            }
            return result;
        }
        public List<PquyenCnang> Check_PquyenCnang(DmUser currentUser)
        {
            try
            {
                List<PquyenCnang> list_pquyen = _context.PquyenCnangs.Where(x => x.MaUser.Equals(currentUser.MaUser)).ToList();
                return list_pquyen;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return new List<PquyenCnang>();
            }
        }
        public string check_UyQuyen_HoSoTPC(DmUser currentUser)
        {
            try
            {
                string formattedOid = currentUser.Oid.ToString().ToLower();

                List<DmUqHstpc> list_uyquyen = _context.DmUqHstpcs.Where(x => (x.MaUserUq.ToLower().Equals(formattedOid))).OrderByDescending(x => x.PrKey).ToList();

                if (list_uyquyen != null && list_uyquyen.Count > 0)
                {
                    // Kiểm tra uỷ quyền của người dùng
                    bool quyenPheDuyet = false;
                    bool quyenChuyenCho = false;

                    if (list_uyquyen.Exists(x => x.LoaiUyquyen.Equals("6")))
                    {
                        quyenPheDuyet = true;
                    }

                    if (list_uyquyen.Exists(x => x.LoaiUyquyen.Equals("10")))
                    {
                        quyenChuyenCho = true;
                    }

                    if (quyenChuyenCho && quyenPheDuyet)
                    {
                        return "FULL_QUYEN";
                    }
                    else if (quyenPheDuyet)
                    {
                        return "PHEDUYET_HS";
                    }
                    else if (quyenChuyenCho)
                    {
                        return "CHUYENCHO_PD";
                    }
                    else
                    {
                        return "";
                    }

                }
                else
                {
                    return "";
                }

            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return "";
            }
        }
        public async Task<string> GetThongTinKyDienTuTBBT(decimal pr_key_hsgd_ctu, string currentUserEmail)
        {
            string SignContent = "";
            var user_login = await _context.DmUsers
        .Where(x => x.Mail == currentUserEmail)
        .FirstOrDefaultAsync();
            if (user_login != null)
            {
                var dm_var = await _context_pias.DmVars
            .Where(vars => vars.MaDonvi == user_login.MaDonvi && vars.Bien == "DON_VI")
            .FirstOrDefaultAsync();
                if (dm_var != null)
                {
                    SignContent = "Ký bởi: " + dm_var.GiaTri;
                }
                var nhat_ky_duyet = await _context.NhatKies
           .Where(x => x.FrKey == pr_key_hsgd_ctu && x.MaTtrangGd == "DTBBT")
           .OrderByDescending(o => o.PrKey)
           .FirstOrDefaultAsync(); 
                if (nhat_ky_duyet != null)
                {
                    SignContent += "\n" + "Ngày ký: " + Convert.ToDateTime(nhat_ky_duyet.NgayCapnhat).ToString("dd/MM/yyyy HH:mm:ss");
                }
            }
            return SignContent;
        }
        public string CheckTrungHsbt(decimal pr_key_hsgd_ctu, string ma_sp)
        {
            string result = "";
            try
            {
                if (ma_sp.Contains("0501"))
                {
                    var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == pr_key_hsgd_ctu).FirstOrDefault();
                    if (hsgd_ctu != null)
                    {
                        var hstrung = (from a in _context_pias_update.HsbtCtus
                                       join b in _context_pias_update.HsbtCts on a.PrKey equals b.FrKey
                                       where a.PrKey != hsgd_ctu.PrKeyBt && a.MaDonvi == hsgd_ctu.MaDonvi && b.MaSp == ma_sp && a.MaLhsbt == hsgd_ctu.MaLhsbt && a.SoHdgcn == hsgd_ctu.SoDonbh && a.SoSeri == hsgd_ctu.SoSeri && a.NgayTthat.Value.Date == hsgd_ctu.NgayTthat.Value.Date
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
        public List<string> Lay_mst_donvicapdon(decimal pr_key_hsgd_ctu)
        {
            List<string> listMasoVat;
            try
            {

                var ctuInfo = _context.HsgdCtus.AsNoTracking().Where(x => x.PrKey == int.Parse(pr_key_hsgd_ctu.ToString())).Select(x => new {
                                                     x.MaDonvi,
                                                     x.MaDonviTt
                                                 })
                                                 .FirstOrDefault();
                string maDonvi = ctuInfo.MaDonvi;
                string maDonviTt = ctuInfo.MaDonviTt;

                listMasoVat = _context_pias.DmKhaches.AsNoTracking()                                 
                                 .Where(x =>
                                     x.MaDonvi == "00" &&
                                     x.MaKh.StartsWith("00.18") &&
                                     new[] { maDonvi, maDonviTt}.Contains(x.MaDonviPban)
                                 )
                                 .Select(x => x.MasoVat)
                                 .ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
            return listMasoVat;
        }

    }
}