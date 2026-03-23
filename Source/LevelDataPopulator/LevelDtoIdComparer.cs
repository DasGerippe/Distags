using LevelTagApi.Dtos;

namespace LevelDataPopulator;

public class LevelDtoIdComparer : IEqualityComparer<PutLevelDto>
{
    public bool Equals(PutLevelDto? x, PutLevelDto? y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (x is null)
            return false;

        if (y is null)
            return false;

        if (x.GetType() != y.GetType())
            return false;

        return x.Id == y.Id;
    }

    public int GetHashCode(PutLevelDto levelDto)
    {
        return levelDto.Id.GetHashCode();
    }
}