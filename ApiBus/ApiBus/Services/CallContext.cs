using System;
using ToleLibraries.ApplicationContext;

namespace ToleLibraries.ApiBus.Services
{
    public class CallContext
    {
        public CallContext(IApplicationContext appContext, ServiceDescriptor service, object serviceObject, string methodName, dynamic arguments)
        {
            AppContext= appContext;
            Arguments = arguments;
            MethodName = methodName;
            Service = service;
            ServiceObject = serviceObject;
        }

        public IApplicationContext AppContext { get; }
        public dynamic Arguments { get; }
        public string MethodName { get; }
        public ServiceDescriptor Service { get; }
        public object ServiceObject { get; }
        public dynamic Result { get; set; }
        public bool IsVoidMethod { get; set; }
        public Exception Exception { get; set; }
    }
}
