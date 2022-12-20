using CinemaCore.Models;
using System.ComponentModel.DataAnnotations;

namespace WebCinema.ViewModel
{
    public class StaffsViewModel
    {
        public IEnumerable<Staffs> Staffs { get; set; }

        [Display(Name="Имя")]
        public string Name { get; set; }
        [Display(Name="Фамилия")]
        public string Surname { get; set; }
        [Display(Name="Отчество")]
        public string MiddleName { get; set; }
        [Display(Name="Должность")]
        public string Post { get; set; }
        [Display(Name="Стаж")]
        public int WorkExperience { get; set; }

        public SortViewModel SortViewModel { get; set; }

        public PageViewModel PageViewModel { get; set; }
    }
}
