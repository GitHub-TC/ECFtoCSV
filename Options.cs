using System;
using System.IO;
using System.Text.Json;

namespace ECFtoCSV
{
    public class AppSettings
    {
        public string EcfInputFile  { get; set; } = string.Empty;
        public string EcfOutputFile { get; set; } = string.Empty;
        public bool   EcfOverwrite  { get; set; } = true;
        public string CsvInputFile  { get; set; } = string.Empty;
        public string CsvOutputFile { get; set; } = string.Empty;
        public bool   CsvOverwrite  { get; set; } = true;
        public string CsvPrefix     { get; set; } = "txt_";
        public string Language      { get; set; } = "de";

        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ECFtoCSV", "settings.json");

        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                    return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(SettingsPath)) ?? new AppSettings();
            }
            catch { }
            return new AppSettings();
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
                File.WriteAllText(SettingsPath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch { }
        }
    }
}
