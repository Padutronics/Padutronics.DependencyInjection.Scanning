namespace Padutronics.DependencyInjection.Scanning;

public interface IConfigurationModule
{
    void Load(IConfigurationBuilder configurationBuilder);
}