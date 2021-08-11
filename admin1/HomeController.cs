

		[HttpPost]
		public JsonResult SaveReport(long UID, long IDToReport, int ProblemID) {
			try {
				Models.Database db = new Models.Database();
				System.Threading.Thread.Sleep(3000);
				bool b = false;
				b = db.InsertReport(UID, IDToReport, ProblemID);
				return Json(new { Status = b });
			}
			catch (Exception ex) {
				return Json(new { Status = -1 }); //error
			}
		}
