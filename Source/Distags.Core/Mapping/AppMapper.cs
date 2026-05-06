using DataClasses;
using Distags.Contracts;
using System.Linq.Expressions;

namespace Distags.Core.Mapping;

public class AppMapper : IEntityMapper<App, AppDto>
{
    public Expression<Func<App, AppDto>> Expression => app => new AppDto
    {
        Name = app.Name,
        ApiKey = app.ApiKey,
    };
}