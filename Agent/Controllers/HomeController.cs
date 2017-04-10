using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Agent.Controllers
{
    public class HomeController : Controller
    {
        Agent.russelEntities db = new Agent.russelEntities();

        public ActionResult Index()
        {
            return View(db.agents.ToList());
        }

        public ActionResult Details(String email)
        {
            if (email == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            agent agent = db.agents.Where(a => a.email == email).FirstOrDefault();

            if (agent == null)
            {
                return HttpNotFound();
            }

            return View(agent);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Agent.agent agent, HttpPostedFileBase photo, HttpPostedFileBase thumbnail)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (photo == null)
                    {
                        ModelState.AddModelError("photo", "Please select a photo");
                        return View(agent);
                    }

                    if (thumbnail == null)
                    {
                        ModelState.AddModelError("thumbnail", "Please select a thumbnail");
                        return View(agent);
                    }

                    string photoName = System.IO.Path.GetFileName(photo.FileName);
                    string photoPath = System.IO.Path.Combine(
                                           Server.MapPath("~/images/agent"), photoName);

                    photo.SaveAs(photoPath);

                    string thumbnailName = System.IO.Path.GetFileName(thumbnail.FileName);
                    string thumbnailPath = System.IO.Path.Combine(
                                           Server.MapPath("~/images/agent"), thumbnailName);

                    thumbnail.SaveAs(thumbnailPath);

                    agent.photo = "~/images/agent/" + photo.FileName;
                    agent.thumbnail = "~/images/agent/" + thumbnail.FileName;

                    db.agents.Add(agent);

                    db.SaveChanges();

                }
                catch (Exception e)
                {
                    ModelState.AddModelError("general", "Unable to save agent");
                    return View(agent);
                }

                return RedirectToAction("Index");
            }

            return View(agent);
        }

        public ActionResult Edit(String email)
        {
            if(email == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            agent agent = db.agents.Where(a => a.email == email).FirstOrDefault();

            if(agent == null)
            {
                return HttpNotFound();
            }

            return View(agent);
        }

        [HttpPost]
        public ActionResult Edit(agent agent)
        {
            if (ModelState.IsValid)
            {
                Agent.agent old = db.agents.Where(a => a.email == agent.email).FirstOrDefault();

                old.firstname = agent.firstname;
                old.lastname = agent.lastname;
                old.phone = agent.phone;

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(agent);
        }

        public ActionResult Delete(String email)
        {
            if (email == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            agent agent = db.agents.Where(a => a.email == email).FirstOrDefault();

            if (agent == null)
            {
                return HttpNotFound();
            }

            db.agents.Remove(agent);

            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}