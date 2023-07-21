using System.Net;
using Skybrud.Social.Google.MyBusiness.Exceptions;
using Skybrud.Social.Http;
using Skybrud.Social.Json;

namespace Skybrud.Social.Google.MyBusiness.Responses {

    /// <summary>
    /// Class representing a response from the YouTube API.
    /// </summary>
    public class MyBusinessResponse : SocialResponse {

        #region Constructors        

        protected MyBusinessResponse(SocialHttpResponse response) : base(response) { }

        #endregion

        #region Static methodssponse = response;

        /// <summary>
        /// Validates the specified <code>response</code>.
        /// </summary>
        /// <param name="response">The response to be validated.</param>
        /// <param name="obj">The object representing the response object.</param>
        public static void ValidateResponse(SocialHttpResponse response, JsonObject obj) {

            // Skip error checking if the server responds with an OK status code
            if (response.StatusCode == HttpStatusCode.OK) return;
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {

                JsonObject error = obj.GetObject("error");

                int code = error.GetInt32("code");
                string message = error.GetString("message");
                message += ". ";

                //Parse "details"
                var details = error.GetArray("details");
                foreach (var detail in details.InternalArray)
                {
                    var detailObject = detail as JsonObject;
                    if (detailObject != null)
                    {
                        var fieldViolations = detailObject.GetArray("fieldViolations");
                        foreach (var field in fieldViolations.InternalArray)
                        {
                            var fieldObject = field as JsonObject;
                            if (fieldObject != null)
                            {
                                message += $"fieldViolation: [{fieldObject.GetString("field")}] {fieldObject.GetString("description")}";
                            }
                        }
                    }
                }

                throw new MyBusinessException(response, code, message);
            }
            else {
                throw new MyBusinessException(response, (int)response.StatusCode, response.Body);
            }

        }

        #endregion

    }

    /// <summary>
    /// Class representing a response from the YouTube API.
    /// </summary>
    public class MyBusinessResponse<T> : MyBusinessResponse
    {

        /// <summary>
        /// Gets the body of the response.
        /// </summary>
        public T Body { get; protected set; }        

        protected MyBusinessResponse(SocialHttpResponse response) : base(response) { }

    }

}