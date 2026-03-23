using System.Text.Json.Serialization;

namespace LevelTagApi.Dtos;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(ContributorUserDto), "user")]
[JsonDerivedType(typeof(ContributorAppDto), "app")]
public abstract class ContributorDto
{
}