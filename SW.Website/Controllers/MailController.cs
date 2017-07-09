using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SW.DAL;
namespace SW.Website.Controllers
{
    public class MailController : Controller
    {
        private ShoppingWebsiteEntities db = new ShoppingWebsiteEntities();

        //
        // GET: /Mail/
        public ActionResult GET()
        {
            var mailList = db.Emails.ToList();
            return View(mailList);
        }

        public ActionResult POST(Email model)
        {
            try 
            {
                model.Date = DateTime.Now;
                db.Emails.Add(model);
                db.SaveChanges();
                TempData["Msg"] = "Message Sent Successfully.";

            }
            catch(Exception e)
            {
                
                TempData["Msg"] = "Sorry! Something went Wrong.";
            }
            

            return RedirectToAction( "Mail","Home");
        }
	}
}