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
    public class ShoppingCartService : EntityService<ShoppingCart>, IShoppingCartService
    {
        private Db_BookShop _db;
        public ShoppingCartService(Db_BookShop db) : base(db)
        {
            this._db = db;
        }

        public bool Update(ShoppingCart model)
        {
            try
            {
                ShoppingCart shoppingCart = this._db.ShoppingCarts.Find(model.Id);
                if (shoppingCart != null)
                {
                    shoppingCart.Count = model.Count;
                    shoppingCart.BookId = model.BookId;
                    shoppingCart.ApplicationUserId = model.ApplicationUserId;

                    this._db.SaveChanges();

                }

                return true;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();

            }
        }
    }
}
