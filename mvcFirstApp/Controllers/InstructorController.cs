using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvcFirstApp.Models.Data;
using mvcFirstApp.Models.Entities;
using mvcFirstApp.Services;
using mvcFirstApp.ViewModels;

namespace mvcFirstApp.Controllers
{
    public class InstructorController : Controller
    {
        private readonly AppDbContext _context;
        public readonly FileUploadService _fileUploadService;

        public InstructorController(AppDbContext context, FileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }
        [HttpGet]
        public IActionResult Index(int page = 1)
        {
            const int pageSize = 6; // Number of instructors per page



            var instructors = _context.Instructors.Include(i => i.Department).AsQueryable();

            var paginatedInstructors = PaginatedList<Instructor>.CreateAsync(instructors, page, pageSize);

            ViewBag.Departments = _context.Departments.ToList();
            return View("Index", paginatedInstructors);
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var instructor = _context.Instructors
                .Include(i => i.Department)
                .Include(i => i.Courses)
                .FirstOrDefault(i => i.Id == id);

            return View("Details", instructor);
        }
        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Departments = _context.Departments.ToList();
            ViewBag.Courses = _context.Courses.ToList();
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
                    ViewBag.Departments = _context.Departments.ToList();
                    return View("Add", instructorFromReq);
                }
            }

            _context.Instructors.Add(instructorFromReq);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var instructor = _context.Instructors
                .Include(i => i.Department)
                .Include(i => i.Courses)
                .FirstOrDefault(i => i.Id == id);
            if (instructor == null)
            {
                return NotFound();
            }

            ViewBag.Departments = _context.Departments.ToList();    
            return View("Edit", instructor);
        }
        [HttpPost]
        public IActionResult SaveEdit(Instructor instructorFromReq)
        {
            if (instructorFromReq.Name == null || instructorFromReq.Email == null || instructorFromReq.Salary == 0)
            {
                ViewBag.Departments = _context.Departments.ToList();
                return View("Edit", instructorFromReq);
            }

            var existingInstructor = _context.Instructors.Find(instructorFromReq.Id);
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
                    ViewBag.Departments = _context.Departments.ToList();
                    return View("Edit", instructorFromReq);
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            var instructor = _context.Instructors.Find(id);
            if (instructor == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(instructor.ImageUrl))
            {
                _fileUploadService.DeleteFile(instructor.ImageUrl);
            }
            _context.Instructors.Remove(instructor); 
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
