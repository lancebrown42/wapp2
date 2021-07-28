using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web2.Controllers
{
    public class AboutUsController : Controller
    {
        // GET: AboutUs
        public ActionResult Index()
        {
			Models.Database db = new Models.Database();
			Models.Owner owner = new Models.Owner();
			owner = db.Owner();
            return View(owner);
        }
		[HttpPost]
		public ActionResult Index(FormCollection col)
		{
			//close
			if(col["btnSubmit"] == "close") {
				return RedirectToAction("../Home/Index");

			}
			else {
				return RedirectToAction("More");
			}
		}
		public ActionResult More()
		{
			Models.Database db = new Models.Database();
			Models.Owner owner = new Models.Owner();
			owner = db.Owner();
			return View(owner);
		}
		[HttpPost]
		public ActionResult More(FormCollection col)
		{
			
			return RedirectToAction("../AboutUs");
		}
    }
}