using System;

namespace ToleLibraries.ApiBus.Exceptions
{
    public class ParameterCreationExcepcion : Exception
    {
        public ParameterCreationExcepcion(Type serviceType, string methodName, string parameters, Exception inner)
            : base($"Error al crear parametros para el método {methodName} en el servicio {serviceType.FullName}. Parametros: {parameters}", inner)
        {
        }
    }
}
