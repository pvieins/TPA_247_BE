using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PVI.DAO.Entities.Models;
using PVI.Helper;
using PVI.Repository.Interfaces;
using RestSharp;
using System.Data;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.ServiceModel.Channels;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PVI.Repository.Repositories
{
    public class HsgdTtrinhRepository : GenericRepository<HsgdTtrinh>, IHsgdTtrinhRepository
    {
        HsgdDxRepository _dx_repo;
        public HsgdTtrinhRepository(GdttContext context, Pvs2024Context context_pias, Pvs2024UpdateContext context_pias_update, PvsTcdContext context_pvs_tcd, Serilog.ILogger logger, IConfiguration conf) : base(context, context_pias, context_pias_update, context_pvs_tcd, logger, conf)
        {
            _dx_repo = new HsgdDxRepository(context, context_pias, context_pias_update, logger, conf);

        }
        public async Task<string> CreateHsgdTtrinh(HsgdTtrinh hsgdTtrinh, List<HsgdTtrinhCt> hsgdTtrinhCt, List<HsgdTtrinhTt> hsgdTtrinhTt, string email)
        {
            try
            {

                var hsgd_ctu = FirstOrDefaultWithNoLock(_context.HsgdCtus.Where(x => x.PrKey == hsgdTtrinh.PrKeyHsgd).AsQueryable());
                if (hsgd_ctu != null)
                {
                    //if (hsgd_ctu.MaTtrangGd != "6")
                    //{
                    //    return "Hồ sơ giám định chưa đc duyệt.Không lập được tờ trình.";
                    //}
                    hsgdTtrinh.MaDonvi = hsgd_ctu.MaDonvi;
                    hsgdTtrinh.TenDttt = hsgd_ctu.BienKsoat;
                    hsgdTtrinh.NgayCtu = DateTime.Now;
                    hsgdTtrinh.MaTtrang = "";
                    hsgdTtrinh.NgGdich = hsgd_ctu.TenKhach != null ? hsgd_ctu.TenKhach : "";
                    var hsbt_ctu = FirstOrDefaultWithNoLock(_context_pias_update.HsbtCtus.Where(x => x.PrKey == hsgd_ctu.PrKeyBt).AsQueryable());
                    if (hsbt_ctu != null)
                    {
                        hsgdTtrinh.SoHsbt = hsbt_ctu.SoHsbt;
                        hsgdTtrinh.NgayTthat = hsbt_ctu.NgayTthat;
                    }
                }
                else
                {
                    return "Hồ sơ giám định không tồn tại.Vui lòng kiểm tra lại.";
                }
                hsgdTtrinh.SoTien = Convert.ToInt64(hsgdTtrinhCt.Sum(item => item.SotienBt));


                await using var contextnew = new GdttContext();
                await using var dbContextTransaction = await contextnew.Database.BeginTransactionAsync();
                try
                {
                    await contextnew.HsgdTtrinhs.AddAsync(hsgdTtrinh);
                    await contextnew.SaveChangesAsync();
                    hsgdTtrinhCt.ForEach(a => a.FrKey = hsgdTtrinh.PrKey);
                    await contextnew.HsgdTtrinhCts.AddRangeAsync(hsgdTtrinhCt);
                    await contextnew.SaveChangesAsync();

                    var list_xml = hsgdTtrinhCt.Where(x => !string.IsNullOrEmpty(x.TenFile) && x.PrKeyXml == 0).GroupBy(g => g.TenFile).Select(s => s.First()).ToList();
                    for (int i = 0; i < list_xml.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(list_xml[i].FileData))
                        {
                            string folderUpload = _configuration["UploadSettings:FolderUpload_MDF1"] ?? "";
                            string url_upload = _configuration["DownloadSettings:UlpoadServer_MDF1"] ?? "";
                            var utilityHelper = new UtilityHelper(_logger);
                            var file_path = utilityHelper.UploadFile_ToAPI(list_xml[i].FileData, ".xml", folderUpload, url_upload, false);
                            if (!string.IsNullOrEmpty(file_path))
                            {
                                hsgdTtrinhCt.Where(x => x.TenFile == list_xml[i].TenFile).ToList().ForEach(a => a.PathXml = file_path);
                            }
                        }

                    }
                    for (int j = 0; j < hsgdTtrinhCt.Count; j++)
                    {
                        if (!string.IsNullOrEmpty(hsgdTtrinhCt[j].PathXml))
                        {
                            HsgdTotrinhXml xml = new HsgdTotrinhXml();
                            xml.FrKey = hsgdTtrinhCt[j].PrKey;
                            xml.PathXml = hsgdTtrinhCt[j].PathXml ?? "";
                            xml.TenFile = hsgdTtrinhCt[j].TenFile ?? "";
                            await contextnew.HsgdTotrinhXmls.AddAsync(xml);

                        }

                    }
                    for (int j = 0; j < hsgdTtrinhTt.Count; j++)
                    {
                        // 2. THÊM: Những record mới (pr_key = 0)
                        var toAdd = hsgdTtrinhTt.Where(x => x.PrKey == 0).ToList();
                        foreach (var item in toAdd)
                        {
                            item.FrKey = hsgdTtrinh.PrKey;
                            contextnew.HsgdTtrinhTt.Add(item);
                        }
                    }
                    await contextnew.SaveChangesAsync();

                    // lưu vào bảng nhật ký
                    if (!string.IsNullOrEmpty(email))
                    {
                        var user = FirstOrDefaultWithNoLock(_context.DmUsers.Where(x => x.Mail == email).AsQueryable());
                        if (user != null)
                        {
                            HsgdTtrinhNky hsgdTtrinhNky = new HsgdTtrinhNky();
                            hsgdTtrinhNky.UserNhan = user.Oid.ToString();
                            hsgdTtrinhNky.UserChuyen = user.Oid.ToString();
                            hsgdTtrinhNky.NgayCnhat = DateTime.Now;
                            hsgdTtrinhNky.FrKey = hsgdTtrinh.PrKey;
                            hsgdTtrinhNky.GhiChu = "Cán bộ " + user.TenUser + " tạo tờ trình";
                            hsgdTtrinhNky.Act = "CREATETOTRINH";

                            await contextnew.HsgdTtrinhNkies.AddAsync(hsgdTtrinhNky);
                            await contextnew.SaveChangesAsync();
                        }
                    }
                    await dbContextTransaction.CommitAsync();
                    return hsgdTtrinh.PrKey.ToString();
                }
                catch (Exception ex)
                {
                    _logger.Error("dbContextTransaction Exception when CreateHsgdTtrinh: " + ex.ToString());
                    _logger.Error("Error record: " + JsonConvert.SerializeObject(hsgdTtrinh));
                    await dbContextTransaction.RollbackAsync();
                    await dbContextTransaction.DisposeAsync();
                    throw;
                }

            }
            catch (Exception ex)
            {
            }
            return null!;
        }
        public string UpdateHsgdTtrinh(HsgdTtrinh hsgdTtrinh, List<HsgdTtrinhCt> hsgdTtrinhCt, List<HsgdTtrinhTt> hsgdTtrinhTt, List<HsgdTtrinhCt> hsgdTtrinhCt_delete, string email)
        {
            try
            {

                using var contextnew = new GdttContext();
                using var dbContextTransaction = contextnew.Database.BeginTransaction();
                try
                {
                    contextnew.HsgdTtrinhs.Update(hsgdTtrinh);
                    contextnew.HsgdTtrinhCts.UpdateRange(hsgdTtrinhCt);
                    contextnew.SaveChanges();
                    contextnew.HsgdTotrinhXmls.Where(x => hsgdTtrinhCt_delete.Select(x => x.PrKey).ToArray().Contains(x.FrKey)).ExecuteDelete();
                    contextnew.HsgdTtrinhCts.RemoveRange(hsgdTtrinhCt_delete);

                    var list_xml = hsgdTtrinhCt.Where(x => !string.IsNullOrEmpty(x.TenFile) && x.PrKeyXml == 0).GroupBy(g => g.TenFile).Select(s => s.First()).ToList();
                    for (int i = 0; i < list_xml.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(list_xml[i].FileData))
                        {
                            string folderUpload = _configuration["UploadSettings:FolderUpload_MDF1"] ?? "";
                            string url_upload = _configuration["DownloadSettings:UlpoadServer_MDF1"] ?? "";
                            var utilityHelper = new UtilityHelper(_logger);
                            var file_path = utilityHelper.UploadFile_ToAPI(list_xml[i].FileData, ".xml", folderUpload, url_upload, false);
                            if (!string.IsNullOrEmpty(file_path))
                            {
                                hsgdTtrinhCt.Where(x => x.TenFile == list_xml[i].TenFile).ToList().ForEach(a => a.PathXml = file_path);
                            }
                        }

                    }
                    for (int j = 0; j < hsgdTtrinhCt.Count; j++)
                    {
                        if (!string.IsNullOrEmpty(hsgdTtrinhCt[j].PathXml) && hsgdTtrinhCt[j].PrKeyXml == 0)
                        {
                            HsgdTotrinhXml xml = new HsgdTotrinhXml();
                            xml.FrKey = hsgdTtrinhCt[j].PrKey;
                            xml.PathXml = hsgdTtrinhCt[j].PathXml ?? "";
                            xml.TenFile = hsgdTtrinhCt[j].TenFile ?? "";
                            contextnew.HsgdTotrinhXmls.Add(xml);
                        }

                    }
                    //Xóa hsgdTtrinhTt 
                    var dbList = contextnew.HsgdTtrinhTt.Where(x => x.FrKey == hsgdTtrinh.PrKey).ToList();
                    var inputKeys = hsgdTtrinhTt.Where(x => x.PrKey > 0).Select(x => x.PrKey).ToList();
                    var toDelete = dbList.Where(x => !inputKeys.Contains(x.PrKey)).ToList();

                    contextnew.HsgdTtrinhTt.RemoveRange(toDelete);
                    for (int j = 0; j < hsgdTtrinhTt.Count; j++)
                    {
                        // 2. THÊM: Những record mới (pr_key = 0)
                        var toAdd = hsgdTtrinhTt.Where(x => x.PrKey == 0).ToList();
                        foreach (var item in toAdd)
                        {
                            item.FrKey = hsgdTtrinh.PrKey;
                            contextnew.HsgdTtrinhTt.Add(item);
                        }

                        // 3. CẬP NHẬT: Những record đã tồn tại
                        foreach (var item in hsgdTtrinhTt.Where(x => x.PrKey > 0))
                        {
                            var dbItem = dbList.FirstOrDefault(x => x.PrKey == item.PrKey);
                            if (dbItem != null)
                            {
                                dbItem.TenChuTk = item.TenChuTk;
                                dbItem.SoTaikhoanNh = item.SoTaikhoanNh;
                                dbItem.TenNh = item.TenNh;
                                dbItem.SotienTt = item.SotienTt;
                                dbItem.LydoTt = item.LydoTt;
                                dbItem.bnkCode = item.bnkCode;
                            }
                        }
                    }
                    //contextnew.HsgdTtrinhs.Update(hsgdTtrinh);
                    // lưu vào bảng nhật ký
                    if (!string.IsNullOrEmpty(email))
                    {
                        var user = FirstOrDefaultWithNoLock(_context.DmUsers.Where(x => x.Mail == email).AsQueryable());
                        if (user != null)
                        {
                            HsgdTtrinhNky hsgdTtrinhNky = new HsgdTtrinhNky();
                            hsgdTtrinhNky.UserNhan = user.Oid.ToString();
                            hsgdTtrinhNky.UserChuyen = user.Oid.ToString();
                            hsgdTtrinhNky.NgayCnhat = DateTime.Now;
                            hsgdTtrinhNky.FrKey = hsgdTtrinh.PrKey;
                            hsgdTtrinhNky.GhiChu = "Cán bộ " + user.TenUser + " cập nhật tờ trình";
                            hsgdTtrinhNky.Act = "UPDATETOTRINH";
                            contextnew.HsgdTtrinhNkies.Add(hsgdTtrinhNky);
                        }
                    }
                    contextnew.SaveChanges();
                    dbContextTransaction.Commit();
                    return hsgdTtrinh.PrKey.ToString();
                }
                catch (Exception ex)
                {
                    _logger.Error("dbContextTransaction Exception when CreateHsgdTtrinh: " + ex.ToString());
                    _logger.Error("Error record: " + JsonConvert.SerializeObject(hsgdTtrinh));
                    dbContextTransaction.Rollback();
                    dbContextTransaction.Dispose();
                    throw;
                }

            }
            catch (Exception ex)
            {
            }
            return null!;
        }
        public CombinedTtrinhResult4 GetPrintToTrinh(decimal prKey, string email)
        {
            try
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
                var wordPdfRequest = new WordToPdfRequest();
                wordPdfRequest.ListData = new List<EntityContent>();
                UpdateProperties update = new UpdateProperties();
                List<tt_giamdinh> hsbt_gd = new List<tt_giamdinh>();
                List<ThuHuong> hsgd_totrinh_tt = new List<ThuHuong>();
                var hsgd_tt = FirstOrDefaultWithNoLock((from a in _context.HsgdTtrinhs
                                                        join b in _context.HsgdCtus on a.PrKeyHsgd equals b.PrKey
                                                        where a.PrKey == prKey
                                                        select new
                                                        {
                                                            TenKhach = b.TenKhach,//tên người được bảo hiểm
                                                            BienKsoat = b.BienKsoat, // biển số xe
                                                            SoSeri = b.SoSeri,// giấy CNBH số
                                                            SoDonbh = b.SoDonbh,// số đơn bh
                                                            TenLaixe = b.TenLaixe,// tên lái xe
                                                            SoGphepLaixe = b.SoGphepLaixe, // giấy phép lái xe
                                                            NgayDauLaixe = b.NgayDauLaixe,
                                                            NgayCuoiLaixe = b.NgayCuoiLaixe,
                                                            NgayDauSeri = b.NgayDauSeri,
                                                            NgayCuoiSeri = b.NgayCuoiSeri,
                                                            SoGphepLuuhanh = b.SoGphepLuuhanh,
                                                            NgayDauLuuhanh = b.NgayDauLuuhanh,
                                                            NgayCuoiLuuhanh = b.NgayCuoiLuuhanh,
                                                            DiaDiemtt = b.DiaDiemtt,
                                                            NguyenNhanTtat = b.NguyenNhanTtat,
                                                            HauQua = a.HauQua,
                                                            SoHsgd = b.SoHsgd,
                                                            PrKeyBt = b.PrKeyBt,
                                                            TaisanThuhoi = a.TaisanThuhoi,
                                                            GiatriThuhoi = a.GiatriThuhoi,
                                                            PanThoiTs = a.PanThoiTs,
                                                            PrKeyGoc = b.PrKeyGoc,
                                                            NamSinh = b.NamSinh,
                                                            NgayTthat = b.NgayTthat,
                                                            YkienGdinh = b.YkienGdinh,
                                                            DexuatPan = b.DexuatPan,
                                                            SoTienThucTe = b.SoTienThucTe,
                                                            SoPhibh = a.SoPhibh,
                                                            MaDonvi = a.MaDonvi,
                                                            NgayThuphi = a.NgayThuphi,
                                                            ChkDaydu = a.ChkDaydu,
                                                            ChkDunghan = a.ChkDunghan,
                                                            ChkChuanopphi = a.ChkChuanopphi,
                                                            MaDonvigd = b.MaDonvigd,
                                                            PrKeyHsgd = a.PrKeyHsgd,
                                                            ChiKhac = a.ChiKhac
                                                        }).AsQueryable());
                if (hsgd_tt != null)
                {
                    //lấy dữ liệu
                    char checkMark = '\u2612';
                    char emptyBox = '\u2610';
                    var thong_tin = (from vars in _context_pias.DmVars
                                     where vars.MaDonvi == hsgd_tt.MaDonvi &&
                                           (vars.Bien == "DONVI_ME" || vars.Bien == "DON_VI" || vars.Bien == "TP")
                                     select vars).ToList();
                    var donvi_me = "";
                    var donvi = "";
                    var tp = "";
                    foreach (var record in thong_tin)
                    {
                        switch (record.Bien)
                        {
                            case "DONVI_ME":
                                donvi_me = record.GiaTri;
                                break;
                            case "DON_VI":
                                donvi = record.GiaTri;
                                break;
                            case "TP":
                                tp = record.GiaTri;
                                break;
                            default:

                                break;

                        }
                    }
                    var don_vi = _context_pias.DmVars.Where(x => x.MaDonvi == hsgd_tt.MaDonvigd && x.Bien == "DON_VI").FirstOrDefault();
                    if (don_vi != null)
                    {
                        update.AddEntityContent(wordPdfRequest, "[DON_VI]", don_vi.GiaTri.ToUpper());
                    }
                    else
                    {
                        update.AddEntityContent(wordPdfRequest, "[DON_VI]", "");
                    }
                    update.AddEntityContent(wordPdfRequest, "[DONVI_ME]", donvi_me.ToUpper());
                    update.AddEntityContent(wordPdfRequest, "[TP]", tp);
                    update.AddEntityContent(wordPdfRequest, "[DATE]", DateTime.Now.Day.ToString());
                    update.AddEntityContent(wordPdfRequest, "[MONTH]", DateTime.Now.Month.ToString());
                    update.AddEntityContent(wordPdfRequest, "[YEAR]", DateTime.Now.Year.ToString());

                    #region lấy dữ liệu lịch sử tổn thất
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    var tu_ngay = DateTime.ParseExact(Convert.ToDateTime(hsgd_tt.NgayDauSeri).ToString("dd/MM/yyyy 00:00:00"), "dd/MM/yyyy HH:mm:ss", provider);
                    var den_ngay = DateTime.ParseExact(Convert.ToDateTime(hsgd_tt.NgayTthat).ToString("dd/MM/yyyy 00:00:00"), "dd/MM/yyyy HH:mm:ss", provider);
                    var uoc1 = (from a in _context_pias_update.HsbtCtus
                                join b in _context_pias_update.HsbtCts on a.PrKey equals b.FrKey
                                join c in _context_pias_update.HsbtUocs on b.PrKey equals c.FrKey
                                where b.MaSp.StartsWith("05") && new[] { "01", "02", "05" }.Contains(b.MaTtrangBt) && new[] { "TBT", "NBT" }.Contains(a.MaLhsbt) && c.NgayPs <= den_ngay && a.PrKey != hsgd_tt.PrKeyBt && a.SoHdgcn == hsgd_tt.SoDonbh && a.SoSeri == hsgd_tt.SoSeri
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
                                where b.MaSp.StartsWith("05") && new[] { "03", "04" }.Contains(b.MaTtrangBt) && new[] { "TBT", "NBT" }.Contains(a.MaLhsbt) && c.NgayPs <= den_ngay && b.NgayHtoanBt > den_ngay && a.PrKey != hsgd_tt.PrKeyBt && a.SoHdgcn == hsgd_tt.SoDonbh && a.SoSeri == hsgd_tt.SoSeri
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
                                                 where b.MaSp.StartsWith("05") && b.MaTtrangBt == "03" && new[] { "TBT", "NBT" }.Contains(a.MaLhsbt) && b.NgayHtoanBt >= tu_ngay && b.NgayHtoanBt <= den_ngay && a.PrKey != hsgd_tt.PrKeyBt && a.SoHdgcn == hsgd_tt.SoDonbh && a.SoSeri == hsgd_tt.SoSeri
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
                    update.AddEntityContent(wordPdfRequest, "[SO_LAN_UBT]", so_lan_ubt.ToString());
                    update.AddEntityContent(wordPdfRequest, "[NGUYEN_TE_UBT]", nguyen_te_ubt.ToString("#,###", cul.NumberFormat));
                    update.AddEntityContent(wordPdfRequest, "[SO_LAN_BT]", so_lan_bt.ToString());
                    update.AddEntityContent(wordPdfRequest, "[NGUYEN_TE_BT]", nguyen_te_bt.ToString("#,###", cul.NumberFormat));
                    #endregion
                    // lấy giá trị thực tế xe theo đơn gốc trên pias
                    //haipv1 note: STBH/MTN tài sản=mtn_gtbh_tsan, G.tri khai báo=giatri_tte
                    var seriPhiBH = GetSoPhiBH(hsgd_tt.SoDonbh, hsgd_tt.SoSeri);
                    var mtnGtbhVnd = GetMtnGtbh(hsgd_tt.SoDonbh);
                    if (mtnGtbhVnd != 0)
                    {
                        update.AddEntityContent(wordPdfRequest, "[GIATRI_TTE]", mtnGtbhVnd.ToString("#,###", cul.NumberFormat));
                    }
                    else
                    {
                        if (seriPhiBH != null)
                        {
                            update.AddEntityContent(wordPdfRequest, "[GIATRI_TTE]", seriPhiBH.GiaTri_Tte.ToString("#,###", cul.NumberFormat));
                        }
                        else
                        {
                            update.AddEntityContent(wordPdfRequest, "[GIATRI_TTE]", hsgd_tt.SoTienThucTe.ToString("#,###", cul.NumberFormat));
                        }
                    }

                    update.AddEntityContent(wordPdfRequest, "[TEN_KHACH]", hsgd_tt.TenKhach);
                    update.AddEntityContent(wordPdfRequest, "[BIENSO_XE]", hsgd_tt.BienKsoat);
                    update.AddEntityContent(wordPdfRequest, "[SO_SERI]", hsgd_tt.SoSeri.ToString());
                    update.AddEntityContent(wordPdfRequest, "[SO_DONBH]", hsgd_tt.SoDonbh);
                    update.AddEntityContent(wordPdfRequest, "[THOI_HAN_BH]", "Từ ngày " + Convert.ToDateTime(hsgd_tt.NgayDauSeri).ToString("dd/MM/yyyy") + " đến ngày " + Convert.ToDateTime(hsgd_tt.NgayCuoiSeri).ToString("dd/MM/yyyy"));
                    update.AddEntityContent(wordPdfRequest, "[TEN_LAI_XE]", hsgd_tt.TenLaixe);
                    update.AddEntityContent(wordPdfRequest, "[SO_GPLX]", hsgd_tt.SoGphepLaixe);
                    update.AddEntityContent(wordPdfRequest, "[SO_GPLH]", hsgd_tt.SoGphepLuuhanh);
                    update.AddEntityContent(wordPdfRequest, "[THOIHAN_GPLX]", "Từ ngày " + Convert.ToDateTime(hsgd_tt.NgayDauLaixe).ToString("dd/MM/yyyy") + " đến ngày " + Convert.ToDateTime(hsgd_tt.NgayCuoiLaixe).ToString("dd/MM/yyyy"));
                    update.AddEntityContent(wordPdfRequest, "[THOIHAN_GPLH]", "Từ ngày " + Convert.ToDateTime(hsgd_tt.NgayDauLuuhanh).ToString("dd/MM/yyyy") + " đến ngày " + Convert.ToDateTime(hsgd_tt.NgayCuoiLuuhanh).ToString("dd/MM/yyyy"));
                    update.AddEntityContent(wordPdfRequest, "[PHI_BH]", hsgd_tt.SoPhibh.ToString("#,###", cul.NumberFormat));
                    update.AddEntityContent(wordPdfRequest, "[NGAY_TONTHAT]", Convert.ToDateTime(hsgd_tt.NgayTthat).ToString("dd/MM/yyyy"));
                    update.AddEntityContent(wordPdfRequest, "[NGAY_TPHI]", hsgd_tt.NgayThuphi);
                    //if (hsgd_tt.ChkDaydu)
                    //{
                    //    update.AddEntityContent(wordPdfRequest, $"[CHK_DAYDU_CO]", checkMark.ToString());
                    //    update.AddEntityContent(wordPdfRequest, $"[CHK_DAYDU_KHONG]", emptyBox.ToString());
                    //}
                    //else
                    //{
                    //    update.AddEntityContent(wordPdfRequest, $"[CHK_DAYDU_CO]", emptyBox.ToString());
                    //    update.AddEntityContent(wordPdfRequest, $"[CHK_DAYDU_KHONG]", checkMark.ToString());
                    //}
                    //if (hsgd_tt.ChkDunghan)
                    //{
                    //    update.AddEntityContent(wordPdfRequest, $"[CHK_DUNGHAN_CO]", checkMark.ToString());
                    //    update.AddEntityContent(wordPdfRequest, $"[CHK_DUNGHAN_KHONG]", emptyBox.ToString());
                    //}
                    //else
                    //{
                    //    update.AddEntityContent(wordPdfRequest, $"[CHK_DUNGHAN_CO]", emptyBox.ToString());
                    //    update.AddEntityContent(wordPdfRequest, $"[CHK_DUNGHAN_KHONG]", checkMark.ToString());
                    //}
                    if (hsgd_tt.ChkChuanopphi)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[CHK_CHUANOPPHI]", checkMark.ToString());
                    }
                    else
                    {
                        //update.AddEntityContent(wordPdfRequest, $"[CHK_CHUANOPPHI]", emptyBox.ToString());
                        if (hsgd_tt.ChkDaydu == true)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[CHK_DAYDU_CO]", checkMark.ToString());
                            update.AddEntityContent(wordPdfRequest, $"[CHK_DAYDU_KHONG]", emptyBox.ToString());
                        }
                        else
                        {
                            update.AddEntityContent(wordPdfRequest, $"[CHK_DAYDU_CO]", emptyBox.ToString());
                            update.AddEntityContent(wordPdfRequest, $"[CHK_DAYDU_KHONG]", checkMark.ToString());
                        }
                        if (hsgd_tt.ChkDunghan == true)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[CHK_DUNGHAN_CO]", checkMark.ToString());
                            update.AddEntityContent(wordPdfRequest, $"[CHK_DUNGHAN_KHONG]", emptyBox.ToString());
                        }
                        else
                        {
                            update.AddEntityContent(wordPdfRequest, $"[CHK_DUNGHAN_CO]", emptyBox.ToString());
                            update.AddEntityContent(wordPdfRequest, $"[CHK_DUNGHAN_KHONG]", checkMark.ToString());
                        }
                    }
                    var so_hopdong = _context_pias.NvuBhtCtus.Where(x => x.PrKey == hsgd_tt.PrKeyGoc).Select(x => x.MaHdong).FirstOrDefault();
                    update.AddEntityContent(wordPdfRequest, "[SO_HOPDONG]", so_hopdong);
                    if (hsgd_tt.NamSinh > 0)
                    {
                        update.AddEntityContent(wordPdfRequest, "[TUOI_LAI_XE]", (DateTime.Now.Year - hsgd_tt.NamSinh).ToString());
                    }
                    else
                    {
                        update.AddEntityContent(wordPdfRequest, "[TUOI_LAI_XE]", "");
                    }
                    List<string> list_DiaDiemtt = ContentHelper.SplitString(ContentHelper.formatNewLine(hsgd_tt.DiaDiemtt), 255);
                    for (int i = 0; i < list_DiaDiemtt.Count(); i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[DIA_DIEM_TN{i}]", list_DiaDiemtt[i]);
                    }
                    for (int i = list_DiaDiemtt.Count(); i < 2; i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[DIA_DIEM_TN{i}]", "");
                    }
                    List<string> list_NguyenNhanTtat = ContentHelper.SplitString(ContentHelper.formatNewLine(hsgd_tt.NguyenNhanTtat), 255);
                    for (int i = 0; i < list_NguyenNhanTtat.Count(); i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[NGUYEN_NHAN_TN{i}]", list_NguyenNhanTtat[i]);
                    }
                    for (int i = list_NguyenNhanTtat.Count(); i < 2; i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[NGUYEN_NHAN_TN{i}]", "");
                    }
                    List<string> list_HauQua = ContentHelper.SplitString(ContentHelper.formatNewLine(hsgd_tt.HauQua), 255);
                    for (int i = 0; i < list_HauQua.Count(); i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[HAU_QUA_TN{i}]", list_HauQua[i]);
                    }
                    for (int i = list_HauQua.Count(); i < 20; i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[HAU_QUA_TN{i}]", "");
                    }

                    List<string> list_YkienGdinh = ContentHelper.SplitString(ContentHelper.formatNewLine(hsgd_tt.YkienGdinh), 255);
                    for (int i = 0; i < list_YkienGdinh.Count(); i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[YKIEN_GDINH{i}]", list_YkienGdinh[i]);
                    }
                    for (int i = list_YkienGdinh.Count(); i < 12; i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[YKIEN_GDINH{i}]", "");
                    }
                    List<string> list_DexuatPan = ContentHelper.SplitString(ContentHelper.formatNewLine(hsgd_tt.DexuatPan), 255);
                    for (int i = 0; i < list_DexuatPan.Count(); i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[DEXUAT_PAN{i}]", list_DexuatPan[i]);
                    }
                    for (int i = list_DexuatPan.Count(); i < 12; i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[DEXUAT_PAN{i}]", "");
                    }
                    update.AddEntityContent(wordPdfRequest, "[SO_HSGD]", hsgd_tt.SoHsgd);

                    update.AddEntityContent(wordPdfRequest, "[GIATRI_THUHOI]", hsgd_tt.GiatriThuhoi.ToString("#,###", cul.NumberFormat));
                    #region lấy giá trị thu hồi trong bảng hsgd_dx

                    if (!string.IsNullOrEmpty(hsgd_tt.TaisanThuhoi))
                    {
                        List<string> list_TaisanThuhoi = ContentHelper.SplitString(ContentHelper.formatNewLine(hsgd_tt.TaisanThuhoi), 255);
                        for (int i = 0; i < list_TaisanThuhoi.Count(); i++)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[TAISAN_THUHOI{i}]", list_TaisanThuhoi[i]);
                        }
                        for (int i = list_TaisanThuhoi.Count(); i < 20; i++)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[TAISAN_THUHOI{i}]", "");
                        }
                        update.AddEntityContent(wordPdfRequest, $"[THTS_CO]", checkMark.ToString());
                        update.AddEntityContent(wordPdfRequest, $"[THTS_KHONG]", emptyBox.ToString());
                    }
                    else
                    {
                        var hsbt_ct = ToListWithNoLock(_context_pias_update.HsbtCts.Where(x => x.FrKey == hsgd_tt.PrKeyBt).Select(s => new
                        {
                            s.PrKey,
                            s.MaSp

                        }).AsQueryable());
                        var hsgd_dx_ct_tmp = (from a in _context.HsgdDxCts where hsbt_ct.Select(x => x.PrKey).ToArray().Contains(a.PrKeyHsbtCt) select a).AsQueryable();
                        var hsgd_dx_ct = ToListWithNoLock((from A in hsbt_ct
                                                           join B in hsgd_dx_ct_tmp on A.PrKey equals B.PrKeyHsbtCt
                                                           // where new[] { "050101", "050104" }.Contains(B.MaSp)
                                                           select new
                                                           {
                                                               PrKey = B.PrKey,
                                                               MaSp = A.MaSp
                                                           }).AsQueryable());
                        List<HsgdDx_HM> obj_dx = new List<HsgdDx_HM>();
                        if (hsgd_dx_ct.Where(x => new[] { "050101", "050104" }.Contains(x.MaSp)).Count() > 0)
                        {
                            var hsgd_dx = ToListWithNoLock((from a in _context.HsgdDxes where hsgd_dx_ct.Select(x => x.PrKey).ToArray().Contains(a.PrKeyDx) select a).AsQueryable());
                            obj_dx = (from A in hsgd_dx
                                      join B in hsgd_dx_ct on A.PrKeyDx equals B.PrKey
                                      join C in _context.DmHmucs on A.MaHmuc equals C.MaHmuc into C1
                                      from C in C1.DefaultIfEmpty()
                                      where new[] { "050101", "050104" }.Contains(B.MaSp) && A.ThuHoiTs
                                      select new HsgdDx_HM
                                      {
                                          MaHmuc = A.MaHmuc,
                                          Hmuc = C != null ? C.TenHmuc ?? "" : A.Hmuc,
                                          ThuHoiTs = A.ThuHoiTs
                                      }).ToList();
                        }
                        List<HsgdDx_HM> obj_dx_tsk = new List<HsgdDx_HM>();
                        if (hsgd_dx_ct.Where(x => !new[] { "050101", "050104" }.Contains(x.MaSp)).Count() > 0)
                        {
                            var hsgd_dx_tsk = ToListWithNoLock((from a in _context.HsgdDxTsks where hsgd_dx_ct.Select(x => x.PrKey).ToArray().Contains(a.PrKeyDx) select a).AsQueryable());
                            obj_dx_tsk = (from A in hsgd_dx_tsk
                                          join B in hsgd_dx_ct on A.PrKeyDx equals B.PrKey
                                          where !new[] { "050101", "050104" }.Contains(B.MaSp) && A.ThuHoiTs
                                          select new HsgdDx_HM
                                          {
                                              Hmuc = A.Hmuc,
                                              ThuHoiTs = A.ThuHoiTs
                                          }).ToList();
                        }
                        var obj = obj_dx.Union(obj_dx_tsk).ToList();
                        if (obj != null && obj.Count > 0)
                        {
                            var dx_tsth = string.Join(", ", obj.Select(s => s.Hmuc).ToList());
                            List<string> list_TaisanThuhoi = ContentHelper.SplitString(ContentHelper.formatNewLine(dx_tsth), 255);
                            for (int i = 0; i < list_TaisanThuhoi.Count(); i++)
                            {
                                update.AddEntityContent(wordPdfRequest, $"[TAISAN_THUHOI{i}]", list_TaisanThuhoi[i]);
                            }
                            for (int i = list_TaisanThuhoi.Count(); i < 20; i++)
                            {
                                update.AddEntityContent(wordPdfRequest, $"[TAISAN_THUHOI{i}]", "");
                            }
                            update.AddEntityContent(wordPdfRequest, $"[THTS_CO]", checkMark.ToString());
                            update.AddEntityContent(wordPdfRequest, $"[THTS_KHONG]", emptyBox.ToString());
                        }
                        else
                        {
                            for (int i = 0; i < 20; i++)
                            {
                                update.AddEntityContent(wordPdfRequest, $"[TAISAN_THUHOI{i}]", "");
                            }
                            update.AddEntityContent(wordPdfRequest, $"[THTS_CO]", emptyBox.ToString());
                            update.AddEntityContent(wordPdfRequest, $"[THTS_KHONG]", checkMark.ToString());
                        }
                    }
                    #endregion
                    List<string> list_PanThoiTs = ContentHelper.SplitString(ContentHelper.formatNewLine(hsgd_tt.PanThoiTs), 255);
                    for (int i = 0; i < list_PanThoiTs.Count(); i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[PAN_GIAIQUYET{i}]", list_PanThoiTs[i]);
                    }
                    for (int i = list_PanThoiTs.Count(); i < 20; i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[PAN_GIAIQUYET{i}]", "");
                    }
                    if (hsgd_tt.ChiKhac > 0)
                    {
                        update.AddEntityContent(wordPdfRequest, "[CHI_KHAC]", hsgd_tt.ChiKhac.ToString("#,###", cul.NumberFormat));
                    }
                    var hsbt_ctu = FirstOrDefaultWithNoLock((from a in _context_pias_update.HsbtCtus
                                                                 //join b in _context_pias.NvuBhtCtus on new { SoDonbh = a.SoHdgcn, SoDonbhBs = a.SoDonbhbs } equals new { SoDonbh = b.SoDonbh, SoDonbhBs = b.SoDonbhBs }
                                                             where a.PrKey == hsgd_tt.PrKeyBt
                                                             select new
                                                             {
                                                                 SoHsbt = a.SoHsbt,
                                                                 DiaChi = a.DiaChi,
                                                                 //NgayThuPhi = a.NgayThuPhi,
                                                                 SotheVcxMoto = a.SotheVcxMoto,
                                                                 ChiKhac = a.ChiKhac
                                                                 //SoPhibh = a.SoPhibh,
                                                                 //NgayTthat = a.NgayTthat
                                                                 //GiatriTteXe = a.GiatriTteXe,
                                                             }).AsQueryable());
                    if (hsbt_ctu != null)
                    {
                        update.AddEntityContent(wordPdfRequest, "[SO_HSBT]", hsbt_ctu.SoHsbt);
                        update.AddEntityContent(wordPdfRequest, "[SOTHE_VCX_MOTO]", hsbt_ctu.SotheVcxMoto);

                        if (string.IsNullOrEmpty(hsbt_ctu.DiaChi))
                        {
                            var dia_chi_seri = (from a in _context_pias.NvuBhtCtus
                                                join b in _context_pias.NvuBhtSeris on a.PrKey equals b.FrKey
                                                where a.SoDonbh == hsgd_tt.SoDonbh && b.SoSeri == hsgd_tt.SoSeri
                                                select b.DiaChi).FirstOrDefault();
                            if (!string.IsNullOrEmpty(dia_chi_seri))
                            {
                                update.AddEntityContent(wordPdfRequest, "[DIA_CHI_KHACH]", dia_chi_seri);
                            }
                            else
                            {
                                update.AddEntityContent(wordPdfRequest, "[DIA_CHI_KHACH]", "");
                            }
                        }
                        else
                        {
                            update.AddEntityContent(wordPdfRequest, "[DIA_CHI_KHACH]", hsbt_ctu.DiaChi);
                        }
                    }
                    else
                    {
                        update.AddEntityContent(wordPdfRequest, "[SO_HSBT]", "");
                        update.AddEntityContent(wordPdfRequest, "[DIA_CHI_KHACH]", "");
                        update.AddEntityContent(wordPdfRequest, "[SOTHE_VCX_MOTO]", "");
                        update.AddEntityContent(wordPdfRequest, "[CHI_KHAC]", "");

                    }
                    #region lấy thông tin giám định
                    hsbt_gd = ToListWithNoLock((from a in _context_pias.HsbtGds
                                                join b in _context_pias.DmKhaches on a.MaDvgd equals b.MaKh into b1
                                                from b in b1.DefaultIfEmpty()
                                                where a.FrKey == hsgd_tt.PrKeyBt
                                                select new tt_giamdinh
                                                {
                                                    sotien_gdinh = a.SoTiengd + a.SoTienv,
                                                    cty_gdinh = b.TenKh
                                                }).AsQueryable());
                    if (hsbt_gd != null && hsbt_gd.Count > 0)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[GD_CO]", checkMark.ToString());
                        update.AddEntityContent(wordPdfRequest, $"[GD_KHONG]", emptyBox.ToString());
                    }
                    else
                    {
                        if (hsgd_tt.ChiKhac > 0)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[GD_CO]", checkMark.ToString());
                            update.AddEntityContent(wordPdfRequest, $"[GD_KHONG]", emptyBox.ToString());
                        }
                        else
                        {
                            update.AddEntityContent(wordPdfRequest, $"[GD_CO]", emptyBox.ToString());
                            update.AddEntityContent(wordPdfRequest, $"[GD_KHONG]", checkMark.ToString());
                        }
                    }
                    #endregion
                    #region tính toán bồi thường VCX, TNDS...
                    var hsgd_tt_ct = (from a in _context.HsgdTtrinhCts
                                      where a.FrKey == prKey
                                      select new
                                      {
                                          FrKey = a.FrKey,
                                          MtnVcx = a.MaSp == "050104" ? a.SotienBh : 0,
                                          SotienBtvcx = a.MaSp == "050104" ? a.SotienBt : 0,
                                          MtnHhoa = a.MaSp == "050102" ? a.SotienBh : 0,
                                          SotienBthhoa = a.MaSp == "050102" ? a.SotienBt : 0,
                                          MtnLpx = new[] { "050103", "050202" }.Contains(a.MaSp) ? a.SotienBh : 0,
                                          SotienBtlpx = new[] { "050103", "050202" }.Contains(a.MaSp) ? a.SotienBt : 0,
                                          MtnNt3 = new[] { "050101", "050105", "050201", "050204" }.Contains(a.MaSp) ? a.SotienBh : 0,
                                          SotienBtnt3 = new[] { "050101", "050105", "050201", "050204" }.Contains(a.MaSp) ? a.SotienBt : 0,
                                          TtoanBtVcx = a.MaSp == "050104" ? a.TinhToanbt : "",
                                          TtoanBtHh = a.MaSp == "050102" ? a.TinhToanbt : "",
                                          TtoanBtLpx = new[] { "050103", "050202" }.Contains(a.MaSp) ? a.TinhToanbt : "",
                                          TtoanBtMtnNt3 = new[] { "050101", "050105", "050201", "050203" }.Contains(a.MaSp) ? a.TinhToanbt : ""
                                      }).AsQueryable();
                    var hsgd_tt_ct_gr = hsgd_tt_ct.GroupBy(g => g.FrKey)
                        .Select(s => new
                        {
                            MtnVcx = s.Max(x => x.MtnVcx),
                            SotienBtvcx = s.Sum(x => x.SotienBtvcx),
                            MtnHhoa = s.Max(x => x.MtnHhoa),
                            SotienBthhoa = s.Sum(x => x.SotienBthhoa),
                            MtnLpx = s.Max(x => x.MtnLpx),
                            SotienBtlpx = s.Sum(x => x.SotienBtlpx),
                            MtnNt3 = s.Sum(x => x.MtnNt3),
                            SotienBtnt3 = s.Sum(x => x.SotienBtnt3),
                            TtoanBtVcx = s.Max(x => x.TtoanBtVcx),
                            TtoanBtHh = s.Max(x => x.TtoanBtHh),
                            TtoanBtLpx = s.Max(x => x.TtoanBtLpx),
                            TtoanBtMtnNt3 = s.Max(x => x.TtoanBtMtnNt3)
                        }).FirstOrDefault();
                    if (hsgd_tt_ct_gr != null)
                    {
                        update.AddEntityContent(wordPdfRequest, "[MTN_VCX]", hsgd_tt_ct_gr.MtnVcx.ToString("#,###", cul.NumberFormat));
                        update.AddEntityContent(wordPdfRequest, "[SOTIEN_BTVCX]", hsgd_tt_ct_gr.SotienBtvcx.ToString("#,###", cul.NumberFormat));
                        update.AddEntityContent(wordPdfRequest, "[MTN_HHOA]", hsgd_tt_ct_gr.MtnHhoa.ToString("#,###", cul.NumberFormat));
                        update.AddEntityContent(wordPdfRequest, "[SOTIEN_BTHHOA]", hsgd_tt_ct_gr.SotienBthhoa.ToString("#,###", cul.NumberFormat));
                        update.AddEntityContent(wordPdfRequest, "[MTN_LPX]", hsgd_tt_ct_gr.MtnLpx.ToString("#,###", cul.NumberFormat));
                        update.AddEntityContent(wordPdfRequest, "[SOTIEN_BTLPX]", hsgd_tt_ct_gr.SotienBtlpx.ToString("#,###", cul.NumberFormat));
                        update.AddEntityContent(wordPdfRequest, "[MTN_NT3]", hsgd_tt_ct_gr.MtnNt3.ToString("#,###", cul.NumberFormat));
                        update.AddEntityContent(wordPdfRequest, "[SOTIEN_BTNT3]", hsgd_tt_ct_gr.SotienBtnt3.ToString("#,###", cul.NumberFormat));
                        if (hsgd_tt_ct_gr.SotienBtvcx != 0)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[VCX_CO]", checkMark.ToString());
                            update.AddEntityContent(wordPdfRequest, $"[VCX_KHONG]", emptyBox.ToString());
                        }
                        else
                        {
                            update.AddEntityContent(wordPdfRequest, $"[VCX_CO]", emptyBox.ToString());
                            update.AddEntityContent(wordPdfRequest, $"[VCX_KHONG]", checkMark.ToString());
                        }
                        if (hsgd_tt_ct_gr.SotienBthhoa != 0)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[HH_CO]", checkMark.ToString());
                            update.AddEntityContent(wordPdfRequest, $"[HH_KHONG]", emptyBox.ToString());
                        }
                        else
                        {
                            update.AddEntityContent(wordPdfRequest, $"[HH_CO]", emptyBox.ToString());
                            update.AddEntityContent(wordPdfRequest, $"[HH_KHONG]", checkMark.ToString());
                        }
                        if (hsgd_tt_ct_gr.SotienBtlpx != 0)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[LPX_CO]", checkMark.ToString());
                            update.AddEntityContent(wordPdfRequest, $"[LPX_KHONG]", emptyBox.ToString());
                        }
                        else
                        {
                            update.AddEntityContent(wordPdfRequest, $"[LPX_CO]", emptyBox.ToString());
                            update.AddEntityContent(wordPdfRequest, $"[LPX_KHONG]", checkMark.ToString());
                        }
                        if (hsgd_tt_ct_gr.SotienBtnt3 != 0)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[TNDS_CO]", checkMark.ToString());
                            update.AddEntityContent(wordPdfRequest, $"[TNDS_KHONG]", emptyBox.ToString());
                        }
                        else
                        {
                            update.AddEntityContent(wordPdfRequest, $"[TNDS_CO]", emptyBox.ToString());
                            update.AddEntityContent(wordPdfRequest, $"[TNDS_KHONG]", checkMark.ToString());
                        }
                        List<string> list_TtoanBtVcx = ContentHelper.SplitString(ContentHelper.formatNewLine(hsgd_tt_ct_gr.TtoanBtVcx), 255);
                        for (int i = 0; i < list_TtoanBtVcx.Count(); i++)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[TTOAN_BTVCX{i}]", list_TtoanBtVcx[i]);
                        }
                        int dem = list_TtoanBtVcx.Count();
                        if (dem == 0)
                        {
                            dem = 1;
                        }
                        for (int i = dem; i < 20; i++)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[TTOAN_BTVCX{i}]", "");
                        }
                        List<string> list_TtoanBtHh = ContentHelper.SplitString(ContentHelper.formatNewLine(hsgd_tt_ct_gr.TtoanBtHh), 255);
                        for (int i = 0; i < list_TtoanBtHh.Count(); i++)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[TTOAN_BTHHOA{i}]", list_TtoanBtHh[i]);
                        }
                        dem = list_TtoanBtHh.Count();
                        if (dem == 0)
                        {
                            dem = 1;
                        }
                        for (int i = dem; i < 20; i++)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[TTOAN_BTHHOA{i}]", "");
                        }
                        List<string> list_TtoanBtLpx = ContentHelper.SplitString(ContentHelper.formatNewLine(hsgd_tt_ct_gr.TtoanBtLpx), 255);
                        for (int i = 0; i < list_TtoanBtLpx.Count(); i++)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[TTOAN_BTLPX{i}]", list_TtoanBtLpx[i]);
                        }
                        dem = list_TtoanBtLpx.Count();
                        if (dem == 0)
                        {
                            dem = 1;
                        }
                        for (int i = dem; i < 20; i++)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[TTOAN_BTLPX{i}]", "");
                        }
                        List<string> list_TtoanBtMtnNt3 = ContentHelper.SplitString(ContentHelper.formatNewLine(hsgd_tt_ct_gr.TtoanBtMtnNt3), 255);
                        for (int i = 0; i < list_TtoanBtMtnNt3.Count(); i++)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[TTOAN_BTNT3{i}]", list_TtoanBtMtnNt3[i]);
                        }
                        dem = list_TtoanBtMtnNt3.Count();
                        if (dem == 0)
                        {
                            dem = 1;
                        }
                        for (int i = dem; i < 20; i++)
                        {
                            update.AddEntityContent(wordPdfRequest, $"[TTOAN_BTNT3{i}]", "");
                        }
                    }
                    var hsgd_tt_ct_all = ToListWithNoLock(_context.HsgdTtrinhCts
                        .Where(x => x.FrKey == prKey)
                        .GroupBy(g => new { g.MaSp, g.SotienBh })
                        .Select(s => new
                        {
                            MaSp = s.Key.MaSp,
                            SotienBh = s.Key.SotienBh,
                            SotienBt = s.Sum(x => x.SotienBt),
                            SotienTu = s.Sum(x => x.SotienTu)
                        }).AsQueryable());
                    double sotien_tu = 0;
                    double sotien_bt = 0;
                    StringBuilder loaihinh_bh = new StringBuilder();
                    if (hsgd_tt_ct_all != null)
                    {
                        for (int i = 0; i < hsgd_tt_ct_all.Count(); i++)
                        {
                            loaihinh_bh.AppendLine(hsgd_tt_ct_all[i].MaSp + "/" + hsgd_tt_ct_all[i].SotienBh.ToString("#,###", cul.NumberFormat) + " đ");
                        }
                        sotien_tu = (double)hsgd_tt_ct_all.Sum(x => x.SotienTu);
                        sotien_bt = (double)hsgd_tt_ct_all.Sum(x => x.SotienBt);
                    }
                    update.AddEntityContent(wordPdfRequest, "[LOAIHINH_BH]", loaihinh_bh.ToString());
                    update.AddEntityContent(wordPdfRequest, "[SOTIEN_BT]", sotien_bt.ToString("#,###", cul.NumberFormat));
                    update.AddEntityContent(wordPdfRequest, "[SOTIEN_BT_BC]", ContentHelper.NumberToText(sotien_bt));
                    update.AddEntityContent(wordPdfRequest, "[SOTIEN_TAMUNG]", sotien_tu.ToString("#,###", cul.NumberFormat));
                    double sotien_cl = sotien_bt - sotien_tu;
                    update.AddEntityContent(wordPdfRequest, "[SOTIEN_CONLAI]", sotien_cl.ToString("#,###", cul.NumberFormat));
                    update.AddEntityContent(wordPdfRequest, "[SOTIEN_CONLAI_BC]", ContentHelper.NumberToText(sotien_cl));
                    #endregion
                    #region ĐỒNG BẢO HIỂM
                    var pr_key_nvu_bht_ctu = _context_pias.NvuBhtCtus.Where(x => x.SoDonbhSdbs == hsgd_tt.SoDonbh).Select(s => s.PrKey).FirstOrDefault();
                    var kh = _context_pias.DmKhaches.Where(x => x.MaKh == "00.18000000").FirstOrDefault();
                    var dbh_byfrkey = _context_pias.NvuBhtDbhs.Where(x => x.FrKey == pr_key_nvu_bht_ctu).GroupBy(g => g.FrKey).Select(s => new
                    {
                        FrKey = s.Key,
                        TyleTg = s.Sum(x => x.TyleTg)
                    }).FirstOrDefault();
                    var dbh1 = ToListWithNoLock((from a in _context_pias.NvuBhtCtus
                                                     //join b in _context_pias.NvuBhtDbhs.Where(x=>x.FrKey == pr_key_nvu_bht_ctu).GroupBy(g=>g.FrKey).Select(s=> new
                                                     //{
                                                     //    FrKey = s.Key,
                                                     //    TyleTg = s.Sum(x=>x.TyleTg)
                                                     //}).AsQueryable() on a.PrKey equals b.FrKey into b1
                                                     //from b in b1.DefaultIfEmpty()
                                                 where a.PrKey == pr_key_nvu_bht_ctu
                                                 select new DongBaoHiem
                                                 {
                                                     MaKH = "00.18000000",
                                                     TenCtyBh = kh != null ? kh.TenKh : "",
                                                     TyleTg = (a.TyleDong != 0 ? a.TyleDong : 100) - (dbh_byfrkey != null ? dbh_byfrkey.TyleTg : 0) * (a.TyleDong != 0 ? a.TyleDong : 100) / 100,
                                                     TyleTaiho = 0,
                                                     VaiTro = a.TyleDong != 0 ? "Đồng phụ" : "Đồng chính"
                                                 }).AsQueryable());
                    var dbh2 = ToListWithNoLock((from a in _context_pias.NvuBhtCtus
                                                 where a.PrKey == pr_key_nvu_bht_ctu && a.TyleDong != 0
                                                 select new DongBaoHiem
                                                 {
                                                     MaKH = "",
                                                     TenCtyBh = "Tỷ lệ của nhà ĐBH chính",
                                                     TyleTg = 100 - a.TyleDong,
                                                     TyleTaiho = 0,
                                                     VaiTro = "Đồng chính"
                                                 }).AsQueryable());
                    var dbh3 = ToListWithNoLock((from a in _context_pias.NvuBhtDbhs
                                                 join b in _context_pias.NvuBhtCtus on a.FrKey equals b.PrKey
                                                 join c in _context_pias.DmKhaches on a.MaKhach equals c.MaKh into c1
                                                 from c in c1.DefaultIfEmpty()
                                                 where a.FrKey == pr_key_nvu_bht_ctu
                                                 select new DongBaoHiem
                                                 {
                                                     MaKH = a.MaKhach,
                                                     TenCtyBh = c != null ? c.TenKh : "",
                                                     TyleTg = a.TyleTg * (b.TyleDong != 0 ? b.TyleDong : 100) / 100,
                                                     TyleTaiho = a.TyleTaiho * (b.TyleDong != 0 ? b.TyleDong : 100) / 100,
                                                     VaiTro = a.VaiTro
                                                 }).AsQueryable());
                    var dbh = dbh1.Union(dbh2).Union(dbh3).ToList();
                    StringBuilder dbh_str = new StringBuilder();
                    if (dbh != null && dbh.Count > 0)
                    {
                        for (int i = 0; i < dbh.Count; i++)
                        {
                            dbh_str.AppendLine(dbh[i].TenCtyBh + " tham gia tỷ lệ " + Math.Round(dbh[i].TyleTg, 2) + "% với vai trò " + dbh[i].VaiTro);
                        }

                    }
                    List<string> list_dbh = ContentHelper.SplitString(dbh_str.ToString(), 255);
                    for (int i = 0; i < list_dbh.Count(); i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[NHA_DBH{i}]", list_dbh[i]);
                    }
                    for (int i = list_dbh.Count(); i < 4; i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[NHA_DBH{i}]", "");
                    }
                    #endregion
                    #region thu đòi người thứ ba
                    var hsbt_thts = _context_pias_update.HsbtThts.Where(x => x.FrKey == hsgd_tt.PrKeyBt && x.LoaiHinhtd == "TDNT3").FirstOrDefault();
                    var pan_giaiquyet_tdnt3 = "";
                    if (hsbt_thts != null)
                    {
                        pan_giaiquyet_tdnt3 = hsbt_thts.GhiChu;
                        update.AddEntityContent(wordPdfRequest, $"[TDNT3_CO]", checkMark.ToString());
                        update.AddEntityContent(wordPdfRequest, $"[TDNT3_KHONG]", emptyBox.ToString());
                    }
                    else
                    {
                        update.AddEntityContent(wordPdfRequest, $"[TDNT3_CO]", emptyBox.ToString());
                        update.AddEntityContent(wordPdfRequest, $"[TDNT3_KHONG]", checkMark.ToString());

                    }
                    List<string> list_pan_giaiquyet_tdnt3 = ContentHelper.SplitString(ContentHelper.formatNewLine(pan_giaiquyet_tdnt3), 255);
                    for (int i = 0; i < list_pan_giaiquyet_tdnt3.Count(); i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[PAN_GIAIQUYET_TDNT3{i}]", list_pan_giaiquyet_tdnt3[i]);
                    }
                    for (int i = list_pan_giaiquyet_tdnt3.Count(); i < 2; i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[PAN_GIAIQUYET_TDNT3{i}]", "");
                    }
                    #endregion
                    #region lấy thông tin thụ hưởng thanh toán
                    hsgd_totrinh_tt = ToListWithNoLock((from a in _context.HsgdTtrinhTt
                                                        where a.FrKey == prKey
                                                        select new ThuHuong
                                                        {
                                                            TenChuTk = a.TenChuTk,
                                                            SoTaikhoanNh = a.SoTaikhoanNh,
                                                            TenNh = a.TenNh,
                                                            LydoTt = a.LydoTt,
                                                            SotienTt = a.SotienTt
                                                        }).AsQueryable());
                    #endregion
                    update.AddEntityContent(wordPdfRequest, $"[DUYETBT_CO]", checkMark.ToString());
                    update.AddEntityContent(wordPdfRequest, $"[DUYETBT_KHONG]", emptyBox.ToString());

                    var ma_donvi_user = _context.DmUsers.Where(x => x.Mail == email).Select(s => s.MaDonvi).FirstOrDefault();
                    var hsgd_tpc = _context.HsgdCtus.Where(x => x.PrKey == hsgd_tt.PrKeyHsgd).Select(s => s.HsgdTpc).FirstOrDefault();
                    if (hsgd_tpc == 1 && new[] { "00", "31", "32" }.Contains(ma_donvi_user))
                    {
                        update.AddEntityContent(wordPdfRequest, "[LBL_NG_KY]", "LÃNH ĐẠO VPCSKH");
                        update.AddEntityContent(wordPdfRequest, "[LBL_PHONGGQ]", "PHÒNG GQKN XCG");
                    }
                    else
                    {
                        update.AddEntityContent(wordPdfRequest, "[LBL_NG_KY]", "LÃNH ĐẠO ĐƠN VỊ");
                        update.AddEntityContent(wordPdfRequest, "[LBL_PHONGGQ]", "PHÒNG GĐBT/GQKN");
                    }

                }

                var listData = wordPdfRequest.ListData;
                _logger.Information("PrintToTrinh " + JsonConvert.SerializeObject(listData));
                var listNew = new CombinedTtrinhResult4
                {
                    ThirdQueryResults = listData,
                    ListGiamDinh = hsbt_gd,
                    ListThuHuong = hsgd_totrinh_tt,
                    ChkChuanopphi = hsgd_tt.ChkChuanopphi
                };

                return listNew;

            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                return null;
            }
        }
        public ListHsgdTtrinh GetListTtrinh(decimal pr_key_hsgd)
        {
            var ListHsgdTtrinh = new ListHsgdTtrinh();
            int Songay_TtGara = 0;
            var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == pr_key_hsgd).Select(n => new
            {
                SoHsgd = n.SoHsgd,
                TenKhach = n.TenKhach,
                DienThoaiNdbh = n.DienThoaiNdbh,
                PrKeyBt = n.PrKeyBt,
                SoTienThucTe = n.SoTienThucTe,
                MaGaraVcx = n.MaGaraVcx,
                MaGaraTnds = n.MaGaraTnds
            }).FirstOrDefault();
            if (hsgd_ctu != null)
            {
                ListHsgdTtrinh.SoHsgd = hsgd_ctu.SoHsgd;
                var so_hsbt = _context_pias_update.HsbtCtus.Where(x => x.PrKey == hsgd_ctu.PrKeyBt).Select(n => n.SoHsbt).FirstOrDefault();
                ListHsgdTtrinh.SoHsbt = so_hsbt;
                //Lấy số ngày thanh toán ở gara để đẩy vào HsgdTtrinhView để tính toán hạn thanh toán khi nhập ngày nhận đủ hồ sơ tài liệu
                if (!string.IsNullOrEmpty(hsgd_ctu.MaGaraVcx))
                {
                    Songay_TtGara = (int)_context.DmGaRas.Where(x => x.MaGara == hsgd_ctu.MaGaraVcx).Select(n => n.SongayThanhtoan).FirstOrDefault();
                }
                else if (!string.IsNullOrEmpty(hsgd_ctu.MaGaraTnds))
                {
                    Songay_TtGara = (int)_context.DmGaRas.Where(x => x.MaGara == hsgd_ctu.MaGaraTnds).Select(n => n.SongayThanhtoan).FirstOrDefault();
                }
                ListHsgdTtrinh.listHsgdTtrinhView = ToListWithNoLock((from a in _context.HsgdTtrinhs
                                                                      join b in _context.DmTtrangTtrinhs on a.MaTtrang equals b.MaTtrangTt into b1
                                                                      from b in b1.DefaultIfEmpty()
                                                                      where (
                                                                      a.PrKeyHsgd == pr_key_hsgd
                                                                      )
                                                                      select new HsgdTtrinhView
                                                                      {
                                                                          PrKey = a.PrKey,
                                                                          Oid = a.Oid,
                                                                          MaDonvi = a.MaDonvi,
                                                                          SoHsbt = a.SoHsbt,
                                                                          TenDttt = a.TenDttt,
                                                                          NgGdich = a.NgGdich,
                                                                          NgayCtu = a.NgayCtu != null ? Convert.ToDateTime(a.NgayCtu).ToString("dd/MM/yyyy") : null,
                                                                          NgayTthat = a.NgayTthat != null ? Convert.ToDateTime(a.NgayTthat).ToString("dd/MM/yyyy") : null,
                                                                          SoTien = a.SoTien,
                                                                          MaTtrang = a.MaTtrang == "" ? "00" : a.MaTtrang,
                                                                          PathTtrinh = a.PathTtrinh,
                                                                          PrKeyCt = a.PrKeyCt,
                                                                          NguyenNhan = a.NguyenNhan,
                                                                          HauQua = a.HauQua,
                                                                          TaisanThuhoi = a.TaisanThuhoi,
                                                                          PanThoiTs = a.PanThoiTs,
                                                                          GiatriThuhoi = a.GiatriThuhoi,
                                                                          GtrinhChikhac = a.GtrinhChikhac,
                                                                          ChiKhac = a.ChiKhac,
                                                                          PrKeyHsgd = a.PrKeyHsgd,
                                                                          TenTtrangTt = b.TenTtrangTt != null ? b.TenTtrangTt : "",
                                                                          SoBthuong = a.SoBthuong,
                                                                          SoNgchet = a.SoNgchet,
                                                                          ThamGia007 = a.ThamGia007,
                                                                          SoPhibh = a.SoPhibh,
                                                                          NgayThuphi = a.NgayThuphi,
                                                                          ChkDunghan = a.ChkDunghan,
                                                                          ChkDaydu = a.ChkDaydu,
                                                                          ChkChuanopphi = a.ChkChuanopphi,
                                                                          ChkTheohopdong = a.ChkTheohopdong,
                                                                          NgayDuTlieu = a.NgayDuTlieu,
                                                                          NgayTtoan = a.NgayTtoan,
                                                                          Songay_TtGara = Songay_TtGara
                                                                      }).OrderBy(x => x.MaTtrang).ThenByDescending(x => x.PrKey).AsQueryable());
                ListHsgdTtrinh.listHsgdTtrinhView.ForEach(a =>
                {
                    a.DienThoaiNdbh = hsgd_ctu.DienThoaiNdbh;
                    a.TenKhach = hsgd_ctu.TenKhach;
                    a.SoTienThucTe = hsgd_ctu.SoTienThucTe;
                });
            }
            return ListHsgdTtrinh;
        }
        public HsgdTtrinhDetail GetTtrinhById(decimal pr_key)
        {
            HsgdTtrinhDetail obj = new HsgdTtrinhDetail();
            try
            {
                obj.hsgdTtrinh = FirstOrDefaultWithNoLock(_context.HsgdTtrinhs.Where(x => x.PrKey == pr_key).Select(n => new HsgdTtrinhView
                {
                    PrKey = n.PrKey,
                    MaDonvi = n.MaDonvi,
                    SoHsbt = n.SoHsbt,
                    TenDttt = n.TenDttt,
                    NgGdich = n.NgGdich,
                    NgayCtu = n.NgayCtu != null ? Convert.ToDateTime(n.NgayCtu).ToString("dd/MM/yyyy") : null,
                    NgayTthat = n.NgayTthat != null ? Convert.ToDateTime(n.NgayTthat).ToString("dd/MM/yyyy") : null,
                    SoTien = n.SoTien,
                    MaTtrang = n.MaTtrang,
                    PathTtrinh = n.PathTtrinh,
                    PrKeyCt = n.PrKeyCt,
                    NguyenNhan = n.NguyenNhan,
                    HauQua = n.HauQua,
                    TaisanThuhoi = n.TaisanThuhoi,
                    PanThoiTs = n.PanThoiTs,
                    GiatriThuhoi = n.GiatriThuhoi,
                    GtrinhChikhac = n.GtrinhChikhac,
                    ChiKhac = n.ChiKhac,
                    PrKeyHsgd = n.PrKeyHsgd,
                    SoBthuong = n.SoBthuong,
                    SoNgchet = n.SoNgchet,
                    ThamGia007 = n.ThamGia007,
                    SoPhibh = n.SoPhibh,
                    NgayThuphi = n.NgayThuphi,
                    ChkDaydu = n.ChkDaydu,
                    ChkDunghan = n.ChkDunghan,
                    ChkChuanopphi = n.ChkChuanopphi,
                    ChkTheohopdong = n.ChkTheohopdong,
                    NgayDuTlieu = n.NgayDuTlieu,
                    NgayTtoan = n.NgayTtoan,
                    Songay_TtGara = 0

                }).AsQueryable());

                obj.hsgdThuHuong = ToListWithNoLock(_context.HsgdTtrinhTt.Where(x => x.FrKey == pr_key).Select(n => new HsgdThuHuongView
                {
                    PrKey = n.PrKey,
                    FrKey = n.FrKey,
                    TenChuTk = n.TenChuTk,
                    SoTaikhoanNh = n.SoTaikhoanNh,
                    TenNh = n.TenNh,
                    SotienTt = n.SotienTt,
                    LydoTt = n.LydoTt,
                    bnkCode=n.bnkCode
                    
                }).AsQueryable());

                if (obj.hsgdTtrinh != null)
                {
                    obj.hsgdTtrinhCt = ToListWithNoLock((from a in _context.HsgdTtrinhCts
                                                         join b in _context.HsgdTotrinhXmls on a.PrKey equals b.FrKey into b1
                                                         from b in b1.DefaultIfEmpty()
                                                         where a.FrKey == pr_key
                                                         select new HsgdTtrinhCtView
                                                         {
                                                             PrKey = a.PrKey,
                                                             FrKey = a.FrKey,
                                                             MaSp = a.MaSp,
                                                             SotienBh = a.SotienBh,
                                                             SotienBt = a.SotienBt,
                                                             SotienTu = a.SotienTu,
                                                             TinhToanbt = a.TinhToanbt,
                                                             MucVat = a.MucVat,
                                                             SoTienBtVat = a.SoTienBtVat,
                                                             PrKeyXml = b != null ? b.PrKey : 0,
                                                             PathXml = b != null ? b.PathXml : "",
                                                             TenFile = b != null ? b.TenFile : "",
                                                             maDKhoan =  a.MaDKhoan ?? ""
                                                         }).AsQueryable());
                    var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == obj.hsgdTtrinh.PrKeyHsgd).FirstOrDefault();
                    if (hsgd_ctu != null)
                    {
                        obj.hsgdTtrinh.TenKhach = hsgd_ctu.TenKhach;
                        obj.hsgdTtrinh.DienThoaiNdbh = hsgd_ctu.DienThoaiNdbh;
                        obj.hsgdTtrinh.SoTienThucTe = hsgd_ctu.SoTienThucTe;
                        //Lấy số ngày thanh toán ở gara để đẩy vào HsgdTtrinhView để tính toán hạn thanh toán khi nhập ngày nhận đủ hồ sơ tài liệu
                        if (!string.IsNullOrEmpty(hsgd_ctu.MaGaraVcx))
                        {
                            obj.hsgdTtrinh.Songay_TtGara = (int)_context.DmGaRas.Where(x => x.MaGara == hsgd_ctu.MaGaraVcx).Select(n => n.SongayThanhtoan).FirstOrDefault();
                        }
                        else if (!string.IsNullOrEmpty(hsgd_ctu.MaGaraTnds))
                        {
                            obj.hsgdTtrinh.Songay_TtGara = (int)_context.DmGaRas.Where(x => x.MaGara == hsgd_ctu.MaGaraTnds).Select(n => n.SongayThanhtoan).FirstOrDefault();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return obj;
        }

        public SeriPhiBH GetSoPhiBH(string so_donbh, decimal so_seri)
        {
            //var seri_ct = ToListWithNoLock((from A in _context_pias.NvuBhtCtus
            //               join B in _context_pias.NvuBhtSeris on A.PrKey equals B.FrKey
            //               join C in _context_pias.NvuBhtSeriCts on B.PrKey equals C.FrKey
            //               where A.SoDonbh == so_donbh && B.SoSeri == so_seri && C.MaSp == "050104"
            //               select new
            //               {
            //                   TongTien = new[] { "02", "03" }.Contains(A.MaSdbs) ? (-1 * B.TongTien) : A.MaSdbs == "05" ? 0 : B.TongTien,
            //                   MtnGtbhVnd = new[] { "02", "03" }.Contains(A.MaSdbs) ? ( -1 * (C.MtnGtbhVnd>0? C.MtnGtbhVnd:C.MtnGtbhTsan)) : A.MaSdbs == "05" ? 0 : (C.MtnGtbhVnd > 0 ? C.MtnGtbhVnd : C.MtnGtbhTsan)
            //               }).AsQueryable());
            //_logger.Information("GetSoPhiBH so_donbh =" + so_donbh + " so_seri =" + so_seri);
            //_logger.Information("GetSoPhiBH seri_ct =" + JsonConvert.SerializeObject(seri_ct));
            //var phibh = seri_ct.GroupBy(g => 1 == 1)
            //            .Select(s => new SeriPhiBH
            //            {
            //                TongTien = s.Sum(x => x.TongTien),
            //                MtnGtbhVnd = s.Sum(x => x.MtnGtbhVnd)
            //            }).FirstOrDefault();
            var phibh = Get_SoPhiBH(so_donbh, so_seri);
            return phibh;

        }
        public CheckDKBS007 CheckDKBS007(decimal pr_key_hsgd)
        {
            CheckDKBS007 BS007 = new CheckDKBS007();
            var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == pr_key_hsgd).FirstOrDefault();
            if (hsgd_ctu != null)
            {
                var so_the = _context_pias_update.HsbtCtus.Where(x => x.PrKey == hsgd_ctu.PrKeyBt).Select(n => n.SotheVcxMoto).FirstOrDefault();
                var list_sr = ToListWithNoLock((from A in _context_pias.NvuBhtCtus
                                                join B in _context_pias.NvuBhtSeris on A.PrKey equals B.FrKey
                                                join C in _context_pias.NvuBhtSeriCts on B.PrKey equals C.FrKey
                                                where A.PrKey == hsgd_ctu.PrKeyGoc && B.SoSeri == hsgd_ctu.SoSeri && B.SoThe == so_the
                                                select new
                                                {
                                                    B.PrKey,
                                                    C.MaSp,
                                                    C.MtnGtbhNte,
                                                    C.MtnGtbhVnd,
                                                    C.MaTtep
                                                }).AsQueryable());
                if (list_sr.Count() > 0)
                {

                    var count_srdk = (from A in _context_pias.NvuBhtSeriDks
                                      where A.FrKey == list_sr[0].PrKey
                                      select A.PrKey).Count();
                    if (count_srdk > 0)
                    {
                        BS007.DKBS007 = true;
                    }
                    else
                    {
                        BS007.DKBS007 = false;
                    }
                }
                else
                {
                    BS007.DKBS007 = false;
                }
                //Lấy số ngày thanh toán ở gara để đẩy vào HsgdTtrinhView để tính toán hạn thanh toán khi nhập ngày nhận đủ hồ sơ tài liệu
                var soNgayThanhToan = (
                                        from a in _context.HsgdCtus
                                        join b in _context.HsgdDxCts on a.PrKeyBt equals b.PrKeyHsbtCtu
                                        join c in _context.DmGaRas on b.MaGara equals c.MaGara
                                        where a.PrKey == pr_key_hsgd
                                        select c.SongayThanhtoan
                                    ).FirstOrDefault();

                BS007.Songay_TtGara = soNgayThanhToan;               

            }
            else
            {
                BS007.DKBS007 = false;
                BS007.Songay_TtGara = 0;
            }

            return BS007;
        }
        public string UpdateTrangThaiHsbtCt(decimal pr_key_hsgd_ttrinh)
        {
            var result = "";
            try
            {
                var so_hsbt = _context.HsgdTtrinhs.Where(x => x.PrKey == pr_key_hsgd_ttrinh).Select(s => s.SoHsbt).FirstOrDefault();
                var tt_sp = _context.HsgdTtrinhCts.Where(x => x.FrKey == pr_key_hsgd_ttrinh).Select(s => s.MaSp).ToList();
                if (tt_sp != null && tt_sp.Count > 0)
                {
                    var pr_key_hsbt = _context_pias_update.HsbtCtus.Where(x => x.SoHsbt == so_hsbt).Select(s => s.PrKey).FirstOrDefault();
                    var hsbt_ct = _context_pias_update.HsbtCts.Where(x => x.FrKey == pr_key_hsbt && tt_sp.Contains(x.MaSp) && x.MaTtrangBt == "01").ToList();
                    if (hsbt_ct.Count > 0)
                    {
                        hsbt_ct.ForEach(a => a.MaTtrangBt = "02");
                        _context_pias_update.HsbtCts.UpdateRange(hsbt_ct);
                        _context_pias_update.SaveChanges();
                        result = "Thành công";


                        //TODO netcore 7.0
                        //_context_pias_update.HsbtCts
                        //    .Where(x => x.FrKey == pr_key_hsbt && tt_sp.Contains(x.MaSp) && x.MaTtrangBt == "01")
                        //    .ExecuteUpdate(s => s.SetProperty(u => u.MaTtrangBt, "02"));
                    }
                    else
                    {
                        result = "Không có dữ liệu hoặc mã sản phẩm không tương thích";
                        _logger.Error("UpdateTrangThaiHsbtCt pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " không có dữ liệu hoặc mã sản phẩm không tương thích ");
                    }
                    var hsbt_gd = _context_pias_update.HsbtGds.Where(x => x.FrKey == pr_key_hsbt && tt_sp.Contains(x.MaSp) && x.MaTtrangGd == "01").ToList();
                    if (hsbt_gd.Count > 0)
                    {
                        hsbt_gd.ForEach(a => a.MaTtrangGd = "02");
                        _context_pias_update.HsbtGds.UpdateRange(hsbt_gd);
                        _context_pias_update.SaveChanges();                      


                    }                  
                }
                else
                {
                    result = "Không tìm thấy tờ trình";
                }
            }
            catch (Exception ex)
            {
                result = "Thất bại";
                _logger.Error("UpdateTrangThaiHsbtCt pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " error " + ex);
            }
            return result;
        }
        public string InsertHsgdAttachFile(decimal pr_key_hsgd_ttrinh)
        {
         
            try
            {               
                var result_totrinh = (
                            from a in _context.HsgdCtus
                            join b in _context.HsgdTtrinhs on a.PrKey equals b.PrKeyHsgd
                            join c in _context.HsgdTtrinhNkies on b.PrKey equals c.FrKey
                            where (b.MaTtrang == "09" || b.MaTtrang == "14")
                                  && b.PathTtrinh != ""
                                  && c.Act == "KyHoSo"
                                  && b.PrKey == pr_key_hsgd_ttrinh
                            group new { a, b, c } by new { a.PrKey, b.PathTtrinh } into g
                            select new
                            {
                                pr_key = Guid.NewGuid().ToString().ToLower(),
                                fr_key = g.Key.PrKey,
                                ma_ctu = "TTBT",
                                file_name = g.Key.PathTtrinh.Substring(
                                    g.Key.PathTtrinh.LastIndexOf("\\") + 1
                                ),
                                directory = g.Key.PathTtrinh,
                                ngay_cnhat = g.Max(x => x.c.NgayCnhat),
                                ghi_chu = "Cập nhật từ ký tờ trình",
                                nguon_tao = "WebPvi247"
                            }
                        ).ToList();
                // Tạo danh sách entity để lưu
                List<HsgdAttachFile> attachFiles = new List<HsgdAttachFile>();
                if (result_totrinh.Any())
                {
                   
                    foreach (var item in result_totrinh)
                    {
                        var atf = new HsgdAttachFile
                        {
                            PrKey = item.pr_key,
                            FrKey = item.fr_key,
                            MaCtu = item.ma_ctu,
                            FileName = item.file_name,
                            Directory = item.directory,
                            ngay_cnhat = item.ngay_cnhat,
                            GhiChu = item.ghi_chu,
                            NguonTao = item.nguon_tao
                        };

                        attachFiles.Add(atf);
                    }
                   
                }
                var prKeyHsgd = (from t in _context.HsgdTtrinhs where t.PrKey == pr_key_hsgd_ttrinh
                                 select t.PrKeyHsgd
                                 ).FirstOrDefault();
                //lấy ảnh duyệt giá, vppb thì lấy ảnh duyệt giá đầu, vppn lấy ảnh duyệt giá cuối
                var result_duyetgia = (
                                from a in _context.HsgdCtus
                                join b in _context.HsgdDgs on a.PrKey equals b.FrKey
                                join c in _context.HsgdDgCts on b.PrKey equals c.FrKey
                                where a.PrKey == prKeyHsgd
                                      && b.LoaiDg == (a.MaDonvigd == "31" ? false : true)
                                select new
                                {
                                    pr_key = Guid.NewGuid().ToString().ToLower(),
                                    fr_key = a.PrKey,
                                    ma_ctu = "PADG",
                                    file_name = "PADG.jpg",
                                    directory = c.PathFile,
                                    ngay_cnhat =DateTime.Now,
                                    ghi_chu = "Ảnh duyệt giá",
                                    nguon_tao = "WebPvi247"
                                }
                            ).ToList();
                if (result_duyetgia.Any())
                {

                    foreach (var item in result_duyetgia)
                    {
                        var atf = new HsgdAttachFile
                        {
                            PrKey = item.pr_key,
                            FrKey = item.fr_key,
                            MaCtu = item.ma_ctu,
                            FileName = item.file_name,
                            Directory = item.directory,
                            ngay_cnhat = item.ngay_cnhat,
                            GhiChu = item.ghi_chu,
                            NguonTao = item.nguon_tao
                        };

                        attachFiles.Add(atf);
                    }

                }
                if (attachFiles.Any())
                {
                    // Add vào context
                    _context.HsgdAttachFiles.AddRange(attachFiles);
                    // Lưu thay đổi vào database
                    _context.SaveChanges();
                    return "Thành công"; ;
                }    
            }
            catch (Exception ex)
            {
                return "Thất bại";
                _logger.Error("InsertHsgdAttachFile pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " error " + ex);
            }
            return "";
        }
        public string Update_hoanthien_hstt(decimal pr_key_hsgd_ttrinh)
        {
            try
            {               
                var Hsgd_ctu = _context.HsgdCtus
                     .FirstOrDefault(c => c.PrKey == _context.HsgdTtrinhs
                                             .Where(t => t.PrKey == pr_key_hsgd_ttrinh)
                                             .Select(t => t.PrKeyHsgd)
                                             .FirstOrDefault());

                if (Hsgd_ctu != null)
                {
                    if (Hsgd_ctu.MaDonvigd == "31")
                    {
                        Hsgd_ctu.HoanThienHstt = true;
                        //Chạy thead gửi email
                        Task.Run(() => SendEmail_QLNV_GDV_HOANTHIENHSTT(Hsgd_ctu.PrKey));
                    } 
                    _context.SaveChanges();
                    return "Thành công";
                }                
            }
            catch (Exception ex)
            {
                return "Thất bại";               
            }
            return "";
        }

        public string ChuyenDuyet(decimal pr_key_hsgd_ttrinh, string email_login, string oid_nhan, bool send_email)
        {
            string result = "";
            try
            {
                var user_login = _context.DmUsers.Where(x => x.Mail == email_login).Join(_context.DmLoaiUsers, a => a.LoaiUser, b => b.LoaiUser, (a, b) => new { a.Oid, a.LoaiUser, a.MaUser, a.TenUser, b.TenLoaiUser }).FirstOrDefault();
                var user_nhan = _context.DmUsers.Where(x => x.Oid == Guid.Parse(oid_nhan)).Join(_context.DmLoaiUsers, a => a.LoaiUser, b => b.LoaiUser, (a, b) => new { a.Oid, a.Mail, a.LoaiUser, a.MaUser, a.TenUser, b.TenLoaiUser }).FirstOrDefault();
                var hsgd_tt = _context.HsgdTtrinhs.Where(x => x.PrKey == pr_key_hsgd_ttrinh).FirstOrDefault();
                var user_cuoi = _context.HsgdTtrinhNkies.Where(x => x.FrKey == pr_key_hsgd_ttrinh).OrderByDescending(x => x.PrKey).Select(s => s.UserNhan).FirstOrDefault() ?? "";
                //if (!user_cuoi.ToLower().Equals(user_login.Oid.ToString().ToLower()))
                //{
                //    result = "Tờ trình này không được chuyển đến bạn. Bạn không có quyền thao tác, vui lòng kiểm tra lại";
                //    return result;
                //}
                if (hsgd_tt != null)
                {

                    if (hsgd_tt.MaTtrang != "01")
                    {
                        result = "Tờ trình đã qua bước chuyển duyệt.Vui lòng kiểm tra lại";
                        return result;
                    }
                    else
                    {
                        if (user_login.LoaiUser == 4 || user_login.LoaiUser == 8)
                        {
                            //var check_kyhs = CheckKyHoSo(hsgd_tt.SoHsbt, user_login.Oid.ToString());
                            //if (check_kyhs == 1)
                            //{
                            //    result = "Mã sản phẩm không có trong cấu hình quyền ký hồ sơ. vui lòng kiểm tra lại!";
                            //    return result;
                            //}
                            //else if (check_kyhs == 3)
                            //{
                            //    result = "Tài khoản chưa được cấu hình quyền ký hồ sơ hoặc chưa được kích hoạt tài khoản. vui lòng kiểm tra lại!";
                            //    return result;
                            //}

                            // hồ sơ giám định đã được duyệt, thì mới được chuyển duyệt tờ trình
                            if (hsgd_tt.PrKeyHsgd > 0)
                            {
                                var MaTtrangGd = _context.HsgdCtus.Where(x => x.PrKey == hsgd_tt.PrKeyHsgd).Select(s => s.MaTtrangGd).FirstOrDefault();
                                var chk_tnds = _context.HsgdTtrinhCts.Where(x => x.FrKey == pr_key_hsgd_ttrinh && x.MaSp != "050101").Count();
                                if (chk_tnds > 0 && MaTtrangGd != "6")
                                {
                                    result = "Hồ sơ giám định chưa đc duyệt. Không chuyển duyệt được tờ trình.";
                                    return result;
                                }
                            }

                            // thực hiện replace text CB xử lý
                            string url_download = _configuration["DownloadSettings:DownloadServer_MDF1"] ?? "";
                            //tải file
                            string Path_orgin = UtilityHelper.getPathAndCopyTempServer(hsgd_tt.PathTtrinh, url_download);
                            string Path_result = Path_orgin + "_edited.pdf";
                            if (System.IO.File.Exists(Path_orgin))
                            {
                                _logger.Information("ChuyenDuyet pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " tải file thành công");
                                var pdfEdit = new PDFEdit(_logger);
                                // pdfEdit.ReplaceTextInPDF(Path_orgin, Path_result, pdfEdit.ListKeyWord("CB_TAT", user_login.MaUser ?? "", user_login.TenUser ?? ""), true);
                                if (pdfEdit.ReplaceTextInPDF(Path_orgin, Path_result, pdfEdit.ListKeyWord("CB_TAT", user_login.MaUser ?? "", user_login.TenUser ?? ""), true))
                                {
                                    _logger.Information("ChuyenDuyet pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " ReplaceTextInPDF CB_TAT thành công");
                                }
                                else
                                {
                                    result = "Chuyển duyệt hồ sơ không thành công";
                                    _logger.Information("ChuyenDuyet pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " ReplaceTextInPDF CB_TAT thất bại");
                                    return result;
                                }
                                // _logger.Information("ChuyenDuyet pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " ReplaceTextInPDF thành công");
                                //upload file sau replace text
                                //var FileInfo = new FileInfo(Path_orgin);
                                //var extn = FileInfo.Extension;
                                var utilityHelper = new UtilityHelper(_logger);
                                string folderUpload = _configuration["UploadSettings:FolderUpload_MDF1"] ?? "";
                                string url_upload = _configuration["DownloadSettings:UlpoadServer_MDF1"] ?? "";
                                var file_path = utilityHelper.UploadFileOld_ToAPI(Convert.ToBase64String(System.IO.File.ReadAllBytes(Path_orgin)), hsgd_tt.PathTtrinh, folderUpload, url_upload);
                                _logger.Information("ChuyenDuyet pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " upload file tờ trình sau ReplaceTextInPDF thành công");
                                // kiểm tra và xóa file ở local 
                                try
                                {
                                    if (System.IO.File.Exists(Path_orgin))
                                    {
                                        System.IO.File.Delete(Path_orgin);
                                    }
                                    if (System.IO.File.Exists(Path_result))
                                    {
                                        System.IO.File.Delete(Path_result);
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                                //update nhật ký
                                string trang_thai_tt = "08";
                                string ten_trang_tt = "Cán bộ đã duyệt/Chờ TP duyệt bồi thường (cán bộ xử lý bấm trình ký)";
                                var todaysdate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                                string noidung_upd = user_login.TenLoaiUser + " " + user_login.TenUser + " chuyển hồ sơ lên " + user_nhan.TenLoaiUser + " " + user_nhan.TenUser + " duyệt ngày " + todaysdate;

                                HsgdTtrinhNky nky = new HsgdTtrinhNky();
                                nky.FrKey = pr_key_hsgd_ttrinh;
                                nky.UserChuyen = user_login.Oid.ToString();
                                nky.UserNhan = user_nhan.Oid.ToString();
                                nky.GhiChu = noidung_upd;
                                nky.NgayCnhat = DateTime.Now;
                                nky.Act = "ChuyenDuyet";
                                var res = UpdateNKyTtrinh(pr_key_hsgd_ttrinh, trang_thai_tt, nky, "ChuyenDuyet");
                                if (res)
                                {
                                    if (send_email)
                                    {
                                        var email = user_nhan.Mail;
                                        if (!string.IsNullOrEmpty(email))
                                        {
                                            SendEmail_ToTrinh(true, email, "Xin ý kiến phê duyệt tờ trình", user_nhan.TenUser ?? "",
                            todaysdate, hsgd_tt.SoHsbt, hsgd_tt.SoTien.ToString("#,###", CultureInfo.GetCultureInfo("vi-VN").NumberFormat) + " VNĐ", ten_trang_tt, noidung_upd, hsgd_tt.PrKeyHsgd, "");
                                        }
                                    }
                                    result = "Chuyển duyệt hồ sơ thành công";
                                    _logger.Information("ChuyenDuyet pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " thành công");
                                }
                                else
                                {
                                    result = "Chuyển duyệt hồ sơ thất bại";
                                    _logger.Information("ChuyenDuyet pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " thất bại");
                                }

                            }
                            else
                            {

                                result = "Lỗi không tải được file tờ trình";
                                _logger.Information("ChuyenDuyet pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " lỗi không tải được file tờ trình");
                            }
                        }
                        else
                        {
                            result = "User là Giám định viên hoặc Cán bộ trung tâm mới thực hiện chức năng này. ";
                            _logger.Information("ChuyenDuyet pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " lỗi không phải Giám định viên hoặc Cán bộ trung tâm");
                        }

                    }


                }
                else
                {
                    result = "Không tồn tại tờ trình";
                    _logger.Information("ChuyenDuyet pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " không tồn tại tờ trình");
                }
            }
            catch (Exception ex)
            {
                _logger.Information("ChuyenDuyet pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " error: " + ex);
            }
            return result;
        }
        public string KyHoSo(decimal pr_key_hsgd_ttrinh, string email_login)
        {
            string result = "";
            try
            {
                var user_login = _context.DmUsers.Where(x => x.Mail == email_login).Join(_context.DmLoaiUsers, a => a.LoaiUser, b => b.LoaiUser, (a, b) => new { a.Oid, a.LoaiUser, a.MaUser, a.TenUser, a.MaDonvi, b.TenLoaiUser }).FirstOrDefault();
                //var user_nhan = _context.DmUsers.Where(x => x.Oid == Guid.Parse(oid_nhan)).Join(_context.DmLoaiUsers, a => a.LoaiUser, b => b.LoaiUser, (a, b) => new { a.Oid, a.LoaiUser, a.MaUser, a.TenUser, a.MaDonvi, b.TenLoaiUser }).FirstOrDefault();
                var hsgd_tt = _context.HsgdTtrinhs.Where(x => x.PrKey == pr_key_hsgd_ttrinh).FirstOrDefault();
                var user_cuoi = _context.HsgdTtrinhNkies.Where(x => x.FrKey == pr_key_hsgd_ttrinh).OrderByDescending(x => x.PrKey).Select(s => s.UserNhan).FirstOrDefault() ?? "";
                if (!user_cuoi.ToLower().Equals(user_login.Oid.ToString().ToLower()))
                {
                    result = "Tờ trình này không được chuyển đến bạn. Bạn không có quyền thao tác, vui lòng kiểm tra lại";
                    return result;
                }
                if (hsgd_tt != null)
                {
                    if (hsgd_tt.MaTtrang == "01")
                    {
                        result = "Tờ trình chưa được Giám định viên/ Cán bộ trung tâm chuyển duyệt. Vui lòng kiểm tra lại";
                        return result;
                    }
                    else if (hsgd_tt.MaTtrang == "09" || hsgd_tt.MaTtrang == "14")
                    {
                        result = "Tờ trình có trạng thái Đã duyệt TPC/Chờ duyệt kế toán thanh toán không thực hiện được. Vui lòng kiểm tra lại";
                        return result;
                    }
                    else if (hsgd_tt.MaTtrang == "15")
                    {
                        result = "Tờ trình có trạng thái Đã hủy. Vui lòng kiểm tra lại";
                        return result;
                    }
                    else
                    {
                        if (new[] { 4, 8, 3, 9, 11, 2, 10 }.Contains(user_login.LoaiUser ?? 0))
                        {
                            var check_kyhs = CheckKyHoSo(hsgd_tt.SoHsbt, user_login.Oid.ToString());
                            if (check_kyhs == 1)
                            {
                                result = "Mã sản phẩm không có trong cấu hình quyền ký hồ sơ. Vui lòng kiểm tra lại!";
                                return result;
                            }
                            else if (check_kyhs == 2)
                            {
                                result = "Số tiền duyệt nhỏ hơn số tiền trong cấu hình quyền ký hồ sơ. Vui lòng kiểm tra lại!";
                                return result;
                            }
                            else if (check_kyhs == 3)
                            {
                                result = "Tài khoản chưa được cấu hình quyền ký hồ sơ hoặc chưa được kích hoạt tài khoản. Vui lòng kiểm tra lại!";
                                return result;
                            }

                            //ký số hồ sơ
                            // soap pias
                            var ws = new ServiceReference1.PiasSoapSoapClient(ServiceReference1.PiasSoapSoapClient.EndpointConfiguration.PiasSoapSoap, _configuration["client:endpoint:address"]);

                            string strSQL = "select top 1 * from hddt_hsm where ma_donvi = '" + user_login.MaDonvi + "' and ngay_hluc < getdate() order by ngay_hluc desc ";
                            var esign = ws.SelectSQL_HDDT(DateTime.Now.Year.ToString(), strSQL, "hddt_hsm");

                            var ds_esign = ConvetXMLToDataset(esign);
                            if (ds_esign != null && ds_esign.Tables.Count > 0 && ds_esign.Tables[0].Rows.Count > 0)
                            {
                                _logger.Information("KyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " select hddt_hsm thành công");
                                var partitionAlias = ds_esign.Tables[0].Rows[0].Field<string>("partition_alias");
                                var privateKeyAlias = ds_esign.Tables[0].Rows[0].Field<string>("private_key_alias");
                                var password = ds_esign.Tables[0].Rows[0].Field<string>("password");
                                var partitionSerial = ds_esign.Tables[0].Rows[0].Field<string>("partition_serial");
                                if (ws.KyToTrinhXCG(hsgd_tt.PathTtrinh, privateKeyAlias))
                                {
                                    _logger.Information("KyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " SignPDF_HILO thành công");
                                    // thực hiện replace text
                                    string url_download = _configuration["DownloadSettings:DownloadServer_MDF1"] ?? "";
                                    //tải file
                                    string Path_orgin = UtilityHelper.getPathAndCopyTempServer(hsgd_tt.PathTtrinh, url_download);
                                    string Path_result = Path_orgin + "_edited.pdf";
                                    if (System.IO.File.Exists(Path_orgin))
                                    {
                                        _logger.Information("KyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " tải file thành công");
                                        var pdfEdit = new PDFEdit(_logger);
                                        var check_chuyenky = _context.HsgdTtrinhNkies.Where(x => x.FrKey == pr_key_hsgd_ttrinh && x.Act == "ChuyenKyHoSo").Count();

                                        if (check_chuyenky == 0)
                                        {
                                            if (pdfEdit.ReplaceTextInPDF(Path_orgin, Path_result, pdfEdit.ListKeyWord("TP_TAT", user_login.MaUser ?? "", user_login.TenUser ?? ""), true))
                                            {
                                                _logger.Information("KyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " ReplaceTextInPDF TP_TAT thành công");
                                            }
                                            else
                                            {
                                                result = "Ký hồ sơ không thành công";
                                                _logger.Information("KyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " ReplaceTextInPDF TP_TAT thất bại");
                                                return result;
                                            }
                                        }
                                        //else
                                        //{
                                        //    Path_result = Path_orgin;
                                        //}
                                        // string Path_result_2 = Path_result + "_edited.pdf";
                                        if (pdfEdit.ReplaceTextInPDF(Path_orgin, Path_result, pdfEdit.ListKeyWord("LD_TAT", user_login.MaUser ?? "", user_login.TenUser ?? ""), true))
                                        {
                                            _logger.Information("KyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " ReplaceTextInPDF LD_TAT thành công");
                                        }
                                        else
                                        {
                                            result = "Ký hồ sơ không thành công";
                                            _logger.Information("KyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " ReplaceTextInPDF LD_TAT thất bại");
                                            return result;
                                        }
                                        //upload file sau replace text
                                        //var FileInfo = new FileInfo(Path_orgin);
                                        //var extn = FileInfo.Extension;
                                        var utilityHelper = new UtilityHelper(_logger);
                                        string folderUpload = _configuration["UploadSettings:FolderUpload_MDF1"] ?? "";
                                        string url_upload = _configuration["DownloadSettings:UlpoadServer_MDF1"] ?? "";
                                        var file_path = utilityHelper.UploadFileOld_ToAPI(Convert.ToBase64String(System.IO.File.ReadAllBytes(Path_orgin)), hsgd_tt.PathTtrinh, folderUpload, url_upload);
                                        // kiểm tra và xóa file ở local 
                                        try
                                        {
                                            if (System.IO.File.Exists(Path_orgin))
                                            {
                                                System.IO.File.Delete(Path_orgin);
                                            }
                                            if (System.IO.File.Exists(Path_result))
                                            {
                                                System.IO.File.Delete(Path_result);
                                            }
                                            //if (System.IO.File.Exists(Path_result_2))
                                            //{
                                            //    System.IO.File.Delete(Path_result_2);
                                            //}
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                        if (file_path != "")
                                        {
                                            _logger.Information("KyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " upload file tờ trình sau ReplaceTextInPDF thành công");

                                            //update nhật ký
                                            string trang_thai_tt = "";
                                            string noidung_upd = "";
                                            var todaysdate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                                            if (new[] { 2, 10 }.Contains(user_login.LoaiUser ?? 0))
                                            {
                                                trang_thai_tt = "14";
                                            }
                                            else
                                            {
                                                trang_thai_tt = "09";
                                            }
                                            noidung_upd = user_login.TenLoaiUser + " " + user_login.TenUser + " duyệt ký điện tử ngày " + todaysdate;
                                            HsgdTtrinhNky nky = new HsgdTtrinhNky();
                                            nky.FrKey = pr_key_hsgd_ttrinh;
                                            nky.UserChuyen = user_login.Oid.ToString();
                                            nky.UserNhan = user_login.Oid.ToString();
                                            nky.GhiChu = noidung_upd;
                                            nky.NgayCnhat = DateTime.Now;
                                            nky.Act = "KyHoSo";
                                            var res = UpdateNKyTtrinh(pr_key_hsgd_ttrinh, trang_thai_tt, nky, "KyHoSo");
                                            if (res)
                                            {
                                                result = "Ký hồ sơ thành công";
                                                UpdateTrangThaiHsbtCt(pr_key_hsgd_ttrinh);
                                                Update_hoanthien_hstt(pr_key_hsgd_ttrinh);
                                                InsertHsgdAttachFile(pr_key_hsgd_ttrinh);
                                                _logger.Information("KyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " thành công");
                                            }
                                            else
                                            {
                                                result = "Ký hồ sơ thất bại";
                                                _logger.Information("KyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " thất bại");
                                            }
                                        }
                                        else
                                        {
                                            _logger.Information("KyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " upload file tờ trình sau ReplaceTextInPDF thất bại");
                                        }


                                    }
                                    else
                                    {
                                        result = "Lỗi không tải được file tờ trình";
                                        _logger.Information("KyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " lỗi không tải được file tờ trình");
                                    }

                                }
                                else
                                {
                                    result = "Ký số HILO thất bại";
                                    _logger.Information("KyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " SignPDF_HILO thất bại");
                                }

                            }
                            else
                            {
                                result = "Chưa cấu hình quyền ký số hoặc ký số hết hiệu lực";
                                _logger.Information("KyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " hddt_hsm không tồn tại");
                            }

                        }
                        else
                        {
                            result = "User không có quyền ký hồ sơ.";
                            _logger.Information("KyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " lỗi không phải loại user được thực hiện 4, 8, 3, 9, 11, 2, 10");
                        }

                    }


                }
                else
                {
                    result = "Không tồn tại tờ trình";
                    _logger.Information("KyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " không tồn tại tờ trình");
                }
            }
            catch (Exception ex)
            {
                _logger.Information("KyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " error: " + ex);
            }
            return result;
        }
        public CheckHD CheckKyHoSo(decimal pr_key_hsgd_ttrinh)
        {
            CheckHD chk = new CheckHD();
            chk.ThongBao = "";
            try
            {
                var hsgd_ttrinh_ct = _context.HsgdTtrinhCts.Where(x => x.FrKey == pr_key_hsgd_ttrinh).Join(_context.HsgdTtrinhs, a => a.FrKey, b => b.PrKey, (a, b) => new { b.SoHsbt, a.MaSp }).ToList();
                if (hsgd_ttrinh_ct.Count > 0)
                {
                    var so_hsbt = hsgd_ttrinh_ct[0].SoHsbt;
                    var pr_key_hsbt_ctu = _context_pias_update.HsbtCtus.Where(x => x.SoHsbt == so_hsbt).Select(s => s.PrKey).FirstOrDefault();
                    var hsbt_ct = _context_pias_update.HsbtCts.Where(x => x.FrKey == pr_key_hsbt_ctu && hsgd_ttrinh_ct.Select(s => s.MaSp).ToArray().Contains(x.MaSp)).ToList();
                    foreach (var item in hsbt_ct)
                    {
                        var hsgd_dx_ct = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCt == item.PrKey).FirstOrDefault();
                        if (hsgd_dx_ct != null)
                        {
                            chk.ChkKhongHoadon = hsgd_dx_ct.ChkKhonghoadon;
                            if (hsgd_dx_ct.ChkKhonghoadon == 0)
                            {
                                if (string.IsNullOrEmpty(item.SerieVat))
                                {
                                    chk.ThongBao = "Mã sản phẩm " + item.MaSp + " chưa nhập 'Số seri VAT'. Vui lòng nhập đầy đủ thông tin trước khi ký hồ sơ!";
                                    return chk;
                                }
                                if (string.IsNullOrEmpty(item.SoHdvat))
                                {
                                    chk.ThongBao = "Mã sản phẩm " + item.MaSp + " chưa nhập 'Số HD VAT'. Vui lòng nhập đầy đủ thông tin trước khi ký hồ sơ!";
                                    return chk;
                                }
                                if (item.NgayHdvat == null)
                                {
                                    chk.ThongBao = "Mã sản phẩm " + item.MaSp + " chưa nhập 'Ngày HD VAT'. Vui lòng nhập đầy đủ thông tin trước khi ký hồ sơ!";
                                    return chk;
                                }
                                if (string.IsNullOrEmpty(item.MaKhvat))
                                {
                                    chk.ThongBao = "Mã sản phẩm " + item.MaSp + " chưa nhập 'Mã KH VAT'. Vui lòng nhập đầy đủ thông tin trước khi ký hồ sơ!";
                                    return chk;
                                }
                                if (string.IsNullOrEmpty(item.MasoVat))
                                {
                                    chk.ThongBao = "Mã sản phẩm " + item.MaSp + " chưa nhập 'Mã số VAT'. Vui lòng nhập đầy đủ thông tin trước khi ký hồ sơ!";
                                    return chk;
                                }
                                if (string.IsNullOrEmpty(item.TenHhoavat))
                                {
                                    chk.ThongBao = "Mã sản phẩm " + item.MaSp + " chưa nhập 'Tên hàng hóa VAT'. Vui lòng nhập đầy đủ thông tin trước khi ký hồ sơ!";
                                    return chk;
                                }
                                var file_xml = _context_pias_update.FileAttachBts.Where(x => x.FrKey == item.PrKey && x.MaCtu == "BTPT" && x.FileName.Contains(".xml")).ToList();
                                if (file_xml.Count == 0)
                                {
                                    chk.ThongBao = "Mã sản phẩm " + item.MaSp + " chưa đính kèm file xml. Vui lòng nhập đầy đủ thông tin trước khi ký hồ sơ!";
                                    return chk;
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(item.SerieVat))
                                {
                                    chk.ThongBao = "Mã sản phẩm " + item.MaSp + " chưa nhập 'Số seri VAT'. Bạn có muốn tiếp tục ký hồ sơ không?";
                                    return chk;
                                }
                                if (string.IsNullOrEmpty(item.SoHdvat))
                                {
                                    chk.ThongBao = "Mã sản phẩm " + item.MaSp + " chưa nhập 'Số HD VAT'. Bạn có muốn tiếp tục ký hồ sơ không?";
                                    return chk;
                                }
                                if (item.NgayHdvat == null)
                                {
                                    chk.ThongBao = "Mã sản phẩm " + item.MaSp + " chưa nhập 'Ngày HD VAT'. Bạn có muốn tiếp tục ký hồ sơ không?";
                                    return chk;
                                }
                                if (string.IsNullOrEmpty(item.MaKhvat))
                                {
                                    chk.ThongBao = "Mã sản phẩm " + item.MaSp + " chưa nhập 'Mã KH VAT'. Bạn có muốn tiếp tục ký hồ sơ không?";
                                    return chk;
                                }
                                if (string.IsNullOrEmpty(item.MasoVat))
                                {
                                    chk.ThongBao = "Mã sản phẩm " + item.MaSp + " chưa nhập 'Mã số VAT'. Bạn có muốn tiếp tục ký hồ sơ không?";
                                    return chk;
                                }
                                if (string.IsNullOrEmpty(item.TenHhoavat))
                                {
                                    chk.ThongBao = "Mã sản phẩm " + item.MaSp + " chưa nhập 'Tên hàng hóa VAT'. Bạn có muốn tiếp tục ký hồ sơ không?";
                                    return chk;
                                }
                                var file_xml = _context_pias_update.FileAttachBts.Where(x => x.FrKey == item.PrKey && x.MaCtu == "BTPT" && x.FileName.Contains(".xml")).ToList();
                                if (file_xml.Count == 0)
                                {
                                    chk.ThongBao = "Mã sản phẩm " + item.MaSp + " chưa đính kèm file xml. Bạn có muốn tiếp tục ký hồ sơ không?";
                                    return chk;
                                }
                            }

                        }
                        else
                        {
                            chk.ThongBao = "Mã sản phẩm " + item.MaSp + " chưa nhập đề xuất PASC. Vui lòng nhập đầy đủ thông tin trước khi ký hồ sơ!";
                            chk.ChkKhongHoadon = 0;
                            return chk;
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Information("CheckKyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " error: " + ex);
            }
            return chk;
        }
        public string ChuyenHoSo(decimal pr_key_hsgd_ttrinh, string email_login, string oid_nhan, bool send_email)
        {
            string result = "";
            try
            {
                var user_login = _context.DmUsers.Where(x => x.Mail == email_login).Join(_context.DmLoaiUsers, a => a.LoaiUser, b => b.LoaiUser, (a, b) => new { a.Oid, a.LoaiUser, a.MaUser, a.TenUser, a.MaDonvi, b.TenLoaiUser }).FirstOrDefault();
                var user_nhan = _context.DmUsers.Where(x => x.Oid == Guid.Parse(oid_nhan)).Join(_context.DmLoaiUsers, a => a.LoaiUser, b => b.LoaiUser, (a, b) => new { a.Oid, a.Mail, a.LoaiUser, a.MaUser, a.TenUser, a.MaDonvi, b.TenLoaiUser }).FirstOrDefault();
                var hsgd_tt = _context.HsgdTtrinhs.Where(x => x.PrKey == pr_key_hsgd_ttrinh).FirstOrDefault();
                var user_cuoi = _context.HsgdTtrinhNkies.Where(x => x.FrKey == pr_key_hsgd_ttrinh).OrderByDescending(x => x.PrKey).Select(s => s.UserNhan).FirstOrDefault() ?? "";
                //if (!user_cuoi.ToLower().Equals(user_login.Oid.ToString().ToLower()))
                //{
                //    result = "Tờ trình này không được chuyển đến bạn. Bạn không có quyền thao tác, vui lòng kiểm tra lại";
                //    return result;
                //}
                if (hsgd_tt != null)
                {
                    if (hsgd_tt.MaTtrang == "01")
                    {
                        result = "Tờ trình chưa qua bước chuyển duyệt không được sử dụng chức năng 'Chuyển hồ sơ'. Vui lòng kiểm tra lại";
                        return result;
                    }
                    if (hsgd_tt.MaTtrang == "09" || hsgd_tt.MaTtrang == "14")
                    {
                        result = "Tờ trình có trạng thái Đã duyệt TPC/Chờ duyệt kế toán thanh toán không thực hiện được. Vui lòng kiểm tra lại";
                        return result;
                    }
                    else if (hsgd_tt.MaTtrang == "15")
                    {
                        result = "Tờ trình có trạng thái Đã hủy. Vui lòng kiểm tra lại";
                        return result;
                    }
                    else
                    {
                        //update nhật ký
                        string trang_thai_tt = "";
                        string ten_trang_tt = _context.DmTtrangTtrinhs.Where(x => x.MaTtrangTt == hsgd_tt.MaTtrang).Select(s => s.TenTtrangTt).FirstOrDefault() ?? "";
                        string noidung_upd = "";
                        var todaysdate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        bool check_doitruong = false;
                        if (user_login.LoaiUser == 4 || user_login.LoaiUser == 8)
                        {
                            var check_kyhs = CheckKyHoSo(hsgd_tt.SoHsbt, user_login.Oid.ToString());
                            if (check_kyhs == 0)//là đội trưởng
                            {
                                check_doitruong = true;
                            }
                        }
                        if (check_doitruong)
                        {
                            noidung_upd = "Đội trưởng " + user_login.TenUser + " chuyển hồ sơ lên " + user_nhan.TenLoaiUser + " " + user_nhan.TenUser + " duyệt ngày " + todaysdate;
                        }
                        else
                        {
                            noidung_upd = user_login.TenLoaiUser + " " + user_login.TenUser + " chuyển hồ sơ lên " + user_nhan.TenLoaiUser + " " + user_nhan.TenUser + " duyệt ngày " + todaysdate;
                        }
                        HsgdTtrinhNky nky = new HsgdTtrinhNky();
                        nky.FrKey = pr_key_hsgd_ttrinh;
                        nky.UserChuyen = user_login.Oid.ToString();
                        nky.UserNhan = user_nhan.Oid.ToString();
                        nky.GhiChu = noidung_upd;
                        nky.NgayCnhat = DateTime.Now;
                        nky.Act = "ChuyenHoSo";
                        var res = UpdateNKyTtrinh(pr_key_hsgd_ttrinh, trang_thai_tt, nky, "ChuyenHoSo");
                        if (res)
                        {
                            if (send_email)
                            {
                                var email = user_nhan.Mail;
                                if (!string.IsNullOrEmpty(email))
                                {
                                    SendEmail_ToTrinh(true, email, "Xin ý kiến phê duyệt tờ trình", user_nhan.TenUser ?? "",
                    todaysdate, hsgd_tt.SoHsbt, hsgd_tt.SoTien.ToString("#,###", CultureInfo.GetCultureInfo("vi-VN").NumberFormat) + " VNĐ", ten_trang_tt, noidung_upd, hsgd_tt.PrKeyHsgd, "");
                                }
                            }
                            result = "Chuyển hồ sơ thành công";
                            _logger.Information("ChuyenHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " thành công");
                        }
                        else
                        {
                            result = "Chuyển hồ sơ thất bại";
                            _logger.Information("ChuyenHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " thất bại");
                        }

                    }


                }
                else
                {
                    result = "Không tồn tại tờ trình";
                    _logger.Information("ChuyenHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " không tồn tại tờ trình");
                }
            }
            catch (Exception ex)
            {
                _logger.Information("ChuyenHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " error: " + ex);
            }
            return result;
        }
        public string ChuyenKyHoSo(decimal pr_key_hsgd_ttrinh, string email_login, string oid_nhan, bool send_email)
        {
            string result = "";
            try
            {
                var user_login = _context.DmUsers.Where(x => x.Mail == email_login).Join(_context.DmLoaiUsers, a => a.LoaiUser, b => b.LoaiUser, (a, b) => new { a.Oid, a.LoaiUser, a.MaUser, a.TenUser, a.MaDonvi, b.TenLoaiUser }).FirstOrDefault();
                var user_nhan = _context.DmUsers.Where(x => x.Oid == Guid.Parse(oid_nhan)).Join(_context.DmLoaiUsers, a => a.LoaiUser, b => b.LoaiUser, (a, b) => new { a.Oid, a.Mail, a.LoaiUser, a.MaUser, a.TenUser, a.MaDonvi, b.TenLoaiUser }).FirstOrDefault();
                var hsgd_tt = _context.HsgdTtrinhs.Where(x => x.PrKey == pr_key_hsgd_ttrinh).FirstOrDefault();
                var user_cuoi = _context.HsgdTtrinhNkies.Where(x => x.FrKey == pr_key_hsgd_ttrinh).OrderByDescending(x => x.PrKey).Select(s => s.UserNhan).FirstOrDefault() ?? "";
                if (!user_cuoi.ToLower().Equals(user_login.Oid.ToString().ToLower()))
                {
                    result = "Tờ trình này không được chuyển đến bạn. Bạn không có quyền thao tác, vui lòng kiểm tra lại";
                    return result;
                }
                if (hsgd_tt != null)
                {
                    if (hsgd_tt.MaTtrang == "01")
                    {
                        result = "Tờ trình chưa qua bước chuyển duyệt không được sử dụng chức năng 'Chuyển ký hồ sơ'. Vui lòng kiểm tra lại";
                        return result;
                    }
                    else if (hsgd_tt.MaTtrang == "09" || hsgd_tt.MaTtrang == "14")
                    {
                        result = "Tờ trình có trạng thái Đã duyệt TPC/Chờ duyệt kế toán thanh toán không thực hiện được. Vui lòng kiểm tra lại";
                        return result;
                    }
                    else if (hsgd_tt.MaTtrang == "15")
                    {
                        result = "Tờ trình có trạng thái Đã hủy. Vui lòng kiểm tra lại";
                        return result;
                    }
                    else
                    {
                        var check_kyhs = CheckKyHoSo(hsgd_tt.SoHsbt, user_login.Oid.ToString());
                        if (check_kyhs == 1)
                        {
                            result = "Mã sản phẩm không có trong cấu hình quyền ký hồ sơ. Vui lòng kiểm tra lại!";
                            return result;
                        }
                        //else if (check_kyhs == 2)
                        //{
                        //    result = "Số tiền duyệt nhỏ hơn số tiền trong cấu hình quyền ký hồ sơ. Vui lòng kiểm tra lại!";
                        //    return result;
                        //}
                        else if (check_kyhs == 3)
                        {
                            result = "Tài khoản chưa được cấu hình quyền ký hồ sơ hoặc chưa được kích hoạt tài khoản. Vui lòng kiểm tra lại!";
                            return result;
                        }
                        bool check_doitruong = false;
                        if (user_login.LoaiUser == 4 || user_login.LoaiUser == 8)
                        {
                            if (check_kyhs != 1 && check_kyhs != 3)
                            {
                                check_doitruong = true;
                            }
                        }
                        if (new[] { 2, 3, 9, 11 }.Contains(user_login.LoaiUser ?? 0) || check_doitruong)
                        {
                            // thực hiện replace text CB xử lý
                            string url_download = _configuration["DownloadSettings:DownloadServer_MDF1"] ?? "";
                            //tải file
                            string Path_orgin = UtilityHelper.getPathAndCopyTempServer(hsgd_tt.PathTtrinh, url_download);
                            string Path_result = Path_orgin + "_edited.pdf";
                            if (System.IO.File.Exists(Path_orgin))
                            {
                                _logger.Information("ChuyenKyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " tải file thành công");
                                var pdfEdit = new PDFEdit(_logger);
                                // pdfEdit.ReplaceTextInPDF(Path_orgin, Path_result, pdfEdit.ListKeyWord("TP_TAT", user_login.MaUser ?? "", user_login.TenUser ?? ""), true);
                                // _logger.Information("ChuyenKyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " ReplaceTextInPDF thành công");
                                if (pdfEdit.ReplaceTextInPDF(Path_orgin, Path_result, pdfEdit.ListKeyWord("TP_TAT", user_login.MaUser ?? "", user_login.TenUser ?? ""), true))
                                {
                                    _logger.Information("ChuyenKyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " ReplaceTextInPDF TP_TAT thành công");
                                }
                                else
                                {
                                    result = "Ký hồ sơ không thành công";
                                    _logger.Information("ChuyenKyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " ReplaceTextInPDF TP_TAT thất bại");
                                    return result;
                                }
                                //upload file sau replace text
                                //var FileInfo = new FileInfo(Path_orgin);
                                //var extn = FileInfo.Extension;
                                var utilityHelper = new UtilityHelper(_logger);
                                string folderUpload = _configuration["UploadSettings:FolderUpload_MDF1"] ?? "";
                                string url_upload = _configuration["DownloadSettings:UlpoadServer_MDF1"] ?? "";
                                var file_path = utilityHelper.UploadFileOld_ToAPI(Convert.ToBase64String(System.IO.File.ReadAllBytes(Path_orgin)), hsgd_tt.PathTtrinh, folderUpload, url_upload);
                                _logger.Information("ChuyenKyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " upload file tờ trình sau ReplaceTextInPDF thành công");
                                // kiểm tra và xóa file ở local 
                                try
                                {
                                    if (System.IO.File.Exists(Path_orgin))
                                    {
                                        System.IO.File.Delete(Path_orgin);
                                    }
                                    if (System.IO.File.Exists(Path_result))
                                    {
                                        System.IO.File.Delete(Path_result);
                                    }
                                }
                                catch (Exception ex)
                                {

                                }

                                //update nhật ký
                                string trang_thai_tt = hsgd_tt.MaTtrang;
                                string ten_trang_tt = "";
                                string noidung_upd = "";
                                var todaysdate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                                if (check_doitruong)
                                {
                                    trang_thai_tt = "08";
                                    ten_trang_tt = "Cán bộ đã duyệt/Chờ TP duyệt bồi thường (cán bộ xử lý bấm trình ký)";
                                    noidung_upd = "Đội trưởng " + user_login.TenUser + " chuyển ký hồ sơ lên " + user_nhan.TenLoaiUser + " " + user_nhan.TenUser + " duyệt ngày " + todaysdate;
                                }
                                else
                                {
                                    trang_thai_tt = "12";
                                    ten_trang_tt = "Đã duyệt bồi thường TPC/Chờ lãnh đạo phê duyệt";
                                    noidung_upd = user_login.TenLoaiUser + " " + user_login.TenUser + " chuyển ký hồ sơ lên " + user_nhan.TenLoaiUser + " " + user_nhan.TenUser + " duyệt ngày " + todaysdate;
                                }
                                HsgdTtrinhNky nky = new HsgdTtrinhNky();
                                nky.FrKey = pr_key_hsgd_ttrinh;
                                nky.UserChuyen = user_login.Oid.ToString();
                                nky.UserNhan = user_nhan.Oid.ToString();
                                nky.GhiChu = noidung_upd;
                                nky.NgayCnhat = DateTime.Now;
                                nky.Act = "ChuyenKyHoSo";
                                var res = UpdateNKyTtrinh(pr_key_hsgd_ttrinh, trang_thai_tt, nky, "ChuyenKyHoSo");
                                if (res)
                                {
                                    if (send_email)
                                    {
                                        var email = user_nhan.Mail;
                                        if (!string.IsNullOrEmpty(email))
                                        {
                                            SendEmail_ToTrinh(true, email, "Xin ý kiến phê duyệt tờ trình", user_nhan.TenUser ?? "",
                            todaysdate, hsgd_tt.SoHsbt, hsgd_tt.SoTien.ToString("#,###", CultureInfo.GetCultureInfo("vi-VN").NumberFormat) + " VNĐ", ten_trang_tt, noidung_upd, hsgd_tt.PrKeyHsgd, "");
                                        }
                                    }
                                    result = "Chuyển ký hồ sơ thành công";
                                    _logger.Information("ChuyenKyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " thành công");
                                }
                                else
                                {
                                    result = "Chuyển ký hồ sơ thất bại";
                                    _logger.Information("ChuyenKyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " thất bại");
                                }

                            }
                            else
                            {

                                result = "Lỗi không tải được file tờ trình";
                                _logger.Information("ChuyenKyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " lỗi không tải được file tờ trình");
                            }
                        }
                        else
                        {
                            result = "User không có quyền thực hiện chức năng này. ";
                            _logger.Information("ChuyenKyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " user không có quyền thực hiện chức năng này");
                        }

                    }


                }
                else
                {
                    result = "Không tồn tại tờ trình";
                    _logger.Information("ChuyenKyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " không tồn tại tờ trình");
                }
            }
            catch (Exception ex)
            {
                _logger.Information("ChuyenKyHoSo pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " error: " + ex);
            }
            return result;
        }
        public string TraLaiHoSo(decimal pr_key_hsgd_ttrinh, string email_login, string oid_nhan, string lido_tc, bool send_email)
        {
            string result = "";
            try
            {
                var user_login = _context.DmUsers.Where(x => x.Mail == email_login).Join(_context.DmLoaiUsers, a => a.LoaiUser, b => b.LoaiUser, (a, b) => new { a.Oid, a.LoaiUser, a.MaUser, a.TenUser, a.MaDonvi, b.TenLoaiUser }).FirstOrDefault();
                var user_nhan = _context.DmUsers.Where(x => x.Oid == Guid.Parse(oid_nhan)).Join(_context.DmLoaiUsers, a => a.LoaiUser, b => b.LoaiUser, (a, b) => new { a.Oid, a.Mail, a.LoaiUser, a.MaUser, a.TenUser, a.MaDonvi, b.TenLoaiUser }).FirstOrDefault();
                var hsgd_tt = _context.HsgdTtrinhs.Where(x => x.PrKey == pr_key_hsgd_ttrinh).FirstOrDefault();
                var user_cuoi = _context.HsgdTtrinhNkies.Where(x => x.FrKey == pr_key_hsgd_ttrinh).OrderByDescending(x => x.PrKey).Select(s => s.UserNhan).FirstOrDefault() ?? "";
                if (!user_cuoi.ToLower().Equals(user_login.Oid.ToString().ToLower()))
                {
                    result = "Tờ trình này không được chuyển đến bạn.Bạn không có quyền thao tác, vui lòng kiểm tra lại";
                    return result;
                }
                if (hsgd_tt != null)
                {
                    if (hsgd_tt.MaTtrang == "01")
                    {
                        result = "Tờ trình chưa qua bước chuyển duyệt không được sử dụng chức năng này. Vui lòng kiểm tra lại";
                        return result;
                    }
                    //else if (hsgd_tt.MaTtrang == "09" || hsgd_tt.MaTtrang == "14")
                    //{
                    //    result = "Tờ trình có trạng thái Đã duyệt TPC/Chờ duyệt kế toán thanh toán không được trả lại. Vui lòng kiểm tra lại";
                    //    return result;
                    //}
                    else if (hsgd_tt.MaTtrang == "15")
                    {
                        result = "Tờ trình có trạng thái Đã hủy. Vui lòng kiểm tra lại";
                        return result;
                    }
                    else
                    {
                        var check_kyhs = CheckKyHoSo(hsgd_tt.SoHsbt, user_login.Oid.ToString());
                        if (check_kyhs == 1)
                        {
                            result = "Mã sản phẩm không có trong cấu hình quyền ký hồ sơ. Vui lòng kiểm tra lại!";
                            return result;
                        }
                        //else if (check_kyhs == 2)
                        //{
                        //    result = "Số tiền duyệt nhỏ hơn số tiền trong cấu hình quyền ký hồ sơ. Vui lòng kiểm tra lại!";
                        //    return result;
                        //}
                        else if (check_kyhs == 3)
                        {
                            result = "Tài khoản chưa được cấu hình quyền ký hồ sơ hoặc chưa được kích hoạt tài khoản. Vui lòng kiểm tra lại!";
                            return result;
                        }
                        bool check_doitruong = false;
                        if (user_login.LoaiUser == 4 || user_login.LoaiUser == 8)
                        {
                            if (check_kyhs == 0)
                            {
                                check_doitruong = true;
                            }
                        }
                        if (new[] { 2, 3, 9, 10, 11 }.Contains(user_login.LoaiUser ?? 0) || check_doitruong)
                        {
                            //update nhật ký
                            string trang_thai_tt = "";
                            string noidung_upd = "";
                            var todaysdate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                            if (check_doitruong)
                            {
                                noidung_upd = "Đội trưởng " + user_login.TenUser + " trả lại hồ sơ " + hsgd_tt.SoHsbt + " ngày " + todaysdate + " lí do là " + lido_tc;
                            }
                            else
                            {
                                noidung_upd = user_login.TenLoaiUser + " " + user_login.TenUser + " trả lại hồ sơ " + hsgd_tt.SoHsbt + " ngày " + todaysdate + " lí do là " + lido_tc;
                            }

                            HsgdTtrinhNky nky = new HsgdTtrinhNky();
                            nky.FrKey = pr_key_hsgd_ttrinh;
                            nky.UserChuyen = user_login.Oid.ToString();
                            nky.UserNhan = user_nhan.Oid.ToString();
                            nky.GhiChu = noidung_upd;
                            nky.NgayCnhat = DateTime.Now;
                            nky.Act = "TraLaiHoSo";
                            var res = UpdateNKyTtrinh(pr_key_hsgd_ttrinh, trang_thai_tt, nky, "TRALAIHOSO");
                            if (res)
                            {
                                if (send_email)
                                {
                                    var email = user_nhan.Mail;
                                    if (!string.IsNullOrEmpty(email))
                                    {
                                        SendEmail_ToTrinh(false, email, "Trả lại hồ sơ", user_nhan.TenUser ?? "",
                        todaysdate, hsgd_tt.SoHsbt, hsgd_tt.SoTien.ToString("#,###", CultureInfo.GetCultureInfo("vi-VN").NumberFormat) + " VNĐ", "Trả lại hồ sơ", noidung_upd, hsgd_tt.PrKeyHsgd, "");
                                    }
                                }
                                result = "Trả lại hồ sơ thành công";
                                _logger.Information("TRALAIHOSO pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " thành công");
                            }
                            else
                            {
                                result = "Trả lại hồ sơ thất bại";
                                _logger.Information("TRALAIHOSO pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " thất bại");
                            }
                        }
                        else
                        {
                            result = "User không có quyền thực hiện chức năng này. ";
                            _logger.Information("TRALAIHOSO pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " user không có quyền thực hiện chức năng này");
                        }


                    }


                }
                else
                {
                    result = "Không tồn tại tờ trình";
                    _logger.Information("TRALAIHOSO pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " không tồn tại tờ trình");
                }
            }
            catch (Exception ex)
            {
                _logger.Information("TRALAIHOSO pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " error: " + ex);
            }
            return result;
        }
        public string HuyToTrinh(decimal pr_key_hsgd_ttrinh, string email_login)
        {
            string result = "";
            try
            {
                var user_login = _context.DmUsers.Where(x => x.Mail == email_login).Join(_context.DmLoaiUsers, a => a.LoaiUser, b => b.LoaiUser, (a, b) => new { a.Oid, a.LoaiUser, a.MaUser, a.TenUser, b.TenLoaiUser }).FirstOrDefault();
                var hsgd_tt = _context.HsgdTtrinhs.Where(x => x.PrKey == pr_key_hsgd_ttrinh).FirstOrDefault();
                if (hsgd_tt != null)
                {
                    var user_cuoi = _context.HsgdTtrinhNkies.Where(x => x.FrKey == pr_key_hsgd_ttrinh).OrderByDescending(x => x.PrKey).Select(s => s.UserNhan).FirstOrDefault() ?? "";
                    if (user_cuoi.ToLower().Equals(user_login.Oid.ToString().ToLower()))
                    {
                        //update nhật ký
                        string trang_thai_tt = "15";
                        var todaysdate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        string noidung_upd = user_login.TenLoaiUser + " " + user_login.TenUser + " hủy tờ trình ngày " + todaysdate;

                        HsgdTtrinhNky nky = new HsgdTtrinhNky();
                        nky.FrKey = pr_key_hsgd_ttrinh;
                        nky.UserChuyen = user_login.Oid.ToString();
                        nky.UserNhan = user_login.Oid.ToString();
                        nky.GhiChu = noidung_upd;
                        nky.NgayCnhat = DateTime.Now;
                        nky.Act = "HuyToTrinh";
                        var res = UpdateNKyTtrinh(pr_key_hsgd_ttrinh, trang_thai_tt, nky, "HuyToTrinh");
                        if (res)
                        {
                            result = "Hủy tờ trình thành công";
                            _logger.Information("HuyToTrinh pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " thành công");
                        }
                        else
                        {
                            result = "Hủy tờ trình thất bại";
                            _logger.Information("HuyToTrinh pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " thất bại");
                        }
                    }
                    else
                    {
                        result = "Tờ trình này không được chuyển đến bạn. Bạn không có quyền hủy, vui lòng kiểm tra lại";
                        _logger.Information("HuyToTrinh pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " tài khoản không có quyền thực hiện hủy tờ trình");
                    }
                }
                else
                {
                    result = "Không tồn tại tờ trình";
                    _logger.Information("HuyToTrinh pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " không tồn tại tờ trình");
                }
            }
            catch (Exception ex)
            {
                _logger.Information("HuyToTrinh pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " error: " + ex);
            }
            return result;
        }
        public int CheckKyHoSo(string so_hsbt, string ma_user)
        {
            int result = 0;
            try
            {
                var hsbt_ct = (from a in _context_pias_update.HsbtCts
                               join b in _context_pias_update.HsbtCtus on a.FrKey equals b.PrKey
                               where b.SoHsbt == so_hsbt
                               select new
                               {
                                   MaSp = a.MaSp,
                                   NguyenTep = a.NguyenTep
                               }).AsQueryable();
                var hsbt_ct_gr = ToListWithNoLock(hsbt_ct.GroupBy(n => new { n.MaSp }).Select(p => new
                {
                    MaSp = p.Key.MaSp,
                    TongTien = p.Sum(x => x.NguyenTep)
                }).AsQueryable());
                if (hsbt_ct_gr.Count() > 0)
                {
                    bool boolValue = 1 != 0;
                    var dm_pquyen_kyhs = _context.DmPquyenKyhs.Where(x => x.MaUser == ma_user && x.IsActive == boolValue).OrderByDescending(o => o.PrKey).FirstOrDefault();
                    Decimal so_tien_duyet = 0;
                    if (dm_pquyen_kyhs != null)
                    {
                        so_tien_duyet = dm_pquyen_kyhs.SoTien ?? 0;
                        var list_ma_sp = dm_pquyen_kyhs.MaSp.Split(",");
                        Decimal so_tien_ct = 0;
                        Decimal conlai = 0;
                        for (int i = 0; i < hsbt_ct_gr.Count(); i++)
                        {
                            if (!list_ma_sp.Contains(hsbt_ct_gr[i].MaSp))
                            {
                                //ma sp khong co trong cau hinh quyen ky ho so
                                result = 1;
                            }
                            so_tien_ct += hsbt_ct_gr[i].TongTien;
                        }
                        conlai = so_tien_duyet - so_tien_ct;
                        if (conlai < 0)
                        {
                            //so tien duyet dang nho hon so tien cua ho so ky
                            result = 2;
                        }
                    }
                    else
                    {
                        //tài khoản chưa được cấu hình quyền ký hồ sơ hoặc chưa được kích hoạt tài khoản. vui lòng kiểm tra lại!
                        result = 3;
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public bool UpdateNKyTtrinh(decimal pr_key_hsgd_ttrinh, string trang_thai_tt, HsgdTtrinhNky hsgdTtrinhNky, string act)
        {
            bool result = true;
            try
            {

                using var contextnew = new GdttContext();
                using var dbContextTransaction = contextnew.Database.BeginTransaction();
                try
                {
                    if (act == "TRALAIHOSO")
                    {
                        contextnew.HsgdTtrinhs
                                .Where(x => x.PrKey == pr_key_hsgd_ttrinh)
                                .ExecuteUpdate(s => s.SetProperty(u => u.MaTtrang, "01").SetProperty(u => u.PathTtrinh, ""));
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(trang_thai_tt))
                        {
                            contextnew.HsgdTtrinhs
                                     .Where(x => x.PrKey == pr_key_hsgd_ttrinh)
                                     .ExecuteUpdate(s => s.SetProperty(u => u.MaTtrang, trang_thai_tt));
                        }
                    }
                    // lưu vào bảng nhật ký
                    contextnew.HsgdTtrinhNkies.Add(hsgdTtrinhNky);
                    contextnew.SaveChanges();
                    dbContextTransaction.Commit();
                    _logger.Information(act + " pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " UpdateNKyTtrinh thành công");
                }
                catch (Exception ex)
                {
                    result = false;
                    _logger.Error(act + " pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " UpdateNKyTtrinh error : " + ex.ToString());
                    dbContextTransaction.Rollback();
                    dbContextTransaction.Dispose();
                }

            }
            catch (Exception ex)
            {
                result = false;
                _logger.Error(act + " pr_key_hsgd_ttrinh =" + pr_key_hsgd_ttrinh + " UpdateNKyTtrinh error : " + ex.ToString());
            }
            return result;
        }
        public void SendEmail_ToTrinh(bool trang_tt, string sTo, string sSubject, string nguoi_duyet, string ngay_duyet, string so_hsbt, string so_tien, string trang_thai, string ghi_chu, decimal pr_key_hsgd, string htmlBody = "")
        {
            try
            {
                MailAddress from = new MailAddress("baohiempvi@pvi.com.vn", "PVI.247", System.Text.Encoding.UTF8);
                MailAddress to = new MailAddress(sTo);
                System.Net.Mail.MailMessage Mail = new System.Net.Mail.MailMessage(from, to);
                Mail.Subject = sSubject;
                Mail.SubjectEncoding = System.Text.Encoding.UTF8;
                Mail.AlternateViews.Add(trang_tt == true ? avHTML_ToTrinh(nguoi_duyet, ngay_duyet, so_hsbt, so_tien, trang_thai, ghi_chu, pr_key_hsgd) : avHTML_TraToTrinh(nguoi_duyet, ngay_duyet, so_hsbt, so_tien, trang_thai, ghi_chu, pr_key_hsgd));
                if (htmlBody != "")
                    Mail.Body = htmlBody;
                Mail.BodyEncoding = System.Text.Encoding.UTF8;
                Mail.IsBodyHtml = true;
                SmtpClient SmtpServer = new SmtpClient();
                SmtpServer.Port = 25;
                SmtpServer.Host = "mailapp.pvi.com.vn";
                SmtpServer.Timeout = 10000;
                SmtpServer.Send(Mail);
                Mail.Dispose();
            }
            catch (Exception ex)
            {
                //ghiloc(DateTime.Now.ToString() + " gửi 8", ex.Message.ToString());
                //MsgBox(ex.ToString());
            }
        }
        public AlternateView avHTML_ToTrinh(string nguoi_duyet, string ngay_duyet, string so_hsbt, string so_tien, string trang_thai, string ghi_chu, decimal pr_key_hsgd)
        {
            string url_hsgd = _configuration["DownloadSettings:url_hsgd"] ?? "";
            string htmlBody = "<html xmlns=\"http://www.w3.org/1999/xhtml\"> "
        + "<head runat=\"server\"> "
        + "    <title></title> "
        + "    <style type=\"text/css\"> "
        + "        #content p { "
        + "            margin: 5px 0; "
        + "            color: #085d60; "
        + "        } "
        + "        * { "
        + "            margin: 0; "
        + "            padding: 0; "
        + "        } "
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
        + "                    <img src=\"cid:Pic1\" style=\"width: 100%; height: 130px; border: none; margin: 0; display: block\" /> "
        + "                </td> "
        + "            </tr> "
        + "            <tr> "
        + "                <td style=\"font-family: Arial; font-size: 13px; padding: 5px; vertical-align: top; text-align:justify\"> "
        + "                    <p style=\"MARGIN-TOP: 30px\"> "
        + "                        <strong> Kính gửi Ông/Bà: " + nguoi_duyet + "</strong> "
        + "                    </p> "
        + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
        + "                        <strong>Ngày chuyển hồ sơ chờ duyệt: " + ngay_duyet + "</strong> "
        + "                    </p> "
        + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
        + "                        <strong>Số HSBT: " + so_hsbt + "</strong> "
        + "                    </p> "
        + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
        + "                        <strong>Số tiền: " + so_tien + "</strong> "
        + "                    </p> "
        + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
        + "                        <strong>Trạng thái: " + trang_thai + "</strong>"
        + "                    </p> "
        + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
        + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
        + "                        <strong>Ghi chú:  " + ghi_chu + "</strong>"
        + "                    </p> "
        + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
        + "                        <strong>Đường dẫn đến hồ sơ giám định: <a target=\"_blank\" href= " + url_hsgd + "ho_so_o_to/hsgd/thong_tin_ho_so/" + pr_key_hsgd + ">Xem</a></strong>"
        + "                    </p> "
        + "                    <p> Trân trọng cảm ơn! </p> "
        + "                    <p> (*) Đây là email hệ thống gửi tự động, vui lòng không trả lời (reply) lại email này.</p> "
        + "                </td> "
        + "            </tr> "
        + "        </table> "
        + "    </form> "
        + "</body> "
        + "</html>";
            string image = _configuration["Word2PdfSettings:BannerToTrinh"];
            AlternateView avHtml = AlternateView.CreateAlternateViewFromString(htmlBody, null/* TODO Change to default(_) if this is not a reference type */, MediaTypeNames.Text.Html);
            LinkedResource pic = new LinkedResource(image, MediaTypeNames.Image.Jpeg);
            pic.ContentId = "Pic1";
            avHtml.LinkedResources.Add(pic);
            return avHtml;
        }
        public AlternateView avHTML_TraToTrinh(string nguoi_duyet, string ngay_duyet, string so_hsbt, string so_tien, string trang_thai, string ghi_chu, decimal pr_key_hsgd)
        {
            string url_hsgd = _configuration["DownloadSettings:url_hsgd"] ?? "";
            string htmlBody = "<html xmlns=\"http://www.w3.org/1999/xhtml\"> "
        + "<head runat=\"server\"> "
        + "    <title></title> "
        + "    <style type=\"text/css\"> "
        + "        #content p { "
        + "            margin: 5px 0; "
        + "            color: #085d60; "
        + "        } "
        + "        * { "
        + "            margin: 0; "
        + "            padding: 0; "
        + "        } "
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
        + "                        <strong> Kính gửi Ông/Bà: " + nguoi_duyet + "</strong> "
        + "                    </p> "
        + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
        + "                        <strong>Ngày trả hồ sơ : " + ngay_duyet + "</strong> "
        + "                    </p> "
        + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
        + "                        <strong>Số HSBT: " + so_hsbt + "</strong> "
        + "                    </p> "
        + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
        + "                        <strong>Số tiền: " + so_tien + "</strong> "
        + "                    </p> "
        + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
        + "                        <strong>Trạng thái: " + trang_thai + "</strong>"
        + "                    </p> "
        + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
        + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
        + "                        <strong>Ghi chú:  " + ghi_chu + "</strong>"
        + "                    </p> "
        + "                    <p style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 30px\"> "
        + "                        <strong>Đường dẫn đến hồ sơ giám định: <a target=\"_blank\" href= " + url_hsgd + "ho_so_o_to/hsgd/thong_tin_ho_so/" + pr_key_hsgd + ">Xem</a></strong>"
        + "                    </p> "
        + "                    <p> Trân trọng cảm ơn! </p> "
        + "                    <p> (*) Đây là email hệ thống gửi tự động, vui lòng không trả lời (reply) lại email này.</p> "
        + "                </td> "
        + "            </tr> "
        + "        </table> "
        + "    </form> "
        + "</body> "
        + "</html>";
            string image = _configuration["Word2PdfSettings:BannerToTrinh"];
            AlternateView avHtml = AlternateView.CreateAlternateViewFromString(htmlBody, null/* TODO Change to default(_) if this is not a reference type */, MediaTypeNames.Text.Html);
            LinkedResource pic = new LinkedResource(image, MediaTypeNames.Image.Jpeg);
            pic.ContentId = "Pic1";
            avHtml.LinkedResources.Add(pic);
            return avHtml;
        }
        public List<HsgdTtrinhNky> GetLichSuPheDuyet(decimal pr_key_hsgd_ttrinh)
        {
            var result = ToListWithNoLock(_context.HsgdTtrinhNkies.Where(x => x.FrKey == pr_key_hsgd_ttrinh).OrderByDescending(o => o.PrKey).AsQueryable());
            return result;
        }
        public TtrinhCount CountTTrinhByTT(string email_login, int nam_dulieu)
        {
            TtrinhCount result = new TtrinhCount();
            var user = _context.DmUsers.Where(x => x.Mail == email_login).FirstOrDefault();
            if (user != null)
            {
                var ma_donvi = user.MaDonvi;
                if (!string.IsNullOrEmpty(user.MaDonviPquyen))
                {
                    ma_donvi = user.MaDonviPquyen;
                }
                List<string> list_ma_donvi_pquyen = ma_donvi.Split(",").ToList();
                var list_tt = ToListWithNoLock(_context.HsgdTtrinhs.Where(x => list_ma_donvi_pquyen.Contains(x.MaDonvi) && x.NgayCtu != null && x.NgayCtu.Value.Year == (nam_dulieu != 0 ? nam_dulieu : DateTime.Now.Year)).AsQueryable());
                if (list_tt.Count() > 0)
                {
                    result.sl_gdvchoduyet = list_tt.Where(x => x.MaTtrang == "01").Count();
                    result.sl_tpchoduyet = list_tt.Where(x => x.MaTtrang == "08").Count();
                    result.sl_ldchoduyet = list_tt.Where(x => x.MaTtrang == "12").Count();
                    result.sl_choduyetttoan = list_tt.Where(x => x.MaTtrang == "09" || x.MaTtrang == "14").Count();
                    result.sl_dahuy = list_tt.Where(x => x.MaTtrang == "15").Count();
                }

            }
            return result;
        }
        public TtrinhLDCount CountTTrinhLDByTT(string email_login, int nam_dulieu)
        {
            TtrinhLDCount result = new TtrinhLDCount();
            var user = _context.DmUsers.Where(x => x.Mail == email_login).FirstOrDefault();
            if (user != null)
            {
                var ma_donvi = user.MaDonvi;
                if (!string.IsNullOrEmpty(user.MaDonviPquyen))
                {
                    ma_donvi = user.MaDonviPquyen;
                }
                List<string> list_ma_donvi_pquyen = ma_donvi.Split(",").ToList();
                List<string> list_ma_ttgd = new List<string> { "6", "12", "13" };

                var list_tt = (from a in _context.NhatKies.Where(x => x.MaTtrangGd == "12")
                               join d in _context.HsgdCtus on a.FrKey equals d.PrKey into d1
                               from d in d1.DefaultIfEmpty()
                               where list_ma_donvi_pquyen.Contains(d.MaDonvi) &&
                                     list_ma_ttgd.Contains(d.MaTtrangGd) &&
                                     d.NgayCtu.HasValue &&
                                     d.NgayCtu.Value.Year == (nam_dulieu != 0 ? nam_dulieu : DateTime.Now.Year)
                               group d by d.SoHsgd into g
                               select new
                               {
                                   SoHsgd = g.Key,
                                   MaTtrangGd = g.First().MaTtrangGd
                               }
      ).ToList();

                //var list_tt = ToListWithNoLock(_context.HsgdTtrinhs.Where(x => list_ma_donvi_pquyen.Contains(x.MaDonvi) && x.NgayCtu != null && x.NgayCtu.Value.Year == (nam_dulieu != 0 ? nam_dulieu : DateTime.Now.Year)).AsQueryable());
                if (list_tt.Count() > 0)
                {
                    result.sl_npc = list_tt.Where(x => x.MaTtrangGd == "12").Count();
                    result.sl_daduyet = list_tt.Where(x => x.MaTtrangGd == "6").Count();
                    result.sl_choduyet = list_tt.Where(x => x.MaTtrangGd == "13").Count();
                }

            }
            return result;
        }
        
        public DmUser? GetUserLogin(string email_login)
        {
            var user = _context.DmUsers.Where(x => x.Mail == email_login).FirstOrDefault();
            return user;
        }
        public List<DmUser> GetListUserChuyenKy(string email_login)
        {
            List<DmUser> result = new List<DmUser>();
            try
            {
                var user_login = _context.DmUsers.Where(x => x.Mail == email_login).FirstOrDefault();
                if (user_login != null)
                {
                    var data = (from a in _context.DmUsers
                                join b in _context.DmDonvis on a.MaDonvi equals b.MaDonvi
                                where a.MaDonvi == user_login.MaDonvi
                                select new DmUser
                                {
                                    Oid = a.Oid,
                                    MaUser = a.MaUser,
                                    TenUser = a.TenUser,
                                    Mail = a.Mail,
                                    TenDonvi = b.TenDonvi,
                                    Dienthoai = a.Dienthoai,
                                    MaUserPias = a.MaUserPias,
                                    LoaiUser = a.LoaiUser
                                }).AsQueryable();
                    //switch (user_login.LoaiUser)
                    //{
                    //    case 4:
                    //        data = data.Where(x => x.LoaiUser == 3);
                    //        break;
                    //    case 3:
                    //        data = data.Where(x => x.LoaiUser == 2);
                    //        break;
                    //    case 8:
                    //        data = data.Where(x => new List<int> { 8, 9, 10, 11 }.Contains(x.LoaiUser ?? 0));
                    //        break;
                    //    case 9:
                    //        data = data.Where(x => new List<int> { 9, 10 }.Contains(x.LoaiUser ?? 0));
                    //        break;
                    //    case 10:
                    //        data = data.Where(x => x.LoaiUser == 10);
                    //        break;
                    //    case 11:
                    //        data = data.Where(x => new List<int> { 9, 10, 11 }.Contains(x.LoaiUser ?? 0));
                    //        break;
                    //    default:
                    //        data = data.Where(x => 1 == 2);
                    //        break;

                    //}
                    result = ToListWithNoLock(data);
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public CombinedTtrinhResult3 CreateBiaHS(BiaHS biahs)
        {
            try
            {

                var wordPdfRequest = new WordToPdfRequest();
                wordPdfRequest.ListData = new List<EntityContent>();
                UpdateProperties update = new UpdateProperties();
                update.AddEntityContent(wordPdfRequest, "[so_hsgd]", biahs.SoHsgd ?? "");
                update.AddEntityContent(wordPdfRequest, "[so_seri]", biahs.SoSeri ?? "");
                update.AddEntityContent(wordPdfRequest, "[ngay_dau_seri]", biahs.NgayDauSeri ?? "");
                update.AddEntityContent(wordPdfRequest, "[ma_donvi]", biahs.TenDonvi ?? "");
                update.AddEntityContent(wordPdfRequest, "[donvi_ttoan]", biahs.TenDonviTToan ?? "");
                update.AddEntityContent(wordPdfRequest, "[bien_ksoat]", biahs.BienKsoat ?? "");
                update.AddEntityContent(wordPdfRequest, "[txt_sanpham]", biahs.MaSP ?? "");
                update.AddEntityContent(wordPdfRequest, "[ngay_dau_seri_ngay_cuoi_seri]", biahs.NgayDauSeri ?? "" + " - " + biahs.NgayCuoiSeri ?? "");
                update.AddEntityContent(wordPdfRequest, "[so_tien_thuc_te]", biahs.SoTienThucTe ?? "");
                update.AddEntityContent(wordPdfRequest, "[txtpvibltt]", biahs.PviBl ?? "");
                update.AddEntityContent(wordPdfRequest, "[ma_gara_vcx]", biahs.TenGara ?? "");
                update.AddEntityContent(wordPdfRequest, "[ma_user]", biahs.GiamDinhVien ?? "");
                update.AddEntityContent(wordPdfRequest, "[dr_ten_khach]", biahs.TenKhach ?? "");
                update.AddEntityContent(wordPdfRequest, "[dr_nguyen_nhan_ttat]", biahs.NguyenNhanTtat ?? "");
                update.AddEntityContent(wordPdfRequest, "[ngay_tthat]", biahs.NgayTthat ?? "");
                update.AddEntityContent(wordPdfRequest, "[dr_dia_tiemtt]", biahs.DiaDiemtt ?? "");
                update.AddEntityContent(wordPdfRequest, "[so_hsbt]", biahs.SoHsbt ?? "");

                var listData = wordPdfRequest.ListData;
                var listNew = new CombinedTtrinhResult3
                {
                    ThirdQueryResults = listData,
                };

                return listNew;

            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                return null;
            }
        }
        public decimal GetMtnGtbh(string soDonBh)
        {
            var result = (from a in _context_pias.NvuBhtCtus
                          join b in _context_pias.NvuBhtSeris on a.PrKey equals b.FrKey
                          join c in _context_pias.NvuBhtSeriCts on b.PrKey equals c.FrKey
                          where a.SoDonbh == soDonBh && c.MaSp == "050104"
                                && new[] { "", "02" }.Contains(a.MaSdbs)
                          orderby a.SoDonbhBs descending
                          select new
                          {
                              MtnGtbhVnd = c.GiatriTte,
                          })
              .FirstOrDefault();
            if (result != null)
            {
                return result.MtnGtbhVnd;
            }
            else
            {
                return 0;
            }

        }
        public PagedList<HoSoTrinhKy> GetDataHsTrinhKy(string email_login, ToTrinhParameters totrinhParameters)
        {
             
            try
            {
                //Add_HsgdTtrinhToHsgdDNTT(email_login);
                //Hàm này dùng để kiểm tra các đề nghị thanh toán đã bị xóa và xóa trong bảng hsgd_dntt và gửi email cho giám định viên biết để vào sửa và làm lại
                //XoaHsgdDNTT_GuiEmailGDV(email_login);
                var user = _context.DmUsers.Where(x => x.Mail == email_login).FirstOrDefault();
                if (user != null)
                {
                    List<string> list_ma_tt = totrinhParameters.ma_ttrang_tt.Split(",").ToList();
                    var data = (from a in _context.HsgdTtrinhs.AsNoTracking()
                                join d in _context.HsgdCtus.AsNoTracking() on a.PrKeyHsgd equals d.PrKey into d1
                                from d in d1.DefaultIfEmpty()
                                join f in _context.HsgdTtrinhNkies.AsNoTracking() on a.PrKey equals f.FrKey into f1
                                from f in f1.DefaultIfEmpty()
                                join c in _context.HsgdTtrinhNkies.AsNoTracking().Where(x => x.Act == "CREATETOTRINH") on a.PrKey equals c.FrKey into c1
                                from c in c1.DefaultIfEmpty()
                                where (
                                    list_ma_tt.Contains(a.MaTtrang) && a.NgayCtu != null && a.NgayCtu.Value.Year == (totrinhParameters.nam_dulieu != 0 ? totrinhParameters.nam_dulieu : DateTime.Now.Year)
                                     )
                                select new HoSoTrinhKy
                                {
                                    PrKey = a.PrKey,
                                    MaDonvi = (a.MaDonvi == "33" ? "32" : a.MaDonvi),
                                    SoHsbt = a.SoHsbt,
                                    SoHsgd = d != null ? d.SoHsgd : "",
                                    TenDttt = a.TenDttt,
                                    NgGdich = a.NgGdich,
                                    NgayCtu = a.NgayCtu,
                                    NgayTthat = a.NgayTthat,
                                    SoTien = (
                                                _context.HsgdTtrinhCts
                                                    .Where(ct => ct.FrKey == a.PrKey
                                                              && !(ct.MaSp == "050101" && ct.MaDKhoan == "05010101")).Select(ct => (decimal?)ct.SotienBt)
                                                    .Sum()
                                            ) ?? a.SoTien,
                                    MaTtrang = a.MaTtrang,
                                    PrKeyNky = f != null ? f.PrKey : 0,
                                    MaDonviTt = d!= null ? d.MaDonviTt : "",                                                                        
                                    MaGDV = c != null ? c.UserChuyen : "",
                                    PrKeyHsgd = d != null ? d.PrKey : 0,
                                    MaDonviCapDon = d != null ? d.SoDonbh.Substring(3, 2) :"",
                                    NgayTtoan = a.NgayTtoan,
                                    NgayPsinh = d != null ? d.NgayCtu : (DateTime?)null,
                                    HoanThienHstt= d != null ? d.HoanThienHstt : true,
                                    PrKeyTTrinhCt=0,
                                    Tinhtrang_thanhtoan =
                                                ((d != null && d.HoanThienHstt)
                                                    ? "Đã H.thiện HSTT"
                                                    : "Chưa H.thiện HSTT")
                                                + " - " +
                                                   (_context.HsgdDntts
                                                            .Where(x => x.PrKeyTtrinh == a.PrKey && x.PrKeyTtoanCtu!=0)
                                                            .Select(x => "Đã tạo ĐNTT: " + x.SoCtu)
                                                            .FirstOrDefault()
                                                        ?? "Chưa tạo ĐNTT"
                                                    )

                                }
                                  ).AsQueryable();
                    List<string> list_ma_donvi_pquyen = user.MaDonviPquyen.Split(",").ToList();
                    if (new[] { "00", "31", "32" }.Contains(user.MaDonvi))
                    {
                        if (!string.IsNullOrEmpty(user.MaDonviPquyen))
                        {
                            data = data.Where(x => list_ma_donvi_pquyen.Contains(x.MaDonvi) || list_ma_donvi_pquyen.Contains(x.MaDonviTt));
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(user.MaDonviPquyen))
                        {
                            data = data.Where(x => list_ma_donvi_pquyen.Contains(x.MaDonvi) || list_ma_donvi_pquyen.Contains(x.MaDonviTt));
                        }
                        else
                        {
                            data = data.Where(x => x.MaDonvi == user.MaDonvi || list_ma_donvi_pquyen.Contains(x.MaDonviTt));
                        }
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.so_hsbt))
                    {
                        data = data.Where(x => x.SoHsbt.ToLower().Contains(totrinhParameters.so_hsbt.ToLower()));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ma_donvi))
                    {
                        data = data.Where(x => x.MaDonvi.Contains(totrinhParameters.ma_donvi));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.so_hsgd))
                    {
                        data = data.Where(x => x.SoHsgd.Contains(totrinhParameters.so_hsgd));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ten_ndbh))
                    {
                        data = data.Where(x => x.NgGdich.ToLower().Contains(totrinhParameters.ten_ndbh.ToLower()));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.bien_ks))
                    {
                        data = data.Where(x => x.TenDttt.ToLower().Contains(totrinhParameters.bien_ks.ToLower()));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ma_trangthai))
                    {
                        data = data.Where(x => x.MaTtrang.ToLower().Contains(totrinhParameters.ma_trangthai.ToLower()));
                    }
                    if (totrinhParameters.so_tien != 0)
                    {
                        data = data.Where(x => x.SoTien == totrinhParameters.so_tien);
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ngay_tt))
                    {
                        data = data.Where(x => x.NgayTthat != null && x.NgayTthat.Value.Date == DateTime.ParseExact(totrinhParameters.ngay_tt, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ngay_nhap))
                    {
                        data = data.Where(x => x.NgayCtu != null && x.NgayCtu.Value.Date == DateTime.ParseExact(totrinhParameters.ngay_nhap, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                    }

                    if (!string.IsNullOrEmpty(totrinhParameters.ma_gdv))
                    {
                        data = data.Where(x => !string.IsNullOrEmpty(x.MaGDV) && x.MaGDV.ToLower().Contains(totrinhParameters.ma_gdv.ToLower()));
                    }
                    if (totrinhParameters.Chuatao_dntt == 1)
                    {
                        // Lấy danh sách số hsbt chưa tạo đề nghị thanh toán
                        try
                        {
                            data = data.Where(x => new[] { "09", "14" }.Contains(x.MaTtrang) && x.HoanThienHstt == true && !_context.HsgdDntts.Any(d => d.PrKeyTtrinh == x.PrKey && d.PrKeyTtrinhCt==0));
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("GetDataHsTrinhKy totrinhParameters.Chuatao_dntt == 1 " + ex.ToString());
                        }
                    }
                    var data_grp = data.GroupBy(n => new { n.PrKey, n.MaDonvi, n.SoHsbt, n.SoHsgd, n.TenDttt, n.NgGdich, n.NgayCtu, n.NgayTthat, n.SoTien, n.MaTtrang, n.MaGDV, n.PrKeyHsgd, n.MaDonviTt, n.MaDonviCapDon, n.NgayTtoan, n.NgayPsinh,n.HoanThienHstt, n.PrKeyTTrinhCt,n.Tinhtrang_thanhtoan }).Select(s => new HoSoTrinhKy
                    {
                        PrKey = s.Key.PrKey,
                        MaDonvi = s.Key.MaDonvi,
                        SoHsbt = s.Key.SoHsbt,
                        SoHsgd = s.Key.SoHsgd,
                        TenDttt = s.Key.TenDttt,
                        NgGdich = s.Key.NgGdich,
                        NgayCtu = s.Key.NgayCtu,
                        NgayTthat = s.Key.NgayTthat,
                        SoTien = s.Key.SoTien,
                        MaTtrang = s.Key.MaTtrang,
                        MaGDV = s.Key.MaGDV,
                        PrKeyHsgd = s.Key.PrKeyHsgd,
                        MaDonviTt = s.Key.MaDonviTt,
                        PrKeyNky = s.Max(g => g.PrKeyNky),
                        MaDonviCapDon = s.Key.MaDonviCapDon,
                        NgayTtoan = s.Key.NgayTtoan,
                        NgayPsinh = s.Key.NgayPsinh,
                        HoanThienHstt=s.Key.HoanThienHstt,
                        PrKeyTTrinhCt=s.Key.PrKeyTTrinhCt,
                        Tinhtrang_thanhtoan=s.Key.Tinhtrang_thanhtoan

                    }
                     ).AsQueryable();

                    var data1 = (from a in data_grp
                                 join b in _context.HsgdTtrinhNkies on a.PrKeyNky equals b.PrKey into b1
                                 from b in b1.DefaultIfEmpty()
                                 select new HoSoTrinhKy
                                 {
                                     PrKey = a.PrKey,
                                     MaDonvi = a.MaDonvi,
                                     SoHsbt = a.SoHsbt,
                                     SoHsgd = a.SoHsgd,
                                     TenDttt = a.TenDttt,
                                     NgGdich = a.NgGdich,
                                     NgayCtu = a.NgayCtu,
                                     NgayTthat = a.NgayTthat,
                                     SoTien = a.SoTien,
                                     MaTtrang = a.MaTtrang,
                                     UserNhan = b != null ? b.UserNhan : "",
                                     MaGDV = a.MaGDV,
                                     PrKeyHsgd = a.PrKeyHsgd,
                                     PrKeyNky = a.PrKeyNky,
                                     MaDonviTt = a.MaDonviTt,
                                     NgayDuyet = b != null ? b.NgayCnhat : null,
                                     MaDonviCapDon = a.MaDonviCapDon,
                                     NgayTtoan = a.NgayTtoan,
                                     NgayPsinh = a.NgayPsinh,
                                     HoanThienHstt=a.HoanThienHstt,
                                     PrKeyTTrinhCt=a.PrKeyTTrinhCt,
                                     Tinhtrang_thanhtoan=a.Tinhtrang_thanhtoan

                                 }
                                  ).AsQueryable();
                    if (!string.IsNullOrEmpty(totrinhParameters.ngay_psinh))
                    {
                        data1 = data1.Where(x => x.NgayPsinh != null && x.NgayPsinh.Value.Date == DateTime.ParseExact(totrinhParameters.ngay_psinh, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ma_nguoiduyet))
                    {
                        data1 = data1.Where(x => !string.IsNullOrEmpty(x.UserNhan) && x.UserNhan.ToLower().Contains(totrinhParameters.ma_nguoiduyet.ToLower())).AsQueryable();
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ngay_duyet))
                    {
                        data1 = data1.Where(x => x.NgayDuyet != null && x.NgayDuyet.Value.Date == DateTime.ParseExact(totrinhParameters.ngay_duyet, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                    }
                    // Thêm filter theo nguoiThuHuong
                    if (!string.IsNullOrEmpty(totrinhParameters.nguoiThuHuong))
                    {
                        // Lấy danh sách PrKey của tờ trình có thông tin thụ hưởng phù hợp
                        var prKeyTtrinhWithThuHuong = (from tt in _context.HsgdTtrinhTt
                                                       where !string.IsNullOrEmpty(tt.TenChuTk) &&
                                                             (tt.TenChuTk.ToLower().Contains(totrinhParameters.nguoiThuHuong.ToLower()) ||
                                                              (!string.IsNullOrEmpty(tt.SoTaikhoanNh) && tt.SoTaikhoanNh.ToLower().Contains(totrinhParameters.nguoiThuHuong.ToLower())) ||
                                                              (!string.IsNullOrEmpty(tt.TenNh) && tt.TenNh.ToLower().Contains(totrinhParameters.nguoiThuHuong.ToLower())))
                                                       select tt.FrKey.Value).Distinct().ToList();

                        data1 = data1.Where(x => prKeyTtrinhWithThuHuong.Contains(x.PrKey));
                    }
                    // return PagedList<HoSoTrinhKy>.ToPagedList(data1, totrinhParameters.pageNumber, totrinhParameters.pageSize);
                    var page_list = PagedList<HoSoTrinhKy>.ToPagedList(data1.OrderByDescending(o => o.PrKey), totrinhParameters.pageNumber, totrinhParameters.pageSize);
                    var dmdonvi = ToListWithNoLock(_context.DmDonvis.Where(x => x.MaDonvi != "" && page_list.Select(x => x.MaDonvi).ToArray().Contains(x.MaDonvi)).Select(s => new { s.MaDonvi, s.TenDonvi }).AsQueryable());
                    var dmttrang = ToListWithNoLock(_context.DmTtrangTtrinhs.Where(x => page_list.Select(x => x.MaTtrang).ToArray().Contains(x.MaTtrangTt)).Select(s => new { s.MaTtrangTt, s.TenTtrangTt }).AsQueryable());
                    var dmusnhan = ToListWithNoLock(_context.DmUsers.Where(x => page_list.Select(x => x.UserNhan.ToLower()).ToArray().Contains(x.Oid.ToString().ToLower())).Select(s => new { s.Oid, s.TenUser }).AsQueryable());
                    var dmgdv = ToListWithNoLock(_context.DmUsers.Where(x => page_list.Select(x => x.MaGDV.ToLower()).ToArray().Contains(x.Oid.ToString().ToLower())).Select(s => new { s.Oid, s.TenUser }).AsQueryable());
                    var hsbt_ctu = ToListWithNoLock((from a in _context_pias.HsbtCtus
                                                     where page_list.Select(x => x.SoHsbt).ToArray().Contains(a.SoHsbt)
                                                     select new
                                                     {
                                                         a.SoHsbt,
                                                         a.MaDonviTt
                                                     }).AsQueryable());
                    var dntt = ToListWithNoLock(_context.HsgdDntts.Where(x => page_list.Select(x => x.PrKey).ToArray().Contains(x.PrKeyTtrinh)).Select(s => new { s.PrKeyTtrinh, s.PrKeyTtoanCtu }).AsQueryable());
                    var prKeyHsgdArray = page_list.Select(p => p.PrKeyHsgd).ToArray();

                    var thongTinThuHuong = ToListWithNoLock((from a in _context.HsgdCtus
                                                             join b in _context.HsgdTtrinhs on a.PrKey equals b.PrKeyHsgd
                                                             join c in _context.HsgdTtrinhTt on b.PrKey equals c.FrKey
                                                             where prKeyHsgdArray.Contains(a.PrKey)
                                                             select new
                                                             {
                                                                 PrKeyHsgd = a.PrKey,
                                                                 PrKeyTtrinh = b.PrKey,
                                                                 FrKey = c.FrKey.Value,
                                                                 TenChuTk = c.TenChuTk,
                                                                 SoTaikhoanNh = c.SoTaikhoanNh,
                                                                 TenNh = c.TenNh
                                                             }).AsQueryable());

                    var list_data = page_list.GetRange(0, page_list.Count);
                    var list_data_end = (from a in list_data
                                     join c in dmdonvi on a.MaDonvi equals c.MaDonvi into c1
                                     from c in c1.DefaultIfEmpty()
                                     join d in dmttrang on a.MaTtrang equals d.MaTtrangTt into d1
                                     from d in d1.DefaultIfEmpty()
                                     join e in dmusnhan on a.UserNhan.ToLower() equals e.Oid.ToString().ToLower() into e1
                                     from e in e1.DefaultIfEmpty()
                                     join f in dmgdv on a.MaGDV.ToLower() equals f.Oid.ToString().ToLower() into f1
                                     from f in f1.DefaultIfEmpty()
                                     join g in hsbt_ctu on a.SoHsbt equals g.SoHsbt into g1
                                     from g in g1.DefaultIfEmpty()
                                     join h in dntt on a.PrKey equals h.PrKeyTtrinh into h1
                                     from h in h1.DefaultIfEmpty()

                                     select new HoSoTrinhKy
                                     {
                                         PrKey = a.PrKey,
                                         MaDonvi = a.MaDonvi,
                                         TenDonVi = c != null ? c.TenDonvi : "",
                                         SoHsbt = a.SoHsbt,
                                         SoHsgd = a.SoHsgd,
                                         TenDttt = a.TenDttt,
                                         NgGdich = a.NgGdich,
                                         NgayCtu = a.NgayCtu,
                                         NgayTthat = a.NgayTthat,
                                         NgayCtuText = a.NgayCtu != null ? Convert.ToDateTime(a.NgayCtu).ToString("dd/MM/yyyy") : null,
                                         NgayTthatText = a.NgayTthat != null ? Convert.ToDateTime(a.NgayTthat).ToString("dd/MM/yyyy") : null,
                                         SoTien = a.SoTien,
                                         MaTtrang = a.MaTtrang,
                                         UserNhan = a.UserNhan,
                                         TenTtrangTt = d != null ? d.TenTtrangTt : "",
                                         MaGDV = a.MaGDV,
                                         TenGDV = f != null ? f.TenUser : "",
                                         PrKeyHsgd = a.PrKeyHsgd,
                                         PrKeyNky = a.PrKeyNky,
                                         //PrKeyTtoanCtu = e != null ? e.PrKeyTtoanCtu : 0,
                                         TenNguoiDuyet = e != null ? e.TenUser : "",
                                         MaDonviTt = g != null && !string.IsNullOrEmpty(g.MaDonviTt) ? g.MaDonviTt : a.MaDonvi,
                                         PrKeyTtoanCtu = h != null ? h.PrKeyTtoanCtu : 0,
                                         NgayDuyetText = a.NgayDuyet != null ? Convert.ToDateTime(a.NgayDuyet).ToString("dd/MM/yyyy") : null,
                                         NgayDuyet = a.NgayDuyet,
                                         MaDonviCapDon = a.MaDonviCapDon,
                                         NgayTtoan = a.NgayTtoan,                                         
                                         NguoiThuHuong = string.Join(" | ", thongTinThuHuong?
                                                                .Where(t => t.PrKeyHsgd == a.PrKeyHsgd)
                                                                .Select(t => $"{t.TenChuTk} - {t.SoTaikhoanNh} - {t.TenNh}")
                                                            ?? Enumerable.Empty<string>()),
                                         NgayPsinh = a.NgayPsinh,
                                         NgayPsinhText = a.NgayPsinh != null ? Convert.ToDateTime(a.NgayPsinh).ToString("dd/MM/yyyy") : null,
                                         HoanThienHstt=a.HoanThienHstt,
                                         SoTaikhoanNh = thongTinThuHuong?.FirstOrDefault(t => t.PrKeyHsgd == a.PrKeyHsgd)?.SoTaikhoanNh ?? "",
                                         PrKeyTTrinhCt=a.PrKeyTTrinhCt,
                                         Tinhtrang_thanhtoan=a.Tinhtrang_thanhtoan
                                     }).ToList();                  
                    
                    page_list.RemoveRange(0, page_list.Count);
                    page_list.AddRange(list_data_end);
                    return page_list;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("GetDataHsTrinhKy error = " + ex.ToString());
                return null;
            }
        }
        public PagedList<HoSoTrinhKy> GetDataHsTrinhKyKoHoaDon(string email_login, ToTrinhParameters totrinhParameters)
        {

            try
            {
                //Add_HsgdTtrinhToHsgdDNTT(email_login);
                //Hàm này dùng để kiểm tra các đề nghị thanh toán đã bị xóa và xóa trong bảng hsgd_dntt và gửi email cho giám định viên biết để vào sửa và làm lại
                //XoaHsgdDNTT_GuiEmailGDV(email_login);
                var user = _context.DmUsers.Where(x => x.Mail == email_login).FirstOrDefault();
                if (user != null)
                {
                    List<string> list_ma_tt = totrinhParameters.ma_ttrang_tt.Split(",").ToList();
                    var data = (from a in _context.HsgdTtrinhs.AsNoTracking()
                                join e in _context.HsgdTtrinhCts.AsNoTracking().Where(x => x.MaSp == "050101" && x.MaDKhoan== "05010101") on a.PrKey equals e.FrKey
                                join d in _context.HsgdCtus.AsNoTracking() on a.PrKeyHsgd equals d.PrKey into d1
                                from d in d1.DefaultIfEmpty()
                                join f in _context.HsgdTtrinhNkies.AsNoTracking() on a.PrKey equals f.FrKey into f1
                                from f in f1.DefaultIfEmpty()
                                join c in _context.HsgdTtrinhNkies.AsNoTracking().Where(x => x.Act == "CREATETOTRINH") on a.PrKey equals c.FrKey into c1
                                from c in c1.DefaultIfEmpty()
                                where (
                                    list_ma_tt.Contains(a.MaTtrang) && a.NgayCtu != null && a.NgayCtu.Value.Year == (totrinhParameters.nam_dulieu != 0 ? totrinhParameters.nam_dulieu : DateTime.Now.Year)
                                     )
                                select new HoSoTrinhKy
                                {
                                    PrKey = a.PrKey,
                                    MaDonvi = (a.MaDonvi == "33" ? "32" : a.MaDonvi),
                                    SoHsbt = a.SoHsbt,
                                    SoHsgd = d != null ? d.SoHsgd : "",
                                    TenDttt = a.TenDttt,
                                    NgGdich = a.NgGdich,
                                    NgayCtu = a.NgayCtu,
                                    NgayTthat = a.NgayTthat,
                                    SoTien = e.SotienBt,
                                    MaTtrang = a.MaTtrang,
                                    PrKeyNky = f != null ? f.PrKey : 0,
                                    MaDonviTt = d != null ? d.MaDonviTt : "",
                                    MaGDV = c != null ? c.UserChuyen : "",
                                    PrKeyHsgd = d != null ? d.PrKey : 0,
                                    MaDonviCapDon = d != null ? d.SoDonbh.Substring(3, 2) : "",
                                    NgayTtoan = a.NgayTtoan,
                                    NgayPsinh = d != null ? d.NgayCtu : (DateTime?)null,
                                    HoanThienHstt = d != null ? d.HoanThienHstt : true,
                                    PrKeyTTrinhCt=e.PrKey
                                }
                                  ).AsQueryable();
                    List<string> list_ma_donvi_pquyen = user.MaDonviPquyen.Split(",").ToList();
                    if (new[] { "00", "31", "32" }.Contains(user.MaDonvi))
                    {
                        if (!string.IsNullOrEmpty(user.MaDonviPquyen))
                        {
                            data = data.Where(x => list_ma_donvi_pquyen.Contains(x.MaDonvi) || list_ma_donvi_pquyen.Contains(x.MaDonviTt));
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(user.MaDonviPquyen))
                        {
                            data = data.Where(x => list_ma_donvi_pquyen.Contains(x.MaDonvi) || list_ma_donvi_pquyen.Contains(x.MaDonviTt));
                        }
                        else
                        {
                            data = data.Where(x => x.MaDonvi == user.MaDonvi || list_ma_donvi_pquyen.Contains(x.MaDonviTt));
                        }
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.so_hsbt))
                    {
                        data = data.Where(x => x.SoHsbt.ToLower().Contains(totrinhParameters.so_hsbt.ToLower()));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ma_donvi))
                    {
                        data = data.Where(x => x.MaDonvi.Contains(totrinhParameters.ma_donvi));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.so_hsgd))
                    {
                        data = data.Where(x => x.SoHsgd.Contains(totrinhParameters.so_hsgd));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ten_ndbh))
                    {
                        data = data.Where(x => x.NgGdich.ToLower().Contains(totrinhParameters.ten_ndbh.ToLower()));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.bien_ks))
                    {
                        data = data.Where(x => x.TenDttt.ToLower().Contains(totrinhParameters.bien_ks.ToLower()));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ma_trangthai))
                    {
                        data = data.Where(x => x.MaTtrang.ToLower().Contains(totrinhParameters.ma_trangthai.ToLower()));
                    }
                    if (totrinhParameters.so_tien != 0)
                    {
                        data = data.Where(x => x.SoTien == totrinhParameters.so_tien);
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ngay_tt))
                    {
                        data = data.Where(x => x.NgayTthat != null && x.NgayTthat.Value.Date == DateTime.ParseExact(totrinhParameters.ngay_tt, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ngay_nhap))
                    {
                        data = data.Where(x => x.NgayCtu != null && x.NgayCtu.Value.Date == DateTime.ParseExact(totrinhParameters.ngay_nhap, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                    }

                    if (!string.IsNullOrEmpty(totrinhParameters.ma_gdv))
                    {
                        data = data.Where(x => !string.IsNullOrEmpty(x.MaGDV) && x.MaGDV.ToLower().Contains(totrinhParameters.ma_gdv.ToLower()));
                    }
                    if (totrinhParameters.Chuatao_dntt == 1)
                    {
                        // Lấy danh sách số hsbt chưa tạo đề nghị thanh toán
                        try
                        {
                            data = data.Where(x => new[] { "09", "14" }.Contains(x.MaTtrang) && x.HoanThienHstt == true && !_context.HsgdDntts.Any(d => d.PrKeyTtrinh == x.PrKey && d.PrKeyTtrinhCt == x.PrKeyTTrinhCt));
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("GetDataHsTrinhKy totrinhParameters.Chuatao_dntt == 1 " + ex.ToString());
                        }
                    }
                    var data_grp = data.GroupBy(n => new { n.PrKey, n.MaDonvi, n.SoHsbt, n.SoHsgd, n.TenDttt, n.NgGdich, n.NgayCtu, n.NgayTthat, n.SoTien, n.MaTtrang, n.MaGDV, n.PrKeyHsgd, n.MaDonviTt, n.MaDonviCapDon, n.NgayTtoan, n.NgayPsinh, n.HoanThienHstt, n.PrKeyTTrinhCt }).Select(s => new HoSoTrinhKy
                    {
                        PrKey = s.Key.PrKey,
                        MaDonvi = s.Key.MaDonvi,
                        SoHsbt = s.Key.SoHsbt,
                        SoHsgd = s.Key.SoHsgd,
                        TenDttt = s.Key.TenDttt,
                        NgGdich = s.Key.NgGdich,
                        NgayCtu = s.Key.NgayCtu,
                        NgayTthat = s.Key.NgayTthat,
                        SoTien = s.Key.SoTien,
                        MaTtrang = s.Key.MaTtrang,
                        MaGDV = s.Key.MaGDV,
                        PrKeyHsgd = s.Key.PrKeyHsgd,
                        MaDonviTt = s.Key.MaDonviTt,
                        PrKeyNky = s.Max(g => g.PrKeyNky),
                        MaDonviCapDon = s.Key.MaDonviCapDon,
                        NgayTtoan = s.Key.NgayTtoan,
                        NgayPsinh = s.Key.NgayPsinh,
                        HoanThienHstt = s.Key.HoanThienHstt,
                        PrKeyTTrinhCt= s.Key.PrKeyTTrinhCt

                    }
                     ).AsQueryable();

                    var data1 = (from a in data_grp
                                 join b in _context.HsgdTtrinhNkies on a.PrKeyNky equals b.PrKey into b1
                                 from b in b1.DefaultIfEmpty()
                                 select new HoSoTrinhKy
                                 {
                                     PrKey = a.PrKey,
                                     MaDonvi = a.MaDonvi,
                                     SoHsbt = a.SoHsbt,
                                     SoHsgd = a.SoHsgd,
                                     TenDttt = a.TenDttt,
                                     NgGdich = a.NgGdich,
                                     NgayCtu = a.NgayCtu,
                                     NgayTthat = a.NgayTthat,
                                     SoTien = a.SoTien,
                                     MaTtrang = a.MaTtrang,
                                     UserNhan = b != null ? b.UserNhan : "",
                                     MaGDV = a.MaGDV,
                                     PrKeyHsgd = a.PrKeyHsgd,
                                     PrKeyNky = a.PrKeyNky,
                                     MaDonviTt = a.MaDonviTt,
                                     NgayDuyet = b != null ? b.NgayCnhat : null,
                                     MaDonviCapDon = a.MaDonviCapDon,
                                     NgayTtoan = a.NgayTtoan,
                                     NgayPsinh = a.NgayPsinh,
                                     HoanThienHstt = a.HoanThienHstt,
                                     PrKeyTTrinhCt = a.PrKeyTTrinhCt


                                 }
                                  ).AsQueryable();
                    if (!string.IsNullOrEmpty(totrinhParameters.ngay_psinh))
                    {
                        data1 = data1.Where(x => x.NgayPsinh != null && x.NgayPsinh.Value.Date == DateTime.ParseExact(totrinhParameters.ngay_psinh, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ma_nguoiduyet))
                    {
                        data1 = data1.Where(x => !string.IsNullOrEmpty(x.UserNhan) && x.UserNhan.ToLower().Contains(totrinhParameters.ma_nguoiduyet.ToLower())).AsQueryable();
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ngay_duyet))
                    {
                        data1 = data1.Where(x => x.NgayDuyet != null && x.NgayDuyet.Value.Date == DateTime.ParseExact(totrinhParameters.ngay_duyet, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                    }
                    // Thêm filter theo nguoiThuHuong
                    if (!string.IsNullOrEmpty(totrinhParameters.nguoiThuHuong))
                    {
                        // Lấy danh sách PrKey của tờ trình có thông tin thụ hưởng phù hợp
                        var prKeyTtrinhWithThuHuong = (from tt in _context.HsgdTtrinhTt
                                                       where !string.IsNullOrEmpty(tt.TenChuTk) &&
                                                             (tt.TenChuTk.ToLower().Contains(totrinhParameters.nguoiThuHuong.ToLower()) ||
                                                              (!string.IsNullOrEmpty(tt.SoTaikhoanNh) && tt.SoTaikhoanNh.ToLower().Contains(totrinhParameters.nguoiThuHuong.ToLower())) ||
                                                              (!string.IsNullOrEmpty(tt.TenNh) && tt.TenNh.ToLower().Contains(totrinhParameters.nguoiThuHuong.ToLower())))
                                                       select tt.FrKey.Value).Distinct().ToList();

                        data1 = data1.Where(x => prKeyTtrinhWithThuHuong.Contains(x.PrKey));
                    }
                    // return PagedList<HoSoTrinhKy>.ToPagedList(data1, totrinhParameters.pageNumber, totrinhParameters.pageSize);
                    var page_list = PagedList<HoSoTrinhKy>.ToPagedList(data1.OrderByDescending(o => o.PrKey), totrinhParameters.pageNumber, totrinhParameters.pageSize);
                    var dmdonvi = ToListWithNoLock(_context.DmDonvis.Where(x => x.MaDonvi != "" && page_list.Select(x => x.MaDonvi).ToArray().Contains(x.MaDonvi)).Select(s => new { s.MaDonvi, s.TenDonvi }).AsQueryable());
                    var dmttrang = ToListWithNoLock(_context.DmTtrangTtrinhs.Where(x => page_list.Select(x => x.MaTtrang).ToArray().Contains(x.MaTtrangTt)).Select(s => new { s.MaTtrangTt, s.TenTtrangTt }).AsQueryable());
                    var dmusnhan = ToListWithNoLock(_context.DmUsers.Where(x => page_list.Select(x => x.UserNhan.ToLower()).ToArray().Contains(x.Oid.ToString().ToLower())).Select(s => new { s.Oid, s.TenUser }).AsQueryable());
                    var dmgdv = ToListWithNoLock(_context.DmUsers.Where(x => page_list.Select(x => x.MaGDV.ToLower()).ToArray().Contains(x.Oid.ToString().ToLower())).Select(s => new { s.Oid, s.TenUser }).AsQueryable());
                    var hsbt_ctu = ToListWithNoLock((from a in _context_pias.HsbtCtus
                                                     where page_list.Select(x => x.SoHsbt).ToArray().Contains(a.SoHsbt)
                                                     select new
                                                     {
                                                         a.SoHsbt,
                                                         a.MaDonviTt
                                                     }).AsQueryable());
                    var dntt = ToListWithNoLock(_context.HsgdDntts.Where(x => page_list.Select(x => x.PrKey).ToArray().Contains(x.PrKeyTtrinh)).Select(s => new { s.PrKeyTtrinh, s.PrKeyTtoanCtu }).AsQueryable());
                    var prKeyHsgdArray = page_list.Select(p => p.PrKeyHsgd).ToArray();

                    var thongTinThuHuong = ToListWithNoLock((from a in _context.HsgdCtus
                                                             join b in _context.HsgdTtrinhs on a.PrKey equals b.PrKeyHsgd
                                                             join c in _context.HsgdTtrinhTt on b.PrKey equals c.FrKey
                                                             where prKeyHsgdArray.Contains(a.PrKey)
                                                             select new
                                                             {
                                                                 PrKeyHsgd = a.PrKey,
                                                                 PrKeyTtrinh = b.PrKey,
                                                                 FrKey = c.FrKey.Value,
                                                                 TenChuTk = c.TenChuTk,
                                                                 SoTaikhoanNh = c.SoTaikhoanNh,
                                                                 TenNh = c.TenNh
                                                             }).AsQueryable());

                    var list_data = page_list.GetRange(0, page_list.Count);
                    var list_data_end = (from a in list_data
                                         join c in dmdonvi on a.MaDonvi equals c.MaDonvi into c1
                                         from c in c1.DefaultIfEmpty()
                                         join d in dmttrang on a.MaTtrang equals d.MaTtrangTt into d1
                                         from d in d1.DefaultIfEmpty()
                                         join e in dmusnhan on a.UserNhan.ToLower() equals e.Oid.ToString().ToLower() into e1
                                         from e in e1.DefaultIfEmpty()
                                         join f in dmgdv on a.MaGDV.ToLower() equals f.Oid.ToString().ToLower() into f1
                                         from f in f1.DefaultIfEmpty()
                                         join g in hsbt_ctu on a.SoHsbt equals g.SoHsbt into g1
                                         from g in g1.DefaultIfEmpty()
                                         join h in dntt on a.PrKey equals h.PrKeyTtrinh into h1
                                         from h in h1.DefaultIfEmpty()

                                         select new HoSoTrinhKy
                                         {
                                             PrKey = a.PrKey,
                                             MaDonvi = a.MaDonvi,
                                             TenDonVi = c != null ? c.TenDonvi : "",
                                             SoHsbt = a.SoHsbt,
                                             SoHsgd = a.SoHsgd,
                                             TenDttt = a.TenDttt,
                                             NgGdich = a.NgGdich,
                                             NgayCtu = a.NgayCtu,
                                             NgayTthat = a.NgayTthat,
                                             NgayCtuText = a.NgayCtu != null ? Convert.ToDateTime(a.NgayCtu).ToString("dd/MM/yyyy") : null,
                                             NgayTthatText = a.NgayTthat != null ? Convert.ToDateTime(a.NgayTthat).ToString("dd/MM/yyyy") : null,
                                             SoTien = a.SoTien,
                                             MaTtrang = a.MaTtrang,
                                             UserNhan = a.UserNhan,
                                             TenTtrangTt = d != null ? d.TenTtrangTt : "",
                                             MaGDV = a.MaGDV,
                                             TenGDV = f != null ? f.TenUser : "",
                                             PrKeyHsgd = a.PrKeyHsgd,
                                             PrKeyNky = a.PrKeyNky,
                                             //PrKeyTtoanCtu = e != null ? e.PrKeyTtoanCtu : 0,
                                             TenNguoiDuyet = e != null ? e.TenUser : "",
                                             MaDonviTt = g != null && !string.IsNullOrEmpty(g.MaDonviTt) ? g.MaDonviTt : a.MaDonvi,
                                             PrKeyTtoanCtu = h != null ? h.PrKeyTtoanCtu : 0,
                                             NgayDuyetText = a.NgayDuyet != null ? Convert.ToDateTime(a.NgayDuyet).ToString("dd/MM/yyyy") : null,
                                             NgayDuyet = a.NgayDuyet,
                                             MaDonviCapDon = a.MaDonviCapDon,
                                             NgayTtoan = a.NgayTtoan,
                                             NguoiThuHuong = string.Join(" | ", thongTinThuHuong?
                                                                    .Where(t => t.PrKeyHsgd == a.PrKeyHsgd)
                                                                    .Select(t => $"{t.TenChuTk} - {t.SoTaikhoanNh} - {t.TenNh}")
                                                                ?? Enumerable.Empty<string>()),
                                             NgayPsinh = a.NgayPsinh,
                                             NgayPsinhText = a.NgayPsinh != null ? Convert.ToDateTime(a.NgayPsinh).ToString("dd/MM/yyyy") : null,
                                             HoanThienHstt = a.HoanThienHstt,
                                             SoTaikhoanNh = thongTinThuHuong?.FirstOrDefault(t => t.PrKeyHsgd == a.PrKeyHsgd)?.SoTaikhoanNh ?? "",
                                             PrKeyTTrinhCt = a.PrKeyTTrinhCt
                                         }).ToList();

                    page_list.RemoveRange(0, page_list.Count);
                    page_list.AddRange(list_data_end);
                    return page_list;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("GetDataHsTrinhKy error = " + ex.ToString());
                return null;
            }
        }
        public void XoaHsgdDNTT_GuiEmailGDV(string email_login)
        {
            try
            {
                var user = _context.DmUsers.Where(x => x.Mail == email_login).FirstOrDefault();
                //ĐOẠN CODE CHO XÓA HSGD_DNTT ĐỂ LẬP LẠI ĐỀ NGHỊ THANH TOÁN
                var hsgddntt = (
                              from a in _context.HsgdTtrinhs
                              join b in _context.HsgdDntts
                                  on a.PrKey equals b.PrKeyTtrinh
                              where new[] { "09", "14" }.Contains(a.MaTtrang)
                                 && a.MaDonvi ==user.MaDonvi && b.PrKeyTtoanCtu != 0 && b.MaCbo != ""
                              select b
                          ).AsNoTracking().ToList();

                // 2. Lấy list các PrKeyTtoanCtu từ hsgddntt
                var listPrKeyTtoanCtu = hsgddntt
                    .Select(x => x.PrKeyTtoanCtu)
                    .Distinct()
                    .ToList();

                // 2. Lấy các PrKeyTtoanCtu này để kiểm tra xem bên thanh toán còn không
                HashSet<decimal> existedKeys;
                using (var _context_pias_ttoan = new Pvs2024TToanContext())
                {
                    existedKeys = _context_pias_ttoan.TtoanCtus.AsNoTracking()
                                          .Where(x => listPrKeyTtoanCtu.Contains(x.PrKey))
                                          .Select(x => x.PrKey)
                                          .ToHashSet();

                }
                // 4. Tìm những bản ghi HsgdDntt KHÔNG tồn tại trong thanh toán → cần xoá
                var listdelete = hsgddntt
                    .Where(d => !existedKeys.Contains(d.PrKeyTtoanCtu))
                    .ToList();
                List<HsgdDntt> emailDatalistdel = new List<HsgdDntt>();
                               
                if (listdelete.Any())
                {
                    emailDatalistdel = listdelete;  // list để gửi email

                    foreach (var item in listdelete)
                    {
                      item.MaCbcnvXly = item.PrKeyTtrinh.ToString();                        
                    }
                    _context.SaveChanges();
                    foreach (var item in listdelete)
                    {
                        item.PrKeyTtrinh = 0;
                    }
                    _context.SaveChanges();
                }

                if (emailDatalistdel.Any())
                {
                    //Chạy thead gửi email
                    Task.Run(() => SendEmail_XOADNTT_GDV(emailDatalistdel));
                }
            }
            catch (Exception ex)
            {           
                _logger.Error("XoaHsgdDNTT_GuiEmailGDV error = " + ex.ToString());
                
            }
        }

        public void Add_HsgdTtrinhToHsgdDNTT(string email_login)
        {
            try
            {
                var user = _context.DmUsers.Where(x => x.Mail == email_login).FirstOrDefault();

                //ĐOẠN CODE XỬ LÝ CHO NHỮNG TRƯỜNG HỢP KHÔNG LẬP DNTT MÀ XỬ LÝ TRÊN PIAS. CÁC TRƯỜNG HỢP NÀY INSERT CHAY PR_KEY_TTRINH VÀO BẢNG HSGD_DNTT ĐỂ HS KHÔNG HIỂN THỊ TRÊN TAB HS CHỜ THANH TOÁN NỮA

                //1/ Lấy các hsgdttrinh ở trạng thái 09,14  mà không có trong hsgd_dntt
                var maTrangThai = new[] { "09", "14" };

                var result =
                    (
                        from a in _context.HsgdCtus.AsNoTracking()
                        join b in _context.HsgdTtrinhs.AsNoTracking()
                            on a.PrKey equals b.PrKeyHsgd
                        where maTrangThai.Contains(b.MaTtrang)
                           && !_context.HsgdDntts.Any(x => x.PrKeyTtrinh == b.PrKey)
                           && a.MaDonvi == user.MaDonvi                           
                        select new
                        {
                            a.SoHsbt,
                            b.PrKey
                        }
                    ).ToList();
                //lấy danh sách hsbt
                var soHsbtList = result
                .Select(x => x.SoHsbt)
                .Distinct()
                .ToList();
                //2/ Kiểm tra bên Pias xem các hồ sơ này đã chuyển 03 hoàn toàn hay chưa
                var trangThaiChapNhan = new[] { "03", "04" };
                var trangThaiLoai = new[] { "01", "02", "05" };

                var soHsbtHopLe = _context_pias.HsbtCtus.AsNoTracking()
                    .Where(c =>
                        c.MaDonvi == user.MaDonvi
                        &&
                        soHsbtList.Contains(c.SoHsbt)
                        &&
                        _context_pias.HsbtCts.AsNoTracking().Any(d =>
                            d.FrKey == c.PrKey
                            && trangThaiChapNhan.Contains(d.MaTtrangBt)
                        )
                        &&
                        !_context_pias.HsbtCts.AsNoTracking().Any(d =>
                            d.FrKey == c.PrKey
                            && trangThaiLoai.Contains(d.MaTtrangBt)
                        )
                    )
                    .Select(c => c.SoHsbt)
                    .Distinct()
                    .ToList();
                //Lấy key của các số hsbt đã chuyển 03 hoàn toàn để dưa vào 
                var prKeys = result
                    .Where(x => soHsbtHopLe.Contains(x.SoHsbt))
                    .Select(x => x.PrKey)
                    .Distinct()
                    .ToList();
                //3/ Nếu đã chuyển 03 rồi thì insert lại pr_key_ttrinh vào bảng hsgd_dntt để không nhảy vào danh sách hồ sơ chờ thanh toán
                foreach (var prKey in prKeys)
                {
                    var entity = new HsgdDntt
                    {
                        PrKeyTtoanCtu = 0,
                        PrKeyTtrinh = prKey,
                        MaCbo = string.Empty,
                        MaCbcnvXly = "03AuToAdd"
                    };

                    _context.HsgdDntts.Add(entity);
                }
                _context.SaveChanges();                
            }
            catch (Exception ex)
            {
                _logger.Error("Add_HsgdTtrinhToHsgdDNTT error = " + ex.ToString());

            }
        }
        public PagedList<HoSoTrinhKy> GetDataHsDaThanhToan(string email_login, ToTrinhParameters totrinhParameters)
        {
            //List<HsgdTtrinhView> result = new List<HsgdTtrinhView>();
            try
            {
                var user = _context.DmUsers.Where(x => x.Mail == email_login).FirstOrDefault();
                if (user != null)
                {
                    List<string> list_ma_tt = totrinhParameters.ma_ttrang_tt.Split(",").ToList();
                    var data = (from a in _context.HsgdTtrinhs
                                join d in _context.HsgdCtus on a.PrKeyHsgd equals d.PrKey into d1
                                from d in d1.DefaultIfEmpty()
                                join f in _context.HsgdTtrinhNkies on a.PrKey equals f.FrKey into f1
                                from f in f1.DefaultIfEmpty()
                                join c in _context.HsgdTtrinhNkies.Where(x => x.Act == "CREATETOTRINH") on a.PrKey equals c.FrKey into c1
                                from c in c1.DefaultIfEmpty()
                                where (
                                    list_ma_tt.Contains(a.MaTtrang) && a.NgayCtu != null && a.NgayCtu.Value.Year == (totrinhParameters.nam_dulieu != 0 ? totrinhParameters.nam_dulieu : DateTime.Now.Year)
                                     )
                                select new HoSoTrinhKy
                                {
                                    PrKey = a.PrKey,
                                    MaDonvi = (a.MaDonvi == "33" ? "32" : a.MaDonvi),
                                    SoHsbt = a.SoHsbt,
                                    SoHsgd = d != null ? d.SoHsgd : "",
                                    TenDttt = a.TenDttt,
                                    NgGdich = a.NgGdich,
                                    NgayCtu = a.NgayCtu,
                                    NgayTthat = a.NgayTthat,
                                    SoTien = a.SoTien,
                                    MaTtrang = a.MaTtrang,
                                    PrKeyNky = f != null ? f.PrKey : 0,
                                    MaDonviTt = d.MaDonviTt,
                                    //MaGDV = d != null ? ( d.NguoiXuly != "" ? d.NguoiXuly : (d.MaUser != null ? d.MaUser.ToString() : "")) : "",
                                    MaGDV = c != null ? c.UserChuyen : "",
                                    PrKeyHsgd = d != null ? d.PrKey : 0,
                                    MaDonviCapDon = d.SoDonbh.Substring(3, 2),
                                    NgayTtoan = a.NgayTtoan,
                                    NgayPsinh = d != null ? d.NgayCtu : (DateTime?)null,
                                    HoanThienHstt = d.HoanThienHstt
                                }
                                  ).AsQueryable();
                    List<string> list_ma_donvi_pquyen = user.MaDonviPquyen.Split(",").ToList();
                    if (new[] { "00", "31", "32" }.Contains(user.MaDonvi))
                    {
                        if (!string.IsNullOrEmpty(user.MaDonviPquyen))
                        {
                            data = data.Where(x => list_ma_donvi_pquyen.Contains(x.MaDonvi) || list_ma_donvi_pquyen.Contains(x.MaDonviTt));
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(user.MaDonviPquyen))
                        {
                            data = data.Where(x => list_ma_donvi_pquyen.Contains(x.MaDonvi) || list_ma_donvi_pquyen.Contains(x.MaDonviTt));
                        }
                        else
                        {
                            data = data.Where(x => x.MaDonvi == user.MaDonvi || list_ma_donvi_pquyen.Contains(x.MaDonviTt));
                        }
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.so_hsbt))
                    {
                        data = data.Where(x => x.SoHsbt.ToLower().Contains(totrinhParameters.so_hsbt.ToLower()));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ma_donvi))
                    {
                        data = data.Where(x => x.MaDonvi.Contains(totrinhParameters.ma_donvi));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.so_hsgd))
                    {
                        data = data.Where(x => x.SoHsgd.Contains(totrinhParameters.so_hsgd));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ten_ndbh))
                    {
                        data = data.Where(x => x.NgGdich.ToLower().Contains(totrinhParameters.ten_ndbh.ToLower()));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.bien_ks))
                    {
                        data = data.Where(x => x.TenDttt.ToLower().Contains(totrinhParameters.bien_ks.ToLower()));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ma_trangthai))
                    {
                        data = data.Where(x => x.MaTtrang.ToLower().Contains(totrinhParameters.ma_trangthai.ToLower()));
                    }
                    if (totrinhParameters.so_tien != 0)
                    {
                        data = data.Where(x => x.SoTien == totrinhParameters.so_tien);
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ngay_tt))
                    {
                        data = data.Where(x => x.NgayTthat != null && x.NgayTthat.Value.Date == DateTime.ParseExact(totrinhParameters.ngay_tt, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ngay_nhap))
                    {
                        data = data.Where(x => x.NgayCtu != null && x.NgayCtu.Value.Date == DateTime.ParseExact(totrinhParameters.ngay_nhap, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                    }

                    if (!string.IsNullOrEmpty(totrinhParameters.ma_gdv))
                    {
                        data = data.Where(x => !string.IsNullOrEmpty(x.MaGDV) && x.MaGDV.ToLower().Contains(totrinhParameters.ma_gdv.ToLower()));
                    }
                    
                     data = data.Where(x =>_context.HsgdDntts.Any(d => d.PrKeyTtrinh == x.PrKey));                        

                    var data_grp = data.GroupBy(n => new { n.PrKey, n.MaDonvi, n.SoHsbt, n.SoHsgd, n.TenDttt, n.NgGdich, n.NgayCtu, n.NgayTthat, n.SoTien, n.MaTtrang, n.MaGDV, n.PrKeyHsgd, n.MaDonviTt, n.MaDonviCapDon, n.NgayTtoan, n.NgayPsinh }).Select(s => new HoSoTrinhKy
                    {
                        PrKey = s.Key.PrKey,
                        MaDonvi = s.Key.MaDonvi,
                        SoHsbt = s.Key.SoHsbt,
                        SoHsgd = s.Key.SoHsgd,
                        TenDttt = s.Key.TenDttt,
                        NgGdich = s.Key.NgGdich,
                        NgayCtu = s.Key.NgayCtu,
                        NgayTthat = s.Key.NgayTthat,
                        SoTien = s.Key.SoTien,
                        MaTtrang = s.Key.MaTtrang,
                        MaGDV = s.Key.MaGDV,
                        PrKeyHsgd = s.Key.PrKeyHsgd,
                        MaDonviTt = s.Key.MaDonviTt,
                        PrKeyNky = s.Max(g => g.PrKeyNky),
                        MaDonviCapDon = s.Key.MaDonviCapDon,
                        NgayTtoan = s.Key.NgayTtoan,
                        NgayPsinh = s.Key.NgayPsinh
                    }
                     ).AsQueryable();

                    var data1 = (from a in data_grp
                                 join b in _context.HsgdTtrinhNkies on a.PrKeyNky equals b.PrKey into b1
                                 from b in b1.DefaultIfEmpty()
                                 select new HoSoTrinhKy
                                 {
                                     PrKey = a.PrKey,
                                     MaDonvi = a.MaDonvi,
                                     SoHsbt = a.SoHsbt,
                                     SoHsgd = a.SoHsgd,
                                     TenDttt = a.TenDttt,
                                     NgGdich = a.NgGdich,
                                     NgayCtu = a.NgayCtu,
                                     NgayTthat = a.NgayTthat,
                                     SoTien = a.SoTien,
                                     MaTtrang = a.MaTtrang,
                                     UserNhan = b != null ? b.UserNhan : "",
                                     MaGDV = a.MaGDV,
                                     PrKeyHsgd = a.PrKeyHsgd,
                                     PrKeyNky = a.PrKeyNky,
                                     MaDonviTt = a.MaDonviTt,
                                     NgayDuyet = b != null ? b.NgayCnhat : null,
                                     MaDonviCapDon = a.MaDonviCapDon,
                                     NgayTtoan = a.NgayTtoan,
                                     NgayPsinh = a.NgayPsinh
                                 }
                                  ).AsQueryable();
                    if (!string.IsNullOrEmpty(totrinhParameters.ngay_psinh))
                    {
                        data1 = data1.Where(x => x.NgayPsinh != null && x.NgayPsinh.Value.Date == DateTime.ParseExact(totrinhParameters.ngay_psinh, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ma_nguoiduyet))
                    {
                        data1 = data1.Where(x => !string.IsNullOrEmpty(x.UserNhan) && x.UserNhan.ToLower().Contains(totrinhParameters.ma_nguoiduyet.ToLower())).AsQueryable();
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ngay_duyet))
                    {
                        data1 = data1.Where(x => x.NgayDuyet != null && x.NgayDuyet.Value.Date == DateTime.ParseExact(totrinhParameters.ngay_duyet, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                    }
                    // return PagedList<HoSoTrinhKy>.ToPagedList(data1, totrinhParameters.pageNumber, totrinhParameters.pageSize);
                    var page_list = PagedList<HoSoTrinhKy>.ToPagedList(data1.OrderByDescending(o => o.PrKey), totrinhParameters.pageNumber, totrinhParameters.pageSize);
                    var dmdonvi = ToListWithNoLock(_context.DmDonvis.Where(x => x.MaDonvi != "" && page_list.Select(x => x.MaDonvi).ToArray().Contains(x.MaDonvi)).Select(s => new { s.MaDonvi, s.TenDonvi }).AsQueryable());
                    var dmttrang = ToListWithNoLock(_context.DmTtrangTtrinhs.Where(x => page_list.Select(x => x.MaTtrang).ToArray().Contains(x.MaTtrangTt)).Select(s => new { s.MaTtrangTt, s.TenTtrangTt }).AsQueryable());
                    var dmusnhan = ToListWithNoLock(_context.DmUsers.Where(x => page_list.Select(x => x.UserNhan.ToLower()).ToArray().Contains(x.Oid.ToString().ToLower())).Select(s => new { s.Oid, s.TenUser }).AsQueryable());
                    var dmgdv = ToListWithNoLock(_context.DmUsers.Where(x => page_list.Select(x => x.MaGDV.ToLower()).ToArray().Contains(x.Oid.ToString().ToLower())).Select(s => new { s.Oid, s.TenUser }).AsQueryable());
                    var hsbt_ctu = ToListWithNoLock((from a in _context_pias.HsbtCtus
                                                     where page_list.Select(x => x.SoHsbt).ToArray().Contains(a.SoHsbt)
                                                     select new
                                                     {
                                                         a.SoHsbt,
                                                         a.MaDonviTt
                                                     }).AsQueryable());
                    var dntt = ToListWithNoLock(_context.HsgdDntts.Where(x => page_list.Select(x => x.PrKey).ToArray().Contains(x.PrKeyTtrinh)).Select(s => new { s.PrKeyTtrinh, s.PrKeyTtoanCtu }).AsQueryable());
                    var prKeyHsgdArray = page_list.Select(p => p.PrKeyHsgd).ToArray();

                    var thongTinThuHuong = ToListWithNoLock((from a in _context.HsgdCtus
                                                             join b in _context.HsgdTtrinhs on a.PrKey equals b.PrKeyHsgd
                                                             join c in _context.HsgdTtrinhTt on b.PrKey equals c.FrKey
                                                             where prKeyHsgdArray.Contains(a.PrKey)
                                                             select new
                                                             {
                                                                 PrKeyHsgd = a.PrKey,
                                                                 PrKeyTtrinh = b.PrKey,
                                                                 FrKey = c.FrKey.Value,
                                                                 TenChuTk = c.TenChuTk,
                                                                 SoTaikhoanNh = c.SoTaikhoanNh,
                                                                 TenNh = c.TenNh
                                                             }).AsQueryable());

                    var list_data = page_list.GetRange(0, page_list.Count);
                    var list_data_end = (from a in list_data
                                         join c in dmdonvi on a.MaDonvi equals c.MaDonvi into c1
                                         from c in c1.DefaultIfEmpty()
                                         join d in dmttrang on a.MaTtrang equals d.MaTtrangTt into d1
                                         from d in d1.DefaultIfEmpty()
                                         join e in dmusnhan on a.UserNhan.ToLower() equals e.Oid.ToString().ToLower() into e1
                                         from e in e1.DefaultIfEmpty()
                                         join f in dmgdv on a.MaGDV.ToLower() equals f.Oid.ToString().ToLower() into f1
                                         from f in f1.DefaultIfEmpty()
                                         join g in hsbt_ctu on a.SoHsbt equals g.SoHsbt into g1
                                         from g in g1.DefaultIfEmpty()
                                         join h in dntt on a.PrKey equals h.PrKeyTtrinh into h1
                                         from h in h1.DefaultIfEmpty()

                                         select new HoSoTrinhKy
                                         {
                                             PrKey = a.PrKey,
                                             MaDonvi = a.MaDonvi,
                                             TenDonVi = c != null ? c.TenDonvi : "",
                                             SoHsbt = a.SoHsbt,
                                             SoHsgd = a.SoHsgd,
                                             TenDttt = a.TenDttt,
                                             NgGdich = a.NgGdich,
                                             NgayCtu = a.NgayCtu,
                                             NgayTthat = a.NgayTthat,
                                             NgayCtuText = a.NgayCtu != null ? Convert.ToDateTime(a.NgayCtu).ToString("dd/MM/yyyy") : null,
                                             NgayTthatText = a.NgayTthat != null ? Convert.ToDateTime(a.NgayTthat).ToString("dd/MM/yyyy") : null,
                                             SoTien = a.SoTien,
                                             MaTtrang = a.MaTtrang,
                                             UserNhan = a.UserNhan,
                                             TenTtrangTt = d != null ? d.TenTtrangTt : "",
                                             MaGDV = a.MaGDV,
                                             TenGDV = f != null ? f.TenUser : "",
                                             PrKeyHsgd = a.PrKeyHsgd,
                                             PrKeyNky = a.PrKeyNky,
                                             //PrKeyTtoanCtu = e != null ? e.PrKeyTtoanCtu : 0,
                                             TenNguoiDuyet = e != null ? e.TenUser : "",
                                             MaDonviTt = g != null && !string.IsNullOrEmpty(g.MaDonviTt) ? g.MaDonviTt : a.MaDonvi,
                                             PrKeyTtoanCtu = h != null ? h.PrKeyTtoanCtu : 0,
                                             NgayDuyetText = a.NgayDuyet != null ? Convert.ToDateTime(a.NgayDuyet).ToString("dd/MM/yyyy") : null,
                                             NgayDuyet = a.NgayDuyet,
                                             MaDonviCapDon = a.MaDonviCapDon,
                                             NgayTtoan = a.NgayTtoan,
                                             NguoiThuHuong = string.Join(" | ", thongTinThuHuong
                                                                .Where(t => t.PrKeyHsgd == a.PrKeyHsgd)
                                                                .Select(t => $"{t.TenChuTk} - {t.SoTaikhoanNh} - {t.TenNh}")),
                                             NgayPsinh = a.NgayPsinh,
                                             NgayPsinhText = a.NgayPsinh != null ? Convert.ToDateTime(a.NgayPsinh).ToString("dd/MM/yyyy") : null
                                         }).ToList();      
                    page_list.RemoveRange(0, page_list.Count);
                    page_list.AddRange(list_data_end);
                    return page_list;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("GetDataHsTrinhKy error = " + ex.ToString());
                return null;
            }
        }
        public PagedList<HoSoTrinhKy> GetDataHsTrinhKyLanhDao(string email_login, ToTrinhParameters totrinhParameters)
        {
            //List<HsgdTtrinhView> result = new List<HsgdTtrinhView>();
            try
            {
                var user = _context.DmUsers.Where(x => x.Mail == email_login).FirstOrDefault();
                if (user != null)
                {
                    //var hsbt_ctu = ToListWithNoLock((from a in _context_pias.HsbtCtus
                    //                                 where page_list.Select(x => x.SoHsbt).ToArray().Contains(a.SoHsbt)
                    //                                 select new
                    //                                 {
                    //                                     a.SoHsbt,
                    //                                     a.MaDonviTt
                    //                                 }).AsQueryable());
                    //lấy trạng thái 12,13,6 với so_tien > 200 000 000
                    List<string> list_ma_ttgd = totrinhParameters.ma_ttrang_gd.Split(",").ToList();
                    //var filteredListMaTtgd = list_ma_ttgd.Where(x => x != "6").ToList();
                    List<string> list_ma_tt = totrinhParameters.ma_ttrang_tt.Split(",").ToList();
                    var data = (
                                from d in _context.HsgdCtus
                                join f in _context.NhatKies.Where(x => x.MaTtrangGd == "12") on d.PrKey equals f.FrKey
                                where
                                 list_ma_ttgd.Contains(d.MaTtrangGd) &&
                                 d.NgayCtu.HasValue
                                && d.NgayCtu.Value.Year == (totrinhParameters.nam_dulieu != 0 ? totrinhParameters.nam_dulieu : DateTime.Now.Year)

                                select new HoSoTrinhKy
                                {
                                    //PrKey = d.PrKey,
                                    MaDonvi = d.MaDonvi,
                                    PrKeyTtoanCtu = d.PrKeyBt,
                                    SoHsbt = null,
                                    SoHsgd = d != null ? d.SoHsgd : "",
                                    TenDttt = d.BienKsoat,
                                    NgGdich = d.TenKhach,
                                    NgayCtu = d.NgayCtu,
                                    NgayTthat = d.NgayTthat,
                                    UserNhan = f.MaUser.ToString(),
                                    //SoTien = a.SoTien,
                                    MaTtrang = d.MaTtrangGd,
                                    //PrKeyNky = f != null ? f.PrKey : 0,
                                    MaGDV = d != null ? (d.NguoiXuly != "" ? d.NguoiXuly : (d.MaUser != null ? d.MaUser.ToString() : "")) : "",
                                    //MaGDV = c != null ? c.UserChuyen : "",
                                    PrKeyHsgd = d != null ? d.PrKey : 0
                                }
                                  ).AsQueryable();

                    List<string> list_ma_donvi_pquyen = user.MaDonviPquyen.Split(",").ToList();
                    if (new[] { "00", "31", "32" }.Contains(user.MaDonvi))
                    {
                        if (!string.IsNullOrEmpty(user.MaDonviPquyen))
                        {
                            data = data.Where(x => list_ma_donvi_pquyen.Contains(x.MaDonvi));
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(user.MaDonviPquyen))
                        {
                            data = data.Where(x => list_ma_donvi_pquyen.Contains(x.MaDonvi));
                        }
                        else
                        {
                            data = data.Where(x => x.MaDonvi == user.MaDonvi);
                        }
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.so_hsbt))
                    {
                        data = data.Where(x => x.SoHsbt.ToLower().Contains(totrinhParameters.so_hsbt.ToLower()));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ma_donvi))
                    {
                        data = data.Where(x => x.MaDonvi.Contains(totrinhParameters.ma_donvi));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.so_hsgd))
                    {
                        data = data.Where(x => x.SoHsgd.Contains(totrinhParameters.so_hsgd));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ten_ndbh))
                    {
                        data = data.Where(x => x.NgGdich.ToLower().Contains(totrinhParameters.ten_ndbh.ToLower()));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.bien_ks))
                    {
                        data = data.Where(x => x.TenDttt.ToLower().Contains(totrinhParameters.bien_ks.ToLower()));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ma_trangthai))
                    {
                        data = data.Where(x => x.MaTtrang.ToLower().Contains(totrinhParameters.ma_trangthai.ToLower()));
                    }
                    if (totrinhParameters.so_tien != 0)
                    {
                        data = data.Where(x => x.SoTien == totrinhParameters.so_tien);
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ngay_tt))
                    {
                        data = data.Where(x => x.NgayTthat != null && x.NgayTthat.Value.Date == DateTime.ParseExact(totrinhParameters.ngay_tt, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                    }
                    if (!string.IsNullOrEmpty(totrinhParameters.ngay_nhap))
                    {
                        data = data.Where(x => x.NgayCtu != null && x.NgayCtu.Value.Date == DateTime.ParseExact(totrinhParameters.ngay_nhap, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                    }

                    if (!string.IsNullOrEmpty(totrinhParameters.ma_gdv))
                    {
                        data = data.Where(x => !string.IsNullOrEmpty(x.MaGDV) && x.MaGDV.ToLower().Contains(totrinhParameters.ma_gdv.ToLower()));
                    }
                    var data_grp = data.GroupBy(n => new { n.MaDonvi, n.PrKeyTtoanCtu, n.SoHsgd, n.TenDttt, n.NgayCtu, n.NgayTthat, n.MaTtrang, n.PrKeyHsgd, n.UserNhan, n.MaGDV }).Select(s => new HoSoTrinhKy
                    {
                        //PrKey = s.Key.PrKey,

                        MaDonvi = s.Key.MaDonvi,
                        //SoHsbt = s.Key.SoHsbt,
                        SoHsgd = s.Key.SoHsgd,
                        TenDttt = s.Key.TenDttt,
                        PrKeyTtoanCtu = s.Key.PrKeyTtoanCtu,
                        NgayCtu = s.Key.NgayCtu,
                        NgayTthat = s.Key.NgayTthat,
                        //SoTien = s.Key.SoTien,
                        MaTtrang = s.Key.MaTtrang,
                        MaGDV = s.Key.MaGDV,
                        PrKeyHsgd = s.Key.PrKeyHsgd,
                        UserNhan = s.Key.UserNhan
                        //PrKeyNky = s.Max(g => g.PrKeyNky)
                    }
                                  ).AsQueryable();

                    var data1 = (from a in data_grp
                                 join b in _context.HsgdDxCts on a.PrKeyTtoanCtu equals b.PrKeyHsbtCtu into b1
                                 from b in b1.DefaultIfEmpty()
                                 select new HoSoTrinhKy
                                 {

                                     MaDonvi = a.MaDonvi,
                                     SoHsbt = null,
                                     SoHsgd = a.SoHsgd,
                                     TenDttt = a.TenDttt,
                                     PrKeyTtoanCtu = a.PrKeyTtoanCtu,
                                     NgayCtu = a.NgayCtu,
                                     NgayTthat = a.NgayTthat,
                                     SoTien = a.SoTien,
                                     MaTtrang = a.MaTtrang,
                                     UserNhan = a != null ? a.UserNhan : "",
                                     MaGDV = a.MaGDV,
                                     PrKeyHsgd = a.PrKeyHsgd,
                                     PrKeyNky = b != null ? b.PrKey : 0,
                                     //NgayDuyet = b != null ? b.NgayCnhat : null,
                                 }
                                  ).ToList();
                    foreach (var obj in data1)
                    {
                        if (obj.PrKeyNky != 0)
                        {
                            var objReloadSum = _dx_repo.ReloadSum(obj.PrKeyNky);
                            if (objReloadSum != null && objReloadSum.Count > 0)
                            {
                                for (int j = 0; j < objReloadSum.Count; j++)
                                {
                                    obj.SoTien += (decimal)objReloadSum[j].StBl;
                                }
                            }
                        }

                    }
                    var groupedData = data1
  .GroupBy(x => x.SoHsgd)
                    .Select(g => new HoSoTrinhKy
                    {
                        MaDonvi = g.First().MaDonvi,
                        SoHsbt = null,
                        SoHsgd = g.Key,
                        TenDttt = g.First().TenDttt,
                        PrKeyTtoanCtu = g.First().PrKeyTtoanCtu,
                        NgayCtu = g.First().NgayCtu,
                        NgayTthat = g.First().NgayTthat,
                        SoTien = g.Sum(x => x.SoTien),
                        UserNhan = g.First().UserNhan,
                        MaTtrang = g.First().MaTtrang,
                        MaGDV = g.First().MaGDV,
                        PrKeyHsgd = g.First().PrKeyHsgd,
                        PrKeyNky = g.First().PrKeyNky
                    }).OrderByDescending(x => x.PrKeyHsgd)
                    .ToList();
                    //if (!string.IsNullOrEmpty(totrinhParameters.ma_nguoiduyet))
                    //{
                    //    data1 = data1.Where(x => !string.IsNullOrEmpty(x.UserNhan) && x.UserNhan.ToLower().Contains(totrinhParameters.ma_nguoiduyet.ToLower())).AsQueryable();
                    //}
                    //if (!string.IsNullOrEmpty(totrinhParameters.ngay_duyet))
                    //{
                    //    data1 = data1.Where(x => x.NgayDuyet != null && x.NgayDuyet.Value.Date == DateTime.ParseExact(totrinhParameters.ngay_duyet, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                    //}
                    // return PagedList<HoSoTrinhKy>.ToPagedList(data1, totrinhParameters.pageNumber, totrinhParameters.pageSize);
                    var page_list = PagedList<HoSoTrinhKy>.ToPagedList(groupedData.AsQueryable(), totrinhParameters.pageNumber, totrinhParameters.pageSize);
                    var dmdonvi = ToListWithNoLock(_context.DmDonvis.Where(x => x.MaDonvi != "" && page_list.Select(x => x.MaDonvi).ToArray().Contains(x.MaDonvi)).Select(s => new { s.MaDonvi, s.TenDonvi }).AsQueryable());
                    //var dmttrang = ToListWithNoLock(_context.DmTtrangTtrinhs.Where(x => page_list.Select(x => x.MaTtrang).ToArray().Contains(x.MaTtrangTt)).Select(s => new { s.MaTtrangTt, s.TenTtrangTt }).AsQueryable());
                    var dmusnhan = ToListWithNoLock(_context.DmUsers.Where(x => page_list.Select(x => x.UserNhan.ToLower()).ToArray().Contains(x.Oid.ToString().ToLower())).Select(s => new { s.Oid, s.TenUser }).AsQueryable());
                    var dmgdv = ToListWithNoLock(_context.DmUsers.Where(x => page_list.Select(x => x.MaGDV.ToLower()).ToArray().Contains(x.Oid.ToString().ToLower())).Select(s => new { s.Oid, s.TenUser }).AsQueryable());
                    var hsbt_ctu = ToListWithNoLock((from a in _context_pias.HsbtCtus
                                                     where page_list.Select(x => x.PrKeyTtoanCtu).ToArray().Contains(a.PrKey)
                                                     select new
                                                     {
                                                         a.PrKey,
                                                         a.SoHsbt,
                                                         a.MaDonviTt
                                                     }).AsQueryable());
                    //var dntt = ToListWithNoLock(_context.HsgdDntts.Where(x => page_list.Select(x => x.PrKey).ToArray().Contains(x.PrKeyTtrinh)).Select(s => new { s.PrKeyTtrinh, s.PrKeyTtoanCtu }).AsQueryable());
                    var list_data = page_list.GetRange(0, page_list.Count);
                    var list_data_end = (from a in list_data
                                         join c in dmdonvi on a.MaDonvi equals c.MaDonvi into c1
                                         from c in c1.DefaultIfEmpty()
                                             //join d in dmttrang on a.MaTtrang equals d.MaTtrangTt into d1
                                             //from d in d1.DefaultIfEmpty()
                                         join e in dmusnhan on a.UserNhan.ToLower() equals e.Oid.ToString().ToLower() into e1
                                         from e in e1.DefaultIfEmpty()
                                         join f in dmgdv on a.MaGDV.ToLower() equals f.Oid.ToString().ToLower() into f1
                                         from f in f1.DefaultIfEmpty()
                                         join g in hsbt_ctu on a.PrKeyTtoanCtu equals g.PrKey into g1
                                         from g in g1.DefaultIfEmpty()
                                             //join h in dntt on a.PrKey equals h.PrKeyTtrinh into h1
                                             //from h in h1.DefaultIfEmpty()
                                         select new HoSoTrinhKy
                                         {
                                             PrKey = a.PrKey,
                                             MaDonvi = a.MaDonvi,
                                             TenDonVi = c != null ? c.TenDonvi : "",
                                             SoHsbt = g != null ? g.SoHsbt : "",
                                             SoHsgd = a.SoHsgd,
                                             TenDttt = a.TenDttt,

                                             NgayCtu = a.NgayCtu,
                                             NgayTthat = a.NgayTthat,
                                             //NgayCtuText = a.NgayCtu != null ? Convert.ToDateTime(a.NgayCtu).ToString("dd/MM/yyyy") : null,
                                             NgayTthatText = a.NgayTthat != null ? Convert.ToDateTime(a.NgayTthat).ToString("dd/MM/yyyy") : null,
                                             SoTien = a.SoTien,
                                             MaTtrang = a.MaTtrang,
                                             UserNhan = a.UserNhan,
                                             //TenTtrangTt = d != null ? d.TenTtrangTt : "",
                                             MaGDV = a.MaGDV,
                                             TenGDV = f != null ? f.TenUser : "",
                                             PrKeyHsgd = a.PrKeyHsgd,
                                             PrKeyNky = a.PrKeyNky,
                                             //PrKeyTtoanCtu = e != null ? e.PrKeyTtoanCtu : 0,
                                             TenNguoiDuyet = e != null ? e.TenUser : "",
                                             MaDonviTt = g != null && !string.IsNullOrEmpty(g.MaDonviTt) ? g.MaDonviTt : a.MaDonvi,
                                             PrKeyTtoanCtu = a.PrKeyTtoanCtu,
                                             NgayDuyetText = a.NgayDuyet != null ? Convert.ToDateTime(a.NgayDuyet).ToString("dd/MM/yyyy") : null,
                                             NgayDuyet = a.NgayDuyet

                                         }).ToList();
                    page_list.RemoveRange(0, page_list.Count);
                    page_list.AddRange(list_data_end);
                    return page_list;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("GetDataHsTrinhKy error = " + ex.ToString());
                return null;
            }
        }
        public HsgdTtrinh? GetTtrinhByOid(Guid oid)
        {
            HsgdTtrinh? obj_result = new HsgdTtrinh();
            try
            {
                obj_result = _context.HsgdTtrinhs.Where(x => x.Oid == oid).FirstOrDefault();
            }
            catch (Exception ex)
            {
            }
            return obj_result;
        }
        public CombinedTtrinhResult4 GetPrintToTrinhTPC(decimal pr_key_hsgd_ctu, string email, int loai_tt)
        {
            try
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
                var wordPdfRequest = new WordToPdfRequest();
                wordPdfRequest.ListData = new List<EntityContent>();
                UpdateProperties update = new UpdateProperties();
                var hsgd_ctu = GetThongTinToTrinhTPC(pr_key_hsgd_ctu, email, loai_tt);
                if (hsgd_ctu != null)
                {

                    update.AddEntityContent(wordPdfRequest, "[DON_VIU]", hsgd_ctu.DonviU);
                    update.AddEntityContent(wordPdfRequest, "[DON_VI]", hsgd_ctu.Donvi);
                    update.AddEntityContent(wordPdfRequest, "[DONVI_ME]", hsgd_ctu.DonviMe);
                    update.AddEntityContent(wordPdfRequest, "[TP]", hsgd_ctu.TP);
                    update.AddEntityContent(wordPdfRequest, "[DATE]", DateTime.Now.Day.ToString());
                    update.AddEntityContent(wordPdfRequest, "[MONTH]", DateTime.Now.Month.ToString());
                    update.AddEntityContent(wordPdfRequest, "[YEAR]", DateTime.Now.Year.ToString());
                    update.AddEntityContent(wordPdfRequest, "[TEN_KHACH]", hsgd_ctu.TenKhach);
                    update.AddEntityContent(wordPdfRequest, "[BIEN_KSOAT]", hsgd_ctu.BienKsoat);
                    update.AddEntityContent(wordPdfRequest, "[SO_SERI]", hsgd_ctu.SoSeri.ToString());
                    update.AddEntityContent(wordPdfRequest, "[NGAY_DAU_SERI]", hsgd_ctu.NgayDauSeri);
                    update.AddEntityContent(wordPdfRequest, "[NGAY_CUOI_SERI]", hsgd_ctu.NgayCuoiSeri);
                    update.AddEntityContent(wordPdfRequest, "[TEN_DONVI]", hsgd_ctu.TenDonvi);
                    update.AddEntityContent(wordPdfRequest, "[SOTIEN_THUCTE]", hsgd_ctu.SoTienThucTe.ToString("#,###", cul.NumberFormat));
                    update.AddEntityContent(wordPdfRequest, "[NGAY_THUPHI]", hsgd_ctu.NgayThuPhi ?? "");
                    update.AddEntityContent(wordPdfRequest, "[NGAY_TTHAT]", ("Khoảng " + hsgd_ctu.GioTthat + " phút ngày " + hsgd_ctu.NgayTthat));

                    List<string> list_DiaDiemtt = ContentHelper.SplitString(ContentHelper.formatNewLine(hsgd_ctu.DiaDiemtt), 255);
                    for (int i = 0; i < list_DiaDiemtt.Count(); i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[DIA_DIEMTT{i}]", list_DiaDiemtt[i]);
                    }
                    for (int i = list_DiaDiemtt.Count(); i < 11; i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[DIA_DIEMTT{i}]", "");
                    }
                    List<string> list_NguyenNhanTtat = ContentHelper.SplitString(ContentHelper.formatNewLine(hsgd_ctu.NguyenNhanTtat), 255);
                    for (int i = 0; i < list_NguyenNhanTtat.Count(); i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[NGUYEN_NHANTT{i}]", list_NguyenNhanTtat[i]);
                    }
                    for (int i = list_NguyenNhanTtat.Count(); i < 20; i++)
                    {
                        update.AddEntityContent(wordPdfRequest, $"[NGUYEN_NHANTT{i}]", "");
                    }
                    update.AddEntityContent(wordPdfRequest, "[LOAI_XE]", hsgd_ctu.TenLoaiXe);
                    update.AddEntityContent(wordPdfRequest, "[NAM_SX]", hsgd_ctu.NamSx.ToString());
                    update.AddEntityContent(wordPdfRequest, "[GARA_SUACHUA]", hsgd_ctu.TenGara);
                    update.AddEntityContent(wordPdfRequest, "[TEN_SP]", hsgd_ctu.TenSP);
                    update.AddEntityContent(wordPdfRequest, "[TONG_CHIPHI]", hsgd_ctu.TongChiPhi.ToString("#,###", cul.NumberFormat));
                    update.AddEntityContent(wordPdfRequest, "[SOTIEN_GIAM]", hsgd_ctu.SotienGiam.ToString("#,###", cul.NumberFormat));
                    update.AddEntityContent(wordPdfRequest, "[TRACHNHIEMPVI]", hsgd_ctu.TrachNhiemPVI.ToString("#,###", cul.NumberFormat));
                    update.AddEntityContent(wordPdfRequest, "[TRACHNHIEMPVI_BC]", ContentHelper.NumberToText((double)hsgd_ctu.TrachNhiemPVI));
                    update.AddEntityContent(wordPdfRequest, "[TYLE_PHI]", hsgd_ctu.TylephiPvi.ToString());
                    update.AddEntityContent(wordPdfRequest, "[DKBS]", hsgd_ctu.DsDkbs);
                    update.AddEntityContent(wordPdfRequest, "[TEN_PTGD]", hsgd_ctu.TenPTGD.ToUpper());
                }

                var listData = wordPdfRequest.ListData;
                _logger.Information("PrintToTrinhTPC " + JsonConvert.SerializeObject(listData));
                var listNew = new CombinedTtrinhResult4
                {
                    ThirdQueryResults = listData
                };

                return listNew;

            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                return null;
            }
        }
        public string UploadToTrinhTPC(UploadToTrinhTPC entity, string email_login)
        {
            var result = "";
            try
            {
                var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == entity.PrKeyHsgdCtu).FirstOrDefault();
                if (hsgd_ctu != null)
                {
                    var check_tpc = CheckHsgdTPC(entity.PrKeyHsgdCtu, "9,10", email_login);
                    if (check_tpc)
                    {
                        // file
                        if (!string.IsNullOrEmpty(entity.fileTT.FileData))
                        {
                            DownloadSettings downloadSettings = new DownloadSettings();
                            Word2PdfSettings word2PdfSettings = new Word2PdfSettings();
                            word2PdfSettings.FilePath = _configuration["Word2PdfSettings:FilePath"] ?? "";
                            var optionsDownloadSettings = Microsoft.Extensions.Options.Options.Create(downloadSettings);
                            var optionsWord2PdfSettings = Microsoft.Extensions.Options.Options.Create(word2PdfSettings);


                            var contentHelper = new ContentHelper(optionsDownloadSettings, optionsWord2PdfSettings, _logger);
                            var downloadFileResult = contentHelper.ConvertFileWordUploadToPdf(entity.fileTT.FileData);
                            string folderUpload = _configuration["UploadSettings:FolderUpload_MDF1"] ?? "";
                            string url_upload = _configuration["DownloadSettings:UlpoadServer_MDF1"] ?? "";
                            var utilityHelper = new UtilityHelper(_logger);
                            var file_path = "";
                            //if (string.IsNullOrEmpty(hsgd_ctu.PathTotrinhTpc))
                            //{
                            file_path = utilityHelper.UploadFile_ToAPI(downloadFileResult.Data, entity.fileTT.FileExtension, folderUpload, url_upload, false);
                            //}
                            //else
                            //{
                            //    file_path = utilityHelper.UploadFileOld_ToAPI(entity.fileTT.FileData, hsgd_ctu.PathTotrinhTpc, folderUpload, url_upload);
                            //}
                            if (!string.IsNullOrEmpty(file_path))
                            {
                                hsgd_ctu.PathTotrinhTpc = file_path;
                                hsgd_ctu.LoaiTotrinhTpc = entity.LoaiTotrinhTpc;
                                _context.HsgdCtus.Update(hsgd_ctu);
                                _context.SaveChanges();
                                result = "Upload file thành công.";
                            }
                            else
                            {
                                result = "Upload mediafile không thành công.";
                            }
                        }
                        else
                        {
                            result = "File upload lỗi.Vui lòng kiểm tra lại.";
                        }
                    }
                    else
                    {
                        result = "Không phải HS trên phân cấp của TVP. Không thực hiện chức năng này.";
                    }

                }

            }
            catch (Exception ex)
            {
                _logger.Error("UploadToTrinhTPC:", ex);
                _logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
            }
            return result;
        }
        public string PheDuyetHsTpc(decimal pr_key_hsgd_ctu, string email_login)
        {
            string result = "";
            try
            {
                var user_login = _context.DmUsers.Where(x => x.Mail == email_login).FirstOrDefault();
                var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == pr_key_hsgd_ctu).FirstOrDefault();
                if (hsgd_ctu != null)
                {
                    // thực hiện replace text CB xử lý
                    string url_download = _configuration["DownloadSettings:DownloadServer_MDF1"] ?? "";
                    //tải file
                    string Path_orgin = UtilityHelper.getPathAndCopyTempServer(hsgd_ctu.PathTotrinhTpc, url_download);
                    string Path_result = Path_orgin + "_edited.pdf";
                    if (System.IO.File.Exists(Path_orgin))
                    {
                        _logger.Information("PheDuyetHsTpc pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " tải file tờ trình tpc thành công");
                        var pdfEdit = new PDFEdit(_logger);
                        if (pdfEdit.ReplaceTextInPDF(Path_orgin, Path_result, pdfEdit.ListKeyWord("VP_TAT", user_login.MaUser ?? "", user_login.TenUser ?? ""), true))
                        {
                            _logger.Information("PheDuyetHsTpc pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " ReplaceTextInPDF VP_TAT thành công");
                        }
                        else
                        {
                            result = "Phê duyệt hồ sơ TPC không thành công";
                            _logger.Information("PheDuyetHsTpc pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " ReplaceTextInPDF VP_TAT thất bại");
                            return result;
                        }
                        var utilityHelper = new UtilityHelper(_logger);
                        string folderUpload = _configuration["UploadSettings:FolderUpload_MDF1"] ?? "";
                        string url_upload = _configuration["DownloadSettings:UlpoadServer_MDF1"] ?? "";
                        var file_path = utilityHelper.UploadFileOld_ToAPI(Convert.ToBase64String(System.IO.File.ReadAllBytes(Path_orgin)), hsgd_ctu.PathTotrinhTpc, folderUpload, url_upload);
                        _logger.Information("PheDuyetHsTpc pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " upload file tờ trình sau ReplaceTextInPDF thành công");
                        // kiểm tra và xóa file ở local 
                        try
                        {
                            if (System.IO.File.Exists(Path_orgin))
                            {
                                System.IO.File.Delete(Path_orgin);
                            }
                            if (System.IO.File.Exists(Path_result))
                            {
                                System.IO.File.Delete(Path_result);
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        ////update nhật ký
                        //NhatKy nky = new NhatKy();
                        //nky.PrKey = 0;
                        //nky.FrKey = Convert.ToInt32(pr_key_hsgd_ctu);
                        //nky.MaTtrangGd = "12";
                        //nky.TenTtrangGd = "HS TPC TVP chờ duyệt";
                        //nky.GhiChu = "Chuyển HSGĐ sang hồ sơ ngoài phân cấp TVP chờ duyệt";
                        //nky.NgayCapnhat = DateTime.Now;
                        //nky.MaUser = user_login.Oid;
                        //_context.NhatKies.Add(nky);
                        //_context.SaveChanges();
                        //if (nky.PrKey > 0)
                        //{
                        //    if (loai_tt == 0)
                        //    {
                        //        SendEmail_ToTrinh_TPC(pr_key_hsgd_ctu, email_login, loai_tt);

                        //    }
                        //    result = "Phê duyệt hồ sơ TPC thành công";
                        //    _logger.Information("PheDuyetHsTpc pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " thành công");
                        //}
                        //else
                        //{
                        //    result = "Phê duyệt hồ sơ TPC thất bại";
                        //    _logger.Information("PheDuyetHsTpc pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " thất bại");
                        //}
                        if (hsgd_ctu.LoaiTotrinhTpc == 0)
                        {
                            SendEmail_ToTrinh_TPC(pr_key_hsgd_ctu, email_login, hsgd_ctu.LoaiTotrinhTpc);

                        }
                        result = "Phê duyệt hồ sơ TPC thành công";
                        _logger.Information("PheDuyetHsTpc pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " thành công");
                    }
                    else
                    {

                        result = "Lỗi không tải được file tờ trình tpc";
                        _logger.Information("PheDuyetHsTpc pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " lỗi không tải được file tờ trình tpc");
                    }
                }


            }
            catch (Exception ex)
            {
                _logger.Information("PheDuyetHsTpc pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " error: " + ex);
            }
            return result;
        }
        public bool CheckHsgdTPC(decimal pr_key_hsgd_ctu, string ma_ttrang, string email_login)
        {
            var result = false;
            try
            {
                var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == pr_key_hsgd_ctu).FirstOrDefault();
                if (hsgd_ctu != null)
                {
                    var hsgd_dx_ct = _context.HsgdDxCts.Where(x => x.PrKeyHsbtCtu == hsgd_ctu.PrKeyBt).ToList();
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
                    var ma_ttrang_gd = _context.NhatKies.Where(x => x.FrKey == pr_key_hsgd_ctu).OrderByDescending(o => o.PrKey).Select(s => s.MaTtrangGd).FirstOrDefault() ?? "";

                    // Check các phân quyền theo số tiền được ủy quyền
                    //var user_login = _context.DmUsers.Where(x => x.Mail == email_login).FirstOrDefault();
                    //string currentOID = user_login.Oid.ToString().ToLower();
                    //DmUqHstpc uyQuyenUser = _context.DmUqHstpcs.Where(x => x.MaUserUq.ToLower().Equals(currentOID) && x.GhSotienUq >= sum_trachnhiempvi).OrderBy(x => x.GhSotienUq).FirstOrDefault();
                    if (sum_trachnhiempvi >= 250000000 && ma_ttrang.Contains(ma_ttrang_gd))
                    {
                        result = true;
                    }

                }

            }
            catch (Exception ex)
            {
                _logger.Error("CheckHsgdTPC:", ex);
            }
            return result;
        }
        public ThongTinToTrinhTPC GetThongTinToTrinhTPC(decimal pr_key_hsgd_ctu, string email_login, int loai_tt)
        {
            try
            {

                var wordPdfRequest = new WordToPdfRequest();
                wordPdfRequest.ListData = new List<EntityContent>();
                UpdateProperties update = new UpdateProperties();
                var tt_tpc = FirstOrDefaultWithNoLock((from a in _context.HsgdCtus
                                                       join donvi in _context.DmDonvis on a.MaDonvi equals donvi.MaDonvi
                                                       where a.PrKey == pr_key_hsgd_ctu
                                                       select new ThongTinToTrinhTPC
                                                       {
                                                           TenKhach = a.TenKhach,
                                                           BienKsoat = a.BienKsoat,
                                                           SoSeri = a.SoSeri,
                                                           NgayDauSeri = a.NgayDauSeri != null ? Convert.ToDateTime(a.NgayDauSeri).ToString("dd/MM/yyyy") : "",
                                                           NgayCuoiSeri = a.NgayCuoiSeri != null ? Convert.ToDateTime(a.NgayCuoiSeri).ToString("dd/MM/yyyy") : "",
                                                           TenDonvi = donvi.TenDonvi,
                                                           LoaiXe = a.LoaiXe,
                                                           HieuXe = a.HieuXe,
                                                           NamSx = a.NamSx,
                                                           SoTienThucTe = a.SoTienThucTe,
                                                           NgayTthat = a.NgayTthat != null ? Convert.ToDateTime(a.NgayTthat).ToString("dd/MM/yyyy") : "",
                                                           GioTthat = a.NgayTthat != null ? Convert.ToDateTime(a.NgayTthat).ToString("HH:mm") : "",
                                                           DiaDiemtt = a.DiaDiemtt,
                                                           NguyenNhanTtat = a.NguyenNhanTtat,
                                                           MaDonvi = a.MaDonvi,
                                                           MaDonvigd = a.MaDonvigd,
                                                           SoDonbh = a.SoDonbh,
                                                           PrKeyBt = a.PrKeyBt,
                                                           PathTotrinhTpc = a.PathTotrinhTpc
                                                       }).AsQueryable());
                if (tt_tpc != null)
                {
                    //lấy dữ liệu
                    char checkMark = '\u2612';
                    char emptyBox = '\u2610';
                    var thong_tin = (from vars in _context_pias.DmVars
                                     where vars.MaDonvi == tt_tpc.MaDonvi &&
                                           (vars.Bien == "DONVI_ME" || vars.Bien == "DON_VI" || vars.Bien == "TP")
                                     select vars).ToList();
                    var donvi_me = "";
                    var donvi = "";
                    var tp = "";
                    foreach (var record in thong_tin)
                    {
                        switch (record.Bien)
                        {
                            case "DONVI_ME":
                                donvi_me = record.GiaTri;
                                break;
                            case "DON_VI":
                                donvi = record.GiaTri;
                                break;
                            case "TP":
                                tp = record.GiaTri;
                                break;
                            default:

                                break;

                        }
                    }
                    var don_vi = _context_pias.DmVars.Where(x => x.MaDonvi == tt_tpc.MaDonvigd && x.Bien == "DON_VI").FirstOrDefault();
                    if (don_vi != null)
                    {
                        tt_tpc.DonviU = don_vi.GiaTri.ToUpper();
                        tt_tpc.Donvi = don_vi.GiaTri;
                    }
                    else
                    {
                        tt_tpc.DonviU = "";
                        tt_tpc.Donvi = "";
                    }
                    tt_tpc.DonviMe = donvi_me.ToUpper();
                    tt_tpc.TP = tp;

                    // lấy giá trị thực tế xe theo đơn gốc trên pias
                    var seriPhiBH = GetSoPhiBH(tt_tpc.SoDonbh, tt_tpc.SoSeri);
                    var mtnGtbhVnd = GetMtnGtbh(tt_tpc.SoDonbh);
                    if (mtnGtbhVnd != 0)
                    {
                        tt_tpc.SoTienThucTe = mtnGtbhVnd;
                    }
                    else
                    {
                        if (seriPhiBH != null)
                        {
                            tt_tpc.SoTienThucTe = seriPhiBH.MtnGtbhVnd;
                        }
                        else
                        {
                            tt_tpc.SoTienThucTe = tt_tpc.SoTienThucTe;
                        }
                    }

                    // lấy ngày thu phí
                    var pr_key_goc = _context_pias.NvuBhtCtus.Where(x => x.SoDonbhSdbs == tt_tpc.SoDonbh).Select(x => x.PrKey).FirstOrDefault();

                    if (pr_key_goc != 0)
                    {
                        var ky_phi = _context_pias.NvuBhtKyphis.Where(x => x.FrKey == pr_key_goc).Select(s => new NvuBhtKyphiView
                        {
                            PrKey = s.PrKey,
                            FrKey = s.FrKey,
                            Stt = s.Stt,
                            NgayHl = s.NgayHl != null ? Convert.ToDateTime(s.NgayHl).ToString("dd/MM/yyyy") : null,
                            TylePhithu = s.TylePhithu,
                            SoTien = s.SoTien
                        }).OrderByDescending(o => o.Stt).FirstOrDefault();
                        if (ky_phi != null)
                        {
                            tt_tpc.NgayThuPhi = ky_phi.NgayHl ?? "";
                        }
                    }
                    else
                    {
                        tt_tpc.NgayThuPhi = "";
                    }

                    // lấy thông tin đề xuất

                    var hsgd_dx_ct = ToListWithNoLock((from a in _context.HsgdDxCts
                                                       where a.PrKeyHsbtCtu == tt_tpc.PrKeyBt
                                                       select new
                                                       {
                                                           PrKey = a.PrKey,
                                                           PrKeyHsbtCt = a.PrKeyHsbtCt,
                                                           HieuXe = a.HieuXe,
                                                           LoaiXe = a.LoaiXe,
                                                           NamSx = a.NamSx,
                                                           MaGara = a.MaGara,
                                                           TenSp = a.MaSp == "050104" ? "VCX" : "TNDS"
                                                       }).AsQueryable());
                    decimal sum_chiphi = 0;
                    decimal sum_trachnhiempvi = 0;
                    decimal sum_ctkh = 0;
                    decimal sum_ggsc = 0;
                    decimal sum_doitru = 0;
                    decimal sum_giamtru = 0;

                    if (hsgd_dx_ct.Count > 0)
                    {
                        if (loai_tt == 0)
                        {
                            for (int i = 0; i < hsgd_dx_ct.Count; i++)
                            {
                                var sum_dx = _dx_repo.ReloadSum(hsgd_dx_ct[i].PrKey);
                                if (sum_dx != null && sum_dx.Count > 0)
                                {
                                    sum_chiphi += sum_dx[0].SumSoTienTtsc ?? 0;
                                    sum_trachnhiempvi += sum_dx[0].StBl ?? 0;
                                    sum_ctkh += sum_dx[0].SoTienctkh ?? 0;
                                    sum_ggsc += sum_dx[0].SumSoTienGgsc ?? 0;
                                    sum_doitru += sum_dx[0].SumSoTienDoitru ?? 0;
                                    sum_giamtru += (sum_dx[0].SumSoTienGiamtru ?? 0) > 0 ? (sum_dx[0].SumSoTienGiamtru ?? 0) : (sum_dx[0].SoTienGtbt ?? 0);
                                }
                            }
                        }
                        var hsgd_dx_ct_tt = hsgd_dx_ct.GroupBy(g => 1 == 1)
                                .Select(s => new
                                {
                                    NamSx = s.Max(x => x.NamSx),
                                    LoaiXe = s.Max(x => x.LoaiXe),
                                    HieuXe = s.Max(x => x.HieuXe),
                                    MaGara = s.Max(x => x.MaGara),
                                    TenSp = s.Min(x => x.TenSp)
                                }).FirstOrDefault();
                        if (hsgd_dx_ct_tt != null)
                        {
                            var hieu_xe = _context.DmHieuxes.Where(x => x.PrKey == hsgd_dx_ct_tt.HieuXe).Select(s => s.HieuXe).FirstOrDefault();
                            var loai_xe = _context.DmLoaixes.Where(x => x.PrKey == hsgd_dx_ct_tt.LoaiXe).Select(s => s.LoaiXe).FirstOrDefault();
                            var tt_xe = hieu_xe ?? "";
                            if (!string.IsNullOrEmpty(loai_xe))
                            {
                                tt_xe += " " + loai_xe;
                            }
                            tt_tpc.TenLoaiXe = tt_xe;
                            tt_tpc.NamSx = hsgd_dx_ct_tt.NamSx;
                            var ten_gara = _context.DmGaRas.Where(x => x.MaGara == hsgd_dx_ct_tt.MaGara).Select(s => s.TenGara).FirstOrDefault();
                            tt_tpc.TenGara = ten_gara != null ? ten_gara : "";
                            tt_tpc.TenSP = hsgd_dx_ct_tt.TenSp ?? "VCX";
                        }
                        else
                        {
                            tt_tpc.TenLoaiXe = "";
                            tt_tpc.NamSx = 0;
                            tt_tpc.TenGara = "";
                            tt_tpc.TenSP = "VCX";
                        }
                    }
                    else
                    {
                        tt_tpc.TenLoaiXe = "";
                        tt_tpc.NamSx = 0;
                        tt_tpc.TenGara = "";
                        tt_tpc.TenSP = "VCX";
                    }
                    tt_tpc.TongChiPhi = sum_chiphi;
                    tt_tpc.SotienGiam = (sum_ctkh + sum_ggsc + sum_doitru + sum_giamtru);
                    tt_tpc.TrachNhiemPVI = sum_trachnhiempvi;
                    //lấy thông tin tỷ lệ phí, đkbs bên TCD
                    var tcd_seri = (from a in _context_pvs_tcd.TcdBhtSeris
                                    join b in _context_pvs_tcd.TcdBhtCtus on a.FrKey equals b.PrKey
                                    where b.SoDonPias == tt_tpc.SoDonbh
                                    select new
                                    {
                                        TylephiPvi = a.TylephiPvi,
                                        DsDkbs = a.DsDkbs
                                    }).FirstOrDefault();
                    if (tcd_seri != null)
                    {
                        tt_tpc.TylephiPvi = tcd_seri.TylephiPvi;
                        update.AddEntityContent(wordPdfRequest, "[TYLE_PHI]", tcd_seri.TylephiPvi.ToString());
                        var dkbs = "";
                        if (string.IsNullOrEmpty(tcd_seri.DsDkbs))
                        {
                            dkbs = "ĐKBS: " + tcd_seri.DsDkbs;
                        }
                        tt_tpc.DsDkbs = dkbs;
                    }
                    else
                    {
                        tt_tpc.TylephiPvi = 0;
                        tt_tpc.DsDkbs = "";
                    }
                    // lấy email, tên của PTGD
                    // lấy email 
                    var email_ptgd = (from a in _context.DmUqHstpcs
                                      join b in _context.DmUsers on a.MaUserUq equals b.Oid.ToString()
                                      where a.GhSotienUq >= sum_trachnhiempvi && a.LoaiUyquyen == "6" && b.LoaiUser == 16
                                      select b).FirstOrDefault();
                    if (email_ptgd != null)
                    {
                        tt_tpc.MailPTGD = email_ptgd.Mail ?? "";
                        tt_tpc.TenPTGD = email_ptgd.TenUser ?? "";
                    }
                    else
                    {
                        tt_tpc.MailPTGD = "";
                        tt_tpc.TenPTGD = "";
                    }

                }
                return tt_tpc;

            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                return null;
            }
        }
        public void SendEmail_ToTrinh_TPC(decimal pr_key_hsgd_ctu, string email_login, int loai_tt)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            AlternateView avHtml = null;
            string url_hsgd = _configuration["DownloadSettings:url_hsgd"] ?? "";
            var email_totrinh_tpc = _configuration["Word2PdfSettings:email_totrinh_tpc"] ?? "";
            string htmlBody = File.ReadAllText(email_totrinh_tpc);
            //string htmlBody = "";
            var hsgd_ctu = GetThongTinToTrinhTPC(pr_key_hsgd_ctu, email_login, loai_tt);
            if (hsgd_ctu != null)
            {

                htmlBody = htmlBody.Replace("[DON_VI]", hsgd_ctu.Donvi);
                htmlBody = htmlBody.Replace("[TEN_KHACH]", hsgd_ctu.TenKhach);
                htmlBody = htmlBody.Replace("[BIEN_KSOAT]", hsgd_ctu.BienKsoat);
                htmlBody = htmlBody.Replace("[LOAI_XE]", hsgd_ctu.TenLoaiXe);
                htmlBody = htmlBody.Replace("[NAM_SX]", hsgd_ctu.NamSx.ToString());
                htmlBody = htmlBody.Replace("[SO_SERI]", hsgd_ctu.SoSeri.ToString());
                htmlBody = htmlBody.Replace("[NGAY_DAU_SERI]", hsgd_ctu.NgayDauSeri);
                htmlBody = htmlBody.Replace("[NGAY_CUOI_SERI]", hsgd_ctu.NgayCuoiSeri);
                htmlBody = htmlBody.Replace("[TEN_DONVI]", hsgd_ctu.TenDonvi);
                htmlBody = htmlBody.Replace("[SOTIEN_THUCTE]", hsgd_ctu.SoTienThucTe.ToString("#,###", cul.NumberFormat));
                htmlBody = htmlBody.Replace("[DKBS]", hsgd_ctu.DsDkbs);
                htmlBody = htmlBody.Replace("[NGAY_THUPHI]", hsgd_ctu.NgayThuPhi);
                htmlBody = htmlBody.Replace("[GIOTTHAT]", hsgd_ctu.GioTthat);
                htmlBody = htmlBody.Replace("[NGAY_TTHAT]", hsgd_ctu.NgayTthat);
                htmlBody = htmlBody.Replace("[DIA_DIEMTT]", hsgd_ctu.DiaDiemtt);
                htmlBody = htmlBody.Replace("[NGUYEN_NHANTT]", hsgd_ctu.NguyenNhanTtat);
                htmlBody = htmlBody.Replace("[TEN_GARA]", hsgd_ctu.TenGara);
                htmlBody = htmlBody.Replace("[TONGCHIPHI]", hsgd_ctu.TongChiPhi.ToString("#,###", cul.NumberFormat));
                htmlBody = htmlBody.Replace("[SOTIEN_GIAM]", hsgd_ctu.SotienGiam.ToString("#,###", cul.NumberFormat));
                htmlBody = htmlBody.Replace("[TRACHNHIEMPVI]", hsgd_ctu.TrachNhiemPVI.ToString("#,###", cul.NumberFormat));
                htmlBody = htmlBody.Replace("[TRACHNHIEMPVI_BC]", ContentHelper.NumberToText((double)hsgd_ctu.TrachNhiemPVI));
                htmlBody = htmlBody.Replace("[TENSP]", hsgd_ctu.TenSP);
                htmlBody = htmlBody.Replace("[TRACHNHIEMPVI]", hsgd_ctu.TrachNhiemPVI.ToString("#,###", cul.NumberFormat));
                htmlBody = htmlBody.Replace("[TRACHNHIEMPVI]", hsgd_ctu.TrachNhiemPVI.ToString("#,###", cul.NumberFormat));
                // lấy email tky
                var user_tky = _context.DmUsers.Where(x => x.LoaiUser == 15).FirstOrDefault();
                var email_tky = "";
                var ten_tky = "";
                if (user_tky != null)
                {
                    email_tky = user_tky.Mail ?? "";
                    ten_tky = user_tky.TenUser ?? "";
                }

                htmlBody = htmlBody.Replace("[TEN_THUKY]", ten_tky);
                htmlBody = htmlBody.Replace("[URL_HSGD]", url_hsgd + "ho_so_trinh_ky_lanh_dao/" + pr_key_hsgd_ctu);
                //_logger.Information("SendEmail_ToTrinh_TPC  pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " htmlBody = " + htmlBody);
                avHtml = AlternateView.CreateAlternateViewFromString(htmlBody, null/* TODO Change to default(_) if this is not a reference type */, MediaTypeNames.Text.Html);

                SendEmail_TPC(email_login, hsgd_ctu.MailPTGD, email_tky, hsgd_ctu.Donvi + " trình Hồ sơ XCG trên phân cấp " + hsgd_ctu.BienKsoat, hsgd_ctu.PathTotrinhTpc, htmlBody, avHtml, pr_key_hsgd_ctu, hsgd_ctu.BienKsoat);
            }


        }
        public void SendEmail_TPC(string email_login, string email_ptgd, string email_tky, string sSubject, string strFileNamePdf, string htmlBody, AlternateView avHtml, decimal pr_key_hsgd_ctu, string BienKsoat)
        {
            try
            {
                MailAddress from = new MailAddress("baohiempvi@pvi.com.vn", "BAOHIEMPVI", System.Text.Encoding.UTF8);
                MailAddress to = new MailAddress(email_ptgd);
                System.Net.Mail.MailMessage Mail = new System.Net.Mail.MailMessage(from, to);
                Mail.CC.Add(email_tky);
                //Mail.Bcc.Add(email_ptgd);
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
                string url_download = _configuration["DownloadSettings:DownloadServer_MDF1"] ?? "";
                string strFileNameLocalPdf = UtilityHelper.getPathAndCopyTempServer(strFileNamePdf, url_download, "Tờ trình.pdf");
                System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(strFileNameLocalPdf);
                Mail.Attachments.Add(attachment);
                //đính kèm pasc
                var hsgd_dx_ct = ToListWithNoLock((from a in _context.HsgdDxCts
                                                   join b in _context.HsgdCtus on a.PrKeyHsbtCtu equals b.PrKeyBt
                                                   where b.PrKey == pr_key_hsgd_ctu
                                                   select new
                                                   {
                                                       PrKey = a.PrKey,
                                                       PrKeyHsbtCt = a.PrKeyHsbtCt,
                                                       LoaiDx = new[] { "050101", "050104" }.Contains(a.MaSp) ? 0 : 1
                                                   }).AsQueryable());
                for (int i = 0; i < hsgd_dx_ct.Count; i++)
                {
                    var result = _dx_repo.PrintPASC(hsgd_dx_ct[i].PrKeyHsbtCt, pr_key_hsgd_ctu, email_login, hsgd_dx_ct[i].LoaiDx);

                    DownloadSettings downloadSettings = new DownloadSettings();
                    //var downloadSettings =(DownloadSettings) _configuration.GetSection("DownloadSettings");
                    //var word2PdfSettings = (Word2PdfSettings) _configuration.GetSection("Word2PdfSettings");
                    Word2PdfSettings word2PdfSettings = new Word2PdfSettings();
                    word2PdfSettings.CurrentPathWordPASC = _configuration["Word2PdfSettings:CurrentPathWordPASC"] ?? "";
                    word2PdfSettings.CurrentPathWordPASC_TSK = _configuration["Word2PdfSettings:CurrentPathWordPASC_TSK"] ?? "";
                    word2PdfSettings.FilePath = _configuration["Word2PdfSettings:FilePath"] ?? "";
                    word2PdfSettings.PathPdf = _configuration["Word2PdfSettings:PathPdf"] ?? "";
                    var optionsDownloadSettings = Microsoft.Extensions.Options.Options.Create(downloadSettings);
                    var optionsWord2PdfSettings = Microsoft.Extensions.Options.Options.Create(word2PdfSettings);


                    var contentHelper = new ContentHelper(optionsDownloadSettings, optionsWord2PdfSettings, _logger);

                    DownloadFileResult result1 = null;

                    if (result != null && result.ThirdQueryResults != null)
                    {
                        result1 = contentHelper.ConvertFileWordToPdf_PASC(result.ThirdQueryResults, result.ListPascDetail, hsgd_dx_ct[i].LoaiDx);
                    }
                    string folderUpload = _configuration["UploadSettings:FolderUpload"] ?? "";
                    string url_upload = _configuration["DownloadSettings:UlpoadServer"] ?? "";
                    var utilityHelper = new UtilityHelper(_logger);
                    var file_path = utilityHelper.UploadFile_ToAPI(result1.Data, ".pdf", folderUpload, url_upload, false);
                    string url_download_dx = _configuration["DownloadSettings:DownloadServer"] ?? "";
                    string strFileNameLocalPdf_dx = UtilityHelper.getPathAndCopyTempServer(file_path, url_download_dx, "Đề xuất " + BienKsoat + " " + (i + 1) + ".pdf");
                    System.Net.Mail.Attachment attachment_dx = new System.Net.Mail.Attachment(strFileNameLocalPdf_dx);
                    Mail.Attachments.Add(attachment_dx);
                }

                SmtpClient SmtpServer = new SmtpClient();
                SmtpServer.Port = 25;
                SmtpServer.Host = "mailapp.pvi.com.vn";
                SmtpServer.EnableSsl = false;
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.Timeout = 15000;
                SmtpServer.Send(Mail);
                Mail.Dispose();
                SmtpServer.Dispose();
                _logger.Information("SendEmail_TPC to " + email_login + ", pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " success");
            }
            catch (Exception ex)
            {
                _logger.Error("Lỗi SendEmail_TPC to " + email_login + ", pr_key_hsgd_ctu = " + pr_key_hsgd_ctu + " error: " + ex.Message.ToString());
            }
        }
        public List<HsgdTtrinhTt> GetHsgdTtrinhTt(decimal pr_key_tt)
        {
            var result = _context.HsgdTtrinhTt.Where(x => x.FrKey == pr_key_tt).ToList();
            return result;

        }
        public HsgdCtu GetHsgdCtuByKey(decimal pr_key)
        {

            HsgdCtu? objResult = new HsgdCtu();
            try
            {
                objResult = _context.HsgdCtus.Where(x => x.PrKey == pr_key).FirstOrDefault();
            }
            catch (Exception ex)
            {
            }
            return objResult;
        }
        public void SendEmail_XOADNTT_GDV( List<HsgdDntt> dntt_delete)
        {
            try
            {
                 
                
                using (var _context_gdtt_new = new GdttContext())
                {
                    foreach (var dntt in dntt_delete)
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
                                    where b.PrKey == dntt.PrKeyTtrinh
                                       && c.Act == "CREATETOTRINH"
                                    orderby c.NgayCnhat descending
                                    select new
                                    {
                                        a.SoHsbt,
                                        a.SoHsgd,
                                        d.Mail
                                    }
                                ).AsNoTracking().FirstOrDefault();
                       
                        if (result != null)
                        {
                            var canbotaodntt = _context_gdtt_new.DmUsers.AsNoTracking().Where(x => x.Mail.Replace("@pvi.com.vn","") == dntt.MaCbo).FirstOrDefault();
                            var canboxuly = _context_gdtt_new.DmUserTtoans.AsNoTracking().Where(x => x.MaUser == dntt.MaCbcnvXly).FirstOrDefault();                            
                                MailAddress from = new MailAddress("baohiempvi@pvi.com.vn", "PVI.247", System.Text.Encoding.UTF8);
                                MailAddress to = new MailAddress(result.Mail);
                                System.Net.Mail.MailMessage Mail = new System.Net.Mail.MailMessage(from, to);
                                Mail.Subject = "PVI247 - Thông báo Đề nghị thanh toán của hồ sơ bồi thường:" + result.SoHsbt + ", Số HS giám định: " + result.SoHsgd + " đã Xóa bên Đơn vị.";
                                Mail.SubjectEncoding = System.Text.Encoding.UTF8;
                                string htmlBody = "PVI247 Thông báo!<br/>";
                                 htmlBody = htmlBody + "Đề nghị thanh toán của hồ sơ bồi thường:" + result.SoHsbt + ", Số HS giám định: " + result.SoHsgd + " đã Xóa trên phần mềm thanh toán tại đơn vị, vui lòng liên hệ lại với Cán bộ xử lý bồi thường đơn vị để biết thông tin cần bổ sung.<br/>";
                               if(canbotaodntt!=null)
                                {
                                htmlBody = htmlBody + "Đơn vị: " + canbotaodntt.TenDonvi + "<br/>";
                                htmlBody = htmlBody + "Tên cán bộ tạo đề nghị thanh toán: " + canbotaodntt.TenUser + "<br/>";
                                htmlBody = htmlBody + "Email: " + canbotaodntt.Mail + "<br/>";
                                htmlBody = htmlBody + "Điện thoại: " + canbotaodntt.Dienthoai + "<br/>";
                                    //Mail.CC.Add(canbotaodntt.Mail);
                                }
                                if (canboxuly != null)
                                {
                                htmlBody = htmlBody + "------------------------------------------------<br/>";
                                htmlBody = htmlBody + "Tên cán bộ xử lý: " + canboxuly.FullName + "<br/>";
                                htmlBody = htmlBody + "Email: " + canboxuly.DcEmail + "<br/>";                                    
                                }
                                if (htmlBody != "")
                                    Mail.Body = htmlBody;
                                Mail.BodyEncoding = System.Text.Encoding.UTF8;
                                Mail.IsBodyHtml = true;
                                SmtpClient SmtpServer = new SmtpClient();
                                SmtpServer.Port = 25;
                                SmtpServer.Host = "mailapp.pvi.com.vn";
                                SmtpServer.Timeout = 10000;
                                SmtpServer.EnableSsl = false;
                                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                                SmtpServer.Credentials = new NetworkCredential("baohiempvi", "bhpvi!@#");
                                SmtpServer.Send(Mail);
                                Mail.Dispose();                            
                        }

                    }
                }    
               
                                
            }
            catch (Exception ex)
            {
                _logger.Error(DateTime.Now.ToString() + "Lỗi SendEmail_XOADNTT_GDV  Error: " + ex.Message.ToString());                

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
                                Mail.From=from;
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
                                htmlBody = htmlBody+ "Hồ sơ bồi thường:" + result.SoHsbt + ", Số HS giám định: " + result.SoHsgd + " đã được giám định viên hoàn thiện hồ sơ, Vui lòng tạo đề nghị thanh toán cho hồ sở trên.<br/>";
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
                                SmtpServer.EnableSsl = false;
                                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                                SmtpServer.Credentials = new NetworkCredential("baohiempvi", "bhpvi!@#");
                                SmtpServer.Send(Mail);
                                Mail.Dispose();
                            }    

                           
                        }
                }


            }
            catch (Exception ex)
            {
                _logger.Error(DateTime.Now.ToString() + "Lỗi SendEmail_QLNV_GDV_HOANTHIENHSTT  Error: " + ex.Message.ToString());

            }
        }

        public string KyHoSoTPC(decimal pr_key_hsgd_ctu, string email_login)
        {
            string result = "";
            try
            {
                var user_login = _context.DmUsers.Where(x => x.Mail == email_login).FirstOrDefault();
                var hsgd_ctu = _context.HsgdCtus.Where(x => x.PrKey == pr_key_hsgd_ctu).FirstOrDefault();
                if (hsgd_ctu != null && !string.IsNullOrEmpty(hsgd_ctu.PathTotrinhTpc))
                {
                    //ký số hồ sơ
                    // soap pias
                    var ws = new ServiceReference1.PiasSoapSoapClient(ServiceReference1.PiasSoapSoapClient.EndpointConfiguration.PiasSoapSoap, _configuration["client:endpoint:address"]);

                    string strSQL = "select top 1 * from hddt_hsm where ma_donvi = '" + user_login.MaDonvi + "' and ngay_hluc < getdate() order by ngay_hluc desc ";
                    var esign = ws.SelectSQL_HDDT(DateTime.Now.Year.ToString(), strSQL, "hddt_hsm");

                    var ds_esign = ConvetXMLToDataset(esign);
                    if (ds_esign != null && ds_esign.Tables.Count > 0 && ds_esign.Tables[0].Rows.Count > 0)
                    {
                        _logger.Information("KyHoSoTPC pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " select hddt_hsm thành công");
                        var partitionAlias = ds_esign.Tables[0].Rows[0].Field<string>("partition_alias");
                        var privateKeyAlias = ds_esign.Tables[0].Rows[0].Field<string>("private_key_alias");
                        var password = ds_esign.Tables[0].Rows[0].Field<string>("password");
                        var partitionSerial = ds_esign.Tables[0].Rows[0].Field<string>("partition_serial");
                        if (ws.KyToTrinhXCG(hsgd_ctu.PathTotrinhTpc, privateKeyAlias))
                        {
                            _logger.Information("KyHoSoTPC pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " SignPDF_HILO thành công");
                            // thực hiện replace text
                            string url_download = _configuration["DownloadSettings:DownloadServer_MDF1"] ?? "";
                            //tải file
                            string Path_orgin = UtilityHelper.getPathAndCopyTempServer(hsgd_ctu.PathTotrinhTpc, url_download);
                            string Path_result = Path_orgin + "_edited.pdf";
                            if (System.IO.File.Exists(Path_orgin))
                            {
                                _logger.Information("KyHoSoTPC pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " tải file thành công");
                                var pdfEdit = new PDFEdit(_logger);

                                if (pdfEdit.ReplaceTextInPDF(Path_orgin, Path_result, pdfEdit.ListKeyWord("GD_TAT", user_login.MaUser ?? "", user_login.TenUser ?? ""), true))
                                {
                                    _logger.Information("KyHoSoTPC pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " ReplaceTextInPDF GD_TAT thành công");
                                }
                                else
                                {
                                    _logger.Information("KyHoSoTPC pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " ReplaceTextInPDF GD_TAT thất bại");
                                    return result;
                                }
                                var utilityHelper = new UtilityHelper(_logger);
                                string folderUpload = _configuration["UploadSettings:FolderUpload_MDF1"] ?? "";
                                string url_upload = _configuration["DownloadSettings:UlpoadServer_MDF1"] ?? "";
                                var file_path = utilityHelper.UploadFileOld_ToAPI(Convert.ToBase64String(System.IO.File.ReadAllBytes(Path_orgin)), hsgd_ctu.PathTotrinhTpc, folderUpload, url_upload);
                                // kiểm tra và xóa file ở local 
                                try
                                {
                                    if (System.IO.File.Exists(Path_orgin))
                                    {
                                        System.IO.File.Delete(Path_orgin);
                                    }
                                    if (System.IO.File.Exists(Path_result))
                                    {
                                        System.IO.File.Delete(Path_result);
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                                if (file_path != "")
                                {
                                    _logger.Information("KyHoSoTPC pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " upload file tờ trình sau ReplaceTextInPDF thành công");


                                    result = "Ký hồ sơ TPC thành công";
                                    _logger.Information("KyHoSoTPC pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " thành công");
                                }
                                else
                                {
                                    _logger.Information("KyHoSoTPC pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " upload file tờ trình sau ReplaceTextInPDF thất bại");
                                }


                            }
                            else
                            {
                                result = "Lỗi không tải được file tờ trình TPC";
                                _logger.Information("KyHoSoTPC pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " lỗi không tải được file tờ trình TPC");
                            }

                        }
                        else
                        {
                            result = "Ký số HILO thất bại";
                            _logger.Information("KyHoSoTPC pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " SignPDF_HILO thất bại");
                        }

                    }
                    else
                    {
                        result = "Chưa cấu hình quyền ký số hoặc ký số hết hiệu lực";
                        _logger.Information("KyHoSoTPC pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " hddt_hsm không tồn tại");
                    }
                }
                else
                {
                    result = "Chưa tạo tờ trình tpc.Vui lòng kiểm tra lại.";
                }

            }
            catch (Exception ex)
            {
                _logger.Information("KyHoSoTPC pr_key_hsgd_ctu =" + pr_key_hsgd_ctu + " error: " + ex);
            }
            return result;
        }
        public HsgdTtrinh GetHsgdTtrinhByIdAsync(decimal prKey)
        {
            return _context.HsgdTtrinhs
                .FirstOrDefault(x => x.PrKey == prKey);
        }

    }
}