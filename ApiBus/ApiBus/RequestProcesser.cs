using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ToleLibraries.ApiBus.Exceptions;
using ToleLibraries.ApiBus.Services;
using ToleLibraries.ApiBus.Requests;

namespace ToleLibraries.ApiBus
{
    public static class RequestProcesser
    {
        public static string StartingPathSegment { get; set; } = "api";

        public static async Task<WebResponse> GetResponse(this ApiBusService apiBusService, WebRequest request)
        {
            var segments = request?.Url?.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            // Thinking on middelware, if path not match with StartingPathSement return null and no process the request.
            if (segments == null || segments.Length < 3 && string.Compare(segments[0], StartingPathSegment, StringComparison.CurrentCultureIgnoreCase) != 0)
                return null;

            //if (segments == null || segments.Length < 3)
            //{
            //    throw new ArgumentException("Request path have to contain 3 segments, apibus starting segment, service name and method name.", nameof(request.Url));
            //}

            var serviceName = segments[1];
            var methodName = segments[2];
            var arguments = request.Body == null
                    ? null
                    : JObject.Parse(request.Body);
            int? statusCode = null;
            CallContext callContext;

            try
            {
                callContext = await apiBusService.CallServiceMethod(serviceName, methodName, arguments);
            }
            catch (AccessDeniedException ex)
            {
                return WebResponse.AccessDenied(ex);
            }
            catch (MethodLocatorExcepcion ex)
            {
                return WebResponse.NotFound(ex);
            }
            catch (Exception ex)
            {
                return WebResponse.Error(ex);
            }

            if (callContext.IsVoidMethod)
            {
                 statusCode = 204;
            }

            return new WebResponse(JsonConvert.SerializeObject(callContext.Result), statusCode);
        }
    }
}
