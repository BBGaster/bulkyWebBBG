using Bulky.DataAcces.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace bulkyWebBBG.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public ShoppingCartController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        [Authorize]
        public IActionResult Index()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                ShoppingListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim, includeProperties: "Product"),
                orderHeader = new()
            };

            foreach (var cart in ShoppingCartVM.ShoppingListCart) 
            {
                cart.Price = GetPriceBasedOnquantity(cart);
                ShoppingCartVM.orderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                ShoppingListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim, includeProperties: "Product"),
               orderHeader= new()
            };

            ShoppingCartVM.orderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim);

            ShoppingCartVM.orderHeader.Name = ShoppingCartVM.orderHeader.ApplicationUser.Name;
            ShoppingCartVM.orderHeader.PhoneNumber = ShoppingCartVM.orderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.orderHeader.StreetAddress = ShoppingCartVM.orderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.orderHeader.City = ShoppingCartVM.orderHeader.ApplicationUser.City;
            ShoppingCartVM.orderHeader.State = ShoppingCartVM.orderHeader.ApplicationUser.State;
            ShoppingCartVM.orderHeader.PostalCode = ShoppingCartVM.orderHeader.ApplicationUser.PostalCode;

            foreach (var cart in ShoppingCartVM.ShoppingListCart)
            {
                cart.Price = GetPriceBasedOnquantity(cart);
                ShoppingCartVM.orderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM.ShoppingListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim, includeProperties: "Product");
              
            ShoppingCartVM.orderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.orderHeader.ApplicationUserId = claim;

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim);

            foreach (var cart in ShoppingCartVM.ShoppingListCart)
            {
                cart.Price = GetPriceBasedOnquantity(cart);
                ShoppingCartVM.orderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                ShoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.orderHeader.OrderStatus = SD.statusPending;
            }
            else
            {
                ShoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.orderHeader.OrderStatus = SD.statusApproved;
            }

            _unitOfWork.OrderHeader.Create(ShoppingCartVM.orderHeader);
            _unitOfWork.Save();
            foreach(var cart in ShoppingCartVM.ShoppingListCart)
            {
                OrderDetail orderDetails = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.orderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetail.Create(orderDetails);
                _unitOfWork.Save();
            }
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                var domain = "https://localhost:7211/";
                var options = new SessionCreateOptions
                {
                    
                    SuccessUrl = domain+ $"customer/shoppingcart/OrderConfirmation?id={ShoppingCartVM.orderHeader.Id}",
                    CancelUrl = domain + "customer/ShoppingCart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                        
                    Mode = "payment",
                };  
                foreach(var items in ShoppingCartVM.ShoppingListCart)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(items.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = items.Product.Title,
                            }
                        },
                        Quantity = items.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }
                var service = new SessionService();
                Session session = service.Create(options);
                _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.orderHeader.Id, session.Id,session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }

            

            return RedirectToAction(nameof( OrderConfirmation ), new { Id = ShoppingCartVM.orderHeader.Id });
        }

        public IActionResult OrderConfirmation(int Id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == Id, includeProperties: "ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if(session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(Id, SD.statusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
            return View(Id);
        }
        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
       
        }
        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            cartFromDb.Count -= 1;
            if(cartFromDb.Count == 0)
            {
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
            }
            else
            {
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));

        }

        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));

        }

        private double GetPriceBasedOnquantity(ShoppingCart shoppingCart) {

            if (shoppingCart.Count <= 50) 
            {
                return shoppingCart.Product.ListPrice;
            }
            if ( shoppingCart.Count <= 100)
            {
                return shoppingCart.Product.ListPrice50;
            }
            else
            {
                return shoppingCart.Product.ListPrice100;
            }

        }
    }
}
