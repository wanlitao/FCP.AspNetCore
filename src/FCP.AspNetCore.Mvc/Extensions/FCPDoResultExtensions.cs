using FCP.Core;
using FCP.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNetCore.Mvc
{
    internal static class FCPDoResultExtensions
    {
        internal static IActionResult ToActionResult<T>(this FCPDoResult<T> doResult, ActionContext context)
        {
            return ToActionResult(doResult, context, FCPActionResultType.none);
        }

        internal static IActionResult ToActionResult<T>(this FCPDoResult<T> doResult,
            ActionContext context, FCPActionResultType resultType)
        {
            if (doResult == null)
                throw new ArgumentNullException(nameof(doResult));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return FCPDoResultConverter.GetConverter(doResult, resultType).Convert(doResult, context);
        }

        internal static IActionResult ToActionResult<T>(this FCPDoResult<FCPPageData<T>> doResult,
            ActionContext context) where T : class
        {
            return ToActionResult(doResult, context, FCPActionResultType.none);
        }

        internal static IActionResult ToActionResult<T>(this FCPDoResult<FCPPageData<T>> doResult,
            ActionContext context, FCPActionResultType resultType) where T : class
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

        #region Paging Link
        internal static IEnumerable<WebLinkingItem> BuildPagingLinks<T>(this FCPDoResult<FCPPageData<T>> doResult,
            ActionContext context) where T : class
        {
            return BuildPagingLinks(doResult, context, "page");
        }

        internal static IEnumerable<WebLinkingItem> BuildPagingLinks<T>(this FCPDoResult<FCPPageData<T>> doResult,
            ActionContext context, string pageParamName) where T : class
        {
            if (doResult == null)
                throw new ArgumentNullException(nameof(doResult));

            if (context == null)
                throw new ArgumentNullException(nameof(context));            

            if (!doResult.isSuc)
                throw new ArgumentException("DoResult should be success");

            var pageData = doResult.data;
            var request = context.HttpContext.Request;
            var queryParams = GetNoPageQueryParams(request, pageParamName);

            if (pageData.pageIndex > 1)
            {
                yield return BuildPagingLink(request, PagingLinkRelations.first,
                    (builder) => builder.Add(pageParamName, "1"), queryParams.ToArray());

                yield return BuildPagingLink(request, PagingLinkRelations.prev,
                    (builder) => builder.Add(pageParamName, (pageData.pageIndex - 1).ToString()), queryParams.ToArray());
            }

            if (pageData.pageIndex < pageData.pageCount)
            {
                yield return BuildPagingLink(request, PagingLinkRelations.next,
                    (builder) => builder.Add(pageParamName, (pageData.pageIndex + 1).ToString()), queryParams.ToArray());

                yield return BuildPagingLink(request, PagingLinkRelations.last,
                    (builder) => builder.Add(pageParamName, pageData.pageCount.ToString()), queryParams.ToArray());
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetNoPageQueryParams(HttpRequest request, string pageParamName)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (pageParamName.isNullOrEmpty())
                throw new ArgumentNullException(nameof(pageParamName));

            var queryParams = request.Query.SelectMany(m => m.Value,
                (item, value) => new KeyValuePair<string, string>(item.Key, value)).ToList();

            queryParams.RemoveAll(m => string.Compare(m.Key, pageParamName, true) == 0);

            return queryParams;
        }

        private static WebLinkingItem BuildPagingLink(HttpRequest request, PagingLinkRelations relation,
            Action<QueryBuilder> addPageParamAction, params KeyValuePair<string, string>[] queryParams)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var queryBuilder = new QueryBuilder(queryParams);
            addPageParamAction?.Invoke(queryBuilder);

            var url = UriHelper.BuildAbsolute(request.Scheme, request.Host, request.PathBase, request.Path, queryBuilder.ToQueryString());

            return new WebLinkingItem { Url = url, Relation = relation.ToString() };
        }
        #endregion
    }
}
