using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace LocalMediaPlayer
{
    class UrlDialog:Window
    {
        public UrlDialog()
        {

        }

        private void InitUI()
        {

        }

        private TextBox UrlTextBox;
    }

    class UriDialogViewModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string URLString
        {
            get => _url;
            set
            {
                _url= value;
                PropertyChanged?.Invoke(_url, new("URLString"));
            }
        }

        private string _url;
    }
}
