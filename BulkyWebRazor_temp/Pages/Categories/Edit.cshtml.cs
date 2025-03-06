using BulkyWebRazor_temp.Data;
using BulkyWebRazor_temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_temp.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public Category? Cate { get; set; }
        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet(int Id)
        {
            Cate = _db.Categories.Find(Id);
            if(Cate == null) 
            {
                RedirectToPage("Index");
            }
            
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid && Cate != null)
            {
                
                _db.Categories.Update(Cate);
                _db.SaveChanges();
                TempData["success"] = "Category Edited";
                //esplicitare il controller può essere evitato nello stesso controller
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
