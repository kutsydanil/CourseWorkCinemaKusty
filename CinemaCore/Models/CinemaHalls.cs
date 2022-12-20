using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CinemaCore.Models;

public partial class CinemaHalls
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Не указан номер зала")]
    [Display(Name = "Номер зала")]
    public int? HallNumber { get; set; }

    [Required(ErrorMessage = "Не указано вместимость зала")]
    [Display(Name = "Вместимость")]
    public int MaxPlaceNumber { get; set; }

    [Required(ErrorMessage = "Не указано количество рядов")]
    [Display(Name = "Количество рядов")]
    public int Rows { get; set; }

    [Required(ErrorMessage = "Не указано количество мест в ряду")]
    [Display(Name = "Количество мест в ряду")]
    public int Columns { get; set; }

    public virtual ICollection<Places> Places { get; } = new List<Places>();
}
