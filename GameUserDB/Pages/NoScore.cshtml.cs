using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GameUserDB.Data;

namespace GameUserDB.Pages
{
    public class NoScoreModel : PageModel
    {
        public string Message { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private ApplicationUser ApUser { get; set; }

        public NoScoreModel(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _context = db;
        }
        public async Task<IActionResult> OnGetAsync()
        {

            if (!User.Identity.IsAuthenticated)//проверка на авторизацию
            {
                Message = "Вы не авторизованы";
                return Page();
            }
            else
            {
                var userID = _userManager.GetUserId(HttpContext.User);//Узнаем ID текущего пользователя
                ApUser = await _context.Users.FindAsync(userID);//Устанавливаем в экземпляр ApUser пользователя из контекста по ID
                if (ApUser == null)//если его ВДРУГ нет, возвращаем NotFound
                {
                    return NotFound();
                }
                Message = "Очки: " + ApUser.Score;
                return Page();
            }

        }
        public async Task<IActionResult> OnPostAsync()
        {
            var userID = _userManager.GetUserId(HttpContext.User);//Узнаем ID текущего пользователя
            ApUser = await _context.Users.FindAsync(userID);//Устанавливаем в экземпляр ApUser пользователя из контекста по ID
            if (ApUser == null)//если его ВДРУГ нет, возвращаем NotFound
            {
                return NotFound();
            }
            if (ApUser.Score < 15)
            {
                ApUser.Score += 150;
                await _context.SaveChangesAsync();//асинхронно сохраняем изменения в контексте
                Message = "Очки: " + ApUser.Score;
                return Page();
            }
            else
            {
                Message = "У вас уже есть " + ApUser.Score + " очков!";
                return Page();
            }

        }
    }
}