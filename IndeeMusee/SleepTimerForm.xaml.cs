using IndeeMusee.Models;
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
using System.Windows.Shapes;

namespace IndeeMusee
{
    /// <summary>
    /// Interaction logic for SleepTimerForm.xaml
    /// </summary>
    public partial class SleepTimerForm : Window
    {
        private DateTime currentDateTime = DateTime.Now;

        public static event ToggleFormDialogNotifyHandler ToggleForm;


        public DateTime CurrentDateTime
        {
            get
            {
                return currentDateTime;
            }
            set
            {
                currentDateTime = value;
            }
        }
        public SleepTimerForm()
        {
            InitializeComponent();
            ToggleForm();
            this.Closing += SleepTimerForm_Closing;
        }

        private void SleepTimerForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ToggleForm();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SleepingModeManager.SetTimer($@"{hour}:{min}:{second}");
            this.Close();

        }

        string hour = "00";
        int checkHourInput = 1;

        string second = "00";
        int checkSecondInput = 1;

        string min = "00";
        int checkMinInput = 1;
        private void TbSecond_KeyDown(object sender, KeyEventArgs e)
        {
            int ascii = KeyInterop.VirtualKeyFromKey(e.Key);

            if (ascii < 47 || ascii > 57)
            {

                e.Handled = true;
                return;
            }

            if (checkSecondInput == 1)
            {
                second = "00";
                checkSecondInput = 0;
            }
            else
            {
                checkSecondInput = 1;
            }


            if (Convert.ToInt32((second + (char)ascii).Substring(1, 2)) <= 60)
            {
                second = second + (char)ascii;
                second = second.Substring(1, 2);
                TbSecond.Text = second;
            }


            e.Handled = true;

        }

        private void TbMin_KeyDown(object sender, KeyEventArgs e)
        {
            int ascii = KeyInterop.VirtualKeyFromKey(e.Key);

            if (ascii < 47 || ascii > 57)
            {
                e.Handled = true;
                return;

            }

            if (checkMinInput == 1)
            {
                min = "00";
                checkMinInput = 0;
            }
            else
            {
                checkMinInput = 1;
            }


            if (Convert.ToInt32((min + (char)ascii).Substring(1, 2)) <= 60)
            {
                min = min + (char)ascii;
                min = min.Substring(1, 2);
                TbMin.Text = min;
            }


            e.Handled = true;
        }

        private void TbHour_KeyDown(object sender, KeyEventArgs e)
        {
            int ascii = KeyInterop.VirtualKeyFromKey(e.Key);



            if (ascii < 47 || ascii > 57)
            {
                e.Handled = true;
                return;

            }

            if (checkHourInput == 1)
            {
                hour = "00";
                checkHourInput = 0;
            }
            else
            {
                checkHourInput = 1;
            }


            if (Convert.ToInt32((hour + (char)ascii).Substring(1, 2)) <= 100)
            {
                hour = hour + (char)ascii;
                hour = hour.Substring(1, 2);
                TbHour.Text = hour;
            }


            e.Handled = true;
        }

        private void TbHour_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TbHour.Text.Length == 1) TbHour.Text = TbHour.Text + "0";

        }

        private void TbSecond_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TbSecond.Text.Length == 1) TbSecond.Text = TbSecond.Text + "0";
        }





        private void TbSecond_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TbSecond.SelectAll();
        }

        private void TbMin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TbMin.Text.Length == 1) TbMin.Text = TbMin.Text + "0";
        }


    }
}
