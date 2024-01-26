using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Models.ViewModel
{
    public class VnPaymentRequestModel
    {
        public int OrderId { get; set; }
        public string FullName {  get; set; }
        public string Description {  get; set; }
        public double Total {  get; set; }

        public DateTime CreatedDate { get; set; }

    }
}
