using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.DAO.Entities.Models;

public partial class DmLoaiHinhTd
{
    public string MaLoaiHinhTd { get; set; }
    public string TenLoaiHinhTd { get; set; }
}
