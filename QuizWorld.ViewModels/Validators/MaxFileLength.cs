using Microsoft.AspNetCore.Http;
using QuizWorld.Common.Bytes;
using System.ComponentModel.DataAnnotations;

namespace QuizWorld.ViewModels.Validators
{
    /// <param name="maxLength">Maximum file size, expressed in the specified bytes unit</param>
    /// <param name="unit">Defaults to megabytes</param>
    public class MaxFileLength(int maxLength, ByteUnits unit = ByteUnits.Megabytes) : ValidationAttribute
        
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not IFormFile || value is null)
            {
                return new ValidationResult("Not a file");
            }

            IFormFile file = (value as IFormFile)!;

            int max = ConvertToBytes(maxLength, unit);
            if (file.Length > max)
            {
                return new ValidationResult("Too large");
            }

            return ValidationResult.Success;
        }
        private static int ConvertToBytes(int maxLength, ByteUnits unit)
        {
            int power = (int)unit;
            var result = maxLength * Math.Pow(1024, power);
            return (int)result;
        }
    }
}
