using AutoMapper;
using PVI.DAO.Entities.Models;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace PVI.Service.Request;

public  class HsbtDxRequest
{
    public List<HsgdDxRequest> hsgdDx { get; set; }
    public HsgdDxCtRequest hsgdDxCt { get; set; }
}
public class HsgdDxRequest
{
    public int PrKey { get; set; }

    public int FrKey { get; set; }// trước đây là pr_key bảng hsgd_ctu, giờ để 0 vì dùng cột PrKeyDx

    public string MaHmuc { get; set; } = null!;
    public string Hmuc { get; set; } = null!;// dùng cho HsgdDxTsk
    public decimal SoTientt { get; set; }

    public decimal SoTienph { get; set; }

    public decimal SoTienson { get; set; }
    public decimal SoTiensc { get; set; }//trong bảng HsgdDxTsk 

    public string GhiChudv { get; set; } = null!;

    public int LoaiDx { get; set; }

    public int VatSc { get; set; }

    public int GiamTruBt { get; set; }

    public bool ThuHoiTs { get; set; }

    public decimal? SoTienDoitru { get; set; }

    public decimal PrKeyDx { get; set; }// là PrKeyHsgdDxCt trong GetListPhaiTraBT
}

