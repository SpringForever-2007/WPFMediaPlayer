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
        public static Style DefaultButtonStyle { get => (Style)Current.Resources["DefaultButtonStyle"]; }
        public static Style SingleLineTextBoxStyle { get => (Style)Current.Resources["SingleLineTextBoxStyle"]; }
        public static Storyboard SlideInAnimation { get => (Storyboard)Current.Resources["SlideInAnimation"]; }
        public static Storyboard SlideOutAnimation { get => (Storyboard)Current.Resources["SlideOutAnimation"]; }
        public static Brush MainWindowBackground { get => (Brush)Current.Resources["MainWindowBackground"]; }
    }
}
