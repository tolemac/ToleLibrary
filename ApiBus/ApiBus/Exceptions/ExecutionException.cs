using System;
using ToleLibraries.ApiBus.Services;

namespace ToleLibraries.ApiBus.Exceptions
{
    public class ExecutionException : Exception
    {
        public CallContext Context { get; }

        public ExecutionException(string serviceName, Type serviceType, CallContext context, Exception inner)
            : base($"Error in method call execution \nService type: {serviceType.FullName}. \nName: {serviceName}", inner)
        {
            Context = context;
        }
    }
}