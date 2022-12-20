using CinemaCore.Models;

namespace WebCinema.ViewModel
{
    public class ListEventsEditViewModel
    {
        public ListEvents ListEvents { get; set; }

        public List<Places> Places { get; set; }

        public CinemaHalls CinemaHalls { get; set; }

        public List<Staffs> Staffs { get; set; }
    }
}
