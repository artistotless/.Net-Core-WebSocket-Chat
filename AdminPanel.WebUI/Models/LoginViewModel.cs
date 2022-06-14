
using System.ComponentModel.DataAnnotations;

namespace AdminPanel.WebUI.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Fill the Email field!")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Fill the Password field!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
