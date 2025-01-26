using System.Windows;
using System.Windows.Controls;
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
            using (XmlReader rd = XmlReader.Create(".\\AppData.xml"))
            {
                rd.ReadStartElement("PlayList");
                string fn, dt;
                while (rd.Read())
                {
                    if (rd.IsStartElement("ListItem"))
                    {
                        fn = rd.GetAttribute(0);
                        dt = rd.GetAttribute(1);
                        AddItem(fn, dt);
                    }
                }
            }
        }

        public void Save()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("\t");
            settings.NewLineChars = ("\n");
            settings.NewLineHandling = NewLineHandling.Replace;

            using (XmlWriter wrt = XmlWriter.Create(".\\AppData.xml", settings))
            {
                wrt.WriteStartDocument();
                wrt.WriteStartElement("PlayList");
                foreach (MediaListItem it in MediaList)
                {
                    wrt.WriteStartElement("ListItem");
                    wrt.WriteAttributeString("FileName", it.FileName);
                    wrt.WriteAttributeString("VideoDateTime", it.DateTime);
                    wrt.WriteEndElement();
                }
                wrt.WriteEndDocument();
            }
        }

        public void AddItem(string uri)
        {
            MediaListItemCtrl ctrl = new();
            MediaList.Add(new(uri, ctrl));
            MediaListStackPanel.Children.Add(ctrl);
            ItemCountLabel.Content = $"{MediaListStackPanel.Children.Count}个项目";
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
            ItemCountLabel.Content = $"{MediaListStackPanel.Children.Count}个项目";
        }

        public List<MediaListItem> MediaList;
        private MainWindow ParentWnd;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MediaListItemCtrl.MainWnd.GoBack();
        }

        private void OperatorComboBox_Click(object sender, RoutedEventArgs e)
        {
            for(int i=0;i<MediaList.Count;)
            {
                if (MediaList[i].IsCheck)
                {
                    MediaListStackPanel.Children.Remove(MediaList[i].Ctrl);
                    MediaList.Remove(MediaList[i]);
                }
                else i++;
            }
            ItemCountLabel.Content = $"{MediaListStackPanel.Children.Count}个项目";
        }
    }
}
