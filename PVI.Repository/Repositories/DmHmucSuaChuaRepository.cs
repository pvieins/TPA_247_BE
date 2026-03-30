using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PVI.DAO.Entities.Models;
using PVI.Repository.Interfaces;
using System.Linq;
using System.Collections.Generic;

namespace PVI.Repository.Repositories
{

    /* Implementation cho interface Hạng mục sửa chữa.
     * lhkhanh - 26/09/2024
     */

    // Class con để trả thông tin của hạng mục.
    public class ListHmuc
    {
        public int count { get; set; }
        public List<DmHmuc> listHmuc { get; set; }
    }

    // Class con để trả thông tin của nhóm hạng mục.
    public class ListNHmuc
    {
        public int count { get; set; }
        public List<DmNhmuc> listNhmuc { get; set; }
    }

    // Class con để trả thông tin của tổng thành xe.
    public class ListTongThanhXe
    {
        public int count { get; set; }
        public List<DmTongthanhxe> listTongThanhXe { get; set; }
    }

    // Kế thừa base.
    public class DmHmucSuaChuaRepository : GenericRepository<DmHmuc>, IDmHmucSuaChuaRepository
    {
        public DmHmucSuaChuaRepository(GdttContext context, Pvs2024Context context_pias, Serilog.ILogger logger, IConfiguration conf) : base(context, context_pias, logger, conf)
        {

        }

        // Tham số: Page & Limit - Dùng để phân trang, có kèm theo các filter.
        public ListHmuc getListHmuc(int pageNumber, int limit, DmHmuc? searchTarget)
        {
            // mã hạng mục trong bảng hsgd_ct

            var list_Hmuc = (from Hmuc in _context.DmHmucs
                             join Nhmuc in _context.DmNhmucs on Hmuc.MaNhmuc equals Nhmuc.MaNhmuc
                             join Tongthanhxe in _context.DmTongthanhxes on Hmuc.MaTongthanhxe equals Tongthanhxe.MaTongthanhxe
                             join User in _context.DmUsers on Hmuc.MaUser equals User.Oid.ToString().ToLower()
                             orderby Hmuc.MaHmuc
                             where (

                             (searchTarget.MaHmuc != null ? Hmuc.MaHmuc.Contains(searchTarget.MaHmuc) : true) &&
                             (searchTarget.TenHmuc != null ? Hmuc.TenHmuc.Contains(searchTarget.TenHmuc) : true) &&
                             (searchTarget.TenNhmuc != null ? Nhmuc.TenNhmuc.Contains(searchTarget.TenNhmuc) : true) &&
                             (searchTarget.TenTongThanhXe != null ? Tongthanhxe.TenTongthanhxe.Contains(searchTarget.TenTongThanhXe) : true) &&
                             (searchTarget.SuDung != null ? Hmuc.SuDung == searchTarget.SuDung.Value : true) &&
                             (searchTarget.TenUser != null ? User.TenUser.Contains(searchTarget.TenUser) : true) &&
                             (searchTarget.NgayCapnhat != null ? Hmuc.NgayCapnhat >= searchTarget.NgayCapnhat : true)
                             )

                             select new DmHmuc
                             {
                                 MaHmuc = Hmuc.MaHmuc,
                                 TenHmuc = Hmuc.TenHmuc,
                                 MaNhmuc = Nhmuc.MaNhmuc,
                                 TenNhmuc = Nhmuc.TenNhmuc,
                                 MaTongthanhxe = Tongthanhxe.MaTongthanhxe,
                                 TenTongThanhXe = Tongthanhxe.TenTongthanhxe,
                                 SuDung = Hmuc.SuDung,
                                 MaUser = Hmuc.MaUser,
                                 TenUser = User.TenUser,
                                 NgayCapnhat = Hmuc.NgayCapnhat
                             }
                        ).ToList();
            ListHmuc toBeReturned = new ListHmuc
            {
                count = (list_Hmuc != null ? list_Hmuc.Count() : 0),
                listHmuc = list_Hmuc.Skip(limit * (pageNumber - 1)).Take(limit).ToList()
            };

            return toBeReturned;
        }


