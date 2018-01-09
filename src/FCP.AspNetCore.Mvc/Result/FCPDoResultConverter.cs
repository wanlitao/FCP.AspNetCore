using FCP.Core;
using FCP.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Mvc
{
    internal abstract class FCPDoResultConverter
    {
        private static readonly IDictionary<string, FCPDoResultConverter> converterDic =
            new Dictionary<string, FCPDoResultConverter>()
        {
            { FCPDoResultType.success.ToString(), new FCPDoResultSuccessConverter() },
            { FCPDoResultType.fail.ToString(), new FCPDoResultFailConverter() },
            { FCPDoResultType.validFail.ToString(), new FCPDoResultValidFailConverter() },
            { FCPDoResultType.notFound.ToString(), new FCPDoResultNotFoundConverter() },
            { FCPDoResultType.unauthorized.ToString(), new FCPDoResultUnauthorizedConverter() }
        };

        internal abstract ActionResult Convert<T>(FCPDoResult<T> doResult);

        internal static FCPDoResultConverter GetConverter<T>(FCPDoResult<T> doResult)
        {
            if (doResult == null)
                throw new ArgumentNullException(nameof(doResult));

            return converterDic[doResult.type];
        }

        /// <summary>
        /// Success Converter
        /// </summary>
        private class FCPDoResultSuccessConverter : FCPDoResultConverter
        {
            internal override ActionResult Convert<T>(FCPDoResult<T> doResult)
            {
                return new OkObjectResult(doResult.data);
            }
        }

        /// <summary>
        /// Fail Converter
        /// </summary>
        private class FCPDoResultFailConverter : FCPDoResultConverter
        {
            internal override ActionResult Convert<T>(FCPDoResult<T> doResult)
            {
                return new ObjectResult(doResult.msg)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        /// <summary>
        /// ValidFail Converter
        /// </summary>
        private class FCPDoResultValidFailConverter : FCPDoResultConverter
        {
            internal override ActionResult Convert<T>(FCPDoResult<T> doResult)
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
        }

        /// <summary>
        /// NotFound Converter
        /// </summary>
        private class FCPDoResultNotFoundConverter : FCPDoResultConverter
        {
            internal override ActionResult Convert<T>(FCPDoResult<T> doResult)
            {
                return new NotFoundObjectResult(doResult.msg);
            }
        }

        /// <summary>
        /// Unauthorized Converter
        /// </summary>
        private class FCPDoResultUnauthorizedConverter : FCPDoResultConverter
        {
            internal override ActionResult Convert<T>(FCPDoResult<T> doResult)
            {
                return new ObjectResult(doResult.msg)
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
        }
    }
}
