using BookShop.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Models.ViewModel
{
    public class CustomerVM
    {
        public IEnumerable<Book> BookList;
        public IEnumerable<Genre> GenreList;
    }
}
