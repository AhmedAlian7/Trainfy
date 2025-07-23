using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
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

        [HttpGet]
        public IActionResult GetResult(int Id, int crsId)
        {
            // view model
            var courseRes = _context.CourseResults
                .Include(c => c.Trainee)
                .Include(c => c.Course)
                .FirstOrDefault(c => c.TraineeId == Id
                && c.CourseId == crsId);

            if (courseRes == null)
            {
                return NotFound("No result found for this trainee in the specified course");
            }


            var passed = courseRes.Degree >= courseRes.Course.MinDegree;
            var percentage = courseRes.Course.Degree > 0
                ? (decimal)courseRes.Degree / courseRes.Course.Degree * 100
                : 0;
            var deptName = _context.Departments
                .Where(d => d.Id == courseRes.Course.DepartmentId)
                .Select(d => d.Name)
                .FirstOrDefault() ?? "N/A";
            var viewModel =  new TraineeAllResultsVM
            {
                TraineeName = courseRes.Trainee?.Name ?? "",
                CourseName = courseRes.Course.Title,
                CourseCredits = courseRes.Course.Credits,
                CourseDept = deptName,
                Grade = courseRes.Degree,
                MaxGrade = courseRes.Course.Degree,
                MinDegree = courseRes.Course.MinDegree,
                IsPassed = passed,
                Color = passed ? "green" : "red",
                Percentage = percentage,
                PerformanceLevel = GetPerformanceLevel
                    (courseRes.Degree, courseRes.Course.MinDegree, courseRes.Course.Degree)
            };

            ViewBag.TraineeId = Id;
            return View("ResultDetails", viewModel);
        }

        [HttpGet]
        public IActionResult GetAllResults(int Id)
        {
            // view model
            var res = _context.CourseResults
                .Include(c => c.Trainee)
                .Include(c => c.Course)
                .Where(c => c.TraineeId == Id)
                .ToList();

            if (res == null || res.Count == 0)
            {
                return NotFound("No results found for this trainee");
            }
            var viewModel = res.Select(courseRes =>
            {
                var passed = courseRes.Degree >= courseRes.Course.MinDegree;
                var percentage = courseRes.Course.Degree > 0
                    ? (decimal)courseRes.Degree / courseRes.Course.Degree * 100
                    : 0;

                return new TraineeAllResultsVM
                {
                    TraineeName = courseRes.Trainee.Name,
                    CourseName = courseRes.Course.Title,
                    CourseId = courseRes.CourseId,
                    Grade = courseRes.Degree,
                    MaxGrade = courseRes.Course.Degree,
                    MinDegree = courseRes.Course.Degree,
                    IsPassed = passed,
                    Color = passed ? "green" : "red",
                    Percentage = percentage,
                    PerformanceLevel = GetPerformanceLevel
                        (courseRes.Degree, courseRes.Course.MinDegree, courseRes.Course.Degree)
                };
            }).ToList();
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
