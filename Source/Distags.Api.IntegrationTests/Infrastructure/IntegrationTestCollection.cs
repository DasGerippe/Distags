using Xunit;

namespace Distags.Api.IntegrationTests.Infrastructure;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class IntegrationTestCollection : ICollectionFixture<DatabaseFixture>
{
    public const string Name = "Integration Tests";
}
