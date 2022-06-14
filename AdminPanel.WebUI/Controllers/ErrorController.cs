using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.WebUI.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }

        public string CustomError()
        {
            return "custom error";
        }

        //TODO: здесь будут другие методы действий для пользовательских ошибок
    }
}
