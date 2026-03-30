namespace PVI.DAO.Entities.Models;
using System.ComponentModel.DataAnnotations;



public partial class DmLoaiHsgd
{
    [Key]
    public string ma_loai_hsgd { get; set; }

    public string ten_loai_hsgd { get; set; }
}
