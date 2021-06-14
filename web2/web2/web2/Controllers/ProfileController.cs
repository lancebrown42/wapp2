using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web2.Controllers{
    public class ProfileController : Controller{
        // GET: Profile
        public ActionResult Index(){
            return View();
        }
        public ActionResult SignIn(){
			Models.User u = new Models.User();
            return View(u);
        }
        public ActionResult SignUp(){
			Models.User u = new Models.User();
            return View(u);
        }
    }
}