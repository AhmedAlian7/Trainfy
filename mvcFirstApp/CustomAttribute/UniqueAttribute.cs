using mvcFirstApp.Models.Data;
using mvcFirstApp.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace mvcFirstApp.CustomAttribute
{
    public class UniqueAttribute : ValidationAttribute
    {
        public UniqueAttribute() { }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("This field is required.");
            }
            var context = validationContext.GetService<AppDbContext>();
            if (context == null)
            {
                // Fallback: create a new context
                using var fallbackContext = new AppDbContext();
                return ValidateUniqueness(validationContext, fallbackContext, value.ToString());
            }

            return ValidateUniqueness(validationContext, context, value.ToString());
        }

        private ValidationResult? ValidateUniqueness(ValidationContext validationContext, AppDbContext context, string title)
        {
            var current = (Course)validationContext.ObjectInstance;


            // Single query to check if title exists
            var exists = context.Courses.Any(c => c.Title == title && c.Id != current.Id); // ); 

            if (exists)
            {
                return new ValidationResult("This course is already exists in the system, course title must be unique.");
            }

            return ValidationResult.Success;
        }
    }
}
