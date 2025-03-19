using Bulky.DataAcces.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace bulkyWebBBG.Areas.Admin.Controllers
{
   
        [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
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
                productVM.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id,includeProperties:"ProductImages");
                     return View(productVM);
            }



            }

        [HttpPost]

        public IActionResult Upsert(ProductVM productVM, List<IFormFile>? files) //IFormFile serve per prendere il File dell'immagine caricata nel form

        {

            if (ModelState.IsValid) //Non dobbiamo validare Category, usa un Annotation in ProductVM e ProductModel per risolvere

            {

                if (files != null)

                {

                    if (productVM.Product.Id == 0)

                    {

                        _unitOfWork.Product.Create(productVM.Product);
                        _unitOfWork.Save();
                    }

                    else

                    {

                        _unitOfWork.Product.Update(productVM.Product);
                        _unitOfWork.Save();
                    }

                    string wwwRootPath = _webHostEnvirement.WebRootPath;

                    if (files != null)

                    {

                        foreach (var file in files)

                        {
                            

                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); //Random name

                            string productPath = @"images\products\product-" + productVM.Product.Id;

                            string finalPath = Path.Combine(wwwRootPath, productPath);

                            if (!Directory.Exists(finalPath))
                                Directory.CreateDirectory(finalPath);

                            //Aggiungi l'immagine

                            using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))

                            { file.CopyTo(fileStream); }

                            ProductImage productImage = new ProductImage()
                            {

                                ImageUrl = @"\" + productPath + @"\" + fileName,

                                ProductId = productVM.Product.Id

                            };

                            if (productVM.Product.ProductImages == null)

                                productVM.Product.ProductImages = new List<ProductImage>();

                            productVM.Product.ProductImages.Add(productImage);

                        }

                        _unitOfWork.Product.Update(productVM.Product);

                        _unitOfWork.Save();

                    }

                }
                TempData["success"] = "Product created/updated successfully";
                return RedirectToAction("Index");
            }
            else
            {
                //Se non funziona ridiamo la stessa pagina poplata (category)
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
        }

        public IActionResult DeleteImage(int imageId)
        {
            var imageToDelete = _unitOfWork.ProductImage.GetFirstOrDefault(u => u.Id == imageId);
            int productId = imageToDelete.ProductId;
            if (imageToDelete != null && !string.IsNullOrEmpty(imageToDelete.ImageUrl))
            {

                var oldImagePath = Path.Combine(_webHostEnvirement.WebRootPath, imageToDelete.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath)) { System.IO.File.Delete(oldImagePath); }
            }
            _unitOfWork.ProductImage.Remove(imageToDelete);
            _unitOfWork.Save();
            TempData["success"] = "Deleted Succesfully";
            return RedirectToAction(nameof(Upsert), new { id = productId });
        }


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


            string productPath = @"images\products\product-" + Id;
            string finalPath = Path.Combine(_webHostEnvirement.WebRootPath, productPath);

            if (Directory.Exists(finalPath)) {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach(var file in filePaths)
                {
                    System.IO.File.Delete(file);
                }
                Directory.Delete(finalPath);
                }


            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message = "delete succesfull" });
        }
        #endregion

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

//[HttpPost]
//public IActionResult Edit(Product obj)
//{

//    if (ModelState.IsValid)
//    {
//        _unitOfWork.Product.Update(obj);
//        _unitOfWork.Save();
//        TempData["success"] = "Product Edited";
//        //esplicitare il controller può essere evitato nello stesso controller
//        return RedirectToAction("Index", "Product");
//    }
//    return View();

//}

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