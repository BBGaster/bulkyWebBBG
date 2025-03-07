using Bulky.DataAcces.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace bulkyWebBBG.Areas.Admin.Controllers
{
   
        [Area("Admin")]
        public class ProductController : Controller
        {
            private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvirement;
            public ProductController(IUnitOfWork db, IWebHostEnvironment webHostEnvirement)
            {
                _unitOfWork = db;
                _webHostEnvirement = webHostEnvirement;
            }
        public IActionResult Index()
        {
            List<Product> obgProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            
            return View(obgProductList);
        }
            public IActionResult Upsert(int? id)
            {
            
           
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),

                Product = new Product()

            };

            if ( id == null || id == 0 ) { return View(productVM); }
            else
            {
                productVM.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
                     return View(productVM);
            }



            }

            [HttpPost]
            public IActionResult upsert(ProductVM obj, IFormFile? file)
            {
                
                if (ModelState.IsValid)
                {
                string wwwRootPath = _webHostEnvirement.WebRootPath;
                if (file!=null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(obj.Product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath)) { System.IO.File.Delete(oldImagePath); }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    obj.Product.ImageUrl = @"\images\product" + fileName;
                }
                if (obj.Product.Id == 0) { 
                    _unitOfWork.Product.Create(obj.Product);
                    TempData["success"] = "Product created";
                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                    TempData["success"] = "Product updated";
                }

                _unitOfWork.Save();
                
                //esplicitare il controller può essere evitato nello stesso controller
                return RedirectToAction("Index", "Product");
                }
                else
                {

                    obj.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    });

             
                return View(obj);
                }

            }

            //public IActionResult Edit(int? Id)
            //{
            //    if (Id == null || Id == 0)
            //    {
            //        return NotFound();
            //    }
            //    Product? productFromDb = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == Id);
            //    // Product? productFromDb = _db.Categories.FirstOrDefault(u=>u.Id == Id);
            //    // Product? productyFromDb = _db.Categories.Where(u=>u.Id == Id).FirstOrDefault();
            //    if (productFromDb == null)
            //    {
            //        return NotFound();
            //    }


            //    return View(productFromDb);
            //}

            [HttpPost]
            public IActionResult Edit(Product obj)
            {

                if (ModelState.IsValid)
                {
                    _unitOfWork.Product.Update(obj);
                    _unitOfWork.Save();
                    TempData["success"] = "Product Edited";
                    //esplicitare il controller può essere evitato nello stesso controller
                    return RedirectToAction("Index", "Product");
                }
                return View();

            }

            //public IActionResult Delete(int? Id)
            //{
            //    if (Id == null || Id == 0)
            //    {
            //        return NotFound();
            //    }
            //    Product? productFromDb = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == Id);
            //    if (productFromDb == null)
            //    {
            //        return NotFound();
            //    }

            //    return View(productFromDb);
            //}

            //[HttpPost, ActionName("Delete")]
            //public IActionResult DeletePost(int? Id)
            //{
            //    Product? obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == Id);
            //    if (obj.Title == null)
            //    {
            //        return NotFound();
            //    }
            //    _unitOfWork.Product.Remove(obj);
            //    _unitOfWork.Save();
            //    TempData["success"] = "Product Deleted";
            //    return RedirectToAction("Index", "Product");

            //}

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> obgProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = obgProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? Id)
        {
           var productToBeDeleted = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == Id);
            if(productToBeDeleted== null) return Json(new {success = false, message = "error while deleting" });

            var oldImagePath = Path.Combine(_webHostEnvirement.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath)) { System.IO.File.Delete(oldImagePath); }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message = "delete succesfull" });
        }
        #endregion

    }
}

