using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EmployeeManager2.Models;
using EmployeeManager2.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using EmployeeManager2.Data;

namespace EmployeeManager2.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHostingEnvironment hostingEnvironment;

        public HomeController(IEmployeeRepository employeeRepository,
                              IHostingEnvironment hostingEnvironment)
        {
            _employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
        }
        [HttpGet]
        public ViewResult Index()
        {
            var model = _employeeRepository.GetAllEmployee();

            var total = _employeeRepository.GetAllEmployee().Sum(s=>s.ReceiptAmount);

            ViewBag.total = total.ToString();
            return View(model);
        }
       
       


        public ViewResult Details(int id)
        {
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = _employeeRepository.GetEmployee(id),
                PageTitle = "Employee Details"
            };
            return View(homeDetailsViewModel);
        }

        [HttpGet]
        //[Authorize(Roles = UtilityClass.AdminUserRole)]
        public ViewResult Create()
        {
            return View();
        }
        
        [HttpGet]
        //[Authorize(Roles = UtilityClass.AdminUserRole)]
        public ViewResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);
            EmployeeCreateViewModel employeeEditViewModel = new EmployeeCreateViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath,
                ReceiptDate = employee.ReceiptDate,
                ReimburseDate = employee.ReimburseDate,
                ReceiptAmount = employee.ReceiptAmount
            };
            return View(employeeEditViewModel);
        }

        [HttpGet]
        [Authorize(Roles = UtilityClass.AdminUserRole)]
        public ActionResult delete(int id)
        {
            if (id > 0 || !string.IsNullOrWhiteSpace(id.ToString()))
            {
                _employeeRepository.Delete(id);
                TempData["Message"] = "Record Deleted Succesfully";
                TempData.Keep();
                return RedirectToAction("Index");
            }
            return View("Index");
        }

        [HttpGet]
        public ActionResult Search()
        {
            string txt = HttpContext.Request.Query["txt"].ToString();
            if (!string.IsNullOrWhiteSpace(txt))
            {
                var model = _employeeRepository.SearchEmployee(txt);
                return View("Index",model);

            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult SearchReceipt()
        {
            string from = HttpContext.Request.Query["from"].ToString();
            string to = HttpContext.Request.Query["to"].ToString();
            string name = HttpContext.Request.Query["name"].ToString();

            ViewBag.name = name;
            ViewBag.fromdate = from;
            ViewBag.todate = to;

            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(to))
            {
                var model = _employeeRepository.SearchwithdateAndName(name, from, to);
                var total1 = _employeeRepository.SearchwithdateAndName(name, from, to).Sum(s => s.ReceiptAmount);

                ViewBag.total = total1.ToString();
                return View("Index", model);
            }


            if (!string.IsNullOrWhiteSpace(name))
            {
                string dateflag = HttpContext.Session.GetString("datefilterflag");
                if (dateflag == "true")
                {
                    string viewbagfromdate = HttpContext.Session.GetString("fromdate");
                    string viewbagtodate = HttpContext.Session.GetString("todate");
                    var model = _employeeRepository.SearchwithdateAndName(name, viewbagfromdate, viewbagtodate);
                    var total1 = _employeeRepository.SearchwithdateAndName(name, viewbagfromdate, viewbagtodate).Sum(s => s.ReceiptAmount);

                    ViewBag.total = total1.ToString();
                    HttpContext.Session.SetString("datefilterflag", "false");
                    return View("Index", model);
                }
                else
                {

                    var model = _employeeRepository.SearchEmployee(name);
                    var total1 = _employeeRepository.SearchEmployee(name).Sum(s => s.ReceiptAmount);

                    ViewBag.total = total1.ToString();
                    return View("Index", model);
                }
            }
            if (string.IsNullOrWhiteSpace(to))
            {
                to = DateTime.Today.ToString("yyyy-MM-dd");
            }
            if (!string.IsNullOrWhiteSpace(from))
            {
                var model = _employeeRepository.SearchReceipt(from,to);
                var total1 = _employeeRepository.SearchReceipt(from, to).Sum(s => s.ReceiptAmount);

                HttpContext.Session.SetString("fromdate", from);
                HttpContext.Session.SetString("todate", to);
                HttpContext.Session.SetString("datefilterflag", "true");
                
                ViewBag.total = total1.ToString();
                return View("Index", model);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
           
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (model.Photos != null && model.Photos.Count > 0)
                {
                    foreach (IFormFile photo in model.Photos)
                    {
                        string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "Images");
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        photo.CopyTo(new FileStream(filePath, FileMode.Create));
                    }
                }
               
                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName,
                    ReceiptDate = model.ReceiptDate,
                    ReimburseDate = model.ReimburseDate,
                    ReceiptAmount = model.ReceiptAmount
                };

                _employeeRepository.Add(newEmployee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }
            return View();
        }
        [HttpPost]
        [Authorize(Roles = UtilityClass.AdminUserRole)]
        public IActionResult Edit(EmployeeCreateViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (model.Photos != null && model.Photos.Count > 0)
                {
                    foreach (IFormFile photo in model.Photos)
                    {
                        string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "Images");
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        photo.CopyTo(new FileStream(filePath, FileMode.Create));
                    }
                }
                else
                {
                    uniqueFileName = model.ExistingPhotoPath;
                }
                Employee newEmployee = new Employee
                {
                    Id = model.Id,
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName,
                     ReceiptDate = model.ReceiptDate,
                    ReimburseDate = model.ReimburseDate,
                    ReceiptAmount = model.ReceiptAmount

                };

                _employeeRepository.Update(newEmployee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }
            return View();
        }
        


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
