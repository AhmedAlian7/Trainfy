using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvcFirstApp.Models.Data;
using mvcFirstApp.Models.Entities;
using mvcFirstApp.Repositories;
using mvcFirstApp.Services;
using mvcFirstApp.ViewModels;
using System.Globalization;

namespace mvcFirstApp.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        private readonly ICourseRepository _courses;
        private readonly IRepository<Department> _departments;
        private readonly IRepository<Instructor> _instructors;

        public CourseController
            (ICourseRepository Courserepository
            ,IRepository<Department> deptRepo
            ,IRepository<Instructor> instructors)
        {
            _courses = Courserepository; 
            _departments = deptRepo;
            _instructors = instructors;
        }

        [HttpGet]
        public IActionResult Index(int page = 1)
        {
            const int pageSize = 6; // Number of courses per page

            var courses = _courses.GetAll(page, pageSize,
                includeProperties: "Department,Instructor");


            ViewBag.Departments = _departments.GetAll().ToList();
            return View("Index", courses);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var course = _courses.GetById(id, "Department,Instructor,CourseResults");
            if (course == null)
            {
                return NotFound();
            }
            return View("Details", course);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Departments = _departments.GetAll().ToList();
            ViewBag.Instructors = _instructors.GetAll().ToList();
            return View("Add");
        }

        [HttpPost]
        public IActionResult SaveAdd(Course courseFromReq)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(courseFromReq.Title) || courseFromReq.Credits <= 0 || courseFromReq.Degree < 0 || courseFromReq.MinDegree < 0)
            {
                ViewBag.Departments = _departments.GetAll().ToList();
                ViewBag.Instructors = _instructors.GetAll().ToList();
                return View("Add", courseFromReq);
            }
            _courses.Add(courseFromReq);
            _courses.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var course = _courses.GetById(id, "Department,Instructor,CourseResults");
            if (course == null)
            {
                return NotFound();
            }

            ViewBag.Departments = _departments.GetAll().ToList();
            ViewBag.Instructors = _instructors.GetAll().ToList();
            return View("Edit", course);
        }
        [HttpPost]
        public IActionResult SaveEdit(int id, Course courseFromReq)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(courseFromReq.Title) || courseFromReq.Credits <= 0 || courseFromReq.Degree < 0 || courseFromReq.MinDegree < 0)
            {
                ViewBag.Departments = _departments.GetAll().ToList();
                ViewBag.Instructors = _instructors.GetAll().ToList();
                return View("Edit", courseFromReq);
            }
            var existingCourse = _courses.GetById(id);
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
            
            _courses.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            _courses.Delete(id);
            _courses.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult GetTraineesResults(int Id // Course Id
            , string searchTerm = "", string sortBy = "", string sortDirection = "", int minDegree =0, int maxDegree = 0) 
        {
            var viewModel = _courses.GetAllTraineeResults
                (Id, searchTerm, sortBy, sortDirection, minDegree, maxDegree);

            if (viewModel == null || viewModel.Count == 0)
            {
                return NotFound("No results found for this course");
            }

            return View("TraineesResults",viewModel); 
        }
    }
}
