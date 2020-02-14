using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using GameUserDB.Data;
using GameUserDB.Services;
using Microsoft.EntityFrameworkCore;

namespace GameUserDB.Pages.UserManage
{
    public class CreateUserModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private ApplicationUser ApUser { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }
        public CreateUserModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }
        public class InputModel
        {
            [Required(ErrorMessage = "Введите {0}")]
            [EmailAddress(ErrorMessage = "Неверный формат {0}")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Введите {0}")]
            [StringLength(16, ErrorMessage = "{0} должен быть по крайней мере из {2} символов и максимально из {1} символов", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Подтвердите пароль")]
            [Compare("Password", ErrorMessage = "Пароли не совпадают")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Введите {0}")]
            [DataType(DataType.Text)]
            [StringLength(5, ErrorMessage = "{0} должна быть из {1} символов", MinimumLength = 5)]
            [Display(Name = "Последовательность")]
            public string Sequence { get; set; }

            [Required(ErrorMessage = "Введите {0}")]
            [Display(Name = "Счёт")]
            public int Score { get; set; }
        }


        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAddUserAsync()
        {
            IActionResult result = await CreateNewUser("user");
            return result;
        }

        public async Task<IActionResult> OnPostAddAdminAsync()
        {
            IActionResult result = await CreateNewUser("user","admin");
            return result;           
        }
        public async Task<IActionResult> CreateNewUser(params string[] roles)
        {
            if (ModelState.IsValid)
            {
                ApUser = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Score = Input.Score,
                    Position = 0,
                    Sequence = Input.Sequence
                };
                var result = await _userManager.CreateAsync(ApUser, Input.Password);
                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(ApUser);
                    //var callbackUrl = Url.EmailConfirmationLink(ApUser.Id, code, Request.Scheme);
                    //await _emailSender.SendEmailConfirmationAsync(Input.Email, callbackUrl);
                    foreach (string role in roles)
                    {
                        await _userManager.AddToRoleAsync(ApUser, role);//
                    }

                    return RedirectToPage("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();//Сюда дойти не должно!
        }
    }
}