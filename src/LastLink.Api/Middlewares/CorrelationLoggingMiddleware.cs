using System.Diagnostics;
using System.Text;

namespace LastLink.Api.Middlewares
{
    public class CorrelationLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationLoggingMiddleware> _logger;

        public CorrelationLoggingMiddleware(
            RequestDelegate next,
            ILogger<CorrelationLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId =
                context.Request.Headers.ContainsKey("X-Correlation-Id")
                ? context.Request.Headers["X-Correlation-Id"].ToString()
                : Guid.NewGuid().ToString();

            context.Items["CorrelationId"] = correlationId;
            context.Response.Headers["X-Correlation-Id"] = correlationId;

            var stopwatch = Stopwatch.StartNew();

            var requestBody = await ReadRequestBody(context.Request);

            _logger.LogInformation(
                "[{CorrelationId}] REQUEST => {Method} {Path} Query: {Query} Body: {Body}",
                correlationId,
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString.ToString(),
                requestBody
            );

            var originalBodyStream = context.Response.Body;
            using var newBodyStream = new MemoryStream();
            context.Response.Body = newBodyStream;

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                _logger.LogInformation(
                    "[{CorrelationId}] RESPONSE => {Status} Body: {Body} | Tempo: {Elapsed} ms",
                    correlationId,
                    context.Response.StatusCode,
                    responseBody,
                    stopwatch.ElapsedMilliseconds
                );

                await newBodyStream.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
            }
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering();

            request.Body.Seek(0, SeekOrigin.Begin);
            string body = await new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true).ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);

            if (body.Length > 2000)
                body = body[..2000] + "... (truncated)";

            return body;
        }
    }
}