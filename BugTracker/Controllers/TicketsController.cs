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
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        UserRolesHelper roleHelper = new UserRolesHelper();

        // GET: Tickets
        [Authorize]
        public ActionResult Index()
        {
            //var tickets = db.Tickets.Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            var openTickets = db.Tickets.Where(c => c.TicketStatusId != 10);
            return View(openTickets.ToList());
            //return View(tickets.ToList());
        }

        // GET: Tickets
        [Authorize]
        public ActionResult Index2()
        {

            //var ticketOwnerId = db.Tickets.FirstOrDefault(t => t.OwnerUserId == user.Id);
            var user = User.Identity.GetUserId();
            var currentUser = db.Users.Find(user);
            List<Tickets> ticketResults = new List<Tickets>();
            var tickets = db.Tickets;  // Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            var projects = db.Projects;
            var ownerTickets = tickets.Where(t => t.OwnerUserId == user);
            var assignedTickets = tickets.Where(t => t.AssignedToUserId == user);
            var userProjects = projects.Where(p => p.ProjectManagerId == user).Include(t => t.Tickets);
            //var test = projects.Find(user).Users.ToList();

            if (roleHelper.IsUserInRole(user, "Submitter"))
            {

                // Tickets for Submitters
                foreach (var t in ownerTickets)
                {
                    ticketResults.Add(t);
                }
            }
            else if (roleHelper.IsUserInRole(user, "Project Manager"))
            {
                // Tickets for Projects Managers
                foreach (var up in userProjects)
                {
                    foreach (var t in up.Tickets)
                    {
                        ticketResults.Add(t);
                    }
                }
            }
            else if (roleHelper.IsUserInRole(user, "Developer"))
            {

                // Tickets for Developers - Addigned Tickets
                foreach (var t in assignedTickets)
                {
                    ticketResults.Add(t);
                }

                // TBD - Tickets for Developers - Assigned Projects

            }
            else  // default to Admin
            {
                foreach (var t in tickets)
                {
                    ticketResults.Add(t);
                }
            }
            return View(ticketResults);
        }


        // GET: Tickets/DeveloperTickets
        [Authorize(Roles = "Admin, Project Manager, Developer")]
        public ActionResult Developer()
        {
            ProjectHelper projectHelper = new ProjectHelper();

            var user = User.Identity.GetUserId();
            var currentUser = db.Users.Find(user);
            List<Projects> devProjects = new List<Projects>();

            // Obtain list of developer projects, pull all tickets for said projects
            // tickts do not hvae to be assigned to dev.
            var devProjTickets = currentUser.Projects.SelectMany(t => t.Tickets.Where(s => s.TicketStatusId != 10));
            return View(devProjTickets);
        }

        // GET: Tickets/Developer
        [Authorize(Roles = "Admin, Project Manager, Developer")]
        public ActionResult DeveloperAssignedTickets()
        {
            var user = User.Identity.GetUserId();
            var currentUser = db.Users.Find(user);
            List<Tickets> ticketResults = new List<Tickets>();
            var tickets = db.Tickets;  // Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            var assignedTickets = tickets.Where(t => t.AssignedToUserId == user);

            // Tickets for Developers - Addigned Tickets
            foreach (var t in assignedTickets)
            {
                ticketResults.Add(t);
            }
            return View(ticketResults);
        }


        // GET: Tickets/Details/5   
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets tickets = db.Tickets.Find(id);
            if (tickets == null)
            {
                return HttpNotFound();
            }
            return View(tickets);
        }

        // GET: Tickets/Create
        [Authorize]
        public ActionResult Create()
        {
            //ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.AssignedToUserId = "Unassigned";

            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");

            //ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId")] Tickets tickets)
        {
            TicketHistories ticketHistory = new TicketHistories();
            
            if (ModelState.IsValid)
            {
                // Create ticket
                tickets.OwnerUserId = User.Identity.GetUserId();  // Assign currently logged in user
                tickets.TicketStatusId = 4;  // Set default status to "4" = "unassigned"
                tickets.Created = DateTime.Now;  // Auto populate blog post create

                // Default priority to 'normal' - PM and Admin can assign priority.
                if (User.IsInRole("Submitter") || User.IsInRole("Developer"))
                {
                    tickets.TicketPriorityId = 1;
                    tickets.TicketTypeId = 6;
                }
 
                // Create initial history entry
                ticketHistory.TicketId = tickets.Id;
                ticketHistory.Property = "Ticket Created";
                ticketHistory.NewValue = "All";
                ticketHistory.UserId = tickets.OwnerUserId;
                ticketHistory.ChangeDate = tickets.Created;

                db.Tickets.Add(tickets);
                db.Histories.Add(ticketHistory);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", tickets.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", tickets.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", tickets.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", tickets.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", tickets.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", tickets.TicketTypeId);
            return View(tickets);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "Admin, Project Manager, Developer")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets tickets = db.Tickets.Find(id);
            if (tickets == null)
            {
                return HttpNotFound();
            }

            var devList = roleHelper.UsersInRole("Developer");
            devList.Add(null);
 
            ViewBag.AssignedToUserId = new SelectList(devList, "Id", "FirstName", tickets.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", tickets.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", tickets.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", tickets.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", tickets.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", tickets.TicketTypeId);
            return View(tickets);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager, Developer")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserID, AssignedToUserId")] Tickets tickets)
        {
            if (ModelState.IsValid)
            {
                //TicketHistories ticketHistory = new TicketHistories();
                HistoryHelper historyHelper = new HistoryHelper();

                // Coded StringBuilder to build developer notification message - might take another approach?!?
                // Note:  if StringBuilder is implemented, remove initial message, at end check to see if length
                // > 0, if so then send notification, else nothing changed.
                StringBuilder updateMessage = new StringBuilder();
                //updateMessage.AppendFormat("The following upatates have been made to ticket: {0}\n", tickets.Id);

                // Retrieve old ticket info & update history table         
                var oldTicketInfo = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == tickets.Id);

                if (oldTicketInfo.Title != tickets.Title)
                {
                    historyHelper.AddHistory(tickets.Id, "Title", oldTicketInfo.Title, tickets.Title, User.Identity.GetUserId());
                    updateMessage.AppendFormat("Ticket Title: {0}, ", tickets.Title);
                }

                if (oldTicketInfo.Description != tickets.Description)
                {
                    historyHelper.AddHistory(tickets.Id, "Description", oldTicketInfo.Description, tickets.Description, User.Identity.GetUserId());
                    updateMessage.AppendFormat("Description: {0}, ", tickets.Description);
                }

                if (oldTicketInfo.TicketTypeId != tickets.TicketTypeId)
                {
                    var oldTicketType = db.TicketTypes.Find(oldTicketInfo.TicketTypeId).Name;
                    var newTicketType = db.TicketTypes.Find(tickets.TicketTypeId).Name;
                    historyHelper.AddHistory(tickets.Id, "Ticket Type", oldTicketType, newTicketType, User.Identity.GetUserId());
                    updateMessage.AppendFormat("Ticket Type: {0}, ", newTicketType);
                }

                if (oldTicketInfo.ProjectId != tickets.ProjectId)
                {
                    var oldProject = db.Projects.Find(oldTicketInfo.ProjectId).Name;
                    var newProject = db.Projects.Find(tickets.ProjectId).Name;
                    historyHelper.AddHistory(tickets.Id, "Project Id", oldProject, newProject, User.Identity.GetUserId());
                    updateMessage.AppendFormat("Project: {0}, ", newProject);
                }

                if (oldTicketInfo.TicketPriorityId != tickets.TicketPriorityId)
                {
                    var oldPriority = db.TicketPriorities.Find(oldTicketInfo.TicketPriorityId).Name;
                    var newPriority = db.TicketPriorities.Find(tickets.TicketPriorityId).Name;
                    historyHelper.AddHistory(tickets.Id, "Priority", oldPriority, newPriority, User.Identity.GetUserId());
                    updateMessage.AppendFormat("Ticket Priority: {0}, ", newPriority);
                }

                if (oldTicketInfo.TicketStatusId != tickets.TicketStatusId)
                {
                    var oldStatus = db.TicketStatuses.Find(oldTicketInfo.TicketStatusId).Name;
                    var newStatus = db.TicketStatuses.Find(tickets.TicketStatusId).Name;
                    historyHelper.AddHistory(tickets.Id, "Ticket Status", oldStatus, newStatus, User.Identity.GetUserId());
                    updateMessage.AppendFormat("Ticket Status: {0}, ", newStatus);
                }

                if (oldTicketInfo.AssignedToUserId != tickets.AssignedToUserId 
                    && oldTicketInfo.AssignedToUserId != null)
                {
                    var oldAssignedUser = db.Users.Find(oldTicketInfo.AssignedToUserId).FirstName + " " + db.Users.Find(tickets.AssignedToUserId).LastName;
                    var newAssignedUser = db.Users.Find(tickets.AssignedToUserId).FirstName + " " + db.Users.Find(tickets.AssignedToUserId).LastName;
                    historyHelper.AddHistory(tickets.Id, "Assigned User", oldAssignedUser, newAssignedUser, User.Identity.GetUserId());
                    updateMessage.AppendFormat("Assigned User: {0}, ", newAssignedUser);
                }

                if (oldTicketInfo.AssignedToUserId == null && oldTicketInfo.AssignedToUserId != tickets.AssignedToUserId)
                {
                    var oldAssignedUser = "Unassigned";
                    var newAssignedUser = db.Users.Find(tickets.AssignedToUserId).FirstName + " " + db.Users.Find(tickets.AssignedToUserId).LastName;
                    historyHelper.AddHistory(tickets.Id, "Assigned User", oldAssignedUser, newAssignedUser, User.Identity.GetUserId());
                    updateMessage.AppendFormat("Assigned User: {0}, ", newAssignedUser);
                }

                tickets.Updated = DateTime.Now;
                if (tickets.TicketStatusId == 4)
                {
                    tickets.AssignedToUserId = null;
                }
                db.Entry(tickets).State = EntityState.Modified;
                db.SaveChanges();

                //Send Notification
                var developer = db.Users.Find(tickets.AssignedToUserId);
                if (developer != null && developer.Email != null)
                {
                    var svc = new EmailService();
                    var msg = new IdentityMessage();
                    msg.Destination = developer.Email;
                    msg.Subject = "Bug Tracker Update: " + tickets.Title;
                    msg.Body = ("The following changes have been made to Ticket ID: " + tickets.Id + " - " + tickets.Title + ": " + updateMessage);
                    await svc.SendAsync(msg);
                }

                if (User.IsInRole("Developer"))
                {
                    return RedirectToAction("Developer", "Tickets");
                }
                else if(User.IsInRole("Project Manager")
                        || (User.IsInRole("Admin")))
                {
                    return RedirectToAction("Index", "Projects");
                }

            }
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", tickets.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", tickets.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", tickets.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", tickets.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", tickets.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", tickets.TicketTypeId);
            return View(tickets);
        }

        //public async Task<ActionResult> TicketNotification(ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await UserManager.FindByNameAsync(model.Email);
        //        if (user == null)
        //        {
        //            // Don't reveal that the user does not exist or is not confirmed
        //            return View("ForgotPasswordConfirmation");
        //        }

        //        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //        // Send an email with this link
        //        string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        //        var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //        await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
        //        return RedirectToAction("ForgotPasswordConfirmation", "Account");
        //    }

            //Note:  Tickets can only be "Resolved" per SRS - not deleted

            // GET: Tickets/Delete/5
            //public ActionResult Delete(int? id)
            //{
            //    if (id == null)
            //    {
            //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //    }
            //    Tickets tickets = db.Tickets.Find(id);
            //    if (tickets == null)
            //    {
            //        return HttpNotFound();
            //    }
            //    return View(tickets);
            //}

            // POST: Tickets/Delete/5
            //[HttpPost, ActionName("Delete")]
            //[ValidateAntiForgeryToken]
            //public ActionResult DeleteConfirmed(int id)
            //{
            //    Tickets tickets = db.Tickets.Find(id);
            //    db.Tickets.Remove(tickets);
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
