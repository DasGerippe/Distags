using System.Linq.Expressions;
using DataClasses;
using Distags.Core.Dtos;

namespace Distags.Core.Mapping;

public class AppMapper : IEntityMapper<App, AppDto>
{
    public Expression<Func<App, AppDto>> Expression => app => new AppDto
    {
        Name = app.Name,
        ApiKey = app.ApiKey,
    };
}