using BookShop.Data;
using BookShop.DataAccess.Repositories.Interface;
using BookShop.Models.Domain;
using BookShop.Repositories.Implementation;
using BookShop.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.DataAccess.Repositories.Implementation
{
    public class ApplicationUserService : EntityService<ApplicationUser>, IApplicationUserService
    {
        private Db_BookShop _db;
        public ApplicationUserService(Db_BookShop db) : base(db)
        {
            this._db = db;
        }
    }
}
