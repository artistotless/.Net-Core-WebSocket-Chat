using AdminPanel.App.Models.Abstract;
using AdminPanel.App.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AdminPanel.WebUI.Controllers.API
{
    /// <summary>
    /// 0 - Все, 1 - Непрочитанные, 2 - Прочитанные 
    /// </summary>
    public enum MessageStatus { Any, Unread, Read }

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MessageController : Controller
    {
        private readonly MessageService _messageService;

        public MessageController(MessageService messageService)
        {
            _messageService = messageService;
        }


        /// <summary>
        /// Получение кол-ва cообщений
        /// </summary>
        [HttpGet("{status}")]
        public async Task<JsonResult> Count(MessageStatus status)
        {
            Response<int> response = null;

            switch (status)
            {
                case MessageStatus.Any:
                    response = await _messageService.CountAsync();
                    break;
                case MessageStatus.Read:
                    response = await _messageService.CountOfUnreadAsync();
                    break;
                case MessageStatus.Unread:
                    response = await _messageService.CountOfUnreadAsync();
                    break;
            }

            if (response != null)
            {
                if (!response.HasError)
                {
                    return Json(response.Result);
                }

                return Json(BadRequest(response.Message));
            }

            return Json(ValidationProblem());
        }

    }
}
