using Bulky.DataAcces.Repository;
using Bulky.DataAcces.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bulkyWebBBG.Areas.Admin.Controllers
{
    [Area("Admin")] 
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork db)
        {
            _unitOfWork = db ;
        }
        public IActionResult Index()
        {
           List<Category> obgCategoryList = _unitOfWork.Category.GetAll().ToList();
            return View(obgCategoryList);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if(obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "the display order cannot match the Name");
            }
            if (obj.Name == "EasterEgg")
            {
                ModelState.AddModelError("", "you found me");
            }
            if (ModelState.IsValid) {
                _unitOfWork.Category.Create(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category created";
            //esplicitare il controller può essere evitato nello stesso controller
            return RedirectToAction("Index", "Category");
            }
            return View();
           
        }

        public IActionResult Edit(int? Id)
        {
            if(Id==null || Id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.GetFirstOrDefault(u=> u.Id == Id);
            // Category? categoryFromDb = _db.Categories.FirstOrDefault(u=>u.Id == Id);
            // Category? categoryFromDb = _db.Categories.Where(u=>u.Id == Id).FirstOrDefault();
            if (categoryFromDb == null)
            {
                return NotFound();
            }


            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category Edited";
                //esplicitare il controller può essere evitato nello stesso controller
                return RedirectToAction("Index", "Category");
            }
            return View();

        }

        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == Id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePost(int? Id)
        {
            Category? obj = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == Id);
            if (obj.Name == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category Deleted";
            return RedirectToAction("Index", "Category");
          
        }

    }
}
