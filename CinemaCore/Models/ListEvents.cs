using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CinemaCore.Models;

public partial class ListEvents
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Не указана дата")]
    [Display(Name = "Дата")]
    [DataType(DataType.Date)]
    [DefaultValue("20.01.2023")]
    //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = "Не указано время начало")]
    [Display(Name = "Начало")]
    [DataType(DataType.Time)]
    public TimeSpan StartTime { get; set; }

    [Required(ErrorMessage = "Не указано время окончание")]
    [Display(Name = "Окончание")]
    [DataType(DataType.Time)]
    public TimeSpan EndTime { get; set; }

    [Required(ErrorMessage = "Не указана цена билета")]
    [Display(Name = "Цена билета")]
    [Range(0, 1200, ErrorMessage = "Недопустимая цена билета")]
    public decimal TicketPrice { get; set; }

    [Required(ErrorMessage = "Не указан фильм")]
    [Display(Name = "Фильм")]
    public int FilmId { get; set; }

    public virtual Films? Film { get; set; } = null!;

    [Required(ErrorMessage = "Не указан номер зала")]
    [Display(Name = "Номер зала")]
    public int CinemaHallId { get; set; }

    public virtual CinemaHalls? CinemaHall { get; set; }

    public virtual ICollection<Places> Places { get; } = new List<Places>();

    public virtual ICollection<StaffCasts> StaffCasts { get; } = new List<StaffCasts>();
}
