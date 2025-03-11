using Bulky.DataAcces.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace bulkyWebBBG.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller 
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvirement;
        public CompanyController(IUnitOfWork db, IWebHostEnvironment webHostEnvirement  )
        {
            _unitOfWork = db;
            _webHostEnvirement = webHostEnvirement;
        }

        public IActionResult Index()
        {
            List<Company> obgCompanyList = _unitOfWork.Company.GetAll().ToList();

            return View(obgCompanyList);
        }

        public IActionResult Upsert(int? id)
        {
            Company? obg = new();
            if (id == null || id == 0) { return View(obg); }
            else
            {
                obg =  _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
                return View(obg);
            }



        }

        [HttpPost]
        public IActionResult upsert(Company obj)
        {

            if (ModelState.IsValid)
            {
               
               
                if (obj.Id == 0)
                {
                    _unitOfWork.Company.Create(obj);
                    TempData["success"] = "Product created";
                }
                else
                {
                    _unitOfWork.Company.Update(obj);
                    TempData["success"] = "Product updated";
                }

                _unitOfWork.Save();

                //esplicitare il controller può essere evitato nello stesso controller
                return RedirectToAction("Index", "Company");
            }
            else
            {
                return View(obj);
            }

        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> obgCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = obgCompanyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? Id)
        {
            var companyToBeDeleted = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == Id);
            if (companyToBeDeleted == null) return Json(new { success = false, message = "error while deleting" });


            _unitOfWork.Company.Remove(companyToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message = "delete succesfull" });
        }
        #endregion


    }
}
