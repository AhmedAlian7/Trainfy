using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvcFirstApp.Models.Data;
using mvcFirstApp.Models.Entities;
using mvcFirstApp.Services;
using mvcFirstApp.ViewModels;

namespace mvcFirstApp.Controllers
{
    public class CourseController : Controller
    {
        private readonly AppDbContext _context;
        public CourseController() { _context = new AppDbContext(); }

        [HttpGet]
        public IActionResult Index(int page = 1)
        {
            const int pageSize = 6; // Number of courses per page


            var courses = _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Instructor)
                .AsQueryable();

            var paginatedCourses = PaginatedList<Course>.CreateAsync(courses, page, pageSize);

            ViewBag.Departments = _context.Departments.ToList();
            return View("Index", paginatedCourses);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var course = _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Instructor)
                .Include(c => c.CourseResults)
                .FirstOrDefault(c => c.Id == id);
            if (course == null)
            {
                return NotFound();
            }
            return View("Details", course);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Departments = _context.Departments.ToList();
            ViewBag.Instructors = _context.Instructors.ToList();
            return View("Add");
        }

        [HttpPost]
        public IActionResult SaveAdd(Course courseFromReq)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(courseFromReq.Title) || courseFromReq.Credits <= 0 || courseFromReq.Degree < 0 || courseFromReq.MinDegree < 0)
            {
                ViewBag.Departments = _context.Departments.ToList();
                ViewBag.Instructors = _context.Instructors.ToList();
                return View("Add", courseFromReq);
            }
            _context.Courses.Add(courseFromReq);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var course = _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Instructor)
                .FirstOrDefault(c => c.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            ViewBag.Departments = _context.Departments.ToList();
            ViewBag.Instructors = _context.Instructors.ToList();
            return View("Edit", course);
        }
        [HttpPost]
        public IActionResult SaveEdit(int id, Course courseFromReq)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(courseFromReq.Title) || courseFromReq.Credits <= 0 || courseFromReq.Degree < 0 || courseFromReq.MinDegree < 0)
            {
                ViewBag.Departments = _context.Departments.ToList();
                ViewBag.Instructors = _context.Instructors.ToList();
                return View("Edit", courseFromReq);
            }
            var existingCourse = _context.Courses.FirstOrDefault(c => c.Id == courseFromReq.Id);
            if (existingCourse == null)
            {
                return NotFound();
            }
            existingCourse.Title = courseFromReq.Title;
            existingCourse.Credits = courseFromReq.Credits;
            existingCourse.Degree = courseFromReq.Degree;
            existingCourse.MinDegree = courseFromReq.MinDegree;
            existingCourse.DepartmentId = courseFromReq.DepartmentId;
            existingCourse.InstructorId = courseFromReq.InstructorId;
            
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var course = _context.Courses.Find(id);
            if (course == null)
            {
                return NotFound();
            }
            _context.Courses.Remove(course);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
