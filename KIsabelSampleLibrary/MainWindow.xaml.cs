using KIsabelSampleLibrary.Controls;
using KIsabelSampleLibrary.Entity;
using KIsabelSampleLibrary.Services;
using System;
using System.Collections.Generic;
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
            App.Services.Samples().RefreshDatabase(App.Services.Settings().Settings.SamplePaths.First(), FeedBack);
        }
        
        public void FeedBack(Sample sample, long currentcount, long totalCount, RefreshDataStatus status)
        {
            try
            {
                TxtLog.Dispatcher.Invoke(
                    new UpdateFeedback(SetSample),
                    new object[] { sample, currentcount, totalCount, status }
                );
            } catch(Exception e)
            {

            }
        }

        public void SetSample(Sample sample, long currentcount, long totalCount, RefreshDataStatus status)
        {
            TxtLog.Text = status + "\n" + currentcount + " / " + totalCount + "\n" + sample?.GetFullPath();
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
                favorites = ChkFavorites.IsChecked
            })
                .OrderBy(s => s.filename).ToList();

            //SamplesList.DisplayMemberPath = "./Content/@filename";
            SamplesList.Items.Clear();

            foreach (var sample in samples)
            {
                ListViewItem item = new ListViewItem()
                {
                    Content = sample,
                    
                };

                item.PreviewMouseRightButtonDown += ListBox_PreviewMouseLeftButtonDown;

                SamplesList.Items.Add(item);
            }
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
            object data = GetDataFromListBox(dragSource, e.GetPosition(parent));

            if (data != null)
            {
                DragDrop.DoDragDrop(parent, data, DragDropEffects.Copy);
            }
        }

        private static object GetDataFromListBox(ListViewItem source, Point point)
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

            RefreshUI();

            listView.SelectedItem = selectedItem;
        }
    }
}
