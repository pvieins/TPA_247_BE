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
using System.ComponentModel.Design;
using System.Reflection.Metadata.Ecma335;
using PdfSharpCore;
using Microsoft.Extensions.Primitives;
using Microsoft.Identity.Client;

namespace PVI.Repository.Repositories
{

    /* Implementation cho interface lich trực.
     * lhkhanh - 10/09/2024
     */

    // Kế thừa base.
    public class LichtrucgdvRepository : GenericRepository<LichTrucgdv>, ILichTrucGDVRepository
    {
        
        // Danh sách gara đi kèm với ghi chú sẽ trả về danh sách khu vực.
        public class DanhSachLichTruc {
            public string ghiChu { get; set; }
            public List<GaraLichTruc> listGara { get; set; }

            public DanhSachLichTruc()
            {
                
            }
        }

        // Class lấy danh sách lịch trực của 1 gara trong khu vực.
        public class GaraLichTruc
        {
            public string tenGara { get; set; }
            public string maGara { get; set; }
            public List<LichTrucgdv> listLichTruc { get; set; }
        }

        public LichtrucgdvRepository(GdttContext context, Pvs2024Context context_pias, Serilog.ILogger logger, IConfiguration conf) : base(context, context_pias, logger, conf)
        {

        }

        // Các đầu GET:
        public List<DmKhuvuc> getListKhuVuc()
        {
            return _context.DmKhuvucs.ToList();
        }

        // Gara khu vực
        public List<DmGaraKhuvuc> getListGaraKhuVuc(string ma_kv)
        {
            return _context.DmGaraKhuvucs.Where(x=>x.MaKv.Equals(ma_kv)).ToList();
        }

        // Cán bộ trực quét theo mã đơn vị của user:
        public List<DmUser> getListCanBoTruc()
        {
            Guid oidCurrentUser = new Guid("8C5E5187-71CC-4884-A6E7-03EEC7BAA142"); // Sau sẽ bind cái này với token của user.
            DmUser currentUser = _context.DmUsers.Where(x => x.Oid == oidCurrentUser).FirstOrDefault();

            if (currentUser != null)
            {
                return _context.DmUsers.Where(x => x.MaDonvi.Equals(currentUser.MaDonvi) && x.IsActive == true).ToList();
            } else
            {
                return new List<DmUser>();
            }
        }


