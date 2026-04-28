using DataClasses;
using Distags.Core.Dtos;
using System.Linq.Expressions;

namespace Distags.Core.Projection;

public class AppProjector : IEntityProjector<App, AppDto>
{
    public Expression<Func<App, AppDto>> Projection => app => new AppDto
    {
        Name = app.Name,
        ApiKey = app.ApiKey,
    };
}