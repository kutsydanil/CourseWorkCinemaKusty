using CinemaCore.Models;
using System.ComponentModel.DataAnnotations;

namespace WebCinema.ViewModel
{
    public class ActorsViewModel
    {
        public IEnumerable<Actors> Actors { get; set; }

        [Display(Name="Имя")]
        public string Name { get; set; }

        [Display(Name="Фамилия")]
        public string Surname { get; set; }

        [Display(Name="Отчество")]
        public string MiddleName { get; set; }

        public PageViewModel PageViewModel { get; set; }
        
        public SortViewModel SortViewModel { get; set; }
    }
}
