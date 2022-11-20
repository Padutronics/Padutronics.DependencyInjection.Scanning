using NUnit.Framework;
using Padutronics.DependencyInjection.Scanning.TestAssembly.Conventions.RegisterOpenTypesAgainstOpenInterfaces;

namespace Padutronics.DependencyInjection.Scanning.Tests.Conventions;

[TestFixture]
internal sealed class Register_open_types_against_open_interfaces
{
    [Test]
    public void Instance_is_resolved()
    {
        // Arrange.
        var containerBuilder = new ContainerBuilder();
        containerBuilder.Scan(
            scannerConfigurator => scannerConfigurator
                .Assembly(TestAssembly.Name)
                .RegisterOpenTypesAgainstOpenInterfaces()
        );

        using IContainer container = containerBuilder.Build();

        // Act.
        IService<int> service = container.Resolve<IService<int>>();

        // Assert.
        Assert.That(service, Is.TypeOf<Service<int>>());
    }
}