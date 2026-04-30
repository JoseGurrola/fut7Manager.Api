using fut7Manager.Api.Services.Interfaces;

namespace fut7Manager.Api.Services {
    public class FileStorageService : IFileStorageService {
        public async Task<string> SaveImageAsync(IFormFile file, string type, HttpRequest request) {
            var folder = type switch {
                "team" => "teams",
                "league" => "leagues",
                "player" => "players",
                _ => "others"
            };

            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "images",
                folder
            );

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/images/{folder}/{fileName}";
        }
    }
}
