using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CinemaCore.Models;

public partial class Staffs
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Не указано имя")]
    [Display(Name = "Имя")]
    [StringLength(50, ErrorMessage = "Длина имени минимум {2} и максимум {1} символов.", MinimumLength = 3)]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Не указана фамилия")]
    [Display(Name = "Фамилия")]
    [StringLength(50, ErrorMessage = "Длина фамилии минимум {2} и максимум {1} символов.", MinimumLength = 3)]
    public string Surname { get; set; } = null!;

    [Display(Name = "Отчество")]
    [Required(ErrorMessage = "Не указано отчество")]
    [StringLength(50, ErrorMessage = "Длина отчества минимум {2} и максимум {1} символов.", MinimumLength = 3)]
    public string MiddleName { get; set; } = null!;

    [Display(Name = "Должность")]
    [Required(ErrorMessage = "Не указана должность")]
    [StringLength(50, ErrorMessage = "Длина должности минимум {2} и максимум {1} символов.", MinimumLength = 3)]
    public string Post { get; set; } = null!;

    [Display(Name = "Стаж работы")]
    [Required(ErrorMessage = "Не указан стаж работы")]
    [Range(0, 800, ErrorMessage = "Недопустимый стаж работы")]
    public int WorkExperience { get; set; }

    public virtual ICollection<StaffCasts> StaffCasts { get; } = new List<StaffCasts>();
}
