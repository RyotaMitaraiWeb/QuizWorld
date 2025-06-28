using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace QuizWorld.ViewModels.Validators
{
    public class IsImage : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not IFormFile file)
                return new ValidationResult(NotAFileErrorMessage);

            try
            {
                using var stream = file.OpenReadStream();
                var imageInfo = SixLabors.ImageSharp.Image.Identify(stream);

                if (imageInfo is null)
                    return new ValidationResult(NotAnImageErrorMessage);

                return ValidationResult.Success;
            }
            catch
            {
                return new ValidationResult(NotAnImageErrorMessage);
            }
        }

        private static readonly string NotAFileErrorMessage = "The provided value is not a file";
        private static readonly string NotAnImageErrorMessage = "The file is not a valid image.";
    }
}
