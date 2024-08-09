using System.Text;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.IO;

namespace LocalMediaPlayer
{
    public class SharedNet: Window
    {
        public SharedNet()
        {
            InitUI();
        }

        private void InitUI()
        {
            Title = "实时网络分享";
            ResizeMode = ResizeMode.NoResize;
            Width = 250;
            Height = 200;

            YesButton = new();
            YesButton.Content = "确定";
            YesButton.Click += YesButton_Click;
            YesButton.Style = App.DefaultButtonStyle;
            YesButton.Width = 80;

            CancelButton = new();
            CancelButton.Content= "取消";
            CancelButton.Click += CancelButton_Click;
            CancelButton.Width = 80;
            CancelButton.Style=App.DefaultButtonStyle;
            page = new();

            StackPanel stackPanel1 = new StackPanel
            {
                Orientation = Orientation.Vertical,

                Children =
                {
                    page,
                    new StackPanel
                    {
                        Orientation= Orientation.Horizontal,

                        Children={ YesButton,CancelButton }
                    }
                }
            };

            Content= stackPanel1;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            if(Page1.IsValidIPAddress(page.IPString))
            {
                YesButton.IsEnabled=false;
                CancelButton.IsEnabled = false;
                page.IsEnabled = false;
                page.TipText = "正在分享...";
                SendFile();
                Close();
            }
        }

        public string fileName = "";

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SendFile()
        {
            try
            {
                string ip = page.IPString;
                Client = new(ip, 6000);
                Stm = Client.GetStream();

                using(FileStream fs=File.Open(@"D:\MyClass\视频\Dumplings.mp4", FileMode.Open))
                {
                    SendFileName(fileName);
                    byte[] buffer = new byte[1024];
                    int bytesRead;
                    while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        // 发送数据到服务器
                        Stm.Write(buffer, 0, bytesRead);
                    }
                }
                Stm.Close();
                Client.Close();
                page.TipText = "成功发送";
                MessageBox.Show("成功发送");
            }
            catch(Exception ex)
            {
                page.TipText=ex.Message;
                MessageBox.Show(ex.Message,"错误",MessageBoxButton.OK,MessageBoxImage.Error);
                Close();
            }
        }

        private void SendFileName(string fileName)
        {
            try
            {
                string ft=Path.GetFileName(fileName);
                byte[] buffer=Encoding.UTF8.GetBytes(ft);
                int length=buffer.Length;
                Stm.Write(BitConverter.GetBytes(length), 0, sizeof(int));
                Stm.Write(buffer, 0, length);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private Button YesButton, CancelButton;
        private Page1 page;
        private TcpClient Client;
        private NetworkStream Stm;

        private class Page1:UserControl//填写发送方信息页
        {
            public Page1()
            {
                InitUI();
            }

            private void InitUI()
            {
                IPTextBox = new();
                IPTextBox.MaxLines = 1;
                IPTextBox.ToolTip = "输入对方的IP地址\0例如：192.168.0.110";
                IPTextBox.LostFocus += IPTextBox_LostFocus;
                IPTextBox.GotFocus += IPTextBox_GotFocus;
                IPTextBox.Width = 120;
                IPTextBox.Style = App.SingleLineTextBoxStyle;
                TipLabel = new();
                TipLabel.TextWrapping = TextWrapping.Wrap;

                Content = new StackPanel
                {
                    Orientation = Orientation.Vertical,

                    Children =
                    {
                        new Label{Content="请输入对方信息"},
                        new StackPanel
                        {
                            Orientation= Orientation.Horizontal,

                            Children =
                            {
                                new Label{Content="IP地址"},
                                IPTextBox
                            }
                        },
                        TipLabel
                    }
                };
            }

            private void IPTextBox_GotFocus(object sender, RoutedEventArgs e)
            {
                TipLabel.Text = "";
            }

            private void IPTextBox_LostFocus(object sender, RoutedEventArgs e)
            {
                if(!IsValidIPAddress(IPTextBox.Text))
                {
                    TipLabel.Foreground = new SolidColorBrush(Colors.Red);
                    TipLabel.Text = "IP地址无效";
                }
                else
                {
                    TipLabel.Foreground = new SolidColorBrush(Colors.Blue);
                    TipLabel.Text = "您应该可以进行下一步了，请点击下一步以继续";
                }
            }

            public string IPString
            {
                get => IPTextBox.Text;
            }

            public static bool IsValidIPAddress(string ip)
            {
                string ipv4Pattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
                string ipv6Pattern = @"^(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))$";

                return Regex.IsMatch(ip, ipv4Pattern) || Regex.IsMatch(ip, ipv6Pattern, RegexOptions.IgnoreCase);
            }

            public string TipText
            {
                get => TipLabel.Text;
                set => TipLabel.Text = value;
            }

            private TextBox IPTextBox;
            private TextBlock TipLabel;
        }
    }
}
