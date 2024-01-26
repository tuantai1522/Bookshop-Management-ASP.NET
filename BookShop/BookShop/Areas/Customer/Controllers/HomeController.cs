using BookShop.DataAccess.Repositories.Interface;
using BookShop.Models;
using BookShop.Models.Domain;
using BookShop.Models.ViewModel;
using BookShop.Repositories.Interface;
using BookShop.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using X.PagedList;

namespace BookShop.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class HomeController : Controller
    {
        private readonly IBookService bookService;
        private readonly IGenreService genreService;
        private readonly IShoppingCartService shoppingCartService;


        public HomeController(IBookService bookService,
            IGenreService genreService,
            IShoppingCartService shoppingCartService)
        {
            this.bookService = bookService;
            this.genreService = genreService;
            this.shoppingCartService = shoppingCartService;
        }

        public IActionResult Index(int? page)
        {
            HttpContext.Session.Get("123");
            CustomerVM customerVM = new()
            {
                BookList = bookService.FindAll(null, includeProperties: "Genres")
                                    .ToList()
                                    .ToPagedList(page ?? Utilities.DefaultPage, Utilities.ItemsOnPage)
                ,
                GenreList = genreService.FindAll(null),
            };
            return View(customerVM);
        }

        public IActionResult GetBooksByGenreID(int id, int? page)
        {
            try
            {
                var list = bookService.FindAll(s => s.GenreID == id)
                    .OrderBy(s => s.Id)
                    .ToList()
                    .ToPagedList(page ?? Utilities.DefaultPage, Utilities.ItemsOnPage);


                CustomerVM customerVM = new()
                {
                    BookList = list,
                    GenreList = genreService.FindAll(null),
                };
                return View(customerVM);



            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #region API CALLS
        [HttpPost]
        [Authorize] // must sign in
        public ActionResult AddToCart(int bookId)
        {
            try
            {
                //get userid
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;



                //cartInDatabase: dùng để kiểm tra cuốn sách này của khách hàng này có trong giỏ hàng chưa
                ShoppingCart cartInDatabase = this.shoppingCartService.FindByID(c =>
                                        c.ApplicationUserId == userId &&
                                        c.BookId == bookId);

                //if this product of user has not existed before => ADD
                if (cartInDatabase == null)
                {
                    //Tạo mới cart: cart này để update hay thêm mới vào trong giỏ hàng
                    ShoppingCart cart = new()
                    {
                        Count = 1,
                        ApplicationUserId = userId,
                        BookId = bookId
                    };

                    this.shoppingCartService.Add(cart);

                    //use Session
                    HttpContext.Session.SetInt32(Utilities.CartSession,
                        this.shoppingCartService.FindAll(c =>
                                        c.ApplicationUserId == userId).Count());

                    return Json(new
                    {
                        success = true,
                        message = "Book added to cart successfully",
                        totalProducts = HttpContext.Session.GetInt32(Utilities.CartSession),
                    });
                }
                else
                {
                    //if this product of user has existed before => UPDATE
                    cartInDatabase.Count += 1;
                    this.shoppingCartService.Update(cartInDatabase);

                    //Tính tổng tiền của giỏ hàng hiện tại
                    ShoppingCartVM listCarts = new()
                    {
                        ShoppingCarts = this.shoppingCartService.FindAll(c => c.ApplicationUserId == userId,
                        includeProperties: "Book"),
                    };

                    decimal total = 0;
                    foreach (ShoppingCart cart in listCarts.ShoppingCarts)
                        total += (cart.Book.Price * cart.Count);



                    return Json(new
                    {
                        success = true,
                        message = "Update cart successfully",
                        totalProducts = HttpContext.Session.GetInt32(Utilities.CartSession),
                        itemsUpdated = cartInDatabase.Count,
                        totalPrice = total,
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error adding book to cart",
                    error = ex.Message
                });
            }

        }
        #endregion

    }
}