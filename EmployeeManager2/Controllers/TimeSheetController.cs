using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManager2.Data;
using EmployeeManager2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManager2.Controllers
{
    [Authorize]
    public class TimeSheetController : Controller
    {
        private readonly AppDbContext _context;

        public TimeSheetController(AppDbContext context)
        {
            _context = context;
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
            var lastnameList = (from ts in _context.Timesheet
                                select new SelectListItem()
                                {
                                    Text = ts.LastName,
                                    Value = ts.LastName,
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
            var sum1 = _context.Timesheet.Sum(p => p.Hours);

            ViewBag.total = sum1.ToString();
            var ts = _context.Timesheet.ToList();
            return View(ts);
        }
        [HttpGet]
        //[Authorize(Roles = UtilityClass.AdminUserRole)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        //[Authorize(Roles = UtilityClass.AdminUserRole)]
        public IActionResult Create(TimeSheet ts)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _context.Timesheet.Add(ts);
            _context.SaveChanges();

            return RedirectToAction("List");
        }

        [HttpGet]
        [Authorize(Roles = UtilityClass.AdminUserRole)]
        public IActionResult Edit(int id)
        {
            var ts = _context.Timesheet.First(s => s.ID == id);
            return View(ts);
        }

        [HttpPost]
        [Authorize(Roles = UtilityClass.AdminUserRole)]
        public IActionResult Edit(TimeSheet ts)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _context.Timesheet.Attach(ts);
            _context.Entry(ts).State = EntityState.Modified;
            _context.SaveChanges();

            return RedirectToAction("List");
        }

        [HttpGet]
        [Authorize(Roles = UtilityClass.AdminUserRole)]
        public IActionResult Delete(int id)
        {
            var ts = _context.Timesheet.First(s => s.ID == id);
            return View(ts);
        }

        [HttpPost]
        [Authorize(Roles = UtilityClass.AdminUserRole)]
        public IActionResult Delete(TimeSheet ts)
        {
            

            _context.Timesheet.Remove(ts);
            _context.SaveChanges();

            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var ts = _context.Timesheet.First(s => s.ID == id);
            return View(ts);
        }

        public ActionResult Search()
        {
            string fromdate = HttpContext.Request.Query["from"].ToString();
            string todate = HttpContext.Request.Query["to"].ToString();
            string bylastname = HttpContext.Request.Query["bylastname"].ToString();
            string byfirstname = HttpContext.Request.Query["byfirstname"].ToString();

            IQueryable<string> TimesheetQuery = from m in _context.Timesheet
                                                orderby m.LastName
                                                select m.LastName;

            var timesheet = from m in _context.Timesheet
                            select m;
            if (!string.IsNullOrWhiteSpace(byfirstname) && !string.IsNullOrWhiteSpace(fromdate) && !string.IsNullOrWhiteSpace(todate))
            {
                timesheet = timesheet.Where(s => s.Date >= Convert.ToDateTime(fromdate) && s.Date <= Convert.ToDateTime(todate) && s.FirstName.Contains(byfirstname));

                var sum5 = timesheet.Where(s => s.Date >= Convert.ToDateTime(fromdate) && s.Date <= Convert.ToDateTime(todate) && s.FirstName.Contains(byfirstname)).Sum(a => a.Hours);
                filldropdown();
                return View("List", timesheet);
            }

            if (!string.IsNullOrWhiteSpace(byfirstname))
            {
                timesheet = timesheet.Where(s => s.FirstName.Contains(byfirstname));

                var sum3 = timesheet.Where(s => s.FirstName.Contains(byfirstname)).Sum(a => a.Hours);

                ViewBag.total = sum3.ToString();
                filldropdown();
                return View("List", timesheet);
            }

            if (bylastname != "All Names")
            {
                timesheet = timesheet.Where(s => s.LastName.Contains(bylastname));

                var sum3 = timesheet.Where(s => s.LastName.Contains(bylastname)).Sum(a => a.Hours);

                ViewBag.total = sum3.ToString();
                //filldropdown();
                //return View("List", timesheet);
            }

            if (!string.IsNullOrWhiteSpace(todate))
            {
                if (string.IsNullOrWhiteSpace(todate))
                {
                    todate = DateTime.Today.ToString("yyyy-MM-dd");
                }
                if (!string.IsNullOrWhiteSpace(todate))
                {
                    timesheet = timesheet.Where(s => s.Date >= Convert.ToDateTime(fromdate) && s.Date <= Convert.ToDateTime(todate));

                    var sum2 = timesheet.Where(s => s.Date >= Convert.ToDateTime(fromdate) && s.Date <= Convert.ToDateTime(todate)).Sum(p => p.Hours);

                    ViewBag.total = sum2.ToString();
                    //filldropdown();
                    //return View("List", timesheet);

                }
            }
            filldropdown();
            return View("List", timesheet);
            //return RedirectToAction("List");
        }
    }
}