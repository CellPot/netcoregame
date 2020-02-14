using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using GameUserDB.Data;
using System.ComponentModel.DataAnnotations;

namespace GameUserDB.Pages.UserManage
{
    public class ChangeUserPassModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }
        public string Message { get; set; }
        public ApplicationUser ApUser { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;

        public class InputModel
        {

            [Required(ErrorMessage = "Введите {0}")]
            [StringLength(16, ErrorMessage = "{0} должен быть длиной от {2} до {1} символов", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Введите {0}")]
            [DataType(DataType.Password)]
            [Display(Name = "Подтвердите пароль")]
            [Compare("Password", ErrorMessage = "Пароли не совпадают")]
            public string ConfirmPassword { get; set; }

        }

        public ChangeUserPassModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            ApUser = await _userManager.FindByIdAsync(id);
            if (ApUser == null)
            {
                return RedirectToPage("Index");
            }
            Message = ApUser.Email;

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string id)
        {
            ApUser = await _userManager.FindByIdAsync(id);
            if (!ModelState.IsValid)
            {
                return Page();
            }
            string token = await _userManager.GeneratePasswordResetTokenAsync(ApUser);
            await _userManager.ResetPasswordAsync(ApUser, token, Input.Password);
            await _userManager.UpdateAsync(ApUser);
            Message = ApUser.Email;
            return RedirectToPage("EditUser");
        }
    }
}