using BookShop.Data;
using BookShop.Models.Domain;
using BookShop.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookShop.Repositories.Implementation
{
    public class EntityService<T> : IEntityService<T> where T : class
    {
        private readonly Db_BookShop _db;
        internal DbSet<T> dbSet;
        public EntityService(Db_BookShop db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        public bool Add(T model)
        {
            try
            {
                this.dbSet.Add(model);
                this._db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();

            }
        }

        public bool Delete(int id)
        {
            try
            {
                var model = this.dbSet.Find(id);
                if (model == null)
                {
                    return false; // Không tìm thấy đối tượng cần xóa
                }

                this.dbSet.Remove(model);

                this._db.SaveChanges();
                return true; // Xóa thành công
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<T> FindAll(Expression<Func<T, bool>>? filter, string? includeProperties = null)
        {
            try
            {
                IQueryable<T> query = this.dbSet;

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                if (!string.IsNullOrEmpty(includeProperties))
                {
                    //there are multiple includes
                    foreach (var includeProp in includeProperties
                        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProp);
                    }
                }
                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();

            }
        }

        public T FindByID(Expression<Func<T, bool>>? filter, string? includeProperties = null)
        {
            try
            {
                IQueryable<T> query = this.dbSet.Where(filter);

                if (!string.IsNullOrEmpty(includeProperties))
                {
                    //there are multiple includes
                    foreach (var includeProp in includeProperties
                        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProp);
                    }
                }

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();

            }
        }
    }
}
