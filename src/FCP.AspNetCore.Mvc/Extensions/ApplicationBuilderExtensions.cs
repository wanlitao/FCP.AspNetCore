using Microsoft.AspNetCore.Http;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWebApiExceptionHandler(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            
            return app.UseMiddleware<WebApiExceptionHandlingMiddleware>();
        }
    }
}
