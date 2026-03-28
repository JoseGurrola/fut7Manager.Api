using System.Text;

namespace fut7Manager.Api.MiddleWare {
    public class RequestResponseLoggingMiddleware {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestResponseLoggingMiddleware> logger) {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context) {
            context.Request.EnableBuffering();

            var requestBody = await new StreamReader(
                context.Request.Body,
                Encoding.UTF8,
                leaveOpen: true).ReadToEndAsync();

            context.Request.Body.Position = 0;

            var queryString = context.Request.QueryString.HasValue
                ? context.Request.QueryString.Value
                : string.Empty;

            _logger.LogInformation(
                $"HTTP {context.Request.Method} {context.Request.Path}{queryString} Body:\n{FormatJson(requestBody)}");

            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            responseBody.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(responseBody).ReadToEndAsync();


            _logger.LogInformation($"Response {context.Response.StatusCode} Body:\n{FormatJson(responseText)}");

            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }

        private string FormatJson(string json) {
            try {
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                return System.Text.Json.JsonSerializer.Serialize(
                    doc,
                    new System.Text.Json.JsonSerializerOptions {
                        WriteIndented = true
                    });
            }
            catch {
                // Si no es JSON válido, se regresa tal cual
                return json;
            }
        }
    }
}
