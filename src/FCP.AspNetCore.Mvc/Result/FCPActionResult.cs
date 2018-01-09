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

            Result = doResult.ToActionResult();
        }

        public FCPDoResult<TResultData> DoResult { get; }

        public ActionResult Result { get; }

        public Task ExecuteResultAsync(ActionContext context)
        {
            return Result.ExecuteResultAsync(context);
        }

        public static implicit operator FCPActionResult<TResultData>(FCPDoResult<TResultData> doResult)
        {
            return new FCPActionResult<TResultData>(doResult);
        }
    }
}
