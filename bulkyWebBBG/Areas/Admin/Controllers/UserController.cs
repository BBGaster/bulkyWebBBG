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

namespace bulkyWebBBG.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
    
        private ApplicationDbContext _db;
        [BindProperty]
        public UserVM UserVM { get; set; }
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public UserController(ApplicationDbContext db, IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager )
        {
            _db = db;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
          
        }

        public IActionResult Index()
        {
            

            return View();
        }

        public IActionResult Permission(string? id)
        {
            UserVM = new UserVM()
            {
                ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == id),
                RoleList = _db.Roles.Select(u => new SelectListItem
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

            return View(UserVM);
        }
        [HttpPost]
        [ActionName("Permission")]
        public IActionResult PermissionPOST()
        {

            string RoleID = _db.UserRoles.FirstOrDefault(u => u.UserId == UserVM.ApplicationUser.Id).RoleId;
            string oldRole = _db.Roles.FirstOrDefault(u => u.Id == RoleID).Name;
            ApplicationUser tuser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == UserVM.ApplicationUser.Id);

            if (UserVM.ApplicationUser.Role == SD.Role_User_Comp && tuser.CompanyId != UserVM.ApplicationUser.CompanyId)
            {
                tuser.CompanyId = UserVM.ApplicationUser.CompanyId;
            }

            if (!(UserVM.ApplicationUser.Role == oldRole))
            {
                
                if(oldRole == SD.Role_User_Comp)
                {
                    tuser.CompanyId = null;
                }
                _userManager.RemoveFromRoleAsync(tuser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(tuser, UserVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }
            

            _db.SaveChanges();
            return RedirectToAction("Index");
        }

            
        



        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> obgUserList = _db.ApplicationUsers.Include(u => u.CompanyObj).ToList();
            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach (var user in obgUserList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

                if (user.CompanyObj == null) { user.CompanyObj = new() { Name = "" }; }
                ;
            }
            return Json(new { data = obgUserList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody]string? Id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == Id);
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
            _db.SaveChanges();

                return Json(new { success = true, message = "operation succesfull" });
        }
        #endregion


    }
}
