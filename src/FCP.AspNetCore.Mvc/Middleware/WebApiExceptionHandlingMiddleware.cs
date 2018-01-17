using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http
{
    public class WebApiExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public WebApiExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context /* other scoped dependencies */)
        {
            var ex = context.Features.Get<IExceptionHandlerFeature>();
            if (ex != null)
            {
                return HandleExceptionAsync(context, ex.Error);
            }

            return _next(context);
        }

        protected virtual Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var result = new ObjectResult(new ErrorResponse(exception.Message))
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            return result.ExecuteResultAsync(new ActionContext() { HttpContext = context });
        }
    }
}
