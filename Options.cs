using CommandLine;

namespace ECFtoCSV
{
    public class Options
    {
        [Option("ecf-file", Required = true, HelpText = "ECF file")]
        public string EcfFile { get; set; }

        [Option("ecf-out-file", Required = false, HelpText = "ECF output file - if differed from the ecf input")]
        public string EcfOutFile { get; set; }

        [Option("csv-file", Required = true, HelpText = "CSV file")]
        public string CsvFile { get; set; }

        [Option("csv-out-file", Required = false, HelpText = "CSV output file - if differed from the csv input")]
        public string CsvOutFile { get; set; }

        [Option("prefix", Required = false, HelpText = "Prefix for the csv tags")]
        public string CsvPrefix { get; set; }
    }
}
