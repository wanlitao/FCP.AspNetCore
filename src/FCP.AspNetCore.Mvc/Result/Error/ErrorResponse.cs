using FCP.Util;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// error response data
    /// </summary>
    public class ErrorResponse
    {
        private const string BadRequest_DefaultMessage = "The request is invalid";
        private const string ModelError_DefaultMessage = "The value is invalid";

        #region Constructors
        public ErrorResponse(string message)
        {
            if (message.isNullOrEmpty())
                throw new ArgumentNullException(nameof(message));

            Message = message;
        }

        public ErrorResponse(ModelStateDictionary modelState)
        {
            if (modelState == null)
                throw new ArgumentNullException(nameof(modelState));

            if (modelState.IsValid)
                throw new ArgumentException("The model state is valid", nameof(modelState));            

            Errors = new Dictionary<string, object>();
            AddModelErrors(modelState);

            Message = BadRequest_DefaultMessage;
        }
        #endregion

        #region Helper Functions
        private void AddModelErrors(ModelStateDictionary modelState)
        {
            foreach (var keyModelStatePair in modelState)
            {
                var key = keyModelStatePair.Key;
                var errors = keyModelStatePair.Value.Errors;
                if (errors.isNotEmpty())
                {
                    var errorMessages = errors.Select(error =>
                    {
                        return string.IsNullOrEmpty(error.ErrorMessage) ?
                            ModelError_DefaultMessage : error.ErrorMessage;
                    }).ToArray();

                    Errors.Add(key, errorMessages);
                }
            }
        }
        #endregion

        public string Message { get; }

        public IDictionary<string, object> Errors { get; }
    }
}
