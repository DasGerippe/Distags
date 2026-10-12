namespace AutoTagging.Abstractions.TaggingStrategy;

public static class TaggingStrategyExtensions
{
    public static Type? GetProgressType(this ITaggingStrategy taggingStrategy)
    {
        Type? typedStrategyInterface = taggingStrategy.GetType()
            .GetInterfaces()
            .FirstOrDefault(@interface =>
                @interface.IsGenericType &&
                @interface.GetGenericTypeDefinition() == typeof(ITaggingStrategy<>));

        return typedStrategyInterface?.GetGenericArguments()[0];
    }
}
