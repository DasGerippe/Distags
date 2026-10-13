using System.Collections.Generic;
using System.IO;

namespace OfficialLevelExport
{
    public class OfficialLevelExporter
    {
        public void ExportOfficialLevelsToCsv(List<ScrapedOfficialLevel> officialLevels, string csvFilePath)
        {
            using (StreamWriter fileWriter = new StreamWriter(csvFilePath))
            {
                string header = CreateCsvLine(
                    nameof(ScrapedOfficialLevel.Name),
                    nameof(ScrapedOfficialLevel.CreatorName),
                    nameof(ScrapedOfficialLevel.GameModes));
                fileWriter.WriteLine(header);

                foreach (ScrapedOfficialLevel scrapedOfficialLevel in officialLevels)
                {
                    string levelLine = CreateCsvLine(
                        scrapedOfficialLevel.Name,
                        scrapedOfficialLevel.CreatorName,
                        string.Join(",", scrapedOfficialLevel.GameModes));
                    fileWriter.WriteLine(levelLine);
                }
            }
        }

        private string CreateCsvLine(params string[] strings)
        {
            return string.Join(";", strings);
        }
    }
}
