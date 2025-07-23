using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvcFirstApp.Models.Data;
using mvcFirstApp.Models.Entities;
using mvcFirstApp.Services;
using mvcFirstApp.ViewModels;

namespace mvcFirstApp.Controllers
{
    public class TraineeController : Controller
    {
        private readonly AppDbContext _context;
        public readonly FileUploadService _fileUploadService;

        public TraineeController(FileUploadService fileUploadService)
        {
            _context = new AppDbContext();
            _fileUploadService = fileUploadService;
        }

        public IActionResult Index(int page = 1)
        {
            int pageSize = 6; // Number of items per page

            var trainees = _context.Trainees
                .Include(t => t.Department)
                .Include(t => t.CourseResults)
                .AsQueryable();
            PaginatedList<Trainee> paginatedList = PaginatedList<Trainee>.CreateAsync(trainees, page, pageSize);
            ViewBag.Departments = _context.Departments.ToList();
            return View("Index", paginatedList);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var trainee = _context.Trainees
                .Include(t => t.Department)
                .Include(t => t.CourseResults)
                .FirstOrDefault(t => t.Id == id);
            if (trainee == null)
            {
                return NotFound();
            }
            return View("Details", trainee);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Departments = _context.Departments.ToList();
            return View("Add");
        }

        [HttpPost]
        public IActionResult SaveAdd(Trainee traineeFromReq)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _context.Departments.ToList();
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
                    ViewBag.Departments = _context.Departments.ToList();
                    return View("Add", traineeFromReq);
                }
            }

            _context.Trainees.Add(traineeFromReq);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var trainee = _context.Trainees
                .Include(t => t.Department)
                .FirstOrDefault(t => t.Id == id);
            if (trainee == null)
            {
                return NotFound();
            }
            ViewBag.Departments = _context.Departments.ToList();
            return View("Edit", trainee);
        }
        [HttpPost]
        public IActionResult SaveEdit(Trainee traineeFromReq)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _context.Departments.ToList();
                return View("Edit", traineeFromReq);
            }
            var existingTrainee = _context.Trainees.Find(traineeFromReq.Id);
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
                    ViewBag.Departments = _context.Departments.ToList();
                    return View("Edit", traineeFromReq);
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]

        public IActionResult Delete(int id)
        {
            var trainee = _context.Trainees.Find(id);
            if (trainee == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(trainee.ImageUrl))
            {
                _fileUploadService.DeleteFile(trainee.ImageUrl);
            }
            _context.Trainees.Remove(trainee);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }   
}
