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
    /// MediaListItemCtrl.xaml 的交互逻辑
    /// </summary>
    public partial class MediaListItemCtrl : UserControl
    {
        public MediaListItemCtrl()
        {
            InitializeComponent();
        }

        public string FileName
        {
            get => (string)FileNameLabel.Content;
            set => FileNameLabel.Content = value;
        }

        public string VideoDateTimeStr
        {
            get => (string)VideoDateTimeLabel.Content;
            set => VideoDateTimeLabel.Content = value;
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Event?.Invoke(this, new(PlayEvent.OpenFromList, FileName));
        }

        public ControlPlayEvent? Event;
    }
}
