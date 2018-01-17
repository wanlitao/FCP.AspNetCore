using FCP.Util;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// 200 response with pagination web linking header
    /// </summary>
    public class OkPagingResult : OkObjectResult
    {
        public const string LinkHeader = "Link";
        public const string TotalHeader = "X-Total-Count";

        public OkPagingResult(object value, long total, params WebLinkingItem[] linkItems)
            : base(value)
        {
            if (total < 0)
                throw new ArgumentOutOfRangeException("invalid total count");

            TotalCount = total;

            Links = linkItems;
        }

        public long TotalCount { get; }

        public IList<WebLinkingItem> Links { get; }

        public override void OnFormatting(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            base.OnFormatting(context);

            context.HttpContext.Response.Headers[TotalHeader] = TotalCount.ToString();
            if (Links.isNotEmpty())
            {
                context.HttpContext.Response.Headers[LinkHeader] = string.Join(",", Links);
            }
        }
    }
}
