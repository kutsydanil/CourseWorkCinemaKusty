using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CinemaCore.Models;

public partial class Actors
{
    public int Id { get; set; }

    [Display(Name = "Имя")]
    [Required(ErrorMessage = "Не указано имя")]
    [StringLength(50, ErrorMessage = "Длина имени минимум {2} и максимум {1} символов.", MinimumLength = 3)]
    public string Name { get; set; } = null!;
    [Display(Name = "Фамилия")]
    [Required(ErrorMessage = "Не указана фамилия")]
    [StringLength(50, ErrorMessage = "Длина фамилии минимум {2} и максимум {1} символов.", MinimumLength = 3)]
    public string Surname { get; set; } = null!;
    [Required(ErrorMessage = "Не указано отчество")]
    [StringLength(50, ErrorMessage = "Длина отчества минимум {2} и максимум {1} символов.", MinimumLength = 3)]
    [Display(Name = "Отчество")]
    public string MiddleName { get; set; } = null!;

    public virtual ICollection<ActorCasts> ActorCasts { get; } = new List<ActorCasts>();
}
