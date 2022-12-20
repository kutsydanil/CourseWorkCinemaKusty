using CinemaCore.Models;
using System.ComponentModel.DataAnnotations;

namespace WebCinema.ViewModel
{
    public class GenresViewModel
    {
        public IEnumerable<Genres> Genres { get; set; }

        [Display(Name = "Название жанра")]
        public string GenreName { get; set; }


        public PageViewModel PageViewModel { get; set; }

        public SortViewModel SortViewModel { get; set; }
    }
}