        //Tra cứu lịch trực có Filter theo mã khu vực.
        public DanhSachLichTruc searchFilterStationSchedule(string ma_kv, DateTime? ngay_xemlich)
        {
            Guid oidCurrentUser = new Guid("8320946A-9EB6-4C02-B3BC-FCB695D9E8BF"); // Fake mã user, sau này sẽ truyền từ token Auth vào.

            DmUser currentUser = _context.DmUsers.Where(x => x.Oid == oidCurrentUser).FirstOrDefault();

            if (currentUser != null)
            {
                // Kiểm tra phân quyền. Chỉ các account thuộc đơn vị 00 hoặc được phân quyền mới được xem lịch trực.
                DmKhuvuc checkKhuVuc = _context.DmKhuvucs.Where(x => x.MaKv == ma_kv).FirstOrDefault();
                PquyenCnang checkChucnang = _context.PquyenCnangs.Where(x => (x.MaUser.Equals(currentUser.MaUser) && x.LoaiQuyen.Equals("XEMLICHTRUC") && x.TrangThai == 1)).FirstOrDefault();

                if (checkKhuVuc != null && (currentUser.MaDonvi.Equals("00") || checkChucnang != null))
                {
                    // Chọn lịch trực ctu.
                    LichtrucCtu ltctu = (from lichTrucCtu in _context.LichtrucCtus
                                         where 
                                         ((lichTrucCtu.MaKv.Equals(ma_kv)) && 
                                         (ngay_xemlich != null ? (lichTrucCtu.TuNgay.Value.Date <= ngay_xemlich.Value.Date && (lichTrucCtu.DenNgay != null ? lichTrucCtu.DenNgay.Value.Date >= ngay_xemlich.Value.Date : true)) : true))
                                         orderby lichTrucCtu.PrKey descending
                                         select new LichtrucCtu
                                         {
                                             PrKey = lichTrucCtu.PrKey,
                                             GhiChu = lichTrucCtu.GhiChu
                                         }
                                           ).FirstOrDefault();

                // Sau đó lấy danh sách cúa tất cả các gara trong khu vực 
                   
                    List<DmGaraKhuvuc> danhSachGaraKv = _context.DmGaraKhuvucs.Where(x=>x.MaKv.Equals(ma_kv)).ToList();

                    // Rồi lấy lịch trực tương ứng với mỗi gara đó.
                    List<GaraLichTruc> danhSachGara = new List<GaraLichTruc>();

                    danhSachGaraKv.ForEach(x =>
                    { 
                        GaraLichTruc grlt = new GaraLichTruc
                        {
                            tenGara = x.TenGara,
                            maGara = x.MaGara,
                            listLichTruc = (ltctu != null ? (_context.LichTrucgdvs.Where(item => (item.FrKey == ltctu.PrKey) && (item.MaGara.Equals(x.MaGara)) && (ngay_xemlich == null ? item.SuDung == 1 : true)).OrderBy(item => item.TenGara).OrderBy(item => item.Thu).OrderByDescending(item=>item.SangChieu).ToList()) : new List<LichTrucgdv>())
                        };

                        danhSachGara.Add(grlt);
                    });

                    // Cuối cùng, tổng hợp toàn bộ, kèm với ghi chú vào Danh Sách Lịch Trực rồi trả về.
                    DanhSachLichTruc dstl = new DanhSachLichTruc
                    {
                        ghiChu = (ltctu != null ? ltctu.GhiChu : "Không có lịch trực nào được kích hoạt trong khoảng thời gian này."),
                        listGara = danhSachGara,
                    };

                    return dstl;
                }
                else
                {
                    return new DanhSachLichTruc
                    {
                        ghiChu = "Không tìm thấy đơn vị, hoặc user không được phân quyền !"
                    };
                        
                }
            } else
            {
                return new DanhSachLichTruc
                {
                    ghiChu = "User không tồn tại !"
                };
            }
        }

        // Lấy danh sách bảng ghi chú của lịch trực.
        // Nếu trả NULL sẽ kiểm tra để báo lỗi.
        public async Task<List<GhichuLichtruc>> getListScheduleNotes (string ma_kv) 
        {
            try
            {
                LichtrucCtu lichtrucctu = (from ltctu in _context.LichtrucCtus
                                                 where ltctu.MaKv.Equals(ma_kv)
                                                   orderby ltctu.PrKey descending
                                                   select new LichtrucCtu
                                                   {
                                                       PrKey = ltctu.PrKey,
                                                   }
                                                   ).FirstOrDefault();

                if (lichtrucctu != null)
                {
                    List<GhichuLichtruc> ghichulichtruc = await _context.GhichuLichtrucs.Where(x => x.FrKey == lichtrucctu.PrKey && x.SuDung.Value == true).ToListAsync();

                    if (ghichulichtruc != null)
                    {
                        return ghichulichtruc;
                    } else
                    {
                        return new List<GhichuLichtruc>();
                    }
                } else
                {
                    return new List<GhichuLichtruc>();
                }

            } catch (Exception err)
            {
                Console.WriteLine(err);
                return null;
            }
        }

