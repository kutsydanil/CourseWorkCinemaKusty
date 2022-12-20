using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace WebCinema.ViewModel.Users
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Не указан электронный адрес")]
        [EmailAddress(ErrorMessage = "Некорректный электронный адрес")]
        [Display(Name = "Электронный адрес")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 6)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
}
