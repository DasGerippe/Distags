using Distags.Contracts;
using Distags.Core.Entities;
using System.Linq.Expressions;

namespace Distags.Core.Mapping;

public class DisplayGroupMapper : IEntityMapper<DisplayGroup, DisplayGroupDto>
{
    public Expression<Func<DisplayGroup, DisplayGroupDto>> Expression => displayGroup => new DisplayGroupDto
    {
        Name = displayGroup.Name,
        DisplayName = displayGroup.DisplayName,
        Description = displayGroup.Description,
        Position = displayGroup.Position,
    };
}