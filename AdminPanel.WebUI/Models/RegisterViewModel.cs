
using System.ComponentModel.DataAnnotations;

namespace AdminPanel.WebUI.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Fill the Email field!")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Fill the Name field!")]
        [StringLength(25,ErrorMessage ="Maximum length of Name - 25 symbols")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Fill the Password field!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
