
﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;


public partial class DmPban
{
    public string MaPban { get; set; } = null!;

    public string TenPban { get; set; } = null!;

    public string TenPbanEng { get; set; } = null!;

    public string TenTat { get; set; } = null!;

    public bool ViewAll { get; set; }

    public string MaDonvi { get; set; } = null!;

    public string MaDonviPban { get; set; } = null!;
}
