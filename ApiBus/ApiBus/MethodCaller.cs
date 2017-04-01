using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ToleLibraries.ApiBus.Exceptions;

namespace ToleLibraries.ApiBus
{
    public interface IMethodCaller<TResult>
    {
        /// <summary>
        /// Llama a un metodo especifico de un servicio pasando los parametros en un objeto JObject
        /// </summary>
        /// <param name="publicInterface">Tipo donde buscar el método</param>
        /// <param name="srv">Servicio donde llamar el método</param>
        /// <param name="methodName">Nombre del metodo a llamar</param>
        /// <param name="parameters">Parametros del método en un JObject</param>
        /// <returns>El resultado de la llamada al método</returns>
        Task<TResult> Call(Type publicInterface, object srv, string methodName, JObject parameters);
    }

    internal class CallMethodResult
    {
        public object Result { get; }
        public bool IsVoid { get; }

        public CallMethodResult(object result, bool isVoid)
        {
            Result = result;
            IsVoid = isVoid;
        }
    }

    internal class MethodCaller : IMethodCaller<CallMethodResult>
    {
        private bool IsAsyncMethod(MethodInfo method)
        {
            if (method.ReturnType == typeof(Task) ||
                method.ReturnType.GetTypeInfo().IsGenericType &&
                method.ReturnType.GetTypeInfo().BaseType == typeof(Task))
                return true;

            var attType = typeof(AsyncStateMachineAttribute);

            // Obtain the custom attribute for the method. 
            // The value returned contains the StateMachineType property. 
            // Null is returned if the attribute isn't present for the method. 
            var attrib = (AsyncStateMachineAttribute)method.GetCustomAttribute(attType);

            return attrib != null;
        }

        /// <summary>
        /// Llama a un metodo especifico de un servicio pasando los parametros en un objeto JObject
        /// </summary>
        /// <param name="publicInterface">Tipo donde buscar el método</param>
        /// <param name="srv">Servicio donde llamar el método</param>
        /// <param name="methodName">Nombre del metodo a llamar</param>
        /// <param name="parameters">Parametros del método en un JObject</param>
        /// <returns>El resultado de la llamada al método</returns>
        public async Task<CallMethodResult> Call(Type publicInterface, object srv, string methodName, JObject parameters)
        {
            MethodInfo method;
            object[] methodParameters;

            try
            {
                method = LocateMethod(publicInterface, methodName, parameters);
            }
            catch (Exception ex)
            {
                throw new MethodLocatorExcepcion(publicInterface, methodName, parameters.ToString(), ex);
            }

            try
            {
                methodParameters = CreateParameters(method.GetParameters(), parameters);
            }
            catch (Exception ex)
            {

                throw new ParameterCreationExcepcion(publicInterface, methodName, parameters.ToString(), ex);
            }

            var isAsync = IsAsyncMethod(method);
            var isVoidMethod = method.ReturnType == typeof(void) ||
                (method.ReturnType == typeof(Task));

            object result = null;
            try
            {
                if (isVoidMethod)
                {
                    if (isAsync)
                    {
                        await (dynamic) method.Invoke(srv, methodParameters);
                    }
                    else
                        method.Invoke(srv, methodParameters);
                }
                else
                {
                    if (isAsync)
                        result = await (dynamic) method.Invoke(srv, methodParameters);
                    else
                        result = method.Invoke(srv, methodParameters);
                }
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }

            return new CallMethodResult(result, isVoidMethod);
        }

        private object[] CreateParameters(ParameterInfo[] methodParameters, JObject callParameters)
        {
            return callParameters == null
                ? null
                : methodParameters.Select(par =>
                    callParameters[par.Name] == null
                        ? par.DefaultValue
                        : callParameters[par.Name].ToObject(par.ParameterType)).ToArray();
        }

        private static MethodInfo LocateMethod(Type srvInterface, string nombreMetodo, JObject parametros)
        {
            var nombreParametros = parametros?.Properties().Select(prop => prop.Name).ToList() ?? new List<string>();
#if NET45 || NETCOREAPP1_1
            var methods = srvInterface.GetMethods();
#endif
#if NETSTANDARD1_6
            var methods = srvInterface.GetTypeInfo().GetMethods();
#endif

            var metodo = nombreParametros.Count == 0
                ? methods
                    .First(
                        m =>
                            m.Name == nombreMetodo && !m.IsGenericMethod &&
                            (m.GetParameters().Length == 0 ||
                            m.GetParameters().All(p => p.HasDefaultValue)))
                : methods
                    .First(
                        m =>
                            m.Name == nombreMetodo && !m.IsGenericMethod &&
                            m.GetParameters().Where(p => !p.HasDefaultValue).Select(p => p.Name).All(pName => nombreParametros.Contains(pName)));

            return metodo;
        }
    }
}
