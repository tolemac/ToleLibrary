using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ToleLibraries.ApiBus.Exceptions;

namespace ToleLibraries.ApiBus.Requests
{
    public class WebResponse
    {
        public int? StatusCode { get; private set; }
        public string Body { get; }
        public Exception Exception { get; private set; }

        public WebResponse(string body, int? statusCode = null)
        {
            StatusCode = statusCode;
            Body = body;
        }

        public static WebResponse NotFound(MethodLocatorExcepcion ex)
        {
            var result = Error(ex);
            result.StatusCode = 404;
            return result;
        }

        public static WebResponse AccessDenied(AccessDeniedException ex)
        {
            var result = Error(ex);
            result.StatusCode = 401;
            return result;
        }

        public static WebResponse Error(Exception ex)
        {
            var body = JsonConvert.SerializeObject(new
            {
                Error = ex.GetType().Name,
                ex.Message
            });


            return new WebResponse(body, 500)
            {
                Exception = ex
            };
        }
    }
}
