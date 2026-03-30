using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Office.Interop.Word;
using PVI.DAO.Entities.Models;
using PVI.Helper;
using PVI.Repository.Interfaces;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PVI.Repository.Repositories
{
    public class KbttCtuRepository : GenericRepository<KbttCtu>, IKbttCtuRepository
    {
        HsgdCtuHelper PheDuyetHelper;
        public KbttCtuRepository(GdttContext context, Pvs2024Context context_pias, Pvs2024UpdateContext context_pias_update, MY_PVIContext context_my_pvi, Serilog.ILogger logger, IConfiguration conf) : base(context, context_pias, context_pias_update, context_my_pvi, logger, conf)
        {

            PheDuyetHelper = new HsgdCtuHelper(context, context_pias, context_pias_update, logger, conf);
        }

        public async Task<PagedList<KbttCtuDto>> GetListPVIMobile(string email, KbttCtuParameters parameters, string MaDonbh)
        {
            try
            {
                // Fetch the user's MaDonVi
                var userInfo = await _context.DmUsers.Where(x => x.Mail == email).Select(x => new
                {
                    MaDonvi = x.MaDonvi,
                    TenUser = x.TenUser
                }
                    ).FirstOrDefaultAsync();
                var maDonVi = userInfo.MaDonvi;
                var tenUser = userInfo.TenUser;
                var maDvChuQuan = new List<string>();
                var listLoaiHinhBh = new List<string>();
                var listUser = new List<string?>();
                if (MaDonbh == "0501")
                {
                    listLoaiHinhBh = new List<string>(new[] { "050101", "050102", "050103", "050104", "050105" });
                }
                else
                {
                    listLoaiHinhBh = new List<string>(new[] { "050201", "050202", "050203", "050204", "050205", "LPX" });
                }
                if (maDonVi == "00")
                {

                    maDvChuQuan = await _context.DmDonvis.Select(x => x.MaDonvi).ToListAsync();

                }
                else if (maDonVi == "31" || maDonVi == "32")
                {
                    maDvChuQuan = await _context.DmDonvis.Where(x => x.MaDvchuquan == maDonVi).Select(x => x.MaDonvi).ToListAsync();

                }
                else
                {
                    maDvChuQuan.Add(maDonVi);
                }
                maDvChuQuan.Add(maDonVi);
                listUser = await _context.DmUsers.Where(x => x.MaDonvi == maDonVi).Select(x => x.MaUser).ToListAsync();

                // Main query
                var query = _context_my_pvi.KbttCtus
                    .Where(a => a.SoDonbh != "");
                //&& (maDvChuQuan.Contains(a.MaDonvi) || listUser.Contains(a.MaUser)) || a.TenUser == tenUser)
                //}
                if (MaDonbh == "0501")
                {
                    query = query.Where(x => x.LoaihinhBh == "VCX" || x.LoaihinhBh.StartsWith("TNDS") || listLoaiHinhBh.Contains(x.LoaihinhBh));
                }
                else
                {
                    query = query.Where(x => listLoaiHinhBh.Contains(x.LoaihinhBh));
                }
                if (parameters.TrangThaiSearch != null && parameters.TrangThaiSearch.Count > 0)
                {
                    query = query.Where(x => parameters.TrangThaiSearch.Contains(x.TinhTrang));
                }
                if (!string.IsNullOrEmpty(parameters.SoSeriSearch))
                {
                    query = query.Where(x => x.SoSeri.ToString().Contains(parameters.SoSeriSearch));
                }
                if (!string.IsNullOrEmpty(parameters.BienKiemSoatSearch))
                {
                    query = query.Where(x => x.BienKsoat.Contains(parameters.BienKiemSoatSearch));
                }
                //if (!string.IsNullOrEmpty(parameters.SoKhungSearch))
                //{
                //    query.Where(x => x..ToString().Contains(parameters.BienKiemSoatSearch));
                //}
                if (!string.IsNullOrEmpty(parameters.NgayCuoiSearch))
                {
                    if (DateTime.TryParseExact(parameters.NgayCuoiSearch, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var ngayCuoiDate))
                    {
                        query = query.Where(x => x.NgayCuoiSeri.HasValue && x.NgayCuoiSeri.Value.Date == ngayCuoiDate.Date);
                    }
                }

                if (!string.IsNullOrEmpty(parameters.NgayKbSearch))
                {
                    if (DateTime.TryParseExact(parameters.NgayKbSearch, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var ngayKbDate))
                    {
                        query = query.Where(x => x.NgayKbtt.HasValue && x.NgayKbtt.Value.Date == ngayKbDate.Date);
                    }
                }
                if (!string.IsNullOrEmpty(parameters.TenDonViSearch))
                {

                    query = query.Where(x => x.TenDonvi.Contains(parameters.TenDonViSearch));
                }
                if (!string.IsNullOrEmpty(parameters.SoDonBhSearch))
                {

                    query = query.Where(x => x.SoDonbh.Contains(parameters.SoDonBhSearch));
                }

                if (!string.IsNullOrEmpty(parameters.NgayDauSearch))
                {
                    if (DateTime.TryParseExact(parameters.NgayDauSearch, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var ngayDauDate))
                    {
                        query = query.Where(x => x.NgayDauSeri.HasValue && x.NgayDauSeri.Value.Date == ngayDauDate.Date);
                    }

                }
                if (!string.IsNullOrEmpty(parameters.NgayGDinhSearch))
                {
                    if (DateTime.TryParseExact(parameters.NgayGDinhSearch, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var ngayGdinhDate))
                    {
                        query = query.Where(x => x.NgayGdinh.HasValue && x.NgayGdinh.Value.Date == ngayGdinhDate.Date);
                    }
                }
                if (parameters.LoaiKbttSearch != 0)
                {
                    query = query.Where(x => x.LoaiKbtt == parameters.LoaiKbttSearch);
                }
                if (!string.IsNullOrEmpty(parameters.NguyenNhanSearch))
                {

                    query = query.Where(x => x.NguyenNhanTtat == parameters.NguyenNhanSearch);
                }


                // Fetch data and apply final transformation
                var result = query
                    .Select(a => new KbttCtuDto
                    {
                        PrKey = a.PrKey,
                        LoaihinhBh = a.LoaihinhBh,
                        NguoiKb = _context_my_pvi.DmUsers
                            .Where(u => u.MaUser == a.UserId)
                            .Select(u => !string.IsNullOrEmpty(u.TenUser) ? u.TenUser : u.MaUser)
                            .FirstOrDefault(),
                        NgayKb = a.NgayKbtt.HasValue ? a.NgayKbtt.Value.ToString("dd/MM/yyyy") : null,
                        MaDonvi = a.MaDonvi,
                        TenDonvi = a.TenDonvi,
                        TenKhach = a.TenKhach,
                        SoDonbh = a.SoDonbh,
                        SoSeri = a.SoSeri,
                        BienKsoat = a.BienKsoat,
                        NgayDauSeri = a.NgayDauSeri.HasValue ? a.NgayDauSeri.Value.ToString("dd/MM/yyyy") : null,
                        NgayCuoiSeri = a.NgayCuoiSeri.HasValue ? a.NgayCuoiSeri.Value.ToString("dd/MM/yyyy") : null,
                        NguyenNhanTtat = a.NguyenNhanTtat,
                        NgayGdinh = a.NgayGdinh.HasValue ? a.NgayGdinh.Value.ToString("dd/MM/yyyy") : null,
                        LoaiKbtt = a.LoaiKbtt == 1 ? "Khách hàng GĐ" : "Khách hàng KBTT",
                        PrKeySeri = a.PrKeySeri,
                        MaDonviChuyen = a.MaDonviChuyen,
                        PrKeyBt = a.PrKeyBt,
                        IsdonviDuyet = a.IsdonviDuyet,
                        TrangThai = a.TinhTrang,
                        SoHsgd = a.SoHsgd,
                        NguoiKtao = a.TenUser + "(" + a.MaUser + ")"
                    }).OrderByDescending(x => x.PrKey).AsQueryable();

                if (!string.IsNullOrEmpty(parameters.NguoiKTaoSearch))
                {

                    result = result.Where(x => x.NguoiKtao.Contains(parameters.NguoiKTaoSearch));
                }
                if (!string.IsNullOrEmpty(parameters.NguoiKbSearch))
                {

                    result = result.Where(x => x.NguoiKb.Contains(parameters.NguoiKbSearch));
                }
                // Wrap result in PagedList
                return await PagedList<KbttCtuDto>.ToPagedListAsync(result, parameters.pageNumber, parameters.pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<KbttCtuDto>> GetListPVIMobileExcel(string email, KbttCtuParameters parameters, string MaDonbh)
        {
            try
            {
                // Fetch the user's MaDonVi
                var userInfo = await _context.DmUsers.Where(x => x.Mail == email).Select(x => new
                {
                    MaDonvi = x.MaDonvi,
                    TenUser = x.TenUser
                }
                    ).FirstOrDefaultAsync();
                var maDonVi = userInfo.MaDonvi;
                var tenUser = userInfo.TenUser;
                var maDvChuQuan = new List<string>();
                var listLoaiHinhBh = new List<string>();
                var listUser = new List<string?>();
                if (MaDonbh == "0501")
                {
                    listLoaiHinhBh = new List<string>(new[] { "050101", "050102", "050103", "050104", "050105" });
                }
                else
                {
                    listLoaiHinhBh = new List<string>(new[] { "050201", "050202", "050203", "050204", "050205", "LPX" });
                }
                if (maDonVi == "00")
                {

                    maDvChuQuan = await _context.DmDonvis.Select(x => x.MaDonvi).ToListAsync();

                }
                else if (maDonVi == "31" || maDonVi == "32")
                {
                    maDvChuQuan = await _context.DmDonvis.Where(x => x.MaDvchuquan == maDonVi).Select(x => x.MaDonvi).ToListAsync();

                }
                else
                {
                    maDvChuQuan.Add(maDonVi);
                }
                maDvChuQuan.Add(maDonVi);
                listUser = await _context.DmUsers.Where(x => x.MaDonvi == maDonVi).Select(x => x.MaUser).ToListAsync();

                // Main query
                var query = _context_my_pvi.KbttCtus
                    .Where(a => a.SoDonbh != "");
                //&& (maDvChuQuan.Contains(a.MaDonvi) || listUser.Contains(a.MaUser)) || a.TenUser == tenUser)
                //}
                if (MaDonbh == "0501")
                {
                    query = query.Where(x => x.LoaihinhBh == "VCX" || x.LoaihinhBh.StartsWith("TNDS") || listLoaiHinhBh.Contains(x.LoaihinhBh));
                }
                else
                {
                    query = query.Where(x => listLoaiHinhBh.Contains(x.LoaihinhBh));
                }
                if (parameters.TrangThaiSearch != null && parameters.TrangThaiSearch.Count > 0)
                {
                    query = query.Where(x => parameters.TrangThaiSearch.Contains(x.TinhTrang));
                }
                if (!string.IsNullOrEmpty(parameters.SoSeriSearch))
                {
                    query = query.Where(x => x.SoSeri.ToString().Contains(parameters.SoSeriSearch));
                }
                if (!string.IsNullOrEmpty(parameters.BienKiemSoatSearch))
                {
                    query = query.Where(x => x.BienKsoat.Contains(parameters.BienKiemSoatSearch));
                }
                //if (!string.IsNullOrEmpty(parameters.SoKhungSearch))
                //{
                //    query.Where(x => x..ToString().Contains(parameters.BienKiemSoatSearch));
                //}
                if (!string.IsNullOrEmpty(parameters.NgayCuoiSearch))
                {
                    if (DateTime.TryParseExact(parameters.NgayCuoiSearch, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var ngayCuoiDate))
                    {
                        query = query.Where(x => x.NgayCuoiSeri.HasValue && x.NgayCuoiSeri.Value.Date == ngayCuoiDate.Date);
                    }
                }

                if (!string.IsNullOrEmpty(parameters.NgayKbSearch))
                {
                    if (DateTime.TryParseExact(parameters.NgayKbSearch, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var ngayKbDate))
                    {
                        query = query.Where(x => x.NgayKbtt.HasValue && x.NgayKbtt.Value.Date == ngayKbDate.Date);
                    }
                }
                if (!string.IsNullOrEmpty(parameters.TenDonViSearch))
                {

                    query = query.Where(x => x.TenDonvi.Contains(parameters.TenDonViSearch));
                }
                if (!string.IsNullOrEmpty(parameters.SoDonBhSearch))
                {

                    query = query.Where(x => x.SoDonbh.Contains(parameters.SoDonBhSearch));
                }

                if (!string.IsNullOrEmpty(parameters.NgayDauSearch))
                {
                    if (DateTime.TryParseExact(parameters.NgayDauSearch, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var ngayDauDate))
                    {
                        query = query.Where(x => x.NgayDauSeri.HasValue && x.NgayDauSeri.Value.Date == ngayDauDate.Date);
                    }

                }
                if (!string.IsNullOrEmpty(parameters.NgayGDinhSearch))
                {
                    if (DateTime.TryParseExact(parameters.NgayGDinhSearch, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var ngayGdinhDate))
                    {
                        query = query.Where(x => x.NgayGdinh.HasValue && x.NgayGdinh.Value.Date == ngayGdinhDate.Date);
                    }
                }
                if (parameters.LoaiKbttSearch != 0)
                {
                    query = query.Where(x => x.LoaiKbtt == parameters.LoaiKbttSearch);
                }
                if (!string.IsNullOrEmpty(parameters.NguyenNhanSearch))
                {

                    query = query.Where(x => x.NguyenNhanTtat == parameters.NguyenNhanSearch);
                }


                // Fetch data and apply final transformation
                var result = query
                    .Select(a => new KbttCtuDto
                    {
                        PrKey = a.PrKey,
                        LoaihinhBh = a.LoaihinhBh,
                        NguoiKb = _context_my_pvi.DmUsers
                            .Where(u => u.MaUser == a.UserId)
                            .Select(u => !string.IsNullOrEmpty(u.TenUser) ? u.TenUser : u.MaUser)
                            .FirstOrDefault(),
                        NgayKb = a.NgayKbtt.HasValue ? a.NgayKbtt.Value.ToString("dd/MM/yyyy") : null,
                        MaDonvi = a.MaDonvi,
                        TenDonvi = a.TenDonvi,
                        TenKhach = a.TenKhach,
                        SoDonbh = a.SoDonbh,
                        SoSeri = a.SoSeri,
                        BienKsoat = a.BienKsoat,
                        NgayDauSeri = a.NgayDauSeri.HasValue ? a.NgayDauSeri.Value.ToString("dd/MM/yyyy") : null,
                        NgayCuoiSeri = a.NgayCuoiSeri.HasValue ? a.NgayCuoiSeri.Value.ToString("dd/MM/yyyy") : null,
                        NguyenNhanTtat = a.NguyenNhanTtat,
                        NgayGdinh = a.NgayGdinh.HasValue ? a.NgayGdinh.Value.ToString("dd/MM/yyyy") : null,
                        LoaiKbtt = a.LoaiKbtt == 1 ? "Khách hàng GĐ" : "Khách hàng KBTT",
                        PrKeySeri = a.PrKeySeri,
                        MaDonviChuyen = a.MaDonviChuyen,
                        PrKeyBt = a.PrKeyBt,
                        IsdonviDuyet = a.IsdonviDuyet,
                        TrangThai = a.TinhTrang,
                        SoHsgd = a.SoHsgd,
                        NguoiKtao = a.TenUser + "(" + a.MaUser + ")"
                    }).OrderByDescending(x => x.PrKey).AsQueryable();

                if (!string.IsNullOrEmpty(parameters.NguoiKTaoSearch))
                {

                    result = result.Where(x => x.NguoiKtao.Contains(parameters.NguoiKTaoSearch));
                }
                if (!string.IsNullOrEmpty(parameters.NguoiKbSearch))
                {

                    result = result.Where(x => x.NguoiKb.Contains(parameters.NguoiKbSearch));
                }
                // Wrap result in PagedList
                return await result.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<dynamic?> GetDetailKbttCtu(decimal prKey)
        {
            var result = await _context_my_pvi.KbttCtus
           .Where(x => x.PrKey == prKey)
           .Select(x => new
           {
               PrKey = x.PrKey,
               LoaiKbtt = x.LoaiKbtt,
               MaDonvi = x.MaDonvi,
               TenDonvi = x.TenDonvi,
               SoDonbh = x.SoDonbh,
               NgayDauSeri = x.NgayDauSeri.HasValue ? x.NgayDauSeri.Value.ToString("dd/MM/yyyy") : null,
               NgayCuoiSeri = x.NgayCuoiSeri.HasValue ? x.NgayCuoiSeri.Value.ToString("dd/MM/yyyy") : null,
               TenKhach = x.NgGdichTh,
               SoSeri = x.SoSeri,
               BienKsoat = x.BienKsoat,
               NgayKbtt = x.NgayKbtt.HasValue ? x.NgayKbtt.Value.ToString("dd/MM/yyyy") : null,
               NgayGdinh = x.NgayGdinh.HasValue ? x.NgayGdinh.Value.ToString("dd/MM/yyyy") : null,
               NgayTthat = x.NgayTthat.HasValue ? x.NgayTthat.Value.ToString("dd/MM/yyyy") : null,
               ThoigianTthat = x.ThoigianTthat,
               DiaDiemtt = x.DiaDiemtt,
               TinhTrang = x.TinhTrang,
               NguyenNhanTtat = x.NguyenNhanTtat,
               HauquaNguoi = x.HauquaNguoi,
               HauquaTsan = x.HauquaTsan,
               NgayTthatkh = x.NgayTthatkh.HasValue ? x.NgayTthatkh.Value.ToString("dd/MM/yyyy") : null,
               ThoigianTthatkh = x.ThoigianTthatkh,
               DiaDiemttkh = x.DiaDiemttkh,
               NguyenNhanTtatkh = x.NguyenNhanTtatkh,
               HauquaNguoikh = x.HauquaNguoikh,
               HauquaTsankh = x.HauquaTsankh,
               CoquanGquyet = x.CoquanGquyet,
               NguoiLienhe = x.NguoiLienhe,
               DienthoaiLienhe = x.DienthoaiLienhe,
               NgayHengd = x.NgayHengd.HasValue ? x.NgayHengd.Value.ToString("dd/MM/yyyy") : null,
               GaraGiamdinh = x.GaraGiamdinh,
               TenGaraGiamdinh = string.Empty,
               GaraSuachua = x.GaraSuachua,
               TenGaraSuachua = string.Empty,
               SoTienugd = x.SoTienugd,
               PrKeySeri = x.PrKeySeri,
               MaDonviChuyen = x.MaDonviChuyen,
               PrKeyBt = x.PrKeyBt,
               IsdonviDuyet = x.IsdonviDuyet,
               PrKeyGdtt = x.PrKeyGdtt,
               MaUserGdv = x.MaUserGdv,
               GdvHotline = x.TenUser + "(" + x.MaUser + ")",
               MaDieukhoanTnds = x.MaDieukhoanTnds,
               DangKiem = x.DangKiem,
               NgayDauDk = x.NgayDauDk.HasValue ? x.NgayDauDk.Value.ToString("dd/MM/yyyy") : null,
               NgayCuoiDk = x.NgayCuoiDk.HasValue ? x.NgayCuoiDk.Value.ToString("dd/MM/yyyy") : null,
               Gplx = x.Gplx,
               HangXe = x.HangXe,
               LoaiHinhBh = x.LoaihinhBh,
               TenLaiXe = x.TenLaiXe
           })
             .FirstOrDefaultAsync();




            return result;
        }

        public async Task<List<ImageKbttResponse>> GetListAnhKbtt(decimal prKey)
        {
            var result = _context_my_pvi.KbttCtus.FirstOrDefault(x => x.PrKey == prKey);
            var listImage = new List<ImageKbttResponse>();
            if (result != null)
            {
                if (result.LoaiKbtt == 1)
                {
                    listImage = await (from A in _context_my_pvi.KbttCtus
                                       join B in _context_my_pvi.KbttCts on A.PrKey equals B.FrKey
                                       join C in _context_my_pvi.KbttAnhs on B.PrKey equals C.FrKey
                                       where A.PrKey == prKey
                                       select new ImageKbttResponse
                                       {
                                           PrKey = C.PrKey,
                                           LoaiKbtt = "khgd",
                                           ViDo = C.ViDo,
                                           KinhDo = C.KinhDo,
                                           Stt = B.Stt,
                                           MaHmuc = B.MaHmuc,
                                           TenHmuc = B.TenHmuc,
                                           PathUrl = Regex.Replace(C.Url, @"^https?://pvi247\.pvi\.com\.vn", "https://cdn247.pvi.com.vn"),
                                           PathFile = C.Path.Replace("\\", "/"),
                                       }).ToListAsync();
                    if (listImage.Count == 0)
                    {
                        listImage = await (from A in _context_my_pvi.KbttCtus
                                           join B in _context_my_pvi.KbttAnhs on A.PrKey equals B.FrKey
                                           where A.PrKey == prKey && B.MaHmuc != ""
                                           select new ImageKbttResponse
                                           {
                                               PrKey = B.PrKey,
                                               LoaiKbtt = "khkbtt",
                                               ViDo = B.ViDo,
                                               KinhDo = B.KinhDo,
                                               TenHmuc = "",
                                               PathUrl = Regex.Replace(B.Url, @"^https?://pvi247\.pvi\.com\.vn", "https://cdn247.pvi.com.vn"),
                                               PathFile = B.Path.Replace("\\", "/"),
                                           }).ToListAsync();

                    }
                }
                else
                {
                    listImage = await (from A in _context_my_pvi.KbttCtus
                                       join B in _context_my_pvi.KbttAnhs on A.PrKey equals B.FrKey
                                       where A.PrKey == prKey
                                       select new ImageKbttResponse
                                       {
                                           PrKey = B.PrKey,
                                           LoaiKbtt = "khkbtt",
                                           ViDo = B.ViDo,
                                           KinhDo = B.KinhDo,
                                           TenHmuc = "",
                                           PathUrl = Regex.Replace(B.Url, @"^https?://pvi247\.pvi\.com\.vn", "https://cdn247.pvi.com.vn"),
                                           PathFile = B.Path.Replace("\\", "/"),
                                       }).ToListAsync();
                    ;
                }
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


                        if (!uniqueDirectories.Add(dir_source))
                            continue;

                        pathFile = UtilityHelper.CopyFile(dir_source, dir_target);
                    }
                    return listImage;
                }
                else
                {
                    return new List<ImageKbttResponse>();
                }
            }
            else
            {
                return new List<ImageKbttResponse>();
            }


        }
        //public static string ModifyTargetDirectory(string dir_source)
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

        public async Task<PagedList<ListAddNewResponse>> GetListHoSo(ListAddNewParameters parameters, string MaDonbh)
        {

            var query = (from A in _context_pias.NvuBhtCtus
                         join B in _context_pias.NvuBhtSeris on A.PrKey equals B.FrKey
                         join C in _context_pias.DmPbans on A.MaPkt equals C.MaPban
                         join D in _context_pias.DmKhaches on A.MaCbkt equals D.MaKh
                         where A.TrangThai == "02"
                               && A.MaDonbh == MaDonbh
                               && A.MaSdbs == ""
                               && EF.Functions.DateDiffDay(B.NgayCuoiSeri, DateTime.Now) < 0
                         select new ListAddNewResponse
                         {
                             MaCtu = MaDonbh == "0502" ? "GDXM" : "GDVC",
                             NgayCtu = DateTime.Now.ToString("dd/MM/yyyy"),
                             MaDonvi = A.MaDonvi,
                             MaPkt = A.MaPkt,
                             TenPkt = C.TenPban,
                             MaDonbh = A.MaDonbh,
                             SoHsgd = "",
                             SoDonbh = A.SoDonbh,
                             BienKsoat = B.BienKsoat,
                             SoSeri = B.SoSeri,
                             NgGdichTh = A.NgGdichTh,
                             NgayDauSeri = B.NgayDauSeri,
                             NgayCuoiSeri = B.NgayCuoiSeri,
                             NgayTbao = DateTime.Now.ToString("dd/MM/yyyy"),
                             NgayTthat = DateTime.Now.ToString("dd/MM/yyyy"),
                             NguyenNhanTtat = "",
                             DiaDiemtt = "",
                             DiaChiKh = B.DiaChi,
                             MaKh = A.MaKh,
                             TrongTai = B.TrongTai,
                             TenKhach = !string.IsNullOrEmpty(B.TenKhach)
                                ? B.TenKhach.Replace("&", " và ")
                                : A.NgGdich.Replace("&", " và "),
                             MaTte = A.MaTte,
                             TygiaHt = A.TygiaHt,
                             TygiaTt = A.TygiaTt,
                             GhiChu = "",
                             MaGaraVcx = "",
                             TenGara = "",
                             NgayGdinh = DateTime.Now.ToString("dd/MM/yyyy"),
                             MaTtrangGd = "",
                             TenTtrangGd = "",
                             PrKeySeri = B.PrKey,
                             MaKthac = A.MaKthac,
                             MaDaily = A.MaDaily,
                             MaCbkt = A.MaCbkt,
                             TenCbkt = D.TenKh,
                             NhanHieu = B.NhanHieu,
                             MaLoaiXe = B.MaLoaixe,
                             MaDongXe = B.MaDongxe,
                             NamSx = B.NamSx,
                             SoCngoi = B.SoCngoi,
                             NgayCapSeri = B.NgayCapSeri,
                             DienThoaiSeri = B.DienThoai,
                             MaNhloaixe = A.MaNhloaixe,
                             MaLhsbt = "",
                             DienThoai = "",
                             NgLienhe = "",
                             MaDviBtHo = "",
                             TenDviBtHo = "",
                             PrKeyBtHo = "",
                             PrKeyGoc = A.PrKey,
                             TongTien = B.TongTien,
                             SoKhung = B.SoKhung,
                             SoLanBt = _context_pias.HsbtCtus
                                 .Count(h => new[] { "TBT", "NBT" }.Contains(h.MaLhsbt) && h.PrKeySeri == B.PrKey)
                         });


            if (!string.IsNullOrEmpty(parameters.SoSeriSearch))
            {
                query = query.Where(x => x.SoSeri == Convert.ToDecimal(parameters.SoSeriSearch));
            }
            if (!string.IsNullOrEmpty(parameters.BienKiemSoatSearch))
            {
                var processedSearchTerm = parameters.BienKiemSoatSearch
                    .Replace("-", "")
                    .Replace(".", "")
                    .Replace(" ", "")
                    .Replace("_", "")
                    .ToUpper();
                query = query.Where(x => x.BienKsoat.Replace("-", "").Replace(".", "").Replace(" ", "").Replace("_", "").ToUpper().Contains(processedSearchTerm));
            }
            if (!string.IsNullOrEmpty(parameters.SoKhungSearch))
            {
                query = query.Where(x => x.SoKhung.Contains(parameters.SoKhungSearch));
            }
            //if (parameters.NgayCuoiSearch != null)
            //{
            //    var ngayCuoi = ConvertDateTime.ConverDateTimeVN(parameters.NgayCuoiSearch);
            //    query = query.Where(x => x.NgayCuoiSeri == ngayCuoi);
            //}

            //// Limit results and return
            //var results = await query.Take(10).ToListAsync();


            return await PagedList<ListAddNewResponse>.ToPagedListAsync(query, parameters.pageNumber, parameters.pageSize);



        }


        public async Task<string> CreateKbtt(CreateKbttCtuRequest request, string email)
        {
            try
            {
                var userInfo = _context.DmUsers.Where(x => x.Mail == email).FirstOrDefault();
                string tenUser = "";
                string maUser = "";
                //string userId = "";
                if (userInfo != null)
                {
                    tenUser = userInfo.TenUser;
                    maUser = userInfo.MaUser;
                    //userId = userInfo.Dienthoai;
                }

                //maUserGdv = x.TenUser + "(" + x.MaUser + ")"
                //string input = request.MaUserGdv;
                //string tenUser = "";
                //string maUser = "";
                //int startIndex = input.LastIndexOf('(');
                //int endIndex = input.LastIndexOf(')');

                //if (startIndex != -1 && endIndex != -1 && startIndex < endIndex)
                //{
                //    tenUser = input.Substring(0, startIndex).Trim();
                //    maUser = input.Substring(startIndex + 1, endIndex - startIndex - 1);


                //}
                //string maDviGd = "";
                //string tenDviGd = "";
                if (request == null)
                {
                    return "Error when creating Kbtt";
                }
                //if (!string.IsNullOrEmpty(request.MaUserGdv))
                //{
                //    var userInfo1 = await _context.DmUsers.Where(x => x.Oid == Guid.Parse(request.MaUserGdv)).FirstOrDefaultAsync();
                //    if(userInfo1 != null)
                //    {
                //        maDviGd = userInfo1.MaDonvi;
                //        tenDviGd = userInfo1.TenDonvi;
                //    }


                //}


                var newKbttCtu = new KbttCtu
                {
                    TenDonvi = request.TenDonvi,
                    MaDonvi = request.MaDonvi,
                    SoDonbh = request.SoDonbh,
                    NgayDauSeri = ConvertDateTime.ConverDateTimeVN(request.NgayDauSeri),
                    NgayCuoiSeri = ConvertDateTime.ConverDateTimeVN(request.NgayCuoiSeri),
                    TenKhach = request.TenKhach,
                    SoSeri = request.SoSeri,
                    BienKsoat = request.BienKsoat,
                    NgayKbtt = ConvertDateTime.ConverDateTimeVN(request.NgayKbtt),
                    NgayTthat = ConvertDateTime.ConverDateTimeVN(request.NgayTthat),
                    //NgayTthatkh = request.NgayTthatkh,
                    MaUserGdv = request.MaUserGdv,
                    NgayGdinh = ConvertDateTime.ConverDateTimeVN(request.NgayGdinh),
                    ThoigianTthat = request.ThoigianTthat,
                    DiaDiemtt = request.DiaDiemtt,
                    NguyenNhanTtat = request.NguyenNhanTtat,
                    HauquaNguoi = request.HauquaNguoi,
                    HauquaTsan = request.HauquaTsan,
                    NguyenNhanTtatkh = request.NguyenNhanTtatkh,
                    HauquaNguoikh = request.HauquaNguoikh,
                    HauquaTsankh = request.HauquaTsankh,
                    SoTienugd = request.SoTienugd,
                    TinhTrang = 1,
                    LoaihinhBh = String.IsNullOrEmpty(request.LoaiHinhBh) ? "050104" : request.LoaiHinhBh,
                    NgGdichTh = request.NgGdichTh,
                    MaPkt = request.MaPkt,
                    MaKthac = request.MaKthac,
                    MaNhloaixe = request.MaNhloaixe,
                    MaKh = request.MaKh,
                    DiaChiKh = request.DiaChiKh,
                    TrongTai = request.TrongTai,
                    TenUser = tenUser,
                    UserId = request.DienthoaiLienhe,
                    MaUser = maUser,
                    MaCbkt = request.MaCbkt,
                    NhanHieu = request.NhanHieu,
                    MaLoaixe = request.MaLoaiXe,
                    MaDongxe = request.MaDongxe,
                    NamSx = !string.IsNullOrEmpty(request.NamSx) ? (int)Convert.ToInt64(request.NamSx) : 0,
                    SoCngoi = request.SoCngoi,
                    NgayCapSeri = ConvertDateTime.ConverDateTimeVN(request.NgayCapSeri),
                    DienThoaiSeri = request.DienThoaiSeri,
                    PrKeySeri = request.PrKeySeri,
                    CoquanGquyet = request.CoquanGquyet,
                    NguoiLienhe = request.NguoiLienhe,
                    DienthoaiLienhe = request.DienthoaiLienhe,
                    ThoigianTthatkh = request.ThoigianTthatkh,
                    DiaDiemttkh = request.DiaDiemttkh,
                    IsdonviDuyet = request.IsdonviDuyet,
                    LoaiKbtt = request.LoaiKbtt,

                };
                await _context_my_pvi.AddAsync(newKbttCtu);
                await _context_my_pvi.SaveChangesAsync();
                return "Success";
            }
            catch (Exception ex)
            {
                _logger.Error("An error occured: " + ex.Message.ToString());
                return "Error";
            }

        }

        public async Task<KbttCtu> UpdateKbtt(decimal prKey, KbttCtuRequest request)
        {
            var record = await _context_my_pvi.KbttCtus
                .FirstOrDefaultAsync(x => x.PrKey == prKey);

            if (record != null)
            {

                record.NguoiLienhe = request.NguoiLienHe ?? record.NguoiLienhe;
                record.DienthoaiLienhe = request.DienThoaiLienHe ?? record.DienthoaiLienhe;
                record.LoaiKbtt = request.LoaiKbtt;
                record.NguyenNhanTtat = request.NguyenNhanTtat ?? record.NguyenNhanTtat;
                record.DiaDiemtt = request.DiaDiemTt ?? record.DiaDiemtt;
                record.HauquaTsan = request.HauquaTsan ?? record.HauquaTsan;
                record.HauquaNguoi = request.HauquaNguoi ?? record.HauquaNguoi;
                record.ThoigianTthat = request.ThoiGianTthat ?? record.ThoigianTthat;
                record.CoquanGquyet = request.CoQuanGquyet ?? record.CoquanGquyet;
                record.SoTienugd = request.SoTienugd;
                record.TinhTrang = request.TinhTrang;
                record.NgayGdinh = ConvertDateTime.ConverDateTimeVN(request.NgayGdinh) ?? record.NgayGdinh;
                record.NgayTthat = ConvertDateTime.ConverDateTimeVN(request.NgayTthat) ?? record.NgayTthat;
                record.MaUserGdv = request.MaUserGdv ?? record.MaUserGdv;
                record.LoaihinhBh = request.LoaiHinhBh ?? record.LoaihinhBh;
                record.UserId = request.DienThoaiLienHe ?? record.UserId;

                await _context_my_pvi.SaveChangesAsync();
            }

            return record;
        }
        public async Task<string> TaoDonHsgd(decimal prKey, string maLhsbt, string donviBth, string MaDonbh)
        {

            var soDonBh = await _context_my_pvi.KbttCtus.Where(x => x.PrKey == prKey).Select(x => x.SoDonbh).FirstOrDefaultAsync();
            if (soDonBh != null)
            {
                var listMaSdbsChecked = new List<string> { "03", "04" };
                var checkSoDonBh = await _context_pias.NvuBhtCtus.Where(x => x.SoDonbh == soDonBh && x.SoDonbhBs == "" && listMaSdbsChecked.Contains(x.MaSdbs)).Select(x => x.PrKey).AnyAsync();
                if (checkSoDonBh)
                {
                    return "Error";
                }
                bool checkSoDonBh2 = await (from A in _context_pias.NvuBhtCtus
                                            join B in _context_pias.NvuBhtSeris on A.PrKey equals B.FrKey
                                            where A.SoDonbhBs == ""
                                               && A.SoDonbh == soDonBh
                                               && A.MaSdbs == "02"
                                               && B.GiongLua == true
                                            select A.PrKey).AnyAsync();
                if (checkSoDonBh2)
                {
                    return "Error";
                }
                var result =
                   from k in _context_my_pvi.KbttCtus
                   where k.TinhTrang == 3 && k.PrKeyGdtt == 0 && k.PrKey == prKey
                   select new
                   {
                       k, // Select all columns from `kbtt_ctu`
                       HieuXe = (from dm in _context_my_pvi.DmXes
                                 where dm.MaHieuxe == k.NhanHieu
                                 select dm.TenHieuxe.Trim()).FirstOrDefault() ?? string.Empty,
                       LoaiXe = (from dm in _context_my_pvi.DmXes
                                 where dm.MaLoaixe == k.MaDongxe
                                 select dm.TenLoaixe.Trim()).FirstOrDefault() ?? string.Empty,
                       MaUser = "USER123",
                       NgayDau = k.NgayDauSeri.HasValue ? k.NgayDauSeri.Value.ToString("dd/MM/yyyy") : null,
                       NgayCuoi = k.NgayCuoiSeri.HasValue ? k.NgayCuoiSeri.Value.ToString("dd/MM/yyyy") : null,
                       NgayKbtt = k.NgayKbtt.HasValue ? k.NgayKbtt.Value.ToString("dd/MM/yyyy") : null,
                       NgGiamDinh = k.NgayGdinh.HasValue ? k.NgayGdinh.Value.ToString("dd/MM/yyyy") : null,
                       NgTonThat = k.NgayTthat.HasValue ? k.NgayTthat.Value.ToString("dd/MM/yyyy") : null,
                       MaUser247 = k.MaUserGdv
                   };

                if (result == null || result.Count() == 0)
                {
                    return "Hồ sơ đang không ở trạng thái chờ duyệt, không thể tạo HSGD";
                }
                else
                {
                    using (var context = new GdttContext())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                _context_pias.Database.SetCommandTimeout(180);
                                var kbttCtu = await _context_my_pvi.KbttCtus.Where(x => x.PrKey == prKey).AsNoTracking().FirstOrDefaultAsync();



                                var prKeySeri = kbttCtu.PrKeySeri;

                                // Count the number of matching records in `hsbt_ctu`
                                var soLanBt = _context_pias.HsbtCtus.Count(h =>
                                    h.PrKeySeri == prKeySeri &&
                                    (h.MaLhsbt == "TBT" || h.MaLhsbt == "NBT"));

                                // Generate new ID for GDVC
                                var maDonvi = kbttCtu.MaDonvi;
                                var maUser = result.Select(x => x.MaUser247).FirstOrDefault();

                                var soCtu = "";



                                string maCtugd = MaDonbh == "0501" ? "GDVC" : "GDXM";
                                int maDviInt = int.Parse(maDonvi);



                                var ctuKt = GetCtuGd(maCtugd, maDviInt);


                                if (ctuKt != null)
                                {
                                    int newID = (int)ctuKt.Num;
                                    //int newID = (int)Math.Round(ctuKt.Num) + 1;
                                    //ctuKt.Num = newID;

                                    //var ctuKt1 = _ctuKtRepository.GetCtuKt(maCtukt, maDviInt);
                                    //_context_pias.SaveChanges();
                                    var newID1 = newID.ToString("D6");

                                    _logger.Information("Create SoCtu success: @so_ctu ", newID1);
                                    soCtu = newID1;
                                }
                                else
                                {
                                    _logger.Error("Error when create SoCtu");

                                    return null;
                                }

                                //var bhcn_ctu = await _kbttBhcnCtuRepository.GetEntityByCondition(a => a.PrKey == result.PrKey);
                                var soHsgd = $"{DateTime.Now:yy}/{maDonvi}/{soCtu}";
                                string maTtrangGd = "9";
                                //string maDonviBth = "";
                                // Check if `soHsgd` is valid
                                var checkSoHdgd = await _context.HsgdCtus.Where(x => x.SoHsgd == soHsgd).AnyAsync();
                                if (checkSoHdgd)
                                {
                                    transaction.Rollback();
                                    return null;
                                }
                                await _context_my_pvi.KbttCtus.Where(x => x.PrKey == prKey).ExecuteUpdateAsync(s => s.SetProperty(a => a.SoHsgd, a => soHsgd));
                                if (maLhsbt == "3")
                                {
                                    maLhsbt = "2";
                                    maTtrangGd = "2";
                                }

                                var soTienUoc = kbttCtu.SoTienugd.ToString();
                                if (!new[] { "000000", "00000", "0000", "000" }.Any(soTienUoc.Contains))
                                {
                                    soTienUoc += "000000";
                                }

                                // Determine contact details
                                var tenLaixe = string.IsNullOrEmpty(kbttCtu.TenNguoiKy) ? kbttCtu.NguoiLienhe : kbttCtu.TenNguoiKy;
                                var dienThoaiChuXe = (bool)kbttCtu.IsChuXe ? kbttCtu.DienthoaiLienhe : string.Empty;
                                var maDviGd = "";
                                var loaiHinhBh = kbttCtu.LoaihinhBh.Contains("05") ? kbttCtu.LoaihinhBh : string.Empty;
                                if (!string.IsNullOrEmpty(maUser))
                                {
                                    maDviGd = await context.DmUsers.Where(x => x.Oid == Guid.Parse(maUser)).Select(x => x.MaDonvi).FirstOrDefaultAsync();

                                }

                                //var maDonViGd = 
                                //  Insert Hsgd_Ctu
                                var newHsgdCtu = new HsgdCtu
                                {
                                    MaCtu = MaDonbh == "0501" ? "GDVC" : "GDXM",
                                    NgayCtu = DateTime.Now,
                                    MaDonvi = maDonvi,
                                    MaPkt = kbttCtu.MaPkt,
                                    MaDonbh = MaDonbh,
                                    SoHsgd = soHsgd,
                                    SoDonbh = kbttCtu.SoDonbh,
                                    BienKsoat = kbttCtu.BienKsoat,
                                    SoSeri = kbttCtu.SoSeri,
                                    NgayDauSeri = kbttCtu.NgayDauSeri,
                                    NgayCuoiSeri = kbttCtu.NgayCuoiSeri,
                                    NgayTbao = kbttCtu.NgayKbtt,
                                    NgayTthat = kbttCtu.NgayTthat,
                                    NguyenNhanTtat = kbttCtu.NguyenNhanTtat,
                                    DiaDiemtt = kbttCtu.DiaDiemtt,
                                    MaKh = kbttCtu.MaKh,
                                    TenKhach = kbttCtu.TenKhach,
                                    MaTte = "VND",
                                    TygiaHt = 1,
                                    TygiaTt = 1,
                                    DiaDiemgd = "",
                                    MaGaraVcx = "",
                                    NgayGdinh = kbttCtu.NgayGdinh != null ? (DateTime)kbttCtu.NgayGdinh : DateTime.Now,
                                    MaTtrangGd = maTtrangGd,
                                    PrKeySeri = prKeySeri,
                                    MaKthac = kbttCtu.MaKthac,
                                    MaDaily = "",
                                    MaCbkt = kbttCtu.MaCbkt,
                                    MaNhloaixe = kbttCtu.MaNhloaixe,
                                    MaLhsbt = maLhsbt,
                                    DienThoai = kbttCtu.DienthoaiLienhe,
                                    SoLanBt = soLanBt,
                                    MaDviBtHo = donviBth,
                                    PrKeyBtHo = "0",
                                    PrKeyGoc = 0,
                                    GhiChu = "",
                                    NgLienhe = tenLaixe,
                                    MaUser = Guid.Parse(maUser),
                                    SoTienugd = decimal.Parse(soTienUoc),
                                    HsgdTpc = 1,
                                    MaDonvigd = (maDviGd == "31" || maDviGd == "32") ? maDviGd : context.DmDonvis.FirstOrDefault(d => d.MaDonvi == maDonvi)?.MaDvchuquan,
                                    NgGdichTh = kbttCtu.NgGdichTh,
                                    PrKeyKbtt = kbttCtu.PrKey,
                                    MaSanpham = loaiHinhBh,
                                    DienThoaiNdbh = dienThoaiChuXe,
                                    MaDonviTt = maDonvi,
                                    ChkDaydu = true,
                                    ChkDunghan = true
                                };

                                // Step 4: Save the new record
                                context.HsgdCtus.Add(newHsgdCtu);
                                await context.SaveChangesAsync();
                                var prKeyGdtt = newHsgdCtu.PrKey;
                                var tenTtrangGd = context.DmTtrangGds.Where(x => x.MaTtrangGd == maTtrangGd).Select(x => x.TenTtrangGd).FirstOrDefault();
                                var addedImageList = new List<HsgdCt>();
                                var nhatKy = new NhatKy
                                {
                                    FrKey = prKeyGdtt,
                                    MaTtrangGd = maTtrangGd,
                                    TenTtrangGd = tenTtrangGd,
                                    MaUser = Guid.Parse(maUser),
                                    GhiChu = "Tạo HSGĐ từ PVI Mobile"
                                };
                                context.NhatKies.Add(nhatKy);
                                var listImage = new List<ImageKbttResponse>();
                                if (kbttCtu.LoaiKbtt == 1)
                                {
                                    listImage = await (from A in _context_my_pvi.KbttCtus
                                                       join B in _context_my_pvi.KbttCts on A.PrKey equals B.FrKey
                                                       join C in _context_my_pvi.KbttAnhs on B.PrKey equals C.FrKey
                                                       where A.PrKey == prKey
                                                       select new ImageKbttResponse
                                                       {
                                                           PrKey = C.PrKey,
                                                           LoaiKbtt = "khgd",
                                                           ViDo = C.ViDo,
                                                           KinhDo = C.KinhDo,
                                                           Stt = B.Stt,
                                                           MaHmuc = B.MaHmuc,
                                                           TenHmuc = B.TenHmuc,
                                                           PathUrl = C.Url,
                                                           PathFile = C.Path.Replace("\\", "/"),
                                                           NgayChup = C.NgayChup,

                                                       }).ToListAsync();
                                    if (listImage.Count == 0)
                                    {
                                        listImage = await (from A in _context_my_pvi.KbttCtus
                                                           join B in _context_my_pvi.KbttAnhs on A.PrKey equals B.FrKey
                                                           where A.PrKey == prKey && B.MaHmuc != ""
                                                           select new ImageKbttResponse
                                                           {
                                                               PrKey = B.PrKey,
                                                               LoaiKbtt = "khkbtt",
                                                               ViDo = B.ViDo,
                                                               KinhDo = B.KinhDo,
                                                               MaHmuc = B.MaHmuc,
                                                               TenHmuc = "",
                                                               PathUrl = Regex.Replace(B.Url, @"^https?://pvi247\.pvi\.com\.vn", "https://cdn247.pvi.com.vn"),
                                                               PathFile = B.Path.Replace("\\", "/"),
                                                           }).ToListAsync();

                                    }
                                }
                                else
                                {
                                    listImage = await (from A in _context_my_pvi.KbttCtus
                                                       join B in _context_my_pvi.KbttAnhs on A.PrKey equals B.FrKey
                                                       where A.PrKey == prKey
                                                       select new ImageKbttResponse
                                                       {
                                                           PrKey = B.PrKey,
                                                           LoaiKbtt = "khkbtt",
                                                           ViDo = B.ViDo,
                                                           KinhDo = B.KinhDo,
                                                           MaHmuc = B.MaHmuc,
                                                           TenHmuc = "",
                                                           PathUrl = B.Url,
                                                           PathFile = B.Path.Replace("\\", "/"),
                                                           NgayChup = B.NgayChup,

                                                       }).ToListAsync();

                                }
                                // insert hsgd_ct
                                foreach (var record in listImage)
                                {

                                    var url = "";
                                    if (string.IsNullOrEmpty(record.PathUrl))
                                    {
                                        url = record.PathFile;
                                    }
                                    else
                                    {
                                        url = record.PathUrl;
                                    }
                                    url = url.Replace(@"CSSK_upload\", @"TCD\CLAIM_XCG\")
                                        .Replace(@"\\pvi.com.vn\DATA\", "https://cdn247.pvi.com.vn/upload_01/")
                                        .Replace("\\", " / ")
                                        .Replace("\\\\", "//")
                                        .Replace("pvi247.pvi.com.vn", "cdn247.pvi.com.vn")
                                        .Replace("DATA\\", "P247_Upload_New\\")
                                        .Replace("data\\", "P247_Upload_New\\")
                                        .Replace("CSSK_upload\\", "TCD\\CLAIM_XCG\\")
                                        .Replace("cssk_upload\\", "TCD\\CLAIM_XCG\\");



                                    var image = new HsgdCt
                                    {
                                        FrKey = prKeyGdtt,
                                        PathFile = record.PathFile,
                                        NgayChup = record.NgayChup,
                                        ViDoChup = record.ViDo,
                                        KinhDoChup = record.KinhDo,
                                        DienGiai = record.TenHmuc,
                                        PathUrl = url,
                                        PathOrginalFile = record.PathUrl,

                                    };
                                    addedImageList.Add(image);
                                }
                                await context.HsgdCts.AddRangeAsync(addedImageList);
                                if (maLhsbt == "2")
                                {
                                    maTtrangGd = "9";

                                    int maDviInt1 = int.Parse(maDonvi);



                                    var ctuKt1 = GetCtuGd(maCtugd, maDviInt1);


                                    if (ctuKt != null)
                                    {
                                        int newID = (int)ctuKt.Num;
                                        //int newID = (int)Math.Round(ctuKt.Num) + 1;
                                        //ctuKt.Num = newID;

                                        //var ctuKt1 = _ctuKtRepository.GetCtuKt(maCtukt, maDviInt);
                                        //_context_pias.SaveChanges();
                                        var newID1 = newID.ToString("D6");

                                        _logger.Information("Create SoCtu success: @so_ctu ", newID1);
                                        soCtu = newID1;
                                    }
                                    var soHsgd1 = $"{DateTime.Now:yy}/{maDonvi}/{soCtu}";
                                    var checkSoHdgd1 = await context.HsgdCtus.Where(x => x.SoHsgd == soHsgd1).AnyAsync();
                                    if (checkSoHdgd1)
                                    {
                                        transaction.Rollback();
                                        return null;
                                    }


                                    var soTienUoc1 = kbttCtu.SoTienugd.ToString();
                                    if (!new[] { "000000", "00000", "0000", "000" }.Any(soTienUoc1.Contains))
                                    {
                                        soTienUoc1 += "000000";
                                    }

                                    // Determine contact details
                                    var tenLaixe1 = string.IsNullOrEmpty(kbttCtu.TenNguoiKy) ? kbttCtu.NguoiLienhe : kbttCtu.TenNguoiKy;
                                    var dienThoaiChuXe1 = (bool)kbttCtu.IsChuXe ? kbttCtu.DienthoaiLienhe : string.Empty;

                                    var loaiHinhBh1 = kbttCtu.LoaihinhBh.Contains("05") ? kbttCtu.LoaihinhBh : string.Empty;

                                    //  Insert Hsgd_Ctu
                                    var newHsgdCtu1 = new HsgdCtu
                                    {
                                        MaCtu = MaDonbh == "0501" ? "GDVC" : "GDXM",
                                        NgayCtu = DateTime.Now,
                                        MaDonvi = maDonvi,
                                        MaPkt = kbttCtu.MaPkt,
                                        MaDonbh = MaDonbh,
                                        SoHsgd = soHsgd,
                                        SoDonbh = kbttCtu.SoDonbh,
                                        BienKsoat = kbttCtu.BienKsoat,
                                        SoSeri = kbttCtu.SoSeri,
                                        NgayDauSeri = kbttCtu.NgayDauSeri,
                                        NgayCuoiSeri = kbttCtu.NgayCuoiSeri,
                                        NgayTbao = kbttCtu.NgayKbtt,
                                        NgayTthat = kbttCtu.NgayTthat,
                                        NguyenNhanTtat = kbttCtu.NguyenNhanTtat,
                                        DiaDiemtt = kbttCtu.DiaDiemtt,
                                        MaKh = kbttCtu.MaKh,
                                        TenKhach = kbttCtu.TenKhach,
                                        MaTte = "VND",
                                        TygiaHt = 1,
                                        TygiaTt = 1,
                                        DiaDiemgd = "",
                                        MaGaraVcx = "",
                                        NgayGdinh = (DateTime)kbttCtu.NgayGdinh,
                                        MaTtrangGd = maTtrangGd,
                                        PrKeySeri = prKeySeri,
                                        MaKthac = kbttCtu.MaKthac,
                                        MaDaily = "",
                                        MaCbkt = kbttCtu.MaCbkt,
                                        MaNhloaixe = kbttCtu.MaNhloaixe,
                                        MaLhsbt = maLhsbt,
                                        DienThoai = kbttCtu.DienthoaiLienhe,
                                        SoLanBt = soLanBt,
                                        MaDviBtHo = donviBth,
                                        PrKeyBtHo = "0",
                                        PrKeyGoc = 0,
                                        GhiChu = "",
                                        NgLienhe = tenLaixe,
                                        MaUser = Guid.Parse(maUser),
                                        SoTienugd = decimal.Parse(soTienUoc),
                                        HsgdTpc = 1,
                                        MaDonvigd = context.DmDonvis.FirstOrDefault(d => d.MaDonvi == maDonvi)?.MaDvchuquan,
                                        NgGdichTh = kbttCtu.NgGdichTh,
                                        PrKeyKbtt = kbttCtu.PrKey,
                                        MaSanpham = loaiHinhBh,
                                        DienThoaiNdbh = dienThoaiChuXe
                                    };

                                    // Step 4: Save the new record
                                    context.HsgdCtus.Add(newHsgdCtu1);
                                    await context.SaveChangesAsync();
                                    var addedImageList1 = new List<HsgdCt>();
                                    prKeyGdtt = newHsgdCtu1.PrKey;
                                    var tenTtrangGd1 = context.DmTtrangGds.Where(x => x.MaTtrangGd == maTtrangGd).Select(x => x.TenTtrangGd).FirstOrDefault();

                                    var nhatKy1 = new NhatKy
                                    {
                                        FrKey = prKeyGdtt,
                                        MaTtrangGd = maTtrangGd,
                                        TenTtrangGd = tenTtrangGd,
                                        MaUser = Guid.Parse(maUser),
                                        GhiChu = "Tạo HSGĐ từ PVI Mobile"
                                    };
                                    context.NhatKies.Add(nhatKy1);
                                    var listImage1 = new List<ImageKbttResponse>();
                                    if (kbttCtu.LoaiKbtt == 1)
                                    {
                                        listImage = await (from A in _context_my_pvi.KbttCtus
                                                           join B in _context_my_pvi.KbttCts on A.PrKey equals B.FrKey
                                                           join C in _context_my_pvi.KbttAnhs on B.PrKey equals C.FrKey
                                                           where A.PrKey == prKey
                                                           select new ImageKbttResponse
                                                           {
                                                               PrKey = C.PrKey,
                                                               LoaiKbtt = "khgd",
                                                               ViDo = C.ViDo,
                                                               KinhDo = C.KinhDo,
                                                               Stt = B.Stt,
                                                               MaHmuc = B.MaHmuc,
                                                               TenHmuc = B.TenHmuc,
                                                               PathUrl = C.Url,
                                                               PathFile = C.Path.Replace("\\", "/"),
                                                               NgayChup = C.NgayChup,

                                                           }).ToListAsync();

                                    }
                                    else
                                    {
                                        listImage = await (from A in _context_my_pvi.KbttCtus
                                                           join B in _context_my_pvi.KbttAnhs on A.PrKey equals B.FrKey
                                                           where A.PrKey == prKey
                                                           select new ImageKbttResponse
                                                           {
                                                               PrKey = B.PrKey,
                                                               LoaiKbtt = "khkbtt",
                                                               ViDo = B.ViDo,
                                                               KinhDo = B.KinhDo,
                                                               TenHmuc = "",
                                                               PathUrl = B.Url,
                                                               PathFile = B.Path.Replace("\\", "/"),
                                                               NgayChup = B.NgayChup,

                                                           }).ToListAsync();

                                    }
                                    // insert hsgd_ct
                                    foreach (var record in listImage)
                                    {

                                        var url = "";
                                        if (string.IsNullOrEmpty(record.PathUrl))
                                        {
                                            url = record.PathFile;
                                        }
                                        else
                                        {
                                            url = record.PathUrl;
                                        }
                                        url = url.Replace(@"CSSK_upload\", @"TCD\CLAIM_XCG\")
                                            .Replace(@"\\pvi.com.vn\DATA\", "https://cdn247.pvi.com.vn/upload_01/")
                                            .Replace("\\", " / ")
                                            .Replace("\\\\", "//")
                                            .Replace("pvi247.pvi.com.vn", "cdn247.pvi.com.vn");
                                        var image = new HsgdCt
                                        {
                                            FrKey = prKeyGdtt,
                                            PathFile = record.PathFile,
                                            NgayChup = record.NgayChup,
                                            ViDoChup = record.ViDo,
                                            KinhDoChup = record.KinhDo,
                                            DienGiai = record.TenHmuc,
                                            PathUrl = url,
                                            PathOrginalFile = record.PathUrl,

                                        };
                                        addedImageList.Add(image);
                                    }
                                    await context.HsgdCts.AddRangeAsync(addedImageList);
                                }
                                var addedHsgdDx = new List<HsgdDx>();
                                //Trường hợp vcx thì update thêm tnds
                                if (kbttCtu.LoaiKbtt == 1)
                                {
                                    var hsgdCtuUpdate = context.HsgdCtus.Where(x => x.PrKey == prKeyGdtt)
                                        .ExecuteUpdate(x => x
                                        .SetProperty(x => x.NamSx, kbttCtu.NamSx)
                                        .SetProperty(x => x.HieuXe, context.DmHieuxes.Where(y => y.HieuXe == "").Select(y => y.PrKey).FirstOrDefault())
                                        .SetProperty(x => x.LoaiXe, context.DmLoaixes.Where(y => y.LoaiXe == "").Select(y => y.PrKey).FirstOrDefault()));

                                    foreach (var record in listImage)
                                    {

                                        var hsgdDx = new HsgdDx
                                        {
                                            FrKey = prKeyGdtt,
                                            MaHmuc = record.MaHmuc,
                                            SoTientt = 0,
                                            SoTienpdtt = 0,
                                            GhiChutt = "",
                                            LoaiDx = 0,
                                            NgayCapnhat = DateTime.Now,
                                            GetDate = DateTime.Now,

                                        };
                                        addedHsgdDx.Add(hsgdDx);
                                    }

                                    //_context.HsgdCtus.Where(x => x.PrKey == hsgd_ctu.PrKey || x.PrKey == Convert.ToInt64(hsgd_ctu.PrKeyBtHo))
                                    //        .ExecuteUpdate(s => s
                                    //        .SetProperty(u => u.TenLaixe, bcgd.TenLaiXe)
                                    //        .SetProperty(u => u.NamSinh, bcgd.NamSinh)
                                    //        .SetProperty(u => u.SoGphepLaixe, bcgd.SoGphepLaixe)
                                    //        .SetProperty(u => u.NgayDauLaixe, bcgd.NgayDauLaixe)
                                    //        .SetProperty(u => u.NgayCuoiLaixe, bcgd.NgayCuoiLaixe)
                                    //        .SetProperty(u => u.MaLoaibang, mlb[0])
                                    //        );

                                }
                                _logger.Information("Hsgd_CTU created : pr_key" + prKeyGdtt);
                                context.SaveChanges();
                                // update tinhTrang kbtt_ctu 
                                using (var contextMyPvi = new MY_PVIContext())
                                {
                                    using (var transaction1 = contextMyPvi.Database.BeginTransaction())
                                    {
                                        try
                                        {
                                            await contextMyPvi.KbttCtus
                                .Where(x => x.PrKey == prKey)
                                .ExecuteUpdateAsync(s => s.SetProperty(a => a.TinhTrang, 4));
                                            transaction1.Commit();
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.Error("Error when save KbttCtu with pr_key = " + prKey);
                                            transaction1.Rollback();
                                        }
                                    }
                                }

                                // Commit the transaction
                                transaction.Commit();
                            }
                            //return "Success";



                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                throw new Exception("Error during operation with pr_key: " + prKey + "" + ex.Message, ex);
                            }



                        }
                    }
                }
                return "Success";
            }
            else
            {
                return null;
            }

        }




        public DmCtugd? GetCtuGd(string maCtuGd, int maDviInt)
        {

            //TODO
            //Chuyen query sang kieu parameter
            //Chuyen sang transaction neu dung chung 1 DB connection, 2 DB thi thoi
            string query = $"DECLARE @Numctu NUMERIC(18,0); " +
                              $"UPDATE dm_ctugd SET DM_CTUGD.num = DM_CTUGD.num +1, @Numctu = DM_CTUGD.num + 1 WHERE ma_ctugd = @maCtugd AND ma_donvi = @maDonvi; " +
                            $"SELECT @Numctu as Numctu";

            var conn = _context.Database.GetDbConnection();
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }


            DmCtugd obj = new DmCtugd();
            var command = conn.CreateCommand();
            command.CommandText = query;
            var p = command.CreateParameter();
            p.ParameterName = "maCtugd";
            p.Value = maCtuGd;

            command.Parameters.Add(p);
            var p1 = command.CreateParameter();
            p1.ParameterName = "maDonvi";
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

        public async Task<string> CapNhatSoHsgd(DateTime startDate, DateTime endDate)
        {
            try
            {
                var listHsThieuHsgd = await _context_my_pvi.KbttCtus
                .Where(x => x.TinhTrang == 4
                            && x.SoHsgd == ""
                            && x.NgayKbtt > startDate
                            && x.NgayKbtt < endDate)
                .ToListAsync();
                if (listHsThieuHsgd.Count > 0)
                {
                    foreach (var hs in listHsThieuHsgd)
                    {
                        var soHsgdBosung = await _context.HsgdCtus.Where(x => x.SoDonbh == hs.SoDonbh && x.BienKsoat == hs.BienKsoat && x.SoSeri == hs.SoSeri).Select(x => x.SoHsgd).FirstOrDefaultAsync();
                        if (String.IsNullOrEmpty(soHsgdBosung))
                        {
                            continue;
                        }
                        else
                        {
                            //hs.SoHsgd = soHsgdBosung;
                            await _context_my_pvi.KbttCtus.Where(x => x.PrKey == hs.PrKey).ExecuteUpdateAsync(s => s.SetProperty(a => a.SoHsgd, a => soHsgdBosung));
                        }
                    }
                    await _context.SaveChangesAsync();
                    return "Done";
                }
                else
                {
                    return "Fail";
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error" + ex.Message.ToString());
                return "Fail";
            }




        }
        public async Task<string> InsertKbttCtu(KbttCtu request)
        {
            if (request.PrKey != 0)
            {
                return null;
            }
            if (string.IsNullOrEmpty(request.DiaDiemtt))
            {
                return null;
            }
            else
            {
                var newKbttCtu = new KbttCtu
                {
                    UserId = request.DienthoaiLienhe?.Trim(),
                    LoaiKbtt = request.LoaiKbtt,
                    NgayKbtt = DateTime.Now,
                    TinhTrang = 1,
                    MaDonvi = request.MaDonvi,
                    TenDonvi = request.TenDonvi,
                    SoDonbh = request.SoDonbh,
                    MaKthac = request.MaKthac,
                    MaCbkt = request.MaCbkt,
                    MaNhloaixe = request.MaNhloaixe,
                    MaPkt = request.MaPkt,
                    MaKh = request.MaKh,
                    TenKhach = request.TenKhach,
                    DiaChiKh = request.DiaChiKh,
                    NgGdichTh = request.NgGdichTh,
                    SoSeri = request.SoSeri,
                    BienKsoat = request.BienKsoat,
                    NhanHieu = request.NhanHieu,
                    MaLoaixe = request.MaLoaixe,
                    MaDongxe = request.MaDongxe,
                    NamSx = request.NamSx,
                    TrongTai = request.TrongTai,
                    SoCngoi = request.SoCngoi,
                    NgayCapSeri = Convert.ToDateTime(request.NgayCapSeri),
                    NgayDauSeri = Convert.ToDateTime(request.NgayDauSeri),
                    NgayCuoiSeri = Convert.ToDateTime(request.NgayCuoiSeri),
                    DienThoaiSeri = request.DienThoaiSeri,
                    NgayTthat = Convert.ToDateTime(request.NgayTthat),
                    ThoigianTthat = request.ThoigianTthat,
                    DiaDiemtt = request.DiaDiemtt,
                    NguyenNhanTtat = request.NguyenNhanTtat,
                    CoquanGquyet = request.CoquanGquyet,
                    HauquaTsan = request.HauquaTsan,
                    HauquaNguoi = request.HauquaNguoi,
                    NgayTthatkh = null,
                    //NgayTthatkh = Convert.ToDateTime(request.NgayTthatkh),
                    ThoigianTthatkh = request.ThoigianTthatkh,
                    DiaDiemttkh = request.DiaDiemttkh,
                    NguyenNhanTtatkh = request.NguyenNhanTtatkh,
                    HauquaTsankh = request.HauquaTsankh,
                    HauquaNguoikh = request.HauquaNguoikh,
                    NgayGdinh = Convert.ToDateTime(request.NgayGdinh),
                    NguoiLienhe = request.NguoiLienhe,
                    DienthoaiLienhe = request.DienthoaiLienhe,
                    GaraGiamdinh = "",
                    GaraSuachua = "",
                    SoTienugd = request.SoTienugd,
                    PrKeySeri = request.PrKeySeri,
                    PrKeyGdtt = 0,
                    LoaihinhBh = "VCX",
                    MaUser = request.MaUser,
                    TenUser = request.TenUser,
                    MaUserGdv = request.MaUserGdv
                };

                await _context_my_pvi.KbttCtus.AddAsync(newKbttCtu);
                var result = await _context_my_pvi.SaveChangesAsync();
                if (result == 1)
                {
                    return "Success";
                }
                else
                {
                    return "Error";
                }

            }
        }


        public async Task<List<LoaiHinhBhDTO>> GetListLoaiHinhBh()
        {
            var listSpAccepted = new List<string> { "050104", "050101", "050103" };

            var listSp = await _context_pias.DmSps
                .Where(x => listSpAccepted.Contains(x.MaSp))
                .Select(sp => new LoaiHinhBhDTO
                {
                    MaSp = sp.MaSp,
                    TenSp = $"{sp.TenSp} (MyPVI)"
                })
                .ToListAsync();

            var additionalLoaiHinhBh = new List<LoaiHinhBhDTO>
    {
        new LoaiHinhBhDTO { MaSp = "VCX", TenSp = "Vật chất xe (PVIMobile)" },
        new LoaiHinhBhDTO { MaSp = "TNDS_CX", TenSp = "Trách nhiệm dân sự (PVIMobile)" }
    };

            return listSp.Concat(additionalLoaiHinhBh).ToList();
        }

        public class KbttCtuRequest
        {
            public string? NguoiLienHe { get; set; }
            public string? DienThoaiLienHe { get; set; }
            public int LoaiKbtt { get; set; }
            public string? NguyenNhanTtat { get; set; }
            public string? DiaDiemTt { get; set; }
            public string? HauquaTsan { get; set; }
            public string? HauquaNguoi { get; set; }
            public string? ThoiGianTthat { get; set; }
            public string? CoQuanGquyet { get; set; }
            public decimal SoTienugd { get; set; }
            public int TinhTrang { get; set; }
            public string? NgayGdinh { get; set; }
            public string? NgayTthat { get; set; }
            public string? MaUserGdv { get; set; }
            public string? LoaiHinhBh { get; set; }
        }
    }
}
