using NUnit.Framework;
using Padutronics.DependencyInjection.Scanning.TestAssembly.Conventions.RegisterConcreteTypesAgainstSelf;

namespace Padutronics.DependencyInjection.Scanning.Tests.Conventions;

[TestFixture]
internal sealed class Register_concrete_types_against_self
{
    [Test]
    public void Instance_is_resolved()
    {
        // Arrange.
        var containerBuilder = new ContainerBuilder();
        containerBuilder.Scan(
            scannerConfigurator => scannerConfigurator
                .Assembly(TestAssembly.Name)
                .RegisterConcreteTypesAgainstSelf()
        );

        using IContainer container = containerBuilder.Build();

        // Act.
        Service service = container.Resolve<Service>();

        // Assert.
        Assert.That(service, Is.Not.Null);
    }
}