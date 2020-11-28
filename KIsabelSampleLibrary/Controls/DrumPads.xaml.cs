using KIsabelSampleLibrary.Entity;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WaveFormRendererLib;

namespace KIsabelSampleLibrary.Controls
{
    public partial class DrumPads : UserControl
    {
        private int rows = 16;
        private int cols = 1;

        public DrumPads()
        {
            InitializeComponent();
            this.DataContext = this;

            _Samples = new Sample[rows][];
            _Buttons = new Button[rows][];

            for (int y = 0; y<rows; y++)
            {
                _Samples[y] = new Sample[cols];
                _Buttons[y] = new Button[cols];
                for (int x=0; x<cols; x++)
                {
                    _Samples[y][x] = null;
                    _Buttons[y][x] = new Button();
                    _Buttons[y][x].Click += OnSampleButtonClick;
                    _Buttons[y][x].AllowDrop = true;
                    _Buttons[y][x].Drop += SampleDrop;
                    _Buttons[y][x].Name = "p" + y + "_" + x;
                   


                    PadsGrid.Children.Add(_Buttons[y][x]);

                    Grid.SetRow(_Buttons[y][x], y);
                    Grid.SetColumn(_Buttons[y][x], x);
                }
            }
        }

        public void SetSample(int y, int x, Sample sample)
        {
            _Samples[y][x] = sample;
            TextBlock content = new TextBlock();
            content.Text = sample.filename;
            content.TextWrapping = TextWrapping.Wrap;
            content.TextAlignment = TextAlignment.Center;
            _Buttons[y][x].Content = content;

        }

        public void OnSampleButtonClick(object sender, RoutedEventArgs e)
        {
            int[] yx = ((Button)sender).Name.Substring(1).Split("_").Select(t => int.Parse(t)).ToArray();

            Sample sample = _Samples[yx[0]][yx[1]];

            if (sample != null)
            {
                App.Services.Audio().PlaySample(sample, App.Services.Samples().GetFolders());
            }
        }

        private void SampleDrop(object sender, DragEventArgs e)
        {
            
            Sample sample = e.Data.GetData(typeof(Sample)) as Sample;

            int[] yx = ((Button)sender).Name.Substring(1).Split("_").Select(t => int.Parse(t)).ToArray();

            _Samples[yx[0]][yx[1]] = sample;
            _Buttons[yx[0]][yx[1]].Content = sample.filename;
        }

        private Sample[][] _Samples;
        private Button[][] _Buttons;

    }
}