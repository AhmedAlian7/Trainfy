using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class TraineeController : Controller
    {
        private readonly ICourseRepository _Courses;
        private readonly IRepository<Department> _Departments;
        private readonly IRepository<Instructor> _Instructors;
        private readonly ITraineeRepository _Trainees;
        public readonly FileUploadService _fileUploadService;

        public TraineeController
            (FileUploadService fileUploadService,
            ICourseRepository Courserepository
            , IRepository<Department> deptRepo
            , IRepository<Instructor> instructors
            , ITraineeRepository traineeRepository)
        {
            _Instructors = instructors;
            _Trainees = traineeRepository;
            _Courses = Courserepository;
            _Departments = deptRepo;
            _fileUploadService = fileUploadService;
        }

        public IActionResult Index(int page = 1)
        {
            int pageSize = 6; // Number of items per page
            var paginatedTrainees = _Trainees.GetAll(page, pageSize,
                includeProperties: "Department");

            ViewBag.Departments = _Departments.GetAll();
            return View("Index", paginatedTrainees);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid trainee ID.");
            }
            var trainee = _Trainees.GetById(id, "Department,CourseResults");

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
            var viewModel = _Trainees.GetTraineeResult(Id, crsId);

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
            var viewModel = _Trainees.GetAllTraineeResults(Id);

            if (viewModel == null || viewModel.Count == 0)
            {
                return NotFound("No results found for this trainee");
            }

            ViewBag.TraineeId = Id;
            return View("AllResults", viewModel);
        }

    }   
}
