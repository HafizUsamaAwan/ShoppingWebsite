using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SW.DAL;
namespace SW.Website.Controllers
{
    public class HomeController : Controller
    {
        private ShoppingWebsiteEntities db = new ShoppingWebsiteEntities();

        public ActionResult GoToShop()
        {
            if(Session["User"]==null)
            {
                return RedirectToAction("Index");
           
            }
            User u = (User)Session["User"];
            if(u.IsAdmin)
            {
                return RedirectToAction("AdminProfileArea");
            }
            else
            {
                return RedirectToAction("UserProfileArea");

            }
        }
        public ActionResult Index()
        {
            if(Session["Items"]==null)
            {
                List<CartItem> cList = new List<CartItem>();
                double totalCost = 0;

                Session["Items"] = cList;
                Session["TotalCost"] = totalCost;
            }
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Mail()
        {
            return View();
        }
        public ActionResult AdminProfileArea()
        {
            if(Session["User"]==null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult UserProfileArea()
        {

            if (Session["User"] == null)
            {
                return RedirectToAction("Index");
            }
            if (Session["Products"] == null)
            {
                Session["Products"] = "Mobile";
            }
            List<Category> list=db.Categories.ToList();
            if(Session["Products"].Equals("Mobile"))
            {
                Session["CategoryID"] = 1;
            }
            else if (Session["Products"].Equals("Accessories"))
            {
                Session["CategoryID"] = 2;
            }
            else if (Session["Products"].Equals("Home"))
            {
                Session["CategoryID"] = 3;
            }
            var products = db.Products.ToList();
            return View(products);
        }

     
    }
}