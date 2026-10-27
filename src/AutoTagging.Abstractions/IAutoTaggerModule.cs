using Microsoft.Extensions.DependencyInjection;

namespace AutoTagging.Abstractions;

public interface IAutoTaggerModule
{
    void ConfigureServices(IServiceCollection services);
}
