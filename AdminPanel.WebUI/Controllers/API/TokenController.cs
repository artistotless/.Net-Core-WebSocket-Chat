using AdminPanel.App.Services;
using AdminPanel.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AdminPanel.WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : Controller
    {
        private PlayerService _playerService;

        public TokenController(PlayerService playerService)
        {
            _playerService = playerService;
        }

        /// <summary>
        /// Получение токена игрока
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> Post([FromBody] PlayerRegister registerData)
        {
            if (ModelState.IsValid)
            {
                var response = await _playerService.RegisterAsync(registerData.Name, registerData.HardWareInfo);
                if (!response.HasError)
                {
                    return Json(response.Result);
                }
                return Json(Problem(detail: response.Message));
            }
            return Json(ValidationProblem());

        }
    }
}
