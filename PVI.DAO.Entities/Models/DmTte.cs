using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVI.DAO.Entities.Models;

public partial class DmTte
{
    public string MaTte { get; set; }
    public string TenTte { get; set; }
    public string MaTteTageTik { get; set; }

}
