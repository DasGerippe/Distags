using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DistagsShortcut
{
    public static class SlugHelper
    {
        public static string CreateSlug(string input)
        {
            string normalizedInputFormD = input.Normalize(NormalizationForm.FormD);

            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in normalizedInputFormD)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            string withoutDiacritics = stringBuilder.ToString();
            string lower = withoutDiacritics.ToLowerInvariant();
            string hyphenated = Regex.Replace(lower, "[^a-z0-9]+", "-");
            string collapsed = Regex.Replace(hyphenated, "-{2,}", "-").Trim('-');
            return collapsed;
        }
    }
}
