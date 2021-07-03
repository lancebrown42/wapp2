using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web2.Controllers {
	public class ProfileController : Controller {

		public ActionResult SignUp() {
			Models.User u = new Models.User();
			return View(u);
		}

		[HttpPost]
		public ActionResult SignUp(FormCollection col) {
			try {
				Models.User u = new Models.User();

				u.FirstName = col["FirstName"];
				u.LastName = col["LastName"];
				u.Email = col["Email"];
				u.UserID = col["UserID"];
				u.Password = col["Password"];

				if (u.FirstName.Length == 0 || u.LastName.Length == 0 || u.Email.Length == 0 || u.UserID.Length == 0 || u.Password.Length == 0) {
					u.ActionType = Models.User.ActionTypes.RequiredFieldsMissing;
					return View(u);
				}
				else {
					if (col["btnSubmit"] == "signup") { //sign up button pressed
						u.Save();
						u.SaveUserSession();
						return RedirectToAction("Index");
					}
					return View(u);
				}
			}
			catch (Exception) {
				Models.User u = new Models.User();
				return View(u);
			}
		}

		public ActionResult Index() {
			Models.User u = new Models.User();
			u = u.GetUserSession();
			return View(u);
		}

		[HttpPost]
		public ActionResult Index(HttpPostedFileBase UserImage, FormCollection col) {
			Models.User u = new Models.User();
			u = u.GetUserSession();

			if (col["btnSubmit"] == "update") { //update button pressed
				u = u.GetUserSession();

				u.FirstName = col["FirstName"];
				u.LastName = col["LastName"];
				u.Email = col["Email"];
				u.UserID = col["UserID"];
				u.Password = col["Password"];

				u.Save();

				u.SaveUserSession();
				return RedirectToAction("Index");
			}
			return View(u);
		}


		public ActionResult SignIn() {
			Models.User u = new Models.User();
			return View(u);
		}

		[HttpPost]
		public ActionResult SignIn(FormCollection col) {
			try {
				Models.User u = new Models.User();

				if (col["btnSubmit"] == "signin") {
					u.UserID = col["UserID"];
					u.Password = col["Password"];

					u = u.Login();
					if (u != null && u.UID > 0) {
						u.SaveUserSession();
						return RedirectToAction("Index");
					}
					else {
						u = new Models.User();
						u.UserID = col["UserID"];
						u.ActionType = Models.User.ActionTypes.LoginFailed;
					}
				}
				return View(u);
			}
			catch (Exception) {
				Models.User u = new Models.User();
				return View(u);
			}
		}

		public ActionResult SignOut() {
			Models.User u = new Models.User();
			u.RemoveUserSession();
			return RedirectToAction("Index", "Home");
		}
	}
}
///////////////////////////////////////////////////////////////////////////////
//Spring 2021
///////////////////////////////////////////////////////////////////////////////
