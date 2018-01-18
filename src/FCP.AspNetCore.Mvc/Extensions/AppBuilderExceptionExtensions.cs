using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public static class AppBuilderExceptionExtensions
    {
        public static IApplicationBuilder UseErrorMessageExceptionHandler(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            return app.UseExceptionHandler(builder => builder.Run(HandleExceptionFeatureAsync));
        }

        private static async Task HandleExceptionFeatureAsync(HttpContext context)
        {
            var ex = context.Features.Get<IExceptionHandlerFeature>();
            if (ex != null)
            {
                await HandleExceptionAsync(context, ex.Error);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var result = new ObjectResult(new ErrorResponse(exception.Message))
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            return result.ExecuteResultAsync(new ActionContext() { HttpContext = context });
        }
    }
}
