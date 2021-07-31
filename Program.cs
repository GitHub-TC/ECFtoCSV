using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ECFtoCSV
{
    public class Program
    {
        static void Main(string[] args) => Parser.Default.ParseArguments<Options>(args).WithParsed(Transfer);

        private static void Transfer(Options options)
        {
            var ecfLines = File.ReadAllLines(options.EcfFile);
            var csvLines = CsvIO.ReadTranslationFromCsv(options.CsvFile);

            int currentDialogHash = 0;
            for (int i = 0; i < ecfLines.Length; i++)
            {
                var trimLine = ecfLines[i].Trim();
                if (trimLine.StartsWith("{ +Dialogue Name: ")) currentDialogHash = ecfLines[i].GetHashCode();
                else if(trimLine.StartsWith("Option_") || trimLine.StartsWith("Output:"))
                {
                    var textStartPos = ecfLines[i].IndexOf('\"', ecfLines[i].IndexOf(':'));
                    var textEndPos   = ecfLines[i].LastIndexOf('\"');

                    if (textStartPos > 0 && textEndPos > 0)
                    {
                        var text = ecfLines[i].Substring(textStartPos + 1, textEndPos - textStartPos - 1);
                        var placeholder = (options.CsvPrefix ?? "txt_") + (currentDialogHash + text.GetHashCode()).ToString("X");
                        var found = csvLines.FirstOrDefault(test => test[1] == text);

                        if (found != null) placeholder = found[0];
                        else               csvLines.Add(new List<string> { placeholder, text });

                        ecfLines[i] = ecfLines[i].Substring(0, textStartPos) + placeholder;
                    }
                }
            }

            File.WriteAllLines         (options.EcfOutFile ?? options.EcfFile, ecfLines);
            CsvIO.WriteTranslationToCsv(csvLines, options.CsvOutFile ?? options.CsvFile);
        }
    }
}
