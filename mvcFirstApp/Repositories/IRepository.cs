using mvcFirstApp.ViewModels;
using System.Linq.Expressions;

namespace mvcFirstApp.Repositories
{
    public interface IRepository<T>
    {
        public PaginatedList<T> GetAll(int pageNumber, int pageSize,
                Expression<Func<T, bool>> filter = null,
                Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                string includeProperties = "");

        public IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");
        public T GetById(int id, string includeProperties = "");
        public void Add(T entity);
        public void Update(T entity);
        public void Delete(int id);
        public void Delete(T entity);
        public void SaveChanges();
    }
}
