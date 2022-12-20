using CinemaCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebCinema.ViewModel
{ 
    public class FilmsViewModel
    {
        public IEnumerable<Films> Films { get; set; }

        public SelectList Genres { get; set; }

        public SelectList CountryProductions { get; set; }

        public SelectList FilmProductions { get; set; }

        [Display(Name="Название")]
        public string Name { get; set; }

        [Display(Name="Жанр")]
        public int GenreId { get; set; }

        [Display(Name="Компания")]
        public int FilmProductionId { get; set; }

        [Display(Name="Страна")]
        public int CountryProductionId { get; set; }

        [Display(Name="Описание")]
        public string Description { get; set; }

        [Display(Name="Возрастной ценз")]
        public int? AgeLimit { get; set; }

        [Display(Name="Длительность")]
        public int? Duration { get; set; }

        public SortViewModel SortViewModel { get; set; }

        public PageViewModel PageViewModel { get; set; }
    }
}
