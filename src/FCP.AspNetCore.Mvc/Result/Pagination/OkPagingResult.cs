namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// 200 response with pagination web linking header
    /// </summary>
    public class OkPagingResult : OkObjectResult
    {
        public OkPagingResult(object value)
            : base(value)
        {

        }
    }
}
