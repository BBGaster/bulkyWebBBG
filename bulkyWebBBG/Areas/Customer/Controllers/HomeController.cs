using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using Bulky.Models.Models;
using Bulky.DataAcces.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Bulky.Utility;

namespace bulkyWebBBG.Areas.Customer.Controllers;
[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        if (claim != null)
        {
            HttpContext.Session.SetString(SD.SessionShoppingCart, claim.Value);
        }
        IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductImages");
        return View(productList);
    }
    public IActionResult Details(int productId)
    {
        Product product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == productId, includeProperties: "Category,ProductImages");
        ShoppingCart cartObj = new ShoppingCart()
        {
            Product = product,
            Count = 1,
            ProductId = productId
        };
        return View(cartObj);
    }
    [HttpPost]
    [Authorize]
    public IActionResult Details(ShoppingCart obj)
    {
        
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        obj.ApplicationUserId = claim.Value;
        
        ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
            u => u.ApplicationUserId == obj.ApplicationUserId && u.ProductId == obj.ProductId
        );
        if (cartFromDb == null)
        {
            _unitOfWork.ShoppingCart.Create(obj);
            _unitOfWork.Save();
            HttpContext.Session.SetInt32(SD.SessionShoppingCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == obj.ApplicationUserId).Count());
        }
        else
        {
            cartFromDb.Count += obj.Count;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
        }
       
       

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
