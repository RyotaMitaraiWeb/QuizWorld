using QuizWorld.ViewModels.Image;

namespace QuizWorld.Web.Contracts
{
    public interface IImageService
    {
        Task<UploadedImageViewModel> UploadImage(UploadImageViewModel model);
        string DefaultFileExtension { get; }
    }
}
