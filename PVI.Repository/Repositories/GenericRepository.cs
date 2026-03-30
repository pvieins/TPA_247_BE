using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using System.Transactions;
using PVI.DAO.Entities.Models;
using Newtonsoft.Json;
using ServiceReference1;
using System.Data;
using System.Text;

namespace PVI.Repository.Repositories;

public abstract class GenericRepository<T> where T : class
{
    protected readonly GdttContext _context;
    protected readonly Pvs2024Context _context_pias;
    protected readonly Pvs2024UpdateContext _context_pias_update;
    protected readonly PvsTcdContext _context_pvs_tcd;
    protected readonly Pvs2024TToanContext _context_pias_ttoan;
    protected readonly MY_PVIContext _context_my_pvi;
    protected readonly PvsHDDTContext _context_pias_hddt; 

    protected readonly Serilog.ILogger _logger;
    protected readonly IConfiguration _configuration;

    protected GenericRepository(GdttContext context, Serilog.ILogger logger, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }
    protected GenericRepository(GdttContext context, Pvs2024Context context_pias, Serilog.ILogger logger, IConfiguration configuration)
    {
        _context = context;
        _context_pias = context_pias;
        _logger = logger;
        _configuration = configuration;
    }
    protected GenericRepository(GdttContext context, Pvs2024Context context_pias, Pvs2024UpdateContext context_pias_update, Serilog.ILogger logger, IConfiguration configuration)
    {
        _context = context;
        _context_pias = context_pias;
        _context_pias_update = context_pias_update;
        _logger = logger;
        _configuration = configuration;
    }
    protected GenericRepository(GdttContext context, Pvs2024Context context_pias, Pvs2024UpdateContext context_pias_update,PvsTcdContext context_pvs_tcd, Serilog.ILogger logger, IConfiguration configuration)
    {
        _context = context;
        _context_pias = context_pias;
        _context_pias_update = context_pias_update;
        _context_pvs_tcd = context_pvs_tcd;
        _logger = logger;
        _configuration = configuration;
    }
    protected GenericRepository(GdttContext context, Pvs2024Context context_pias, Pvs2024UpdateContext context_pias_update, MY_PVIContext context_my_pvi, Serilog.ILogger logger, IConfiguration configuration)
    {
        _context = context;
        _context_pias = context_pias;
        _context_pias_update = context_pias_update;
        _context_my_pvi = context_my_pvi;
        _logger = logger;
        _configuration = configuration;
    }
    protected GenericRepository(GdttContext context, Pvs2024Context context_pias, Pvs2024UpdateContext context_pias_update, Pvs2024TToanContext context_pias_ttoan, Serilog.ILogger logger, IConfiguration configuration)
    {
        _context = context;
        _context_pias = context_pias;
        _context_pias_update = context_pias_update;
        _context_pias_ttoan = context_pias_ttoan;
        _logger = logger;
        _configuration = configuration;
    }

    // Context có chứa HDDt
    protected GenericRepository(GdttContext context, Pvs2024Context context_pias, Pvs2024UpdateContext context_pias_update, PvsHDDTContext context_pias_hddt, Serilog.ILogger logger, IConfiguration configuration)
    {
        _context = context;
        _context_pias = context_pias;
        _context_pias_update = context_pias_update;
        _context_pias_hddt = context_pias_hddt;
        _logger = logger;
        _configuration = configuration;
    }

    public IQueryable<T> FindAll()
    {

        return _context.Set<T>();
    }


    public async Task<T> GetById(string id)
    {
        return await _context.Set<T>().FindAsync(id);
    }


    public async Task<T> GetById(decimal id)
    {
        return await _context.Set<T>().FindAsync(id);
    }
    public  T GetByIdNoAsync(decimal id)
    {
        return  _context.Set<T>().Find(id);
    }
    public T GetByIdNoAsyncPias(decimal id)
    {
        return _context_pias_update.Set<T>().Find(id);
    }
    // Sử dụng đối với những entity có composite key, ví dụ như điểm trực.
    // Sử dụng trường PrKey cho id1 và trường khác cho Id2 (ví dụ mã điểm trực)
    public T GetByIdNoAsyncCompKey(int id1, string id2)
    {
        return _context.Set<T>().Find(id1, id2);
    }

