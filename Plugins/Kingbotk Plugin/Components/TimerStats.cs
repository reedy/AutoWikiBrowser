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
        private PluginSettingsControl.Stats mStats;
        private TimeSpan TimeSpan;
        private DateTime Start;
        private int mNumberOfEdits;
        private int mSkipped;

        private Label mETALabel;

        private int NumberOfEdits
        {
            get { return mNumberOfEdits; }
            set
            {
                mNumberOfEdits = value;
                EditsLabel.Text = mNumberOfEdits.ToString(CultureInfo.InvariantCulture);
            }
        }

        internal void Init(AsyncApiEdit e, Label ETALabel, PluginSettingsControl.Stats Stats)
        {
            if (!TimerEnabled)
            {
                ResetVars();
                mETALabel = ETALabel;

                TimerEnabled = true;

                mStats = Stats;
                mStats.SkipMisc += mStats_SkipMisc;

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
            Start = DateTime.Now;
            mSkipped = 0;
        }

        internal void StopStats()
        {
            ResetVars();
            TimerEnabled = false;
        }

        private string ETA
        {
            get { return mETALabel.Text.Replace("ETC: ", ""); }
            set { mETALabel.Text = "ETC: " + value; }
        }

        private void CalculateETA(double SecondsPerPage)
        {
            int Count = PluginManager.AWBForm.ListMaker.Count;

            if (Count == 0)
            {
                ETA = "Now";
            }
            else
            {
                DateTime ETADateTime = DateTime.Now.AddSeconds(SecondsPerPage*Count);

                if (ETADateTime.Date == DateTime.Now.Date)
                {
                    ETA = ETADateTime.ToString("HH:mm") + " today";
                }
                else if (DateTime.Now.AddDays(1).Date == ETADateTime.Date)
                {
                    ETA = ETADateTime.ToString("HH:mm") + " tomorrow";
                }
                else
                {
                    ETA = ETADateTime.ToString("HH:mm, ddd d MMM");
                }
            }
        }

        private bool TimerEnabled
        {
            get { return Timer1.Enabled; }
            set
            {
                if (mETALabel != null)
                {
                    mETALabel.Visible = value;
                }
                Timer1.Enabled = value;
            }
        }

        // Event handlers
        private readonly Regex timerregexp = new Regex("\\..*");
        private int UpdateETACount;

        private void Timer1_Tick(object sender, EventArgs e)
        {
            UpdateETACount += 1;
            TimeSpan = DateTime.Now - Start;
            TimerLabel.Text = timerregexp.Replace(TimeSpan.ToString(), "");
            double secondsPerPage = NumberOfEdits == 0
                ? TimeSpan.TotalSeconds
                : Math.Round(TimeSpan.TotalSeconds/NumberOfEdits, 2);

            if (double.IsInfinity(secondsPerPage))
            {
                SpeedLabel.Text = "0";
                ETA = "-";
                if (UpdateETACount > 9)
                    UpdateETACount = 0;
            }
            else
            {
                SpeedLabel.Text = secondsPerPage + " s/p";
                if (UpdateETACount > 9 || ETA == "-")
                {
                    UpdateETACount = 0;
                    if ((NumberOfEdits + mSkipped) == 0)
                    {
                        CalculateETA(TimeSpan.TotalSeconds);
                    }
                    else
                    {
                        CalculateETA(TimeSpan.TotalSeconds/(NumberOfEdits + mSkipped));
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

        private void mStats_SkipMisc(int val)
        {
            mSkipped += 1;
        }

        public TimerStats()
        {
            InitializeComponent();
        }
    }
}