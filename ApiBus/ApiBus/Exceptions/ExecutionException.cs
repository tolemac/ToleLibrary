using System;

namespace ToleLibraries.ApiBus.Exceptions
{
    public class ExecutionException : Exception
    {
        public ExecutionException(string serviceName, Type serviceType, Exception inner)
            : base($"Error in method call execution \nService type: {serviceType.FullName}. \nName: {serviceName}", inner)
        {
        }
    }
}