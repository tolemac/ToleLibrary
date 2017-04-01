using System.Threading.Tasks;
using ToleLibraries.ApiBus.Exceptions;
using ToleLibraries.ApiBus.Services;
using Xunit;

namespace ToleLibraries.ApiBus.Tests
{
    public class ServiceContainerTests
    {
        [Fact]
        public void CanAddServiceToContainerAndMatchsByName()
        {
            var sc = new ServiceContainer();
            var desc1 = new ServiceDescriptor(typeof(ITestService));

            sc.AddService(desc1);
            var desc2 = sc.GetServiceDescriptorByName("TestService");

            Assert.True(desc1 == desc2);
        }

        [Fact]
        public async void CantAddTwoServicesWithTheSameName()
        {
            var sc = new ServiceContainer();
            var desc1 = new ServiceDescriptor(typeof(ITestService));
            var desc2 = new ServiceDescriptor(typeof(TestService));

            sc.AddService(desc1);

            await Assert.ThrowsAsync<OnlyOneServiceByName>(async () =>
            {
                await Task.Run(() => { sc.AddService(desc2); });
            });
        }
    }
}
