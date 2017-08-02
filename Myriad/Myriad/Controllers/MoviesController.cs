﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Myriad.Models;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;
using System.Configuration;

namespace Myriad.Controllers
{
    public class MoviesController : Controller
    {
        private static readonly string _awsAccessKey = ConfigurationManager.AppSettings["AccessId"];

        private static readonly string _awsSecretKey = ConfigurationManager.AppSettings["secretKey"];

        private static readonly string _bucketName = ConfigurationManager.AppSettings["bucketname"];
        private MyriadDbEntities db = new MyriadDbEntities();

        public static IList<SelectListItem> GetGender()
        {
            IList<SelectListItem> _result = new List<SelectListItem>();
            _result.Add(new SelectListItem { Value = "1", Text = "Male" });
            _result.Add(new SelectListItem { Value = "2", Text = "Female" });
            _result.Add(new SelectListItem { Value = "3", Text = "Others" });
            return _result;
        }
        // GET: Movies
        public ActionResult Index()
        {
            
            db.SaveChanges();
            var movies = db.Movies.Include(m => m.Producer);
            var mvModel = new MovieViewModel();

            var mvModelList = new List<MovieViewModel>();
            foreach (var item in movies)
            {
                var results = from a in db.Actors
                              select new
                              {
                                  a.ActID,
                                  a.Name,
                                  Checked = ((from ab in db.MovieActors
                                              where ((item.MovID == ab.MovID) & (a.ActID == ab.ActID))
                                              select ab).Count() > 0)

                              };
                var actorsList = new List<CheckActorsModel>();

                foreach (var item1 in results)
                {
                    actorsList.Add(new CheckActorsModel { id = item1.ActID, Name = item1.Name, isChecked = item1.Checked });
                }

                mvModelList.Add(new MovieViewModel
                {
                    MovID = item.MovID,
                    Name = item.Name,
                    Producer = item.Producer,
                    Plot = item.Plot,
                    Poster = item.Poster,
                    ProID = item.ProID,
                    ReleaseDate = item.ReleaseDate,
                    ActorsList = actorsList
                });
            }
            return View(mvModelList);
        }

        // GET: Movies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            var results = from a in db.Actors
                          select new
                          {
                              a.ActID,
                              a.Name,
                              Checked = ((from ab in db.MovieActors
                                          where ((id == ab.MovID) & (a.ActID == ab.ActID))
                                          select ab).Count() > 0)

                          };

            var moviewView = new MovieViewModel();
            moviewView.MovID = id.Value;
            moviewView.Name = movie.Name;
            moviewView.Plot = movie.Plot;
            moviewView.Poster = movie.Poster;
            moviewView.ProID = movie.ProID;
            moviewView.ReleaseDate = movie.ReleaseDate;
            moviewView.Producer = movie.Producer;

            var actorsList = new List<CheckActorsModel>();

            foreach (var item in results)
            {
                actorsList.Add(new CheckActorsModel { id = item.ActID, Name = item.Name, isChecked = item.Checked });

            }
            moviewView.ActorsList = actorsList;
            ViewBag.ProID = new SelectList(db.Producers, "ProID", "Name", movie.ProID);
            return View(moviewView);
        }



