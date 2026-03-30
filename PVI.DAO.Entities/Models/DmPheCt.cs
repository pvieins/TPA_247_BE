using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;


public partial class DmPheCt
{
    public decimal PrKey { get; set; }

    public decimal FrKey { get; set; }

    public string MaDonvi { get; set; } = null!;

    public bool KhoaSo { get; set; }

    /// <summary>
    /// Neu co gia tri bang 1 thi chi co admin moi co quyen mo khoa
    /// </summary>
    public bool KhoaSoFull { get; set; }

    public string KyKhoa { get; set; } = null!;

    public DateTime? TuNgay { get; set; }

    public DateTime? DenNgay { get; set; }
}
