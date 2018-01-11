using FCP.Core;
using FCP.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Microsoft.AspNetCore.Mvc
{
    internal abstract class FCPDoResultConverter
    {
        private static readonly FCPDoResultConverter[] Converters = new FCPDoResultConverter[]
        {
            new FCPDoResultSuccessConverter(),
            new FCPDoResultFailConverter(),
            new FCPDoResultValidFailConverter(),
            new FCPDoResultNotFoundConverter(),

            new FCPDoResultCreatedConverter(),
            new FCPDoResultNoContentConverter()
        };

        internal abstract IActionResult Convert<T>(FCPDoResult<T> doResult, ActionContext context);

        protected abstract bool CanConvert<T>(FCPDoResult<T> doResult, FCPActionResultType resultType);

        internal static FCPDoResultConverter GetConverter<T>(FCPDoResult<T> doResult, FCPActionResultType resultType)
        {
            if (doResult == null)
                throw new ArgumentNullException(nameof(doResult));

            foreach(var converter in Converters)
            {
                if (converter.CanConvert(doResult, resultType))
                {
                    return converter;
                }
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Success Converter
        /// </summary>
        private class FCPDoResultSuccessConverter : FCPDoResultConverter
        {
            internal override IActionResult Convert<T>(FCPDoResult<T> doResult, ActionContext context)
            {
                return new OkObjectResult(doResult.data);
            }

            protected override bool CanConvert<T>(FCPDoResult<T> doResult, FCPActionResultType resultType)
                => doResult.isSuc && resultType == FCPActionResultType.none;
        }

        /// <summary>
        /// Fail Converter
        /// </summary>
        private class FCPDoResultFailConverter : FCPDoResultConverter
        {
            internal override IActionResult Convert<T>(FCPDoResult<T> doResult, ActionContext context)
            {
                return new ObjectResult(doResult.msg)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

            protected override bool CanConvert<T>(FCPDoResult<T> doResult, FCPActionResultType resultType)
                => doResult.type == FCPDoResultType.fail.ToString();
        }

        /// <summary>
        /// ValidFail Converter
        /// </summary>
        private class FCPDoResultValidFailConverter : FCPDoResultConverter
        {
            internal override IActionResult Convert<T>(FCPDoResult<T> doResult, ActionContext context)
            {
                var modelStateDict = new ModelStateDictionary();
                if (doResult.validFailResults.isNotEmpty())
                {
                    foreach (var validFailResult in doResult.validFailResults)
                    {
                        modelStateDict.AddModelError(validFailResult.Key, validFailResult.Value);
                    }
                }

                return new BadRequestObjectResult(modelStateDict);
            }

            protected override bool CanConvert<T>(FCPDoResult<T> doResult, FCPActionResultType resultType)
                => doResult.isValidFail;
        }

        /// <summary>
        /// NotFound Converter
        /// </summary>
        private class FCPDoResultNotFoundConverter : FCPDoResultConverter
        {
            internal override IActionResult Convert<T>(FCPDoResult<T> doResult, ActionContext context)
            {
                return new NotFoundObjectResult(doResult.msg);
            }

            protected override bool CanConvert<T>(FCPDoResult<T> doResult, FCPActionResultType resultType)
                => doResult.type == FCPDoResultType.notFound.ToString();
        }

        /// <summary>
        /// Created Converter
        /// </summary>
        private class FCPDoResultCreatedConverter : FCPDoResultConverter
        {
            internal override IActionResult Convert<T>(FCPDoResult<T> doResult, ActionContext context)
            {
                var location = $"{UriHelper.GetEncodedUrl(context.HttpContext.Request)}/{doResult.data}";
                return new CreatedResult(location, doResult.data);
            }

            protected override bool CanConvert<T>(FCPDoResult<T> doResult, FCPActionResultType resultType)
                => doResult.isSuc && resultType == FCPActionResultType.created;
        }

        /// <summary>
        /// NoContent Converter
        /// </summary>
        private class FCPDoResultNoContentConverter : FCPDoResultConverter
        {
            internal override IActionResult Convert<T>(FCPDoResult<T> doResult, ActionContext context)
            {                
                return new NoContentResult();
            }

            protected override bool CanConvert<T>(FCPDoResult<T> doResult, FCPActionResultType resultType)
                => doResult.isSuc && resultType == FCPActionResultType.noContent;
        }
    }
}
