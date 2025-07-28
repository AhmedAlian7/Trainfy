using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvcFirstApp.Models.Data;
using mvcFirstApp.Models.Entities;
using mvcFirstApp.Repositories;
using mvcFirstApp.Services;
using mvcFirstApp.ViewModels;

namespace mvcFirstApp.Controllers
{
    [Authorize]
    public class InstructorController : Controller
    {
        private readonly ICourseRepository _Courses;
        private readonly IRepository<Department> _Departments;
        private readonly IRepository<Instructor> _Instructors;
        public readonly FileUploadService _fileUploadService;

        public InstructorController
            (FileUploadService fileUploadService,
            ICourseRepository Courserepository
            ,IRepository<Department> deptRepo
            ,IRepository<Instructor> instructors)
        {
            _fileUploadService = fileUploadService;
            _Courses = Courserepository;
            _Departments = deptRepo;
            _Instructors = instructors;
        }
        [HttpGet]
        public IActionResult Index(int page = 1)
        {
            const int pageSize = 6; // Number of instructors per page



            var paginatedInstructors = _Instructors.GetAll(page, pageSize,
                includeProperties: "Department");

            ViewBag.Departments = _Departments.GetAll().ToList();
            return View("Index", paginatedInstructors);
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var instructor = _Instructors.GetById(id, "Department,Courses");

            return View("Details", instructor);
        }
        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Departments = _Departments.GetAll().ToList();
            return View("Add");
        }
        [HttpPost]
        public IActionResult SaveAdd(Instructor instructorFromReq)
        {
            if (string.IsNullOrWhiteSpace(instructorFromReq.Name) ||
                    string.IsNullOrWhiteSpace(instructorFromReq.Email) ||
                    string.IsNullOrWhiteSpace(instructorFromReq.Address) ||
                    instructorFromReq.Salary <= 0 ||
                    instructorFromReq.DepartmentId <= 0)
            {
                ViewBag.Departments = _Departments.GetAll().ToList();
                return View("Add", instructorFromReq);
            }

            if (instructorFromReq.ImageFile != null && instructorFromReq.ImageFile.Length > 0)
            {
                try
                {
                    var imageUrl = _fileUploadService.UploadFile(instructorFromReq.ImageFile, "uploads/instructors");
                    instructorFromReq.ImageUrl = imageUrl;
                }
                catch (Exception ex)
                {
                    ViewBag.Departments = _Departments.GetAll().ToList();
                    return View("Add", instructorFromReq);
                }
            }

            _Instructors.Add(instructorFromReq);
            _Instructors.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid instructor ID.");
            }
            var instructor = _Instructors.GetById(id, "Department,Courses");

            if (instructor == null)
            {
                return NotFound();
            }

            ViewBag.Departments = _Departments.GetAll().ToList();    
            return View("Edit", instructor);
        }
        [HttpPost]
        public IActionResult SaveEdit(Instructor instructorFromReq)
        {
            if (instructorFromReq.Name == null || instructorFromReq.Email == null || instructorFromReq.Salary == 0)
            {
                ViewBag.Departments = _Departments.GetAll().ToList();
                return View("Edit", instructorFromReq);
            }

            var existingInstructor = _Instructors.GetById(instructorFromReq.Id);
            if (existingInstructor == null)
            {
                return NotFound();
            }

            existingInstructor.Name = instructorFromReq.Name;
            existingInstructor.Email = instructorFromReq.Email;
            existingInstructor.Address = instructorFromReq.Address;
            existingInstructor.Salary = instructorFromReq.Salary;
            existingInstructor.DepartmentId = instructorFromReq.DepartmentId;

            if (instructorFromReq.ImageFile != null && instructorFromReq.ImageFile.Length > 0)
            {

                try
                {
                    var newImageUrl = _fileUploadService.UploadFile(instructorFromReq.ImageFile, "uploads/instructors");

                    // Delete old image if any
                    if (!string.IsNullOrEmpty(existingInstructor.ImageUrl))
                    {
                        _fileUploadService.DeleteFile(existingInstructor.ImageUrl);
                    }

                    existingInstructor.ImageUrl = newImageUrl;
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("ImageFile", ex.Message);
                    ViewBag.Departments = _Departments.GetAll().ToList();
                    return View("Edit", instructorFromReq);
                }
            }

            _Departments.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            var instructor = _Instructors.GetById(id);
            if (instructor == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(instructor.ImageUrl))
            {
                _fileUploadService.DeleteFile(instructor.ImageUrl);
            }
            _Instructors.Delete(instructor.Id); 
            _Instructors.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
