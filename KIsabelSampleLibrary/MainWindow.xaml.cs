using KIsabelSampleLibrary.Controls;
using KIsabelSampleLibrary.Entity;
using KIsabelSampleLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KIsabelSampleLibrary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SettingsWindow settingswindow { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            settingswindow = new SettingsWindow(this);
            settingswindow.Show();
        }

        public void RefreshUI()
        {
            

            LibraryTreeView.Items.Clear();

            FolderTree tree = App.Services.Samples().GetFolderTree(App.Services.Settings().Settings.SamplePaths.FirstOrDefault());

            TreeViewItem rootItem = new TreeViewItem();
            
            LibraryTreeView.Items.Add(rootItem);
            rootItem.Header = App.Services.Settings().Settings.SamplePaths.FirstOrDefault();
            AddTreeElement(rootItem, tree.Elements.First());

            RefreshSamplesList();
           
        }

        public void RefreshLibraryDb()
        {
            App.Services.Samples().RefreshDatabase(App.Services.Settings().Settings.SamplePaths.First());
        }

        private void AddTreeElement(TreeViewItem item, FolderTreeElement element)
        {
            foreach (var child in element.Elements)
            {
                TreeViewItem childItem = new TreeViewItem();
                childItem.Header = child.Path.Replace(element.Path, "");
                childItem.ToolTip = child.Path;
                AddTreeElement(childItem, child);
                item.Items.Add(childItem);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            RefreshLibraryDb();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            RefreshUI();
        }

        private void RefreshSamplesList()
        {
            string pathQuery = null;
            if (LibraryTreeView.SelectedItem != null)
            {
                pathQuery = ((TreeViewItem)(LibraryTreeView.SelectedItem))?.ToolTip?.ToString().Replace(App.Services.Settings().Settings.SamplePaths.First(), "\\");
            }

            List<Sample> samples = App.Services.Samples().FindSamples(new SampleSearchModel()
            {
                path = pathQuery
            })
                .OrderBy(s => s.filename).ToList();

            SamplesList.DisplayMemberPath = "filename";
            SamplesList.Items.Clear();


            foreach (var sample in samples)
            {
                SamplesList.Items.Add(sample);
            }
        }

        private void LibraryTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void LibraryTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RefreshSamplesList();
        }
    }
}
