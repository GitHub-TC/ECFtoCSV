using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ECFtoCSV
{
    public partial class MainWindow : Window
    {
        private readonly AppSettings _settings;
        private bool _suppressLangChange;

        public MainWindow()
        {
            InitializeComponent();
            _settings = AppSettings.Load();
            LoadSettingsToUi();
        }

        // ── Lokalisierung ──────────────────────────────────────────────────────

        private string Loc(string key) =>
            Application.Current.TryFindResource(key) as string ?? key;

        private void ApplyLanguage(string lang)
        {
            var dict = new ResourceDictionary
            {
                Source = new Uri($"Resources/Strings.{lang}.xaml", UriKind.Relative)
            };
            var old = Application.Current.Resources.MergedDictionaries
                          .FirstOrDefault(d => d.Source?.OriginalString.Contains("/Strings.") == true);
            if (old != null)
                Application.Current.Resources.MergedDictionaries.Remove(old);
            Application.Current.Resources.MergedDictionaries.Add(dict);

            UpdateOutputStates(); // Platzhaltertexte in neuer Sprache setzen
        }

        private void CmbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppressLangChange) return;
            if (CmbLanguage.SelectedItem is ComboBoxItem item && item.Tag is string lang)
                ApplyLanguage(lang);
        }

        // ── Einstellungen ──────────────────────────────────────────────────────

        private void LoadSettingsToUi()
        {
            TxtEcfInput.Text = _settings.EcfInputFile;
            TxtCsvInput.Text = _settings.CsvInputFile;
            TxtPrefix.Text   = _settings.CsvPrefix;

            if (_settings.EcfOverwrite) RbEcfOverwrite.IsChecked = true;
            else { RbEcfSeparate.IsChecked = true; TxtEcfOutput.Text = _settings.EcfOutputFile; }

            if (_settings.CsvOverwrite) RbCsvOverwrite.IsChecked = true;
            else { RbCsvSeparate.IsChecked = true; TxtCsvOutput.Text = _settings.CsvOutputFile; }

            // Sprach-ComboBox setzen ohne das SelectionChanged-Event auszulösen
            _suppressLangChange = true;
            foreach (ComboBoxItem item in CmbLanguage.Items)
                if (item.Tag as string == _settings.Language)
                    { CmbLanguage.SelectedItem = item; break; }
            _suppressLangChange = false;

            ApplyLanguage(_settings.Language);
        }

        private void SaveSettingsFromUi()
        {
            _settings.EcfInputFile  = TxtEcfInput.Text;
            _settings.EcfOverwrite  = RbEcfOverwrite.IsChecked == true;
            _settings.EcfOutputFile = _settings.EcfOverwrite ? string.Empty : TxtEcfOutput.Text;
            _settings.CsvInputFile  = TxtCsvInput.Text;
            _settings.CsvOverwrite  = RbCsvOverwrite.IsChecked == true;
            _settings.CsvOutputFile = _settings.CsvOverwrite ? string.Empty : TxtCsvOutput.Text;
            _settings.CsvPrefix     = TxtPrefix.Text;
            _settings.Language      = (CmbLanguage.SelectedItem as ComboBoxItem)?.Tag as string ?? "de";
            _settings.Save();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            SaveSettingsFromUi();
            base.OnClosing(e);
        }

        // ── Ausgabe-Felder ─────────────────────────────────────────────────────

        private void UpdateOutputStates()
        {
            if (TxtEcfOutput == null || TxtCsvOutput == null) return;

            var ph = Loc("str_placeholder");

            bool ecfSep = RbEcfSeparate.IsChecked == true;
            TxtEcfOutput.IsEnabled       = ecfSep;
            BtnBrowseEcfOutput.IsEnabled = ecfSep;
            if (!ecfSep)
            {
                TxtEcfOutput.Text       = ph;
                TxtEcfOutput.Foreground = Brushes.Gray;
            }
            else
            {
                TxtEcfOutput.Foreground = Brushes.Black;
                if (string.IsNullOrWhiteSpace(TxtEcfOutput.Text) || TxtEcfOutput.Text == ph)
                    TxtEcfOutput.Text = string.Empty;
            }

            bool csvSep = RbCsvSeparate.IsChecked == true;
            TxtCsvOutput.IsEnabled       = csvSep;
            BtnBrowseCsvOutput.IsEnabled = csvSep;
            if (!csvSep)
            {
                TxtCsvOutput.Text       = ph;
                TxtCsvOutput.Foreground = Brushes.Gray;
            }
            else
            {
                TxtCsvOutput.Foreground = Brushes.Black;
                if (string.IsNullOrWhiteSpace(TxtCsvOutput.Text) || TxtCsvOutput.Text == ph)
                    TxtCsvOutput.Text = string.Empty;
            }
        }

        private void EcfOutOption_Changed(object sender, RoutedEventArgs e) => UpdateOutputStates();
        private void CsvOutOption_Changed(object sender, RoutedEventArgs e) => UpdateOutputStates();

        // ── Datei-Dialoge ──────────────────────────────────────────────────────

        private void BrowseEcfInput_Click(object sender, RoutedEventArgs e)
        {
            var p = OpenFile(Loc("str_dlg_ecf_input"), Loc("str_filter_ecf"), TxtEcfInput.Text);
            if (p != null) TxtEcfInput.Text = p;
        }

        private void BrowseEcfOutput_Click(object sender, RoutedEventArgs e)
        {
            var p = SaveFile(Loc("str_dlg_ecf_output"), Loc("str_filter_ecf"), TxtEcfInput.Text);
            if (p != null) TxtEcfOutput.Text = p;
        }

        private void BrowseCsvInput_Click(object sender, RoutedEventArgs e)
        {
            var p = OpenFile(Loc("str_dlg_csv_input"), Loc("str_filter_csv"), TxtCsvInput.Text);
            if (p != null) TxtCsvInput.Text = p;
        }

        private void BrowseCsvOutput_Click(object sender, RoutedEventArgs e)
        {
            var p = SaveFile(Loc("str_dlg_csv_output"), Loc("str_filter_csv"), TxtCsvInput.Text);
            if (p != null) TxtCsvOutput.Text = p;
        }

        private string? OpenFile(string title, string filter, string? currentPath)
        {
            var dlg = new OpenFileDialog { Title = title, Filter = filter, CheckFileExists = false };
            if (!string.IsNullOrWhiteSpace(currentPath))
                dlg.InitialDirectory = Path.GetDirectoryName(currentPath);
            return dlg.ShowDialog() == true ? dlg.FileName : null;
        }

        private string? SaveFile(string title, string filter, string? referencePath)
        {
            var dlg = new SaveFileDialog { Title = title, Filter = filter };
            if (!string.IsNullOrWhiteSpace(referencePath))
            {
                dlg.InitialDirectory = Path.GetDirectoryName(referencePath);
                dlg.FileName = Path.GetFileNameWithoutExtension(referencePath) + "_out" + Path.GetExtension(referencePath);
            }
            return dlg.ShowDialog() == true ? dlg.FileName : null;
        }

        // ── Konvertierung ──────────────────────────────────────────────────────

        private async void BtnConvert_Click(object sender, RoutedEventArgs e)
        {
            TxtLog.Clear();

            if (string.IsNullOrWhiteSpace(TxtEcfInput.Text))
                { Log(Loc("str_err_no_ecf_input")); return; }
            if (!File.Exists(TxtEcfInput.Text))
                { Log($"{Loc("str_err_ecf_not_found")}\n   {TxtEcfInput.Text}"); return; }
            if (string.IsNullOrWhiteSpace(TxtCsvInput.Text))
                { Log(Loc("str_err_no_csv")); return; }

            var ecfOut = RbEcfOverwrite.IsChecked == true ? TxtEcfInput.Text : TxtEcfOutput.Text.Trim();
            var csvOut = RbCsvOverwrite.IsChecked == true ? TxtCsvInput.Text : TxtCsvOutput.Text.Trim();

            if (string.IsNullOrWhiteSpace(ecfOut))
                { Log(Loc("str_err_no_ecf_out")); return; }
            if (string.IsNullOrWhiteSpace(csvOut))
                { Log(Loc("str_err_no_csv_out")); return; }

            SaveSettingsFromUi();
            BtnConvert.IsEnabled = false;
            try
            {
                var ecfIn  = TxtEcfInput.Text;
                var csvIn  = TxtCsvInput.Text;
                var prefix = TxtPrefix.Text;

                Log(Loc("str_log_starting"));
                Log(Loc("str_log_ecf_in")  + ecfIn);
                Log(Loc("str_log_ecf_out") + ecfOut);
                Log(Loc("str_log_csv_in")  + csvIn);
                Log(Loc("str_log_csv_out") + csvOut);
                Log(Loc("str_log_prefix")  + prefix);
                Log(string.Empty);

                var (newEntries, total) = await Task.Run(() =>
                    EcfConverter.Convert(ecfIn, ecfOut, csvIn, csvOut, prefix));

                Log(string.Format(Loc("str_log_done_fmt"), newEntries, total));
            }
            catch (Exception ex)
            {
                Log($"{Loc("str_err_generic")} {ex.Message}");
            }
            finally
            {
                BtnConvert.IsEnabled = true;
            }
        }

        private void Log(string message)
        {
            TxtLog.AppendText(message + Environment.NewLine);
            TxtLog.ScrollToEnd();
        }
    }
}