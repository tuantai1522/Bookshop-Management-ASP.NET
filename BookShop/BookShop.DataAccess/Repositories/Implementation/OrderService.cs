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
    public class OrderService : EntityService<Order>, IOrderService
    {
        private Db_BookShop _db;
        public OrderService(Db_BookShop db) : base(db)
        {
            this._db = db;
        }

        public bool Update(Order model)
        {
            try
            {
                Order order = this._db.Orders.Find(model.Id);
                if (order != null)
                {
                    order.ApplicationUserId = model.ApplicationUserId;
                    order.OrderDate = model.OrderDate;
                    order.Name = model.Name;
                    order.StreetAddress = model.StreetAddress;
                    order.City = model.City;
                    order.PhoneNumber = model.PhoneNumber;

                    this._db.SaveChanges();

                }

                return true;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();

            }
        }
        //Every repository has their own update function because sometimes we only update some thing, not all properties
    }
}
