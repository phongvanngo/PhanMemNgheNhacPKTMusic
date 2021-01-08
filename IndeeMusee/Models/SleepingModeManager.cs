using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace IndeeMusee.Models
{
    public static class SleepingModeManager
    {
        public static DispatcherTimer SleepTimer = new DispatcherTimer();

        public static int RemainingTime;
        public static void SetTimer(string time)
        {
            SleepTimer.Stop();

            RemainingTime = (int)TimeSpan.ParseExact(time, @"hh\:mm\:ss", null).TotalSeconds;
            SleepTimer.Interval = TimeSpan.FromSeconds(1);
            SleepTimer.Tick += SleepTimer_Tick;
            SleepTimer.Start();
        }

        private static void SleepTimer_Tick(object sender, EventArgs e)
        {
            if (RemainingTime == 0)
            {
                SleepTimer.Stop();
                NowPlayingSong.IsPlaying = false;
                GoodNightForm goodNightForm = new GoodNightForm();
                goodNightForm.ShowDialog();

            }
            else
            {
                RemainingTime--;

            }
        }
    }
}
