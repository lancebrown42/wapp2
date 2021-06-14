using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web2Blank.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            Models.User u = new Models.User();
            u.FirstName = "Chris";
            u.LastName = "Johnson";
            u.UserID = "chris.johnson";
            u.Password = "Efje$";
            return View(u);
        }
    }
}