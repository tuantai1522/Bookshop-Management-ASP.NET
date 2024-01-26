using BookShop.Models.Domain;
using BookShop.Repositories.Implementation;
using BookShop.Repositories.Interface;
using BookShop.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Utilities.Role_Admin)]

    public class GenreController : Controller
    {
        private readonly IGenreService service;

        public GenreController(IGenreService genreService)
        {
            service = genreService;
        }


        public IActionResult Index()
        {
            var list = service.FindAll(null);
            return View(list);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Add(Genre model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool result = service.Add(model);
            if (result)
            {
                TempData["msg"] = "Added successfully";
                return RedirectToAction(nameof(Add));
            }
            TempData["msg"] = "Something went wrong";

            return View(model);

        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var model = service.FindByID(g => g.Id == id);
            return View(model);
        }

        [HttpPost]
        public IActionResult Update(Genre model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool result = service.Update(model);
            if (result)
                return RedirectToAction("Index");

            return View(model);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var model = service.FindByID(g => g.Id == id);
            return View(model);

        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            var model = service.FindByID(g => g.Id == id);
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete_Post(int id)
        {
            bool result = service.Delete(id);
            if (result)
                return RedirectToAction("Index");
            return View();
        }
    }
}
