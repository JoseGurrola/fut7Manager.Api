namespace fut7Manager.Api.Services.Interfaces {
    public interface IFileStorageService {
        Task<string> SaveImageAsync(IFormFile file, string type, HttpRequest request);
    }
}
