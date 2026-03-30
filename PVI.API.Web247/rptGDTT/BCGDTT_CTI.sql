USE [GDTT]
GO
/****** Object:  StoredProcedure [dbo].[BCGDTT_CTI]    Script Date: 2/11/2026 8:55:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
--EXEC BCGDTT_CTI @SoHsgd = N'25/42/000246';
--EXEC BCGDTT_CTI  @TuNgay='01/01/2025',@DenNgay='02/06/2025',@MaDonVi=N'01,02,03,04,05,06,07,08,09,10,11,13,26,28,34,37,38,41,42,47',@maCanBo='DD056574-E484-40DB-943F-6334486FB2F8,DD056574-E484-40DB-943F-6334486FB2F8';
--EXEC BCGDTT_CTI  @TuNgay='01/01/2025',@DenNgay='02/03/2025',@maCanBo='DD056574-E484-40DB-943F-6334486FB2F8,DD056574-E484-40DB-943F-6334486FB2F8';
ALTER PROCEDURE [dbo].[BCGDTT_CTI]
	
	@MaDonVi NVARCHAR(max) = NULL,
    @MaDonViTt NVARCHAR(max) = NULL,
    @TuNgay NVARCHAR(20) = NULL,
    @DenNgay NVARCHAR(20) = NULL,
    @TuNgayDuyettpc NVARCHAR(20) = NULL,
    @DenNgayDuyettpc NVARCHAR(20) = NULL,
    @TuNgayPDTT NVARCHAR(20) = NULL,
    @DenNgayPDTT NVARCHAR(20) = NULL,
    @MaTtrangGd NVARCHAR(50) = NULL,
    @LoaiHsgd NVARCHAR(20) = NULL,
    @SoHsgd NVARCHAR(50) = NULL,
    @SoAnChi NVARCHAR(50) = NULL,
    @BienKSoat NVARCHAR(50) = NULL,
    @IsTPC BIT = NULL,
    @maCanBo NVARCHAR(max) = NULL,
	@email_run NVARCHAR(50) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	if exists (select 1 from tempdb.dbo.sysobjects where name='#Temp_basehsgd')	drop table #Temp_basehsgd;
	declare @ma_user NVARCHAR(20),@loai_user NVARCHAR(10) ,@pquyen_upl_hinh_anh bit,@sql nvarchar(max),@ParamDef nvarchar(max);
--1/ Tạo bảng temp
			CREATE TABLE #Temp_basehsgd(
				[pr_key] [int] NOT NULL,
				[pr_key_bt] [decimal](18, 0) NOT NULL,
				[ten_donvi] [nvarchar](50) NULL,
				[ten_donvi_tt] [nvarchar](50) NULL,
				[ten_khach] [nvarchar](150) NOT NULL,
				[so_donbh] [nvarchar](23) NOT NULL,
				[so_hsgd] [nvarchar](12) NOT NULL,
				[so_seri] [decimal](18, 0) NOT NULL,
				[bien_ksoat] [nvarchar](250) NOT NULL,
				[hieu_xe] [nvarchar](50) NULL,
				[loai_xe] [nvarchar](50) NULL,
				[ngay_dau_seri] [smalldatetime] NULL,
				[ngay_cuoi_seri] [smalldatetime] NULL,
				[ngay_ctu] [smalldatetime] NULL,
				[ngay_tthat] [smalldatetime] NULL,
				[dia_diemtt] [nvarchar](max) NOT NULL,
				[nguyen_nhan_ttat] [nvarchar](max) NOT NULL,
				[so_ngaybh] [int] NULL,
				[tb_tt] [int] NULL,
				[ct_nd] [int] NULL,
				[ngayct] [smalldatetime] NULL,
				[ngay_duyet] [smalldatetime] NULL,
				[ngay_huy_hs] [smalldatetime] NULL,
				[so_lan_gd] [decimal](18, 0) NOT NULL,
				[ma_user] [nvarchar](50) NULL,
				[so_tienu] [decimal](18, 0) NOT NULL,
				[so_tienp] [decimal](18, 0) NOT NULL,
				[gdv] [nvarchar](100) NOT NULL,
				[tinh_trang] [nvarchar](50) NULL,
				[ma_lhsbt] [nvarchar](13) NOT NULL,
				[thoi_gian_xly] [nvarchar](13) NOT NULL,
				[hsgd_tpc] [nvarchar](9) NOT NULL,
				[ten_gara] [nvarchar](150) NOT NULL,
				[ten_tat_gara] [nvarchar](50) NOT NULL,
				[ma_gara] [nvarchar](11) NOT NULL,
				[ghi_chu] [nvarchar](2000) NOT NULL,
				[so_tienUbandau] [decimal](18, 0) NOT NULL,
				[SoTienGtbt] [decimal](18, 0) NULL,
				[SoTienGtbtTNDS] [decimal](18, 0) NULL,
				[SoTienGtbtKhac] [decimal](18, 0) NULL,
				[ng_lienhe] [nvarchar](200) NOT NULL,
				[dien_thoai] [nvarchar](50) NOT NULL,
				[dien_thoai_ndbh] [nvarchar](50) NOT NULL,
				[ma_lhsbt_new] [nvarchar](8) NOT NULL,
				[sum_tienthaythe] [decimal](38, 0) NULL,
				[sum_tiensuachua] [decimal](38, 0) NULL,
				[sum_tienson] [decimal](38, 0) NULL,
				[sum_sotiendoitru_vcx] [decimal](38, 0) NULL,
				[sum_sotiendoitru_tnds] [decimal](38, 0) NULL,
				[st_bl_vcx] [numeric](38, 0) NULL,
				[st_bl_tnds] [numeric](38, 0) NULL,
				[so_tienugddx_vcx] [decimal](38, 0) NULL,
				[tien_pheduyet_vcx] [decimal](38, 0) NULL,
				[so_tienugddx_tnds_nguoi] [decimal](38, 0) NULL,
				[tien_pheduyet_tnds_nguoi] [decimal](38, 0) NULL,
				[so_tienugddx_tnds] [decimal](38, 0) NULL,
				[tien_pheduyet_tnds] [decimal](38, 0) NULL,
				[so_tienugddx_khac] [decimal](38, 0) NULL,
				[tien_pheduyet_khac] [decimal](38, 0) NULL,
				[ngay_duyettpc] [smalldatetime] NULL,
				[cbott] [nvarchar](250) NULL,
				[ngay_bstt] [nvarchar](20)  NULL,
				[ghi_chudx] [nvarchar](max) NOT NULL,
				[ghi_chudxtt] [nvarchar](2000) NOT NULL,
				[ghi_chudx_tnds] [nvarchar](max) NOT NULL,
				[ghi_chudx_tndstt] [nvarchar](2000) NOT NULL,
				[ghi_chudx_tsk] [nvarchar](max) NOT NULL,
				[ghi_chudx_tsktt] [nvarchar](2000) NOT NULL,
				[vat] [int] NULL,
				[so_tienctkh] [decimal](18, 0) NULL,
				[lydo_ctkh] [nvarchar](max) NULL,
				[vat_tnds] [int] NULL,
				[lydo_ctkh_tnds] [nvarchar](max) NULL,
				[so_tienctkh_tnds] [decimal](18, 0) NULL,
				[tylegg_phutungvcx] [decimal](18, 4) NULL,
				[tylegg_suachuavcx] [decimal](18, 4) NULL,
				[tylegg_phutungtnds] [decimal](18, 4) NULL,
				[tylegg_suachuatnds] [decimal](18, 4) NULL,
				[ggphutungvcx] [decimal](38, 6) NULL,
				[ggsuachuavcx] [decimal](38, 6) NULL,
				[ggphutungthds] [decimal](38, 6) NULL,
				[ggsuachuatnds] [decimal](38, 6) NULL,
				[so_tienctkh_tsk] [decimal](18, 0) NULL,
				[lydo_ctkh_tsk] [nvarchar](max) NULL,
				[ma_nguyen_nhan_ttat] [nvarchar](10) NULL,
				[ten_nguyen_nhan_ttat] [nvarchar](100) NULL,
				[ma_dkhoan] [nvarchar](10) NOT NULL,				
				[ten_loai_dongco] [nvarchar](50) NULL,
				[sotien_ttpin] [decimal](18, 0) NOT NULL,
				[ten_cbotrinh] [nvarchar](100) NOT NULL,
				[vai_tro] [nvarchar](20) NOT NULL,
				[tyle_tg] [decimal](18, 0) NOT NULL,
				[so_hsbt] [nvarchar](30) NOT NULL,
				[ngay_pd_tt] [smalldatetime] NULL,
				[ten_nguoi_duyet] [nvarchar](50) NULL,
				[nguon_tao] [nvarchar](100) NULL,
				[ma_cbott] [nvarchar](50) NULL,
				[canbo_pdtt] [nvarchar](50) NULL,
				[ngay_dutlieu] [smalldatetime] NULL
			)
BEGIN TRY
--2/ Đẩy dữ liệu vào temp							
set @sql= N'INSERT INTO #Temp_basehsgd([pr_key],[pr_key_bt],[ten_donvi],[ten_donvi_tt],[ten_khach],[so_donbh],[so_hsgd],[so_seri],[bien_ksoat],[hieu_xe],[loai_xe],[ngay_dau_seri],[ngay_cuoi_seri],[ngay_ctu],[ngay_tthat],[dia_diemtt],[nguyen_nhan_ttat],[so_ngaybh],[tb_tt],[ct_nd],[ngayct],[ngay_duyet],[ngay_huy_hs],[so_lan_gd],[ma_user],[so_tienu],[so_tienp],[gdv],[tinh_trang],[ma_lhsbt],[thoi_gian_xly],[hsgd_tpc],[ten_gara],[ten_tat_gara],[ma_gara],[ghi_chu],[so_tienUbandau],[SoTienGtbt],[SoTienGtbtTNDS],[SoTienGtbtKhac],[ng_lienhe],[dien_thoai],[dien_thoai_ndbh],[ma_lhsbt_new],[sum_tienthaythe],[sum_tiensuachua],[sum_tienson],[sum_sotiendoitru_vcx],[sum_sotiendoitru_tnds],[st_bl_vcx],[st_bl_tnds],[so_tienugddx_vcx],[tien_pheduyet_vcx],[so_tienugddx_tnds_nguoi],[tien_pheduyet_tnds_nguoi],[so_tienugddx_tnds],[tien_pheduyet_tnds],[so_tienugddx_khac],[tien_pheduyet_khac],[ngay_duyettpc],[cbott],[ngay_bstt],[ghi_chudx],[ghi_chudxtt],[ghi_chudx_tnds],[ghi_chudx_tndstt],[ghi_chudx_tsk],[ghi_chudx_tsktt],[vat],[so_tienctkh],[lydo_ctkh],[vat_tnds],[lydo_ctkh_tnds],[so_tienctkh_tnds],[tylegg_phutungvcx],[tylegg_suachuavcx],[tylegg_phutungtnds],[tylegg_suachuatnds],[ggphutungvcx],[ggsuachuavcx],[ggphutungthds],[ggsuachuatnds],[so_tienctkh_tsk],[lydo_ctkh_tsk],[ma_nguyen_nhan_ttat],[ten_nguyen_nhan_ttat],[ma_dkhoan],[ten_loai_dongco],[sotien_ttpin],[ten_cbotrinh],[vai_tro],[tyle_tg],[so_hsbt],[ngay_pd_tt],[ten_nguoi_duyet],[nguon_tao],[ma_cbott],[canbo_pdtt],[ngay_dutlieu])
		SELECT  A.pr_key, 
			          pr_key_bt, 
			     (SELECT ten_donvi 
			      FROM dm_donvi 
			      WHERE ma_donvi=A.ma_donvi) AS ten_donvi, 
			     (SELECT ten_donvi 
			      FROM dm_donvi 
			      WHERE ma_donvi=A.ma_donvi_tt) AS ten_donvi_tt, 
			          ten_khach, 
			          so_donbh, 
			          so_hsgd, 
			          so_seri, 
			          bien_ksoat, 
			     (SELECT hieu_xe 
			      FROM dm_hieuxe 
			      WHERE pr_key=vcx.hieu_xe) hieu_xe, 
			     (SELECT loai_xe 
			      FROM dm_loaixe 
			      WHERE pr_key=vcx.loai_xe) loai_xe, 
			          A.ngay_dau_seri, 
			          A.ngay_cuoi_seri, 
			          ngay_ctu, 
			          ngay_tthat, 
			          A.dia_diemtt, 
			          A.nguyen_nhan_ttat, 
			          DATEDIFF(DAY, A.ngay_dau_seri, A.ngay_tthat) AS so_ngaybh, 
			          DATEDIFF(DAY, A.ngay_tbao, A.ngay_ctu) AS tb_tt,0 ct_nd, 
			          A.ngay_ctu AS ngayct, 
			         CAST(NULL AS DATETIME) AS ngay_duyet, 
			         CAST(NULL AS DATETIME) AS ngay_huy_hs , 
			          so_lan_bt AS so_lan_gd, 
			          CAST(A.ma_user AS NVARCHAR(50)) , 
			          isnull( 
			                   (SELECT top 1 so_tien 
			                    FROM hsgd_dg with(nolock) 
			                    WHERE loai_dg=0 
			                      AND fr_key=A.pr_key),0) AS so_tienu, 
			          isnull( 
			                   (SELECT top 1 so_tien 
			                    FROM hsgd_dg with(nolock) 
			                    WHERE loai_dg=1 
			                      AND fr_key=A.pr_key),0) AS so_tienp, 
			     cast('''' as nvarchar(100)) AS gdv, 
			     (SELECT ten_ttrang_gd 
			      FROM dm_ttrang_gd 
			      WHERE ma_ttrang_gd=A.ma_ttrang_gd) AS tinh_trang, 
			          CASE 
			              WHEN ma_lhsbt=1 THEN N''Tự giám định'' 
			              WHEN ma_lhsbt=2 THEN N''Nhờ giám định''
			              ELSE N''Giám định hộ'' 
			          END AS ma_lhsbt, 
			          CASE 
			              WHEN datediff(DAY, CONVERT(date,ngay_ctu, 103), getdate()) <= 10 THEN N''Trong 10 Ngày'' 
			              ELSE N''Quá 10 Ngày'' 
			          END AS thoi_gian_xly, 
			          CASE 
			              WHEN hsgd_tpc=1 THEN N''Hồ sơ TPC'' 
			              ELSE N''Hồ sơ DPC'' 
			          END AS hsgd_tpc, 
			          CASE 
			              WHEN ((vcx.ma_gara='''' or vcx.ma_gara IS null) and (tnds.ma_gara<>'''' or tnds.ma_gara is not null))  THEN isnull( 
			                                                     (SELECT top 1 ten_gara 
			                                                      FROM dm_ga_ra 
			                                                      WHERE ma_gara=tnds.ma_gara),'''') 
			              ELSE isnull( 
			                            (SELECT top 1 ten_gara 
			                             FROM dm_ga_ra 
			                             WHERE ma_gara=vcx.ma_gara),'''') 
			          END AS ten_gara, 
			          CASE 
			              WHEN ((vcx.ma_gara='''' or vcx.ma_gara IS null) and (tnds.ma_gara<>'''' or tnds.ma_gara is not null)) THEN isnull( 
			                                                     (SELECT top 1 ten_tat 
			                                                      FROM dm_ga_ra 
			                                                      WHERE ma_gara=tnds.ma_gara),'''') 
			              ELSE isnull( 
			                            (SELECT top 1 ten_tat 
			                             FROM dm_ga_ra 
			                             WHERE ma_gara=vcx.ma_gara),'''') 
			          END AS ten_tat_gara, 
			          CASE 
			              WHEN ((vcx.ma_gara='''' or vcx.ma_gara IS null) and (tnds.ma_gara<>'''' or tnds.ma_gara is not null)) THEN isnull( 
			                                                     (SELECT top 1 ma_gara 
			                                                      FROM dm_ga_ra 
			                                                      WHERE ma_gara=tnds.ma_gara),'''') 
			              ELSE isnull( 
			                            (SELECT top 1 ma_gara 
			                             FROM dm_ga_ra 
			                             WHERE ma_gara=vcx.ma_gara),'''') 
			          END AS ma_gara, 
			          A.ghi_chu, 
			          so_tienugd so_tienUbandau, 
			          vcx.SoTienGtbt, 
			          tnds.SoTienGtbt SoTienGtbtTNDS, 
			          tsk.SoTienGtbt SoTienGtbtKhac,					 			
			          isnull(ng_lienhe, '''') AS ng_lienhe, 
			          isnull(dien_thoai, '''') AS dien_thoai, 
			          isnull(dien_thoai_ndbh, '''') AS dien_thoai_ndbh, 
			          ma_lhsbt AS ma_lhsbt_new, 
			     (SELECT isnull(sum(so_tientt), 0) 
			      FROM hsgd_dx with(nolock) 
			      WHERE pr_key_dx<>0 and (pr_key_dx=vcx.pr_key or pr_key_dx = tnds.pr_key)) AS sum_tienthaythe, 
			     (SELECT isnull(sum(so_tienph), 0) 
			      FROM hsgd_dx with(nolock) 
			      WHERE pr_key_dx<>0 and (pr_key_dx=vcx.pr_key or pr_key_dx = tnds.pr_key)) AS sum_tiensuachua, 
			     (SELECT isnull(sum(so_tienson), 0) 
			      FROM hsgd_dx with(nolock) 
			      WHERE pr_key_dx<>0 and (pr_key_dx=vcx.pr_key or pr_key_dx = tnds.pr_key)) AS sum_tienson, 
				  (SELECT isnull(sum(so_tien_doitru), 0) 
			      FROM hsgd_dx with(nolock) 
			      WHERE pr_key_dx<>0 and (pr_key_dx=vcx.pr_key)) AS sum_sotiendoitru_vcx, 
				  (SELECT isnull(sum(so_tien_doitru), 0) 
			      FROM hsgd_dx with(nolock) 
			      WHERE pr_key_dx<>0 and (pr_key_dx = tnds.pr_key)) AS sum_sotiendoitru_tnds, 
			     (SELECT ISNULL((ROUND(ISNULL((SUM(so_tientt) + SUM(so_tienph)+ SUM(so_tienson)),0)+ISNULL(SUM(so_tien_vat), 0) -(ISNULL(SUM(so_tienggsc), 0))-(ISNULL(SUM(so_tien_doitru), 0)) -(ISNULL(iif(SUM(sum_so_tien_giamtru)=0, vcx.SoTienGtbt, SUM(sum_so_tien_giamtru)), 0)), 0)-vcx.so_tienctkh),0) st_bl 
			      FROM 
			        (SELECT so_tientt, 
			                so_tienph, 
			                so_tienson, 
			                ((isnull(so_tientt, 0) + isnull(so_tienph, 0) + isnull(so_tienson, 0))*(CAST(vat_sc AS int)*0.01)) AS so_tien_vat, 
			                ((so_tientt + so_tienph+ so_tienson) + (so_tientt + so_tienph+ so_tienson)*iif(vat_sc<>0, 1.0*vat_sc/100, 0)) AS so_tienttsc, 
			                ((((so_tientt+so_tientt*iif(vat_sc<>0, 1.0*vat_sc/100, 0))-((so_tientt+so_tientt*iif(vat_sc<>0, 1.0*vat_sc/100, 0))*(IIF(vcx.tylegg_phutungvcx <> 0, vcx.tylegg_phutungvcx / 100, 0))))+(((so_tienph+so_tienson)+(so_tienph+so_tienson)*iif(vat_sc<>0, 1.0*vat_sc/100, 0))-(((so_tienph+so_tienson)+(so_tienph+so_tienson)*iif(vat_sc<>0, 1.0*vat_sc/100, 0))*(IIF(vcx.tylegg_suachuavcx <> 0, vcx.tylegg_suachuavcx/ 100, 0)))))*giam_tru_bt/100) AS sum_so_tien_giamtru, 
			                ((so_tientt+so_tientt*iif(vat_sc<>0, 1.0*vat_sc/100, 0))*(IIF(vcx.tylegg_phutungvcx <> 0, vcx.tylegg_phutungvcx / 100, 0))+((so_tienph+so_tienson)+(so_tienph+so_tienson)*iif(vat_sc<>0, 1.0*vat_sc/100, 0))*(IIF(vcx.tylegg_suachuavcx <> 0, vcx.tylegg_suachuavcx / 100, 0))) AS so_tienggsc , 
			                so_tien_doitru 
			         FROM hsgd_dx with(nolock) 
			         WHERE pr_key_dx<>0 and pr_key_dx = vcx.pr_key) J) st_bl_vcx, 
			     (SELECT ISNULL((ROUND(ISNULL((SUM(so_tientt) + SUM(so_tienph)+ SUM(so_tienson)),0)+ISNULL(SUM(so_tien_vat), 0) -(ISNULL(SUM(so_tienggsc), 0)) -(ISNULL(SUM(so_tien_doitru), 0)) -(ISNULL(iif(SUM(sum_so_tien_giamtru)=0, tnds.SoTienGtbt, SUM(sum_so_tien_giamtru)), 0)), 0)-tnds.so_tienctkh),0) st_bl 
			      FROM 
			        (SELECT so_tientt, 
			                so_tienph, 
			                so_tienson, 
			                ((isnull(so_tientt, 0) + isnull(so_tienph, 0) + isnull(so_tienson, 0))*(CAST(vat_sc AS int)*0.01)) AS so_tien_vat, 
			                ((so_tientt + so_tienph+ so_tienson) + (so_tientt + so_tienph+ so_tienson)*iif(vat_sc<>0, 1.0*vat_sc/100, 0)) AS so_tienttsc, 
			                ((((so_tientt+so_tientt*iif(vat_sc<>0, 1.0*vat_sc/100, 0))-((so_tientt+so_tientt*iif(vat_sc<>0, 1.0*vat_sc/100, 0))*(IIF(tnds.tylegg_phutungvcx <> 0, tnds.tylegg_phutungvcx / 100, 0))))+(((so_tienph+so_tienson)+(so_tienph+so_tienson)*iif(vat_sc<>0, 1.0*vat_sc/100, 0))-(((so_tienph+so_tienson)+(so_tienph+so_tienson)*iif(vat_sc<>0, 1.0*vat_sc/100, 0))*(IIF(tnds.tylegg_suachuavcx <> 0, tnds.tylegg_suachuavcx / 100, 0)))))*giam_tru_bt/100) AS sum_so_tien_giamtru, 
			                ((so_tientt+so_tientt*iif(vat_sc<>0, 1.0*vat_sc/100, 0))*(IIF(tnds.tylegg_phutungvcx <> 0, tnds.tylegg_phutungvcx / 100, 0))+((so_tienph+so_tienson)+(so_tienph+so_tienson)*iif(vat_sc<>0, 1.0*vat_sc/100, 0))*(IIF(tnds.tylegg_suachuavcx <> 0, tnds.tylegg_suachuavcx / 100, 0))) AS so_tienggsc , 
			                so_tien_doitru 
			         FROM hsgd_dx with(nolock) 
			         WHERE pr_key_dx<>0 and  pr_key_dx = tnds.pr_key) K) AS st_bl_tnds, 
			     (SELECT isnull(sum(so_tientt), 0)+isnull(sum(so_tienph), 0)+isnull(sum(so_tienson), 0) 
			      FROM hsgd_dx with(nolock) 
			      WHERE pr_key_dx<>0 and pr_key_dx = vcx.pr_key) AS so_tienugddx_vcx, 
			     (SELECT isnull(sum(so_tienpdtt), 0)+isnull(sum(so_tienpdsc), 0) 
			      FROM hsgd_dx with(nolock) 
			      WHERE pr_key_dx<>0 and pr_key_dx = vcx.pr_key) AS tien_pheduyet_vcx, 
			     (SELECT isnull(sum(so_tientt), 0)+isnull(sum(so_tienph), 0)+isnull(sum(so_tienson), 0) 
			      FROM hsgd_dx with(nolock) 
			      WHERE pr_key_dx<>0 and pr_key_dx = tnds.pr_key and tnds.ma_dkhoan =''05010101'') AS so_tienugddx_tnds_nguoi, 
			     (SELECT isnull(sum(so_tienpdtt), 0)+isnull(sum(so_tienpdsc), 0) 
			      FROM hsgd_dx with(nolock) 
			      WHERE pr_key_dx<>0 and pr_key_dx = tnds.pr_key and tnds.ma_dkhoan =''05010101'') AS tien_pheduyet_tnds_nguoi, 
			     (SELECT isnull(sum(so_tientt), 0)+isnull(sum(so_tienph), 0)+isnull(sum(so_tienson), 0) 
			      FROM hsgd_dx with(nolock) 
			      WHERE pr_key_dx<>0 and pr_key_dx = tnds.pr_key and tnds.ma_dkhoan =''05010102'') AS so_tienugddx_tnds, 
			     (SELECT isnull(sum(so_tienpdtt), 0)+isnull(sum(so_tienpdsc), 0) 
			      FROM hsgd_dx with(nolock) 
			      WHERE pr_key_dx<>0 and pr_key_dx = tnds.pr_key and tnds.ma_dkhoan =''05010102'') AS tien_pheduyet_tnds, 
			     (SELECT isnull(sum(so_tientt), 0)+isnull(sum(so_tiensc), 0) 
			      FROM hsgd_dx_tsk with(nolock) 
			      WHERE pr_key_dx<>0 and pr_key_dx = tsk.pr_key) AS so_tienugddx_khac, 
			     (SELECT isnull(sum(so_tienpdtt), 0)+isnull(sum(so_tienpdsc), 0) 
			      FROM hsgd_dx_tsk with(nolock) 
			      WHERE pr_key_dx<>0 and pr_key_dx = tsk.pr_key) AS tien_pheduyet_khac
			      ,CAST(NULL AS DATETIME) AS ngay_duyettpc
				  ,cast('''' AS nvarchar(100)) cbott, 
			          CAST(NULL AS DATETIME) ngay_bstt, 
			          isnull(vcx.ghi_chudx, '''') ghi_chudx, 
			          isnull(ghi_chudxtt, '''') ghi_chudxtt, 
			          isnull(tnds.ghi_chudx, '''')ghi_chudx_tnds, 
			          isnull(ghi_chudx_tndstt, '''')ghi_chudx_tndstt, 
			          isnull(tsk.ghi_chudx, '''')ghi_chudx_tsk, 
			          isnull(ghi_chudx_tsktt, '''')ghi_chudx_tsktt, 
			          CASE 
			              WHEN vcx.vat=1 THEN 10 
			              ELSE '''' 
			          END AS vat, 
			          vcx.so_tienctkh, 
			          vcx.lydo_ctkh, 
			          CASE 
			              WHEN tnds.vat=1 THEN 10 
			              ELSE '''' 
			          END AS vat_tnds, 
			          tnds.lydo_ctkh lydo_ctkh_tnds, 
			          tnds.so_tienctkh so_tienctkh_tnds, 
			          vcx.tylegg_phutungvcx, 
			          vcx.tylegg_suachuavcx, 
			          tnds.tylegg_phutungvcx tylegg_phutungtnds, 
			          tnds.tylegg_suachuavcx tylegg_suachuatnds, 
					(SELECT CASE 
			                 WHEN A.hsgd_tpc = 1 THEN sum(B.so_tienpdtt)*vcx.tylegg_phutungvcx/100 
			                 ELSE sum(B.so_tientt)*vcx.tylegg_phutungvcx/100 
			             END AS ggphutung 
			      FROM hsgd_dx B with(nolock) 
			      WHERE pr_key_dx<>0 and B.pr_key_dx = vcx.pr_key) AS ggphutungvcx, 
			     (SELECT CASE 
			                 WHEN A.hsgd_tpc = 1 THEN sum(B.so_tienpdsc)*vcx.tylegg_suachuavcx/100 
			                 ELSE (sum(B.so_tienph)+SUM(B.so_tienson))*vcx.tylegg_suachuavcx/100 
			             END AS ggsuachua 
			      FROM hsgd_dx B  with(nolock) 
			      WHERE pr_key_dx<>0 and pr_key_dx = vcx.pr_key) AS ggsuachuavcx, 
			     (SELECT CASE 
			                 WHEN A.hsgd_tpc = 1 THEN sum(B.so_tienpdtt)*tnds.tylegg_phutungvcx/100 
			                 ELSE sum(B.so_tientt)*tnds.tylegg_phutungvcx/100 
			             END AS ggphutung 
			      FROM hsgd_dx B with(nolock) 
			      WHERE pr_key_dx<>0 and pr_key_dx = tnds.pr_key) AS ggphutungthds, 
			     (SELECT CASE 
			                 WHEN A.hsgd_tpc = 1 THEN sum(B.so_tienpdsc)*tnds.tylegg_suachuavcx/100 
			                 ELSE (sum(B.so_tienph)+SUM(B.so_tienson))*tnds.tylegg_suachuavcx/100 
			             END AS ggsuachua 
			      FROM hsgd_dx B with(nolock) 
			      WHERE pr_key_dx<>0 and pr_key_dx = tnds.pr_key) AS ggsuachuatnds , 
			          tsk.so_tienctkh so_tienctkh_tsk, 
			          tsk.lydo_ctkh lydo_ctkh_tsk, 
			          ma_nguyen_nhan_ttat, 
			     (SELECT top 1 ten_nntt 
			      FROM dm_nguyennhan_tonthat 
			      WHERE ma_nntt=A.ma_nguyen_nhan_ttat) AS ten_nguyen_nhan_ttat, 
			 isnull(tnds.ma_dkhoan, '''') ma_dkhoan, 			
			 case when  isnull(vcx.ma_loai_dongco, '''')=''01'' then N''Động cơ đốt trong'' when  isnull(vcx.ma_loai_dongco, '''')=''02'' then N''Động cơ điện'' when  isnull(vcx.ma_loai_dongco, '''')=''03'' then N''Động cơ lai điện'' when  isnull(vcx.ma_loai_dongco, '''')=''04'' then N''Động cơ khác'' else '''' end ten_loai_dongco,
			 isnull(vcx.sotien_ttpin,0) sotien_ttpin, 
			 cast('''' as nvarchar(30)) as ten_cbotrinh 
			  ,isnull(vai_tro,'''') vai_tro
			  ,isnull(tyle_tg,0) tyle_tg
			  ,isnull(so_hsbt,'''') so_hsbt
			  ,CAST(NULL AS DATETIME) ngay_pd_tt
			  ,cast('''' as nvarchar(50)) as ten_nguoi_duyet
			  ,cast('''' as nvarchar(50)) as nguon_tao
			  ,case when (select top 1 loai_user from dm_user where cast(Oid as nvarchar(50))=cast(A.ma_user as nvarchar(50))) in (4,7) then A.nguoi_xuly else cast(A.ma_user as nvarchar(50)) end ma_cbott
			  ,cast('''' as nvarchar(50)) as canbo_pdtt
			  ,CAST(NULL AS DATETIME) ngay_dutlieu
			  FROM hsgd_ctu A with(nolock)  
			   left join (select * from hsgd_dx_ct with(nolock) where ma_sp =''050104'') vcx on a.pr_key_bt = vcx.pr_key_hsbt_ctu and vcx.pr_key_hsbt_ctu <> 0 
			   left join (select * from hsgd_dx_ct with(nolock) where ma_sp =''050101'') tnds on a.pr_key_bt = tnds.pr_key_hsbt_ctu and tnds.pr_key_hsbt_ctu <> 0 
			   left join (select * from hsgd_dx_ct with(nolock) where ma_sp not in (''050101'',''050104'')) tsk on a.pr_key_bt = tsk.pr_key_hsbt_ctu  and tsk.pr_key_hsbt_ctu <> 0  
			   where 1=1';


			   IF @MaDonVi IS NOT NULL AND LTRIM(RTRIM(@MaDonVi)) <> N''
					SET @sql += N' and A.ma_donvi IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT(@MaDonVi,'',''))';				
				IF @TuNgay IS NOT NULL AND @TuNgay <> N''
					SET @sql += N' AND CONVERT(date, ngay_ctu, 103) >= CONVERT(date,@TuNgay, 103)';
				IF @DenNgay IS NOT NULL AND @DenNgay <> N''
					SET @sql += N' AND CONVERT(date, ngay_ctu, 103) <= CONVERT(date,@DenNgay, 103)';	
				IF @MaTtrangGd IS NOT NULL AND @MaTtrangGd <> N''
					SET @sql += N' AND ma_ttrang_gd IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT(@MaTtrangGd, '',''))';
				IF @LoaiHsgd IS NOT NULL AND @LoaiHsgd <> N''
					SET @sql += N' AND ma_lhsbt IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT(@LoaiHsgd, '',''))';
				IF @SoHsgd IS NOT NULL AND @SoHsgd <> N''
					--SET @sql += N' AND so_hsgd LIKE N''%' + @SoHsgd + N'%''';
					SET @sql += N' AND so_hsgd LIKE N''%'' + @SoHsgd + N''%''';
				IF @SoAnChi IS NOT NULL AND @SoAnChi <> N''
					--SET @sql += N' AND so_seri LIKE N''%' + @SoAnChi + N'%''';
					SET @sql += N' AND so_seri LIKE N''%'' + @SoAnChi + N''%''';
				IF @BienKSoat IS NOT NULL AND @BienKSoat <> N''
					--SET @sql += N' AND replace(replace(replace(upper(bien_ksoat),''-'',''''),''.'',''''),'' '','''') LIKE N''%' + @BienKSoat + N'%''';	
					SET @sql += N' AND replace(replace(replace(upper(bien_ksoat),''-'',''''),''.'',''''),'' '','''') LIKE N''%'' + @BienKSoat + N''%''';
					
				IF @IsTPC IS NOT NULL
					SET @sql += N' AND hsgd_tpc = ' + CAST(@IsTPC AS NVARCHAR(1));	  	

				SET @ParamDef = N'
					@MaDonVi NVARCHAR(max),
					@MaDonViTt NVARCHAR(max),
					@TuNgay NVARCHAR(20),
					@DenNgay NVARCHAR(20),					
					@MaTtrangGd NVARCHAR(50),
					@LoaiHsgd NVARCHAR(20),
					@SoHsgd NVARCHAR(50),
					@SoAnChi NVARCHAR(50),
					@BienKSoat NVARCHAR(50),
					@IsTPC BIT
				';
				--print @SQL;
				-- 🔹 3. Gọi thực thi SQL động với tất cả biến truyền vào
				EXEC sp_executesql 
					@SQL,
					@ParamDef,
					@MaDonVi = @MaDonVi,
					@MaDonViTt = @MaDonViTt,
					@TuNgay = @TuNgay,
					@DenNgay = @DenNgay,					
					@MaTtrangGd = @MaTtrangGd,
					@LoaiHsgd = @LoaiHsgd,
					@SoHsgd = @SoHsgd,
					@SoAnChi = @SoAnChi,
					@BienKSoat = @BienKSoat,
					@IsTPC = @IsTPC;	

					
					
END TRY
BEGIN CATCH
    DECLARE @err NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @line INT = ERROR_LINE();
    DECLARE @proc NVARCHAR(128) = ERROR_PROCEDURE();

    PRINT '===> Lỗi tại dòng ' + CAST(@line AS NVARCHAR(10));
    PRINT '===> Thủ tục: ' + ISNULL(@proc, 'Không xác định');
    PRINT '===> Thông báo: ' + @err;
    
END CATCH
			
			IF @maCanBo IS NOT NULL AND @maCanBo <> N''
			begin
				SET @sql=N'DELETE from #Temp_basehsgd where LOWER(ma_cbott) not in (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT(LOWER(@maCanBo), '',''))';			
				SET @ParamDef = N'
				@maCanBo NVARCHAR(max)
				';
				EXEC sp_executesql @sql,@ParamDef,@maCanBo = @maCanBo;
			end						
			--update tên giám định viên
			update a 
			set a.cbott=b.ten_user+'('+replace(b.Mail,'@pvi.com.vn','')+')'
			from #Temp_basehsgd a inner join dm_user b WITH (NOLOCK) on a.ma_cbott=cast(b.Oid as nvarchar(50)); 
			
				--update ngày phê duyệt-----------------------------------------------------------------------
			    --SELECT max(n.ngay_cnhat) ngay_cnhat,t.pr_key_hsgd into #temp01 FROM hsgd_ttrinh t WITH (NOLOCK) INNER JOIN hsgd_ttrinh_nky n WITH (NOLOCK) ON t.pr_key = n.fr_key AND n.act = 'KyHoSo' WHERE t.pr_key_hsgd in (select pr_key from #Temp_basehsgd) group by t.pr_key_hsgd;
				SELECT pr_key_hsgd, user_chuyen, ngay_cnhat INTO #temp01 FROM ( SELECT t.pr_key_hsgd,n.user_chuyen,n.ngay_cnhat, ROW_NUMBER() OVER (PARTITION BY t.pr_key_hsgd ORDER BY n.ngay_cnhat DESC) AS rn  FROM hsgd_ttrinh t WITH (NOLOCK) INNER JOIN hsgd_ttrinh_nky n WITH (NOLOCK) ON t.pr_key = n.fr_key AND n.act = 'KyHoSo' WHERE t.pr_key_hsgd IN (SELECT pr_key FROM #Temp_basehsgd)) x WHERE rn = 1;
				update a 
				set ngay_pd_tt=b.ngay_cnhat,canbo_pdtt=c.ten_user 
				from #Temp_basehsgd a inner join #temp01 b on a.pr_key=b.pr_key_hsgd inner join dm_user c on cast(c.Oid as nvarchar(50)) =b.user_chuyen;;
				
				--Update tên cán bộ trình-------------------------------------------------------------------
				SELECT max(n.ngay_cnhat) ngay_cnhat,t.pr_key_hsgd,n.user_chuyen into #temp02 FROM hsgd_ttrinh t WITH (NOLOCK) INNER JOIN hsgd_ttrinh_nky n WITH (NOLOCK) ON t.pr_key = n.fr_key AND n.act = 'CREATETOTRINH' WHERE t.pr_key_hsgd in (select pr_key from #Temp_basehsgd) group by t.pr_key_hsgd,n.user_chuyen;
				update a 
				set ten_cbotrinh=c.ten_user 
				from #Temp_basehsgd a inner join #temp02 b on a.pr_key=b.pr_key_hsgd inner join dm_user c on cast(c.Oid as nvarchar(50)) =b.user_chuyen;

				--Update người tạo hồ sơ và nguồn tạo-------------------------------------------------------------------
				SELECT a.pr_key AS pr_key_hsgd,nk.ma_user,nk.ghi_chu INTO #temp03 
					FROM #Temp_basehsgd a
						OUTER APPLY (
							SELECT TOP 1 pr_key, ma_user, ghi_chu
							FROM nhat_ky nk WITH (NOLOCK)
							WHERE nk.ma_ttrang_gd IN ('1','2','9')
							  AND nk.fr_key = a.pr_key
							ORDER BY nk.pr_key ASC
						) nk;
				update a 
				set gdv=c.ten_user,nguon_tao=b.ghi_chu 
				from #Temp_basehsgd a inner join #temp03 b on a.pr_key=b.pr_key_hsgd inner join dm_user c on cast(c.Oid as nvarchar(50)) =cast(b.ma_user as nvarchar(50));

				--Update ngày duyệt, người duyệt-------------------------------------------------------------------
				SELECT a.pr_key AS pr_key_hsgd,nk.ngay_capnhat,nk.ma_user INTO #temp04 
					FROM #Temp_basehsgd a
						OUTER APPLY (
							SELECT TOP 1 ngay_capnhat, ma_user
							FROM nhat_ky nk WITH (NOLOCK)
							WHERE nk.ma_ttrang_gd ='6' and nk.ten_ttrang_gd=N'Đã duyệt'
							  AND nk.fr_key = a.pr_key
							ORDER BY nk.pr_key DESC
						) nk;
				update a 
				set ngay_duyet=b.ngay_capnhat,ngay_duyettpc=b.ngay_capnhat,ten_nguoi_duyet=c.ten_user 
				from #Temp_basehsgd a inner join #temp04 b on a.pr_key=b.pr_key_hsgd inner join dm_user c on cast(c.Oid as nvarchar(50)) =cast(b.ma_user as nvarchar(50));

				--Update ngày bổ sung-------------------------------------------------------------------
				select max(ngay_capnhat) ngay_capnhat,fr_key pr_key_hsgd into #temp05 from nhat_ky WITH (NOLOCK) where ma_ttrang_gd ='5' and fr_key in (select pr_key from #Temp_basehsgd) group by fr_key

				update a 
				set ngay_bstt=b.ngay_capnhat 
				from #Temp_basehsgd a inner join #temp05 b on a.pr_key=b.pr_key_hsgd;

				--Update ngày hủy-------------------------------------------------------------------
				select max(ngay_capnhat) ngay_capnhat,fr_key pr_key_hsgd into #temp06 from nhat_ky WITH (NOLOCK) where ma_ttrang_gd ='7' and fr_key in (select pr_key from #Temp_basehsgd) group by fr_key
				update a 
				set ngay_huy_hs=b.ngay_capnhat 
				from #Temp_basehsgd a inner join #temp06 b on a.pr_key=b.pr_key_hsgd;
							
				UPDATE #Temp_basehsgd set ct_nd=DATEDIFF(day,ngayct,ngay_duyet);	

				--Update Ngay du du lieu lay theo to trinh gan nhat---------------------------------
				SELECT a.pr_key AS pr_key_hsgd,nk.ngay_dutlieu INTO #temp07 
					FROM #Temp_basehsgd a
						OUTER APPLY (
							SELECT TOP 1 ngay_dutlieu
							FROM hsgd_ttrinh nk WITH (NOLOCK)
							WHERE nk.pr_key_hsgd = a.pr_key
							ORDER BY nk.pr_key DESC
						) nk;
				update a 
				set ngay_dutlieu=b.ngay_dutlieu 
				from #Temp_basehsgd a inner join #temp07 b on a.pr_key=b.pr_key_hsgd;

			   select @ma_user=ma_user,@loai_user=loai_user,@pquyen_upl_hinh_anh=pquyen_upl_hinh_anh from dm_user WITH (NOLOCK) where Mail=@email_run;
				--   --update lại ng_lienhe,dien_thoai,dien_thoai_ndbh
				 UPDATE #Temp_basehsgd
					SET ng_lienhe = 
							CASE 
								WHEN @pquyen_upl_hinh_anh = 1 
									 OR @ma_user = 'quyenvm' 
								THEN ng_lienhe 
								ELSE '' 
							END,
    
						dien_thoai = 
							CASE 
								WHEN @pquyen_upl_hinh_anh = 1 
									 OR @ma_user = 'quyenvm' 
								THEN 
									CASE 
										WHEN @loai_user NOT IN (6,1) AND ma_lhsbt_new = '3' THEN '' 
										ELSE dien_thoai 
									END
								ELSE ''
							END,

						dien_thoai_ndbh = 
							CASE 
								WHEN @pquyen_upl_hinh_anh = 1 
									 OR @ma_user = 'quyenvm' 
								THEN 
									CASE 
										WHEN @loai_user NOT IN (6,1) AND ma_lhsbt_new = '3' THEN '' 
										ELSE dien_thoai_ndbh 
									END
								ELSE ''
							END;
				
			set @sql=N'select ten_nguoi_duyet,pr_key,ten_donvi,ten_donvi_tt,ten_khach,so_donbh,so_hsgd,so_seri,bien_ksoat,max(hieu_xe) hieu_xe,max(loai_xe) loai_xe,ngay_dau_seri,ngay_cuoi_seri,ngay_ctu,ngay_tthat,dia_diemtt,nguyen_nhan_ttat,so_ngaybh,tb_tt,ct_nd,ngay_huy_hs,ng_lienhe,dien_thoai,dien_thoai_ndbh, ma_lhsbt_new, thoi_gian_xly,'''' ma_user,  so_lan_gd, so_tienu, so_tienp, gdv, tinh_trang, ma_lhsbt,hsgd_tpc,max(ten_gara) ten_gara,max(ten_tat_gara) ten_tat_gara,max(ma_gara) ma_gara,ghi_chu, so_tienUbandau,sum(sum_tienthaythe) sum_tienthaythe,sum(sum_tiensuachua) sum_tiensuachua,sum(sum_tienson) sum_tienson,sum(sum_sotiendoitru_vcx) sum_sotiendoitru_vcx,sum(sum_sotiendoitru_tnds) sum_sotiendoitru_tnds,sum(so_tienugddx_vcx) so_tienugddx_vcx,sum(tien_pheduyet_vcx) tien_pheduyet_vcx,sum(so_tienugddx_tnds) so_tienugddx_tnds,sum(tien_pheduyet_tnds) tien_pheduyet_tnds,sum(so_tienugddx_tnds_nguoi) so_tienugddx_tnds_nguoi,sum(tien_pheduyet_tnds_nguoi) tien_pheduyet_tnds_nguoi,sum(so_tienugddx_khac) so_tienugddx_khac,sum(tien_pheduyet_khac) tien_pheduyet_khac, ngay_duyettpc, cbott, ngay_bstt,pr_key_bt,max(ghi_chudx) ghi_chudx,ghi_chudxtt,max(ghi_chudx_tnds) ghi_chudx_tnds,ghi_chudx_tndstt,max(ghi_chudx_tsk) ghi_chudx_tsk,ghi_chudx_tsktt,max(vat) vat,max(so_tienctkh) so_tienctkh,max(lydo_ctkh) lydo_ctkh, max(vat_tnds) vat_tnds,max(lydo_ctkh_tnds) lydo_ctkh_tnds,max(so_tienctkh_tnds) so_tienctkh_tnds,max(tylegg_phutungvcx) tylegg_phutungvcx,max(tylegg_suachuavcx) tylegg_suachuavcx,max(tylegg_phutungtnds) tylegg_phutungtnds,max(tylegg_suachuatnds) tylegg_suachuatnds,sum(ggphutungvcx) ggphutungvcx,sum(ggsuachuavcx) ggsuachuavcx,sum(ggphutungthds) ggphutungthds,sum(ggsuachuatnds) ggsuachuatnds, max(so_tienctkh_tsk) so_tienctkh_tsk,max(lydo_ctkh_tsk) lydo_ctkh_tsk,ma_nguyen_nhan_ttat, ten_nguyen_nhan_ttat, sum(st_bl_vcx) st_bl_vcx,sum(st_bl_tnds) st_bl_tnds,max(SoTienGtbt) SoTienGtbt,max(SoTienGtbtTNDS) SoTienGtbtTNDS,max(SoTienGtbtKhac) SoTienGtbtKhac,isnull(so_hsbt,'''') so_hsbt,vai_tro,tyle_tg,ngay_pd_tt,ma_dkhoan,ten_loai_dongco,sotien_ttpin,ten_cbotrinh,nguon_tao,canbo_pdtt,ngay_dutlieu 
				       from #Temp_basehsgd
				       where 1=1 ';
				 IF @TuNgayDuyettpc IS NOT NULL AND @TuNgayDuyettpc <> N''
					SET @sql += N' AND CONVERT(date, ngay_duyettpc, 103) >= CONVERT(date, @TuNgayDuyettpc, 103)';
				IF @DenNgayDuyettpc IS NOT NULL AND @DenNgayDuyettpc <> N''
					SET @sql += N' AND CONVERT(date, ngay_duyettpc, 103) <= CONVERT(date,@DenNgayDuyettpc, 103)';				
				IF @TuNgayPDTT IS NOT NULL AND @TuNgayPDTT <> N''
					SET @sql += N' AND CONVERT(date, ngay_pd_tt, 103) >= CONVERT(date,@TuNgayPDTT, 103)';
				IF @DenNgayPDTT IS NOT NULL AND @DenNgayPDTT <> N''
					SET @sql += N' AND CONVERT(date, ngay_pd_tt, 103) <= CONVERT(date,@DenNgayPDTT, 103)';	
			set	@sql +=' group by ten_nguoi_duyet, pr_key,ten_donvi,ten_donvi_tt,ten_khach,so_donbh,so_hsgd,so_seri,bien_ksoat,ngay_dau_seri,ngay_cuoi_seri,ngay_ctu,ngay_tthat,dia_diemtt,nguyen_nhan_ttat,so_ngaybh,tb_tt,ct_nd,  ngay_huy_hs, ng_lienhe, dien_thoai, dien_thoai_ndbh, ma_lhsbt_new, thoi_gian_xly,so_lan_gd, so_tienu, so_tienp, gdv, tinh_trang, ma_lhsbt,hsgd_tpc,ghi_chu,so_tienUbandau, ngay_duyettpc, cbott, ngay_bstt,pr_key_bt,ghi_chudxtt,ghi_chudx_tndstt,ghi_chudx_tsktt,ma_nguyen_nhan_ttat, ten_nguyen_nhan_ttat,so_hsbt,vai_tro,tyle_tg,ngay_pd_tt,ma_dkhoan,ten_loai_dongco,sotien_ttpin,ten_cbotrinh,nguon_tao,canbo_pdtt,ngay_dutlieu  order by pr_key'
			SET @ParamDef = N'					
					@TuNgayDuyettpc NVARCHAR(20),
					@DenNgayDuyettpc NVARCHAR(20),
					@TuNgayPDTT NVARCHAR(20),
					@DenNgayPDTT NVARCHAR(20)					
				';

				EXEC sp_executesql 
					@SQL,
					@ParamDef,					
					@TuNgayDuyettpc = @TuNgayDuyettpc,
					@DenNgayDuyettpc = @DenNgayDuyettpc,
					@TuNgayPDTT = @TuNgayPDTT,
					@DenNgayPDTT = @DenNgayPDTT;

END






