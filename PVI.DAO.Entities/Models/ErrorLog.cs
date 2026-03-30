using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class ErrorLog
{
    public int ErrorLogId { get; set; }

    public DateTime? ErrorDate { get; set; }

    public string? ErrorMsg { get; set; }

    public int? ErrorNumber { get; set; }

    public string ErrorProc { get; set; } = null!;

    public int? ErrorLine { get; set; }
}
