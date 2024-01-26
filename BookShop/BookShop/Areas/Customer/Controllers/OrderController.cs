using BookShop.DataAccess.Repositories.Interface;
using BookShop.Models.Domain;
using BookShop.Models.ViewModel;
using BookShop.Repositories.Implementation;
using BookShop.Repositories.Interface;
using BookShop.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace BookShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IOrderDetailService orderDetailService;
        private readonly IShoppingCartService shoppingCartService;
        private readonly IApplicationUserService applicationUserService;
        private readonly IVnPayService vnPayService;
        public OrderController(IShoppingCartService shoppingCartService,
                               IOrderService orderService,
                               IOrderDetailService orderDetailService,
                               IApplicationUserService applicationUserService,
                               IVnPayService vnPayService)
        {
            this.shoppingCartService = shoppingCartService;
            this.orderService = orderService;
            this.applicationUserService = applicationUserService;
            this.vnPayService = vnPayService;
            this.orderDetailService = orderDetailService;

        }

        //Cart để thể hiện giỏ hàng
        [BindProperty]
        public ShoppingCartVM listCarts { get; set; }

        [Authorize(Roles = Utilities.Role_Admin)]
        public IActionResult Index()
        {
            IEnumerable<Order> list = this.orderService
                                     .FindAll(null, includeProperties: "ApplicationUser").OrderBy(item => item.Id);
            return View(list);
        }

        [Authorize]
        public IActionResult Details(string ApplicationUserId)
        {
            //get userid
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            IEnumerable<Order> orders = this.orderService.FindAll(x => x.ApplicationUserId == userId);

            List<OrderVM> orderList = new List<OrderVM>();

            foreach (Order order in orders)
            {
                IEnumerable<OrderDetail> orderDetails = this.orderDetailService.FindAll(x => x.OrderId == order.Id,
                                                                            includeProperties: "Book");

                OrderVM orderVM = new()
                {
                    OrderId = order.Id,
                    OrderDate = order.OrderDate,
                    TotalPrice = order.Total,
                    orderDetails = orderDetails,
                };
                orderList.Add(orderVM);   
            }    
            return View(orderList);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Summary()
        {
            //get userid
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            listCarts = new()
            {
                ShoppingCarts = this.shoppingCartService.FindAll(c => c.ApplicationUserId == userId,
                includeProperties: "Book"),
                Order = new Order()
            };

            listCarts.Order.ApplicationUser = this.applicationUserService.FindByID(u => u.Id == userId);

            listCarts.Order.Name = listCarts.Order.ApplicationUser.Name;
            listCarts.Order.PhoneNumber = listCarts.Order.ApplicationUser.PhoneNumber;
            listCarts.Order.StreetAddress = listCarts.Order.ApplicationUser.StreetAddress;
            listCarts.Order.City = listCarts.Order.ApplicationUser.City;

            foreach (ShoppingCart cart in listCarts.ShoppingCarts)
            {
                listCarts.Order.Total += (cart.Book.Price * cart.Count);
            }
            return View(listCarts);
        }


        [Authorize]
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {

            //get userid
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            var user = this.applicationUserService.FindByID(u => u.Id == userId);

            listCarts = new()
            {
                ShoppingCarts = this.shoppingCartService.FindAll(c => c.ApplicationUserId == userId,
                        includeProperties: "Book"),
                Order = new Order()
            };

            double total = 0;
            foreach (ShoppingCart cart in listCarts.ShoppingCarts)
            {
                total += (double)(cart.Book.Price * cart.Count);
            }
            if (user != null)
            {
                var vnPayModel = new VnPaymentRequestModel
                {
                    OrderId = new Random().Next(1000, 10000),
                    FullName = $"{user.Name} {user.PhoneNumber}",
                    Description = "Thanh toan don hang",
                    Total = total,
                    CreatedDate = DateTime.Now,
                };
                return Redirect(vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
            }
            return RedirectToAction("Summary");
        }

        public IActionResult PaymentFail()
        {
            return View();
        }

        public IActionResult PaymentSuccess()
        {
            return View();
        }
        [Authorize]
        public IActionResult PaymentBack()
        {
            var response = vnPayService.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["ErrorMessage"] = $"Lỗi thanh toán VN Pay";
                return RedirectToAction("PaymentFail");
            }


            //-------------------------------Lưu vào database-------------------------------

            //get userid
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            var user = this.applicationUserService.FindByID(u => u.Id == userId);

            listCarts = new()
            {
                ShoppingCarts = this.shoppingCartService.FindAll(c => c.ApplicationUserId == userId,
                    includeProperties: "Book"),
                Order = new Order()
            };

            Order order = new Order
            {
                ApplicationUserId = userId,
                OrderDate = DateTime.Now,
                Total = listCarts.ShoppingCarts.Sum(p => p.Book.Price),
                Name = user.Name,
                StreetAddress = user.StreetAddress,
                City = user.City,
                PhoneNumber = user.PhoneNumber,
            };

            bool result = orderService.Add(order);
            if (result)
            {
                //Add order Details
                foreach (ShoppingCart cart in listCarts.ShoppingCarts)
                {
                    OrderDetail orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        BookId = cart.BookId,
                        Count = cart.Count,
                        Price = (double)cart.Book.Price * cart.Count,
                    };
                    this.orderDetailService.Add(orderDetail);
                }

                //Xóa toàn bộ dữ liệu trong cart
                this.listCarts.ShoppingCarts = new List<ShoppingCart>();

                //set Session
                if (HttpContext.Session != null)
                {
                    HttpContext.Session.SetInt32(Utilities.CartSession, 0);
                }


                TempData["SuccessMessage"] = $"Thanh toán VN Pay thành công";
                return RedirectToAction("PaymentSuccess");
            }
            else
            {
                TempData["ErrorMessage"] = $"Lỗi server";
                return RedirectToAction("PaymentFail");
            }

        }

    }
}
