using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class AdminController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        UserRolesHelper roleHelper = new UserRolesHelper();

        // GET:  Admin/Index
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            List<UserRoleViewModel> users = new List<UserRoleViewModel>();
            foreach (var user in db.Users.ToList())
            {
                var roleModel = new UserRoleViewModel();
                var userRoles = roleHelper.ListUserRoles(user.Id).ToList();
                roleModel.user = user;
                roleModel.roles = userRoles;
                users.Add(roleModel);
            }
            return View(users);
        }

        // GET: Admin/RoleManagement
        [Authorize(Roles = "Admin")]
        public ActionResult RoleManagement(string Id)
        {
            var user = db.Users.Find(Id);
            var role = new RoleManagementViewModel();
            role.Id = Id;
            role.roleList = new MultiSelectList(db.Roles, "Name", "Name", role.selectedRoles);
            role.firstName = user.FirstName;
            role.lastName = user.LastName;
            role.selectedRoles = roleHelper.ListUserRoles(user.Id).ToArray();
            return View(role);

        }

        // POST: Admin/RoleManagement
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult RoleManagement(RoleManagementViewModel model)
        {
            var y = db.Users.Find(model.Id);
            foreach (var item in db.Roles.Select(r => r.Name).ToList())
            {
                roleHelper.RemoveUserFromRole(y.Id, item);
            }
            foreach (var item in model.selectedRoles)
            {
                roleHelper.AddUserToRole(y.Id, item);
            }
            return RedirectToAction("Index");
        }
            
    }
}