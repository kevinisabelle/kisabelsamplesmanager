using KIsabelSampleLibrary.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Player));

        public Player()
        {
            InitializeComponent();
            this.DataContext = this;
            this.PreviewMouseRightButtonDown += Player_PreviewMouseLeftButtonDown;
        }

        public string Title { get; set; }
        public int MaxLength { get; set; }
        private List<Sample> _Samples;

        public List<Sample> Samples
        {
            get
            {
                return _Samples;
            }
            set
            {
                _Samples = value;
                RefreshUI();
            }
        }

        private void RefreshUI()
        {
            if (_Samples == null || _Samples.Count() == 0)
            {
                LblFilename.Text = "";
                TxtTags.Text = "";
                TxtGenres.Text = "";
                LblDuration.Content = "N/A";
                ImgSoundImage.Source = null;
                return;
            }

            if (_Samples.Count() > 1)
            {
                LblFilename.Text = string.Join(',',_Samples.Select(s => s.filename));

                TxtTags.Text = GetTagsTextFromSamples();
                TxtTags.IsEnabled = IsTagsTextEnabled();
                TxtGenres.Text = GetGenresTextFromSamples();
                TxtGenres.IsEnabled = IsGenresTextEnabled();

                LblDuration.Content = _Samples.Count() + " samples";
            }
            else
            {
                Sample _Sample = _Samples[0];

                string sampleFullPath = _Sample.GetFullAbsolutePath(App.Services.Samples().GetFolders());
        
                TxtTags.IsEnabled = true;
                TxtGenres.IsEnabled = true;

                LblFilename.Text = sampleFullPath;
                TxtTags.Text = _Sample.tags;
                TxtGenres.Text = _Sample.genres;
                LblDuration.Content = _Sample.lengthMs + "ms";
            }

            Sample lastSample = _Samples.Last();
            string sampleFullPathLast = lastSample.GetFullAbsolutePath(App.Services.Samples().GetFolders());

            if (File.Exists(sampleFullPathLast))
            {
                WaveFormRenderer renderer = new WaveFormRenderer();
                System.Drawing.Image image = renderer.Render(sampleFullPathLast, new StandardWaveFormRendererSettings()
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

            if (App.Services.Settings().Settings.AutoplaySamplesOnClick)
            {
                try
                {
                    App.Services.Audio().PlaySample(_Samples.Last(), App.Services.Samples().GetFolders());
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }

        private string GetTagsTextFromSamples()
        {
            string[][] orderedUnderscoredTagsPerSample = _Samples.Select(s => s.GetTags().OrderBy(t => t).Select(t => t.ToLower()).ToArray()).ToArray();
            string[] toStringWithTags = orderedUnderscoredTagsPerSample.Select(s => string.Join('|', s)).Where(t => t != "").ToArray();
            string[] distinct = toStringWithTags.Distinct().ToArray();
            return distinct.FirstOrDefault() == null ? "" : distinct.FirstOrDefault();
        }

        private bool IsTagsTextEnabled()
        {
            string[][] orderedUnderscoredTagsPerSample = _Samples.Select(s => s.GetTags().OrderBy(t => t).Select(t => t.ToLower()).ToArray()).ToArray();
            string[] toStringWithTags = orderedUnderscoredTagsPerSample.Select(s => string.Join('|', s)).Where(t => t != "").ToArray();
            string[] distinct = toStringWithTags.Distinct().ToArray();
            return !(distinct.Length > 1);
            
        }

        private string GetGenresTextFromSamples()
        {
            string[][] orderedUnderscoredGenresPerSample = _Samples.Select(s => s.GetGenres().OrderBy(t => t).Select(t => t.ToLower()).ToArray()).ToArray();
            string[] toStringWithGenres = orderedUnderscoredGenresPerSample.Select(s => string.Join('|', s)).Where(t => t != "").ToArray();
            string[] distinct = toStringWithGenres.Distinct().ToArray();
            return distinct.FirstOrDefault() == null ? "" : distinct.FirstOrDefault();
        }

        private bool IsGenresTextEnabled()
        {
            string[][] orderedUnderscoredGenresPerSample = _Samples.Select(s => s.GetGenres().OrderBy(t => t).Select(t => t.ToLower()).ToArray()).ToArray();
            string[] toStringWithGenres = orderedUnderscoredGenresPerSample.Select(s => string.Join('|', s)).Where(t => t != "").ToArray();
            string[] distinct = toStringWithGenres.Distinct().ToArray();
            return !(distinct.Length > 1);
        }

        private void Player_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Player parent = (Player)sender;

            if (parent.Samples == null)
            {
                return;
            }

            object data2 = parent.Samples.Select(s => s.GetFullAbsolutePath(App.Services.Samples().GetFolders())).ToArray();
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
            try
            {
                if (_Samples == null || _Samples.Count() > 1)
                {
                    return;
                }

                App.Services.Audio().PlaySample(_Samples[0], App.Services.Samples().GetFolders());

            } catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void BtnSaveTagsGenres_Click(object sender, RoutedEventArgs e)
        {
            if (TxtGenres.IsEnabled)
            {
                _Samples.ForEach(s =>
                {
                    s.genres = TxtGenres.Text;
                });
            }

            if (TxtTags.IsEnabled)
            {
                _Samples.ForEach(s =>
                {
                    s.tags = TxtTags.Text;
                });
            }

            App.Services.Db().Samples.UpdateRange(_Samples);
            App.Services.Db().SaveChanges();
        }

        private void TxtGenres_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ((Button)sender).IsEnabled = true;
        }

        private void TxtTags_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ((Button)sender).IsEnabled = true;
        }
    }
}