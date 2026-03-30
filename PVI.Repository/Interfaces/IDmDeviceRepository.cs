using PVI.DAO.Entities.Models;
using PVI.Helper;
using System.Linq.Expressions;
using static PVI.Repository.Repositories.DmDeviceRepository;


namespace PVI.Repository.Interfaces;

// Interface cho danh mục ủy quyền 
// khanhlh - 01/10/2024

public interface IDmDeviceRepository : IGenericRepository<DmDevice>
{
    DanhSachDevice getListDevice(int pageNumber, int limit, DmDevice searchTarget, string currentUserEmail);
    Task<string> createDevice(DmDevice device, string currentUserEmail);
    Task<string> updateDevice(int prKey, DmDevice device, string currentUserEmail);
}
