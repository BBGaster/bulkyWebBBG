using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using Bulky.Models.Models;
using Bulky.DataAcces.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");
        return View(productList);
    }
    public IActionResult Details(int id)
    {
        Product product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id, includeProperties: "Category");
        ShoppingCart cartObj = new ShoppingCart()
        {
            Product = product,
            ProductId = product.Id
        };
        return View(cartObj);
    }
    [HttpPost]
    [Authorize]
    public IActionResult Details(ShoppingCart obj)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        obj.ApplicationUserId = claim;
        obj.ProductId = obj.Product.Id;
        obj.Product = null;

        ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
            u => u.ApplicationUserId == obj.ApplicationUserId && u.ProductId == obj.ProductId
        );
        if (cartFromDb == null)
        {
            _unitOfWork.ShoppingCart.Create(obj);
        }
        else
        {
            cartFromDb.Count += obj.Count;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
        }
       
        _unitOfWork.Save();

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