        // Tham số: Page & Limit - Dùng để phân trang, có kèm theo các filter.
        // Có ghép trang khá phức tạp
        // khanhlh - 23/12/2024
        public ListHmuc getListHmuc_HSGD_Anh(int pageNumber, int limit, int pr_key, DmHmuc? searchTarget)
        {
            // mã hạng mục trong bảng hsgd_ct
            var list_ma_hmuc = _context.HsgdCts.Where(x => x.MaHmuc != "" && x.MaHmucSc != null && x.MaHmucSc != "" && x.FrKey == pr_key).OrderBy(g => g.PrKey).Distinct().ToList();

            // Nối trang: Nếu số lượng cần lấy nhỏ hơn lượng hạng mục trong danh sách ảnh, thì nối trang, còn không thì cứ lấy thẳng.  
            int so_luong_can_lay = limit <= list_ma_hmuc.Count() - (limit * (pageNumber - 1)) ? limit : list_ma_hmuc.Count() - limit * (pageNumber - 1);
           
            if (so_luong_can_lay > 0 && so_luong_can_lay <= limit) {

                // Lấy danh sách hạng mục từ HSGD_CT
                var list_Hmuc = (from hmuc_mini in list_ma_hmuc
                                 join Hmuc in _context.DmHmucs on hmuc_mini.MaHmucSc equals Hmuc.MaHmuc
                                 join Nhmuc in _context.DmNhmucs on Hmuc.MaNhmuc equals Nhmuc.MaNhmuc
                                 join Tongthanhxe in _context.DmTongthanhxes on Hmuc.MaTongthanhxe equals Tongthanhxe.MaTongthanhxe
                                 where
                                   (searchTarget.TenHmuc != null ? Hmuc.TenHmuc.Contains(searchTarget.TenHmuc) : true) &&
                                   (searchTarget.TenNhmuc != null ? Nhmuc.TenNhmuc.Contains(searchTarget.TenNhmuc) : true) &&
                                   (searchTarget.TenTongThanhXe != null ? Tongthanhxe.TenTongthanhxe.Contains(searchTarget.TenTongThanhXe) : true)

                                 select new DmHmuc
                                 {
                                     MaHmuc = Hmuc.MaHmuc,
                                     TenHmuc = Hmuc.TenHmuc,
                                     MaNhmuc = Nhmuc.MaNhmuc,
                                     TenNhmuc = Nhmuc.TenNhmuc,
                                     MaTongthanhxe = Tongthanhxe.MaTongthanhxe,
                                     TenTongThanhXe = Tongthanhxe.TenTongthanhxe,
                                 }
                           ).ToList();

                ListHmuc list_tra_ve = new ListHmuc
                {
                    count = _context.DmHmucs.Count(),
                    // Tính toán lượng record
                    listHmuc = list_Hmuc.Skip(so_luong_can_lay == limit ? 0 : list_Hmuc.Count() - so_luong_can_lay).Take(so_luong_can_lay).ToList()
                };

                // Sau đó, nếu số lượng này ít hơn số record trong trang thì bắt đầu nối thêm từ danh sách hạng mục gốc vào
                if (so_luong_can_lay < limit)
                {
                    List<DmHmuc> list_hmuc_goc = (from Hmuc in _context.DmHmucs 
                                                  join Nhmuc in _context.DmNhmucs on Hmuc.MaNhmuc equals Nhmuc.MaNhmuc
                                                  join Tongthanhxe in _context.DmTongthanhxes on Hmuc.MaTongthanhxe equals Tongthanhxe.MaTongthanhxe
                                                  where
                                                   (searchTarget.TenHmuc != null ? Hmuc.TenHmuc.Contains(searchTarget.TenHmuc) : true) &&
                                                   (searchTarget.TenNhmuc != null ? Nhmuc.TenNhmuc.Contains(searchTarget.TenNhmuc) : true) &&
                                                   (searchTarget.TenTongThanhXe != null ? Tongthanhxe.TenTongthanhxe.Contains(searchTarget.TenTongThanhXe) : true)
                                                  select new DmHmuc
                                                  {
                                                      MaHmuc = Hmuc.MaHmuc,
                                                      TenHmuc = Hmuc.TenHmuc,
                                                      MaNhmuc = Nhmuc.MaNhmuc,
                                                      TenNhmuc = Nhmuc.TenNhmuc,
                                                      MaTongthanhxe = Tongthanhxe.MaTongthanhxe,
                                                      TenTongThanhXe = Tongthanhxe.TenTongthanhxe,
                                                  }
                             ).Skip(list_tra_ve.listHmuc.Count()).Take(limit - so_luong_can_lay).ToList();
                    list_tra_ve.listHmuc.AddRange(list_hmuc_goc);
                    return list_tra_ve;
                } else
                {
                    // Không thì trả thẳng
                    return list_tra_ve;
                }
            } else
            {
                ListHmuc list_tra_ve = new ListHmuc
                {
                    count = _context.DmHmucs.Count(),
                    listHmuc = getListHmuc(pageNumber, limit, searchTarget).listHmuc
                };
                return list_tra_ve;
            }

        }

