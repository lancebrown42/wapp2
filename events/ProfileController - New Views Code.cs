
		public ActionResult Event() {
			Models.User u = new Models.User();
			Models.Event e = new Models.Event();
			u = u.GetUserSession();
			e.User = u;

			if (e.User.IsAuthenticated) {
				if (RouteData.Values["id"] == null) { //add an empty event
					e.Start = new DateTime(DateTime.Now.Year + 1, DateTime.Now.Month, DateTime.Now.Day, 13, 0, 0);
					e.End = new DateTime(DateTime.Now.Year + 1, DateTime.Now.Month, DateTime.Now.Day, 17, 0, 0);
				}
				else { //get the event
					long id = Convert.ToInt64(RouteData.Values["id"]);
					e = e.GetEvent(id);
				}
			}
			return View(e);
		}

		[HttpPost]
		public ActionResult Event(HttpPostedFileBase EventImage, FormCollection col) {
			Models.User u = new Models.User();
			u = u.GetUserSession();

			if (col["btnSubmit"] == "close") {
				if(col["from"] == null) return RedirectToAction("MyEvents");
				return RedirectToAction("Index", "Home");
			}

			if (col["btnSubmit"] == "event-gallery") {
				return RedirectToAction("EventGallery", new { @id = Convert.ToInt64(RouteData.Values["id"]) });
			}

			if (col["btnSubmit"] == "delete") {
				long lngID = Convert.ToInt64(RouteData.Values["id"]);
				return RedirectToAction("DeleteEvent", new { @id = lngID });
			}

			if (col["btnSubmit"] == "save") {

				Models.Event e = new Models.Event();

				if (RouteData.Values["id"] != null) e.ID = Convert.ToInt64(RouteData.Values["id"]);
				e.User = u;
				e.Title = col["Title"];
				if (col["IsActive"].ToString().Contains("true")) e.IsActive = true; else e.IsActive = false;
				e.Description = col["Description"];

				e.Start = DateTime.Parse(string.Concat(col["Start"].ToString(), " ", col["Start.TimeOfDay"]));
				e.End = DateTime.Parse(string.Concat(col["End"].ToString(), " ", col["End.TimeOfDay"]));

				e.Location = new Models.Location();
				e.Location.Title = col["Location.Title"];
				e.Location.Description = col["Location.Description"];

				e.Location.Address = new Models.Address();
				e.Location.Address.Address1 = col["Location.Address.Address1"];
				e.Location.Address.Address2 = col["Location.Address.Address2"];
				e.Location.Address.City = col["Location.Address.City"];
				e.Location.Address.State = col["Location.Address.State"];
				e.Location.Address.Zip = col["Location.Address.Zip"];

				if (e.Title.Length == 0 || e.Description.Length == 0 || e.Location.Title.Length == 0) {
					e.ActionType = Models.Event.ActionTypes.RequiredFieldsMissing;
					return View(e);
				}

				e.Save();

				if (EventImage != null) {
					e.EventImage = new Models.Image();
					if (col["EventImage.ImageID"].ToString() == "") {
						e.EventImage.ImageID = 0;
					}
					else {
						e.EventImage.ImageID = Convert.ToInt32(col["EventImage.ImageID"]);
					}

					e.EventImage.Primary = true;
					e.EventImage.FileName = Path.GetFileName(EventImage.FileName);
					if (e.EventImage.IsImageFile()) {
						e.EventImage.Size = EventImage.ContentLength;
						Stream stream = EventImage.InputStream;
						BinaryReader binaryReader = new BinaryReader(stream);
						e.EventImage.ImageData = binaryReader.ReadBytes((int)stream.Length);

						e.UpdatePrimaryImage();
					}
				}

				if (e.ID > 0) {
					return RedirectToAction("Event", new { @id = e.ID });
				}
			}
			return View();
		}

		public ActionResult EventGallery() {
			Models.Event e = new Models.Event();
			Models.User u = new Models.User();
			u = u.GetUserSession();
			e.User = u;

			if (e.User.IsAuthenticated) {
				Models.Database db = new Models.Database();
				long lngID = Convert.ToInt64(RouteData.Values["id"]);
				e = e.GetEvent(lngID);
				e.Images = db.GetEventImages(lngID);
			}
			return View(e);
		}

		[HttpPost]
		public ActionResult EventGallery(IEnumerable<HttpPostedFileBase> files) {
			Models.Event e = new Models.Event();
			e.User = new Models.User();
			e.User = e.User.GetUserSession();
			e.ID = Convert.ToInt64(RouteData.Values["id"]);
			foreach (var file in files) {
				e.AddEventImage(file);
			}
			return Json("file(s) uploaded successfully");
		}

		[HttpPost]
		public JsonResult DeleteEventImage(long UID, long ID) {
			try {
				string type = string.Empty;
				Models.Database db = new Models.Database();
				if (db.DeleteEventImage(ID)) return Json(new { Status = 1 }); //deleted
				return Json(new { Status = 0 }); //not deleted
			}
			catch (Exception ex) {
				return Json(new { Status = -1 }); //error
			}
		}

		public ActionResult DeleteEvent() {
			Models.Event e = new Models.Event();
			e.User = new Models.User();
			e.User = e.User.GetUserSession();
			if (e.User.IsAuthenticated) {
				long lngID = Convert.ToInt64(RouteData.Values["id"]);
				e = e.GetEvent(lngID);
			}
			return View(e);
		}

		[HttpPost]
		public ActionResult DeleteEvent(FormCollection col) {
			Models.User u = new Models.User();
			u = u.GetUserSession();
			if (u.IsAuthenticated) {
				long lngID = Convert.ToInt64(RouteData.Values["id"]);

				if (col["btnSubmit"] == "close") return RedirectToAction("Event", new { @id = lngID });
				if (col["btnSubmit"] == "delete") {
					Models.Database db = new Models.Database();
					db.DeleteEvent(lngID);
				}
			}
			return RedirectToAction("MyEvents"); //this should never happen
		}

