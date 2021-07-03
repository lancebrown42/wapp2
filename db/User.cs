using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace web2.Models {

	public class User {
		public long UID = 0;
		public string FirstName = string.Empty;
		public string LastName = string.Empty;
		public string UserID = string.Empty;
		public string Password = string.Empty;
		public string Email = string.Empty;
		public ActionTypes ActionType = ActionTypes.NoType;

		public bool IsAuthenticated {
			get {
				if (UID > 0) return true;
				return false;
			}
		}

		public User Login() {
			try {
				Database db = new Database();
				return db.Login(this);
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public User.ActionTypes Save() {
			try {
				Database db = new Database();
				if (UID == 0) { //insert new user
					this.ActionType = db.InsertUser(this);
				}
				else {
					this.ActionType = db.UpdateUser(this);
				}
				return this.ActionType;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public bool RemoveUserSession() {
			try {
				HttpContext.Current.Session["CurrentUser"] = null;
				return true;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public User GetUserSession() {
			try {
				User u = new User();
				if (HttpContext.Current.Session["CurrentUser"] == null) {
					return u;
				}
				u = (User)HttpContext.Current.Session["CurrentUser"];
				return u;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public bool SaveUserSession() {
			try {
				HttpContext.Current.Session["CurrentUser"] = this;
				return true;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public enum ActionTypes {
			NoType = 0,
			InsertSuccessful = 1,
			UpdateSuccessful = 2,
			DuplicateEmail = 3,
			DuplicateUserID = 4,
			Unknown = 5,
			RequiredFieldsMissing = 6,
			LoginFailed = 7
		}
	}
}
///////////////////////////////////////////////////////////////////////////////
//Spring 2021
///////////////////////////////////////////////////////////////////////////////