    public async Task<T> GetById(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAll()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task Add(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
    }
    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }
    public void DeletePias(T entity)
    {
        _context_pias_update.Set<T>().Remove(entity);
    }
    public void DeleteAll(IEnumerable<T> entity)
    {
        _context.Set<T>().RemoveRange(entity);
    }
    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }
    public async Task<bool> CheckExists(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().AnyAsync(predicate);
    }
    public virtual async Task<T> GetEntityByCondition(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(predicate);
    }
    public virtual async Task<T> GetEntityByConditionPias(Expression<Func<T, bool>> predicate)
    {
        return await _context_pias_update.Set<T>().FirstOrDefaultAsync(predicate);
    }
    public virtual T GetEntityByConditionNoAsync(Expression<Func<T, bool>> predicate)
    {
        return _context.Set<T>().FirstOrDefault(predicate);
    }
    public virtual T GetEntityByConditionNoAsyncPias(Expression<Func<T, bool>> predicate)
    {
        return _context_pias_update.Set<T>().FirstOrDefault(predicate);
    }
    public virtual async Task<List<T>> GetListEntityByCondition(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().Where(predicate).ToListAsync();
    }
    public virtual List<T> GetListEntityByConditionNoAsync(Expression<Func<T, bool>> predicate)
    {
        return  _context.Set<T>().Where(predicate).ToList();
    }
    public virtual List<T> GetListEntityByConditionNoAsyncPias(Expression<Func<T, bool>> predicate)
    {
        return _context_pias_update.Set<T>().Where(predicate).ToList();
    }
    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
    public async Task SaveAsyncPias()
    {
        await _context_pias_update.SaveChangesAsync();
    }
    public void Save()
    {
         _context.SaveChanges();
    }
    public async Task<List<T>> ToListWithNoLockAsync<T>(IQueryable<T> query, CancellationToken cancellationToken = default, Expression<Func<T, bool>> expression = null)
    {
        List<T> result = default;
        using (var scope = CreateTransactionAsync())
        {
            if (expression is object)
            {
                query = query.Where(expression);
            }
            result = await query.ToListAsync(cancellationToken);
            scope.Complete();
        }
        return result;
    }
    public List<T> ToListWithNoLock<T>(IQueryable<T> query, Expression<Func<T, bool>> expression = null)
    {
        List<T> result = default;
        using (var scope = CreateTransaction())
        {
            if (expression is object)
            {
                query = query.Where(expression);
            }
            result = query.ToList();
            scope.Complete();
        }
        return result;
    }
    public T FirstOrDefaultWithNoLock<T>(IQueryable<T> query, Expression<Func<T, bool>> expression = null)
    {
        using (var scope = CreateTransaction())
        {
            if (expression is object)
            {
                query = query.Where(expression);
            }
            T result = query.FirstOrDefault();
            scope.Complete();
            return result;
        }
    }

    public async Task<T> FirstOrDefaultWithNoLockAsync<T>(IQueryable<T> query, CancellationToken cancellationToken = default, Expression<Func<T, bool>> expression = null)
    {
        using (var scope = CreateTransactionAsync())
        {
            if (expression is object)
            {
                query = query.Where(expression);
            }
            T result = await query.FirstOrDefaultAsync(cancellationToken);
            scope.Complete();
            return result;
        }
    }

    public static TransactionScope CreateTransaction()
    {
        return new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions()
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            });
    }

    public static TransactionScope CreateTransactionAsync()
    {
        return new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions()
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            },
            TransactionScopeAsyncFlowOption.Enabled);
    }
    public SeriPhiBH Get_SoPhiBH(string so_donbh, decimal so_seri)
    {
        var seri_ct = ToListWithNoLock((from A in _context_pias.NvuBhtCtus
                                        join B in _context_pias.NvuBhtSeris on A.PrKey equals B.FrKey
                                        join C in _context_pias.NvuBhtSeriCts on B.PrKey equals C.FrKey
                                        where A.SoDonbh == so_donbh && B.SoSeri == so_seri && C.MaSp == "050104"
                                        select new
                                        {
                                            TongTien = new[] { "02", "03" }.Contains(A.MaSdbs) ? (-1 * B.TongTien) : A.MaSdbs == "05" ? 0 : B.TongTien,
                                            MtnGtbhVnd = new[] { "02", "03" }.Contains(A.MaSdbs) ? (-1 * (C.MtnGtbhTsan > 0 ? C.MtnGtbhTsan : C.MtnGtbhVnd)) : A.MaSdbs == "05" ? 0 : (C.MtnGtbhTsan > 0 ? C.MtnGtbhTsan : C.MtnGtbhVnd),
                                            GiaTri_Tte = new[] { "02", "03" }.Contains(A.MaSdbs) ? (-1 * (C.GiatriTte > 0 ? C.GiatriTte : 0)) : A.MaSdbs == "05" ? 0 : (C.GiatriTte > 0 ? C.GiatriTte : 0)
                                        }).AsQueryable());
        _logger.Information("GetSoPhiBH so_donbh =" + so_donbh + " so_seri =" + so_seri);
        _logger.Information("GetSoPhiBH seri_ct =" + JsonConvert.SerializeObject(seri_ct));
        var phibh = seri_ct.GroupBy(g => 1 == 1)
                    .Select(s => new SeriPhiBH
                    {
                        TongTien = s.Sum(x => x.TongTien),
                        MtnGtbhVnd = s.Sum(x => x.MtnGtbhVnd),
                        GiaTri_Tte = s.Sum(x => x.GiaTri_Tte)
                    }).FirstOrDefault();
        return phibh;

    }
  
    public static DataSet ConvetXMLToDataset(ArrayOfXElement ds_xml)
    {

        DataSet ds = new DataSet();
        try
        {
            var strSchema = ds_xml.Nodes[0].ToString();
            var strData = ds_xml.Nodes[1].ToString();
            var strXml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n\t<DataSet>";
            strXml += strSchema + strData;
            strXml += "</DataSet>";
            ds.ReadXml(new MemoryStream(Encoding.UTF8.GetBytes(strXml)));
        }
        catch (Exception)
        {
        }
        return ds;
    }
   
}
