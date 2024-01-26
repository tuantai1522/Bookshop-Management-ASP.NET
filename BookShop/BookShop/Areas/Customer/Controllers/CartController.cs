using BookShop.DataAccess.Repositories.Interface;
using BookShop.Models.Domain;
using BookShop.Models.ViewModel;
using BookShop.Repositories.Interface;
using BookShop.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace BookShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IShoppingCartService shoppingCartService;

        public CartController(IShoppingCartService shoppingCartService)
        {
            this.shoppingCartService = shoppingCartService;
        }

        public ShoppingCartVM listCarts { get; set; }
        public IActionResult Index()
        {
            //get userid
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;



            ShoppingCartVM listCarts = new()
            {
                ShoppingCarts = this.shoppingCartService.FindAll(c => c.ApplicationUserId == userId,
                includeProperties: "Book"),
                Order = new Order(),
            };

            decimal total = 0;
            foreach (ShoppingCart cart in listCarts.ShoppingCarts)
                total += (cart.Book.Price * cart.Count);

            listCarts.Order.Total = total;

            return View(listCarts);
        }

        public IActionResult PaymentBack()
        {
            return View();
        }
        #region API CALLS
        [HttpPost]
        [Authorize] // must sign in
        public IActionResult RemoveFromCart(int bookId, int quantity)
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

                //Xóa 1
                if(quantity == 0)
                {
                    if (cartInDatabase != null)
                    {
                        cartInDatabase.Count -= 1;

                        this.shoppingCartService.Update(cartInDatabase);

                        if (cartInDatabase.Count == 0)
                        {
                            bool res = this.shoppingCartService.Delete(cartInDatabase.Id);

                            if (res == false)
                            {
                                return Json(new
                                {
                                    success = false,
                                    message = "Server error when removing cart",
                                });

                            }
                        }



                        decimal total = 0;

                        //Tính tổng tiền của giỏ hàng hiện tại
                        ShoppingCartVM listCarts = new()
                        {
                            ShoppingCarts = this.shoppingCartService.FindAll(c => c.ApplicationUserId == userId,
                            includeProperties: "Book"),
                        };

                        foreach (ShoppingCart cart in listCarts.ShoppingCarts)
                            total += (cart.Book.Price * cart.Count);

                        return Json(new
                        {
                            success = true,
                            message = "Removing from cart successfully",
                            totalProducts = this.shoppingCartService.FindAll(c => c.ApplicationUserId == userId).Count(),
                            itemsUpdated = cartInDatabase.Count,
                            totalPrice = total,
                        });


                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Not finding book to remove",
                        });
                    }
                }
                //Xóa hết
                else
                {
                    if (cartInDatabase != null)
                    {
                        bool res = this.shoppingCartService.Delete(cartInDatabase.Id);

                        if (res == false)
                        {
                            return Json(new
                            {
                                success = false,
                                message = "Server error when removing cart",
                            });

                        }

                        decimal total = 0;

                        //Tính tổng tiền của giỏ hàng hiện tại
                        ShoppingCartVM listCarts = new()
                        {
                            ShoppingCarts = this.shoppingCartService.FindAll(c => c.ApplicationUserId == userId,
                            includeProperties: "Book"),
                        };

                        foreach (ShoppingCart cart in listCarts.ShoppingCarts)
                            total += (cart.Book.Price * cart.Count);

                        return Json(new
                        {
                            success = true,
                            message = "Removing from cart successfully",
                            totalProducts = this.shoppingCartService.FindAll(c => c.ApplicationUserId == userId).Count(),
                            itemsUpdated = 0,
                            totalPrice = total,
                        });


                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Not finding book to remove",
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error deleting book to cart",
                    error = ex.Message
                });
            }
        }
        #endregion

    }
}
