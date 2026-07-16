using Distags.Contracts;
using Distags.Core.Entities;
using System.Linq.Expressions;

namespace Distags.Core.Mapping;

public class LogicGroupMapper : IEntityMapper<LogicGroup, LogicGroupDto>
{
    public Expression<Func<LogicGroup, LogicGroupDto>> Expression => logicGroup => new LogicGroupDto
    {
        Name = logicGroup.Name,
        AllExclusive = logicGroup.AllExclusive,
    };
}