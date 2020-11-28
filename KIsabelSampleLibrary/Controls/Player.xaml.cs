using KIsabelSampleLibrary.Entity;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WaveFormRendererLib;

namespace KIsabelSampleLibrary.Controls
{
    public partial class Player : UserControl
    {
        public Player()
        {
            InitializeComponent();
            this.DataContext = this;
            this.PreviewMouseRightButtonDown += Player_PreviewMouseLeftButtonDown;
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
                LblFilename.Text = "";
                TxtTags.Text = "";
                TxtGenres.Text = "";
                LblDuration.Content = "N/A";
                ImgSoundImage.Source = null;
                return;
            }

            string sampleFullPath = _Sample.GetFullAbsolutePath(App.Services.Samples().GetFolders());

            LblFilename.Text = sampleFullPath;
            TxtTags.Text = _Sample.tags;
            TxtGenres.Text = _Sample.genres;
            LblDuration.Content = _Sample.lengthMs + "ms";

            if (File.Exists(sampleFullPath))
            {
                WaveFormRenderer renderer = new WaveFormRenderer();
                System.Drawing.Image image = renderer.Render(sampleFullPath, new StandardWaveFormRendererSettings()
                {
                    Width = 2000,
                    PixelsPerPeak = 2,
                    BackgroundColor = System.Drawing.Color.Black,
                    BottomPeakPen = Pens.White,
                    TopPeakPen = Pens.White,
                    TopHeight = 100,
                    BottomHeight = 100
                });

                ImgSoundImage.Stretch = Stretch.Fill;
                ImgSoundImage.Source = GetImageStream(image);
            } 
            else
            {
                ImgSoundImage.Source = null;
            }

        }

        private void Player_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Player parent = (Player)sender;

            if (parent.Sample == null)
            {
                return;
            }

            object data2 = new string[] { parent.Sample.GetFullAbsolutePath(App.Services.Samples().GetFolders()) };
            DataObject data = new DataObject(DataFormats.FileDrop, data2);
            if (data != null)
            {
                DragDrop.DoDragDrop(parent, data, DragDropEffects.Copy);
            }
        }

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr value);

        public static BitmapSource GetImageStream(System.Drawing.Image myImage)
        {
            var bitmap = new Bitmap(myImage);
            IntPtr bmpPt = bitmap.GetHbitmap();
            BitmapSource bitmapSource =
             System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                   bmpPt,
                   IntPtr.Zero,
                   Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());

            //freeze bitmapSource and clear memory to avoid memory leaks
            bitmapSource.Freeze();
            DeleteObject(bmpPt);

            return bitmapSource;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            App.Services.Audio().PlaySample(_Sample, App.Services.Samples().GetFolders());
        }
    }
}