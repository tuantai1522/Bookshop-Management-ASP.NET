using BookShop.Models.Domain;
using BookShop.Models.ViewModel;
using BookShop.Repositories.Interface;
using BookShop.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using X.PagedList;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Utilities.Role_Admin)]

    public class BookController : Controller
    {
        private readonly IBookService bookService;
        private readonly IGenreService genreService;
        private readonly IWebHostEnvironment webHostEnvironment;

        public BookController(IBookService bookService, IGenreService genreService, IWebHostEnvironment webHostEnvironment)
        {
            this.bookService = bookService;
            this.genreService = genreService;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(int? page)
        {
            var list = bookService.FindAll(null, includeProperties: "Genres");
            return View(list);
        }

        private BookVM RetrieveBookVM()
        {
            return new BookVM
            {
                GenreList = genreService.FindAll(null).Select(genre => new SelectListItem
                {
                    Text = genre.GenreName,
                    Value = genre.Id.ToString()
                }),
                Book = new Book()
            };
        }
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            BookVM bookVM = RetrieveBookVM();

            //insert
            if (id == null || id == 0)
            {
                return View(bookVM);
            }
            else //update => to pass book with id into current view
            {
                bookVM.Book = bookService.FindByID(b => b.Id == id, "Genres");
            }
            return View(bookVM);
        }

        [HttpPost]
        public IActionResult Upsert(BookVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                //working with image url
                string wwwRootPath = webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); // to create random name
                    string bookPath = Path.Combine(wwwRootPath, @"images\Book"); // folder to store all images url


                    //if image can find in images folder

                    if (!string.IsNullOrEmpty(obj.Book.ImageFileName))
                    {
                        //Delete the old image

                        //TrimStart: loại bọ kí tự '\' ở đầu
                        var oldImagePath =
                            Path.Combine(wwwRootPath, obj.Book.ImageFileName.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    //final location of storing image url
                    using (var fileStream = new FileStream(Path.Combine(bookPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    obj.Book.ImageFileName = @"\images\Book\" + fileName; //store in obj to store in database


                }

                //insert
                if (obj.Book.Id == 0)
                {
                    // If I don't add image
                    if (string.IsNullOrEmpty(obj.Book.ImageFileName))
                    {
                        obj.Book.ImageFileName = "";
                    }

                    bool result = bookService.Add(obj.Book);
                    if (result)
                        return RedirectToAction(nameof(Index));
                }
                else // update
                {
                    bool result = bookService.Update(obj.Book);
                    if (result)
                        return RedirectToAction(nameof(Index));
                }

            }
            TempData["msg"] = "Something went wrong";
            BookVM bookVM = RetrieveBookVM();
            return View(bookVM);

        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var model = bookService.FindByID(b => b.Id == id, "Genres");
            return View(model);

        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            var model = bookService.FindByID(b => b.Id == id, "Genres");

            return View(model);
        }

        [HttpPost]
        public IActionResult Delete_Post(int id)
        {
            bool result = bookService.Delete(id);
            if (result)
                return RedirectToAction("Index");
            return View();
        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            //includeProperties: Genres
            List<Book> books = bookService.FindAll(null, includeProperties: "Genres").ToList();

            return Json(new { data = books });
        }
        #endregion


    }
}