        // Tham số: Page & Limit - Dùng để phân trang, có kèm theo các filter.
        public ListNHmuc getListNHmuc(int pageNumber, int limit, DmNhmuc searchTarget, bool getFull)
        {
            var list_NHmuc = (from NHmuc in _context.DmNhmucs
                              join Tongthanhxe in _context.DmTongthanhxes on NHmuc.MaTongthanhxe equals Tongthanhxe.MaTongthanhxe
                              join User in _context.DmUsers on NHmuc.MaUser equals User.Oid.ToString().ToLower()
                              orderby NHmuc.MaNhmuc
                              where (

                              (searchTarget.MaNhmuc != null ? NHmuc.MaNhmuc.Contains(searchTarget.MaNhmuc) : true) &&
                              (searchTarget.TenNhmuc != null ? NHmuc.TenNhmuc.Contains(searchTarget.TenNhmuc) : true) &&
                              (searchTarget.TenTongThanhXe != null ? Tongthanhxe.TenTongthanhxe.Contains(searchTarget.TenTongThanhXe) : true) &&
                              (searchTarget.SuDung != null ? NHmuc.SuDung == searchTarget.SuDung.Value : true) &&
                              (searchTarget.TenUser != null ? User.TenUser.Contains(searchTarget.TenUser) : true) &&
                              (searchTarget.NgayCapnhat != null ? NHmuc.NgayCapnhat >= searchTarget.NgayCapnhat : true)
                              )

                              select new DmNhmuc
                              {
                                  MaNhmuc = NHmuc.MaNhmuc,
                                  TenNhmuc = NHmuc.TenNhmuc,
                                  MaTongthanhxe = NHmuc.MaTongthanhxe,
                                  TenTongThanhXe = Tongthanhxe.TenTongthanhxe,
                                  SuDung = NHmuc.SuDung,
                                  MaUser = NHmuc.MaUser,
                                  TenUser = User.TenUser,
                                  NgayCapnhat = NHmuc.NgayCapnhat,
                                  DanhSachHmuc = (getFull ? _context.DmHmucs.Where(x => x.MaNhmuc.Equals(NHmuc.MaNhmuc)).OrderBy(x => x.MaHmuc).ToList() : null)
                              }
                        ).ToList();

            ListNHmuc toBeReturned = new ListNHmuc
            {
                count = (list_NHmuc != null ? list_NHmuc.Count() : 0),
                listNhmuc = list_NHmuc.Skip(limit * (pageNumber - 1)).Take(limit).ToList()
            };

            return toBeReturned;
        }

