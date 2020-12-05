using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManager2.Data;
using EmployeeManager2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManager2.Controllers
{
    [Authorize]
    public class MileageController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHostingEnvironment hostingEnvironment;

        public MileageController(AppDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            this.hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            return View();
            
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public void filldropdown()
        {
            var lastnameList = (from m in _context.Mileage
                                select new SelectListItem()
                                {
                                    Text = m.LastName,
                                    Value = m.LastName,
                                }).ToList();

            lastnameList.Insert(0, new SelectListItem()
            {
                Text = "All Names",
                Value = "All Names"
            });

            ViewBag.Listofnames = lastnameList;
        }

        public IActionResult List()
        {
            filldropdown();
            var sum1 = _context.Mileage.Sum(p => p.Miles);

            ViewBag.total = sum1.ToString();
            var m = _context.Mileage.ToList();
            return View(m);
        }
        [HttpGet]
        //[Authorize(Roles = UtilityClass.AdminUserRole)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        //[Authorize(Roles = UtilityClass.AdminUserRole)]
        public IActionResult Create(Mileage m)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _context.Mileage.Add(m);
            _context.SaveChanges();
            return RedirectToAction("List");
        }

        [HttpGet]
        [Authorize(Roles = UtilityClass.AdminUserRole)]
        public IActionResult Edit(int id)
        {
            var m = _context.Mileage.First(s => s.ID == id);
            return View(m);
        }

        [HttpPost]
        [Authorize(Roles = UtilityClass.AdminUserRole)]
        public IActionResult Edit(Mileage m)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _context.Mileage.Attach(m);
            _context.Entry(m).State = EntityState.Modified;
            _context.SaveChanges();

            return RedirectToAction("List");
        }

        [HttpGet]
        [Authorize(Roles = UtilityClass.AdminUserRole)]
        public IActionResult Delete(int id)
        {
            var m = _context.Mileage.First(s => s.ID == id);
            return View(m);
        }

        [HttpPost]
        [Authorize(Roles = UtilityClass.AdminUserRole)]
        public IActionResult Delete(Mileage m)
        {
            

            _context.Mileage.Remove(m);
            _context.SaveChanges();
         

            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var m = _context.Mileage.First(s => s.ID == id);
            return View(m);
        }

        public ActionResult Search()
        {
            string fromdate = HttpContext.Request.Query["from"].ToString();
            string todate = HttpContext.Request.Query["to"].ToString();
            string bylastname = HttpContext.Request.Query["bylastname"].ToString();
            string byfirstname = HttpContext.Request.Query["byfirstname"].ToString();

            IQueryable<string> MileageQuery = from m in _context.Mileage  
                                                orderby m.LastName
                                                select m.LastName;

            var mileage = from m in _context.Mileage
                            select m;

            if (!string.IsNullOrWhiteSpace(byfirstname) && !string.IsNullOrWhiteSpace(fromdate) && !string.IsNullOrWhiteSpace(todate))
            {
                mileage = mileage.Where(s => s.TravelDate >= Convert.ToDateTime(fromdate) && s.TravelDate <= Convert.ToDateTime(todate) && s.FirstName.Contains(byfirstname));

                var sum5 = mileage.Where(s => s.TravelDate >= Convert.ToDateTime(fromdate) && s.TravelDate <= Convert.ToDateTime(todate) && s.FirstName.Contains(byfirstname)).Sum(a => a.Miles);
                ViewBag.total = sum5.ToString();
                filldropdown();
                return View("List", mileage);
            }
            if (!string.IsNullOrWhiteSpace(byfirstname))
            {
                mileage = mileage.Where(s => s.FirstName.Contains(byfirstname));

                var sum3 = mileage.Where(s => s.FirstName.Contains(byfirstname)).Sum(a => a.Miles);

                ViewBag.total = sum3.ToString();
                filldropdown();
                return View("List", mileage);
            }

            if (bylastname != "All Names")
            {
                mileage = mileage.Where(s => s.LastName.Contains(bylastname));

                var sum3 = mileage.Where(s => s.LastName.Contains(bylastname)).Sum(a => a.Miles);

                ViewBag.total = sum3.ToString();
                //filldropdown();
                //return View("List", mileage);
            }

            if (!string.IsNullOrWhiteSpace(todate))
            {
                if (string.IsNullOrWhiteSpace(todate))
                {
                    todate = DateTime.Today.ToString("yyyy-MM-dd");
                }
                if (!string.IsNullOrWhiteSpace(todate))
                {
                    mileage = mileage.Where(s => s.TravelDate >= Convert.ToDateTime(fromdate) && s.TravelDate <= Convert.ToDateTime(todate));

                    var sum2 = mileage.Where(s => s.TravelDate >= Convert.ToDateTime(fromdate) && s.TravelDate <= Convert.ToDateTime(todate)).Sum(p => p.Miles);

                    ViewBag.total = sum2.ToString();
                    //filldropdown();
                    //return View("List", mileage);

                }
            }
            filldropdown();
            return View("List", mileage);
            //return RedirectToAction("List");
        }
    }
}