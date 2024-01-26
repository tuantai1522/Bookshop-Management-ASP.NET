using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Models.Domain
{
    public class ShoppingCart
    {
        public int Id { get; set; }

        public int Count { get; set; } // số lượng sách của BookId trong giỏ hàng

        public int BookId { get; set; }
        [ForeignKey("BookId")]

        public Book Book { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]

        public ApplicationUser ApplicationUser { get; set; }
    }
}
