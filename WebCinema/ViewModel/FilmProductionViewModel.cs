using CinemaCore.Models;
using System.ComponentModel.DataAnnotations;

namespace WebCinema.ViewModel
{
    public class FilmProductionViewModel
    {
        public IEnumerable<FilmProductions> FilmProductions { get; set; }

        [Display(Name = "Название производителя")]
        public string ProductionName { get; set; }

        public PageViewModel PageViewModel { get; set; }

        public SortViewModel SortViewModel { get; set; }


    }
}
