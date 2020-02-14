using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using GameUserDB.Data;

namespace GameUserDB.Pages.UserManage
{
    public class ChangeUserRoleModel : PageModel
    {
        public List<IdentityRole> AllRoles { get; set; }
        public IList<string> UserRoles { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ApplicationUser ApUser { get; set; }
        public ChangeUserRoleModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            ApUser = await _userManager.FindByIdAsync(id);
            if (ApUser == null)
            {
                return RedirectToPage("Index");
            }
            //Message = ApUser.Email;
            UserRoles = await _userManager.GetRolesAsync(ApUser);
            AllRoles = _roleManager.Roles.ToList();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string id, string[] selectedRoles)
        {
            ApUser = await _userManager.FindByIdAsync(id);
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // получем список ролей пользователя
            UserRoles = await _userManager.GetRolesAsync(ApUser);
            // получаем все роли
            AllRoles = _roleManager.Roles.ToList();
            // получаем список ролей, которые были добавлены
            var addedRoles = selectedRoles.Except(UserRoles);
            // получаем роли, которые были удалены
            var removedRoles = UserRoles.Except(selectedRoles);
            await _userManager.AddToRolesAsync(ApUser, addedRoles);
            await _userManager.RemoveFromRolesAsync(ApUser, removedRoles);
            return RedirectToPage("Index");
        }
    }
}