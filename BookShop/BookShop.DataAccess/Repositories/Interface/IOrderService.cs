using BookShop.Models.Domain;
using BookShop.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.DataAccess.Repositories.Interface
{
    public interface IOrderService : IEntityService<Order>
    {
        bool Update(Order model);

    }
}
