using BookShop.Models.Domain;

namespace BookShop.Repositories.Interface
{
    public interface IBookService : IEntityService<Book>
    {
        bool Update(Book model);

    }
}
