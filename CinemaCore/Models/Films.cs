using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CinemaCore.Models;

public partial class Films
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Не указано название")]
    [Display(Name = "Название")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Не указано жанр")]
    [Display(Name = "Жанр")]
    public int GenreId { get; set; }

    [Required(ErrorMessage = "Не указана длительность")]
    [Display(Name = "Длительность")]
    public int Duration { get; set; }

    [Required(ErrorMessage = "Не указан производитель")]
    [Display(Name = "Компания-производитель")]
    public int FilmProductionId { get; set; }

    [Required(ErrorMessage = "Не указана страна")]
    [Display(Name = "Страна производства")]
    public int CountryProductionId { get; set; }

    [Required(ErrorMessage = "Не указано возрастное ограничение")]
    [Display(Name = "Возрастное ограничение")]
    [Range(1, 110, ErrorMessage = "Недопустимый возраст")]
    public int AgeLimit { get; set; }

    [Required(ErrorMessage = "Не указано описание")]
    [Display(Name = "Описание")]
    public string Description { get; set; } = null!;

    public virtual ICollection<ActorCasts> ActorCasts { get; } = new List<ActorCasts>();

    public virtual CountryProductions? CountryProduction { get; set; } = null!;

    public virtual FilmProductions? FilmProduction { get; set; } = null!;

    public virtual Genres? Genre { get; set; } = null!;

    public virtual ICollection<ListEvents> ListEvents { get; } = new List<ListEvents>();
}