        // Tra cứu lịch trực cụ thể theo FR Key của tuần trực đó.
        public List<LichTrucgdv> SearchLichTrucTheoFrKey(int fr_key)
        {
            Guid oidCurrentUser = new Guid("8320946A-9EB6-4C02-B3BC-FCB695D9E8BF");
            DmUser currentUser = _context.DmUsers.Where(x => x.Oid == oidCurrentUser).FirstOrDefault();

            if (currentUser != null)
            {
                // Kiểm tra phân quyền. Chỉ các account thuộc đơn vị 00 hoặc được phân quyền mới được xem lịch trực.
                PquyenCnang checkChucnang = _context.PquyenCnangs.Where(x => (x.MaUser.Equals(currentUser.MaUser) && x.LoaiQuyen.Equals("XEMLICHTRUC") && x.TrangThai == 1)).FirstOrDefault();

                if (currentUser.MaDonvi.Equals("00") || checkChucnang != null)
                {
                    List<LichTrucgdv> list_lich_truc = (from lich_truc in _context.LichTrucgdvs
                                                        join khuvuc in _context.DmKhuvucs on lich_truc.MaKv equals khuvuc.MaKv
                                                        where lich_truc.FrKey == fr_key
                                                        orderby lich_truc.TenGara, lich_truc.Thu

                                                        select new LichTrucgdv
                                                        {
                                                            PrKey = lich_truc.PrKey,
                                                            FrKey = lich_truc.FrKey,
                                                            MaKv = lich_truc.MaKv,
                                                            MaGara = lich_truc.MaGara,
                                                            TenGara = lich_truc.TenGara,
                                                            Thu = lich_truc.Thu,
                                                            SangChieu = lich_truc.SangChieu,
                                                            Thoigian = lich_truc.Thoigian,
                                                            NgayTao = lich_truc.NgayTao,
                                                            NgayBo = lich_truc.NgayBo,
                                                            NgayCapnhat = lich_truc.NgayCapnhat,
                                                            SuDung = lich_truc.SuDung,
                                                            MaUser = lich_truc.MaUser,
                                                            TenUser = lich_truc.TenUser,
                                                        }
                             ).ToList();
                    return list_lich_truc;
                }
                else
                {
                    return new List<LichTrucgdv>();
                }
            }
            else
            {
                return new List<LichTrucgdv>();
            }
        }

        // Kiêm tra xem có phải tạo lịch trực mới hay không.
        // Nếu sửa lịch trực mà ngày khác nhau thì PHẢI TẠO LỊCH TRỰC MỚI. Còn nếu sửa trong cùng ngày thì chỉ cần trả lại lịch trực mới nhất.
        public LichtrucCtu checkAddLichTrucMoi(string ma_kv, DmUser currentUser)
        {
            LichtrucCtu lichtruc = _context.LichtrucCtus.Where(x => x.MaKv.Equals(ma_kv)).OrderByDescending(x => x.PrKey).FirstOrDefault();

            // Kiểm tra xem lịch trực đã tồn tại hay chưa.
            if (lichtruc != null)
            {
                // Nếu trùng ngày thì update, nếu khác ngày thì tạo mới.
                if (lichtruc.NgayTao.Value.Date != DateTime.Now.Date)
                {
                    // Khác ngày thì tạo mới lại. 
                    // Trước đó, khoán ngày hết cho lịch trực hiện tại.
                    lichtruc.DenNgay = DateTime.Now.AddDays(-1);
                    _context.LichtrucCtus.Update(lichtruc);

                    LichtrucCtu newLichtruc = new LichtrucCtu
                    {
                        PrKey = 0,//_context.LichtrucCtus.Count() + 1,
                        MaDonvi = currentUser.MaDonvi,
                        MaKv = ma_kv,
                        TuNgay = DateTime.Now,
                        GhiChu = lichtruc.GhiChu,
                        NgayTao = DateTime.Now,
                    };

                     _context.LichtrucCtus.Add(newLichtruc);

                    _context.SaveChanges();

                    LichtrucCtu toBeReturned = _context.LichtrucCtus.Where(x => x.MaKv.Equals(ma_kv)).OrderByDescending(x => x.PrKey).FirstOrDefault();

                    // Sau đó, copy lại toàn bộ ghi chú và danh sách giám định viên qua.
                    List<LichTrucgdv> danhSachLichTruc = _context.LichTrucgdvs.Where(x => x.FrKey == lichtruc.PrKey).ToList();
                    danhSachLichTruc.ForEach(x =>
                    {

                        x.PrKey = 0;
                        x.FrKey = toBeReturned.PrKey;
                        _context.LichTrucgdvs.Add(x);
                    });

                    List<GhichuLichtruc> danhSachGhiChu = _context.GhichuLichtrucs.Where(x => x.FrKey == lichtruc.PrKey && x.SuDung == true).ToList();
                    danhSachGhiChu.ForEach(x =>
                    {
                        x.PrKey = 0; //_context.LichTrucgdvs.Count() + 1;
                        x.FrKey = toBeReturned.PrKey;
                        _context.GhichuLichtrucs.Add(x);
                    });

                    _context.SaveChanges();

                    return toBeReturned;
                }

                // Còn nếu cùng ngày thì giữ nguyên.
                else
                {
                    return lichtruc;
                }

            }
            // Nếu trống thì tạo lịch trực mới.
            else
            {
                LichtrucCtu newLichtruc = new LichtrucCtu
                {
                    MaDonvi = currentUser.MaDonvi,
                    MaKv = ma_kv,
                    TuNgay = DateTime.Now,
                    GhiChu = "",
                    NgayTao = DateTime.Now,
                };

                _context.LichtrucCtus.Add(newLichtruc);
                _context.SaveChanges();

                // Lấy lại lịch trực đã ăn PrKey từ DB để trả về.
                LichtrucCtu toBeReturned = _context.LichtrucCtus.Where(x => x.MaKv.Equals(ma_kv)).OrderByDescending(x => x.PrKey).FirstOrDefault();
                return toBeReturned;
            }
        }

