using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace LocalMediaPlayer
{
    public enum PlayEvent
    {
        Play = 0,
        Pause,
        PositionChanged,
        StartChangePosition,
        EndChangePosition,
        SoundChanged,
        MediaSpeedChanged,
        Edit,
        OpenFromList,
        SharedToLocal,
        SharedByNet,
        SharedToEMail
    }

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
                __Page = new(this);
                __Page.Load();
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
                        try
                        {
                            Process ps = new();
                            ps.StartInfo.FileName = "MediaEditor";
                            ps.StartInfo.Arguments = theMediaPlayer.Source.LocalPath;
                            ps.Start();
                            break;
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                            Environment.Exit(-1);
                            break;
                        }
                    }
                case PlayEvent.SharedToLocal:
                    {
                        SaveFileDialog dlg = new();
                        dlg.Filter = "视频文件|*.wmv;*.asf;*.avi;*.mp4;*.m4a;*.m4v;*.mp3;*.wav;*.wma";
                        dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                        if ((bool)dlg.ShowDialog())
                        {
                            string fn = dlg.FileName;
                            if(fn!=theMediaPlayer.Source.LocalPath)
                            {
                                //复制文件
                                File.Copy(theMediaPlayer.Source.LocalPath,fn);
                                MessageBox.Show($"{theMediaPlayer.Source.LocalPath}已成功分享到{fn}");
                            }
                        }
                        break;
                    }
                case PlayEvent.SharedByNet:
                    {
                        SharedNet net = new();
                        net.fileName= theMediaPlayer.Source.LocalPath;
                        Stop();
                        net.ShowDialog();
                        break;
                    }
                case PlayEvent.SharedToEMail:
                    {
                        SharedEMail page = new(this,theMediaPlayer.Source.LocalPath);
                        Navigate(page);
                        break;
                    }
            }
        }

        private enum FileComboBoxItem
        {
            OpenFile = 0,
            NetStream,
            PlayList,
            About,
            Ezit
        }

        private void FileComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (FileComboBox.SelectedIndex)
            {
                case (int)FileComboBoxItem.OpenFile:
                    {
                        OpenFileDialog dlg = new();
                        dlg.Filter = filter;
                        dlg.InitialDirectory = __LocalFilePath;
                        dlg.Multiselect = false;
                        if ((bool)dlg.ShowDialog())
                        {
                            OpenUrl(dlg.FileName);
                            AddToList();
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
                        Close();
                        break;
                    }
                case (int)FileComboBoxItem.About:
                    {
                        AboutDialog dlg = new();
                        dlg.ShowDialog();
                        break;
                    }
                case (int)FileComboBoxItem.NetStream:
                    {
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

        private void AddToList()
        {
            foreach(MediaListItem it in __Page.MediaList)
            {
                if (theMediaPlayer.Source.LocalPath == it.FileName)
                    return;
            }

            __Page.AddItem(theMediaPlayer.Source.LocalPath);
        }

        public void OpenUrl(string videopath)
        {
            Uri uri = new(videopath);
            theMediaPlayer.Source = uri;
            theMediaPlayer.Play();
            FileNameLabel.Content = Path.GetFileName(videopath);
            __LocalFilePath = Path.GetDirectoryName(videopath);
        }

        public void Stop()
        {
            theMediaPlayer.Source = null;
            FileNameLabel.Content = "";
            thePlayCtrl.UnEnable();
        }

        private string __LocalFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
        private PlayListPage __Page;

        private void NavigationWindow_Closed(object sender, EventArgs e)
        {
            __Page.Save();
        }

        ~MainWindow()
        {
            __Page.Save();
        }

        private void theMediaPlayer_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(thePlayCtrl.IsEnabled)
            {
                thePlayCtrl.PlayOrPause();
            }
        }

        private void NavigationWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch(e.Key)
            {
                case System.Windows.Input.Key.Space:
                    thePlayCtrl.PlayOrPause();
                    break;
                case System.Windows.Input.Key.Left:
                    thePlayCtrl.BackOrFront(true);
                    break;
                case System.Windows.Input.Key.Right:
                    thePlayCtrl.BackOrFront(false);
                    break;
                default:break;
            }
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if(files.Length>0)
            {
                if(IsMediaFile(files[0]))
                {
                    OpenUrl(files[0]);
                    AddToList();
                }
            }
        }

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects= DragDropEffects.None;
            }
        }

        private static readonly string[] audioExtensions = { ".mp3", ".wav", ".ogg", ".flac", ".m4a", ".wma" };
        private static readonly string[] videoExtensions = { ".mp4", ".avi", ".mov", ".flv", ".mkv", ".wmv", ".mpg", ".mpeg" };
        private static string filter = "所有媒体文件|*.mp3;*.wav;*.ogg;*.flac;*.m4a;*.wma;*.mp4;*.avi;*.mov;*.flv;*.mkv;*.wmv;*.mpg;*.mpeg|" +
                "音频文件|*.mp3;*.wav;*.ogg;*.flac;*.m4a;*.wma|" +
                "视频文件|*.mp4;*.avi;*.mov;*.flv;*.mkv;*.wmv;*.mpg;*.mpeg|" +
                "所有文件|*.*";

        public static bool IsMediaFile(string filePath)
        {
            // 检查字符串是否为空或空字符串
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }

            // 获取文件的扩展名
            string fileExtension = Path.GetExtension(filePath).ToLowerInvariant();

            // 检查是否是音频文件扩展名
            if (Array.IndexOf(audioExtensions, fileExtension) >= 0)
            {
                return true;
            }

            // 检查是否是视频文件扩展名
            if (Array.IndexOf(videoExtensions, fileExtension) >= 0)
            {
                return true;
            }

            // 如果不是以上任何一种扩展名，则不是多媒体文件
            return false;
        }

        private void NavigationWindow_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if(Content!=null)
            {
                App.SlideOutAnimation.Begin(Content as FrameworkElement);
            }
        }

        private void NavigationWindow_Navigated(object sender, NavigationEventArgs e)
        {
            if(Content != null)
            {
                App.SlideInAnimation.Begin(Content as FrameworkElement);
            }
        }

        private void theMediaPlayer_BufferingStarted(object sender, RoutedEventArgs e)
        {
            BuffingPopup.IsOpen = true;
        }

        private void theMediaPlayer_BufferingEnded(object sender, RoutedEventArgs e)
        {
            BuffingPopup.IsOpen = false;
        }
    }
}