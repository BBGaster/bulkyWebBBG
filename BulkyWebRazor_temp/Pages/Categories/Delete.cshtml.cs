using BulkyWebRazor_temp.Data;
using BulkyWebRazor_temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_temp.Pages.Categories
{
    [BindProperties]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        
        public Category? Cate { get; set; }
        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet(int Id)
        {
            Cate = _db.Categories.Find(Id);
            if (Cate == null)
            {
                RedirectToPage("Index");
            }

        }
        public IActionResult OnPost()
        {
            Category? obj = _db.Categories.Find(Cate.Id);
            if (obj != null)
            {
                TempData["success"] = "Category Deleted";
                _db.Categories.Remove(obj);
                _db.SaveChanges();
                //esplicitare il controller può essere evitato nello stesso controller
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
