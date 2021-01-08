using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace IndeeMusee
{
    /// <summary>
    /// Interaction logic for FormIntro.xaml
    /// </summary>
    public partial class FormIntro : Window
    {
        public FormIntro()
        {
            InitializeComponent();

        }

        DispatcherTimer timer = new DispatcherTimer();
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(3.6);
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            this.DialogResult = true;
            this.Close();
        }
    }
}
