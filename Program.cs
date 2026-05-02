using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ECFtoCSV
{
    public static class EcfConverter
    {
        /// <summary>
        /// Verarbeitet eine ECF-Datei: ersetzt Dialogtexte durch Schlüssel und aktualisiert die CSV-Übersetzungsdatei.
        /// Erkannte Zeilentypen: Output:, Option_*, Description:, SellingText:
        /// </summary>
        /// <returns>Anzahl neu hinzugefügter Einträge und Gesamtanzahl.</returns>
        public static (int newEntries, int totalEntries) Convert(
            string ecfFile, string ecfOutFile,
            string csvFile, string csvOutFile,
            string csvPrefix)
        {
            var ecfLines = File.ReadAllLines(ecfFile);
            var csvLines = File.Exists(csvFile) && new FileInfo(csvFile).Length > 0
                ? CsvIO.ReadTranslationFromCsv(csvFile)
                : new List<List<string>>();

            int initialCount = csvLines.Count;
            int currentBlockHash = 0;

            for (int i = 0; i < ecfLines.Length; i++)
            {
                var trimLine = ecfLines[i].Trim();

                // Jeden Block-Header als Kontext für den Hash verwenden
                if (trimLine.StartsWith("{"))
                    currentBlockHash = ecfLines[i].GetHashCode();
                else if (trimLine.StartsWith("Option_")     ||
                         trimLine.StartsWith("Output:")     ||
                         trimLine.StartsWith("Description:") ||
                         trimLine.StartsWith("SellingText:"))
                {
                    var textStartPos = ecfLines[i].IndexOf('\"', ecfLines[i].IndexOf(':'));
                    var textEndPos   = ecfLines[i].LastIndexOf('\"');

                    if (textStartPos > 0 && textEndPos > textStartPos)
                    {
                        var text = ecfLines[i].Substring(textStartPos + 1, textEndPos - textStartPos - 1);

                        if (csvLines.All(l => l.FirstOrDefault() != text))
                        {
                            var placeholder = (csvPrefix ?? "txt_") + (currentBlockHash + text.GetHashCode()).ToString("X");
                            var found = csvLines.FirstOrDefault(test => test.Count > 1 && test[1] == text);

                            if (found != null) placeholder = found[0];
                            else               csvLines.Add(new List<string> { placeholder, text });

                            ecfLines[i] = ecfLines[i].Substring(0, textStartPos) + placeholder;
                        }
                    }
                }
            }

            File.WriteAllLines(ecfOutFile, ecfLines);
            CsvIO.WriteTranslationToCsv(csvLines, csvOutFile);

            return (csvLines.Count - initialCount, csvLines.Count);
        }
    }
}
