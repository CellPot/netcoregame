using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
//
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GameUserDB.Data;

namespace GameUserDB.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string MainButMessage { get; set; }
        public string ScoreMessage { get; set; }
        public string WinLoseMessage { get; set; }

        public List<ButtonProp> ButtonPropList { get; set; }

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Sequence sequence;
        private ApplicationUser ApUser { get; set; }
        private SituationMessage SitMessage { get; set; }
        
        public IndexModel(ApplicationDbContext db, UserManager<ApplicationUser> manager)
        {
            _context = db;
            _userManager = manager;
            sequence = new Sequence();
            SitMessage = new SituationMessage();

            ButtonPropList = new List<ButtonProp>();
            for (int i = 0; i < 11; i++)
                ButtonPropList.Add(new ButtonProp());
        }

        public async Task<IActionResult> OnGetAsync()
        {
            MainButMessage = "Новая игра";

            if (!User.Identity.IsAuthenticated)//проверка на авторизацию
            {
                ButtonPropList.ForEach((x) => x.Disabled = true);
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
                //ApUser.Score = 5000;//изменяем данные о пользователя
                //await _context.SaveChangesAsync();//асинхронно сохраняем изменения в контексте

                switch (ApUser.Position)
                {
                    case 0:
                        ButtonPropList.ForEach((x) => x.Disabled = true);
                        ButtonsEnable(0);
                        MainButMessage = "Новая игра";
                        WinLoseMessage = SitMessage.GetMessage(0);
                        break;
                    case 1:
                        ButtonPropList.ForEach((x) => x.Disabled = true);
                        ButtonsEnable(1, 2);
                        MainButMessage = "Новая игра";
                        WinLoseMessage = SitMessage.GetMessage(1);
                        break;
                    case 2:
                        ButtonPropList.ForEach((x) => x.Disabled = true);
                        ButtonsEnable(0, 3, 4);
                        MainButMessage = "Забрать очки";
                        WinLoseMessage = SitMessage.GetMessage(2);
                        break;
                    case 3:
                        ButtonPropList.ForEach((x) => x.Disabled = true);
                        ButtonsEnable(0, 5, 6);
                        MainButMessage = "Забрать очки";
                        WinLoseMessage = SitMessage.GetMessage(3);
                        break;
                    case 4:
                        ButtonPropList.ForEach((x) => x.Disabled = true);
                        ButtonsEnable(0, 7, 8);
                        MainButMessage = "Забрать очки";
                        WinLoseMessage = SitMessage.GetMessage(4);
                        break;
                    case 5:
                        ButtonPropList.ForEach((x) => x.Disabled = true);
                        ButtonsEnable(0, 9, 10);
                        MainButMessage = "Забрать очки";
                        WinLoseMessage = SitMessage.GetMessage(5);
                        break;
                    case 99:
                        ButtonPropList.ForEach((x) => x.Disabled = true);
                        ButtonsEnable(0);
                        MainButMessage = "Забрать очки";
                        WinLoseMessage = SitMessage.GetMessage(6);
                        break;
                    case 95:
                        ButtonPropList.ForEach((x) => x.Disabled = true);
                        ButtonsEnable(0);
                        MainButMessage = "Новая игра";
                        WinLoseMessage = SitMessage.GetMessage(7);
                        ApUser.Position = 0;
                        break;
                    default:
                        return RedirectToPage("/Error");
                }

                ScoreMessage = ApUser.Score.ToString();                
                return Page();
            }
        }

        public void ButtonsDisable(params int[] buttonNumbers)
        {
            foreach (int i in buttonNumbers)
            {
                ButtonPropList[i].Disabled = true;
            }
        }
        public void ButtonsEnable(params int[] buttonNumbers)
        {
            foreach (int i in buttonNumbers)
            {
                ButtonPropList[i].Disabled = false;
            }
        }
        public void ChangeUserPosition(int row, int column)
        {
            var userID = _userManager.GetUserId(HttpContext.User);//Узнаем ID текущего пользователя
            ApUser = _context.Users.Find(userID);//Устанавливаем в экземпляр ApUser пользователя из контекста по ID
            if (ApUser == null)//если его ВДРУГ нет, возвращаем NotFound
            {
                RedirectToPage("/Error");
            }
            char[] userSequence = ApUser.Sequence.ToCharArray();//получаем последовательность посимвольно вида "xxxxx", где х = 0 или 1
            int userColumn = int.Parse(userSequence[row].ToString());//узнаем столбец на указанной строке

            if (userColumn == column)//сравниваем значение столбца
            {
                switch (row)//выбираем строку для смены позиции
                {
                    case 0:
                        ApUser.Position = 2;
                        break;
                    case 1:
                        ApUser.Position = 3;
                        break;
                    case 2:
                        ApUser.Position = 4;
                        break;
                    case 3:
                        ApUser.Position = 5;
                        break;
                    case 4:
                        ApUser.Position = 99;
                        break;
                    default:
                        ApUser.Position = 95;
                        break;
                }
            }
            else
            {
                ApUser.Position = 95;
            }

            _context.Attach(ApUser).State = EntityState.Modified;
            _context.SaveChanges();

            ButtonPropList.ForEach((x) => x.Disabled = true);
        }

        public async Task<IActionResult> OnPostStartAsync()
        {
            if (!User.Identity.IsAuthenticated)//проверка на авторизацию
            {
                return Page();
            }
            var userID = _userManager.GetUserId(HttpContext.User);//Узнаем ID текущего пользователя
            ApUser = await _context.Users.FindAsync(userID);//Устанавливаем в экземпляр ApUser пользователя из контекста по ID
            if (ApUser == null)//если его ВДРУГ нет, возвращаем NotFound
            {
                return NotFound();
            }

            int position = ApUser.Position;
            if (ApUser.Score < 15)
            {
                if (ApUser.Position==95 || ApUser.Position==0)
                {
                    return RedirectToPage("NoScore");
                }
            }
            switch (position)
            {
                case 0:
                case 95:
                        ApUser.Score -= 15;
                        ApUser.Position = 1;
                        ApUser.Sequence = sequence.Shuffle();
                        //ApUser.Sequence = "111111";
                    break;
                case 2:
                        ApUser.Score += 10;
                    goto default;
                case 3:
                        ApUser.Score += 20;
                    goto default;
                case 4:
                        ApUser.Score += 40;
                    goto default;
                case 5:
                        ApUser.Score += 75;
                    goto default;
                case 99:
                        ApUser.Score += 150;
                    goto default;
                default:
                        ApUser.Position = 0;
                        break;
            }
            _context.Attach(ApUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToPage("/Index");
        }

        //Post-методы для каждой из кнопок
        public async Task<IActionResult> OnPostChange1LAsync()
        {
            await Task.Run(() => ChangeUserPosition(0, 0));
            return RedirectToPage("/Index");
        }
        public async Task<IActionResult> OnPostChange1RAsync()
        {
            await Task.Run(() => ChangeUserPosition(0, 1));
            return RedirectToPage("/Index");
        }
        public async Task<IActionResult> OnPostChange2LAsync()
        {
            await Task.Run(() => ChangeUserPosition(1, 0));
            return RedirectToPage("/Index");
        }
        public async Task<IActionResult> OnPostChange2RAsync()
        {
            await Task.Run(() => ChangeUserPosition(1, 1));
            return RedirectToPage("/Index");
        }
        public async Task<IActionResult> OnPostChange3LAsync()
        {
            await Task.Run(() => ChangeUserPosition(2, 0));
            return RedirectToPage("/Index");
        }
        public async Task<IActionResult> OnPostChange3RAsync()
        {
            await Task.Run(() => ChangeUserPosition(2, 1));
            return RedirectToPage("/Index");
        }
        public async Task<IActionResult> OnPostChange4LAsync()
        {
            await Task.Run(() => ChangeUserPosition(3, 0));
            return RedirectToPage("/Index");
        }
        public async Task<IActionResult> OnPostChange4RAsync()
        {
            await Task.Run(() => ChangeUserPosition(3, 1));
            return RedirectToPage("/Index");
        }
        public async Task<IActionResult> OnPostChange5LAsync()
        {
            await Task.Run(() => ChangeUserPosition(4, 0));
            return RedirectToPage("/Index");
        }
        public async Task<IActionResult> OnPostChange5RAsync()
        {
            await Task.Run(() => ChangeUserPosition(4, 1));
            return RedirectToPage("/Index");
        }                
    }
}
