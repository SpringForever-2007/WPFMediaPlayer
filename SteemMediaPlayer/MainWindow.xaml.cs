using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;

namespace SteemMediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string[] cmdargs = Environment.GetCommandLineArgs();
            if(cmdargs.Length==2)
            {
                Open(cmdargs[1]);
            }
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            Open(URLTextBox.Text);
        }

        private void FlushButton_Click(object sender, RoutedEventArgs e)
        {
            Open(URLTextBox.Text);
        }

        public static bool IsURL(string text)
        {
            // 简化的网址正则表达式
            string pattern = @"^(https?://)?([\da-z.-]+)\.([a-z.]{2,6})([/\w .-]*)*/?$";

            // 创建Regex对象，并指定忽略大小写
            Regex regex = new(pattern, RegexOptions.IgnoreCase);

            // 使用IsMatch方法检查文本是否与正则表达式匹配
            return regex.IsMatch(text);
        }

        public bool IsPlaying
        {
            get => __IsPlaying;
            set
            {
                __IsPlaying = value;
                if(__IsPlaying)
                {
                    Player.Play();
                    MediaStatus.Content = "播放中";
                }
                else
                {
                    Player.Pause();
                    MediaStatus.Content = "已暂停";
                }
            }
        }

        private bool __IsPlaying;

        private void Player_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsPlaying = !IsPlaying;
        }

        private void URLTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsURL(URLTextBox.Text))
            {
                URLTextBox.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
            else
            {
                URLTextBox.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
        }

        private void Player_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show(e.ErrorException.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Player.Volume = e.NewValue;
        }

        public void Open(string uri)
        {
            URLTextBox.Text = uri;

            try
            {
                if (URLTextBox.Text.Length > 0)
                {
                    Player.Source = new Uri(URLTextBox.Text);
                    IsPlaying = true;
                    Player.Volume = VolumeSlider.Value;
                    URLLabel.Content = URLTextBox.Text;
                }
                else MessageBox.Show("链接错误", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Space)
            {
                IsPlaying = !IsPlaying;
            }
            else if(e.Key == Key.Escape)
            {
                if(MessageBox.Show("你要退出本应用吗？","提示",
                    MessageBoxButton.YesNo,MessageBoxImage.Question)
                    ==MessageBoxResult.Yes)
                    Environment.Exit(0);
            }
        }

        private void URLTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Open(URLTextBox.Text);
            }
        }
    }
}