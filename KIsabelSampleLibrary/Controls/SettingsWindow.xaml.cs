using KIsabelSampleLibrary.Entity;
using KIsabelSampleLibrary.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SettingsWindow));

        SettingsService settings { get; set; }
        MainWindow mainWindow { get; set; }
        List<SamplesFolder> folders { get; set; }
        
        public SettingsWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            settings = App.Services.Settings();

            RefreshUIFromModel();

            
            RefreshFolders();
        }

        private void RefreshUIFromModel()
        {
            folders = App.Services.Samples().GetFolders();
            CboAudioDeviceType.SelectedValue = settings.Settings.AudioDriver.GetHashCode();
            RefreshDevicesValues();
        }



        private void RefreshDevicesValues()
        {
            settings.Settings.AudioDriver = (AudioDriverType)((KeyValuePair<int, string>)CboAudioDeviceType.SelectedItem).Key;
            CboDeviceId.ItemsSource = AudioService.GetAvailableInterfaces(settings.Settings.AudioDriver);
            
        }

        private void UpdateSettingsFromUIAndSave()
        {
            foreach (var folder in folders)
            {
                folder.BasePath = PathHelper.SanitizeBaseFolderPath(folder.BasePath);
                App.Services.Db().SamplesFolders.Update(folder);
                App.Services.Db().SaveChanges();
            }

            settings.SaveSettings();

        }


        private void CboAudioDeviceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshDevicesValues();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateSettingsFromUIAndSave();
            mainWindow.RefreshUI();
            this.Hide();
        }

        private void BtnAddFolder_Click(object sender, RoutedEventArgs e)
        {
            SamplesFolder newFolder = new SamplesFolder()
            {
                
            };

            App.Services.Db().SamplesFolders.Add(newFolder);
            App.Services.Db().SaveChanges();

            Panel folderPanel = CreateLibFolderPanel(newFolder);

            SamplesFoldersPanel.Children.Add(folderPanel);
        }

        private void RefreshFolders()
        {
            SamplesFoldersPanel.Children.Clear();

            foreach (var folder in folders)
            {
                SamplesFoldersPanel.Children.Add(CreateLibFolderPanel(folder));
            }
        }

        private Panel CreateLibFolderPanel(SamplesFolder folder)
        {
            Grid masterPanel = new Grid();
            masterPanel.Margin = new Thickness(5, 5, 5, 5);

            masterPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100) });
            masterPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            masterPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            masterPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            masterPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            Label LblBasePath = new Label();
            LblBasePath.Content = folder.Id + " - Base Path";
            masterPanel.Children.Add(LblBasePath);

            TextBox txtBasePath = new TextBox();
            txtBasePath.Name = "txtBasePath";

            masterPanel.Children.Add(txtBasePath);

            Label LblName = new Label();
            LblName.Content = "Name";
            masterPanel.Children.Add(LblName);

            TextBox txtName = new TextBox();
            txtName.Name = "txtName";
            txtName.Text = folder.Name;

            masterPanel.Children.Add(txtName);

            Label lblNote = new Label();
            lblNote.Content = "Note";
            masterPanel.Children.Add(lblNote);

            TextBox txtNote = new TextBox();
            txtNote.Name = "txtNote";
            txtNote.Text = folder.Notes;
            masterPanel.Children.Add(txtNote);

            // Bindings
            Binding pathBinding = new Binding("BasePath");
            pathBinding.Source = folder;
            txtBasePath.SetBinding(TextBox.TextProperty, pathBinding);

            Binding nameBinding = new Binding("Name");
            nameBinding.Source = folder;
            txtName.SetBinding(TextBox.TextProperty, nameBinding);

            Binding noteBinding = new Binding("Notes");
            noteBinding.Source = folder;
            txtNote.SetBinding(TextBox.TextProperty, noteBinding);

            // Set positions in grid
            Grid.SetColumn(LblBasePath, 0);
            Grid.SetColumn(LblName, 0);
            Grid.SetColumn(lblNote, 0);

            Grid.SetColumn(txtBasePath, 1);
            Grid.SetColumn(txtName, 1);
            Grid.SetColumn(txtNote, 1);

            Grid.SetRow(LblName, 1);
            Grid.SetRow(txtName, 1);

            Grid.SetRow(lblNote, 2);
            Grid.SetRow(txtNote, 2);

            return masterPanel;
        }

    }
}
