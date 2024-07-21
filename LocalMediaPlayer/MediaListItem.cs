using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalMediaPlayer
{
    public class MediaListItem
    {
        public MediaListItem(string fileName, MediaListItemCtrl ctrl)
        {
            __Ctrl = ctrl;
            FileName = fileName;
        }

        public MediaListItem(string fileName,string videoDateTime,MediaListItemCtrl ctrl)
        {
            __Ctrl=ctrl;
            FileName = fileName;
            __VideoDateTime = videoDateTime;
            __Ctrl.VideoDateTimeLabel.Content = videoDateTime;
        }

        public string FileName
        {
            get => __FileName;
            set
            {
                __FileName = value;
                __VideoDateTime = GetNowDateTimeStr();
                __Ctrl.FileName = __FileName;
                __Ctrl.VideoDateTimeStr = __VideoDateTime;
            }
        }

        public string DateTime
        {
            get => __VideoDateTime;
        }

        public bool IsCheck
        {
            get => (bool)__Ctrl.ChooseCheckBox.IsChecked;
        }

        private string __FileName = "";
        private string __VideoDateTime = "";
        private MediaListItemCtrl __Ctrl;
        private string GetNowDateTimeStr()
        {
            DateTime dt = new();
            string str;
            str = string.Format("{0:D4}/{1:D2}/{2:D2} {3:D2}:{4:D2}:{5:D2}",
                dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
            return str;
        }
    }
}
