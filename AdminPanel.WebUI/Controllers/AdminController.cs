using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using AdminPanel.WebUI.Helpers;
using Microsoft.AspNetCore.Authorization;
using AdminPanel.WebUI.Models;
using AdminPanel.App.Services;

namespace AdminPanel.WebUI.Controllers
{
    public class AdminController : Controller
    {
        private OperatorService _operatorService;

        public AdminController(OperatorService operatorService)
        {
            _operatorService = operatorService;
        }

        //Домашняя страница для операторов
        [Authorize]
        public IActionResult Index() => View();

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel data)
        {
            if (ModelState.IsValid)
            {
                var response = await _operatorService.LoginAsync(data.Email, data.Password);
                if (!response.HasError)
                {
                    await Authenticate(response.Result);
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }
            return View(data);
        }

        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel data)
        {
            if (ModelState.IsValid)
            {
                var response = await _operatorService.RegisterAsync(data.Email, data.Name, data.Password);
                if (!response.HasError)
                {
                    await Authenticate(response.Result);
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }
            return View(data);
        }

        private async Task Authenticate(ClaimsIdentity identity)
        {
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", this.ShortName(nameof(AdminController)));
        }
    }
}
