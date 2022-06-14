
using System.ComponentModel.DataAnnotations;

namespace AdminPanel.WebUI.Models
{
    
    public class PlayerRegister
    {
        /// <summary>
        /// Имя игрока
        /// </summary>
        [Required(ErrorMessage = "Обязательное поле - Name")]
        [StringLength(maximumLength: 16)]
        public string Name { get; set; }

        /// <summary>
        /// Информация о железе устройства
        /// </summary>
        [Required(ErrorMessage = "Обязательное поле - HardWareInfo")]
        public string HardWareInfo { get; set; }
    }
}
