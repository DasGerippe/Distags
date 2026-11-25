using System.Text.RegularExpressions;

namespace AutoTagging.Event.Collection;

public class CollectionNameEventSource
{
    private readonly Regex _CollectionNameRegex;
    private readonly string _TagNameTemplate;

    public string CollectionSearchText { get; private set; }

    public CollectionNameEventSource(string collectionSearchText, string collectionNameRegex, string tagNameTemplate)
    {
        CollectionSearchText = collectionSearchText;
        _CollectionNameRegex = new Regex(collectionNameRegex);
        _TagNameTemplate = tagNameTemplate;
    }

    public string? GetEventTagName(string collectionName)
    {
        Match match = _CollectionNameRegex.Match(collectionName);

        string[] tagNameArgs = match.Groups.Values.Skip(1).Select(group => group.Value).ToArray();

        return match.Success
            ? string.Format(_TagNameTemplate, tagNameArgs)
            : null;
    }
}