        // Lấy danh sách tổng thành xe.
        // Trường getFull: Có trả về danh sách các nhóm hạng mục thuộc tổng thành xe đó hay không.
        public ListTongThanhXe getListTongThanhXe(bool getFull)
        {
            var list_tong_thanh_xe = (
                              from Tongthanhxe in _context.DmTongthanhxes
                              join User in _context.DmUsers on Tongthanhxe.MaUser equals User.Oid.ToString()
                              orderby Tongthanhxe.MaTongthanhxe
                              select new DmTongthanhxe
                              {
                                  MaTongthanhxe = Tongthanhxe.MaTongthanhxe,
                                  TenTongthanhxe = Tongthanhxe.TenTongthanhxe,
                                  MaUser = Tongthanhxe.MaUser,
                                  TenUser = User.TenUser,
                                  NgayCapnhat = Tongthanhxe.NgayCapnhat,
                                  DanhSachNhmuc = (getFull ? _context.DmNhmucs.Where(x => x.MaTongthanhxe.Equals(Tongthanhxe.MaTongthanhxe)).OrderBy(x => x.MaNhmuc).ToList() : null)
                              }
                        ).ToList();

            ListTongThanhXe toBeReturned = new ListTongThanhXe
            {
                count = (list_tong_thanh_xe != null ? list_tong_thanh_xe.Count() : 0),
                listTongThanhXe = list_tong_thanh_xe,
            };
            return toBeReturned;
        }

        // Tạo nhóm hạng mục.
        public async Task<string> CreateNHmuc(DmNhmuc nhomHangMuc, string currentUserEmail)
        {
            var currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
            // Kiểm tra nhóm hạng mục không tồn tại thì mới tiến hành insert
            var checkExist = _context.DmNhmucs.Where(x => x.MaNhmuc == nhomHangMuc.MaNhmuc).FirstOrDefault();
            if (checkExist == null && currentUser != null)
            {
                try
                {
                    // Tiến hành lưu vào DB.
                    nhomHangMuc.NgayCapnhat = DateTime.Now;
                    nhomHangMuc.MaUser = currentUser.Oid.ToString();
                    _context.DmNhmucs.Add(nhomHangMuc);
                    await _context.SaveChangesAsync();

                    return nhomHangMuc.MaNhmuc.ToString();
                }
                catch (Exception ex)
                {
                    _logger.Error("dbContextTransaction Exception when CreateNHmuc: " + ex.ToString());
                    _logger.Error("Error record: " + JsonConvert.SerializeObject(nhomHangMuc));
                    await _context.DisposeAsync();
                    throw;
                }
            }
            else
            {
                return $"Nhóm hạng mục {nhomHangMuc.MaNhmuc} đã tồn tại.";
            }
        }

        // Tạo hạng mục.
        public async Task<string> CreateHmuc(DmHmuc hangMuc, string currentUserEmail)
        {
            var currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
            // Kiểm tra nhóm hạng mục không tồn tại thì mới tiến hành insert
            var checkExist = _context.DmHmucs.Where(x => x.MaHmuc == hangMuc.MaHmuc).FirstOrDefault();
            if (checkExist == null && currentUser != null)
            {
                try
                {
                    // Tiến hành lưu vào DB.
                    hangMuc.NgayCapnhat = DateTime.Now;
                    hangMuc.MaUser = currentUser.Oid.ToString();
                    _context.DmHmucs.Add(hangMuc);
                    await _context.SaveChangesAsync();

                    return hangMuc.MaHmuc.ToString();
                }
                catch (Exception ex)
                {
                    _logger.Error("dbContextTransaction Exception when CreateHmuc: " + ex.ToString());
                    _logger.Error("Error record: " + JsonConvert.SerializeObject(hangMuc));
                    await _context.DisposeAsync();
                    throw;
                }
            }
            else
            {
                return $"Hạng mục {hangMuc.MaHmuc} đã tồn tại.";
            }
        }
    
