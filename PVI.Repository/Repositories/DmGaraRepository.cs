using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using PVI.DAO.Entities.Models;
using PVI.Helper;
using PVI.Repository.Interfaces;
using ServiceReference1;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.WebSockets;
using static iTextSharp.text.pdf.events.IndexEvents;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PVI.Repository.Repositories
{

    /* Implementation cho interface Danh mục điểm trực.
     * lhkhanh - 22/08/2024
     */

    // Kế thừa base.
    public class DmGaraRepository : GenericRepository<DmGaRa>, IDmGaraRepository
    {
        public DmGaraRepository(GdttContext context, Pvs2024Context context_pias, Serilog.ILogger logger, IConfiguration conf) : base(context, context_pias, logger, conf)
        {

        }

        // Lay danh sach cac gara.
        // Tham số: Page & Limit - Dùng để phân trang, có kèm theo các filter.
        public Task<List<DmGaRa>> getGarageList(int pageNumber, int limit)
        {

            var list_gara = (from gara in _context.DmGaRas
                             where gara.GaraTthai
                             select new DmGaRa
                             {
                                 MaGara = gara.MaGara,
                                 TenGara = gara.TenGara,
                                 TenTat = gara.TenTat,
                                 MaDonvi = gara.MaDonvi,
                                 DiaChi = gara.DiaChi,
                                 DiaChiXuong = gara.DiaChi,
                                 TenTinh = gara.TenTinh,
                                 QuanHuyen = gara.QuanHuyen,
                                 TyleggPhutung = gara.TyleggPhutung,
                                 TyleggSuachua = gara.TyleggSuachua,
                                 EmailGara = gara.EmailGara,
                                 DienThoaiGara = gara.DienThoaiGara,
                                 NgayCnhat = gara.NgayCnhat,
                                 MaUsercNhat = gara.MaUsercNhat,
                                 GaraTthai = gara.GaraTthai,
                                 bnkCode=gara.bnkCode,
                                 ten_ctk=gara.ten_ctk,
                                 Count = _context.DmGaRas.Count()
                             }
                      ).Skip(limit * (pageNumber - 1)).Take(limit).AsQueryable();
            return ToListWithNoLockAsync(list_gara);
        }

        // Lay danh sach cac gara.
        // Tham số: Page & Limit - Dùng để phân trang, có kèm theo các filter.
        public PagedList<DmGaRa> searchFilterGarage(DmGaraFilter searchTarget)
        {
            var dm_kh = ToListWithNoLock((from a in _context_pias.DmKhaches
                                          where a.Gara == true
                                          select new
                                          {
                                              a.MaKh,
                                              a.MasoVat,
                                              a.TkVnd,
                                              a.NganHang
                                          }).AsQueryable());
            var dm_gara = ToListWithNoLock(_context.DmGaRas.Where(x => x.GaraTthai == true).AsQueryable());
            var list_gara = (from gara in dm_gara
                             join b in dm_kh on gara.MaGara equals b.MaKh into b1
                             from b in b1.DefaultIfEmpty()
                             where (
                              gara.GaraTthai == true
                             )
                             select new DmGaRa
                             {
                                 MaGara = gara.MaGara,
                                 TenGara = gara.TenGara,
                                 TenTat = gara.TenTat,
                                 MaDonvi = gara.MaDonvi,
                                 DiaChi = gara.DiaChi,
                                 DiaChiXuong = gara.DiaChi,
                                 TenTinh = gara.TenTinh,
                                 QuanHuyen = gara.QuanHuyen,
                                 TyleggPhutung = gara.TyleggPhutung,
                                 TyleggSuachua = gara.TyleggSuachua,
                                 SongayThanhtoan = gara.SongayThanhtoan,
                                 EmailGara = gara.EmailGara,
                                 DienThoaiGara = gara.DienThoaiGara,
                                 NgayCnhat = gara.NgayCnhat,
                                 MaUsercNhat = gara.MaUsercNhat,
                                 GaraTthai = gara.GaraTthai,
                                 MasoVat = b != null ? b.MasoVat : "",
                                 TkVnd = b != null ? b.TkVnd : "",
                                 NganHang = b != null ? b.NganHang : "",
                                 bnkCode=gara.bnkCode,
                                 ten_ctk=gara.ten_ctk,
                                 thoa_thuan_hop_tac = gara.thoa_thuan_hop_tac
                             }
                      ).AsQueryable();
            if (!string.IsNullOrEmpty(searchTarget.maGara))
            {
                list_gara = list_gara.Where(x => x.MaGara.Contains(searchTarget.maGara));
            }
            if (!string.IsNullOrEmpty(searchTarget.tenGara))
            {
                list_gara = list_gara.Where(x => x.TenGara.Contains(searchTarget.tenGara));
            }
            if (!string.IsNullOrEmpty(searchTarget.tenTat))
            {
                list_gara = list_gara.Where(x => x.TenTat.Contains(searchTarget.tenTat));
            }
            if (!string.IsNullOrEmpty(searchTarget.diaChi))
            {
                list_gara = list_gara.Where(x => x.DiaChi.Contains(searchTarget.diaChi));
            }
            if (!string.IsNullOrEmpty(searchTarget.diaChiXuong))
            {
                list_gara = list_gara.Where(x => x.DiaChiXuong.Contains(searchTarget.diaChiXuong));
            }
            if (!string.IsNullOrEmpty(searchTarget.tenTinh))
            {
                list_gara = list_gara.Where(x => x.TenTinh.Contains(searchTarget.tenTinh));
            }
            if (!string.IsNullOrEmpty(searchTarget.quanHuyen))
            {
                list_gara = list_gara.Where(x => x.QuanHuyen.Contains(searchTarget.quanHuyen));
            }
            if (searchTarget.tyleggPhutung != 0)
            {
                list_gara = list_gara.Where(x => x.TyleggPhutung == searchTarget.tyleggPhutung);
            }
            if (searchTarget.tyleggSuachua != 0)
            {
                list_gara = list_gara.Where(x => x.TyleggSuachua == searchTarget.tyleggSuachua);
            }
            if (!string.IsNullOrEmpty(searchTarget.emailGara))
            {
                list_gara = list_gara.Where(x => x.EmailGara.Contains(searchTarget.emailGara));
            }
            if (!string.IsNullOrEmpty(searchTarget.dienthoaiGara))
            {
                list_gara = list_gara.Where(x => x.DienThoaiGara.Contains(searchTarget.dienthoaiGara));
            }
            if (searchTarget.ngayCnhat != null)
            {
                list_gara = list_gara.Where(x => x.NgayCnhat != null && x.NgayCnhat.Value.Date == searchTarget.ngayCnhat.Value.Date);
            }
            if (!string.IsNullOrEmpty(searchTarget.MasoVat))
            {
                list_gara = list_gara.Where(x => x.MasoVat.Contains(searchTarget.MasoVat));
            }
            if (searchTarget.thoaThuanHopTac.HasValue)
            {
                list_gara = list_gara.Where(x => x.thoa_thuan_hop_tac == searchTarget.thoaThuanHopTac);
            }
            return PagedList<DmGaRa>.ToPagedList(list_gara.AsQueryable(), searchTarget.pageNumber, searchTarget.pageSize);
        }
        public Task<List<GaRaView>> getAllGara(DmGaraFilter searchTarget)
        {
            var data = (from gara in _context.DmGaRas

                        where (
                        gara.GaraTthai == true
                        )
                        select new GaRaView
                        {
                            MaGara = gara.MaGara,
                            TenGara = gara.TenGara,
                            TenTat = gara.TenTat,
                            TyleggPhutung = gara.TyleggPhutung,
                            TyleggSuachua = gara.TyleggSuachua,
                            bnkCode=gara.bnkCode,
                            ten_ctk=gara.ten_ctk

                        }
                      ).AsQueryable();
            if (!string.IsNullOrEmpty(searchTarget.maGara))
            {
                data = data.Where(x => x.MaGara.Contains(searchTarget.maGara));
            }
            if (!string.IsNullOrEmpty(searchTarget.tenGara))
            {
                data = data.Where(x => x.TenGara.Contains(searchTarget.tenGara));
            }
            if (!string.IsNullOrEmpty(searchTarget.tenTat))
            {
                data = data.Where(x => x.TenTat.Contains(searchTarget.tenTat));
            }
            if (searchTarget.tyleggPhutung != 0)
            {
                data = data.Where(x => x.TyleggPhutung == searchTarget.tyleggPhutung);
            }
            if (searchTarget.tyleggSuachua != 0)
            {
                data = data.Where(x => x.TyleggSuachua == searchTarget.tyleggSuachua);
            }
            return ToListWithNoLockAsync(data);
        }
        // Update Gara
        public string updateGarage(DmGaRa gara, string currentUserEmail, string TkVnd, string NganHang,string bnkCode,string ten_ctk)
        {
            try
            {
                DmUser currentUser = _context.DmUsers.Where(x => x.Mail == currentUserEmail).FirstOrDefault();
                int[] allowedUsers = new int[] { 1, 2, 3, 6, 8, 9, 10, 11 }; // Chỉ có các tài khoản loại này mới có quyền sửa
                if (currentUser != null && (Array.Exists(allowedUsers, x => x == currentUser.LoaiUser) || currentUser.MaUser == "thangpt"))
                {
                    gara.NgayCnhat = DateTime.Now;
                    gara.MaUsercNhat = currentUser.MaUser;
                    gara.DiaChi = gara.DiaChiXuong;
                    gara.bnkCode = bnkCode;
                    gara.ten_ctk = ten_ctk;

                    // gọi piassoap để cập nhật thông tin tài khoản
                    PiasSoapSoap ws = new PiasSoapSoapClient(ServiceReference1.PiasSoapSoapClient.EndpointConfiguration.PiasSoapSoap, _configuration["client:endpoint:address"]);
                    var client = new ServiceReference1.PiasSoapSoapClient(ServiceReference1.PiasSoapSoapClient.EndpointConfiguration.PiasSoapSoap, _configuration["client:endpoint:address"]);
                    client.Endpoint.Binding.SendTimeout = new TimeSpan(0, 20, 0);
                    client.InnerChannel.OperationTimeout = new TimeSpan(20, 20, 20);
                    var up_stk_nghang = client.Update_Gara(gara.MaGara, TkVnd, NganHang);
                    if (up_stk_nghang)
                    {
                        _context.DmGaRas.Update(gara);
                        _context.SaveChanges();
                        return gara.MaGara.ToString();
                    }
                    else
                    {
                        return "Cập nhật không thành công!";
                    }

                }
                else
                {
                    return "Tài khoản không được phân quyền để cập nhật Gara";
                }
            }

            catch (Exception ex)
            {
                _logger.Error("dbContextTransaction Exception when updateGarage: " + ex.ToString());
                _logger.Error("Error record: " + JsonConvert.SerializeObject(gara));
                _context.Dispose();
                throw;
            }
        }

        // Đồng bộ danh mục Gara từ PIAS.
        // Cách đồng bộ gara như sau:
        // Lấy ngày cập nhật cuối cùng của 247, sau đó so qua PIAS.
        // Nếu PIAS 
        public string syncGarageFromPias()
        {
            try
            {

                // Lấy danh sách các gara trên PIAS được cập nhật sau ngày trên để đồng bộ qua 247.

                // Gara đã đánh dấu là sử dụng
                List<DmKhach> list_garage_pias = (from gara in _context_pias.DmKhaches
                                                  where (gara.Gara)
                                                  select new DmKhach
                                                  {
                                                      MaKh = gara.MaKh, // Mã Gara
                                                      TenKh = gara.TenKh, // Tên Gara
                                                      TenTat = gara.TenTat,
                                                      MaDonvi = gara.MaDonvi,
                                                      DiaChi = gara.DiaChi,
                                                      GaraTthai = gara.GaraTthai,
                                                      MaTinh = gara.MaTinh,
                                                      Tel = gara.Tel,
                                                      NgayCnhat = gara.NgayCnhat
                                                  }).Distinct().ToList();


                List<DmGaRa> list_gara_247 = _context.DmGaRas.ToList();

                // Dùng Hash Set để search cho nhanh.
                HashSet<string> list_garage_247 = new HashSet<string>(_context.DmGaRas.Select(x => x.MaGara));

                // Sau đó, đồng bộ ngược những gara chưa có về 247.
                foreach (var garage in list_garage_pias)
                {
                    if (!list_garage_247.Contains(garage.MaKh))
                    {
                        DmGaRa new_garage = new DmGaRa();
                        new_garage.MaGara = garage.MaKh;
                        new_garage.TenGara = garage.TenKh;
                        new_garage.TenTat = garage.TenTat;
                        new_garage.MaDonvi = garage.MaDonvi;
                        new_garage.DiaChi = garage.DiaChi ?? "";
                        new_garage.DiaChiXuong = garage.DiaChi ?? "";
                        new_garage.GaraTthai = garage.GaraTthai ?? false;
                        if (!string.IsNullOrEmpty(garage.MaTinh) && garage.MaTinh.Length >= 4)
                        {
                            var tinh = _context.DmTinhs.Where(x => x.MaTinh == garage.MaTinh.Substring(0, 4) && x.TongHop == 1).FirstOrDefault();
                            if (tinh != null)
                            {
                                new_garage.TenTinh = tinh.TenTinh;
                            }
                            else
                            {
                                new_garage.TenTinh = "";
                            }
                        }
                        else
                        {
                            new_garage.TenTinh = "";
                        }
                        if (!string.IsNullOrEmpty(garage.MaTinh) && garage.MaTinh.Length >= 6)
                        {
                            var huyen = _context.DmTinhs.Where(x => x.MaTinh == garage.MaTinh && x.TongHop == 0).FirstOrDefault();
                            if (huyen != null)
                            {
                                new_garage.QuanHuyen = huyen.TenTinh;
                            }
                            else
                            {
                                new_garage.QuanHuyen = "";
                            }
                        }
                        else
                        {
                            new_garage.QuanHuyen = "";
                        }
                        //new_garage.TenTinh = (!string.IsNullOrEmpty(garage.MaTinh) && garage.MaTinh.Length >= 4) ? _context.DmTinhs.Where(x => x.MaTinh == garage.MaTinh.Substring(0, 4) && x.TongHop == 1).FirstOrDefault().TenTinh : "";
                        //new_garage.QuanHuyen = (!string.IsNullOrEmpty(garage.MaTinh) && garage.MaTinh.Length >= 6) ? _context.DmTinhs.Where(x => x.MaTinh == garage.MaTinh && x.TongHop == 0).FirstOrDefault().TenTinh : "";
                        new_garage.NgayCnhat = DateTime.Now;
                        new_garage.MaUsercNhat = "";
                        new_garage.EmailGara = garage.Email;

                        if (!string.IsNullOrEmpty(garage.Tel))
                        {
                            new_garage.DienThoaiGara = garage.Tel;
                        }
                        _context.DmGaRas.Add(new_garage);
                        //try
                        //{
                        //    _context.SaveChanges();
                        //}
                        //catch (Exception ex)
                        //{
                        //    _logger.Information("syncGarageFromPias ADD gara = " + JsonConvert.SerializeObject(new_garage));
                        //    _logger.Error("syncGarageFromPias ADD gara error " + ex.Message);
                        //}
                    }
                    // Tiến hành CRUD cập nhật lại gara
                    // khanhlh - 28/02/2025
                    else
                    {
                        DmGaRa existingGara = list_gara_247.Where(x => x.MaGara == garage.MaKh).FirstOrDefault();
                        if (existingGara != null)
                        {
                            existingGara.MaDonvi = garage.MaDonvi ?? "";
                            existingGara.TenGara = garage.TenKh ?? "";
                            existingGara.TenTat = garage.TenTat ?? "";
                            existingGara.DienThoaiGara = existingGara.DienThoaiGara ?? "";
                            existingGara.EmailGara = existingGara.EmailGara ?? "";
                            //existingGara.TenTinh = (!string.IsNullOrEmpty(garage.MaTinh) && garage.MaTinh.Length >= 4) ? _context.DmTinhs.Where(x => x.MaTinh == garage.MaTinh.Substring(0, 4) && x.TongHop == 1).FirstOrDefault().TenTinh : ""; 
                            //existingGara.QuanHuyen = (!string.IsNullOrEmpty(garage.MaTinh) && garage.MaTinh.Length >= 6) ? _context.DmTinhs.Where(x => x.MaTinh == garage.MaTinh.Substring(0, 6) && x.TongHop == 0).FirstOrDefault().TenTinh : "";
                            if (!string.IsNullOrEmpty(garage.MaTinh) && garage.MaTinh.Length >= 4)
                            {
                                var tinh = _context.DmTinhs.Where(x => x.MaTinh == garage.MaTinh.Substring(0, 4) && x.TongHop == 1).FirstOrDefault();
                                if (tinh != null)
                                {
                                    existingGara.TenTinh = tinh.TenTinh;
                                }
                                else
                                {
                                    existingGara.TenTinh = "";
                                }
                            }
                            else
                            {
                                existingGara.TenTinh = "";
                            }
                            if (!string.IsNullOrEmpty(garage.MaTinh) && garage.MaTinh.Length >= 6)
                            {
                                var huyen = _context.DmTinhs.Where(x => x.MaTinh == garage.MaTinh && x.TongHop == 0).FirstOrDefault();
                                if (huyen != null)
                                {
                                    existingGara.QuanHuyen = huyen.TenTinh;
                                }
                                else
                                {
                                    existingGara.QuanHuyen = "";
                                }
                            }
                            else
                            {
                                existingGara.QuanHuyen = "";
                            }
                            existingGara.DiaChi = garage.DiaChi ?? "";
                            existingGara.DiaChiXuong = garage.DiaChi ?? "";
                            existingGara.GaraTthai = garage.GaraTthai ?? false;
                            existingGara.NgayCnhat = (existingGara.NgayCnhat > garage.NgayCnhat) ? existingGara.NgayCnhat : garage.NgayCnhat;
                            _context.DmGaRas.Update(existingGara);
                        }
                    }


                }
                _context.SaveChanges();
                return "1"; // Trả 1 (TRUE) là thành công
            }
            catch (Exception ex)
            {
                _logger.Error("EXCEPTION_GARAGESYNC: " + ex.Message);
                return "Lỗi đồng bộ Gara, vui lòng liên hệ IT. Mã lỗi: EXCEPTION_GARAGESYNC";
            }
        }

    }
}