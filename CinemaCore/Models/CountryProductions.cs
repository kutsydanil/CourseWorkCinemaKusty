using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace CinemaCore.Models;

public partial class CountryProductions
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Не указано название")]
    [StringLength(50, ErrorMessage = "Длина названия минимум {2} и максимум {1} символов. ",MinimumLength = 3)]
    [Display(Name = "Название")]
    public string? Name { get; set; }

    public virtual ICollection<Films> Films { get; } = new List<Films>();
}
