using System;

namespace Microsoft.AspNetCore.Mvc
{
    public static class MvcOptionsExtensions
    {
        public static MvcOptions AddProducesJson(this MvcOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            options.Filters.Add(new ProducesAttribute("application/json"));

            return options;
        }
    }
}
