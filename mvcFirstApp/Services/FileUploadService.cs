namespace mvcFirstApp.Services
{
    public class FileUploadService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public FileUploadService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string UploadFile(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                return null;

            // Validate file size
            if (file.Length > MaxFileSize)
            {
                throw new InvalidOperationException("File size cannot exceed 5MB.");
            }

            // Validate file extension
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException("Only image files (jpg, jpeg, png, gif, webp) are allowed.");
            }

            // Create unique filename
            var fileName = Guid.NewGuid().ToString() + fileExtension;

            // Create upload directory if it doesn't exist
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, folderName);
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Save file
            var filePath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // Return relative path for storing in database
            return $"/{folderName}/{fileName}";
        }

        public bool DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            try
            {
                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
            }
            catch (Exception)
            {
                // Log exception if needed
            }

            return false;
        }
    }
}
