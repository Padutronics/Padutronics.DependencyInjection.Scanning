using NUnit.Framework;
using Padutronics.DependencyInjection.Scanning.TestAssembly.Conventions.RegisterFactories;

namespace Padutronics.DependencyInjection.Scanning.Tests.Conventions;

[TestFixture]
internal sealed class Register_factories
{
    [Test]
    public void Instance_is_resolved()
    {
        // Arrange.
        var containerBuilder = new ContainerBuilder();
        containerBuilder.Scan(
            scannerConfigurator => scannerConfigurator
                .Assembly(TestAssembly.Name)
                .RegisterFactories()
        );

        using IContainer container = containerBuilder.Build();

        // Act.
        IServiceFactory service = container.Resolve<IServiceFactory>();

        // Assert.
        Assert.That(service, Is.Not.Null);
    }
}