using SW.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SW.Website.Controllers
{
    public class UserController : Controller
    {
        private ShoppingWebsiteEntities db = new ShoppingWebsiteEntities();
        //
        // GET: /User/

        public ActionResult Register(User user)
        {
            try
            {
                user.CreatedOn = DateTime.Now;
                user.IsActive = true;
                db.Users.Add(user);
                db.SaveChanges();
                TempData["RegisterMsg"] = "You are register successfully!";


                Session["UserName"] = user.FName;
                Session["UserId"] = user.UserID;
                Session["IsAdmin"] = user.IsAdmin;

                Session["User"] = user;


                return RedirectToAction("UserProfileArea", "Home");


            }
            catch(Exception e)
            {
                TempData["RegisterMsg"] = "Sorry! something went wrong.";
                return RedirectToAction("Index", "Home");

            }
        }

        public ActionResult Login(User user)
        {
            var userList = db.Users.ToList();

            foreach(var u in userList)
            {
                if(u.Login.Equals(user.Login)&&u.Password.Equals(user.Password))
                {

                    TempData["RegisterMsg"] = "You are Login successfully!";

                    Session["UserName"] = u.FName;
                    Session["UserId"] = u.UserID;
                    Session["IsAdmin"] = u.IsAdmin;

                    Session["User"] = u;
                    
                    if(Session["IsAdmin"].Equals(true))
                    {
                        return RedirectToAction("AdminProfileArea", "Home");
                    }
                    else
                    {
                        return RedirectToAction("UserProfileArea", "Home");
                    }

                }
            }

            TempData["RegisterMsg"] = "You are not Login successfully!";
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Logout()
        {
            Session["UserName"] = null;
            Session["UserId"] = null;
            Session["IsAdmin"] = null;

            Session["User"] = null;

            List<CartItem> cList = new List<CartItem>();
            double totalCost = 0;

            Session["Items"] = cList;
            Session["TotalCost"] = totalCost;

            return RedirectToAction("Index", "Home");
        }
	}
}