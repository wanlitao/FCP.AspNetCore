using FCP.Core;
using System;

namespace Microsoft.AspNetCore.Mvc
{
    internal static class FCPDoResultActionResultExtensions
    {
        internal static ActionResult ToActionResult<T>(this FCPDoResult<T> doResult)
        {
            if (doResult == null)
                throw new ArgumentNullException(nameof(doResult));

            return FCPDoResultConverter.GetConverter(doResult).Convert(doResult);
        }
    }
}
