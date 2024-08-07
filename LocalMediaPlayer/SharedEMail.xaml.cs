using System.Windows.Controls;
using System.ComponentModel;
using System.Net.Mail;
using System.Net;
using System.Windows;

namespace LocalMediaPlayer
{
    /// <summary>
    /// SharedEMail.xaml 的交互逻辑
    /// </summary>
    public partial class SharedEMail : Page
    {
        public SharedEMail(MainWindow wnd, string fileName)
        {
            InitializeComponent();
            MainWnd = wnd;
            FileName = fileName;
            
        }

        private MainWindow MainWnd;
        private string FileName;

        private void YesButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SendEMail();
        }

        private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MainWnd.GoBack();
        }

        private void SendEMail()
        {
            try
            {
                // 邮件发送者信息
                string fromAddress = Model.UserEMailAddress;
                string fromPassword = Model.UserEMailPassword;
                // 邮件接收者信息
                string toAddress = Model.OtherEMailAddress;

                // 创建邮件消息
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(fromAddress);
                mail.To.Add(toAddress);
                mail.Subject = Model.Title;
                mail.Body = Model.Body;

                // 添加附件
                Attachment attachment = new Attachment(FileName);
                mail.Attachments.Add(attachment);

                // SMTP客户端设置
                SmtpClient smtp = new();
                smtp.Credentials = new NetworkCredential(fromAddress, fromPassword);
                InitEMailClientInfo(smtp, toAddress);

                // 发送邮件
                smtp.Send(mail);
                MessageBox.Show("成功发送");
                MainWnd.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitEMailClientInfo(SmtpClient smtp,string Addr)
        {
            string[] param = Addr.Split('@');
            if(param.Length==2)
            {
                switch (param[1])
                {
                    case "qq.com":
                        smtp.Host = "smtp.qq.com";
                        smtp.Port = 465;
                        smtp.EnableSsl = true;
                        break;
                    case "163.com":
                        smtp.Host = "smtp.163.com";
                        smtp.Port = 465;
                        smtp.EnableSsl = true;
                        break;
                    case "126.com":
                        smtp.Host = "smtp.126.com";
                        smtp.Port = 465;
                        smtp.EnableSsl = true;
                        break;
                    case "139.com":
                        smtp.Host = "smtp.139.com";
                        smtp.Port = 465;
                        smtp.EnableSsl = true;
                        break;
                    case "sina.com":
                        smtp.Host = "smtp.sina.com";
                        smtp.Port = 465;
                        smtp.EnableSsl = true;
                        break;
                    case "sohu.com":
                        smtp.Host = "smtp.sohu.com";
                        smtp.Port = 465;
                        smtp.EnableSsl = true;
                        break;
                    case "tom.com":
                        smtp.Host = "smtp.tom.com";
                        smtp.Port = 465;
                        smtp.EnableSsl = true;
                        break;
                    case "outlook.com":
                    case "hotmail.com":
                    case "live.com":
                        smtp.Host = "smtp-mail.outlook.com";
                        smtp.Port = 587; // 或者 465，取决于用户配置
                        smtp.EnableSsl = true;
                        break;
                    // 可以继续添加其他邮箱服务提供商的设置
                    default:
                        throw new Exception("不支持的邮箱服务提供商");
                }
            }
            else
            {
                throw new Exception("接收方的地址有误");
            }
        }
    }

    public class SharedEMailViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                PropertyChanged?.Invoke(value, new("Title"));
            }
        }

        public string Body
        {
            get => _body;
            set
            {
                _body = value;
                PropertyChanged?.Invoke(value, new("Body"));
            }
        }

        public string UserEMailAddress
        {
            get => _useremailaddress;
            set
            {
                _useremailaddress = value;
                PropertyChanged?.Invoke(value, new("UserEMailAddress"));
            }
        }

        public string UserEMailPassword
        {
            get => _useremailpassword;
            set
            {
                _useremailpassword = value;
                PropertyChanged?.Invoke(value, new("UserEMailPassword"));
            }
        }

        public string OtherEMailAddress
        {
            get => _otheremailaddress;
            set
            {
                _otheremailaddress = value;
                PropertyChanged?.Invoke(value, new("OtherEMailAddress"));
            }
        }

        private string _title, _body, _useremailaddress,
            _useremailpassword, _otheremailaddress;

        public SharedEMailViewModel()
        {
            _title = "Title";
            _body = "body";
            _useremailaddress = "zhangsan123@example.com";
            _useremailpassword = "Password";
            _otheremailaddress = "lisi456@email.com";
        }
    }
}
