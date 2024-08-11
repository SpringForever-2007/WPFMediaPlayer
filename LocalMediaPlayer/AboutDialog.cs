using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace LocalMediaPlayer
{
    public class AboutDialog:Window
    {
        public AboutDialog()
        {
            InitUI();
        }

        private void InitUI()
        {
            try
            {
                Title = "关于";
                ResizeMode = ResizeMode.NoResize;
                Width=250;
                Height=200;

                View = new();
                string html = File.ReadAllText("AboutView.html");
                View.NavigateToString(html);
                MoreButton = new();
                MoreButton.Content = "更多";
                MoreButton.Click += MoreButton_Click;
                MoreButton.Width = 80;
                MoreButton.TabIndex = 2;

                OKButton = new();
                OKButton.Content = "确定";
                OKButton.Click += OKButton_Click;
                OKButton.Width = 80;
                OKButton.Style = App.DefaultButtonStyle;
                OKButton.TabIndex = 1;
                OKButton.IsDefault = true;

                Content = new WrapPanel
                {
                    Orientation = Orientation.Vertical,

                    Children =
                    {
                        new TextBlock{Text=Introduction,Margin=new(10,5,10,5),TextWrapping=TextWrapping.Wrap},
                        new StackPanel
                            {
                                Orientation=Orientation.Horizontal,
                                FlowDirection = FlowDirection.RightToLeft,
                                VerticalAlignment = VerticalAlignment.Bottom,

                                Children =
                                {
                                    MoreButton,OKButton
                                }
                            }
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(-1);
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MoreButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("AboutView.html") { UseShellExecute = true });
            }
            catch (Exception)
            {
                // 处理可能出现的异常，例如URL格式不正确
            }
        }

        private WebBrowser View;
        private Button MoreButton, OKButton;
        private readonly string Introduction =
@"Copyright(C) Jiang Xinghua 2024
LocalMediaPlayer是一个简单的本地媒体播放器，若要了解更多，请点击【更多按钮】。";
    }
}
