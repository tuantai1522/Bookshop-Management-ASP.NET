using BookShop.Data;
using BookShop.Models.Domain;
using BookShop.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Repositories.Implementation
{
    public class BookService : EntityService<Book>, IBookService
    {
        private Db_BookShop _db;
        public BookService(Db_BookShop db) : base(db)
        {
            this._db = db;
        }
        //Every repository has their own update function because sometimes we only update some thing, not all properties

        public bool Update(Book model)
        {
            try
            {
                Book book = this._db.Books.Find(model.Id);
                if (book != null)
                {
                    book.Title = model.Title;
                    book.AuthorName = model.AuthorName;
                    book.TotalPages = model.TotalPages;
                    book.Price = model.Price;
                    book.GenreID = model.GenreID;

                    if (!string.IsNullOrEmpty(model.ImageFileName))
                    {
                        book.ImageFileName = model.ImageFileName;
                    }

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
