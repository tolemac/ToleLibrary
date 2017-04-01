using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ToleLibraries.ApiBus.Exceptions;
using ToleLibraries.ApiBus.Requests;
using ToleLibraries.ApiBus.Services;
using Xunit;

namespace ToleLibraries.ApiBus.Tests
{
    public class RequestProcessorTests
    {
        private readonly ApiBusService _apiBusService;

        public RequestProcessorTests()
        {
            var dr = new ApiBusDependencyResolverServiceMock();
            _apiBusService = new ApiBusService(dr);

            _apiBusService.AddService(new ServiceDescriptor(typeof(ITestService)), sd => { });
            RequestProcesser.StartingPathSegment = "TestApi";
        }

        [Fact]
        public async Task CallVoidMethodWithoutParams()
        {
            var parameters = JObject.FromObject(new { });
            var body = JsonConvert.SerializeObject(parameters);
            
            var result = await _apiBusService.GetResponse(new WebRequest("TestApi/TestService/VoidMethodWithoutParams", body));
            
            Assert.True(result.StatusCode == 204);
            Assert.Null(JsonConvert.DeserializeObject(result.Body));
        }

        [Fact]
        public async Task CallVoidMethodWithParams()
        {
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = new TestObject(),
                param3 = (string)null
            });
            var body = JsonConvert.SerializeObject(parameters);

            var result = await _apiBusService.GetResponse(new WebRequest("TestApi/TestService/VoidMethodWithParams", body));
            Assert.True(result.StatusCode == 204);
            Assert.True(result.Body == JsonConvert.SerializeObject(null));
        }

        [Fact]
        public async Task CallVoidMethodWithDefaultParams()
        {
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = new TestObject()
            });
            var body = JsonConvert.SerializeObject(parameters);

            var result = await _apiBusService.GetResponse(new WebRequest("TestApi/TestService/VoidMethodWithParams", body));
            Assert.True(result.StatusCode == 204);
            Assert.True(result.Body == JsonConvert.SerializeObject(null));
        }

        [Fact]
        public async Task CallMethodCrashIfMethodNotExistsOrNoParameterMatchs()
        {
            var parameters = JObject.FromObject(new
            {
                param1 = ""
            });
            var body = JsonConvert.SerializeObject(parameters);

            var result = await _apiBusService.GetResponse(new WebRequest("TestApi/TestService/VoidMethodWithParams", body));
            Assert.True(result.StatusCode == 404);
            Assert.True(result.Exception.GetType() == typeof(MethodLocatorExcepcion));
            
            result = await _apiBusService.GetResponse(new WebRequest("TestApi/TestService/NOTEXISTS-METHOD", body));
            Assert.True(result.StatusCode == 404);
            Assert.True(result.Exception.GetType() == typeof(MethodLocatorExcepcion));
        }

        [Fact]
        public async Task CallMethodCrashIfCantBuildMethodParameters()
        {
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = "Type Mistmatch"
            });
            var body = JsonConvert.SerializeObject(parameters);

            var result = await _apiBusService.GetResponse(new WebRequest("TestApi/TestService/VoidMethodWithParams", body));

            Assert.True(result.StatusCode == 500);
            Assert.True(result.Exception.GetType() == typeof(ParameterCreationExcepcion));
        }

        [Fact]
        public async Task CallMethodWithoutParams()
        {
            var parameters = JObject.FromObject(new { });
            var body = JsonConvert.SerializeObject(parameters);

            var result = await _apiBusService.GetResponse(new WebRequest("TestApi/TestService/MethodWithoutParams", body));

            Assert.False(result.StatusCode == 204);
            Assert.Equal(JsonConvert.SerializeObject(TestService.NoParamsResult), result.Body);
        }

        [Fact]
        public async Task CallMethodWithParams()
        {
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = TestService.NoParamsResult
            });
            var body = JsonConvert.SerializeObject(parameters);

            var result = await _apiBusService.GetResponse(new WebRequest("TestApi/TestService/MethodWithParams", body));

            Assert.False(result.StatusCode == 204);
            Assert.Equal(JsonConvert.SerializeObject(TestService.NoParamsResult), result.Body);
        }

        [Fact]
        public async Task CallAsyncVoidMethodWithParams()
        {
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = new TestObject(),
                param3 = (string)null
            });
            var body = JsonConvert.SerializeObject(parameters);

            var result = await _apiBusService.GetResponse(new WebRequest("TestApi/TestService/AsyncVoidMethodWithParams", body));

            Assert.True(result.StatusCode == 204);
            Assert.True(result.Body == JsonConvert.SerializeObject(null));
        }

        [Fact]
        public async Task CallAsyncMethodWithParams()
        {
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = TestService.NoParamsResult
            });
            var body = JsonConvert.SerializeObject(parameters);

            var result = await _apiBusService.GetResponse(new WebRequest("TestApi/TestService/AsyncMethodWithParams", body));

            Assert.False(result.StatusCode == 204);
            Assert.Equal(JsonConvert.SerializeObject(TestService.NoParamsResult), result.Body);
        }

        [Fact]
        public async Task CallMethodNotPublishOnInterfaceCrash()
        {
            var parameters = JObject.FromObject(new {});
            var body = JsonConvert.SerializeObject(parameters);

            var result =
                await _apiBusService.GetResponse(new WebRequest("TestApi/TestService/MethodNotPublishOnInterface", body));
            Assert.True(result.StatusCode == 404);
            Assert.True(result.Exception.GetType() == typeof(MethodLocatorExcepcion));
        }

    }
}
