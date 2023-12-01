using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

//TODO: Change this namespace to match your project
namespace FinalProject.Models
{
    public class DisplayStartTimesViewModel
    {
        public int TransactionID { get; set; }
        public List<Schedule> Schedules { get; set; }
        public Movie SelectedMovie { get; set; }
        public int SelectedScheduleID { get; set; }
    }

}