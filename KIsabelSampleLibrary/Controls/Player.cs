using KIsabelSampleLibrary.Entity;
using System;
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

        public Sample Sample { get; set; }
    }
}