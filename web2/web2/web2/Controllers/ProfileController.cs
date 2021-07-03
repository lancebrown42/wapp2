using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace web2.Controllers
{
	public class ProfileController : Controller
	{
		public ActionResult Gallery()
		{
			Models.User u = new Models.User();
			u = u.GetUserSession();
			if (u.IsAuthenticated) {
				Models.Database db = new Models.Database();
				u.Images = db.GetUserImages(u.UID);
			}
			return View(u);
		}

		[HttpPost]
		public ActionResult Gallery(IEnumerable<HttpPostedFileBase> files)
		{
			Models.User u = new Models.User();
			u = u.GetUserSession();
			foreach (var file in files) {
				u.AddGalleryImage(file);
			}
			return Json("file(s) uploaded successfully");
		}

		[HttpPost]
		public JsonResult DeleteImage(long UID, long ID)
		{
			try {
				string type = string.Empty;
				Models.Database db = new Models.Database();
				if (db.DeleteUserImage(ID)) return Json(new { Status = 1 }); //deleted
				return Json(new { Status = 0 }); //not deleted
			}
			catch (Exception) {
				return Json(new { Status = -1 }); //error
			}
		}

		public ActionResult MyEvents()
		{
			Models.User u = new Models.User();
			u = u.GetUserSession();
			return View(u);
		}

		public ActionResult Index()
		{
			Models.User u = new Models.User();
			u = u.GetUserSession();
			if (u.IsAuthenticated) {
				Models.Database db = new Models.Database();
				List<Models.Image> images = new List<Models.Image>();
				images = db.GetUserImages(u.UID, 0, true);
				u.UserImage = new Models.Image();
				if (images.Count > 0) u.UserImage = images[0];
			}
			return View(u);
		}

		[HttpPost]
		public ActionResult Index(HttpPostedFileBase UserImage, FormCollection col)
		{
			try {
				Models.User u = new Models.User();
				u = u.GetUserSession();

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
					if (col["btnSubmit"] == "update") { //update button pressed
						u.Save();

						u.UserImage = new Models.Image();
						u.UserImage.ImageID = System.Convert.ToInt32(col["UserImage.ImageID"]);

						if (UserImage != null) {
							u.UserImage = new Models.Image();
							u.UserImage.ImageID = Convert.ToInt32(col["UserImage.ImageID"]);
							u.UserImage.Primary = true;
							u.UserImage.FileName = Path.GetFileName(UserImage.FileName);
							if (u.UserImage.IsImageFile()) {
								u.UserImage.Size = UserImage.ContentLength;
								Stream stream = UserImage.InputStream;
								BinaryReader binaryReader = new BinaryReader(stream);
								u.UserImage.ImageData = binaryReader.ReadBytes((int)stream.Length);
								u.UpdatePrimaryImage();
							}
						}

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

		public ActionResult SignIn()
		{
			Models.User u = new Models.User();
			return View(u);
		}

		[HttpPost]
		public ActionResult SignIn(FormCollection col)
		{
			try {
				Models.User u = new Models.User();

				if (col["btnSubmit"] == "signin") {
					u.UserID = col["UserID"];
					u.Password = col["Password"];
					if(u.UserID==String.Empty || u.Password == String.Empty) {
						u.ActionType = Models.User.ActionTypes.RequiredFieldsMissing;
						return (View(u));
					}

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

		public ActionResult SignUp()
		{
			Models.User u = new Models.User();
			return View(u);
		}

		[HttpPost]
		public ActionResult SignUp(FormCollection col)
		{
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
						Models.User.ActionTypes at = Models.User.ActionTypes.NoType;
						at = u.Save();
						switch (at) {
							case Models.User.ActionTypes.InsertSuccessful:
								u.SaveUserSession();
								return RedirectToAction("Index");
							//break;
							default:
								return View(u);
								//break;
						}
					}
					else {
						return View(u);
					}
				}
			}
			catch (Exception) {
				Models.User u = new Models.User();
				return View(u);
			}
		}

		public ActionResult SignOut()
		{
			Models.User u = new Models.User();
			u.RemoveUserSession();
			return RedirectToAction("Index", "Home");
		}
	}
}