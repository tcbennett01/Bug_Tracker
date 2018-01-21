using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketHistories
    {
       
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Property { get; set; } 
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public bool? Changed { get; set; }
        public string UserId { get; set; }

        [Display(Name="Change Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy h:mm tt}")]
        public DateTimeOffset? ChangeDate { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Tickets Ticket { get; set; }
    }
}