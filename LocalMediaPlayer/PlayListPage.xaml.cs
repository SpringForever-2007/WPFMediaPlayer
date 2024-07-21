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
        public PlayListPage()
        {
            InitializeComponent();
            __MediaList = new();
        }

        public void Load()
        {
            try
            {
                using (XmlReader rd = XmlReader.Create("..\\..\\..\\PlayListData.xml"))
                {
                    rd.ReadStartElement("PlayList");
                    int num = int.Parse(rd.GetAttribute("Number"));
                    string fn, dt;
                    while(rd.Read())
                    {
                        if(rd.IsStartElement("ListItem"))
                        {
                            num--;
                            fn = rd.GetAttribute("FileName");
                            dt = rd.GetAttribute("VidepDateTime");
                            AddItem(fn, dt);
                        }
                    }
                    rd.ReadEndElement();
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

                using (XmlWriter wrt = XmlWriter.Create("..\\..\\..\\PlayListData.xml",settings))
                {
                    wrt.WriteStartDocument();
                    wrt.WriteStartElement("PlayList");
                    wrt.WriteAttributeString("Number", __MediaList.Count.ToString());
                    foreach(MediaListItem it in __MediaList)
                    {
                        wrt.WriteStartElement("ListItem");
                        wrt.WriteAttributeString("FileName", it.FileName);
                        wrt.WriteAttributeString("VideoDateTime", it.DateTime);
                        wrt.WriteEndElement();
                    }
                    wrt.WriteEndElement();
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
            __MediaList.Add(new(uri, ctrl));
            MediaListStackPanel.Children.Add(ctrl);
        }

        private void AddItem(string uri,string videodatetime)
        {
            MediaListItemCtrl ctrl = new();
            __MediaList.Add(new(uri,videodatetime, ctrl));
            MediaListStackPanel.Children.Add(ctrl);
        }

        private List<MediaListItem> __MediaList;
    }
}