        // Tạo nhóm hạng mục.
        public async Task<string> UpdateNHmuc(DmNhmuc nhomHangMuc, string currentUserEmail)
        {
            var currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
            // Kiểm tra nhóm hạng mục không tồn tại thì mới tiến hành insert
            DmNhmuc checkExist = _context.DmNhmucs.Where(x => x.MaNhmuc == nhomHangMuc.MaNhmuc).FirstOrDefault();
            if (checkExist != null && currentUser != null)
            {
                try
                {
                    checkExist.SuDung = nhomHangMuc.SuDung;
                    checkExist.TenNhmuc = nhomHangMuc.TenNhmuc;
                    checkExist.NgayCapnhat = DateTime.Now;
                    checkExist.MaUser = currentUser.Oid.ToString();

                    // Nếu tắt nhóm hạng mục thì toàn bộ hạng mục của nhóm đó cũng sẽ bị tắt theo.
                    if (checkExist.SuDung == 0) {
                        List<DmHmuc> danhSachHmuc = await _context.DmHmucs.Where(x => x.MaNhmuc.Equals(checkExist.MaNhmuc)).ToListAsync();
                        if (danhSachHmuc != null && danhSachHmuc.Count > 0)
                        {
                            danhSachHmuc.ForEach(item =>
                            {
                                item.SuDung = 0;
                                _context.DmHmucs.Update(item);
                            });
                        }
                    }
                   
                    _context.DmNhmucs.Update(checkExist);
                    await _context.SaveChangesAsync();

                    return checkExist.MaNhmuc.ToString();
                }
                catch (Exception ex)
                {
                    _logger.Error("dbContextTransaction Exception when UpdateNHmuc: " + ex.ToString());
                    _logger.Error("Error record: " + JsonConvert.SerializeObject(checkExist));
                    await _context.DisposeAsync();
                    throw;
                }
            }
            else
            {
                return $"Nhóm hạng mục {nhomHangMuc.MaNhmuc} không tồn tại.";
            }
        }

        // Tạo hạng mục.
        public async Task<string> updateHmuc(DmHmuc hangMuc, string currentUserEmail)
        {
            var currentUser = _context.DmUsers.Where(x => x.Mail.Equals(currentUserEmail)).FirstOrDefault();
            // Kiểm tra nhóm hạng mục không tồn tại thì mới tiến hành insert
            var checkExist = _context.DmHmucs.Where(x => x.MaHmuc == hangMuc.MaHmuc).FirstOrDefault();
            if (checkExist != null && currentUser != null)
            {
                try
                {
                    checkExist.TenHmuc = hangMuc.TenHmuc;
                    checkExist.SuDung = hangMuc.SuDung;
                    checkExist.NgayCapnhat = DateTime.Now;
                    checkExist.MaUser = currentUser.Oid.ToString();

                    // Nếu hạng mục con được bật lên thì nhóm hạng mục cũng phải nghiễm nhiên được bật lên.
                    if (checkExist.SuDung == 1)
                    {
                        DmNhmuc nhomHangMucGoc = _context.DmNhmucs.Where(x => x.MaNhmuc.Equals(checkExist.MaNhmuc)).FirstOrDefault();
                        if (nhomHangMucGoc != null)
                        {
                            nhomHangMucGoc.SuDung = 1;
                            _context.DmNhmucs.Update(nhomHangMucGoc);
                        }
                    }

                    _context.DmHmucs.Update(checkExist);
                    await _context.SaveChangesAsync();

                    return checkExist.MaHmuc.ToString();
                }
                catch (Exception ex)
                {
                    _logger.Error("dbContextTransaction Exception when UpdateHmuc: " + ex.ToString());
                    _logger.Error("Error record: " + JsonConvert.SerializeObject(checkExist));
                    await _context.DisposeAsync();
                    throw;
                }
            }
            else
            {
                return $"Hạng mục {hangMuc.MaHmuc} không tồn tại.";
            }
        }



    }
}