        // Validate lại phân quyền của user trước khi tiến hành insert.
        // Kiểm tra phân quyền của User: 
        // - User phải thuộc đơn vị trung tâm và là loại user quản lý
        // - HOẶC User đã được cấp quyền.
        public bool validatePhanQuyenEdit (DmUser currentUser)
        { 
            List<PquyenCnang> phanquyen = _context.PquyenCnangs.Where(x => x.LoaiQuyen.Equals("SUALICHTRUC")).ToList();

            bool checkDonvi = (currentUser.MaDonvi == "00" || currentUser.MaDonvi == "31" || currentUser.MaDonvi == "32"); // Kiểm tra đơn vị
            bool checkLoaiUser = (currentUser.LoaiUser == 1 || currentUser.LoaiUser == 6 || currentUser.LoaiUser == 9 || currentUser.LoaiUser == 10 || currentUser.LoaiUser == 11); // Kiểm tra user phải thuộc loại quản lý.
            bool checkPhanQuyen = (phanquyen != null && phanquyen.Find(x => x.MaUser == currentUser.MaUser) != null); // Kiểm tra user được phân quyền

            return (checkPhanQuyen || (checkDonvi && checkLoaiUser));
        }

        // Update lại thanh ghi chú của mỗi khu vực trực.
        // khanhlh - 19/09/2024
        public async Task<string> updateScheduleNote(string ma_kv, string ghiChu)
        {
            Guid oidCurrentUser = new Guid("8320946A-9EB6-4C02-B3BC-FCB695D9E8BF");
            try
            {
                DmUser currentUser = await _context.DmUsers.Where(x => x.Oid == oidCurrentUser).FirstOrDefaultAsync();
                if (currentUser != null)
                {
                    // Kiểm tra phân quyền của User: 
                    // - User phải thuộc đơn vị trung tâm và là loại user quản lý
                    // - HOẶC User đã được cấp quyền.

                    if (validatePhanQuyenEdit(currentUser))
                    {
                        // Kiểm tra xem có phải tạo lại lịch trực mới không.
                        LichtrucCtu lichTrucHienTai = checkAddLichTrucMoi(ma_kv, currentUser);

                        lichTrucHienTai.GhiChu = ghiChu;

                        _context.LichtrucCtus.Update(lichTrucHienTai);
                        await _context.SaveChangesAsync();
                        return lichTrucHienTai.PrKey.ToString();
                    }
                    else
                    {
                        return "User không được phân quyền thực hiện chức năng này.";
                    }
                }
                else
                {
                    return "User không tồn tại";
                }
            }
            catch (Exception err)
            {
                return "Lỗi update ghi chú lịch trực.";
            }
        }

