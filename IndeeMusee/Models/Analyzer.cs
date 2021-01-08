using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace IndeeMusee.Models
{
    internal class Analyzer
    {
        private bool _enable;               //enabled status
        private DispatcherTimer _t;         //timer that refreshes the display
        private float[] _fft;               //buffer for fft data
        private ProgressBar _l, _r;         //progressbars for left and right channel intensity
        private WASAPIPROC _process;        //callback function to obtain data
        private int _lastlevel;             //last output level
        private int _hanctr;                //last output level counter
        private List<byte> _spectrumdata;   //spectrum data buffer
        private Views.Spectrum _spectrum;         //spectrum dispay control
        private bool _initialized;          //initialized flag
        private int devindex;               //used device index
        private int _lines = 32;            // number of spectrum lines
        private int _deviceCount;
        private DispatcherTimer _t2;

        //ctor
        public Analyzer(ProgressBar left, ProgressBar right, Views.Spectrum spectrum)
        {
            _enable = true;
            _fft = new float[1024];
            _lastlevel = 0;
            _hanctr = 0;
            _t = new DispatcherTimer();
            _t.Tick += _t_Tick;
            _t.Interval = TimeSpan.FromMilliseconds(25); //40hz refresh rate
            _t.IsEnabled = false;
            _t2 = new DispatcherTimer();
            _t2.Interval = TimeSpan.FromSeconds(1);
            _t2.Tick += _t2_Tick;
            //_t2.Start();
            _l = left;
            _r = right;
            _l.Minimum = 0;
            _r.Minimum = 0;
            _r.Maximum = ushort.MaxValue;
            _l.Maximum = ushort.MaxValue;
            _process = new WASAPIPROC(Process);
            _spectrumdata = new List<byte>();
            _spectrum = spectrum;
            _initialized = false;
            NewInit();
        }

        private void _t2_Tick(object sender, EventArgs e)
        {
            int temp = 0;
            for (int i = 0; i < BassWasapi.BASS_WASAPI_GetDeviceCount(); i++)
            {
                var device = BassWasapi.BASS_WASAPI_GetDeviceInfo(i);
                if (device.IsEnabled && device.IsLoopback)
                {
                    temp++;
                }
            }
            if (temp != _deviceCount)
            {
                _hanctr = 0;
                _l.Value = 0;
                _r.Value = 0;
                _initialized = false;
                Free();
                NewInit();
            }
        }


        // initialization
        private void NewInit()
        {
            _t.Stop();
            _deviceCount = 0;
            bool result = false;
            List<int> DeviceActiveIndex = new List<int>();
            for (int i = 0; i < BassWasapi.BASS_WASAPI_GetDeviceCount(); i++)
            {
                var device = BassWasapi.BASS_WASAPI_GetDeviceInfo(i);
                if (device.IsEnabled && device.IsLoopback)
                {
                    DeviceActiveIndex.Add(i);
                    _deviceCount++;
                }
            }
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, false);
            result = Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            if (!result) throw new Exception("Init Error");
            if (_enable)
            {

                if (!_initialized)
                {
                    devindex = DeviceActiveIndex[DeviceActiveIndex.Count - 1];
                    result = BassWasapi.BASS_WASAPI_Init(devindex, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _process, IntPtr.Zero);

                    if (!result)
                    {
                        var error = Bass.BASS_ErrorGetCode();
                        MessageBox.Show(error.ToString());
                    }
                    else
                    {
                        _initialized = true;
                    }
                }
                BassWasapi.BASS_WASAPI_Start();
            }
            else BassWasapi.BASS_WASAPI_Stop(true);
            System.Threading.Thread.Sleep(500);
            _t.Start();
        }

        //timer 
        private void _t_Tick(object sender, EventArgs e)
        {
            int ret = BassWasapi.BASS_WASAPI_GetData(_fft, (int)BASSData.BASS_DATA_FFT2048); //get channel fft data

            if (ret < -1) return;
            int x, y;
            int b0 = 0;
            int hz = 255;

            //computes the spectrum data, the code is taken from a bass_wasapi sample.
            for (x = 0; x < _lines; x++)
            {
                float peak = 0;
                int b1 = (int)Math.Pow(2, x * 10.0 / (_lines - 1));
                if (b1 > 1023) b1 = 1023;
                if (b1 <= b0) b1 = b0 + 1;
                for (; b0 < b1; b0++)
                {
                    if (peak < _fft[1 + b0]) peak = _fft[1 + b0];
                }
                y = (int)(Math.Sqrt(peak) * 3 * hz - 4);
                if (y > hz) y = hz;
                if (y < 0) y = 0;
                _spectrumdata.Add((byte)y);
            }

            _spectrum.Set(_spectrumdata);

            _spectrumdata.Clear();


            int level = BassWasapi.BASS_WASAPI_GetLevel();
            _l.Value = Utils.LowWord32(level);
            _r.Value = Utils.HighWord32(level);
            if (level == _lastlevel && level != 0) _hanctr++;
            _lastlevel = level;

            //Required, because some programs hang the output. If the output hangs for a 75ms
            //this piece of code re initializes the output so it doesn't make a gliched sound for long.
            if (_hanctr > 3)
            {
                _hanctr = 0;
                _l.Value = 0;
                _r.Value = 0;
                _initialized = false;
                Free();
                NewInit();
            }
        }

        // WASAPI callback, required for continuous recording
        private int Process(IntPtr buffer, int length, IntPtr user)
        {
            return length;
        }

        //cleanup
        public void Free()
        {
            BassWasapi.BASS_WASAPI_Free();
            Bass.BASS_Free();
        }
    }
}
