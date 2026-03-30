using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PVI.DAO.Entities.Models;
using PVI.Helper;
using PVI.Helper.Enums;
using PVI.Repository.Interfaces;
using ServiceReference1;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using static Azure.Core.HttpHeader;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace PVI.Repository.Repositories
{

    /* Implementation cho interface Báo cáo.
     * lhkhanh - 05/11/2024
     */

    // LƯU Ý:
    /* Các API dưới đây đều có 2 filter: Một filter to (Main_Filter), và 1 filter nhỏ hơn (Side_Filter) cho bảng dữ liệu
     * Trong 1 function, các dữ liệu báo cáo sẽ được chain 2 lần do có 2 loại filter này, nhưng về cơ bản thì vẫn là GET data.
     * 
     * Lý do các API to như vậy để dễ bảo trì. Toàn bộ implementation đang được để ngay trong function. 
     * Khi lên Production, tuỳ theo nhu cầu mà có thể refactor cho gọn
     */

    // Kế thừa base.
    public class BaoCaoRepository : GenericRepository<HsgdCtu>, IBaoCaoRepository
    {
        BaoCaoHelper BCHelper = null;
        HsgdDxRepository _dx_repo = null;
        public BaoCaoRepository(GdttContext context, Pvs2024Context context_pias, Pvs2024UpdateContext context_pias_update, Serilog.ILogger logger, IConfiguration conf) : base(context, context_pias, logger, conf)
        {
            BCHelper = new BaoCaoHelper(context, context_pias, logger, conf);
            _dx_repo = new HsgdDxRepository(context, context_pias, context_pias_update, logger, conf);
        }

        // Lấy danh sách đơn vị - riêng của báo cáo do có lấy cả mã đơn vị phân quyền.

        public List<DmDonvi> getListDonvi_BaoCao(string currentUserEmail)
        {
            try
            {
                DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
                if (currentUser != null)
                {
                    // Sử dụng để kiểm tra tra phân quyền đơn vị của user:

                    string[] donvi_pquyen = null; // Danh sách đơn vị phân quyền

                    string type_of_user = ""; // Sử dụng để lọc đơn vị theo loại user

                    // Gắn mã theo dạng string để chắc chắn check chuẩn.
                    if (currentUser.MaDonvi == "00" || currentUser.MaDonvi == "31" || currentUser.MaDonvi == "32")
                    {
                        type_of_user = "FULL_AUTH";
                    }
                    else if (!String.IsNullOrEmpty(currentUser.MaDonviPquyen))
                    {
                        type_of_user = "HAVE_DVI_PQUYEN";
                        donvi_pquyen = currentUser.MaDonviPquyen.Split(',');
                    }
                    else
                    {
                        type_of_user = "NO_DVI";
                    }

                    List<DmDonvi> listDonvi = (from donvi in _context.DmDonvis
                                               where (
                                                  (type_of_user == "NO_DVI" ? donvi.MaDonvi.Equals(currentUser.MaDonvi) && donvi.MaDonvi != "00" : (
                                                   type_of_user == "HAVE_DVI_PQUYEN" ? (Array.Exists(donvi_pquyen, x => x == donvi.MaDonvi) || donvi.MaDonvi.Equals(currentUser.MaDonvi)) : true))
                                               )
                                               select new DmDonvi
                                               {
                                                   MaDonvi = donvi.MaDonvi,
                                                   TenDonvi = donvi.TenDonvi,
                                                   MaDvchuquan = donvi.MaDvchuquan,
                                                   MaKh = donvi.MaKh,
                                               }
                                                ).ToList();
                    return listDonvi;
                }
                else
                {
                    return new List<DmDonvi>();
                }
            }
            catch (Exception err)
            {
                return new List<DmDonvi>();
            }
        }

        // 1 - Thống kê GDTT theo đơn vị

        public ThongKe_GDTT_DonVi_Response ThongKe_GDTT_Donvi(ThongKe_GDTT_DonVi_Filter filter, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
            int[] acceptedUsers = new int[] { 1, 2, 3, 6, 9, 10 };
            if (currentUser != null && (Array.Exists(acceptedUsers, x => x == currentUser.LoaiUser) || _context.PquyenCnangs.Where(x => x.MaUser.Equals(currentUser.MaUser)).FirstOrDefault().LoaiQuyen.Equals("BAOCAO03")))
            {
                try
                {
                    // Lấy danh sách đơn vị theo filter.
                    List<DmDonvi> danhSachDonVi = _context.DmDonvis.Where(x => (filter.MaDonVi != null ? x.MaDonvi.Equals(filter.MaDonVi) : true) && x.MaDonvi != "00" && x.MaDonvi != "31" && x.MaDonvi != "32").ToList();

                    ThongKe_GDTT_DonVi_Response result = new ThongKe_GDTT_DonVi_Response
                    {
                        Count = danhSachDonVi.Count(),
                        Data = new List<ThongKe_GDTT_DonVi_Item>()
                    };

                    // Sau đó thống kê cho từng đơn vị:
                    danhSachDonVi.ForEach(item =>
                    {
                        // Tính toán cho từng đơn vị
                        List<decimal> thong_ke_result = BCHelper.TinhToan_SoLuong_GDTT_Donvi(item.MaDonvi, filter.TuNgay, (filter.DenNgay));
                        ThongKe_GDTT_DonVi_Item item_01 = new ThongKe_GDTT_DonVi_Item
                        {
                            TenDonvi = item.TenDonvi,
                            SoLuongNhapPIAS = thong_ke_result[0],
                            SoLuongGDTT = thong_ke_result[1],
                            SoLuongKhongGDTT = thong_ke_result[2],
                            TyLeGDTT = (float)(thong_ke_result[0] != 0 ? thong_ke_result[1] / thong_ke_result[0] * 100 : 0),
                            ChuaGiaoGD = thong_ke_result[3],
                            DaGiaoGD = thong_ke_result[4],
                            DangGD = thong_ke_result[5],
                            ChoPD = thong_ke_result[6],
                            BSTT = thong_ke_result[7],
                            DaDuyet = thong_ke_result[8],
                            TongCong = thong_ke_result[9]
                        };

                        result.Data.Add(item_01);
                    });

                    return result;

                }
                catch (Exception err)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }



        // 2 - Thống kê GDTT theo GDV

        public ThongKe_GDTT_GDV_Response ThongKe_GDTT_GDV(ThongKe_GDTT_GDV_Filter filter, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
            int[] acceptedUsers = new int[] { 1, 2, 3, 6, 9, 10 };
            if (currentUser != null && (Array.Exists(acceptedUsers, x => x == currentUser.LoaiUser) || _context.PquyenCnangs.Where(x => x.MaUser.Equals(currentUser.MaUser)).FirstOrDefault().LoaiQuyen.Equals("BAOCAO03")))
            {
                try
                {
                    // Lấy danh sách đơn vị theo filter.
                    List<DmDonvi> danhSachDonVi = _context.DmDonvis.Where(x => (filter.MaDonVi != null ? x.MaDonvi.Equals(filter.MaDonVi) : true) && x.MaDonvi != "00" && x.MaDonvi != "31" && x.MaDonvi != "32").ToList();

                    ThongKe_GDTT_GDV_Response result = new ThongKe_GDTT_GDV_Response
                    {
                        Count = danhSachDonVi.Count(),
                        Data = new List<ThongKe_GDTT_GDV_Item>()
                    };

                    // Sau đó thống kê cho từng đơn vị:
                    danhSachDonVi.ForEach(item =>
                    {
                        // Tính toán cho GDV của từng đơn vị

                        result.Data.AddRange(BCHelper.TinhToan_SoLuong_GDTT_GDV(item.MaDonvi, filter.TuNgay, filter.DenNgay));

                    });

                    return result;

                }
                catch (Exception err)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }



        // 4 - Tra cứu giá phụ tùng

        public SearchGiaPhuTungResponse SearchGiaPhuTung(SearchGiaPhuTung_Main_Filter filter, int pageNumber, int pageSize, string currentUserEmail)
        {
            try
            {
                var subQuery1 = _context.HsgdDxes.Where(dx => dx.FrKey != 0).Select(dx => dx.PrKey);
                var subQuery2 = _context.HsgdDxes.Where(dx => dx.FrKey == 0).Select(dx => dx.PrKey);
                // Subquery: lấy ngày cập nhật lớn nhất theo FrKey có TrangThai = "6"
                var latestDates = from nk in _context.NhatKies
                                  where nk.MaTtrangGd == "6"
                                  group nk by nk.FrKey into g
                                  select new
                                  {
                                      FrKey = g.Key,
                                      MaxNgayCapNhat = g.Max(x => x.NgayCapnhat)
                                  };
                var query1 = (from hsgdCtu in _context.HsgdCtus
                              join hsgdDx in _context.HsgdDxes on hsgdCtu.PrKey equals hsgdDx.FrKey
                              join gara in _context.DmGaRas on hsgdCtu.MaGaraVcx equals gara.MaGara
                              join hieuxe in _context.DmHieuxes on hsgdCtu.HieuXe equals hieuxe.PrKey
                              join loaixe in _context.DmLoaixes on hsgdCtu.LoaiXe equals loaixe.PrKey
                              join nk in latestDates on hsgdCtu.PrKey equals nk.FrKey into nk_join
                              from nk_data in nk_join.DefaultIfEmpty()
                              where (
                                 hsgdDx.SoTientt != 0 && hsgdCtu.MaTtrangGd == "6" && subQuery1.Contains(hsgdDx.PrKey) &&
                                 (filter.MaDonVi != null ? hsgdCtu.MaDonvi.Equals(filter.MaDonVi) : true) &&
                                 (filter.TuNgay != null ? nk_data.MaxNgayCapNhat != null && EF.Functions.DateDiffDay(filter.TuNgay, nk_data.MaxNgayCapNhat) >= 0 : true) &&
                                 (filter.DenNgay != null ? nk_data.MaxNgayCapNhat != null && EF.Functions.DateDiffDay(filter.DenNgay, nk_data.MaxNgayCapNhat) <= 0 : true) &&
                                 (filter.MaHmuc != null ? hsgdDx.MaHmuc.Equals(filter.MaHmuc) : true) &&
                                 (filter.HieuXe != null ? hsgdCtu.HieuXe == filter.HieuXe : true) &&
                                 (filter.LoaiXe != null ? hsgdCtu.LoaiXe == filter.LoaiXe : true) &&
                                 (filter.XuatXu != null ? hsgdCtu.XuatXu.Equals(filter.XuatXu) : true) &&
                                 (filter.Tinh != null ? gara.TenTinh.Contains(filter.Tinh) : true) &&
                                 (filter.QuanHuyen != null ? gara.QuanHuyen.Equals(filter.QuanHuyen) : true)
                              )

                              select new SearchGiaPhuTungItem
                              {
                                  PrKey = hsgdCtu.PrKey,
                                  BienKSoat = hsgdCtu.BienKsoat,
                                  SoHsgd = hsgdCtu.SoHsgd,
                                  NgayTThat = hsgdCtu.NgayTthat,
                                  HieuXe = hieuxe.HieuXe,
                                  LoaiXe = loaixe.LoaiXe,
                                  NamSx = hsgdCtu.NamSx,
                                  XuatXu = hsgdCtu.XuatXu,
                                  TenHmucThayThe = _context.DmHmucs.Where(x => x.MaHmuc.Equals(hsgdDx.MaHmuc)).FirstOrDefault().TenHmuc,
                                  GhiChuDonVi = hsgdDx.GhiChudv,
                                  GiaPhuTung = hsgdDx.SoTientt,
                                  GiaThayThe = hsgdDx.SoTienpdtt,
                                  TenGara = gara.TenGara,
                                  NgayDuyet = hsgdCtu.NgayDuyet,
                                  Tinh = gara.TenTinh,
                                  QuanHuyen = gara.QuanHuyen
                              }
                                                       ).AsNoTracking().AsQueryable();
                var query2 = (from hsgdCtu in _context.HsgdCtus
                              join hsgdDxct in _context.HsgdDxCts on hsgdCtu.PrKeyBt equals hsgdDxct.PrKeyHsbtCtu
                              join hsgdDx in _context.HsgdDxes on hsgdDxct.PrKey equals hsgdDx.PrKeyDx
                              join gara in _context.DmGaRas on hsgdDxct.MaGara equals gara.MaGara
                              join hieuxe in _context.DmHieuxes on hsgdDxct.HieuXe equals hieuxe.PrKey
                              join loaixe in _context.DmLoaixes on hsgdDxct.LoaiXe equals loaixe.PrKey
                              join nk in latestDates on hsgdCtu.PrKey equals nk.FrKey into nk_join
                              from nk_data in nk_join.DefaultIfEmpty()
                              where (
                                 hsgdDx.SoTientt != 0 && hsgdCtu.MaTtrangGd == "6" && subQuery2.Contains(hsgdDx.PrKey) &&
                                 (filter.MaDonVi != null ? hsgdCtu.MaDonvi.Equals(filter.MaDonVi) : true) &&
                                 (filter.TuNgay != null ? nk_data.MaxNgayCapNhat != null && EF.Functions.DateDiffDay(filter.TuNgay, nk_data.MaxNgayCapNhat) >= 0 : true) &&
                                 (filter.DenNgay != null ? nk_data.MaxNgayCapNhat != null && EF.Functions.DateDiffDay(filter.DenNgay, nk_data.MaxNgayCapNhat) <= 0 : true) &&
                                 (filter.MaHmuc != null ? hsgdDx.MaHmuc.Equals(filter.MaHmuc) : true) &&
                                 (filter.HieuXe != null ? hsgdDxct.HieuXe == filter.HieuXe : true) &&
                                 (filter.LoaiXe != null ? hsgdDxct.LoaiXe == filter.LoaiXe : true) &&
                                 (filter.XuatXu != null ? hsgdDxct.XuatXu.Equals(filter.XuatXu) : true) &&
                                 (filter.Tinh != null ? gara.TenTinh.Contains(filter.Tinh) : true) &&
                                 (filter.QuanHuyen != null ? gara.QuanHuyen.Equals(filter.QuanHuyen) : true)
                              )

                              select new SearchGiaPhuTungItem
                              {
                                  PrKey = hsgdCtu.PrKey,
                                  BienKSoat = hsgdCtu.BienKsoat,
                                  SoHsgd = hsgdCtu.SoHsgd,
                                  NgayTThat = hsgdCtu.NgayTthat,
                                  HieuXe = hieuxe.HieuXe,
                                  LoaiXe = loaixe.LoaiXe,
                                  NamSx = hsgdCtu.NamSx,
                                  XuatXu = hsgdCtu.XuatXu,
                                  TenHmucThayThe = _context.DmHmucs.Where(x => x.MaHmuc.Equals(hsgdDx.MaHmuc)).FirstOrDefault().TenHmuc,
                                  GhiChuDonVi = hsgdDx.GhiChudv,
                                  GiaPhuTung = hsgdDx.SoTientt,
                                  GiaThayThe = hsgdDx.SoTienpdtt,
                                  TenGara = gara.TenGara,
                                  NgayDuyet = hsgdCtu.NgayDuyet,
                                  Tinh = gara.TenTinh,
                                  QuanHuyen = gara.QuanHuyen
                              }
                                                       ).AsNoTracking().AsQueryable();
                var main_item_list = query1.Concat(query2);

                // Request 22/11/2024: Không filter toàn bộ record mà chỉ filter 

                // LƯU Ý: TẠM THỜI KHÔNG XOÁ BLOCK DƯỚI ĐỂ TRONG TRƯỜNG HỢP CẦN FILTER TOÀN BỘ RECORD CÓ THỂ THỰC HIỆN LUÔN

                //var itemList = (from item in main_item_list
                //                where (
                //                   (filter.sideFilter.BienKSoat != null ? item.BienKSoat.Contains(filter.sideFilter.BienKSoat) : true) &&
                //                   (filter.sideFilter.SoHsgd != null ? item.SoHsgd.Contains(filter.sideFilter.SoHsgd) : true) &&
                //                   (filter.sideFilter.NgayTThat != null ? item.NgayTThat != null && item.NgayTThat.Value.Date >= filter.sideFilter.NgayTThat.Value.Date : true) &&
                //                   (filter.sideFilter.HieuXe != null ? item.HieuXe.Contains(filter.sideFilter.HieuXe) : true) &&
                //                   (filter.sideFilter.LoaiXe != null ? item.LoaiXe.Contains(filter.sideFilter.LoaiXe) : true) &&
                //                   (filter.sideFilter.NamSx != null ? item.NamSx.Equals(filter.sideFilter.NamSx) : true) &&
                //                   (filter.sideFilter.XuatXu != null ? item.XuatXu.Equals(filter.sideFilter.XuatXu) : true) &&
                //                   (filter.sideFilter.TenHmucThayThe != null ? item.TenHmucThayThe.Contains(filter.sideFilter.TenHmucThayThe) : true) &&
                //                   (filter.sideFilter.GhiChuDonVi != null ? item.GhiChuDonVi.Contains(filter.sideFilter.GhiChuDonVi) : true) &&
                //                   (filter.sideFilter.GiaPhuTung != null ? item.GiaPhuTung.ToString().StartsWith(filter.sideFilter.GiaPhuTung.ToString()) : true) &&
                //                   (filter.sideFilter.GiaThayThe != null ? item.GiaThayThe.ToString().StartsWith(filter.sideFilter.GiaThayThe.ToString()) : true) &&
                //                   (filter.sideFilter.TenGara != null ? item.TenGara.Contains(filter.sideFilter.TenGara) : true) &&
                //                   (filter.sideFilter.NgayDuyet != null ? item.NgayDuyet.Value.Date != null && (item.NgayDuyet.Value.Date >= filter.sideFilter.NgayDuyet.Value.Date) : true) &&
                //                   (filter.sideFilter.Tinh != null ? item.Tinh.Contains(filter.sideFilter.Tinh) : true) &&
                //                   (filter.sideFilter.QuanHuyen != null ? item.QuanHuyen.Contains(filter.sideFilter.QuanHuyen) : true)
                //                )
                //                select new SearchGiaPhuTungItem
                //                {
                //                    PrKey = item.PrKey,
                //                    BienKSoat = item.BienKSoat,
                //                    SoHsgd = item.SoHsgd,
                //                    NgayTThat = item.NgayTThat,
                //                    HieuXe = item.HieuXe,
                //                    LoaiXe = item.LoaiXe,
                //                    NamSx = item.NamSx,
                //                    XuatXu = item.XuatXu,
                //                    TenHmucThayThe = item.TenHmucThayThe,
                //                    GhiChuDonVi = item.GhiChuDonVi,
                //                    GiaPhuTung = item.GiaPhuTung,
                //                    GiaThayThe = item.GiaThayThe,
                //                    TenGara = item.TenGara,
                //                    NgayDuyet = item.NgayDuyet,
                //                    Tinh = item.Tinh,
                //                    QuanHuyen = item.QuanHuyen
                //                }
                //                ).AsQueryable();


                SearchGiaPhuTungResponse searchResult = new SearchGiaPhuTungResponse
                {
                    Count = main_item_list.Count(),
                    Data = main_item_list.OrderByDescending(x => x.SoHsgd).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
                };

                return searchResult;

            }
            catch (Exception err)
            {
                // Nếu có lỗi thì trả entity rỗng. 
                return new SearchGiaPhuTungResponse();
            }
        }

        // Báo cáo tình hình hồ sơ trên phân cấp
        // LƯU Ý: NGHIỆP VỤ NÀY KHOÁ TRƯỜNG TRẠNG THÁI VÀ LOẠI HSGD !
        public HSTPC_Response BCHSTPC_TrenPhanCap(HSTPC_Filter filter, int pageNumber, int pageSize, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
            int[] acceptedUsers = new int[] { 1, 2, 3, 6, 9, 10 };
            if (currentUser != null && (Array.Exists(acceptedUsers, x => x == currentUser.LoaiUser) || _context.PquyenCnangs.Where(x => x.MaUser.Equals(currentUser.MaUser)).FirstOrDefault().LoaiQuyen.Equals("BAOCAO03")))
            {
                try
                {
                    // Do 2 DB khác nhau nên phải kéo list từ 2 bên khác nhau.
                    // Lấy bảng hsgdCtu từ GDTT

                    var list_hsgd_ctu = (from hsgdCtu in _context.HsgdCtus
                                             //join hsgdCt in _context.HsgdCts on hsgdCtu.PrKey equals hsgdCt.FrKey
                                         join TtrangGd in _context.DmTtrangGds on hsgdCtu.MaTtrangGd equals TtrangGd.MaTtrangGd
                                         orderby hsgdCtu.PrKey descending

                                         where (
                                             hsgdCtu.HsgdTpc == 1 && hsgdCtu.NgayDuyet != null &&
                                             (filter.MaDonVi != null ? hsgdCtu.MaDonvi.Equals(filter.MaDonVi) : true) &&
                                             (filter.TuNgay != null ? hsgdCtu.NgayCtu >= filter.TuNgay : true) &&
                                             (filter.DenNgay != null ? hsgdCtu.NgayCtu <= filter.DenNgay : true) &&
                                             (filter.SoHsgd != null ? hsgdCtu.SoHsgd.Equals(filter.SoHsgd) : true) &&
                                             (filter.SoAnChi != null ? hsgdCtu.SoSeri.Equals(filter.SoAnChi) : true) &&
                                             (filter.BienKSoat != null ? hsgdCtu.BienKsoat.Equals(filter.BienKSoat) : true)
                                         )

                                         select new HSTPC_Item
                                         {
                                             PrKey = hsgdCtu.PrKey,
                                             MaDonvi = hsgdCtu.MaDonvi,
                                             SoHsgd = hsgdCtu.SoHsgd,
                                             TenKhach = hsgdCtu.TenKhach,
                                             SoAnChi = hsgdCtu.SoSeri,
                                             BienKSoat = hsgdCtu.BienKsoat,
                                             TuNgay = hsgdCtu.NgayDauSeri,
                                             DenNgay = hsgdCtu.NgayCuoiSeri,
                                             NgayTThat = hsgdCtu.NgayTthat,
                                             UocBT = hsgdCtu.SoTienugd,
                                             NgayDeXuat = hsgdCtu.NgayGdinh,
                                             UocDX = _context.HsgdDgs.Where(x => x.FrKey == hsgdCtu.PrKey && !x.LoaiDg).FirstOrDefault().SoTien,
                                             NgayDuyet = hsgdCtu.NgayDuyet ?? null,
                                             SoTienPD = _context.HsgdDgs.Where(x => x.FrKey == hsgdCtu.PrKey && x.LoaiDg).FirstOrDefault().SoTien,
                                             DonviSuaChua = hsgdCtu.MaDonvi,
                                             MaTtrangGd = hsgdCtu.MaTtrangGd,
                                             SoNgayXuLy = hsgdCtu.NgayDuyet != null ? BCHelper.DAY_DIFF(hsgdCtu.NgayGdinh, hsgdCtu.NgayDuyet.Value) : -1,
                                             giamDinhVien = _context.DmUsers.Where(x => x.Oid == hsgdCtu.MaUser).FirstOrDefault().TenUser,
                                             canBoTT = !String.IsNullOrEmpty(hsgdCtu.NguoiXuly) ? _context.DmUsers.Where(x => x.Oid.ToString().ToLower() == (hsgdCtu.NguoiXuly)).FirstOrDefault().TenUser : "",
                                             GhiChu = hsgdCtu.GhiChu
                                         }
                                            ).AsQueryable();

                    HSTPC_Response searchResult = new HSTPC_Response
                    {
                        Count = list_hsgd_ctu.Count(),
                        Data = list_hsgd_ctu.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
                    };

                    // Request 22/11/2024: Không filter toàn bộ record mà chỉ filter 
                    // LƯU Ý: TẠM THỜI KHÔNG XOÁ BLOCK DƯỚI ĐỂ TRONG TRƯỜNG HỢP CẦN FILTER TOÀN BỘ RECORD CÓ THỂ THỰC HIỆN LUÔN

                    //var itemList = (from item in list_hsgd_ctu
                    //                     where (
                    //                      (filter.sideFilter.TenDonvi != null ? item.TenDonvi.Contains(filter.sideFilter.TenDonvi) : true) &&
                    //                        (filter.sideFilter.BienKSoat != null ? item.BienKSoat.Contains(filter.sideFilter.BienKSoat) : true) &&
                    //                        (filter.sideFilter.SoHsgd != null ? item.SoHsgd.Contains(filter.sideFilter.SoHsgd) : true) &&
                    //                        (filter.sideFilter.HieuXe != null ? item.HieuXe.Contains(filter.sideFilter.HieuXe) : true) &&
                    //                        (filter.sideFilter.LoaiXe != null ? item.LoaiXe.Contains(filter.sideFilter.LoaiXe) : true) &&
                    //                        (filter.sideFilter.NamSx != null ? item.NamSx.Equals(filter.sideFilter.NamSx) : true) &&
                    //                        (filter.sideFilter.XuatXu != null ? item.XuatXu.Equals(filter.sideFilter.XuatXu) : true) &&
                    //                        (filter.sideFilter.NgayDuyet != null ? (item.NgayDuyet != null) && (item.NgayDuyet.Value.Date >= filter.sideFilter.NgayDuyet.Value.Date) : true) &&
                    //                        //(filter.sideFilter.Tinh != null ? item.Tinh.Contains(filter.sideFilter.Tinh) : true) &&
                    //                        (filter.sideFilter.SoTienThucTe != null ? item.SoTienThucTe.ToString().StartsWith(filter.sideFilter.SoTienThucTe.ToString()) : true)
                    //                     )
                    //                     select new SearchGtttItem
                    //                     {
                    //                         PrKey = item.PrKey,
                    //                         PrKeySerial = item.PrKeySerial,
                    //                         TenDonvi = item.TenDonvi,
                    //                         BienKSoat = item.BienKSoat,
                    //                         SoHsgd = item.SoHsgd,
                    //                         HieuXe = item.HieuXe,
                    //                         LoaiXe = item.LoaiXe,
                    //                         NamSx = item.NamSx,
                    //                         XuatXu = item.XuatXu,
                    //                         NgayDuyet = item.NgayDuyet,
                    //                         //Tinh = item.Tinh,
                    //                         SoTienThucTe = item.SoTienThucTe
                    //                     }
                    //          ).AsQueryable();




                    return searchResult;

                }
                catch (Exception err)
                {
                    return new HSTPC_Response();
                }
            }
            else
            {
                // User không có quyền xem báo cáo hoặc xuất Excel.
                // Báo count lỗi.
                return new HSTPC_Response()
                {
                    Count = 4040404
                };
            }
        }


        // Tra cứu giá trị thực tế
        public SearchGtttResponse SearchGttt(SearchGttt_Main_Filter filter, int pageNumber, int pageSize, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
            int[] acceptedUsers = new int[] { 1, 2, 3, 6, 9, 10 };
            if (currentUser != null && (Array.Exists(acceptedUsers, x => x == currentUser.LoaiUser) || _context.PquyenCnangs.Where(x => x.MaUser.Equals(currentUser.MaUser)).FirstOrDefault().LoaiQuyen.Equals("BAOCAO07")))
            {
                try
                {
                    // Do 2 DB khác nhau nên phải kéo list từ 2 bên khác nhau.
                    // Lấy bảng hsgdCtu từ GDTT

                    var list_hsgd_ctu = (from hsgdCtu in _context.HsgdCtus
                                         join gara in _context.DmGaRas on hsgdCtu.MaGaraVcx equals gara.MaGara
                                         join hieuxe in _context.DmHieuxes on hsgdCtu.HieuXe equals hieuxe.PrKey
                                         join loaixe in _context.DmLoaixes on hsgdCtu.LoaiXe equals loaixe.PrKey
                                         where (
                                            (filter.MaDonVi != null ? hsgdCtu.MaDonvi.Equals(filter.MaDonVi) : true) &&
                                            (filter.TuNgay != null ? (hsgdCtu.NgayDuyet != null) && (hsgdCtu.NgayDuyet.Value.Date >= filter.TuNgay.Value.Date) : true) &&
                                            (filter.BienKSoat != null ? hsgdCtu.BienKsoat.Contains(filter.BienKSoat) : true) &&
                                            (filter.DenNgay != null ? (hsgdCtu.NgayDuyet != null) && (hsgdCtu.NgayDuyet.Value.Date <= filter.DenNgay.Value.Date) : true) &&
                                            (filter.SoHSGD != null ? hsgdCtu.SoHsgd.Contains(filter.SoHSGD) : true) &&
                                            (filter.HieuXe != null ? hsgdCtu.HieuXe.ToString().Equals(filter.HieuXe) : true) &&
                                            (filter.LoaiXe != null ? hsgdCtu.LoaiXe.ToString().Equals(filter.LoaiXe) : true) &&
                                            (filter.XuatXu != null ? hsgdCtu.XuatXu.Equals(filter.XuatXu) : true) &&
                                            (filter.NamSx != null ? hsgdCtu.NamSx.Equals(filter.Tinh) : true)
                                         )

                                         select new SearchGtttItem
                                         {
                                             PrKey = hsgdCtu.PrKey,
                                             PrKeySerial = hsgdCtu.PrKeySeri,
                                             TenDonvi = _context.DmDonvis.Where(x => x.MaDonvi.Equals(hsgdCtu.MaDonvi)).FirstOrDefault().TenDonvi,
                                             BienKSoat = hsgdCtu.BienKsoat,
                                             SoHsgd = hsgdCtu.SoHsgd,
                                             HieuXe = hieuxe.HieuXe,
                                             LoaiXe = loaixe.LoaiXe,
                                             NamSx = hsgdCtu.NamSx,
                                             XuatXu = hsgdCtu.XuatXu ?? "",
                                             NgayDuyet = hsgdCtu.NgayDuyet,
                                             SoTienThucTe = hsgdCtu.SoTienThucTe
                                         }

                         ).AsNoTracking().AsQueryable();

                    SearchGtttResponse searchResult = new SearchGtttResponse
                    {
                        Count = list_hsgd_ctu.Count(),
                        Data = list_hsgd_ctu.OrderByDescending(x => x.SoHsgd).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
                    };

                    // Request 22/11/2024: Không filter toàn bộ record mà chỉ filter 
                    // LƯU Ý: TẠM THỜI KHÔNG XOÁ BLOCK DƯỚI ĐỂ TRONG TRƯỜNG HỢP CẦN FILTER TOÀN BỘ RECORD CÓ THỂ THỰC HIỆN LUÔN

                    //var itemList = (from item in list_hsgd_ctu
                    //                     where (
                    //                      (filter.sideFilter.TenDonvi != null ? item.TenDonvi.Contains(filter.sideFilter.TenDonvi) : true) &&
                    //                        (filter.sideFilter.BienKSoat != null ? item.BienKSoat.Contains(filter.sideFilter.BienKSoat) : true) &&
                    //                        (filter.sideFilter.SoHsgd != null ? item.SoHsgd.Contains(filter.sideFilter.SoHsgd) : true) &&
                    //                        (filter.sideFilter.HieuXe != null ? item.HieuXe.Contains(filter.sideFilter.HieuXe) : true) &&
                    //                        (filter.sideFilter.LoaiXe != null ? item.LoaiXe.Contains(filter.sideFilter.LoaiXe) : true) &&
                    //                        (filter.sideFilter.NamSx != null ? item.NamSx.Equals(filter.sideFilter.NamSx) : true) &&
                    //                        (filter.sideFilter.XuatXu != null ? item.XuatXu.Equals(filter.sideFilter.XuatXu) : true) &&
                    //                        (filter.sideFilter.NgayDuyet != null ? (item.NgayDuyet != null) && (item.NgayDuyet.Value.Date >= filter.sideFilter.NgayDuyet.Value.Date) : true) &&
                    //                        //(filter.sideFilter.Tinh != null ? item.Tinh.Contains(filter.sideFilter.Tinh) : true) &&
                    //                        (filter.sideFilter.SoTienThucTe != null ? item.SoTienThucTe.ToString().StartsWith(filter.sideFilter.SoTienThucTe.ToString()) : true)
                    //                     )
                    //                     select new SearchGtttItem
                    //                     {
                    //                         PrKey = item.PrKey,
                    //                         PrKeySerial = item.PrKeySerial,
                    //                         TenDonvi = item.TenDonvi,
                    //                         BienKSoat = item.BienKSoat,
                    //                         SoHsgd = item.SoHsgd,
                    //                         HieuXe = item.HieuXe,
                    //                         LoaiXe = item.LoaiXe,
                    //                         NamSx = item.NamSx,
                    //                         XuatXu = item.XuatXu,
                    //                         NgayDuyet = item.NgayDuyet,
                    //                         //Tinh = item.Tinh,
                    //                         SoTienThucTe = item.SoTienThucTe
                    //                     }
                    //          ).AsQueryable();


                    // Hợp với bảng NVU_BHT_SERI bên PIAS để lấy tên tỉnh.
                    // LƯU Ý: KHÔNG NÊN ĐỂ SQL vào loop như thế này ! Tuy nhiên trong trường hợp này do operation nhẹ, và bảng NvuBhtSeri cũng có rất nhiều record dẫn đến việc join bảng lâu. Để luôn như này để tăng tốc độ.

                    searchResult.Data.ForEach(ctu =>
                    {
                        if (ctu.PrKeySerial != 0)
                        {
                            NvuBhtSeri bht = _context_pias.NvuBhtSeris.Where(x => x.PrKey == ctu.PrKeySerial).FirstOrDefault();
                            if (!String.IsNullOrEmpty(bht.TinhKhach))
                            {
                                DmTinhPIAS tinh = _context_pias.DmTinhPIASes.Where(x => x.MaTinh.Equals(bht.TinhKhach.ToUpper())).FirstOrDefault();
                                ctu.Tinh = tinh.TenTinh;
                            }
                        }
                    }
                    );

                    return searchResult;

                }
                catch (Exception err)
                {
                    return new SearchGtttResponse();
                }
            }
            else
            {
                // User không có quyền xem báo cáo hoặc xuất Excel.
                // Báo count lỗi.
                return new SearchGtttResponse
                {
                    Count = 4040404
                };
            }
        }

        // Báo cáo thu hồi tài sản
        public BCThuHoiTSItemResponse SearchBCThuHoiTS(BCThuHoiTS_Main_Filter filter, int pageNumber, int pageSize, string currentUserEmail)
        {
            var currentUser = _context.DmUsers.FirstOrDefault(u => u.Mail.Equals(currentUserEmail));
            // cấu hình cho phép vai trò được xử lý
            var acceptedRoles = new HashSet<LoaiUserEnum>
            {
                LoaiUserEnum.QuanTriHeThong,
                LoaiUserEnum.QuanTriDonVi,
                LoaiUserEnum.TruongPhong,
                LoaiUserEnum.BanQuanLy,
                LoaiUserEnum.TruongPhongTrungTam,
                LoaiUserEnum.LanhDaoTrungTam,
                LoaiUserEnum.PhoPhongTrungTam,
            };
            // Bước 1: Kiểm tra quyền truy cập với acceptedRoles truyền vào
            if (!HasValidPermission(currentUser, acceptedRoles))
                return new BCThuHoiTSItemResponse();
            try
            {
                var result = new Tuple<List<ThuHoiTSItems>, Int64>(null, 0);
                if (pageNumber == -1 && pageSize == -1)
                {
                    result = GetReportThuHoiTaiSanAsyncV2(filter, currentUser, pageNumber, pageSize);
                }
                else
                {
                    result = GetReportThuHoiTaiSanAsync(filter, currentUser, pageNumber, pageSize);
                }
                BCThuHoiTSItemResponse searchResult = new BCThuHoiTSItemResponse
                {
                    Count = result.Item1.Count(),
                    TotalRecord = result.Item2,
                    Data = result.Item1
                };
                return searchResult;
            }
            catch (Exception err)
            {
                _logger.Information(err, err.Message.ToString());
                return new BCThuHoiTSItemResponse();
            }
        }
        public async Task<ThongKeGDTT_General_Response> ThongKeGDTT(ThongKeGDTT_General_Main_Filter filter, int pageNumber, int pageSize, string currentUserEmail)
        {
            DmUser currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
            int[] acceptedUsers = new int[] { 1, 2, 3, 6, 9, 10, 11 };
            if (currentUser != null && (Array.Exists(acceptedUsers, x => x == currentUser.LoaiUser) || _context.DmUqHstpcs.Where(x => x.MaUserUq.ToLower().Equals(currentUser.Oid.ToString().ToLower())).OrderByDescending(x => x.NgayHl).FirstOrDefault() != null || _context.PquyenCnangs.Where(x => x.MaUser.Equals(currentUser.MaUser) && x.TrangThai == 1).FirstOrDefault().LoaiQuyen.Equals("BAOCAO03")))
            {
                string store = "";
                try
                {
                 
                    // Filter Ngày                    
                    //if (!String.IsNullOrEmpty(filter.TuNgay))
                    //{
                    //    condition += " and convert(date, ngay_ctu,103) >= convert(date,'" + UtilityHelper.ReplaceSqlInjection(filter.TuNgay) + "',103)  ";
                    //}
                    //if (!String.IsNullOrEmpty(filter.DenNgay))
                    //{
                    //    condition += " and convert(date, ngay_ctu,103) <= convert(date,'" + UtilityHelper.ReplaceSqlInjection(filter.DenNgay) + "',103) ";
                    //}
                    ////Ngày PD tờ trình
                    //if (!String.IsNullOrEmpty(filter.TuNgayPDTT))
                    //{
                    //    conditionNgayPDTT += " and convert(date, ngay_pd_tt,103) >= convert(date,'" + UtilityHelper.ReplaceSqlInjection(filter.TuNgayPDTT) + "',103)  ";
                    //}
                    //if (!String.IsNullOrEmpty(filter.DenNgayPDTT))
                    //{
                    //    conditionNgayPDTT += " and convert(date, ngay_pd_tt,103) <= convert(date,'" + UtilityHelper.ReplaceSqlInjection(filter.DenNgayPDTT) + "',103) ";
                    //}
                    ////Ngày PD
                    //if (!String.IsNullOrEmpty(filter.TuNgayDuyettpc))
                    //{
                    //    conditionNgayPD += " and convert(date, ngay_duyettpc,103) >= convert(date,'" + UtilityHelper.ReplaceSqlInjection(filter.TuNgayDuyettpc) + "',103)  ";
                    //}
                    //if (!String.IsNullOrEmpty(filter.DenNgayDuyettpc))
                    //{
                    //    conditionNgayPD += " and convert(date, ngay_duyettpc,103) <= convert(date,'" + UtilityHelper.ReplaceSqlInjection(filter.DenNgayDuyettpc) + "',103) ";
                    //}
                    //// Filter Mã Đơn vị
                    //if (!String.IsNullOrEmpty(filter.MaDonVi))
                    //{
                    //    condition += " and A.ma_donvi IN ('" + UtilityHelper.ReplaceSqlInjection(filter.MaDonVi).Replace(",", "','") + "')";
                    //}
                    //// Filter Mã Đơn vị
                    //if (!String.IsNullOrEmpty(filter.MaDonViTt))
                    //{
                    //    condition += " and A.ma_donvi_tt IN ('" + UtilityHelper.ReplaceSqlInjection(filter.MaDonViTt).Replace(",", "','") + "')";
                    //}

                    //// Filter Mã Tinh Trang
                    //if (!String.IsNullOrEmpty(filter.MaTtrangGd))
                    //{
                    //    condition += " and ma_ttrang_gd IN ('" + UtilityHelper.ReplaceSqlInjection(filter.MaTtrangGd).Replace(",", "','") + "')";
                    //}

                    //// Filter Mã Loại hồ sơ
                    //if (!String.IsNullOrEmpty(filter.LoaiHsgd))
                    //{
                    //    condition += " and ma_lhsbt IN (" + UtilityHelper.ReplaceSqlInjection(filter.LoaiHsgd).Replace("0","") + ")";
                    //}
                    //// Filter mã cán bộ
                    //if (!String.IsNullOrEmpty(filter.maCanBo))
                    //{
                    //    string maCanBoList = UtilityHelper.ReplaceSqlInjection(filter.maCanBo).Replace(",","','").ToLower();
                    //    condition += " and LOWER(ma_user) IN ('" + maCanBoList + "')";
                    //}
                    //// Filter Số HSGD
                    //if (!String.IsNullOrEmpty(filter.SoHsgd))
                    //{
                    //    condition += " and so_hsgd like ('" + UtilityHelper.ReplaceSqlInjection(filter.SoHsgd) + "')";
                    //}

                    //// Filter Số Ấn chỉ
                    //if (!String.IsNullOrEmpty(filter.SoAnChi))
                    //{
                    //    condition += " and so_seri like ('" + UtilityHelper.ReplaceSqlInjection(filter.SoAnChi) + "')";
                    //}

                    //// Filter BKS
                    //if (!String.IsNullOrEmpty(filter.BienKSoat))
                    //{
                    //    condition += filter + " and replace(replace(replace(upper(bien_ksoat),'-',''),'.',''),' ','') like '%" + UtilityHelper.ReplaceSqlInjection(filter.BienKSoat).Replace(" ", "").Replace("-", "").Replace(".", "") + "%'";
                    //}

                    //// Loại hồ sơ (TPC / DPC)
                    //if (filter.IsTPC != null)
                    //{
                    //    condition += " and hsgd_tpc = " + filter.IsTPC + "";
                    //}

                    //PiasSoapSoap ws = new PiasSoapSoapClient(ServiceReference1.PiasSoapSoapClient.EndpointConfiguration.PiasSoapSoap, _configuration["client:endpoint:address"]);
                    //var client = new ServiceReference1.PiasSoapSoapClient(ServiceReference1.PiasSoapSoapClient.EndpointConfiguration.PiasSoapSoap, _configuration["client:endpoint:address"]);
                    //client.Endpoint.Binding.SendTimeout = new TimeSpan(0, 20, 0);
                    //client.InnerChannel.OperationTimeout = new TimeSpan(20, 20, 20);
                    ////var result = await ws.BCGDTT_CTIAsync(condition);
                    //var result = await client.BCGDTT_CTIAsync(condition, conditionNgayPDTT, conditionNgayPD, currentUserEmail);
                    //_logger.Information("Result from SOAP : Bao Cao 03: " + result);
                    //DataSet reports = ConvetXMLToDataset(result);
                    //ThongKeGDTT_General_Response returningResult = new ThongKeGDTT_General_Response
                    //{
                    //    Count = reports.Tables[0].Rows.Count,
                    //    Data = JsonConvert.SerializeObject(reports.Tables[0]),
                    //};

                    store = "EXEC BCGDTT_CTI @email_run='"+ currentUserEmail+"'";
                    if (!String.IsNullOrEmpty(filter.SoHsgd))
                    {
                        store += ",@SoHsgd='" + filter.SoHsgd + "'";
                    }
                    if (!String.IsNullOrEmpty(filter.TuNgay))
                    {
                        store += ", @TuNgay='" + filter.TuNgay + "'";
                    }
                    if (!String.IsNullOrEmpty(filter.DenNgay))
                    {
                        store += ", @DenNgay='" + filter.DenNgay + "'";                        
                    }
                    ////Ngày PD tờ trình
                    if (!String.IsNullOrEmpty(filter.TuNgayPDTT))
                    {
                        store += ", @TuNgayPDTT='" + filter.TuNgayPDTT + "'";                        
                    }
                    if (!String.IsNullOrEmpty(filter.DenNgayPDTT))
                    {
                        store += ", @DenNgayPDTT='" + filter.DenNgayPDTT + "'";                        
                    }
                    ////Ngày PD
                    if (!String.IsNullOrEmpty(filter.TuNgayDuyettpc))
                    {
                        store += ", @TuNgayDuyettpc='" + filter.TuNgayDuyettpc + "'";                        
                    }
                    if (!String.IsNullOrEmpty(filter.DenNgayDuyettpc))
                    {
                        store += ", @DenNgayDuyettpc='" + filter.DenNgayDuyettpc + "'";
                       
                    }
                    //// Filter Mã Đơn vị
                    if (!String.IsNullOrEmpty(filter.MaDonVi))
                    {
                        store += ", @MaDonVi ='"+filter.MaDonVi + "'";
                    }
                    //// Filter Mã Đơn vị
                    if (!String.IsNullOrEmpty(filter.MaDonViTt))
                    {
                        store += ", @MaDonViTt ='" +filter.MaDonViTt + "'";
                    }

                    //// Filter Mã Tinh Trang
                    if (!String.IsNullOrEmpty(filter.MaTtrangGd))
                    {
                        store += ", @MaTtrangGd = '" + filter.MaTtrangGd + "'";
                    }

                    //// Filter Mã Loại hồ sơ
                    if (!String.IsNullOrEmpty(filter.LoaiHsgd))
                    {
                        store += ", @LoaiHsgd ='" + filter.LoaiHsgd.Replace("0", "") + "'";
                    }
                    //// Filter mã cán bộ
                    if (!String.IsNullOrEmpty(filter.maCanBo))
                    {
                        store += ", @maCanBo ='" + filter.maCanBo.ToLower() + "'";
                    }

                    //// Filter Số Ấn chỉ
                    if (!String.IsNullOrEmpty(filter.SoAnChi))
                    {
                        store += ", @SoAnChi='" + filter.SoAnChi + "'";
                    }

                    //// Filter BKS
                    if (!String.IsNullOrEmpty(filter.BienKSoat))
                    {
                        store +=", @BienKSoat ='" + filter.BienKSoat.Replace(" ", "").Replace("-", "").Replace(".", "") + "'";
                    }

                    //// Loại hồ sơ (TPC / DPC)
                    if (filter.IsTPC != null)
                    {
                        store += ", @IsTPC = " + filter.IsTPC + "";
                    }


                    _context.Database.SetCommandTimeout(TimeSpan.FromMinutes(20));
                    var results = await _context.Set<ThongKeGDTT_Item>().FromSqlRaw(store).ToListAsync();
                    //var results = await _context.Database.SqlQueryRaw<ThongKeGDTT_Item>(store).ToListAsync();

                    ThongKeGDTT_General_Response returningResult = new ThongKeGDTT_General_Response
                    {
                        Count = results.Count,
                        Data = JsonConvert.SerializeObject(results),
                    };

                    return returningResult;                    

                }
                catch (Exception ex)
                {
                    _logger.Information("Error when running BC03 store= " + store +" ex=" + ex.Message);
                    if (ex.InnerException != null)
                        _logger.Information("Chi tiết: " + ex.InnerException.Message);
                    return null;
                }
            }
            else
            {
                _logger.Information("User không được phân quyền chạy BC03");
                return null;
            }
        }



        public static DataSet ConvetXMLToDataset(ArrayOfXElement ds_xml)
        {

            DataSet ds = new DataSet();
            try
            {
                var strSchema = ds_xml.Nodes[0].ToString();
                var strData = ds_xml.Nodes[1].ToString();
                var strXml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n\t<DataSet>";
                strXml += strSchema + strData;
                strXml += "</DataSet>";
                ds.ReadXml(new MemoryStream(Encoding.UTF8.GetBytes(strXml)));
            }
            catch (Exception)
            {
            }
            return ds;
        }
        // check quyền user trong BCThuHoiTS
        private bool HasValidPermission(DmUser currentUser, HashSet<LoaiUserEnum> acceptedRoles)
        {
            if (acceptedRoles == null || !acceptedRoles.Any())
                return false;
            return currentUser != null && acceptedRoles.Contains((LoaiUserEnum)currentUser.LoaiUser);
        }

        /// <summary>
        /// Chuẩn bị các tham số SQL và cập nhật filter theo đầu vào.
        /// </summary>
        /// <param name="filter">Bộ lọc đầu vào từ người dùng</param>
        /// <param name="currentUser">Người dùng hiện tại để lấy thông tin đơn vị</param>
        /// <param name="pageNumber">Số trang phân trang</param>
        /// <param name="pageSize">Kích thước mỗi trang</param>
        /// <returns>Tuple chứa danh sách tham số SQL và bộ lọc đã xử lý</returns>
        private Tuple<List<SqlParameter>, BCThuHoiTS_Main_Filter> PrepareSqlParametersAndFilter(
            BCThuHoiTS_Main_Filter filter,
            DmUser currentUser,
            int pageNumber,
            int pageSize)
        {
            var parameters = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(filter.MaDonVi))
            {
                parameters.Add(new SqlParameter("@MaDonVi", filter.MaDonVi));
            }
            else
            {
                filter.MaDonVi = string.IsNullOrEmpty(currentUser.MaDonviPquyen) ? currentUser.MaDonvi : currentUser.MaDonviPquyen;
                parameters.Add(new SqlParameter("@MaDonVi", filter.MaDonVi));
            }

            if (!string.IsNullOrEmpty(filter.TuNgay))
                parameters.Add(new SqlParameter("@TuNgay", filter.TuNgay));
            else
                parameters.Add(new SqlParameter("@TuNgay", DBNull.Value));

            if (!string.IsNullOrEmpty(filter.DenNgay))
                parameters.Add(new SqlParameter("@DenNgay", filter.DenNgay));
            else
                parameters.Add(new SqlParameter("@DenNgay", DBNull.Value));

            if (!string.IsNullOrEmpty(filter.SoDonBH))
                parameters.Add(new SqlParameter("@SoDonBH", filter.SoDonBH));
            else
                parameters.Add(new SqlParameter("@SoDonBH", DBNull.Value));

            if (!string.IsNullOrEmpty(filter.BienKSoat))
            {
                var processedBienKsoat = filter.BienKSoat.ToUpper().Replace(" ", "").Replace("-", "").Replace(".", "").Trim();
                var prKeys = _context.HsgdCtus
                    .Where(h => h.BienKsoat != null &&
                                h.BienKsoat.Replace("-", "").Replace(".", "").Replace(" ", "").ToUpper().Contains(processedBienKsoat))
                    .Select(h => h.PrKey)
                    .ToList();
                string prKeysString = string.Join("','", prKeys.Select(key => key.ToString()));
                filter.BienKSoat = $"'{prKeysString}'";
            }

            if (!string.IsNullOrEmpty(filter.MaTtrangGd))
                parameters.Add(new SqlParameter("@MaTtrangGd", filter.MaTtrangGd));
            else
                parameters.Add(new SqlParameter("@MaTtrangGd", DBNull.Value));

            if (!string.IsNullOrEmpty(filter.LoaiHsgd))
                parameters.Add(new SqlParameter("@LoaiHsgd", filter.LoaiHsgd.Replace("0", "")));
            else
                parameters.Add(new SqlParameter("@LoaiHsgd", DBNull.Value));

            if (!string.IsNullOrEmpty(filter.SoHsgd))
                parameters.Add(new SqlParameter("@SoHsgd", filter.SoHsgd));
            else
                parameters.Add(new SqlParameter("@SoHsgd", DBNull.Value));

            if (!string.IsNullOrEmpty(filter.SoAnChi))
            {
                var result = _context.HsgdCtus
                .Where(h => h.SoSeri.ToString().Contains(filter.SoAnChi.Trim()))
                .Select(h => h.PrKey)
                .ToList();
                string prKeysString = string.Join("','", result.Select(key => key.ToString()));
                filter.SoAnChi = $"'{prKeysString}'";
            }
            if (pageNumber > -1 && pageSize > -1)
            {
                parameters.Add(new SqlParameter("@Offset", (pageNumber - 1) * pageSize));
                parameters.Add(new SqlParameter("@PageSize", pageSize));
            }
            return new Tuple<List<SqlParameter>, BCThuHoiTS_Main_Filter>(parameters, filter);
        }

        /// <summary>
        /// Xây dựng danh sách điều kiện WHERE động dựa trên các giá trị trong filter.
        /// </summary>
        /// <param name="filter">Bộ lọc đầu vào từ người dùng</param>
        /// <returns>Danh sách chuỗi điều kiện SQL</returns>
        private List<string> BuildDynamicWhereConditions(BCThuHoiTS_Main_Filter filter)
        {
            var conditions = new List<string>();

            if (!string.IsNullOrEmpty(filter.MaDonVi))
            {
                conditions.Add(filter.MaDonVi.Contains(",")
                    ? $"A.ma_donvi IN (" + string.Join(",", filter.MaDonVi.Split(',').Select(s => $"'{s.Trim()}'")) + ")"
                    : $"A.ma_donvi = @MaDonVi");
            }

            if (!string.IsNullOrEmpty(filter.TuNgay))
                conditions.Add("A.ngay_ctu >= CONVERT(smalldatetime, @TuNgay, 120)");

            if (!string.IsNullOrEmpty(filter.DenNgay))
                conditions.Add("A.ngay_ctu <= CONVERT(smalldatetime, @DenNgay, 120)");

            if (!string.IsNullOrEmpty(filter.SoDonBH))
                conditions.Add("A.so_donbh LIKE '%' + @SoDonBH + '%'");

            if (!string.IsNullOrEmpty(filter.BienKSoat))
                conditions.Add($"A.pr_key IN ({filter.BienKSoat})");

            if (!string.IsNullOrEmpty(filter.MaTtrangGd))
                conditions.Add(filter.MaTtrangGd.Contains(",")
                ? $"A.ma_ttrang_gd IN (" + string.Join(",", filter.MaTtrangGd.Split(',').Select(s => $"'{s.Trim()}'")) + ")"
                : $"A.ma_ttrang_gd = @MaTtrangGd");
            else
                conditions.Add("A.ma_ttrang_gd <> '7'");
            if (!string.IsNullOrEmpty(filter.LoaiHsgd))
                conditions.Add("CHARINDEX(',' + CAST(A.ma_lhsbt AS VARCHAR) + ',', ',' + @LoaiHsgd + ',') > 0");

            if (!string.IsNullOrEmpty(filter.SoHsgd))
                conditions.Add("A.so_hsgd LIKE '%' + @SoHsgd + '%'");

            if (!string.IsNullOrEmpty(filter.SoAnChi))
                conditions.Add($"A.pr_key IN ({filter.SoAnChi})");

            return conditions;
        }

        /// <summary>
        /// Lấy danh sách báo cáo thu hồi tài sản theo điều kiện lọc.
        /// </summary>
        /// <param name="filter">Bộ lọc đầu vào từ người dùng</param>
        /// <param name="currentUser">Thông tin người dùng hiện tại</param>
        /// <param name="pageNumber">Số trang</param>
        /// <param name="pageSize">Số lượng bản ghi trên mỗi trang</param>
        /// <returns>Danh sách kết quả báo cáo thu hồi tài sản</returns>
        public Tuple<List<ThuHoiTSItems>, Int64> GetReportThuHoiTaiSanAsync(BCThuHoiTS_Main_Filter filter, DmUser currentUser, int pageNumber, int pageSize)
        {
            // Giá trị mặc định khi lỗi
            var defaultResult = new Tuple<List<ThuHoiTSItems>, long>(new List<ThuHoiTSItems>(), 0);
            try
            {
                var resultTuple = PrepareSqlParametersAndFilter(filter, currentUser, pageNumber, pageSize);
                filter = resultTuple.Item2;
                var whereConditions = BuildDynamicWhereConditions(filter);
                //Do đặc thù của 2 VP, VPPN chỉ lấy C.thu_hoi_ts=1, VPPB thì lấy hết: (currentUser.MaDonvi=="32"? " AND C.thu_hoi_ts=1" : " ") +
                var whereClause = (whereConditions.Count > 0 ? " AND " + string.Join(" AND ", whereConditions) : "");
                var pagination = BuildPaginationClause();
                string sqlTotalCount = BuildThuHoiTaiSanTotalRowQuery(whereClause);
                string sqlQuery = BuildThuHoiTaiSanQuery(whereClause, pagination);
                var resultDataTable = ExecuteSqlQueryMulti(sqlTotalCount, sqlQuery, resultTuple.Item1);
                var dataTable = resultDataTable?.Item1;
                var counter = resultDataTable?.Item2 ?? 0;

                // Validate datatable
                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    _logger.Information("Không có dữ liệu trả về từ truy vấn.");
                    return defaultResult;
                }
                // Parse dữ liệu
                var thuHoiTSItems = ParseDataTableToThuHoiTSItems(dataTable);

                if (thuHoiTSItems == null)
                {
                    _logger.Information("ParseDataTableToThuHoiTSItems trả về null.");
                    return defaultResult;
                }
                return new Tuple<List<ThuHoiTSItems>, Int64>(thuHoiTSItems, counter);
            }
            catch (InvalidCastException ex)
            {
                _logger.Error("InvalidCastException: " + ex.Message, ex);
                return defaultResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception: " + ex.Message, ex);
                return defaultResult;
            }
        }
        public Tuple<List<ThuHoiTSItems>, Int64> GetReportThuHoiTaiSanAsyncV2(
       BCThuHoiTS_Main_Filter filter,
       DmUser currentUser,
       int pageNumber,
       int pageSize)
        {
            // Giá trị mặc định khi lỗi
            var defaultResult = new Tuple<List<ThuHoiTSItems>, long>(new List<ThuHoiTSItems>(), 0);
            try
            {
                var resultTuple = PrepareSqlParametersAndFilter(filter, currentUser, pageNumber, pageSize);
                filter = resultTuple.Item2;
                var whereConditions = BuildDynamicWhereConditions(filter);
                // (currentUser.MaDonvi == "32" ? " AND C.thu_hoi_ts=1" : " ")
                var whereClause = (whereConditions.Count > 0 ? " AND " + string.Join(" AND ", whereConditions) : "");
                string sqlQuery = BuildThuHoiTaiSanQuery(whereClause, "");
                var resultDataTable = ExecuteSqlQueryMultiV2(sqlQuery, resultTuple.Item1);
                var dataTable = resultDataTable?.Item1;
                var counter = resultDataTable?.Item2 ?? 0;

                // Validate datatable
                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    _logger.Information("Không có dữ liệu trả về từ truy vấn.");
                    return defaultResult;
                }
                // Parse dữ liệu
                var thuHoiTSItems = ParseDataTableToThuHoiTSItems(dataTable);

                if (thuHoiTSItems == null)
                {
                    _logger.Information("ParseDataTableToThuHoiTSItems trả về null.");
                    return defaultResult;
                }
                return new Tuple<List<ThuHoiTSItems>, Int64>(thuHoiTSItems, counter);
            }
            catch (InvalidCastException ex)
            {
                _logger.Error("InvalidCastException: " + ex.Message, ex);
                return defaultResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception: " + ex.Message, ex);
                return defaultResult;
            }
        }
        /// <summary>
        /// Xây dựng câu truy vấn SQL động cho báo cáo thu hồi tài sản.
        /// </summary>
        /// <param name="whereClause">Chuỗi điều kiện WHERE đã được xây dựng</param>
        /// <returns>Chuỗi SQL hoàn chỉnh</returns>
        private string BuildThuHoiTaiSanQuery(string whereClause, string pagination)
        {

            return $@"
            SELECT 
            1 AS so_luong,
            '' AS trong_luong,
            CASE WHEN D.thu_hoi_ts = 1 OR D.so_tienpd_doitru != 0 THEN '' ELSE 'x' END AS khong_thuhoi,
            CASE WHEN D.thu_hoi_ts = 1 THEN 'x' ELSE '' END AS thuhoi_cho_thanhly,
            ISNULL(CAST(
                CASE WHEN D.so_tienpd_doitru != 0 THEN D.so_tienpd_doitru ELSE 0 END
            AS DECIMAL(18,2)), 0) AS doi_tru,
            '' AS luukho,
            D.ngay_capnhat,D.ghi_chudv
            ,CASE
                WHEN D.hsgd_tpc_ = 1 AND D.ma_donvigd IN ('31', '32') AND DATEDIFF(DAY, '2022-04-12', D.ngay_ctu) > 0
                    THEN CAST(D.pr_key AS NVARCHAR(20))
                WHEN D.hsgd_tpc_ = 1 AND D.ma_donvigd IN ('31', '32') AND DATEDIFF(DAY, '2022-04-12', D.ngay_ctu) <= 0
                    THEN CAST(D.pr_key AS NVARCHAR(20))
                WHEN D.hsgd_tpc_ = 1 AND D.ma_donvigd NOT IN ('31', '32')
                    THEN CAST(D.pr_key AS NVARCHAR(20))
                WHEN D.hsgd_tpc_ = 0
                    THEN CAST(D.pr_key AS NVARCHAR(20))
            END AS pr_key,
            D.pr_key_old,
            D.ten_donvi,
            D.ten_khach,
            D.so_donbh,
            D.so_hsgd,
            D.so_seri,
            D.bien_ksoat,
            D.hieu_xe,
            D.loai_xe,
            D.hmuc AS ten_hmuc,
            D.ngay_dau_seri,
            D.ngay_cuoi_seri,
            D.ngay_ctu,
            D.gdv,
            D.tinh_trang,
            D.ma_lhsbt,
            D.hsgd_tpc,
            ISNULL(CAST(
                CASE 
                    WHEN D.ma_ttrang_gd IN ('0','1','2','3') THEN D.so_tienugd
                    WHEN D.ma_ttrang_gd IN ('4','5','6','7','8','9','10','11') THEN
                        (SELECT ISNULL(SUM(so_tientt), 0) + ISNULL(SUM(so_tienph), 0) + ISNULL(SUM(so_tienson), 0)
                         FROM hsgd_dx WITH(NOLOCK) WHERE pr_key_dx = D.pr_key_dx AND ma_hmuc = D.ma_hmuc)
                    ELSE 0
                END AS DECIMAL(18,2)
            ), 0) AS so_tienugddx,
            ISNULL(CAST((
                SELECT ISNULL(SUM(so_tienpdtt), 0) + ISNULL(SUM(so_tienpdsc), 0)
                FROM hsgd_dx WITH(NOLOCK) WHERE pr_key_dx = D.pr_key_dx AND ma_hmuc = D.ma_hmuc
            ) AS DECIMAL(18,2)), 0) AS tien_pheduyet
        FROM (
            SELECT 
                A.so_tienugd, A.ma_ttrang_gd, A.pr_key,
                (SELECT ten_donvi FROM dm_donvi WITH(NOLOCK) WHERE ma_donvi = A.ma_donvi) AS ten_donvi,
                A.ten_khach, A.so_donbh, A.so_hsgd, A.so_seri, A.bien_ksoat,
                (SELECT hieu_xe FROM dm_hieuxe WITH(NOLOCK) WHERE pr_key = B.hieu_xe) AS hieu_xe,
                (SELECT loai_xe FROM dm_loaixe WITH(NOLOCK) WHERE pr_key = B.loai_xe) AS loai_xe,
                A.ngay_dau_seri, A.ngay_cuoi_seri, A.ngay_ctu,
                (SELECT CASE WHEN loai_user=4 THEN ten_user +'(GĐV)' WHEN loai_user=7 THEN ten_user +'(CBKD)' ELSE ten_user END
                 FROM dm_user WITH(NOLOCK) WHERE oid = A.ma_user) AS gdv,
                (SELECT ten_ttrang_gd FROM dm_ttrang_gd WITH(NOLOCK) WHERE ma_ttrang_gd = A.ma_ttrang_gd) AS tinh_trang,
                CASE WHEN A.ma_lhsbt=1 THEN N'Tự giám định'
                     WHEN A.ma_lhsbt=2 THEN N'Nhờ giám định'
                     ELSE N'Giám định hộ' END AS ma_lhsbt,
                CASE WHEN A.hsgd_tpc=1 THEN N'Hồ sơ TPC' ELSE N'Hồ sơ DPC' END AS hsgd_tpc,
                A.ma_donvigd, A.hsgd_tpc AS hsgd_tpc_,C.pr_key_dx,C.ma_hmuc,C.thu_hoi_ts,C.pr_key AS pr_key_old,C.so_tienpd_doitru, C.ngay_capnhat,C.ghi_chudv,iif(C.hmuc='',(select top 1 ten_hmuc from dm_hmuc with (nolock) where ma_hmuc=C.ma_hmuc),C.hmuc) hmuc
            FROM hsgd_ctu A WITH(NOLOCK) inner join hsgd_dx_ct B WITH(NOLOCK) on A.pr_key_bt=B.pr_key_hsbt_ctu inner join hsgd_dx C  WITH(NOLOCK) on B.pr_key=C.pr_key_dx
            WHERE C.so_tientt<>0 and C.pr_key in ( select pr_key from hsgd_dx WITH(NOLOCK) where fr_key=0) {whereClause}
	        union all
	         SELECT 
                A.so_tienugd, A.ma_ttrang_gd, A.pr_key,
                (SELECT ten_donvi FROM dm_donvi WITH(NOLOCK) WHERE ma_donvi = A.ma_donvi) AS ten_donvi,
                A.ten_khach, A.so_donbh, A.so_hsgd, A.so_seri, A.bien_ksoat,
                (SELECT hieu_xe FROM dm_hieuxe WITH(NOLOCK) WHERE pr_key = A.hieu_xe) AS hieu_xe,
                (SELECT loai_xe FROM dm_loaixe WITH(NOLOCK) WHERE pr_key = A.loai_xe) AS loai_xe,
                A.ngay_dau_seri, A.ngay_cuoi_seri, A.ngay_ctu,
                (SELECT CASE WHEN loai_user=4 THEN ten_user +'(GĐV)' WHEN loai_user=7 THEN ten_user +'(CBKD)' ELSE ten_user END
                 FROM dm_user WITH(NOLOCK) WHERE oid = A.ma_user) AS gdv,
                (SELECT ten_ttrang_gd FROM dm_ttrang_gd WITH(NOLOCK) WHERE ma_ttrang_gd = A.ma_ttrang_gd) AS tinh_trang,
                CASE WHEN A.ma_lhsbt=1 THEN N'Tự giám định'
                     WHEN A.ma_lhsbt=2 THEN N'Nhờ giám định'
                     ELSE N'Giám định hộ' END AS ma_lhsbt,
                CASE WHEN A.hsgd_tpc=1 THEN N'Hồ sơ TPC' ELSE N'Hồ sơ DPC' END AS hsgd_tpc,
                A.ma_donvigd, A.hsgd_tpc AS hsgd_tpc_,C.pr_key_dx,C.ma_hmuc,C.thu_hoi_ts,C.pr_key AS pr_key_old,C.so_tienpd_doitru, C.ngay_capnhat,C.ghi_chudv,iif(C.hmuc='',(select top 1 ten_hmuc from dm_hmuc with (nolock) where ma_hmuc=C.ma_hmuc),C.hmuc) hmuc
            FROM hsgd_ctu A WITH(NOLOCK) inner join hsgd_dx C  WITH(NOLOCK) on A.pr_key=C.fr_key
            WHERE C.so_tientt<>0 and C.pr_key  in ( select pr_key from hsgd_dx WITH(NOLOCK) where fr_key<>0) {whereClause} 
        ) D  ORDER BY D.pr_key_old DESC,D.hmuc {pagination}";
        }
        private string BuildPaginationClause()
        {
            return "OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        }
        private string BuildThuHoiTaiSanTotalRowQuery(string whereClause)
        {
            return $@"
                SELECT 
                    count(*) as counter
                FROM (
                    SELECT 
                        A.so_tienugd, A.ma_ttrang_gd, A.pr_key,
                        A.ma_donvi AS ten_donvi,
                        A.ten_khach, A.so_donbh, A.so_hsgd, A.so_seri, A.bien_ksoat,                        
                        A.ngay_dau_seri, A.ngay_cuoi_seri, A.ngay_ctu,
                        A.ma_user AS gdv,
                        A.ma_ttrang_gd AS tinh_trang,
                        A.ma_lhsbt AS ma_lhsbt,
                        A.hsgd_tpc AS hsgd_tpc,
                        A.ma_donvigd, A.hsgd_tpc AS hsgd_tpc_
                    FROM hsgd_ctu A WITH(NOLOCK) inner join hsgd_dx_ct B WITH(NOLOCK) on A.pr_key_bt=B.pr_key_hsbt_ctu inner join hsgd_dx C  WITH(NOLOCK) on B.pr_key=C.pr_key_dx
                    WHERE C.so_tientt<>0 and C.pr_key in ( select pr_key from hsgd_dx WITH(NOLOCK) where fr_key=0) {whereClause}
                    union all
					SELECT 
                        A.so_tienugd, A.ma_ttrang_gd, A.pr_key,
                        A.ma_donvi AS ten_donvi,
                        A.ten_khach, A.so_donbh, A.so_hsgd, A.so_seri, A.bien_ksoat,                        
                        A.ngay_dau_seri, A.ngay_cuoi_seri, A.ngay_ctu,
                        A.ma_user AS gdv,
                        A.ma_ttrang_gd AS tinh_trang,
                        A.ma_lhsbt AS ma_lhsbt,
                        A.hsgd_tpc AS hsgd_tpc,
                        A.ma_donvigd, A.hsgd_tpc AS hsgd_tpc_
                    FROM hsgd_ctu A WITH(NOLOCK) inner join hsgd_dx C  WITH(NOLOCK) on A.pr_key=C.fr_key
                    WHERE C.so_tientt<>0 and C.pr_key in ( select pr_key from hsgd_dx WITH(NOLOCK) where fr_key<>0) {whereClause}
                ) B ";
        }
        private Tuple<System.Data.DataTable, Int64> ExecuteSqlQueryMulti(string sqlCount, string sqlData, List<SqlParameter> parameters)
        {
            var dataTable = new System.Data.DataTable();
            long totalCount = 0;
            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    connection.Open();

                    // 1. Chạy truy vấn COUNT
                    using (var countCmd = connection.CreateCommand())
                    {
                        countCmd.CommandText = sqlCount;

                        foreach (var p in parameters.Where(p => p.ParameterName != "@Offset" && p.ParameterName != "@PageSize"))
                        {
                            var cloned = new SqlParameter(p.ParameterName, p.SqlDbType)
                            {
                                Value = p.Value ?? DBNull.Value
                            };
                            countCmd.Parameters.Add(cloned);
                        }

                        var countResult = countCmd.ExecuteScalar();
                        totalCount = Convert.ToInt64(countResult);
                    }

                    // 2. Chạy truy vấn SELECT chính
                    using (var dataCmd = connection.CreateCommand())
                    {
                        dataCmd.CommandText = sqlData;

                        foreach (var p in parameters)
                        {
                            var cloned = new SqlParameter(p.ParameterName, p.SqlDbType)
                            {
                                Value = p.Value ?? DBNull.Value
                            };
                            dataCmd.Parameters.Add(cloned);
                        }

                        using (var reader = dataCmd.ExecuteReader())
                        {
                            dataTable.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Information(ex.Message.ToString());
                throw;
            }
            return new Tuple<System.Data.DataTable, Int64>(dataTable, totalCount);
        }
        private Tuple<System.Data.DataTable, Int64> ExecuteSqlQueryMultiV2(string sqlData, List<SqlParameter> parameters)
        {
            var dataTable = new System.Data.DataTable();
            long totalCount = 0;

            using (var connection = _context.Database.GetDbConnection())
            {
                connection.Open();
                // 2. Chạy truy vấn SELECT chính
                using (var dataCmd = connection.CreateCommand())
                {
                    dataCmd.CommandText = sqlData;
                    dataCmd.CommandTimeout = 600; // 600 giây = 20 phút
                    foreach (var p in parameters)
                    {
                        var cloned = new SqlParameter(p.ParameterName, p.SqlDbType)
                        {
                            Value = p.Value ?? DBNull.Value
                        };
                        dataCmd.Parameters.Add(cloned);
                    }

                    using (var reader = dataCmd.ExecuteReader())
                    {
                        dataTable.Load(reader);
                    }
                }
            }

            return new Tuple<System.Data.DataTable, Int64>(dataTable, totalCount);
        }
        public List<ThuHoiTSItems> ParseDataTableToThuHoiTSItems(System.Data.DataTable dataTable)
        {
            var items = new List<ThuHoiTSItems>();

            foreach (DataRow row in dataTable.Rows)
            {
                var item = new ThuHoiTSItems
                {
                    SoLuong = row["so_luong"] != DBNull.Value ? Convert.ToInt32(row["so_luong"]) : 0, // Default 0
                    TrongLuong = row["trong_luong"] != DBNull.Value ? row["trong_luong"].ToString() : string.Empty, // Default empty string
                    KhongThuHoi = row["khong_thuhoi"] != DBNull.Value ? row["khong_thuhoi"].ToString() : string.Empty, // Default empty string
                    ThuHoiChoTL = row["thuhoi_cho_thanhly"] != DBNull.Value ? row["thuhoi_cho_thanhly"].ToString() : string.Empty, // Default empty string
                    DoiTru = row["doi_tru"] != DBNull.Value ? Convert.ToInt64(row["doi_tru"]) : 0, // Nullable decimal
                    LuuKhoTSD = row["luukho"] != DBNull.Value ? row["luukho"].ToString() : string.Empty, // Default empty string
                    NgayCapNhat = row["ngay_capnhat"] != DBNull.Value ? Convert.ToDateTime(row["ngay_capnhat"]) : (DateTime?)null, // Nullable DateTime
                    GhiChu = row["ghi_chudv"] != DBNull.Value ? row["ghi_chudv"].ToString() : string.Empty, // Default empty string
                    PrKey = row["pr_key"] != DBNull.Value ? row["pr_key"].ToString() : string.Empty, // Default empty string
                    PrKeyOld = row["pr_key_old"] != DBNull.Value ? Convert.ToInt32(row["pr_key_old"]) : 0, // Default 0
                    TenDonVi = row["ten_donvi"] != DBNull.Value ? row["ten_donvi"].ToString() : string.Empty, // Default empty string
                    TenKhach = row["ten_khach"] != DBNull.Value ? row["ten_khach"].ToString() : string.Empty, // Default empty string
                    SoDonBH = row["so_donbh"] != DBNull.Value ? row["so_donbh"].ToString() : string.Empty, // Default empty string
                    SoHsgd = row["so_hsgd"] != DBNull.Value ? row["so_hsgd"].ToString() : string.Empty, // Default empty string
                    SoSeri = row["so_seri"] != DBNull.Value ? row["so_seri"].ToString() : string.Empty, // Default empty string
                    BienKSoat = row["bien_ksoat"] != DBNull.Value ? row["bien_ksoat"].ToString() : string.Empty, // Default empty string
                    HieuXe = row["hieu_xe"] != DBNull.Value ? row["hieu_xe"].ToString() : string.Empty, // Default empty string
                    LoaiXe = row["loai_xe"] != DBNull.Value ? row["loai_xe"].ToString() : string.Empty, // Default empty string
                    VatTuTH = row["ten_hmuc"] != DBNull.Value ? row["ten_hmuc"].ToString() : string.Empty, // Default empty string
                    NgayDauSeri = row["ngay_dau_seri"] != DBNull.Value ? Convert.ToDateTime(row["ngay_dau_seri"]) : (DateTime?)null, // Nullable DateTime
                    NgayCuoiSeri = row["ngay_cuoi_seri"] != DBNull.Value ? Convert.ToDateTime(row["ngay_cuoi_seri"]) : (DateTime?)null, // Nullable DateTime
                    NgayCTU = row["ngay_ctu"] != DBNull.Value ? Convert.ToDateTime(row["ngay_ctu"]) : (DateTime?)null, // Nullable DateTime
                    GDV = row["gdv"] != DBNull.Value ? row["gdv"].ToString() : string.Empty, // Default empty string
                    TinhTrang = row["tinh_trang"] != DBNull.Value ? row["tinh_trang"].ToString() : string.Empty, // Default empty string
                    MaLHSBT = row["ma_lhsbt"] != DBNull.Value ? row["ma_lhsbt"].ToString() : string.Empty, // Default empty string
                    HSGDTpc = row["hsgd_tpc"] != DBNull.Value ? row["hsgd_tpc"].ToString() : string.Empty, // Default empty string
                    SoTienUgdDx = row["so_tienugddx"] != DBNull.Value ? Convert.ToInt64(row["so_tienugddx"]) : 0, // Default 0
                    TienPheDuyet = row["tien_pheduyet"] != DBNull.Value ? Convert.ToInt64(row["tien_pheduyet"]) : 0 // Default 0
                };

                items.Add(item);
            }

            return items;
        }


    }
}