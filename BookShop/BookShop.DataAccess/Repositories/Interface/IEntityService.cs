using BookShop.Models.Domain;
using System.Linq.Expressions;

namespace BookShop.Repositories.Interface
{
    public interface IEntityService<T> where T : class
    {
        bool Add(T model);
        //bool Update(T model);
        bool Delete(int id);
        T FindByID(Expression<Func<T, bool>>? filter, string? includeProperties = null);
        IEnumerable<T> FindAll(Expression<Func<T, bool>>? filter, string? includeProperties = null);
    }
}
