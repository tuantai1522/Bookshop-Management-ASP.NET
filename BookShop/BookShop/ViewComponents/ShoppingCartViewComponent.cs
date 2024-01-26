using BookShop.DataAccess.Repositories.Interface;
using BookShop.Repositories.Interface;
using BookShop.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookShop.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IShoppingCartService shoppingCartService;

        public ShoppingCartViewComponent(IShoppingCartService shoppingCartService)
        {
            this.shoppingCartService = shoppingCartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                //use Session
                HttpContext.Session.SetInt32(Utilities.CartSession,
                    shoppingCartService.FindAll(c => c.ApplicationUserId == claim.Value).Count());
                return View(HttpContext.Session.GetInt32(Utilities.CartSession));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }

    }
}
