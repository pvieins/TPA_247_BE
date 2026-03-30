using AutoMapper;
using PVI.Repository.Interfaces;
using PVI.Repository.Repositories;
using PVI.DAO.Entities.Models;
using PVI.Service.Request;
using Azure.Core;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using PVI.Helper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Cryptography;
using PVI.Service.Request;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.Service.ActionProcess
{
    public class DmUserService
    {
        private readonly IDmUserRepository _dmUserRepository;
        private readonly Serilog.ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public DmUserService(IDmUserRepository dmUserRepository, IMapper mapper, Serilog.ILogger logger, IConfiguration conf)
        {
            _dmUserRepository = dmUserRepository;
            _mapper = mapper;
            _logger = logger;
            _configuration = conf;
        }
        public List<DmUserView> GetListGiamDV(string currentUserEmail)
        {
            List<DmUserView> obj_result = new List<DmUserView>();

            try
            {
                DmUser currentUser = _dmUserRepository.GetEntityByConditionNoAsync(x => x.Mail == currentUserEmail);
                if (currentUser != null)
                {
                    obj_result = _dmUserRepository.GetListEntityByConditionNoAsync(x => (x.LoaiUser == 4 || x.LoaiUser == 7 || x.IsGdvHotro.Value || x.LoaiUser == 8) && x.IsActive == true).Select(s => new DmUserView
                    {
                        Oid = s.Oid,
                        MaUser = s.MaUser,
                        TenUser = s.TenUser,
                        LoaiUser = s.LoaiUser,
                        Dienthoai = s.Dienthoai
                    }).ToList();
                    if (obj_result != null)
                    {
                        var dm_loai_user = _dmUserRepository.getDMLoaiUser();
                        obj_result = (from a in obj_result
                                      join b in dm_loai_user on a.LoaiUser equals b.LoaiUser into b1
                                      from b in b1.DefaultIfEmpty()
                                      select new DmUserView
                                      {
                                          Oid = a.Oid,
                                          MaUser = a.MaUser,
                                          TenUser = a.TenUser + (b != null ? (": " + b.TenLoaiUser) : ""),
                                          LoaiUser = a.LoaiUser,
                                          Dienthoai = a.Dienthoai
                                      }).ToList();
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return obj_result;
        }
        public List<DmUserView> GetListCanBoTT(string currentUserEmail)
        {
            List<DmUserView> obj_result = new List<DmUserView>();
            try
            {
                var currentUser = _dmUserRepository.GetEntityByConditionNoAsync(x => x.Mail == currentUserEmail);
                if (currentUser != null)
                {
                    if (currentUser.LoaiUser == 8)
                    {
                        obj_result = _dmUserRepository.GetListEntityByConditionNoAsync(x => x.LoaiUser == 7).Select(s => new DmUserView
                        {
                            Oid = s.Oid,
                            MaUser = s.MaUser,
                            TenUser = s.TenUser,
                            LoaiUser = s.LoaiUser
                        }).ToList();
                    }
                    else
                    {
                        var list_loai_user = new List<int?> { 6, 7, 8, 9, 4, 10 };
                        obj_result = _dmUserRepository.GetListEntityByConditionNoAsync(x => list_loai_user.Any(p => p == x.LoaiUser) && x.IsActive == true).Select(s => new DmUserView
                        {
                            Oid = s.Oid,
                            MaUser = s.MaUser,
                            TenUser = s.TenUser,
                            LoaiUser = s.LoaiUser
                        }).ToList();
                    }
                    if (obj_result != null)
                    {
                        var dm_loai_user = _dmUserRepository.getDMLoaiUser();
                        obj_result = (from a in obj_result
                                      join b in dm_loai_user on a.LoaiUser equals b.LoaiUser into b1
                                      from b in b1.DefaultIfEmpty()
                                      select new DmUserView
                                      {
                                          Oid = a.Oid,
                                          MaUser = a.MaUser,
                                          TenUser = a.TenUser + (b.TenLoaiUser != "" ? (": " + b.TenLoaiUser) : ""),
                                          LoaiUser = a.LoaiUser
                                      }).ToList();
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return obj_result;
        }
        public List<DmUserView> GetListCanBoDuyet(string currentUserEmail)
        {
            var list_dv = _dmUserRepository.GetListCanBoDuyet(currentUserEmail);
            return list_dv;
        }
        public List<DmUserView> GetListDoiTruong(string currentUserEmail)
        {
            var list_dv = _dmUserRepository.GetListDoiTruong(currentUserEmail);
            return list_dv;
        }

        public List<DmDonvi> getDMDonvi(string currentUserEmail)
        {
            var list_dv = _dmUserRepository.getDMDonvi(currentUserEmail);
            return list_dv;
        }
        public List<DmLoaiUser> getDMLoaiUser()
        {
            var list_loai = _dmUserRepository.getDMLoaiUser();
            return list_loai;
        }

        public PagedList<DmUser> getListUserGDTT(int pageNumber, int pageSize, string currentUserEmail)
        {
            var list_tt = _dmUserRepository.getListUserGDTT(pageNumber, pageSize, currentUserEmail).Result;
            return list_tt;
        }

        public PagedList<DmUser> getListUserGDDK(int pageNumber, int pageSize, string currentUserEmail)
        {
            var list_tt = _dmUserRepository.getListUserGDDK(pageNumber, pageSize, currentUserEmail).Result;
            return list_tt;
        }

        public DanhSachUser searchUserGDTT(int pageNumber, int limit, UserGDTT searchTarget, string currentUserEmail)
        {
            var toBeSearched = _mapper.Map<UserGDTT, DmUser>(searchTarget);
            DanhSachUser listUser = _dmUserRepository.searchFilterUserGDTT(pageNumber, limit, toBeSearched, currentUserEmail).Result;
            return listUser;
        }

        public DanhSachUser searchUserGDDK(int pageNumber, int limit, UserGDDK searchTarget, string currentUserEmail)
        {
            var toBeSearched = _mapper.Map<UserGDDK, DmUser>(searchTarget);
            toBeSearched.IsActiveGddk = true;
            DanhSachUser listUser = _dmUserRepository.searchFilterUserGDDK(pageNumber, limit, toBeSearched, currentUserEmail).Result;
            return listUser;
        }

        public Task<string> createUserGDTT(UserGDTT searchTarget, string currentUserEmail)
        {
            var toBeCreated = _mapper.Map<UserGDTT, DmUser>(searchTarget);
            var createdUser = _dmUserRepository.createUser(toBeCreated, currentUserEmail);
            return createdUser;
        }

        public Task<string> createUserGDDK(UserGDDK searchTarget, string currentUserEmail)
        {
            var toBeCreated = _mapper.Map<UserGDDK, DmUser>(searchTarget);
            var createdUser = _dmUserRepository.createUser(toBeCreated, currentUserEmail);
            return createdUser;
        }
        public DmUser getUserPiasFromEmail(string userEmail)
        {
            DmUser toBeReturned = _dmUserRepository.getUserPiasFromEmail(new DmUser(), userEmail);
            return toBeReturned;
        }

        public List<DmUser> getListUserPiasFromDonvi(int pageNumber, int pageSize)
        {
            List<DmUser> toBeReturned = _dmUserRepository.getListUserPiasFromDonvi(new DmUser() { MaDonvi = "00" }, pageNumber, pageSize);
            return toBeReturned;
        }


        // Update user GDTT
        public async Task<string> updateUserGDTT(UserGDTT updateTarget, string currentUserEmail)
        {
            var result = "";
            try
            {
                var userGDTT = await _dmUserRepository.GetEntityByCondition(x => x.MaUser == updateTarget.MaUser);

                if (userGDTT != null)
                {
                    DmUser targetToBeUpdated = new DmUser
                    {

                        Dienthoai = updateTarget.Dienthoai,
                        TenUser = updateTarget.TenUser,
                        MaUser = updateTarget.MaUser,
                        MaDonvi = updateTarget.MaDonvi,
                        Mail = updateTarget.Mail,
                        LoaiUser = updateTarget.LoaiUser,
                        IsActive = updateTarget.IsActive,
                        LoaiCbo = updateTarget.LoaiCbo,
                        PhanQuyen = updateTarget.PhanQuyen,
                        MaDonviPquyen = updateTarget.MaDonviPquyen,
                        MaUserPias = updateTarget.MaUserPias,
                        IsActiveGddk = updateTarget.IsActiveGddk,
                        PquyenUplHinhAnh = updateTarget.PquyenUplHinhAnh,
                        IsGdvHotro = updateTarget.IsGdvHotro,
                        IsActiveGqkn = updateTarget.IsActiveGqkn,
                        IsactiveChkc = updateTarget.IsactiveChkc,
                        IsActiveKytt = updateTarget.IsActiveKytt
                    };

                    targetToBeUpdated.NgayCnhat = DateTime.Now;
                    if (targetToBeUpdated.MaUserPias == null)
                    {
                        targetToBeUpdated.MaUserPias = "";
                    }
                    result = await _dmUserRepository.UpdateUser(targetToBeUpdated, currentUserEmail);
                }
                else
                {
                    result = $"User {updateTarget.MaUser} không tồn tại";
                }
            }
            catch (Exception ex)
            {
                //_logger.Error("CreateDiemtruc:", ex);
                //_logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
            }
            return result;
        }

        // Update thêm user GDDK
        //public async Task<string> updateUserGDDK(UserGDDK updateTarget, string currentUserEmail)
        //{
        //    var result = "";
        //    try
        //    {
        //        var userGDDK = await _dmUserRepository.GetEntityByCondition(x => x.MaUser == updateTarget.MaUser);

        //        if (userGDDK != null)
        //        {
        //            var targetToBeUpdated = _mapper.Map(updateTarget, userGDDK);
        //            targetToBeUpdated.NgayCnhat = DateTime.Now;
        //            result = await _dmUserRepository.UpdateUser(targetToBeUpdated, currentUserEmail);
        //        }
        //        else
        //        {
        //            result = $"User {updateTarget.MaUser} không tồn tại";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //_logger.Error("CreateDiemtruc:", ex);
        //        //_logger.Error("Error record: " + JsonConvert.SerializeObject(entity));
        //    }
        //    return result;
        //}

        public async Task<string> generateJWTToken(string ma_user)
        {
            string token = await _dmUserRepository.GenerateJWTToken(ma_user);
            return token;
        }
        public List<DmUserView> GetListCanBoGDTT(string currentUserEmail)
        {
            var list_cb = _dmUserRepository.GetListCanBoGDTT(currentUserEmail);
            return list_cb;
        }

    }
}