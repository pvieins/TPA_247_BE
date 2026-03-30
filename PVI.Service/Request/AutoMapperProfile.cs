using AutoMapper;
using PVI.DAO.Entities.Models;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace PVI.Service.Request
{ 
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<HsgdTtrinhRequest, HsgdTtrinh>();
            CreateMap<HsgdTtrinh, HsgdTtrinhRequest>();
            CreateMap<HsgdTtrinhCtRequest, HsgdTtrinhCt>();
            CreateMap<HsgdTtrinhCt, HsgdTtrinhCtRequest>();

            CreateMap<HsbtCtDetailRequest, HsbtCt>();
            CreateMap<HsbtCt, HsbtCtDetailRequest>();
            CreateMap<HsgdDxCtRequest, HsgdDxCt>();
            CreateMap<HsgdDxCt, HsgdDxCtRequest>();

            CreateMap<HsbtGdRequest, HsbtGd>();
            CreateMap<HsbtGd, HsbtGdRequest>();
            CreateMap<HsbtUocGdRequest, HsbtUocGd>();
            CreateMap<HsbtUocGd, HsbtUocGdRequest>();
            CreateMap<HsbtThtsRequest, HsbtTht>();
            CreateMap<HsbtTht, HsbtThtsRequest>();
            CreateMap<HsgdDxRequest, HsgdDx>();
            CreateMap<HsgdDx, HsgdDxRequest>();
            CreateMap<HsgdDxRequest, HsgdDxTsk>();
            CreateMap<HsgdDxTsk, HsgdDxRequest>();
            CreateMap<ImportPASCRequest, HsgdDx>();
            CreateMap<HsgdDx, ImportPASCRequest>();

            CreateMap<DmGaraRequest, DmGaRa>();
            CreateMap<DmGaRa, DmGaraRequest>();

            CreateMap<UserGDTT, DmUser>();
            CreateMap<DmUser, UserGDTT>();

            CreateMap<UserGDDK, DmUser>();
            CreateMap<DmUser, UserGDDK>();

            //CreateMap<DmGaraFilter, DmGaRa>();
            //CreateMap<DmGaRa, DmGaraFilter>();

            CreateMap<DmDiemtrucRequest, DmDiemtruc>();
            CreateMap<DmDiemtruc, DmDiemtrucRequest>();

            CreateMap<DmDiemtrucFilter, DmDiemtruc>();
            CreateMap<DmDiemtruc, DmDiemtrucFilter>();

            CreateMap<DmPquyenKyHsRequest, DmPquyenKyhs>();
            CreateMap<DmPquyenKyhs, DmPquyenKyHsRequest>();

            CreateMap<DmQuyenKyFilter, DmPquyenKyhs>();
            CreateMap<DmPquyenKyhs, DmQuyenKyFilter>();

            CreateMap<HsgdCtuUpdateRequest, HsgdCtu>();
            CreateMap<HsgdCtu, HsgdCtuUpdateRequest>();

            CreateMap<DmNHmucRequest, DmNhmuc>();
            CreateMap<DmNhmuc, DmNHmucRequest>();

            CreateMap<DmHmucRequest, DmHmuc>();
            CreateMap<DmHmuc, DmHmucRequest>();

            CreateMap<DmNHmucFilter, DmNhmuc>();
            CreateMap<DmNhmuc, DmHmucFilter>();

            CreateMap<DmHmucFilter, DmHmuc>();
            CreateMap<DmHmuc, DmHmucFilter>();

            CreateMap<DmHmuc_PASC_Filter, DmHmuc>();
            CreateMap<DmHmuc, DmHmuc_PASC_Filter>();

            CreateMap<DmNhmucUpdate, DmNhmuc>();
            CreateMap<DmNhmuc, DmNhmucUpdate>();

            CreateMap<DmHmucUpdate, DmHmuc>();
            CreateMap<DmHmuc, DmHmucUpdate>();

            CreateMap<UyQuyenFilter, DmUqHstpc>();
            CreateMap<DmUqHstpc, UyQuyenFilter>();

            CreateMap<UyQuyenRequest, DmUqHstpc>();
            CreateMap<DmUqHstpc, UyQuyenRequest>();

            CreateMap<KhuVucFilter, DmKhuvuc>();
            CreateMap<DmKhuvuc, KhuVucFilter>();

            CreateMap<KhuVucCreate, DmKhuvuc>();
            CreateMap<DmKhuvuc, KhuVucCreate>();

            CreateMap<KhuVucUpdate, DmKhuvuc>();
            CreateMap<DmKhuvuc, KhuVucUpdate>();

            CreateMap<DeviceFilter, DmDevice>();
            CreateMap<DmDevice, DeviceFilter>();

            CreateMap<DeviceRequest, DmDevice>();
            CreateMap<DmDevice, DeviceRequest>();

            CreateMap<HieuXeRequest, DmHieuxe>();
            CreateMap<DmHieuxe, HieuXeRequest>();

            CreateMap<LoaiXeFilter, DmLoaixe>();
            CreateMap<DmLoaixe, LoaiXeFilter>();

            CreateMap<LoaiXeRequest, DmLoaixe>();
            CreateMap<DmLoaixe, LoaiXeRequest>();

            CreateMap<GaraKhuVucFilter, DmGaraKhuvuc>();
            CreateMap<DmGaraKhuvuc, GaraKhuVucFilter>();

            CreateMap<GaraKhuVucRequest, DmGaraKhuvuc>();
            CreateMap<DmGaraKhuvuc, GaraKhuVucRequest>();

            CreateMap<LichtrucgdvFilter, LichTrucgdv>();
            CreateMap<LichTrucgdv, LichtrucgdvFilter>();
        }
    }
}