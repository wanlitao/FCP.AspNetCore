using FCP.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc.ApiExplorer
{
    public class FCPActionResultApiDescriptionProvider : IApiDescriptionProvider
    {
        /// <summary>
        /// Ensure the first Provider invoke the OnProvidersExecuted method
        /// </summary>
        public virtual int Order => Int32.MinValue;

        public void OnProvidersExecuted(ApiDescriptionProviderContext context)
        {
        }

        public void OnProvidersExecuting(ApiDescriptionProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            foreach (var action in context.Actions.OfType<ControllerActionDescriptor>())
            {
                if (action.AttributeRouteInfo != null && action.AttributeRouteInfo.SuppressPathMatching)
                {
                    continue;
                }

                action.FilterDescriptors = action.FilterDescriptors ?? new List<FilterDescriptor>();

                var responseMetadataAttributes = GetFCPActionResultResponseMetadataAttributes(action);
                foreach (var responseMetadataAttribute in responseMetadataAttributes)
                {
                    action.FilterDescriptors.Add(new FilterDescriptor(responseMetadataAttribute, FilterScope.Action));
                }
            }
        }

        private static IEnumerable<IApiResponseMetadataProvider> GetFCPActionResultResponseMetadataAttributes(ControllerActionDescriptor action)
        {
            var declaredReturnType = action.MethodInfo.ReturnType;

            // Unwrap the type if it's a Task<T>. The Task (non-generic) case was already handled.
            var unwrappedType = UnwrapGenericType(declaredReturnType, typeof(Task<>));

            if (IsGenericType(unwrappedType, typeof(FCPActionResult<>)))
            {
                var statusCode = StatusCodes.Status200OK;
                var responseType = UnwrapGenericType(unwrappedType, typeof(FCPActionResult<>));

                if (IsGenericType(unwrappedType, typeof(FCPCreatedActionResult<>)))
                {
                    statusCode = StatusCodes.Status201Created;
                }
                else if (IsGenericType(unwrappedType, typeof(FCPNoContentActionResult<>)))
                {
                    statusCode = StatusCodes.Status204NoContent;
                    responseType = typeof(void);
                }

                // Unwrap the type if it's FCPPageData<T>
                if (IsGenericType(responseType, typeof(FCPPageData<>)))
                {
                    responseType = UnwrapGenericType(responseType, typeof(FCPPageData<>));
                    responseType = typeof(IList<>).MakeGenericType(responseType);
                }

                yield return new ProducesResponseTypeAttribute(responseType, statusCode);
            }

            Type UnwrapGenericType(Type type, Type queryType)
            {
                var genericType = ClosedGenericMatcher.ExtractGenericInterface(type, queryType);
                return genericType?.GenericTypeArguments[0] ?? type;
            }

            bool IsGenericType(Type type, Type queryType)
            {
                var genericType = ClosedGenericMatcher.ExtractGenericInterface(type, queryType);
                return genericType != null;
            }
        }
    }
}
