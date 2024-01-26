using BookShop.Data;
using BookShop.Models.Domain;
using BookShop.Repositories.Interface;

namespace BookShop.Repositories.Implementation
{
    public class GenreService : EntityService<Genre>, IGenreService
    {
        private Db_BookShop _db;
        public GenreService(Db_BookShop db) : base(db)
        {
            this._db = db;
        }
        //Every repository has their own update function because sometimes we only update some thing, not all properties

        public bool Update(Genre model)
        {
            try
            {
                Genre genre = this._db.Genres.Find(model.Id);
                if (genre != null)
                {
                    genre.GenreName = model.GenreName;

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
