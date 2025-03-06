using bulkyWebBBG.Data;
using bulkyWebBBG.Migrations;
using bulkyWebBBG.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bulkyWebBBG.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db ;
        }
        public IActionResult Index()
        {
           List<Category> obgCategoryList = _db.Categories.ToList();
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
            _db.Categories.Add(obj);
            _db.SaveChanges();
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
            Category? categoryFromDb = _db.Categories.Find(Id);
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
                _db.Categories.Update(obj);
                _db.SaveChanges();
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
            Category? categoryFromDb = _db.Categories.Find(Id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePost(int? Id)
        {
            Category? obj = _db.Categories.Find(Id);
            if (obj.Name == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = "Category Deleted";
            return RedirectToAction("Index", "Category");
          
        }

    }
}
