using FCP.Core;
using System;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc
{
    public class FCPActionResult<TResultData> : IActionResult
    {
        public FCPActionResult(FCPDoResult<TResultData> doResult)
        {
            DoResult = doResult ?? throw new ArgumentNullException(nameof(doResult));
        }

        public FCPDoResult<TResultData> DoResult { get; }

        protected virtual IActionResult FormatActionResult(ActionContext context)
        {
            return DoResult.ToActionResult(context);
        }

        public virtual Task ExecuteResultAsync(ActionContext context)
        {
            var actionResult = FormatActionResult(context);

            return actionResult.ExecuteResultAsync(context);
        }

        public static implicit operator FCPActionResult<TResultData>(FCPDoResult<TResultData> doResult)
        {
            return new FCPActionResult<TResultData>(doResult);
        }
    }
}
