using NUnit.Framework;
using Padutronics.DependencyInjection.Scanning.TestAssembly.Conventions.RegisterConcreteTypesAgainstAllInterfaces;

namespace Padutronics.DependencyInjection.Scanning.Tests.Conventions;

[TestFixture]
internal sealed class Register_concrete_types_against_all_interfaces_when_type_implements_multiple_interfaces
{
    [Test]
    public void Instance_is_resolved_if_service_is_resolved_by_first_interface()
    {
        // Arrange.
        var containerBuilder = new ContainerBuilder();
        containerBuilder.Scan(
            scannerConfigurator => scannerConfigurator
                .Assembly(TestAssembly.Name)
                .RegisterConcreteTypesAgainstAllInterfaces()
        );

        using IContainer container = containerBuilder.Build();

        // Act.
        IService1 service = container.Resolve<IService1>();

        // Assert.
        Assert.That(service, Is.TypeOf<Service>());
    }

    [Test]
    public void Instance_is_resolved_if_service_is_resolved_by_second_interface()
    {
        // Arrange.
        var containerBuilder = new ContainerBuilder();
        containerBuilder.Scan(
            scannerConfigurator => scannerConfigurator
                .Assembly(TestAssembly.Name)
                .RegisterConcreteTypesAgainstAllInterfaces()
        );

        using IContainer container = containerBuilder.Build();

        // Act.
        IService2 service = container.Resolve<IService2>();

        // Assert.
        Assert.That(service, Is.TypeOf<Service>());
    }
}