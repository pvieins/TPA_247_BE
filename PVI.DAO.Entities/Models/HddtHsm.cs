using System;
using System.Collections.Generic;

 namespace PVI.DAO.Entities.Models
{
    public partial class HddtHsm
    {
        public int PrKey { get; set; }
        public string MaDonvi { get; set; } = null!;
        public string PartitionAlias { get; set; } = null!;
        public string PartitionSerial { get; set; } = null!;
        public string PrivateKeyAlias { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateTime NgayHluc { get; set; }
        public string TvanUsername { get; set; } = null!;
        public string TvanPassword { get; set; } = null!;
        public string TaxCode { get; set; } = null!;
        public string Mst { get; set; } = null!;
        public string SerialNumber { get; set; } = null!;
    }
}
