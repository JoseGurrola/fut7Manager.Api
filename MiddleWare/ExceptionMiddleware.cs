using System.Net;
using System.Text.Json;

namespace fut7Manager.Middleware {

    // Middleware personalizado para capturar errores globales
    public class ExceptionMiddleware {
        private readonly RequestDelegate _next; // Referencia al siguiente middleware del pipeline
        private readonly ILogger<ExceptionMiddleware> _logger;


        // El constructor recibe automáticamente estas dependencias
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger) {
            _next = next;
            _logger = logger;
        }

        // Método que se ejecuta cada vez que llega una request HTTP
        public async Task InvokeAsync(HttpContext context) {

            try {

                // Pasa la request al siguiente middleware del pipeline (auth, controllers, etc.)
                await _next(context);

            }
            catch (Exception ex) {

                // Si ocurre cualquier excepción en la API  se registra en el log
                _logger.LogError(ex,
                    "Unhandled exception for {method} {url}",
                    context.Request.Method,
                    context.Request.Path);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; //500
                context.Response.ContentType = "application/json";
                //objeto de respuesta
                var response = new {
                    success = false,
                    message = $"Internal server error: {ex.Message}"
                };

                var json = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(json);
            }
        }
    }
}