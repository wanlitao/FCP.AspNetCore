namespace Microsoft.AspNetCore.Mvc
{
    public class WebLinkingItem
    {
        public string Url { get; set; }

        public string Relation { get; set; }

        public override string ToString()
        {
            return $"<{Url}>; rel=\"{Relation}\"";
        }
    }
}
