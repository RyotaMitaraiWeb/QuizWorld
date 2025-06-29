using Microsoft.AspNetCore.Http;

namespace QuizWorld.ViewModels.Image
{
    public class UploadImageViewModel
    {
        public IFormFile Image { get; set; } = default!;
        public string Directory { get; set; } = default!;
        public string FileName { get; set; } = default!;
        public ImageDimensionOptionsViewModel? ResizeOptions { get; set; }
    }
}
