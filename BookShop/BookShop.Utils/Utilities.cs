using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Web;
using System.Text;
using System.Security.Claims;

namespace BookShop.Utils
{
    public static class Utilities
    {
        public const int ItemsOnPage = 2;
        public const int DefaultPage = 1;

        public const string Role_Customer = "Customer";
        public const string Role_Admin = "Admin";

        public const string CartSession = "ShoppingCartSession";


    }
}