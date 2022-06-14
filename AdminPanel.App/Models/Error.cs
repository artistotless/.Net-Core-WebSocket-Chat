using AdminPanel.App.Models.Abstract;

namespace AdminPanel.App.Models
{
    public class Error
    {
        public const string DB_FAIL = "Ошибка подключения к Базе Данных";
        public const string USER_EXIST = "Такой пользователь уже существует";
        public const string USER_NOT_FOUND = "Пользователь не найден";
        public const string PASSWORD_INVALID = "Неверный пароль";
        public const string UNKNOWN = "Произошла неизвестная ошибка";
        public const string WEBSOCKET = "Произошла ошибка при передаче данных по WebSocket";

        public static void SetError<T>(Response<T> response, string errorMessage)
        {
            response.HasError = true;
            response.Message = errorMessage;
        }
    }
}
