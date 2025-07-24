using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using mvcFirstApp.Models.Data;
using mvcFirstApp.Models.Entities;
using mvcFirstApp.Repositories;
using mvcFirstApp.Services;
using mvcFirstApp.ViewModels;

namespace mvcFirstApp.Controllers
{
    public class TraineeController : Controller
    {
        private readonly IRepository<Course> _Courses;
        private readonly IRepository<Department> _Departments;
        private readonly IRepository<Instructor> _Instructors;
        private readonly IRepository<Trainee> _Trainees;
        private readonly ITraineeRepository _traineeRepository;
        public readonly FileUploadService _fileUploadService;

        public TraineeController
            (FileUploadService fileUploadService,
            IRepository<Course> Courserepository
            , IRepository<Department> deptRepo
            , IRepository<Instructor> instructors
            , IRepository<Trainee> trainees
            , ITraineeRepository traineeRepository)
        {
            _Instructors = instructors;
            _Trainees = trainees;
            _Courses = Courserepository;
            _Departments = deptRepo;
            _fileUploadService = fileUploadService;
            _traineeRepository = traineeRepository;
            _traineeRepository = traineeRepository;
        }

        public IActionResult Index(int page = 1)
        {
            int pageSize = 6; // Number of items per page
            var paginatedTrainees = _Courses.GetAll(page, pageSize,
                includeProperties: "Department,CoursesResults");

            return View("Index", paginatedTrainees);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid trainee ID.");
            }
            var trainee = _Trainees.GetById(id, "Department,CoursesResults");

            if (trainee == null)
            {
                return NotFound();
            }
            return View("Details", trainee);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Departments = _Departments.GetAll().ToList();
            return View("Add");
        }

        [HttpPost]
        public IActionResult SaveAdd(Trainee traineeFromReq)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _Departments.GetAll().ToList();
                return View("Add", traineeFromReq);
            }

            if (traineeFromReq.ImageFile != null && traineeFromReq.ImageFile.Length > 0)
            {
                try
                {
                    var imageUrl = _fileUploadService.UploadFile(traineeFromReq.ImageFile, "uploads/trainees");
                    traineeFromReq.ImageUrl = imageUrl;
                }
                catch (Exception ex)
                {
                    ViewBag.Departments = _Departments.GetAll().ToList();
                    return View("Add", traineeFromReq);
                }
            }

            _Trainees.Add(traineeFromReq);
            _Trainees.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var trainee = _Trainees.GetById(id, "Department,CoursesResults");
            if (trainee == null)
            {
                return NotFound();
            }
            ViewBag.Departments = _Departments.GetAll().ToList();
            return View("Edit", trainee);
        }
        [HttpPost]
        public IActionResult SaveEdit(Trainee traineeFromReq)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _Departments.GetAll().ToList();
                return View("Edit", traineeFromReq);
            }
            var existingTrainee = _Trainees.GetById(traineeFromReq.Id);
            if (existingTrainee == null)
            {
                return NotFound();
            }
            existingTrainee.Name = traineeFromReq.Name;
            existingTrainee.Email = traineeFromReq.Email;
            existingTrainee.Address = traineeFromReq.Address;
            existingTrainee.DateOfBirth = traineeFromReq.DateOfBirth;
            existingTrainee.DepartmentId = traineeFromReq.DepartmentId;

            if (traineeFromReq.ImageFile != null && traineeFromReq.ImageFile.Length > 0)
            {
                try
                {
                    var imageUrl = _fileUploadService.UploadFile(traineeFromReq.ImageFile, "uploads/trainees");
                    existingTrainee.ImageUrl = imageUrl;
                }
                catch (Exception ex)
                {
                    ViewBag.Departments = _Departments.GetAll().ToList();
                    return View("Edit", traineeFromReq);
                }
            }
            _Trainees.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var trainee = _Trainees.GetById(id);
            if (trainee == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(trainee.ImageUrl))
            {
                _fileUploadService.DeleteFile(trainee.ImageUrl);
            }
            _Trainees.Delete(trainee.Id);
            _Trainees.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetResult(int Id, int crsId)
        {
            var viewModel = _traineeRepository.GetTraineeResult(Id, crsId);

            if (viewModel == null)
            {
                return NotFound("No result found for this trainee in the specified course");
            }

            ViewBag.TraineeId = Id;
            return View("ResultDetails", viewModel);
        }

        [HttpGet]
        public IActionResult GetAllResults(int Id)
        {
            var viewModel = _traineeRepository.GetAllTraineeResults(Id);

            if (viewModel == null || viewModel.Count == 0)
            {
                return NotFound("No results found for this trainee");
            }

            ViewBag.TraineeId = Id;
            return View("AllResults", viewModel);
        }
        private static string GetPerformanceLevel(int actualGrade, int minDegree, int maxDegree)
        {
            if (maxDegree <= minDegree) return "Invalid";
            if (actualGrade < minDegree) return "F"; // Failed

            var passingRange = maxDegree - minDegree;
            var gradeAboveMin = actualGrade - minDegree;
            var performancePercentage = (double)gradeAboveMin / passingRange;

            return performancePercentage switch
            {
                >= 0.85 => "A",
                >= 0.70 => "B",
                >= 0.5 => "C",
                >= 0.25 => "D",
                _ => "D-"
            };
        }
    }   
}
