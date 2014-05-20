using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;

//Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
//Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

//This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

//You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
using WikiFunctions.API;

namespace AutoWikiBrowser.Plugins.Kingbotk.Components
{
    internal sealed partial class TimerStats
    {
        private PluginSettingsControl.Stats _stats;
        private TimeSpan _timeSpan;
        private DateTime _start;
        private int _numberOfEdits;
        private int _skipped;

        private Label _etaLabel;

        private int NumberOfEdits
        {
            get { return _numberOfEdits; }
            set
            {
                _numberOfEdits = value;
                EditsLabel.Text = _numberOfEdits.ToString(CultureInfo.InvariantCulture);
            }
        }

        internal void Init(AsyncApiEdit e, Label etaLabel, PluginSettingsControl.Stats stats)
        {
            if (!TimerEnabled)
            {
                ResetVars();
                _etaLabel = etaLabel;

                TimerEnabled = true;

                _stats = stats;
                _stats.SkipMisc += StatsSkipMisc;

                Timer1_Tick(null, null);
            }
        }

        internal void Reset()
        {
            ResetVars();
            TimerLabel.Text = "0:00";
            SpeedLabel.Text = "0";
            EditsLabel.Text = "0";
        }

        private void ResetVars()
        {
            NumberOfEdits = 0;
            _start = DateTime.Now;
            _skipped = 0;
        }

        internal void StopStats()
        {
            ResetVars();
            TimerEnabled = false;
        }

        private string ETA
        {
            get { return _etaLabel.Text.Replace("ETC: ", ""); }
            set { _etaLabel.Text = "ETC: " + value; }
        }

        private void CalculateETA(double secondsPerPage)
        {
            int count = PluginManager.AWBForm.ListMaker.Count;

            if (count == 0)
            {
                ETA = "Now";
            }
            else
            {
                DateTime etaDateTime = DateTime.Now.AddSeconds(secondsPerPage*count);

                if (etaDateTime.Date == DateTime.Now.Date)
                {
                    ETA = etaDateTime.ToString("HH:mm") + " today";
                }
                else if (DateTime.Now.AddDays(1).Date == etaDateTime.Date)
                {
                    ETA = etaDateTime.ToString("HH:mm") + " tomorrow";
                }
                else
                {
                    ETA = etaDateTime.ToString("HH:mm, ddd d MMM");
                }
            }
        }

        private bool TimerEnabled
        {
            get { return Timer1.Enabled; }
            set
            {
                if (_etaLabel != null)
                {
                    _etaLabel.Visible = value;
                }
                Timer1.Enabled = value;
            }
        }

        // Event handlers
        private readonly Regex _timerregexp = new Regex("\\..*");
        private int _updateETACount;

        private void Timer1_Tick(object sender, EventArgs e)
        {
            _updateETACount += 1;
            _timeSpan = DateTime.Now - _start;
            TimerLabel.Text = _timerregexp.Replace(_timeSpan.ToString(), "");
            double secondsPerPage = NumberOfEdits == 0
                ? _timeSpan.TotalSeconds
                : Math.Round(_timeSpan.TotalSeconds/NumberOfEdits, 2);

            if (double.IsInfinity(secondsPerPage))
            {
                SpeedLabel.Text = "0";
                ETA = "-";
                if (_updateETACount > 9)
                    _updateETACount = 0;
            }
            else
            {
                SpeedLabel.Text = secondsPerPage + " s/p";
                if (_updateETACount > 9 || ETA == "-")
                {
                    _updateETACount = 0;
                    if ((NumberOfEdits + _skipped) == 0)
                    {
                        CalculateETA(_timeSpan.TotalSeconds);
                    }
                    else
                    {
                        CalculateETA(_timeSpan.TotalSeconds/(NumberOfEdits + _skipped));
                    }
                }
            }
        }

        internal void IncrementSavedEdits(AsyncApiEdit sender, SaveInfo save)
        {
            IncrementSavedEdits();
        }

        internal void IncrementSavedEdits()
        {
            NumberOfEdits += 1;
        }

        private void StatsSkipMisc(int val)
        {
            _skipped += 1;
        }

        public TimerStats()
        {
            InitializeComponent();
        }
    }
}