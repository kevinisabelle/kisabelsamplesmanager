using KIsabelSampleLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KIsabelSampleLibrary.Controls
{
    /// <summary>
    /// Logique d'interaction pour SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        SettingsService settings { get; set; }
        MainWindow mainWindow { get; set; }
        
        public SettingsWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            settings = App.Services.Settings();
            SamplesPaths.Text = string.Join('\n', settings.Settings.SamplePaths);
        }

        private void UpdateSettingsFromUIAndSave()
        {
            settings.Settings.SamplePaths = SamplesPaths.Text.Split("\n").ToList();
            settings.SaveSettings();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateSettingsFromUIAndSave();
            mainWindow.RefreshUI();
            this.Hide();
        }
    }
}
