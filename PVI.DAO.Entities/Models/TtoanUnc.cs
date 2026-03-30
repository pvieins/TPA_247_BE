using System;
using System.Collections.Generic;

namespace PVI.DAO.Entities.Models;

public partial class TtoanUnc
{
    public decimal TxId { get; set; }

    public decimal FrKey { get; set; }

    public DateTime? TxDt { get; set; }

    public string TxDesc { get; set; } = null!;

    public string SignChecker { get; set; } = null!;

    public string SignMaker { get; set; } = null!;

    public string BenBnkCd { get; set; } = null!;

    public int TrangThai { get; set; }

    public string GhiChu { get; set; } = null!;

    public DateTime? TxnDate { get; set; }

    public string FrAcctId { get; set; } = null!;

    public string FrAcctNm { get; set; } = null!;

    public string UsrChecker { get; set; } = null!;

    public string UsrMaker { get; set; } = null!;

    public string MessageId { get; set; } = null!;

    public decimal TxIdTct { get; set; }

    public int TrangThaiTkhoan { get; set; }

    public DateTime? NgayGuiUnc { get; set; }

    public decimal TxAmt { get; set; }

    public string ToAccId { get; set; } = null!;

    public string ToAccNm { get; set; } = null!;
}
