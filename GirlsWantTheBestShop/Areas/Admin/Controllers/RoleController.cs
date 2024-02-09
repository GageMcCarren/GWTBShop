using GirlsWantTheBestShop.Areas.Admin.Models;
using GirlsWantTheBestShop.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks.Dataflow;

namespace GirlsWantTheBestShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        RoleManager<IdentityRole> _roleManager;
        UserManager<IdentityUser> _userManager;
        ApplicationDbContext _context;

       
        public RoleController(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, UserManager<IdentityUser> user)
        {
            _roleManager = roleManager;
            _context = context;
            _userManager = user;
        }


        [Area("Admin")]
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            ViewBag.Roles = roles;

            return View("~/Areas/Admin/Views/Role/Index.cshtml");
        }

        [HttpGet]
        [Route("Admin/Role/Create")]
        public IActionResult Create()
        {
            return View("~/Areas/Admin/Views/Role/Create.cshtml");
        }

        [HttpPost]
        [Route("Admin/Role/Create")]
        public async Task<IActionResult> Create(string name)
        {
            IdentityRole role = new IdentityRole();
            role.Name = name;
            var isExist = await _roleManager.RoleExistsAsync(role.Name);
            if (isExist)
            {
                ViewBag.mgs = "This role already exists";
                ViewBag.name = name;
                return View("~/Areas/Admin/Views/Role/Create.cshtml");
            }
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                TempData["save"] = "Role has been saved successfully";
                return RedirectToAction("Index", new { area = "Admin" });
            }
            return View("Index");
        }


        [HttpGet]
        [Route("Admin/Role/Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var role =await _roleManager.FindByIdAsync(id);
            if(role == null)
            {
                return NotFound();
            }
            ViewBag.id = role.Id;
            ViewBag.name = role.Name;

            return View("~/Areas/Admin/Views/Role/Edit.cshtml");
        }

        [HttpPost]
        [Route("Admin/Role/Edit")]
        public async Task<IActionResult> Edit(string id,string name)
        {
           
            var role = await _roleManager.FindByIdAsync(id);
            if (role==null)
            {
                return NotFound(); 
            }
            role.Name = name;
            var isExist = await _roleManager.RoleExistsAsync(role.Name);
            if (isExist)
            {
                ViewBag.mgs = "This role already exists"; 
                ViewBag.name = name;    
                return View();  
            }
            var result = await _roleManager.UpdateAsync(role);  
            if(result.Succeeded)
            {
                TempData["save"] = "Role has been updated successfully"; 
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        [Route("Admin/Role/Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            ViewBag.id = role.Id;
            ViewBag.name = role.Name;

            return View("~/Areas/Admin/Views/Role/Delete.cshtml");
        }

        [HttpPost]
        [Route("Admin/Role/Delete")]
        public async Task<IActionResult> DeleteConfirmation(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            var result = await _roleManager.DeleteAsync(role); 

            if (result.Succeeded)
            {
                TempData["delete"] = "Role has been deleted successfully";
                return RedirectToAction("Index");
            }

            return View("~/Areas/Admin/Views/Role/Delete.cshtml");
        }


        [HttpGet]
        [Route("Admin/Role/Assign")]
        public async Task<IActionResult> Assign()
        {
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers.Where(f => f.LockoutEnd<DateTime.Now || f.LockoutEnd==null).ToList(), "Id", "UserName") ;
            ViewData["RoleId"] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");

            return View("~/Areas/Admin/Views/Role/Assign.cshtml");
        }


        [HttpPost]
        [Route("Admin/Role/Assign")]
        public async Task<IActionResult> Assign (RoleUserVm roleUser)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(c => c.Id == roleUser.UserId);
           var isCheckRoleAssign = await _userManager.IsInRoleAsync(user, roleUser.RoleId);
            if (isCheckRoleAssign)
            {
                ViewBag.mgs = "This user  is already assigned this role";
                ViewData["UserId"] = new SelectList(_context.ApplicationUsers.Where(f => f.LockoutEnd < DateTime.Now || f.LockoutEnd == null).ToList(), "Id", "UserName");
                ViewData["RoleId"] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name"); 
                return View();
            }

            var role = await _userManager.AddToRoleAsync(user,roleUser.RoleId);
            if(role.Succeeded)
            {
                TempData["save"] = "User Role assigned.";
                return RedirectToAction("Index");
            }

            return View("~/Areas/Admin/Views/Role/Assign.cshtml"); 
        }

        [HttpGet]
        [Route("Admin/Role/AssignUserRole")]
        public ActionResult AssignUserRole()
        {
            var result = from ur in _context.UserRoles
                         join r in _context.Roles on ur.RoleId equals r.Id
                         join a in _context.ApplicationUsers on ur.UserId equals a.Id
                         select new UserRoleMaping()
                         {
                             UserId = ur.UserId,
                             RoleId = ur.RoleId,
                             UserName = a.UserName,
                             RoleName = r.Name
                         };
            ViewBag.UserRoles = result;
            return View("~/Areas/Admin/Views/Role/AssignUserRole.cshtml");
        }
    }
}
