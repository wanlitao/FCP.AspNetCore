using Newtonsoft.Json;
using System;

namespace Microsoft.AspNetCore.Mvc
{
    public static class MvcJsonOptionsExtensions
    {
        public static MvcJsonOptions UseNullValueIgnore(this MvcJsonOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            return options;
        }
    }
}
