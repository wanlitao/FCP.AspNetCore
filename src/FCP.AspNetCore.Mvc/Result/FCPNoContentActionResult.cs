using FCP.Core;

namespace Microsoft.AspNetCore.Mvc
{
    public class FCPNoContentActionResult<TResultData> : FCPActionResult<TResultData>
    {
        public FCPNoContentActionResult(FCPDoResult<TResultData> doResult)
            : base(doResult)
        { }

        protected override IActionResult FormatActionResult(ActionContext context)
        {
            return DoResult.ToActionResult(context, FCPActionResultType.noContent);
        }

        public static implicit operator FCPNoContentActionResult<TResultData>(FCPDoResult<TResultData> doResult)
        {
            return new FCPNoContentActionResult<TResultData>(doResult);
        }
    }
}
