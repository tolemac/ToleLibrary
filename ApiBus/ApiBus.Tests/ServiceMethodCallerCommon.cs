using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ToleLibraries.ApiBus.Tests
{
    internal class TestCall
    {
        public Type Type { get; set; }
        public object Srv { get; set; }
        public string MethodName { get; set; }
        public JObject Parameters { get; set; }
    }

    internal class MethodCallerMock : IMethodCaller<CallMethodResult>
    {
        public ConcurrentBag<TestCall> CallList = new ConcurrentBag<TestCall>();

        public async Task<CallMethodResult> Call(Type publicInterface, object srv, string methodName, JObject parameters)
        {
            if (parameters["param1"]?.ToString() == "33")
            {
                throw new Exception("Test exception, param1 is 33.");
            }
            await Task.Run(() =>
            {
                CallList.Add(new TestCall
                {
                    Type = publicInterface,
                    MethodName = methodName,
                    Parameters = parameters,
                    Srv = srv
                });
            });
            return new CallMethodResult(null, true);
        }
    }
}
