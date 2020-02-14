using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GameUserDB.Data;
//using Microsoft.AspNetCore.Authorization;

namespace GameUserDB.Pages.UserManage
{
    //[Authorize(Roles = "admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private ApplicationUser ApUser { get; set; }
        public List<ApplicationUser> ApUsers { get; set; }

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public void OnGet()
        {
            ApUsers = _userManager.Users.ToList();
        }
        public async Task<ActionResult> OnPostDel(string id)
        {
            ApUsers = _userManager.Users.ToList();
            ApUser = await _userManager.FindByIdAsync(id);
            if (ApUser != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(ApUser);
            }
            return RedirectToPage("Index");
        }
    }
}