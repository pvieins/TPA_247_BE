using PVI.DAO.Entities.Models;
using PVI.Helper;
using System.Linq.Expressions;
using static iTextSharp.text.pdf.events.IndexEvents;

namespace PVI.Repository.Interfaces;

// Interface cho danh mục Gara
// khanhlh - 26/07/2024

public interface IDmGaraRepository : IGenericRepository<DmGaRa>
{
    // Lấy đầy đủ thông tin gara
    Task<List<DmGaRa>> getGarageList(int pageNumber, int limit);

    PagedList<DmGaRa> searchFilterGarage(DmGaraFilter searchTarget);
    Task<List<GaRaView>> getAllGara(DmGaraFilter searchTarget);
    // Update thông tin gara
    string updateGarage(DmGaRa garage, string currentUserEmail, string TkVnd,string NganHang,string bnkCode,string ten_ctk);

    // Đồng bộ gara qua Pias 
    string syncGarageFromPias();
}
