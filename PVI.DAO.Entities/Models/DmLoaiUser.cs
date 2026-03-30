using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class DmLoaiUser
{
    public int Id { get; set; }

    public int LoaiUser { get; set; }

    public string TenLoaiUser { get; set; } = null!;

    public DateTime NgayCnhat { get; set; }
}
