using SW.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SW.Website.Controllers
{

    public class ProductController : Controller
    {
        private ShoppingWebsiteEntities db = new ShoppingWebsiteEntities();
        //
        // GET: /Product/
        public ActionResult ProductManagement()
        {
            IEnumerable<Product> products = db.Products.ToList();
            return View(products);
        }
        public ActionResult BuyNow()
        {
           
            List<CartItem> clist = (List<CartItem>)Session["Items"];
            double totalCost = (double)Session["totalCost"];



            if(clist.Count==0)
            {
                return RedirectToAction("Checkout");
            }

            Cart cart = new Cart();
            cart.TotalCost = totalCost;
            cart.CreatedOn = DateTime.Now;
            cart.IsActive = true;

            db.Carts.Add(cart);
            db.SaveChanges();

            foreach(var item in clist)
            {
                CartItem cI = new CartItem();
                cI.PID = item.PID;
                cI.Quantity = item.Quantity;
                cI.TotalCost = item.TotalCost;
                cI.CID = cart.CartID;
                cI.IsActive = true;

                db.CartItems.Add(cI);
                db.SaveChanges();
            }

            Payment p = new Payment();
            p.Ammount = totalCost;
            p.PaymentType = "On Delivery";

            User u = (User)Session["User"];
            p.CustomerID = u.UserID;
            p.CartID = cart.CartID;
            p.Date = DateTime.Now;

            db.Payments.Add(p);
            db.SaveChanges();

            



            return View(p);
        }
        public ActionResult Checkout()
        {
            List<CartItem> cList = (List<CartItem>)Session["Items"];
            return View(cList);
        }
        public ActionResult RemoveItem(int id)
        {
           

            List<CartItem> cList = (List<CartItem>)Session["Items"];

            double totalCost = (double)Session["totalCost"];
            totalCost -= cList[id].TotalCost;


            cList.RemoveAt(id);

            Session["totalCost"] = totalCost;
            Session["Items"] = cList;
            return RedirectToAction("Checkout");
        }
        public ActionResult EmptyCart()
        {
            double totalCost = 0;
            Session["TotalCost"] = totalCost;
            Session["Products"] = "Mobile";
            return RedirectToAction("UserProfileArea", "Home");
        }
        public ActionResult Create()
        {
            Product dto = new  SW.DAL.Product();
            ViewBag.Categories = db.Categories.ToList();
            
            return View(dto);
        }

        [HttpPost]
        public ActionResult Save(Product dto)
        {
            var uniqueName = "";

            if (Request.Files["Image"] != null)
            {
                var file = Request.Files["Image"];
                if (file.FileName != "")
                {
                    var ext = System.IO.Path.GetExtension(file.FileName);

                    //generate a unique name using Guid
                    uniqueName = Guid.NewGuid().ToString() + ext;

                    //get physical path of our folder where we want to save image
                    var rootPath = Server.MapPath("~/UploadedFiles");

                    var fileSavePath = System.IO.Path.Combine(rootPath, uniqueName);

                    // save uploaded file to "UploadedFiles" folder

                    file.SaveAs(fileSavePath);

                    dto.PictureName = uniqueName;
                }
            }

            dto.IsActive = true;
            if (dto.ProductID > 0)
            {
                var prod = db.Products.Find(dto.ProductID);
                if(prod!=null)
                {
                    prod.PName = dto.PName;
                    prod.Price = dto.Price;
                    prod.CategoryID = dto.CategoryID;
                    prod.PictureName = dto.PictureName;
                    prod.ModifiedBy = 1;
                    prod.ModifiedOn = DateTime.Now;
                }
            }
            else
            {
                dto.CreatedBy = 1;
                dto.CreatedOn = DateTime.Now;
                db.Products.Add(dto);    
            }
            
            
            db.SaveChanges();

            TempData["Msg"] = "Record is saved!";
            //save the product
            return RedirectToAction("ProductManagement");
        }


        public ActionResult Edit(int id)
        {
            
            Product p = db.Products.Find(id);
            ViewBag.Categories = db.Categories.ToList();
            
            return View("Create", p);    
        }
     

        public ActionResult Delete(int id)
        {
            var p = db.Products.Find(id);
            p.IsActive = false;
            db.SaveChanges();
            return RedirectToAction("ProductManagement");
        }
        public ActionResult Details(int id)
        {

            var p = db.Products.Find(id);
            ViewBag.Category = db.Categories.Find(p.CategoryID);
            return View(p);
        }
        public ActionResult ShowDetails(int id)
        {

            var p = db.Products.Find(id);
            ViewBag.Category = db.Categories.Find(p.CategoryID);
            return View(p);
        }
        public ActionResult ShowMobiles()
        {
            Session["Products"] = "Mobile";
            return RedirectToAction("UserProfileArea", "Home");

        }
        public ActionResult ShowAccessories()
        {
            Session["Products"] = "Accessories";
            return RedirectToAction("UserProfileArea", "Home");

        }
        public ActionResult ShowHome()
        {
            Session["Products"] = "Home";
            return RedirectToAction("UserProfileArea", "Home");

        }
        public ActionResult AddToCart(int prodID,int prodQuantity)
        {
            if(prodQuantity<=0)
            {
                TempData["ErrorMsg"] = "Invalid Quantity";
                return Redirect("~/Product/ShowDetails/"+prodID);
            }
            else
            {

                List<CartItem> cList = (List<CartItem>)Session["Items"];
                double totalCost = (double)Session["TotalCost"];

                var prod = db.Products.Find(prodID);
                if(prod!=null)
                {
             
                    int index=IsExist(cList,prodID);
                    if(index != -1)
                    {
                        cList[index].Quantity += prodQuantity;
                        cList[index].TotalCost += prodQuantity * prod.Price;
                        totalCost += prod.Price * prodQuantity;
                    }
                    else
                    {
                        CartItem cI = new CartItem();
                        cI.PID = prodID;
                        cI.Quantity = prodQuantity;
                        cI.TotalCost = prod.Price * prodQuantity;
                        cI.IsActive = true;
                        cI.Product = prod;
                        cList.Add(cI);
                        totalCost += prod.Price * prodQuantity;

                    }
                    
                    Session["Items"] = cList;
                    Session["TotalCost"] = totalCost;
                    
                }

                Session["Products"] = "Mobile";
                return RedirectToAction("UserProfileArea", "Home");
            }
            
        }

        int IsExist(List<CartItem> cList,int prodID)
        {
            for(int i=0;i<cList.Count;i++)
            {
                if(cList[i].PID==prodID)
                {
                    return i;
                }
            }
            return -1;
        }
	}
}