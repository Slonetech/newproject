using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SchoolApi.Attributes
{
    public class ValidEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var email = value.ToString();
            if (string.IsNullOrWhiteSpace(email))
                return new ValidationResult("Email is required.");

            // Basic email validation
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(email))
                return new ValidationResult("Invalid email format.");

            return ValidationResult.Success;
        }
    }

    public class ValidPhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var phoneNumber = value.ToString();
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return ValidationResult.Success; // Phone number is optional

            // Basic phone number validation (allows various formats)
            var phoneRegex = new Regex(@"^[\+]?[1-9][\d]{0,15}$");
            if (!phoneRegex.IsMatch(phoneNumber.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "")))
                return new ValidationResult("Invalid phone number format.");

            return ValidationResult.Success;
        }
    }

    public class ValidGradeAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (value is double grade)
            {
                if (grade < 0 || grade > 100)
                    return new ValidationResult("Grade must be between 0 and 100.");
            }
            else if (value is int gradeInt)
            {
                if (gradeInt < 0 || gradeInt > 100)
                    return new ValidationResult("Grade must be between 0 and 100.");
            }

            return ValidationResult.Success;
        }
    }

    public class ValidDateRangeAttribute : ValidationAttribute
    {
        private readonly DateTime _minDate;
        private readonly DateTime _maxDate;

        public ValidDateRangeAttribute(string minDate = "1900-01-01", string maxDate = "2100-12-31")
        {
            _minDate = DateTime.Parse(minDate);
            _maxDate = DateTime.Parse(maxDate);
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (value is DateTime date)
            {
                if (date < _minDate || date > _maxDate)
                    return new ValidationResult($"Date must be between {_minDate:yyyy-MM-dd} and {_maxDate:yyyy-MM-dd}.");
            }

            return ValidationResult.Success;
        }
    }

    public class ValidStudentGradeAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (value is int grade)
            {
                if (grade < 1 || grade > 12)
                    return new ValidationResult("Student grade must be between 1 and 12.");
            }

            return ValidationResult.Success;
        }
    }

    public class ValidCourseCreditsAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (value is int credits)
            {
                if (credits < 1 || credits > 6)
                    return new ValidationResult("Course credits must be between 1 and 6.");
            }

            return ValidationResult.Success;
        }
    }
}