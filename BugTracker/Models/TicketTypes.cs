using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketTypes
    {
        
        //public TicketTypes()
        //{
        //    Ticket = new HashSet<Tickets>();
        //}

        public int Id { get; set; }

        [Display(Name = "Ticket Type")]
        public string Name { get; set; }

        //public virtual ICollection<Tickets> Ticket { get; set; }
    }
}