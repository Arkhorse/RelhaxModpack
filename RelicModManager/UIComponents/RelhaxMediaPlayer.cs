﻿using System.Windows.Forms;
using NAudio.Wave;

namespace RelhaxModpack
{
    public partial class RelhaxMediaPlayer : UserControl
    {
        private Timer UITimer = new Timer();
        IWavePlayer waveOutDevice = new WaveOut();
        MediaFoundationReader audioFileReader2;
        public string MediaURL;        
        public string StopText
        {
            get
            {
                return StopButton.Text;
            }
            set
            {
                StopButton.Text = value;
            }
        }
        public string PlayPauseText
        {
            get
            {
                return PlayPause.Text;
            }
            set
            {
                PlayPause.Text = value;
            }
        }

        public RelhaxMediaPlayer()
        {
            InitializeComponent();
        }

        private void WaveOutDevice_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            Stop_Click(null, null);
        }

        private void UITimer_Tick(object sender, System.EventArgs e)
        {
            if (waveOutDevice.PlaybackState != PlaybackState.Playing)
                Stop_Click(null,null);
            if(Seekbar.Minimum <= audioFileReader2.CurrentTime.TotalMilliseconds && audioFileReader2.CurrentTime.TotalMilliseconds <= Seekbar.Maximum)
                Seekbar.Value = (int)audioFileReader2.CurrentTime.TotalMilliseconds;
        }

        private void Volume_Scroll(object sender, System.EventArgs e)
        {
            float volScroll = Volume.Value;
            waveOutDevice.Volume = volScroll / 10;
        }
        //to process a grab/drag
        private void Seekbar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            //pause
            waveOutDevice.Pause();
            UITimer.Stop();
            //becuase of the unusable first 10 pixels or so
            //subtract out the area we can't use for scrolling. it's always constant (or very close)
            double mouseX = e.X - 10;
            //get the total scroll bar usable length
            double scrollWidth = Seekbar.Size.Width - 23;
            //make sure it's a positive number (border at beginning of scroll bar)
            if (mouseX < 0)
                mouseX = 0;
            //border at end of scroll bar
            if (mouseX > scrollWidth)
                mouseX = scrollWidth;
            //get the percent of where the seekbar is, 0-1 form
            double seekPos = mouseX / scrollWidth;
            //set the seekbar UI value to the scrolled location
            double newPos = Seekbar.Maximum * seekPos;
            Seekbar.Value = (int)newPos;
            audioFileReader2.CurrentTime = new System.TimeSpan(0, 0, 0, 0, Seekbar.Value);
        }

        private void Stop_Click(object sender, System.EventArgs e)
        {
            waveOutDevice.Stop();
            UITimer.Stop();
            Seekbar.Value = 0;
            audioFileReader2.Position = 0;
        }

        private void PlayPause_Click(object sender, System.EventArgs e)
        {
            switch(waveOutDevice.PlaybackState)
            {
                case PlaybackState.Stopped:
                case PlaybackState.Paused:
                    waveOutDevice.Play();
                    UITimer.Start();
                    break;
                case PlaybackState.Playing:
                    waveOutDevice.Pause();
                    UITimer.Stop();
                    break;
            }
        }

        private void RelhaxMediaPlayer_Load(object sender, System.EventArgs e)
        {
            //init
            FileName.Text = "LOADING";
            Application.DoEvents();
            audioFileReader2 = new MediaFoundationReader(MediaURL);
            waveOutDevice.Init(audioFileReader2);
            waveOutDevice.Stop();
            //https://stackoverflow.com/questions/10371741/naudio-seeking-and-navigation-to-play-from-the-specified-position
            Seekbar.Maximum = (int)audioFileReader2.TotalTime.TotalMilliseconds;
            FileName.Text = System.IO.Path.GetFileName(MediaURL);
            //start volume at 50 percent
            Volume.Value = 5;
            waveOutDevice.Volume = 0.5f;
            //hook up events
            UITimer.Tick += UITimer_Tick;
            waveOutDevice.PlaybackStopped += WaveOutDevice_PlaybackStopped;
            //set timer interval update speed (msec)
            UITimer.Interval = 100;
            UITimer.Stop();
        }
    }
}
