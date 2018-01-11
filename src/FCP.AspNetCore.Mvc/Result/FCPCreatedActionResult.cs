using FCP.Core;

namespace Microsoft.AspNetCore.Mvc
{
    public class FCPCreatedActionResult<TKey> : FCPActionResult<TKey>
    {
        public FCPCreatedActionResult(FCPDoResult<TKey> doResult)
            : base(doResult)
        { }

        protected override IActionResult FormatActionResult(ActionContext context)
        {
            return DoResult.ToActionResult(context, FCPActionResultType.created);
        }

        public static implicit operator FCPCreatedActionResult<TKey>(FCPDoResult<TKey> doResult)
        {
            return new FCPCreatedActionResult<TKey>(doResult);
        }
    }
}
