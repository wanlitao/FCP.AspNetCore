using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FCPMvcApiExplorerMvcCoreBuilderExtensions
    {
        public static IMvcCoreBuilder AddFCPActionResultResponseType(this IMvcCoreBuilder builder)
        {
            if (builder == null)            
                throw new ArgumentNullException(nameof(builder));            

            AddApiExplorerServices(builder.Services);
            return builder;
        }

        // Internal for testing.
        internal static void AddApiExplorerServices(IServiceCollection services)
        {
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IApiDescriptionProvider, FCPActionResultApiDescriptionProvider>());            
        }
    }
}
