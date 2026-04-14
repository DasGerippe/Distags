using DataClasses;
using LevelTagApi.Dtos.Contribution;

namespace LevelTagApi.Conversion;

public class ContributionDtoConverterFactory : IDtoConverterFactory<Contribution, ContributionDto, ContributionConversionParameters>
{
    public IDtoConverter<Contribution, ContributionDto> GetDtoConverter(ContributionConversionParameters conversionParams)
    {
        return new ContributionDtoConverter(conversionParams);
    }
}