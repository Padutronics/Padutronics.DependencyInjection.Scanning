using NUnit.Framework;
using Padutronics.DependencyInjection.Scanning.TestAssembly.Configure;

namespace Padutronics.DependencyInjection.Scanning.Tests;

[TestFixture]
internal sealed class Configure_scanner
{
    [Test]
    public void The_same_instance_is_resolved_if_service_is_configured_as_singleton()
    {
        // Arrange.
        var containerBuilder = new ContainerBuilder();
        containerBuilder.Scan(
            scannerConfigurator => scannerConfigurator
                .Assembly(TestAssembly.Name)
                .RegisterConcreteTypesAgainstAllInterfaces()
                .Configure<Service>(lifetimeStage => lifetimeStage.SingleInstance())
        );

        using IContainer container = containerBuilder.Build();

        // Act.
        IService1 service1 = container.Resolve<IService1>();
        IService2 service2 = container.Resolve<IService2>();

        // Assert.
        Assert.That(service1, Is.SameAs(service2));
    }
}