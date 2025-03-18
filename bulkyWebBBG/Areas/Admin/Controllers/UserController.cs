using Bulky.DataAcces.Data;
using Bulky.DataAcces.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace bulkyWebBBG.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
    
       
        [BindProperty]
        public UserVM UserVM { get; set; }
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        public UserController( IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager  )
        {
           
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            

            return View();
        }

        public IActionResult Permission(string? id)
        {
            UserVM = new UserVM()
            {
                ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == id,includeProperties:"Company"),
                RoleList = _roleManager.Roles.Select(u => new SelectListItem
                /*_db.Roles.Select(u => new SelectListItem*/
                {
                    Text = u.Name,
                    Value = u.Name
                }),
                CompanyList = _unitOfWork.Company.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })

            };
            UserVM.ApplicationUser.Role = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == id))
                .GetAwaiter().GetResult().FirstOrDefault();
            return View(UserVM);
        }
        [HttpPost]
        [ActionName("Permission")]
        public IActionResult PermissionPOST()
        {
            string oldRole = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == UserVM.ApplicationUser.Id))
                .GetAwaiter().GetResult().FirstOrDefault();
            ApplicationUser tuser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == UserVM.ApplicationUser.Id);

            if (UserVM.ApplicationUser.Role == SD.Role_User_Comp && tuser.CompanyId != UserVM.ApplicationUser.CompanyId)
            {
                tuser.CompanyId = UserVM.ApplicationUser.CompanyId;
            }

            if (!(UserVM.ApplicationUser.Role == oldRole))
            {
                if (oldRole == SD.Role_User_Comp)
                {
                    tuser.CompanyId = null;
                }
                _unitOfWork.ApplicationUser.Update(tuser);
                _userManager.RemoveFromRoleAsync(tuser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(tuser, UserVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }
            

            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

            
        



        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> obgUserList = _unitOfWork.ApplicationUser.GetAll(includeProperties: "Company").ToList();
            
            
       
            foreach (var user in obgUserList)
            {
 
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

                if (user.Company == null) { user.Company = new() { Name = "" }; }
                ;
            }
            return Json(new { data = obgUserList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody]string? Id)
        {
            var objFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == Id);
            if (objFromDb == null) 
            {
                return Json(new { success = false, message = "Error While Locking/Unlocking" });
            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);

            }
            _unitOfWork.ApplicationUser.Update(objFromDb);
            _unitOfWork.Save();

                return Json(new { success = true, message = "operation succesfull" });
        }
        #endregion


    }
}
