using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace LocalMediaPlayer
{
    public enum PlayEvent
    {
        Play=0,
        Pause,
        PositionChanged,
        StartChangePosition,
        EndChangePosition,
        SoundChanged,
        MediaSpeedChanged,
        Edit,
        OpenFromList
    }

    public enum MediaSpeed
    {
        Speed0_25=25,
        Speed0_50=50,
        Speed0_75=70,
        Speed1_00=100,
        Speed1_50=150,
        Speed2_00=200
    }

    public class ControlPlayEventHandleArgs:EventArgs
    {
        public ControlPlayEventHandleArgs(PlayEvent e,object Param)
        {
            Event = e;
            this.Param = Param;
        }

        public PlayEvent Event { get; private set; }
        public object Param { get; private set; }
    }

    public delegate void ControlPlayEvent(object sender, ControlPlayEventHandleArgs e);

    /// <summary>
    /// PlayCtrl.xaml 的交互逻辑
    /// </summary>
    public partial class PlayCtrl : UserControl
    {
        public PlayCtrl()
        {
            InitializeComponent();
            __Timer = new();
            SetSpeed(MediaSpeed.Speed1_00);
            __Timer.Tick += On_TimeTixk;
        }

        private void On_TimeTixk(object? sender, EventArgs e)
        {
            Position += TimeSpan.FromSeconds(1);
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            IsPlaying = !__IsPlaying;
            PlayEvent en;
            if(__IsPlaying)
                en = PlayEvent.Play;
            else en = PlayEvent.Pause;
            Event?.Invoke(this, new ControlPlayEventHandleArgs(en, __IsPlaying));
        }

        public bool IsPlaying
        {
            get => __IsPlaying;
            set
            {
                __IsPlaying = value;
                if(__IsPlaying)
                {
                    PlayButton.Template = (ControlTemplate)Resources["roundbutton2paused"];
                    PlayButton.ToolTip = "暂停";
                    __Timer.Start();
                }
                else
                {
                    PlayButton.Template = (ControlTemplate)Resources["roundbutton2playing"];
                    PlayButton.ToolTip = "播放";
                    __Timer.Stop();
                }
            }
        }

        public TimeSpan Position
        {
            get => __Position;
            set
            {
                __Position = value;
                PositionLabel.Content=TimeSpanToString(__Position);
                PositionSlider.Value = __Position.TotalSeconds;
            }
        }

        public TimeSpan NaturalDuration
        {
            get => __NaturalDuration;
            set
            {
                __NaturalDuration = value;
                NaturalDurationLabel.Content=TimeSpanToString(__NaturalDuration);
                PositionSlider.Maximum = __NaturalDuration.TotalSeconds;
            }
        }

        public ControlPlayEvent? Event;
        private bool __IsPlaying;
        private TimeSpan __Position;
        private TimeSpan __NaturalDuration;
        private DispatcherTimer __Timer;
        private double __MediaSpeed;

        private static string TimeSpanToString(TimeSpan tm)
        {
            string str;
            str=string.Format("{0:D2}:{1:D2}:{2:D2}",
                tm.Hours,tm.Minutes, tm.Seconds);
            return str;
        }

        private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue - e.OldValue == 1)
            {
                //不做任何处理，防止循环更改
            }
            else
            {
                PositionLabel.Content = TimeSpanToString(TimeSpan.FromSeconds(e.NewValue));
                __Position = TimeSpan.FromSeconds(e.NewValue);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackOrFront(true);
        }

        private void FrontButton_Click(object sender, RoutedEventArgs e)
        {
            BackOrFront(false);
        }

        private void PositionSlider_GotFocus(object sender, RoutedEventArgs e)
        {
            IsPlaying = false;
            Event?.Invoke(this, new(PlayEvent.Pause, IsPlaying));
        }

        private void PositionSlider_LostFocus(object sender, RoutedEventArgs e)
        {
            Event?.Invoke(this, new(PlayEvent.PositionChanged, __Position));
        }

        private void SoundSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Event?.Invoke(this, new(PlayEvent.SoundChanged, e.NewValue));
        }

        private void SetSpeed(MediaSpeed speed)
        {
            __MediaSpeed = (double)speed / 100;
            __Timer.Interval=TimeSpan.FromSeconds(1/__MediaSpeed);
            Event?.Invoke(this, new(PlayEvent.MediaSpeedChanged, __MediaSpeed));
        }

        private void Speed_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var s=(MenuItem)sender;
            SetSpeed((MediaSpeed)(double.Parse((string)s.Header)*100));
        }

        private void OpenEditorMeniItem_Click(object sender, RoutedEventArgs e)
        {
            Event?.Invoke(this, new(PlayEvent.Edit, this));
        }

        public void BackOrFront(bool IsBack)
        {
            if(IsBack)
            {
                if (__Position.TotalSeconds < 30)
                    Position = TimeSpan.Zero;
                else Position = __Position - new TimeSpan(0, 0, 30);
                Event?.Invoke(this, new(PlayEvent.PositionChanged, Position));
            }
            else
            {
                if (__Position.TotalSeconds + 30 > __NaturalDuration.TotalSeconds)
                    Position = __NaturalDuration;
                else Position = __Position + new TimeSpan(0, 0, 30);
                Event?.Invoke(this, new(PlayEvent.PositionChanged, Position));
            }
        }

        public void PlayOrPause()
        {
            IsPlaying = !IsPlaying;
            PlayEvent en;
            if (__IsPlaying)
                en = PlayEvent.Play;
            else en = PlayEvent.Pause;
            Event?.Invoke(this, new ControlPlayEventHandleArgs(en, __IsPlaying));
        }
    }
}
