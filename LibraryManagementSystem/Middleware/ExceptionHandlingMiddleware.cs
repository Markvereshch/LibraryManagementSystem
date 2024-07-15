namespace LibraryManagementSystem.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WARNING! Unhandled error has occurred");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                var details = new
                {
                    Title = "Unhandled error has occurred",
                    ErrorMessage = ex.Message,
                    Status = context.Response.StatusCode
                };
                await context.Response.WriteAsJsonAsync(details);
            }
        }
    }
}
