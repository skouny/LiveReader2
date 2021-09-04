using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveReader2
{
    public partial class WebReaderControl : UserControl
    {
        #region Init
        private SportName _Sport;
        private SourceName _Source;
        private WebReaderBase _Reader;
        private WebReader _Reader2;
        public WebReaderControl()
        {
            InitializeComponent();
            this.ControlInit();
        }
        #endregion
        #region Control Events
        private void checkBoxAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAutoStart.Checked && (_Reader == null || !_Reader.Created))
            {
                this.Start();
            }
        }
        private void buttonOpenForm_Click(object sender, EventArgs e)
        {
            this.Open();
        }
        private void buttonStart_Click(object sender, EventArgs e)
        {
            this.Start();
        }
        private void buttonStop_Click(object sender, EventArgs e)
        {
            this.Stop();
        }
        private void buttonRestart_Click(object sender, EventArgs e)
        {
            this.Restart();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (this.IsRunning && numericUpDownAutoRestart.Value > 0)
            {
                if (DateTime.Compare(DateTime.Now, this.Started.AddMinutes((double)numericUpDownAutoRestart.Value)) > 0)
                {
                    this.Restart();
                }
            }
            // Auto start flashscore at any case
        }
        private void buttonClearDB_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this.ClearDB());
        }
        #endregion
        #region Methods
        private void ControlInit()
        {
            buttonOpenForm.Text = string.Format("{0} - {1}", Source, Sport);
            buttonOpenForm.Enabled = false;
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
            switch (Sport)
            {
                case SportName.Football:
                    switch (Source)
                    {
                        case SourceName.Opap:
                            _Reader = new OpapFootball();
                            break;
                        case SourceName.NowGoal:
                            _Reader = new NowGoalFootball();
                            break;
                        case SourceName.FlashScore:
                            _Reader2 = new WebReader();
                            break;
                        case SourceName.Futbol24:
                            _Reader = new Futbol24Football();
                            break;
                    }
                    break;
                case SportName.Basket:
                    switch (Source)
                    {
                        case SourceName.Opap:
                            _Reader = new OpapBasket();
                            break;
                        case SourceName.NowGoal:
                            _Reader = new NowGoalBasket();
                            break;
                    }
                    break;
            }
        }
        public void Open()
        {
            if (_Reader != null) _Reader.Show();
            if (_Reader2 != null) _Reader2.Show();
        }
        public void Start()
        {
            this.ControlInit();
            if (_Reader != null) _Reader.Show();
            if (_Reader2 != null) _Reader2.Show();
            this.Started = DateTime.Now;
            buttonOpenForm.Enabled = true;
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
        }
        public void Stop()
        {
            if (_Reader != null)
            {
                _Reader.Stop();
                _Reader.Dispose();
            }
            if (_Reader2 != null)
            {
                //_Reader2.Stop();
                _Reader2.Dispose();
            }
            buttonOpenForm.Enabled = false;
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
        }
        public void Restart()
        {
            buttonRestart.Enabled = false;
            this.Stop();
            this.Start();
            buttonRestart.Enabled = true;
        }
        public string ClearDB()
        {
            return WebDB.table_live_mix_delete(this.Sport, this.Source);
        }
        #endregion
        #region Public Properties
        public SportName Sport { get { return _Sport; } set { _Sport = value; ControlInit(); } }
        public SourceName Source { get { return _Source; } set { _Source = value; ControlInit(); } }
        public bool AutoStart
        {
            get
            {
                return checkBoxAutoStart.Checked;
            }
            set
            {
                checkBoxAutoStart.Checked = value;
            }
        }
        public bool IsRunning
        {
            get
            {
                if (_Reader != null) return _Reader.Created;
                if (_Reader2 != null) return _Reader2.Created;
                return false;
            }
        }
        public DateTime Started { get; set; }
        public decimal AutoRestart { get { return numericUpDownAutoRestart.Value; } set { numericUpDownAutoRestart.Value = value; } }
        #endregion
    }
}
