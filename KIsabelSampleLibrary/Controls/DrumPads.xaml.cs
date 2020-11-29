using KIsabelSampleLibrary.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    public partial class DrumPads : UserControl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(DrumPads));


        private readonly int rows = 16;
        private readonly int cols = 1;

        public DrumKit DrumKit
        {
            get
            {
                return _DrumKit;
            }

            set
            {
                _DrumKit = value;
                RefreshUIFromDrumKit();
            }
        }

        private DrumKit _DrumKit { get; set; }

        private List<DrumKit> AvailableKits { get; set; }

        public DrumPads()
        {
            InitializeComponent();
            
            this.DataContext = this;
            RefreshDrumKits();
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
                    _Buttons[y][x].PreviewMouseRightButtonDown += Pad_PreviewMouseLeftButtonDown;
                    _Buttons[y][x].Content = null;
                    PadsGrid.Children.Add(_Buttons[y][x]);

                    Grid.SetRow(_Buttons[y][x], y+2);
                    Grid.SetColumn(_Buttons[y][x], x);
                }
            }
        }

        public void RefreshDrumKits()
        {
            AvailableKits = App.Services.Db().DrumKits.ToList();
            CboKits.DisplayMemberPath = "IdName";
            
            CboKits.ItemsSource = AvailableKits;

        }

        

        public void RefreshUIFromDrumKit()
        {  
            ClearUI();

            TxtKitName.Text = _DrumKit.Name;
            LblKitId.Content = _DrumKit.Id;

            UpdateButtonFromSlotData(_DrumKit.Slot0, 0, 0);
            UpdateButtonFromSlotData(_DrumKit.Slot1, 1, 0);
            UpdateButtonFromSlotData(_DrumKit.Slot2, 2, 0);
            UpdateButtonFromSlotData(_DrumKit.Slot3, 3, 0);
            UpdateButtonFromSlotData(_DrumKit.Slot4, 4, 0);
            UpdateButtonFromSlotData(_DrumKit.Slot5, 5, 0);
            UpdateButtonFromSlotData(_DrumKit.Slot6, 6, 0);
            UpdateButtonFromSlotData(_DrumKit.Slot7, 7, 0);
            UpdateButtonFromSlotData(_DrumKit.Slot8, 8, 0);
            UpdateButtonFromSlotData(_DrumKit.Slot9, 9, 0);
            UpdateButtonFromSlotData(_DrumKit.Slot10, 10, 0);
            UpdateButtonFromSlotData(_DrumKit.Slot11, 11, 0);
            UpdateButtonFromSlotData(_DrumKit.Slot12, 12, 0);
            UpdateButtonFromSlotData(_DrumKit.Slot13, 13, 0);
            UpdateButtonFromSlotData(_DrumKit.Slot14, 14, 0);
            UpdateButtonFromSlotData(_DrumKit.Slot15, 15, 0);

        }

        private void UpdateButtonFromSlotData(string slotX, int y, int x)
        {
            if (slotX != null)
            {
                Sample sample0 = App.Services.Samples().FindSamples(new Services.SampleSearchModel()
                {
                    query = slotX
                }).FirstOrDefault();

                if (sample0 != null)
                {
                    _Buttons[y][x].Content = sample0;
                    _Samples[y][x] = sample0;
                }
            }
            else
            {
                _Buttons[y][x].Content = null;
                _Samples[y][x] = null;
            }
        }

        private void ClearUI()
        {
            TxtKitName.Text = "Kit " + DateTime.Now.ToString("yyyy MM dd HH:mm:ss");
            LblKitId.Content = "";

            for (int y = 0; y < rows; y++)
            {
                //_Samples[y] = new Sample[cols];
                //_Buttons[y] = new Button[cols];

                for (int x = 0; x < cols; x++)
                {
                    _Samples[y][x] = null;
                    _Buttons[y][x].Content = null;
                }
            }
        }

        public void UpdateKitFromUI()
        {
            List<SamplesFolder> folders = App.Services.Samples().GetFolders();

            if (_DrumKit == null)
            {
                _DrumKit = new DrumKit();
            }

            _DrumKit.Name = TxtKitName.Text;

            _DrumKit.Slot0 = _Samples[0][0]?.GetFullPath(folders);
            _DrumKit.Slot1 = _Samples[1][0]?.GetFullPath(folders);
            _DrumKit.Slot2 = _Samples[2][0]?.GetFullPath(folders);
            _DrumKit.Slot3 = _Samples[3][0]?.GetFullPath(folders);
            _DrumKit.Slot4 = _Samples[4][0]?.GetFullPath(folders);
            _DrumKit.Slot5 = _Samples[5][0]?.GetFullPath(folders);
            _DrumKit.Slot6 = _Samples[6][0]?.GetFullPath(folders);
            _DrumKit.Slot7 = _Samples[7][0]?.GetFullPath(folders);
            _DrumKit.Slot8 = _Samples[8][0]?.GetFullPath(folders);
            _DrumKit.Slot9 = _Samples[9][0]?.GetFullPath(folders);
            _DrumKit.Slot10 = _Samples[10][0]?.GetFullPath(folders);
            _DrumKit.Slot11 = _Samples[11][0]?.GetFullPath(folders);
            _DrumKit.Slot12 = _Samples[12][0]?.GetFullPath(folders);
            _DrumKit.Slot13 = _Samples[13][0]?.GetFullPath(folders);
            _DrumKit.Slot14 = _Samples[14][0]?.GetFullPath(folders);
            _DrumKit.Slot15 = _Samples[15][0]?.GetFullPath(folders);
        }

        private void SaveCurrentKit()
        {
            UpdateKitFromUI();
            
            if (_DrumKit.Id == 0)
            {
                App.Services.Db().DrumKits.Add(_DrumKit);
            } 
            else
            {
                App.Services.Db().DrumKits.Update(_DrumKit);
            }

            App.Services.Db().SaveChanges();

            RefreshDrumKits();
        }

        private void LoadSelectedKit()
        {
            
            if (CboKits.SelectedItem != null)
            {
                _DrumKit = CboKits.SelectedItem as DrumKit;
                RefreshUIFromDrumKit();
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

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files != null)
            {
                sample = App.Services.Samples().FindSamples(new Services.SampleSearchModel()
                {
                    query = files[0]
                }).FirstOrDefault();
            }

            if (sample != null)
            {
                int[] yx = ((Button)sender).Name.Substring(1).Split("_").Select(t => int.Parse(t)).ToArray();

                _Samples[yx[0]][yx[1]] = sample;
                _Buttons[yx[0]][yx[1]].Content = sample;

                return;
            }

        }

        private void Pad_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button parent = (Button)sender;

            if (parent.Content == null)
            {
                return;
            }

            object data2 = new string[] { (parent.Content as Sample).GetFullAbsolutePath(App.Services.Samples().GetFolders()) };
            DataObject data = new DataObject(DataFormats.FileDrop, data2);
            if (data != null)
            {
                DragDrop.DoDragDrop(parent, data, DragDropEffects.Copy);
            }
        }

        private Sample[][] _Samples;
        private Button[][] _Buttons;

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentKit();
        }

        private void BtnLoadKit_Click(object sender, RoutedEventArgs e)
        {
            LoadSelectedKit();
        }

        private void BtnAddNew_Click(object sender, RoutedEventArgs e)
        {
            ClearUI();
            _DrumKit = new DrumKit();
        }
    }
}