        public ActionResult CreateMovies()
        {
            List<SelectListItem> listSelectListItems = new List<SelectListItem>();

            foreach (Actor actor in db.Actors)
            {
                SelectListItem selectList = new SelectListItem()
                {
                    Text = actor.Name,
                    Value = actor.ActID.ToString(),
                    Selected = false
                };
                listSelectListItems.Add(selectList);
            }

            ExampleViewModel mvModel = new ExampleViewModel()
            {
                ActorsList = listSelectListItems
            };

            var results = from a in db.Actors
                          select new
                          {
                              a.ActID,
                              a.Name,
                              Checked = false
                          };

            var moviewView = new MovieViewModel();

            var actorsList = new List<CheckActorsModel>();

            foreach (var item in results)
            {
                actorsList.Add(new CheckActorsModel { id = item.ActID, Name = item.Name, isChecked = item.Checked });

            }
            moviewView.ActorsList = actorsList;
            ViewBag.ProID = new SelectList(db.Producers, "ProID", "Name");
            ViewBag.ActList = new MultiSelectList(actorsList, "ActList", "Name");
            return View(mvModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMovies(ExampleViewModel mvModel)
        {
            var movie = new Movie();
            movie.Name = mvModel.Name;
            movie.Plot = mvModel.Plot;
            movie.Poster = mvModel.Poster;
            movie.ReleaseDate = mvModel.ReleaseDate;
            movie.ProID = mvModel.ProID;
            if (ModelState.IsValid)
            {
                //foreach (var item in db.MovieActors)
                //{
                //    if (item.MovID == movie.MovID)
                //        db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                //}
                db.Movies.Add(movie);
                foreach (var item in mvModel.SelectedActorsIdList)
                {

                    db.MovieActors.Add(new MovieActor() { MovID = movie.MovID, ActID = item });
                }



                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProID = new SelectList(db.Producers, "ProID", "Name", movie.ProID);
            return View(movie);
        }



        // GET: Movies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }

            var results = from a in db.Actors
                          select new
                          {
                              a.ActID,
                              a.Name,
                              Checked = ((from ab in db.MovieActors
                                          where ((id == ab.MovID) & (a.ActID == ab.ActID))
                                          select ab).Count() > 0)

                          };

            var moviewView = new MovieViewModel();
            moviewView.MovID = id.Value;
            moviewView.Name = movie.Name;
            moviewView.Plot = movie.Plot;
            moviewView.Poster = movie.Poster;
            moviewView.ProID = movie.ProID;
            moviewView.ReleaseDate = movie.ReleaseDate;

            var actorsList = new List<CheckActorsModel>();

            foreach (var item in results)
            {
                actorsList.Add(new CheckActorsModel { id = item.ActID, Name = item.Name, isChecked = item.Checked });

            }
            moviewView.ActorsList = actorsList;
            ViewBag.ProID = new SelectList(db.Producers, "ProID", "Name", movie.ProID);
            return View(moviewView);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MovieViewModel mvModel)
        {

            var movie = db.Movies.Find(mvModel.MovID);
            movie.Name = mvModel.Name;
            movie.Plot = mvModel.Plot;
            movie.Poster = mvModel.Poster;
            movie.ReleaseDate = mvModel.ReleaseDate;
            movie.ProID = mvModel.ProID;
            if (ModelState.IsValid)
            {
                foreach (var item in db.MovieActors)
                {
                    if (item.MovID == movie.MovID)
                        db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                }

                foreach (var item in mvModel.ActorsList)
                {
                    if (item.isChecked)
                        db.MovieActors.Add(new MovieActor() { MovID = movie.MovID, ActID = item.id });
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProID = new SelectList(db.Producers, "ProID", "Name", movie.ProID);
            return View(movie);
        }

        // GET: Movies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Movie movie = db.Movies.Find(id);
            IEnumerable<MovieActor> movieActor = db.MovieActors.Where(m => m.MovID == id);
            foreach (var item in movieActor)
            {
                db.MovieActors.Remove(item);
            }

            db.Movies.Remove(movie);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public PartialViewResult CreateActorPartialView(int? id)
        {
            ActorsViewModels actor = new ActorsViewModels();
            Movie movie = new Movie();
            ViewBag.ProID = new SelectList(db.Producers, "ProID", "Name", movie.ProID);
            var a = new SelectList(MoviesController.GetGender(), "Value", "Text", actor.Sex);

            ViewBag.Sex = new SelectList(MoviesController.GetGender(), "Value", "Text", actor.Sex);
            return PartialView("CreateActorPartialView");
        }

        [HttpPost]
        public ActionResult CreateActorPartialView(ActorsViewModels actorModel)
        {

            if (ModelState.IsValid)
            {
                Actor actor = new Actor();
                actor.Name = actorModel.Name;
                actor.Sex = (byte)actorModel.Sex;
                actor.Bio = actorModel.Bio;
                actor.DOB = actorModel.DOB;
                actor.MovieActors = actorModel.MovieActors;
                db.Actors.Add(actor);
                db.SaveChanges();

                return Content("Actor Added Successfully");
            }

            return View(actorModel);
        }

        public PartialViewResult CreateProducerPartialView()
        {
            Producer pro = new Producer();
            ViewBag.Sex = new SelectList(MoviesController.GetGender(), "Value", "Text", pro.Sex);

            return PartialView("CreateProducerPartialView");
        }

        [HttpPost]
        public ActionResult CreateProducerPartialView(Producer producer)
        {
            if (ModelState.IsValid)
            {
                db.Producers.Add(producer);
                db.SaveChanges();
                return Content("Producer Added Successfully");
            }

            return View(producer);
        }

        //third trial

        public PartialViewResult CreateMoviesPartialView(int? id)
        {
            var movieModel = new MovieViewModel();
            movieModel.ActorsList = GetAactorsCheckList();
            Movie movie = new Movie();
            ViewBag.ProID = new SelectList(db.Producers, "ProID", "Name", movie.ProID);

            return PartialView("CreateMoviesPartialView", movieModel);
        }
        //fghjhvm
        [HttpPost]
        public ActionResult CreateMoviesPartialView(MovieViewModel mvModel, List<CheckActorsModel> caModel, HttpPostedFileBase file)
        {
            var movie = new Movie();
            movie.Name = mvModel.Name;
            movie.Plot = mvModel.Plot;

            movie.ReleaseDate = mvModel.ReleaseDate;
            movie.ProID = mvModel.ProID;
            mvModel.ActorsList = caModel;
            if (ModelState.IsValid)
            {
                //movie.Poster = file.ToString();
                var fileName = Path.GetFileName(file.FileName); //getting only file name(ex-ganesh.jpg)  
                //var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  
                //if (ext == ".jpg") //check what type of extension  
                //{
                    string keyname = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension  
                //    string myfile = name + "_" + movie.MovID + ext; //appending the name with id  
                //    // store the file inside ~/project folder(Img)  
                //    var path = Path.Combine(Server.MapPath("~/App_Data/img"), myfile);
                //    //var path = Path.Combine("https://drive.google.com/drive/folders/0B0iUAcFc1a98Ni1wMGVIdGhGSlU",myfile);
                //    movie.Poster = path;
                //    file.SaveAs(path);
                //}
                //else
                //{
                //    ViewBag.message = "Please choose only Image file";
                //}


                try
                {
                    IAmazonS3 client;
                    using (client = Amazon.AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretKey))
                    {
                        var request = new PutObjectRequest()
                        {
                            BucketName = _bucketName,
                            CannedACL = S3CannedACL.PublicRead,//PERMISSION TO FILE PUBLIC ACCESIBLE
                            Key = string.Format("UPLOADS/{0}", file.FileName),
                            InputStream = file.InputStream//SEND THE FILE STREAM
                        };

                        client.PutObject(request);
                    }
                }
                catch (Exception ex)
                {


                }
                movie.Poster = "https://"+"s3-us-west-2.amazonaws.com/myriadposterappharbour/UPLOADS/" + keyname + ".jpg";


                db.Movies.Add(movie);
                if (mvModel.ActorsList != null)
                {
                    foreach (var item in mvModel.ActorsList)
                    {
                        if (item.isChecked)
                            db.MovieActors.Add(new MovieActor() { MovID = movie.MovID, ActID = item.id });
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            CreateAll c = new CreateAll();
            c.movieModel = mvModel;
            ViewBag.ProID = new SelectList(db.Producers, "ProID", "Name", movie.ProID);
            return View(mvModel);
        }


        public ActionResult AddToCatalogue()
        {
            var model = new CreateAll();
            model.actor = new Actor();
            model.movieModel = new MovieViewModel();
            model.producer = new Producer();

            model.movieModel.ActorsList = GetAactorsCheckList();
            ViewBag.ProID = new SelectList(db.Producers, "ProID", "Name", model.movieModel.ProID);

            return View(model);
        }

        public ActionResult CheckActorsPartialView(MovieViewModel m)
        {

            var actorsList = new List<CheckActorsModel>();
            actorsList = GetAactorsCheckList();
            return View(actorsList);
        }

        public List<CheckActorsModel> GetAactorsCheckList()
        {
            var results = from a in db.Actors
                          select new
                          {
                              a.ActID,
                              a.Name,
                              Checked = false
                          };

            var actorsList = new List<CheckActorsModel>();

            foreach (var item in results)
            {
                actorsList.Add(new CheckActorsModel { id = item.ActID, Name = item.Name, isChecked = item.Checked });
            }
            return actorsList;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