        // Update lại thanh ghi chú của mỗi khu vực trực.
        // khanhlh - 19/09/2024

        public async Task<string> updateIndividualNote(int pr_key, string ghiChu)
        {
            Guid oidCurrentUser = new Guid("8320946A-9EB6-4C02-B3BC-FCB695D9E8BF");
            try
            {
                DmUser currentUser = await _context.DmUsers.Where(x => x.Oid == oidCurrentUser).FirstOrDefaultAsync();
                if (currentUser != null)
                {
                    // Kiểm tra phân quyền của User: 
                    // - User phải thuộc đơn vị trung tâm và là loại user quản lý
                    // - HOẶC User đã được cấp quyền.

                    if (validatePhanQuyenEdit(currentUser)) // Validate lại phân quyền.
                    {
                        // Lấy lịch trực bằng PrKey
                        GhichuLichtruc lichtruc = await _context.GhichuLichtrucs.Where(x=>x.PrKey==pr_key).FirstOrDefaultAsync();

                        if (lichtruc != null)
                        {
                            lichtruc.GhiChu = ghiChu;
                            _context.GhichuLichtrucs.Update(lichtruc);
                            await _context.SaveChangesAsync();
                            return lichtruc.PrKey.ToString();
                        } else
                        {
                            return $"Pr Key {lichtruc.FrKey} không tồn tại";
                        }
                    }
                    else
                    {
                        return "User không được phân quyền thực hiện chức năng này.";
                    }
                }
                else
                {
                    return "User không tồn tại";
                }
            }
            catch (Exception err)
            {
                return "Lỗi update ghi chú lịch trực.";
            }
        }



