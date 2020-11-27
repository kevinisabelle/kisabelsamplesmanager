using KIsabelSampleLibrary.Entity;
using System;
using System.Windows;
using System.Windows.Controls;

namespace KIsabelSampleLibrary.Controls
{
    public partial class Player : UserControl
    {
        public Player()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string Title { get; set; }

        public int MaxLength { get; set; }

        private Sample _Sample;

        public Sample Sample
        {
            get
            {
                return _Sample;
            }
            set
            {
                _Sample = value;
                RefreshUI();
            }
        }

        private void RefreshUI()
        {
            if (_Sample == null)
            {
                LblFilename.Content = "";
                TxtTags.Text = "";
                TxtGenres.Text = "";
                LblDuration.Content = "N/A";
                return;
            }

            LblFilename.Content = _Sample.GetFullPath();
            TxtTags.Text = _Sample.tags;
            TxtGenres.Text = _Sample.genres;
            LblDuration.Content = _Sample.lengthMs + "ms";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            App.Services.Audio().PlaySample(_Sample);
        }
    }
}