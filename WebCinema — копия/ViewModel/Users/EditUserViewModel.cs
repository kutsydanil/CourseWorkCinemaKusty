using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WebCinema.ViewModel.Users
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Не указан электронный адрес")]
        [EmailAddress(ErrorMessage = "Некорректный электронный адрес")]
        [Display(Name = "Электронный адрес")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Не указан новый пароль")]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

    }
}
