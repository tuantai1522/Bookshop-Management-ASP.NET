using BookShop.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Models.ViewModel
{
    public class OrderVM
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        public decimal TotalPrice { get; set; }

        public IEnumerable<OrderDetail>? orderDetails { get; set; }

    }
}
