using System.Text.RegularExpressions;

namespace AutoTagging.MissingLevel;

public class DistanceFileNameValidator
{
    public static string GetValidFileName(string dirtyFileName, string replaceInvalidCharsWith = "")
    {
        return Regex.Replace(dirtyFileName, "[^\\w\\s\\.]", replaceInvalidCharsWith, RegexOptions.None);
    }

    public static bool IsValidFileName(string filename)
    {
        if (string.IsNullOrEmpty(filename) || !(filename == DistanceFileNameValidator.GetValidFileName(filename, string.Empty)))
            return false;
        return filename.Split(new char[1] { ' ' }).Length - 1 != filename.Length;
    }
}
