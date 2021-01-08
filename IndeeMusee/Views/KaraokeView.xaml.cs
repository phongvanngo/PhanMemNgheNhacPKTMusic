using IndeeMusee.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IndeeMusee.Views
{
    /// <summary>
    /// Interaction logic for KaraokeView.xaml
    /// </summary>
    public partial class KaraokeView : UserControl
    {
        int nextTextBlock = 1;

        DoubleAnimation animationLyric1 = new DoubleAnimation();
        DoubleAnimation animationLyric2 = new DoubleAnimation();
        Storyboard storyboard_lyric1;
        Storyboard storyboard_lyric2;
        double width_tb_1 = 200;
        double width_tb_2 = 200;
        TimeSpan duration;
        public KaraokeView()
        {
            InitializeComponent();

            NowPlayingSong.ChangeLyric += NowPlayingSong_ChangeLyric;
            NowPlayingSong.SetupNewLyric += NowPlayingSong_SetupNewLyric;



            Storyboard.SetTargetName(animationLyric1, "tb_lyric1_run");
            Storyboard.SetTargetName(animationLyric2, "tb_lyric2_run");

            Storyboard.SetTargetProperty(animationLyric1,
                new PropertyPath(TextBlock.WidthProperty));

            Storyboard.SetTargetProperty(animationLyric2,
                new PropertyPath(TextBlock.WidthProperty));

            Storyboard storyboard_lyric1 = new Storyboard();
            Storyboard storyboard_lyric2 = new Storyboard();

        }

        private void NowPlayingSong_SetupNewLyric()
        {
            nextTextBlock = 1;
            tb_lyric1_run.Text = "";
            tb_lyric1_static.Text = "";
            tb_lyric2_static.Text = "";
            tb_lyric2_run.Text = "";

        }

        private double MeasureString(string candidate, TextBlock textBlock)
        {
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                textBlock.FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                1);

            return formattedText.Width;
        }

        private void NowPlayingSong_ChangeLyric(LyricModel lyric, string nextLyric)
        {
            //tb_lyric2_static.Text = tb_lyric1_static.ActualWidth.ToString();
            try
            {


                switch (nextTextBlock)
                {
                    case 1:
                        tb_lyric1_static.Text = lyric.Content;
                        tb_lyric1_run.Text = lyric.Content;
                        nextTextBlock = 2;

                        //set up lyric 2;
                        if (nextLyric != "")
                        {
                            tb_lyric2_static.Text = nextLyric;
                            tb_lyric2_run.Text = "";
                            tb_lyric1_run.Width = 0;
                        }

                        animationLyric1.From = 0;
                        //animationLyric1.To = lyric.Content.Length * 11;
                        animationLyric1.To = MeasureString(lyric.Content, tb_lyric1_static);

                        duration = TimeSpan.FromMilliseconds(lyric.TimeEnd - lyric.TimeStart);
                        if (duration.TotalMilliseconds <= 0) duration = TimeSpan.FromMilliseconds(1);
                        animationLyric1.Duration = new Duration(duration);
                        storyboard_lyric1 = new Storyboard();
                        storyboard_lyric1.Children.Add(animationLyric1);
                        storyboard_lyric1.Begin(this);

                        break;
                    case 2:
                        tb_lyric2_static.Text = lyric.Content;
                        tb_lyric2_run.Text = lyric.Content;
                        nextTextBlock = 1;

                        animationLyric2.From = 0;
                        //animationLyric2.To = lyric.Content.Length * 10;
                        animationLyric2.To = MeasureString(lyric.Content, tb_lyric2_static);

                        duration = TimeSpan.FromMilliseconds(lyric.TimeEnd - lyric.TimeStart);
                        if (duration.TotalMilliseconds <= 0) duration = TimeSpan.FromMilliseconds(1);
                        animationLyric2.Duration = new Duration(duration);
                        storyboard_lyric2 = new Storyboard();
                        storyboard_lyric2.Children.Add(animationLyric2);
                        storyboard_lyric2.Begin(this);

                        break;

                    default:
                        break;
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
