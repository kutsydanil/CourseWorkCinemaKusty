using CinemaCore.Models;
using System.ComponentModel.DataAnnotations;

namespace WebCinema.ViewModel
{
    public class CountryProductionsViewModel
    {
        public IEnumerable<CountryProductions> CountryProductions { get; set; }

        [Display(Name = "Страна производства")]
        public string CountryName { get; set; }

        public PageViewModel PageViewModel { get; set; }

        public SortViewModel SortViewModel { get; set; }
    }
}
