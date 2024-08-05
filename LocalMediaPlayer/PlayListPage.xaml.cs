using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace LocalMediaPlayer
{
    /// <summary>
    /// PlayListPage.xaml 的交互逻辑
    /// </summary>
    public partial class PlayListPage : Page
    {
        public PlayListPage(MainWindow wnd)
        {
            InitializeComponent();
            MediaList = new();
            ParentWnd = wnd;
            MediaListItemCtrl.MainWnd = ParentWnd;
        }

        public void Load()
        {
            try
            {
                using (XmlReader rd = XmlReader.Create(".\\PlayListData.xml"))
                {
                    rd.ReadStartElement("PlayList");
                    string fn, dt;
                    while(rd.Read())
                    {
                        if(rd.IsStartElement("ListItem"))
                        {
                            fn = rd.GetAttribute(0);
                            dt = rd.GetAttribute(1);
                            AddItem(fn, dt);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(-1);
            }
        }

        public void Save()
        {
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = ("\t");
                settings.NewLineChars = ("\n");
                settings.NewLineHandling = NewLineHandling.Replace;

                using (XmlWriter wrt = XmlWriter.Create(".\\PlayListData.xml",settings))
                {
                    wrt.WriteStartDocument();
                    wrt.WriteStartElement("PlayList");
                    wrt.WriteAttributeString("Number", MediaList.Count.ToString());
                    foreach(MediaListItem it in MediaList)
                    {
                        wrt.WriteStartElement("ListItem");
                        wrt.WriteAttributeString("FileName", it.FileName);
                        wrt.WriteAttributeString("VideoDateTime", it.DateTime);
                        wrt.WriteEndElement();
                    }
                    wrt.WriteEndDocument();
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message,"错误",MessageBoxButton.OK, MessageBoxImage.Error);
                Environment .Exit(-1);
            }
        }

        public void AddItem(string uri)
        {
            MediaListItemCtrl ctrl = new();
            MediaList.Add(new(uri, ctrl));
            MediaListStackPanel.Children.Add(ctrl);
        }

        private void PlayListItem_Event(object sender, ControlPlayEventHandleArgs e)
        {
            ParentWnd.Navigate(ParentWnd);
            ParentWnd.OpenUrl((string)e.Param);
        }

        private void AddItem(string uri,string videodatetime)
        {
            MediaListItemCtrl ctrl = new();
            MediaList.Add(new(uri,videodatetime, ctrl));
            MediaListStackPanel.Children.Add(ctrl);
        }

        public List<MediaListItem> MediaList;
        private MainWindow ParentWnd;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MediaListItemCtrl.MainWnd.GoBack();
        }
    }
}
