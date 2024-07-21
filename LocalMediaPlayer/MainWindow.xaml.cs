using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace LocalMediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                thePlayCtrl.Event += DealEvent;
                string icopath = Path.GetFullPath("..\\..\\..\\NewMedia.ico");
                Icon = BitmapFrame.Create(new Uri(icopath));
                __Page = new();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(-1);
            }
        }

        private void DealEvent(object sender, ControlPlayEventHandleArgs e)
        {
            switch (e.Event)
            {
                case PlayEvent.Play:
                    theMediaPlayer.Play();
                    break;
                case PlayEvent.Pause:
                    theMediaPlayer.Pause();
                    break;
                case PlayEvent.PositionChanged:
                    theMediaPlayer.Position = (TimeSpan)e.Param;
                    break;
                case PlayEvent.StartChangePosition:
                    theMediaPlayer.IsMuted = true;
                    break;
                case PlayEvent.EndChangePosition:
                    theMediaPlayer.IsMuted = false;
                    break;
                case PlayEvent.SoundChanged:
                    theMediaPlayer.Volume = (double)e.Param;
                    break;
                case PlayEvent.MediaSpeedChanged:
                    theMediaPlayer.SpeedRatio=(double)e.Param;
                    break;
                case PlayEvent.Edit:
                    {
                        Process ps = new();
                        ps.StartInfo.FileName = "mediaplayer";
                        ps.StartInfo.Arguments = theMediaPlayer.Source.ToString();
                        ps.Start();
                        break;
                    }
            }
        }

        private enum FileComboBoxItem
        {
            OpenFile = 0,
            PlayList,
            Ezit
        }

        private void FileComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (FileComboBox.SelectedIndex)
            {
                case (int)FileComboBoxItem.OpenFile:
                    {
                        OpenFileDialog dlg = new();
                        dlg.Filter = "视频文件|*.wmv;*.asf;*.avi;*.mp4;*.m4a;*.m4v;*.mp3;*.wav;*.wma";
                        dlg.InitialDirectory = __LocalFilePath;
                        dlg.Multiselect = false;
                        if ((bool)dlg.ShowDialog())
                        {
                            string videopath = dlg.FileName;
                            Uri uri = new(videopath);
                            theMediaPlayer.Source = uri;
                            theMediaPlayer.Play();
                            FileNameLabel.Content = Path.GetFileName(dlg.FileName);
                            __LocalFilePath = Path.GetDirectoryName(dlg.FileName);
                        }
                        break;
                    }
                case (int)FileComboBoxItem.PlayList:
                    {
                        Navigate(__Page);
                        break;
                    }
                case (int)FileComboBoxItem.Ezit:
                    {
                        Environment.Exit(0);
                        break;
                    }
            }
            FileComboBox.SelectedIndex = -1;
        }

        private void MediaPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show(e.ErrorException.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            theMediaPlayer.Close();
        }

        private void theMediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            theMediaPlayer.Position = TimeSpan.Zero;
            thePlayCtrl.Position = theMediaPlayer.Position;
            thePlayCtrl.IsPlaying = true;
            theMediaPlayer.Play();
        }

        private void theMediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            thePlayCtrl.IsEnabled = true;
            thePlayCtrl.NaturalDuration = theMediaPlayer.NaturalDuration.TimeSpan;
            thePlayCtrl.Position = theMediaPlayer.Position;
            thePlayCtrl.IsPlaying = true;
        }

        private string __LocalFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
        private PlayListPage __Page;
    }
}