namespace BugTracker.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context)
        {
            // Create role(s)
            var roleManager = new RoleManager<IdentityRole>(
            new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }

            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }

            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            // Create user(s) if s/he do not exist
            var userManager = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "tcbenett01@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "tcbennett01@gmail.com",
                    Email = "tcbennett01@gmail.com",
                    FirstName = "Tim",
                    LastName = "Bennett",
                    DisplayName = "Tim"
                }, "initialP@ssword!");
            }

            if (!context.Users.Any(u => u.Email == "ProjectManager1@test.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "ProjectManager1@test.com",
                    Email = "ProjectManager1@test.com",
                    FirstName = "Joe",
                    LastName = "ProjectManager1",
                    DisplayName = "Joe"
                }, "initialP@ssword!");
            }

            if (!context.Users.Any(u => u.Email == "ProjectManager2@test.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "ProjectManager2@test.com",
                    Email = "ProjectManager2@test.com",
                    FirstName = "Jan",
                    LastName = "ProjectManager2",
                    DisplayName = "Jan"
                }, "initialP@ssword!");
            }

            if (!context.Users.Any(u => u.Email == "guestsubmitter@mybugtracker.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "guestsubmitter@mybugtracker.com",
                    Email = "guestsubmitter@mybugtracker.com",
                    FirstName = "Guest",
                    LastName = "Submitter",
                    DisplayName = "Guest Submitter"
                }, "initialP@ssword!");
            }

            if (!context.Users.Any(u => u.Email == "guestdeveloper@mybugtracker.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "guestdeveloper@mybugtracker.com",
                    Email = "guestdeveloper@mybugtracker.com",
                    FirstName = "Guest",
                    LastName = "Developer",
                    DisplayName = "Guest Developer"
                }, "initialP@ssword!");
            }

            if (!context.Users.Any(u => u.Email == "guestprojectmanager@mybugtracker.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "guestprojectmanager@mybugtracker.com",
                    Email = "guestprojectmanager@mybugtracker.com",
                    FirstName = "Guest",
                    LastName = "ProjectManager",
                    DisplayName = "Guest ProjectManager"
                }, "initialP@ssword!");
            }

            if (!context.Users.Any(u => u.Email == "guestadmin@mybugtracker.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "guestadmin@mybugtracker.com",
                    Email = "guestadmin@mybugtracker.com",
                    FirstName = "Guest",
                    LastName = "Admin",
                    DisplayName = "Guest Admin"
                }, "initialP@ssword!");
            }

            if (!context.Users.Any(u => u.Email == "Developer1@test.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "Developer1@test.com",
                    Email = "Developer1@test.com",
                    FirstName = "Bill",
                    LastName = "Developer1",
                    DisplayName = "Bill"
                }, "initialP@ssword!");
            }

            if (!context.Users.Any(u => u.Email == "Developer2@test.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "Developer2@test.com",
                    Email = "Developer2@test.com",
                    FirstName = "Lisa",
                    LastName = "Developer2",
                    DisplayName = "Lisa"
                }, "initialP@ssword!");
            }

            if (!context.Users.Any(u => u.Email == "Submitter1@test.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "Submitter1@test.com",
                    Email = "Submitter1@test.com",
                    FirstName = "Sally",
                    LastName = "Submitter",
                    DisplayName = "Sal"
                }, "initialP@ssword!");
            }

            if (!context.Users.Any(u => u.Email == "Submitter2@test.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "Submitter2@test.com",
                    Email = "Submitter2@test.com",
                    FirstName = "Paul",
                    LastName = "Submitter2",
                    DisplayName = "Paul"
                }, "initialP@ssword!");
            }

            if (!context.Users.Any(u => u.Email == "Submitter3@test.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "Submitter3@test.com",
                    Email = "Submitter3@test.com",
                    FirstName = "Angie",
                    LastName = "Submitter3",
                    DisplayName = "Angie"
                }, "initialP@ssword!");
            }

            if (!context.Users.Any(u => u.Email == "Submitter4@test.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "Submitter4@test.com",
                    Email = "Submitter4@test.com",
                    FirstName = "Harry",
                    LastName = "Submitter4",
                    DisplayName = "Harry"
                }, "initialP@ssword!");
            }

            // Add user(s) to role
            var userAdmin = userManager.FindByEmail("tcbennett01@gmail.com").Id;
            userManager.AddToRole(userAdmin, "Admin");

            var userPM1 = userManager.FindByEmail("ProjectManager1@test.com").Id;
            userManager.AddToRole(userPM1, "Project Manager");

            var userPM2 = userManager.FindByEmail("ProjectManager2@test.com").Id;
            userManager.AddToRole(userPM2, "Project Manager");

            var userDev1 = userManager.FindByEmail("Developer1@test.com").Id;
            userManager.AddToRole(userDev1, "Developer");

            var userDev2 = userManager.FindByEmail("Developer2@test.com").Id;
            userManager.AddToRole(userDev2, "Developer");

            var userSubmit1 = userManager.FindByEmail("Submitter1@test.com").Id;
            userManager.AddToRole(userSubmit1, "Submitter");

            var userSubmit2 = userManager.FindByEmail("Submitter2@test.com").Id;
            userManager.AddToRole(userSubmit2, "Submitter");

            var userSubmit3 = userManager.FindByEmail("Submitter3@test.com").Id;
            userManager.AddToRole(userSubmit3, "Submitter");

            var userSubmit4 = userManager.FindByEmail("Submitter4@test.com").Id;
            userManager.AddToRole(userSubmit4, "Submitter");

            var userGuest1 = userManager.FindByEmail("guestsubmitter@mybugtracker.com").Id;
            userManager.AddToRole(userGuest1, "Submitter");

            var userGuest2 = userManager.FindByEmail("guestdeveloper@mybugtracker.com").Id;
            userManager.AddToRole(userGuest2, "Developer");

            var userGuest3 = userManager.FindByEmail("guestprojectmanager@mybugtracker.com").Id;
            userManager.AddToRole(userGuest3, "Project Manager");

            var userGuest4 = userManager.FindByEmail("guestadmin@mybugtracker.com").Id;
            userManager.AddToRole(userGuest4, "Admin");

            if (!context.TicketTypes.Any(t => t.Name == "Software"))
            {
                context.TicketTypes.Add(new TicketTypes { Name = "Software" });
            }

            if (!context.TicketTypes.Any(t => t.Name == "Design"))
            {
                context.TicketTypes.Add(new TicketTypes { Name = "Design" });
            }

            if (!context.TicketTypes.Any(t => t.Name == "Hardware"))
            {
                context.TicketTypes.Add(new TicketTypes { Name = "Hardware" });
            }

            if (!context.TicketTypes.Any(t => t.Name == "User Experience"))
            {
                context.TicketTypes.Add(new TicketTypes { Name = "User Experience" });
            }

            if (!context.TicketTypes.Any(t => t.Name == "Other"))
            {
                context.TicketTypes.Add(new TicketTypes { Name = "Other" });
            }

            if (!context.TicketTypes.Any(t => t.Name == "Unassigned"))
            {
                context.TicketTypes.Add(new TicketTypes { Name = "Unassigned" });
            }

            if (!context.TicketStatuses.Any(t => t.Name == "Assigned"))
            {
                context.TicketStatuses.Add(new TicketStatuses { Name = "Assigned" });
            }

            if (!context.TicketStatuses.Any(t => t.Name == "Development"))
            {
                context.TicketStatuses.Add(new TicketStatuses { Name = "Development" });
            }

            if (!context.TicketStatuses.Any(t => t.Name == "Testing"))
            {
                context.TicketStatuses.Add(new TicketStatuses { Name = "Testing" });
            }

            if (!context.TicketStatuses.Any(t => t.Name == "Unassigned"))
            {
                context.TicketStatuses.Add(new TicketStatuses { Name = "Unassigned" });
            }

            if (!context.TicketStatuses.Any(t => t.Name == "Resolved"))
            {
                context.TicketStatuses.Add(new TicketStatuses { Name = "Resolved" });
            }

            if (!context.TicketPriorities.Any(t => t.Name == "Low"))
            {
                context.TicketPriorities.Add(new TicketPriorities { Name = "Low" });
            }

            if (!context.TicketPriorities.Any(t => t.Name == "Medium"))
            {
                context.TicketPriorities.Add(new TicketPriorities { Name = "Medium" });
            }

            if (!context.TicketPriorities.Any(t => t.Name == "High"))
            {
                context.TicketPriorities.Add(new TicketPriorities { Name = "High" });
            }

            if (!context.TicketPriorities.Any(t => t.Name == "Critical"))
            {
                context.TicketPriorities.Add(new TicketPriorities { Name = "Critical" });
            }
        }
    }
}
