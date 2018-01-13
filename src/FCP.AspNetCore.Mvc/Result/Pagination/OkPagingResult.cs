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

        public OkPagingResult(object value, params WebLinkingItem[] linkItems)
            : base(value)
        {
            Links = linkItems;
        }

        public IList<WebLinkingItem> Links { get; set; }

        public override void OnFormatting(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            base.OnFormatting(context);

            if (Links.isNotEmpty())
            {
                context.HttpContext.Response.Headers[LinkHeader] = string.Join(",", Links);
            }
        }
    }
}
