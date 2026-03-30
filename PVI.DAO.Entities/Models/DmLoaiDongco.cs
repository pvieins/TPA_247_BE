using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class DmLoaiDongco
{
    public int PrKey { get; set; }

    public string MaLoaiDongco { get; set; } = null!;

    public string TenLoaiDongco { get; set; } = null!;
}
