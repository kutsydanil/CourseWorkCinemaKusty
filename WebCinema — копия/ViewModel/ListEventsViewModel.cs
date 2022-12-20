using CinemaCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace WebCinema.ViewModel
{
    public class ListEventsViewModel
    {
        public IEnumerable<ListEvents> ListEvents { get; set; }

        [Display(Name = "Фильм")]
        public SelectList Films { get; set; }

        [Display(Name = "Зал")]
        public SelectList Halls { get; set; }

        public int HallId { get; set; }

        public int FilmId { get; set; }

        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

        [Display(Name = "Начало")]
        public TimeSpan StartTime { get; set; }

        [Display(Name = "Конец")]
        public TimeSpan EndTime { get; set; }

        [Display(Name = "Цена билета")]
        public decimal TicketPrice { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        public PageViewModel PageViewModel { get; set; }

        public SortViewModel SortViewModel { get; set; }

        public bool SearchForDate { get; set;}
    }
}
