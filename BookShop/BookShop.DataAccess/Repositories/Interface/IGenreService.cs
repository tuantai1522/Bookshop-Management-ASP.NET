using BookShop.Models.Domain;

namespace BookShop.Repositories.Interface
{
    public interface IGenreService : IEntityService<Genre>
    {
        bool Update(Genre model);
    }
}
