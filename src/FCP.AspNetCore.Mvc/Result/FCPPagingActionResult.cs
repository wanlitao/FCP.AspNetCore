using FCP.Core;

namespace Microsoft.AspNetCore.Mvc
{
    public class FCPPagingActionResult<TResultData> : FCPActionResult<FCPPageData<TResultData>>
        where TResultData : class
    {
        public FCPPagingActionResult(FCPDoResult<FCPPageData<TResultData>> doResult)
            : base(doResult)
        { }

        protected override IActionResult FormatActionResult(ActionContext context)
        {
            return base.FormatActionResult(context);
        }

        public static implicit operator FCPPagingActionResult<TResultData>(FCPDoResult<FCPPageData<TResultData>> doResult)
        {
            return new FCPPagingActionResult<TResultData>(doResult);
        }
    }
}
