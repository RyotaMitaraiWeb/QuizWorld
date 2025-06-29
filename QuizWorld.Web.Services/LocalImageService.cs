using QuizWorld.ViewModels.Image;
using QuizWorld.Web.Contracts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace QuizWorld.Web.Services
{
    public class LocalImageService(IContentRootPathProvider contentRoot) : IImageService
    {
        private readonly IContentRootPathProvider _contentRootPathProvider = contentRoot;
        public static readonly string BasePath = "images";
        public string ImagesPath => $"{_contentRootPathProvider.Path}/{BasePath}";

        public string DefaultFileExtension { get; } = "webp";
        public async Task<UploadedImageViewModel> UploadImage(UploadImageViewModel model)
        {
            using Image image = await LoadImage(model);

            if (model.ResizeOptions is not null)
            {
                ResizeImage(image, model.ResizeOptions);
            }

            string path = GetImagePath(model);
            await ConvertToWebp(image, path);

            return new UploadedImageViewModel()
            {
                Name = Path.Combine(model.Directory, model.FileName),
            };
        }

        /// <summary>
        /// Constructs the path to the file, creating its directory if said directory
        /// does not exist
        /// </summary>
        /// <param name="model"></param>
        /// <returns>A valid path to the image</returns>
        private string GetImagePath(UploadImageViewModel model)
        {
            string directory = Path.Combine(ImagesPath, model.Directory);
            string path = Path.Combine(directory, model.FileName);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return path;
        }

        private static async Task<Image> LoadImage(UploadImageViewModel model)
        {
            var file = model.Image;
            using var stream = file.OpenReadStream();
            var image = await Image.LoadAsync<Rgba32>(stream);
            return image;
        }

        private static void ResizeImage(Image image, ImageDimensionOptionsViewModel resizeOptions)
        {
            var size = new Size()
            {
                Width = resizeOptions.Width,
                Height = resizeOptions.Height
            };

            image.Mutate(i => i.Resize(size));
        }

        private static async Task ConvertToWebp(Image image, string filePath)
        {
            using var stream = new MemoryStream();
            var encoder = new WebpEncoder()
            {
                Quality = ImageQuality,
                FileFormat = WebpFileFormatType.Lossy,
            };

            await image.SaveAsWebpAsync(filePath, encoder);
        }

        public Task<DeletedImageResultViewModel> DeleteImage(DeleteImageViewModel data)
        {
            string filePath = Path.Combine(ImagesPath, data.Directory, data.FileName);
            var result = new DeletedImageResultViewModel()
            {
                ImageExisted = false,
            };

            if (!File.Exists(filePath))
            {
                return Task.FromResult(result);
            }

            result.ImageExisted = true;
            File.Delete(filePath);
            return Task.FromResult(result);
        }

        private static readonly int ImageQuality = 75;
    }
}
