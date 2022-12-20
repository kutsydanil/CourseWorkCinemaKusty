using CinemaCore.Models;
using System.ComponentModel.DataAnnotations;

namespace WebCinema.ViewModel
{
    public class CinemaHallViewModel
    {
        public IEnumerable<CinemaHalls> CinemaHalls { get; set; }

        [Display(Name = "Номер зала")]
        public int? HallNumber { get; set; }

        [Display(Name = "Максимальное количество мест")]
        public int? MaxPlaceNumber { get; set; }

        public SortViewModel SortViewModel { get; set; }

        public PageViewModel PageViewModel { get; set; }
    }
}
