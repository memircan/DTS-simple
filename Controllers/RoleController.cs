using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models.Authentication;
using WebApplication1.Models.ViewModels;
using System.Linq;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        
        readonly RoleManager<AppRole> _roleManager;
        readonly UserManager<AppUser> _userManager;
        public RoleController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // ROL ATAMA-YETKİ VERME
        public async Task<IActionResult> RoleAssign(string id)
        {   
            if(id == "1")
            {
                TempData["ErrorMsg"] = "Ana Kullanıcı(admin) yetkileri değiştirilemez";
                return RedirectToAction("Index", "User");
            }
            else
            {
                AppUser user = await _userManager.FindByIdAsync(id);
                List<AppRole> allRoles = _roleManager.Roles.ToList();
                List<string> userRoles = await _userManager.GetRolesAsync(user) as List<string>;
                List<RoleAssignViewModel> assignRoles = new List<RoleAssignViewModel>();
                allRoles.ForEach(role => assignRoles.Add(new RoleAssignViewModel
                {
                    HasAssign = userRoles.Contains(role.Name),
                    RoleId = role.Id,
                    RoleName = role.Name
                }));
                return View(assignRoles);
            }
            
        }
        [HttpPost]
        public async Task<ActionResult> RoleAssign(List<RoleAssignViewModel> modelList, string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user.Id != 1)            
                foreach (RoleAssignViewModel role in modelList)
                    if (role.HasAssign)
                        await _userManager.AddToRoleAsync(user, role.RoleName);
                    else
                        await _userManager.RemoveFromRoleAsync(user, role.RoleName);
                return RedirectToAction("Index","User");
        }


        


    }

}
