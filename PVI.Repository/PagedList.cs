using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Transactions;

namespace PVI.Helper
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }


        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            AddRange(items);
        }

        public static PagedList<T> CreatePagedList(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }


        public static PagedList<T> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            List<T> items = default;
            //var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            using (var scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions()
                            {
                                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
                            },
                            TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                    scope.Complete();
                }
                catch (Exception)
                {
                    scope.Complete();
                }
            }
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

        public static async Task<PagedList<T>> ToPagedListAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();

            List<T> items;

            using (var scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions()
                            {
                                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
                            },
                            TransactionScopeAsyncFlowOption.Enabled))
            {
                items = await source.Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

                scope.Complete();
            }

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
