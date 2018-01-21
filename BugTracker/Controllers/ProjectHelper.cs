using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Controllers
{
    public class ProjectHelper
    {

        private UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        private ApplicationDbContext db = new ApplicationDbContext();
        UserRolesHelper userHelper = new UserRolesHelper();


        public void AssignProjectUser(int projectId, string userId)
        {

            var project = db.Projects.Find(projectId);
            var user = db.Users.Find(userId);
            project.Users.Add(user);
            db.SaveChanges();

            // db.Projects.Find(projectId).Users.Add(db.Users.Find(userId));
        }

        public void RemoveProjectUser(int projectId, string userId)
        {
            var project = db.Projects.Find(projectId);
            var user = db.Users.Find(userId);
            project.Users.Remove(user);
            db.SaveChanges();
        }

        // Is user in project
        public bool UserInProjet( int projectId, string userId)
        {
            var user = db.Users.Find(userId);
            var project = db.Projects.Find(projectId);
            if (project.Users.Contains(user)) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // List of project managers - needed?
        public ICollection<ApplicationUser> ProjectManagers()
        {
            var projectManagerList = new List<ApplicationUser>();
            var List = manager.Users.ToList();
            foreach (var user in List)
            {
                if (userHelper.IsUserInRole(user.Id, "Project Manager"))
                    projectManagerList.Add(user);
            }

            return projectManagerList;
        }

        // List projects by role
        public List<Projects> ListProjects(string userId)
        //public IEnumerable<Projects> AssignedProjects(string userId)
        {
            var user = db.Users.Find(userId);
           
            var resultList = new List<Projects>();
            var userProjects = new List<Projects>();
            //List<Projects> resultList = new List<Projects>();


            // Find user projects
            if (userHelper.IsUserInRole(userId,"Admin"))
            {
                userProjects = db.Projects.ToList();
                //userProjects = db.Projects.Include("Tickets").Include("Users").Where(p => p.ProjectManagerId == userId).ToList();
                //devProjects = db.Projects.Where(p => p.Tickets.Any(t => t.AssignedToUserId == userId)).ToList();
                //userProjects = db.Projects.Where(p=> p.ProjectManagerId == userId).ToList();
                //userProjects = db.Projects.Include("Tickets")
            }
            else
            {
                userProjects = db.Projects.Where(p => p.ProjectManagerId.Contains(userId)).ToList();

            }
  
            foreach(var project in userProjects)
            {
                // Only return projects not marked as complete
                Projects projects = new Projects();
                if (!project.ProjectComplete)
                {
                    resultList.Add(project);
                }
            }

            return resultList;

         }




        // Option 1: List users assigned to project
        //public List<ApplicationUser> ListUsersInProject(int projectId)
        public List<string> ListUsersInProject(int projectId)
        {
            var project = db.Projects.Find(projectId);
            var users = project.Users.Select(u => u.Id).ToList();
            return users;
        }



        public bool ProjectComplete(int projectId)
        {
            var flag = db.Projects.Find(projectId);
            if (flag.ProjectComplete == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}