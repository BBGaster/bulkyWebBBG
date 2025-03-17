using Bulky.DataAcces.Repository.IRepository;
using Bulky.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace bulkyWebBBG.VIewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {

        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var count = 0;
           
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var shoppingCart = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value, includeProperties: "Product");
                count = shoppingCart.Count();
                if (count == null) {
                    HttpContext.Session.SetInt32(SD.SessionShoppingCart, count);
                    return View(HttpContext.Session.GetInt32(SD.SessionShoppingCart));
                }
                
                return View(count);
            }
            else 
            {
                HttpContext.Session.Clear();
                return View(0);
            }

                
            
        }

    }
}
