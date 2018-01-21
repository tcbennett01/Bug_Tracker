using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class ProjectViewModel
    {

        public int Id { get; set; }

        [Display(Name = "Project Name")]
        public string projectName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string Description { get; set; }
        public int ticketCount { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTimeOffset Created { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy h:mm tt}")]
        public DateTimeOffset? Updated { get; set; }

        [Display(Name = "Full Name")]
        public string fullName { get; set; }
        public SelectList projectManagers { get; set; }
        public MultiSelectList developers { get; set; }
        public MultiSelectList userList { get; set; }
        //public List<string>SelectedDevelopers { get; set; }
        public string[] selectedUsers { get; set;  }
        
    }
}