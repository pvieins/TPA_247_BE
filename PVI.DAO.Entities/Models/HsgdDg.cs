using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class HsgdDg
{
    public int PrKey { get; set; }

    public string DeXuat { get; set; } = null!;

    public decimal SoTien { get; set; }
    public DateTime? NgayBaoGia { get; set; }
    public DateTime? NgayDuyetGia { get; set; }

    public Guid? MaUser { get; set; }
    public Guid? MaUserDuyet { get; set; }

    public bool LoaiDg { get; set; }

    public int FrKey { get; set; }
    public DateTime? NgayCapNhat { get; set; }
    public bool Hienthi { get; set; }

    public virtual ICollection<HsgdDgCt> HsgdDgCts { get; } = new List<HsgdDgCt>();
}
