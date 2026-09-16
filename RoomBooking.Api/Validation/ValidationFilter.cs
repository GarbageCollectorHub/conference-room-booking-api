using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace RoomBooking.Api.Validation
{
    // Один фільтр на всі endpoints: для кожного аргументу метода контролера шукає в DI відповідний валідатор FluentValidation
    // Якщо є помилки, одразу повертає 400 зі списком усіх помилок, контролер не викликається
    internal sealed class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (object? argument in context.ActionArguments.Values)
            {
                if (argument is not null)
                {
                    await ValidateAsync(argument, context);
                }
            }

            if (!context.ModelState.IsValid)
            {
                ApiBehaviorOptions options = context.HttpContext.RequestServices
                    .GetRequiredService<IOptions<ApiBehaviorOptions>>()
                    .Value;

                context.Result = options.InvalidModelStateResponseFactory(context);
                return;
            }

            await next();
        }

        private static async Task ValidateAsync(object argument, ActionExecutingContext context)
        {
            IValidator? validator = FindValidator(argument, context.HttpContext);

            if (validator is null)
            {
                return;
            }

            // тип DTO тут відомий лише як object, валідатор сам перевіряє що тип підходить
            ValidationResult result = await validator.ValidateAsync(
                new ValidationContext<object>(argument),
                context.HttpContext.RequestAborted);    // abort якщо клієнт оборвав запит

            foreach (ValidationFailure error in result.Errors)
            {
                context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
        }

        private static IValidator? FindValidator(object argument, HttpContext httpContext)
        {
            Type validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());

            return httpContext.RequestServices.GetService(validatorType) as IValidator;
        }
    }
}