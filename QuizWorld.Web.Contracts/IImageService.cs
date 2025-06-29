using QuizWorld.ViewModels.Image;

namespace QuizWorld.Web.Contracts
{
    public interface IImageService
    {
        Task<UploadedImageViewModel> UploadImage(UploadImageViewModel model);
        Task<DeletedImageResultViewModel> DeleteImage(DeleteImageViewModel model);
        string DefaultFileExtension { get; }
    }
}
