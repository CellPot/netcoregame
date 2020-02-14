using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GameUserDB.Data;
using System.ComponentModel.DataAnnotations;

namespace GameUserDB.Pages.UserManage
{
    public class EditUserModel : PageModel
    {
        public ApplicationUser ApUser { get; set; }
        public string Message { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;

        public class InputModel
        {
            [Required(ErrorMessage = "{0} не может быть пустым")]
            [EmailAddress(ErrorMessage = "Неверный формат {0}")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Введите {0}")]
            [StringLength(5, ErrorMessage = "{0} должна быть из {1} символов", MinimumLength = 5)]
            [Display(Name = "Последовательность")]
            public string Sequence { get; set; }


            [Required(ErrorMessage = "Введите {0}")]
            [Display(Name = "Счёт")]
            public int Score { get; set; }

        }

        public EditUserModel(UserManager<ApplicationUser> userManager)
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
            ApUser.UserName = Input.Email;
            ApUser.Email = Input.Email;
            ApUser.Score = Input.Score;
            ApUser.Sequence = Input.Sequence;
            ApUser.Position = 0;
            await _userManager.UpdateAsync(ApUser);
            Message = ApUser.Email;
            return RedirectToPage("Index");
        }
    }
}