using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                Width = 200;
                Height = 150;
                ResizeMode = ResizeMode.NoResize;

                View = new();
                string html = File.ReadAllText("AboutView.html");
                View.NavigateToString(html);
                MoreButton = new();
                MoreButton.Content = "更多";
                MoreButton.Click += MoreButton_Click;
                MoreButton.Margin = new(10, 5, 10, 5);
                OKButton = new();
                OKButton.Content = "确定";
                OKButton.Click += OKButton_Click;
                OKButton.Margin = new(10,5,10,5);

                Content = new StackPanel
                {
                    Orientation = Orientation.Vertical,

                    Children =
                {
                    new TextBlock{Text=Introduction,Margin=new(10,5,10,5),TextWrapping=TextWrapping.Wrap},
                    new StackPanel
                    {
                        Orientation=Orientation.Horizontal,
                        FlowDirection = FlowDirection.RightToLeft,

                        Children =
                        {
                            MoreButton,OKButton
                        }
                    }
                }
                };
            }
            catch(Exception ex)
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
            Window wnd = new();
            wnd.Title = "更多";
            wnd.Content = View;
            wnd.ShowDialog();
        }

        private WebBrowser View;
        private Button MoreButton, OKButton;
        private readonly string Introduction =
@"Copyright(C) Jiang Xinghua 2024
LocalMediaPlayer是一个简单的本地媒体播放器，若要了解更多，请点击【更多按钮】。";
    }
}
