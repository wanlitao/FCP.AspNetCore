using FCP.Core;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Mvc
{
    internal static class FCPDoResultExtensions
    {
        internal static IActionResult ToActionResult<T>(this FCPDoResult<T> doResult, ActionContext context)
        {
            return ToActionResult(doResult, context, FCPActionResultType.none);
        }

        internal static IActionResult ToActionResult<T>(this FCPDoResult<T> doResult, ActionContext context, FCPActionResultType resultType)
        {
            if (doResult == null)
                throw new ArgumentNullException(nameof(doResult));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return FCPDoResultConverter.GetConverter(doResult, resultType).Convert(doResult, context);
        }

        internal static FCPDoResult<FCPPageData<T>> AsPaging<T>(this FCPDoResult<IList<T>> doResult) where T : class
        {
            if (doResult == null)
                throw new ArgumentNullException(nameof(doResult));

            return new FCPDoResult<FCPPageData<T>>
            {
                type = doResult.type,
                msg = doResult.msg,
                validFailResults = doResult.validFailResults,
                data = doResult.isSuc ? new FCPPageData<T>
                        {
                            pageIndex = 1,
                            pageSize = doResult.data.Count,
                            total = doResult.data.Count,
                            data = doResult.data
                        } : null
            };
        }
    }
}
