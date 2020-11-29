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
            folders = App.Services.Samples().GetFolders();
            RefreshFolders();
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
            StackPanel panel = new StackPanel();

            Label label = new Label();
            label.Content = folder.Id + " - Base Path";
            panel.Children.Add(label);

            panel.Orientation = Orientation.Vertical;
            panel.Margin = new Thickness(5, 5, 5, 5);
            
          
            Binding pathBinding = new Binding("BasePath");
            pathBinding.Source = folder;

            TextBox txtBasePath = new TextBox();
            txtBasePath.Name = "txtBasePath";
            //txtBasePath.Text = pathBinding;
            txtBasePath.SetBinding(TextBox.TextProperty, pathBinding);

            panel.Children.Add(txtBasePath);

            Label labelName = new Label();
            labelName.Content = "Name";
            panel.Children.Add(labelName);

            TextBox txtName = new TextBox();
            txtName.Name = "txtName";
            txtName.Text = folder.Name;

           
            panel.Children.Add(txtName);

            Label labelNote = new Label();
            labelNote.Content = "Note";
            panel.Children.Add(labelNote);

            TextBox txtNote = new TextBox();
            txtNote.Name = "txtNote";
            txtNote.Text = folder.Notes;
            panel.Children.Add(txtNote);
            
            Separator sep = new Separator();
            panel.Children.Add(sep);

            return panel;
        }
    }
}