        // Gán / xóa cán bộ trực, cập nhật lịch trực mới của mỗi khu vực trực.
        // khanhlh - 19/09/2024
        // Update lại thanh ghi chú của mỗi khu vực trực.
        // khanhlh - 19/09/2024
        public async Task<string> updateSchedulePerson(string ma_kv, string ma_gara, string thu, string sang_chieu, string[] ma_user_deleted, string[] ma_user_added)
            // Params thời gian: 1 = buổi sáng, 2 = buổi chiều, 3 = cả ngày.
        {
            Guid oidCurrentUser = new Guid("8320946A-9EB6-4C02-B3BC-FCB695D9E8BF");
            try
            {
                DmUser currentUser = await _context.DmUsers.Where(x => x.Oid == oidCurrentUser).FirstOrDefaultAsync();
                if (currentUser != null)
                {
                    // Kiểm tra phân quyền của User: 
                    // - User phải thuộc đơn vị trung tâm và là loại user quản lý
                    // - HOẶC User đã được cấp quyền.

                    if (validatePhanQuyenEdit(currentUser))
                    {
                        // Lấy tuần trực mới nhất.
                        LichtrucCtu lichTrucHienTai = checkAddLichTrucMoi(ma_kv, currentUser);

                        // Nếu là cả ngày thì check cả sáng cả chiều

                        // Đầu tiên, TIẾN HÀNH XÓA USER

                        if (sang_chieu.Equals("ca_ngay"))
                        {
                            // Check buổi sáng

                            // Danh sách lịch trực sáng cho ngày hôm đó.
                            List<LichTrucgdv> danhSachLichTrucSang = await _context.LichTrucgdvs.Where(x => (x.FrKey == lichTrucHienTai.PrKey) && x.MaGara.Equals(ma_gara) && x.Thu.Equals(thu) && x.SangChieu.Equals("sang") && x.SuDung == 1).ToListAsync();

                            // Tiến hành thao tác với danh sách các cán bộ bị xóa.
                            if (ma_user_deleted != null && danhSachLichTrucSang != null)
                            {
                                for (int i = 0; i < ma_user_deleted.Length; i++)
                                {
                                    LichTrucgdv toBeCancelled = danhSachLichTrucSang.Find(x => x.MaUser.Equals(ma_user_deleted[i]));
                                    if (toBeCancelled != null)
                                    {
                                        toBeCancelled.SuDung = 0;
                                        toBeCancelled.NgayBo = DateTime.Now;
                                        toBeCancelled.NgayCapnhat = DateTime.Now;
                                        _context.LichTrucgdvs.Update(toBeCancelled);
                                        _context.SaveChanges();
                                    }

                                    // Sau khi xóa cán bộ, check xem trong lịch trực đó còn cán bộ đó không.
                                    // Nếu không còn thì tiến hành xóa khỏi bảng ghi chú.
                                    LichTrucgdv checkPersonExist = await _context.LichTrucgdvs.Where(x => (x.FrKey == lichTrucHienTai.PrKey) && (x.MaUser.Equals(ma_user_deleted[i])) && (x.SuDung == 1)).FirstOrDefaultAsync(); // Nếu user hoàn toàn không còn tồn tại trong bảng lịch trực thì xóa khỏi ghi chú.
                                    if (checkPersonExist == null)
                                    {
                                        GhichuLichtruc gclc = await _context.GhichuLichtrucs.Where(x => (x.FrKey == lichTrucHienTai.PrKey) && (x.MaUser.Equals(ma_user_deleted[i])) && x.SuDung == true).FirstOrDefaultAsync();
                                        if (gclc != null)
                                        {
                                            gclc.SuDung = false;
                                            _context.GhichuLichtrucs.Update(gclc);
                                        }
                                    }
                                }
                            }


                            // Sau khi check sáng xong thì check chiều.

                            // Danh sách lịch trực buổi chiều:
                            List<LichTrucgdv> danhSachLichTrucChieu = await _context.LichTrucgdvs.Where(x => (x.FrKey == lichTrucHienTai.PrKey) && x.MaGara.Equals(ma_gara) && x.Thu.Equals(thu) && x.SangChieu.Equals("chieu") && x.SuDung == 1).ToListAsync();

                            // Tiến hành thao tác trên các giám định viên bị gỡ.
                            if (ma_user_deleted != null && danhSachLichTrucChieu != null)
                            {
                                for (int i = 0; i < ma_user_deleted.Length; i++)
                                {
                                    LichTrucgdv toBeCancelled = danhSachLichTrucChieu.Find(x => x.MaUser.Equals(ma_user_deleted[i]));
                                    if (toBeCancelled != null)
                                    {
                                        toBeCancelled.SuDung = 0;
                                        toBeCancelled.NgayBo = DateTime.Now;
                                        toBeCancelled.NgayCapnhat = DateTime.Now;
                                        _context.LichTrucgdvs.Update(toBeCancelled);
                                        _context.SaveChanges();
                                    }

                                    // Sau khi xóa cán bộ, check xem trong lịch trực đó còn cán bộ đó không.
                                    // Nếu không còn thì tiến hành xóa khỏi bảng ghi chú.

                                    LichTrucgdv checkPersonExist = _context.LichTrucgdvs.Where(x => (x.FrKey == lichTrucHienTai.PrKey) && (x.MaUser.Equals(ma_user_deleted[i])) && (x.SuDung == 1)).FirstOrDefault(); // Nếu user hoàn toàn không còn tồn tại trong bảng lịch trực thì xóa khỏi ghi chú.
                                    if (checkPersonExist == null)
                                    {
                                        GhichuLichtruc gclc = await _context.GhichuLichtrucs.Where(x => (x.FrKey == lichTrucHienTai.PrKey) && (x.MaUser.Equals(ma_user_deleted[i])) && x.SuDung == true).FirstOrDefaultAsync();
                                        if (gclc != null)
                                        {
                                            gclc.SuDung = false;
                                            _context.GhichuLichtrucs.Update(gclc);
                                        }
                                    }
                                }
                            }
                        }

                        else
                        {
                            // Lấy danh sách tất cả lịch trực
                            List<LichTrucgdv> danhSachLichTruc = await _context.LichTrucgdvs.Where(x => (x.FrKey == lichTrucHienTai.PrKey) && x.MaGara.Equals(ma_gara) && x.Thu.Equals(thu) && x.SangChieu.Equals(sang_chieu) && x.SuDung == 1).ToListAsync();

                            // Trước hết, tiến hành thao tác trên các giám định viên bị xóa.
                            if (ma_user_deleted != null && danhSachLichTruc != null)
                            {
                                // Đầu tiên, tiến hành kiểm tra các cán bộ bị xóa.
                                for (int i = 0; i < ma_user_deleted.Length; i++)
                                {
                                    LichTrucgdv toBeCancelled = danhSachLichTruc.Find(x => x.MaUser.Equals(ma_user_deleted[i]));
                                    if (toBeCancelled != null)
                                    {
                                        toBeCancelled.SuDung = 0;
                                        toBeCancelled.NgayBo = DateTime.Now;
                                        toBeCancelled.NgayCapnhat = DateTime.Now;
                                        _context.LichTrucgdvs.Update(toBeCancelled);
                                        _context.SaveChanges();
                                    }

                                    // Sau khi xóa cán bộ, check xem trong lịch trực đó còn cán bộ đó không.
                                    // Nếu không còn thì tiến hành xóa khỏi bảng ghi chú.
                                    LichTrucgdv checkPersonExist = _context.LichTrucgdvs.Where(x => (x.FrKey == lichTrucHienTai.PrKey) && (x.MaUser.Equals(ma_user_deleted[i])) && (x.SuDung == 1)).FirstOrDefault(); // Nếu user hoàn toàn không còn tồn tại trong bảng lịch trực thì xóa khỏi ghi chú.
                                    if (checkPersonExist == null)
                                    {
                                        GhichuLichtruc gclc = await _context.GhichuLichtrucs.Where(x => (x.FrKey == lichTrucHienTai.PrKey) && (x.MaUser.Equals(ma_user_deleted[i])) && x.SuDung == true).FirstOrDefaultAsync();
                                        if (gclc != null)
                                        {
                                            gclc.SuDung = false;
                                            _context.GhichuLichtrucs.Update(gclc);
                                        }
                                    }
                                }
                            }
                        }

                        // Sau đó, TIẾN HÀNH UPDATE CÁC USER HIỆN ĐANG TRỰC

                        if (ma_user_added != null) { 
                        
                            // Chạy dọc list các user hiện đang trực
                            for (int i = 0; i < ma_user_added.Length; i++)
                            {

                                // Kiểm tra xem user này có đang trực buổi nào không. Nếu trùng thì không thêm mới.
                                LichTrucgdv CheckExistMorning = _context.LichTrucgdvs.Where(x => (x.SuDung == 1) && (x.MaKv.Equals(ma_kv) && (x.MaGara.Equals(ma_gara)) && (x.Thu.Equals(thu)) && x.SangChieu.Equals("sang") && x.MaUser.Equals(ma_user_added[i]))).FirstOrDefault(); // Kiếm tra xem user này đã đang trực chưa.
                                LichTrucgdv CheckExistAfternoon = _context.LichTrucgdvs.Where(x => (x.SuDung == 1) && (x.MaKv.Equals(ma_kv) && (x.MaGara.Equals(ma_gara)) && (x.Thu.Equals(thu)) && x.SangChieu.Equals("chieu") && x.MaUser.Equals(ma_user_added[i]))).FirstOrDefault();

                                // Lấy thông tin cán bộ và thông tin Gara khu vực.
                                DmUser canBoTruc = _context.DmUsers.Where(x => x.MaUser.Equals(ma_user_added[i])).FirstOrDefault();
                                DmGaraKhuvuc garaKV = _context.DmGaraKhuvucs.Where(x => x.MaGara.Equals(ma_gara)).FirstOrDefault();
                                // Nếu chọn cả ngày thì thêm cả sáng cả chiều.

                                if (sang_chieu.Equals("ca_ngay"))
                                {
                                    if (CheckExistMorning == null)
                                    {
                                        LichTrucgdv toBeAddedMorning = new LichTrucgdv
                                        {
                                            PrKey = 0,
                                            FrKey = lichTrucHienTai.PrKey,
                                            MaKv = ma_kv,
                                            MaGara = ma_gara,
                                            TenGara = garaKV.TenGara,
                                            Thu = thu,
                                            SangChieu = "sang",
                                            Thoigian = "",
                                            NgayTao = DateTime.Now,
                                            NgayBo = null,
                                            NgayCapnhat = DateTime.Now,
                                            SuDung = 1,
                                            MaUser = ma_user_added[i],
                                            TenUser = canBoTruc.TenUser,
                                        };
                                        _context.LichTrucgdvs.Add(toBeAddedMorning);
                                    }

                                    if (CheckExistAfternoon == null)
                                    {
                                        LichTrucgdv toBeAddedAfternoon = new LichTrucgdv
                                        {
                                            PrKey = 0,
                                            FrKey = lichTrucHienTai.PrKey,
                                            MaKv = ma_kv,
                                            MaGara = ma_gara,
                                            TenGara = garaKV.TenGara,
                                            Thu = thu,
                                            SangChieu = "chieu",
                                            Thoigian = "",
                                            NgayTao = DateTime.Now,
                                            NgayBo = null,
                                            NgayCapnhat = DateTime.Now,
                                            SuDung = 1,
                                            MaUser = ma_user_added[i],
                                            TenUser = canBoTruc.TenUser,
                                        };
                                        _context.LichTrucgdvs.Add(toBeAddedAfternoon);
                                    }

                                }

                                // Nếu không phải cả ngày thì chỉ check cho buổi đó.
                                else
                                {
                                    if ((sang_chieu.Equals("sang") && CheckExistMorning == null) || (sang_chieu.Equals("chieu") && CheckExistAfternoon == null))
                                    {
                                        LichTrucgdv toBeAdded = new LichTrucgdv
                                        {
                                            PrKey = 0,
                                            FrKey = lichTrucHienTai.PrKey,
                                            MaKv = ma_kv,
                                            MaGara = ma_gara,
                                            TenGara = garaKV.TenGara,
                                            Thu = thu,
                                            SangChieu = sang_chieu,
                                            Thoigian = "",
                                            NgayTao = DateTime.Now,
                                            NgayCapnhat = DateTime.Now,
                                            SuDung = 1,
                                            MaUser = ma_user_added[i],
                                            TenUser = canBoTruc.TenUser,
                                        };

                                        _context.LichTrucgdvs.Add(toBeAdded);
                                    }

                                }

                                // Sau khi thêm cán bộ, check xem trong lịch trực đó còn cán bộ đó không.
                                // Nếu không còn thì tiến hành thêm vào bảng ghi chú.
                                GhichuLichtruc gclc = await _context.GhichuLichtrucs.Where(x => x.FrKey == lichTrucHienTai.PrKey && x.MaUser.Equals(ma_user_added[i]) && x.SuDung == true).FirstOrDefaultAsync();
                                if (gclc == null)
                                {
                                    GhichuLichtruc newGclc = new GhichuLichtruc
                                    {
                                        PrKey = 0,
                                        FrKey = lichTrucHienTai.PrKey,
                                        MaUser = canBoTruc.MaUser,
                                        TenUser = canBoTruc.TenUser,
                                        DienThoai = canBoTruc.Dienthoai,
                                        GhiChu = "",
                                        SuDung = true,
                                    };

                                    _context.GhichuLichtrucs.Update(newGclc);
                                }
                            }
                        }

                        _context.LichtrucCtus.Update(lichTrucHienTai);
                        await _context.SaveChangesAsync();
                        return lichTrucHienTai.PrKey.ToString();
                    }
                    else
                    {
                        return "User không được phân quyền thực hiện chức năng này.";
                    }
                }
                else
                {
                    return "User không tồn tại";
                }
            }
            catch (Exception err)
            {
                return "Lỗi update ghi chú lịch trực.";
            }
        }



    }
}