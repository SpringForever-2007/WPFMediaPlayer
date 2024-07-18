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

        }

        public void Save()
        {

        }

        public void AddItem(string uri)
        {
            __MediaList.Add(new(uri));
        }

        private List<MediaListItem> __MediaList;
    }
}
