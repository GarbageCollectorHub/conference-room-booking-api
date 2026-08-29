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
            CancellationToken cancellationToken
            )
        {
            int? statusCode = exception switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                ConflictException => StatusCodes.Status409Conflict,
                BusinessRuleException => StatusCodes.Status400BadRequest,
                _ => null
            };

            // Не наша помилка: віддаємо стандартному обробнику, який поверне 500
            // без подробиць і запише виняток у лог.
            if (statusCode is null)
            {
                return false;
            }

            httpContext.Response.StatusCode = statusCode.Value;

            return await _problemDetails.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = "Request cannot be completed.",
                    Detail = exception.Message
                }
            });
        }
    }
}
