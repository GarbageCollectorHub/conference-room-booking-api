using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RoomBooking.Domain.Exceptions;

namespace RoomBooking.Api.ErrorHandling
{
    internal sealed class DomainExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetails;

        public DomainExceptionHandler(IProblemDetailsService problemDetails)
        {
            _problemDetails = problemDetails;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            int? statusCode = exception switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                ConflictException => StatusCodes.Status409Conflict,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                BusinessRuleException => StatusCodes.Status400BadRequest,
                _ => null
            };

            // Не наша помилка - далі её обробляє UseExceptionHandler
            if (statusCode is null)
            {
                return false;
            }

            httpContext.Response.StatusCode = statusCode.Value;

            ProblemDetails problem = new()
            {
                Status = statusCode,
                Title = "Request cannot be completed.",
                Detail = exception.Message
            };

            ProblemDetailsContext problemContext = new()
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = problem
            };

            // ProblemDetailsService не пише відповідь, якщо клієнт просить тип, який він
            // не обслуговує (Swagger шле accept: text/plain). Тоді формуємо тіло самі.
            if (!await _problemDetails.TryWriteAsync(problemContext))
            {
                problem.Extensions["traceId"] = httpContext.TraceIdentifier;

                await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
            }

            return true;
        }
    }
}