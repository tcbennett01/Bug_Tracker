using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    [Authorize(Roles ="Admin, Project Manager")]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectHelper projectHelper = new ProjectHelper();
        private UserRolesHelper roleHelper = new UserRolesHelper();

        // GET: Projects
        // ISSUES:  Using logged in user to return PM name creates issues when Admin?!?
        public ActionResult Index()
        {
            List<ProjectViewModel> projects = new List<ProjectViewModel>();
            var loggedtUser = User.Identity.GetUserId();
            var currentUser = db.Users.Find(loggedtUser);
            var projectList = projectHelper.ListProjects(User.Identity.GetUserId());
            
            var tickets = new Tickets();
            foreach (var item in projectList)
            {
                var tempVar = new ProjectViewModel();
                tempVar.Id = item.Id;                     
                tempVar.firstName = currentUser.FirstName;
                tempVar.lastName = currentUser.LastName;
                tempVar.fullName = currentUser.FirstName + " " + currentUser.LastName;
                tempVar.projectName = item.Name;
                tempVar.Description = item.Description;
                tempVar.Created = item.Created;
                tempVar.Updated = item.LastUpdate;
                tempVar.ticketCount = item.Tickets.Where(t => t.TicketStatusId != 10).Count();            
                           
                projects.Add(tempVar);
            }

            return View(projects);

        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Projects projects = db.Projects.Find(id);

            var assnUser = new AssignProjectsViewModel();
            assnUser.Id = projects.Id;
            assnUser.userList = new MultiSelectList(db.Users, "Id", "DisplayName", assnUser.selectedUsers);
            assnUser.projectName = projects.Name;
            assnUser.selectedUsers = projectHelper.ListUsersInProject(projects.Id).ToList();

            ProjectDetails pd = new ProjectDetails();

            // Find project tickets that are currently unresolved
            var openProjectTickets = projects.Tickets.Where(p => p.TicketStatusId != 10);

            pd.Project = projects;
            pd.AssignUsers = assnUser;

            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(pd);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {

            var project = new Projects();
            var projManagers = roleHelper.UsersInRole("Project Manager");

            ViewBag.ProjectManagerId = new SelectList(projManagers, "Id", "DisplayName", project.ProjectManagerId);


            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,Name,Description, ProjectManagerId")] Projects projects)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(projects);
                projects.Created = DateTime.Now;  // Auto populate create date
                projects.LastUpdate = DateTime.Now;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(projects);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Projects.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }

            var projManagers = roleHelper.UsersInRole("Project Manager");
            projManagers.Add(null);

            ViewBag.ProjectManagerId = new SelectList(projManagers, "Id", "DisplayName", projects.ProjectManagerId);
            return View(projects);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Edit([Bind(Include = "Id,Name, ProjectManagerId, Created, ProjectComplete, Description")] Projects projects)
        {
            if (ModelState.IsValid)
            {
                projects.LastUpdate = DateTime.Now;
                db.Entry(projects).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(projects);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Projects.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }

        // GET: Project/Assignment
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult AssignUsers(int Id)
        {
            var project = db.Projects.Find(Id);
            //var user = new ProjectManagementViewModel();
            var user = new AssignProjectsViewModel();
            user.Id = Id;
            user.userList = new MultiSelectList(db.Users, "Id", "FirstName", user.selectedUsers);
            user.projectName = project.Name;
            user.selectedUsers = projectHelper.ListUsersInProject(project.Id).ToList();
            return View(user);
        }

        //POST:  Project/Assignment
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult AssignUsers(AssignProjectsViewModel model)
        {
            var x = db.Projects.Find(model.Id);
            foreach (var item in db.Users.Select(u => u.Id).ToList())
            {
                projectHelper.RemoveProjectUser(x.Id, item);
            }
            foreach (var item in model.selectedUsers)
            {
                projectHelper.AssignProjectUser(x.Id, item);
            }
            //return RedirectToAction("Index");
            return RedirectToAction("Details", "Projects", new { id = model.Id });
        }


        // Locking down project delete ability to prevent fk issues
        // since tickets cannot be deleted.  Enhancement = add project
        // status including a "resolved" status.

        // POST: Projects/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Projects projects = db.Projects.Find(id);
        //    db.Projects.Remove(projects);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
