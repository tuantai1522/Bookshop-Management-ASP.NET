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
    public class OrderDetailService : EntityService<OrderDetail>, IOrderDetailService
    {
        private Db_BookShop _db;
        public OrderDetailService(Db_BookShop db) : base(db)
        {
            this._db = db;
        }
        public bool Update(OrderDetail model)
        {
            try
            {
                OrderDetail orderDetail = this._db.OrderDetails.Find(model.Id);
                if (orderDetail != null)
                {
                    orderDetail.OrderId = model.OrderId;
                    orderDetail.BookId = model.BookId;
                    orderDetail.Count = model.Count;
                    orderDetail.Price = model.Price;

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
