using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WebCinema.Models.Users
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Не указан электронный адрес")]
        [EmailAddress(ErrorMessage = "Некорректный электронный адрес")]
        [Display(Name = "Электронный адрес")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Не указан новый пароль")]
        [StringLength(100, ErrorMessage = "Пароль должен иметь минимум {2} и максимум {1} символов.", MinimumLength = 6)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }


        [Required(ErrorMessage = "Не указан старый пароль")]
        [Display(Name = "Старый пароль")]
        public string OldPassword { get; set; }

    }
}
