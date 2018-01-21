using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Projects
    {
        public Projects()
        {
            Users = new HashSet<ApplicationUser>();
            Tickets = new HashSet<Tickets>();
        }
        public int Id { get; set; }
        [Display(Name="Project Name")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProjectManagerId { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTimeOffset Created { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy h:mm tt}")]
        public DateTimeOffset LastUpdate { get; set; }

        public bool ProjectComplete { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Tickets> Tickets { get; set; }
    }
}