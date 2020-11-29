using System.Collections.Generic;
using System.Windows.Controls;

namespace KIsabelSampleLibrary.Controls
{
    /// <summary>
    /// Logique d'interaction pour StringsListSelector.xaml
    /// </summary>
    public partial class StringsListSelector : UserControl
    {
        private List<string> _Options = new List<string>();

        public bool CanAddNew { get; set; }



        public List<string> Options
        {
            get
            {
                return _Options;
            }

            set
            {
                _Options = value;
                RefreshOptions();
            }
        }

        private List<string> _SelectedItems  = new List<string>()
        {
            "test1", "test2"
        };

        public List<string> SelectedItems
        {
            get
            {
                return _SelectedItems;
            }

            set
            {
                _SelectedItems = value;
            }
        }

        public StringsListSelector()
        {
            InitializeComponent();
            RefreshOptions();
        }

        private void RefreshOptions()
        {
            SelectionsPanel.Children.Clear();

            foreach (var selectedValue in _SelectedItems)
            {
                Label label = new Label();

                StackPanel tagPanel = new StackPanel();
                tagPanel.Orientation = Orientation.Horizontal;

                Label tagNameLabel = new Label();
                tagNameLabel.Content = selectedValue;

                tagPanel.Children.Add(tagNameLabel);

                Button removeButton = new Button();
                removeButton.Content = "x";
                removeButton.ToolTip = selectedValue;
                

                tagPanel.Children.Add(removeButton);

                label.ToolTip = selectedValue;
                label.Content = tagPanel;
                SelectionsPanel.Children.Add(label);
            }
        }
    }
}
