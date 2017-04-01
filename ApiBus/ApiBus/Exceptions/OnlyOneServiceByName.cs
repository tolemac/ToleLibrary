using System;

namespace ToleLibraries.ApiBus.Exceptions
{
    public class OnlyOneServiceByName : Exception
    {
        public OnlyOneServiceByName(string serviceName, Type serviceType, Exception inner)
            : base($"Error adding second service with the same name, only one per name is accepted. \nService type: {serviceType.FullName}. \nName: {serviceName}", inner)
        {
        }
    }
}
