using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvcFirstApp.Models.Data;
using mvcFirstApp.Models.Entities;

namespace mvcFirstApp.Controllers
{
    public class CourseController : Controller
    {
        private readonly AppDbContext _context;
        public CourseController() { _context = new AppDbContext(); }

        [HttpGet]
        public IActionResult Index()
        {
            var courses = _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Instructor)
                .ToList();
            return View("Index", courses);
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
            if (string.IsNullOrEmpty(courseFromReq.Title) || courseFromReq.Credits <= 0 || courseFromReq.Degree < 0 || courseFromReq.MinDegree < 0)
            {
                return View("Add", courseFromReq);
            }
            _context.Courses.Add(courseFromReq);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
