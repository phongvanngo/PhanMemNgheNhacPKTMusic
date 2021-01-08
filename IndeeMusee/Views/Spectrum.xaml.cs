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

namespace IndeeMusee.Views
{
    /// <summary>
    /// Interaction logic for Spectrum.xaml
    /// </summary>
    public partial class Spectrum : UserControl
    {
        public Spectrum()
        {
            InitializeComponent();
        }
        public void Set(List<byte> data)
        {
            if (data.Count < 16) return;
            Bar01.Value = data[0];
            Bar02.Value = data[0];
            Bar03.Value = data[1];
            Bar04.Value = data[1];
            Bar05.Value = data[2];
            Bar06.Value = data[2];
            Bar07.Value = data[3];
            Bar08.Value = data[3];
            Bar09.Value = data[4];
            Bar10.Value = data[4];
            Bar11.Value = data[5];
            Bar12.Value = data[5];
            Bar13.Value = data[6];
            Bar14.Value = data[6];
            Bar15.Value = data[7];
            Bar16.Value = data[7];
            Bar17.Value = data[8];
            Bar18.Value = data[8];
            Bar19.Value = data[9];
            Bar20.Value = data[9];
            Bar21.Value = data[10];
            Bar22.Value = data[10];
            Bar23.Value = data[11];
            Bar24.Value = data[11];
            Bar25.Value = data[12];
            Bar26.Value = data[12];
            Bar27.Value = data[13];
            Bar28.Value = data[13];
            Bar29.Value = data[14];
            Bar30.Value = data[14];
            Bar31.Value = data[15];
            Bar32.Value = data[15];

        }
    }
}
