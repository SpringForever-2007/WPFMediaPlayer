using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LocalMediaPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow mainwin = new();
            if(e.Args.Length > 0)
            {
                if(File.Exists(e.Args[0]))
                    mainwin.OpenUrl(e.Args[0]);
            }
            mainwin.Show();
            base.OnStartup(e);
        }

        public static Style DefaultButtonStyle { get => (Style)Current.Resources["DefaultButtonStyle"]; }
        public static Style SingleLineTextBoxStyle { get => (Style)Current.Resources["SingleLineTextBoxStyle"]; }
        public static Storyboard SlideInAnimation { get => (Storyboard)Current.Resources["SlideInAnimation"]; }
        public static Storyboard SlideOutAnimation { get => (Storyboard)Current.Resources["SlideOutAnimation"]; }
        public static Brush MainWindowBackground { get => (Brush)Current.Resources["MainWindowBackground"]; }
    }
}
