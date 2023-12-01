using System;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public class SystemDate
    {
        public Int32 SystemDateID { get; set; }

        [Required]
        //[Range(typeof(DateTime), "1/1/1900 12:00 AM", "1/1/3000 12:00 AM")]
        // Since we ran into errors with the range validation for the date,
        // we use custom validation on the controller side instead.
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}")]
        public DateTime CurrentDate { get; set; }
    }
}

