using AdminPanel.App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AdminPanel.WebUI.Controllers
{
    [Authorize(Policy = "CanRead")]
    public class DialogController : Controller
    {
        private readonly MessageService _messages;

        public DialogController(MessageService messages)
        {
            _messages = messages;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _messages.GetDialogsAsync();
            if (response.HasError)
            {
                return Problem("Ошибка загрузки диалогов", statusCode: 400);
            }
            return View(response.Result);
        }

        public IActionResult Show(int playerId)
        {
            ViewBag.IncludeWS = true;
            return View(playerId);
        }
    }
}
