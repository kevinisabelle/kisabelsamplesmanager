using KIsabelSampleLibrary.Controls;
using KIsabelSampleLibrary.Entity;
using KIsabelSampleLibrary.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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
using static KIsabelSampleLibrary.Services.SamplesService;

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
            RefreshUI();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            settingswindow = new SettingsWindow(this);
            settingswindow.Show();
        }

        public void RefreshUI()
        {
            LibraryTreeView.Items.Clear();

            FolderTree tree = App.Services.Samples().GetFolderTree(App.Services.Samples().GetFolders());

            foreach (var rootItemE in tree.Elements)
            {
                TreeViewItem rootItem = new TreeViewItem();

                LibraryTreeView.Items.Add(rootItem);
                rootItem.ToolTip = rootItemE.Folder.BasePath;
                rootItem.Header = rootItemE.Folder.Name + " (" + rootItemE.Folder.BasePath + ")";
                AddTreeElement(rootItem, rootItemE);
            }

            RefreshSamplesList();
        }

        private List<SamplesFolder> foldersTemp;

        public void RefreshLibraryDb()
        {
            foldersTemp = App.Services.Samples().GetFolders();
            App.Services.Samples().RefreshDatabase(FeedBack);
        }
        
        public void FeedBack(Sample sample, long currentcount, long totalCount, SamplesFolder folder, int currentFolder, int totalFolders, RefreshDataStatus status)
        {
            try
            {
                TxtLog.Dispatcher.Invoke(
                    new UpdateFeedback(SetSample),
                    new object[] { sample, currentcount, totalCount, folder, currentFolder, totalFolders, status }
                );
            } catch(Exception e)
            {

            }
        }

        public void SetSample(Sample sample, long currentcount, long totalCount, SamplesFolder folder, int currentFolder, int totalFolders, RefreshDataStatus status)
        {
            TxtLog.Text = status + "\n" + currentcount + " / " + totalCount + "\n" + sample?.GetFullPath(foldersTemp);
        }

        private void AddTreeElement(TreeViewItem item, FolderTreeElement element)
        {
            foreach (var child in element.Elements)
            {
                TreeViewItem childItem = new TreeViewItem();
                childItem.Header = PathHelper.SanitizeSamplePathFolder(child.Path.Replace(element.Path, ""), false);

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
                pathQuery = ((TreeViewItem)(LibraryTreeView.SelectedItem))?.ToolTip?.ToString();
            } 

            List<Sample> samples = App.Services.Samples().FindSamples(new SampleSearchModel()
            {
                path = pathQuery,
                query = TxtQuery.Text == "" ? null : TxtQuery.Text,
                genres = TxtGenres.Text == "" ? null : TxtGenres.Text.Split(" "),
                tags = TxtTags.Text == "" ? null : TxtTags.Text.Split(" "),
                favorites = ChkFavorites.IsChecked,
                missingFiles = ChkMissingFiles.IsChecked,
            })
                .OrderBy(s => s.filename).ToList();

            //SamplesList.DisplayMemberPath = "./Content/@filename";
            SamplesList.Items.Clear();

            foreach (var sample in samples)
            {
                ListViewItem item = new ListViewItem()
                {
                    Content = sample,
                    Foreground = new SolidColorBrush(sample.isFilePresent ? Colors.Black : Colors.Red),
                    FontWeight = sample.favorite ? FontWeights.Bold : FontWeights.Normal
                };

                item.PreviewMouseRightButtonDown += ListBox_PreviewMouseLeftButtonDown;

                SamplesList.Items.Add(item);
            }

            SampleGroupBox.Header = "Samples (" + SamplesList.Items.Count + ")"; 
        }

        private void LibraryTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RefreshSamplesList();
        }

        private void SamplesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender.GetType() != typeof(ListView))
            {
                return;
            }

            if (SamplesList.SelectedItem == null)
            {
                return;
            }

            SamplePlayer.Sample = (Sample)((ListViewItem)SamplesList.SelectedItem).Content;
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            RefreshSamplesList();
        }

        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItem parent = (ListViewItem)sender;
            ListViewItem  dragSource = parent;
            object data = GetDataFromListBox(dragSource);

            if (data != null)
            {
                DragDrop.DoDragDrop(parent, data, DragDropEffects.Copy);
            }
        }

        private static object GetDataFromListBox(ListViewItem source)
        {
            return source.Content as Sample;

        }

        private void SamplesList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListView listView = sender as ListView;

            if (listView.SelectedItem != null)
            {
                Sample sample = (listView.SelectedItem as ListViewItem).Content as Sample;
                sample.favorite = !sample.favorite;
                App.Services.Samples().SaveSample(sample);
               
            }

            object selectedItem = listView.SelectedItem;

            RefreshSamplesList();

            listView.SelectedItem = selectedItem;
        }

        private void ResetFolderSelection()
        {
            if (LibraryTreeView.SelectedItem == null)
            {
                RefreshSamplesList();
                return;
            }

            ((TreeViewItem)(LibraryTreeView.SelectedItem)).IsSelected = false;
            
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ResetFolderSelection();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            TxtQuery.Text = "";
            TxtGenres.Text = "";
            TxtTags.Text = "";
            ChkFavorites.IsChecked = false;
            
            ResetFolderSelection();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                App.Services.Samples().RemoveMissingFiles();
                RefreshUI();
            }
        }
    }
}
